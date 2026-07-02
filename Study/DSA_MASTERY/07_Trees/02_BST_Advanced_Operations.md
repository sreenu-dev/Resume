# BST Advanced Operations — Mastery Guide

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** BST properties, inorder traversal, augmented trees
> **Core Theme:** Exploiting BST's sorted inorder property to build O(log N)
> operations and solve structural transformation problems.

---

## 1. BST Validation — Correct Approach with Bounds (LeetCode 98)

**The naive mistake:** Checking only `node.left.val < node.val < node.right.val`
fails for cases like:

```
    5
   / \
  1   4
     / \
    3   6
```
Node 4 has `4 < 5` (valid locally) but its subtree is in the wrong range.

**Correct approach:** Pass valid range (min_val, max_val) down the tree.

```python
def isValidBST(root: TreeNode) -> bool:
    def validate(node, min_val, max_val):
        if not node:
            return True
        if not (min_val < node.val < max_val):
            return False
        return (validate(node.left, min_val, node.val) and
                validate(node.right, node.val, max_val))

    return validate(root, float('-inf'), float('inf'))
```
**Time:** O(N) | **Space:** O(H)

**Alternative — inorder should be strictly increasing:**

```python
def isValidBSTInorder(root: TreeNode) -> bool:
    prev = [float('-inf')]

    def inorder(node):
        if not node:
            return True
        if not inorder(node.left):
            return False
        if node.val <= prev[0]:
            return False
        prev[0] = node.val
        return inorder(node.right)

    return inorder(root)
```
**Time:** O(N) | **Space:** O(H)

---

## 2. BST Iterator — O(H) Space, O(1) Amortized (LeetCode 173)

```python
class BSTIterator:
    def __init__(self, root: TreeNode):
        self.stack = []
        self._push_left(root)

    def _push_left(self, node):
        while node:
            self.stack.append(node)
            node = node.left

    def next(self) -> int:
        node = self.stack.pop()
        if node.right:
            self._push_left(node.right)
        return node.val

    def hasNext(self) -> bool:
        return bool(self.stack)
```
**Time:** O(H) per `next()` worst case; O(1) amortized over N calls
**Space:** O(H)

**Amortized analysis:** Each node is pushed and popped exactly once. Total operations = 2N → O(1) amortized per call.

---

## 3. BST to Sorted Doubly Linked List (LeetCode 426)

```python
def treeToDoublyList(root: TreeNode) -> TreeNode:
    if not root:
        return None

    head = prev = None

    def inorder(node):
        nonlocal head, prev
        if not node:
            return
        inorder(node.left)

        if prev:
            prev.right = node
            node.left = prev
        else:
            head = node

        prev = node
        inorder(node.right)

    inorder(root)

    head.left = prev
    prev.right = head

    return head
```
**Time:** O(N) | **Space:** O(H)

---

## 4. Kth Smallest in BST (LeetCode 230)

### Simple Inorder O(K) Solution

```python
def kthSmallest(root: TreeNode, k: int) -> int:
    def inorder(node):
        if not node:
            return
        yield from inorder(node.left)
        yield node.val
        yield from inorder(node.right)

    for i, val in enumerate(inorder(root), 1):
        if i == k:
            return val
```
**Time:** O(H + K) | **Space:** O(H)

### O(H) with Augmented BST

```python
def kthSmallestAugmented(root: TreeNode, k: int) -> int:
    """Assumes root.left has .size pre-computed."""
    while root:
        left_size = root.left.size if root.left else 0
        if k == left_size + 1:
            return root.val
        elif k <= left_size:
            root = root.left
        else:
            k -= left_size + 1
            root = root.right
```
**Time:** O(H) | **Space:** O(1)

---

## 5. Recover BST — O(1) Space Morris (LeetCode 99)

**Two nodes in BST are swapped. Recover without changing structure.**

```python
def recoverTree(root: TreeNode) -> None:
    first = second = prev = None
    curr = root

    while curr:
        if not curr.left:
            if prev and prev.val > curr.val:
                if not first:
                    first = prev
                second = curr
            prev = curr
            curr = curr.right
        else:
            pred = curr.left
            while pred.right and pred.right is not curr:
                pred = pred.right

            if pred.right is None:
                pred.right = curr
                curr = curr.left
            else:
                pred.right = None
                if prev and prev.val > curr.val:
                    if not first:
                        first = prev
                    second = curr
                prev = curr
                curr = curr.right

    first.val, second.val = second.val, first.val
```
**Time:** O(N) | **Space:** O(1)

**Two-violation logic:**
- Adjacent swap: one violation (first=prev, second=curr)
- Non-adjacent swap: two violations (first set at first violation's prev, second updated at second violation's curr)

---

## 6. Count BST Nodes in Range (LeetCode 938)

```python
def rangeSumBST(root: TreeNode, low: int, high: int) -> int:
    if not root:
        return 0
    if root.val < low:
        return rangeSumBST(root.right, low, high)
    if root.val > high:
        return rangeSumBST(root.left, low, high)
    return (root.val +
            rangeSumBST(root.left, low, high) +
            rangeSumBST(root.right, low, high))
```
**Time:** O(N) worst case, O(log N + K) for balanced BST
**Space:** O(H)

---

## 7. Convert Sorted Array/List to BST

### From Sorted Array (LeetCode 108)

```python
def sortedArrayToBST(nums: list[int]) -> TreeNode:
    def build(lo, hi):
        if lo > hi:
            return None
        mid = (lo + hi) // 2
        node = TreeNode(nums[mid])
        node.left = build(lo, mid - 1)
        node.right = build(mid + 1, hi)
        return node
    return build(0, len(nums) - 1)
```
**Time:** O(N) | **Space:** O(log N)

### From Sorted Linked List (LeetCode 109)

```python
def sortedListToBST(head: ListNode) -> TreeNode:
    def find_middle(lo, hi):
        slow = fast = lo
        while fast is not hi and fast.next is not hi:
            slow = slow.next
            fast = fast.next.next
        return slow

    def build(lo, hi):
        if lo == hi:
            return None
        mid = find_middle(lo, hi)
        node = TreeNode(mid.val)
        node.left = build(lo, mid)
        node.right = build(mid.next, hi)
        return node

    return build(head, None)
```
**Time:** O(N log N) | **Space:** O(log N)

---

## 8. Balance a BST (LeetCode 1382)

```python
def balanceBST(root: TreeNode) -> TreeNode:
    sorted_vals = []

    def inorder(node):
        if not node: return
        inorder(node.left)
        sorted_vals.append(node.val)
        inorder(node.right)

    inorder(root)

    def build(lo, hi):
        if lo > hi: return None
        mid = (lo + hi) // 2
        node = TreeNode(sorted_vals[mid])
        node.left = build(lo, mid - 1)
        node.right = build(mid + 1, hi)
        return node

    return build(0, len(sorted_vals) - 1)
```
**Time:** O(N) | **Space:** O(N)

---

## 9. Inorder Successor & Predecessor in BST

```python
def inorderSuccessor(root: TreeNode, p: TreeNode) -> TreeNode:
    successor = None
    while root:
        if p.val < root.val:
            successor = root
            root = root.left
        else:
            root = root.right
    return successor

def inorderPredecessor(root: TreeNode, p: TreeNode) -> TreeNode:
    predecessor = None
    while root:
        if p.val > root.val:
            predecessor = root
            root = root.right
        else:
            root = root.left
    return predecessor
```
**Time:** O(H) | **Space:** O(1)

---

## 10. Delete Node in BST (LeetCode 450)

```python
def deleteNode(root: TreeNode, key: int) -> TreeNode:
    if not root:
        return None

    if key < root.val:
        root.left = deleteNode(root.left, key)
    elif key > root.val:
        root.right = deleteNode(root.right, key)
    else:
        if not root.left:
            return root.right
        if not root.right:
            return root.left

        # Find inorder successor (min of right subtree)
        successor = root.right
        while successor.left:
            successor = successor.left

        root.val = successor.val
        root.right = deleteNode(root.right, successor.val)

    return root
```
**Time:** O(H) | **Space:** O(H)

---

## 11. Unique BSTs — Catalan Number + DP (LeetCode 96)

```python
def numTrees(n: int) -> int:
    dp = [0] * (n + 1)
    dp[0] = dp[1] = 1

    for nodes in range(2, n + 1):
        for root in range(1, nodes + 1):
            dp[nodes] += dp[root - 1] * dp[nodes - root]

    return dp[n]
```
**Time:** O(N²) | **Space:** O(N)

**Mathematical fact:** `dp[n]` = nth Catalan number = C(2n, n) / (n + 1)

### Generate All Unique BSTs (LeetCode 95)

```python
def generateTrees(n: int) -> list[TreeNode]:
    if n == 0:
        return []

    def generate(lo, hi):
        if lo > hi:
            return [None]
        trees = []
        for root_val in range(lo, hi + 1):
            for left in generate(lo, root_val - 1):
                for right in generate(root_val + 1, hi):
                    node = TreeNode(root_val)
                    node.left = left
                    node.right = right
                    trees.append(node)
        return trees

    return generate(1, n)
```
**Time:** O(Catalan(N) × N) | **Space:** O(Catalan(N) × N)

---

## BST Operations Summary

| Operation | Naive | Optimal | Technique |
|---|---|---|---|
| Validate | O(N²) | O(N) | Pass bounds down recursion |
| Iterator | O(N) space | O(H) space | Controlled inorder via stack |
| Kth smallest | O(N) | O(H) augmented | Size-augmented BST |
| Recover swapped | O(H) space | O(1) space | Morris traversal |
| Range sum | O(N) | O(log N + K) | BST pruning |
| Delete | O(H) | O(H) | Successor replacement |

## Interview Tips

1. **BST validation**: Always use bounds. The "pass min/max bounds" is the only correct O(N) approach.
2. **Iterator**: The controlled inorder (push left spine lazily) is a common FAANG question.
3. **Recover BST**: Adjacent swaps produce one violation, non-adjacent produce two — handle both cases.
4. **Catalan numbers**: Know that the number of unique BSTs with N nodes is the Nth Catalan number ≈ 4^N / (N^1.5 × sqrt(π)).

# Advanced Tree Traversals — Morris, Iterative, Path Problems

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Recursion, tree pointers, stack-based DFS
> **Core Theme:** Achieving O(1)-space traversals and solving complex tree
> structural problems through invariant-driven algorithms.

---

## 1. Morris Traversal — O(1) Space Inorder

### Threading Mechanism

Morris traversal creates temporary "threads" (right pointers) from a node's
**inorder predecessor** back to the node itself.

**Algorithm:**
1. If no left child: visit current, move right
2. If left child exists: find inorder predecessor (rightmost in left subtree)
   - If predecessor.right is None: thread it to current, go left
   - If predecessor.right is current: unthread it, visit current, go right

```python
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def morrisInorder(root: TreeNode) -> list[int]:
    result = []
    curr = root

    while curr:
        if not curr.left:
            result.append(curr.val)
            curr = curr.right
        else:
            pred = curr.left
            while pred.right and pred.right is not curr:
                pred = pred.right

            if pred.right is None:
                pred.right = curr      # Thread
                curr = curr.left
            else:
                pred.right = None      # Unthread
                result.append(curr.val)
                curr = curr.right

    return result
```
**Time:** O(N) — each edge traversed at most twice
**Space:** O(1)

### Morris Preorder

```python
def morrisPreorder(root: TreeNode) -> list[int]:
    result = []
    curr = root

    while curr:
        if not curr.left:
            result.append(curr.val)
            curr = curr.right
        else:
            pred = curr.left
            while pred.right and pred.right is not curr:
                pred = pred.right

            if pred.right is None:
                result.append(curr.val)   # Visit BEFORE going left
                pred.right = curr
                curr = curr.left
            else:
                pred.right = None
                curr = curr.right

    return result
```
**Time:** O(N) | **Space:** O(1)

---

## 2. Iterative Postorder — Single Stack

```python
def postorderTraversal(root: TreeNode) -> list[int]:
    if not root:
        return []
    result = []
    stack = [root]

    while stack:
        node = stack.pop()
        result.append(node.val)
        if node.left:
            stack.append(node.left)
        if node.right:
            stack.append(node.right)

    return result[::-1]
```
**Time:** O(N) | **Space:** O(N)

**True single-pass (no reversal):**
```python
def postorderSinglePass(root: TreeNode) -> list[int]:
    result = []
    stack = []
    prev = None
    curr = root

    while curr or stack:
        while curr:
            stack.append(curr)
            curr = curr.left

        curr = stack[-1]

        if not curr.right or curr.right is prev:
            result.append(curr.val)
            stack.pop()
            prev = curr
            curr = None
        else:
            curr = curr.right

    return result
```
**Time:** O(N) | **Space:** O(H)

---

## 3. Vertical Order Traversal (LeetCode 987)

```python
from collections import defaultdict, deque

def verticalTraversal(root: TreeNode) -> list[list[int]]:
    if not root:
        return []

    col_map = defaultdict(list)
    queue = deque([(root, 0, 0)])

    while queue:
        node, row, col = queue.popleft()
        col_map[col].append((row, node.val))

        if node.left:
            queue.append((node.left, row + 1, col - 1))
        if node.right:
            queue.append((node.right, row + 1, col + 1))

    result = []
    for col in sorted(col_map.keys()):
        result.append([val for _, val in sorted(col_map[col])])

    return result
```
**Time:** O(N log N) | **Space:** O(N)

---

## 4. Zigzag Level Order Traversal (LeetCode 103)

```python
from collections import deque

def zigzagLevelOrder(root: TreeNode) -> list[list[int]]:
    if not root:
        return []
    result = []
    queue = deque([root])
    left_to_right = True

    while queue:
        level_size = len(queue)
        level = deque()

        for _ in range(level_size):
            node = queue.popleft()
            if left_to_right:
                level.append(node.val)
            else:
                level.appendleft(node.val)
            if node.left:
                queue.append(node.left)
            if node.right:
                queue.append(node.right)

        result.append(list(level))
        left_to_right = not left_to_right

    return result
```
**Time:** O(N) | **Space:** O(N)

---

## 5. Binary Tree Cameras — Greedy on Tree (LeetCode 968)

**State machine DFS:** Each node returns: 0=Not covered, 1=Has camera, 2=Covered.

```python
def minCameraCover(root: TreeNode) -> int:
    cameras = [0]

    def dfs(node) -> int:
        if not node:
            return 2    # Null nodes are "covered"

        left = dfs(node.left)
        right = dfs(node.right)

        if left == 0 or right == 0:
            cameras[0] += 1
            return 1

        if left == 1 or right == 1:
            return 2

        return 0

    if dfs(root) == 0:
        cameras[0] += 1

    return cameras[0]
```
**Time:** O(N) | **Space:** O(H)

---

## 6. House Robber III — Tree DP (LeetCode 337)

```python
def rob(root: TreeNode) -> int:
    def dfs(node):
        if not node:
            return 0, 0

        left_rob, left_skip = dfs(node.left)
        right_rob, right_skip = dfs(node.right)

        rob_curr = node.val + left_skip + right_skip
        skip_curr = max(left_rob, left_skip) + max(right_rob, right_skip)

        return rob_curr, skip_curr

    return max(dfs(root))
```
**Time:** O(N) | **Space:** O(H)

---

## 7. Serialize & Deserialize Binary Tree (LeetCode 297)

```python
class Codec:
    def serialize(self, root: TreeNode) -> str:
        tokens = []
        def preorder(node):
            if not node:
                tokens.append('#')
                return
            tokens.append(str(node.val))
            preorder(node.left)
            preorder(node.right)
        preorder(root)
        return ','.join(tokens)

    def deserialize(self, data: str) -> TreeNode:
        tokens = iter(data.split(','))

        def build():
            val = next(tokens)
            if val == '#':
                return None
            node = TreeNode(int(val))
            node.left = build()
            node.right = build()
            return node

        return build()
```
**Time:** O(N) serialize and deserialize | **Space:** O(N)

---

## 8. Binary Tree Maximum Path Sum (LeetCode 124)

```python
def maxPathSum(root: TreeNode) -> int:
    max_sum = [float('-inf')]

    def max_gain(node) -> int:
        if not node:
            return 0

        left_gain = max(0, max_gain(node.left))
        right_gain = max(0, max_gain(node.right))

        price_through = node.val + left_gain + right_gain
        max_sum[0] = max(max_sum[0], price_through)

        return node.val + max(left_gain, right_gain)

    max_gain(root)
    return max_sum[0]
```
**Time:** O(N) | **Space:** O(H)

---

## 9. Count Good Nodes (LeetCode 1448)

```python
def goodNodes(root: TreeNode) -> int:
    def dfs(node, max_so_far) -> int:
        if not node:
            return 0
        is_good = 1 if node.val >= max_so_far else 0
        new_max = max(max_so_far, node.val)
        return is_good + dfs(node.left, new_max) + dfs(node.right, new_max)

    return dfs(root, float('-inf'))
```
**Time:** O(N) | **Space:** O(H)

---

## 10. Path Sum III — O(N) Using Prefix Sum (LeetCode 437)

```python
from collections import defaultdict

def pathSum(root: TreeNode, targetSum: int) -> int:
    prefix_counts = defaultdict(int)
    prefix_counts[0] = 1

    def dfs(node, current_sum) -> int:
        if not node:
            return 0
        current_sum += node.val
        count = prefix_counts[current_sum - targetSum]
        prefix_counts[current_sum] += 1
        count += dfs(node.left, current_sum)
        count += dfs(node.right, current_sum)
        prefix_counts[current_sum] -= 1   # Backtrack
        return count

    return dfs(root, 0)
```
**Time:** O(N) | **Space:** O(N) for prefix map

**Backtracking is critical:** Without decrementing, paths from one branch
would incorrectly be counted in another branch.

---

## 11. Lowest Common Ancestor — Binary Tree (LeetCode 236)

```python
def lowestCommonAncestor(root: TreeNode, p: TreeNode, q: TreeNode) -> TreeNode:
    if not root or root is p or root is q:
        return root

    left = lowestCommonAncestor(root.left, p, q)
    right = lowestCommonAncestor(root.right, p, q)

    if left and right:
        return root
    return left or right
```
**Time:** O(N) | **Space:** O(H)

---

## 12. All Paths from Root to Leaves (LeetCode 257)

```python
def binaryTreePaths(root: TreeNode) -> list[str]:
    result = []
    def dfs(node, path):
        path.append(str(node.val))
        if not node.left and not node.right:
            result.append('->'.join(path))
        else:
            if node.left: dfs(node.left, path)
            if node.right: dfs(node.right, path)
        path.pop()
    if root: dfs(root, [])
    return result
```
**Time:** O(N) | **Space:** O(H)

---

## Traversal Complexity Summary

| Traversal | Time | Space | Notes |
|---|---|---|---|
| Recursive inorder | O(N) | O(H) | Call stack |
| Iterative inorder | O(N) | O(H) | Explicit stack |
| Morris inorder | O(N) | O(1) | Threads tree temporarily |
| Morris preorder | O(N) | O(1) | Visit on thread creation |
| Iterative postorder | O(N) | O(H) | Reversed preorder |
| BFS level order | O(N) | O(W) | W = max width |
| Vertical order | O(N log N) | O(N) | Sorting columns |

## Interview Tips

1. **Morris traversal**: Explain the threading concept first. Interviewer wants to see WHY it works, not just the code.
2. **Tree DP**: Define the return type of the recursive function clearly (e.g., "returns max gain starting from this node going downward").
3. **Path Sum III**: Prefix sum + backtracking pattern. Know it cold.
4. **Serialize/Deserialize**: Preorder with null markers is the clearest approach.
5. **Camera problem**: The greedy argument (never place camera at leaf, place at leaf's parent) is the key insight to explain.

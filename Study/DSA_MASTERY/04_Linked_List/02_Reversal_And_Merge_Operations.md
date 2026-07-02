# Reversal & Merge Operations — Linked List Mastery

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Pointer manipulation, recursion, heap-based merging
> **Core Theme:** In-place structural transformations and merge-based
> techniques that demonstrate deep linked list pointer fluency.

---

## 1. In-Place Reversal — Iterative & Recursive

### Iterative (Canonical)

```python
class ListNode:
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next

def reverseList(head: ListNode) -> ListNode:
    prev, curr = None, head
    while curr:
        nxt = curr.next    # Save next
        curr.next = prev   # Reverse link
        prev = curr        # Advance prev
        curr = nxt         # Advance curr
    return prev            # New head
```
**Time:** O(N) | **Space:** O(1)

**Invariant:** At each step, `prev` points to the already-reversed sublist,
`curr` points to the not-yet-reversed sublist. When `curr` is None, `prev` is the new head.

### Recursive

```python
def reverseListRecursive(head: ListNode) -> ListNode:
    if not head or not head.next:
        return head
    new_head = reverseListRecursive(head.next)
    head.next.next = head   # Node after head points back to head
    head.next = None        # Head becomes new tail
    return new_head
```
**Time:** O(N) | **Space:** O(N) call stack

**Recursive insight:** `reverseListRecursive(head.next)` returns the head
of the fully reversed sublist. At that point, `head.next` is the last node
of the reversed sublist, and we make it point back to `head`.

---

## 2. Reverse Linked List Between Positions (LeetCode 92)

**Approach:** Use a dummy node, advance to position `left-1`, then do an
iterative "head insertion" to reverse the sublist of length `right-left+1`.

```python
def reverseBetween(head: ListNode, left: int, right: int) -> ListNode:
    dummy = ListNode(0)
    dummy.next = head
    prev = dummy

    # Step 1: Move prev to node just before position 'left'
    for _ in range(left - 1):
        prev = prev.next

    curr = prev.next    # curr = node at position 'left'

    # Step 2: Head-insertion reversal
    for _ in range(right - left):
        nxt = curr.next
        curr.next = nxt.next
        nxt.next = prev.next
        prev.next = nxt

    return dummy.next
```
**Time:** O(N) | **Space:** O(1)

**Head-insertion technique trace (reverse 2→5 in 1→2→3→4→5):**
- Initial: prev→1, curr→2, list: 1→2→3→4→5
- i=0: extract 3, insert after 1: 1→3→2→4→5
- i=1: extract 4, insert after 1: 1→4→3→2→5
- i=2: extract 5, insert after 1: 1→5→4→3→2
Result: 1→5→4→3→2 ✓

---

## 3. Reverse Nodes in K-Group (LeetCode 25) — Full Solution

```python
def reverseKGroup(head: ListNode, k: int) -> ListNode:
    def has_k_nodes(node: ListNode, k: int) -> bool:
        count = 0
        while node and count < k:
            node = node.next
            count += 1
        return count == k

    def reverse_k(head: ListNode, k: int) -> tuple:
        prev, curr = None, head
        for _ in range(k):
            nxt = curr.next
            curr.next = prev
            prev = curr
            curr = nxt
        return prev, head, curr

    dummy = ListNode(0)
    dummy.next = head
    group_prev = dummy

    while has_k_nodes(group_prev.next, k):
        new_head, group_tail, remaining = reverse_k(group_prev.next, k)
        group_prev.next = new_head
        group_tail.next = remaining
        group_prev = group_tail

    return dummy.next
```
**Time:** O(N) | **Space:** O(1)

**Trace on `1→2→3→4→5`, k=2:**
- Group 1: reverse `1→2` → `2→1`, stitch: dummy→2→1→3→4→5
- Group 2: reverse `3→4` → `4→3`, stitch: dummy→2→1→4→3→5
- Group 3: `5` alone, `has_k_nodes` returns False → left unchanged
- Result: `2→1→4→3→5` ✓

---

## 4. Merge Two Sorted Lists (LeetCode 21)

```python
def mergeTwoLists(l1: ListNode, l2: ListNode) -> ListNode:
    dummy = ListNode(0)
    curr = dummy
    while l1 and l2:
        if l1.val <= l2.val:
            curr.next = l1
            l1 = l1.next
        else:
            curr.next = l2
            l2 = l2.next
        curr = curr.next
    curr.next = l1 or l2
    return dummy.next
```
**Time:** O(M + N) | **Space:** O(1)

---

## 5. Merge K Sorted Lists — Heap Approach (LeetCode 23)

```python
import heapq

def mergeKLists(lists: list[ListNode]) -> ListNode:
    heap = []
    for i, node in enumerate(lists):
        if node:
            heapq.heappush(heap, (node.val, i, node))

    dummy = ListNode(0)
    curr = dummy

    while heap:
        val, i, node = heapq.heappop(heap)
        curr.next = node
        curr = curr.next
        if node.next:
            heapq.heappush(heap, (node.next.val, i, node.next))

    return dummy.next
```
**Time:** O(N log K) where N = total nodes, K = number of lists
**Space:** O(K) for the heap

**Why O(N log K)?** Each of the N nodes is pushed and popped from the
heap exactly once. Each push/pop is O(log K). Total: N × O(log K).

**Python gotcha:** `ListNode` objects aren't comparable. The `index i`
as tiebreaker prevents comparison errors when two values are equal.

---

## 6. Sort Linked List — Merge Sort O(N log N) Time (LeetCode 148)

```python
def sortList(head: ListNode) -> ListNode:
    # Count total length
    length, curr = 0, head
    while curr:
        length += 1
        curr = curr.next

    dummy = ListNode(0)
    dummy.next = head

    size = 1
    while size < length:
        prev, curr = dummy, dummy.next

        while curr:
            left = curr
            right = split(left, size)
            curr  = split(right, size)

            merged_head, merged_tail = merge(left, right)
            prev.next = merged_head
            merged_tail.next = curr
            prev = merged_tail

        size *= 2

    return dummy.next

def split(head: ListNode, n: int):
    """Advance n steps, sever, return next node."""
    for _ in range(n - 1):
        if head and head.next:
            head = head.next
    if not head:
        return None
    nxt = head.next
    head.next = None
    return nxt

def merge(l1: ListNode, l2: ListNode):
    """Merge two sorted lists. Returns (head, tail)."""
    dummy = ListNode(0)
    curr = dummy
    while l1 and l2:
        if l1.val <= l2.val:
            curr.next, l1 = l1, l1.next
        else:
            curr.next, l2 = l2, l2.next
        curr = curr.next
    curr.next = l1 or l2
    while curr.next:
        curr = curr.next
    return dummy.next, curr
```
**Time:** O(N log N) | **Space:** O(log N) recursive, O(1) iterative bottom-up

---

## 7. LRU Cache — Doubly Linked List + HashMap (LeetCode 146)

```python
class DLinkedNode:
    def __init__(self, key=0, val=0):
        self.key = key
        self.val = val
        self.prev = None
        self.next = None

class LRUCache:
    """
    O(1) get and put using:
    - HashMap for O(1) lookup by key
    - Doubly linked list for O(1) move-to-front and eviction
    Sentinel head and tail nodes eliminate edge-case null checks.
    """
    def __init__(self, capacity: int):
        self.cap = capacity
        self.cache = {}
        self.head = DLinkedNode()
        self.tail = DLinkedNode()
        self.head.next = self.tail
        self.tail.prev = self.head

    def _remove(self, node: DLinkedNode) -> None:
        node.prev.next = node.next
        node.next.prev = node.prev

    def _insert_at_tail(self, node: DLinkedNode) -> None:
        node.prev = self.tail.prev
        node.next = self.tail
        self.tail.prev.next = node
        self.tail.prev = node

    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        node = self.cache[key]
        self._remove(node)
        self._insert_at_tail(node)
        return node.val

    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self._remove(self.cache[key])
        node = DLinkedNode(key, value)
        self.cache[key] = node
        self._insert_at_tail(node)
        if len(self.cache) > self.cap:
            lru = self.head.next
            self._remove(lru)
            del self.cache[lru.key]
```
**Time:** O(1) for both `get` and `put` | **Space:** O(capacity)

---

## 8. Flatten Multilevel Doubly Linked List (LeetCode 430)

```python
class Node:
    def __init__(self, val, prev=None, next=None, child=None):
        self.val = val
        self.prev = prev
        self.next = next
        self.child = child

def flatten(head: Node) -> Node:
    if not head:
        return head

    dummy = Node(0)
    dummy.next = head
    head.prev = dummy

    stack = [head]
    prev = dummy

    while stack:
        curr = stack.pop()

        prev.next = curr
        curr.prev = prev

        if curr.next:
            stack.append(curr.next)
        if curr.child:
            stack.append(curr.child)
            curr.child = None

        prev = curr

    dummy.next.prev = None
    return dummy.next
```
**Time:** O(N) | **Space:** O(N) stack in worst case

---

## 9. Swap Nodes in Pairs (LeetCode 24)

```python
def swapPairs(head: ListNode) -> ListNode:
    dummy = ListNode(0)
    dummy.next = head
    prev = dummy

    while prev.next and prev.next.next:
        a = prev.next
        b = prev.next.next

        prev.next = b
        a.next = b.next
        b.next = a

        prev = a

    return dummy.next
```
**Time:** O(N) | **Space:** O(1)

---

## 10. Odd-Even Linked List (LeetCode 328)

```python
def oddEvenList(head: ListNode) -> ListNode:
    if not head or not head.next:
        return head

    odd = head
    even = head.next
    even_head = even

    while even and even.next:
        odd.next = even.next
        odd = odd.next
        even.next = odd.next
        even = even.next

    odd.next = even_head
    return head
```
**Time:** O(N) | **Space:** O(1)

---

## 11. Add Two Numbers I & II

### Variant I — Digits in Reverse Order (LeetCode 2)

```python
def addTwoNumbers(l1: ListNode, l2: ListNode) -> ListNode:
    dummy = ListNode(0)
    curr = dummy
    carry = 0

    while l1 or l2 or carry:
        val = carry
        if l1:
            val += l1.val
            l1 = l1.next
        if l2:
            val += l2.val
            l2 = l2.next
        carry, digit = divmod(val, 10)
        curr.next = ListNode(digit)
        curr = curr.next

    return dummy.next
```
**Time:** O(max(M, N)) | **Space:** O(max(M, N))

### Variant II — Digits in Natural Order (LeetCode 445)

```python
def addTwoNumbersII(l1: ListNode, l2: ListNode) -> ListNode:
    s1, s2 = [], []
    while l1:
        s1.append(l1.val)
        l1 = l1.next
    while l2:
        s2.append(l2.val)
        l2 = l2.next

    carry = 0
    result_head = None

    while s1 or s2 or carry:
        val = carry
        if s1: val += s1.pop()
        if s2: val += s2.pop()
        carry, digit = divmod(val, 10)
        node = ListNode(digit)
        node.next = result_head
        result_head = node

    return result_head
```
**Time:** O(M + N) | **Space:** O(M + N) for stacks

---

## Summary: In-Place Manipulation Patterns

| Operation | Key Trick | Pitfall |
|---|---|---|
| Full reversal | 3-pointer prev/curr/nxt | Forgetting to null old head |
| Partial reversal (l..r) | Head-insertion into fixed prev | Off-by-one in loop count |
| K-group reversal | has_k_nodes check + stitch tail | Not handling partial last group |
| Merge sorted | Dummy head + interleave | Not attaching remaining tail |
| Merge K sorted | Min-heap + list index tiebreak | ListNode not comparable |
| Sort list | Bottom-up merge sort | Tracking tail in merge result |
| LRU | Sentinel DLL + HashMap | Forgetting to delete from map on evict |

## Interview Tips

1. **Always draw the pointer diagram** for reversal problems.
2. **Dummy/sentinel nodes** eliminate 80% of edge cases in merge/reversal.
3. For **Reverse K-Group**, practice the has_k_nodes + stitch pattern.
4. **LRU Cache**: The sentinel DLL pattern is expected by FAANG interviewers.
5. For **sort list**, clarify if O(log N) stack space is acceptable before
   choosing top-down vs bottom-up.

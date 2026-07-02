# Balanced BST Concepts — AVL, Red-Black, and Order Statistics

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** BST operations, tree rotations, amortized analysis
> **Core Theme:** Understanding balanced tree invariants and applying them
> through problem patterns using sorted containers.

---

## 1. AVL Tree Rotations

**Height balance invariant:** |height(left) - height(right)| ≤ 1 for every node.

### Single Left Rotation (Right-heavy)

```
    A                B
   / \              / \
  X   B    →      A   Z
     / \         / \
    Y   Z       X   Y
```

### Single Right Rotation (Left-heavy)

```
      A              B
     / \            / \
    B   Z    →     X   A
   / \                / \
  X   Y              Y   Z
```

### Left-Right Rotation: left-rotate left child, then right-rotate root.
### Right-Left Rotation: right-rotate right child, then left-rotate root.

```python
class AVLNode:
    def __init__(self, val):
        self.val = val
        self.left = self.right = None
        self.height = 1

def height(node):
    return node.height if node else 0

def balance_factor(node):
    return height(node.left) - height(node.right) if node else 0

def rotate_right(node):
    new_root = node.left
    node.left = new_root.right
    new_root.right = node
    node.height = 1 + max(height(node.left), height(node.right))
    new_root.height = 1 + max(height(new_root.left), height(new_root.right))
    return new_root

def rotate_left(node):
    new_root = node.right
    node.right = new_root.left
    new_root.left = node
    node.height = 1 + max(height(node.left), height(node.right))
    new_root.height = 1 + max(height(new_root.left), height(new_root.right))
    return new_root

def avl_insert(root, val):
    if not root:
        return AVLNode(val)
    if val < root.val:
        root.left = avl_insert(root.left, val)
    elif val > root.val:
        root.right = avl_insert(root.right, val)
    else:
        return root

    root.height = 1 + max(height(root.left), height(root.right))
    bf = balance_factor(root)

    if bf > 1 and val < root.left.val:    # Left-Left
        return rotate_right(root)
    if bf < -1 and val > root.right.val:  # Right-Right
        return rotate_left(root)
    if bf > 1 and val > root.left.val:    # Left-Right
        root.left = rotate_left(root.left)
        return rotate_right(root)
    if bf < -1 and val < root.right.val:  # Right-Left
        root.right = rotate_right(root.right)
        return rotate_left(root)

    return root
```

**AVL height bound:** At most 1.44 log₂(N+2) - 0.328

---

## 2. Red-Black Tree Properties

1. Every node is Red or Black
2. Root is Black
3. Every leaf (NIL) is Black
4. Red nodes have both children Black (no two consecutive reds)
5. All paths from any node to descendant leaves contain equal Black-nodes

**Height bound:** ≤ 2 log₂(N+1)

**Proof:** Let `b` = black-height. Tree with black-height `b` has ≥ 2^b - 1 internal nodes.
So N ≥ 2^b - 1 → b ≤ log₂(N+1). Total height ≤ 2b ≤ 2 log₂(N+1).

**RB vs AVL:**
- AVL: more strictly balanced → faster lookups
- RB: fewer rotations on insert/delete → faster writes
- Python's `sortedcontainers.SortedList` uses B-tree principles

---

## 3. Sliding Window Median — Two Heaps (LeetCode 480)

```python
import heapq
from collections import defaultdict

def medianSlidingWindow(nums: list[int], k: int) -> list[float]:
    lo = []    # Max-heap (negated)
    hi = []    # Min-heap
    removed = defaultdict(int)
    lo_size = hi_size = 0

    def add(num):
        nonlocal lo_size, hi_size
        if not lo or num <= -lo[0]:
            heapq.heappush(lo, -num)
            lo_size += 1
        else:
            heapq.heappush(hi, num)
            hi_size += 1
        balance()

    def remove(num):
        nonlocal lo_size, hi_size
        removed[num] += 1
        if num <= -lo[0]:
            lo_size -= 1
        else:
            hi_size -= 1
        balance()

    def clean(heap, is_lo):
        while heap:
            top = -heap[0] if is_lo else heap[0]
            if removed[top]:
                removed[top] -= 1
                if is_lo: heapq.heappop(lo)
                else: heapq.heappop(hi)
            else:
                break

    def balance():
        nonlocal lo_size, hi_size
        while lo_size > hi_size + 1:
            clean(lo, True)
            val = -heapq.heappop(lo); lo_size -= 1
            heapq.heappush(hi, val); hi_size += 1
        while hi_size > lo_size:
            clean(hi, False)
            val = heapq.heappop(hi); hi_size -= 1
            heapq.heappush(lo, -val); lo_size += 1

    def get_median():
        clean(lo, True); clean(hi, False)
        if k % 2 == 1:
            return float(-lo[0])
        return (-lo[0] + hi[0]) / 2.0

    for i in range(k):
        add(nums[i])

    result = [get_median()]
    for i in range(k, len(nums)):
        add(nums[i])
        remove(nums[i - k])
        result.append(get_median())

    return result
```
**Time:** O(N log K) | **Space:** O(K)

---

## 4. My Calendar I — SortedList (LeetCode 729)

```python
from sortedcontainers import SortedList

class MyCalendar:
    def __init__(self):
        self.calendar = SortedList()

    def book(self, start: int, end: int) -> bool:
        idx = self.calendar.bisect_right((start, end)) - 1
        if idx >= 0 and self.calendar[idx][1] > start:
            return False
        if idx + 1 < len(self.calendar) and self.calendar[idx+1][0] < end:
            return False
        self.calendar.add((start, end))
        return True
```
**Time:** O(log N) per book | **Space:** O(N)

---

## 5. My Calendar III — Max K Simultaneous Events (LeetCode 732)

```python
from sortedcontainers import SortedDict

class MyCalendarThree:
    def __init__(self):
        self.diff = SortedDict()

    def book(self, start: int, end: int) -> int:
        self.diff[start] = self.diff.get(start, 0) + 1
        self.diff[end]   = self.diff.get(end, 0) - 1
        curr = max_k = 0
        for v in self.diff.values():
            curr += v
            max_k = max(max_k, curr)
        return max_k
```
**Time:** O(N log N) per book | **Space:** O(N)

---

## 6. Count of Range Sum (LeetCode 327)

```python
def countRangeSum(nums: list[int], lower: int, upper: int) -> int:
    from sortedcontainers import SortedList

    prefix = [0]
    for num in nums:
        prefix.append(prefix[-1] + num)

    count = 0
    seen = SortedList()

    for p in prefix:
        lo = seen.bisect_left(p - upper)
        hi = seen.bisect_right(p - lower)
        count += hi - lo
        seen.add(p)

    return count
```
**Time:** O(N log N) | **Space:** O(N)

---

## 7. The Skyline Problem (LeetCode 218)

```python
import heapq

def getSkyline(buildings: list[list[int]]) -> list[list[int]]:
    events = []
    for l, r, h in buildings:
        events.append((l, -h, r))
        events.append((r, 0, 0))

    events.sort()
    result = []
    heap = [(0, float('inf'))]
    prev_max = 0

    for x, neg_h, end in events:
        if neg_h != 0:
            heapq.heappush(heap, (neg_h, end))

        while heap[0][1] <= x:
            heapq.heappop(heap)

        curr_max = -heap[0][0]
        if curr_max != prev_max:
            result.append([x, curr_max])
            prev_max = curr_max

    return result
```
**Time:** O(N log N) | **Space:** O(N)

---

## 8. Smallest Number of Operations — K-Increasing (LeetCode 2111)

```python
from bisect import bisect_right

def kIncreasing(arr: list[int], k: int) -> int:
    def lis_length_non_decreasing(seq):
        tails = []
        for x in seq:
            pos = bisect_right(tails, x)
            if pos == len(tails):
                tails.append(x)
            else:
                tails[pos] = x
        return len(tails)

    total = 0
    for r in range(k):
        subseq = arr[r::k]
        lis_len = lis_length_non_decreasing(subseq)
        total += len(subseq) - lis_len

    return total
```
**Time:** O(N log N) | **Space:** O(N/k)

---

## AVL vs RB vs SortedList Trade-offs

| Property | AVL | Red-Black | SortedList (Python) |
|---|---|---|---|
| Height | 1.44 log N | 2 log N | O(log N) amortized |
| Search | O(log N) | O(log N) | O(log N) |
| Insert | O(log N), ≤2 rotations | O(log N), ≤3 rotations | O(log N) amortized |
| Delete | O(log N) | O(log N), fewer rotations | O(log N) |
| Best for | Read-heavy | Write-heavy | Python interviews |

## Interview Strategy

1. **Check if `sortedcontainers.SortedList` is allowed** — it provides O(log N) insert, delete, bisect and simplifies many problems.
2. **Skyline**: Lazy deletion from heap (check top validity at query time) is the correct pattern.
3. **Sliding window median**: Two-heap with lazy deletion is the O(N log K) solution.
4. **Sweep line + difference array**: `SortedDict` difference array is elegant for counting overlapping intervals.
5. **Carousel rule**: For any problem involving maintaining a sorted window, think SortedList first.

# Segment Tree — Full Mastery with Lazy Propagation

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Divide-and-conquer, binary indexing
> **Core Theme:** Building and extending segment trees for O(log N) range
> queries and updates, including the lazy propagation mechanism.

---

## 1. Point Update + Range Sum Query

```python
class SegmentTree:
    def __init__(self, nums: list[int]):
        self.n = len(nums)
        self.tree = [0] * (4 * self.n)
        self._build(nums, 0, 0, self.n - 1)

    def _build(self, nums, node, lo, hi):
        if lo == hi:
            self.tree[node] = nums[lo]
            return
        mid = (lo + hi) // 2
        self._build(nums, 2*node+1, lo, mid)
        self._build(nums, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def update(self, idx, val, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if lo == hi:
            self.tree[node] = val
            return
        mid = (lo + hi) // 2
        if idx <= mid:
            self.update(idx, val, 2*node+1, lo, mid)
        else:
            self.update(idx, val, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def query(self, l, r, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo:
            return 0
        if l <= lo and hi <= r:
            return self.tree[node]
        mid = (lo + hi) // 2
        return (self.query(l, r, 2*node+1, lo, mid) +
                self.query(l, r, 2*node+2, mid+1, hi))
```
**Build:** O(N) | **Update:** O(log N) | **Query:** O(log N) | **Space:** O(N)

---

## 2. Lazy Propagation — Range Update + Range Query

**The Lazy Tag Mechanism:**

When applying a range update, store the "pending" update at internal nodes.
Before using or updating children, **push down** the tag.

**Invariant:** `tree[node]` is correct only if lazy tags of all ancestors
have been applied. Always push down before accessing children.

```python
class LazySegTree:
    """
    Supports range add (l, r, val) and range sum query (l, r).
    lazy[node] = pending add to all elements in node's range.
    """
    def __init__(self, nums: list[int]):
        self.n = len(nums)
        self.tree = [0] * (4 * self.n)
        self.lazy = [0] * (4 * self.n)
        self._build(nums, 0, 0, self.n - 1)

    def _build(self, nums, node, lo, hi):
        if lo == hi:
            self.tree[node] = nums[lo]
            return
        mid = (lo + hi) // 2
        self._build(nums, 2*node+1, lo, mid)
        self._build(nums, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def _push_down(self, node, lo, hi):
        if self.lazy[node] != 0:
            mid = (lo + hi) // 2
            left, right = 2*node+1, 2*node+2
            self.tree[left]  += self.lazy[node] * (mid - lo + 1)
            self.tree[right] += self.lazy[node] * (hi - mid)
            self.lazy[left]  += self.lazy[node]
            self.lazy[right] += self.lazy[node]
            self.lazy[node] = 0

    def range_add(self, l, r, val, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo:
            return
        if l <= lo and hi <= r:
            self.tree[node] += val * (hi - lo + 1)
            self.lazy[node] += val
            return
        self._push_down(node, lo, hi)
        mid = (lo + hi) // 2
        self.range_add(l, r, val, 2*node+1, lo, mid)
        self.range_add(l, r, val, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def range_sum(self, l, r, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo:
            return 0
        if l <= lo and hi <= r:
            return self.tree[node]
        self._push_down(node, lo, hi)
        mid = (lo + hi) // 2
        return (self.range_sum(l, r, 2*node+1, lo, mid) +
                self.range_sum(l, r, 2*node+2, mid+1, hi))
```
**Range update:** O(log N) | **Range query:** O(log N) | **Space:** O(N)

---

## 3. Range Assign + Range Sum (Assign Lazy)

```python
class AssignLazySegTree:
    UNSET = float('inf')

    def __init__(self, n: int):
        self.n = n
        self.tree = [0] * (4 * n)
        self.lazy = [self.UNSET] * (4 * n)

    def _push_down(self, node, lo, hi):
        if self.lazy[node] != self.UNSET:
            mid = (lo + hi) // 2
            val = self.lazy[node]
            self.tree[2*node+1] = val * (mid - lo + 1)
            self.tree[2*node+2] = val * (hi - mid)
            self.lazy[2*node+1] = self.lazy[2*node+2] = val
            self.lazy[node] = self.UNSET

    def range_assign(self, l, r, val, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo: return
        if l <= lo and hi <= r:
            self.tree[node] = val * (hi - lo + 1)
            self.lazy[node] = val
            return
        self._push_down(node, lo, hi)
        mid = (lo + hi) // 2
        self.range_assign(l, r, val, 2*node+1, lo, mid)
        self.range_assign(l, r, val, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]
```

---

## 4. Range Minimum Query Segment Tree

```python
class RMQSegTree:
    def __init__(self, nums: list[int]):
        self.n = len(nums)
        self.tree = [float('inf')] * (4 * self.n)
        self._build(nums, 0, 0, self.n - 1)

    def _build(self, nums, node, lo, hi):
        if lo == hi:
            self.tree[node] = nums[lo]
            return
        mid = (lo + hi) // 2
        self._build(nums, 2*node+1, lo, mid)
        self._build(nums, 2*node+2, mid+1, hi)
        self.tree[node] = min(self.tree[2*node+1], self.tree[2*node+2])

    def query_min(self, l, r, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo: return float('inf')
        if l <= lo and hi <= r: return self.tree[node]
        mid = (lo + hi) // 2
        return min(self.query_min(l, r, 2*node+1, lo, mid),
                   self.query_min(l, r, 2*node+2, mid+1, hi))
```
**Time:** O(log N) per query | **Space:** O(N)

---

## 5. Dynamic Segment Tree (Sparse)

**For very large value ranges (e.g., 10^9) where array-based tree is infeasible:**

```python
class DynamicSegNode:
    __slots__ = ['val', 'left', 'right']
    def __init__(self):
        self.val = 0
        self.left = None
        self.right = None

class DynamicSegTree:
    def __init__(self, lo: int, hi: int):
        self.root = DynamicSegNode()
        self.lo = lo
        self.hi = hi

    def update(self, idx: int, delta: int, node=None, lo=None, hi=None):
        if node is None: node, lo, hi = self.root, self.lo, self.hi
        node.val += delta
        if lo == hi: return
        mid = (lo + hi) // 2
        if idx <= mid:
            if not node.left: node.left = DynamicSegNode()
            self.update(idx, delta, node.left, lo, mid)
        else:
            if not node.right: node.right = DynamicSegNode()
            self.update(idx, delta, node.right, mid+1, hi)

    def query(self, l: int, r: int, node=None, lo=None, hi=None):
        if node is None: node, lo, hi = self.root, self.lo, self.hi
        if not node or l > hi or r < lo: return 0
        if l <= lo and hi <= r: return node.val
        mid = (lo + hi) // 2
        return (self.query(l, r, node.left, lo, mid) +
                self.query(l, r, node.right, mid+1, hi))
```
**Time:** O(log(max_val)) per operation | **Space:** O(N log(max_val))

---

## 6. Merge Sort Tree for K-th Order Statistics

```python
from bisect import bisect_left

class MergeSortTree:
    """For queries: how many elements in range [l,r] are ≤ x?"""
    def __init__(self, nums: list[int]):
        self.n = len(nums)
        self.tree = [[] for _ in range(4 * self.n)]
        self._build(nums, 0, 0, self.n - 1)

    def _build(self, nums, node, lo, hi):
        if lo == hi:
            self.tree[node] = [nums[lo]]
            return
        mid = (lo + hi) // 2
        self._build(nums, 2*node+1, lo, mid)
        self._build(nums, 2*node+2, mid+1, hi)
        self.tree[node] = sorted(self.tree[2*node+1] + self.tree[2*node+2])

    def count_leq(self, l, r, x, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo: return 0
        if l <= lo and hi <= r:
            return bisect_left(self.tree[node], x + 1)
        mid = (lo + hi) // 2
        return (self.count_leq(l, r, x, 2*node+1, lo, mid) +
                self.count_leq(l, r, x, 2*node+2, mid+1, hi))
```
**Build:** O(N log N) | **Query:** O(log²N) | **Space:** O(N log N)

---

## 7. Count of Smaller Numbers After Self (LeetCode 315) — BIT/Coordinate Compress

```python
def countSmaller(nums: list[int]) -> list[int]:
    sorted_unique = sorted(set(nums))
    rank = {v: i+1 for i, v in enumerate(sorted_unique)}
    n = len(sorted_unique)

    tree = [0] * (n + 1)

    def update(i):
        while i <= n:
            tree[i] += 1
            i += i & (-i)

    def query(i):
        s = 0
        while i > 0:
            s += tree[i]
            i -= i & (-i)
        return s

    result = []
    for num in reversed(nums):
        r = rank[num]
        result.append(query(r - 1))
        update(r)

    return result[::-1]
```
**Time:** O(N log N) | **Space:** O(N)

---

## Segment Tree vs BIT

| Capability | Segment Tree | BIT (Fenwick) |
|---|---|---|
| Range sum query | ✓ O(log N) | ✓ O(log N) |
| Point update | ✓ O(log N) | ✓ O(log N) |
| Range update | ✓ with lazy | ✓ with 2 BITs |
| Range min/max | ✓ | ✗ (not standard) |
| Lazy propagation | ✓ | ✗ |
| Implementation complexity | Higher | Lower |
| Constant factor | ~4 | ~2 |

## Interview Tips

1. **The 4N array size:** Use `4 * N` for safety. Tree height is `ceil(log2(N))`, and complete binary tree has at most `4N` nodes.
2. **Push down before recursing:** Always call `_push_down` before accessing children — most common implementation bug.
3. **Identity elements:** Sum→0, Min→+∞, Max→-∞, Product→1.
4. **Dynamic seg tree:** When value range is huge but operations are few, use pointer-based allocation.
5. **Range assign vs range add:** These are different lazy types with different push-down logic. Mix them carefully.

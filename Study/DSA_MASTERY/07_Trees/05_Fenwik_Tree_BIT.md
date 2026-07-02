# Binary Indexed Tree (Fenwick Tree) — Deep Mastery

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Bit manipulation, prefix sums, cumulative operations
> **Core Theme:** Exploiting binary representation to achieve O(log N) prefix
> queries with minimal code complexity compared to segment trees.

---

## 1. The lowbit Trick — Binary Representation Explanation

**`lowbit(x) = x & (-x)`** extracts the lowest set bit of x.

**Why this works:** In two's complement, `-x` flips all bits of `x` and adds 1.
The lowest set bit of `x` is preserved; all lower bits become 0.

```
x       = ...1 0 1 1 0 0  (binary)
-x      = ...0 1 0 1 0 0  (flip + 1)
x & -x  = ...0 0 0 1 0 0  = lowest set bit
```

**In a Fenwick tree, `tree[i]` stores the sum of elements from index
`i - lowbit(i) + 1` to `i`.** The responsibility range of node `i` has length `lowbit(i)`.

```
i=1 (001): covers [1,1]     length 1
i=2 (010): covers [1,2]     length 2
i=3 (011): covers [3,3]     length 1
i=4 (100): covers [1,4]     length 4
i=6 (110): covers [5,6]     length 2
i=8 (1000): covers [1,8]    length 8
```

**Prefix sum query:** Sum elements 1..x by repeatedly stripping the lowest bit.
**Point update:** Propagate change upward by adding the lowest bit.

---

## 2. Basic BIT — Point Update + Prefix Sum

```python
class BIT:
    """1-indexed Fenwick Tree for prefix sums."""

    def __init__(self, n: int):
        self.n = n
        self.tree = [0] * (n + 1)

    def update(self, i: int, delta: int) -> None:
        """Add delta to position i (1-indexed)."""
        while i <= self.n:
            self.tree[i] += delta
            i += i & (-i)

    def prefix_sum(self, i: int) -> int:
        """Return sum of elements from 1 to i (inclusive)."""
        s = 0
        while i > 0:
            s += self.tree[i]
            i -= i & (-i)
        return s

    def range_sum(self, l: int, r: int) -> int:
        """Return sum of elements from l to r (inclusive)."""
        return self.prefix_sum(r) - self.prefix_sum(l - 1)

    def build(self, nums: list[int]) -> None:
        """Build BIT from array in O(N) time."""
        for i, val in enumerate(nums, 1):
            self.tree[i] += val
            j = i + (i & -i)
            if j <= self.n:
                self.tree[j] += self.tree[i]
```
**Update:** O(log N) | **Query:** O(log N) | **Build:** O(N) | **Space:** O(N)

---

## 3. Range Update + Point Query

**To add `val` to range `[l, r]` and query single position `i`:**

```python
class RangeUpdateBIT:
    def __init__(self, n: int):
        self.bit = BIT(n)
        self.n = n

    def range_add(self, l: int, r: int, val: int) -> None:
        self.bit.update(l, val)
        self.bit.update(r + 1, -val)

    def point_query(self, i: int) -> int:
        return self.bit.prefix_sum(i)
```
**Time:** O(log N) both operations

---

## 4. Range Update + Range Query — Two BITs

**Formula:** `prefix_sum(i) = B1.prefix(i) * i - B2.prefix(i)`

```python
class RangeUpdateRangeQuery:
    def __init__(self, n: int):
        self.n = n
        self.b1 = [0] * (n + 2)
        self.b2 = [0] * (n + 2)

    def _update(self, bit, i, val):
        while i <= self.n:
            bit[i] += val
            i += i & (-i)

    def _prefix(self, bit, i):
        s = 0
        while i > 0:
            s += bit[i]
            i -= i & (-i)
        return s

    def range_add(self, l: int, r: int, val: int) -> None:
        self._update(self.b1, l, val)
        self._update(self.b1, r+1, -val)
        self._update(self.b2, l, val*(l-1))
        self._update(self.b2, r+1, -val*r)

    def prefix_sum(self, i: int) -> int:
        return self._prefix(self.b1, i) * i - self._prefix(self.b2, i)

    def range_sum(self, l: int, r: int) -> int:
        return self.prefix_sum(r) - self.prefix_sum(l - 1)
```
**Time:** O(log N) per operation | **Space:** O(N)

---

## 5. 2D BIT for Rectangle Sum Queries

```python
class BIT2D:
    def __init__(self, m: int, n: int):
        self.m = m
        self.n = n
        self.tree = [[0] * (n + 1) for _ in range(m + 1)]

    def update(self, x: int, y: int, delta: int) -> None:
        i = x
        while i <= self.m:
            j = y
            while j <= self.n:
                self.tree[i][j] += delta
                j += j & (-j)
            i += i & (-i)

    def prefix_sum(self, x: int, y: int) -> int:
        s = 0
        i = x
        while i > 0:
            j = y
            while j > 0:
                s += self.tree[i][j]
                j -= j & (-j)
            i -= i & (-i)
        return s

    def rectangle_sum(self, x1, y1, x2, y2) -> int:
        return (self.prefix_sum(x2, y2)
                - self.prefix_sum(x1-1, y2)
                - self.prefix_sum(x2, y1-1)
                + self.prefix_sum(x1-1, y1-1))
```
**Time:** O(log M × log N) per operation | **Space:** O(M × N)

---

## 6. BIT for Order Statistics — Find K-th Element in O(log N)

```python
class OrderStatisticBIT:
    def __init__(self, max_val: int):
        self.n = max_val
        self.tree = [0] * (max_val + 1)

    def update(self, i: int, delta: int) -> None:
        while i <= self.n:
            self.tree[i] += delta
            i += i & (-i)

    def kth_smallest(self, k: int) -> int:
        """Find k-th smallest using binary lifting on BIT — O(log N)."""
        pos = 0
        log = self.n.bit_length()

        for i in range(log, -1, -1):
            npos = pos + (1 << i)
            if npos <= self.n and self.tree[npos] < k:
                k -= self.tree[npos]
                pos = npos

        return pos + 1
```
**Time:** O(log N) | **Space:** O(max_val)

**Key insight:** Walk the BIT from highest bit to lowest. At each step, if
the left half's count is < k, move right and subtract left count from k.

---

## 7. Count Inversions Using BIT (LeetCode 315 variant)

```python
def countInversions(nums: list[int]) -> int:
    sorted_nums = sorted(set(nums))
    rank = {v: i+1 for i, v in enumerate(sorted_nums)}
    n = len(sorted_nums)

    tree = [0] * (n + 2)

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

    inversions = 0
    for num in reversed(nums):
        r = rank[num]
        inversions += query(r - 1)
        update(r)

    return inversions
```
**Time:** O(N log N) | **Space:** O(N)

---

## 8. Range Frequency Query (Offline)

```python
from collections import defaultdict
from bisect import bisect_left, bisect_right

class RangeFrequency:
    def __init__(self, arr: list[int]):
        self.positions = defaultdict(list)
        for i, v in enumerate(arr):
            self.positions[v].append(i)

    def query(self, l: int, r: int, x: int) -> int:
        pos = self.positions.get(x, [])
        return bisect_right(pos, r) - bisect_left(pos, l)
```
**Preprocessing:** O(N) | **Query:** O(log N) | **Space:** O(N)

---

## 9. Point Update Range Max — Monotone BIT

```python
class MaxBIT:
    """Only works for monotonically non-decreasing updates."""
    def __init__(self, n: int):
        self.n = n
        self.tree = [0] * (n + 1)

    def update(self, i: int, val: int) -> None:
        while i <= self.n:
            self.tree[i] = max(self.tree[i], val)
            i += i & (-i)

    def prefix_max(self, i: int) -> int:
        result = 0
        while i > 0:
            result = max(result, self.tree[i])
            i -= i & (-i)
        return result
```
**Time:** O(log N) | **Space:** O(N)

---

## 10. Longest Increasing Subsequence via Max-BIT

```python
def lengthOfLIS(nums: list[int]) -> int:
    sorted_unique = sorted(set(nums))
    rank = {v: i+1 for i, v in enumerate(sorted_unique)}
    n = len(sorted_unique)

    bit = [0] * (n + 1)

    def update(i, val):
        while i <= n:
            bit[i] = max(bit[i], val)
            i += i & (-i)

    def query(i):
        res = 0
        while i > 0:
            res = max(res, bit[i])
            i -= i & (-i)
        return res

    lis_len = 0
    for num in nums:
        r = rank[num]
        best = query(r - 1) + 1
        update(r, best)
        lis_len = max(lis_len, best)

    return lis_len
```
**Time:** O(N log N) | **Space:** O(N)

---

## BIT vs Segment Tree Decision Guide

| Scenario | Choose |
|---|---|
| Point update + range sum | BIT (simpler) |
| Range update + range sum | BIT (2 BITs) or Seg Tree (lazy) |
| Range min/max query | Seg Tree only |
| Lazy propagation needed | Seg Tree only |
| Very large range, sparse updates | Dynamic Seg Tree |
| Implementation speed in interview | BIT |
| Rectangle sum queries | 2D BIT |

## Interview Tips

1. **Always use 1-indexed BIT** — the lowbit trick breaks at index 0.
2. **The O(N) build:** `j = i + (i & -i)` propagates each node to its parent.
3. **Range update + range query:** The two-BIT formula `B1*i - B2` is elegant — derive it once and memorize.
4. **Order statistics:** The bitwise descent in `kth_smallest` is O(log N) vs O(log²N) for binary search over queries.
5. **BIT for LIS:** Coordinate compression + max-BIT is a powerful pattern for sequence DP problems.

# Quickselect & Order Statistics — Advanced Mastery Guide

> **The hardest binary search problem:** Finding the median of two sorted arrays O(log min(M,N)) with full derivation. Plus Quickselect, median-of-medians, and augmented BSTs.

---

## Table of Contents
1. [Quickselect — O(N) Average](#quickselect)
2. [Median-of-Medians — O(N) Worst Case](#mom)
3. [Median of Two Sorted Arrays — Full Derivation](#median-two)
4. [K-th Smallest in Sorted Matrix](#kth-matrix)
5. [Order Statistics Tree](#ost)
6. [Problems 1–7 with Full Solutions](#problems)
7. [Complexity Summary](#summary)

---

## 1. Quickselect — O(N) Average <a name="quickselect"></a>

### The Algorithm

Quickselect is quicksort's cousin: partition array around a pivot, recurse only into the relevant half.

```python
import random
from typing import Optional

def quickselect(arr: list, k: int) -> int:
    """
    Find kth smallest element (1-indexed).
    Time: O(N) average, O(N²) worst, Space: O(1) iterative
    
    Randomized pivot prevents adversarial O(N²) with high probability.
    """
    arr = arr[:]  # don't modify original
    lo, hi = 0, len(arr) - 1
    target = k - 1  # 0-indexed target
    
    while lo <= hi:
        pivot_idx = partition(arr, lo, hi)
        
        if pivot_idx == target:
            return arr[pivot_idx]
        elif pivot_idx < target:
            lo = pivot_idx + 1
        else:
            hi = pivot_idx - 1
    
    return arr[lo]


def partition(arr: list, lo: int, hi: int) -> int:
    """
    Lomuto partition scheme.
    Randomly choose pivot to avoid O(N²) worst case.
    Returns final position of pivot.
    """
    # Randomize pivot
    rand_idx = random.randint(lo, hi)
    arr[rand_idx], arr[hi] = arr[hi], arr[rand_idx]
    
    pivot = arr[hi]
    i = lo - 1  # boundary of elements <= pivot
    
    for j in range(lo, hi):
        if arr[j] <= pivot:
            i += 1
            arr[i], arr[j] = arr[j], arr[i]
    
    arr[i + 1], arr[hi] = arr[hi], arr[i + 1]
    return i + 1


def partition_hoare(arr: list, lo: int, hi: int) -> int:
    """
    Hoare's partition — faster in practice (3× fewer swaps than Lomuto).
    Uses two pointers moving toward center.
    
    IMPORTANT: Returns partition index p where arr[lo:p+1] <= arr[p+1:].
    The pivot may NOT be at index p after this function.
    """
    pivot = arr[(lo + hi) // 2]
    i, j = lo - 1, hi + 1
    
    while True:
        i += 1
        while arr[i] < pivot:
            i += 1
        j -= 1
        while arr[j] > pivot:
            j -= 1
        if i >= j:
            return j
        arr[i], arr[j] = arr[j], arr[i]
```

### Complexity Analysis

**Average case O(N):**
- After partitioning, expected pivot position is uniform in [0, N-1]
- Expected work: `T(N) = T(N/2) + O(N)` → O(N) by Master theorem

**Worst case O(N²):**
- Pivot always chosen as min/max → T(N) = T(N-1) + O(N) → O(N²)
- Randomized pivot makes this probability exponentially small: P(> cN²) < 2^(-c)

**Why not just sort?**
- Sorting is O(N log N) — Quickselect is **strictly faster** for selection
- When k << N or k ≈ N, Quickselect with early termination wins

---

## 2. Median-of-Medians — O(N) Worst Case <a name="mom"></a>

### The Algorithm

Guarantees **linear worst-case** by choosing a pivot that's always between 30th and 70th percentile.

```python
def median_of_medians(arr: list, k: int) -> int:
    """
    Find kth smallest element in O(N) WORST CASE.
    
    Strategy:
    1. Divide into groups of 5
    2. Find median of each group (sorting groups of 5 is O(1))
    3. Recursively find median of medians
    4. Use this as pivot for partition
    5. Recurse into correct partition
    
    Time: O(N) worst case, Space: O(log N) stack
    """
    def _select(arr, k):
        n = len(arr)
        if n <= 5:
            return sorted(arr)[k]
        
        # Step 1: Split into groups of 5
        groups = [arr[i:i+5] for i in range(0, n, 5)]
        
        # Step 2: Find median of each group
        medians = [sorted(g)[len(g) // 2] for g in groups]
        
        # Step 3: Recursively find median of medians
        pivot = _select(medians, len(medians) // 2)
        
        # Step 4: Partition around pivot
        low  = [x for x in arr if x < pivot]
        mid  = [x for x in arr if x == pivot]
        high = [x for x in arr if x > pivot]
        
        # Step 5: Recurse into correct part
        if k < len(low):
            return _select(low, k)
        elif k < len(low) + len(mid):
            return pivot
        else:
            return _select(high, k - len(low) - len(mid))
    
    return _select(arr[:], k)


# Why pivot is always between 30%-70%?
# With groups of 5, at least ceil(n/10) groups have median <= pivot.
# So at least ceil(n/10) * 3 = 3n/10 elements <= pivot.
# Similarly, >= 3n/10 elements >= pivot.
# Partition always reduces problem to at most 7n/10 + 6 elements.
# T(n) = T(n/5) + T(7n/10 + 6) + O(n) → O(n) (geometric series)
```

### When to Use Median-of-Medians vs Randomized Quickselect

| Criterion | Randomized | Median-of-Medians |
|-----------|------------|-------------------|
| Average time | O(N) | O(N) |
| Worst case | O(N²) | **O(N)** |
| Constant factor | ~2 | ~10-20 |
| Adversarial input | Vulnerable (without RNG) | Immune |
| Interview preference | ✓ (simpler) | If asked for O(N) guarantee |

---

## 3. Median of Two Sorted Arrays — Full Derivation <a name="median-two"></a>

**LeetCode 4 — The hardest binary search problem.**

**Problem:** Given sorted arrays `A` (size M) and `B` (size N), find median of combined array in O(log min(M, N)).

### The Key Insight: Binary Partition

The median partitions the combined 2N elements into two equal halves. We need to find where to "cut" both arrays such that:
1. Left half has exactly `(M + N) / 2` elements
2. `max(left_A, left_B) <= min(right_A, right_B)`

```
Left half                    |  Right half
A[0..i-1]  (i elements)     |  A[i..M-1]
B[0..j-1]  (j elements)     |  B[j..N-1]

Constraint: i + j = half_len = (M + N + 1) // 2
```

Binary search on `i` (cut position in A, smaller array):

```python
def find_median_sorted_arrays(nums1: list[int], nums2: list[int]) -> float:
    """
    LeetCode 4. Binary partition approach.
    Time: O(log min(M, N)), Space: O(1)
    
    ALWAYS binary search on the smaller array for O(log min(M,N)).
    """
    A, B = nums1, nums2
    M, N = len(A), len(B)
    
    # Ensure A is the smaller array
    if M > N:
        return find_median_sorted_arrays(B, A)
    
    # Binary search on cut position in A
    lo, hi = 0, M
    half_len = (M + N + 1) // 2  # left half size (round up handles odd total)
    
    while lo <= hi:
        i = (lo + hi) // 2  # cut position in A: A[0:i] in left half
        j = half_len - i    # cut position in B: B[0:j] in left half
        
        # Guard against out-of-bounds with sentinel values
        A_left  = A[i - 1] if i > 0 else float('-inf')
        A_right = A[i]     if i < M else float('inf')
        B_left  = B[j - 1] if j > 0 else float('-inf')
        B_right = B[j]     if j < N else float('inf')
        
        if A_left <= B_right and B_left <= A_right:
            # Perfect partition found!
            if (M + N) % 2 == 1:
                return float(max(A_left, B_left))
            else:
                return (max(A_left, B_left) + min(A_right, B_right)) / 2.0
        
        elif A_left > B_right:
            # A's left part too large — move cut left in A
            hi = i - 1
        else:
            # B's left part too large — move cut right in A
            lo = i + 1
    
    raise ValueError("Arrays are not sorted")


# Step-by-step trace for A=[1,3], B=[2]:
# M=2, N=1, half_len=(3+1)//2=2
# lo=0, hi=2
# 
# i=1, j=1:
#   A_left=1, A_right=3, B_left=2, B_right=inf
#   1<=inf: ok. 2<=3: ok. Perfect partition!
#   (2+1)%2=1 → return max(1,2)=2. ✓
#
# A=[1,2], B=[3,4]:
# M=2, N=2, half_len=2
# i=1, j=1: A_left=1, A_right=2, B_left=3, B_right=4
#   1<=4: ok. 3<=2? NO → B too large, lo=2
# i=2, j=0: A_left=2, A_right=inf, B_left=-inf, B_right=3
#   2<=3: ok. -inf<=inf: ok. Perfect!
#   (4)%2=0 → return (max(2,-inf)+min(inf,3))/2 = (2+3)/2 = 2.5 ✓
```

### Why This is O(log min(M, N))

Binary search on the smaller array (size M after swap):
- Range: [0, M], size M + 1
- Each step halves the range
- Total steps: `log₂(M + 1)` = O(log M) = O(log min(M, N))

**Common interview mistakes:**
1. Binary searching on the larger array → O(log max) not O(log min)
2. Off-by-one in `half_len` formula → wrong median for odd total length
3. Forgetting sentinel values → index out of bounds
4. Not handling `i=0` or `i=M` edge cases

---

## 4. K-th Smallest in Sorted Matrix <a name="kth-matrix"></a>

```python
import heapq

def kth_smallest_matrix(matrix: list[list[int]], k: int) -> int:
    """
    LeetCode 378. N×N matrix, rows and columns sorted.
    
    Approach 1: Binary search on value. O(N log(max-min)).
    Approach 2: Min-heap. O(k log N).
    """
    n = len(matrix)
    
    # Approach 1: Binary search — O(N log(max-min))
    def count_less_equal(mid: int) -> int:
        """Count elements <= mid in sorted matrix. O(N)."""
        count = 0
        row, col = n - 1, 0
        while row >= 0 and col < n:
            if matrix[row][col] <= mid:
                count += row + 1
                col += 1
            else:
                row -= 1
        return count
    
    lo, hi = matrix[0][0], matrix[n-1][n-1]
    while lo < hi:
        mid = (lo + hi) // 2
        if count_less_equal(mid) >= k:
            hi = mid
        else:
            lo = mid + 1
    
    return lo  # lo == hi == kth smallest


def kth_smallest_matrix_heap(matrix: list[list[int]], k: int) -> int:
    """
    Min-heap approach: O(k log N).
    Better when k << N².
    """
    n = len(matrix)
    # (value, row, col)
    heap = [(matrix[0][0], 0, 0)]
    visited = {(0, 0)}
    
    for _ in range(k - 1):
        val, r, c = heapq.heappop(heap)
        for dr, dc in [(0, 1), (1, 0)]:
            nr, nc = r + dr, c + dc
            if 0 <= nr < n and 0 <= nc < n and (nr, nc) not in visited:
                heapq.heappush(heap, (matrix[nr][nc], nr, nc))
                visited.add((nr, nc))
    
    return heapq.heappop(heap)[0]
```

---

## 5. Order Statistics Tree (Augmented BST) <a name="ost"></a>

An augmented BST where each node stores the **size of its subtree**. Supports:
- `select(k)` → find kth smallest element: O(log N)
- `rank(x)` → find rank of element x: O(log N)

```python
class OSTNode:
    def __init__(self, val):
        self.val = val
        self.left = self.right = None
        self.size = 1  # subtree size

class OrderStatisticsTree:
    """
    Augmented BST supporting O(log N) select and rank.
    (Without balancing: O(N) worst case; with AVL/Red-Black: O(log N) guaranteed)
    
    In practice: use SortedList from sortedcontainers (Python).
    """
    
    def __init__(self):
        self.root = None
    
    def _size(self, node: Optional[OSTNode]) -> int:
        return node.size if node else 0
    
    def _update(self, node: OSTNode):
        if node:
            node.size = 1 + self._size(node.left) + self._size(node.right)
    
    def insert(self, val: int):
        def _insert(node, val):
            if not node:
                return OSTNode(val)
            if val < node.val:
                node.left = _insert(node.left, val)
            else:
                node.right = _insert(node.right, val)
            self._update(node)
            return node
        self.root = _insert(self.root, val)
    
    def select(self, k: int) -> int:
        """Find kth smallest (1-indexed). O(log N) balanced."""
        def _select(node, k):
            left_size = self._size(node.left)
            if k == left_size + 1:
                return node.val
            elif k <= left_size:
                return _select(node.left, k)
            else:
                return _select(node.right, k - left_size - 1)
        return _select(self.root, k)
    
    def rank(self, val: int) -> int:
        """Find rank (1-indexed position) of val. O(log N) balanced."""
        def _rank(node, val):
            if not node:
                return 0
            if val < node.val:
                return _rank(node.left, val)
            elif val > node.val:
                return 1 + self._size(node.left) + _rank(node.right, val)
            else:
                return self._size(node.left) + 1
        return _rank(self.root, val)


# Python: use sortedcontainers.SortedList for O(log N) select/rank in practice
from sortedcontainers import SortedList

def kth_largest_stream(stream: list[int], queries: list[int]) -> list[int]:
    """Efficiently answer: after seeing stream[0:i], what's the kth largest?"""
    sl = SortedList()
    results = []
    for i, x in enumerate(stream):
        sl.add(x)
        k = queries[i]
        results.append(sl[-(k)] if k <= len(sl) else -1)
    return results
```

---

## 6. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: K-th Largest Element in Array
**LeetCode 215**

```python
def find_kth_largest(nums: list[int], k: int) -> int:
    """
    LeetCode 215.
    
    Method 1: Quickselect O(N) average
    Method 2: Min-heap of size k — O(N log k)
    Method 3: Sort — O(N log N)
    """
    # Method 1: Quickselect (kth largest = (N-k+1)th smallest)
    return quickselect(nums, len(nums) - k + 1)

def find_kth_largest_heap(nums: list[int], k: int) -> int:
    """Min-heap of size k. O(N log k)."""
    heap = []
    for num in nums:
        heapq.heappush(heap, num)
        if len(heap) > k:
            heapq.heappop(heap)
    return heap[0]
```

---

### Problem 2: K-th Smallest Prime Fraction
**LeetCode 786**

```python
def kth_smallest_prime_fraction(arr: list[int], k: int) -> list[int]:
    """
    LeetCode 786. arr = sorted primes + 1. Find kth smallest fraction p/q.
    
    Binary search on value: O(N log(N × max_value))
    
    For a given threshold x, count fractions <= x using two pointers.
    """
    n = len(arr)
    
    lo, hi = 0.0, 1.0
    
    while hi - lo > 1e-9:
        mid = (lo + hi) / 2
        
        count = 0
        max_frac = 0.0
        best_p, best_q = -1, -1
        j = 1
        
        for i in range(n - 1):
            while j < n and arr[i] >= mid * arr[j]:
                j += 1
            count += n - j
            if j < n and arr[i] / arr[j] > max_frac:
                max_frac = arr[i] / arr[j]
                best_p, best_q = i, j
        
        if count == k:
            return [arr[best_p], arr[best_q]]
        elif count < k:
            lo = mid
        else:
            hi = mid
    
    return [-1, -1]
```

---

### Problem 3: Find Median from Data Stream
**LeetCode 295**

```python
class MedianFinder:
    """
    LeetCode 295.
    
    Two heaps: max-heap for lower half, min-heap for upper half.
    Invariant: |lower| == |upper| or |lower| == |upper| + 1
    
    Add: O(log N), Find median: O(1)
    """
    
    def __init__(self):
        self.lower = []  # max-heap (negate for Python's min-heap)
        self.upper = []  # min-heap
    
    def addNum(self, num: int) -> None:
        # Add to lower max-heap
        heapq.heappush(self.lower, -num)
        
        # Balance: lower's max must <= upper's min
        if self.upper and -self.lower[0] > self.upper[0]:
            heapq.heappush(self.upper, -heapq.heappop(self.lower))
        
        # Rebalance sizes: |lower| must be >= |upper|
        if len(self.lower) < len(self.upper):
            heapq.heappush(self.lower, -heapq.heappop(self.upper))
        elif len(self.lower) > len(self.upper) + 1:
            heapq.heappush(self.upper, -heapq.heappop(self.lower))
    
    def findMedian(self) -> float:
        if len(self.lower) > len(self.upper):
            return -self.lower[0]
        return (-self.lower[0] + self.upper[0]) / 2.0
```

---

### Problem 4: K Closest Points to Origin
**LeetCode 973**

```python
def k_closest(points: list[list[int]], k: int) -> list[list[int]]:
    """
    LeetCode 973.
    
    Method 1: Sort by distance → O(N log N)
    Method 2: Max-heap of size k → O(N log k)
    Method 3: Quickselect → O(N) average
    """
    # Method 3: Quickselect on squared distances
    def dist_sq(p):
        return p[0]**2 + p[1]**2
    
    points = points[:]
    lo, hi = 0, len(points) - 1
    target = k - 1
    
    while lo < hi:
        pivot_idx = partition_by_dist(points, lo, hi)
        if pivot_idx == target:
            break
        elif pivot_idx < target:
            lo = pivot_idx + 1
        else:
            hi = pivot_idx - 1
    
    return points[:k]


def partition_by_dist(points, lo, hi):
    """Partition points by squared distance."""
    def dist_sq(p):
        return p[0]**2 + p[1]**2
    
    rand_idx = random.randint(lo, hi)
    points[rand_idx], points[hi] = points[hi], points[rand_idx]
    pivot_dist = dist_sq(points[hi])
    
    i = lo - 1
    for j in range(lo, hi):
        if dist_sq(points[j]) <= pivot_dist:
            i += 1
            points[i], points[j] = points[j], points[i]
    
    points[i+1], points[hi] = points[hi], points[i+1]
    return i + 1
```

---

### Problem 5: Sliding Window Median
**LeetCode 480**

```python
from sortedcontainers import SortedList

def median_sliding_window(nums: list[int], k: int) -> list[float]:
    """
    LeetCode 480. Maintain sorted list of current window, find median each step.
    
    SortedList: O(log k) insert/delete, O(1) median access
    Total: O(N log k)
    """
    sl = SortedList()
    result = []
    
    for i, num in enumerate(nums):
        sl.add(num)
        
        if len(sl) > k:
            sl.remove(nums[i - k])
        
        if len(sl) == k:
            if k % 2 == 1:
                result.append(float(sl[k // 2]))
            else:
                result.append((sl[k // 2 - 1] + sl[k // 2]) / 2.0)
    
    return result
```

---

### Problem 6: K-th Smallest in Multiplication Table
**LeetCode 668**

```python
def find_kth_number(m: int, n: int, k: int) -> int:
    """
    LeetCode 668. m×n multiplication table, find kth smallest.
    Binary search on value.
    
    Time: O(m log(m×n))
    """
    def count_less_equal(x: int) -> int:
        """Count numbers in table that are <= x."""
        count = 0
        for i in range(1, m + 1):
            count += min(x // i, n)
        return count
    
    lo, hi = 1, m * n
    while lo < hi:
        mid = (lo + hi) // 2
        if count_less_equal(mid) >= k:
            hi = mid
        else:
            lo = mid + 1
    
    return lo
```

---

### Problem 7: Nth Highest Salary (Order Statistics Pattern)

```python
def nth_highest_salary(salaries: list[int], n: int) -> Optional[int]:
    """
    SQL analog: SELECT DISTINCT salary ORDER BY salary DESC LIMIT 1 OFFSET n-1
    
    Using Quickselect on distinct values.
    Time: O(D) average where D = distinct count.
    """
    distinct = list(set(salaries))
    if n > len(distinct):
        return None
    
    # kth largest = (D - n + 1)th smallest of distinct values
    k = len(distinct) - n + 1
    return quickselect(distinct, k)
```

---

## 7. Complexity Summary <a name="summary"></a>

### Selection Algorithm Comparison

| Problem | Algorithm | Time | Space |
|---------|-----------|------|-------|
| kth largest (array) | Randomized Quickselect | O(N) avg | O(1) |
| kth largest (array) | Median-of-Medians | O(N) worst | O(log N) |
| kth largest (stream) | Two-Heap | O(N log k) | O(k) |
| kth smallest (sorted matrix) | Binary search | O(N log(max-min)) | O(1) |
| Median of two sorted arrays | Binary partition | O(log min(M,N)) | O(1) |
| Sliding window median | SortedList | O(N log k) | O(k) |
| kth in multiplication table | Binary search | O(M log MN) | O(1) |

### FAANG Interview Frequency

```
High: Kth largest, Median from stream, Kth in matrix, Median of two arrays
Medium: Sliding window median, K closest points
Advanced: Kth in multiplication table, Order statistics tree
```

### Interview Tips

> **"Quickselect has O(N²) worst case — is that acceptable?"** — Yes for most interviews. Mention median-of-medians for O(N) guarantee, but randomized pivot makes O(N²) exponentially unlikely.

> **"Why binary search for median of two sorted arrays, not merge?"** — Merge gives O(M+N). Binary search gives O(log min(M,N)) — dramatically faster for large arrays. The insight is we're binary searching on **which elements go into the left half**, not on the values.

> **"When to use each approach for kth element?"**
> - `k` is small: Min-heap O(N + k log N)
> - `k` varies: Quickselect O(N) average
> - Multiple queries: Sort once O(N log N)
> - Streaming: Two-heap O(log N) per element

---

*Previous: [Advanced Sorting ←](01_Advanced_Sorting.md) | Next: [Number Theory →](../13_Mathematical_Algorithms/01_Number_Theory.md)*

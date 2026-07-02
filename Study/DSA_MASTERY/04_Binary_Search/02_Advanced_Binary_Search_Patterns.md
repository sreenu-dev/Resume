# Advanced Binary Search Patterns — Mastery Guide

## Core Concept & Invariant

Beyond classic binary search on sorted arrays, advanced patterns apply binary search to:
1. **Structurally sorted** data (rotated arrays, 2D matrices)
2. **Implicit order** (peak finding, kth smallest in matrix)
3. **Value space** (median of two sorted arrays)
4. **Infinite structures** (unbounded arrays)

**The universal invariant for all binary search variants**:
> Maintain a search space [lo, hi] such that the answer always lies within it.
> Each step PROVABLY eliminates at least half the remaining search space.

The hardest binary search problems (median of two sorted arrays) require a binary partition argument where the correctness proof is non-trivial and must be internalized, not memorized.

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template: Search First/Last Position (handles duplicates)
# ─────────────────────────────────────────────────────────────
def find_first(arr: list, target: int) -> int:
    """First occurrence of target in sorted array."""
    lo, hi = 0, len(arr) - 1
    result = -1
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        if arr[mid] == target:
            result = mid
            hi = mid - 1    # Keep searching left for FIRST occurrence
        elif arr[mid] < target:
            lo = mid + 1
        else:
            hi = mid - 1
    return result

def find_last(arr: list, target: int) -> int:
    """Last occurrence of target in sorted array."""
    lo, hi = 0, len(arr) - 1
    result = -1
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        if arr[mid] == target:
            result = mid
            lo = mid + 1    # Keep searching right for LAST occurrence
        elif arr[mid] < target:
            lo = mid + 1
        else:
            hi = mid - 1
    return result

def find_range(arr: list, target: int) -> list:
    """[first, last] occurrence of target. O(log n)."""
    return [find_first(arr, target), find_last(arr, target)]
```

---

## Complexity Analysis

| Problem | Time | Space | Key Insight |
|---------|------|-------|-------------|
| Search rotated (no dup) | O(log n) | O(1) | Half is always sorted |
| Search rotated (dup) | O(n) worst | O(1) | Duplicates break invariant |
| Find minimum in rotated | O(log n) | O(1) | Minimum is inflection point |
| Peak element | O(log n) | O(1) | Always go toward higher neighbor |
| Search 2D matrix (fully sorted) | O(log(mn)) | O(1) | Treat as 1D array |
| Search 2D matrix (row-sorted) | O(n) | O(1) | Start top-right |
| Kth smallest in matrix | O(n log(max)) | O(1) | Binary search on value |
| Median of two sorted arrays | O(log(min(m,n))) | O(1) | Binary partition |

---

## Classic Problems

### Problem 1: Search in Rotated Sorted Array — Medium

**Problem**: Search target in a sorted array that's been rotated at an unknown pivot.

```python
def search_rotated(nums: list, target: int) -> int:
    """
    Key insight: even though rotated, ONE of the two halves is always sorted.
    
    At any mid:
    - If nums[lo] ≤ nums[mid]: left half [lo..mid] is sorted
    - Otherwise: right half [mid..hi] is sorted
    
    Once you know which half is sorted, determine which half contains target.
    
    Proof: A rotation creates exactly one "break point" where arr[i] > arr[i+1].
    Any subarray that doesn't contain the break point is fully sorted.
    Either [lo..mid] or [mid+1..hi] is break-point-free → one is always sorted.
    """
    lo, hi = 0, len(nums) - 1
    
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        
        if nums[mid] == target:
            return mid
        
        if nums[lo] <= nums[mid]:   # Left half is sorted
            if nums[lo] <= target < nums[mid]:
                hi = mid - 1   # Target in sorted left half
            else:
                lo = mid + 1   # Target in right half
        else:                       # Right half is sorted
            if nums[mid] < target <= nums[hi]:
                lo = mid + 1   # Target in sorted right half
            else:
                hi = mid - 1   # Target in left half
    
    return -1

# Time: O(log n)  Space: O(1)

def search_rotated_with_duplicates(nums: list, target: int) -> bool:
    """
    With duplicates: worst case O(n).
    
    When nums[lo] == nums[mid], cannot determine which half is sorted.
    Example: [1,1,1,1,1,0,1,1] — lo=1, mid=1, hi=1 → impossible to decide.
    Solution: increment lo (skip one duplicate). Degrades to O(n) when all duplicates.
    
    Key: with duplicates, we lose the O(log n) guarantee.
    """
    lo, hi = 0, len(nums) - 1
    
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        
        if nums[mid] == target:
            return True
        
        if nums[lo] < nums[mid]:   # STRICTLY less — left is sorted
            if nums[lo] <= target < nums[mid]:
                hi = mid - 1
            else:
                lo = mid + 1
        elif nums[lo] > nums[mid]:  # Right is sorted
            if nums[mid] < target <= nums[hi]:
                lo = mid + 1
            else:
                hi = mid - 1
        else:   # nums[lo] == nums[mid] — can't determine; skip one
            lo += 1
    
    return False
# Time: O(n) worst case  Space: O(1)
```

### Problem 2: Find Minimum in Rotated Sorted Array — Medium

```python
def find_min_rotated(nums: list) -> int:
    """
    Find the minimum element (= the inflection point / rotation point).
    
    Invariant: minimum is always in [lo, hi].
    
    If nums[mid] > nums[hi]: minimum is in RIGHT half (mid+1..hi)
      - Because the sorted right portion starts below mid (rotation point is there)
    If nums[mid] ≤ nums[hi]: minimum is in LEFT half (lo..mid)
      - Because the sorted portion runs from mid to hi; minimum is at or before mid
    
    Why compare with hi (not lo)?
    Comparing with lo creates ambiguity when the array is unrotated (lo < mid < hi all in order).
    Comparing with hi: if array unrotated, nums[mid] < nums[hi] always → correctly go left.
    """
    lo, hi = 0, len(nums) - 1
    
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if nums[mid] > nums[hi]:
            lo = mid + 1   # Minimum is in right half
        else:
            hi = mid       # Minimum is at mid or in left half
    
    return nums[lo]

# Time: O(log n)  Space: O(1)

def find_min_rotated_with_duplicates(nums: list) -> int:
    """
    With duplicates: O(n) worst case.
    When nums[mid] == nums[hi], can't determine — shrink hi.
    """
    lo, hi = 0, len(nums) - 1
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if nums[mid] > nums[hi]:
            lo = mid + 1
        elif nums[mid] < nums[hi]:
            hi = mid
        else:
            hi -= 1   # Skip one duplicate at hi — could lose minimum but bounds remain
    return nums[lo]
# Time: O(n) worst case (all same elements)  Space: O(1)
```

### Problem 3: Find Peak Element — Medium (Formal O(log n) Proof)

**Problem**: Find any peak element (greater than both neighbors). Assume nums[-1] = nums[n] = -∞.

```python
def find_peak_element(nums: list) -> int:
    """
    Binary search on peaks. NOT searching for a specific value.
    
    Formal proof that O(log n) is achievable:
    
    Claim: if nums[mid] < nums[mid+1], there exists a peak in [mid+1, hi].
    Proof: Define the subarray nums[mid+1..hi]. Since nums[mid] < nums[mid+1]:
    - If hi = mid+1: nums[mid+1] > nums[mid] and nums[mid+1] > nums[mid+2] = -∞ → peak at mid+1
    - If hi > mid+1: the subarray has a "rising" start (nums[mid+1] > nums[mid]).
      If nums[hi] > nums[hi-1]: nums[hi] is a peak (nums[hi+1] = -∞).
      Otherwise: by continuity of discrete functions, there must be a peak in (mid+1, hi).
      (Specifically: the global max of nums[mid+1..hi] is a peak.)
    
    Symmetric argument: if nums[mid] < nums[mid-1], peak exists in [lo, mid-1].
    
    Algorithm: always move toward the higher neighbor.
    """
    lo, hi = 0, len(nums) - 1
    
    while lo < hi:
        mid = lo + (hi - lo) // 2
        
        if nums[mid] < nums[mid+1]:
            lo = mid + 1   # Peak in right half (proven above)
        else:
            hi = mid       # Peak in left half (symmetric argument)
    
    return lo   # lo == hi == peak index

# Time: O(log n)  Space: O(1)

def find_peak_2d(matrix: list) -> list:
    """
    2D peak: element ≥ all 4 neighbors. Find any such peak.
    
    Binary search on column: find peak column col_mid.
    In col_mid, find the maximum element.
    If max element > both horizontal neighbors → it's a 2D peak.
    Otherwise, go toward the larger horizontal neighbor.
    
    Time: O(m log n) where matrix is m×n  Space: O(1)
    """
    m, n = len(matrix), len(matrix[0])
    lo, hi = 0, n - 1
    
    while lo < hi:
        col_mid = lo + (hi - lo) // 2
        
        # Find maximum in column col_mid
        max_row = max(range(m), key=lambda r: matrix[r][col_mid])
        
        left  = matrix[max_row][col_mid-1] if col_mid > 0 else float('-inf')
        right = matrix[max_row][col_mid+1] if col_mid < n-1 else float('-inf')
        
        if matrix[max_row][col_mid] < right:
            lo = col_mid + 1
        elif matrix[max_row][col_mid] < left:
            hi = col_mid - 1
        else:
            return [max_row, col_mid]   # 2D peak found
    
    # lo == hi — this column contains a peak
    max_row = max(range(m), key=lambda r: matrix[r][lo])
    return [max_row, lo]
```

### Problem 4: Search in 2D Matrix — Medium

```python
def search_matrix_fully_sorted(matrix: list, target: int) -> bool:
    """
    Matrix where rows are sorted AND first element of each row > last element of previous row.
    → Treat as 1D sorted array of mn elements.
    
    Binary search: index i → row=i//n, col=i%n
    """
    m, n = len(matrix), len(matrix[0])
    lo, hi = 0, m * n - 1
    
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        val = matrix[mid // n][mid % n]
        if val == target:
            return True
        elif val < target:
            lo = mid + 1
        else:
            hi = mid - 1
    
    return False
# Time: O(log(mn))  Space: O(1)

def search_matrix_row_sorted(matrix: list, target: int) -> bool:
    """
    Matrix where each row is sorted AND each column is sorted.
    (Does NOT require first element of row i+1 > last element of row i)
    
    Binary search cannot treat as 1D. Use "staircase search."
    
    Start at top-right corner (or bottom-left):
    - If matrix[r][c] == target: found
    - If matrix[r][c] > target: move left (eliminate column c for rows 0..r)
    - If matrix[r][c] < target: move down (eliminate row r for columns c..n-1)
    
    Invariant: target (if it exists) is always in matrix[r..m-1][0..c]
    Each step eliminates one row or one column → O(m+n) total steps.
    
    This is O(m+n) NOT O(log(mn)) — different problem, different complexity!
    """
    if not matrix or not matrix[0]: return False
    m, n = len(matrix), len(matrix[0])
    r, c = 0, n-1   # Start at top-right
    
    while r < m and c >= 0:
        if matrix[r][c] == target:
            return True
        elif matrix[r][c] > target:
            c -= 1   # Eliminate column c
        else:
            r += 1   # Eliminate row r
    
    return False
# Time: O(m+n)  Space: O(1)
```

### Problem 5: Median of Two Sorted Arrays — Very Hard (O(log(min(m,n))))

**Problem**: Given two sorted arrays of sizes m and n, find median. Must be O(log(min(m,n))).

```python
def find_median_sorted_arrays(nums1: list, nums2: list) -> float:
    """
    THE hardest standard binary search problem. Full derivation:
    
    Let A = nums1 (size m), B = nums2 (size n), m ≤ n (ensure by swapping).
    
    We partition both arrays into left and right halves such that:
    - len(A_left) + len(B_left) = (m+n+1)//2  (half the elements)
    - max(A_left) ≤ min(B_right)  AND  max(B_left) ≤ min(A_right)
    
    If both conditions hold: the median = average of max(A_left∪B_left) and min(A_right∪B_right)
    (or just max(A_left∪B_left) if total length is odd)
    
    Binary search on the partition index i of A (0 ≤ i ≤ m):
    - A is partitioned at index i: A_left = A[0..i-1], A_right = A[i..m-1]
    - j = half_len - i: B is partitioned at index j
    
    Condition for CORRECT partition: A[i-1] ≤ B[j] AND B[j-1] ≤ A[i]
    (The elements just crossing the partition boundary are in order)
    
    If A[i-1] > B[j]: i is too large → move i left (hi = i-1)
    If B[j-1] > A[i]: i is too small → move i right (lo = i+1)
    
    Proof of correctness:
    Monotone property: if A[i-1] > B[j], increasing j (decreasing i) lowers A[i-1]
    (moving toward earlier A elements, which are smaller) and increases B[j]
    (moving toward later B elements, which are larger). So the comparison is monotone.
    
    EDGE CASES:
    - i=0: A_left is empty → A[i-1] = -∞ (no constraint from left of A)
    - i=m: A_right is empty → A[i] = +∞
    - j=0: B_left is empty → B[j-1] = -∞
    - j=n: B_right is empty → B[j] = +∞
    """
    A, B = nums1, nums2
    m, n = len(A), len(B)
    
    # Ensure A is the shorter array (binary search on shorter)
    if m > n:
        return find_median_sorted_arrays(B, A)
    
    half_len = (m + n + 1) // 2   # Size of left partition
    lo, hi = 0, m
    
    while lo <= hi:
        i = lo + (hi - lo) // 2   # Partition index for A
        j = half_len - i          # Corresponding partition index for B
        
        # Handle edge cases with sentinels
        A_left_max  = A[i-1]  if i > 0 else float('-inf')
        A_right_min = A[i]    if i < m else float('inf')
        B_left_max  = B[j-1]  if j > 0 else float('-inf')
        B_right_min = B[j]    if j < n else float('inf')
        
        if A_left_max <= B_right_min and B_left_max <= A_right_min:
            # CORRECT partition found
            left_max = max(A_left_max, B_left_max)
            right_min = min(A_right_min, B_right_min)
            
            if (m + n) % 2 == 1:
                return float(left_max)
            else:
                return (left_max + right_min) / 2.0
        
        elif A_left_max > B_right_min:
            hi = i - 1   # A's left part too large; move partition left
        else:
            lo = i + 1   # B's left part too large; move partition right
    
    # Should never reach here if input is valid
    raise ValueError("Input arrays are not sorted")

# Time: O(log(min(m,n)))  Space: O(1)

def verify_median():
    """Verification with test cases."""
    cases = [
        ([1,3], [2], 2.0),
        ([1,2], [3,4], 2.5),
        ([0,0], [0,0], 0.0),
        ([], [1], 1.0),
        ([2], [], 2.0),
        ([1,3,5,7], [2,4,6,8], 4.5),
        ([1,2,3,4,5], [6,7,8,9,10], 5.5),
    ]
    for A, B, expected in cases:
        result = find_median_sorted_arrays(A, B)
        status = "✓" if abs(result - expected) < 1e-9 else "✗"
        print(f"{status} median({A}, {B}) = {result} (expected {expected})")

verify_median()
```

### Problem 6: Search in Infinite Sorted Array — Medium

**Problem**: Sorted array of unknown size. Find target. API: get(i) returns arr[i] or ∞ if out of bounds.

```python
class InfiniteArray:
    """Mock infinite sorted array."""
    def __init__(self, arr: list):
        self._arr = arr
    def get(self, i: int) -> int:
        return self._arr[i] if i < len(self._arr) else float('inf')

def search_infinite(arr: InfiniteArray, target: int) -> int:
    """
    Phase 1: Exponential search to find bounds [lo, hi] containing target.
    Phase 2: Binary search within [lo, hi].
    
    Phase 1: Start with window [0,1]. Double until hi element ≥ target.
    This finds hi in O(log p) steps where p = actual position of target.
    At that point, lo = hi/2 (from previous iteration).
    
    Phase 2: Binary search in O(log(hi-lo)) = O(log p) steps.
    
    Total: O(log p) where p = index of target.
    """
    # Phase 1: Exponential search to find range
    lo, hi = 0, 1
    while arr.get(hi) < target:
        lo = hi
        hi *= 2
    
    # Phase 2: Binary search in [lo, hi]
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        val = arr.get(mid)
        if val == target:
            return mid
        elif val < target:
            lo = mid + 1
        else:
            hi = mid - 1
    
    return -1

# Time: O(log p)  Space: O(1)
# p = index of target. If target not present, p = first index ≥ target.
```

### Problem 7: Count of Smaller Numbers After Self — Hard

**Problem**: For each nums[i], count how many elements to its right are smaller.

```python
def count_smaller(nums: list) -> list:
    """
    Multiple approaches:
    A. Brute force: O(n²) — nested loop
    B. Modified merge sort: O(n log n) — count inversions during merge
    C. Binary Indexed Tree (BIT/Fenwick): O(n log n) — coordinate compress + BIT
    D. Binary search + sorted list: O(n log n) — maintain sorted list, bisect for position
    
    We show approach D (binary search) and B (merge sort).
    """
    import bisect
    
    # Approach D: Sorted list + binary search
    result = [0] * len(nums)
    sorted_list = []
    
    for i in range(len(nums) - 1, -1, -1):
        # Find position where nums[i] would be inserted (= count of smaller elements)
        pos = bisect.bisect_left(sorted_list, nums[i])
        result[i] = pos
        bisect.insort(sorted_list, nums[i])   # O(n) insert (list shift) — use SortedList for true O(log n)
    
    return result
# Time: O(n²) due to list insort (use sortedcontainers.SortedList for O(n log n))

def count_smaller_merge_sort(nums: list) -> list:
    """
    Merge sort approach: O(n log n) guaranteed.
    
    Key insight: during merging, when we pick an element from the RIGHT subarray,
    all remaining elements in the LEFT subarray that are larger than it 
    contribute +1 to their "count of smaller elements to the right."
    
    Track original indices to update the result array correctly.
    """
    result = [0] * len(nums)
    indexed = list(enumerate(nums))
    
    def merge_count(arr: list) -> list:
        if len(arr) <= 1:
            return arr
        
        mid = len(arr) // 2
        left = merge_count(arr[:mid])
        right = merge_count(arr[mid:])
        
        merged = []
        i = j = 0
        while i < len(left) and j < len(right):
            if left[i][1] <= right[j][1]:
                # right[0..j-1] were already placed (all smaller than left[i])
                result[left[i][0]] += j
                merged.append(left[i])
                i += 1
            else:
                merged.append(right[j])
                j += 1
        
        while i < len(left):
            result[left[i][0]] += j   # All remaining right elements are smaller
            merged.append(left[i])
            i += 1
        while j < len(right):
            merged.append(right[j])
            j += 1
        
        return merged
    
    merge_count(indexed)
    return result

# Time: O(n log n)  Space: O(n)
```

---

## Advanced Variations

### Sqrt(x) — Integer Square Root

```python
def my_sqrt(x: int) -> int:
    """
    Find floor(sqrt(x)) without using math.sqrt.
    Binary search on the answer space [0, x].
    
    Invariant: lo ≤ floor(sqrt(x)) ≤ hi
    Feasibility: mid² ≤ x
    """
    if x < 2: return x
    lo, hi = 1, x // 2 + 1
    
    while lo < hi:
        mid = lo + (hi - lo + 1) // 2  # Maximize: template 2
        if mid * mid <= x:
            lo = mid
        else:
            hi = mid - 1
    
    return lo

# Time: O(log x)  Space: O(1)

def my_pow(x: float, n: int) -> float:
    """
    Fast exponentiation: x^n in O(log n).
    Binary search structure: halve the exponent each step.
    """
    if n < 0:
        x, n = 1/x, -n
    
    result = 1.0
    while n:
        if n & 1:            # If n is odd, multiply by x
            result *= x
        x *= x               # Square x
        n >>= 1              # Halve n
    
    return result
# Time: O(log n)  Space: O(1)
```

### First Bad Version

```python
def first_bad_version(n: int, is_bad) -> int:
    """
    Find first bad version using isBadVersion(v) API.
    Versions are 1 to n; once bad, all subsequent are bad.
    
    Classic binary search minimize (first True in monotone boolean sequence).
    """
    lo, hi = 1, n
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if is_bad(mid):
            hi = mid       # mid is bad; first bad might be here or earlier
        else:
            lo = mid + 1   # mid is good; first bad must be after
    return lo
# Time: O(log n)  Space: O(1)
```

### Count Negative Numbers in Sorted Matrix

```python
def count_negatives(grid: list) -> int:
    """
    Row-sorted descending, count all negative numbers.
    
    Use binary search per row to find first negative index.
    Total negatives in row r = n - first_negative_index.
    
    Optimization: since rows are also descending, the first negative
    index is non-decreasing. Use this for O(m+n) total.
    """
    m, n = len(grid), len(grid[0])
    count = 0
    col = n - 1   # Start from rightmost column
    
    for r in range(m):
        # Move left while grid[r][col] is negative
        while col >= 0 and grid[r][col] < 0:
            col -= 1
        count += n - col - 1
    
    return count
# Time: O(m+n) — staircase search  Space: O(1)
```

---

## The Median of Two Arrays — Deep Dive

```python
def median_analysis():
    """
    Complete analysis of the O(log(min(m,n))) median algorithm.
    
    Why NOT O(log(m+n)):
    We binary search on the SHORTER array only (size min(m,n)).
    Each iteration of binary search eliminates half of A's possible partition points.
    Total iterations = log(m) where m = min(m,n). NOT log(m+n).
    
    Why the partition is valid:
    Given total length L = m + n:
    - Left partition has exactly L//2 elements (or (L+1)//2 for odd)
    - If we choose i elements from A, we take j = (L+1)//2 - i from B
    - j is determined by i — only one free variable → binary search
    
    Why convergence is guaranteed:
    The condition A[i-1] ≤ B[j] is monotone in i:
    - As i increases: A[i-1] increases (A is sorted) and B[j] decreases (j decreases, B sorted)
    - So if A[i-1] > B[j]: i is too large → must decrease i → hi = i-1
    - If A[i-1] ≤ B[j] and B[j-1] > A[i]: i is too small → lo = i+1
    - This is a proper binary search → converges in O(log m) steps
    
    Proof that the found partition gives the median:
    When partition is valid (A[i-1] ≤ B[j] and B[j-1] ≤ A[i]):
    - All elements in left partition ≤ all elements in right partition
    - Left partition has exactly (m+n+1)//2 elements
    - The maximum of left partition is the median (for odd total)
    - Average of max(left) and min(right) is the median (for even total)
    """
    # Example walkthrough:
    A = [1, 3, 5, 7]       # m = 4
    B = [2, 4, 6, 8]       # n = 4
    # total = 8, half_len = 4
    # Search: i in [0, 4]
    # 
    # i=2: A_left=[1,3], A_right=[5,7], j=2, B_left=[2,4], B_right=[6,8]
    #   A[i-1]=3 ≤ B[j]=6 ✓ and B[j-1]=4 ≤ A[i]=5 ✓
    # VALID! left_max = max(3,4) = 4, right_min = min(5,6) = 5
    # median = (4+5)/2 = 4.5 ✓
    
    result = find_median_sorted_arrays(A, B)
    print(f"Median of {A} and {B} = {result}")   # Expected: 4.5

median_analysis()
```

---

## Edge Cases Bible

1. **Rotated array: lo vs hi comparison**: Compare `nums[lo] <= nums[mid]` (not `< nums[mid]`). Without equality, `nums=[1,1,2]` fails when lo=0, mid=0, both equal.

2. **Peak finding: comparing mid with mid+1**: Since loop is `while lo < hi`, mid < hi always, so `mid+1` is always a valid index. But if the loop were `while lo <= hi`, mid could equal hi and mid+1 would be out of bounds.

3. **Median of arrays: ensure m ≤ n**: Always swap so we binary search on the shorter array. Without this, j could go negative (if m > n and i is small).

4. **Median edge cases**:
   - One empty array: binary search on non-empty array works; sentinels handle the other
   - Arrays of size 1: works; partition at i=0 or i=1 with sentinels
   - Duplicate elements across arrays: algorithm handles correctly via ≤ comparison

5. **Infinite array search: duplicate target elements**: `bisect_left` behavior — the search finds the FIRST occurrence. For duplicates, loop through to find specific one.

6. **Count of smaller via merge sort: stable sort needed**: When `left[i][1] == right[j][1]`, we pick left (not right) to ensure stability. Otherwise, equal elements count each other incorrectly.

7. **2D matrix — which variant**: Two different problems:
   - Rows sorted, rows form complete sorted sequence → O(log mn) binary search
   - Rows and columns sorted (but rows don't chain) → O(m+n) staircase
   Never apply O(log mn) to the second variant!

8. **Find minimum in rotated — no duplicates assumed**: Algorithm breaks with duplicates (`nums=[1,1,1,0,1]`, hi decrements may skip minimum). Must use the duplicate-aware version.

9. **Exponential search upper bound**: When doubling hi, ensure hi doesn't overflow. In Python: not an issue. In C++: cap at INT_MAX.

10. **Sqrt overflow**: `mid * mid` can overflow for large x. Use `mid <= x // mid` instead of `mid * mid <= x` to avoid overflow in integer languages.

---

## Interview Tips

### What Interviewers Look For

1. **Immediately identify the half that's always sorted (rotated array)**: "At any midpoint, one of the two halves is guaranteed to be sorted. I use `nums[lo] <= nums[mid]` to determine which." State this before coding.

2. **Median of two arrays — communicate the approach first**: "This requires a binary partition of the shorter array. I maintain the invariant that left partition ≤ right partition using the cross-array comparisons." Draw the partition diagram if possible.

3. **The +1 in Template 2**: "For maximization binary search, I use `mid = lo + (hi - lo + 1) // 2`. Without the +1, when lo+1=hi and the current mid equals lo, we'd infinite loop." Show this awareness.

4. **Why O(log(min(m,n))) not O(log(m+n)) for median**: "We binary search on the shorter array only — log(min(m,n)) iterations. Not log(m+n) — that would require searching the combined space, which is unnecessary since j is fully determined by i."

5. **Edge cases for rotated search**: "Three cases I always check: 1) normal non-rotated, 2) left half contains pivot, 3) right half contains pivot. With duplicates, add: 4) can't determine → skip one."

6. **Staircase search for row-and-column sorted matrix**: "This is O(m+n) not O(log(mn)). I start at top-right (or bottom-left). If greater than target: move left (eliminate column). If less: move down (eliminate row). Each step eliminates one row or column."

7. **Count smaller after self — prefer merge sort in interview**: "The merge sort approach is deterministic O(n log n) and easy to verify. The Fenwick tree approach is faster in practice but harder to implement correctly under pressure."

8. **The universally applicable pattern**: For any problem where you can write `is_feasible(X)` returning bool and the function is monotone, binary search on X works. Ask yourself: "If X works, does X+1 (or X-1) also work?" If yes, binary search is applicable.

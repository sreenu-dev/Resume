# Prefix Sum & Difference Arrays — Mastery Guide

## Core Concept & Invariant

**Prefix Sum**: `P[i] = arr[0] + arr[1] + ... + arr[i-1]` (1-indexed, P[0]=0)

**Invariant**: `sum(arr[l..r]) = P[r+1] - P[l]`

This single invariant enables:
- Range sum query in O(1) (after O(n) build)
- Subarray sum = K problems via hash map on prefix sums
- XOR, product, and other associative operations
- Multi-dimensional extensions

**Difference Array**: `D[i] = arr[i] - arr[i-1]` (D[0] = arr[0])

**Invariant**: `arr[i] = D[0] + D[1] + ... + D[i]` (prefix sum of D = original array)

This enables: range update in O(1), query in O(n) — the dual of prefix sum.

**Duality**:
| Structure | Build | Range Query | Range Update |
|-----------|-------|-------------|--------------|
| Prefix Sum | O(n) | O(1) | O(n) |
| Difference Array | O(n) | O(n) | O(1) |
| Segment Tree | O(n) | O(log n) | O(log n) |
| Fenwick Tree | O(n) | O(log n) | O(log n) |

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: 1D Prefix Sum
# ─────────────────────────────────────────────────────────────
def build_prefix_sum(arr: list) -> list:
    """
    P[0] = 0 (sentinel — critical for clean range queries)
    P[i] = P[i-1] + arr[i-1]
    sum(arr[l..r]) = P[r+1] - P[l]  (0-indexed arr, l and r inclusive)
    """
    n = len(arr)
    P = [0] * (n + 1)
    for i in range(n):
        P[i+1] = P[i] + arr[i]
    return P

def range_sum(P: list, l: int, r: int) -> int:
    """O(1) range sum query. l and r are 0-indexed, inclusive."""
    return P[r+1] - P[l]

# ─────────────────────────────────────────────────────────────
# Template 2: 2D Prefix Sum
# ─────────────────────────────────────────────────────────────
def build_2d_prefix(matrix: list) -> list:
    """
    P[i][j] = sum of matrix[0..i-1][0..j-1] (1-indexed prefix)
    
    Formula: P[i][j] = P[i-1][j] + P[i][j-1] - P[i-1][j-1] + matrix[i-1][j-1]
    (inclusion-exclusion to avoid double-counting the overlap)
    
    Query sum(r1,c1..r2,c2) = P[r2+1][c2+1] - P[r1][c2+1] - P[r2+1][c1] + P[r1][c1]
    """
    m, n = len(matrix), len(matrix[0])
    P = [[0] * (n+1) for _ in range(m+1)]
    for i in range(1, m+1):
        for j in range(1, n+1):
            P[i][j] = (matrix[i-1][j-1] + P[i-1][j] + P[i][j-1] - P[i-1][j-1])
    return P

def range_sum_2d(P: list, r1: int, c1: int, r2: int, c2: int) -> int:
    """O(1) 2D range sum query. All indices 0-based, inclusive."""
    return P[r2+1][c2+1] - P[r1][c2+1] - P[r2+1][c1] + P[r1][c1]

# ─────────────────────────────────────────────────────────────
# Template 3: Difference Array (1D)
# ─────────────────────────────────────────────────────────────
def build_difference_array(arr: list) -> list:
    """D[i] = arr[i] - arr[i-1], D[0] = arr[0]"""
    n = len(arr)
    D = [0] * n
    D[0] = arr[0]
    for i in range(1, n):
        D[i] = arr[i] - arr[i-1]
    return D

def range_update(D: list, l: int, r: int, val: int) -> None:
    """
    Add val to all elements in arr[l..r] in O(1).
    D[l] += val   (start of range: arr[l..] all increase by val)
    D[r+1] -= val (end of range: cancel the effect after r)
    """
    D[l] += val
    if r + 1 < len(D):
        D[r+1] -= val

def reconstruct(D: list) -> list:
    """Reconstruct array from difference array: prefix sum of D."""
    arr = D[:]
    for i in range(1, len(arr)):
        arr[i] += arr[i-1]
    return arr

# ─────────────────────────────────────────────────────────────
# Template 4: Prefix XOR
# ─────────────────────────────────────────────────────────────
def build_prefix_xor(arr: list) -> list:
    """
    XOR[i] = arr[0] XOR arr[1] XOR ... XOR arr[i-1]
    xor(arr[l..r]) = XOR[r+1] XOR XOR[l]  (a XOR a = 0)
    """
    n = len(arr)
    xor = [0] * (n+1)
    for i in range(n):
        xor[i+1] = xor[i] ^ arr[i]
    return xor
```

---

## Complexity Analysis

| Operation | Prefix Sum | Difference Array | 2D Prefix Sum |
|-----------|------------|------------------|---------------|
| Build | O(n) | O(n) | O(mn) |
| Point Update | O(n) rebuild | O(1) | O(mn) rebuild |
| Range Update | O(n) | O(1) | O(1) with 2D diff |
| Range Query | O(1) | O(n) scan | O(1) |
| Space | O(n) | O(n) | O(mn) |

---

## Classic Problems

### Problem 1: Subarray Sum Equals K — Medium (HashMap Trick)

**Problem**: Count subarrays with sum exactly K.

```python
def subarray_sum_equals_k(nums: list, k: int) -> int:
    """
    Key insight: sum(arr[l..r]) = K ↔ P[r+1] - P[l] = K ↔ P[l] = P[r+1] - K
    
    For each position r, count how many l values have P[l] = P[r+1] - K.
    Store prefix sum counts in a hash map.
    
    Why count[0] = 1 initially:
    This handles subarrays starting from index 0 (P[l=0] = 0 is valid).
    
    Does NOT work for: maximum subarray (Kadane's), positive-only (two-pointer).
    WORKS for: negative numbers, any K, counts subarrays (not max/min length).
    """
    count = {0: 1}   # prefix_sum → frequency
    prefix = 0
    result = 0
    
    for x in nums:
        prefix += x
        # How many previous prefixes equal (prefix - k)?
        result += count.get(prefix - k, 0)
        count[prefix] = count.get(prefix, 0) + 1
    
    return result

# Time: O(n)  Space: O(n)

def subarray_sum_divisible_by_k(nums: list, k: int) -> int:
    """
    Count subarrays with sum divisible by K.
    
    Key: sum(l..r) % K = 0 ↔ P[r+1] % K = P[l] % K
    So count pairs of equal prefix sum moduli.
    
    Python note: negative modulo is always non-negative in Python.
    ((prefix % k) + k) % k handles negative prefix sums correctly.
    """
    remainder_count = {0: 1}
    prefix = 0
    result = 0
    
    for x in nums:
        prefix += x
        rem = prefix % k
        result += remainder_count.get(rem, 0)
        remainder_count[rem] = remainder_count.get(rem, 0) + 1
    
    return result
# Time: O(n)  Space: O(k)
```

### Problem 2: Count Subarrays with XOR Equal to K — Medium

```python
def count_subarrays_xor_k(arr: list, k: int) -> int:
    """
    XOR version of subarray sum = K.
    
    xor(l..r) = K ↔ XOR[r+1] XOR XOR[l] = K ↔ XOR[l] = XOR[r+1] XOR K
    
    (Since a XOR b = c ↔ a = b XOR c for XOR)
    
    Exactly same structure as subarray sum = K, with XOR instead of addition.
    """
    xor_count = {0: 1}
    prefix_xor = 0
    result = 0
    
    for x in arr:
        prefix_xor ^= x
        target = prefix_xor ^ k   # Need XOR[l] = prefix_xor XOR k
        result += xor_count.get(target, 0)
        xor_count[prefix_xor] = xor_count.get(prefix_xor, 0) + 1
    
    return result
# Time: O(n)  Space: O(n)

def count_nice_subarrays(nums: list, k: int) -> int:
    """
    Count subarrays with exactly k odd numbers.
    Transform: replace each num with num%2 (1=odd, 0=even).
    Then count subarrays with sum = k → same as subarray_sum_equals_k.
    """
    return subarray_sum_equals_k([x % 2 for x in nums], k)
# Time: O(n)  Space: O(n)
```

### Problem 3: 2D Prefix Sum — Rectangle Queries — Medium

```python
def num_submatrix_with_target(matrix: list, target: int) -> int:
    """
    Count submatrices with sum = target.
    
    Approach: Fix top and bottom rows, apply 1D subarray sum = target for each column.
    Compress each column into a 1D array using prefix sums.
    
    Time: O(m² × n)  Space: O(n)
    
    For m,n ≤ 100: O(10^6) — fast enough.
    """
    m, n = len(matrix), len(matrix[0])
    result = 0
    
    for r1 in range(m):
        col_sum = [0] * n
        for r2 in range(r1, m):
            # Accumulate column sums for rows r1..r2
            for j in range(n):
                col_sum[j] += matrix[r2][j]
            
            # Count subarrays in col_sum with sum = target
            prefix_count = {0: 1}
            prefix = 0
            for x in col_sum:
                prefix += x
                result += prefix_count.get(prefix - target, 0)
                prefix_count[prefix] = prefix_count.get(prefix, 0) + 1
    
    return result
# Time: O(m²n)  Space: O(n + m)

def max_sum_rectangle(matrix: list) -> int:
    """
    Find the rectangle with maximum sum (generalization of Kadane's to 2D).
    Fix left and right column boundaries, apply 1D Kadane's on column sums.
    Time: O(n²m)  Space: O(m)
    """
    m, n = len(matrix), len(matrix[0])
    best = float('-inf')
    
    for left in range(n):
        row_sum = [0] * m
        for right in range(left, n):
            for i in range(m):
                row_sum[i] += matrix[i][right]
            
            # Kadane's on row_sum
            curr = best_so_far = row_sum[0]
            for x in row_sum[1:]:
                curr = max(x, curr + x)
                best_so_far = max(best_so_far, curr)
            best = max(best, best_so_far)
    
    return best
# Time: O(n²m)  Space: O(m)
```

### Problem 4: Range Update with Difference Array — Car Pooling — Medium

```python
def car_pooling(trips: list, capacity: int) -> bool:
    """
    trips[i] = [num_passengers, from_stop, to_stop]
    Check if car can carry all passengers without exceeding capacity.
    
    Difference array approach:
    - For each trip, add passengers at from_stop, remove at to_stop.
    - Reconstruct with prefix sum to get passengers at each stop.
    - Check if any stop exceeds capacity.
    
    Key: we don't need the final array — just check at each stop.
    """
    MAX_STOP = 1001
    diff = [0] * (MAX_STOP + 1)
    
    for passengers, start, end in trips:
        diff[start] += passengers
        diff[end] -= passengers   # Passengers LEAVE at 'end' stop
    
    # Prefix sum of diff = actual passenger count at each stop
    current = 0
    for passengers_change in diff:
        current += passengers_change
        if current > capacity:
            return False
    
    return True

# Time: O(n + MAX_STOP)  Space: O(MAX_STOP)

def corporate_flight_bookings(bookings: list, n: int) -> list:
    """
    bookings[i] = [first, last, seats] — book 'seats' seats for flights first..last.
    Return total seats booked for each of n flights.
    
    Classic difference array application.
    """
    diff = [0] * (n + 2)   # +2 for 1-indexed + sentinel
    
    for first, last, seats in bookings:
        diff[first] += seats
        diff[last + 1] -= seats
    
    # Prefix sum
    result = []
    current = 0
    for i in range(1, n+1):
        current += diff[i]
        result.append(current)
    
    return result
# Time: O(n + number_of_bookings)  Space: O(n)

def range_addition(n: int, updates: list) -> list:
    """
    Apply updates: each update adds val to arr[lo..hi].
    Return final array. Classic difference array use case.
    """
    diff = [0] * (n + 1)
    for lo, hi, val in updates:
        diff[lo] += val
        diff[hi + 1] -= val
    
    result = []
    current = 0
    for i in range(n):
        current += diff[i]
        result.append(current)
    return result
# Time: O(n + updates)  Space: O(n)
```

### Problem 5: Maximum Circular Subarray Sum — Hard

**Problem**: Find max subarray sum in a circular array (can wrap around).

```python
def max_subarray_sum_circular(nums: list) -> int:
    """
    Two cases:
    Case 1: Max subarray is non-wrapping → standard Kadane's
    Case 2: Max subarray wraps around → total_sum - min_subarray_sum
    
    Why Case 2 works:
    A wrapping subarray = all elements EXCEPT some contiguous middle part.
    Max wrapping sum = total_sum - min_non_wrapping_sum.
    
    Edge case: if all elements negative, Case 2 gives total - total = 0,
    but Case 1 gives the least negative element. Handle by checking if
    max_sum > 0 (if not, all elements negative, return max_sum from Case 1).
    
    Prefix sum insight:
    min_subarray = min over l≤r of (P[r+1] - P[l]) = min_P[r+1] - max_P[l]
    But simpler to use Kadane's variant for min subarray.
    """
    # Case 1: Standard Kadane's for max
    max_sum = curr_max = nums[0]
    # Case 2: Kadane's for min
    min_sum = curr_min = nums[0]
    total = nums[0]
    
    for x in nums[1:]:
        total += x
        curr_max = max(x, curr_max + x)
        max_sum = max(max_sum, curr_max)
        curr_min = min(x, curr_min + x)
        min_sum = min(min_sum, curr_min)
    
    # If max_sum ≤ 0: all elements negative, circular sum = max element = max_sum
    # Otherwise: max of non-circular (max_sum) and circular (total - min_sum)
    if max_sum <= 0:
        return max_sum
    return max(max_sum, total - min_sum)

# Time: O(n)  Space: O(1)

def max_subarray_circular_prefix(nums: list) -> int:
    """
    Prefix sum approach (more explicit, same complexity):
    max circular sum = max over (P[r+1] - P[l]) for any l,r
    where the subarray can wrap, so we extend nums to 2*n.
    But constrain window size ≤ n.
    O(n) with sliding window max on prefix sums.
    """
    n = len(nums)
    extended = nums + nums   # Circular via duplication
    prefix = [0] * (2*n + 1)
    for i in range(2*n):
        prefix[i+1] = prefix[i] + extended[i]
    
    from collections import deque
    dq = deque([0])    # Indices of prefix, monotonically increasing values
    best = float('-inf')
    
    for i in range(1, 2*n + 1):
        # Window: max subarray length = n → remove indices too old
        while dq and dq[0] < i - n:
            dq.popleft()
        # Best ending at i: prefix[i] - min_prefix[i-n..i-1]
        best = max(best, prefix[i] - prefix[dq[0]])
        # Maintain increasing deque (min at front)
        while dq and prefix[dq[-1]] >= prefix[i]:
            dq.pop()
        dq.append(i)
    
    return best
```

### Problem 6: Difference Array for 2D — Bomb Problem — Hard

```python
def max_bombs(grid: list, bombs: list) -> int:
    """
    Each bomb[i] = (r1, c1, r2, c2) — affects rectangle r1..r2, c1..c2.
    After all bombs, find cell with maximum total impact.
    
    2D Difference Array:
    D[r1][c1] += 1
    D[r1][c2+1] -= 1
    D[r2+1][c1] -= 1
    D[r2+1][c2+1] += 1
    Then 2D prefix sum of D gives impact at each cell.
    
    Derivation: Each cell (r,c) should count all bombs where r1≤r≤r2 and c1≤c≤c2.
    The 2D diff trick achieves this in O(bombs + mn).
    """
    m, n = len(grid), len(grid[0])
    D = [[0] * (n+2) for _ in range(m+2)]
    
    for r1, c1, r2, c2 in bombs:
        D[r1][c1] += 1
        D[r1][c2+1] -= 1
        D[r2+1][c1] -= 1
        D[r2+1][c2+1] += 1
    
    # 2D prefix sum of D
    # First: prefix sum along rows
    for r in range(m+2):
        for c in range(1, n+2):
            D[r][c] += D[r][c-1]
    # Then: prefix sum along columns
    for c in range(n+2):
        for r in range(1, m+2):
            D[r][c] += D[r-1][c]
    
    best = 0
    for r in range(m):
        for c in range(n):
            if grid[r][c] == 1:   # Only cells with targets
                best = max(best, D[r][c])
    return best
# Time: O(mn + bombs)  Space: O(mn)
```

### Problem 7: Prefix Sum for String Hashing — Medium

```python
def prefix_hash_matching(text: str, pattern: str) -> list:
    """
    Polynomial rolling hash for pattern matching.
    Prefix hash enables O(1) substring hash comparison.
    
    hash(text[l..r]) = (H[r+1] - H[l] × BASE^(r-l+1)) mod MOD
    
    All occurrences of pattern in text in O(|text| + |pattern|) expected.
    """
    BASE, MOD = 31, (1 << 61) - 1
    
    def build_prefix_hash(s: str):
        n = len(s)
        H = [0] * (n + 1)
        power = [1] * (n + 1)
        for i in range(n):
            H[i+1] = (H[i] * BASE + ord(s[i]) - ord('a') + 1) % MOD
            power[i+1] = power[i] * BASE % MOD
        return H, power
    
    def get_hash(H, power, l, r):
        """Hash of s[l..r] (0-indexed inclusive)."""
        return (H[r+1] - H[l] * power[r-l+1]) % MOD
    
    Ht, pt = build_prefix_hash(text)
    Hp, pp = build_prefix_hash(pattern)
    pat_hash = get_hash(Hp, pp, 0, len(pattern)-1)
    m = len(pattern)
    
    result = []
    for i in range(len(text) - m + 1):
        if get_hash(Ht, pt, i, i+m-1) == pat_hash:
            # Verify to handle hash collisions
            if text[i:i+m] == pattern:
                result.append(i)
    
    return result
# Time: O(|text| + |pattern|) expected  Space: O(|text| + |pattern|)
```

### Problem 8: Continuous Subarray Sum (Multiple of K) — Medium

```python
def check_subarray_sum(nums: list, k: int) -> bool:
    """
    Find subarray of length ≥ 2 with sum multiple of k.
    
    sum(l..r) ≡ 0 (mod k) ↔ P[r+1] ≡ P[l] (mod k)
    Need r - l ≥ 1, i.e., at least 2 elements.
    
    Store FIRST OCCURRENCE of each remainder.
    When same remainder seen again at index r+1 and l was at index ≤ r-1:
    r+1 - l ≥ 2 means the subarray has ≥ 2 elements.
    """
    # Maps remainder → earliest prefix index having that remainder
    first_occurrence = {0: -1}   # P[0]=0, "before" index 0
    prefix = 0
    
    for i, x in enumerate(nums):
        prefix += x
        rem = prefix % k
        
        if rem in first_occurrence:
            if i - first_occurrence[rem] >= 2:
                return True
        else:
            first_occurrence[rem] = i
    
    return False
# Time: O(n)  Space: O(k)
```

---

## Advanced Variations

### Prefix Sum on Trees (Path Queries)

```python
def path_sum_equals_k(root, k: int) -> int:
    """
    Count paths in binary tree with sum = k.
    Prefix sum on tree path from root.
    
    DFS with running prefix sum + hashmap → O(n) time.
    (Extension of subarray sum = k to trees)
    """
    from collections import defaultdict
    
    count = defaultdict(int)
    count[0] = 1
    result = [0]
    
    def dfs(node, prefix):
        if not node: return
        prefix += node.val
        result[0] += count[prefix - k]
        count[prefix] += 1
        dfs(node.left, prefix)
        dfs(node.right, prefix)
        count[prefix] -= 1   # Backtrack: remove current path's prefix
    
    dfs(root, 0)
    return result[0]
# Time: O(n)  Space: O(n) — hash map stores one entry per node in worst case
```

### Sparse Table for Range Minimum Query (O(1) Query After O(n log n) Build)

```python
import math

def build_sparse_table(arr: list) -> list:
    """
    Sparse table for Range Minimum Query (RMQ).
    Idempotent operations (min, max, gcd) allow O(1) queries after O(n log n) build.
    
    ST[k][i] = min(arr[i..i+2^k-1])
    
    Build: ST[0][i] = arr[i]
           ST[k][i] = min(ST[k-1][i], ST[k-1][i+2^(k-1)])
    
    Query(l,r): k = floor(log2(r-l+1))
                min(ST[k][l], ST[k][r-2^k+1])
    The two overlapping intervals of size 2^k cover [l,r] completely.
    Overlap is fine for min (idempotent: min(a,a) = a).
    
    WHY this works: min(arr[l..r]) = min(any two sub-intervals covering [l,r])
    """
    n = len(arr)
    LOG = max(1, int(math.log2(n)) + 1)
    ST = [[float('inf')] * n for _ in range(LOG)]
    ST[0] = arr[:]
    
    for k in range(1, LOG):
        for i in range(n - (1 << k) + 1):
            ST[k][i] = min(ST[k-1][i], ST[k-1][i + (1 << (k-1))])
    
    return ST

def rmq(ST: list, l: int, r: int) -> int:
    """O(1) range minimum query."""
    k = int(math.log2(r - l + 1))
    return min(ST[k][l], ST[k][r - (1 << k) + 1])

# Build: O(n log n)  Query: O(1)  Space: O(n log n)
# Use when: many queries (>> n), static array, idempotent operation
```

---

## Edge Cases Bible

1. **Off-by-one in prefix sum**: P[0] = 0 (empty prefix). sum(arr[l..r]) = P[r+1] - P[l]. Common bug: using P[r] - P[l-1] with 0-indexed P where P[0] = arr[0] — this misses P[-1].

2. **2D prefix sum formula**: The inclusion-exclusion has 4 terms. Missing the +P[i-1][j-1] (adding back the doubly-subtracted corner) is the most common 2D prefix bug.

3. **Difference array boundary**: D[r+1] -= val requires r+1 ≤ n. Always allocate diff array of size n+1 (or n+2 for safety) to avoid out-of-bounds.

4. **Modular arithmetic in subarray sum divisible by K**: In Python, `(-1) % 5 = 4` (always non-negative), so modulo works correctly. In C++/Java, use `((prefix % k) + k) % k`.

5. **Circular subarray edge case**: When all elements are negative, max circular sum = max single element (Case 1). The formula `total - min_sum` would give 0 (total = min_sum for all-negative), which is wrong.

6. **Prefix XOR vs prefix sum**: XOR is its own inverse (a XOR a = 0). Sum needs subtraction. Never mix these operations.

7. **Subarray sum = K with consecutive subarray size constraint**: "Length ≥ 2" requires tracking index of first occurrence, not frequency — first_occurrence[rem] stores the EARLIEST index.

8. **2D difference array and query order**: Apply row prefix sum BEFORE column prefix sum (or vice versa, but consistently). Wrong order gives wrong results.

9. **Sparse table for non-idempotent operations**: Sparse table ONLY works for idempotent operations (min, max, gcd). For sum queries, use prefix sum (O(1)) or Fenwick tree (O(log n) for updates).

10. **Path sum in tree vs array**: Tree DFS backtracking requires decrementing the hash map count after visiting both children — forgetting this causes prefix sums from other root-to-node paths to bleed into the count.

---

## Interview Tips

### What Interviewers Look For

1. **Immediately recognize the "prefix sum + hash map" pattern**: When you see "count subarrays with sum/XOR/divisibility condition," say "this is prefix sum + hash map, O(n) solution."

2. **Explain the key identity clearly**: "sum(l..r) = P[r+1] - P[l], so we need P[l] = P[r+1] - K. For each r, look up P[r+1]-K in our hash map."

3. **The P[0]=0 sentinel**: Always explain why you initialize count = {0:1} or first_occurrence = {0:-1}. "This handles subarrays starting from index 0, where the prefix before index 0 is 0."

4. **Difference array vs prefix sum**: "Difference array is the dual — O(1) range update, O(n) to reconstruct. Use when you have many updates followed by one final read."

5. **2D prefix sum for matrix problems**: When seeing "count submatrices with property X" or "sum of rectangle," immediately say "2D prefix sum + enumerate two row boundaries."

6. **Circular array trick**: "Total sum minus minimum subarray sum gives maximum circular subarray sum. Edge case: all negative — return max single element."

7. **Common follow-up**: "Can you do this with O(1) space?" For count-based problems (not max/min), usually no — the hash map is inherent. For max/min, often Kadane's gives O(1) space.

8. **String hashing as prefix sum**: Polynomial hash behaves exactly like prefix sum but with multiplication instead of addition. Useful for substring comparison in O(1) after O(n) preprocessing.

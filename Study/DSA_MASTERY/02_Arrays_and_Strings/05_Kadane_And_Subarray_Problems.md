# Kadane's Algorithm & Subarray Problems — Mastery Guide

## Core Concept & Invariant

Kadane's algorithm solves **maximum subarray sum** in O(n) time, O(1) space via
a local/global running maximum.

**The fundamental invariant**:
```
curr[i] = maximum sum subarray ending exactly at index i
        = max(nums[i], curr[i-1] + nums[i])
```

**Proof of correctness**: 
- If `curr[i-1] > 0`: extending the previous subarray improves the sum → include it.
- If `curr[i-1] ≤ 0`: starting fresh at nums[i] is better → discard previous.
- This greedy choice is optimal because we never need to "restart" at an earlier index once we've decided to drop the previous subarray.

**Generalization principle**: Kadane's underlies many advanced problems:
- Product subarray → track min and max simultaneously
- Circular subarray → total_sum - min_subarray
- K-concatenation → use prefix/suffix decomposition
- Non-adjacent elements → separate DP formulation

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Standard Kadane's
# ─────────────────────────────────────────────────────────────
def max_subarray(nums: list) -> int:
    """
    curr = max sum ending at current position
    best = global maximum
    """
    curr = best = nums[0]
    for x in nums[1:]:
        curr = max(x, curr + x)
        best = max(best, curr)
    return best

# ─────────────────────────────────────────────────────────────
# Template 2: Kadane's with Indices Tracking
# ─────────────────────────────────────────────────────────────
def max_subarray_with_indices(nums: list) -> tuple:
    """Returns (max_sum, start_index, end_index)."""
    curr = best = nums[0]
    curr_start = best_start = best_end = 0
    
    for i in range(1, len(nums)):
        if curr + nums[i] >= nums[i]:
            curr += nums[i]
        else:
            curr = nums[i]
            curr_start = i    # New subarray starts here
        
        if curr > best:
            best = curr
            best_start = curr_start
            best_end = i
    
    return best, best_start, best_end

# ─────────────────────────────────────────────────────────────
# Template 3: Kadane's for Minimum Subarray
# ─────────────────────────────────────────────────────────────
def min_subarray(nums: list) -> int:
    """Symmetric: track minimum instead of maximum."""
    curr = worst = nums[0]
    for x in nums[1:]:
        curr = min(x, curr + x)
        worst = min(worst, curr)
    return worst
```

---

## Complexity Analysis

| Problem | Time | Space | Key Insight |
|---------|------|-------|-------------|
| Max sum subarray | O(n) | O(1) | Kadane's |
| Max product subarray | O(n) | O(1) | Track min and max |
| Max circular subarray | O(n) | O(1) | Total - min_subarray |
| Max subarray one deletion | O(n) | O(1) | Forward/backward DP |
| Non-adjacent max sum | O(n) | O(1) | House robber DP |
| K-concatenation max sum | O(n) | O(1) | Prefix/suffix + middle |
| Max length subarray sum K | O(n) | O(n) | Prefix sum + hash |
| Count subarrays sum > K | O(n log n) | O(n) | Merge sort / BIT |

---

## Classic Problems

### Problem 1: Maximum Sum Subarray — Kadane's Formal Proof — Medium

```python
def max_subarray_proof(nums: list) -> int:
    """
    Formal correctness proof:
    
    Claim: curr[i] = max{sum(nums[l..i]) : 0 ≤ l ≤ i}
    
    Base: curr[0] = nums[0] = max sum ending at index 0. ✓
    
    Inductive step: Assume curr[i-1] = max sum of subarray ending at i-1.
    
    Any subarray ending at i either:
    (A) Consists of just nums[i] → sum = nums[i]
    (B) Extends a subarray ending at i-1 → sum = curr[i-1] + nums[i]
        (We choose the BEST subarray ending at i-1, which is curr[i-1])
    
    Therefore: curr[i] = max(nums[i], curr[i-1] + nums[i]) ✓
    
    Global maximum: best = max(curr[0], ..., curr[n-1]) ✓
    
    Time: O(n)  Space: O(1)
    """
    if not nums:
        return 0
    
    curr = best = nums[0]
    for x in nums[1:]:
        curr = max(x, curr + x)
        best = max(best, curr)
    return best

def max_subarray_divide_conquer(nums: list, lo: int, hi: int) -> int:
    """
    Divide & conquer O(n log n) approach.
    Demonstrates WHY Kadane's is preferable (same result, worse complexity).
    
    The cross-subarray (spanning mid) requires O(n) per merge step → T(n) = 2T(n/2) + O(n) = O(n log n).
    """
    if lo == hi:
        return nums[lo]
    
    mid = (lo + hi) // 2
    
    # Max subarray in left half
    left_max = max_subarray_divide_conquer(nums, lo, mid)
    # Max subarray in right half
    right_max = max_subarray_divide_conquer(nums, mid+1, hi)
    
    # Max subarray crossing mid
    left_sum = float('-inf')
    running = 0
    for i in range(mid, lo-1, -1):
        running += nums[i]
        left_sum = max(left_sum, running)
    
    right_sum = float('-inf')
    running = 0
    for i in range(mid+1, hi+1):
        running += nums[i]
        right_sum = max(right_sum, running)
    
    cross_max = left_sum + right_sum
    return max(left_max, right_max, cross_max)
# Time: O(n log n)  Space: O(log n) stack — WORSE than Kadane's O(n)/O(1)
```

### Problem 2: Maximum Product Subarray — Medium-Hard

**Problem**: Find subarray with maximum product (array may have negative numbers and zeros).

```python
def max_product(nums: list) -> int:
    """
    Cannot use Kadane's directly because:
    - Multiplying by a negative flips max ↔ min
    - Two negatives make a positive (large negative × large negative = large positive)
    - Zero resets the product
    
    Solution: Track BOTH current max AND current min product ending at position i.
    
    max_prod[i] = max of:
      - nums[i] alone (start fresh)
      - max_prod[i-1] × nums[i] (extend positive streak)
      - min_prod[i-1] × nums[i] (two negatives → large positive)
    
    min_prod[i] = min of:
      - nums[i] alone
      - max_prod[i-1] × nums[i] (positive × negative → very negative)
      - min_prod[i-1] × nums[i] (extend negative streak)
    
    Key: must use PREVIOUS max/min (before update), so store them first.
    """
    if not nums:
        return 0
    
    curr_max = curr_min = best = nums[0]
    
    for x in nums[1:]:
        # Must use prev values — save before overwriting
        prev_max, prev_min = curr_max, curr_min
        
        curr_max = max(x, prev_max * x, prev_min * x)
        curr_min = min(x, prev_max * x, prev_min * x)
        best = max(best, curr_max)
    
    return best

# Time: O(n)  Space: O(1)

def max_product_alternative(nums: list) -> int:
    """
    Alternative: prefix products from left and right.
    
    Key insight: if we could ignore zeros, the max product subarray
    must include either the first element or the last element of some
    contiguous segment between zeros.
    
    Scanning left-to-right and right-to-left with running product,
    reset on zero (treat as new segment). The maximum seen is the answer.
    
    Why this works: any subarray with max product either has all positive prefixes
    from the left or all from the right. The negatives cancel in pairs.
    Traversing both directions catches all cases.
    """
    n = len(nums)
    best = max(nums)   # At minimum, single element
    
    # Left to right
    prod = 1
    for x in nums:
        prod *= x
        best = max(best, prod)
        if prod == 0: prod = 1   # Reset at zero
    
    # Right to left
    prod = 1
    for x in reversed(nums):
        prod *= x
        best = max(best, prod)
        if prod == 0: prod = 1
    
    return best
```

### Problem 3: Maximum Subarray with At Most One Deletion — Hard

**Problem**: Delete at most one element; maximize subarray sum.

```python
def maximum_sum_with_deletion(arr: list) -> int:
    """
    Two DP arrays:
    fwd[i] = max sum of subarray ending at i (standard Kadane's)
    bwd[i] = max sum of subarray starting at i (Kadane's from right)
    
    Answer is max of:
    1. No deletion: max(fwd[i]) — standard max subarray
    2. Delete element i: fwd[i-1] + bwd[i+1] — best left + best right avoiding i
    
    Time: O(n)  Space: O(n)
    """
    n = len(arr)
    fwd = arr[:]   # fwd[i] = max subarray sum ending at i
    bwd = arr[:]   # bwd[i] = max subarray sum starting at i
    
    for i in range(1, n):
        fwd[i] = max(arr[i], fwd[i-1] + arr[i])
    
    for i in range(n-2, -1, -1):
        bwd[i] = max(arr[i], bwd[i+1] + arr[i])
    
    best = max(fwd)  # Case: no deletion
    
    for i in range(1, n-1):   # Delete element i
        best = max(best, fwd[i-1] + bwd[i+1])
    
    return best

# Time: O(n)  Space: O(n) — can optimize to O(1) with running max tracking

def maximum_sum_with_deletion_o1(arr: list) -> int:
    """
    O(1) space version.
    
    Keep track of:
    - curr_no_del: max subarray sum ending at i with no deletion
    - curr_one_del: max subarray sum ending at i with exactly one deletion used
    
    Transitions:
    curr_no_del[i] = max(arr[i], curr_no_del[i-1] + arr[i])
    curr_one_del[i] = max(
        curr_no_del[i-1],           # Delete arr[i] (the element just "deleted")
        curr_one_del[i-1] + arr[i]  # Keep arr[i], deletion was earlier
    )
    """
    if len(arr) == 1:
        return arr[0]
    
    no_del = arr[0]
    one_del = float('-inf')
    best = arr[0]
    
    for x in arr[1:]:
        one_del = max(no_del, one_del + x)   # Delete x OR extend with deletion used
        no_del = max(x, no_del + x)           # Standard Kadane
        best = max(best, no_del, one_del)
    
    return best
# Time: O(n)  Space: O(1)
```

### Problem 4: Maximum Circular Subarray Sum — Hard

```python
def max_subarray_circular(nums: list) -> int:
    """
    Two cases for circular max:
    
    Case 1: Non-wrapping subarray → standard Kadane's
    Case 2: Wrapping subarray = complement of a non-wrapping subarray
            = total_sum - min_non_wrapping_subarray
    
    Proof of Case 2:
    A wrapping subarray uses nums[k..n-1] and nums[0..j] for some 0 ≤ j < k.
    Equivalently, it EXCLUDES nums[j+1..k-1] (a contiguous middle portion).
    So: max wrapping sum = total_sum - min(nums[j+1..k-1])
                         = total_sum - min_subarray_sum
    
    Edge case: if all elements are negative:
    - Case 2 gives: total - min = 0 (impossible — can't have empty subarray)
    - Case 2 would give 0 for all-negative array (total = min_subarray)
    - So answer must come from Case 1 (least negative single element)
    - Check: if max_sum (Case 1) ≤ 0, return max_sum
    """
    total = sum(nums)
    max_sum = min_sum = nums[0]
    curr_max = curr_min = nums[0]
    
    for x in nums[1:]:
        curr_max = max(x, curr_max + x)
        max_sum = max(max_sum, curr_max)
        curr_min = min(x, curr_min + x)
        min_sum = min(min_sum, curr_min)
    
    # If max_sum ≤ 0: all elements negative → best is single element (max_sum)
    return max(max_sum, total - min_sum) if max_sum > 0 else max_sum

# Time: O(n)  Space: O(1)
```

### Problem 5: Maximum Sum of Non-Adjacent Elements (House Robber) — Medium

```python
def rob(nums: list) -> int:
    """
    Cannot take adjacent elements. Max sum of non-adjacent subset.
    
    DP: dp[i] = max sum using elements from nums[0..i]
    dp[i] = max(dp[i-1],          # Skip nums[i]
                dp[i-2] + nums[i]) # Take nums[i] (must skip i-1)
    
    Kadane's-style: only need last two dp values.
    
    Generalization: "cooldown" problems use similar DP.
    """
    if not nums: return 0
    if len(nums) == 1: return nums[0]
    
    prev2 = nums[0]
    prev1 = max(nums[0], nums[1])
    
    for x in nums[2:]:
        curr = max(prev1, prev2 + x)
        prev2, prev1 = prev1, curr
    
    return prev1

# Time: O(n)  Space: O(1)

def rob_circular(nums: list) -> int:
    """
    Circular variant: first and last are adjacent.
    
    Reduce to two linear House Robber problems:
    - Problem 1: nums[0..n-2] (exclude last)
    - Problem 2: nums[1..n-1] (exclude first)
    Answer = max of both.
    
    Proof: the optimal solution either includes nums[0] or not.
    If it includes nums[0], it cannot include nums[n-1] → solve nums[0..n-2].
    If it doesn't include nums[0], solve nums[1..n-1].
    """
    def rob_linear(arr: list) -> int:
        if not arr: return 0
        if len(arr) == 1: return arr[0]
        prev2 = arr[0]
        prev1 = max(arr[0], arr[1])
        for x in arr[2:]:
            prev2, prev1 = prev1, max(prev1, prev2 + x)
        return prev1
    
    n = len(nums)
    if n == 1: return nums[0]
    return max(rob_linear(nums[:-1]), rob_linear(nums[1:]))
# Time: O(n)  Space: O(1)
```

### Problem 6: K-Concatenation Maximum Sum — Hard

**Problem**: Given arr and k (k copies concatenated), find max subarray sum.

```python
def k_concatenation_max_sum(arr: list, k: int) -> int:
    """
    Three cases based on total sum and k:
    
    Case k=1: standard Kadane's on arr
    Case k≥2 and sum(arr) ≤ 0: max is within first two copies
                                (adding more copies doesn't help — sum is non-positive)
    Case k≥2 and sum(arr) > 0:  the middle copies contribute sum(arr) each
                                max = prefix_max(arr) + suffix_max(arr) + (k-2)*sum(arr)
    
    Where:
    prefix_max(arr) = max subarray sum starting from index 0
    suffix_max(arr) = max subarray sum ending at index n-1
    
    Why prefix_max + suffix_max + (k-2)*sum(arr):
    The optimal wrapping subarray uses:
    - A suffix of the first copy
    - All of the (k-2) middle copies
    - A prefix of the last copy
    
    Equivalently: suffix_max(first) + (k-2)*sum + prefix_max(last)
    Since all copies are identical: suffix_max(arr) + (k-2)*sum(arr) + prefix_max(arr)
    """
    MOD = 10**9 + 7
    n = len(arr)
    total = sum(arr)
    
    def kadane(a: list) -> int:
        curr = best = a[0]
        for x in a[1:]:
            curr = max(x, curr + x)
            best = max(best, curr)
        return best
    
    def max_prefix(a: list) -> int:
        """Max sum subarray starting from index 0."""
        best = curr = 0
        for x in a:
            curr += x
            best = max(best, curr)
        return best
    
    def max_suffix(a: list) -> int:
        """Max sum subarray ending at index n-1."""
        best = curr = 0
        for x in reversed(a):
            curr += x
            best = max(best, curr)
        return best
    
    if k == 1:
        return max(0, kadane(arr)) % MOD
    
    if total > 0:
        # Wrapping subarray spans multiple copies
        ans = max_suffix(arr) + (k - 2) * total + max_prefix(arr)
    else:
        # Can only benefit from at most 2 copies
        ans = kadane(arr * 2)   # Only need 2 copies max
    
    return max(0, ans) % MOD

# Time: O(n)  Space: O(1) (not counting the copy for k=1, sum computation)
```

### Problem 7: Maximum Length Subarray with Sum K — Medium

```python
def max_length_subarray_sum_k(nums: list, k: int) -> int:
    """
    Prefix sum + hash map. For each r, find EARLIEST l with P[l] = P[r+1] - k.
    
    Store FIRST occurrence of each prefix sum (unlike count which stores all).
    This gives the maximum length subarray.
    
    Works for negative numbers too (unlike two-pointer which requires positives).
    """
    first_seen = {0: -1}   # prefix_sum → first index (before array = -1)
    prefix = 0
    best = 0
    
    for i, x in enumerate(nums):
        prefix += x
        target = prefix - k
        
        if target in first_seen:
            length = i - first_seen[target]
            best = max(best, length)
        
        # Only store FIRST occurrence (to maximize length)
        if prefix not in first_seen:
            first_seen[prefix] = i
    
    return best

# Time: O(n)  Space: O(n)

def max_length_subarray_sum_k_nonneg(nums: list, k: int) -> int:
    """
    O(1) space alternative for non-negative arrays using two pointers.
    Monotonic: adding elements increases sum, removing decreases.
    """
    lo = 0
    window_sum = 0
    best = 0
    
    for hi in range(len(nums)):
        window_sum += nums[hi]
        while window_sum > k and lo <= hi:
            window_sum -= nums[lo]
            lo += 1
        if window_sum == k:
            best = max(best, hi - lo + 1)
    
    return best
# Time: O(n)  Space: O(1) — only for non-negative arrays
```

---

## Advanced Variations

### Maximum Sum Submatrix (2D Kadane's)

```python
def max_sum_submatrix(matrix: list) -> int:
    """
    Extend Kadane's to 2D: O(n²m) time.
    Fix left and right column boundaries, apply 1D Kadane on compressed row sums.
    """
    m, n = len(matrix), len(matrix[0])
    best = float('-inf')
    
    for left in range(n):
        row_sum = [0] * m
        for right in range(left, n):
            for i in range(m):
                row_sum[i] += matrix[i][right]
            
            # 1D Kadane on row_sum
            curr = max_so_far = row_sum[0]
            for x in row_sum[1:]:
                curr = max(x, curr + x)
                max_so_far = max(max_so_far, curr)
            best = max(best, max_so_far)
    
    return best
# Time: O(n²m)  Space: O(m)
```

### Maximum Sum Increasing Subsequence

```python
def max_sum_increasing_subsequence(arr: list) -> int:
    """
    Find subsequence (not necessarily contiguous) that is strictly increasing
    with maximum sum.
    
    DP: dp[i] = max sum increasing subsequence ending at arr[i]
    dp[i] = arr[i] + max(dp[j] for j < i if arr[j] < arr[i])
    
    O(n²) DP — can optimize to O(n log n) with Fenwick tree on values.
    """
    n = len(arr)
    dp = arr[:]   # At minimum, take just arr[i]
    
    for i in range(1, n):
        for j in range(i):
            if arr[j] < arr[i]:
                dp[i] = max(dp[i], dp[j] + arr[i])
    
    return max(dp)
# Time: O(n²)  Space: O(n)
```

### Count Subarrays with Sum Greater Than K

```python
def count_subarrays_sum_greater_k(nums: list, k: int) -> int:
    """
    Count subarrays where sum > k.
    
    Approach: merge sort on prefix sums.
    sum(l..r) = P[r+1] - P[l] > k ↔ P[r+1] - P[l] > k ↔ P[r+1] > P[l] + k
    
    For each r+1, count l values with P[l] < P[r+1] - k.
    This is a "count inversions" type problem → merge sort.
    
    Alternative O(n log n) via Fenwick tree on compressed prefix sums.
    """
    n = len(nums)
    prefix = [0] * (n + 1)
    for i, x in enumerate(nums):
        prefix[i+1] = prefix[i] + x
    
    result = [0]
    
    def merge_count(arr: list, temp: list, lo: int, hi: int) -> None:
        if hi - lo <= 1:
            return
        mid = (lo + hi) // 2
        merge_count(arr, temp, lo, mid)
        merge_count(arr, temp, mid, hi)
        
        # Count: for each r in [mid, hi), count l in [lo, mid) with arr[l] < arr[r] - k
        j = lo
        for r in range(mid, hi):
            while j < mid and arr[j] < arr[r] - k:
                j += 1
            result[0] += j - lo
        
        # Standard merge sort
        i, p, q = lo, lo, mid
        while p < mid and q < hi:
            if arr[p] <= arr[q]:
                temp[i] = arr[p]; p += 1
            else:
                temp[i] = arr[q]; q += 1
            i += 1
        while p < mid: temp[i] = arr[p]; p += 1; i += 1
        while q < hi:  temp[i] = arr[q]; q += 1; i += 1
        arr[lo:hi] = temp[lo:hi]
    
    temp = prefix[:]
    merge_count(prefix, temp, 0, n+1)
    return result[0]
# Time: O(n log n)  Space: O(n)
```

---

## Edge Cases Bible

1. **All negative numbers**: Kadane's correctly returns the least-negative element (max single element). Never return 0 for this problem — empty subarray is not allowed.

2. **Single element array**: Both max and product subarrays return nums[0]. Handle before the loop to avoid index errors.

3. **Product with zero**: Zero resets the product. The max product subarray might be on one side of the zero. The two-pass (left/right product) approach handles this by resetting prod=1 at zero.

4. **Product with odd number of negatives**: One negative will always be "left out" of the max product subarray — the two-pass approach naturally handles this.

5. **Circular array — all same sign**: If all positive, Case 2 (total - min_subarray) should give the full array sum (min_subarray = 0, invalid). But if we allow empty subarray for min, we get total - 0 = total. Careful: the min subarray must be NON-EMPTY. However, since all elements positive, min_subarray = min single element, and total - that is still valid.

6. **K-concatenation with k=1**: No wrapping possible. Just standard Kadane's.

7. **Deletion subarray — array of length 1**: Cannot delete the only element and have a non-empty subarray. Return nums[0].

8. **House Robber with k=2 elements**: dp[0] = nums[0], dp[1] = max(nums[0], nums[1]). Make sure to handle len(nums) <= 2 before the loop.

9. **Maximum length subarray sum K with negative numbers**: Two-pointer won't work. Must use prefix sum + hash map. Forgetting this leads to incorrect O(n) solutions that fail on negative-number inputs.

10. **Subarray sum count > K vs ≥ K**: The strict vs non-strict inequality changes the merge sort counting — verify which comparator to use.

---

## Interview Tips

### What Interviewers Look For

1. **State Kadane's recurrence immediately**: "dp[i] = max(nums[i], dp[i-1] + nums[i]). If extending is worse than starting fresh, start fresh." Don't just code it — explain the recurrence.

2. **Maximum PRODUCT insight**: "Unlike sum, a large negative × large negative = large positive. So I must track both the minimum and maximum product ending at each position." This is the key distinguishing insight interviewers test.

3. **Circular array — the complement trick**: "A circular subarray excludes a contiguous middle portion. So max circular = total - min non-circular subarray. I run Kadane's for both max and min simultaneously."

4. **K-concatenation analysis**: "For k copies, if the array sum is positive, each full middle copy adds to the answer. So I compute prefix_max + (k-2)×total + suffix_max for the cross-copy wrapping."

5. **Non-adjacent = House Robber**: This is one of the most important DP patterns. Always know it cold. The O(1) space rolling variable approach: "at each step, I either take current + prev2 or skip (take prev1)."

6. **Don't confuse subarray with subsequence**: "Subarray = contiguous (use Kadane's). Subsequence = can skip (use LIS-style DP)." Clarify this with the interviewer immediately.

7. **When to use prefix sum vs Kadane's**:
   - COUNT subarrays with sum = K → prefix sum + hash map (O(n))
   - MAXIMUM sum subarray → Kadane's (O(n))
   - MAXIMUM LENGTH subarray with sum = K → prefix sum + hash map (O(n))
   - MAXIMUM sum with constraints (circular, deletion) → Kadane's variants

8. **Follow-up on complexity**: Always mention "this is O(n) time, O(1) space — provably optimal since we must read every element at least once (Ω(n) lower bound)."

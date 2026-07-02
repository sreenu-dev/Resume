# Two Pointers — Mastery Guide

## Core Concept & Invariant

Two pointers maintain a **loop invariant** about the relationship between pointer positions.
The correctness proof always reduces to: "the pointer we move cannot possibly contribute to a better answer."

**Three canonical configurations**:
1. **Opposite-direction** (lo/hi converging): Start at both ends, move toward each other
2. **Same-direction** (fast/slow or sliding): Both start at left, different speeds
3. **Multi-pointer**: Three or more pointers (3-sum, Dutch National Flag)

**The fundamental correctness argument** (for opposite-direction):
> At any state (lo, hi), if we've proven that ALL pairs involving arr[lo] with indices < lo are suboptimal,
> AND all pairs involving arr[hi] with indices > hi are suboptimal,
> THEN we can safely move whichever pointer we can prove contributes no further optimal answer.

**Invariant template**:
```
Invariant I(lo, hi): The optimal answer (if it exists) uses an index in [lo, hi].
Termination: lo < hi (or lo ≤ hi depending on problem)
Progress: Each step strictly decreases (hi - lo), so algorithm terminates.
```

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Opposite-direction (sorted array, pair problem)
# ─────────────────────────────────────────────────────────────
def two_pointer_opposite(arr: list, target: int):
    """
    Find pair summing to target in sorted array.
    Invariant: if answer exists, it's within [lo, hi].
    """
    lo, hi = 0, len(arr) - 1
    while lo < hi:
        s = arr[lo] + arr[hi]
        if s == target:
            return (lo, hi)
        elif s < target:
            lo += 1    # arr[lo] too small — no pair with arr[lo] can reach target now
        else:
            hi -= 1    # arr[hi] too large — no pair with arr[hi] can reach target now
    return None

# ─────────────────────────────────────────────────────────────
# Template 2: Same-direction (fast/slow for array partition)
# ─────────────────────────────────────────────────────────────
def partition_by_condition(arr: list, condition) -> int:
    """
    Partition: elements satisfying condition go to front.
    Returns the boundary index.
    slow: boundary of "good" elements seen so far
    fast: explores new elements
    """
    slow = 0
    for fast in range(len(arr)):
        if condition(arr[fast]):
            arr[slow], arr[fast] = arr[fast], arr[slow]
            slow += 1
    return slow   # arr[:slow] satisfies condition

# ─────────────────────────────────────────────────────────────
# Template 3: Three pointers (Dutch National Flag)
# ─────────────────────────────────────────────────────────────
def dutch_national_flag(arr: list, pivot: int) -> None:
    """
    Partition into < pivot, == pivot, > pivot in O(n) time, O(1) space.
    Invariant:
    - arr[0..lo-1]  < pivot
    - arr[lo..mid-1] == pivot  
    - arr[mid..hi]  unknown
    - arr[hi+1..n-1] > pivot
    """
    lo, mid, hi = 0, 0, len(arr) - 1
    while mid <= hi:
        if arr[mid] < pivot:
            arr[lo], arr[mid] = arr[mid], arr[lo]
            lo += 1; mid += 1
        elif arr[mid] == pivot:
            mid += 1
        else:
            arr[mid], arr[hi] = arr[hi], arr[mid]
            hi -= 1
        # Note: mid NOT incremented after swap with hi — arr[hi] unknown
```

---

## Formal Correctness Proof: Opposite-Direction Two-Pointer

For the sorted array pair-sum problem (find i < j such that arr[i] + arr[j] = target):

**Theorem**: The two-pointer algorithm finds a pair if one exists.

**Proof by loop invariant**:
- **Invariant I(lo, hi)**: If a valid pair (i, j) with i < j exists, then lo ≤ i and j ≤ hi.
- **Base**: I(0, n−1) trivially true.
- **Maintenance**:
  - If arr[lo] + arr[hi] < target: We move lo → lo+1.
    - Any pair (lo, k) for k ≤ hi gives arr[lo] + arr[k] ≤ arr[lo] + arr[hi] < target.
    - So lo cannot be part of any valid pair. Eliminating lo preserves I.
  - If arr[lo] + arr[hi] > target: We move hi → hi-1.
    - Any pair (k, hi) for k ≥ lo gives arr[k] + arr[hi] ≥ arr[lo] + arr[hi] > target.
    - So hi cannot be part of any valid pair. Eliminating hi preserves I.
- **Termination**: lo < hi strictly decreases each step → terminates in O(n). ∎

---

## Complexity Analysis

| Pattern | Time | Space | Notes |
|---------|------|-------|-------|
| Opposite-direction pair | O(n) | O(1) | Requires sorted array |
| Partition (fast/slow) | O(n) | O(1) | In-place |
| Dutch National Flag | O(n) | O(1) | Exactly 1 pass |
| 3-Sum | O(n²) | O(1) | Sort + two-pointer per element |
| K-Sum (K fixed) | O(n^(K-1)) | O(K) recursion | Reduce to (K-1)-sum |
| Trapping rain water | O(n) | O(1) | Two-pointer proof crucial |

---

## Classic Problems

### Problem 1: Container With Most Water — Medium

**Problem**: Given heights array h, find two lines maximizing min(h[l], h[r]) × (r − l).

```python
def max_area(height: list) -> int:
    """
    Correctness proof: when moving the shorter pointer, we're not missing anything.
    
    At state (lo, hi), current area = min(h[lo], h[hi]) × (hi - lo).
    Say h[lo] ≤ h[hi] (lo is the bottleneck).
    
    For ANY j < hi:
        area(lo, j) = min(h[lo], h[j]) × (j - lo) ≤ h[lo] × (j - lo) < h[lo] × (hi - lo)
    
    So keeping lo and trying smaller j CANNOT improve area.
    → Move lo (the shorter pointer). ✓
    """
    lo, hi = 0, len(height) - 1
    best = 0
    
    while lo < hi:
        area = min(height[lo], height[hi]) * (hi - lo)
        best = max(best, area)
        
        if height[lo] <= height[hi]:
            lo += 1   # Proven: lo can't improve by pairing with anything left of hi
        else:
            hi -= 1   # Symmetric argument
    
    return best

# Time: O(n)  Space: O(1)
# Edge cases: n=2 (single comparison), all equal heights, strictly increasing/decreasing
```

### Problem 2: Trapping Rain Water — Hard

**Problem**: Given elevation map, compute total trapped water.

```python
def trap(height: list) -> int:
    """
    Key insight: water at position i = min(max_left[i], max_right[i]) - height[i]
    
    Two-pointer approach avoids O(n) prefix/suffix arrays.
    
    Invariant: 
    - left_max = max(height[0..lo])
    - right_max = max(height[hi..n-1])
    
    If left_max ≤ right_max:
        water at lo = left_max - height[lo]  (right side is at least left_max tall)
        This is EXACT because we know right_max ≥ left_max, so right_max doesn't constrain.
    Else:
        water at hi = right_max - height[hi]
    
    Proof: When left_max ≤ right_max and we compute water[lo] = left_max - h[lo]:
    - The actual water is min(max_left[lo], max_right[lo]) - h[lo]
    - max_left[lo] = left_max (we've scanned left side completely)
    - max_right[lo] ≥ right_max ≥ left_max
    - So min(...) = left_max → our formula is correct. ✓
    """
    if not height: return 0
    lo, hi = 0, len(height) - 1
    left_max = right_max = 0
    water = 0
    
    while lo < hi:
        if height[lo] <= height[hi]:
            if height[lo] >= left_max:
                left_max = height[lo]
            else:
                water += left_max - height[lo]
            lo += 1
        else:
            if height[hi] >= right_max:
                right_max = height[hi]
            else:
                water += right_max - height[hi]
            hi -= 1
    
    return water

# Time: O(n)  Space: O(1)
# Comparison: stack approach O(n) time O(n) space, prefix array O(n) time O(n) space
# Two-pointer wins on space with equal time.
```

### Problem 3: Three-Sum — Medium-Hard (Deceptively Tricky)

**Problem**: Find all unique triplets summing to zero.

```python
def three_sum(nums: list) -> list:
    """
    Reduction: for each nums[i], find two-sum = -nums[i] in nums[i+1..].
    
    Time: O(n²) — O(n) outer × O(n) two-pointer inner
    Space: O(1) auxiliary (output doesn't count)
    
    Key challenges:
    1. Duplicates: skip duplicate values at all three pointer positions
    2. Early termination: if nums[i] > 0, no triplet can sum to 0
    3. Left pointer starts at i+1 (not 0) to avoid duplicates and backward checking
    """
    nums.sort()                          # O(n log n)
    result = []
    n = len(nums)
    
    for i in range(n - 2):
        # Skip duplicate values for first element
        if i > 0 and nums[i] == nums[i-1]:
            continue
        # Early exit: if smallest remaining is > 0, no solution
        if nums[i] > 0:
            break
        
        lo, hi = i + 1, n - 1
        target = -nums[i]
        
        while lo < hi:
            s = nums[lo] + nums[hi]
            if s == target:
                result.append([nums[i], nums[lo], nums[hi]])
                # Skip duplicates for second and third element
                while lo < hi and nums[lo] == nums[lo + 1]:
                    lo += 1
                while lo < hi and nums[hi] == nums[hi - 1]:
                    hi -= 1
                lo += 1; hi -= 1
            elif s < target:
                lo += 1
            else:
                hi -= 1
    
    return result

# Time: O(n²)  Space: O(1) auxiliary
# Cannot do better than O(n²) for 3-sum in comparison model (lower bound)

def four_sum(nums: list, target: int) -> list:
    """
    Generalization: fix first two with nested loops, two-pointer for last two.
    Time: O(n³)  Space: O(1) auxiliary
    """
    nums.sort()
    n = len(nums)
    result = []
    
    for i in range(n - 3):
        if i > 0 and nums[i] == nums[i-1]: continue
        for j in range(i+1, n - 2):
            if j > i+1 and nums[j] == nums[j-1]: continue
            lo, hi = j+1, n-1
            t = target - nums[i] - nums[j]
            while lo < hi:
                s = nums[lo] + nums[hi]
                if s == t:
                    result.append([nums[i], nums[j], nums[lo], nums[hi]])
                    while lo < hi and nums[lo] == nums[lo+1]: lo += 1
                    while lo < hi and nums[hi] == nums[hi-1]: hi -= 1
                    lo += 1; hi -= 1
                elif s < t: lo += 1
                else: hi -= 1
    
    return result

def k_sum(nums: list, target: int, k: int) -> list:
    """
    General K-sum via recursion: reduce to (K-1)-sum.
    Time: O(n^(K-1))  Space: O(K) recursion depth
    """
    nums.sort()
    
    def backtrack(start: int, target: int, k: int) -> list:
        if k == 2:
            lo, hi = start, len(nums) - 1
            results = []
            while lo < hi:
                s = nums[lo] + nums[hi]
                if s == target:
                    results.append([nums[lo], nums[hi]])
                    while lo < hi and nums[lo] == nums[lo+1]: lo += 1
                    while lo < hi and nums[hi] == nums[hi-1]: hi -= 1
                    lo += 1; hi -= 1
                elif s < target: lo += 1
                else: hi -= 1
            return results
        
        result = []
        for i in range(start, len(nums) - k + 1):
            if i > start and nums[i] == nums[i-1]: continue
            for sub in backtrack(i+1, target - nums[i], k-1):
                result.append([nums[i]] + sub)
        return result
    
    return backtrack(0, target, k)
```

### Problem 4: Dutch National Flag / Sort Colors — Medium

**Problem**: Sort array of 0s, 1s, 2s in one pass.

```python
def sort_colors(nums: list) -> None:
    """
    Dutch National Flag algorithm (Dijkstra 1976).
    
    Three regions maintained throughout:
    [0..lo-1]  = 0s  (red)
    [lo..mid-1] = 1s  (white)
    [mid..hi]  = unclassified
    [hi+1..n-1] = 2s  (blue)
    
    Key invariant: the unclassified region [mid..hi] shrinks each iteration.
    When mid > hi: all elements classified. Done.
    """
    lo, mid, hi = 0, 0, len(nums) - 1
    
    while mid <= hi:
        if nums[mid] == 0:
            nums[lo], nums[mid] = nums[mid], nums[lo]
            lo += 1
            mid += 1
            # Safe to increment mid: nums[lo-1] was 1 (previously classified)
        elif nums[mid] == 1:
            mid += 1
        else:  # nums[mid] == 2
            nums[mid], nums[hi] = nums[hi], nums[mid]
            hi -= 1
            # Do NOT increment mid: nums[hi+1] (just moved to mid) is unknown

# Time: O(n) — exactly one pass  Space: O(1)

def generalized_dnf(arr: list, values: list) -> None:
    """
    Generalize Dutch National Flag to K colors.
    Time: O(K·n) using K-1 applications of 2-way partition.
    Or O(n log K) if we can binary search for the right color.
    """
    # Sort each "color band" from left to right
    lo = 0
    for i in range(len(values) - 1):
        pivot = values[i]
        # Partition: elements == pivot go to [lo, ...]
        mid = lo
        for j in range(lo, len(arr)):
            if arr[j] <= pivot:
                arr[lo], arr[j] = arr[j], arr[lo]
                lo += 1
```

### Problem 5: Minimum Window Substring via Two Pointers — Hard

**Problem**: Smallest window in s containing all characters of t.

```python
from collections import Counter

def min_window(s: str, t: str) -> str:
    """
    Two-pointer (expand-shrink) approach.
    This IS the sliding window approach, but framed as two-pointer for clarity.
    
    Invariant:
    - [lo, hi) is the current window
    - 'formed' tracks how many distinct chars in t are fully covered
    - When formed == required: try to shrink from left
    - Otherwise: expand right
    
    Key: 'have' counts how many characters in window meet t's requirement.
    formed increments only when a character's count in window reaches t's count.
    """
    if not t or not s: return ""
    
    need = Counter(t)            # required counts
    required = len(need)         # number of distinct chars needed
    
    lo = 0
    formed = 0                   # distinct chars in window meeting requirement
    window = {}
    best = float('inf'), 0, 0   # (length, lo, hi)
    
    for hi, ch in enumerate(s):
        window[ch] = window.get(ch, 0) + 1
        
        # Check if current char's count meets requirement
        if ch in need and window[ch] == need[ch]:
            formed += 1
        
        # Shrink window while all requirements met
        while formed == required and lo <= hi:
            # Update answer
            if hi - lo + 1 < best[0]:
                best = (hi - lo + 1, lo, hi)
            
            # Remove leftmost character
            left_ch = s[lo]
            window[left_ch] -= 1
            if left_ch in need and window[left_ch] < need[left_ch]:
                formed -= 1
            lo += 1
    
    return "" if best[0] == float('inf') else s[best[1]:best[2]+1]

# Time: O(|s| + |t|)  Space: O(|t|) for counters
```

### Problem 6: Valid Palindrome II — Medium (One Deletion Allowed)

```python
def valid_palindrome_ii(s: str) -> bool:
    """
    Check if s can become palindrome by deleting at most 1 character.
    
    Two-pointer: when mismatch found, try skipping left or right character.
    """
    def is_palindrome_range(s: str, lo: int, hi: int) -> bool:
        while lo < hi:
            if s[lo] != s[hi]: return False
            lo += 1; hi -= 1
        return True
    
    lo, hi = 0, len(s) - 1
    while lo < hi:
        if s[lo] != s[hi]:
            # Try skipping lo or hi
            return is_palindrome_range(s, lo+1, hi) or \
                   is_palindrome_range(s, lo, hi-1)
        lo += 1; hi -= 1
    return True

# Time: O(n)  Space: O(1)

def valid_palindrome_k_deletions(s: str, k: int) -> bool:
    """
    Generalization: palindrome with at most k deletions.
    Use DP: longest palindromic subsequence of s.
    If len(s) - lps(s) ≤ k, answer is True.
    
    Time: O(n²)  Space: O(n²) or O(n) with rolling array
    """
    n = len(s)
    # LPS via DP (longest palindromic subsequence)
    dp = [[0]*n for _ in range(n)]
    for i in range(n): dp[i][i] = 1
    for length in range(2, n+1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                dp[i][j] = dp[i+1][j-1] + 2
            else:
                dp[i][j] = max(dp[i+1][j], dp[i][j-1])
    lps = dp[0][n-1]
    return (n - lps) <= k
```

### Problem 7: Move Zeros Without Relative Order Change — Easy (Master-level Analysis)

```python
def move_zeroes(nums: list) -> None:
    """
    Move all zeros to end while preserving relative order of non-zeros.
    
    Fast/slow two-pointer:
    - slow: boundary — nums[0..slow-1] are final non-zero positions
    - fast: scans for non-zeros
    
    Invariant: after each step, nums[0..slow-1] contains the non-zeros 
    in their original relative order.
    """
    slow = 0
    for fast in range(len(nums)):
        if nums[fast] != 0:
            nums[slow] = nums[fast]
            slow += 1
    
    # Fill rest with zeros
    while slow < len(nums):
        nums[slow] = 0
        slow += 1

# Time: O(n)  Space: O(1)
# Better than swap-based approach for many zeros (avoids swapping 0s among themselves)

def move_zeroes_optimal_swaps(nums: list) -> None:
    """
    Swap-based: minimizes number of write operations.
    Useful when writes are expensive (flash memory, etc.).
    """
    slow = 0
    for fast in range(len(nums)):
        if nums[fast] != 0:
            nums[slow], nums[fast] = nums[fast], nums[slow]
            slow += 1
# Swaps ≤ number of non-zeros — minimizes writes.
```

---

## Advanced Variations

### Multi-Pass Two-Pointer for Complex Constraints

```python
def three_pointer_problem(arr: list) -> int:
    """
    Example: find i < j < k such that arr[i] < arr[j] < arr[k].
    
    Approach: prefix min from left, suffix max from right, middle scan.
    Three passes, O(n) each → O(n) total.
    """
    n = len(arr)
    # Left pass: left_min[i] = min(arr[0..i])
    left_min = [0] * n
    left_min[0] = arr[0]
    for i in range(1, n): left_min[i] = min(left_min[i-1], arr[i])
    
    # Right pass: right_max[i] = max(arr[i..n-1])
    right_max = [0] * n
    right_max[n-1] = arr[n-1]
    for i in range(n-2, -1, -1): right_max[i] = max(right_max[i+1], arr[i])
    
    # Check middle
    for j in range(1, n-1):
        if left_min[j-1] < arr[j] < right_max[j+1]:
            return j
    return -1
# Time: O(n)  Space: O(n)
```

### Partitioning with Multiple Conditions (Lomuto vs Hoare)

```python
def lomuto_partition(arr: list, lo: int, hi: int) -> int:
    """
    Lomuto: pivot = arr[hi]. One-directional scan.
    Simpler but does more swaps. Slower in practice.
    """
    pivot = arr[hi]
    i = lo
    for j in range(lo, hi):
        if arr[j] <= pivot:
            arr[i], arr[j] = arr[j], arr[i]
            i += 1
    arr[i], arr[hi] = arr[hi], arr[i]
    return i

def hoare_partition(arr: list, lo: int, hi: int) -> int:
    """
    Hoare: pivot = arr[lo]. Bidirectional (two-pointer).
    Fewer swaps, handles equal elements better.
    Returns index j such that arr[lo..j] ≤ pivot ≤ arr[j+1..hi] (roughly).
    
    NOTE: Hoare returns j such that [lo..j] and [j+1..hi] are the two partitions.
    The pivot is NOT necessarily at index j!
    """
    pivot = arr[(lo + hi) // 2]
    i, j = lo - 1, hi + 1
    while True:
        i += 1
        while arr[i] < pivot: i += 1
        j -= 1
        while arr[j] > pivot: j -= 1
        if i >= j: return j
        arr[i], arr[j] = arr[j], arr[i]
```

---

## Edge Cases Bible

1. **Empty or single-element array**: Always check `if len(arr) < 2: return` before starting two-pointer. Many off-by-one bugs come from arrays of length 0 or 1.

2. **All elements equal**: In three-sum, if all elements are 0, the only answer is [0,0,0]. Skip duplicates carefully — the deduplication logic `if nums[lo] == nums[lo+1]` assumes lo+1 is a valid index.

3. **Integer overflow in pair sum**: In Python this doesn't matter, but in Java/C++ `arr[lo] + arr[hi]` can overflow int. Use long arithmetic.

4. **Palindrome edge cases**: Single character is always a palindrome. Empty string — define as palindrome or not depending on problem statement.

5. **Dutch National Flag with single or two distinct values**: Algorithm still works (one or two regions become empty), but verify loop termination: `while mid <= hi` handles hi = -1 gracefully.

6. **Three-sum with multiple solutions**: If the problem asks for unique triplets, the deduplication logic at ALL THREE pointer levels is required, not just one. Missing any level causes duplicates.

7. **Two-pointer on non-sorted array**: The correctness proof requires sorted order for opposite-direction pointers. Applying opposite-direction to unsorted array = WRONG. Always verify sort precondition.

8. **Container with most water: width vs height confusion**: Area = min(h[l], h[r]) × (r − l), where (r − l) is the WIDTH (number of units, indices differ). Common bug: using r − l + 1 or forgetting the width term.

9. **Shrinking window in min-window-substring**: When `formed == required`, the inner while loop may exhaust all valid windows. Ensure best is updated BEFORE shrinking, not after.

---

## Interview Tips

### What Interviewers Look For

1. **State the invariant before coding**: "My invariant is: all pairs involving arr[lo] with anything right of hi have been eliminated." This signals rigorous thinking.

2. **Justify pointer movement**: Interviewers WILL ask "why move left pointer and not right?" Be ready with the proof: "because all remaining pairs with arr[lo] have area ≤ current area."

3. **Two-pointer vs sliding window distinction**: 
   - Two-pointer: both pointers can move in either direction, crossing forbidden
   - Sliding window: right pointer only moves right, left follows to maintain window constraint
   - Some problems work both ways — know the distinction.

4. **3-sum deduplications**: This trips up ~80% of candidates. Walk through deduplication logic explicitly: "I skip duplicate i values, then after finding a valid triplet, skip duplicates for lo and hi before moving both inward."

5. **Complexity analysis**: Always state "O(n) time, O(1) space — better than hash-based O(n) space approach." Shows you considered alternatives.

6. **When NOT to use two-pointers**: 
   - Unsorted arrays where sorted order is semantically meaningful
   - When you need to track indices AND the array isn't sorted by value
   - When the feasibility check is more complex than O(1) per step → sliding window with deque

7. **Common bugs to avoid live**:
   - `while lo < hi` vs `while lo <= hi` — for pairs you want strict inequality
   - Forgetting to restore state when trying both deletion options in palindrome-II
   - Infinite loop when both lo and hi don't move (check all three if/elif/else branches cover all cases)

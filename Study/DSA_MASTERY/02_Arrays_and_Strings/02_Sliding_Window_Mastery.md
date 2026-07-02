# Sliding Window — Mastery Guide

## Core Concept & Invariant

A sliding window maintains a **contiguous subarray** as a "window" that expands and shrinks
while preserving a property. The key invariant is:

> At every step, the window [lo, hi] represents the largest (or smallest) valid window
> ending at index `hi` that satisfies the constraint.

**Two canonical patterns**:
1. **Fixed-size window**: Window always has exactly K elements; slide right by 1.
2. **Variable-size window**: Expand right when valid, shrink left when invalid.

**Correctness argument**: The right pointer `hi` moves monotonically right (never back). For each `hi`, we move `lo` as far right as needed to restore validity. Since `lo` also only moves right, total pointer movements = O(n). Key insight: **lo never needs to go left** because if [lo', hi] was invalid with lo' < lo, then [lo'-1, hi] is also invalid.

**The "at most K" trick**:
`count(exactly K distinct) = count(at most K distinct) − count(at most K-1 distinct)`

This converts "exactly K" (hard to maintain) to "at most K" (easy with shrink step).

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Fixed-size window
# ─────────────────────────────────────────────────────────────
def fixed_window(arr: list, k: int):
    """
    Compute something for every window of exactly size k.
    Time: O(n)  Space: O(1) or O(k) depending on what's tracked.
    """
    if len(arr) < k:
        return []
    
    # Initialize first window
    window_val = sum(arr[:k])   # or however you compute initial window
    results = [window_val]
    
    # Slide: add right element, remove left element
    for i in range(k, len(arr)):
        window_val += arr[i] - arr[i - k]   # O(1) update
        results.append(window_val)
    
    return results

# ─────────────────────────────────────────────────────────────
# Template 2: Variable-size window (maximize window)
# ─────────────────────────────────────────────────────────────
def variable_window_maximize(arr: list, constraint) -> int:
    """
    Find longest subarray satisfying constraint.
    Expand hi until constraint violated, then shrink lo until valid again.
    """
    lo = 0
    state = {}   # track window state
    best = 0
    
    for hi in range(len(arr)):
        # Expand: include arr[hi] in window
        # update state with arr[hi]
        
        # Shrink: if constraint violated, move lo right
        while not constraint(state):   # or: while violation
            # update state removing arr[lo]
            lo += 1
        
        best = max(best, hi - lo + 1)
    
    return best

# ─────────────────────────────────────────────────────────────
# Template 3: Variable-size window (minimize window)
# ─────────────────────────────────────────────────────────────
def variable_window_minimize(arr: list, target) -> int:
    """
    Find shortest subarray satisfying constraint (≥ target sum, etc.).
    Shrink lo as much as possible while constraint still holds.
    """
    lo = 0
    current = 0
    best = float('inf')
    
    for hi in range(len(arr)):
        current += arr[hi]            # Expand
        
        while current >= target:      # Constraint satisfied: try to shrink
            best = min(best, hi - lo + 1)
            current -= arr[lo]
            lo += 1
    
    return best if best != float('inf') else 0

# ─────────────────────────────────────────────────────────────
# Template 4: Exactly K via At-Most-K subtraction
# ─────────────────────────────────────────────────────────────
def at_most_k(arr: list, k: int) -> int:
    """Count subarrays with at most k distinct elements."""
    from collections import defaultdict
    count = defaultdict(int)
    distinct = 0
    lo = 0
    result = 0
    
    for hi in range(len(arr)):
        if count[arr[hi]] == 0:
            distinct += 1
        count[arr[hi]] += 1
        
        while distinct > k:
            count[arr[lo]] -= 1
            if count[arr[lo]] == 0:
                distinct -= 1
            lo += 1
        
        result += hi - lo + 1   # All subarrays ending at hi with ≤ k distinct
    return result

def exactly_k_distinct_subarrays(arr: list, k: int) -> int:
    """Count subarrays with exactly k distinct elements. O(n) time."""
    return at_most_k(arr, k) - at_most_k(arr, k - 1)
```

---

## Complexity Analysis

| Pattern | Time | Space | Notes |
|---------|------|-------|-------|
| Fixed-size window sum/max | O(n) | O(1) | Simple sliding |
| Variable window (hash map) | O(n) | O(k) | k = distinct elements |
| Sliding window maximum | O(n) | O(k) | Monotonic deque |
| Minimum window substring | O(\|s\| + \|t\|) | O(\|t\|) | Two counters |
| Exactly-K-distinct | O(n) | O(k) | At-most trick |
| Permutation in string | O(\|s\| + \|t\|) | O(26) | Fixed window |

---

## Classic Problems

### Problem 1: Minimum Window Substring — Hard (Full Derivation)

**Problem**: Find smallest substring of s containing all characters of t.

```python
from collections import Counter

def min_window(s: str, t: str) -> str:
    """
    Full derivation:
    
    State: window character counts vs required counts.
    'formed': # distinct characters that meet their required count.
    
    Key invariant: formed == required ↔ window contains all of t.
    
    Why this is O(|s| + |t|):
    - hi moves right |s| times (total)
    - lo moves right at most |s| times (total, not per hi step)
    - Each character processed at most twice (once by hi, once by lo)
    → O(|s|) total for the main loop, O(|t|) for Counter initialization
    
    When to shrink vs expand:
    - formed < required: MUST expand (window incomplete)
    - formed == required: CAN shrink (window complete — minimize it)
    """
    if not t or not s: return ""
    
    need = Counter(t)
    required = len(need)       # unique chars needed
    
    window = {}
    formed = 0                 # unique chars in window meeting count
    lo = 0
    best_len, best_lo = float('inf'), 0
    
    for hi, ch in enumerate(s):
        # Expand
        window[ch] = window.get(ch, 0) + 1
        if ch in need and window[ch] == need[ch]:
            formed += 1
        
        # Shrink greedily (while complete)
        while formed == required:
            # Record answer
            if hi - lo + 1 < best_len:
                best_len = hi - lo + 1
                best_lo = lo
            
            # Remove left
            left = s[lo]
            window[left] -= 1
            if left in need and window[left] < need[left]:
                formed -= 1
            lo += 1
    
    return s[best_lo:best_lo + best_len] if best_len != float('inf') else ""

# Time: O(|s| + |t|)  Space: O(|t|)

def min_window_optimized(s: str, t: str) -> str:
    """
    Optimization: prefilter s to only characters in t.
    Reduces effective |s| when t is small and s has many irrelevant chars.
    
    filtered_s = [(index, char) for index, char in enumerate(s) if char in t]
    Then run the same algorithm on filtered_s but use original indices.
    Worst case still O(|s|), but practical speedup when |t| << |s|.
    """
    if not t or not s: return ""
    need = Counter(t)
    required = len(need)
    
    # Filter s to relevant characters only
    filtered = [(i, ch) for i, ch in enumerate(s) if ch in need]
    
    window = {}
    formed = 0
    lo = 0
    best = float('inf'), 0, 0
    
    for r, (hi_idx, ch) in enumerate(filtered):
        window[ch] = window.get(ch, 0) + 1
        if window[ch] == need[ch]:
            formed += 1
        
        while formed == required:
            lo_idx = filtered[lo][0]
            if hi_idx - lo_idx + 1 < best[0]:
                best = (hi_idx - lo_idx + 1, lo_idx, hi_idx)
            
            left_ch = filtered[lo][1]
            window[left_ch] -= 1
            if window[left_ch] < need[left_ch]:
                formed -= 1
            lo += 1
    
    return s[best[1]:best[2]+1] if best[0] != float('inf') else ""
```

### Problem 2: Sliding Window Maximum (Monotonic Deque) — Hard

**Problem**: For each window of size k in array, find the maximum.

```python
from collections import deque

def sliding_window_maximum(nums: list, k: int) -> list:
    """
    Monotonic deque approach: maintain decreasing deque of INDICES.
    
    Invariant: deque contains indices in [lo, hi] such that
    nums[deque[0]] ≥ nums[deque[1]] ≥ ... (decreasing values).
    The front of the deque is always the maximum of the current window.
    
    Why O(n): each index is added to deque exactly once and removed at most once.
    → Total deque operations = O(n), even though inner while loop runs.
    
    This is NOT just a sliding window — it's a MONOTONIC DEQUE (see File 07).
    The key property: we never need an element smaller than the current
    new element, because the current element will be in the window longer
    (it expires later) and is larger (better answer). So smaller earlier
    elements are useless.
    """
    dq = deque()   # stores indices; values are decreasing
    result = []
    
    for hi in range(len(nums)):
        # Remove elements outside window
        while dq and dq[0] < hi - k + 1:
            dq.popleft()
        
        # Remove elements smaller than current (they'll never be max)
        while dq and nums[dq[-1]] < nums[hi]:
            dq.pop()
        
        dq.append(hi)
        
        # Window has k elements
        if hi >= k - 1:
            result.append(nums[dq[0]])
    
    return result

# Time: O(n) — each element enqueued/dequeued at most once
# Space: O(k) — deque size bounded by window size

def sliding_window_minimum(nums: list, k: int) -> list:
    """Symmetric: monotonic INCREASING deque for window minimum."""
    dq = deque()
    result = []
    for hi in range(len(nums)):
        while dq and dq[0] < hi - k + 1:
            dq.popleft()
        while dq and nums[dq[-1]] > nums[hi]:
            dq.pop()
        dq.append(hi)
        if hi >= k - 1:
            result.append(nums[dq[0]])
    return result
```

### Problem 3: Longest Substring Without Repeating Characters — Medium

**Problem**: Find length of longest substring without duplicate characters.

```python
def length_of_longest_substring(s: str) -> int:
    """
    Variable window: window [lo, hi] has all unique characters.
    
    When we encounter s[hi] already in window:
    - Move lo past the previous occurrence of s[hi]
    - Window becomes valid again (all unique)
    
    Optimization: store last_seen[ch] = most recent index of ch.
    Move lo to max(lo, last_seen[ch] + 1) — this is the key step.
    The max() ensures lo never moves backward.
    """
    last_seen = {}     # char → last index seen
    lo = 0
    best = 0
    
    for hi, ch in enumerate(s):
        if ch in last_seen and last_seen[ch] >= lo:
            lo = last_seen[ch] + 1   # Jump lo past previous occurrence
        last_seen[ch] = hi
        best = max(best, hi - lo + 1)
    
    return best

# Time: O(n)  Space: O(min(n, alphabet_size))

def length_of_longest_substring_two_distinct(s: str) -> int:
    """
    Longest substring with at most 2 distinct characters.
    Generalization to K distinct: same structure, change 2 → K.
    """
    from collections import defaultdict
    count = defaultdict(int)
    distinct = 0
    lo = 0
    best = 0
    
    for hi, ch in enumerate(s):
        if count[ch] == 0:
            distinct += 1
        count[ch] += 1
        
        while distinct > 2:
            count[s[lo]] -= 1
            if count[s[lo]] == 0:
                distinct -= 1
            lo += 1
        
        best = max(best, hi - lo + 1)
    return best

# Time: O(n)  Space: O(K) where K = number of distinct chars allowed
```

### Problem 4: Longest Subarray with at Most K Zeros (Fruit into Baskets variant) — Medium

**Problem**: Replace at most K zeros; find longest subarray.

```python
def longest_ones(nums: list, k: int) -> int:
    """
    Window invariant: number of 0s in [lo, hi] ≤ k.
    
    This is equivalent to: find longest subarray with at most k zeros.
    Also equivalent to: Fruit Into Baskets with 2 fruit types
    (replace 0→fruit_A, 1→fruit_B, k=total_A_allowed).
    
    Classic constraint: zeros ≤ k.
    When zeros > k: shrink from left until zeros ≤ k.
    """
    lo = 0
    zeros = 0
    best = 0
    
    for hi in range(len(nums)):
        if nums[hi] == 0:
            zeros += 1
        
        while zeros > k:
            if nums[lo] == 0:
                zeros -= 1
            lo += 1
        
        best = max(best, hi - lo + 1)
    return best

# Time: O(n)  Space: O(1)

def fruit_into_baskets(fruits: list) -> int:
    """
    Two baskets (types), each can hold one type of fruit.
    Find longest subarray with at most 2 distinct values.
    """
    from collections import defaultdict
    basket = defaultdict(int)
    lo = 0
    best = 0
    
    for hi, f in enumerate(fruits):
        basket[f] += 1
        
        while len(basket) > 2:
            basket[fruits[lo]] -= 1
            if basket[fruits[lo]] == 0:
                del basket[fruits[lo]]
            lo += 1
        
        best = max(best, hi - lo + 1)
    return best
# Time: O(n)  Space: O(1) since at most 3 keys tracked
```

### Problem 5: Permutation in String — Medium

**Problem**: Check if any permutation of p appears as substring of s.

```python
def check_inclusion(p: str, s: str) -> bool:
    """
    Fixed-size window of size len(p). Check if window is a permutation of p.
    
    Instead of sorting window each time (O(k log k) per step),
    use a difference counter: diff[ch] = count_in_window - count_in_p.
    A valid window has all diff values = 0, tracked by 'mismatch' count.
    
    Why this is O(|s|) not O(|s|·|alphabet|):
    Each step updates at most 2 characters (right added, left removed).
    The 'mismatch' counter tells us validity in O(1).
    """
    k = len(p)
    if k > len(s): return False
    
    need = Counter(p)
    window = Counter(s[:k])
    
    # mismatch: number of characters where window[ch] != need[ch]
    mismatch = sum(1 for ch in set(need) | set(window) if window[ch] != need[ch])
    
    if mismatch == 0: return True
    
    for i in range(k, len(s)):
        # Add right character
        right = s[i]
        old_diff_r = (window[right] != need[right])
        window[right] += 1
        new_diff_r = (window[right] != need[right])
        mismatch += new_diff_r - old_diff_r
        
        # Remove left character
        left = s[i - k]
        old_diff_l = (window[left] != need[left])
        window[left] -= 1
        new_diff_l = (window[left] != need[left])
        mismatch += new_diff_l - old_diff_l
        if window[left] == 0:
            del window[left]
        
        if mismatch == 0: return True
    
    return False

# Time: O(|s| + |p|)  Space: O(|p| + alphabet_size)

def find_all_anagrams(s: str, p: str) -> list:
    """
    Find all starting indices of anagrams of p in s.
    Same approach — collect all valid window positions.
    """
    k = len(p)
    if k > len(s): return []
    
    need = Counter(p)
    window = Counter(s[:k])
    mismatch = sum(1 for ch in set(need) | set(window) if window[ch] != need[ch])
    result = [0] if mismatch == 0 else []
    
    for i in range(k, len(s)):
        right, left = s[i], s[i-k]
        
        old_r = (window[right] != need[right])
        window[right] += 1
        mismatch += (window[right] != need[right]) - old_r
        
        old_l = (window[left] != need[left])
        window[left] -= 1
        mismatch += (window[left] != need[left]) - old_l
        if window[left] == 0: del window[left]
        
        if mismatch == 0: result.append(i - k + 1)
    
    return result
# Time: O(|s| + |p|)  Space: O(|p|)
```

### Problem 6: Subarrays with K Distinct Integers — Hard (Exactly-K Trick)

**Problem**: Count subarrays with exactly K distinct integers.

```python
def subarrays_with_k_distinct(nums: list, k: int) -> int:
    """
    Exact-K via At-Most-K subtraction.
    
    Key insight: "exactly K" windows are hard to count directly because
    when we find a valid window, we can't easily count all valid sub-windows.
    
    "At most K" is easy: for each hi, lo is the leftmost position making
    the window valid. The count of valid windows ENDING at hi = hi - lo + 1.
    
    count(exactly K) = count(at most K) - count(at most K-1)
    
    Proof: Subarrays with exactly k distinct = 
           {subarrays with ≤ k distinct} - {subarrays with ≤ k-1 distinct}
           = {subarrays with ≤ k distinct} - {subarrays with ≤ k distinct and missing at least 1}
           = {subarrays with exactly k distinct} ✓
    """
    from collections import defaultdict
    
    def at_most_k(k: int) -> int:
        count = defaultdict(int)
        distinct = 0
        lo = 0
        result = 0
        for hi in range(len(nums)):
            if count[nums[hi]] == 0:
                distinct += 1
            count[nums[hi]] += 1
            while distinct > k:
                count[nums[lo]] -= 1
                if count[nums[lo]] == 0:
                    distinct -= 1
                lo += 1
            result += hi - lo + 1   # All subarrays [j..hi] for lo ≤ j ≤ hi
        return result
    
    return at_most_k(k) - at_most_k(k - 1)

# Time: O(n)  Space: O(k)

def count_subarrays_with_product_less_than_k(nums: list, k: int) -> int:
    """
    Count subarrays with product < k. Same sliding window pattern.
    Constraint: product < k (shrink when product ≥ k).
    Contribution: hi - lo + 1 subarrays end at hi (all starting from lo to hi).
    """
    if k <= 1: return 0
    lo = 0
    product = 1
    result = 0
    
    for hi in range(len(nums)):
        product *= nums[hi]
        while product >= k and lo <= hi:
            product //= nums[lo]
            lo += 1
        result += hi - lo + 1
    return result
# Time: O(n)  Space: O(1)
```

### Problem 7: Minimum Size Subarray Sum — Medium

**Problem**: Find minimum length subarray with sum ≥ target.

```python
def min_subarray_len(target: int, nums: list) -> int:
    """
    Variable window minimization pattern.
    
    Key: when window sum ≥ target, try to shrink from left.
    This is the opposite of maximization — we shrink while valid,
    not while invalid.
    
    Correctness: for each hi, lo is pushed as far right as possible
    while still keeping sum ≥ target. This gives the minimum length
    valid window ending at hi.
    """
    lo = 0
    window_sum = 0
    best = float('inf')
    
    for hi in range(len(nums)):
        window_sum += nums[hi]
        
        # Shrink window while still valid (sum ≥ target)
        while window_sum >= target:
            best = min(best, hi - lo + 1)
            window_sum -= nums[lo]
            lo += 1
    
    return best if best != float('inf') else 0

# Time: O(n)  Space: O(1)

# Alternative: binary search on window size + prefix sum → O(n log n)
# The sliding window O(n) approach is optimal.

import bisect

def min_subarray_len_binary_search(target: int, nums: list) -> int:
    """
    O(n log n) approach using prefix sums + binary search.
    Useful when you want to understand the relationship to prefix sum technique.
    Not recommended — sliding window is strictly better.
    """
    prefix = [0] * (len(nums) + 1)
    for i, x in enumerate(nums):
        prefix[i+1] = prefix[i] + x
    
    best = float('inf')
    for i in range(1, len(nums) + 1):
        needed = target + prefix[i-1]
        j = bisect.bisect_left(prefix, needed)
        if j <= len(nums):
            best = min(best, j - (i-1))
    
    return best if best != float('inf') else 0
```

---

## When to Use HashMap vs Array vs Bitset in Window

```python
# ── Choice 1: Fixed small alphabet (e.g., lowercase letters) ──
# Use array[26] — O(1) access, cache-friendly
def window_with_array(s: str) -> None:
    count = [0] * 26   # index = ord(ch) - ord('a')
    # Access: count[ord(ch) - ord('a')]
    # Faster than dict for small alphabets

# ── Choice 2: Large/arbitrary keys ──
# Use defaultdict(int) or Counter
from collections import defaultdict
def window_with_dict(s: str) -> None:
    count = defaultdict(int)
    # Access: count[ch]

# ── Choice 3: Boolean membership tracking ──
# Use bitset (int as bitmask) for 26 lowercase letters
def window_with_bitset(s: str) -> None:
    mask = 0
    for ch in s:
        bit = 1 << (ord(ch) - ord('a'))
        mask |= bit
    distinct = bin(mask).count('1')

# ── Performance comparison ──
# array[26]: O(1) access, ~26 cache-line elements → very fast
# dict: O(1) amortized but hash overhead → slower for small alphabets
# bitset: O(1) but only tracks presence, not counts

# ── Decision rule ──
# Alphabet ≤ 64 and only tracking presence → bitset (int)
# Alphabet ≤ 256 and tracking counts → array
# Arbitrary keys → dict/Counter
```

---

## Advanced Variations

### Constrained Subsequence Sum (Sliding Window DP)

```python
from collections import deque

def constrained_subset_sum(nums: list, k: int) -> int:
    """
    Maximum sum of non-empty subsequence where consecutive chosen elements
    are at most k indices apart.
    
    DP: dp[i] = max sum of valid subsequence ending at i.
    dp[i] = nums[i] + max(0, max(dp[i-k..i-1]))
    
    Naive: O(nk) — iterate last k positions.
    Sliding window maximum: O(n) — maintain deque of max dp values in window of size k.
    """
    n = len(nums)
    dp = nums[:]     # dp[i] = best subsequence sum ending at i
    dq = deque()     # monotonic decreasing deque of dp indices
    
    for i in range(n):
        # Remove elements out of window [i-k, i-1]
        while dq and dq[0] < i - k:
            dq.popleft()
        
        # Best previous dp value in window
        if dq:
            dp[i] = max(dp[i], nums[i] + dp[dq[0]])
        
        # Maintain decreasing deque
        while dq and dp[dq[-1]] < dp[i]:
            dq.pop()
        dq.append(i)
    
    return max(dp)

# Time: O(n)  Space: O(k) for deque
```

### Number of Subarrays with Bounded Maximum

```python
def num_subarray_bounded_max(nums: list, left: int, right: int) -> int:
    """
    Count subarrays where max element is in [left, right].
    
    Approach: count(max ≤ right) - count(max ≤ left-1)
    
    count(max ≤ bound): use sliding window where we reset count when nums[i] > bound.
    For each position, add (i - last_reset) valid subarrays ending here.
    """
    def count_max_at_most(bound: int) -> int:
        result = 0
        cur = 0
        for x in nums:
            cur = cur + 1 if x <= bound else 0
            result += cur
        return result
    
    return count_max_at_most(right) - count_max_at_most(left - 1)

# Time: O(n)  Space: O(1)
```

---

## Edge Cases Bible

1. **Empty string/array**: Always handle `if not s: return ""` or `if len(nums) == 0: return 0` before starting the window.

2. **k > len(arr) for fixed window**: Fixed window template assumes `len(arr) >= k`. Add guard: `if len(arr) < k: return []`.

3. **All elements satisfy constraint**: Variable window expands to entire array. Best = entire array length. Verify lo stays at 0 in this case.

4. **No valid window exists**: Minimum window problems return 0 or "". Initialize `best = float('inf')` and check at end.

5. **Duplicate characters in permutation check**: Counter handles this. If p = "aab", need Counter('aab') = {'a':2, 'b':1}, not just distinct characters.

6. **At-most-K trick with k=0**: `at_most_k(0)` should return count of subarrays with 0 distinct chars = only empty subarrays = 0. Ensure the algorithm handles this (distinct > 0 triggers shrink before any addition).

7. **Sliding window max with k=1**: Every element is both added and immediately considered as max. Result is just nums itself.

8. **Negative numbers in minimum subarray sum**: Standard sliding window assumes positive numbers (so adding elements increases sum). For negative numbers, use Kadane's or prefix sum approaches instead.

9. **Counter equality check for permutations**: Two counters are equal if all keys and values match. Counter({'a':1}) != Counter({'a':1, 'b':0}) in Python — always delete keys with 0 count or compare properly.

10. **lo advancing past hi**: In some edge cases (e.g., single element window), lo might equal hi. Ensure `while` condition prevents lo > hi.

---

## Interview Tips

### What Interviewers Look For

1. **Immediately identify the pattern**: "This is a variable-size window because we're maximizing/minimizing the window size subject to a constraint." Then state the constraint clearly.

2. **State when to expand vs shrink**: "We expand `hi` always (one step per iteration). We shrink `lo` while the constraint is violated (maximization) or while the constraint holds (minimization)."

3. **The at-most-K trick**: If asked about "exactly K distinct," immediately say "I'll compute at_most(K) - at_most(K-1)." This is a FAANG favorite and many candidates miss it.

4. **Distinguish from two-pointer**: Sliding window has right pointer moving right always; two-pointer can have both pointers move in various patterns. Sliding window is a specialization.

5. **Why monotonic deque for window max**: "A simple sorted structure would be O(n log k). The monotonic deque gives O(n) because each element is added and removed exactly once, and we never need elements smaller than the current element for future windows."

6. **When sliding window doesn't work**:
   - Non-contiguous elements (need backtracking → DP)
   - 2D windows (need 2D prefix sum or row-by-row sliding)
   - Negative numbers in sum-based window (losing an element can decrease sum → window must shrink even when sum is too small)

7. **Code the shrink step carefully**: For maximum-window problems, shrink while `violated`. For minimum-window problems, shrink while `satisfied`. Confusing these is the most common bug.

8. **Complexity justification**: "Each element is added to the window once (O(n) total) and removed at most once (O(n) total). The inner while loop does NOT make this O(n²) — it's O(n) total across all iterations of the outer loop."

# Binary Search on Answer — Mastery Guide

## Core Concept & Invariant

Binary search on the **answer space** (not the data) applies when:
1. The answer is a monotone function: there exists a threshold X such that feasible(X) is True for all X ≥ threshold (or all X ≤ threshold for minimization).
2. Checking feasibility of a given answer X can be done efficiently (typically O(n) or O(n log n)).

**The Fundamental Invariant**:
```
lo is always a feasible answer (or below feasible range)
hi is always above the feasible range (or infeasible)

After convergence: lo = hi = the smallest feasible answer
```

**Identification checklist**:
- Problem asks for minimum/maximum of some value
- The answer has a natural range [lo, hi]
- If answer X works, does X+1 (or X-1) also work? → monotone → binary search applicable
- Can you write `is_feasible(X)` that returns bool?

**Template selection**:
- **Minimize**: find smallest X where `feasible(X)` is True
- **Maximize**: find largest X where `feasible(X)` is True

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Find MINIMUM value X where feasible(X) is True
# ─────────────────────────────────────────────────────────────
def binary_search_min(lo: int, hi: int, feasible) -> int:
    """
    Invariant: answer ∈ [lo, hi]
    Post-condition: lo = hi = minimum feasible value
    
    Loop terminates because: each iteration reduces hi - lo by at least 1.
    When lo < hi: mid < hi always (integer division), so hi shrinks or lo grows.
    """
    while lo < hi:
        mid = lo + (hi - lo) // 2   # Avoid overflow; always < hi
        if feasible(mid):
            hi = mid     # mid works; try smaller → shrink hi to mid
        else:
            lo = mid + 1  # mid doesn't work; answer must be > mid
    return lo   # lo == hi == answer

# ─────────────────────────────────────────────────────────────
# Template 2: Find MAXIMUM value X where feasible(X) is True
# ─────────────────────────────────────────────────────────────
def binary_search_max(lo: int, hi: int, feasible) -> int:
    """
    Invariant: answer ∈ [lo, hi]
    Post-condition: lo = hi = maximum feasible value
    """
    while lo < hi:
        mid = lo + (hi - lo + 1) // 2   # +1 to avoid infinite loop when lo+1=hi
        if feasible(mid):
            lo = mid     # mid works; try larger → shrink lo to mid
        else:
            hi = mid - 1  # mid doesn't work; answer must be < mid
    return lo

# ─────────────────────────────────────────────────────────────
# Template 3: Float binary search (when answer is real-valued)
# ─────────────────────────────────────────────────────────────
def binary_search_float(lo: float, hi: float, feasible, iterations: int = 100) -> float:
    """
    For real-valued answers. Use fixed iterations instead of lo < hi.
    100 iterations gives precision of (hi-lo) / 2^100 ≈ 10^-30.
    """
    for _ in range(iterations):
        mid = (lo + hi) / 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid
    return lo
```

---

## Complexity Analysis

| Problem | Search Space | Check Complexity | Total |
|---------|-------------|-----------------|-------|
| Koko Eating Bananas | [1, max_pile] | O(n) | O(n log max) |
| Ship Packages | [max_weight, total] | O(n) | O(n log sum) |
| Split Array Largest Sum | [max_el, total] | O(n) | O(n log sum) |
| Min Days for Bouquets | [1, n] | O(n) | O(n log n) |
| Magnetic Force Balls | [1, max_gap] | O(n) | O(n log max) |
| Kth Smallest in Matrix | [min, max] | O(n) | O(n log(max-min)) |
| Aggressive Cows | [1, max_gap] | O(n) | O(n log max) |
| Allocate Min Pages | [max_page, total] | O(n) | O(n log sum) |

---

## Classic Problems

### Problem 1: Koko Eating Bananas — Medium

**Problem**: Eat k bananas per hour; minimum k to finish all piles in h hours.

```python
def min_eating_speed(piles: list, h: int) -> int:
    """
    Monotone property: if speed k works, speed k+1 also works.
    Find MINIMUM speed k such that total_hours(k) ≤ h.
    
    total_hours(k) = Σ ceil(pile / k)
    
    Search space: [1, max(piles)]
    - Lower bound: 1 banana/hour (minimum possible speed)
    - Upper bound: max(piles) (can finish largest pile in 1 hour)
    
    Feasibility check: O(n) — sum up ceil(pile/k) for all piles.
    Binary search: O(log(max_pile)) iterations.
    Total: O(n log(max_pile))
    """
    import math
    
    def feasible(speed: int) -> bool:
        return sum(math.ceil(pile / speed) for pile in piles) <= h
    
    lo, hi = 1, max(piles)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid   # Works; try slower
        else:
            lo = mid + 1  # Too slow
    
    return lo

# Time: O(n log(max_pile))  Space: O(1)

# ── Verification ──
# piles = [3,6,7,11], h = 8
# feasible(4): ceil(3/4)+ceil(6/4)+ceil(7/4)+ceil(11/4) = 1+2+2+3 = 8 ≤ 8 ✓
# feasible(3): ceil(3/3)+ceil(6/3)+ceil(7/3)+ceil(11/3) = 1+2+3+4 = 10 > 8 ✗
# Answer: 4
```

### Problem 2: Capacity to Ship Packages Within D Days — Medium

**Problem**: Minimum ship capacity to ship all weights within D days.

```python
def ship_within_days(weights: list, days: int) -> int:
    """
    Monotone: if capacity C is sufficient, C+1 is also sufficient.
    Find MINIMUM C such that packages can be shipped in ≤ days.
    
    Feasibility: greedily fill each day without exceeding C.
    Days needed with capacity C = number of groups when greedily partitioning.
    
    Search space: [max(weights), sum(weights)]
    - Lower bound: max(weights) — must fit heaviest single package
    - Upper bound: sum(weights) — ship everything in one day
    """
    def feasible(capacity: int) -> bool:
        days_used = 1
        current_load = 0
        for w in weights:
            if current_load + w > capacity:
                days_used += 1    # Start a new day
                current_load = 0
            current_load += w
        return days_used <= days
    
    lo, hi = max(weights), sum(weights)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid + 1
    
    return lo

# Time: O(n log(sum(weights)))  Space: O(1)
```

### Problem 3: Split Array Largest Sum — Hard

**Problem**: Split array into k non-empty subarrays minimizing the maximum subarray sum.

```python
def split_array(nums: list, k: int) -> int:
    """
    OBSERVE: This is IDENTICAL to the ship-within-days problem with days=k!
    Minimizing maximum subarray sum = minimizing "capacity" for k "ships/days."
    
    Monotone: if we can split into k parts with max ≤ X, we can with max ≤ X+1.
    Find MINIMUM X such that we can split into ≤ k parts with each sum ≤ X.
    
    Feasibility: greedily assign elements to current part; start new part when sum exceeds X.
    """
    def feasible(max_sum: int) -> bool:
        parts = 1
        current = 0
        for x in nums:
            if x > max_sum:
                return False   # Single element exceeds max — impossible
            if current + x > max_sum:
                parts += 1
                current = 0
            current += x
        return parts <= k
    
    lo, hi = max(nums), sum(nums)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid + 1
    
    return lo

# Time: O(n log(sum))  Space: O(1)
```

### Problem 4: Minimum Days to Make M Bouquets — Medium

**Problem**: From n roses, bloom[i] = bloom day. Make m bouquets, each needs k adjacent flowers.

```python
def min_days(bloomDay: list, m: int, k: int) -> int:
    """
    Monotone: if day d is sufficient (enough bouquets), day d+1 is also sufficient.
    Find MINIMUM day d such that at least m bouquets can be made.
    
    Feasibility: on day d, flowers with bloomDay ≤ d have bloomed.
    Count consecutive bloomed flowers; whenever we get k consecutive, form a bouquet.
    
    Search space: [1, max(bloomDay)]
    Edge case: if m×k > n, impossible — return -1 immediately.
    """
    n = len(bloomDay)
    if m * k > n:
        return -1
    
    def feasible(day: int) -> bool:
        bouquets = 0
        consecutive = 0
        for bloom in bloomDay:
            if bloom <= day:
                consecutive += 1
                if consecutive == k:
                    bouquets += 1
                    consecutive = 0
            else:
                consecutive = 0
        return bouquets >= m
    
    lo, hi = 1, max(bloomDay)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid + 1
    
    return lo

# Time: O(n log(max_bloom))  Space: O(1)
```

### Problem 5: Magnetic Force Between Two Balls — Hard

**Problem**: Place m balls in n positions maximizing the MINIMUM distance between any two balls.

```python
def max_distance(position: list, m: int) -> int:
    """
    Maximize the minimum distance between balls.
    
    Monotone: if minimum distance d is achievable, d-1 is also achievable.
    Find MAXIMUM d such that we can place m balls with minimum gap ≥ d.
    
    Feasibility: greedily place balls at valid positions (gap ≥ d from last placed).
    
    NOTICE: This is a MAXIMIZATION problem → use Template 2.
    
    Search space: [1, (max_pos - min_pos) / (m-1)]
    - Tightest possible: all balls adjacent → gap = 1
    - Loosest possible: balls at endpoints + evenly spaced
    """
    position.sort()
    n = len(position)
    
    def feasible(min_dist: int) -> bool:
        """Can we place m balls with minimum gap ≥ min_dist?"""
        count = 1   # Place first ball at position[0]
        last_placed = position[0]
        
        for i in range(1, n):
            if position[i] - last_placed >= min_dist:
                count += 1
                last_placed = position[i]
                if count == m:
                    return True
        
        return count >= m
    
    lo, hi = 1, (position[-1] - position[0]) // (m - 1)
    
    while lo < hi:
        mid = lo + (hi - lo + 1) // 2   # Template 2: +1 to avoid stalling
        if feasible(mid):
            lo = mid   # Works; try larger minimum distance
        else:
            hi = mid - 1
    
    return lo

# Time: O(n log n) sort + O(n log(max_gap)) search = O(n log n)  Space: O(1)
```

### Problem 6: Find Kth Smallest in Sorted Matrix — Hard

**Problem**: n×n matrix where rows and columns are sorted. Find kth smallest element.

```python
def kth_smallest_matrix(matrix: list, k: int) -> int:
    """
    Binary search on VALUE (not index).
    
    For each candidate value X, count elements ≤ X in the matrix.
    This count is monotone: larger X → more elements ≤ X.
    
    Find MINIMUM X such that count(≤ X) ≥ k.
    
    Counting elements ≤ X in sorted matrix: O(n)
    - Start at top-right corner
    - If matrix[r][c] ≤ X: all elements in column c of rows 0..r are ≤ X (count r+1)
    - Move right → go down (count r+1 in current column), then move left
    - If matrix[r][c] > X: move left
    
    Wait — standard approach for sorted-rows-and-columns matrix:
    Start at bottom-left: move right if ≤ X, move up if > X.
    
    IMPORTANT: The answer X must be an actual element in the matrix.
    Binary search finds the smallest X (possibly not in matrix) such that
    count(≤ X) ≥ k. The answer is guaranteed to be an actual element because:
    the kth smallest IS an actual element, and the count "jumps" at element values.
    """
    n = len(matrix)
    
    def count_less_equal(mid: int) -> int:
        """Count elements ≤ mid in O(n) using sorted structure."""
        count = 0
        row, col = n-1, 0   # Start at bottom-left
        while row >= 0 and col < n:
            if matrix[row][col] <= mid:
                count += row + 1   # All elements in this column up to row
                col += 1
            else:
                row -= 1
        return count
    
    lo, hi = matrix[0][0], matrix[n-1][n-1]
    
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if count_less_equal(mid) >= k:
            hi = mid     # mid or smaller
        else:
            lo = mid + 1
    
    return lo

# Time: O(n log(max-min))  Space: O(1)
# The answer is always a matrix element — when lo converges, count(lo-1) < k ≤ count(lo)
# meaning lo is the exact kth smallest.
```

### Problem 7: Allocate Minimum Pages (Painter's Partition) — Hard

**Problem**: Allocate n books to m students; each student reads contiguous books. Minimize maximum pages read by any student.

```python
def allocate_pages(pages: list, m: int) -> int:
    """
    Identical structure to Split Array Largest Sum (Problem 3).
    Minimize the maximum pages any student reads.
    
    Feasibility: can m students read all books with max ≤ X pages each?
    Greedy: assign books to current student until adding next would exceed X.
    
    Edge cases:
    - m > n: impossible (need n students, have more than n books? No — need each
             student to get at least 1 book, but m can be up to n). Actually if m > n,
             the extra students read 0 books and answer = max single book.
    - Single student: must read all books → answer = sum(pages)
    """
    if m > len(pages):
        return max(pages)   # Extra students read 0; max single book
    
    def feasible(max_pages: int) -> bool:
        students = 1
        current = 0
        for p in pages:
            if p > max_pages:
                return False
            if current + p > max_pages:
                students += 1
                current = 0
            current += p
        return students <= m
    
    lo, hi = max(pages), sum(pages)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid + 1
    
    return lo

# Time: O(n log(sum))  Space: O(1)
```

---

## Advanced Variations

### Aggressive Cows (Classic Variation)

```python
def aggressive_cows(stalls: list, cows: int) -> int:
    """
    Place cows in stalls maximizing minimum distance between any two cows.
    Identical pattern to Magnetic Force Between Balls (Problem 5).
    
    This is the original problem that popularized "binary search on answer."
    """
    stalls.sort()
    n = len(stalls)
    
    def feasible(min_dist: int) -> bool:
        placed = 1
        last = stalls[0]
        for i in range(1, n):
            if stalls[i] - last >= min_dist:
                placed += 1
                last = stalls[i]
                if placed == cows: return True
        return placed >= cows
    
    lo, hi = 1, stalls[-1] - stalls[0]
    while lo < hi:
        mid = lo + (hi - lo + 1) // 2
        if feasible(mid): lo = mid
        else: hi = mid - 1
    return lo

# Time: O(n log n) sort + O(n log(range))  Space: O(1)
```

### Minimize Maximum Value in Array

```python
def minimize_max(nums: list, p: int) -> int:
    """
    Select p pairs of adjacent elements; replace each pair with min(pair).
    Minimize the maximum element remaining.
    
    Wait — this is a different formulation. Let's use a cleaner advanced example:
    
    After selecting p non-overlapping adjacent pairs and subtracting their difference,
    minimize the resulting maximum difference. (LeetCode 2616)
    """
    nums.sort()
    
    def feasible(max_diff: int) -> bool:
        """Can we select p non-overlapping pairs with difference ≤ max_diff?"""
        pairs = 0
        i = 0
        while i < len(nums) - 1:
            if nums[i+1] - nums[i] <= max_diff:
                pairs += 1
                i += 2   # Skip both elements (non-overlapping)
            else:
                i += 1
        return pairs >= p
    
    lo, hi = 0, nums[-1] - nums[0]
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if feasible(mid):
            hi = mid
        else:
            lo = mid + 1
    return lo

# Time: O(n log n + n log(max_diff))  Space: O(1)
```

### Binary Search on Answer for Weighted Median

```python
def find_best_meeting_point(positions: list, weights: list) -> int:
    """
    Find position x minimizing Σ weights[i] × |positions[i] - x|.
    (Weighted median minimizes weighted sum of absolute deviations.)
    
    Binary search approach for integer positions:
    - f(x) is convex → unimodal → ternary search or direct weighted median
    - With weights, the optimal x is the weighted median.
    
    Direct solution (O(n log n)):
    Sort by position. Find median position weighted by weights.
    Binary search approach for demonstration.
    """
    sorted_pairs = sorted(zip(positions, weights))
    total_weight = sum(weights)
    
    def cost(x: int) -> int:
        return sum(w * abs(p - x) for p, w in sorted_pairs)
    
    # The cost function is convex; ternary search finds minimum
    lo, hi = min(positions), max(positions)
    while lo < hi:
        mid = lo + (hi - lo) // 2
        if cost(mid) <= cost(mid + 1):
            hi = mid
        else:
            lo = mid + 1
    return lo

# Time: O(n log n + n log(range))  Space: O(n)
```

---

## Edge Cases Bible

1. **lo and hi initialization**: lo must be a valid lower bound (could be the answer). hi must be above all valid answers. Wrong initialization → answer lies outside search range → incorrect result.

2. **Template 2 (maximize) infinite loop**: Using `mid = (lo + hi) // 2` in Template 2 causes infinite loop when lo+1=hi and feasible(lo) is True — mid=lo, lo stays lo. Fix: `mid = lo + (hi - lo + 1) // 2`.

3. **Integer overflow in mid computation**: `(lo + hi) // 2` overflows in languages with fixed-width integers when lo+hi > INT_MAX. Always use `lo + (hi - lo) // 2`.

4. **Off-by-one in feasibility count**: In ship-within-days, starting with `days_used=1` (not 0) is critical. If you start with 0 and count each "start new day" differently, you get off-by-one errors.

5. **Non-integer answer for matrix kth smallest**: The binary search converges to the kth smallest ELEMENT because the count function is a step function jumping at element values. When lo = an actual element and count(lo-1) < k ≤ count(lo), lo is the answer.

6. **Empty array edge cases**: `max(weights)` and `sum(weights)` on empty list raise ValueError. Always check `if not arr: return 0` or handle edge case first.

7. **m=1 for split array**: Answer is sum(nums) — one group contains everything. lo = max(nums), hi = sum(nums), feasible(sum) is always True. The loop immediately sets lo = sum(nums).

8. **Identical elements in sorted positions**: For magnetic force problem, if all positions are the same, distance = 0. Handle: if m=1, answer is 0 (any single position works). If m>1 and n < m: impossible.

9. **Float search precision**: For float binary search (e.g., square root), using `while hi - lo > 1e-9` can cause infinite loops if the precision isn't achievable. Use fixed iteration count instead.

10. **Kth smallest count ambiguity**: `count_less_equal(mid) >= k` vs `> k`. Using `>=` ensures we find the minimum X with at least k elements ≤ X. The convergence guarantees this is an actual matrix element.

---

## Interview Tips

### What Interviewers Look For

1. **Recognize the pattern immediately**: When a problem says "minimize the maximum" or "maximize the minimum," say "This is binary search on the answer space." Then identify the search space and feasibility check.

2. **State the monotone property explicitly**: "If capacity C allows shipping in D days, then capacity C+1 also allows it. So the feasibility function is monotonically non-decreasing." This is the proof of correctness.

3. **Derive the search space tightly**: "Lower bound is max(weights) — we must fit each package individually. Upper bound is sum(weights) — we can always ship everything in one day." Tight bounds → fewer iterations.

4. **The "minimize max = maximize min" duality**:
   - "Minimize maximum sum of subarray" (split array, ship packages, allocate pages) all use the SAME template.
   - "Maximize minimum distance" (magnetic force, aggressive cows) are the dual — use Template 2 (maximize).
   - Interviewers often give one type in multiple disguises.

5. **Complexity statement**: "Binary search over the answer space does O(log(range)) iterations. Each iteration runs the O(n) feasibility check. Total: O(n log range)."

6. **Template 2 pitfall**: "For maximization, I use `mid = lo + (hi - lo + 1) // 2` — the `+1` prevents infinite looping when lo+1=hi and the current mid would equal lo repeatedly."

7. **The greedy feasibility check**: The feasibility check almost always uses a greedy scan: "Iterate through elements, greedily assign to current group/day/ship until limit exceeded, then start new group." This greedy is optimal for these problems because:
   - We're checking existence (not optimizing), so greedy suffices.
   - Starting a new group as late as possible minimizes the number of groups.

8. **Common interview disguises of this pattern**:
   - "Minimum bandwidth to stream video" → binary search on bandwidth, check if all segments fit in time limit
   - "Minimum number of workers needed" → binary search on tasks per worker, feasibility check
   - "Kth largest/smallest across sorted lists" → binary search on value, rank counting

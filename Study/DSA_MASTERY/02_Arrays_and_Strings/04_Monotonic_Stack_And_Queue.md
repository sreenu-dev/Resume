# Monotonic Stack & Queue — Mastery Guide

## Core Concept & Invariant

A **monotonic stack** maintains elements in strictly increasing or decreasing order.
The key invariant: **every element is pushed and popped at most once** → O(n) total.

**Monotonic Increasing Stack**: stack top is the minimum of remaining candidates.
When a smaller element arrives, all larger elements on top are popped (they'll never be the "next smaller" for any future element because this smaller element is closer and smaller).

**Monotonic Decreasing Stack**: stack top is the maximum. When a larger element arrives, all smaller elements are popped.

**The O(n) proof**: Each element is pushed exactly once. Popping can happen multiple times per step, but total pops across ALL steps ≤ total pushes = n. Hence total operations = O(n).

**Three canonical applications**:
1. **Next Greater/Smaller Element** — find NGE for every array element
2. **Contribution technique** — sum of subarray mins/maxes
3. **Histogram / rectangle problems** — largest rectangle under profile

**Monotonic Deque** extends this to **sliding window extrema** by additionally removing front when outside window.

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Next Greater Element (Monotonic Decreasing Stack)
# ─────────────────────────────────────────────────────────────
def next_greater_element(arr: list) -> list:
    """
    For each i, find the first index j > i with arr[j] > arr[i].
    
    Stack stores indices of elements waiting for their NGE.
    When arr[i] > arr[stack.top()], stack.top() found its NGE at i.
    
    Invariant: stack is monotonically DECREASING (values from bottom to top).
    If stack were to have an increasing pair (a < b), b would never see 
    a future NGE before a does — so b is useless and should have been popped.
    """
    n = len(arr)
    result = [-1] * n
    stack = []   # Stores indices
    
    for i in range(n):
        # Pop all elements for which arr[i] is the NGE
        while stack and arr[stack[-1]] < arr[i]:
            idx = stack.pop()
            result[idx] = arr[i]   # or i for the index
        stack.append(i)
    # Remaining elements in stack have no NGE → result[idx] = -1 (already set)
    return result

# ─────────────────────────────────────────────────────────────
# Template 2: Previous Greater Element (Monotonic Decreasing Stack)
# ─────────────────────────────────────────────────────────────
def previous_greater_element(arr: list) -> list:
    """
    For each i, find last j < i with arr[j] > arr[i].
    Process LEFT to RIGHT, maintaining decreasing stack.
    """
    n = len(arr)
    result = [-1] * n
    stack = []
    
    for i in range(n):
        while stack and arr[stack[-1]] <= arr[i]:
            stack.pop()
        result[i] = arr[stack[-1]] if stack else -1
        stack.append(i)
    return result

# ─────────────────────────────────────────────────────────────
# Template 3: Monotonic Deque (Sliding Window Max)
# ─────────────────────────────────────────────────────────────
from collections import deque

def sliding_window_max(arr: list, k: int) -> list:
    """
    Deque stores indices in DECREASING value order.
    Front = index of window maximum.
    Remove front if outside window, remove back if smaller than new element.
    """
    dq = deque()
    result = []
    for i in range(len(arr)):
        while dq and dq[0] <= i - k:
            dq.popleft()
        while dq and arr[dq[-1]] <= arr[i]:
            dq.pop()
        dq.append(i)
        if i >= k - 1:
            result.append(arr[dq[0]])
    return result
```

---

## Complexity Analysis

| Problem | Time | Space | Pattern |
|---------|------|-------|---------|
| Next/Prev Greater/Smaller | O(n) | O(n) | Monotonic stack |
| Largest Rect Histogram | O(n) | O(n) | Monotonic stack |
| Trapping Rain Water | O(n) | O(n) or O(1) | Stack or two-pointer |
| Sum of Subarray Mins | O(n) | O(n) | Contribution + stack |
| Sliding Window Max | O(n) | O(k) | Monotonic deque |
| Constrained Subsequence Sum | O(n) | O(k) | Monotonic deque + DP |
| 132 Pattern | O(n) | O(n) | Monotonic stack |

---

## Classic Problems

### Problem 1: Largest Rectangle in Histogram — Hard (Full Derivation)

**Problem**: Find largest rectangle area in histogram.

```python
def largest_rectangle_histogram(heights: list) -> int:
    """
    Key insight: for each bar h[i], what's the widest rectangle with height h[i]?
    Width = (right_boundary[i] - left_boundary[i] - 1)
    where right_boundary[i] = first index r > i with h[r] < h[i]
         left_boundary[i]  = last index l < i with h[l] < h[i]
    
    These are Previous Smaller and Next Smaller Elements → monotonic stack.
    
    One-pass algorithm (compute both boundaries simultaneously):
    - Maintain increasing stack (bottom to top)
    - When h[i] < h[stack.top()]: stack.top() found its RIGHT boundary at i
      LEFT boundary of stack.top() = new stack.top() after popping (previous smaller)
    
    The rectangle for popped element mid:
    - height = h[mid]
    - left = stack.top() (previous smaller, -1 if empty)
    - right = i (current element, which is smaller)
    - width = right - left - 1
    """
    heights = heights + [0]   # Sentinel: forces all elements to be popped
    stack = [-1]              # Sentinel: left boundary for first element
    best = 0
    
    for i, h in enumerate(heights):
        while stack[-1] != -1 and heights[stack[-1]] >= h:
            height = heights[stack.pop()]
            width = i - stack[-1] - 1   # stack[-1] is now the new left boundary
            best = max(best, height * width)
        stack.append(i)
    
    return best

# Time: O(n) — each index pushed/popped exactly once
# Space: O(n) for stack

def largest_rectangle_histogram_verbose(heights: list) -> int:
    """Same algorithm with explicit boundary arrays for clarity."""
    n = len(heights)
    # Next Smaller to the Left (NSL)
    nsl = [-1] * n
    stack = []
    for i in range(n):
        while stack and heights[stack[-1]] >= heights[i]:
            stack.pop()
        nsl[i] = stack[-1] if stack else -1
        stack.append(i)
    
    # Next Smaller to the Right (NSR)
    nsr = [n] * n
    stack = []
    for i in range(n-1, -1, -1):
        while stack and heights[stack[-1]] >= heights[i]:
            stack.pop()
        nsr[i] = stack[-1] if stack else n
        stack.append(i)
    
    best = 0
    for i in range(n):
        width = nsr[i] - nsl[i] - 1
        best = max(best, heights[i] * width)
    return best

def maximal_rectangle(matrix: list) -> int:
    """
    Maximal rectangle in binary matrix.
    Reduce to histogram problem row by row.
    
    For each row, compute cumulative heights: heights[j] = consecutive 1s above.
    Then apply largest_rectangle_histogram.
    """
    if not matrix or not matrix[0]: return 0
    m, n = len(matrix), len(matrix[0])
    heights = [0] * n
    best = 0
    
    for row in matrix:
        for j in range(n):
            heights[j] = heights[j] + 1 if row[j] == '1' else 0
        best = max(best, largest_rectangle_histogram(heights[:]))
    
    return best
# Time: O(mn)  Space: O(n)
```

### Problem 2: Sum of Subarray Minimums — Hard (Contribution Technique)

**Problem**: Sum of min(subarray) over all subarrays.

```python
def sum_subarray_mins(arr: list) -> int:
    """
    Contribution technique:
    For each element arr[i], count how many subarrays have arr[i] as their minimum.
    
    arr[i] is the minimum of subarray [l+1, r] if:
    - Previous Smaller or Equal element is at l (exclusive left boundary)
    - Next Smaller element is at r (exclusive right boundary)
    
    left[i] = i - prev_smaller[i]  (number of ways to choose left endpoint)
    right[i] = next_smaller[i] - i (number of ways to choose right endpoint)
    
    Contribution of arr[i] = arr[i] × left[i] × right[i]
    
    IMPORTANT: Use STRICT smaller on one side and NON-STRICT on the other
    to avoid double-counting when equal elements exist.
    Convention: prev <= (prev_smaller_or_equal), next < (next_strictly_smaller)
    
    Why this avoids double-counting: for equal elements arr[i] == arr[j] (i < j),
    the subarray containing both will be counted by j (since j's prev_smaller is i,
    meaning i is the "left" boundary, so j "owns" the range [i..j]).
    """
    MOD = 10**9 + 7
    n = len(arr)
    
    # Previous Smaller or Equal (using ≤ to handle duplicates)
    prev = [-1] * n
    stack = []
    for i in range(n):
        while stack and arr[stack[-1]] > arr[i]:   # strictly greater → pop
            stack.pop()
        prev[i] = stack[-1] if stack else -1
        stack.append(i)
    
    # Next Smaller (strictly)
    nxt = [n] * n
    stack = []
    for i in range(n-1, -1, -1):
        while stack and arr[stack[-1]] >= arr[i]:  # greater or equal → pop
            stack.pop()
        nxt[i] = stack[-1] if stack else n
        stack.append(i)
    
    result = 0
    for i in range(n):
        left = i - prev[i]     # Number of valid left choices
        right = nxt[i] - i     # Number of valid right choices
        result = (result + arr[i] * left * right) % MOD
    
    return result

# Time: O(n)  Space: O(n)

def sum_subarray_maxs(arr: list) -> int:
    """
    Symmetric: contribution of each arr[i] as subarray maximum.
    Use next greater (strictly) and previous greater or equal.
    """
    MOD = 10**9 + 7
    n = len(arr)
    
    # Previous Greater or Equal
    prev = [-1] * n
    stack = []
    for i in range(n):
        while stack and arr[stack[-1]] < arr[i]:
            stack.pop()
        prev[i] = stack[-1] if stack else -1
        stack.append(i)
    
    # Next Strictly Greater
    nxt = [n] * n
    stack = []
    for i in range(n-1, -1, -1):
        while stack and arr[stack[-1]] <= arr[i]:
            stack.pop()
        nxt[i] = stack[-1] if stack else n
        stack.append(i)
    
    result = 0
    for i in range(n):
        left = i - prev[i]
        right = nxt[i] - i
        result = (result + arr[i] * left * right) % MOD
    return result
```

### Problem 3: Trapping Rain Water — Stack Approach — Hard

```python
def trap_stack(height: list) -> int:
    """
    Horizontal layer approach via monotonic stack.
    Stack stores indices; maintains DECREASING heights.
    
    When height[i] > height[stack[-1]], a "valley" is complete:
    - bottom = stack.pop() (the lowest point of the valley)
    - left_wall = stack[-1] (left boundary — after popping bottom)
    - height of water = min(height[left_wall], height[i]) - height[bottom]
    - width = i - left_wall - 1
    
    This computes water layer by layer (horizontal strips).
    Compare with two-pointer (vertical columns) — same O(n) but different approach.
    
    Advantage of stack: more intuitive for understanding 3D water filling.
    Advantage of two-pointer: O(1) space.
    """
    stack = []
    water = 0
    
    for i, h in enumerate(height):
        while stack and height[stack[-1]] < h:
            bottom_idx = stack.pop()
            if not stack: break
            
            left_idx = stack[-1]
            width = i - left_idx - 1
            bounded_height = min(height[left_idx], h) - height[bottom_idx]
            water += bounded_height * width
        
        stack.append(i)
    
    return water
# Time: O(n)  Space: O(n)
```

### Problem 4: Daily Temperatures — Medium

```python
def daily_temperatures(temperatures: list) -> list:
    """
    Find how many days until a warmer temperature.
    Classic NGE problem with day-count answer.
    
    Stack stores indices of days waiting for warmer day.
    When temp[i] > temp[stack[-1]], the wait is over: result = i - stack[-1].
    """
    n = len(temperatures)
    result = [0] * n
    stack = []
    
    for i, temp in enumerate(temperatures):
        while stack and temperatures[stack[-1]] < temp:
            j = stack.pop()
            result[j] = i - j
        stack.append(i)
    
    return result
# Time: O(n)  Space: O(n)
```

### Problem 5: Remove K Digits to Get Minimum Number — Medium

```python
def remove_k_digits(num: str, k: int) -> str:
    """
    Remove k digits to get minimum possible number.
    
    Greedy with monotonic stack:
    Maintain an increasing stack (digits form the result from left to right).
    When digit is smaller than stack top AND k > 0: pop (remove) the larger digit.
    
    Why greedy works: removing a digit d[i] where d[i] > d[i+1] always
    produces a smaller number than not removing it. The leftmost such pair
    should be resolved first (higher-order digits matter more).
    
    Edge cases: leading zeros, k unused after processing all digits.
    """
    stack = []
    
    for digit in num:
        # Pop larger digits (greedy: smaller digit at higher position is better)
        while k > 0 and stack and stack[-1] > digit:
            stack.pop()
            k -= 1
        stack.append(digit)
    
    # If k remaining: remove from end (stack is non-decreasing at this point)
    if k > 0:
        stack = stack[:-k]
    
    # Remove leading zeros
    result = ''.join(stack).lstrip('0')
    return result if result else '0'

# Time: O(n)  Space: O(n)
```

### Problem 6: 132 Pattern — Hard

**Problem**: Find indices i < j < k such that arr[i] < arr[k] < arr[j].

```python
def find_132_pattern(nums: list) -> bool:
    """
    132 pattern: need a "1" (minimum on left), "3" (tall middle), "2" (between 1 and 3).
    
    Scan RIGHT to LEFT with monotonic stack:
    - Stack stores candidates for the "2" (the middle-height element)
    - third = -inf: current best "2" candidate (element after "3" in future left scan)
    - For each element nums[i] going right-to-left:
      - If nums[i] < third: found the "1" → pattern exists!
      - While stack top < nums[i]: pop → update third (this is a "2" candidate)
      - Push nums[i] as potential "3"
    
    Why this works:
    - We process potential "3"s from right to left
    - When we pop smaller elements (potential "2"s), we save the largest popped as 'third'
    - third is always < the element that caused it to be popped (which is a valid "3")
    - If we ever find nums[i] < third while processing left, we have 1 < 2 < 3 pattern
    """
    stack = []
    third = float('-inf')   # The "2" in 132 pattern — best candidate so far
    
    for num in reversed(nums):
        if num < third:
            return True     # Found "1" (num) < "2" (third) < "3" (what popped third)
        while stack and stack[-1] < num:
            third = stack.pop()   # This element is a valid "2" for this "3" (num)
        stack.append(num)   # Push as potential "3"
    
    return False
# Time: O(n)  Space: O(n)
```

### Problem 7: Asteroid Collision — Medium

```python
def asteroid_collision(asteroids: list) -> list:
    """
    Positive = moves right, negative = moves left.
    Collision: positive asteroid followed (on stack) by negative asteroid.
    Larger magnitude wins; equal → both destroyed.
    
    Stack maintains asteroids that survive so far.
    
    Cases when processing asteroid a:
    - a > 0: push (moving right, no immediate collision with right-movers)
    - a < 0: collide with positive asteroids on stack
      - |a| > stack[-1]: stack[-1] destroyed, continue
      - |a| == stack[-1]: both destroyed, stop
      - |a| < stack[-1]: a destroyed, stop
      - stack empty or stack[-1] < 0: a survives, push
    """
    stack = []
    
    for a in asteroids:
        alive = True
        while alive and a < 0 and stack and stack[-1] > 0:
            if stack[-1] < -a:
                stack.pop()   # Right-mover destroyed
            elif stack[-1] == -a:
                stack.pop()   # Both destroyed
                alive = False
            else:
                alive = False  # Left-mover (a) destroyed
        if alive:
            stack.append(a)
    
    return stack
# Time: O(n) — each asteroid pushed/popped at most once
# Space: O(n)
```

---

## Advanced Variations

### Maximum Width Ramp

```python
def max_width_ramp(nums: list) -> int:
    """
    Find maximum j - i where i < j and nums[i] <= nums[j].
    
    Two-pass monotonic approach:
    1. Build decreasing stack of candidates for i (decreasing from left)
       - Only elements that could be the minimum for some ramp
       - If nums[i] ≥ nums[j] for j < i, then j is strictly better as left endpoint
    2. Scan from right: for each j, pop stack while nums[stack[-1]] <= nums[j]
       - Greedy: use the rightmost valid j first to maximize width
    """
    n = len(nums)
    # Build decreasing stack of left candidates
    candidates = []
    for i in range(n):
        if not candidates or nums[candidates[-1]] > nums[i]:
            candidates.append(i)
    
    best = 0
    # Scan from right, match with leftmost valid candidate
    for j in range(n-1, -1, -1):
        while candidates and nums[candidates[-1]] <= nums[j]:
            best = max(best, j - candidates[-1])
            candidates.pop()
    
    return best
# Time: O(n)  Space: O(n)
```

### Online Stock Span

```python
class StockSpanner:
    """
    Span of price on day i = max consecutive days (ending at i) where price ≤ price[i].
    
    Monotonic stack stores (price, span) pairs.
    When new price arrives, pop all smaller prices and accumulate their spans.
    
    This efficiently computes "how far back can we go?" in O(1) amortized per call.
    Each price is pushed and popped at most once → O(n) total for n calls.
    """
    def __init__(self):
        self.stack = []   # (price, span) pairs
    
    def next(self, price: int) -> int:
        span = 1
        while self.stack and self.stack[-1][0] <= price:
            span += self.stack.pop()[1]   # Absorb the span of smaller prices
        self.stack.append((price, span))
        return span
# Time: O(1) amortized per call  Space: O(n) total
```

### Constrained Subsequence Sum (Monotonic Deque DP)

```python
def constrained_subset_sum(nums: list, k: int) -> int:
    """
    Maximum sum of non-empty subsequence where consecutive indices differ by ≤ k.
    
    dp[i] = max sum subsequence ending at i
    dp[i] = nums[i] + max(0, max(dp[max(0,i-k)..i-1]))
    
    Naive: O(nk). With sliding window max on dp values: O(n).
    
    Deque stores indices in DECREASING dp value order.
    Front = index with max dp value in window [i-k, i-1].
    """
    n = len(nums)
    dp = nums[:]
    dq = deque()
    
    for i in range(n):
        # Remove front if outside window
        while dq and dq[0] < i - k:
            dq.popleft()
        
        # Use max dp in window (if positive)
        if dq and dp[dq[0]] > 0:
            dp[i] += dp[dq[0]]
        
        # Maintain decreasing deque
        while dq and dp[dq[-1]] <= dp[i]:
            dq.pop()
        dq.append(i)
    
    return max(dp)
# Time: O(n)  Space: O(k) for deque
```

### Number of Visible People in Queue

```python
def can_see_persons_count(heights: list) -> list:
    """
    Person i can see person j (i < j) if no one between them is taller than both.
    
    Person i sees person j if: max(heights[i+1..j-1]) < min(heights[i], heights[j])
    
    Monotonic stack approach:
    Process right to left. For each person i, scan right using decreasing stack.
    i can see j if heights[j] was visible from i's perspective.
    """
    n = len(heights)
    result = [0] * n
    stack = []
    
    for i in range(n-1, -1, -1):
        count = 0
        while stack and heights[stack[-1]] < heights[i]:
            count += 1
            stack.pop()
        # Can also see the first person taller (or equal) than heights[i]
        if stack:
            count += 1
        result[i] = count
        stack.append(i)
    
    return result
# Time: O(n)  Space: O(n)
```

---

## Edge Cases Bible

1. **Sentinel values**: Adding a 0 at the end of histogram forces all elements to be popped. Adding -1 to the stack at start serves as a left boundary sentinel. Both are critical for clean code.

2. **Equal elements in contribution technique**: When computing sum of subarray mins with equal elements, one side must use strict inequality and the other non-strict. Inconsistency leads to double-counting or missing subarrays.

3. **132 pattern direction**: Must scan RIGHT TO LEFT. Scanning left to right misses the key insight about maintaining the "2" candidate.

4. **Remove K digits — all same digits**: e.g., "1111", k=2 → "11". Stack never pops (no element > next). Handle by slicing `stack[:-k]`.

5. **Asteroid collision — all same direction**: All positive or all negative → no collisions. Code handles this because the while loop condition `a < 0 and stack and stack[-1] > 0` prevents false pops.

6. **Sliding window max — k=1**: Every element is both the window's start and end. Result = nums itself. Deque works correctly but verify the `if i >= k-1` condition.

7. **Empty stack for left boundary**: In histogram problem, when stack is empty after popping, width = i (entire array from start to i). Sentinel -1 in stack handles this: width = i - (-1) - 1 = i.

8. **Monotonic stack with duplicates**: For NGE, if arr = [3,3,3], each element sees no greater element. Stack pops with strict `<`, so duplicates remain. Verify whether problem uses strict or non-strict NGE.

9. **Stock spanner resets**: If StockSpanner is used, the stack state persists across calls. This is intentional — each call is a new trading day and the stack represents history.

10. **Sum of subarray mins modular arithmetic**: The result can be huge (O(n² × max_val)). Always apply modulo before overflow. In Python this is not an issue, but mention it in interviews for production code.

---

## Interview Tips

### What Interviewers Look For

1. **State the stack invariant**: "I maintain a monotonically increasing stack (values from bottom to top). When a smaller element arrives, I pop everything larger — those elements have found their Next Smaller Element."

2. **The O(n) argument**: "Although the while loop looks like it could be O(n) per iteration, across the entire array, each element is pushed exactly once and popped at most once. So total operations are O(2n) = O(n)." Stating this proactively shows sophistication.

3. **Contribution technique**: This is a high-signal technique. "For each element arr[i], I find its left and right boundaries (Previous/Next Smaller Element). It's the minimum of exactly left×right subarrays, contributing arr[i]×left×right to the total." Interviewers love this.

4. **Histogram → matrix reduction**: "I notice this is a matrix problem that reduces to histogram on each row. I can maintain cumulative heights and apply largest-rectangle-in-histogram per row in O(n) time."

5. **When to use stack vs deque**: 
   - Stack only: NGE/NSE, histogram, contribution technique (no window constraint)
   - Deque: sliding window extrema (need to expire old elements from front)

6. **132 pattern trick**: "The key insight is scanning right to left. 'Third' stores the best '2' candidate — the largest element we've popped from the stack. If we find any element smaller than 'third', we've found the '1'."

7. **Avoid the O(n²) approach**: Common wrong answer is "for each element, scan left/right for boundaries." Always push toward the O(n) stack solution by stating "this is a monotonic stack problem — I can find all boundaries in O(n) total."

8. **Common mistakes to avoid**:
   - Forgetting the sentinel (0) at the end of histogram array
   - Not handling the "remove from end" case in Remove K Digits when k is not exhausted
   - In 132 pattern: checking `num <= third` vs `num < third` (the third must be strictly between num and the "3")

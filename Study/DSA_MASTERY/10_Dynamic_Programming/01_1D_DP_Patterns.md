# 1D Dynamic Programming Patterns
## Advanced Mastery — FAANG Interview Preparation

> **Prerequisite:** Basic DP familiarity. This file focuses on subtle state design, edge cases, and the transition proof for each problem.

---

## Table of Contents
1. [House Robber I — State Transition Proof](#1-house-robber-i)
2. [House Robber II — Circular Array](#2-house-robber-ii-circular)
3. [House Robber III — Tree DP Preview](#3-house-robber-iii)
4. [Jump Game I, II, III — DP vs Greedy](#4-jump-game-variants)
5. [Decode Ways — Handling Zeros](#5-decode-ways)
6. [Maximum Product Subarray — Track Min AND Max](#6-maximum-product-subarray)
7. [Paint Fence](#7-paint-fence)
8. [Student Attendance Record II — State Machine](#8-student-attendance-record-ii)
9. [Minimum Swaps to Make Sequences Increasing](#9-minimum-swaps)
10. [Fibonacci Space Optimization & Climbing Stairs](#10-fibonacci-and-climbing-stairs)
11. [Minimum Cost Climbing Stairs](#11-minimum-cost-climbing-stairs)

---

## 1. House Robber I

**Problem:** [LC 198] Given an array of non-negative integers, find the maximum sum such that no two selected elements are adjacent.

### State Definition

```
dp[i] = maximum money robbed from houses 0..i
```

### Recurrence Derivation (with proof)

At house `i`, we have exactly two choices:
- **Rob house i:** We cannot rob house `i-1`, so we get `nums[i] + dp[i-2]`
- **Skip house i:** Best we can do is `dp[i-1]`

```
dp[i] = max(nums[i] + dp[i-2], dp[i-1])
```

**Optimality proof:** Suppose OPT skips house `i` but picks both `i-1` and some earlier subset S. If OPT robs house `i`, it must skip `i-1`, but the best from houses `0..i-2` is `dp[i-2]`. Since we take the max, dp[i] captures both cases.

**Base cases:**
- `dp[0] = nums[0]`
- `dp[1] = max(nums[0], nums[1])`

```python
def rob(nums: list[int]) -> int:
    n = len(nums)
    if n == 1:
        return nums[0]
    if n == 2:
        return max(nums[0], nums[1])
    
    # Space-optimized: O(1) using two variables
    prev2 = nums[0]
    prev1 = max(nums[0], nums[1])
    
    for i in range(2, n):
        curr = max(nums[i] + prev2, prev1)
        prev2 = prev1
        prev1 = curr
    
    return prev1

# Test
print(rob([1, 2, 3, 1]))   # 4  (rob index 0 and 2)
print(rob([2, 7, 9, 3, 1]))  # 12 (rob index 0, 2, 4)
```

> **Time:** O(N) — single pass  
> **Space:** O(1) — two variables instead of full dp array

---

## 2. House Robber II — Circular

**Problem:** [LC 213] Same as House Robber I, but houses are in a circle (house 0 and house N-1 are adjacent).

### Key Insight: Two-Pass Technique

The circular constraint means: if we rob house 0, we cannot rob house N-1, and vice versa. Break the circle by running House Robber I **twice**:
- Pass 1: Houses `0..N-2` (exclude last house)
- Pass 2: Houses `1..N-1` (exclude first house)
- Answer: `max(pass1, pass2)`

**Why this is correct:** The optimal solution either includes house 0 or doesn't.
- If it includes house 0 → house N-1 is excluded → subproblem is houses `0..N-2`
- If it doesn't include house 0 → subproblem is houses `1..N-1`

Both cases are captured by taking the maximum.

```python
def rob_circular(nums: list[int]) -> int:
    def rob_linear(arr: list[int]) -> int:
        if not arr:
            return 0
        if len(arr) == 1:
            return arr[0]
        prev2, prev1 = arr[0], max(arr[0], arr[1])
        for i in range(2, len(arr)):
            curr = max(arr[i] + prev2, prev1)
            prev2, prev1 = prev1, curr
        return prev1
    
    n = len(nums)
    if n == 1:
        return nums[0]
    if n == 2:
        return max(nums[0], nums[1])
    
    # Two passes: exclude last OR exclude first
    return max(rob_linear(nums[:-1]), rob_linear(nums[1:]))

# Test
print(rob_circular([2, 3, 2]))  # 3
print(rob_circular([1, 2, 3, 1]))  # 4
print(rob_circular([1, 2, 3]))  # 3
```

> **Time:** O(N) — two linear passes  
> **Space:** O(1) — only constant extra space (slicing creates O(N) copies; use index ranges to avoid)

---

## 3. House Robber III — Tree DP Preview

**Problem:** [LC 337] Houses are nodes in a binary tree. Adjacent = parent-child. Maximize money with no adjacent node robbery.

### State Definition (2-State Tree DP)

```
For each node:
  rob[node]   = max money when we DO rob this node
  skip[node]  = max money when we DO NOT rob this node
```

### Recurrence

```
rob[node]  = node.val + skip[node.left] + skip[node.right]
skip[node] = max(rob[node.left], skip[node.left]) + max(rob[node.right], skip[node.right])
```

The answer is `max(rob[root], skip[root])`.

```python
from typing import Optional

class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def rob_tree(root: Optional[TreeNode]) -> int:
    def dfs(node):
        # Returns (rob_this_node, skip_this_node)
        if not node:
            return 0, 0
        
        left_rob, left_skip = dfs(node.left)
        right_rob, right_skip = dfs(node.right)
        
        rob_curr = node.val + left_skip + right_skip
        skip_curr = max(left_rob, left_skip) + max(right_rob, right_skip)
        
        return rob_curr, skip_curr
    
    rob, skip = dfs(root)
    return max(rob, skip)
```

> **Time:** O(N) — each node visited once  
> **Space:** O(H) — recursion stack depth = tree height (O(log N) balanced, O(N) worst)

---

## 4. Jump Game Variants

### Jump Game I — Can You Reach End? (Greedy is Better)

**Problem:** [LC 55] `nums[i]` = max jump length from index `i`. Can you reach the last index?

**DP approach:**
```
dp[i] = True if index i is reachable
dp[i] = any(dp[j] and j + nums[j] >= i for j in range(i))
```

**Greedy approach (O(N)):** Track the farthest index reachable so far.

```python
def can_jump_greedy(nums: list[int]) -> bool:
    max_reach = 0
    for i, jump in enumerate(nums):
        if i > max_reach:
            return False  # Can't reach index i
        max_reach = max(max_reach, i + jump)
    return True
```

> **Time:** O(N) | **Space:** O(1)

### Jump Game II — Minimum Jumps (Greedy BFS)

**Problem:** [LC 45] Minimum number of jumps to reach the last index.

**DP state:** `dp[i]` = minimum jumps to reach index `i`.  
`dp[i] = min(dp[j] + 1)` for all `j < i` where `j + nums[j] >= i`.  
This is O(N²).

**Greedy BFS (O(N)):** Think in "levels" like BFS. The farthest you can reach in `k` jumps defines one level.

```python
def jump_min(nums: list[int]) -> int:
    n = len(nums)
    jumps = 0
    curr_end = 0    # end of current jump's reachable range
    farthest = 0    # farthest reachable with one more jump
    
    for i in range(n - 1):  # don't need to jump FROM last index
        farthest = max(farthest, i + nums[i])
        if i == curr_end:
            # Must use a jump here
            jumps += 1
            curr_end = farthest
            if curr_end >= n - 1:
                break
    
    return jumps

# Test
print(jump_min([2, 3, 1, 1, 4]))  # 2
print(jump_min([2, 3, 0, 1, 4]))  # 2
```

> **Time:** O(N) | **Space:** O(1)

### Jump Game III — Can You Reach Zero? (BFS/DFS, not DP)

**Problem:** [LC 1306] From index `i`, you can jump to `i + nums[i]` or `i - nums[i]`. Can you reach any index with `nums[index] == 0`?

```python
from collections import deque

def can_reach_zero(arr: list[int], start: int) -> bool:
    n = len(arr)
    visited = set()
    queue = deque([start])
    
    while queue:
        i = queue.popleft()
        if arr[i] == 0:
            return True
        if i in visited:
            continue
        visited.add(i)
        for next_i in [i + arr[i], i - arr[i]]:
            if 0 <= next_i < n and next_i not in visited:
                queue.append(next_i)
    
    return False
```

> **Time:** O(N) | **Space:** O(N)

---

## 5. Decode Ways

**Problem:** [LC 91] Count the number of ways to decode a digit string (1→A, 2→B, ..., 26→Z).

### State Definition

```
dp[i] = number of ways to decode s[0..i-1] (first i characters)
```

### Recurrence — Critical Edge Cases

```
dp[i] = dp[i-1]  (if s[i-1] != '0', single-digit decode)
      + dp[i-2]  (if s[i-2:i] in "10".."26", two-digit decode)
```

**Edge cases that trip people up:**
1. `s[i-1] == '0'`: single-digit decode is INVALID (no letter 0)
2. `s[i-2] == '0'`: two-digit decode starting with 0 is INVALID (no "00", "01", etc.)
3. Two-digit code must be ≤ 26
4. Leading zeros make the entire string undecipherable

```python
def num_decodings(s: str) -> int:
    n = len(s)
    if not s or s[0] == '0':
        return 0
    
    # dp[i] = ways to decode s[:i]
    dp = [0] * (n + 1)
    dp[0] = 1       # empty string: 1 way (base case)
    dp[1] = 1       # s[0] is non-zero (checked above)
    
    for i in range(2, n + 1):
        # Single digit: s[i-1]
        one_digit = int(s[i-1])
        if one_digit != 0:      # '0' cannot be decoded alone
            dp[i] += dp[i-1]
        
        # Two digits: s[i-2:i]
        two_digit = int(s[i-2:i])
        if 10 <= two_digit <= 26:   # must be 10..26
            dp[i] += dp[i-2]
    
    return dp[n]

# Test
print(num_decodings("12"))     # 2  ("AB" or "L")
print(num_decodings("226"))    # 3  ("BZ","VF","BBF")
print(num_decodings("06"))     # 0  (leading zero invalid)
print(num_decodings("10"))     # 1  (only "J")
print(num_decodings("2101"))   # 1
```

> **Time:** O(N) | **Space:** O(N) — reducible to O(1) with two variables

### Decode Ways II — with '*' wildcard

**Problem:** [LC 639] Same but `'*'` can represent any digit 1-9.

```python
def num_decodings_with_wildcard(s: str) -> int:
    MOD = 10**9 + 7
    n = len(s)
    
    def ways_one(c):
        return 9 if c == '*' else (0 if c == '0' else 1)
    
    def ways_two(c1, c2):
        # count valid 2-digit codes using c1, c2
        if c1 == '*' and c2 == '*':
            return 15  # 11-19 (9) + 21-26 (6)
        if c1 == '*':
            d2 = int(c2)
            return 2 if d2 <= 6 else 1  # 1X and 2X if X<=6, else only 1X
        if c2 == '*':
            d1 = int(c1)
            if d1 == 1: return 9    # 11-19
            if d1 == 2: return 6    # 21-26
            return 0
        return 1 if 10 <= int(c1+c2) <= 26 else 0
    
    prev2, prev1 = 1, ways_one(s[0])
    for i in range(1, n):
        curr = (ways_one(s[i]) * prev1 + ways_two(s[i-1], s[i]) * prev2) % MOD
        prev2, prev1 = prev1, curr
    return prev1
```

> **Time:** O(N) | **Space:** O(1)

---

## 6. Maximum Product Subarray

**Problem:** [LC 152] Find the contiguous subarray with the largest product.

### Key Insight: Track Both Min and Max

Negative × Negative = Positive. So a large negative product can become large positive when multiplied by another negative. We must track:

```
max_ending_here[i] = maximum product subarray ending at index i
min_ending_here[i] = minimum product subarray ending at index i
```

### State Transition

```
max_dp[i] = max(nums[i],
               nums[i] * max_dp[i-1],   # extend positive streak
               nums[i] * min_dp[i-1])   # negative * negative = positive

min_dp[i] = min(nums[i],
               nums[i] * max_dp[i-1],   # positive becomes negative
               nums[i] * min_dp[i-1])   # extend negative streak
```

```python
def max_product(nums: list[int]) -> int:
    if not nums:
        return 0
    
    max_prod = min_prod = result = nums[0]
    
    for i in range(1, len(nums)):
        # When nums[i] is negative, max and min swap
        candidates = (nums[i], nums[i] * max_prod, nums[i] * min_prod)
        max_prod = max(candidates)
        min_prod = min(candidates)
        result = max(result, max_prod)
    
    return result

# Test
print(max_product([2, 3, -2, 4]))      # 6  ([2,3])
print(max_product([-2, 0, -1]))        # 0
print(max_product([-2, 3, -4]))        # 24 (all three: -2*3*-4)
print(max_product([-2, -3, -4]))       # 12 ([-3,-4] or [-2,-3])
```

> **Time:** O(N) | **Space:** O(1)

**Subtle pitfall:** At `nums[i]`, always consider `nums[i]` alone (fresh start), in case both `max_prod` and `min_prod` are negative — multiplying by negative gives positive, which might still be less than `nums[i]` itself.

---

## 7. Paint Fence

**Problem:** [LC 276] Paint `n` fence posts with `k` colors such that no more than 2 adjacent posts have the same color. Count the number of ways.

### State Definition

```
same[i]  = ways to paint post i the SAME color as post i-1
diff[i]  = ways to paint post i a DIFFERENT color from post i-1
```

### Recurrence

```
same[i]  = diff[i-1]                        (only allowed if prev two were different)
diff[i]  = (same[i-1] + diff[i-1]) * (k-1)  (any of k-1 colors different from post i-1)
```

Total ways at post `i` = `same[i] + diff[i]`.

```python
def num_ways_paint(n: int, k: int) -> int:
    if n == 0 or k == 0:
        return 0
    if n == 1:
        return k
    
    same = k          # post 1 = post 0: k ways (same color for both)
    # Wait: for i=1, same means posts 0 and 1 are same: k ways
    # diff means posts 0 and 1 differ: k*(k-1) ways
    same = k          # k choices for post 0, then 1 way to match
    diff = k * (k - 1)  # k choices for post 0, k-1 choices for post 1
    
    for i in range(2, n):
        prev_same, prev_diff = same, diff
        same = prev_diff              # can only continue same if prev was diff
        diff = (prev_same + prev_diff) * (k - 1)
    
    return same + diff

# Test
print(num_ways_paint(3, 2))   # 6
print(num_ways_paint(1, 1))   # 1
print(num_ways_paint(7, 2))   # 42
```

> **Time:** O(N) | **Space:** O(1)

---

## 8. Student Attendance Record II

**Problem:** [LC 552] Count strings of length `n` over {A, L, P} with: at most 1 'A', no 3+ consecutive 'L's.

### State Machine Design

The state must capture all carry-forward constraints:

```
State = (number_of_A_so_far, number_of_trailing_L)
      = (a, l)  where a ∈ {0, 1}, l ∈ {0, 1, 2}
```

So 6 states total. For each state, define how each new character transitions to the next state.

```python
def check_record(n: int) -> int:
    MOD = 10**9 + 7
    
    # dp[a][l] = count of valid strings with:
    # - exactly 'a' A's so far (a in {0,1})
    # - exactly 'l' trailing L's (l in {0,1,2})
    
    # Initial: empty string
    dp = [[0] * 3 for _ in range(2)]
    dp[0][0] = 1  # empty string: 0 A's, 0 trailing L's
    
    for _ in range(n):
        new_dp = [[0] * 3 for _ in range(2)]
        
        for a in range(2):
            for l in range(3):
                if dp[a][l] == 0:
                    continue
                cnt = dp[a][l]
                
                # Append 'P': resets trailing L count
                new_dp[a][0] = (new_dp[a][0] + cnt) % MOD
                
                # Append 'A': only if no A used yet
                if a == 0:
                    new_dp[1][0] = (new_dp[1][0] + cnt) % MOD
                
                # Append 'L': only if trailing L count < 2
                if l < 2:
                    new_dp[a][l + 1] = (new_dp[a][l + 1] + cnt) % MOD
        
        dp = new_dp
    
    return sum(dp[a][l] for a in range(2) for l in range(3)) % MOD

# Test
print(check_record(2))   # 8
print(check_record(1))   # 3
print(check_record(10101))  # large number
```

> **Time:** O(N × 6) = O(N) — 6 states, each processed per step  
> **Space:** O(1) — only 6 states stored

**Matrix exponentiation:** This 6-state machine can be solved in O(log N) using matrix exponentiation, critical for very large N.

```python
import numpy as np

def check_record_fast(n: int) -> int:
    MOD = 10**9 + 7
    # State order: (a=0,l=0), (a=0,l=1), (a=0,l=2), (a=1,l=0), (a=1,l=1), (a=1,l=2)
    # Transition matrix: T[i][j] = does state j transition to state i?
    T = [
        [1, 1, 1, 0, 0, 0],  # state 0 <- P from states 0,1,2 and not from A-states
        [1, 0, 0, 0, 0, 0],  # state 1 <- L from state 0
        [0, 1, 0, 0, 0, 0],  # state 2 <- L from state 1
        [1, 1, 1, 1, 1, 1],  # state 3 <- A from all 0-A states and P from 3,4,5
        [0, 0, 0, 1, 0, 0],  # state 4 <- L from state 3
        [0, 0, 0, 0, 1, 0],  # state 5 <- L from state 4
    ]
    # (Simplified — actual implementation needs careful matrix setup)
    # This shows the O(log N) approach direction
    pass
```

---

## 9. Minimum Swaps to Make Sequences Increasing

**Problem:** [LC 801] Two sequences A and B of same length. Swap A[i] and B[i] if you want. Make both sequences strictly increasing with minimum swaps.

### State Definition (2-State DP)

```
keep[i] = min swaps to make A[0..i] and B[0..i] valid, where A[i], B[i] are NOT swapped at i
swap[i] = min swaps to make A[0..i] and B[0..i] valid, where A[i], B[i] ARE swapped at i
```

### Transition Analysis

At position `i`, we check two conditions:
1. **Natural (no swap at i) is valid with previous choice**: `A[i] > A[i-1]` and `B[i] > B[i-1]`
2. **Cross (swap at i is compatible with no swap at i-1)**: `A[i] > B[i-1]` and `B[i] > A[i-1]`

Note: Since the input guarantees a solution exists, **exactly one or both** of these conditions hold at each step.

```python
def min_swap(A: list[int], B: list[int]) -> int:
    n = len(A)
    INF = float('inf')
    
    keep = 0          # no swap at position 0
    swap = 1          # swap at position 0
    
    for i in range(1, n):
        new_keep = new_swap = INF
        
        natural = A[i] > A[i-1] and B[i] > B[i-1]
        cross   = A[i] > B[i-1] and B[i] > A[i-1]
        
        if natural:
            new_keep = min(new_keep, keep)      # keep both
            new_swap = min(new_swap, swap + 1)  # swap at i, was swapped at i-1
        
        if cross:
            new_keep = min(new_keep, swap)      # keep at i, was swapped at i-1
            new_swap = min(new_swap, keep + 1)  # swap at i, was not swapped at i-1
        
        keep, swap = new_keep, new_swap
    
    return min(keep, swap)

# Test
print(min_swap([1,3,5,4], [1,2,3,7]))  # 1
print(min_swap([0,3,5,8,9], [2,1,4,6,9]))  # 1
```

> **Time:** O(N) | **Space:** O(1)

---

## 10. Fibonacci and Climbing Stairs

**Problem:** [LC 70] Count ways to climb `n` stairs taking 1 or 2 steps at a time.

This is Fibonacci with different initial conditions. Key insight: `ways(n) = ways(n-1) + ways(n-2)`.

```python
def climb_stairs(n: int) -> int:
    if n <= 2:
        return n
    a, b = 1, 2
    for _ in range(3, n + 1):
        a, b = b, a + b
    return b
```

> **Time:** O(N) | **Space:** O(1)

### Generalized K-Step Climbing

```python
def climb_stairs_k_steps(n: int, k: int) -> int:
    """Can take 1 to k steps at a time."""
    dp = [0] * (n + 1)
    dp[0] = 1
    
    # Use sliding window sum for O(N) instead of O(NK)
    window_sum = 1  # dp[0]
    
    for i in range(1, n + 1):
        dp[i] = window_sum
        window_sum += dp[i]
        if i >= k:
            window_sum -= dp[i - k]
    
    return dp[n]
```

> **Time:** O(N) with sliding window | **Space:** O(N)

---

## 11. Minimum Cost Climbing Stairs

**Problem:** [LC 746] `cost[i]` = cost to step from stair `i`. You can step 1 or 2 stairs. Find minimum cost to reach the top (one step beyond last stair).

### State Definition

```
dp[i] = minimum cost to reach stair i
```

Note: you start from either stair 0 or stair 1 (no cost to step onto them).

```python
def min_cost_climbing(cost: list[int]) -> int:
    n = len(cost)
    # dp[i] = min cost to step FROM stair i (i.e., pay cost[i] and move)
    # To reach "top" (index n), come from stair n-1 or n-2
    
    if n == 0: return 0
    if n == 1: return cost[0]
    
    a = cost[0]   # dp[0]
    b = cost[1]   # dp[1]
    
    for i in range(2, n):
        # cost[i] = pay to leave stair i; arrive from i-1 or i-2
        curr = cost[i] + min(a, b)
        a, b = b, curr
    
    # Top is reached from n-1 or n-2 (paying those costs to leave)
    return min(a, b)

# Test
print(min_cost_climbing([10, 15, 20]))       # 15
print(min_cost_climbing([1, 100, 1, 1, 1, 100, 1, 1, 100, 1]))  # 6
```

> **Time:** O(N) | **Space:** O(1)

---

## Summary: 1D DP Pattern Cards

| Problem | State | Transition | Complexity |
|---|---|---|---|
| House Robber I | `dp[i]` = max from 0..i | `max(nums[i]+dp[i-2], dp[i-1])` | O(N), O(1) |
| House Robber II | two linear passes | exclude first / exclude last | O(N), O(1) |
| Jump Game II | BFS levels | greedy farthest | O(N), O(1) |
| Decode Ways | `dp[i]` = ways for s[:i] | 1-digit + 2-digit choices | O(N), O(1) |
| Max Product Subarray | track max AND min | swap on negative multiply | O(N), O(1) |
| Paint Fence | same / diff | same=prev_diff, diff=(sum)*(k-1) | O(N), O(1) |
| Attendance Record II | (A_count, L_streak) | 6-state machine | O(N), O(1) |
| Min Swaps Increasing | keep / swap | natural + cross conditions | O(N), O(1) |
| Climb Stairs | Fibonacci | dp[i]=dp[i-1]+dp[i-2] | O(N), O(1) |
| Min Cost Stairs | `dp[i]` = cost from i | `cost[i]+min(dp[i-1],dp[i-2])` | O(N), O(1) |

---

## Advanced Note: When 1D DP Becomes State Machine DP

Whenever the valid next state depends on **which state you're in**, not just the index, you need multiple state variables. The general pattern:

```python
# k states at each position
states = [initial_state_0, initial_state_1, ..., initial_state_k]

for i in range(1, n):
    new_states = [INF] * k
    for curr_state in range(k):
        for next_state in range(k):
            if valid_transition(curr_state, next_state, i):
                new_states[next_state] = min(
                    new_states[next_state],
                    states[curr_state] + transition_cost(curr_state, next_state, i)
                )
    states = new_states

answer = min(states)
```

This pattern handles: Stock trading problems (buy/sell/hold/cooldown), attendance records, paint problems with constraints, and many more.

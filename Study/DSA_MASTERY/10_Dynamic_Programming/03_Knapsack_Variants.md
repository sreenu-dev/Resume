# Knapsack Variants — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Theme:** Every knapsack variant is a restriction/modification on "which items can be chosen and how many times." Master the 0/1 case thoroughly — every other variant flows from it.

---

## Table of Contents
1. [0/1 Knapsack — Full Derivation and Space Optimization](#1-01-knapsack)
2. [Unbounded Knapsack — Coin Change & Rod Cutting](#2-unbounded-knapsack)
3. [Bounded Knapsack — Binary Grouping Trick](#3-bounded-knapsack)
4. [Partition Equal Subset Sum — Subset Sum as 0/1 Knapsack](#4-partition-equal-subset-sum)
5. [Last Stone Weight II — Minimize Difference](#5-last-stone-weight-ii)
6. [Target Sum — Count Ways with Signs](#6-target-sum)
7. [Ones and Zeroes — 2D Knapsack](#7-ones-and-zeroes)
8. [Profitable Schemes — 3D Knapsack](#8-profitable-schemes)
9. [Combination Sum IV — Order Matters](#9-combination-sum-iv)
10. [Count of Subsets with Given Difference](#10-count-of-subsets-with-given-difference)
11. [Fractional Knapsack — Why DP Doesn't Apply](#11-fractional-knapsack)

---

## 1. 0/1 Knapsack — Full Derivation

**Problem:** Given `n` items with weights `w[i]` and values `v[i]`, and a knapsack of capacity `W`. Each item can be taken at most once. Maximize total value.

### State Definition

```
dp[i][w] = maximum value achievable using items 0..i-1 with weight capacity w
```

### Recurrence

For each item `i`, two exclusive choices:
```
Don't take item i: dp[i][w] = dp[i-1][w]
Take item i:       dp[i][w] = dp[i-1][w - w[i]] + v[i]  (if w >= w[i])

dp[i][w] = max(dp[i-1][w], dp[i-1][w - w[i]] + v[i])
```

**Why this has optimal substructure:**  
After deciding on item `i`, the remaining capacity must be filled optimally from the remaining items — this is exactly `dp[i-1][w - w[i]]`.

### Full Implementation with Space Optimization

```python
def knapsack_01(weights: list[int], values: list[int], W: int) -> int:
    n = len(weights)
    
    # 2D solution for clarity
    # dp = [[0]*(W+1) for _ in range(n+1)]
    # for i in range(1, n+1):
    #     for w in range(W+1):
    #         dp[i][w] = dp[i-1][w]
    #         if weights[i-1] <= w:
    #             dp[i][w] = max(dp[i][w], dp[i-1][w-weights[i-1]] + values[i-1])
    # return dp[n][W]
    
    # 1D space-optimized (REVERSE iteration is critical!)
    dp = [0] * (W + 1)
    
    for i in range(n):
        # Iterate W down to weights[i] — CRITICAL: prevents using item i twice
        # If we iterated forward, dp[w-weights[i]] would already reflect item i
        for w in range(W, weights[i] - 1, -1):
            dp[w] = max(dp[w], dp[w - weights[i]] + values[i])
    
    return dp[W]

# Test
weights = [1, 3, 4, 5]
values  = [1, 4, 5, 7]
W = 7
print(knapsack_01(weights, values, W))  # 9 (items 1 and 2: weight 3+4=7, value 4+5=9)
```

> **Time:** O(N × W) | **Space:** O(W) — rolling array

### Why Reverse Iteration Works (Proof)

In the 2D version, `dp[i][w]` reads from `dp[i-1][...]` — a completely separate row. When we collapse to 1D:
- **Forward iteration:** `dp[w - weights[i]]` was ALREADY updated for item `i` → item `i` can be picked multiple times (becomes unbounded knapsack).
- **Reverse iteration:** `dp[w - weights[i]]` is still from item `i-1` iteration (not yet updated) → item `i` picked at most once.

### Backtracking to Find Selected Items

```python
def knapsack_with_backtrack(weights, values, W):
    n = len(weights)
    dp = [[0]*(W+1) for _ in range(n+1)]
    
    for i in range(1, n+1):
        for w in range(W+1):
            dp[i][w] = dp[i-1][w]
            if weights[i-1] <= w:
                dp[i][w] = max(dp[i][w], dp[i-1][w-weights[i-1]] + values[i-1])
    
    # Backtrack
    selected = []
    w = W
    for i in range(n, 0, -1):
        if dp[i][w] != dp[i-1][w]:
            selected.append(i-1)
            w -= weights[i-1]
    
    return dp[n][W], selected[::-1]
```

---

## 2. Unbounded Knapsack

**Problem:** Same as 0/1, but each item can be used **any number of times**.

### Key Difference: Forward Iteration

```python
def unbounded_knapsack(weights: list[int], values: list[int], W: int) -> int:
    dp = [0] * (W + 1)
    
    for w in range(1, W + 1):
        for i in range(len(weights)):
            if weights[i] <= w:
                # FORWARD: dp[w - weights[i]] already accounts for item i → reuse allowed
                dp[w] = max(dp[w], dp[w - weights[i]] + values[i])
    
    return dp[W]
```

> **Time:** O(W × N) | **Space:** O(W)

### Coin Change — Minimum Coins (Unbounded)

**Problem:** [LC 322] Fewest coins to make amount, each coin usable unlimited times.

```python
def coin_change(coins: list[int], amount: int) -> int:
    INF = float('inf')
    dp = [INF] * (amount + 1)
    dp[0] = 0  # 0 coins to make amount 0
    
    for amt in range(1, amount + 1):
        for coin in coins:
            if coin <= amt and dp[amt - coin] != INF:
                dp[amt] = min(dp[amt], dp[amt - coin] + 1)
    
    return dp[amount] if dp[amount] != INF else -1

print(coin_change([1,2,5], 11))  # 3 (5+5+1)
print(coin_change([2], 3))       # -1
```

> **Time:** O(amount × N_coins) | **Space:** O(amount)

### Coin Change II — Count Ways (Unbounded Counting)

**Problem:** [LC 518] Count combinations (order doesn't matter) to make amount.

**Critical distinction:** Iterate coins in outer loop, amounts in inner loop — ensures each combination counted once, not permutations.

```python
def change(amount: int, coins: list[int]) -> int:
    dp = [0] * (amount + 1)
    dp[0] = 1  # one way to make 0
    
    # Outer loop: coins (ensures combinations, not permutations)
    for coin in coins:
        for amt in range(coin, amount + 1):
            dp[amt] += dp[amt - coin]
    
    return dp[amount]

print(change(5, [1,2,5]))   # 4 (5, 2+2+1, 2+1+1+1, 1+1+1+1+1)
print(change(3, [2]))       # 0
```

> **Time:** O(amount × N_coins) | **Space:** O(amount)

### Rod Cutting

**Problem:** Rod of length N. Prices for each length. Maximize revenue by cutting.

```python
def rod_cutting(prices: list[int], n: int) -> int:
    # prices[i] = price for rod of length i+1
    dp = [0] * (n + 1)
    
    for length in range(1, n + 1):
        for cut in range(1, length + 1):
            if cut <= len(prices):
                dp[length] = max(dp[length], prices[cut-1] + dp[length - cut])
    
    return dp[n]

print(rod_cutting([1,5,8,9,10,17,17,20], 8))  # 22 (cut into 2+6 or 6+2)
```

> **Time:** O(N²) | **Space:** O(N)

---

## 3. Bounded Knapsack — Binary Grouping Trick

**Problem:** Item `i` can be used at most `count[i]` times. Naively O(N × W × max_count). Optimize using binary grouping to O(N × W × log(max_count)).

### Binary Grouping (Binary Representation Trick)

Split each item with count `c` into groups of sizes `1, 2, 4, ..., 2^k, remainder`. This allows representing any count from 1 to c using at most `log(c)` groups, each group treated as a single 0/1 item.

```python
def bounded_knapsack(weights: list[int], values: list[int], 
                     counts: list[int], W: int) -> int:
    # Decompose bounded items into 0/1 items via binary grouping
    new_weights, new_values = [], []
    
    for i in range(len(weights)):
        c = counts[i]
        k = 1
        while c > 0:
            take = min(k, c)
            new_weights.append(weights[i] * take)
            new_values.append(values[i] * take)
            c -= take
            k *= 2
    
    # Now solve 0/1 knapsack on expanded items
    dp = [0] * (W + 1)
    for wi, vi in zip(new_weights, new_values):
        for w in range(W, wi - 1, -1):
            dp[w] = max(dp[w], dp[w - wi] + vi)
    
    return dp[W]

# Test: 3 items, each usable at most count[i] times
weights = [1, 2, 3]
values  = [2, 3, 4]
counts  = [3, 2, 5]
print(bounded_knapsack(weights, values, counts, 10))  # 15
```

> **Time:** O(N × log(max_count) × W) | **Space:** O(N × log(max_count) + W)

**Why binary grouping works:**  
Any count from 1 to c can be expressed as a sum of binary powers. By creating "meta-items" of size 1, 2, 4, 8, ... (and remainder), we can pick any subset of meta-items to achieve any count from 0 to c using only 0/1 choices.

---

## 4. Partition Equal Subset Sum

**Problem:** [LC 416] Can the array be partitioned into two subsets with equal sum?

### Reduction to Subset Sum (0/1 Knapsack Decision)

If total sum is odd → impossible. Otherwise, find a subset with sum = total/2.

```
dp[w] = True if subset summing to exactly w exists
```

This is 0/1 knapsack where value = weight = nums[i], and we want dp[target] = True.

```python
def can_partition(nums: list[int]) -> bool:
    total = sum(nums)
    if total % 2 != 0:
        return False
    
    target = total // 2
    dp = [False] * (target + 1)
    dp[0] = True  # empty subset sums to 0
    
    for num in nums:
        # Reverse iteration: 0/1 knapsack (each item used at most once)
        for w in range(target, num - 1, -1):
            dp[w] = dp[w] or dp[w - num]
    
    return dp[target]

print(can_partition([1,5,11,5]))  # True (1+5+5=11)
print(can_partition([1,2,3,5]))   # False
```

> **Time:** O(N × sum/2) | **Space:** O(sum/2)

---

## 5. Last Stone Weight II

**Problem:** [LC 1049] Stones with weights. Any two stones collide: result is |w1 - w2|. Minimize the last stone's weight.

### Insight: Minimize |S1 - S2| = Partition into Two Groups

Assign `+` or `-` to each stone. Goal: minimize |sum(+) - sum(-)|. This is equivalent to partitioning into two groups minimizing their difference. Equivalent to finding subset with sum as close to total/2 as possible.

```python
def last_stone_weight_ii(stones: list[int]) -> int:
    total = sum(stones)
    target = total // 2
    
    # dp[w] = True if achievable subset sum
    dp = [False] * (target + 1)
    dp[0] = True
    
    for stone in stones:
        for w in range(target, stone - 1, -1):
            dp[w] = dp[w] or dp[w - stone]
    
    # Find the largest achievable sum <= total/2
    for s in range(target, -1, -1):
        if dp[s]:
            return total - 2 * s
    
    return total  # shouldn't reach here

print(last_stone_weight_ii([2,7,4,1,8,1]))  # 1
print(last_stone_weight_ii([31,26,33,21,40]))  # 5
```

> **Time:** O(N × sum/2) | **Space:** O(sum/2)

---

## 6. Target Sum — Count Ways

**Problem:** [LC 494] Assign `+` or `-` to each number. Count assignments that sum to target T.

### Mathematical Reduction

Let P = subset assigned `+`, N = subset assigned `-`.
```
P - N = T
P + N = total
=> P = (total + T) / 2
```

Count subsets summing to `(total + T) / 2`. If `(total + T)` is odd or negative → 0 ways.

```python
def find_target_sum_ways(nums: list[int], target: int) -> int:
    total = sum(nums)
    
    # P = (total + target) / 2, must be non-negative integer
    if (total + target) % 2 != 0 or abs(target) > total:
        return 0
    
    subset_sum = (total + target) // 2
    
    # Count ways to achieve subset_sum (0/1 knapsack — counting version)
    dp = [0] * (subset_sum + 1)
    dp[0] = 1  # one way to achieve sum 0: empty subset
    
    for num in nums:
        for w in range(subset_sum, num - 1, -1):
            dp[w] += dp[w - num]
    
    return dp[subset_sum]

print(find_target_sum_ways([1,1,1,1,1], 3))  # 5
print(find_target_sum_ways([1], 1))           # 1
```

> **Time:** O(N × subset_sum) | **Space:** O(subset_sum)

**Note:** Zeros in the array double the count (a zero can be `+0` or `-0`). The reduction handles this correctly since `dp[w] += dp[w - 0] = dp[w]` doubles all existing counts for zero elements.

---

## 7. Ones and Zeroes — 2D Knapsack

**Problem:** [LC 474] Given strings of '0's and '1's. Find the largest subset such that the total '0's ≤ m and total '1's ≤ n.

### State Definition (2D Capacity)

```
dp[i][j] = maximum number of strings in subset with at most i zeros and j ones
```

Two capacity dimensions: zeros and ones. This is a 0/1 knapsack with 2D weight.

```python
def find_max_form(strs: list[str], m: int, n: int) -> int:
    # dp[i][j] = max strings with i zeros budget and j ones budget
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    for s in strs:
        zeros = s.count('0')
        ones = s.count('1')
        
        # 2D reverse iteration (both dimensions!)
        for i in range(m, zeros - 1, -1):
            for j in range(n, ones - 1, -1):
                dp[i][j] = max(dp[i][j], dp[i - zeros][j - ones] + 1)
    
    return dp[m][n]

print(find_max_form(["10","0001","111001","1","0"], m=5, n=3))  # 4
print(find_max_form(["10","0","1"], m=1, n=1))  # 2 ("10" and "0" or "10" and "1")
```

> **Time:** O(|strs| × M × N) | **Space:** O(M × N)

**Generalization:** A K-dimensional knapsack has a K-dimensional dp array and requires K nested reverse loops per item.

---

## 8. Profitable Schemes — 3D Knapsack

**Problem:** [LC 879] G gang members, P minimum profit. Each crime needs `group[i]` members and gives `profit[i]`. Count schemes using ≤ G members yielding ≥ P profit. Mod 10^9+7.

### State Definition (3D)

```
dp[k][g][p] = number of schemes using first k crimes, 
              with g members used, generating p profit
```

Space optimized to 2D since we iterate crimes outer:

```
dp[g][p] = number of schemes with g members and p profit (from crimes processed so far)
```

```python
def profitable_schemes(n: int, min_profit: int, group: list[int], profit: list[int]) -> int:
    MOD = 10**9 + 7
    K = len(group)
    
    # dp[g][p] = ways to use g members and achieve exactly p profit
    # But we want at-least min_profit, so cap p at min_profit
    dp = [[0] * (min_profit + 1) for _ in range(n + 1)]
    dp[0][0] = 1  # 0 members, 0 profit: 1 way (do nothing)
    
    for k in range(K):
        g_k, p_k = group[k], profit[k]
        
        # Reverse both dimensions (0/1 knapsack on both constraints)
        for g in range(n, g_k - 1, -1):
            for p in range(min_profit, -1, -1):
                # New profit: min(p + p_k, min_profit) to cap at boundary
                new_p = min(p + p_k, min_profit)
                dp[g][new_p] = (dp[g][new_p] + dp[g - g_k][p]) % MOD
    
    # Sum all dp[g][min_profit] for g in 0..n
    return sum(dp[g][min_profit] for g in range(n + 1)) % MOD

print(profitable_schemes(5, 3, [2,2], [2,3]))  # 2
print(profitable_schemes(10, 5, [2,3,5], [6,7,8]))  # 7
```

> **Time:** O(K × N × P) | **Space:** O(N × P)

**Key trick:** Capping profit at `min_profit` collapses the "at least P" condition into a single boundary state. All profits ≥ min_profit accumulate in `dp[...][min_profit]`.

---

## 9. Combination Sum IV — Order Matters (Unbounded + Permutations)

**Problem:** [LC 377] Count sequences (order matters) of numbers from nums that sum to target. Each number can be used any number of times.

### Key Insight: Permutations = Outer loop is TARGET, inner is COINS

Compare:
- **Combinations** (Coin Change II): outer loop = coins → each coin added once per "era"
- **Permutations** (Combination Sum IV): outer loop = target → count sequences

```python
def combination_sum_iv(nums: list[int], target: int) -> int:
    dp = [0] * (target + 1)
    dp[0] = 1  # one way to form sum 0: empty sequence
    
    # Outer loop: TARGET (not nums) — this counts permutations
    for t in range(1, target + 1):
        for num in nums:
            if num <= t:
                dp[t] += dp[t - num]
    
    return dp[target]

print(combination_sum_iv([1,2,3], 4))  # 7
# (1+1+1+1, 1+1+2, 1+2+1, 2+1+1, 2+2, 1+3, 3+1)
```

> **Time:** O(target × N) | **Space:** O(target)

**Why outer=target gives permutations:**  
For `dp[4]`, we try adding each num to reach 4. Adding 1 to dp[3], 2 to dp[2], 3 to dp[1]. Each path to dp[t] represents a distinct ordered sequence ending in a specific num.

---

## 10. Count of Subsets with Given Difference

**Problem:** Count subsets S1 and S2 partitioning array where S1 - S2 = diff.

### Mathematical Reduction

```
S1 + S2 = total
S1 - S2 = diff
=> S1 = (total + diff) / 2
```

Count subsets summing to `(total + diff) / 2`:

```python
def count_subsets_diff(nums: list[int], diff: int) -> int:
    total = sum(nums)
    if (total + diff) % 2 != 0:
        return 0
    target = (total + diff) // 2
    if target < 0:
        return 0
    
    # Count subsets (handle zeros carefully!)
    # Zeros create extra subsets: each zero can be in S1 or S2
    zero_count = nums.count(0)
    non_zero = [x for x in nums if x > 0]
    
    dp = [0] * (target + 1)
    dp[0] = 1
    for num in non_zero:
        for w in range(target, num - 1, -1):
            dp[w] += dp[w - num]
    
    # Each zero doubles the count (can go to either subset)
    return dp[target] * (2 ** zero_count)

print(count_subsets_diff([1,1,2,3], 1))  # 3
```

> **Time:** O(N × target) | **Space:** O(target)

---

## 11. Fractional Knapsack — Why DP Doesn't Apply

**Why greedy works (exchange argument proof):**

Sort items by value/weight ratio, take greedily. Suppose OPT doesn't take item `i` at full capacity but takes item `j` with lower ratio. Swap `ε` of `j` with `ε` of `i` → total value increases. Contradiction. So greedy is optimal.

**Why DP doesn't apply:**  
Fractional knapsack has no "discrete" decisions — we can take any fraction. There are infinitely many possible states (any real-valued weight taken). DP requires a finite, discrete state space. Greedy gives O(N log N).

```python
def fractional_knapsack(weights: list[float], values: list[float], W: float) -> float:
    # Sort by value/weight ratio descending
    items = sorted(zip(weights, values), key=lambda x: x[1]/x[0], reverse=True)
    
    total_value = 0.0
    remaining = W
    
    for w, v in items:
        if remaining <= 0:
            break
        take = min(w, remaining)
        total_value += take * (v / w)
        remaining -= take
    
    return total_value

print(fractional_knapsack([10,20,30], [60,100,120], 50))  # 240.0
```

> **Time:** O(N log N) — dominated by sort | **Space:** O(1)

---

## Summary: Knapsack Variant Recognition Guide

| Variant | Loop Order | Key Constraint | LC Example |
|---|---|---|---|
| 0/1 Knapsack | Items outer, W inner REVERSE | Each item ≤ 1 time | 0/1 base |
| Unbounded | W outer, items inner FORWARD | Each item unlimited | Coin Change |
| Bounded | Binary split → 0/1 | Each item ≤ count[i] | Bounded |
| Partition/Subset Sum | Items outer, W inner REVERSE | Exact sum target | LC 416 |
| Count ways (combos) | Items outer, W inner FORWARD | Count, not order | LC 518 |
| Count ways (perms) | W outer, items inner | Order matters | LC 377 |
| 2D Knapsack | Items outer, 2D REVERSE | Two capacity dims | LC 474 |
| 3D Knapsack | Items outer, 3D REVERSE | Three capacity dims | LC 879 |

### The Golden Rule for Knapsack Loop Order

```
COMBINATIONS (order doesn't matter):
    for item in items:          ← outer loop = items
        for w in range(W, item-1, -1):  ← 0/1: REVERSE
            dp[w] = max/+= dp[w-item]

PERMUTATIONS (order matters):
    for w in range(1, W+1):     ← outer loop = target
        for item in items:
            dp[w] = max/+= dp[w-item]

UNBOUNDED (unlimited use):
    for item in items:          ← outer loop = items
        for w in range(item, W+1):  ← FORWARD (allows reuse)
            dp[w] = max/+= dp[w-item]
```

### When to Recognize Knapsack in Disguise

1. **"Partition array into two groups"** → Subset sum = 0/1 knapsack
2. **"Assign + or - to numbers, reach target"** → Transform to subset sum
3. **"Pick items with multiple constraints"** → Multi-dimensional knapsack
4. **"Count/find subsets with sum X"** → 0/1 or unbounded knapsack
5. **"Minimum/maximum value using exactly K items"** → Add K as a dimension

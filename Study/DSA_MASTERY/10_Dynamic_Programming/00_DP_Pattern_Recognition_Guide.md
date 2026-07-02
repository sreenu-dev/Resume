# Dynamic Programming: Pattern Recognition & Meta-Guide
## Advanced Mastery for FAANG Interviews

> **Audience:** Engineers who know basic DP (fibonacci, coin change). This file teaches you **how to think** about DP — the meta-skill that makes every subsequent problem easier.

---

## Table of Contents
1. [The 5-Step DP Design Methodology](#1-the-5-step-dp-design-methodology)
2. [DP vs Greedy — The Exchange Argument](#2-dp-vs-greedy--the-exchange-argument)
3. [How to Recognize DP: Decision Flowchart](#3-how-to-recognize-dp-decision-flowchart)
4. [The 10 Core DP Patterns with Recognition Signatures](#4-the-10-core-dp-patterns-with-recognition-signatures)
5. [State Compression Techniques](#5-state-compression-techniques)
6. [Pull vs Push DP](#6-pull-vs-push-dp)
7. [Forward vs Backward Recurrence](#7-forward-vs-backward-recurrence)
8. [Rolling Array Space Optimization](#8-rolling-array-space-optimization)
9. [Common DP State Design Mistakes](#9-common-dp-state-design-mistakes)
10. [DP on Intervals, Strings, Trees, Graphs, Digits, Subsets](#10-dp-domains-at-a-glance)
11. [Full Worked Example: Applying the 5-Step Methodology](#11-full-worked-example)

---

## 1. The 5-Step DP Design Methodology

Every DP solution, no matter how complex, follows the same five steps. Internalizing this process turns DP from black magic into engineering.

### Step 1 — Identify Optimal Substructure

**Question to ask:** "Can the optimal solution to the whole problem be constructed from optimal solutions to subproblems?"

Optimal substructure means: if you fix one decision at the top level, the remaining subproblem must be solved optimally too. If this is violated (e.g., optimal path in a graph with cycles), DP won't work directly.

**Formal test:** Suppose OPT(problem) makes choice `c`. Then OPT(subproblem after choice `c`) must also be optimal for the subproblem. If you can find a counter-example — a case where making a locally sub-optimal choice in the sub-problem yields a globally better answer — optimal substructure fails.

**Examples of optimal substructure:**
- Shortest path in DAG: removing the last edge leaves an optimal sub-path ✓
- Longest path in general graph: removing the last node does NOT yield longest sub-path ✗ (NP-hard)
- Knapsack: taking item `i`, the remaining capacity must be filled optimally ✓

### Step 2 — Define State Completely and Minimally

The state is the information you need to **fully determine** the answer without looking at how you got there (Markov property).

**Completely:** Every piece of information needed to solve the subproblem must be encoded.  
**Minimally:** No redundant information. Extra state dimensions multiply your complexity.

**State design checklist:**
```
□ What position/index am I at?
□ What constraints carry forward? (remaining capacity, budget, count)
□ What choices have I made that affect future decisions?
□ Can I derive any state variable from others? (remove it if so)
□ Is there a "phase" or "mode" I'm in? (state machine)
```

**Anti-pattern:** Including the full path taken so far in the state → exponential states.  
**Fix:** Only include what *matters* for future decisions (e.g., last element chosen, not all elements).

### Step 3 — Write the Recurrence with Base Cases

The recurrence expresses the answer to a state in terms of smaller states. Always write it mathematically first, then code it.

```
dp[state] = optimize over all valid transitions from smaller states
```

**Base cases:** The smallest states with trivially known answers (empty string, index 0, no items).

**Common mistakes:**
- Off-by-one in base cases (dp[0] vs dp[1])
- Forgetting to handle the "do nothing" option
- Transition direction wrong (going from future to past instead of past to future)

### Step 4 — Determine Bottom-Up Computation Order

For bottom-up DP, you must compute smaller subproblems before larger ones. The dependency graph of states must be a DAG.

**Technique:** Look at your recurrence. `dp[i][j]` depends on what? If it depends on `dp[i-1][...]` and `dp[i][j-1]`, iterate `i` outer, `j` inner in increasing order.

**Topological order** is needed when states don't have a natural linear order (e.g., tree DP, DAG DP).

### Step 5 — Optimize Space

Once the recurrence is correct, look at the dependency pattern:
- **Only previous row needed?** → Rolling array (2 rows)
- **Only dp[i-1] needed?** → Single variable
- **Diagonal dependencies?** → Special rolling

```python
# Example: Before space optimization (2D)
dp = [[0] * (W+1) for _ in range(n+1)]

# After: rolling array (only need previous row)
prev = [0] * (W+1)
curr = [0] * (W+1)
# OR: in-place with careful iteration direction
```

---

## 2. DP vs Greedy — The Exchange Argument

Greedy is faster (usually O(N log N) vs O(N²) for DP) but only works when a **greedy choice** is globally safe.

### The Exchange Argument

To **prove** greedy works:
1. Assume an optimal solution OPT doesn't make the greedy choice.
2. Show you can "exchange" the non-greedy choice for the greedy choice without making the solution worse.
3. Conclude greedy reaches an optimal solution.

### Classic Examples

**Interval Scheduling (Greedy — sort by end time):**  
Proof: If OPT picks interval A that ends later than greedy's choice B, swap A→B. B ends earlier, so it conflicts with at most as many future intervals as A. Solution stays valid and no worse.

**Coin Change (DP — greedy fails):**  
Coins = {1, 3, 4}, target = 6. Greedy picks 4+1+1=3 coins. DP finds 3+3=2 coins. Exchange argument fails because taking a "large" coin can block better combinations.

**The key test:** "Does taking the locally best option now ever prevent a globally better solution later?"
- If NO (provable) → Greedy
- If YES (find a counter-example) → DP

---

## 3. How to Recognize DP: Decision Flowchart

```
START: Is the problem asking for optimization (min/max) or counting?
           │
     YES ──┤
           │
           ▼
     Does the problem have overlapping subproblems?
     (Would a naive recursive solution recompute the same state?)
           │
     YES ──┤
           │
           ▼
     Does optimal substructure hold?
     (Can we build optimal solution from optimal sub-solutions?)
           │
     YES ──┤
           │
           ▼
     → USE DYNAMIC PROGRAMMING

     NO at any step → Consider: Greedy, Divide & Conquer, or reformulate
```

### Linguistic Signatures of DP Problems

| Phrasing | Likely DP Type |
|---|---|
| "Maximum/minimum ... subsequence" | 1D DP / LIS / LCS |
| "Number of ways to ..." | Counting DP |
| "Can you reach / is it possible" | Boolean DP |
| "Optimal strategy for two players" | Game theory DP |
| "Partition into K groups" | Subset/partition DP |
| "Over all substrings/intervals" | Interval DP |
| "On a tree, find optimal ..." | Tree DP |
| "Numbers from 0 to N with property" | Digit DP |
| "Subset of items with constraints" | Knapsack DP |
| "Two sequences, find common ..." | LCS/Edit distance |

---

## 4. The 10 Core DP Patterns with Recognition Signatures

### Pattern 1: Linear DP (1D State)
**Signature:** Decision at index `i` depends only on previous indices.  
**State:** `dp[i]` = answer for prefix/suffix of length `i`.  
**Examples:** House Robber, Climb Stairs, Max Subarray.

### Pattern 2: Grid/2D DP
**Signature:** Movement on a 2D grid, usually top-left to bottom-right.  
**State:** `dp[i][j]` = answer at cell (i,j).  
**Examples:** Unique Paths, Min Path Sum, Maximal Square.

### Pattern 3: Knapsack DP
**Signature:** Choose items with weights/costs to maximize value under a budget.  
**State:** `dp[i][w]` = best value using first `i` items with weight capacity `w`.  
**Examples:** 0/1 Knapsack, Subset Sum, Partition Equal Subset.

### Pattern 4: LCS/Edit Distance (Two-Sequence DP)
**Signature:** Two strings/arrays; operations on one or both.  
**State:** `dp[i][j]` = answer considering first `i` chars of s1 and first `j` chars of s2.  
**Examples:** LCS, Edit Distance, Distinct Subsequences.

### Pattern 5: Interval DP
**Signature:** "Optimal way to handle a contiguous subarray [i..j]."  
**State:** `dp[i][j]` = answer for subarray from i to j.  
**Fill order:** Increasing interval length.  
**Examples:** Matrix Chain Multiplication, Burst Balloons, Palindrome Partitioning.

### Pattern 6: Tree DP
**Signature:** Optimal answer on a subtree; children results combined.  
**State:** `dp[node][...]` defined on subtree rooted at node.  
**Examples:** House Robber III, Max Path Sum, Diameter.

### Pattern 7: Bitmask DP
**Signature:** N ≤ 20 items, need to track which subset has been used.  
**State:** `dp[mask]` or `dp[mask][i]`.  
**Examples:** TSP, Worker Assignment, Partition into K Subsets.

### Pattern 8: Digit DP
**Signature:** Count integers in [0, N] satisfying a digit-level property.  
**State:** `dp[pos][tight][...state]`.  
**Examples:** Count numbers with digit sum ≡ 0 (mod K), no consecutive 1s.

### Pattern 9: State Machine DP
**Signature:** System has distinct modes/states that transition on each step.  
**State:** `dp[i][state]` where state ∈ {finite set of modes}.  
**Examples:** Stock buy/sell with cooldown, attendance record.

### Pattern 10: Subset DP / SOS (Sum over Subsets)
**Signature:** For every subset, compute aggregate over all its subsets.  
**State:** `dp[mask]` iterating over all 2^N subsets.  
**Examples:** Number of subsets with XOR = target, counting subset sums.

---

## 5. State Compression Techniques

### Technique 1: Bitmask for Small Sets
When the "which items are used" matters and N ≤ 20, encode the set as an integer bitmask.

```python
# State: dp[mask] = can we reach this subset configuration?
n = 4
dp = [False] * (1 << n)
dp[0] = True  # empty set is always reachable

for mask in range(1 << n):
    if dp[mask]:
        for i in range(n):
            if not (mask >> i & 1):  # item i not yet used
                dp[mask | (1 << i)] = True
```

**Time:** O(2^N × N) | **Space:** O(2^N)

### Technique 2: Coordinate Compression
When state values are large but sparse, map them to small indices.

```python
values = sorted(set(arr))
compress = {v: i for i, v in enumerate(values)}
# Now use compress[arr[i]] as index instead of arr[i]
```

### Technique 3: Profile DP (Broken Profile)
For grid problems where the state is the "boundary" between processed and unprocessed cells — encode this profile as a bitmask of the current column's state.

### Technique 4: Segment Tree / BIT as DP Structure
When `dp[i] = max(dp[j]) for j in some range`, use a Segment Tree to query max in O(log N) instead of O(N).

```python
# LIS in O(N log N) using Fenwick Tree storing max dp values
# dp[i] = 1 + max(dp[j]) for all j where arr[j] < arr[i]
```

---

## 6. Pull vs Push DP

These are two equivalent ways to write the same recurrence — choose whichever makes the transition logic cleaner.

### Pull DP (Standard)
"To compute `dp[i]`, I look back at all states that could lead to `i` and pull the best value."

```python
# Pull: dp[i] is computed from previous states
for i in range(1, n):
    for j in range(i):
        if valid_transition(j, i):
            dp[i] = max(dp[i], dp[j] + cost(j, i))
```

### Push DP
"When I process state `j`, I push (propagate) its result forward to all states that `j` can reach."

```python
# Push: from state j, update future states
for j in range(n):
    for i in range(j+1, n):
        if valid_transition(j, i):
            dp[i] = max(dp[i], dp[j] + cost(j, i))
```

**When to prefer Push:**
- When the set of states reachable FROM `j` is easier to enumerate than the set of states that lead TO `i`.
- BFS-style DP (shortest path in DAG).

**When to prefer Pull:**
- When computing `dp[i]` in isolation is cleaner.
- Most standard DP problems.

---

## 7. Forward vs Backward Recurrence

### Forward (left-to-right): `dp[i]` = answer for prefix ending at `i`
```python
# Best subsequence ending at index i
for i in range(n):
    dp[i] = 1  # just arr[i] itself
    for j in range(i):
        if arr[j] < arr[i]:
            dp[i] = max(dp[i], dp[j] + 1)
answer = max(dp)
```

### Backward (right-to-left): `dp[i]` = answer for suffix starting at `i`
```python
# Best subsequence starting at index i
for i in range(n-1, -1, -1):
    dp[i] = 1
    for j in range(i+1, n):
        if arr[j] > arr[i]:
            dp[i] = max(dp[i], dp[j] + 1)
answer = max(dp)
```

**Dungeon Game is the canonical example** of why backward DP is necessary: the constraint at each cell depends on future cells, so you must compute right-to-left, bottom-to-top.

---

## 8. Rolling Array Space Optimization

When `dp[i]` depends only on `dp[i-1]` (and possibly `dp[i-2]`), reduce O(N²) space to O(N) or O(1).

### 1D Rolling (Two Rows → One Row)

```python
# 2D knapsack: dp[i][w] depends on dp[i-1][w] and dp[i-1][w-wt[i]]
# Optimize: single 1D array, iterate w in REVERSE to avoid using item twice

dp = [0] * (W + 1)
for item_weight, item_value in items:
    for w in range(W, item_weight - 1, -1):  # CRITICAL: reverse!
        dp[w] = max(dp[w], dp[w - item_weight] + item_value)

# Forward iteration would mean item can be used multiple times (unbounded knapsack)
```

**Time:** O(N × W) | **Space:** O(W) instead of O(N × W)

### Why Reverse for 0/1 Knapsack?

When iterating `w` from high to low, `dp[w - item_weight]` still holds the value from the *previous* item (i-1 row). If we iterate forward, `dp[w - item_weight]` would already have been updated with the current item, effectively allowing the item to be used again.

### 2D Rolling (Matrix → Two Rows)

```python
# LCS: dp[i][j] depends on dp[i-1][j-1], dp[i-1][j], dp[i][j-1]
# Keep only 2 rows: prev and curr

prev = [0] * (m + 1)
for i in range(1, n + 1):
    curr = [0] * (m + 1)
    for j in range(1, m + 1):
        if s1[i-1] == s2[j-1]:
            curr[j] = prev[j-1] + 1
        else:
            curr[j] = max(prev[j], curr[j-1])
    prev = curr
```

**Space:** O(M) instead of O(N × M)

---

## 9. Common DP State Design Mistakes

### Mistake 1: Under-Specified State
```python
# WRONG: dp[i] = max profit by day i (missing: do we hold stock?)
# RIGHT: dp[i][0] = max profit on day i without stock
#        dp[i][1] = max profit on day i holding stock
```

### Mistake 2: Over-Specified State (too many dimensions)
```python
# WRONG: dp[i][j][last_choice][second_last_choice]
# Only the last choice matters for transition → dp[i][j][last_choice]
```

### Mistake 3: Wrong Base Case
```python
# Min path sum: forget to initialize dp[0][j] and dp[i][0] borders
# "Count ways": base case should be 1 (empty string/set), not 0
```

### Mistake 4: Incorrect Iteration Order
```python
# Interval DP: must iterate by INCREASING LENGTH, not by i then j
for length in range(2, n+1):        # correct
    for i in range(n - length + 1):
        j = i + length - 1
```

### Mistake 5: Integer Overflow / Infinity Initialization
```python
INF = float('inf')
dp = [INF] * n       # for minimization
dp = [-INF] * n      # for maximization
dp = [0] * n         # for counting
```

---

## 10. DP Domains at a Glance

| Domain | State Shape | Fill Order | Key Insight |
|---|---|---|---|
| **1D sequences** | `dp[i]` | Left→Right | prefix/suffix |
| **2D grid** | `dp[i][j]` | Row by row | top-left→bottom-right |
| **Interval** | `dp[i][j]` | Increasing length | split point k |
| **Tree** | `dp[node][extra]` | Post-order DFS | children first |
| **Strings (2-seq)** | `dp[i][j]` | Increasing i,j | match/mismatch |
| **Knapsack** | `dp[i][w]` | Items × capacity | include/exclude |
| **Bitmask** | `dp[mask]` | Increasing popcount | enumerate subsets |
| **Digit** | `dp[pos][tight][...]` | Recursive + memo | tight constraint |
| **Graph DAG** | `dp[node]` | Topological order | relax edges |
| **State machine** | `dp[i][state]` | Left→Right | transition matrix |

---

## 11. Full Worked Example: Applying the 5-Step Methodology

**Problem:** [LC 312] Burst Balloons — Given N balloons with values, burst all of them. When you burst balloon `i`, you gain `nums[left] * nums[i] * nums[right]` coins. Maximize total coins.

### Step 1: Identify Optimal Substructure

If we think about the **first** balloon to burst, the problem decomposes poorly — after bursting balloon `i`, left and right neighbors merge and change the subproblems' structure.

**Key insight (reverse thinking):** Think about the **last** balloon to burst in interval `[i, j]`. If balloon `k` is last, it's surrounded by virtual balloons at the boundaries. This gives clean optimal substructure: `dp[i][j]` = best coins from bursting all balloons strictly between the boundary balloons `i` and `j`.

### Step 2: Define State

```
dp[i][j] = maximum coins from bursting all balloons in the OPEN interval (i, j)
            (balloons i and j are NOT burst — they are boundary sentinels)
```

Add virtual `1` balloons at both ends: `nums = [1] + nums + [1]`

### Step 3: Write Recurrence

For each possible "last balloon to burst" `k` in `(i, j)`:
```
dp[i][j] = max over k in (i+1, j-1) of:
            dp[i][k] + nums[i]*nums[k]*nums[j] + dp[k][j]
```
Base case: `dp[i][j] = 0` when `j - i < 2` (no balloons between i and j).

### Step 4: Computation Order

`dp[i][j]` depends on `dp[i][k]` and `dp[k][j]` where `i < k < j`. Both sub-intervals are shorter. So fill by **increasing interval length**.

### Step 5: Space Optimization

No obvious reduction below O(N²) since all `dp[i][j]` may be needed. Accept O(N²).

```python
def maxCoins(nums: list[int]) -> int:
    # Step 5: Add sentinels
    nums = [1] + nums + [1]
    n = len(nums)
    
    # dp[i][j] = max coins from open interval (i,j)
    dp = [[0] * n for _ in range(n)]
    
    # Step 4: Fill by increasing length
    for length in range(2, n):          # length = j - i
        for i in range(0, n - length):
            j = i + length
            # Step 3: Try every last balloon k
            for k in range(i + 1, j):
                coins = nums[i] * nums[k] * nums[j]
                dp[i][j] = max(dp[i][j], dp[i][k] + coins + dp[k][j])
    
    return dp[0][n-1]

# Test
print(maxCoins([3, 1, 5, 8]))  # Output: 167
print(maxCoins([1, 5]))         # Output: 10
```

> **Time:** O(N³) — three nested loops over N positions  
> **Space:** O(N²) — the DP table

---

## Quick Reference: DP Pattern Recognition Card

```
Problem asks for min/max/count?  ──YES──►
  Has overlapping subproblems?   ──YES──►
    Has optimal substructure?    ──YES──► THINK DP

State Design Questions:
  • What index/position am I at?
  • What carry-forward constraints exist?
  • What mode/phase am I in?
  • Can I remove any dimension? (derive from others)

Recurrence Template:
  dp[state] = optimize { transition_cost + dp[substate] }
             for all valid substate → state transitions

Space Optimization:
  • Only prev row used? → O(N) rolling array
  • Only dp[i-1] used?  → O(1) two variables
  • Knapsack (0/1)?    → iterate capacity REVERSE

Complexity Estimation:
  • States × Transitions per state = Total work
  • 1D: O(N) or O(N²)
  • 2D: O(N²) or O(N²M)
  • Bitmask: O(2^N × N)
  • Digit: O(digits × states × 10)
```

---

## The 10 Most Important DP Problems to Master (One Per Pattern)

| # | Problem | Pattern | Key Insight |
|---|---|---|---|
| 1 | House Robber | 1D Linear | Include/exclude with gap |
| 2 | Unique Paths | 2D Grid | Additive paths |
| 3 | 0/1 Knapsack | Knapsack | Reverse iteration |
| 4 | LCS / Edit Distance | Two-Sequence | Match vs. mismatch |
| 5 | Burst Balloons | Interval | Last to burst, not first |
| 6 | House Robber III | Tree | Post-order, rob/skip |
| 7 | TSP (bitmask) | Bitmask | dp[mask][node] |
| 8 | Count special numbers | Digit | tight + pos + state |
| 9 | Best Time Buy/Sell III | State Machine | Phase transitions |
| 10 | Divide Conquer Opt. | CHT/Knuth | Monotone opt |

---

*This meta-guide is the foundation. Every pattern is explored in depth in the subsequent files (01 through 09).*

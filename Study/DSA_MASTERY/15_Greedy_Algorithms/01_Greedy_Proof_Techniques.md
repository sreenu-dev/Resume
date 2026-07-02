# Greedy Proof Techniques — Advanced Mastery Guide

> **A greedy algorithm without a proof is a heuristic.** This guide teaches you how to rigorously prove greedy correctness — the skill that separates senior engineers from juniors in algorithm design discussions.

---

## Table of Contents
1. [When Can We Be Greedy? — The Framework](#framework)
2. [Exchange Argument — The Master Proof Technique](#exchange)
3. [Greedy Stays Ahead Argument](#stays-ahead)
4. [Matroid Theory — Why Greedy Works](#matroid)
5. [Greedy vs DP Decision Framework](#vs-dp)
6. [Problems 1–8 with Formal Correctness Proofs](#problems)
7. [Interview Proof Cheat Sheet](#cheat-sheet)

---

## 1. When Can We Be Greedy? <a name="framework"></a>

### The Fundamental Question

A greedy algorithm makes **locally optimal** choices at each step. It's correct only when local optimality leads to global optimality.

**Two sufficient conditions:**

1. **Greedy choice property:** There exists an optimal solution that includes the greedy choice. (We can always "fix" any optimal solution to include our first greedy choice without making it worse.)

2. **Optimal substructure:** After making the greedy choice, the remaining subproblem has the same structure, and an optimal solution to it extends to an optimal global solution.

**Contrast with DP:** DP also uses optimal substructure but explores all choices. Greedy fixes one choice per step.

```
Problem has optimal substructure?
  No  → Neither greedy nor DP works directly
  Yes → 
    Has greedy choice property?
      Yes → Greedy (O(N log N) or better typically)
      No  → DP (O(N²) or O(N·K) typically)
```

---

## 2. Exchange Argument — The Master Proof Technique <a name="exchange"></a>

### The Template

1. **Assume** there exists an optimal solution OPT that **differs** from the greedy solution G.
2. **Find** the first position where they differ.
3. **Show** you can **exchange** OPT's choice for G's choice at that position without making OPT worse (i.e., the exchange is neutral or improves OPT).
4. **Conclude** by repeatedly swapping: OPT can be transformed into G without quality loss, so G is also optimal.

### Canonical Example: Activity Selection

**Problem:** Given N activities with start/finish times, select maximum non-overlapping activities.

**Greedy:** Sort by finish time, greedily pick each activity if it doesn't conflict with last picked.

**Proof via exchange argument:**

```
Theorem: Greedy-Activity-Select produces an optimal solution.

Proof:
Let G = {g₁, g₂, ..., gₖ} be greedy solution (sorted by finish time)
Let O = {o₁, o₂, ..., oₘ} be any optimal solution (sorted by finish time)

Claim: k = m (greedy picks as many as optimal)

Inductive exchange:
Suppose O and G agree on first i-1 activities: o₁=g₁,...,oᵢ₋₁=gᵢ₋₁
At position i: greedy picks gᵢ (earliest finishing among valid options)

Case 1: oᵢ = gᵢ → agree on position i, continue.
Case 2: oᵢ ≠ gᵢ → By greedy choice: finish(gᵢ) ≤ finish(oᵢ)
  (greedy always picks earliest finish, oᵢ was also valid, so gᵢ ≤ oᵢ in finish time)
  
  EXCHANGE: Replace oᵢ with gᵢ in O.
  - gᵢ starts after finish(oᵢ₋₁) = finish(gᵢ₋₁): compatible with previous.
  - gᵢ finishes ≤ oᵢ finishes: doesn't conflict with later activities in O.
  - New solution O' has same size as O (still m activities).
  
After m such exchanges, O becomes G, and G has same size as O.
Therefore |G| = |O| = m. Greedy is optimal. ∎
```

```python
def activity_selection(activities: list[tuple[int,int]]) -> list[tuple[int,int]]:
    """
    Activity Selection Problem.
    Greedy: sort by finish time, pick if non-overlapping.
    Time: O(N log N) sort + O(N) scan.
    """
    # Sort by finish time (the greedy choice)
    sorted_acts = sorted(activities, key=lambda x: x[1])
    
    selected = [sorted_acts[0]]
    last_finish = sorted_acts[0][1]
    
    for start, finish in sorted_acts[1:]:
        if start >= last_finish:  # non-overlapping
            selected.append((start, finish))
            last_finish = finish
    
    return selected

# Proof: greedy picks earliest finish → leaves maximum "room" for future activities.
# Any other choice at each step would only restrict future options.
```

---

## 3. Greedy Stays Ahead Argument <a name="stays-ahead"></a>

**Pattern:** Show that at every step i, the greedy solution is "at least as good" as any other solution up to that point.

### Jump Game Proof

```python
def can_jump(nums: list[int]) -> bool:
    """
    LeetCode 55. Can you reach the last index?
    Greedy: track maximum reachable index at each step.
    
    Greedy stays ahead proof:
    Let reach[i] = max index reachable after processing first i elements.
    
    Claim: reach[i] = max_{j≤i} (j + nums[j]) is optimal.
    
    Proof: Any algorithm A has reach_A[i] ≤ reach[i] because:
    - From any position j ≤ i, the farthest reachable is j + nums[j].
    - Greedy tracks the maximum over all such j.
    - No algorithm can reach farther than reach[i] from positions 0..i.
    
    Therefore, greedy's reach is always ≥ any other algorithm's reach.
    If greedy can't reach the end, no algorithm can. ∎
    """
    max_reach = 0
    for i in range(len(nums)):
        if i > max_reach:
            return False  # can't reach position i
        max_reach = max(max_reach, i + nums[i])
    return True

# Stronger version: Jump Game II (minimum jumps)
def jump_ii(nums: list[int]) -> int:
    """
    LeetCode 45. Greedy stays ahead: at each 'level', maximize reach.
    
    Proof: we must make at least one jump per BFS level.
    Taking the greedy (farthest reach) jump at each level is optimal
    because it ensures we enter the next level with maximum coverage.
    Any other choice would give ≤ coverage → ≥ more jumps later.
    """
    jumps = 0
    current_end = 0
    farthest = 0
    
    for i in range(len(nums) - 1):
        farthest = max(farthest, i + nums[i])
        if i == current_end:
            jumps += 1
            current_end = farthest
            if current_end >= len(nums) - 1:
                break
    
    return jumps
```

---

## 4. Matroid Theory — Why Greedy Works <a name="matroid"></a>

### What is a Matroid?

A **matroid** (E, I) is a set system where:
- **E** = ground set of elements
- **I** = collection of "independent sets" (feasible subsets)

**Matroid axioms:**
1. **Hereditary:** ∅ ∈ I; if A ∈ I and B ⊆ A, then B ∈ I.
2. **Augmentation:** If A, B ∈ I and |A| < |B|, then ∃x ∈ B\A such that A∪{x} ∈ I.

**Theorem (Rado-Edmonds):** A greedy algorithm that always adds the maximum-weight independent element produces an optimal basis of a weighted matroid.

### Examples of Matroids

```python
# 1. Graphic Matroid (Kruskal's MST)
# E = edges of graph, I = acyclic subsets (forests)
# Axioms hold: subsets of forests are forests; smaller forest can be augmented from larger

# 2. Uniform Matroid
# E = any set, I = subsets of size ≤ k
# Greedy: pick k largest-weight elements → optimal

# 3. Partition Matroid
# E = elements partitioned into groups G₁, ..., Gₖ
# I = subsets with ≤ cᵢ elements from Gᵢ

def kruskal_mst(n: int, edges: list[tuple[int,int,int]]) -> list[tuple[int,int,int]]:
    """
    Kruskal's MST = Greedy on graphic matroid.
    Sort edges by weight, add if doesn't form cycle (independent set stays acyclic).
    
    Matroid guarantee: this greedy produces optimal (minimum weight spanning tree).
    
    Time: O(E log E + E α(N)) with Union-Find
    """
    # Union-Find
    parent = list(range(n))
    rank = [0] * n
    
    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]  # path compression
            x = parent[x]
        return x
    
    def union(x, y):
        px, py = find(x), find(y)
        if px == py:
            return False  # cycle detected
        if rank[px] < rank[py]:
            px, py = py, px
        parent[py] = px
        if rank[px] == rank[py]:
            rank[px] += 1
        return True
    
    edges.sort(key=lambda e: e[2])  # greedy: smallest weight first
    mst = []
    
    for u, v, w in edges:
        if union(u, v):
            mst.append((u, v, w))
            if len(mst) == n - 1:
                break
    
    return mst
```

### Non-Matroid Example (Greedy Fails)

```python
# Weighted Job Scheduling: maximize total weight of non-overlapping jobs
# NOT a matroid: augmentation axiom fails
# Example: {(0,3,5), (3,6,5), (0,6,8)} — weight 8 job spans two weight-5 jobs
# Greedy (highest weight first) picks 8, gets value 8
# Optimal: pick two weight-5 jobs, get 10
# → Must use DP, not greedy

def weighted_job_scheduling_dp(jobs: list[tuple[int,int,int]]) -> int:
    """
    jobs = list of (start, end, weight).
    DP + binary search: O(N log N).
    """
    from bisect import bisect_right
    
    jobs.sort(key=lambda j: j[1])
    ends = [j[1] for j in jobs]
    n = len(jobs)
    
    dp = [0] * (n + 1)
    
    for i, (start, end, weight) in enumerate(jobs):
        # Find last job that doesn't conflict (ends ≤ start)
        idx = bisect_right(ends, start, 0, i)
        dp[i+1] = max(dp[i], dp[idx] + weight)
    
    return dp[n]
```

---

## 5. Greedy vs DP Decision Framework <a name="vs-dp"></a>

```
Problem: find optimal (max/min) over combinatorial choices
         ↓
Optimal substructure?
  No → Neither (exponential / specialized algorithms)
  Yes ↓
Greedy choice property?
  Yes → Greedy (single globally-valid local choice)
  No  → DP (must consider all choices at each step)
  
Hints for greedy:
  - "Earliest deadline first"
  - "Largest value first"  
  - "Minimum remaining" (coins with denominations 1,5,10,25)
  - Problem on intervals/scheduling
  
Hints for DP:
  - Overlapping subproblems
  - "Number of ways" problems
  - Choices affect each other in complex ways
  - 0/1 Knapsack (greedy doesn't work: fractions vs whole items)
```

---

## 6. Problems with Formal Correctness Proofs <a name="problems"></a>

---

### Problem 1: Gas Station (LeetCode 134)

```python
def can_complete_circuit(gas: list[int], cost: list[int]) -> int:
    """
    LeetCode 134. Circular route, find starting station.
    
    Greedy observation: if total gas >= total cost, a solution exists.
    Greedy: if at any point current tank goes negative, start over from next station.
    
    PROOF:
    1. If total_gas >= total_cost, a valid starting point exists (pigeonhole-like argument).
    2. Claim: if we fail at station j starting from i, then no station between i..j works.
       Proof: Let prefix[k] = sum(gas[i..k] - cost[i..k]).
       We fail at j because prefix[j] < 0.
       For any start k in (i, j): cumulative from k = prefix[j] - prefix[k-1].
       Since prefix[k-1] >= 0 (we hadn't failed before), prefix[j] - prefix[k-1] ≤ prefix[j] < 0.
       So starting from k also fails. ✓
    3. Therefore, the valid start must be station j+1 (reset here).
    
    Time: O(N), Space: O(1)
    """
    total_gas = sum(gas)
    total_cost = sum(cost)
    
    if total_gas < total_cost:
        return -1
    
    tank = 0
    start = 0
    
    for i in range(len(gas)):
        tank += gas[i] - cost[i]
        if tank < 0:
            start = i + 1  # current station can't be starting point
            tank = 0        # reset tank
    
    return start
```

---

### Problem 2: Task Scheduler (LeetCode 621)

```python
def least_interval(tasks: list[str], n: int) -> int:
    """
    LeetCode 621. Schedule tasks with cooldown n between same tasks.
    Minimize total time.
    
    Greedy: always pick the most frequent available task.
    
    Mathematical formula:
    Let f = max frequency, count_max = number of tasks with max frequency.
    
    Lower bound: max((f-1)*(n+1) + count_max, total_tasks)
    
    PROOF:
    - We need f-1 "gaps" between the last and first occurrence of the most frequent task.
    - Each gap is at least n+1 slots (n cooldown + 1 task slot).
    - The last group of count_max tasks adds count_max more.
    - If total tasks > (f-1)*(n+1) + count_max, tasks fill all gaps → no idle needed.
    
    Time: O(N), Space: O(1) (26 letters)
    """
    from collections import Counter
    
    freq = Counter(tasks)
    max_freq = max(freq.values())
    count_max = sum(1 for f in freq.values() if f == max_freq)
    
    return max(len(tasks), (max_freq - 1) * (n + 1) + count_max)
```

---

### Problem 3: Assign Cookies (LeetCode 455)

```python
def find_content_children(g: list[int], s: list[int]) -> int:
    """
    LeetCode 455. Assign cookies (sizes in s) to children (greed factors in g).
    Child i is content if cookie >= g[i]. Maximize content children.
    
    Greedy: sort both, match smallest sufficient cookie to least greedy child.
    
    EXCHANGE ARGUMENT PROOF:
    Claim: giving a child the smallest sufficient cookie is optimal.
    
    Suppose OPT gives child i a larger cookie c' when smaller c would suffice (c≤c').
    Exchange: give child i cookie c instead.
    - Child i is still content (c >= g[i]).
    - c' is now available for other children.
    - Since c' >= c >= g[i], c' might satisfy a greedier child that c can't.
    - This exchange doesn't decrease the number of content children.
    
    By induction, sorting and matching greedily is optimal. ∎
    
    Time: O(N log N + M log M), Space: O(1)
    """
    g.sort()
    s.sort()
    
    child = cookie = 0
    while child < len(g) and cookie < len(s):
        if s[cookie] >= g[child]:
            child += 1  # this child is content
        cookie += 1
    
    return child
```

---

### Problem 4: Lemonade Change (LeetCode 860)

```python
def lemon_change(bills: list[int]) -> bool:
    """
    LeetCode 860. Each customer pays $5, $10, or $20. Lemonade costs $5.
    Can you always give correct change?
    
    Greedy: when giving $15 change, prefer to use one $10 + one $5 over three $5s.
    
    PROOF:
    $5 bills are strictly more useful than $10 bills (can change both $10 and $20).
    $10 bills can only change $20.
    
    For $20 change ($15 needed):
    Option A: one $10 + one $5
    Option B: three $5s
    
    Prefer A: this preserves more $5 bills for future $10 customers.
    Each $5 saved is worth more (can change more situations).
    Formally: $5 ≻ $10 in utility. Use $10 first when possible. ✓
    
    Time: O(N), Space: O(1)
    """
    five = ten = 0
    
    for bill in bills:
        if bill == 5:
            five += 1
        elif bill == 10:
            if five == 0:
                return False
            five -= 1
            ten += 1
        else:  # bill == 20
            if ten > 0 and five > 0:
                ten -= 1; five -= 1  # Greedy: prefer $10+$5
            elif five >= 3:
                five -= 3
            else:
                return False
    
    return True
```

---

### Problem 5: Queue Reconstruction by Height (LeetCode 406)

```python
def reconstruct_queue(people: list[list[int]]) -> list[list[int]]:
    """
    LeetCode 406. [height, k] where k = people with height >= this who stand before.
    
    Greedy: sort by height descending, then by k ascending. Insert at position k.
    
    PROOF (exchange argument):
    After sorting tallest first: when inserting person [h, k], all already-placed
    people have height ≥ h.
    
    Inserting at position k: exactly k people before this person have height ≥ h
    (all placed people have height ≥ h, and we placed k of them before position k).
    
    Shorter people inserted later don't affect this person's k-count because
    they have height < h and aren't counted in k.
    
    Any other insertion order would fail: if we insert a shorter person first,
    it could displace taller people incorrectly. ✓
    
    Time: O(N²) insertions, Space: O(N)
    """
    people.sort(key=lambda x: (-x[0], x[1]))
    result = []
    for person in people:
        result.insert(person[1], person)
    return result
```

---

### Problem 6: Minimum Cost to Move Chips (LeetCode 1217)

```python
def min_cost_to_equal_chips(position: list[int]) -> int:
    """
    LeetCode 1217. Move chips to one position, even moves free, odd moves cost 1.
    
    Observation: all even positions are "free" to consolidate, all odd positions are "free"
    to consolidate among themselves. Moving between even/odd costs 1 per chip.
    
    Greedy: choose the parity (even or odd) with more chips — move fewer chips.
    
    This is a greedy choice: minimize total cost by choosing the majority parity.
    
    PROOF: the only non-zero cost is moving from even positions to odd (or vice versa).
    Each chip needs either 0 or 1 cost. Choosing majority minimizes total cost to minority count.
    
    Time: O(N), Space: O(1)
    """
    even = sum(1 for p in position if p % 2 == 0)
    odd = len(position) - even
    return min(even, odd)
```

---

### Problem 7: Non-Overlapping Intervals (LeetCode 435)

```python
def erase_overlap_intervals(intervals: list[list[int]]) -> int:
    """
    LeetCode 435. Min intervals to remove so rest don't overlap.
    = N - max non-overlapping intervals (classic activity selection).
    
    PROOF (same as activity selection, exchange argument):
    Sort by end time. Greedy: keep interval with earliest end that doesn't overlap.
    
    Claim: this maximizes the number of non-overlapping intervals.
    
    Exchange argument:
    Suppose OPT keeps interval with later end Eₒ when we'd keep Eₐ (Eₐ ≤ Eₒ).
    Replace Eₒ with Eₐ in OPT:
    - Eₐ doesn't conflict with anything before (it's compatible with last kept).
    - Eₐ ends ≤ Eₒ, so next interval in OPT compatible with Eₒ is also compatible with Eₐ.
    - Size of solution unchanged.
    
    Repeat: convert OPT to greedy solution without loss. ✓
    """
    if not intervals:
        return 0
    
    intervals.sort(key=lambda x: x[1])
    kept = 1
    last_end = intervals[0][1]
    
    for start, end in intervals[1:]:
        if start >= last_end:
            kept += 1
            last_end = end
    
    return len(intervals) - kept
```

---

### Problem 8: Fractional Knapsack (Greedy Correct, 0/1 Needs DP)

```python
def fractional_knapsack(capacity: int, items: list[tuple[int,int]]) -> float:
    """
    items = list of (weight, value).
    Can take fractions. Greedy: sort by value/weight ratio, take highest first.
    
    PROOF: Greedy is optimal for fractional knapsack.
    Exchange argument: any solution that doesn't take items in value/weight order
    can be improved by swapping a low-ratio portion for a high-ratio portion.
    
    Time: O(N log N), Space: O(1)
    """
    # Sort by value/weight ratio (descending)
    sorted_items = sorted(items, key=lambda x: x[1]/x[0], reverse=True)
    
    total_value = 0.0
    remaining = capacity
    
    for weight, value in sorted_items:
        if remaining <= 0:
            break
        take = min(weight, remaining)
        total_value += take * (value / weight)
        remaining -= take
    
    return total_value

def knapsack_01_dp(capacity: int, items: list[tuple[int,int]]) -> int:
    """
    0/1 Knapsack: greedy FAILS. Must use DP.
    
    Why greedy fails: items = [(10, 60), (20, 100), (30, 120)], capacity = 50
    Greedy (ratio): (10,60) ratio=6, take → (20,100) ratio=5, take → 
                    (30,120) ratio=4, can't fit 30 → value = 60+100 = 160
    Optimal: (20,100) + (30,120) = 220 ✓
    
    Time: O(N × capacity), Space: O(capacity) with 1D DP
    """
    n = len(items)
    dp = [0] * (capacity + 1)
    
    for weight, value in items:
        for c in range(capacity, weight - 1, -1):
            dp[c] = max(dp[c], dp[c - weight] + value)
    
    return dp[capacity]
```

---

## 7. Interview Proof Cheat Sheet <a name="cheat-sheet"></a>

### Exchange Argument Template

```
1. State what greedy choice you're making and WHY (e.g., earliest finish time)
2. Assume OPT differs from GREEDY at some first position
3. Show: swapping OPT's choice for GREEDY's choice at that position
   - Does NOT remove any other elements
   - Does NOT increase cost / decrease profit
   - Results in an equally good or better solution
4. Inductive conclusion: repeat until OPT = GREEDY
```

### Greedy Correctness Indicators

| If problem has... | Greedy likely correct? |
|-------------------|----------------------|
| Interval scheduling (maximize count) | YES — earliest finish |
| Interval partitioning (minimize rooms) | YES — earliest start |
| Minimum spanning tree | YES — Kruskal/Prim |
| Single-source shortest paths | YES — Dijkstra (non-negative weights) |
| Fractional knapsack | YES — max ratio first |
| 0/1 Knapsack | NO — use DP |
| Coin change (arbitrary denominations) | NO — use DP |
| Coin change (powers of k: 1,k,k²,...) | YES — greedy works |
| Matrix chain multiplication | NO — use DP |
| Activity selection | YES — earliest finish |
| Huffman coding | YES — min frequency first |

### Key Proof Phrases for Interviews

```
"Greedy is correct because [greedy choice] gives the most flexibility for future choices."
"By an exchange argument: any optimal solution using a different first choice 
 can be modified to use our greedy choice without decreasing the objective."
"The greedy stays ahead: at every step, our solution is at least as good as any other."
"This is a matroid: by the Rado-Edmonds theorem, greedy on weighted matroids is optimal."
```

### Common Interview Mistakes

```python
# MISTAKE 1: Claiming greedy works without proof
# "I'll sort by X and greedily pick" — interviewer: "Why is this optimal?"

# MISTAKE 2: Greedy on 0/1 Knapsack-like problems
items = [(1, 6), (2, 10), (3, 12)]  # weight, value
# Greedy by ratio: picks (1,6) ratio=6, (2,10) ratio=5, total=16
# Optimal: picks (2,10)+(3,12) = 22 ✗

# MISTAKE 3: Wrong greedy choice for activity selection
# Sorting by START time (wrong) vs FINISH time (correct)
# Counterexample: [(0,100), (1,2), (3,4)] sorted by start:
# Wrong greedy picks (0,100) and can't pick others → 1 activity
# Correct greedy: sort by finish → (1,2),(3,4) → 2 activities ✓

# MISTAKE 4: Greedy on interval scheduling weighted variant
# Weighted: choose fewest to miss, maximizing WEIGHT sum → needs DP
```

---

*Previous: [Pruning Techniques ←](../14_Recursion_Backtracking_DC/02_Pruning_Techniques.md) | Next: [Interval Scheduling & Greedy →](02_Interval_Scheduling_And_Greedy.md)*

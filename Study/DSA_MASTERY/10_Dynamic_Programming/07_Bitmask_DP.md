# Bitmask DP — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Pattern:** N ≤ 20 items. Track "which items have been used" with an integer bitmask of N bits. States: 2^N × (other dimension). The bitmask IS the state.

---

## Table of Contents
1. [Bitmask DP Fundamentals](#1-bitmask-dp-fundamentals)
2. [Traveling Salesman Problem — TSP](#2-traveling-salesman-problem)
3. [Shortest Path Visiting All Nodes](#3-shortest-path-visiting-all-nodes)
4. [Minimum Cost to Assign Workers to Jobs](#4-minimum-cost-to-assign-workers)
5. [Can I Win](#5-can-i-win)
6. [Stickers to Spell Word](#6-stickers-to-spell-word)
7. [Partition to K Equal Subset Sum](#7-partition-to-k-equal-subset-sum)
8. [Minimum XOR Sum of Two Arrays](#8-minimum-xor-sum)
9. [Count Ways to Divide Array into K Groups](#9-distribute-repeating-integers)
10. [Maximum AND Sum of Array](#10-maximum-and-sum)
11. [Sum Over Subsets (SOS DP)](#11-sum-over-subsets-sos-dp)

---

## 1. Bitmask DP Fundamentals

### Bit Operations Cheat Sheet

```python
# Set/check/clear individual bits
mask | (1 << i)      # set bit i
mask & (1 << i)      # check if bit i is set (non-zero = set)
mask & ~(1 << i)     # clear bit i
mask ^ (1 << i)      # toggle bit i

# Enumerate bits
bin(mask).count('1')         # popcount (number of set bits)
mask & (-mask)               # lowest set bit (LSB)
mask & (mask - 1)            # clear lowest set bit

# Enumerate all subsets of a mask
sub = mask
while sub > 0:
    # process sub
    sub = (sub - 1) & mask   # next smaller subset of mask

# Enumerate all submasks of all masks in O(3^N)
for mask in range(1 << N):
    sub = mask
    while sub > 0:
        process(mask, sub)
        sub = (sub - 1) & mask
```

### Template: Bitmask DP with Position

```python
N = ...   # number of items (N <= 20)
INF = float('inf')

# dp[mask][i] = optimal value for state where:
#   mask = set of items already used
#   i    = last item chosen (or current position)

dp = [[INF] * N for _ in range(1 << N)]
# Base case: start at node 0
dp[1 << 0][0] = 0  # only node 0 visited, currently at node 0

for mask in range(1 << N):
    for i in range(N):
        if dp[mask][i] == INF:
            continue
        if not (mask >> i & 1):  # i not in mask (consistency check)
            continue
        
        for j in range(N):
            if mask >> j & 1:  # j already visited
                continue
            new_mask = mask | (1 << j)
            dp[new_mask][j] = min(dp[new_mask][j], dp[mask][i] + cost[i][j])

# Answer: min over all ending positions
answer = min(dp[(1<<N)-1][i] + cost[i][0] for i in range(N))
```

> **Time:** O(2^N × N²) | **Space:** O(2^N × N)

---

## 2. Traveling Salesman Problem

**Problem:** N cities with pairwise distances. Find the shortest tour visiting every city exactly once and returning to start.

### State Definition

```
dp[mask][i] = minimum distance to visit exactly the cities in mask,
              ending at city i
              (city 0 is always the start)
```

### Recurrence

```
For each city j not in mask:
    dp[mask | (1<<j)][j] = min over dp[mask][i] + dist[i][j]
                           for all i in mask
```

**Answer:** `min(dp[(1<<N)-1][i] + dist[i][0])` for all `i` — complete the tour by returning to city 0.

```python
def tsp(dist: list[list[int]]) -> int:
    n = len(dist)
    INF = float('inf')
    FULL = (1 << n) - 1
    
    # dp[mask][i] = min cost to visit cities in mask, ending at i
    dp = [[INF] * n for _ in range(1 << n)]
    dp[1][0] = 0  # Start at city 0 (mask = 0b0001 for n=4)
    
    for mask in range(1 << n):
        for last in range(n):
            if dp[mask][last] == INF:
                continue
            if not (mask >> last & 1):
                continue  # last not in current set
            
            for nxt in range(n):
                if mask >> nxt & 1:
                    continue  # already visited
                new_mask = mask | (1 << nxt)
                cost = dp[mask][last] + dist[last][nxt]
                dp[new_mask][nxt] = min(dp[new_mask][nxt], cost)
    
    # Close the tour: return to city 0
    return min(dp[FULL][i] + dist[i][0] for i in range(n) if dp[FULL][i] != INF)

# Test: 4 cities
dist = [
    [0, 10, 15, 20],
    [10, 0, 35, 25],
    [15, 35, 0, 30],
    [20, 25, 30, 0]
]
print(tsp(dist))  # 80 (0→1→3→2→0: 10+25+30+15=80)
```

> **Time:** O(2^N × N²) | **Space:** O(2^N × N)

**Practical limit:** N ≤ 20 for reasonable runtime (2^20 × 20² ≈ 400M operations).

### TSP Backtracking (Finding the Route)

```python
def tsp_with_path(dist):
    n = len(dist)
    INF = float('inf')
    FULL = (1 << n) - 1
    
    dp   = [[INF]*n for _ in range(1<<n)]
    prev = [[-1]*n  for _ in range(1<<n)]
    dp[1][0] = 0
    
    for mask in range(1<<n):
        for last in range(n):
            if dp[mask][last]==INF or not(mask>>last&1): continue
            for nxt in range(n):
                if mask>>nxt&1: continue
                nm = mask|(1<<nxt)
                cost = dp[mask][last]+dist[last][nxt]
                if cost < dp[nm][nxt]:
                    dp[nm][nxt] = cost
                    prev[nm][nxt] = last
    
    # Find best ending city
    best_end = min(range(n), key=lambda i: dp[FULL][i]+dist[i][0] if dp[FULL][i]!=INF else INF)
    
    # Reconstruct
    path = [best_end]
    mask = FULL
    cur  = best_end
    while mask != 1:
        p = prev[mask][cur]
        path.append(p)
        mask ^= (1 << cur)
        cur = p
    
    return dp[FULL][best_end]+dist[best_end][0], path[::-1]
```

---

## 3. Shortest Path Visiting All Nodes

**Problem:** [LC 847] Undirected graph. Find shortest path (BFS steps, edges weighted 1) that visits every node at least once. Can revisit nodes.

### State: (current_node, visited_mask)

Since nodes can be revisited, the state must include the bitmask. Use BFS for shortest path.

```python
from collections import deque

def shortest_path_visiting_all(graph: list[list[int]]) -> int:
    n = len(graph)
    FULL = (1 << n) - 1
    
    # BFS: (node, visited_mask) → steps
    # Initialize: start from every node
    visited = [[False] * (1 << n) for _ in range(n)]
    queue = deque()
    
    for i in range(n):
        state = 1 << i
        queue.append((i, state, 0))
        visited[i][state] = True
    
    while queue:
        node, mask, steps = queue.popleft()
        
        if mask == FULL:
            return steps
        
        for neighbor in graph[node]:
            new_mask = mask | (1 << neighbor)
            if not visited[neighbor][new_mask]:
                visited[neighbor][new_mask] = True
                queue.append((neighbor, new_mask, steps + 1))
    
    return -1  # should never reach here for connected graph

print(shortest_path_visiting_all([[1,2,3],[0],[0],[0]]))  # 4
print(shortest_path_visiting_all([[1],[0,2,4],[1,3,4],[2],[1,2]]))  # 4
```

> **Time:** O(2^N × N²) — states × neighbors | **Space:** O(2^N × N)

---

## 4. Minimum Cost to Assign Workers

**Problem:** N workers and N jobs. `cost[i][j]` = cost for worker `i` to do job `j`. Assign each job to exactly one worker. Minimize total cost. (Classic Assignment Problem — Hungarian in O(N³), Bitmask DP in O(2^N × N²))

### State: assign jobs to first `popcount(mask)` workers

```
dp[mask] = minimum cost assigning jobs in `mask` to workers 0..popcount(mask)-1
```

When we set `dp[mask]`, the worker index = `popcount(mask) - 1` (0-indexed).

```python
def assign_workers(cost: list[list[int]]) -> int:
    n = len(cost)
    INF = float('inf')
    
    dp = [INF] * (1 << n)
    dp[0] = 0
    
    for mask in range(1 << n):
        if dp[mask] == INF:
            continue
        
        worker = bin(mask).count('1')  # current worker index (0-based)
        if worker >= n:
            continue
        
        for job in range(n):
            if mask >> job & 1:  # job already assigned
                continue
            new_mask = mask | (1 << job)
            dp[new_mask] = min(dp[new_mask], dp[mask] + cost[worker][job])
    
    return dp[(1 << n) - 1]

cost = [[9,2,7,8],[6,4,3,7],[5,8,1,8],[7,6,9,4]]
print(assign_workers(cost))  # 13 (2+3+1+7 or similar optimal)
```

> **Time:** O(2^N × N) | **Space:** O(2^N)

**Note:** For N ≤ 15, bitmask DP (O(2^N × N)) is faster than Hungarian (O(N³)) when N is small. For N > 20, use Hungarian.

---

## 5. Can I Win

**Problem:** [LC 464] Players alternately choose an integer from 1..maxChoosableInteger (cannot reuse). First player to push cumulative total ≥ desiredTotal wins. Can the first player guarantee a win?

### State: bitmask of chosen numbers

```
dp[mask] = True if the CURRENT player (whoever's turn it is with this mask used) can win
```

```python
from functools import lru_cache

def can_i_win(max_choosable: int, desired_total: int) -> bool:
    # Edge cases
    if desired_total <= 0:
        return True
    total_sum = max_choosable * (max_choosable + 1) // 2
    if total_sum < desired_total:
        return False
    
    @lru_cache(maxsize=None)
    def can_win(used_mask, remaining):
        for i in range(1, max_choosable + 1):
            if used_mask >> i & 1:
                continue  # already used
            
            # Choose i: if i >= remaining, current player wins
            if i >= remaining:
                return True
            
            # If opponent loses after we pick i → we win
            if not can_win(used_mask | (1 << i), remaining - i):
                return True
        
        return False  # all choices lead to opponent winning
    
    return can_win(0, desired_total)

print(can_i_win(10, 11))  # False
print(can_i_win(10, 0))   # True
print(can_i_win(10, 40))  # False (sum=55>=40 but strategy fails)
```

> **Time:** O(2^N × N) | **Space:** O(2^N)

---

## 6. Stickers to Spell Word

**Problem:** [LC 691] Stickers (each usable multiple times). Minimum stickers to spell target word. Characters from each sticker can be freely rearranged.

### State: bitmask of which target characters are satisfied

```
dp[mask] = minimum stickers to satisfy characters at positions in mask
```

```python
from functools import lru_cache
from collections import Counter

def min_stickers(stickers: list[str], target: str) -> int:
    n = len(target)
    
    # Pre-compute: for each sticker, which target characters it can cover and how many
    sticker_counts = [Counter(s) for s in stickers]
    
    @lru_cache(maxsize=None)
    def dp(mask):
        # mask = bitmask of target characters still needed
        if mask == 0:
            return 0
        
        # Find the first unmet character (bit)
        first_unmet = -1
        for i in range(n):
            if mask >> i & 1:
                first_unmet = i
                break
        
        result = float('inf')
        
        for sticker in sticker_counts:
            # Only try stickers that can cover the first unmet character
            if sticker[target[first_unmet]] == 0:
                continue
            
            # Use this sticker: reduce mask by what this sticker provides
            new_mask = mask
            for i in range(n):
                if (mask >> i & 1) and sticker[target[i]] > 0:
                    sticker[target[i]] -= 1
                    new_mask ^= (1 << i)
            
            # Restore sticker counts
            for i in range(n):
                if not (mask >> i & 1) or (new_mask >> i & 1):
                    pass  # bit was already 0, or still 1 — no change
                else:
                    sticker[target[i]] += 1  # restore
            
            sub_result = dp(new_mask)
            if sub_result != float('inf'):
                result = min(result, 1 + sub_result)
        
        return result
    
    # Cleaner implementation without Counter mutation:
    @lru_cache(maxsize=None)
    def dp_clean(mask):
        if mask == 0:
            return 0
        
        # Fix first unset character
        first = -1
        for i in range(n):
            if mask >> i & 1:
                first = i
                break
        
        res = float('inf')
        for sc in sticker_counts:
            if sc[target[first]] == 0:
                continue  # sticker can't help with first unmet char
            
            new_mask = mask
            temp = Counter(sc)  # copy to avoid mutation issues with lru_cache
            for i in range(n):
                if (new_mask >> i & 1) and temp[target[i]] > 0:
                    temp[target[i]] -= 1
                    new_mask ^= (1 << i)
            
            sub = dp_clean(new_mask)
            if sub != float('inf'):
                res = min(res, 1 + sub)
        
        return res
    
    result = dp_clean((1 << n) - 1)
    return result if result != float('inf') else -1

print(min_stickers(["with","example","science"], "thehat"))  # 3
print(min_stickers(["notice","possible"], "basicbasic"))     # -1
```

> **Time:** O(2^N × |stickers| × N) | **Space:** O(2^N)

---

## 7. Partition to K Equal Subset Sum

**Problem:** [LC 698] Can the array be partitioned into K subsets of equal sum?

### Bitmask DP Approach

```
dp[mask] = True if the elements in `mask` can form some number of complete groups
           where each group has exactly target = total/K sum
```

Also track: `current_sum = sum of mask modulo target` (how filled the current incomplete bucket is).

```python
from functools import lru_cache

def can_partition_k_subsets(nums: list[int], k: int) -> bool:
    total = sum(nums)
    if total % k != 0:
        return False
    
    target = total // k
    nums.sort(reverse=True)  # prune: large elements first
    
    if nums[0] > target:
        return False
    
    n = len(nums)
    
    @lru_cache(maxsize=None)
    def dp(mask, current_sum):
        """Can we partition unused elements (complement of mask) into remaining buckets?"""
        if mask == (1 << n) - 1:
            return True  # all elements used
        
        for i in range(n):
            if mask >> i & 1:
                continue  # already used
            if current_sum + nums[i] > target:
                continue  # exceeds bucket
            
            new_mask = mask | (1 << i)
            new_sum  = (current_sum + nums[i]) % target
            
            if dp(new_mask, new_sum):
                return True
        
        return False
    
    return dp(0, 0)

print(can_partition_k_subsets([4,3,2,3,5,2,1], 4))  # True ([5],[4,1],[3,2],[3,2])
print(can_partition_k_subsets([1,2,3,4], 3))         # False
```

> **Time:** O(2^N × N) | **Space:** O(2^N)

**Optimization:** Sort descending + early termination on `nums[i] > remaining_in_bucket` significantly prunes the search space.

---

## 8. Minimum XOR Sum

**Problem:** [LC 1879] Two arrays A and B (same length). Reorder B to minimize sum of XOR(A[i], B[i]).

### State: bitmask of which B elements have been assigned

```
dp[mask] = minimum XOR sum when elements in mask of B are assigned to A[0..popcount(mask)-1]
```

```python
def minimum_xor_sum(nums1: list[int], nums2: list[int]) -> int:
    n = len(nums1)
    INF = float('inf')
    
    dp = [INF] * (1 << n)
    dp[0] = 0
    
    for mask in range(1 << n):
        if dp[mask] == INF:
            continue
        
        # Next index in nums1 to assign
        i = bin(mask).count('1')
        if i >= n:
            continue
        
        for j in range(n):
            if mask >> j & 1:
                continue  # nums2[j] already used
            
            new_mask = mask | (1 << j)
            cost = nums1[i] ^ nums2[j]
            dp[new_mask] = min(dp[new_mask], dp[mask] + cost)
    
    return dp[(1 << n) - 1]

print(minimum_xor_sum([1,2], [2,3]))    # 2 (1^2 + 2^3 = 3+1=4? No: 1^3+2^2=2+0=2)
print(minimum_xor_sum([1,0,3], [5,3,2]))  # 8
```

> **Time:** O(2^N × N) | **Space:** O(2^N)

---

## 9. Distribute Repeating Integers

**Problem:** [LC 1655] Quantities of n customers. Integers supply (can have repeats). Can all customers be satisfied? (customers[i] units needed).

### Bitmask DP on Customer Subsets

```
dp[mask] = minimum integer supply "blocks" needed to satisfy customer subset mask
```

```python
def can_distribute(nums: list[int], quantity: list[int]) -> bool:
    from collections import Counter
    
    counts = sorted(Counter(nums).values(), reverse=True)
    n = len(quantity)
    
    # Precompute subset sums
    subset_sum = [0] * (1 << n)
    for mask in range(1 << n):
        for i in range(n):
            if mask >> i & 1:
                subset_sum[mask] += quantity[i]
    
    # dp[mask] = can we satisfy customers in mask with some prefix of counts?
    dp = [False] * (1 << n)
    dp[0] = True
    
    for count in counts:  # for each distinct integer value
        # Process masks in decreasing order (to avoid reusing count)
        for mask in range((1 << n) - 1, -1, -1):
            if not dp[mask]:
                continue
            
            # Try to assign this count to some subset of remaining customers
            remaining = ((1 << n) - 1) ^ mask  # customers not yet satisfied
            sub = remaining
            while sub > 0:
                if subset_sum[sub] <= count:
                    dp[mask | sub] = True
                sub = (sub - 1) & remaining
    
    return dp[(1 << n) - 1]

print(can_distribute([1,2,3,4], [2]))         # True
print(can_distribute([1,2,3,3], [2]))         # True
print(can_distribute([1,1,2,2], [2,2]))       # True
```

> **Time:** O(3^N × |counts|) — subset enumeration | **Space:** O(2^N)

---

## 10. Maximum AND Sum of Array

**Problem:** [LC 2172] N integers and `numSlots` slots. Each slot holds ≤ 2 integers. Maximize sum of `nums[i] AND slot_index` for all assigned pairs.

### State: which slot positions are used

Since each slot can hold 2 items, `mask` represents how many times each slot has been filled. Encode as: bit 2k-2 and bit 2k-1 for slot k.

```python
def maximum_and_sum(nums: list[int], num_slots: int) -> int:
    n = len(nums)
    # Each slot has 2 "positions" → total positions = 2 * num_slots
    total_positions = 2 * num_slots
    
    dp = [0] * (1 << total_positions)
    
    for mask in range(1 << total_positions):
        idx = bin(mask).count('1')  # which nums index we're assigning
        if idx >= n:
            continue
        
        for pos in range(total_positions):
            if mask >> pos & 1:
                continue  # position already filled
            
            slot = pos // 2 + 1  # slot number (1-indexed)
            new_mask = mask | (1 << pos)
            gain = nums[idx] & slot
            dp[new_mask] = max(dp[new_mask], dp[mask] + gain)
    
    return max(dp)

print(maximum_and_sum([1,2,3,4,5,6], 3))  # 9
print(maximum_and_sum([1,3,10,4,7,1], 9)) # 24
```

> **Time:** O(2^(2×numSlots) × numSlots) | **Space:** O(2^(2×numSlots))

---

## 11. Sum Over Subsets (SOS DP)

**Problem:** For every mask, compute `f[mask] = sum of a[sub] for all sub ⊆ mask`. Naively O(3^N) (enumerate all submasks of all masks). DP achieves O(N × 2^N).

### SOS DP Template

```python
def sum_over_subsets(a: list[int], n: int) -> list[int]:
    """
    f[mask] = sum of a[sub] for all sub ⊆ mask
    
    DP: process bit by bit.
    After considering bit k: f[mask] accounts for all subsets of mask that
    can freely vary in bits 0..k, and must equal mask in bits k+1..N-1.
    """
    f = a[:]  # copy
    
    for k in range(n):
        for mask in range(1 << n):
            if mask >> k & 1:
                f[mask] += f[mask ^ (1 << k)]
                # Add contribution of submasks that have bit k CLEARED
    
    return f

# Test: n=3, a=[0]*8, a[0b101]=1, a[0b011]=1
n = 3
a = [0] * (1 << n)
a[0b101] = 1  # = 5
a[0b011] = 1  # = 3
f = sum_over_subsets(a, n)
# f[0b111] = a[0b101] + a[0b011] + a[0b111] + a[0b001] + ... (all subsets of 111)
print(f[0b111])  # 2 (only 0b101 and 0b011 are non-zero)
print(f[0b101])  # 1 (subset {0b101}: only 0b101 itself and 0b001, 0b100, 0b000 → just 0b101)
```

> **Time:** O(N × 2^N) | **Space:** O(2^N)

### SOS DP Correctness Proof

After the k-th iteration, `f[mask]` = sum of `a[sub]` for all `sub ⊆ mask` that agree with `mask` on bits `k+1..N-1` (and can freely vary on bits `0..k`).

After all N iterations: `f[mask]` = sum over ALL subsets of mask. ✓

### Application: Count Submasks with Property

```python
def count_pairs_with_zero_and(a: list[int]) -> int:
    """Count pairs (i,j) where a[i] & a[j] == 0."""
    n = max(a).bit_length()
    cnt = [0] * (1 << n)
    for x in a:
        cnt[x] += 1
    
    # f[mask] = number of elements x where x ⊆ mask (x AND mask == x, i.e., x is submask)
    # Wait, we want: for each x, how many y have x & y == 0?
    # x & y == 0 iff y ⊆ complement(x)
    # So: for each x, answer += cnt_subsets[complement(x)]
    
    # Compute cnt_subsets[mask] = sum of cnt[sub] for sub ⊆ mask (using SOS)
    f = cnt[:]
    for k in range(n):
        for mask in range(1 << n):
            if mask >> k & 1:
                f[mask] += f[mask ^ (1 << k)]
    
    total = 0
    FULL = (1 << n) - 1
    for x in a:
        total += f[FULL ^ x]  # complement of x
    # Subtract self-pairs if x & x == 0 (only x=0 case)
    total -= cnt[0]  # (0,0) counted once, remove duplicate
    return total // 2  # each pair counted twice
```

> **Time:** O(N × 2^N) | **Space:** O(2^N)

---

## Bitmask DP Problem Recognition Guide

| N range | Approach | State Size | Typical Problem |
|---|---|---|---|
| N ≤ 5 | Brute force | — | Too small for bitmask |
| N ≤ 15 | Bitmask DP | O(2^N × N) | Assignment, TSP |
| N ≤ 20 | Bitmask DP | O(2^N × N²) | TSP stretched |
| N ≤ 25 | Meet in middle | O(2^(N/2)) | Subset sum |
| N > 25 | DP with SOS | O(N × 2^N) | Aggregate problems |

### Key Recognition Phrases

```
"Visit/assign each of N items exactly once" → TSP-style dp[mask][last]
"Partition N items into groups" → dp[mask] = can we form groups from these items
"Two-player game on N choices" → dp[mask] = current player wins?
"Minimum cost matching N items" → Assignment dp[mask][worker_count]
"Count subsets with AND/OR/XOR property" → SOS DP
```

### Bitmask DP Iteration Order

```python
# Standard: iterate mask in increasing order
# This ensures that when processing mask, all submasks < mask are already done

for mask in range(1 << n):       # increasing order
    for i in range(n):
        if mask >> i & 1:
            # dp[mask] can use dp[mask without bit i] = dp[mask ^ (1<<i)]
            dp[mask] = update(dp[mask ^ (1<<i)], ...)

# Alternatively: iterate by popcount (number of bits set)
from itertools import combinations
for k in range(n+1):
    for bits in combinations(range(n), k):
        mask = sum(1<<b for b in bits)
        # process dp[mask]
```

# Bellman-Ford and Floyd-Warshall — FAANG Mastery Guide

> **Audience**: Engineers who know Dijkstra — master negative-weight handling, all-pairs paths, and arbitrage.  
> **Goal**: Complete mastery of Bellman-Ford (with SPFA) and Floyd-Warshall including proofs, negative cycle detection, and 5+ hard problems.

---

## Table of Contents
1. [Bellman-Ford — Full Implementation & Proof](#1-bellman-ford)
2. [Why N-1 Iterations Suffice — Formal Proof](#2-n-1-iterations-proof)
3. [Negative Cycle Detection](#3-negative-cycle-detection)
4. [SPFA — Bellman-Ford with Queue Optimization](#4-spfa)
5. [Floyd-Warshall — O(V³) All-Pairs Shortest Path](#5-floyd-warshall)
6. [Transitive Closure and Negative Cycles via Floyd-Warshall](#6-transitive-closure)
7. [Problem Set with Full Solutions](#7-problem-set)
8. [Interview Tips and Comparisons](#8-interview-tips)

---

## 1. Bellman-Ford — Full Implementation

Bellman-Ford handles **negative edge weights** (Dijkstra cannot). It iteratively **relaxes** all edges, propagating shortest distances one "hop" at a time.

```python
from typing import List, Tuple

def bellman_ford(n: int, edges: List[Tuple[int, int, int]], src: int) -> List[float]:
    """
    Bellman-Ford single-source shortest path.
    edges: list of (u, v, weight). Weights can be negative.
    Returns dist[] where dist[i] = shortest distance from src to i.
    Returns None if a negative cycle is reachable from src.
    
    Time:  O(V × E)
    Space: O(V)
    """
    INF  = float('inf')
    dist = [INF] * n
    dist[src] = 0

    # N-1 relaxation rounds
    for i in range(n - 1):
        updated = False
        for u, v, w in edges:
            if dist[u] != INF and dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                updated = True
        if not updated:   # Early termination: no improvement → done
            break

    # N-th round: if any relaxation still possible → negative cycle
    for u, v, w in edges:
        if dist[u] != INF and dist[u] + w < dist[v]:
            return None   # Negative cycle detected

    return dist
```

---

## 2. Why N-1 Iterations Suffice — Formal Proof

**Theorem**: After k iterations of Bellman-Ford, `dist[v]` contains the shortest path from source to v using **at most k edges**.

**Proof by induction**:

- **Base case (k=0)**: `dist[src]=0`, all others INF. Correct: path with 0 edges from src to src has cost 0; no other node is reachable with 0 edges.

- **Inductive step**: Assume after k iterations, `dist[v]` = shortest path using ≤ k edges. In iteration k+1, for every edge (u,v,w), we check if `dist[u] + w < dist[v]`. By inductive hypothesis, `dist[u]` = shortest path to u using ≤ k edges. So `dist[u] + w` = cost of the shortest ≤k-edge path to u, plus edge (u,v) = cost of a ≤(k+1)-edge path to v. We take the minimum, so `dist[v]` correctly becomes shortest path using ≤ k+1 edges.

- **Why N-1 suffices**: In a graph with N nodes, any shortest path (without negative cycles) has at most **N-1 edges** (a path longer than N-1 would visit a node twice = cycle; if that cycle has non-negative weight, removing it gives a shorter path; negative cycles are handled separately).

- **Therefore**: After N-1 iterations, all shortest paths (using ≤ N-1 edges) are correct.

**Corollary**: If any distance still improves in the N-th iteration, the improved path uses N edges, which means it goes through a cycle. Since it's an improvement (lower cost), the cycle must have **negative total weight**.

---

## 3. Negative Cycle Detection

```python
def detect_negative_cycles(n: int, edges: List[Tuple[int, int, int]]) -> List[int]:
    """
    Find ALL nodes reachable through a negative cycle.
    Steps:
    1. Run N-1 rounds of relaxation (may not have reached all nodes from single source)
    2. Run another N rounds — any node whose distance changes is reachable via neg cycle
    3. BFS/DFS from those nodes to find all affected nodes
    
    Uses multi-source approach to handle all components.
    """
    INF  = float('inf')
    dist = [0] * n    # Multi-source: start all at 0 (virtual super-source)

    for _ in range(n - 1):
        for u, v, w in edges:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w

    # Nodes that can be relaxed in round N are on or reachable from negative cycles
    on_neg_cycle = [False] * n
    for _ in range(n):
        for u, v, w in edges:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                on_neg_cycle[v] = True

    # BFS: propagate "reachable from negative cycle" through forward edges
    from collections import deque
    adj = [[] for _ in range(n)]
    for u, v, w in edges:
        adj[u].append(v)

    queue = deque(i for i in range(n) if on_neg_cycle[i])
    while queue:
        u = queue.popleft()
        for v in adj[u]:
            if not on_neg_cycle[v]:
                on_neg_cycle[v] = True
                queue.append(v)

    return [i for i in range(n) if on_neg_cycle[i]]
```
**Time**: O(V × E) | **Space**: O(V)

---

## 4. SPFA — Shortest Path Faster Algorithm

SPFA is Bellman-Ford with a queue: only re-process nodes whose distances were just improved. In practice O(kE) where k is small, but worst case still O(VE).

```python
from collections import deque

def spfa(n: int, edges: List[Tuple[int, int, int]], src: int):
    """
    SPFA: Bellman-Ford optimized with queue.
    Only enqueue nodes whose distance was updated.
    
    Average case: O(E). Worst case: O(VE).
    Negative cycle detection: if any node is enqueued > N times.
    """
    adj = [[] for _ in range(n)]
    for u, v, w in edges:
        adj[u].append((v, w))

    INF       = float('inf')
    dist      = [INF] * n
    dist[src] = 0
    in_queue  = [False] * n
    count     = [0] * n     # Times each node is enqueued

    queue = deque([src])
    in_queue[src] = True
    count[src] = 1

    while queue:
        u = queue.popleft()
        in_queue[u] = False

        for v, w in adj[u]:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                if not in_queue[v]:
                    queue.append(v)
                    in_queue[v] = True
                    count[v] += 1
                    if count[v] > n:     # Node enqueued N+1 times → neg cycle
                        return None

    return dist
```
**Time**: O(E) average, O(VE) worst | **Space**: O(V+E)

---

## 5. Floyd-Warshall — All-Pairs Shortest Path

### Algorithm

Floyd-Warshall computes shortest paths between **every pair** of nodes using dynamic programming over intermediate nodes.

**DP formulation**:  
`dp[k][i][j]` = shortest path from i to j using only nodes {0, 1, ..., k} as intermediates.

**Recurrence**:  
`dp[k][i][j] = min(dp[k-1][i][j], dp[k-1][i][k] + dp[k-1][k][j])`

**Why it works**: For any shortest path from i to j, either:
1. It doesn't use node k → same as `dp[k-1][i][j]`
2. It uses node k → it goes i→...→k→...→j, both subpaths use only nodes {0..k-1} → `dp[k-1][i][k] + dp[k-1][k][j]`

Since we process k from 0 to n-1, by the time we use k, `dp[i][k]` and `dp[k][j]` already hold optimal values for paths through {0..k-1}. Space-optimized to 2D.

```python
def floyd_warshall(n: int, edges: List[Tuple[int, int, int]]) -> List[List[float]]:
    """
    All-pairs shortest path.
    Returns dist[i][j] = shortest path from i to j.
    dist[i][j] = -inf if j is reachable from i via a negative cycle.
    
    Time:  O(V³)
    Space: O(V²)
    """
    INF  = float('inf')
    dist = [[INF]*n for _ in range(n)]
    for i in range(n):
        dist[i][i] = 0

    for u, v, w in edges:
        dist[u][v] = min(dist[u][v], w)   # Handle parallel edges

    # Core DP: try every intermediate node k
    for k in range(n):
        for i in range(n):
            for j in range(n):
                if dist[i][k] != INF and dist[k][j] != INF:
                    dist[i][j] = min(dist[i][j], dist[i][k] + dist[k][j])

    # Negative cycle detection: if dist[i][i] < 0, i is on a negative cycle
    # Propagate: if path i→j passes through a negative cycle, dist[i][j] = -inf
    for i in range(n):
        for j in range(n):
            for k in range(n):
                if dist[k][k] < 0 and dist[i][k] != INF and dist[k][j] != INF:
                    dist[i][j] = -INF

    return dist
```
**Time**: O(V³) | **Space**: O(V²)

---

## 6. Transitive Closure and Applications

### Transitive Closure

```python
def transitive_closure(n: int, edges: List[Tuple[int, int]]) -> List[List[bool]]:
    """
    reachable[i][j] = True if j is reachable from i.
    Floyd-Warshall variant using boolean OR instead of min.
    Time: O(V³), Space: O(V²)
    """
    reach = [[False]*n for _ in range(n)]
    for i in range(n):
        reach[i][i] = True
    for u, v in edges:
        reach[u][v] = True

    for k in range(n):
        for i in range(n):
            for j in range(n):
                reach[i][j] = reach[i][j] or (reach[i][k] and reach[k][j])

    return reach
```

### Negative Cycle Detection in Undirected Graph via Floyd-Warshall

In an undirected graph with a negative-weight edge (u,v,w) where w < 0, you can traverse it back and forth for unbounded gain. Check: if any edge (u,v,w) has w < 0 → immediately a negative cycle.

For directed graphs: after Floyd-Warshall, if `dist[i][i] < 0` for any i → negative cycle through i.

---

## 7. Problem Set

---

### Problem 1: Cheapest Flights Within K Stops — Bellman-Ford Approach (LC 787)

```python
def find_cheapest_price(n: int, flights: List[List[int]],
                         src: int, dst: int, k: int) -> int:
    """
    Bellman-Ford with exactly K+1 rounds.
    CRITICAL: Must snapshot prev distances each round to prevent using > k+1 edges
    in a single round (cascading updates within one iteration).
    """
    INF  = float('inf')
    dist = [INF] * n
    dist[src] = 0

    for i in range(k + 1):   # k stops = k+1 edges = k+1 rounds
        temp = dist[:]        # Snapshot: prevent cascading updates
        for u, v, price in flights:
            if dist[u] != INF and dist[u] + price < temp[v]:
                temp[v] = dist[u] + price
        dist = temp

    return -1 if dist[dst] == INF else dist[dst]
```
**Time**: O(K × E) | **Space**: O(V)

**Why snapshot is critical**: Without `temp = dist[:]`, within round i you might update dist[u] and then use the updated dist[u] to update dist[v] in the same round — effectively using 2+ edges in one "round," violating the at-most-K-stops constraint.

---

### Problem 2: Network Delay Time — Bellman-Ford vs Dijkstra Comparison (LC 743)

```python
def network_delay_bellman(times: List[List[int]], n: int, k: int) -> int:
    """
    Bellman-Ford approach to LC 743.
    All weights are positive here, but demonstrates the algorithm.
    """
    INF  = float('inf')
    dist = [INF] * (n+1)
    dist[k] = 0

    for _ in range(n - 1):
        updated = False
        for u, v, w in times:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                updated = True
        if not updated:
            break

    max_dist = max(dist[1:])
    return -1 if max_dist == INF else max_dist
```
**Comparison**: For this problem (positive weights), Dijkstra is O((V+E) log V) vs Bellman-Ford O(VE). Dijkstra wins. Use Bellman-Ford only when negative weights exist.

---

### Problem 3: Find the City with Smallest Number of Neighbors — Floyd-Warshall (LC 1334)

```python
def find_the_city_fw(n: int, edges: List[List[int]], distanceThreshold: int) -> int:
    """
    Floyd-Warshall for all-pairs shortest paths, then count reachable cities per node.
    More elegant than running Dijkstra n times for small n.
    """
    INF  = float('inf')
    dist = [[INF]*n for _ in range(n)]
    for i in range(n):
        dist[i][i] = 0
    for u, v, w in edges:
        dist[u][v] = min(dist[u][v], w)
        dist[v][u] = min(dist[v][u], w)

    for k in range(n):
        for i in range(n):
            for j in range(n):
                if dist[i][k] + dist[k][j] < dist[i][j]:
                    dist[i][j] = dist[i][k] + dist[k][j]

    best_city  = -1
    best_count = n + 1

    for i in range(n):
        count = sum(1 for j in range(n) if i != j and dist[i][j] <= distanceThreshold)
        if count <= best_count:   # >= city index wins ties
            best_count = count
            best_city  = i

    return best_city
```
**Time**: O(V³) | **Space**: O(V²)

---

### Problem 4: Arbitrage Detection (Negative Cycle in Log-Transformed Graph)

```python
def find_arbitrage(currencies: List[str],
                   exchange_rates: List[List[float]]) -> bool:
    """
    Arbitrage: sequence of currency exchanges that returns profit.
    e.g., USD → EUR → GBP → USD with product of rates > 1.
    
    Transform: take -log of each rate.
    - Product of rates > 1 ↔ sum of log(rates) > 0 ↔ sum of -log(rates) < 0
    - Arbitrage ↔ negative cycle in transformed graph
    
    Apply Bellman-Ford negative cycle detection.
    """
    import math
    n = len(currencies)
    
    # Build edges with -log(rate) weights
    edges = []
    for i in range(n):
        for j in range(n):
            if i != j and exchange_rates[i][j] > 0:
                edges.append((i, j, -math.log(exchange_rates[i][j])))

    # Multi-source Bellman-Ford (start from all nodes to detect any neg cycle)
    dist = [0.0] * n   # Start all at 0 (virtual super-source)

    for _ in range(n - 1):
        for u, v, w in edges:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w

    # Check for negative cycle in N-th round
    for u, v, w in edges:
        if dist[u] + w < dist[v]:
            return True   # Arbitrage opportunity found

    return False
```
**Time**: O(V³) for complete graph | **Space**: O(V²)

**Key Insight**: 
- Exchange rate cycle profit > 1 means `r1 × r2 × ... × rk > 1`
- Taking log: `log(r1) + log(r2) + ... + log(rk) > 0`
- Negating: `-log(r1) + (-log(r2)) + ... + (-log(rk)) < 0`
- A cycle with negative sum = negative cycle in the transformed graph

---

### Problem 5: Minimum Cost to Make Array Palindrome via Floyd-Warshall

This demonstrates Floyd-Warshall for all-pairs distances used in DP.

```python
def min_cost_to_equalize(dist: List[List[int]], costs: List[int], n: int) -> int:
    """
    Given pairwise travel costs (already run Floyd-Warshall),
    find minimum cost to bring all elements to same location.
    
    For each candidate meeting point c:
    total_cost = sum(dist[i][c] * weight[i]) for all i
    Return minimum over all c.
    """
    min_cost = float('inf')
    for c in range(n):
        total = sum(dist[i][c] * costs[i] for i in range(n))
        min_cost = min(min_cost, total)
    return min_cost
```

---

## 8. Interview Tips and Comparisons

### Algorithm Selection Matrix

| Condition | Algorithm | Time |
|---|---|---|
| Single source, non-negative weights | Dijkstra | O((V+E) log V) |
| Single source, negative weights, no neg cycle | Bellman-Ford | O(VE) |
| Single source, negative weights, detect neg cycle | Bellman-Ford | O(VE) |
| Single source, DAG (any weights) | Topo DP | O(V+E) |
| Single source, 0-1 weights | 0-1 BFS | O(V+E) |
| Single source, at-most-K edges | Bellman-Ford (K rounds) | O(KE) |
| All pairs, sparse | Dijkstra × V times | O(V(V+E) log V) |
| All pairs, dense | Floyd-Warshall | O(V³) |
| Reachability only | BFS/DFS or Floyd-Warshall (bool) | O(V+E) or O(V³) |

### The Snapshot Trick for K-Constrained Bellman-Ford

The most common interview bug with K-stop problems:
```python
# WRONG: cascading updates within one round
for u, v, w in flights:
    dist[v] = min(dist[v], dist[u] + w)  # dist[u] might have been updated this round!

# CORRECT: snapshot prevents within-round cascading
temp = dist[:]
for u, v, w in flights:
    if dist[u] + w < temp[v]:  # Use old dist[u], update temp[v]
        temp[v] = dist[u] + w
dist = temp
```

### Floyd-Warshall Loop Order
The loop order is **k (intermediate), i (source), j (dest)**. Never change this. k must be outermost because when processing intermediate node k, we need `dist[i][k]` and `dist[k][j]` to already reflect paths through intermediates {0..k-1}.

### Negative Cycle Invariants

| Graph Type | Detection Method |
|---|---|
| Directed | After N-1 BF rounds, check if N-th round improves any distance |
| Undirected | Any negative edge weight → immediate negative cycle |
| All-pairs | After Floyd-Warshall, if `dist[i][i] < 0` for any i |
| Arbitrage | Bellman-Ford on -log(rates) graph |

### SPFA Anti-Hack
SPFA's worst case O(VE) is triggered by adversarial inputs. In competitive programming, Dijkstra is preferred when weights are non-negative. SPFA is used for:
- Negative weights (Dijkstra fails)
- Average-case performance matters
- Minimum cost flow (where Bellman-Ford is needed in successive shortest path)

### Edge Cases

| Scenario | Handling |
|---|---|
| Source unreachable to target | dist[target] remains INF → return -1 |
| Negative cycle on shortest path | Return -∞ or flag as invalid |
| Self-loop with negative weight | Negative cycle → detect in N-th round |
| Zero-weight edges | Bellman-Ford handles normally |
| Disconnected graph | INF distances to unreachable nodes |

---

*Next: [06_Union_Find_DSU_Advanced.md](06_Union_Find_DSU_Advanced.md) — DSU with rollback, weighted DSU, virtual nodes, offline algorithms*

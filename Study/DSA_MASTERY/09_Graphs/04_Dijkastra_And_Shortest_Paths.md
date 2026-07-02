# Dijkstra and Shortest Paths — FAANG Mastery Guide

> **Audience**: Engineers who know BFS — master Dijkstra variants, state-space search, and modified algorithms.  
> **Goal**: Every Dijkstra variant asked at FAANG, with correctness proofs and full Python implementations.

---

## Table of Contents
1. [Dijkstra's Algorithm — Full Implementation & Correctness Proof](#1-dijkstras-algorithm)
2. [Why Greedy Works: Non-Negative Weight Invariant](#2-correctness-proof)
3. [Time Complexity — O((V+E) log V) Derivation](#3-time-complexity)
4. [Dijkstra on State Graphs (Not Just Nodes)](#4-dijkstra-on-states)
5. [Key Dijkstra Variants](#5-key-variants)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. Dijkstra's Algorithm — Full Implementation

```python
import heapq
from typing import List, Dict, Tuple
from collections import defaultdict

def dijkstra(graph: Dict[int, List[Tuple[int, int]]], src: int, n: int) -> List[int]:
    """
    Dijkstra's single-source shortest path.
    graph[u] = list of (neighbor, weight). All weights >= 0.
    Returns dist array where dist[i] = shortest distance from src to i.
    
    Time:  O((V+E) log V)
    Space: O(V+E)
    """
    INF  = float('inf')
    dist = [INF] * n
    dist[src] = 0

    # Min-heap: (distance, node)
    heap = [(0, src)]

    while heap:
        d, u = heapq.heappop(heap)

        # Lazy deletion: skip if we've already found a shorter path
        if d > dist[u]:
            continue

        for v, w in graph[u]:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                heapq.heappush(heap, (dist[v], v))

    return dist


def dijkstra_with_path(graph: Dict[int, List[Tuple[int, int]]], src: int,
                        dst: int, n: int) -> Tuple[int, List[int]]:
    """
    Returns (shortest distance, path from src to dst).
    Uses predecessor array to reconstruct path.
    """
    INF  = float('inf')
    dist = [INF] * n
    prev = [-1] * n
    dist[src] = 0
    heap = [(0, src)]

    while heap:
        d, u = heapq.heappop(heap)
        if d > dist[u]:
            continue
        if u == dst:
            break
        for v, w in graph[u]:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                prev[v] = u
                heapq.heappush(heap, (dist[v], v))

    # Reconstruct path
    path = []
    node = dst
    while node != -1:
        path.append(node)
        node = prev[node]
    path.reverse()

    return dist[dst], path if path[0] == src else []
```

---

## 2. Correctness Proof — Why Greedy Works

**Theorem**: When Dijkstra pops node u from the heap, `dist[u]` is the true shortest distance from source to u.

**Proof by contradiction** (relies on non-negative weights):

Assume node u is popped with `dist[u] = d`, but the true shortest distance is `d* < d`. Then there exists a shorter path `S → ... → p → q → ... → u` where:
- `p` is the last already-finalized node on this path.
- `q` is the first unfinalized node.

Since `q` is on a shorter path to u:
- `dist[q] ≤ d* + w(q→...→u) ≤ d*`
- But all weights are **non-negative**, so `dist[q] ≤ dist[u]`
- But `dist[q] ≤ d* < d = dist[u]` → q would have been popped **before** u (heap pops minimum first).
- Contradiction: q should have been processed and its neighbors relaxed, giving a better dist[u].

**Critical dependency**: Non-negative weights ensure that once a node is popped, no future relaxation can improve its distance. With negative weights, this fails — use Bellman-Ford.

---

## 3. Time Complexity — O((V+E) log V) Derivation

With a **binary min-heap**:

| Operation | Cost | How Many Times |
|---|---|---|
| `heappush` | O(log V) | At most E times (one per edge relaxation) |
| `heappop` | O(log V) | At most E times (lazy deletion) |
| **Total** | **O(E log V)** | (assuming E ≥ V for connected graph) |

More precisely: **O((V+E) log V)** because initialization is O(V) and we process E edges.

**With Fibonacci heap**: O(E + V log V) — optimal but rarely implemented in interviews.

**Dense graphs** (E ≈ V²): O(V² log V) with binary heap vs O(V²) with simple array-based Dijkstra. For dense graphs, linear scan is actually better:

```python
def dijkstra_dense(adj_matrix: List[List[int]], src: int) -> List[int]:
    """
    Dijkstra with O(V²) simple array scan — optimal for dense graphs.
    adj_matrix[u][v] = weight (or INF if no edge).
    """
    n = len(adj_matrix)
    INF = float('inf')
    dist      = [INF] * n
    finalized = [False] * n
    dist[src] = 0

    for _ in range(n):
        # Find unfinalized node with minimum distance — O(V)
        u = min((i for i in range(n) if not finalized[i]), key=lambda x: dist[x])
        finalized[u] = True

        for v in range(n):
            if not finalized[v] and adj_matrix[u][v] < INF:
                dist[v] = min(dist[v], dist[u] + adj_matrix[u][v])

    return dist
```
**Time**: O(V²) | **Space**: O(V)

---

## 4. Dijkstra on State Graphs

When the problem has **constraints** (fuel, keys, stops, etc.), the "node" in Dijkstra becomes a **state** = (position, constraint_value). This is the most powerful Dijkstra pattern.

```python
def dijkstra_state(graph, src, constraints, n, max_constraint):
    """
    State = (node, constraint_remaining).
    dist[node][constraint] = min cost to reach node with constraint remaining.
    """
    INF  = float('inf')
    dist = [[INF] * (max_constraint+1) for _ in range(n)]
    dist[src][max_constraint] = 0

    heap = [(0, src, max_constraint)]  # (cost, node, constraint)

    while heap:
        cost, u, c = heapq.heappop(heap)
        if cost > dist[u][c]:
            continue
        for v, w, c_cost in graph[u]:     # c_cost = constraint consumed by this edge
            nc = c - c_cost
            if nc >= 0 and cost + w < dist[v][nc]:
                dist[v][nc] = cost + w
                heapq.heappush(heap, (dist[v][nc], v, nc))

    return min(dist[n-1])  # Best cost to reach destination under any constraint level
```

---

## 5. Key Dijkstra Variants

### Variant 1: Maximum Probability Path (LC 1514)

```python
def max_probability(n: int, edges: List[List[int]], succProb: List[float],
                    start_node: int, end_node: int) -> float:
    """
    Maximize probability product along path.
    Transform: maximization with products → negate log probabilities → minimization with sums.
    OR: Use max-heap directly (negate probabilities).
    """
    adj = defaultdict(list)
    for i, (u, v) in enumerate(edges):
        adj[u].append((v, succProb[i]))
        adj[v].append((u, succProb[i]))

    prob = [0.0] * n
    prob[start_node] = 1.0
    # Max-heap: negate probability
    heap = [(-1.0, start_node)]

    while heap:
        p, u = heapq.heappop(heap)
        p = -p
        if p < prob[u]:
            continue
        if u == end_node:
            return prob[u]
        for v, edge_prob in adj[u]:
            new_prob = prob[u] * edge_prob
            if new_prob > prob[v]:
                prob[v] = new_prob
                heapq.heappush(heap, (-new_prob, v))

    return 0.0
```
**Time**: O((V+E) log V) | **Space**: O(V+E)

### Variant 2: Path with Minimum Effort (LC 1631)

```python
def minimum_effort_path(heights: List[List[int]]) -> int:
    """
    Effort = max |height difference| along path.
    Minimize the maximum absolute difference.
    
    Modified Dijkstra: dist[r][c] = min effort to reach (r,c).
    Relaxation: effort = max(current_effort, |heights[r][c] - heights[nr][nc]|)
    """
    rows, cols = len(heights), len(heights[0])
    INF  = float('inf')
    dist = [[INF]*cols for _ in range(rows)]
    dist[0][0] = 0
    heap = [(0, 0, 0)]  # (effort, row, col)

    directions = [(0,1),(0,-1),(1,0),(-1,0)]

    while heap:
        effort, r, c = heapq.heappop(heap)
        if r == rows-1 and c == cols-1:
            return effort
        if effort > dist[r][c]:
            continue
        for dr, dc in directions:
            nr, nc = r+dr, c+dc
            if 0 <= nr < rows and 0 <= nc < cols:
                new_effort = max(effort, abs(heights[r][c] - heights[nr][nc]))
                if new_effort < dist[nr][nc]:
                    dist[nr][nc] = new_effort
                    heapq.heappush(heap, (new_effort, nr, nc))

    return dist[rows-1][cols-1]
```
**Time**: O(rows × cols × log(rows × cols)) | **Space**: O(rows × cols)

---

## 6. Problem Set

---

### Problem 1: Network Delay Time (LC 743)

```python
def network_delay_time(times: List[List[int]], n: int, k: int) -> int:
    """
    Signal sent from k. After delay time, all nodes receive it.
    Return min time for ALL nodes to receive, or -1 if impossible.
    = Single-source shortest path from k, then max of all distances.
    """
    adj = defaultdict(list)
    for u, v, w in times:
        adj[u].append((v, w))

    dist = {k: 0}
    heap = [(0, k)]

    while heap:
        d, u = heapq.heappop(heap)
        if d > dist.get(u, float('inf')):
            continue
        for v, w in adj[u]:
            if d + w < dist.get(v, float('inf')):
                dist[v] = d + w
                heapq.heappush(heap, (dist[v], v))

    if len(dist) < n:
        return -1
    return max(dist.values())
```
**Time**: O((V+E) log V) | **Space**: O(V+E)

---

### Problem 2: Find the City with Smallest Number of Neighbors (LC 1334)

```python
def find_the_city(n: int, edges: List[List[int]], distanceThreshold: int) -> int:
    """
    Find city i such that count of cities reachable within distanceThreshold is minimized.
    Ties broken by largest city index.
    
    Run Dijkstra from EVERY node (or use Floyd-Warshall).
    """
    adj = defaultdict(list)
    for u, v, w in edges:
        adj[u].append((v, w))
        adj[v].append((u, w))

    def dijkstra_from(src):
        dist = [float('inf')] * n
        dist[src] = 0
        heap = [(0, src)]
        while heap:
            d, u = heapq.heappop(heap)
            if d > dist[u]:
                continue
            for v, w in adj[u]:
                if dist[u] + w < dist[v]:
                    dist[v] = dist[u] + w
                    heapq.heappush(heap, (dist[v], v))
        return dist

    best_city  = -1
    best_count = n + 1

    for city in range(n):
        dist = dijkstra_from(city)
        count = sum(1 for j in range(n) if j != city and dist[j] <= distanceThreshold)
        # Tie-breaking: largest city index wins (so >= for equal counts)
        if count <= best_count:
            best_count = count
            best_city  = city

    return best_city
```
**Time**: O(V × (V+E) log V) | **Space**: O(V+E)

---

### Problem 3: Swim in Rising Water (LC 778)

```python
def swim_in_water(grid: List[List[int]]) -> int:
    """
    grid[r][c] = elevation. Can move to neighbor if current time t >= max elevation on path.
    Return min t to reach bottom-right from top-left.
    
    Modified Dijkstra: minimize the MAXIMUM elevation on path.
    dist[r][c] = min(max elevation) to reach (r,c).
    """
    n = len(grid)
    dist = [[float('inf')]*n for _ in range(n)]
    dist[0][0] = grid[0][0]
    heap = [(grid[0][0], 0, 0)]

    while heap:
        t, r, c = heapq.heappop(heap)
        if r == n-1 and c == n-1:
            return t
        if t > dist[r][c]:
            continue
        for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
            nr, nc = r+dr, c+dc
            if 0 <= nr < n and 0 <= nc < n:
                new_t = max(t, grid[nr][nc])
                if new_t < dist[nr][nc]:
                    dist[nr][nc] = new_t
                    heapq.heappush(heap, (new_t, nr, nc))

    return dist[n-1][n-1]
```
**Time**: O(n² log n) | **Space**: O(n²)

---

### Problem 4: Cheapest Flights Within K Stops (LC 787) — Modified Dijkstra vs Bellman-Ford

```python
def find_cheapest_price_dijkstra(n: int, flights: List[List[int]],
                                  src: int, dst: int, k: int) -> int:
    """
    At most K stops = at most K+1 edges.
    State = (cost, node, stops_remaining).
    
    WARNING: Standard Dijkstra doesn't work directly because a path with fewer
    stops might have higher cost but enable cheaper paths later.
    The state must include stops_remaining.
    """
    adj = defaultdict(list)
    for u, v, price in flights:
        adj[u].append((v, price))

    # dist[node][stops] = min cost to reach node with stops_remaining stops left
    INF  = float('inf')
    dist = [[INF] * (k+2) for _ in range(n)]
    dist[src][k+1] = 0

    heap = [(0, src, k+1)]  # (cost, node, stops_remaining)

    while heap:
        cost, u, stops = heapq.heappop(heap)
        if u == dst:
            return cost
        if stops == 0 or cost > dist[u][stops]:
            continue
        for v, price in adj[u]:
            new_cost = cost + price
            if new_cost < dist[v][stops-1]:
                dist[v][stops-1] = new_cost
                heapq.heappush(heap, (new_cost, v, stops-1))

    return -1
```
**Time**: O(E × K × log(V × K)) | **Space**: O(V × K)

```python
def find_cheapest_price_bellman(n: int, flights: List[List[int]],
                                 src: int, dst: int, k: int) -> int:
    """
    Bellman-Ford approach: exactly k+1 relaxation rounds.
    Each round processes all edges exactly once — models "one more hop."
    Must use PREVIOUS round's distances (copy!) to prevent using more than k+1 edges.
    """
    INF  = float('inf')
    dist = [INF] * n
    dist[src] = 0

    for _ in range(k+1):
        temp = dist[:]   # CRUCIAL: snapshot of current distances
        for u, v, price in flights:
            if dist[u] + price < temp[v]:
                temp[v] = dist[u] + price
        dist = temp

    return -1 if dist[dst] == INF else dist[dst]
```
**Time**: O(K × E) | **Space**: O(V)

**Why Bellman-Ford is cleaner here**: The "at most K stops" constraint maps perfectly to Bellman-Ford's "at most K+1 iterations." Dijkstra needs state augmentation to handle this.

---

### Problem 5: Keys and Rooms (LC 841) — Reachability via DFS/BFS

```python
def can_visit_all_rooms(rooms: List[List[int]]) -> bool:
    """
    Room 0 is unlocked. Keys in each room unlock other rooms.
    Can we visit all rooms?
    Standard DFS/BFS reachability on implicit graph.
    """
    from collections import deque
    visited = {0}
    queue   = deque([0])

    while queue:
        room = queue.popleft()
        for key in rooms[room]:
            if key not in visited:
                visited.add(key)
                queue.append(key)

    return len(visited) == len(rooms)
```

---

### Problem 6: Dijkstra with Keys — State = (node, keys_bitmask)

```python
def shortest_path_all_keys(grid: List[str]) -> int:
    """
    LC 864: Find shortest path collecting all keys.
    Keys: a-f (lowercase). Locks: A-F (uppercase). Must have key to pass lock.
    
    State = (row, col, keys_collected_bitmask).
    This is Dijkstra (BFS here since uniform cost) on the state space.
    """
    from collections import deque
    rows, cols = len(grid), len(grid[0])
    
    start_r = start_c = 0
    all_keys = 0
    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == '@':
                start_r, start_c = r, c
            elif grid[r][c].islower():
                all_keys |= (1 << (ord(grid[r][c]) - ord('a')))

    # BFS since all moves cost 1 (BFS = Dijkstra for unit weights)
    visited = set()
    queue = deque([(start_r, start_c, 0, 0)])  # (r, c, keys, steps)
    visited.add((start_r, start_c, 0))

    while queue:
        r, c, keys, steps = queue.popleft()

        for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
            nr, nc = r+dr, c+dc
            if not (0 <= nr < rows and 0 <= nc < cols):
                continue
            cell = grid[nr][nc]
            if cell == '#':
                continue
            # Lock: must have key
            if cell.isupper() and not (keys & (1 << (ord(cell.lower()) - ord('a')))):
                continue

            new_keys = keys
            if cell.islower():
                new_keys |= (1 << (ord(cell) - ord('a')))

            if new_keys == all_keys:
                return steps + 1

            state = (nr, nc, new_keys)
            if state not in visited:
                visited.add(state)
                queue.append((nr, nc, new_keys, steps+1))

    return -1
```
**Time**: O(rows × cols × 2^K) where K = number of keys (≤ 6)  
**Space**: O(rows × cols × 2^K)

---

### Problem 7: K-th Shortest Path (Yen's Algorithm Concept)

```python
def k_shortest_paths(graph: dict, src: int, dst: int, k: int) -> List[int]:
    """
    Find k shortest path distances from src to dst.
    Yen's algorithm: augment Dijkstra to allow revisiting nodes.
    Each node can be popped from heap up to k times.
    
    Time: O(k × E × log(V))
    """
    count = defaultdict(int)   # How many times each node has been finalized
    heap  = [(0, src)]

    results = []

    while heap and len(results) < k:
        d, u = heapq.heappop(heap)
        count[u] += 1

        if count[u] > k:       # Don't process a node more than k times
            continue
        if u == dst:
            results.append(d)

        for v, w in graph.get(u, []):
            if count[v] < k:   # Still worth exploring
                heapq.heappush(heap, (d + w, v))

    return results
```
**Time**: O(k × E × log(k × V)) | **Space**: O(k × V)

---

## 7. Interview Tips and Edge Cases

### The Lazy Deletion Idiom
```python
if d > dist[u]:
    continue
```
This is essential for correctness with Python's `heapq` (no decrease-key). Without it, stale entries cause incorrect results. Always include it.

### State-Space Dijkstra Template
When you see: "shortest path with constraint X (fuel, stops, keys, time budget)"
→ State = `(position, X_remaining)`, distance table is 2D: `dist[position][X_remaining]`

### When NOT to Use Dijkstra

| Situation | Use Instead |
|---|---|
| Negative edge weights | Bellman-Ford |
| Negative cycles | Bellman-Ford (detect) |
| All-pairs shortest path | Floyd-Warshall |
| Unweighted graph | BFS (O(V+E) vs O((V+E) log V)) |
| 0-1 weights only | 0-1 BFS with deque |
| DAG (any weights) | Topo sort DP (O(V+E)) |

### Edge Cases

| Scenario | Fix |
|---|---|
| Source == Destination | Return 0 immediately |
| Disconnected graph | Some distances remain INF |
| Self-loops | Dijkstra naturally handles (non-negative = no improvement) |
| Parallel edges | Take minimum weight among parallel edges, or process all |
| Very large graphs | Use bidirectional Dijkstra (halves search space) |

### Bidirectional Dijkstra (Advanced)
Run Dijkstra simultaneously from source and target. When a node is settled by **both** searches, candidate answer = dist_forward[u] + dist_backward[u]. The true answer = minimum over all such candidates.

```python
def bidirectional_dijkstra(graph, rgraph, src, dst, n):
    """graph = forward adj, rgraph = reverse adj."""
    INF = float('inf')
    df  = [INF]*n; df[src] = 0
    db  = [INF]*n; db[dst] = 0
    hf  = [(0, src)]
    hb  = [(0, dst)]
    settled_f = set()
    settled_b = set()
    ans = INF

    while hf or hb:
        if hf:
            d, u = heapq.heappop(hf)
            if u not in settled_f:
                settled_f.add(u)
                if u in settled_b:
                    ans = min(ans, df[u] + db[u])
                for v, w in graph.get(u, []):
                    if df[u]+w < df[v]:
                        df[v] = df[u]+w
                        heapq.heappush(hf, (df[v], v))
        # Mirror for backward search
        if hb:
            d, u = heapq.heappop(hb)
            if u not in settled_b:
                settled_b.add(u)
                if u in settled_f:
                    ans = min(ans, df[u] + db[u])
                for v, w in rgraph.get(u, []):
                    if db[u]+w < db[v]:
                        db[v] = db[u]+w
                        heapq.heappush(hb, (db[v], v))
    return ans
```

### Complexity Summary

| Algorithm | Time | Space | Use Case |
|---|---|---|---|
| Dijkstra (heap) | O((V+E) log V) | O(V+E) | Sparse graphs |
| Dijkstra (array) | O(V²) | O(V) | Dense graphs |
| Dijkstra (state) | O(S log S) where S=state space | O(S) | Constrained paths |
| Bidirectional | O(b^(d/2) log b) | O(b^(d/2)) | Long paths |
| K-th shortest | O(kE log V) | O(kV) | Multiple paths |

---

*Next: [05_Bellman_Ford_And_Floyd_Warshall.md](05_Bellman_Ford_And_Floyd_Warshall.md) — Negative weights, all-pairs shortest paths, arbitrage detection*

# Minimum Spanning Tree — FAANG Mastery Guide

> **Audience**: Engineers who know basic graphs — master Kruskal's, Prim's, cut property proofs, MST applications.  
> **Goal**: Every MST algorithm and variant asked at FAANG, with correctness proofs and complete Python implementations.

---

## Table of Contents
1. [Cut Property — Why MST Algorithms Are Correct](#1-cut-property)
2. [Kruskal's Algorithm — O(E log E)](#2-kruskals-algorithm)
3. [Prim's Algorithm — O(E log V)](#3-prims-algorithm)
4. [Kruskal's vs Prim's — When to Use Which](#4-comparison)
5. [MST Variants](#5-mst-variants)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. Cut Property — Why MST Algorithms Are Correct

### Definition

A **cut** (S, V\S) partitions graph vertices into two non-empty sets. An edge **crosses** the cut if one endpoint is in S and the other in V\S.

### Cut Property (Fundamental MST Theorem)

**Theorem**: Let (S, V\S) be any cut of graph G. If edge e is the **minimum-weight edge** crossing this cut, then e is in **every MST** of G.

**Proof by contradiction**:  
Suppose MST T doesn't contain e = (u, v, w_e) where u∈S, v∈V\S.
- T is a spanning tree, so T has a unique path from u to v. This path must cross the cut (S, V\S) at some edge e' = (u', v', w').
- Since e is the minimum edge crossing the cut: `w_e ≤ w`.
- If `w_e < w`: Remove e' from T, add e → new spanning tree T' with lower total weight → contradicts T being MST.
- If `w_e = w`: T' is also an MST (equal weight), and it contains e → MST exists containing e. □

**How algorithms use this**:
- **Kruskal's**: Processes edges in sorted order. Each edge considered crosses a cut between two currently disconnected components. If it's the lightest such edge (it is — we sorted), it belongs in MST.
- **Prim's**: The cut is always (visited, unvisited). The lightest crossing edge is added greedily.

### Cycle Property (Complementary)

**Theorem**: For any cycle C, the maximum-weight edge in C is **never in any MST** (unless there are ties).

---

## 2. Kruskal's Algorithm

```python
from typing import List, Tuple

class DSU:
    def __init__(self, n):
        self.parent = list(range(n))
        self.rank   = [0] * n
        self.size   = [1] * n

    def find(self, x):
        if self.parent[x] != x:
            self.parent[x] = self.find(self.parent[x])
        return self.parent[x]

    def union(self, x, y) -> bool:
        rx, ry = self.find(x), self.find(y)
        if rx == ry:
            return False
        if self.rank[rx] < self.rank[ry]:
            rx, ry = ry, rx
        self.parent[ry] = rx
        self.size[rx] += self.size[ry]
        if self.rank[rx] == self.rank[ry]:
            self.rank[rx] += 1
        return True


def kruskals_mst(n: int, edges: List[Tuple[int, int, int]]) -> Tuple[int, List[Tuple]]:
    """
    Kruskal's MST algorithm.
    edges: list of (weight, u, v).
    Returns (total_MST_weight, MST_edges).
    
    Time:  O(E log E) — dominated by sorting
    Space: O(V + E)
    """
    edges_sorted = sorted(edges)   # Sort by weight O(E log E)
    dsu = DSU(n)
    mst_weight = 0
    mst_edges  = []

    for w, u, v in edges_sorted:
        if dsu.union(u, v):   # Different components → safe to add (cut property)
            mst_weight += w
            mst_edges.append((w, u, v))
            if len(mst_edges) == n - 1:   # MST complete
                break

    # If MST has < n-1 edges → disconnected graph (spanning FOREST)
    return mst_weight, mst_edges
```

**Kruskal's Correctness via Cut Property**:  
When we process edge e = (u, v, w) in sorted order and u, v are in different components:
- Let S = component containing u, V\S = rest of graph.
- e is the minimum edge crossing (S, V\S) because all lighter edges have already been processed (sorted order), and if they connected u's component to v's component, they'd have already merged them → contradiction.
- By cut property, e must be in the MST. ✓

---

## 3. Prim's Algorithm

```python
import heapq
from collections import defaultdict

def prims_mst(n: int, edges: List[Tuple[int, int, int]]) -> Tuple[int, List[Tuple]]:
    """
    Prim's MST using min-heap (lazy variant).
    edges: list of (u, v, weight).
    Start from node 0.
    
    Time:  O(E log V) with binary heap
    Space: O(V + E)
    """
    adj = defaultdict(list)
    for u, v, w in edges:
        adj[u].append((w, v))
        adj[v].append((w, u))

    visited    = [False] * n
    mst_weight = 0
    mst_edges  = []
    # Heap: (weight, from_node, to_node)
    heap = [(0, -1, 0)]   # Start at node 0 with 0-weight virtual edge

    while heap and len(mst_edges) < n - 1:
        w, u, v = heapq.heappop(heap)
        if visited[v]:
            continue   # Lazy deletion: already in MST
        visited[v] = True

        if u != -1:   # Skip the virtual starting edge
            mst_weight += w
            mst_edges.append((w, u, v))

        for nw, nb in adj[v]:
            if not visited[nb]:
                heapq.heappush(heap, (nw, v, nb))

    return mst_weight, mst_edges


def prims_mst_dense(n: int, adj_matrix: List[List[int]]) -> int:
    """
    Prim's with O(V²) array scan — optimal for dense graphs (E ≈ V²).
    adj_matrix[u][v] = weight (INF if no edge).
    """
    INF = float('inf')
    key       = [INF] * n    # key[v] = min edge weight connecting v to MST
    in_mst    = [False] * n
    key[0]    = 0
    total_w   = 0

    for _ in range(n):
        # Find minimum key vertex not yet in MST — O(V)
        u = min((i for i in range(n) if not in_mst[i]), key=lambda x: key[x])
        in_mst[u] = True
        total_w += key[u]

        for v in range(n):
            if not in_mst[v] and adj_matrix[u][v] < key[v]:
                key[v] = adj_matrix[u][v]

    return total_w
```
**Heap Prim's**: O(E log V) | O(V+E)  
**Array Prim's**: O(V²) | O(V)

---

## 4. Kruskal's vs Prim's

| Criterion | Kruskal's | Prim's (Heap) | Prim's (Array) |
|---|---|---|---|
| Time | O(E log E) | O(E log V) | O(V²) |
| Space | O(V+E) | O(V+E) | O(V) |
| Best for | Sparse graphs, offline edge list | Sparse graphs, adj list | Dense graphs |
| Handles disconnected? | Yes (forest) | Need outer loop | Need outer loop |
| Implementation | DSU + sort | Heap + adj list | Simple loop |
| Negative weights | Yes | Yes | Yes |

**Rule of thumb**:
- E ≪ V² (sparse): use Kruskal's or heap Prim's
- E ≈ V² (dense): use array Prim's O(V²) — better constant than O(E log V) = O(V² log V)

---

## 5. MST Variants

### Minimum Spanning Forest (Disconnected Graph)

```python
def minimum_spanning_forest(n: int, edges: List[Tuple[int, int, int]]) -> int:
    """MST for disconnected graph = MSF. Same Kruskal's — just accept < n-1 edges."""
    dsu = DSU(n)
    total = 0
    for w, u, v in sorted(edges):
        if dsu.union(u, v):
            total += w
    return total
```

### Maximum Spanning Tree

```python
def maximum_spanning_tree(n: int, edges: List[Tuple[int, int, int]]) -> int:
    """Sort edges in DESCENDING order — same algorithm gives maximum ST."""
    dsu = DSU(n)
    total = 0
    for w, u, v in sorted(edges, reverse=True):
        if dsu.union(u, v):
            total += w
    return total
```

### Second Minimum Spanning Tree

```python
def second_mst(n: int, edges: List[Tuple[int, int, int]]) -> int:
    """
    Find MST weight, then for each non-MST edge (u,v,w):
    - The second MST replaces the MAX edge on the MST path from u to v with (u,v,w).
    - Second MST weight = MST weight - max_path_edge + w.
    - Return minimum over all such swaps.
    
    Requires: LCA + path maximum queries on MST tree.
    Time: O(E log V)
    """
    # Step 1: Build MST
    edges_sorted = sorted(edges)
    dsu = DSU(n)
    mst_edges = []
    mst_w = 0
    for w, u, v in edges_sorted:
        if dsu.union(u, v):
            mst_edges.append((w, u, v))
            mst_w += w

    # Step 2: Build adjacency list for MST
    adj = defaultdict(list)
    for w, u, v in mst_edges:
        adj[u].append((v, w))
        adj[v].append((u, w))

    # Step 3: DFS to find max edge on path between any two nodes
    # (Full LCA implementation omitted for brevity — use sparse table for O(log n) per query)
    def max_edge_on_path(src, dst) -> int:
        """BFS/DFS to find max edge weight on tree path from src to dst."""
        visited = {src: (None, 0)}
        stack   = [src]
        while stack:
            u = stack.pop()
            if u == dst:
                # Trace back path
                max_w = 0
                cur   = dst
                while visited[cur][0] is not None:
                    max_w = max(max_w, visited[cur][1])
                    cur   = visited[cur][0]
                return max_w
            for v, w in adj[u]:
                if v not in visited:
                    visited[v] = (u, w)
                    stack.append(v)
        return float('inf')

    # Step 4: Try replacing each non-MST edge
    mst_edge_set = {(u,v) for _, u, v in mst_edges} | {(v,u) for _, u, v in mst_edges}
    second = float('inf')

    for w, u, v in edges:
        if (u, v) not in mst_edge_set:
            path_max = max_edge_on_path(u, v)
            second = min(second, mst_w - path_max + w)

    return second
```

### Borůvka's Algorithm (Parallel MST)

```python
def boruvkas_mst(n: int, edges: List[Tuple[int, int, int]]) -> int:
    """
    Borůvka's: O(E log V). Naturally parallelizable.
    Each round: every component finds its cheapest outgoing edge. Add all. Merge.
    Number of rounds: O(log V) (each round at least halves components).
    
    Advantage: Can be parallelized — each component independently finds its cheapest edge.
    Used in MapReduce-style distributed MST algorithms.
    """
    dsu = DSU(n)
    mst_weight = 0
    num_components = n

    while num_components > 1:
        # cheapest[comp] = (min_weight, u, v) for cheapest edge leaving comp
        cheapest = {}

        for w, u, v in edges:
            ru, rv = dsu.find(u), dsu.find(v)
            if ru == rv:
                continue
            if ru not in cheapest or w < cheapest[ru][0]:
                cheapest[ru] = (w, u, v)
            if rv not in cheapest or w < cheapest[rv][0]:
                cheapest[rv] = (w, u, v)

        # Add cheapest edges
        for comp, (w, u, v) in cheapest.items():
            if dsu.union(u, v):
                mst_weight += w
                num_components -= 1

    return mst_weight
```
**Time**: O(E log V) | **Space**: O(V+E)

---

## 6. Problem Set

---

### Problem 1: Minimum Cost to Connect All Points (LC 1584)

```python
def min_cost_connect_points(points: List[List[int]]) -> int:
    """
    Points in 2D plane. Cost = Manhattan distance.
    Find MST of complete graph.
    
    Prim's O(N²) is better than Kruskal's O(N² log N) here (dense graph).
    """
    n = len(points)
    INF = float('inf')
    in_mst = [False] * n
    key    = [INF] * n
    key[0] = 0
    total  = 0

    def dist(i, j):
        return abs(points[i][0]-points[j][0]) + abs(points[i][1]-points[j][1])

    for _ in range(n):
        # Find unvisited node with min key
        u = min((i for i in range(n) if not in_mst[i]), key=lambda x: key[x])
        in_mst[u] = True
        total += key[u]

        for v in range(n):
            if not in_mst[v]:
                d = dist(u, v)
                if d < key[v]:
                    key[v] = d

    return total
```
**Time**: O(N²) | **Space**: O(N)

---

### Problem 2: Optimize Water Distribution in a Village (LC 1168)

```python
def min_cost_to_supply_water(n: int, wells: List[int],
                              pipes: List[List[int]]) -> int:
    """
    Either dig a well at village i (cost wells[i-1]) or lay pipe (cost given).
    
    Key insight: Add virtual node 0. Edge from 0 to village i has cost wells[i-1]
    (represents "digging a well at i" = "connecting i to water source 0").
    Find MST of this augmented graph.
    """
    # Build edge list with virtual node 0
    all_edges = []
    for i, cost in enumerate(wells, 1):
        all_edges.append((cost, 0, i))   # Well = edge from virtual source to village

    for house1, house2, cost in pipes:
        all_edges.append((cost, house1, house2))

    # Kruskal's on augmented graph with n+1 nodes (0..n)
    dsu = DSU(n + 1)
    total = 0

    for w, u, v in sorted(all_edges):
        if dsu.union(u, v):
            total += w

    return total
```
**Time**: O((N+E) log(N+E)) | **Space**: O(N+E)

---

### Problem 3: Minimum Spanning Tree for Clustering (K Clusters)

```python
def k_clusters_mst(n: int, edges: List[Tuple[int, int, int]], k: int) -> int:
    """
    Find k clusters maximizing minimum inter-cluster distance.
    Equivalent to: build MST, remove k-1 most expensive edges.
    The minimum inter-cluster distance = weight of the k-th most expensive MST edge.
    
    This is the core idea behind single-linkage clustering.
    """
    dsu = DSU(n)
    mst_edges = []

    for w, u, v in sorted(edges):
        if dsu.union(u, v):
            mst_edges.append(w)
            if len(mst_edges) == n - 1:
                break

    # MST has n-1 edges. Remove k-1 most expensive → k clusters.
    # The minimum inter-cluster edge = (k-1)-th largest MST edge weight from end.
    if len(mst_edges) < n - 1:
        return -1   # Graph not connected

    mst_edges.sort()
    # After removing k-1 largest edges, minimum gap = smallest of the (k-1) removed edges
    return mst_edges[n - k]
```
**Time**: O(E log E) | **Space**: O(V+E)

---

### Problem 4: Critical Connections in a Network — MST Context (LC 1192)

A bridge in the graph is always in the MST. Revisiting from MST perspective:

```python
def critical_connections_mst(n: int, connections: List[List[int]]) -> List[List[int]]:
    """
    Every bridge is a 'critical connection.'
    From MST perspective: a bridge is an edge that, if removed, disconnects the graph.
    Use Tarjan's bridge finding (DFS-based).
    
    All bridges appear in EVERY spanning tree.
    Non-bridge edges appear in SOME but not all spanning trees.
    """
    adj  = [[] for _ in range(n)]
    for u, v in connections:
        adj[u].append(v)
        adj[v].append(u)

    disc = [-1]*n; low = [-1]*n; timer = [0]; result = []

    def dfs(u, par):
        disc[u] = low[u] = timer[0]; timer[0] += 1
        for v in adj[u]:
            if disc[v] == -1:
                dfs(v, u)
                low[u] = min(low[u], low[v])
                if low[v] > disc[u]: result.append([u, v])
            elif v != par:
                low[u] = min(low[u], disc[v])

    dfs(0, -1)
    return result
```

---

### Problem 5: Swim in Rising Water — MST/Dijkstra Connection (LC 778)

```python
def swim_in_water_mst(grid: List[List[int]]) -> int:
    """
    Equivalent to: find the path from (0,0) to (n-1,n-1) that minimizes
    the maximum elevation along the path.
    
    MST interpretation: build edges between adjacent cells sorted by max elevation.
    This is equivalent to finding the path where the bottleneck (max edge weight) is minimized.
    
    Kruskal's-like approach: add edges in weight order until (0,0) and (n-1,n-1) connect.
    """
    n = len(grid)

    # Build edges: for each pair of adjacent cells, edge weight = max(grid[r1][c1], grid[r2][c2])
    edges = []
    for r in range(n):
        for c in range(n):
            if r+1 < n:
                edges.append((max(grid[r][c], grid[r+1][c]), r*n+c, (r+1)*n+c))
            if c+1 < n:
                edges.append((max(grid[r][c], grid[r][c+1]), r*n+c, r*n+c+1))

    dsu = DSU(n * n)
    for w, u, v in sorted(edges):
        dsu.union(u, v)
        if dsu.connected(0, n*n-1):
            return w

    return grid[0][0]  # When n==1
```
**Time**: O(n² log n) | **Space**: O(n²)

---

### Problem 6: Approximate TSP Using MST

```python
def approx_tsp_mst(n: int, dist_matrix: List[List[float]]) -> float:
    """
    2-approximation for Metric TSP using MST.
    
    Algorithm:
    1. Build MST (cost = OPT_MST ≤ OPT_TSP, since TSP minus one edge is a spanning tree)
    2. Double all MST edges (now every node has even degree)
    3. Find Euler circuit on doubled graph
    4. Shortcut repeated vertices → Hamiltonian cycle
    
    Result cost ≤ 2 × OPT_MST ≤ 2 × OPT_TSP (2-approximation guarantee).
    
    For Christofides: 1.5-approximation (uses min-weight perfect matching of odd-degree nodes).
    """
    # Step 1: MST with array Prim's
    INF = float('inf')
    in_mst = [False]*n; key = [INF]*n; parent = [-1]*n; key[0] = 0
    for _ in range(n):
        u = min((i for i in range(n) if not in_mst[i]), key=lambda x: key[x])
        in_mst[u] = True
        for v in range(n):
            if not in_mst[v] and dist_matrix[u][v] < key[v]:
                key[v] = dist_matrix[u][v]; parent[v] = u

    # Step 2: DFS pre-order on MST = Hamiltonian path (shortcutted Euler tour)
    adj = defaultdict(list)
    for v in range(1, n):
        adj[parent[v]].append(v)
        adj[v].append(parent[v])

    visited = [False]*n; tour = []
    stack = [0]
    while stack:
        u = stack.pop()
        if not visited[u]:
            visited[u] = True; tour.append(u)
            for v in adj[u]:
                if not visited[v]:
                    stack.append(v)

    # Step 3: Compute tour cost
    cost = sum(dist_matrix[tour[i]][tour[i+1]] for i in range(n-1))
    cost += dist_matrix[tour[-1]][tour[0]]
    return cost
```

---

## 7. Interview Tips and Edge Cases

### MST Uniqueness
MST is **unique** when all edge weights are distinct. When edges have equal weights, multiple MSTs can exist (same total weight, different edge sets).

**Proof of uniqueness with distinct weights**: If two MSTs T1, T2 differ, consider the minimum-weight edge e in T1\T2. Adding e to T2 creates a cycle. The maximum edge on this cycle must be > e (since e is minimum in T1\T2 and all weights distinct). But removing that edge from T2 gives a tree with lower weight — contradicts T2 being MST.

### Edge Cases

| Scenario           | Handling                                               |
| --------------------| --------------------------------------------------------|
| Disconnected graph | Kruskal returns MSF; Prim needs outer loop             |
| Single node        | MST weight = 0, no edges                               |
| Parallel edges     | Include in edge list; Kruskal naturally picks lightest |
| Self-loops         | DSU.find(u)==find(v) for (u,u) → skipped               |
| All same weight    | Any spanning tree is an MST                            |
| Negative weights   | MST algorithms still work (Kruskal, Prim)              |

### Interview: MST vs Shortest Path

| Property | MST | Shortest Path |
|---|---|---|
| Goal | Minimize total tree weight | Minimize path length |
| Uses all nodes? | Yes | No (just src to dst) |
| Tree structure? | Yes (n-1 edges) | No |
| Negative weights? | Fine | Needs Bellman-Ford |
| Directional? | Undirected only | Directed or undirected |

### The Bottleneck Spanning Tree = MST

For problems asking "minimize the maximum edge weight on a path from s to t" (minimax path):
- The answer is the maximum edge weight on the path from s to t **in the MST**.
- Equivalently, find the MST: the minimax path uses only MST edges.

---

*Next: [08_Strongly_Connected_Components.md](08_Strongly_Connected_Components.md) — Kosaraju's, Tarjan's SCC, 2-SAT, condensation DAG*

# Network Flow — FAANG Mastery Guide

> **Audience**: Engineers who know graphs and matching — master Ford-Fulkerson, Edmonds-Karp, Dinic's algorithm, max-flow min-cut, and all flow applications.  
> **Goal**: Complete mastery of flow algorithms asked at FAANG, including Dinic's full implementation, proofs, and 5+ hard problems.

---

## Table of Contents
1. [Flow Networks — Definitions and Properties](#1-flow-networks)
2. [Ford-Fulkerson — Augmenting Paths Concept](#2-ford-fulkerson)
3. [Edmonds-Karp — BFS Augmenting Paths O(VE²)](#3-edmonds-karp)
4. [Max-Flow Min-Cut Theorem — Proof](#4-max-flow-min-cut)
5. [Dinic's Algorithm — O(V²E), O(E√V) for Unit Graphs](#5-dinics-algorithm)
6. [Applications of Network Flow](#6-applications)
7. [Problem Set with Full Solutions](#7-problem-set)
8. [Interview Tips and Edge Cases](#8-interview-tips)

---

## 1. Flow Networks — Definitions and Properties

A **flow network** is a directed graph G = (V, E) with:
- A **source** s with in-degree 0.
- A **sink** t with out-degree 0.
- Each edge (u,v) has **capacity** c(u,v) ≥ 0.
- Each edge carries a **flow** f(u,v).

**Flow constraints**:
1. **Capacity**: 0 ≤ f(u,v) ≤ c(u,v) for all (u,v).
2. **Conservation**: For all v ≠ s,t: Σ_u f(u,v) = Σ_w f(v,w) (flow in = flow out).
3. **Antisymmetry**: f(u,v) = -f(v,u) (flow on reverse edge is negative).

**Residual graph**: Given flow f, the residual capacity `r(u,v) = c(u,v) - f(u,v)`. The residual graph contains all edges with r > 0, including **reverse edges** (which "cancel" existing flow).

**Why reverse edges matter**: Sending flow on a reverse edge (v,u) is equivalent to "undoing" some flow previously sent on (u,v). This allows the algorithm to "reroute" flow to find better paths.

---

## 2. Ford-Fulkerson — Augmenting Paths

```python
from collections import defaultdict
from typing import List

class MaxFlow:
    """
    Ford-Fulkerson with DFS augmenting paths.
    Time: O(E × max_flow) — can be very slow for large capacities (non-integer or irrational?)
    Space: O(V+E)
    
    NOT RECOMMENDED in practice: use Edmonds-Karp or Dinic's.
    Presented for conceptual clarity.
    """
    def __init__(self, n: int):
        self.n   = n
        self.cap = defaultdict(int)   # Residual capacities

    def add_edge(self, u: int, v: int, cap: int):
        self.cap[u, v] += cap   # Forward edge
        # Reverse edge with 0 capacity (if not exists, self.cap[(v,u)] = 0 by default)

    def dfs(self, u: int, t: int, pushed: int, visited: set) -> int:
        if u == t:
            return pushed
        visited.add(u)
        for v in range(self.n):
            if v not in visited and self.cap[u, v] > 0:
                result = self.dfs(v, t, min(pushed, self.cap[u, v]), visited)
                if result > 0:
                    self.cap[u, v] -= result
                    self.cap[v, u] += result
                    return result
        return 0

    def max_flow(self, s: int, t: int) -> int:
        flow = 0
        while True:
            pushed = self.dfs(s, t, float('inf'), set())
            if pushed == 0:
                break
            flow += pushed
        return flow
```
**Time**: O(E × F) where F = max flow value (terrible for large F)  
**Space**: O(V+E)

**Why Ford-Fulkerson can be slow**: With DFS, it may choose augmenting paths of capacity 1 even when larger paths exist. For a graph with max flow F, this needs F iterations. With irrational capacities, it may not even terminate.

---

## 3. Edmonds-Karp — O(VE²) Maximum Flow

Edmonds-Karp fixes Ford-Fulkerson by always choosing the **shortest augmenting path** (BFS).

**Why BFS guarantees O(VE²)**:
- Each BFS augmentation ensures the shortest path length is non-decreasing.
- The shortest path length can increase at most V times (0, 2, 4, ..., V-2 hops).
- Between each length increase, at most E augmentations occur (each saturates at least one edge).
- Total augmentations ≤ V × E / 2 = O(VE).
- Each augmentation costs O(E) for BFS.
- Total: O(VE²).

```python
from collections import deque

class EdmondsKarp:
    """
    Edmonds-Karp maximum flow using adjacency list with edge objects.
    
    Time:  O(VE²)
    Space: O(V+E)
    """
    def __init__(self, n: int):
        self.n   = n
        self.adj = [[] for _ in range(n)]   # adj[u] = list of edge indices

        # Edge storage: [to, rev_idx, cap]
        # Edge i: (to=edges[i][0], reverse_edge_idx=edges[i][1], capacity=edges[i][2])
        self.edges = []

    def add_edge(self, u: int, v: int, cap: int):
        """Add directed edge u→v with given capacity."""
        self.adj[u].append(len(self.edges))
        self.edges.append([v, len(self.edges)+1, cap])    # Forward edge
        self.adj[v].append(len(self.edges))
        self.edges.append([u, len(self.edges)-1, 0])      # Reverse edge (cap=0)

    def bfs(self, s: int, t: int) -> List[int]:
        """
        BFS to find shortest augmenting path.
        Returns parent edge index array (prev_edge[v] = edge used to reach v).
        """
        prev_edge = [-1] * self.n
        visited   = [False] * self.n
        visited[s] = True
        queue = deque([s])

        while queue:
            u = queue.popleft()
            for eid in self.adj[u]:
                v, _, cap = self.edges[eid]
                if not visited[v] and cap > 0:
                    visited[v] = True
                    prev_edge[v] = eid
                    if v == t:
                        return prev_edge
                    queue.append(v)
        return None

    def max_flow(self, s: int, t: int) -> int:
        flow = 0
        while True:
            prev_edge = self.bfs(s, t)
            if prev_edge is None:
                break

            # Find bottleneck capacity along the path
            path_flow = float('inf')
            v = t
            while v != s:
                eid = prev_edge[v]
                path_flow = min(path_flow, self.edges[eid][2])
                v = self.edges[self.edges[eid][1]][0]   # Go to u via reverse edge

            # Update capacities along the path
            v = t
            while v != s:
                eid = prev_edge[v]
                self.edges[eid][2]                      -= path_flow   # Forward: reduce
                self.edges[self.edges[eid][1]][2]       += path_flow   # Reverse: increase
                v = self.edges[self.edges[eid][1]][0]

            flow += path_flow

        return flow
```
**Time**: O(VE²) | **Space**: O(V+E)

---

## 4. Max-Flow Min-Cut Theorem

### Definitions

**Cut (S, T)**: A partition of V into S (containing s) and T (containing t).  
**Capacity of cut**: c(S,T) = Σ_{u∈S, v∈T, (u,v)∈E} c(u,v) (only forward edges).  
**Min cut**: Cut with minimum capacity.

### Max-Flow Min-Cut Theorem

**Theorem**: The maximum flow from s to t equals the minimum cut capacity separating s from t.

**Proof** (three equivalent conditions):

Let f be a flow in network G. The following are equivalent:
1. f is a maximum flow.
2. There is no augmenting path in the residual graph G_f.
3. There exists a cut (S, T) with c(S, T) = |f| (flow value = cut capacity).

**(1 → 2)**: If an augmenting path exists, send more flow → f is not maximum. Contrapositive: maximum flow → no augmenting path.

**(2 → 3)**: If no augmenting path exists, let S = nodes reachable from s in G_f. Then T = V\S contains t. For every edge (u,v) with u∈S, v∈T: residual capacity r(u,v) = 0 → f(u,v) = c(u,v) (saturated). For every edge (u,v) with u∈T, v∈S: f(u,v) = 0 (no reverse flow). So |f| = c(S,T) (all cross-cut edges saturated, no back-flow).

**(3 → 1)**: |f| = c(S,T) ≥ min cut ≥ |f| (since any flow ≤ any cut). So |f| = min cut = max flow. ✓

### Finding the Min Cut

```python
def find_min_cut(ek: EdmondsKarp, s: int, t: int) -> tuple:
    """
    After running max_flow, find the min cut edges.
    S = nodes reachable from s in residual graph.
    Min cut edges = edges from S to T with full capacity used.
    """
    ek.max_flow(s, t)

    # BFS on residual graph to find S
    visited = [False] * ek.n
    visited[s] = True
    queue = deque([s])
    while queue:
        u = queue.popleft()
        for eid in ek.adj[u]:
            v, _, cap = ek.edges[eid]
            if not visited[v] and cap > 0:
                visited[v] = True
                queue.append(v)

    S = {i for i in range(ek.n) if visited[i]}
    T = {i for i in range(ek.n) if not visited[i]}

    # Min cut edges: original forward edges from S to T with 0 residual capacity
    cut_edges = []
    for u in S:
        for eid in ek.adj[u]:
            v, rev_eid, cap = ek.edges[eid]
            if v in T:
                # Original capacity = current cap + flow used = cap + reverse edge capacity
                orig_cap = cap + ek.edges[rev_eid][2]
                if orig_cap > 0 and cap == 0:   # Saturated forward edge
                    cut_edges.append((u, v, orig_cap))

    return S, T, cut_edges
```

---

## 5. Dinic's Algorithm — O(V²E), O(E√V) for Unit Graphs

### Algorithm

Dinic's improvement over Edmonds-Karp: instead of one BFS augmentation per iteration, find a **blocking flow** in the **level graph** (BFS layered graph). This reduces the number of phases to O(V).

**Phase**:
1. **BFS**: Build level graph — only edges (u,v) where `level[v] = level[u] + 1`.
2. **DFS**: Find blocking flow in level graph (saturates at least one edge per path).
3. Add blocking flow to total. Repeat until t is unreachable from s.

```python
from collections import deque

class Dinic:
    """
    Dinic's maximum flow algorithm.
    
    Time:  O(V²E) general graphs.
           O(E√V) for unit-capacity graphs (bipartite matching special case).
           O(V^(2/3) × E) for unit-capacity general graphs.
    Space: O(V+E)
    """
    def __init__(self, n: int):
        self.n     = n
        self.graph = [[] for _ in range(n)]   # Adjacency list of edge indices
        self.edges = []                         # Edge list: [to, rev_idx, cap]

    def add_edge(self, u: int, v: int, cap: int):
        self.graph[u].append(len(self.edges))
        self.edges.append([v, len(self.edges)+1, cap])   # Forward
        self.graph[v].append(len(self.edges))
        self.edges.append([u, len(self.edges)-1, 0])     # Reverse

    def bfs_level(self, s: int, t: int) -> bool:
        """BFS to compute level[] for level graph. Returns True if t is reachable."""
        self.level = [-1] * self.n
        self.level[s] = 0
        queue = deque([s])

        while queue:
            u = queue.popleft()
            for eid in self.graph[u]:
                v, _, cap = self.edges[eid]
                if cap > 0 and self.level[v] == -1:
                    self.level[v] = self.level[u] + 1
                    queue.append(v)

        return self.level[t] != -1

    def dfs_blocking(self, u: int, t: int, pushed: int) -> int:
        """
        DFS to find blocking flow in level graph.
        Uses 'iter' array to avoid re-scanning dead edges (optimization).
        """
        if u == t:
            return pushed
        while self.iter[u] < len(self.graph[u]):
            eid = self.graph[u][self.iter[u]]
            v, _, cap = self.edges[eid]
            if cap > 0 and self.level[v] == self.level[u] + 1:
                result = self.dfs_blocking(v, t, min(pushed, cap))
                if result > 0:
                    self.edges[eid][2]                       -= result
                    self.edges[self.edges[eid][1]][2]        += result
                    return result
            self.iter[u] += 1   # Move past dead edge
        return 0

    def max_flow(self, s: int, t: int) -> int:
        flow = 0
        while self.bfs_level(s, t):
            self.iter = [0] * self.n   # Reset edge iterator for each phase
            while True:
                f = self.dfs_blocking(s, t, float('inf'))
                if f == 0:
                    break
                flow += f
        return flow
```
**Time**: O(V²E) general, O(E√V) unit graphs | **Space**: O(V+E)

### Why O(V²E)?

- **O(V) BFS phases**: Each phase, the shortest s-t path length increases by at least 1. Maximum path length is V-1 → at most V phases.
- **O(VE) per phase**: Blocking flow in level graph takes O(VE) (each DFS either augments or moves the iterator past a dead edge; dead-edge moves are bounded by E; augmenting paths are bounded by V per edge).
- **Total**: O(V) phases × O(VE) per phase = O(V²E).

### Why O(E√V) for Unit Graphs?

In unit-capacity graphs, each augmenting path uses exactly 1 unit of flow. After O(√V) phases, the max flow found so far is at least `max_flow - √V × (remaining augmentations)`. This bounds total phases at O(√V). Each phase: O(E). Total: O(E√V).

### The `iter[]` Optimization (Dead-Edge Pruning)

Without `iter[]`: DFS re-scans edges already found dead (capacity 0 in level graph). This makes DFS O(VE) instead of the needed O(VE) total per phase.

With `iter[u]`: When DFS finds edge u→v has no further augmenting capacity in the level graph, increment `iter[u]` to skip it permanently for this phase. Each edge is visited at most twice per phase (once when useful, once when skipped). This gives O(E) total edge visits per phase.

---

## 6. Applications of Network Flow

### Application 1: Maximum Bipartite Matching as Flow

```python
def bipartite_matching_flow(left: int, right: int,
                             edges: List[List[int]]) -> int:
    """
    Maximum bipartite matching modeled as max flow.
    
    Construction:
    - Super source s, super sink t.
    - s → each left node: capacity 1.
    - Each left node → connected right nodes: capacity 1.
    - Each right node → t: capacity 1.
    
    Max flow = max matching.
    """
    n = left + right + 2
    s = left + right       # Super source
    t = left + right + 1   # Super sink

    dinic = Dinic(n)

    # Source to left nodes
    for u in range(left):
        dinic.add_edge(s, u, 1)

    # Left to right edges
    for u, v in edges:
        dinic.add_edge(u, left + v, 1)

    # Right nodes to sink
    for v in range(right):
        dinic.add_edge(left + v, t, 1)

    return dinic.max_flow(s, t)
```

### Application 2: Project Selection (Closure Problem)

```python
def project_selection(projects: List[int], machines: List[int],
                       dependencies: List[List[int]]) -> int:
    """
    projects[i] = profit of project i (positive).
    machines[j] = cost of machine j (positive — cost to buy).
    dependencies: project i requires machine j.
    
    Choose subset of projects to maximize: Σ profits - Σ machine costs.
    
    Reduction to Min-Cut:
    - Source s, Sink t.
    - s → project i: capacity = profit[i].
    - machine j → t: capacity = cost[j].
    - project i → machine j (for dependency): capacity = infinity.
    
    Max profit = Σ all profits - Min Cut.
    (Cutting s→i means "skip project i"; cutting j→t means "don't buy machine j" — but if project i selected and depends on j, the infinite edge forces cutting the project.)
    """
    n_proj = len(projects)
    n_mach = len(machines)
    total_nodes = n_proj + n_mach + 2
    s = n_proj + n_mach
    t = n_proj + n_mach + 1

    dinic = Dinic(total_nodes)
    total_profit = sum(p for p in projects if p > 0)

    # Source → profitable projects
    for i, profit in enumerate(projects):
        if profit > 0:
            dinic.add_edge(s, i, profit)

    # Machines → sink
    for j, cost in enumerate(machines):
        dinic.add_edge(n_proj + j, t, cost)

    # Dependencies: infinite capacity (can't separate project from its machine)
    INF = float('inf')
    for i, j in dependencies:
        dinic.add_edge(i, n_proj + j, INF)

    min_cut = dinic.max_flow(s, t)
    return total_profit - min_cut
```

---

## 7. Problem Set

---

### Problem 1: Maximum Flow in a Network (Dinic's)

```python
def max_flow_network(n: int, edges: List[List[int]], s: int, t: int) -> int:
    """
    Standard max flow. edges: list of [u, v, capacity].
    Returns maximum flow from s to t.
    """
    dinic = Dinic(n)
    for u, v, cap in edges:
        dinic.add_edge(u, v, cap)
    return dinic.max_flow(s, t)
```

---

### Problem 2: Escape Problem — Can All People Escape?

```python
def escape_problem(grid: List[List[str]]) -> bool:
    """
    People 'P' must escape to exits 'E'. Walls '#'. Each cell can be used by at most one person.
    Can all people escape simultaneously?
    
    Model as flow:
    - Split each walkable cell into cell_in and cell_out with capacity 1 (prevents sharing).
    - Source s → each 'P' cell: capacity 1.
    - Each 'E' cell → super sink t: capacity 1.
    - Adjacent walkable cells: cell_out(u) → cell_in(v): capacity 1.
    
    Max flow ≥ number of people → all can escape.
    """
    rows, cols = len(grid), len(grid[0])
    people = 0
    
    def cell_in(r, c):  return (r*cols+c) * 2
    def cell_out(r, c): return (r*cols+c) * 2 + 1

    total_nodes = rows*cols*2 + 2
    s = total_nodes - 2
    t = total_nodes - 1
    dinic = Dinic(total_nodes)

    for r in range(rows):
        for c in range(cols):
            if grid[r][c] != '#':
                # Split: in → out with capacity 1
                dinic.add_edge(cell_in(r,c), cell_out(r,c), 1)

                if grid[r][c] == 'P':
                    people += 1
                    dinic.add_edge(s, cell_in(r,c), 1)
                elif grid[r][c] == 'E':
                    dinic.add_edge(cell_out(r,c), t, 1)

                for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
                    nr, nc = r+dr, c+dc
                    if 0<=nr<rows and 0<=nc<cols and grid[nr][nc] != '#':
                        dinic.add_edge(cell_out(r,c), cell_in(nr,nc), 1)

    return dinic.max_flow(s, t) >= people
```
**Time**: O(V²E) | **Space**: O(V+E)

---

### Problem 3: Minimum Path Cover in DAG via Flow

```python
def min_path_cover_flow(n: int, edges: List[List[int]]) -> int:
    """
    Minimum number of vertex-disjoint paths to cover all nodes.
    = n - Maximum Bipartite Matching (see File 09).
    
    Flow implementation:
    - Source s, Sink t.
    - s → u_out for each node u: cap 1.
    - v_in → t for each node v: cap 1.
    - u_out → v_in for each edge (u,v) in DAG: cap 1.
    
    Max flow = max matching. Min path cover = n - max flow.
    """
    # Node u → u_out = u, u_in = u + n
    total = 2*n + 2
    s = 2*n; t = 2*n+1
    dinic = Dinic(total)

    for u in range(n):
        dinic.add_edge(s, u, 1)          # Source → u_out
        dinic.add_edge(u+n, t, 1)        # v_in → sink

    for u, v in edges:
        dinic.add_edge(u, v+n, 1)        # u_out → v_in

    return n - dinic.max_flow(s, t)
```
**Time**: O(V × E + E√V) for unit-capacity bipartite flow | **Space**: O(V+E)

---

### Problem 4: Critical Edges in Flow Network

```python
def find_critical_flow_edges(dinic: Dinic, s: int, t: int, n: int) -> List[tuple]:
    """
    Find edges that, if removed, decrease the maximum flow.
    = Forward edges in the min cut that are fully saturated.
    
    After computing max flow:
    1. Find S = nodes reachable from s in residual graph.
    2. Critical edges = original forward edges from S to V\S with 0 residual capacity.
    """
    dinic.max_flow(s, t)

    # BFS on residual
    visited = [False] * n
    visited[s] = True
    queue = deque([s])
    while queue:
        u = queue.popleft()
        for eid in dinic.graph[u]:
            v, _, cap = dinic.edges[eid]
            if not visited[v] and cap > 0:
                visited[v] = True
                queue.append(v)

    critical = []
    for u in range(n):
        if not visited[u]:
            continue
        for eid in dinic.graph[u]:
            v, rev_eid, cap = dinic.edges[eid]
            if not visited[v]:
                orig_cap = cap + dinic.edges[rev_eid][2]  # Recover original capacity
                if orig_cap > 0:
                    critical.append((u, v, orig_cap))

    return critical
```

---

### Problem 5: Maximum Disjoint Paths (Edge-Disjoint)

```python
def max_edge_disjoint_paths(n: int, edges: List[List[int]],
                             s: int, t: int) -> int:
    """
    Maximum number of edge-disjoint paths from s to t.
    (Each edge used at most once across all paths.)
    
    By Menger's theorem: = min edge cut = max flow with unit capacities.
    
    Simply set all edge capacities to 1 and run max flow.
    """
    dinic = Dinic(n)
    for u, v in edges:
        dinic.add_edge(u, v, 1)
        dinic.add_edge(v, u, 1)   # Undirected
    return dinic.max_flow(s, t)
```

For **vertex-disjoint** paths: split each internal node v into v_in and v_out with capacity 1. Then max flow = maximum number of vertex-disjoint paths.

---

### Problem 6: Circulation with Lower Bounds

```python
def feasible_circulation(n: int, edges: List[List[int]],
                          lower: List[int], upper: List[int]) -> bool:
    """
    Find if a feasible circulation exists with lower[i] ≤ flow[i] ≤ upper[i].
    
    Reduction to standard max flow:
    1. For each edge (u,v) with lower bound l:
       - Force l units of flow on (u,v).
       - Adjust capacities: new capacity = upper[i] - lower[i].
       - Add forced flow effects to a super source/sink.
    2. Check if max flow from super source to super sink equals sum of lower bounds.
    """
    # excess[v] = net forced flow into v
    excess = [0] * n
    s = n; t = n+1   # Super source and sink
    dinic = Dinic(n + 2)
    total_lower = 0

    for i, (u, v) in enumerate(edges):
        l, c = lower[i], upper[i]
        dinic.add_edge(u, v, c - l)   # Reduced capacity edge
        excess[v] += l
        excess[u] -= l
        total_lower += l

    for v in range(n):
        if excess[v] > 0:
            dinic.add_edge(s, v, excess[v])   # Super source → node with excess
        elif excess[v] < 0:
            dinic.add_edge(v, t, -excess[v])  # Node with deficit → super sink

    return dinic.max_flow(s, t) == total_lower
```

---

## 8. Interview Tips and Edge Cases

### Algorithm Selection for Flow Problems

| Graph Type | Best Algorithm | Time |
|---|---|---|
| General sparse | Dinic's | O(V²E) |
| Unit-capacity (bipartite matching) | Dinic's | O(E√V) |
| General dense | Dinic's | O(V²E) |
| Simple (conceptual) | Edmonds-Karp | O(VE²) |
| Very small | Ford-Fulkerson | O(EF) |

### The Edge Representation Trick

Always represent edges in pairs: edge i is the forward edge, edge i^1 (or edge i XOR 1) is the reverse. This makes `update_reverse` O(1):

```python
# Add forward edge at index 2k, reverse at 2k+1
# To get reverse of edge at index i: use i^1
def add_edge(u, v, cap):
    edges.append([v, cap])     # index 2k: forward
    edges.append([u, 0])       # index 2k+1: reverse
    adj[u].append(len(edges)-2)
    adj[v].append(len(edges)-1)

# Update: edges[eid][1] -= flow; edges[eid^1][1] += flow
```

### Modeling Tips

| Problem | Flow Model |
|---|---|
| Maximum bipartite matching | s→L, L→R, R→t, all cap 1 |
| Min vertex cover (bipartite) | König's: = max matching |
| Max independent set (bipartite) | = n - max matching |
| Min path cover (DAG) | = n - max matching via flow |
| Project selection / closure | s→profitable, costly→t, ∞ deps |
| Node capacity constraint | Split node: v_in →(cap)→ v_out |
| Disjoint paths (edge) | All caps = 1 |
| Disjoint paths (vertex) | Split nodes, internal cap = 1 |
| Circulation with lower bounds | Super source/sink reduction |

### Common Mistakes

| Mistake                                        | Fix                                                 |
| ------------------------------------------------| -----------------------------------------------------|
| Forgetting reverse edges                       | Always add both forward + reverse (cap=0)           |
| Not resetting `iter[]` each Dinic phase        | `iter = [0]*n` inside `while bfs()` loop            |
| Using edge index instead of edge^1 for reverse | Use paired edge representation                      |
| Infinite capacity for "must use" edges         | Use a large constant (e.g., 10^9), not Python `inf` |
| Not handling disconnected graphs               | BFS correctly returns None if t unreachable         |
| Directed vs undirected                         | Undirected: add both (u,v,cap) and (v,u,cap)        |

### Max-Flow Min-Cut Applications Cheat Sheet

| Problem | Min Cut Interpretation |
|---|---|
| Network reliability | Min edges to disconnect s from t |
| Image segmentation | Min cost to separate foreground/background pixels |
| Project selection | Max profit = total profit - min cut |
| Isolation | Min resources to "block" all s→t paths |
| Transportation | Bottleneck capacity on network |

### Complexity Summary

| Algorithm | Time | Space | Notes |
|---|---|---|---|
| Ford-Fulkerson (DFS) | O(EF) | O(V+E) | Bad for large F |
| Edmonds-Karp (BFS) | O(VE²) | O(V+E) | Good reference |
| Dinic's | O(V²E) | O(V+E) | Industry standard |
| Dinic's (unit cap) | O(E√V) | O(V+E) | = Hopcroft-Karp |
| Push-Relabel | O(V²√E) | O(V+E) | Fastest in practice |

### The Dinic's + Bipartite Matching Equivalence

Dinic's algorithm on unit-capacity bipartite graphs is equivalent to Hopcroft-Karp:
- Each BFS phase in Dinic's = one BFS phase in Hopcroft-Karp (building level graph).
- Each DFS blocking flow in Dinic's = finding all vertex-disjoint augmenting paths in Hopcroft-Karp.
- Both achieve O(E√V).

This unification shows that network flow algorithms are generalizations of matching algorithms.

---

## Summary: The Complete Graph Algorithms Map

| Category | Algorithms | Key Problem Types |
|---|---|---|
| BFS | Multi-source, 0-1 BFS, Bidirectional | Shortest paths, level traversal |
| DFS | 3-coloring, Tarjan's, Euler | Cycles, bridges, APs |
| Topo Sort | Kahn's, DFS post-order | DAG ordering, DAG DP |
| Shortest Path | Dijkstra, Bellman-Ford, Floyd-Warshall | Weighted paths, neg cycles |
| DSU | Rank+compression, weighted, rollback | Connectivity, online merging |
| MST | Kruskal, Prim, Borůvka | Min spanning structures |
| SCC | Kosaraju, Tarjan, 2-SAT | Directed connectivity |
| Matching | Augmenting, Hopcroft-Karp, König | Assignment, cover |
| Flow | Edmonds-Karp, Dinic's | Max flow, min cut, closure |

---

*Series Complete — Files 01 through 10 cover the full spectrum of advanced graph algorithms for FAANG mastery.*

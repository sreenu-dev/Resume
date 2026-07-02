# Advanced DFS Patterns — FAANG Mastery Guide

> **Audience**: Engineers beyond DFS basics — master Tarjan's algorithms, coloring, timestamps, Eulerian paths.  
> **Goal**: Every advanced DFS pattern tested at FAANG, with proofs and complete Python implementations.

---

## Table of Contents
1. [DFS Coloring — White/Gray/Black](#1-dfs-coloring)
2. [DFS Timestamps — Discovery & Finish Times](#2-dfs-timestamps)
3. [Articulation Points and Bridges — Tarjan's Algorithm](#3-articulation-points-and-bridges)
4. [Low-Link Values — Deep Derivation](#4-low-link-values)
5. [DFS for Connected Components](#5-connected-components)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. DFS Coloring — White/Gray/Black

DFS coloring is the canonical way to detect **cycles in directed graphs**. Each node has three states:

| Color | Meaning |
|-------|---------|
| **White** (0) | Not yet visited |
| **Gray** (1) | Currently being processed (on the DFS stack) |
| **Black** (2) | Fully processed — all descendants explored |

**Cycle Detection Rule**:  
- If during DFS from node u we encounter a **gray** neighbor v → **back edge** → cycle exists.
- A **black** neighbor means we've fully explored that subtree already — no cycle through this edge.

**Why gray means cycle**: Gray = "currently on the recursion stack." Reaching a gray node means we've found a path from that node back to itself — a cycle by definition.

```python
from typing import List

def has_cycle_directed(n: int, edges: List[List[int]]) -> bool:
    """
    Detect cycle in a directed graph using DFS 3-coloring.
    WHITE=0, GRAY=1, BLACK=2
    """
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)

    color = [0] * n  # All white initially

    def dfs(u: int) -> bool:
        color[u] = 1  # Mark GRAY — entering DFS subtree
        for v in adj[u]:
            if color[v] == 1:   # Back edge → cycle
                return True
            if color[v] == 0:   # Tree edge → recurse
                if dfs(v):
                    return True
        color[u] = 2  # Mark BLACK — subtree fully explored
        return False

    for node in range(n):
        if color[node] == 0:
            if dfs(node):
                return True
    return False
```
**Time**: O(V+E)  
**Space**: O(V) for color array + O(V) recursion stack

**Critical Interview Point**: In **undirected** graphs, coloring still works but "gray neighbor" check must exclude the **parent** edge. A node being gray via its parent is not a cycle. Use parent tracking or edge-based visited sets.

---

## 2. DFS Timestamps — Discovery & Finish Times

Every node u gets two timestamps:
- `disc[u]`: when DFS **first visits** u (discovery time)
- `finish[u]`: when DFS **fully completes** u (finish time)

```python
timer = [0]

def dfs_timestamps(u, adj, disc, finish, visited):
    visited[u] = True
    timer[0] += 1
    disc[u] = timer[0]
    for v in adj[u]:
        if not visited[v]:
            dfs_timestamps(v, adj, disc, finish, visited)
    timer[0] += 1
    finish[u] = timer[0]
```

### What Timestamps Tell You

| Relationship | Meaning |
|---|---|
| `disc[u] < disc[v] < finish[v] < finish[u]` | v is a descendant of u (tree edge) |
| `disc[v] < disc[u] < finish[u] < finish[v]` | u is a descendant of v (back edge in undirected) |
| `finish[v] < disc[u]` | v finished before u started (cross/forward edge) |

**Application — Topological Sort**: In a DAG, nodes with **larger finish times** come first in topological order. DFS-based topo sort = reverse of finish time ordering.

**Application — Kosaraju's SCC**: The node with the largest finish time in the original graph is the root of an SCC. (See File 08.)

---

## 3. Articulation Points and Bridges — Tarjan's Algorithm

### Definitions
- **Articulation Point (Cut Vertex)**: Removing it increases the number of connected components.
- **Bridge**: An edge whose removal increases connected components.

### The Low-Link Insight

`low[u]` = minimum discovery time reachable from the subtree rooted at u using **at most one back edge**.

```
low[u] = min(
    disc[u],                         # u itself
    min(disc[v] for back edges u→v), # via back edges
    min(low[v] for tree edges u→v)   # propagated from children
)
```

**Bridge Condition**: Edge (u, v) is a bridge if `low[v] > disc[u]`.  
- This means: from v's subtree, there is **no back edge** reaching u or any ancestor of u. Removing (u, v) disconnects v's subtree.

**Articulation Point Conditions**:
1. **Root of DFS tree** with ≥ 2 children (removing root disconnects its children).
2. **Non-root** u with a child v where `low[v] >= disc[u]`.  
   - `low[v] >= disc[u]` means v's subtree cannot reach above u → u is a cut vertex.

```python
def find_bridges(n: int, edges: List[List[int]]) -> List[List[int]]:
    """
    Tarjan's bridge-finding algorithm.
    Returns list of bridge edges.
    Time: O(V+E), Space: O(V)
    """
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)

    disc   = [-1] * n
    low    = [-1] * n
    timer  = [0]
    bridges = []

    def dfs(u: int, parent: int):
        disc[u] = low[u] = timer[0]
        timer[0] += 1

        for v in adj[u]:
            if disc[v] == -1:       # Tree edge
                dfs(v, u)
                low[u] = min(low[u], low[v])
                if low[v] > disc[u]:   # Bridge condition
                    bridges.append([u, v])
            elif v != parent:       # Back edge (skip parent to avoid false positive)
                low[u] = min(low[u], disc[v])

    for i in range(n):
        if disc[i] == -1:
            dfs(i, -1)

    return bridges
```
**Time**: O(V+E)  
**Space**: O(V)

```python
def find_articulation_points(n: int, edges: List[List[int]]) -> List[int]:
    """
    Tarjan's articulation point algorithm.
    """
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)

    disc   = [-1] * n
    low    = [-1] * n
    timer  = [0]
    is_ap  = [False] * n

    def dfs(u: int, parent: int):
        disc[u] = low[u] = timer[0]
        timer[0] += 1
        children = 0

        for v in adj[u]:
            if disc[v] == -1:
                children += 1
                dfs(v, u)
                low[u] = min(low[u], low[v])
                # Non-root AP condition
                if parent != -1 and low[v] >= disc[u]:
                    is_ap[u] = True
            elif v != parent:
                low[u] = min(low[u], disc[v])

        # Root AP condition
        if parent == -1 and children >= 2:
            is_ap[u] = True

    for i in range(n):
        if disc[i] == -1:
            dfs(i, -1)

    return [i for i in range(n) if is_ap[i]]
```
**Time**: O(V+E)  
**Space**: O(V)

---

## 4. Low-Link Values — Deep Derivation

**Why `low[v] > disc[u]` means bridge?**

After DFS explores child v of u:
- `low[v]` = earliest disc time reachable from v's subtree.
- If `low[v] > disc[u]`, then v's subtree **cannot reach u or any ancestor of u** without using edge (u,v).
- Therefore, cutting edge (u,v) isolates v's subtree — it IS a bridge.

**If `low[v] <= disc[u]`**:
- v's subtree has a back edge to u or some ancestor of u.
- Even after removing (u,v), v's subtree stays connected to u via that back edge.
- NOT a bridge.

**Multi-edge caveat**: If there are multiple edges between u and v, the "skip parent" trick fails. Fix: track edge index, skip only the specific parent **edge** (not all edges to parent).

```python
def find_bridges_multigraph(n: int, edges: List[List[int]]) -> List[List[int]]:
    """Handles multi-edges (parallel edges) correctly."""
    adj = [[] for _ in range(n)]  # adj[u] = [(v, edge_id)]
    for i, (u, v) in enumerate(edges):
        adj[u].append((v, i))
        adj[v].append((u, i))

    disc   = [-1] * n
    low    = [-1] * n
    timer  = [0]
    bridges = []

    def dfs(u: int, parent_edge: int):
        disc[u] = low[u] = timer[0]
        timer[0] += 1
        for v, eid in adj[u]:
            if eid == parent_edge:     # Skip the exact edge we came from
                continue
            if disc[v] == -1:
                dfs(v, eid)
                low[u] = min(low[u], low[v])
                if low[v] > disc[u]:
                    bridges.append([u, v])
            else:
                low[u] = min(low[u], disc[v])

    for i in range(n):
        if disc[i] == -1:
            dfs(i, -1)
    return bridges
```

---

## 5. Connected Components

```python
def count_components_dfs(n: int, edges: List[List[int]]) -> int:
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)

    visited = [False] * n
    count = 0

    def dfs(u):
        visited[u] = True
        for v in adj[u]:
            if not visited[v]:
                dfs(v)

    for i in range(n):
        if not visited[i]:
            dfs(i)
            count += 1
    return count
```
**Time**: O(V+E) | **Space**: O(V)

---

## 6. Problem Set

---

### Problem 1: Number of Islands — All Variants

```python
def num_islands(grid: List[List[str]]) -> int:
    """LC 200. Standard grid DFS."""
    if not grid:
        return 0
    rows, cols = len(grid), len(grid[0])
    count = 0

    def dfs(r, c):
        if r < 0 or r >= rows or c < 0 or c >= cols or grid[r][c] != '1':
            return
        grid[r][c] = '0'   # Mark visited in-place
        dfs(r+1, c); dfs(r-1, c); dfs(r, c+1); dfs(r, c-1)

    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == '1':
                count += 1
                dfs(r, c)
    return count
```
**Time**: O(rows × cols) | **Space**: O(rows × cols) recursion stack

```python
def num_islands_3d(grid_list: List[List[List[str]]]) -> int:
    """
    3D variant: islands span across multiple 2D grids stacked vertically.
    6-directional connectivity.
    """
    if not grid_list:
        return 0
    D, R, C = len(grid_list), len(grid_list[0]), len(grid_list[0][0])
    visited = set()
    count = 0

    def dfs(d, r, c):
        if (d,r,c) in visited or not (0<=d<D and 0<=r<R and 0<=c<C):
            return
        if grid_list[d][r][c] != '1':
            return
        visited.add((d,r,c))
        for dd,dr,dc in [(1,0,0),(-1,0,0),(0,1,0),(0,-1,0),(0,0,1),(0,0,-1)]:
            dfs(d+dd, r+dr, c+dc)

    for d in range(D):
        for r in range(R):
            for c in range(C):
                if grid_list[d][r][c] == '1' and (d,r,c) not in visited:
                    dfs(d,r,c)
                    count += 1
    return count
```

---

### Problem 2: Clone Graph (LC 133) — DFS with Memoization

```python
from typing import Optional

class Node:
    def __init__(self, val=0, neighbors=None):
        self.val = val
        self.neighbors = neighbors if neighbors is not None else []

def clone_graph(node: Optional[Node]) -> Optional[Node]:
    """Deep clone using DFS + visited dict as clone map."""
    if not node:
        return None

    cloned = {}  # original → clone

    def dfs(n: Node) -> Node:
        if n in cloned:
            return cloned[n]
        clone = Node(n.val)
        cloned[n] = clone          # Register BEFORE recursing (handles cycles)
        for nb in n.neighbors:
            clone.neighbors.append(dfs(nb))
        return clone

    return dfs(node)
```
**Time**: O(V+E) | **Space**: O(V)

**Key Pattern**: Register the clone in the map **before** recursing into neighbors. This handles cycles — if a neighbor leads back to an already-cloned node, we return the existing clone.

---

### Problem 3: Course Schedule II (LC 210) — DFS Topological Sort

```python
def find_order(numCourses: int, prerequisites: List[List[int]]) -> List[int]:
    """
    Returns topological order, or [] if cycle exists.
    Uses DFS post-order (finish time ordering).
    """
    adj = [[] for _ in range(numCourses)]
    for a, b in prerequisites:
        adj[b].append(a)  # b must come before a

    # 0=unvisited, 1=in-stack (gray), 2=done (black)
    state  = [0] * numCourses
    result = []
    cycle  = [False]

    def dfs(u):
        if cycle[0]:
            return
        state[u] = 1
        for v in adj[u]:
            if state[v] == 1:   # Back edge → cycle
                cycle[0] = True
                return
            if state[v] == 0:
                dfs(v)
        state[u] = 2
        result.append(u)        # Post-order: append after all descendants done

    for i in range(numCourses):
        if state[i] == 0:
            dfs(i)

    return [] if cycle[0] else result[::-1]  # Reverse post-order = topo order
```
**Time**: O(V+E) | **Space**: O(V)

---

### Problem 4: Find Eventual Safe States (LC 802)

```python
def eventual_safe_nodes(graph: List[List[int]]) -> List[int]:
    """
    A node is 'safe' if every path from it leads to a terminal node (no outgoing edges).
    Equivalently: a node is UNSAFE if it's on a cycle or leads to a cycle.
    
    Use 3-coloring: gray = on current path, black = safe.
    """
    n = len(graph)
    # 0=unvisited, 1=unsafe (gray/cycle), 2=safe (black)
    state = [0] * n

    def dfs(u: int) -> bool:
        """Returns True if u is safe."""
        if state[u] == 2:
            return True    # Already confirmed safe
        if state[u] == 1:
            return False   # On current path → cycle → unsafe

        state[u] = 1       # Mark as being explored
        for v in graph[u]:
            if not dfs(v):
                return False   # Any unsafe neighbor makes u unsafe

        state[u] = 2       # All paths lead to terminal → safe
        return True

    return [i for i in range(n) if dfs(i)]
```
**Time**: O(V+E) | **Space**: O(V)

---

### Problem 5: Maximum Area of Island (LC 695) — DFS with Return Value

```python
def max_area_of_island(grid: List[List[int]]) -> int:
    rows, cols = len(grid), len(grid[0])

    def dfs(r, c) -> int:
        if r < 0 or r >= rows or c < 0 or c >= cols or grid[r][c] == 0:
            return 0
        grid[r][c] = 0   # Mark visited
        return 1 + dfs(r+1,c) + dfs(r-1,c) + dfs(r,c+1) + dfs(r,c-1)

    return max((dfs(r,c) for r in range(rows) for c in range(cols)
                if grid[r][c] == 1), default=0)
```
**Time**: O(rows × cols) | **Space**: O(rows × cols)

---

### Problem 6: Count Sub-Islands (LC 1905) — DFS Intersection

```python
def count_sub_islands(grid1: List[List[int]], grid2: List[List[int]]) -> int:
    """
    An island in grid2 is a sub-island if ALL its cells are land in grid1.
    
    DFS the entire island in grid2 even if it fails (collect all cells),
    then check if ALL cells of that island are land in grid1.
    """
    rows, cols = len(grid2), len(grid2[0])

    def dfs(r, c) -> bool:
        if r < 0 or r >= rows or c < 0 or c >= cols or grid2[r][c] == 0:
            return True
        grid2[r][c] = 0
        # Check grid1 — but still explore full island in grid2
        is_sub = (grid1[r][c] == 1)
        # Use '&=' not 'and' — must DFS ALL neighbors regardless of is_sub
        is_sub &= dfs(r+1, c)
        is_sub &= dfs(r-1, c)
        is_sub &= dfs(r, c+1)
        is_sub &= dfs(r, c-1)
        return is_sub

    count = 0
    for r in range(rows):
        for c in range(cols):
            if grid2[r][c] == 1:
                if dfs(r, c):
                    count += 1
    return count
```
**Time**: O(rows × cols) | **Space**: O(rows × cols)

**Critical Bug to Avoid**: Using `and` instead of `&=`. With `and`, Python short-circuits — if the first recursive call returns False, the remaining cells are never DFS'd, leaving grid2 in an inconsistent state.

---

### Problem 7: Path with Maximum Gold (LC 1219) — DFS Backtracking

```python
def get_maximum_gold(grid: List[List[int]]) -> int:
    """
    Cannot revisit cells. No zeros. Start/end anywhere.
    Classic DFS backtracking with state restoration.
    """
    rows, cols = len(grid), len(grid[0])
    max_gold = [0]

    def dfs(r, c, current_gold):
        current_gold += grid[r][c]
        max_gold[0] = max(max_gold[0], current_gold)

        temp = grid[r][c]
        grid[r][c] = 0   # Mark visited by zeroing

        for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
            nr, nc = r+dr, c+dc
            if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] != 0:
                dfs(nr, nc, current_gold)

        grid[r][c] = temp   # Restore (backtrack)

    for r in range(rows):
        for c in range(cols):
            if grid[r][c] != 0:
                dfs(r, c, 0)

    return max_gold[0]
```
**Time**: O(rows × cols × 3^(rows×cols)) worst case, but practical due to sparse gold  
**Space**: O(rows × cols)

---

### Problem 8: Reconstruct Itinerary (LC 332) — Eulerian Path via Hierholzer's

```python
from collections import defaultdict

def find_itinerary(tickets: List[List[str]]) -> List[str]:
    """
    Eulerian path: use every edge exactly once.
    Hierholzer's algorithm: DFS + post-order append.
    
    Why post-order? We want to follow dead ends LAST. By appending to result
    after all neighbors are exhausted, we naturally build the path in reverse.
    
    Sort adjacency lists for lexicographic ordering.
    """
    adj = defaultdict(list)
    for src, dst in sorted(tickets, reverse=True):  # Sort reversed for efficient pop
        adj[src].append(dst)

    result = []

    def dfs(airport: str):
        while adj[airport]:
            nxt = adj[airport].pop()   # Take lexicographically smallest (sorted reversed)
            dfs(nxt)
        result.append(airport)         # Post-order: append after exhausting all edges

    dfs("JFK")
    return result[::-1]   # Reverse post-order = correct itinerary
```
**Time**: O(E log E) for sorting | **Space**: O(E)

**Deep Insight**: Why does post-order give Eulerian path?  
- When we reach a dead end (no more outgoing edges), we know this must be the **last** node in the path.
- By appending in post-order and reversing, we correctly place dead-end nodes at the end of the itinerary.

---

### Problem 9: Detect Cycle in Undirected Graph via DFS

```python
def can_finish_undirected(n: int, edges: List[List[int]]) -> bool:
    """
    Detect cycle in undirected graph.
    Parent tracking prevents false positives from parent back-edges.
    """
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)

    visited = [False] * n

    def dfs(u: int, parent: int) -> bool:
        visited[u] = True
        for v in adj[u]:
            if not visited[v]:
                if dfs(v, u):
                    return True
            elif v != parent:   # Visited and not parent → back edge → cycle
                return True
        return False

    for i in range(n):
        if not visited[i]:
            if dfs(i, -1):
                return True
    return False
```
**Time**: O(V+E) | **Space**: O(V)

---

### Problem 10: Critical Connections in a Network (LC 1192) — Tarjan's Bridges

```python
def critical_connections(n: int, connections: List[List[int]]) -> List[List[int]]:
    """
    Directly applies Tarjan's bridge-finding.
    A critical connection = a bridge.
    """
    adj = [[] for _ in range(n)]
    for u, v in connections:
        adj[u].append(v)
        adj[v].append(u)

    disc = [-1] * n
    low  = [-1] * n
    timer = [0]
    result = []

    def dfs(u, parent):
        disc[u] = low[u] = timer[0]
        timer[0] += 1
        for v in adj[u]:
            if disc[v] == -1:
                dfs(v, u)
                low[u] = min(low[u], low[v])
                if low[v] > disc[u]:
                    result.append([u, v])
            elif v != parent:
                low[u] = min(low[u], disc[v])

    dfs(0, -1)
    return result
```
**Time**: O(V+E) | **Space**: O(V)

---

## 7. Interview Tips and Edge Cases

### The DFS Recursion Stack Problem
Python's default recursion limit is 1000. For large graphs, either:
1. Use `sys.setrecursionlimit(10**6)` (risky for memory)
2. Convert recursive DFS to **iterative** using an explicit stack

```python
import sys
from collections import deque

def dfs_iterative(adj, start, n):
    """Iterative DFS using explicit stack. Avoids recursion limit."""
    visited = [False] * n
    stack = [start]
    order = []

    while stack:
        u = stack.pop()
        if visited[u]:
            continue
        visited[u] = True
        order.append(u)
        for v in adj[u]:
            if not visited[v]:
                stack.append(v)
    return order
```

**Post-order iterative DFS** (needed for topo sort, Tarjan's):
```python
def dfs_postorder_iterative(adj, start, n):
    visited = [False] * n
    stack = [(start, False)]   # (node, processed)
    result = []

    while stack:
        u, processed = stack.pop()
        if processed:
            result.append(u)   # Post-order action
            continue
        if visited[u]:
            continue
        visited[u] = True
        stack.append((u, True))   # Re-push to process after children
        for v in adj[u]:
            if not visited[v]:
                stack.append((v, False))
    return result
```

### Common DFS Mistakes at FAANG Interviews

| Mistake | Fix |
|---|---|
| Not handling disconnected graphs | Outer loop over all unvisited nodes |
| Using `and` instead of `&=` for island counting | Always explore full island before short-circuiting |
| Forgetting to restore state in backtracking | Always pair modification with restoration |
| Multi-edge false cycle detection | Track edge ID, not just parent node |
| Recursion limit exceeded | Use iterative DFS or increase limit |
| Gray node vs black node confusion | Gray = on stack (cycle). Black = done (safe). |

### DFS vs BFS Decision Matrix

| Problem Type | Preferred |
|---|---|
| Shortest path (unweighted) | BFS |
| Topological sort | DFS (post-order) or BFS (Kahn's) |
| Cycle detection in directed | DFS (3-coloring) |
| Articulation points / bridges | DFS (Tarjan's) |
| Eulerian path | DFS (Hierholzer's) |
| Backtracking / all paths | DFS |
| Connected components | Either |

### Tarjan's Low-Link Mental Model

Think of `low[u]` as the **highest node** (closest to root) that u's subtree can **"phone home" to** without using the tree edge that brought us to u.

If v's subtree **cannot phone home** above u (`low[v] > disc[u]`), then the edge (u,v) is a bridge — it's the only line of communication.

---

*Next: [03_Topological_Sort_And_DAGs.md](03_Topological_Sort_And_DAGs.md) — Kahn's algorithm, DAG DP, alien dictionary*

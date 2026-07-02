# Advanced BFS Patterns — FAANG Mastery Guide

> **Audience**: Engineers who already know BFS fundamentals.  
> **Goal**: Master every advanced BFS variant asked at Google, Meta, Amazon, Apple, Netflix.

---

## Table of Contents
1. [Multi-Source BFS — Proof of Correctness](#1-multi-source-bfs)
2. [0-1 BFS with Deque](#2-0-1-bfs)
3. [BFS on Implicit Graphs](#3-bfs-on-implicit-graphs)
4. [Bidirectional BFS — O(b^(d/2)) Proof](#4-bidirectional-bfs)
5. [BFS Cycle Detection in Undirected Graphs](#5-bfs-cycle-detection)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. Multi-Source BFS

### Why Start All Sources Simultaneously?

In single-source BFS, the invariant is: **nodes at distance d are processed before nodes at distance d+1**.  
Multi-source BFS extends this: treat all sources as a single virtual super-source connected to each real source by a zero-weight edge.

**Correctness Proof:**  
- Let `d(v, S)` = minimum distance from any source in S to node v.  
- When we enqueue all sources at level 0, the BFS processes level by level.  
- At any point, if node v is dequeued at level k, then `d(v, S) = k` — because BFS guarantees the **first time** a node is visited is via the shortest path.  
- If we had started single-source BFS from each source and taken the minimum, we'd get the same answer, but at cost O(|S| × (V+E)). Multi-source BFS does it in O(V+E).

**Key Insight**: Sequential single-source BFS from multiple sources is wasteful. The multi-source technique is strictly superior.

```python
from collections import deque
from typing import List

def multi_source_bfs(grid: List[List[int]], sources: List[tuple]) -> List[List[int]]:
    """
    Compute minimum distance from ANY source to every cell.
    sources: list of (row, col) tuples that are sources (distance 0).
    Returns distance grid (-1 if unreachable).
    """
    rows, cols = len(grid), len(grid[0])
    dist = [[-1] * cols for _ in range(rows)]
    queue = deque()

    # Enqueue ALL sources at distance 0 — this is the key insight
    for r, c in sources:
        dist[r][c] = 0
        queue.append((r, c))

    directions = [(0,1),(0,-1),(1,0),(-1,0)]

    while queue:
        r, c = queue.popleft()
        for dr, dc in directions:
            nr, nc = r + dr, c + dc
            if 0 <= nr < rows and 0 <= nc < cols and dist[nr][nc] == -1 and grid[nr][nc] == 0:
                dist[nr][nc] = dist[r][c] + 1
                queue.append((nr, nc))

    return dist
```
**Time**: O(V+E) = O(rows × cols)  
**Space**: O(V) for the queue and distance array

---

## 2. 0-1 BFS

### When to Use: Edge weights are 0 or 1 ONLY

Standard Dijkstra: O((V+E) log V). For 0-1 graphs, use a **deque** instead:
- Weight 0 edge → push to **front** of deque (like it costs nothing)
- Weight 1 edge → push to **back** of deque

**Why this works**: The deque always maintains the property that the front holds the minimum-distance node, identical to Dijkstra's priority queue guarantee — but O(V+E) instead of O((V+E) log V).

**Correctness**: This is a special case of Dial's algorithm (bucket queue with bucket count = max_weight + 1). With max_weight=1, two buckets suffice → deque.

```python
from collections import deque
from typing import List

def zero_one_bfs(graph: dict, source: int, n: int) -> List[int]:
    """
    graph[u] = list of (v, weight) where weight in {0, 1}
    Returns shortest distances from source to all nodes.
    """
    INF = float('inf')
    dist = [INF] * n
    dist[source] = 0
    dq = deque([source])

    while dq:
        u = dq.popleft()
        for v, w in graph[u]:
            if dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
                if w == 0:
                    dq.appendleft(v)   # Zero cost → process immediately
                else:
                    dq.append(v)       # Unit cost → process later

    return dist
```
**Time**: O(V+E)  
**Space**: O(V+E)

### Classic 0-1 BFS Problem: Minimum Cost Path on Grid

```python
def min_cost_path(grid: List[List[int]]) -> int:
    """
    LC 1368: Minimum cost to make at least one valid path in a grid.
    Arrow direction costs 0 to follow, 1 to change.
    """
    rows, cols = len(grid), len(grid[0])
    # direction encoding: 1=right, 2=left, 3=down, 4=up
    dirs = [(0,1),(0,-1),(1,0),(-1,0)]
    
    INF = float('inf')
    dist = [[INF]*cols for _ in range(rows)]
    dist[0][0] = 0
    dq = deque([(0, 0, 0)])  # (cost, row, col)

    while dq:
        cost, r, c = dq.popleft()
        if cost > dist[r][c]:
            continue
        for i, (dr, dc) in enumerate(dirs):
            nr, nc = r+dr, c+dc
            if 0 <= nr < rows and 0 <= nc < cols:
                # grid[r][c]-1 gives index of the "free" direction
                w = 0 if grid[r][c]-1 == i else 1
                if dist[r][c] + w < dist[nr][nc]:
                    dist[nr][nc] = dist[r][c] + w
                    if w == 0:
                        dq.appendleft((dist[nr][nc], nr, nc))
                    else:
                        dq.append((dist[nr][nc], nr, nc))

    return dist[rows-1][cols-1]
```
**Time**: O(rows × cols)  
**Space**: O(rows × cols)

---

## 3. BFS on Implicit Graphs

Implicit graphs are never explicitly constructed — nodes and edges are generated on the fly. The state space **is** the graph.

**Pattern**: Define (1) what a state is, (2) how to generate neighbors, (3) when to stop.

```python
def bfs_implicit(start_state, is_goal, get_neighbors) -> int:
    """Generic BFS template for implicit graphs."""
    from collections import deque
    visited = {start_state}
    queue = deque([(start_state, 0)])

    while queue:
        state, dist = queue.popleft()
        if is_goal(state):
            return dist
        for nxt in get_neighbors(state):
            if nxt not in visited:
                visited.add(nxt)
                queue.append((nxt, dist + 1))
    return -1
```

---

## 4. Bidirectional BFS

### Why O(b^(d/2)) Instead of O(b^d)?

Let b = branching factor, d = depth of solution.

- **Unidirectional BFS** explores ~b^d nodes.
- **Bidirectional BFS** runs from both source and target simultaneously, meeting in the middle.
  - Each side explores ~b^(d/2) nodes.
  - Total: 2 × b^(d/2) ≪ b^d for large d.

**Example**: b=10, d=10 → unidirectional: 10^10. Bidirectional: 2×10^5. **100,000× faster**.

**Correctness subtlety**: When the two frontiers meet, you cannot immediately return — you must check if a shorter path exists through other meeting points at the current level. Continue until both levels have been fully processed.

```python
from collections import deque
from typing import Optional

def bidirectional_bfs(graph: dict, src: int, dst: int) -> int:
    """
    Returns shortest path length from src to dst.
    graph is adjacency list (undirected).
    """
    if src == dst:
        return 0

    # Two frontiers and their visited-distance dicts
    front_visited = {src: 0}
    back_visited  = {dst: 0}
    front_queue   = deque([src])
    back_queue    = deque([dst])

    def expand(queue, visited, other_visited):
        """Expand one level of BFS. Return overlap distance if found."""
        best = float('inf')
        for _ in range(len(queue)):
            node = queue.popleft()
            for nb in graph.get(node, []):
                if nb not in visited:
                    visited[nb] = visited[node] + 1
                    queue.append(nb)
                if nb in other_visited:
                    best = min(best, visited[nb] + other_visited[nb])
        return best

    ans = float('inf')
    while front_queue and back_queue:
        # Always expand the smaller frontier
        if len(front_queue) <= len(back_queue):
            ans = min(ans, expand(front_queue, front_visited, back_visited))
        else:
            ans = min(ans, expand(back_queue, back_visited, front_visited))
        # Early termination: if best answer found ≤ current minimum frontier depth
        if ans < float('inf'):
            return ans

    return -1 if ans == float('inf') else ans
```
**Time**: O(b^(d/2))  
**Space**: O(b^(d/2))

---

## 5. BFS Cycle Detection in Undirected Graph

BFS detects cycles by checking if we encounter a visited node that is **not** the parent of the current node.

```python
from collections import deque

def has_cycle_bfs(adj: dict, n: int) -> bool:
    """
    Detects cycle in undirected graph using BFS.
    adj: adjacency list {node: [neighbors]}
    Handles disconnected graphs.
    """
    visited = [False] * n

    for start in range(n):
        if visited[start]:
            continue
        queue = deque([(start, -1)])  # (node, parent)
        visited[start] = True

        while queue:
            node, parent = queue.popleft()
            for nb in adj.get(node, []):
                if not visited[nb]:
                    visited[nb] = True
                    queue.append((nb, node))
                elif nb != parent:
                    return True  # Cross edge → cycle

    return False
```
**Time**: O(V+E)  
**Space**: O(V)

**Edge Case**: Multi-edges (two edges between same pair). With a single parent tracking, a multi-edge is falsely detected as a cycle. Fix: track the **edge** (not just the parent node), or use degree-based checks.

---

## 6. Problem Set

---

### Problem 1: Word Ladder (LC 127) — BFS + Bidirectional

```python
from collections import defaultdict, deque

def word_ladder(beginWord: str, endWord: str, wordList: list) -> int:
    """
    Minimum number of transformations: beginWord → endWord.
    Each step: change exactly one letter, result must be in wordList.
    """
    word_set = set(wordList)
    if endWord not in word_set:
        return 0

    L = len(beginWord)
    # Precompute generic → list of words (for faster neighbor lookup)
    # e.g., "*ot" → ["hot","dot","lot"]
    combo_dict = defaultdict(list)
    for word in word_set:
        for i in range(L):
            combo_dict[word[:i]+'*'+word[i+1:]].append(word)

    # Bidirectional BFS
    begin_visited = {beginWord: 1}
    end_visited   = {endWord: 1}
    begin_q = deque([beginWord])
    end_q   = deque([endWord])

    def visit_word_node(queue, visited, other_visited):
        word = queue.popleft()
        for i in range(L):
            pattern = word[:i] + '*' + word[i+1:]
            for adj_word in combo_dict[pattern]:
                if adj_word in other_visited:
                    return visited[word] + other_visited[adj_word]
                if adj_word not in visited:
                    visited[adj_word] = visited[word] + 1
                    queue.append(adj_word)
        return None

    while begin_q and end_q:
        # Expand smaller frontier
        if len(begin_q) > len(end_q):
            begin_q, end_q = end_q, begin_q
            begin_visited, end_visited = end_visited, begin_visited

        result = visit_word_node(begin_q, begin_visited, end_visited)
        if result:
            return result

    return 0
```
**Time**: O(M² × N) where M = word length, N = wordList size  
**Space**: O(M² × N) for the pattern dictionary

---

### Problem 2: Rotting Oranges (LC 994) — Multi-Source BFS

```python
def oranges_rotting(grid: List[List[int]]) -> int:
    """
    0=empty, 1=fresh, 2=rotten.
    Each minute, rotten orange infects 4-directional fresh neighbors.
    Return min minutes to rot all, or -1.
    """
    rows, cols = len(grid), len(grid[0])
    fresh = 0
    queue = deque()

    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == 2:
                queue.append((r, c, 0))  # Multi-source: all rotten at time 0
            elif grid[r][c] == 1:
                fresh += 1

    if fresh == 0:
        return 0

    directions = [(0,1),(0,-1),(1,0),(-1,0)]
    max_time = 0

    while queue:
        r, c, time = queue.popleft()
        for dr, dc in directions:
            nr, nc = r+dr, c+dc
            if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] == 1:
                grid[nr][nc] = 2
                fresh -= 1
                max_time = max(max_time, time+1)
                queue.append((nr, nc, time+1))

    return max_time if fresh == 0 else -1
```
**Time**: O(rows × cols)  
**Space**: O(rows × cols)

---

### Problem 3: Walls and Gates (LC 286) — Multi-Source BFS

```python
def walls_and_gates(rooms: List[List[int]]) -> None:
    """
    INF=empty room, -1=wall, 0=gate.
    Fill each empty room with distance to nearest gate. In-place.
    """
    INF = 2147483647
    rows, cols = len(rooms), len(rooms[0])
    queue = deque()

    # Multi-source: start BFS from ALL gates simultaneously
    for r in range(rows):
        for c in range(cols):
            if rooms[r][c] == 0:
                queue.append((r, c))

    directions = [(0,1),(0,-1),(1,0),(-1,0)]
    while queue:
        r, c = queue.popleft()
        for dr, dc in directions:
            nr, nc = r+dr, c+dc
            if 0 <= nr < rows and 0 <= nc < cols and rooms[nr][nc] == INF:
                rooms[nr][nc] = rooms[r][c] + 1
                queue.append((nr, nc))
```
**Time**: O(rows × cols)  
**Space**: O(rows × cols)

---

### Problem 4: Pacific Atlantic Water Flow (LC 417) — Multi-Source BFS

```python
def pacific_atlantic(heights: List[List[int]]) -> List[List[int]]:
    """
    Water flows from cell to neighbor if neighbor height ≤ current height.
    Find cells from which water can reach BOTH Pacific and Atlantic.
    
    Key insight: Reverse the flow direction. BFS inward from ocean borders.
    """
    rows, cols = len(heights), len(heights[0])
    directions = [(0,1),(0,-1),(1,0),(-1,0)]

    def bfs(sources):
        visited = set(sources)
        queue = deque(sources)
        while queue:
            r, c = queue.popleft()
            for dr, dc in directions:
                nr, nc = r+dr, c+dc
                if (0 <= nr < rows and 0 <= nc < cols
                        and (nr,nc) not in visited
                        and heights[nr][nc] >= heights[r][c]):  # Reversed condition
                    visited.add((nr, nc))
                    queue.append((nr, nc))
        return visited

    # Pacific: top row + left col; Atlantic: bottom row + right col
    pacific_sources = [(0, c) for c in range(cols)] + [(r, 0) for r in range(1, rows)]
    atlantic_sources = [(rows-1, c) for c in range(cols)] + [(r, cols-1) for r in range(rows-1)]

    pacific_reach  = bfs(pacific_sources)
    atlantic_reach = bfs(atlantic_sources)

    return [[r, c] for r in range(rows) for c in range(cols)
            if (r,c) in pacific_reach and (r,c) in atlantic_reach]
```
**Time**: O(rows × cols)  
**Space**: O(rows × cols)

---

### Problem 5: Shortest Path in Binary Matrix (LC 1091)

```python
def shortest_path_binary_matrix(grid: List[List[int]]) -> int:
    """
    8-directional movement. 0=open, 1=blocked.
    Return length of shortest clear path top-left to bottom-right, or -1.
    """
    n = len(grid)
    if grid[0][0] == 1 or grid[n-1][n-1] == 1:
        return -1
    if n == 1:
        return 1

    queue = deque([(0, 0, 1)])  # (row, col, path_length)
    grid[0][0] = 1  # Mark visited in-place

    dirs = [(-1,-1),(-1,0),(-1,1),(0,-1),(0,1),(1,-1),(1,0),(1,1)]
    while queue:
        r, c, dist = queue.popleft()
        for dr, dc in dirs:
            nr, nc = r+dr, c+dc
            if 0 <= nr < n and 0 <= nc < n and grid[nr][nc] == 0:
                if nr == n-1 and nc == n-1:
                    return dist + 1
                grid[nr][nc] = 1
                queue.append((nr, nc, dist+1))

    return -1
```
**Time**: O(n²)  
**Space**: O(n²)

---

### Problem 6: Minimum Genetic Mutation (LC 433) — BFS on Implicit Graph

```python
def min_mutation(startGene: str, endGene: str, bank: list) -> int:
    """
    Genes: 8-char strings over {A,C,G,T}.
    One mutation = change one character; result must be in bank.
    Return min mutations or -1.
    """
    bank_set = set(bank)
    if endGene not in bank_set:
        return -1

    queue = deque([(startGene, 0)])
    visited = {startGene}

    while queue:
        gene, mutations = queue.popleft()
        if gene == endGene:
            return mutations
        for i in range(len(gene)):
            for c in 'ACGT':
                if c != gene[i]:
                    new_gene = gene[:i] + c + gene[i+1:]
                    if new_gene in bank_set and new_gene not in visited:
                        visited.add(new_gene)
                        queue.append((new_gene, mutations+1))

    return -1
```
**Time**: O(B × L × 4) where B = bank size, L = gene length (8)  
**Space**: O(B)

---

### Problem 7: Snakes and Ladders (LC 909)

```python
def snakes_and_ladders(board: List[List[int]]) -> int:
    """
    BFS on board cells 1..n².
    Key: decode cell number to (row, col) carefully — alternating row directions.
    """
    n = len(board)

    def cell_to_pos(cell):
        """Convert 1-indexed cell to board (row, col)."""
        cell -= 1
        row_from_bottom = cell // n
        col = cell % n
        if row_from_bottom % 2 == 1:  # Odd rows (from bottom) go right-to-left
            col = n - 1 - col
        row = n - 1 - row_from_bottom
        return row, col

    visited = {1}
    queue = deque([(1, 0)])  # (cell, moves)

    while queue:
        cell, moves = queue.popleft()
        for dice in range(1, 7):
            nxt = cell + dice
            if nxt > n * n:
                break
            r, c = cell_to_pos(nxt)
            if board[r][c] != -1:
                nxt = board[r][c]  # Teleport via snake or ladder
            if nxt == n * n:
                return moves + 1
            if nxt not in visited:
                visited.add(nxt)
                queue.append((nxt, moves+1))

    return -1
```
**Time**: O(n²)  
**Space**: O(n²)

---

### Problem 8: Jump Game IV (LC 1345) — BFS with Value-Indexed Jumps

```python
from collections import defaultdict

def min_jumps(arr: List[int]) -> int:
    """
    From index i, can jump to i-1, i+1, or any j where arr[j]==arr[i].
    Return min jumps from index 0 to index n-1.
    
    Key insight: Group indices by value. After processing a value group once,
    remove it to avoid O(n²) worst case.
    """
    n = len(arr)
    if n == 1:
        return 0

    # Precompute value → list of indices
    value_to_indices = defaultdict(list)
    for i, v in enumerate(arr):
        value_to_indices[v].append(i)

    visited = {0}
    queue = deque([(0, 0)])  # (index, jumps)

    while queue:
        idx, jumps = queue.popleft()

        # Generate neighbors: left, right, same-value teleports
        neighbors = []
        if idx > 0:
            neighbors.append(idx-1)
        if idx < n-1:
            neighbors.append(idx+1)
        # Same-value jumps — critical: clear after using to prevent re-processing
        neighbors.extend(value_to_indices[arr[idx]])
        value_to_indices[arr[idx]] = []  # Prune: never revisit this value group

        for nxt in neighbors:
            if nxt == n-1:
                return jumps+1
            if nxt not in visited:
                visited.add(nxt)
                queue.append((nxt, jumps+1))

    return -1
```
**Time**: O(n) amortized (each index visited once, each value group processed once)  
**Space**: O(n)

---

### Problem 9: Minimum Moves to Reach Target with Rotations (LC 1210) — BFS on State Space

```python
def minimum_moves(grid: List[List[int]]) -> int:
    """
    Snake occupies two cells. State = (head_row, head_col, direction).
    direction: 0=horizontal (head right), 1=vertical (head down).
    Target: snake head at (n-1, n-1) horizontal.
    Moves: move right, move down, rotate clockwise/counterclockwise.
    """
    n = len(grid)
    # Initial state: head at (0,1) horizontal → (row, col of head, dir)
    start = (0, 1, 0)
    target = (n-1, n-1, 0)

    visited = {start}
    queue = deque([(start, 0)])

    while queue:
        (r, c, d), moves = queue.popleft()
        if (r, c, d) == target:
            return moves

        states = []
        if d == 0:  # Horizontal: tail=(r, c-1), head=(r, c)
            # Move right: new head=(r, c+1), tail=(r, c)
            if c+1 < n and grid[r][c+1] == 0:
                states.append((r, c+1, 0))
            # Move down: new head=(r+1, c), tail=(r+1, c-1)
            if r+1 < n and grid[r+1][c] == 0 and grid[r+1][c-1] == 0:
                states.append((r+1, c, 0))
            # Rotate clockwise → vertical, head at (r+1, c-1)... 
            # Actually: tail stays at (r, c-1) becomes top, head rotates to (r+1, c-1)
            if r+1 < n and grid[r+1][c] == 0 and grid[r+1][c-1] == 0:
                states.append((r+1, c-1, 1))  # head now at row+1, same col as tail
        else:  # Vertical: tail=(r-1, c), head=(r, c)
            # Move down: new head=(r+1, c)
            if r+1 < n and grid[r+1][c] == 0:
                states.append((r+1, c, 1))
            # Move right: new head=(r, c+1), tail=(r-1, c+1)
            if c+1 < n and grid[r][c+1] == 0 and grid[r-1][c+1] == 0:
                states.append((r, c+1, 1))
            # Rotate counterclockwise → horizontal
            if c+1 < n and grid[r][c+1] == 0 and grid[r-1][c+1] == 0:
                states.append((r-1, c+1, 0))

        for state in states:
            if state not in visited:
                visited.add(state)
                queue.append((state, moves+1))

    return -1
```
**Time**: O(n²) — at most n² states  
**Space**: O(n²)

---

### Problem 10: Bus Routes (LC 815) — BFS on Route-Level Nodes

```python
def num_buses_to_destination(routes: List[List[int]], source: int, target: int) -> int:
    """
    Key insight: BFS over ROUTES (not stops). 
    A stop can appear in multiple routes — model as bipartite: stops ↔ routes.
    """
    if source == target:
        return 0

    from collections import defaultdict
    stop_to_routes = defaultdict(set)
    for i, route in enumerate(routes):
        for stop in route:
            stop_to_routes[stop].add(i)

    visited_stops  = {source}
    visited_routes = set()
    queue = deque([source])
    buses = 0

    while queue:
        buses += 1
        for _ in range(len(queue)):  # Process one level (one bus ride)
            stop = queue.popleft()
            for route_id in stop_to_routes[stop]:
                if route_id in visited_routes:
                    continue
                visited_routes.add(route_id)
                for nxt_stop in routes[route_id]:
                    if nxt_stop == target:
                        return buses
                    if nxt_stop not in visited_stops:
                        visited_stops.add(nxt_stop)
                        queue.append(nxt_stop)

    return -1
```
**Time**: O(Σ|route_i|) — each stop and route processed once  
**Space**: O(Σ|route_i|)

---

## 7. Interview Tips and Edge Cases

### The BFS Invariant — Remind Yourself Constantly
> **At the moment a node is first dequeued, its distance is finalized and optimal.**

This only holds for **unweighted** graphs (or 0-1 with deque). For weighted, Dijkstra.

### Edge Cases That Kill Interviews

| Scenario | Fix |
|---|---|
| Source == Target | Return 0 immediately |
| No path exists | Return -1 or handle empty queue |
| Disconnected graph | Outer loop over all unvisited nodes |
| Grid with all walls | Check before BFS starts |
| Single-cell grid | Handle n=1 before BFS |
| Multi-edges (parallel edges) | Track edges, not just parent node |
| Cycles with same parent tracking | Multi-edges need edge-based tracking |

### State Design for BFS Problems

When a problem seems hard, ask: **"What is my state?"**

- Grid navigation: `(row, col)` → add direction, key, visited subset if needed
- Word ladder: `string` state
- Snake: `(head_row, head_col, direction)` — captures full configuration
- With constraints (at most K steps): `(node, k_remaining)` → 2D state

### Complexity Cheat Sheet

| BFS Variant | Time | Space |
|---|---|---|
| Single-source BFS | O(V+E) | O(V) |
| Multi-source BFS | O(V+E) | O(V) |
| 0-1 BFS (deque) | O(V+E) | O(V) |
| Bidirectional BFS | O(b^(d/2)) | O(b^(d/2)) |
| BFS on state space | O(\|S\|) | O(\|S\|) |

### When to Choose BFS over DFS
- Shortest path in unweighted graph → **always BFS**
- Level-order traversal → **BFS**
- Cycle detection in undirected → **either** (BFS slightly more intuitive)
- Topological sort → **DFS** (post-order) or **Kahn's BFS** (in-degree)
- Connected components → **either**
- All paths / backtracking → **DFS**

### The "Virtual Super-Source" Trick
Any time you see "distance from nearest X" or "minimum steps to reach any of these targets," immediately think:
1. Collect all X into a set
2. Enqueue them all at distance 0
3. Run standard BFS

This transforms O(|X| × (V+E)) multi-run single-source into O(V+E) single multi-source run.

---

*Next: [02_DFS_Advanced_Patterns.md](02_DFS_Advanced_Patterns.md) — DFS coloring, Tarjan's bridge-finding, Euler paths*

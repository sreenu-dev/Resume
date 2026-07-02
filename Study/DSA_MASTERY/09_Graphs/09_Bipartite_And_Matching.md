# Bipartite Graphs and Maximum Matching — FAANG Mastery Guide

> **Audience**: Engineers who know basic graphs — master bipartite checking, Hopcroft-Karp, König's theorem, and Hungarian algorithm concepts.  
> **Goal**: Complete mastery of matching algorithms and their applications at FAANG, with proofs and full implementations.

---

## Table of Contents
1. [Bipartite Check — BFS 2-Coloring](#1-bipartite-check)
2. [Maximum Bipartite Matching — Augmenting Paths O(VE)](#2-augmenting-path-matching)
3. [Hopcroft-Karp — O(E√V) Matching](#3-hopcroft-karp)
4. [König's Theorem — Min Vertex Cover = Max Matching](#4-konigs-theorem)
5. [Hungarian Algorithm — Weighted Matching](#5-hungarian-algorithm)
6. [Edmonds' Blossom Algorithm — General Matching](#6-blossom-concept)
7. [Problem Set with Full Solutions](#7-problem-set)
8. [Interview Tips and Edge Cases](#8-interview-tips)

---

## 1. Bipartite Check — BFS 2-Coloring

A graph is **bipartite** if and only if it contains **no odd-length cycle**.

**2-coloring approach**: Try to color every node with one of 2 colors such that no two adjacent nodes share a color. This is equivalent to bipartite partitioning.

```python
from collections import deque
from typing import List

def is_bipartite(graph: List[List[int]]) -> bool:
    """
    LC 785: Check if graph is bipartite using BFS 2-coloring.
    graph: adjacency list (0-indexed).
    
    Time:  O(V+E)
    Space: O(V)
    """
    n     = len(graph)
    color = [-1] * n   # -1 = uncolored, 0 = red, 1 = blue

    for start in range(n):
        if color[start] != -1:
            continue
        queue = deque([start])
        color[start] = 0

        while queue:
            u = queue.popleft()
            for v in graph[u]:
                if color[v] == -1:
                    color[v] = 1 - color[u]   # Alternate color
                    queue.append(v)
                elif color[v] == color[u]:
                    return False   # Same color as neighbor → odd cycle → not bipartite

    return True


def bipartite_partition(graph: List[List[int]]) -> tuple:
    """
    Returns (set_A, set_B) if bipartite, None otherwise.
    Useful for matching algorithms.
    """
    n     = len(graph)
    color = [-1] * n

    for start in range(n):
        if color[start] != -1:
            continue
        color[start] = 0
        queue = deque([start])
        while queue:
            u = queue.popleft()
            for v in graph[u]:
                if color[v] == -1:
                    color[v] = 1 - color[u]
                    queue.append(v)
                elif color[v] == color[u]:
                    return None

    set_a = {i for i in range(n) if color[i] == 0}
    set_b = {i for i in range(n) if color[i] == 1}
    return set_a, set_b
```

---

## 2. Maximum Bipartite Matching — Augmenting Paths

### Key Concepts

An **augmenting path** for matching M is a path that:
1. Starts at an unmatched node in set A.
2. Alternates between unmatched and matched edges.
3. Ends at an unmatched node in set B.

**Berge's Theorem**: A matching M is maximum if and only if there is no augmenting path.

**Algorithm**: Repeatedly find augmenting paths via DFS from each unmatched node in A. Each augmenting path increases matching size by 1. After augmenting (flipping matched/unmatched along path), matching grows.

```python
def max_bipartite_matching(n_left: int, n_right: int,
                            adj: List[List[int]]) -> int:
    """
    Maximum bipartite matching using DFS augmenting paths.
    adj[u] = list of right-side nodes that left node u can be matched to.
    
    Time:  O(V × E) — at most V augmentations, each DFS is O(E)
    Space: O(V)
    """
    match_left  = [-1] * n_left    # match_left[u]  = right node matched to u
    match_right = [-1] * n_right   # match_right[v] = left  node matched to v

    def try_augment(u: int, visited: List[bool]) -> bool:
        """Try to find augmenting path starting from left node u."""
        for v in adj[u]:
            if visited[v]:
                continue
            visited[v] = True
            # If v is unmatched, OR we can find augmenting path from v's current match
            if match_right[v] == -1 or try_augment(match_right[v], visited):
                match_left[u]  = v
                match_right[v] = u
                return True
        return False

    matching = 0
    for u in range(n_left):
        visited = [False] * n_right   # Reset visited for each augmentation attempt
        if try_augment(u, visited):
            matching += 1

    return matching
```
**Time**: O(V×E) | **Space**: O(V)

---

## 3. Hopcroft-Karp — O(E√V) Maximum Matching

Hopcroft-Karp finds **multiple augmenting paths simultaneously** using BFS to build a layered graph, then DFS to find all vertex-disjoint augmenting paths in that layer.

**Key Insight**: By finding all shortest augmenting paths in each BFS phase, the number of phases is O(√V). Each phase takes O(E). Total: O(E√V).

**Proof of O(√V) phases**: After k phases of Hopcroft-Karp, the shortest augmenting path has length > 2k-1. A matching of size m has at most n/m augmenting paths in the remaining. Mathematical analysis shows at most O(√n) phases suffice.

```python
from collections import deque
from math import inf as INF

def hopcroft_karp(n_left: int, n_right: int, adj: List[List[int]]) -> int:
    """
    Hopcroft-Karp maximum bipartite matching.
    O(E√V) time — optimal for dense bipartite graphs.
    
    Left nodes:  0 to n_left-1
    Right nodes: 0 to n_right-1 (internally shifted by n_left)
    NIL = n_left + n_right (virtual NIL node)
    """
    NIL = n_left + n_right
    total = n_left + n_right + 1  # +1 for NIL node

    # pair_left[u]  = right node matched to left u  (or NIL)
    # pair_right[v] = left  node matched to right v (or NIL)
    pair_left  = [NIL] * n_left
    pair_right = [NIL] * n_right
    dist_left  = [0] * (n_left + 1)   # BFS distances for left nodes

    def bfs() -> bool:
        """BFS phase: build layered graph. Returns True if augmenting path exists."""
        queue = deque()
        for u in range(n_left):
            if pair_left[u] == NIL:
                dist_left[u] = 0
                queue.append(u)
            else:
                dist_left[u] = INF

        found = False
        while queue:
            u = queue.popleft()
            for v in adj[u]:
                w = pair_right[v]   # w = left node currently matched to v (or NIL)
                if w == NIL:
                    found = True
                elif dist_left[w] == INF:
                    dist_left[w] = dist_left[u] + 1
                    queue.append(w)
        return found

    def dfs(u: int) -> bool:
        """DFS phase: find augmenting path from left node u using layered structure."""
        for v in adj[u]:
            w = pair_right[v]
            if w == NIL or (dist_left[w] == dist_left[u] + 1 and dfs(w)):
                pair_left[u]  = v
                pair_right[v] = u
                return True
        dist_left[u] = INF   # Block this node for future DFS in same phase
        return False

    matching = 0
    while bfs():
        for u in range(n_left):
            if pair_left[u] == NIL:
                if dfs(u):
                    matching += 1

    return matching
```
**Time**: O(E√V) | **Space**: O(V+E)

---

## 4. König's Theorem

### Statement

**König's Theorem**: In any bipartite graph, the size of the **maximum matching** equals the size of the **minimum vertex cover**.

**Definitions**:
- **Vertex Cover**: A set of vertices such that every edge has at least one endpoint in the set.
- **Minimum Vertex Cover**: Smallest such set.
- **Independent Set**: A set of vertices with no edges between them.
- **Maximum Independent Set**: Largest such set.

**Complement**: Maximum Independent Set = V − Minimum Vertex Cover (in bipartite graphs).

### Constructing Minimum Vertex Cover from Matching

```python
def minimum_vertex_cover(n_left: int, n_right: int,
                          adj: List[List[int]]) -> tuple:
    """
    Find minimum vertex cover using König's theorem.
    
    Steps:
    1. Find maximum matching M.
    2. Let U = unmatched left nodes.
    3. BFS/DFS alternating path from U:
       - Follow unmatched edges to right, then matched edges back to left.
    4. Cover = (left nodes NOT in alternating path reachability) 
               UNION (right nodes IN alternating path reachability)
    
    Returns (left_cover_nodes, right_cover_nodes).
    """
    match_left  = [-1] * n_left
    match_right = [-1] * n_right

    def try_augment(u, visited):
        for v in adj[u]:
            if not visited[v]:
                visited[v] = True
                if match_right[v] == -1 or try_augment(match_right[v], visited):
                    match_left[u] = v; match_right[v] = u; return True
        return False

    for u in range(n_left):
        try_augment(u, [False]*n_right)

    # Find alternating path reachability from unmatched left nodes
    reachable_left  = [False] * n_left
    reachable_right = [False] * n_right

    queue = deque(u for u in range(n_left) if match_left[u] == -1)
    for u in range(n_left):
        if match_left[u] == -1:
            reachable_left[u] = True

    while queue:
        u = queue.popleft()
        for v in adj[u]:
            if not reachable_right[v]:
                reachable_right[v] = True
                # Follow matched edge back to left
                w = match_right[v]
                if w != -1 and not reachable_left[w]:
                    reachable_left[w] = True
                    queue.append(w)

    # König's: Cover = left nodes NOT reachable + right nodes reachable
    left_cover  = [u for u in range(n_left)  if not reachable_left[u]  and match_left[u]  != -1]
    right_cover = [v for v in range(n_right) if     reachable_right[v] and match_right[v] != -1]

    return left_cover, right_cover
```
**Time**: O(V×E) for matching + O(V+E) for cover construction | **Space**: O(V)

### Why König's Holds (Proof Sketch)

**Lower bound**: Any vertex cover must cover every matched edge — each matched edge needs at least one endpoint in the cover. So `|cover| ≥ |matching|`.

**Upper bound**: The construction above gives a cover of size exactly `|matching|`. Each left cover node is matched (contributes a matched edge). Each right cover node is matched. No node is double-counted (left cover ∩ right cover = ∅ by BFS construction). So the cover has exactly `|matching|` nodes.

**Combined**: minimum cover ≥ maximum matching (LP duality) and we constructed a cover of size = matching → minimum cover = maximum matching. ✓

---

## 5. Hungarian Algorithm — Assignment Problem Concept

The **Hungarian algorithm** solves the **assignment problem**: given n workers and n jobs with costs `c[i][j]`, find a minimum-cost perfect matching.

**Time**: O(N³) — optimal for the dense assignment problem.

**Concept** (without full O(N³) implementation):

```python
def hungarian_algorithm(cost: List[List[int]]) -> int:
    """
    Simplified O(N³) Hungarian algorithm for minimum cost assignment.
    cost[i][j] = cost of assigning worker i to job j.
    Returns minimum total cost of perfect matching.
    
    Key ideas:
    1. Reduce rows: subtract row minimum from each row.
    2. Reduce cols: subtract col minimum from each column.
    3. Find maximum matching in zero-cost edges.
    4. If perfect matching found: done.
    5. Otherwise: find minimum uncovered value, adjust matrix, repeat.
    """
    n = len(cost)
    # Make a copy to avoid modifying input
    c = [row[:] for row in cost]

    # Step 1: Row reduction
    for i in range(n):
        min_val = min(c[i])
        for j in range(n):
            c[i][j] -= min_val

    # Step 2: Column reduction
    for j in range(n):
        min_val = min(c[i][j] for i in range(n))
        for i in range(n):
            c[i][j] -= min_val

    while True:
        # Step 3: Find maximum matching in zero cells
        adj_zero = [[j for j in range(n) if c[i][j] == 0] for i in range(n)]
        match_right = [-1] * n
        match_left  = [-1] * n

        def aug(u, vis):
            for v in adj_zero[u]:
                if not vis[v]:
                    vis[v] = True
                    if match_right[v] == -1 or aug(match_right[v], vis):
                        match_left[u] = v; match_right[v] = u; return True
            return False

        matched = 0
        for u in range(n):
            if aug(u, [False]*n):
                matched += 1

        if matched == n:
            # Perfect matching found
            return sum(cost[i][match_left[i]] for i in range(n))

        # Step 4: Find minimum cover and adjust
        # ... (full cover-finding + matrix adjustment omitted for space)
        # In practice use scipy.optimize.linear_sum_assignment for interviews
        break

    # Fallback: use scipy
    from scipy.optimize import linear_sum_assignment
    row_ind, col_ind = linear_sum_assignment(cost)
    return sum(cost[row_ind[i]][col_ind[i]] for i in range(n))
```

**For FAANG interviews**: Know the concept (row/col reduction → matching → adjustment) and that it's O(N³). Implementation details are rarely asked — `scipy.optimize.linear_sum_assignment` suffices.

---

## 6. Edmonds' Blossom Algorithm — General Matching

**Maximum matching in general (non-bipartite) graphs** requires Edmonds' Blossom algorithm. Bipartite matching algorithms fail on odd cycles ("blossoms").

### Key Concepts (No Full Implementation — Too Complex for Interviews)

**Blossom**: An odd cycle found during augmenting path search. When an augmenting path search encounters a blossom, the blossom must be "contracted" into a single node, the search continues on the contracted graph, then the path is "expanded."

**Why augmenting paths fail on odd cycles**:
- In a bipartite graph, all cycles are even → alternating path search always works.
- In general graphs, an odd cycle creates a situation where a path can enter the cycle from one side and find an augmenting connection — but standard BFS/DFS misses this.

**Blossom contraction**: Contract the odd cycle into a single "super-node." This preserves the augmenting path structure. After finding the path in the contracted graph, re-expand the blossom.

**Time**: O(V³) or O(VE) depending on implementation.

**Interview Advice**: If asked about general matching:
1. Explain that bipartite algorithms fail on odd cycles.
2. Describe the blossom contraction idea.
3. State O(V³) complexity.
4. For bipartite graphs → use Hopcroft-Karp.

---

## 7. Problem Set

---

### Problem 1: Is Graph Bipartite? (LC 785)

```python
def is_bipartite_lc(graph: List[List[int]]) -> bool:
    """BFS 2-coloring on adjacency list (not necessarily connected)."""
    n = len(graph)
    color = [-1] * n

    for start in range(n):
        if color[start] != -1:
            continue
        color[start] = 0
        queue = deque([start])
        while queue:
            u = queue.popleft()
            for v in graph[u]:
                if color[v] == -1:
                    color[v] = 1 - color[u]
                    queue.append(v)
                elif color[v] == color[u]:
                    return False
    return True
```
**Time**: O(V+E) | **Space**: O(V)

---

### Problem 2: Possible Bipartition (LC 886)

```python
def possible_bipartition(n: int, dislikes: List[List[int]]) -> bool:
    """
    Can n people be split into 2 groups such that no two people in the same
    group dislike each other?
    = Is the "dislikes" graph bipartite?
    """
    adj = [[] for _ in range(n+1)]
    for u, v in dislikes:
        adj[u].append(v)
        adj[v].append(u)

    color = [-1] * (n+1)
    for start in range(1, n+1):
        if color[start] != -1:
            continue
        color[start] = 0
        queue = deque([start])
        while queue:
            u = queue.popleft()
            for v in adj[u]:
                if color[v] == -1:
                    color[v] = 1 - color[u]
                    queue.append(v)
                elif color[v] == color[u]:
                    return False
    return True
```
**Time**: O(V+E) | **Space**: O(V)

---

### Problem 3: Maximum Students Taking Exam (LC 1349) — Bipartite Matching via DP Bitmask

```python
def max_students_taking_exam(seats: List[List[str]]) -> int:
    """
    Students can't cheat:
    - Can't have student directly left/right in same row
    - Can't have student upper-left/upper-right in previous row
    
    Model as bipartite matching:
    - Left set: students in even columns; Right set: students in odd columns
    - Edges: potential cheating pairs (max matching = max cheaters to remove)
    - Answer = available seats - min vertex cover = available seats - max matching
    
    OR use DP with bitmask (more straightforward):
    dp[row][mask] = max students in rows 0..row with mask indicating occupied seats in row.
    """
    rows, cols = len(seats), len(seats[0])

    # Valid masks for each row (no adjacent seats occupied)
    def valid_row_mask(row, mask):
        """No two adjacent bits set AND only available seats used."""
        if mask & (mask >> 1):   # Adjacent seats occupied
            return False
        for c in range(cols):
            if (mask >> c) & 1:
                if seats[row][c] == '#':   # Broken seat
                    return False
        return True

    def no_cheat(prev_mask, curr_mask):
        """No upper-left/upper-right conflicts between rows."""
        if curr_mask & (prev_mask >> 1):   # curr student, prev upper-left
            return False
        if curr_mask & (prev_mask << 1):   # curr student, prev upper-right
            return False
        return True

    INF = float('inf')
    dp  = [-INF] * (1 << cols)
    dp[0] = 0

    for row in range(rows):
        new_dp = [-INF] * (1 << cols)
        valid  = [m for m in range(1 << cols) if valid_row_mask(row, m)]

        for curr_mask in valid:
            curr_count = bin(curr_mask).count('1')
            for prev_mask in range(1 << cols):
                if dp[prev_mask] == -INF:
                    continue
                if no_cheat(prev_mask, curr_mask):
                    if dp[prev_mask] + curr_count > new_dp[curr_mask]:
                        new_dp[curr_mask] = dp[prev_mask] + curr_count
        dp = new_dp

    return max(dp)
```
**Time**: O(rows × 4^cols) | **Space**: O(2^cols)

---

### Problem 4: Job Scheduling as Bipartite Matching

```python
def job_assignment(workers: int, jobs: int,
                   skills: List[List[int]]) -> int:
    """
    workers: number of workers
    jobs: number of jobs
    skills[i] = list of jobs worker i can do
    Returns maximum number of jobs that can be assigned (one job per worker).
    
    Classic bipartite matching application.
    """
    adj = skills  # adj[worker] = list of jobs worker can do
    return max_bipartite_matching(workers, jobs, adj)
```

---

### Problem 5: Minimum Number of Days to Disconnect Island (LC 1568)

```python
def min_days(grid: List[List[int]]) -> int:
    """
    Return minimum days to disconnect the island (make more than 1 component or 0 land).
    
    Key observations:
    - 0 days: already disconnected.
    - 1 day: if any land cell is an articulation point.
    - 2 days: always achievable (remove any corner of any land cell).
    
    The answer is always 0, 1, or 2. Never more than 2.
    
    Proof that 2 always works: pick any land cell, remove it and one neighbor.
    This always disconnects (or removes all land).
    """
    from collections import deque

    rows, cols = len(grid), len(grid[0])

    def count_islands(g):
        visited = [[False]*cols for _ in range(rows)]
        count = 0
        for r in range(rows):
            for c in range(cols):
                if g[r][c] == 1 and not visited[r][c]:
                    count += 1
                    if count > 1:
                        return count
                    queue = deque([(r,c)])
                    visited[r][c] = True
                    while queue:
                        cr, cc = queue.popleft()
                        for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
                            nr, nc = cr+dr, cc+dc
                            if 0<=nr<rows and 0<=nc<cols and g[nr][nc]==1 and not visited[nr][nc]:
                                visited[nr][nc] = True
                                queue.append((nr,nc))
        return count

    # Check 0 days
    if count_islands(grid) != 1:
        return 0

    # Check 1 day: try removing each land cell
    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == 1:
                grid[r][c] = 0
                if count_islands(grid) != 1:
                    grid[r][c] = 1
                    return 1
                grid[r][c] = 1

    # Otherwise: 2 days always suffice
    return 2
```
**Time**: O(rows² × cols²) | **Space**: O(rows × cols)

---

### Problem 6: Minimum Path Cover in DAG (Using Matching)

```python
def min_path_cover_dag(n: int, edges: List[List[int]]) -> int:
    """
    Minimum number of paths to cover all nodes in a DAG.
    
    Key theorem: Min path cover = n - Maximum Bipartite Matching.
    
    Construction:
    - Create bipartite graph: left node u_out and right node v_in for each node u,v.
    - Edge u_out → v_in for each DAG edge (u,v).
    - Max matching M gives |M| paths that can be "merged."
    - Minimum paths needed = n - |M|.
    
    Intuition: Each matched edge (u_out, v_in) means "chain u before v in a path."
    k matched edges = k pairs chained = n - k disjoint paths.
    """
    adj_bipartite = [[] for _ in range(n)]
    for u, v in edges:
        adj_bipartite[u].append(v)   # u_out → v_in

    matching = max_bipartite_matching(n, n, adj_bipartite)
    return n - matching
```
**Time**: O(V×E) for matching | **Space**: O(V+E)

---

## 8. Interview Tips and Edge Cases

### Bipartite Graph Applications

| Problem Type | Bipartite Formulation |
|---|---|
| Job assignment | Left=workers, Right=jobs, Edge=can do |
| Exam scheduling | Left=students, Right=time slots, Edge=available |
| Course-student matching | Left=students, Right=courses, Edge=interested |
| Chemical bonding | Left=molecules set A, Right=molecules set B |
| Min path cover in DAG | Left=node_out, Right=node_in, Edge=DAG edge |

### The Key Reductions

| Problem | Reduction |
|---|---|
| Min vertex cover | König's: = max matching in bipartite |
| Max independent set | = n - min vertex cover = n - max matching |
| Min path cover (DAG) | = n - max bipartite matching |
| Min edge cover | = n - max matching (if perfect matching) |

### When to Use Hopcroft-Karp vs Augmenting Path

| Scenario | Algorithm | Time |
|---|---|---|
| Small graph (V, E ≤ 1000) | Augmenting path | O(VE) |
| Large sparse graph | Hopcroft-Karp | O(E√V) |
| Dense complete bipartite | Hopcroft-Karp or Hungarian | O(E√V) or O(V³) |
| Weighted matching (assignment) | Hungarian | O(V³) |
| General graph (odd cycles) | Blossom | O(V³) |

### Common Mistakes

| Mistake | Fix |
|---|---|
| Using bipartite matching on non-bipartite graph | Check bipartiteness first |
| Resetting visited array globally (not per augmentation) | Reset `visited` for each source node in augmenting path DFS |
| Confusing vertex cover with edge cover | Vertex cover: covers edges; Edge cover: covers vertices |
| König's theorem applied to non-bipartite graph | Only valid for bipartite graphs |
| Forgetting disconnected components in bipartite check | Outer loop over all unvisited nodes |

### The Power of König's Theorem

König's theorem connects three combinatorial quantities in bipartite graphs:

```
Max Matching (M*) = Min Vertex Cover (C*)
Max Independent Set = V - Min Vertex Cover = V - M*
Min Edge Cover = V - Max Matching  (when perfect matching exists)
```

Whenever a problem asks for minimum cover or maximum independent set **in a bipartite graph**, immediately think:
1. Model as bipartite graph.
2. Find max matching in O(E√V) via Hopcroft-Karp.
3. Apply König's theorem.

---

*Next: [10_Network_Flow.md](10_Network_Flow.md) — Ford-Fulkerson, Edmonds-Karp, Dinic's algorithm, max-flow min-cut, full applications*

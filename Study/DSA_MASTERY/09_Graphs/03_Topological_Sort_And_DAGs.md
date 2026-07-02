# Topological Sort and DAGs — FAANG Mastery Guide

> **Audience**: Engineers who know basic sorting — master DAG DP, critical paths, and every topo-sort variant.  
> **Goal**: Complete mastery of DAG algorithms asked at Google, Meta, Amazon, Apple, Netflix.

---

## Table of Contents
1. [Kahn's Algorithm — BFS-Based Topo Sort](#1-kahns-algorithm)
2. [DFS-Based Topo Sort — Post-Order Reverse](#2-dfs-based-topo-sort)
3. [Cycle Detection via Topo Sort](#3-cycle-detection)
4. [Critical Path Method — Longest Path in DAG](#4-critical-path-method)
5. [DAG Dynamic Programming](#5-dag-dynamic-programming)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. Kahn's Algorithm — BFS-Based Topo Sort

### Algorithm

Kahn's algorithm exploits the **in-degree** property: any node with in-degree 0 can be safely placed first (no dependency on it from unprocessed nodes).

**Steps**:
1. Compute in-degree of every node.
2. Enqueue all nodes with in-degree 0.
3. Process queue: dequeue u, add to result, **decrement** in-degree of all neighbors. If any neighbor's in-degree hits 0, enqueue it.
4. If result contains all nodes → valid topo order. Otherwise → cycle exists.

**Why it works**: At every step, we process nodes that have no remaining unprocessed dependencies. Decrementing in-degree simulates "satisfying" a prerequisite. A node reaches 0 only when ALL its dependencies are processed.

```python
from collections import deque
from typing import List

def topological_sort_kahn(n: int, edges: List[List[int]]) -> List[int]:
    """
    Kahn's BFS-based topological sort.
    edges: list of [u, v] meaning u must come before v.
    Returns topo order, or [] if cycle detected.
    Time: O(V+E), Space: O(V+E)
    """
    adj     = [[] for _ in range(n)]
    in_deg  = [0] * n

    for u, v in edges:
        adj[u].append(v)
        in_deg[v] += 1

    queue  = deque(i for i in range(n) if in_deg[i] == 0)
    result = []

    while queue:
        u = queue.popleft()
        result.append(u)
        for v in adj[u]:
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)

    return result if len(result) == n else []   # len < n → cycle
```
**Time**: O(V+E) | **Space**: O(V+E)

### Why Kahn's Is Preferred Over DFS Topo Sort in Practice

1. **Cycle detection is trivial**: if output length < n, a cycle exists (no separate logic needed).
2. **Naturally level-by-level**: useful for computing earliest start times (critical path).
3. **No recursion**: avoids stack overflow on large graphs.
4. **Parallelism**: nodes processed at the same "level" (same BFS round) have no dependency on each other → can execute in parallel.

---

## 2. DFS-Based Topo Sort — Post-Order Reverse

**Insight**: In DFS, a node is added to the result **after all its dependencies are fully explored** (post-order). Reversing this gives topo order.

```python
def topological_sort_dfs(n: int, edges: List[List[int]]) -> List[int]:
    """
    DFS post-order topological sort.
    Returns topo order ([] if cycle detected).
    Time: O(V+E), Space: O(V)
    """
    adj   = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)

    # 0=white, 1=gray (on stack), 2=black (done)
    state  = [0] * n
    result = []
    has_cycle = [False]

    def dfs(u):
        if has_cycle[0]:
            return
        state[u] = 1
        for v in adj[u]:
            if state[v] == 1:
                has_cycle[0] = True
                return
            if state[v] == 0:
                dfs(v)
        state[u] = 2
        result.append(u)   # Post-order

    for i in range(n):
        if state[i] == 0:
            dfs(i)

    return [] if has_cycle[0] else result[::-1]
```
**Time**: O(V+E) | **Space**: O(V)

---

## 3. Cycle Detection via Topo Sort

**Key Theorem**: A directed graph has a topological ordering **if and only if** it is a DAG (Directed Acyclic Graph).

**Proof sketch**:
- If a cycle exists, no node in the cycle can be placed before others in the cycle → no valid topo order.
- If no cycle exists, DFS/Kahn's always produces a valid ordering.

**Practical detection**: Run Kahn's. If `len(result) < n`, the remaining nodes (those not in result) form one or more cycles.

```python
def find_cycle_nodes(n: int, edges: List[List[int]]) -> List[int]:
    """Returns nodes that are part of a cycle (if any)."""
    adj    = [[] for _ in range(n)]
    in_deg = [0] * n
    for u, v in edges:
        adj[u].append(v)
        in_deg[v] += 1

    queue  = deque(i for i in range(n) if in_deg[i] == 0)
    topo_set = set()

    while queue:
        u = queue.popleft()
        topo_set.add(u)
        for v in adj[u]:
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)

    return [i for i in range(n) if i not in topo_set]
```

---

## 4. Critical Path Method — Longest Path in DAG

In project scheduling, the **critical path** is the longest path through the DAG — it determines the minimum completion time. No shortest-path algorithm works here (longest path is NP-hard in general), but in a **DAG it's O(V+E)** using DP in topological order.

```python
def longest_path_dag(n: int, edges: List[List[int]]) -> int:
    """
    Finds the length of the longest path in a DAG.
    edges: list of [u, v, weight].
    Returns longest path length.
    Time: O(V+E), Space: O(V)
    """
    adj    = [[] for _ in range(n)]
    in_deg = [0] * n
    for u, v, w in edges:
        adj[u].append((v, w))
        in_deg[v] += 1

    # Kahn's for topo order
    queue  = deque(i for i in range(n) if in_deg[i] == 0)
    dp     = [0] * n   # dp[u] = longest path ending at u

    while queue:
        u = queue.popleft()
        for v, w in adj[u]:
            dp[v] = max(dp[v], dp[u] + w)
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)

    return max(dp)
```
**Time**: O(V+E) | **Space**: O(V)

### Earliest/Latest Start Times (Full CPM)

```python
def critical_path_method(n: int, edges: List[List[int]], durations: List[int]) -> dict:
    """
    Computes ES (Earliest Start), EF (Earliest Finish),
    LS (Latest Start), LF (Latest Finish), Slack for each task.
    edges: [u, v] meaning task u must finish before task v starts.
    """
    adj     = [[] for _ in range(n)]
    radj    = [[] for _ in range(n)]  # Reverse graph for backward pass
    in_deg  = [0] * n

    for u, v in edges:
        adj[u].append(v)
        radj[v].append(u)
        in_deg[v] += 1

    # Forward pass: compute ES and EF
    ES = [0] * n
    EF = [d for d in durations]
    queue = deque(i for i in range(n) if in_deg[i] == 0)
    topo = []

    in_deg_copy = in_deg[:]
    while queue:
        u = queue.popleft()
        topo.append(u)
        EF[u] = ES[u] + durations[u]
        for v in adj[u]:
            ES[v] = max(ES[v], EF[u])
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)

    project_duration = max(EF)

    # Backward pass: compute LF and LS
    LF = [project_duration] * n
    LS = [0] * n
    out_deg = [0] * n
    for u in range(n):
        for v in adj[u]:
            out_deg[u] += 1

    queue = deque(i for i in range(n) if out_deg[i] == 0)
    out_deg_copy = out_deg[:]
    while queue:
        v = queue.popleft()
        LS[v] = LF[v] - durations[v]
        for u in radj[v]:
            LF[u] = min(LF[u], LS[v])
            out_deg_copy[u] -= 1
            if out_deg_copy[u] == 0:
                queue.append(u)

    slack = [LS[i] - ES[i] for i in range(n)]
    critical = [i for i in range(n) if slack[i] == 0]

    return {"ES": ES, "EF": EF, "LS": LS, "LF": LF,
            "slack": slack, "critical_path": critical,
            "project_duration": project_duration}
```
**Time**: O(V+E) | **Space**: O(V)

---

## 5. DAG Dynamic Programming

Many DP problems are secretly DAG DP: states are nodes, transitions are edges, and DP runs in topological order.

### Patterns

```python
# Pattern 1: Number of paths from source to every node
def count_paths_dag(n, edges, source):
    adj    = [[] for _ in range(n)]
    in_deg = [0] * n
    for u, v in edges:
        adj[u].append(v)
        in_deg[v] += 1

    dp = [0] * n
    dp[source] = 1
    queue = deque(i for i in range(n) if in_deg[i] == 0)
    in_deg_copy = in_deg[:]

    while queue:
        u = queue.popleft()
        for v in adj[u]:
            dp[v] += dp[u]
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)
    return dp

# Pattern 2: Shortest path in DAG (works even with negative weights!)
def shortest_path_dag(n, edges, source):
    """Unlike Dijkstra, DAG shortest path handles negative weights via topo order."""
    adj    = [[] for _ in range(n)]
    in_deg = [0] * n
    for u, v, w in edges:
        adj[u].append((v, w))
        in_deg[v] += 1

    INF = float('inf')
    dp  = [INF] * n
    dp[source] = 0
    queue = deque(i for i in range(n) if in_deg[i] == 0)
    in_deg_copy = in_deg[:]

    while queue:
        u = queue.popleft()
        for v, w in adj[u]:
            dp[v] = min(dp[v], dp[u] + w)
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)
    return dp
```

---

## 6. Problem Set

---

### Problem 1: Course Schedule I & II (LC 207, 210)

```python
def can_finish(numCourses: int, prerequisites: List[List[int]]) -> bool:
    """LC 207: Can we complete all courses? Iff no cycle in dependency DAG."""
    adj    = [[] for _ in range(numCourses)]
    in_deg = [0] * numCourses
    for a, b in prerequisites:
        adj[b].append(a)
        in_deg[a] += 1

    queue = deque(i for i in range(numCourses) if in_deg[i] == 0)
    count = 0
    while queue:
        u = queue.popleft()
        count += 1
        for v in adj[u]:
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)
    return count == numCourses


def find_order(numCourses: int, prerequisites: List[List[int]]) -> List[int]:
    """LC 210: Return valid course order, or [] if impossible."""
    adj    = [[] for _ in range(numCourses)]
    in_deg = [0] * numCourses
    for a, b in prerequisites:
        adj[b].append(a)
        in_deg[a] += 1

    queue  = deque(i for i in range(numCourses) if in_deg[i] == 0)
    result = []
    while queue:
        u = queue.popleft()
        result.append(u)
        for v in adj[u]:
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)
    return result if len(result) == numCourses else []
```
**Time**: O(V+E) | **Space**: O(V+E)

---

### Problem 2: Course Schedule IV (LC 1462) — Reachability Queries on DAG

```python
def check_if_prerequisite(numCourses: int, prerequisites: List[List[int]],
                           queries: List[List[int]]) -> List[bool]:
    """
    For each query [u, v]: is u a direct or indirect prerequisite of v?
    
    Approach: Topo sort + propagate reachability sets.
    reachable[u] = set of all nodes reachable from u.
    
    Better for dense queries: Floyd-Warshall-style bit propagation.
    Time: O(V² + E + Q), Space: O(V²)
    """
    adj    = [[] for _ in range(numCourses)]
    in_deg = [0] * numCourses
    for u, v in prerequisites:
        adj[u].append(v)
        in_deg[v] += 1

    # reachable[u] = bitmask of nodes reachable from u
    reachable = [0] * numCourses

    queue = deque(i for i in range(numCourses) if in_deg[i] == 0)
    in_deg_copy = in_deg[:]

    while queue:
        u = queue.popleft()
        for v in adj[u]:
            reachable[u] |= (1 << v) | reachable[v]  # u can reach v and everything v reaches
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)

    return [(reachable[u] >> v) & 1 == 1 for u, v in queries]
```
**Time**: O(V²/64 + E + Q) with bitmask | **Space**: O(V²/64)

**Note**: For numCourses > 64, use `set`-based approach or bitarray:
```python
def check_if_prerequisite_sets(numCourses, prerequisites, queries):
    adj    = [[] for _ in range(numCourses)]
    in_deg = [0] * numCourses
    for u, v in prerequisites:
        adj[u].append(v)
        in_deg[v] += 1

    reachable = [set() for _ in range(numCourses)]
    queue = deque(i for i in range(numCourses) if in_deg[i] == 0)
    in_deg_copy = in_deg[:]

    # Process in REVERSE topo order (leaves first, propagate backward)
    topo = []
    while queue:
        u = queue.popleft()
        topo.append(u)
        for v in adj[u]:
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)

    for u in reversed(topo):
        for v in adj[u]:
            reachable[u].add(v)
            reachable[u] |= reachable[v]

    return [v in reachable[u] for u, v in queries]
```

---

### Problem 3: Alien Dictionary (LC 269) — Deduce Order from Sorted Strings

```python
def alien_order(words: List[str]) -> str:
    """
    Given words sorted in alien language alphabetical order,
    deduce the character ordering.
    
    Key insight: Compare adjacent words to extract ordering constraints.
    Then topological sort the character graph.
    """
    # Initialize: all unique chars have in-degree 0
    adj    = {c: [] for word in words for c in word}
    in_deg = {c: 0  for word in words for c in word}

    for i in range(len(words)-1):
        w1, w2 = words[i], words[i+1]
        min_len = min(len(w1), len(w2))

        # Edge case: w1 is prefix of w2 but longer → invalid
        if len(w1) > len(w2) and w1[:min_len] == w2[:min_len]:
            return ""

        for j in range(min_len):
            if w1[j] != w2[j]:
                adj[w1[j]].append(w2[j])
                in_deg[w2[j]] += 1
                break   # Only first differing character gives ordering info

    # Kahn's topological sort on character graph
    queue  = deque(c for c in in_deg if in_deg[c] == 0)
    result = []

    while queue:
        c = queue.popleft()
        result.append(c)
        for nb in adj[c]:
            in_deg[nb] -= 1
            if in_deg[nb] == 0:
                queue.append(nb)

    # Valid iff all characters appear in result
    return "".join(result) if len(result) == len(in_deg) else ""
```
**Time**: O(C + U + min(N-1, U)) where C=total chars, U=unique chars, N=words  
**Space**: O(U + E)

**Edge Cases**:
- `["z","x"]` → `"zx"` (z before x)
- `["z","z"]` → `"z"` (same word, no info)
- `["abc","ab"]` → `""` (invalid: longer word is prefix of shorter)
- `["a","b","a"]` → `""` (cycle: a<b<a)

---

### Problem 4: Minimum Height Trees (LC 310) — Graph Center Finding

```python
def find_min_height_trees(n: int, edges: List[List[int]]) -> List[int]:
    """
    Root at the center of the tree minimizes height.
    The center is the last 1-2 nodes remaining after iteratively
    removing leaf nodes (like topological sort peeling).
    
    Key insight: This is topo sort on an undirected tree — leaves have degree 1.
    """
    if n == 1:
        return [0]
    if n == 2:
        return [0, 1]

    adj    = [set() for _ in range(n)]
    degree = [0] * n
    for u, v in edges:
        adj[u].add(v)
        adj[v].add(u)
        degree[u] += 1
        degree[v] += 1

    # Initial leaves: degree == 1
    leaves = deque(i for i in range(n) if degree[i] == 1)
    remaining = n

    while remaining > 2:
        remaining -= len(leaves)
        new_leaves = deque()
        for leaf in leaves:
            for nb in adj[leaf]:
                degree[nb] -= 1
                if degree[nb] == 1:
                    new_leaves.append(nb)
        leaves = new_leaves

    return list(leaves)
```
**Time**: O(n) | **Space**: O(n)

**Insight**: The center of a tree (node minimizing max distance to any leaf) is always the last 1 or 2 nodes standing after iterative leaf removal. A tree can have at most 2 centers.

---

### Problem 5: Sequence Reconstruction (LC 444)

```python
def sequence_reconstruction(nums: List[int], sequences: List[List[int]]) -> bool:
    """
    Can [nums] be the UNIQUE shortest supersequence reconstructible from sequences?
    Conditions:
    1. nums is a valid topo sort of the DAG formed by sequences.
    2. It's the ONLY valid topo sort (at every step, queue has exactly 1 element).
    """
    n   = max(nums)
    adj = [set() for _ in range(n+1)]
    in_deg = [0] * (n+1)

    for seq in sequences:
        for i in range(len(seq)-1):
            u, v = seq[i], seq[i+1]
            if v not in adj[u]:
                adj[u].add(v)
                in_deg[v] += 1

    queue = deque(i for i in range(1, n+1) if in_deg[i] == 0)
    idx   = 0

    while queue:
        if len(queue) > 1:       # Multiple choices → not unique
            return False
        u = queue.popleft()
        if idx >= len(nums) or nums[idx] != u:
            return False
        idx += 1
        for v in adj[u]:
            in_deg[v] -= 1
            if in_deg[v] == 0:
                queue.append(v)

    return idx == len(nums)
```
**Time**: O(V+E) | **Space**: O(V+E)

---

### Problem 6: Parallel Courses (LC 1136)

```python
def minimum_semesters(n: int, relations: List[List[int]]) -> int:
    """
    BFS topo sort — process all courses with 0 prerequisites simultaneously.
    Number of BFS rounds = minimum semesters.
    """
    adj    = [[] for _ in range(n+1)]
    in_deg = [0] * (n+1)
    for u, v in relations:
        adj[u].append(v)
        in_deg[v] += 1

    queue    = deque(i for i in range(1, n+1) if in_deg[i] == 0)
    semesters = 0
    taken     = 0

    while queue:
        semesters += 1
        for _ in range(len(queue)):   # One semester = one BFS level
            u = queue.popleft()
            taken += 1
            for v in adj[u]:
                in_deg[v] -= 1
                if in_deg[v] == 0:
                    queue.append(v)

    return semesters if taken == n else -1
```
**Time**: O(V+E) | **Space**: O(V+E)

---

### Problem 7: Find All Possible Recipes (LC 2115)

```python
def find_all_recipes(recipes: List[str], ingredients: List[List[str]],
                     supplies: List[str]) -> List[str]:
    """
    supplies = available ingredients (no dependencies).
    recipes[i] needs ingredients[i] (which may be other recipes).
    Return all makeable recipes.
    
    Model as DAG: ingredient/recipe nodes, edges ingredient→recipe.
    Kahn's from supplies.
    """
    adj    = {}
    in_deg = {}

    recipe_set = set(recipes)

    # Initialize in-degrees for recipes
    for i, recipe in enumerate(recipes):
        in_deg[recipe] = 0
        for ing in ingredients[i]:
            if ing not in adj:
                adj[ing] = []
            adj[ing].append(recipe)
            in_deg[recipe] = in_deg.get(recipe, 0) + 1

    # For non-recipe ingredients with no in_deg entry, ignore
    queue  = deque(s for s in supplies)
    result = []

    while queue:
        item = queue.popleft()
        if item in recipe_set:
            result.append(item)
        for nxt in adj.get(item, []):
            in_deg[nxt] -= 1
            if in_deg[nxt] == 0:
                queue.append(nxt)

    return result
```
**Time**: O(V+E) | **Space**: O(V+E)

---

### Problem 8: Minimum Time to Complete Tasks (Task Scheduler with Dependencies)

```python
def minimum_time(n: int, relations: List[List[int]], time: List[int]) -> int:
    """
    LC 2050: Find minimum time to finish all courses given prerequisites
    and per-course time requirements.
    
    This is longest path in DAG with node weights (task durations).
    dp[u] = minimum time to FINISH task u (including all its prerequisites).
    """
    adj    = [[] for _ in range(n+1)]
    in_deg = [0] * (n+1)
    for u, v in relations:
        adj[u].append(v)
        in_deg[v] += 1

    # dp[u] = earliest finish time for task u
    dp    = [time[i-1] for i in range(n+1)]   # dp[i] initialized to task i's duration
    dp[0] = 0
    queue = deque(i for i in range(1, n+1) if in_deg[i] == 0)
    in_deg_copy = in_deg[:]

    while queue:
        u = queue.popleft()
        for v in adj[u]:
            # v can only start after u finishes
            dp[v] = max(dp[v], dp[u] + time[v-1])
            in_deg_copy[v] -= 1
            if in_deg_copy[v] == 0:
                queue.append(v)

    return max(dp[1:])
```
**Time**: O(V+E) | **Space**: O(V+E)

---

## 7. Interview Tips and Edge Cases

### The "Unique Topo Sort" Trick
If a problem asks for a **unique** ordering, run Kahn's and check that the queue **never has more than 1 element**. Multiple elements = multiple valid orderings.

### Topo Sort on Disconnected Graphs
Always initialize the queue with **ALL** nodes of in-degree 0, not just one. Disconnected graphs have multiple source nodes.

### Edge Cases That Kill Candidates

| Scenario | Handling |
|---|---|
| Self-loops | Immediately detects as cycle (in-degree never reaches 0) |
| Disconnected DAG | Outer loop / all-source initialization |
| Nodes with no edges | They start in the topo order; still count |
| Empty graph | Return all nodes in any order |
| Alien dict: duplicate consecutive words | `["abc","abc"]` → just skip, no info |
| Alien dict: prefix violation | `["ab","a"]` → return `""` immediately |

### DAG DP Ordering Rule
**Always process edges in topo order**: when processing node u, all predecessors of u have already been processed. This ensures dp[u] is computed using finalized values.

### Complexity Cheat Sheet

| Algorithm | Time | Space |
|---|---|---|
| Kahn's Topo Sort | O(V+E) | O(V+E) |
| DFS Topo Sort | O(V+E) | O(V) |
| Longest Path DAG | O(V+E) | O(V) |
| All-Pairs Reachability | O(V²) | O(V²) |
| Critical Path Method | O(V+E) | O(V) |
| Count Paths in DAG | O(V+E) | O(V) |

### When DAG DP Beats Other Algorithms
- **Longest path**: O(V+E) via DAG DP vs NP-hard in general graphs.
- **Shortest path with negative weights**: O(V+E) via DAG DP vs O(VE) Bellman-Ford.
- **Number of paths**: O(V+E) via DAG DP — no general algorithm needed.

The moment you see "DAG" or confirm "no cycles," switch to topological DP.

---

*Next: [04_Dijkstra_And_Shortest_Paths.md](04_Dijkstra_And_Shortest_Paths.md) — Dijkstra variants, state-space Dijkstra, modified algorithms*

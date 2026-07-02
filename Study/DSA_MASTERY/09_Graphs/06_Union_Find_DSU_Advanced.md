# Advanced Union-Find (DSU) — FAANG Mastery Guide

> **Audience**: Engineers who know basic DSU — master weighted DSU, rollback, virtual nodes, and amortized analysis.  
> **Goal**: Every advanced DSU pattern asked at FAANG with proofs, amortized complexity, and 7+ complete solutions.

---

## Table of Contents
1. [DSU with Union by Rank + Path Compression — Amortized O(α(N)) Proof](#1-dsu-with-optimizations)
2. [DSU with Rollback — Offline Algorithms](#2-dsu-with-rollback)
3. [Weighted DSU — Parity and Bipartite Checking](#3-weighted-dsu)
4. [Virtual Nodes in DSU](#4-virtual-nodes)
5. [DSU on Offline Queries](#5-offline-queries)
6. [Problem Set with Full Solutions](#6-problem-set)
7. [Interview Tips and Edge Cases](#7-interview-tips)

---

## 1. DSU with Union by Rank + Path Compression

### The Canonical Implementation

```python
class DSU:
    """
    Disjoint Set Union with union by rank and path compression.
    Time per operation: O(α(N)) amortized — effectively O(1).
    Space: O(N)
    """
    def __init__(self, n: int):
        self.parent = list(range(n))
        self.rank   = [0] * n
        self.size   = [1] * n
        self.components = n

    def find(self, x: int) -> int:
        """Path compression: make all nodes on path point directly to root."""
        if self.parent[x] != x:
            self.parent[x] = self.find(self.parent[x])   # Path compression
        return self.parent[x]

    def union(self, x: int, y: int) -> bool:
        """
        Union by rank. Returns True if x and y were in different components.
        Union by rank ensures tree height stays O(log N) without path compression.
        With path compression: height effectively O(α(N)).
        """
        rx, ry = self.find(x), self.find(y)
        if rx == ry:
            return False   # Already connected
        if self.rank[rx] < self.rank[ry]:
            rx, ry = ry, rx
        self.parent[ry] = rx
        self.size[rx] += self.size[ry]
        if self.rank[rx] == self.rank[ry]:
            self.rank[rx] += 1
        self.components -= 1
        return True

    def connected(self, x: int, y: int) -> bool:
        return self.find(x) == self.find(y)

    def get_size(self, x: int) -> int:
        return self.size[self.find(x)]
```

### Amortized O(α(N)) Proof Sketch

**Ackermann function** A(m, n) grows faster than any primitive recursive function. Its **inverse** α(n) grows so slowly that for all practical purposes α(n) ≤ 4 (even for n = 2^(2^(2^(2^16)))).

**Intuition for the proof** (Tarjan's potential analysis):

Define a "rank" function where rank[x] is an upper bound on the height of x's subtree:
- Union by rank: when merging trees of ranks r and s (r < s), result has rank s (unchanged). When r == s, result has rank r+1. This ensures tree height is at most O(log N) WITHOUT path compression.
- Path compression: every find traversal re-links all traversed nodes directly to root. This "pays" for future traversals by flattening the tree.

The amortized cost uses a potential function based on ranks. Each operation is charged O(α(N)) amortized because:
- Each node's rank only increases (never decreases)
- Path compression can only flatten trees (reduces future find costs)
- The "potential" (work done in restructuring) is bounded by O(N α(N)) for all operations

**Practical Takeaway**: Union by rank alone gives O(log N). Path compression alone gives O(log N) amortized. Together: O(α(N)) ≈ O(1).

---

## 2. DSU with Rollback — For Offline Algorithms

Standard DSU with path compression cannot be rolled back (path compression rewrites parent pointers). For rollback, use **union by rank WITHOUT path compression**.

Tree height without path compression: O(log N). With rollback support.

```python
class DSUWithRollback:
    """
    DSU with union by rank (NO path compression) supporting rollback.
    Used in offline algorithms (e.g., offline LCA, offline dynamic connectivity).
    
    Time: O(log N) per operation (no path compression).
    Space: O(N + Q) where Q = number of union operations (for history).
    """
    def __init__(self, n: int):
        self.parent     = list(range(n))
        self.rank       = [0] * n
        self.size       = [1] * n
        self.components = n
        self.history    = []   # Stack of (type, node, old_value) for rollback

    def find(self, x: int) -> int:
        """No path compression — essential for rollback correctness."""
        while self.parent[x] != x:
            x = self.parent[x]
        return x

    def union(self, x: int, y: int) -> bool:
        rx, ry = self.find(x), self.find(y)
        if rx == ry:
            self.history.append(None)   # Sentinel: no-op union
            return False
        if self.rank[rx] < self.rank[ry]:
            rx, ry = ry, rx
        # Save state before modification
        self.history.append((rx, ry, self.rank[rx], self.size[rx]))
        self.parent[ry] = rx
        self.size[rx] += self.size[ry]
        if self.rank[rx] == self.rank[ry]:
            self.rank[rx] += 1
        self.components -= 1
        return True

    def rollback(self):
        """Undo the last union operation."""
        record = self.history.pop()
        if record is None:
            return   # Was a no-op union
        rx, ry, old_rank_rx, old_size_rx = record
        self.parent[ry] = ry        # Detach ry
        self.rank[rx]   = old_rank_rx
        self.size[rx]   = old_size_rx
        self.components += 1

    def save(self) -> int:
        """Return current history length (checkpoint)."""
        return len(self.history)

    def restore(self, checkpoint: int):
        """Rollback to checkpoint."""
        while len(self.history) > checkpoint:
            self.rollback()
```

### Application: Offline Dynamic Connectivity

```python
def offline_dynamic_connectivity(n: int, edges: List[tuple],
                                  queries: List[tuple]) -> List[bool]:
    """
    Edges have active intervals [l, r]. Queries ask: "at time t, are u and v connected?"
    
    Segment tree on time axis + DSU with rollback.
    Each edge exists during [l, r]: add to all O(log Q) segments covering [l, r].
    DFS on segment tree, applying/rolling back edges at each node.
    
    Time: O((E + Q) log Q × log N)
    """
    # Simplified sketch — full implementation requires segment tree
    Q = len(queries)
    results = []
    dsu = DSUWithRollback(n)

    # For each query time t, collect all edges active at t
    # Then union them, check connectivity, rollback
    for t, u, v in queries:
        checkpoint = dsu.save()
        for eu, ev, l, r in edges:
            if l <= t <= r:
                dsu.union(eu, ev)
        results.append(dsu.connected(u, v))
        dsu.restore(checkpoint)

    return results
```

---

## 3. Weighted DSU — Parity and Bipartite Checking

Weighted DSU stores **relative weights** between nodes (e.g., parity, ratio). The weight `w[x]` represents the relationship between x and `parent[x]`.

```python
class WeightedDSU:
    """
    Weighted DSU for parity/bipartite problems.
    weight[x] = XOR distance from x to its root (0=same group, 1=different group).
    
    Bipartite check: graph is bipartite iff no odd cycle exists.
    Parity constraint: can represent 2-coloring as edge weights.
    """
    def __init__(self, n: int):
        self.parent = list(range(n))
        self.weight = [0] * n   # XOR weight to parent

    def find(self, x: int):
        """Returns (root, parity_to_root). Path compression with weight update."""
        if self.parent[x] == x:
            return x, 0
        root, parent_weight = self.find(self.parent[x])
        self.weight[x] ^= parent_weight   # Accumulate XOR up to root
        self.parent[x] = root
        return root, self.weight[x]

    def union(self, x: int, y: int, w: int) -> bool:
        """
        Merge sets with constraint: parity(x) XOR parity(y) = w.
        w=0 means x,y in same group. w=1 means x,y in different groups.
        Returns False if constraint is contradicted (odd cycle detected).
        """
        rx, wx = self.find(x)
        ry, wy = self.find(y)
        if rx == ry:
            # Check consistency: existing parity(x XOR y) should equal w
            return (wx ^ wy) == w
        # Merge: set parent[ry] = rx, compute weight to maintain constraint
        self.parent[ry] = rx
        self.weight[ry] = wx ^ wy ^ w
        return True

    def is_bipartite_edge(self, x: int, y: int) -> bool:
        """Check if adding edge (x,y) creates an odd cycle."""
        return self.union(x, y, 1)   # Different groups constraint
```

### Bipartite Check Using Weighted DSU

```python
def is_bipartite_dsu(n: int, edges: List[List[int]]) -> bool:
    dsu = WeightedDSU(n)
    for u, v in edges:
        if not dsu.union(u, v, 1):   # u and v should be in different groups
            return False
    return True
```
**Time**: O(E × α(N)) | **Space**: O(N)

---

## 4. Virtual Nodes in DSU

Virtual nodes let you model complex groupings where multiple physical nodes share a common "group representative" node that doesn't correspond to an actual input node.

```python
class DSUWithVirtualNodes:
    """
    Virtual nodes allow:
    - Grouping rows with columns (bipartite DSU)
    - Representing "any row r" or "any column c" as single nodes
    """
    def __init__(self, rows: int, cols: int):
        # Virtual node IDs: rows [0..rows-1], cols [rows..rows+cols-1]
        n = rows + cols
        self.parent = list(range(n))
        self.rank   = [0] * n
        self.rows   = rows

    def find(self, x):
        if self.parent[x] != x:
            self.parent[x] = self.find(self.parent[x])
        return self.parent[x]

    def union(self, x, y):
        rx, ry = self.find(x), self.find(y)
        if rx == ry:
            return
        if self.rank[rx] < self.rank[ry]:
            rx, ry = ry, rx
        self.parent[ry] = rx
        if self.rank[rx] == self.rank[ry]:
            self.rank[rx] += 1

    def union_row_col(self, r: int, c: int):
        """Connect row r with column c via virtual node."""
        self.union(r, self.rows + c)

    def same_component(self, x, y):
        return self.find(x) == self.find(y)
```

---

## 5. Offline Queries with DSU

For problems where queries are known upfront, process them offline in a smarter order.

---

## 6. Problem Set

---

### Problem 1: Number of Connected Components in Undirected Graph (LC 323)

```python
def count_components(n: int, edges: List[List[int]]) -> int:
    dsu = DSU(n)
    for u, v in edges:
        dsu.union(u, v)
    return dsu.components
```
**Time**: O(E × α(N)) | **Space**: O(N)

---

### Problem 2: Accounts Merge (LC 721) — DSU with String Keys

```python
def accounts_merge(accounts: List[List[str]]) -> List[List[str]]:
    """
    Merge accounts sharing at least one email.
    DSU on emails. Each email's representative = account owner.
    
    Key insight: Index emails 0..total_emails-1, DSU on indices.
    """
    email_to_id = {}   # email → unique integer ID
    email_to_name = {} # email → account name
    idx = 0

    for account in accounts:
        name = account[0]
        for email in account[1:]:
            if email not in email_to_id:
                email_to_id[email]   = idx
                email_to_name[email] = name
                idx += 1

    dsu = DSU(idx)

    # Union all emails within the same account
    for account in accounts:
        first_email_id = email_to_id[account[1]]
        for email in account[2:]:
            dsu.union(first_email_id, email_to_id[email])

    # Group emails by their root representative
    from collections import defaultdict
    groups = defaultdict(list)
    for email, eid in email_to_id.items():
        root = dsu.find(eid)
        groups[root].append(email)

    result = []
    for root, emails in groups.items():
        # Find account name from any email in this group
        name = email_to_name[emails[0]]
        result.append([name] + sorted(emails))

    return result
```
**Time**: O(N × α(N)) where N = total emails | **Space**: O(N)

---

### Problem 3: Redundant Connection (LC 684) — Find Cycle Edge

```python
def find_redundant_connection(edges: List[List[int]]) -> List[int]:
    """
    In a tree with one extra edge, find the extra edge (last one creating cycle).
    DSU: if both endpoints are already in same component, this edge is redundant.
    """
    n   = len(edges)
    dsu = DSU(n + 1)  # Nodes are 1-indexed

    for u, v in edges:
        if not dsu.union(u, v):   # Returns False if already connected
            return [u, v]

    return []
```
**Time**: O(N × α(N)) | **Space**: O(N)

---

### Problem 4: Redundant Connection II (LC 685) — Directed Graph

```python
def find_redundant_directed_connection(edges: List[List[int]]) -> List[int]:
    """
    Directed graph (rooted tree + one extra edge). Find the extra edge.
    
    Two cases:
    1. A node has two parents (in-degree 2). Remove the second parent edge.
    2. There's a cycle (no node with in-degree 2). Standard DSU cycle detection.
    
    When in-degree-2 node exists, try removing candidate edges and check validity.
    """
    n = len(edges)
    parent = [0] * (n+1)   # parent[v] = u if edge u→v exists

    # Find node with two parents (in-degree 2)
    cand1 = cand2 = None
    for u, v in edges:
        if parent[v] == 0:
            parent[v] = u
        else:
            # v has two incoming edges
            cand1 = [parent[v], v]   # First edge to v
            cand2 = [u, v]           # Second edge to v (current)

    dsu = DSU(n+1)

    for u, v in edges:
        if [u, v] == cand2:
            continue   # Skip the second candidate edge
        if not dsu.union(u, v):
            # Cycle detected
            if cand1 is None:
                return [u, v]   # No in-degree-2 node; this cycle edge is answer
            else:
                return cand1    # In-degree-2 node exists; remove first edge

    return cand2   # No cycle when skipping cand2 → cand2 is the answer
```
**Time**: O(N × α(N)) | **Space**: O(N)

---

### Problem 5: Most Stones Removed with Same Row or Column (LC 947)

```python
def remove_stones(stones: List[List[int]]) -> int:
    """
    Stones can be removed if there's another stone in same row or column.
    A stone can be removed iff it's in a connected component with >1 stone.
    Max removals = total stones - number of connected components.
    
    Virtual node trick: node for row r = r, node for col c = c + 10001.
    Union all stones' rows with their columns.
    """
    dsu = DSU(20002)   # rows [0..9999], cols [10001..19999]

    for r, c in stones:
        dsu.union(r, c + 10001)

    # Count distinct connected components that contain at least one stone
    roots = {dsu.find(r) for r, c in stones} | {dsu.find(c + 10001) for r, c in stones}

    # Actually count components by stone rows/cols
    stone_nodes = set()
    for r, c in stones:
        stone_nodes.add(r)
        stone_nodes.add(c + 10001)

    components = len({dsu.find(x) for x in stone_nodes})
    return len(stones) - components
```
**Time**: O(N × α(N)) | **Space**: O(N)

---

### Problem 6: Satisfiability of Equality Equations (LC 990)

```python
def equations_possible(equations: List[str]) -> bool:
    """
    Equations: "a==b" or "a!=b" (single lowercase letters).
    Check if all constraints can be satisfied simultaneously.
    
    Strategy:
    1. Process all '==' equations first (union endpoints).
    2. Check all '!=' equations: if both endpoints in same component → contradiction.
    """
    dsu = DSU(26)  # 26 letters

    for eq in equations:
        if eq[1] == '=':   # '=='
            dsu.union(ord(eq[0])-ord('a'), ord(eq[3])-ord('a'))

    for eq in equations:
        if eq[1] == '!':   # '!='
            if dsu.connected(ord(eq[0])-ord('a'), ord(eq[3])-ord('a')):
                return False

    return True
```
**Time**: O(N × α(26)) = O(N) | **Space**: O(1) (26 letters)

---

### Problem 7: Making a Large Island (LC 827) — DSU for Connected Regions

```python
def largest_island(grid: List[List[int]]) -> int:
    """
    Can flip exactly one 0 to 1. Find maximum island size after flip.
    
    Strategy:
    1. DSU to find all existing islands and their sizes.
    2. For each 0 cell, check 4-directional islands and sum unique component sizes + 1.
    3. Return maximum.
    """
    n   = len(grid)
    dsu = DSU(n * n)

    # Step 1: Build DSU for existing 1s
    for r in range(n):
        for c in range(n):
            if grid[r][c] == 1:
                for dr, dc in [(0,1),(1,0)]:
                    nr, nc = r+dr, c+dc
                    if 0 <= nr < n and 0 <= nc < n and grid[nr][nc] == 1:
                        dsu.union(r*n+c, nr*n+nc)

    # Step 2: Try flipping each 0
    max_size = max((dsu.get_size(r*n+c) for r in range(n) for c in range(n)
                    if grid[r][c] == 1), default=0)

    for r in range(n):
        for c in range(n):
            if grid[r][c] == 0:
                seen_roots = set()
                size = 1   # The flipped cell itself
                for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
                    nr, nc = r+dr, c+dc
                    if 0 <= nr < n and 0 <= nc < n and grid[nr][nc] == 1:
                        root = dsu.find(nr*n+nc)
                        if root not in seen_roots:
                            seen_roots.add(root)
                            size += dsu.get_size(nr*n+nc)
                max_size = max(max_size, size)

    return max_size
```
**Time**: O(n² × α(n²)) | **Space**: O(n²)

---

## 7. Interview Tips and Edge Cases

### When to Use DSU vs BFS/DFS for Connectivity

| Criteria | Prefer DSU | Prefer BFS/DFS |
|---|---|---|
| Dynamic edge additions (online) | DSU | — |
| Static graph, single query | — | BFS/DFS |
| Multiple connectivity queries | DSU | — |
| Need rollback (edge deletion) | DSU with rollback | — |
| Path between nodes | — | BFS/DFS |
| Cycle detection (undirected) | DSU | BFS/DFS |
| Cycle detection (directed) | — | DFS (3-coloring) |

### Common DSU Mistakes

| Mistake | Fix |
|---|---|
| Forgetting path compression | Always compress in find() |
| Union without rank → O(N) per op | Always use rank or size |
| Path compression + rollback | Use rank-only DSU for rollback |
| 1-indexed nodes | Initialize DSU with n+1 |
| Virtual nodes need extended array | DSU size = actual nodes + virtual |
| Checking connectivity before union | find(x)==find(y) before union |

### The Virtual Node Pattern
Whenever you need to connect "all elements sharing property X":
- Create a virtual node for property X
- Union every element having property X with the virtual node
- Checking if two elements share property X = check if they're in the same component

This avoids O(K²) unions for K elements with same property → O(K) unions instead.

### α(N) — The Inverse Ackermann Function

For interview purposes: **O(α(N)) = O(1) practically**. But on paper, say "amortized O(α(N)) per operation with union by rank and path compression."

---

*Next: [07_Minimum_Spanning_Tree.md](07_Minimum_Spanning_Tree.md) — Kruskal's, Prim's, cut property proof, MST applications*

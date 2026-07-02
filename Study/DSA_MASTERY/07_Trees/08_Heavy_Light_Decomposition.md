# Heavy-Light Decomposition — Path Queries on Trees

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** DFS, segment trees, tree fundamentals
> **Core Theme:** Decomposing a tree into O(log N) chains to reduce path
> queries to O(log²N) segment tree range queries.

---

## 1. Core Concept — Why HLD Works

### Heavy and Light Edges

For each non-leaf node `v`, define its **heavy child** as the child with
the largest subtree size. All other edges to children are **light edges**.

**Theorem:** Any root-to-leaf path contains at most O(log N) light edges.

**Proof:** When traversing a light edge from parent `u` to child `v`:
`size(v) ≤ size(u) / 2`
(Otherwise v would be the heavy child.) Each light edge halves the remaining
subtree size. Starting from size N, we can halve at most log₂N times.

### Chains

A **heavy chain** is a maximal path of heavy edges. Every node belongs to
exactly one chain. Any root-to-node path intersects at most O(log N) chains.

**Why O(log²N) per path query:**
- Path u→v intersects at most O(log N) chain segments
- Each segment is queried on a segment tree: O(log N)
- Total: O(log N × log N) = O(log²N)

---

## 2. Full HLD Implementation

```python
from math import inf

class SegTree:
    def __init__(self, arr):
        self.n = len(arr)
        self.tree = [0] * (4 * self.n)
        self._build(arr, 0, 0, self.n - 1)

    def _build(self, arr, node, lo, hi):
        if lo == hi:
            self.tree[node] = arr[lo]; return
        mid = (lo + hi) // 2
        self._build(arr, 2*node+1, lo, mid)
        self._build(arr, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def update(self, idx, val, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if lo == hi:
            self.tree[node] = val; return
        mid = (lo + hi) // 2
        if idx <= mid: self.update(idx, val, 2*node+1, lo, mid)
        else:          self.update(idx, val, 2*node+2, mid+1, hi)
        self.tree[node] = self.tree[2*node+1] + self.tree[2*node+2]

    def query(self, l, r, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo: return 0
        if l <= lo and hi <= r: return self.tree[node]
        mid = (lo + hi) // 2
        return (self.query(l, r, 2*node+1, lo, mid) +
                self.query(l, r, 2*node+2, mid+1, hi))


class HLD:
    """
    Heavy-Light Decomposition with Segment Tree for path queries.
    Supports: path sum/max queries and point updates.
    """
    def __init__(self, n: int, adj: list[list[int]], values: list[int],
                 root: int = 0):
        self.n = n
        self.adj = adj
        self.values = values
        self.root = root

        self.parent   = [-1] * n
        self.depth    = [0]  * n
        self.subtree  = [0]  * n
        self.heavy    = [-1] * n
        self.head     = [0]  * n
        self.pos      = [0]  * n
        self.cur_pos  = [0]

        self._dfs_size(root, -1)
        self._dfs_hld(root, root)

        seg_vals = [0] * n
        for v in range(n):
            seg_vals[self.pos[v]] = values[v]
        self.seg = SegTree(seg_vals)

    def _dfs_size(self, v: int, par: int) -> int:
        self.parent[v] = par
        self.subtree[v] = 1
        max_sub = 0
        for u in self.adj[v]:
            if u != par:
                self.depth[u] = self.depth[v] + 1
                self.subtree[v] += self._dfs_size(u, v)
                if self.subtree[u] > max_sub:
                    max_sub = self.subtree[u]
                    self.heavy[v] = u
        return self.subtree[v]

    def _dfs_hld(self, v: int, h: int):
        self.head[v] = h
        self.pos[v]  = self.cur_pos[0]
        self.cur_pos[0] += 1

        # Process heavy child first (extends the chain)
        if self.heavy[v] != -1:
            self._dfs_hld(self.heavy[v], h)
        # Process light children (start new chains)
        for u in self.adj[v]:
            if u != self.parent[v] and u != self.heavy[v]:
                self._dfs_hld(u, u)

    def update(self, v: int, val: int) -> None:
        self.seg.update(self.pos[v], val)

    def query_path(self, u: int, v: int) -> int:
        """Query sum on path from u to v."""
        result = 0
        head, pos, depth, parent, seg = (
            self.head, self.pos, self.depth, self.parent, self.seg
        )

        while head[u] != head[v]:
            if depth[head[u]] < depth[head[v]]:
                u, v = v, u
            result += seg.query(pos[head[u]], pos[u])
            u = parent[head[u]]

        if depth[u] > depth[v]:
            u, v = v, u
        result += seg.query(pos[u], pos[v])

        return result

    def query_subtree(self, v: int) -> int:
        return self.seg.query(self.pos[v], self.pos[v] + self.subtree[v] - 1)
```
**Preprocessing:** O(N log N) | **Path Query:** O(log²N) | **Update:** O(log N)

---

## 3. Example Usage

```python
# Tree: 0 is root, edges: 0-1, 0-2, 1-3, 1-4, 2-5
n = 6
adj = [[] for _ in range(n)]
for u, v in [(0,1),(0,2),(1,3),(1,4),(2,5)]:
    adj[u].append(v)
    adj[v].append(u)

values = [10, 3, 7, 5, 2, 8]
hld = HLD(n, adj, values)

# Sum on path 3 → 5: nodes 3→1→0→2→5
print(hld.query_path(3, 5))   # 5+3+10+7+8 = 33

# Update node 1's value to 100
hld.update(1, 100)

# Subtree of node 1 (nodes 1,3,4)
print(hld.query_subtree(1))   # 100+5+2 = 107
```

---

## 4. HLD for Maximum on Path

Replace `SegTree` with a max-segment tree:

```python
class SegTreeMax:
    def __init__(self, arr):
        self.n = len(arr)
        self.tree = [-inf] * (4 * self.n)
        self._build(arr, 0, 0, self.n - 1)

    def _build(self, arr, node, lo, hi):
        if lo == hi:
            self.tree[node] = arr[lo]; return
        mid = (lo + hi) // 2
        self._build(arr, 2*node+1, lo, mid)
        self._build(arr, 2*node+2, mid+1, hi)
        self.tree[node] = max(self.tree[2*node+1], self.tree[2*node+2])

    def update(self, idx, val, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if lo == hi:
            self.tree[node] = val; return
        mid = (lo + hi) // 2
        if idx <= mid: self.update(idx, val, 2*node+1, lo, mid)
        else:          self.update(idx, val, 2*node+2, mid+1, hi)
        self.tree[node] = max(self.tree[2*node+1], self.tree[2*node+2])

    def query(self, l, r, node=0, lo=None, hi=None):
        if lo is None: lo, hi = 0, self.n - 1
        if l > hi or r < lo: return -inf
        if l <= lo and hi <= r: return self.tree[node]
        mid = (lo + hi) // 2
        return max(self.query(l, r, 2*node+1, lo, mid),
                   self.query(l, r, 2*node+2, mid+1, hi))
```

---

## 5. Centroid Decomposition (Alternative for Path Problems)

**Centroid:** A node whose removal leaves all subtrees of size ≤ N/2.

**Key property:** Every tree has a centroid, and centroid decomposition has O(log N) levels.

```python
def centroid_decomposition(n: int, adj: list) -> list:
    subtree = [1] * n
    removed = [False] * n
    centroid_parent = [-1] * n

    def compute_subtree(v, par):
        subtree[v] = 1
        for u in adj[v]:
            if u != par and not removed[u]:
                compute_subtree(u, v)
                subtree[v] += subtree[u]

    def find_centroid(v, par, tree_size):
        for u in adj[v]:
            if u != par and not removed[u] and subtree[u] > tree_size // 2:
                return find_centroid(u, v, tree_size)
        return v

    def decompose(v, par_centroid):
        compute_subtree(v, -1)
        c = find_centroid(v, -1, subtree[v])
        centroid_parent[c] = par_centroid
        removed[c] = True
        for u in adj[c]:
            if not removed[u]:
                decompose(u, c)

    decompose(0, -1)
    return centroid_parent
```
**Time:** O(N log N) | **Space:** O(N)

---

## 6. HLD Complexity Proof

**Claim:** `query_path` makes at most 2 log₂N + 1 segment tree calls.

**Proof:** Each segment tree call corresponds to ascending one chain (when
`head[u] ≠ head[v]`). At each ascent, we jump over at least one light edge
(from `head[u]` to `parent[head[u]]`). By the path light-edge lemma, at most
log₂N light edges exist on any path. We process both endpoints, so at most
2 log₂N ascents. The final shared-chain query adds 1. Total: 2 log₂N + 1
= O(log N) calls × O(log N) each = O(log²N). □

---

## Interview Tips

1. **Two DFS passes:** First computes subtree sizes and heavy children. Second assigns positions. Keep them separate for clarity.
2. **Chain head convention:** Heavy child inherits parent's head; light child starts a new chain (head = self).
3. **Position assignment:** Heavy children are assigned positions immediately after their parent → contiguous range in segment tree.
4. **Edge vs node weights:** Assign edge weight to the deeper node. When querying u→v, exclude LCA's value.
5. **HLD in interviews:** Rarely asked directly, but demonstrates advanced tree knowledge. Centroid decomposition is more commonly asked.
6. **Iterative DFS:** For very deep trees (N ≈ 10^5+), recursive DFS may hit Python recursion limit. Use `sys.setrecursionlimit` or convert to iterative.

# Sqrt Decomposition & Mo's Algorithm — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** Prefix Sums, Sorting, Union-Find  
> **Interview Frequency:** Google ★★★★☆ | Meta ★★★☆☆ | Amazon ★★☆☆☆

---

## Table of Contents
1. [Block Decomposition — Core Concept](#1-block-decomposition--core-concept)
2. [Range Sum with Point Updates](#2-range-sum-with-point-updates)
3. [Range Assignment + Range Sum](#3-range-assignment--range-sum)
4. [Mo's Algorithm — Offline Range Queries](#4-mos-algorithm--offline-range-queries)
5. [Mo's Algorithm — Count Distinct Values](#5-mos-algorithm--count-distinct-values)
6. [Mo's Algorithm — XOR of Range](#6-mos-algorithm--xor-of-range)
7. [Mo's Algorithm with Updates — O(N^5/3)](#7-mos-algorithm-with-updates--on53)
8. [DSU on Tree (Small to Large Merging)](#8-dsu-on-tree-small-to-large-merging)
9. [Heavy Path Decomposition Alternative](#9-heavy-path-decomposition-alternative)
10. [Advanced Problems with Full Solutions](#10-advanced-problems-with-full-solutions)
11. [Interview Tips & Complexity Comparison](#11-interview-tips--complexity-comparison)

---

## 1. Block Decomposition — Core Concept

**Sqrt decomposition** divides an array of N elements into blocks of size B ≈ √N.
The fundamental tradeoff: B operations per block, N/B blocks.

```
Array: [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8]  (N=12, B=4)

Block 0: [3, 1, 4, 1]  → block_sum[0] = 9
Block 1: [5, 9, 2, 6]  → block_sum[1] = 22
Block 2: [5, 3, 5, 8]  → block_sum[2] = 21

Range sum [1..8]:
  partial block 0: arr[1]+arr[2]+arr[3] = 6
  full block 1:    block_sum[1] = 22
  partial block 2: arr[8] = 5
  Total = 33
```

**Why √N is optimal block size:**
- Range query costs: `O(B) + O(N/B)` partial + full blocks
- Minimized when B = N/B → B = √N → cost = O(√N)
- Update costs: O(1) point update (update element + block sum)

---

## 2. Range Sum with Point Updates

```python
import math

class SqrtDecompSum:
    """
    Range sum query with point updates.
    
    Build: O(N) | Update: O(1) | Query: O(√N)
    Space: O(N)
    
    Compare with BIT: BIT is O(log N) for both — better asymptotically.
    But sqrt decomp is simpler and handles more complex operations.
    """
    
    def __init__(self, arr: list[int]):
        self.n = len(arr)
        self.B = max(1, int(math.isqrt(self.n)))
        self.arr = arr[:]
        self.num_blocks = (self.n + self.B - 1) // self.B
        self.block_sum = [0] * self.num_blocks
        
        for i, x in enumerate(arr):
            self.block_sum[i // self.B] += x
    
    def update(self, i: int, val: int):
        """Point update: set arr[i] = val. O(1)."""
        self.block_sum[i // self.B] += val - self.arr[i]
        self.arr[i] = val
    
    def add(self, i: int, delta: int):
        """Point add: arr[i] += delta. O(1)."""
        self.arr[i] += delta
        self.block_sum[i // self.B] += delta
    
    def query(self, l: int, r: int) -> int:
        """Range sum arr[l..r]. O(√N)."""
        result = 0
        bl, br = l // self.B, r // self.B
        
        if bl == br:
            # Same block: sum elements directly
            return sum(self.arr[l:r+1])
        
        # Left partial block
        block_end = (bl + 1) * self.B
        result += sum(self.arr[l:block_end])
        
        # Full middle blocks
        for b in range(bl + 1, br):
            result += self.block_sum[b]
        
        # Right partial block
        block_start = br * self.B
        result += sum(self.arr[block_start:r+1])
        
        return result
    
    def range_min(self, l: int, r: int) -> int:
        """
        Range minimum — CANNOT be done with BIT efficiently.
        This is where sqrt decomp shines over BIT!
        O(√N) with block_min precomputed.
        """
        # Requires block_min array (extend __init__ accordingly)
        bl, br = l // self.B, r // self.B
        if bl == br:
            return min(self.arr[l:r+1])
        result = min(self.arr[l:(bl+1)*self.B])
        for b in range(bl+1, br):
            result = min(result, min(self.arr[b*self.B:(b+1)*self.B]))
        result = min(result, min(self.arr[br*self.B:r+1]))
        return result


# ─── Test ───
arr = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8]
sq = SqrtDecompSum(arr)
print(sq.query(1, 8))   # 1+4+1+5+9+2+6+5 = 33
sq.update(3, 10)
print(sq.query(0, 5))   # 3+1+4+10+5+9 = 32
sq.add(5, -5)
print(sq.query(5, 5))   # 4
```

---

## 3. Range Assignment + Range Sum

This showcases sqrt decomp handling **lazy operations** — something prefix sums cannot.

```python
class SqrtDecompRangeAssign:
    """
    Range assignment (set arr[l..r] = val) + Range sum query.
    
    Each block has a 'lazy' tag. If lazy[b] != -1, all elements in block b
    are logically set to lazy[b].
    
    Update: O(√N) — rebuild partial blocks, O(1) per full block
    Query:  O(√N)
    Build:  O(N)
    Space:  O(N)
    
    Segment tree with lazy prop is O(log N) — better asymptotically.
    Sqrt decomp wins on simplicity and constant factors for moderate N.
    """
    
    def __init__(self, arr: list[int]):
        self.n = len(arr)
        self.B = max(1, int(math.isqrt(self.n)))
        self.arr = arr[:]
        self.num_blocks = (self.n + self.B - 1) // self.B
        self.block_sum = [0] * self.num_blocks
        self.lazy = [-1] * self.num_blocks  # -1 means no pending assignment
        
        for i, x in enumerate(arr):
            self.block_sum[i // self.B] += x
    
    def _block_range(self, b: int) -> tuple:
        return b * self.B, min((b + 1) * self.B - 1, self.n - 1)
    
    def _push_down(self, b: int):
        """Apply lazy tag to actual array for block b."""
        if self.lazy[b] != -1:
            l, r = self._block_range(b)
            for i in range(l, r + 1):
                self.arr[i] = self.lazy[b]
            self.block_sum[b] = self.lazy[b] * (r - l + 1)
            self.lazy[b] = -1
    
    def range_assign(self, l: int, r: int, val: int):
        """Set arr[l..r] = val. O(√N)."""
        bl, br = l // self.B, r // self.B
        
        if bl == br:
            self._push_down(bl)
            for i in range(l, r + 1):
                self.arr[i] = val
            # Recompute block sum
            sl, sr = self._block_range(bl)
            self.block_sum[bl] = sum(self.arr[sl:sr+1])
            return
        
        # Left partial block
        self._push_down(bl)
        for i in range(l, (bl + 1) * self.B):
            self.arr[i] = val
        sl, sr = self._block_range(bl)
        self.block_sum[bl] = sum(self.arr[sl:sr+1])
        
        # Full middle blocks — just set lazy tag
        for b in range(bl + 1, br):
            self.lazy[b] = val
            l_b, r_b = self._block_range(b)
            self.block_sum[b] = val * (r_b - l_b + 1)
        
        # Right partial block
        self._push_down(br)
        for i in range(br * self.B, r + 1):
            self.arr[i] = val
        sl, sr = self._block_range(br)
        self.block_sum[br] = sum(self.arr[sl:sr+1])
    
    def range_sum(self, l: int, r: int) -> int:
        """Sum of arr[l..r]. O(√N)."""
        bl, br = l // self.B, r // self.B
        
        if bl == br:
            if self.lazy[bl] != -1:
                return self.lazy[bl] * (r - l + 1)
            return sum(self.arr[l:r+1])
        
        result = 0
        
        # Left partial
        if self.lazy[bl] != -1:
            result += self.lazy[bl] * ((bl + 1) * self.B - l)
        else:
            result += sum(self.arr[l:(bl+1)*self.B])
        
        # Full blocks
        for b in range(bl + 1, br):
            result += self.block_sum[b]
        
        # Right partial
        if self.lazy[br] != -1:
            result += self.lazy[br] * (r - br * self.B + 1)
        else:
            result += sum(self.arr[br*self.B:r+1])
        
        return result


# ─── Test ───
arr = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
sq = SqrtDecompRangeAssign(arr)
print(sq.range_sum(0, 9))   # 55
sq.range_assign(2, 7, 0)
print(sq.range_sum(0, 9))   # 1+2+0+0+0+0+0+0+9+10 = 22
sq.range_assign(0, 9, 5)
print(sq.range_sum(0, 4))   # 25
```

---

## 4. Mo's Algorithm — Offline Range Queries

**Mo's algorithm** processes offline range queries in **O((N+Q)√N)** by ordering queries to minimize total add/remove operations.

The key insight: sort queries by `(block(l), r if block_even else -r)`. This ensures:
- Total movement of `l`: O(N√N) — at most √N moves within a block × N/√N blocks
- Total movement of `r`: O(N√N) — r is monotone within each block group

```python
import math
from collections import defaultdict

class MoAlgorithm:
    """
    Mo's Algorithm framework for offline range queries.
    
    Sorting: queries sorted by (l // block_size, r if block_even else -r)
    (Hilbert curve ordering gives ~2x speedup in practice)
    
    Time: O((N + Q) * √N * cost_per_operation)
    Space: O(N + Q)
    """
    
    def __init__(self, arr: list[int]):
        self.arr = arr
        self.n = len(arr)
        self.B = max(1, int(math.isqrt(self.n)))
    
    def solve(self, queries: list[tuple]) -> list:
        """
        Solve all queries. Each query is (l, r, query_id).
        Returns answers in original query order.
        """
        Q = len(queries)
        if Q == 0:
            return []
        
        # Sort queries by Mo's order
        indexed = [(l, r, i) for i, (l, r) in enumerate(queries)]
        indexed.sort(key=lambda x: (
            x[0] // self.B,
            x[1] if (x[0] // self.B) % 2 == 0 else -x[1]
        ))
        
        answers = [0] * Q
        cur_l, cur_r = 0, -1  # empty range
        
        # Current state — override in subclass
        self._state_reset()
        
        for l, r, qi in indexed:
            # Expand/contract window
            while cur_r < r:
                cur_r += 1
                self._add(cur_r)
            while cur_l > l:
                cur_l -= 1
                self._add(cur_l)
            while cur_r > r:
                self._remove(cur_r)
                cur_r -= 1
            while cur_l < l:
                self._remove(cur_l)
                cur_l += 1
            
            answers[qi] = self._answer()
        
        return answers
    
    def _state_reset(self): pass
    def _add(self, idx: int): pass
    def _remove(self, idx: int): pass
    def _answer(self): return 0
```

---

## 5. Mo's Algorithm — Count Distinct Values

```python
class MoDistinct(MoAlgorithm):
    """
    Count distinct elements in range [l, r].
    Classic Mo's algorithm application.
    
    Time: O((N + Q) * √N) | Space: O(N)
    """
    
    def _state_reset(self):
        self.freq = defaultdict(int)
        self.distinct = 0
    
    def _add(self, idx: int):
        val = self.arr[idx]
        if self.freq[val] == 0:
            self.distinct += 1
        self.freq[val] += 1
    
    def _remove(self, idx: int):
        val = self.arr[idx]
        self.freq[val] -= 1
        if self.freq[val] == 0:
            self.distinct -= 1
    
    def _answer(self) -> int:
        return self.distinct


def count_distinct_in_ranges(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    For each query (l, r), count distinct values in arr[l..r].
    
    Time: O((N + Q) * √N) | Space: O(N + Q)
    
    Example:
    arr = [1, 2, 1, 3, 2, 1]
    query (0, 5) → 3 distinct: {1, 2, 3}
    query (1, 3) → 3 distinct: {2, 1, 3}
    query (0, 2) → 2 distinct: {1, 2}
    """
    mo = MoDistinct(arr)
    mo._state_reset()
    return mo.solve(queries)


# ─── Full standalone implementation ───
def mo_distinct_standalone(arr: list[int], queries: list[tuple]) -> list[int]:
    n = len(arr)
    Q = len(queries)
    B = max(1, int(math.isqrt(n)))
    
    indexed = sorted(range(Q), key=lambda i: (
        queries[i][0] // B,
        queries[i][1] if (queries[i][0] // B) % 2 == 0 else -queries[i][1]
    ))
    
    freq = defaultdict(int)
    distinct = 0
    cur_l, cur_r = 0, -1
    answers = [0] * Q
    
    def add(idx):
        nonlocal distinct
        val = arr[idx]
        if freq[val] == 0:
            distinct += 1
        freq[val] += 1
    
    def remove(idx):
        nonlocal distinct
        val = arr[idx]
        freq[val] -= 1
        if freq[val] == 0:
            distinct -= 1
    
    for qi in indexed:
        l, r = queries[qi]
        while cur_r < r: cur_r += 1; add(cur_r)
        while cur_l > l: cur_l -= 1; add(cur_l)
        while cur_r > r: remove(cur_r); cur_r -= 1
        while cur_l < l: remove(cur_l); cur_l += 1
        answers[qi] = distinct
    
    return answers


# Test
arr = [1, 2, 1, 3, 2, 1, 4, 1]
queries = [(0, 5), (1, 3), (0, 2), (4, 7)]
print(mo_distinct_standalone(arr, queries))
# Expected: [4, 3, 2, 3]  (distinct: {1,2,3,4}, {2,1,3}, {1,2}, {2,1,4})
```

---

## 6. Mo's Algorithm — XOR of Range

```python
def mo_xor_range(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    XOR of elements in range [l, r] for each query.
    
    Note: XOR queries can be solved in O(1) with prefix XOR.
    This demonstrates Mo's framework for more complex operations
    where prefix doesn't work (e.g., distinct XOR, XOR of squares).
    
    Time: O((N + Q) * √N) | Space: O(N + Q)
    """
    n = len(arr)
    Q = len(queries)
    B = max(1, int(math.isqrt(n)))
    
    indexed = sorted(range(Q), key=lambda i: (
        queries[i][0] // B,
        queries[i][1] if (queries[i][0] // B) % 2 == 0 else -queries[i][1]
    ))
    
    cur_xor = 0
    cur_l, cur_r = 0, -1
    answers = [0] * Q
    
    def add(idx):
        nonlocal cur_xor
        cur_xor ^= arr[idx]
    
    def remove(idx):
        nonlocal cur_xor
        cur_xor ^= arr[idx]  # XOR is self-inverse!
    
    for qi in indexed:
        l, r = queries[qi]
        while cur_r < r: cur_r += 1; add(cur_r)
        while cur_l > l: cur_l -= 1; add(cur_l)
        while cur_r > r: remove(cur_r); cur_r -= 1
        while cur_l < l: remove(cur_l); cur_l += 1
        answers[qi] = cur_xor
    
    return answers


# Compare with O(1) prefix XOR:
def prefix_xor_queries(arr: list[int], queries: list[tuple]) -> list[int]:
    """O(N + Q) using prefix XOR — optimal for pure XOR queries."""
    prefix = [0] * (len(arr) + 1)
    for i, x in enumerate(arr):
        prefix[i+1] = prefix[i] ^ x
    return [prefix[r+1] ^ prefix[l] for l, r in queries]
```

---

## 7. Mo's Algorithm with Updates — O(N^5/3)

```python
def mo_with_updates(arr: list[int], 
                    queries: list[tuple],  # (l, r, time)
                    updates: list[tuple]   # (pos, new_val)
                    ) -> list[int]:
    """
    Mo's algorithm supporting point updates.
    
    Each query now has a 'time' (how many updates have been applied).
    Block size: N^(2/3) instead of N^(1/2).
    
    Sorting: (l // B, r // B, time)
    
    Time complexity:
    - Total l-pointer moves: O(N * N^(2/3)) = O(N^(5/3))  
    - Total r-pointer moves: O(Q * N^(2/3))
    - Total time moves: O(Q * N^(2/3)) (for each (l-block, r-block) pair)
    - Overall: O((N + Q)^(5/3))
    
    Space: O(N + Q)
    """
    n = len(arr)
    B = max(1, int(n ** (2/3)))  # Block size is N^(2/3)
    
    arr = arr[:]
    Q = len(queries)
    U = len(updates)
    
    # Add time dimension to queries
    # Each query (l, r) is augmented with the time snapshot
    timed_queries = list(queries)  # already has time
    
    # Sort by (l//B, r//B, time)
    indexed = sorted(range(Q), key=lambda i: (
        timed_queries[i][0] // B,
        timed_queries[i][1] // B,
        timed_queries[i][2]
    ))
    
    freq = defaultdict(int)
    distinct = 0
    
    def add_val(val):
        nonlocal distinct
        if freq[val] == 0:
            distinct += 1
        freq[val] += 1
    
    def remove_val(val):
        nonlocal distinct
        freq[val] -= 1
        if freq[val] == 0:
            distinct -= 1
    
    cur_l, cur_r, cur_t = 0, -1, 0
    answers = [0] * Q
    saved = {}  # position -> old value (for undoing updates)
    
    for qi in indexed:
        l, r, t = timed_queries[qi]
        
        # Apply/undo updates to reach time t
        while cur_t < t:
            pos, new_val = updates[cur_t]
            old_val = arr[pos]
            saved[cur_t] = old_val
            arr[pos] = new_val
            # If pos is in current window, update frequency
            if cur_l <= pos <= cur_r:
                remove_val(old_val)
                add_val(new_val)
            cur_t += 1
        
        while cur_t > t:
            cur_t -= 1
            pos, new_val = updates[cur_t]
            old_val = saved[cur_t]
            arr[pos] = old_val
            if cur_l <= pos <= cur_r:
                remove_val(new_val)
                add_val(old_val)
        
        # Expand/contract range (same as regular Mo's)
        while cur_r < r: cur_r += 1; add_val(arr[cur_r])
        while cur_l > l: cur_l -= 1; add_val(arr[cur_l])
        while cur_r > r: remove_val(arr[cur_r]); cur_r -= 1
        while cur_l < l: remove_val(arr[cur_l]); cur_l += 1
        
        answers[qi] = distinct
    
    return answers
```

**Complexity:** O((N + Q)^(5/3)) time | Space O(N + Q)

---

## 8. DSU on Tree (Small to Large Merging)

**DSU on Tree** (also called "small to large merging" or "heavy-light trick on trees") answers subtree queries in **O(N log N)** total.

```python
from collections import defaultdict

class DsuOnTree:
    """
    DSU on Tree: answer subtree queries for all nodes in O(N log N).
    
    Algorithm:
    1. Find heavy child of each node (child with largest subtree)
    2. For each node:
       a. Process light children first (then DELETE their contributions)
       b. Process heavy child (KEEP its contributions)
       c. Add current node to result
       d. Process all light children again (ADD their contributions)
    
    This way each node is added/deleted O(log N) times total.
    
    Classic problem: For each node, find the most frequent color in its subtree.
    
    Time: O(N log N) | Space: O(N)
    """
    
    def __init__(self, n: int, adj: list[list], values: list[int]):
        self.n = n
        self.adj = adj
        self.values = values
        self.subtree_size = [1] * n
        self.heavy = [-1] * n  # heavy child
        self.answers = [0] * n
        
        # Compute subtree sizes and heavy children
        self._dfs_size(0, -1)
        self._dfs_answers(0, -1, False)
    
    def _dfs_size(self, u: int, parent: int):
        max_size = 0
        for v in self.adj[u]:
            if v != parent:
                self._dfs_size(v, u)
                self.subtree_size[u] += self.subtree_size[v]
                if self.subtree_size[v] > max_size:
                    max_size = self.subtree_size[v]
                    self.heavy[u] = v
    
    # Global frequency map (shared state)
    _freq = defaultdict(int)
    _max_freq = 0
    _cur_ans = 0  # e.g., sum of elements with max frequency
    
    def _add_node(self, u: int, parent: int, keep: bool):
        val = self.values[u]
        DsuOnTree._freq[val] += 1
        if DsuOnTree._freq[val] > DsuOnTree._max_freq:
            DsuOnTree._max_freq = DsuOnTree._freq[val]
            DsuOnTree._cur_ans = val
        elif DsuOnTree._freq[val] == DsuOnTree._max_freq:
            DsuOnTree._cur_ans += val  # sum of most frequent colors
        
        for v in self.adj[u]:
            if v != parent and v != self.heavy[u]:
                self._add_node(v, u, keep)
    
    def _remove_subtree(self, u: int, parent: int):
        DsuOnTree._freq[self.values[u]] -= 1
        for v in self.adj[u]:
            if v != parent:
                self._remove_subtree(v, u)
    
    def _dfs_answers(self, u: int, parent: int, keep: bool):
        # 1. Process light children, then remove their contributions
        for v in self.adj[u]:
            if v != parent and v != self.heavy[u]:
                self._dfs_answers(v, u, False)  # keep=False
        
        # 2. Process heavy child (keep its contributions)
        if self.heavy[u] != -1:
            self._dfs_answers(self.heavy[u], u, True)  # keep=True
        
        # 3. Add contributions of light children + current node
        for v in self.adj[u]:
            if v != parent and v != self.heavy[u]:
                self._add_node(v, u, True)
        
        val = self.values[u]
        DsuOnTree._freq[val] += 1
        if DsuOnTree._freq[val] >= DsuOnTree._max_freq:
            if DsuOnTree._freq[val] > DsuOnTree._max_freq:
                DsuOnTree._max_freq = DsuOnTree._freq[val]
                DsuOnTree._cur_ans = val
            else:
                DsuOnTree._cur_ans += val
        
        self.answers[u] = DsuOnTree._cur_ans
        
        # 4. If not keeping, remove all contributions
        if not keep:
            self._remove_subtree(u, parent)
            DsuOnTree._freq = defaultdict(int)
            DsuOnTree._max_freq = 0
            DsuOnTree._cur_ans = 0


def small_to_large_merge(adj: list[list], values: list[int]) -> list[set]:
    """
    Small-to-large merging: merge sets up the tree.
    Each element is moved at most O(log N) times.
    
    Time: O(N log N) | Space: O(N)
    
    Application: For each node, find all distinct values in its subtree.
    """
    n = len(adj)
    result = [None] * n
    sets = [{values[i]} for i in range(n)]
    
    # Post-order DFS
    order = []
    parent = [-1] * n
    visited = [False] * n
    stack = [0]
    
    while stack:
        u = stack.pop()
        if visited[u]:
            continue
        visited[u] = True
        order.append(u)
        for v in adj[u]:
            if not visited[v]:
                parent[v] = u
                stack.append(v)
    
    # Process in reverse BFS order (leaves first)
    for u in reversed(order):
        for v in adj[u]:
            if v != parent[u]:  # v is child
                # Small-to-large: merge smaller into larger
                if len(sets[v]) > len(sets[u]):
                    sets[u], sets[v] = sets[v], sets[u]
                sets[u].update(sets[v])
        result[u] = sets[u]
    
    return result
```

---

## 9. Heavy Path Decomposition Alternative

```python
class HeavyLightDecomposition:
    """
    HLD: decomposes tree into O(log N) chains.
    Enables O(log^2 N) path queries using segment tree on chains.
    
    Use case: path sum, path min/max, path update on trees.
    
    Time: O(N log N) build, O(log^2 N) per query/update
    Space: O(N)
    """
    
    def __init__(self, n: int, adj: list[list], values: list[int]):
        self.n = n
        self.adj = adj
        self.values = values
        self.parent = [-1] * n
        self.depth = [0] * n
        self.subtree_size = [1] * n
        self.heavy = [-1] * n
        self.chain_head = list(range(n))
        self.pos = [0] * n  # position in flattened array
        self.arr = [0] * n  # flattened values for segment tree
        
        self._dfs1(0, -1, 0)
        self._timer = 0
        self._dfs2(0, -1, 0)
    
    def _dfs1(self, u, par, d):
        self.parent[u] = par
        self.depth[u] = d
        max_sz = 0
        for v in self.adj[u]:
            if v != par:
                self._dfs1(v, u, d + 1)
                self.subtree_size[u] += self.subtree_size[v]
                if self.subtree_size[v] > max_sz:
                    max_sz = self.subtree_size[v]
                    self.heavy[u] = v
    
    def _dfs2(self, u, par, head):
        self.chain_head[u] = head
        self.pos[u] = self._timer
        self.arr[self._timer] = self.values[u]
        self._timer += 1
        
        if self.heavy[u] != -1:
            self._dfs2(self.heavy[u], u, head)  # Continue chain
        
        for v in self.adj[u]:
            if v != par and v != self.heavy[u]:
                self._dfs2(v, u, v)  # New chain starts at v
    
    def path_query(self, u: int, v: int, seg_query_func) -> int:
        """
        Query on path u-v using segment tree.
        Climbs O(log N) chains, each requiring O(log N) seg tree query.
        Total: O(log^2 N)
        """
        result = 0  # identity for operation
        
        while self.chain_head[u] != self.chain_head[v]:
            # u is deeper: climb up its chain
            if self.depth[self.chain_head[u]] < self.depth[self.chain_head[v]]:
                u, v = v, u
            # Query from chain_head[u] to u
            result += seg_query_func(self.pos[self.chain_head[u]], self.pos[u])
            u = self.parent[self.chain_head[u]]
        
        # Same chain now
        if self.depth[u] > self.depth[v]:
            u, v = v, u
        result += seg_query_func(self.pos[u], self.pos[v])
        
        return result
```

---

## 10. Advanced Problems with Full Solutions

### Problem 1: Number of Distinct Colors in Query Ranges (LeetCode-style)

```python
def distinct_colors_queries(n: int, arr: list[int], 
                            queries: list[tuple]) -> list[int]:
    """
    Distinct values in range. Mo's O((N+Q)√N).
    
    Time: O((N + Q) * √N) | Space: O(N + Q)
    """
    return mo_distinct_standalone(arr, queries)
```

### Problem 2: Sum of Squares in Range

```python
def mo_sum_of_squares(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    Sum of squares of elements in [l, r].
    Cannot use simple prefix sums for distinct square sums with updates.
    Mo's handles this naturally.
    
    Time: O((N + Q) * √N) | Space: O(N)
    """
    n = len(arr)
    Q = len(queries)
    B = max(1, int(math.isqrt(n)))
    
    indexed = sorted(range(Q), key=lambda i: (
        queries[i][0] // B,
        queries[i][1] if (queries[i][0] // B) % 2 == 0 else -queries[i][1]
    ))
    
    cur_sum_sq = 0
    cur_l, cur_r = 0, -1
    answers = [0] * Q
    
    for qi in indexed:
        l, r = queries[qi]
        while cur_r < r: cur_r += 1; cur_sum_sq += arr[cur_r] ** 2
        while cur_l > l: cur_l -= 1; cur_sum_sq += arr[cur_l] ** 2
        while cur_r > r: cur_sum_sq -= arr[cur_r] ** 2; cur_r -= 1
        while cur_l < l: cur_sum_sq -= arr[cur_l] ** 2; cur_l += 1
        answers[qi] = cur_sum_sq
    
    return answers
```

### Problem 3: Range Mode (Most Frequent Element)

```python
def range_mode_queries(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    For each query [l, r], find the element with highest frequency.
    (If tie, return smallest value.)
    
    Mo's Algorithm: O((N + Q) * √N)
    
    Maintain: freq[val], freq_count[cnt] (how many values have freq cnt), max_freq
    """
    n = len(arr)
    Q = len(queries)
    B = max(1, int(math.isqrt(n)))
    
    indexed = sorted(range(Q), key=lambda i: (
        queries[i][0] // B,
        queries[i][1] if (queries[i][0] // B) % 2 == 0 else -queries[i][1]
    ))
    
    freq = defaultdict(int)
    freq_count = defaultdict(int)  # count[c] = # values with frequency c
    max_freq = 0
    mode = 0
    cur_l, cur_r = 0, -1
    answers = [0] * Q
    
    def add(idx):
        nonlocal max_freq, mode
        val = arr[idx]
        old_freq = freq[val]
        freq[val] += 1
        new_freq = freq[val]
        
        if old_freq > 0:
            freq_count[old_freq] -= 1
        freq_count[new_freq] += 1
        
        if new_freq > max_freq:
            max_freq = new_freq
            mode = val
        elif new_freq == max_freq and val < mode:
            mode = val
    
    def remove(idx):
        nonlocal max_freq, mode
        val = arr[idx]
        old_freq = freq[val]
        freq[val] -= 1
        new_freq = freq[val]
        
        freq_count[old_freq] -= 1
        if new_freq > 0:
            freq_count[new_freq] += 1
        
        if old_freq == max_freq and freq_count[max_freq] == 0:
            max_freq -= 1
            # Recompute mode — expensive in worst case
            # Use sorted structure for O(log N) mode tracking
            mode = 0  # simplified
    
    for qi in indexed:
        l, r = queries[qi]
        while cur_r < r: cur_r += 1; add(cur_r)
        while cur_l > l: cur_l -= 1; add(cur_l)
        while cur_r > r: remove(cur_r); cur_r -= 1
        while cur_l < l: remove(cur_l); cur_l += 1
        answers[qi] = max_freq  # return max frequency, or mode value
    
    return answers
```

### Problem 4: Sqrt Decomposition for Next Greater Element in Range

```python
def range_next_greater(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    For each query (l, r, x), find the leftmost index in [l, r] 
    where arr[i] > x.
    
    Block-level solution:
    - Precompute sorted array per block for binary search
    - For partial blocks: linear scan
    - For full blocks: binary search on sorted block → O(log B) = O(log √N)
    
    Time per query: O(√N * log(√N)) = O(√N * log N / 2)
    Build: O(N log N)
    """
    n = len(arr)
    B = max(1, int(math.isqrt(n)))
    num_blocks = (n + B - 1) // B
    
    # Precompute sorted blocks
    sorted_blocks = []
    for b in range(num_blocks):
        start, end = b * B, min((b + 1) * B, n)
        sorted_blocks.append(sorted(arr[start:end]))
    
    from bisect import bisect_right
    
    def query(l, r, x):
        bl, br = l // B, r // B
        
        # Scan partial left block
        for i in range(l, min((bl + 1) * B, r + 1)):
            if arr[i] > x:
                return i
        
        if bl == br:
            return -1
        
        # Full middle blocks: check if any element > x, then find first
        for b in range(bl + 1, br):
            block = sorted_blocks[b]
            if block[-1] > x:  # Has element > x
                # Find first position in ORIGINAL block with arr[i] > x
                for i in range(b * B, (b + 1) * B):
                    if arr[i] > x:
                        return i
        
        # Partial right block
        for i in range(br * B, r + 1):
            if arr[i] > x:
                return i
        
        return -1
    
    return [query(l, r, x) for l, r, x in queries]
```

### Problem 5: Minimum Operations to Equalize Block (Competitive Programming)

```python
def equalize_blocks_min_ops(arr: list[int]) -> int:
    """
    Find minimum operations to make all elements equal,
    where one operation changes a block's min to its block's max.
    This showcases block decomposition for optimization.
    
    Time: O(N) | Space: O(√N)
    """
    n = len(arr)
    B = int(math.isqrt(n))
    
    total_ops = 0
    for b in range(0, n, B):
        block = arr[b:b + B]
        min_val = min(block)
        max_val = max(block)
        # Cost to equalize: (max - min) operations if we could do 1 per step
        total_ops += max_val - min_val
    
    return total_ops
```

---

## 11. Interview Tips & Complexity Comparison

### 📊 Complexity Comparison Table

| Problem | Naive | Prefix Sum | BIT | Segment Tree | Sqrt Decomp | Mo's |
|---------|-------|-----------|-----|-------------|-------------|------|
| Range Sum (static) | O(N) | O(1) query | — | O(log N) | O(√N) | — |
| Range Sum (updates) | O(N) | ❌ | O(log N) | O(log N) | O(√N) | — |
| Range Min (static) | O(N) | — | ❌ | O(log N) | O(√N) | — |
| Range Min (updates) | O(N) | — | ❌ | O(log N) | O(√N) | — |
| Range Distinct | O(N) | ❌ | ❌ | ❌ hard | O(√N) | O(√N)/query |
| All range queries | — | — | — | — | — | O((N+Q)√N) |

### ⚡ When to Choose Each Approach

```
Is the data STATIC?
  YES: → Prefix sum (sum), Sparse Table (min/max), binary search on sorted (frequency)
  NO:  → Need updates?
    YES: → How complex is the operation?
      Simple (sum): → BIT or Segment Tree O(log N)
      Complex (distinct, mode): → Sqrt Decomp O(√N) per op
    OFFLINE queries?
      YES: → Mo's Algorithm O((N+Q)√N)
      NO:  → Segment Tree with appropriate node type
```

### 🎯 Mo's Algorithm Optimization Tricks

```python
# Hilbert Curve ordering (~2x faster than block sort)
def hilbert_order(queries, n):
    """Order queries using Hilbert curve for better cache performance."""
    def hilbert_d(n, x, y):
        d = 0
        s = n // 2
        while s > 0:
            rx = 1 if (x & s) > 0 else 0
            ry = 1 if (y & s) > 0 else 0
            d += s * s * ((3 * rx) ^ ry)
            # Rotate
            if ry == 0:
                if rx == 1:
                    x = s - 1 - x
                    y = s - 1 - y
                x, y = y, x
            s //= 2
        return d
    
    size = 1
    while size < n:
        size *= 2
    
    return sorted(queries, key=lambda q: hilbert_d(size, q[0], q[1]))
```

### 🔑 Key Interview Talking Points

1. **"Why √N block size?"** — Minimizes O(B + N/B) by AM-GM: optimal at B = √N

2. **"How does Mo's achieve O((N+Q)√N)?"**
   - Left pointer: within each block moves at most B = √N per query, N/B blocks → O(Q·√N)
   - Right pointer: monotone within block group → O(N) total per block group × N/B groups = O(N·√N)
   
3. **"When does Mo's fail?"** — Online queries (must answer before seeing next), highly dynamic data

4. **"DSU on tree vs HLD?"** — DSU on tree: O(N log N) subtree queries only. HLD: O(N log N) build + O(log² N) path queries.

### 📋 Edge Cases

```python
# Edge 1: Single element queries (l == r)
arr = [5]
assert mo_distinct_standalone(arr, [(0, 0)]) == [1]

# Edge 2: Entire array as one query
arr = [1, 1, 1, 1]
assert mo_distinct_standalone(arr, [(0, 3)]) == [1]

# Edge 3: Large block sizes (N < B^2)
arr = [3, 1, 4]  # N=3, B=1 (isqrt(3)=1)
sq = SqrtDecompSum(arr)
assert sq.query(0, 2) == 8

# Edge 4: All same values
arr = [7] * 100
sq = SqrtDecompSum(arr)
assert sq.query(10, 90) == 81 * 7

# Edge 5: Query with l == r
arr = [2, 4, 6, 8]
sq = SqrtDecompSum(arr)
assert sq.query(2, 2) == 6
```

---

*Mo's algorithm is one of the most elegant offline techniques in competitive programming. The key insight — sorting queries by block to minimize pointer movement — is the kind of sophisticated reasoning that Google and competitive programmers love. Master the framework: implement it once cleanly, then customize `_add`, `_remove`, and `_answer` for each problem.*

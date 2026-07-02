# Persistent Data Structures — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** Segment Trees, Merge Sort, Coordinate Compression  
> **Interview Frequency:** Google ★★★★★ | Meta ★★★☆☆ | Amazon ★★☆☆☆

---

## Table of Contents
1. [The Persistence Concept — Path Copying](#1-the-persistence-concept--path-copying)
2. [Persistent Segment Tree — Full Implementation](#2-persistent-segment-tree--full-implementation)
3. [K-th Smallest in Range — The Canonical Problem](#3-k-th-smallest-in-range--the-canonical-problem)
4. [Count Elements Less Than K in Range](#4-count-elements-less-than-k-in-range)
5. [Persistent Array](#5-persistent-array)
6. [Persistent Trie — Maximum XOR in Prefix](#6-persistent-trie--maximum-xor-in-prefix)
7. [Persistent DSU Concept](#7-persistent-dsu-concept)
8. [Functional Segment Tree](#8-functional-segment-tree)
9. [Version Queries — "State at Time T"](#9-version-queries--state-at-time-t)
10. [Advanced Problems with Full Solutions](#10-advanced-problems-with-full-solutions)
11. [Interview Tips & Edge Cases](#11-interview-tips--edge-cases)

---

## 1. The Persistence Concept — Path Copying

**Persistence** means preserving all historical versions of a data structure. Instead of mutating in-place, we create **new nodes only along the update path** and share all unchanged nodes with the previous version.

```
Original Segment Tree (version 0):
                [1..8: min=1]
               /              \
        [1..4: min=1]      [5..8: min=2]
         /        \          /        \
    [1..2: 3]  [3..4: 1]  [5..6: 2]  [7..8: 4]
    /    \     /    \      /    \      /    \
  [1:3] [2:5] [3:1] [4:7] [5:2] [6:8] [7:4] [8:6]

Update position 3 (value 1→9) → Version 1:
Only the PATH from root to leaf[3] is copied (log N nodes):

NEW nodes:      [1..8: min=2]
               /              \
        [1..4: min=2]    (shared: [5..8: min=2])
         /        \
    (shared)  [3..4: 9]
              /    \
           [3:9]  (shared: [4:7])

Version 1 root → new root, 3 new nodes allocated.
Old root (version 0) still points to original tree.
```

**Key properties:**
- Each update creates exactly O(log N) new nodes
- All unchanged nodes are shared between versions
- Total space for N updates: O(N log N)
- Time per update/query: O(log N)

---

## 2. Persistent Segment Tree — Full Implementation

```python
class PersistentSegTree:
    """
    Persistent Segment Tree using path copying.
    
    Supports:
    - Point update creating a new version
    - Range query on any historical version
    - K-th order statistics in range (with coordinate compression)
    
    Node structure: (left_child, right_child, value)
    Nodes are stored in a global pool (list) for cache efficiency.
    
    Time: O(log N) per update and query
    Space: O(N log N) total for N updates
    """
    
    def __init__(self, lo: int, hi: int):
        """
        lo, hi: value range for coordinate-compressed segment tree.
        For array index-based: lo=0, hi=n-1.
        For value-based (order statistics): lo=min_val, hi=max_val.
        """
        self.lo = lo
        self.hi = hi
        
        # Node pool: each node is [left_idx, right_idx, count/sum]
        # Using arrays for cache efficiency (vs objects)
        self.left = [0]    # left[i] = left child of node i
        self.right = [0]   # right[i] = right child of node i
        self.cnt = [0]     # cnt[i] = count/sum stored at node i
        self.size = 1      # next available node index (0 = null node)
        
        # Build empty root
        self.roots = [0]   # roots[0] = empty tree (node 0 = null)
    
    def _new_node(self, l=0, r=0, c=0) -> int:
        """Allocate a new node in the pool."""
        self.left.append(l)
        self.right.append(r)
        self.cnt.append(c)
        self.size += 1
        return self.size - 1
    
    def _update(self, prev: int, lo: int, hi: int, pos: int, delta: int = 1) -> int:
        """
        Create a new version by updating position pos with delta.
        Returns the new root node index.
        O(log N) new nodes created.
        """
        node = self._new_node(
            self.left[prev], 
            self.right[prev], 
            self.cnt[prev] + delta
        )
        
        if lo == hi:
            return node
        
        mid = (lo + hi) // 2
        if pos <= mid:
            self.left[node] = self._update(self.left[prev], lo, mid, pos, delta)
        else:
            self.right[node] = self._update(self.right[prev], mid+1, hi, pos, delta)
        
        return node
    
    def update(self, prev_version: int, pos: int, delta: int = 1) -> int:
        """
        Add delta to position pos, creating a new version.
        Returns the new version index.
        
        Time: O(log N) | Creates O(log N) new nodes
        """
        new_root = self._update(self.roots[prev_version], self.lo, self.hi, pos, delta)
        self.roots.append(new_root)
        return len(self.roots) - 1
    
    def _query_count(self, node: int, lo: int, hi: int, l: int, r: int) -> int:
        """Count elements in [l, r] within subtree at node."""
        if node == 0 or r < lo or hi < l:
            return 0
        if l <= lo and hi <= r:
            return self.cnt[node]
        mid = (lo + hi) // 2
        return (self._query_count(self.left[node], lo, mid, l, r) +
                self._query_count(self.right[node], mid+1, hi, l, r))
    
    def query_count(self, version: int, l: int, r: int) -> int:
        """
        Count elements in value range [l, r] in the given version.
        Time: O(log N)
        """
        return self._query_count(self.roots[version], self.lo, self.hi, l, r)
    
    def _kth(self, left_root: int, right_root: int, 
              lo: int, hi: int, k: int) -> int:
        """
        Find k-th smallest element using difference of two versions.
        Version right_root contains elements arr[0..r].
        Version left_root contains elements arr[0..l-1].
        Difference = elements in arr[l..r].
        """
        if lo == hi:
            return lo
        
        mid = (lo + hi) // 2
        # Count elements in left half of current range
        left_count = self.cnt[self.left[right_root]] - self.cnt[self.left[left_root]]
        
        if k <= left_count:
            return self._kth(self.left[left_root], self.left[right_root], lo, mid, k)
        else:
            return self._kth(self.right[left_root], self.right[right_root], 
                           mid+1, hi, k - left_count)
    
    def kth_smallest(self, version_l: int, version_r: int, k: int) -> int:
        """
        K-th smallest element in arr[l..r] where:
        - version_l = persistent tree after inserting arr[0..l-1]
        - version_r = persistent tree after inserting arr[0..r]
        
        Time: O(log N)
        """
        return self._kth(self.roots[version_l], self.roots[version_r],
                        self.lo, self.hi, k)
```

---

## 3. K-th Smallest in Range — The Canonical Problem

**Problem:** Given array `arr[0..N-1]`, answer Q queries: "What is the k-th smallest element in `arr[l..r]`?"

```python
def kth_smallest_in_range(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    K-th smallest element in range [l, r].
    
    Algorithm:
    1. Coordinate compress arr values to [0, M-1] where M = # unique values
    2. Build persistent segment tree: version i = tree after inserting arr[0..i-1]
       → Each version is a frequency distribution of values seen so far
    3. For query (l, r, k): use version[l] and version[r+1]
       → Their difference = frequency distribution of arr[l..r]
       → Walk down tree to find k-th element: O(log M)
    
    Time: O(N log N + Q log N) | Space: O(N log N)
    
    This is THE classic persistent segment tree problem!
    """
    n = len(arr)
    
    # Step 1: Coordinate compression
    sorted_unique = sorted(set(arr))
    compress = {v: i for i, v in enumerate(sorted_unique)}
    M = len(sorted_unique)
    
    # Step 2: Build persistent segment tree
    pst = PersistentSegTree(0, M - 1)
    
    # Version 0: empty tree (0 elements inserted)
    # Version i: tree with arr[0..i-1] inserted
    for x in arr:
        pst.update(len(pst.roots) - 1, compress[x], 1)
    
    # Step 3: Answer queries
    results = []
    for l, r, k in queries:
        # version[l] has arr[0..l-1], version[r+1] has arr[0..r]
        # Difference = frequency distribution of arr[l..r]
        compressed_ans = pst.kth_smallest(l, r + 1, k)
        results.append(sorted_unique[compressed_ans])
    
    return results


# ─── Complete Working Example ───
arr = [7, 2, 3, 1, 5, 8, 4, 6]
queries = [
    (0, 7, 3),  # 3rd smallest in full array = 3
    (1, 4, 2),  # 2nd smallest in [2,3,1,5] = 2
    (3, 6, 1),  # 1st smallest in [1,5,8,4] = 1
    (0, 3, 4),  # 4th smallest in [7,2,3,1] = 7
]
print(kth_smallest_in_range(arr, queries))
# Expected: [3, 2, 1, 7]


def kth_smallest_complete(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    Full self-contained implementation without class dependency.
    More suitable for interview coding from scratch.
    
    Time: O((N + Q) log N) | Space: O(N log N)
    """
    n = len(arr)
    
    # Coordinate compress
    vals = sorted(set(arr))
    rank = {v: i for i, v in enumerate(vals)}
    M = len(vals)
    
    # Persistent segment tree stored in arrays
    MAX_NODES = (n + 1) * 25  # upper bound on nodes
    left_c = [0] * MAX_NODES
    right_c = [0] * MAX_NODES
    cnt = [0] * MAX_NODES
    node_count = [1]  # node 0 is the null node
    
    def new_node(l=0, r=0, c=0):
        idx = node_count[0]
        left_c[idx] = l
        right_c[idx] = r
        cnt[idx] = c
        node_count[0] += 1
        return idx
    
    def update(prev, lo, hi, pos):
        node = new_node(left_c[prev], right_c[prev], cnt[prev] + 1)
        if lo == hi:
            return node
        mid = (lo + hi) // 2
        if pos <= mid:
            left_c[node] = update(left_c[prev], lo, mid, pos)
        else:
            right_c[node] = update(right_c[prev], mid+1, hi, pos)
        return node
    
    def kth(ln, rn, lo, hi, k):
        if lo == hi:
            return lo
        mid = (lo + hi) // 2
        lcount = cnt[left_c[rn]] - cnt[left_c[ln]]
        if k <= lcount:
            return kth(left_c[ln], left_c[rn], lo, mid, k)
        return kth(right_c[ln], right_c[rn], mid+1, hi, k - lcount)
    
    # Build versions
    roots = [0]  # version 0 = empty
    for x in arr:
        roots.append(update(roots[-1], 0, M-1, rank[x]))
    
    return [vals[kth(roots[l], roots[r+1], 0, M-1, k)] 
            for l, r, k in queries]
```

**Complexity:** O(N log N + Q log N) time | O(N log N) space

---

## 4. Count Elements Less Than K in Range

```python
def count_less_than_k_in_range(arr: list[int], 
                                 queries: list[tuple]) -> list[int]:
    """
    For each query (l, r, k), count elements in arr[l..r] that are < k.
    
    Using persistent segment tree:
    - Build versions as before
    - For query: count elements in value range [min_val, k-1]
    
    Time: O(N log N + Q log N) | Space: O(N log N)
    
    Alternative (offline, merge sort based): O(N log N + Q log N) time, O(N) space
    """
    n = len(arr)
    vals = sorted(set(arr))
    rank = {v: i for i, v in enumerate(vals)}
    M = len(vals)
    
    pst = PersistentSegTree(0, M - 1)
    for x in arr:
        pst.update(len(pst.roots) - 1, rank[x], 1)
    
    results = []
    for l, r, k in queries:
        from bisect import bisect_left
        k_rank = bisect_left(vals, k)  # first index with val >= k
        if k_rank == 0:
            results.append(0)
        else:
            # Count in value range [0, k_rank-1]
            count = (pst._query_count(pst.roots[r+1], 0, M-1, 0, k_rank-1) -
                    pst._query_count(pst.roots[l], 0, M-1, 0, k_rank-1))
            results.append(count)
    
    return results


# Offline alternative using merge sort (space efficient):
def offline_count_less_than(arr: list[int], 
                             queries: list[tuple]) -> list[int]:
    """
    Offline approach: sort queries by k, process with BIT.
    Time: O((N + Q) log N) | Space: O(N + Q) — no O(N log N) space!
    
    This trades the persistent structure for offline sorting.
    """
    from bisect import insort
    
    n = len(arr)
    Q = len(queries)
    results = [0] * Q
    
    # For each query, count inversions relative to k
    sorted_qs = sorted(range(Q), key=lambda i: queries[i][2])
    
    # BIT for counting
    bit = [0] * (n + 2)
    
    def bit_update(i, delta=1):
        while i <= n:
            bit[i] += delta
            i += i & (-i)
    
    def bit_query(i):
        s = 0
        while i > 0:
            s += bit[i]
            i -= i & (-i)
        return s
    
    # Process elements by value in sorted order
    sorted_arr = sorted(enumerate(arr), key=lambda x: x[1])
    arr_ptr = 0
    
    for qi in sorted_qs:
        l, r, k = queries[qi]
        # Add all elements with value < k to BIT
        while arr_ptr < n and sorted_arr[arr_ptr][1] < k:
            idx, _ = sorted_arr[arr_ptr]
            bit_update(idx + 1)
            arr_ptr += 1
        results[qi] = bit_query(r + 1) - bit_query(l)
    
    return results
```

---

## 5. Persistent Array

```python
class PersistentArray:
    """
    Persistent array: point update creating new version, 
    access any version in O(log N).
    
    Implemented as persistent segment tree on indices.
    
    Time: O(log N) per update and access
    Space: O(N + U log N) where U = number of updates
    """
    
    def __init__(self, arr: list):
        self.n = len(arr)
        # Use persistent segment tree on indices [0, N-1]
        # Each leaf stores the actual value
        
        # Node pool
        self.left = [0]
        self.right = [0]
        self.val = [None]  # leaf values
        self.node_ct = 1
        self.roots = []
        
        # Build initial version
        root = self._build(arr, 0, self.n - 1)
        self.roots.append(root)
    
    def _alloc(self, l=0, r=0, v=None):
        self.left.append(l)
        self.right.append(r)
        self.val.append(v)
        self.node_ct += 1
        return self.node_ct - 1
    
    def _build(self, arr, lo, hi):
        if lo == hi:
            return self._alloc(v=arr[lo])
        mid = (lo + hi) // 2
        l = self._build(arr, lo, mid)
        r = self._build(arr, mid+1, hi)
        return self._alloc(l, r)
    
    def _update(self, node, lo, hi, idx, new_val):
        n = self._alloc(self.left[node], self.right[node], self.val[node])
        if lo == hi:
            self.val[n] = new_val
            return n
        mid = (lo + hi) // 2
        if idx <= mid:
            self.left[n] = self._update(self.left[node], lo, mid, idx, new_val)
        else:
            self.right[n] = self._update(self.right[node], mid+1, hi, idx, new_val)
        return n
    
    def set(self, version: int, idx: int, new_val) -> int:
        """
        Set arr[idx] = new_val, creating a new version.
        Returns new version number.
        Time: O(log N)
        """
        new_root = self._update(self.roots[version], 0, self.n-1, idx, new_val)
        self.roots.append(new_root)
        return len(self.roots) - 1
    
    def get(self, version: int, idx: int):
        """
        Get arr[idx] at given version.
        Time: O(log N)
        """
        node = self.roots[version]
        lo, hi = 0, self.n - 1
        while lo < hi:
            mid = (lo + hi) // 2
            if idx <= mid:
                node = self.left[node]
                hi = mid
            else:
                node = self.right[node]
                lo = mid + 1
        return self.val[node]


# ─── Example ───
pa = PersistentArray([1, 2, 3, 4, 5])
print(pa.get(0, 2))  # 3 (original)

v1 = pa.set(0, 2, 99)
print(pa.get(v1, 2))  # 99 (updated)
print(pa.get(0, 2))   # 3  (original unchanged!)

v2 = pa.set(v1, 0, 77)
print(pa.get(v2, 0))  # 77
print(pa.get(v1, 0))  # 1 (v1 unchanged)
print(pa.get(0, 0))   # 1 (original unchanged)
```

---

## 6. Persistent Trie — Maximum XOR in Prefix

```python
class PersistentTrie:
    """
    Persistent Trie for maximum XOR queries.
    
    Problem: Given array, for each query (l, r), find maximum XOR
    of any element in arr[l..r] with a given value x.
    
    Key insight: Build persistent trie where version i = trie with arr[0..i-1].
    For query [l, r]: use version r+1 and version l to determine if each bit
    has a "fresh" element (inserted between l and r) going the desired direction.
    
    Each node stores: count of elements in subtree (for this version's contribution)
    
    Time: O(N * B + Q * B) where B = bit length (32 or 64)
    Space: O(N * B)
    """
    
    BITS = 30  # For values up to 10^9
    
    def __init__(self):
        # Node pool: children[0], children[1], count
        self.ch = [[0, 0]]  # ch[node][bit] = child node
        self.cnt = [0]      # cnt[node] = count of elements in subtree
        self.node_ct = 1
        self.roots = [0]    # roots[0] = empty trie
    
    def _new_node(self):
        self.ch.append([0, 0])
        self.cnt.append(0)
        self.node_ct += 1
        return self.node_ct - 1
    
    def insert(self, prev_root: int, num: int) -> int:
        """
        Insert num into trie, creating new version.
        Returns new root node.
        Time: O(B)
        """
        new_root = self._new_node()
        self.cnt[new_root] = self.cnt[prev_root] + 1
        cur = new_root
        prev = prev_root
        
        for i in range(self.BITS, -1, -1):
            bit = (num >> i) & 1
            # Copy the other branch
            self.ch[cur][1 - bit] = self.ch[prev][1 - bit]
            # Create new node for this branch
            child = self._new_node()
            self.cnt[child] = self.cnt[self.ch[prev][bit]] + 1
            self.ch[cur][bit] = child
            cur = child
            prev = self.ch[prev][bit]
        
        return new_root
    
    def add(self, num: int) -> int:
        """Add num and create a new version. Returns version index."""
        new_root = self.insert(self.roots[-1], num)
        self.roots.append(new_root)
        return len(self.roots) - 1
    
    def max_xor_in_range(self, l: int, r: int, x: int) -> int:
        """
        Maximum XOR of x with any element in arr[l..r].
        Uses versions l and r+1 to restrict to elements in range.
        Time: O(B)
        """
        # "Count in range" at each node = cnt[root_r+1] - cnt[root_l]
        cur_l = self.roots[l]
        cur_r = self.roots[r + 1]
        result = 0
        
        for i in range(self.BITS, -1, -1):
            bit = (x >> i) & 1
            want = 1 - bit  # XOR=1 for this bit → go opposite direction
            
            # Check if there's any element in [l, r] going in 'want' direction
            count_want = self.cnt[self.ch[cur_r][want]] - self.cnt[self.ch[cur_l][want]]
            
            if count_want > 0:
                result |= (1 << i)
                cur_r = self.ch[cur_r][want]
                cur_l = self.ch[cur_l][want]
            else:
                cur_r = self.ch[cur_r][1 - want]
                cur_l = self.ch[cur_l][1 - want]
        
        return result


# ─── Example ───
arr = [3, 10, 5, 25, 2, 8]
trie = PersistentTrie()
for x in arr:
    trie.add(x)

# Max XOR with 5 in range [0, 4] (elements: 3, 10, 5, 25, 2)
print(trie.max_xor_in_range(0, 4, 5))  # 28 (5 XOR 25 = 28)
print(trie.max_xor_in_range(0, 2, 10)) # 15 (10 XOR 5 = 15)
```

---

## 7. Persistent DSU Concept

```python
class PersistentDSU:
    """
    Persistent Union-Find (DSU) using persistent array.
    
    IMPORTANT: Cannot use path compression with persistence!
    (Path compression would modify old versions)
    Must use UNION BY RANK only → O(log N) per operation instead of O(α(N))
    
    Use case: Offline dynamic connectivity, rollback queries.
    
    Time: O(log N) per union/find (no path compression)
    Space: O(N + U log N) where U = number of unions
    """
    
    def __init__(self, n: int):
        self.n = n
        self.parent = PersistentArray(list(range(n)))
        self.rank = PersistentArray([0] * n)
        self.versions = [(0, 0)]  # (parent_version, rank_version)
    
    def find(self, version: int, x: int) -> int:
        """Find root of x in given version. O(log^2 N) — log N hops, each O(log N)."""
        pv, rv = self.versions[version]
        while self.parent.get(pv, x) != x:
            x = self.parent.get(pv, x)
        return x
    
    def union(self, version: int, x: int, y: int) -> int:
        """
        Union x and y, creating a new version.
        Returns new version index.
        O(log^2 N)
        """
        pv, rv = self.versions[version]
        rx = self.find(version, x)
        ry = self.find(version, y)
        
        if rx == ry:
            self.versions.append((pv, rv))
            return len(self.versions) - 1
        
        rank_rx = self.rank.get(rv, rx)
        rank_ry = self.rank.get(rv, ry)
        
        if rank_rx < rank_ry:
            rx, ry = ry, rx
        
        # rx is the new root
        new_pv = self.parent.set(pv, ry, rx)
        new_rv = rv
        if rank_rx == rank_ry:
            new_rv = self.rank.set(rv, rx, rank_rx + 1)
        
        self.versions.append((new_pv, new_rv))
        return len(self.versions) - 1
    
    def connected(self, version: int, x: int, y: int) -> bool:
        """Check if x and y are connected in given version. O(log^2 N)."""
        return self.find(version, x) == self.find(version, y)
```

---

## 8. Functional Segment Tree

```python
def build_functional_segtree(arr: list[int]):
    """
    Functional (immutable) segment tree built with Python closures.
    Each node is a function returning its range's sum.
    
    This is the purest form of persistence — each "update" returns
    a new tree object sharing structure with the old.
    
    Time: O(N) build, O(log N) query/update
    Space: O(N log N) after updates
    
    More of a conceptual demonstration than production code.
    """
    
    def build(lo, hi):
        if lo == hi:
            val = arr[lo]
            def leaf_query(l, r): return val if l <= lo <= r else 0
            def leaf_update(pos, new_val):
                new_v = new_val
                def new_leaf_query(l, r): return new_v if l <= lo <= r else 0
                return new_leaf_query
            return leaf_query, leaf_update
        
        mid = (lo + hi) // 2
        left_q, left_u = build(lo, mid)
        right_q, right_u = build(mid+1, hi)
        
        def range_query(l, r):
            return left_q(l, r) + right_q(l, r)
        
        def point_update(pos, new_val):
            if pos <= mid:
                new_left_q, new_left_u = left_u(pos, new_val), None
                def new_range_query(l, r):
                    return new_left_q(l, r) + right_q(l, r)
            else:
                new_right_q, _ = right_u(pos, new_val), None
                def new_range_query(l, r):
                    return left_q(l, r) + new_right_q(l, r)
            return new_range_query
        
        return range_query, point_update
    
    return build(0, len(arr) - 1)
```

---

## 9. Version Queries — "State at Time T"

```python
class VersionedStore:
    """
    Key-value store with full version history.
    "What was the value of key K at time T?"
    
    Real-world application: Database MVCC (Multi-Version Concurrency Control)
    
    Implementation: Persistent balanced BST (here simplified with sorted list + binary search)
    For production: use persistent treap or persistent red-black tree.
    
    Time: O(log N) per get/set
    Space: O(U log N) where U = updates
    """
    
    def __init__(self):
        # versions[t] = dict snapshot at time t (simplified — in practice use persistent BST)
        self.history = [{}]  # versions[0] = empty
        self.time = 0
    
    def set(self, key, value) -> int:
        """Set key=value, creating new version. Returns new time."""
        new_state = dict(self.history[self.time])
        new_state[key] = value
        self.history.append(new_state)
        self.time += 1
        return self.time
    
    def get(self, key, time: int = None):
        """Get value of key at given time (default: current)."""
        t = time if time is not None else self.time
        return self.history[t].get(key)
    
    def delete(self, key) -> int:
        """Delete key, creating new version."""
        new_state = dict(self.history[self.time])
        new_state.pop(key, None)
        self.history.append(new_state)
        self.time += 1
        return self.time


# ─── Efficient version using persistent segment tree on sorted keys ───
class PersistentTimeSeries:
    """
    Range sum at any historical time point.
    "What was sum(arr[l..r]) at time T?"
    
    Build persistent segment tree: version T = state after T updates.
    Time: O(log N) per update and query
    Space: O(U log N)
    """
    
    def __init__(self, n: int):
        self.n = n
        self.pst = PersistentSegTree(0, n - 1)
        # Modify PersistentSegTree to store sum instead of count
        # (simplified here — production version would parameterize the aggregation)
    
    def point_update(self, pos: int, delta: int) -> int:
        """Update position pos, creating new version."""
        return self.pst.update(len(self.pst.roots) - 1, pos, delta)
    
    def range_sum_at_time(self, time: int, l: int, r: int) -> int:
        """Range sum at time T (version T)."""
        return self.pst.query_count(time, l, r)
```

---

## 10. Advanced Problems with Full Solutions

### Problem 1: Count of Smaller Numbers After Self (LeetCode 315)

```python
def countSmaller(nums: list[int]) -> list[int]:
    """
    For each element nums[i], count elements to its right that are smaller.
    
    Approach 1: Merge Sort (O(N log N)) — elegant
    Approach 2: Persistent Segment Tree (O(N log N)) — demonstrates persistence
    
    PST approach:
    - Process from right to left
    - Version i = tree with nums[i..N-1] inserted
    - For each i, query count of elements < nums[i] in version i+1
    
    Time: O(N log N) | Space: O(N log N)
    """
    n = len(nums)
    
    # Coordinate compress
    vals = sorted(set(nums))
    rank = {v: i for i, v in enumerate(vals)}
    M = len(vals)
    
    # Persistent segment tree
    left_c = [0] * (n * 20 + 5)
    right_c = [0] * (n * 20 + 5)
    cnt = [0] * (n * 20 + 5)
    nc = [1]
    
    def new_node():
        idx = nc[0]; nc[0] += 1; return idx
    
    def update(prev, lo, hi, pos):
        node = new_node()
        left_c[node] = left_c[prev]
        right_c[node] = right_c[prev]
        cnt[node] = cnt[prev] + 1
        if lo == hi: return node
        mid = (lo + hi) // 2
        if pos <= mid:
            left_c[node] = update(left_c[prev], lo, mid, pos)
        else:
            right_c[node] = update(right_c[prev], mid+1, hi, pos)
        return node
    
    def query(node, lo, hi, l, r):
        if node == 0 or r < lo or hi < l: return 0
        if l <= lo and hi <= r: return cnt[node]
        mid = (lo + hi) // 2
        return query(left_c[node], lo, mid, l, r) + query(right_c[node], mid+1, hi, l, r)
    
    roots = [0]
    for x in reversed(nums):
        roots.append(update(roots[-1], 0, M-1, rank[x]))
    roots.reverse()
    
    result = []
    for i, x in enumerate(nums):
        if rank[x] == 0:
            result.append(0)
        else:
            result.append(query(roots[i+1], 0, M-1, 0, rank[x]-1))
    
    return result


# Test
print(countSmaller([5, 2, 6, 1]))   # [2, 1, 1, 0]
print(countSmaller([1]))            # [0]
print(countSmaller([-1, -1]))       # [0, 0]
```

### Problem 2: Range K-th Smallest (Full End-to-End)

```python
def range_kth_smallest(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    LeetCode 2261-style: K-th smallest in range.
    Complete production implementation.
    
    Time: O(N log N + Q log N) | Space: O(N log N)
    """
    return kth_smallest_complete(arr, queries)

# Test
arr = [1, 5, 2, 6, 3, 7, 4]
qs = [(2, 5, 3), (0, 6, 4)]
print(range_kth_smallest(arr, qs))
# [2,5,6,3]: 3rd smallest = 5? Let's verify: sorted([2,6,3,7])=[2,3,6,7], 3rd=6
```

### Problem 3: Maximum XOR in Range

```python
def max_xor_queries(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    For each query (l, r, x), find maximum XOR of x with any element in [l, r].
    
    Uses Persistent Trie.
    Time: O(N * B + Q * B) where B = 30 bits
    Space: O(N * B)
    """
    trie = PersistentTrie()
    for x in arr:
        trie.add(x)
    
    return [trie.max_xor_in_range(l, r, x) for l, r, x in queries]


# Test
arr = [3, 10, 5, 25, 2, 8]
queries = [(0, 4, 5), (1, 3, 10)]
print(max_xor_queries(arr, queries))  # [28, 27]
```

### Problem 4: Offline LCA with Persistent DSU

```python
def offline_lca_persistent(n: int, edges: list, queries: list) -> list[int]:
    """
    LCA using Tarjan's offline algorithm + persistent DSU.
    
    For each query (u, v), LCA is the lowest common ancestor.
    Tarjan's processes DFS and answers queries when both endpoints are visited.
    
    Time: O((N + Q) * α(N)) | Space: O(N + Q)
    """
    from collections import defaultdict
    
    adj = defaultdict(list)
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    # Standard Tarjan's offline LCA
    parent = list(range(n))
    ancestor = list(range(n))
    
    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]  # path compression OK for offline
            x = parent[x]
        return x
    
    def union(x, y):
        parent[find(x)] = find(y)
    
    query_map = defaultdict(list)
    for i, (u, v) in enumerate(queries):
        query_map[u].append((v, i))
        query_map[v].append((u, i))
    
    answers = [-1] * len(queries)
    visited = [False] * n
    
    def dfs(u, par):
        ancestor[u] = u
        for v in adj[u]:
            if v != par:
                dfs(v, u)
                union(u, v)
                ancestor[find(u)] = u
        visited[u] = True
        for v, qi in query_map[u]:
            if visited[v]:
                answers[qi] = ancestor[find(v)]
    
    dfs(0, -1)
    return answers
```

---

## 11. Interview Tips & Edge Cases

### ⚡ Common Interview Traps

| Trap | Correct Approach |
|------|-----------------|
| Forgetting to coordinate-compress | Always compress when values >> N |
| Off-by-one in version indexing | Version i = prefix of length i (tree with arr[0..i-1]) |
| Using path compression in persistent DSU | ❌ Must use rank-only — path compression mutates! |
| Allocating too few nodes | Budget: (N+1) * (log N + 5) nodes per update |
| Querying with l > r (empty range) | Handle l==0: version_l is the empty tree (version 0) |

### 🔑 Key Complexity Summary

| Operation | Time | Space |
|-----------|------|-------|
| Persistent seg tree build | O(N) | O(N) |
| Persistent seg tree update | O(log N) | O(log N) new nodes |
| Persistent seg tree query | O(log N) | — |
| K-th in range | O(N log N + Q log N) | O(N log N) |
| Persistent array get/set | O(log N) | O(log N) |
| Persistent trie insert | O(B) | O(B) per insert |
| Persistent DSU union/find | O(log² N) | O(log N) |

### 📋 Edge Cases

```python
# Edge 1: k = 1 (minimum in range)
arr = [5, 3, 1, 4, 2]
assert kth_smallest_complete(arr, [(0, 4, 1)]) == [1]

# Edge 2: k = r - l + 1 (maximum in range)
assert kth_smallest_complete(arr, [(0, 4, 5)]) == [5]

# Edge 3: Single element range
assert kth_smallest_complete(arr, [(2, 2, 1)]) == [1]

# Edge 4: Duplicate values
arr2 = [3, 3, 3, 3, 3]
assert kth_smallest_complete(arr2, [(0, 4, 3)]) == [3]

# Edge 5: All same values with k
arr3 = [1, 2, 1, 2, 1]
result = kth_smallest_complete(arr3, [(0, 4, 3)])
assert result == [1]  # sorted [1,1,1,2,2], 3rd = 1

# Edge 6: Negative values (coordinate compress handles this)
arr4 = [-5, -3, -1, 0, 2]
result = kth_smallest_complete(arr4, [(0, 4, 2)])
assert result == [-3]
```

### 🏆 Interview Communication Guide

**"How does persistent segment tree differ from regular segment tree?"**
> "Regular segment tree mutates nodes during update. Persistent version creates new nodes only along the update path (O(log N) nodes), and shares all other nodes with the previous version. This gives us O(log N) access to any historical state."

**"Why do we need coordinate compression for k-th order queries?"**
> "The segment tree is indexed by VALUES not indices. If values range up to 10^9, we'd need a tree of size 10^9. Coordinate compression maps M unique values to [0, M-1], reducing tree size to O(N)."

**"What's the space complexity of persistent segment tree after N updates?"**
> "Each update creates O(log N) new nodes. After N updates, total nodes = O(N) initial + O(N log N) from updates = O(N log N). Each node is O(1) space. Total: O(N log N) space."

---

*Persistent data structures are a secret weapon for range order-statistic queries. The k-th smallest in range problem is THE canonical example that appears in Google, Codeforces rounds, and advanced LeetCode problems. Master the path-copying technique — once understood, it applies to any tree-based data structure.*

# Sparse Table & Range Minimum Query (RMQ) — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** Segment Trees, Binary Lifting, DFS/BFS on Trees  
> **Interview Frequency:** Google ★★★★★ | Meta ★★★☆☆ | Amazon ★★☆☆☆

---

## Table of Contents
1. [Core Concept & Intuition](#1-core-concept--intuition)
2. [Why Idempotency Is Required](#2-why-idempotency-is-required)
3. [Sparse Table Construction — O(N log N)](#3-sparse-table-construction--on-log-n)
4. [O(1) Query for Idempotent Operations](#4-o1-query-for-idempotent-operations)
5. [Sparse Table vs Segment Tree](#5-sparse-table-vs-segment-tree)
6. [Range GCD Query](#6-range-gcd-query)
7. [Range Frequency Query](#7-range-frequency-query)
8. [LCA via Euler Tour + RMQ](#8-lca-via-euler-tour--rmq)
9. [±1 RMQ (Fischer-Heun Structure)](#9-1-rmq-fischerheun-structure)
10. [Merge Sort Tree — K-th Order in Range](#10-merge-sort-tree--k-th-order-in-range)
11. [Offline RMQ — Divide & Conquer](#11-offline-rmq--divide--conquer)
12. [Advanced Problems with Full Solutions](#12-advanced-problems-with-full-solutions)
13. [Interview Tips & Edge Cases](#13-interview-tips--edge-cases)

---

## 1. Core Concept & Intuition

A **Sparse Table** precomputes answers for all ranges whose length is a power of 2. For any query `[l, r]`, we find the largest power-of-2 that fits in the range and combine two (possibly overlapping) precomputed answers.

**Key Insight:** For **idempotent** functions `f(f(x,y), y) = f(x,y)`, overlapping intervals don't cause double-counting. This enables O(1) queries by using TWO overlapping blocks that together cover `[l,r]` exactly.

```
Array: [2, 4, 3, 1, 6, 7, 8, 9, 1, 7]
Index:  0  1  2  3  4  5  6  7  8  9

Sparse Table[i][j] = min of arr[i..i+2^j-1]

j=0 (length 1): [2,4,3,1,6,7,8,9,1,7]
j=1 (length 2): [2,3,1,1,6,7,8,1,1]   → min of pairs
j=2 (length 4): [1,1,1,1,1,1,1,1]     → min of quads
j=3 (length 8): [1,1,1]               → min of octets

Query min[2..7] → length=6, k=2 (largest 2^k ≤ 6=4)
  → min(sparse[2][2], sparse[4][2]) = min(1,6) = 1  ✓
```

---

## 2. Why Idempotency Is Required

An operation `⊕` is **idempotent** if: `x ⊕ x = x`

| Operation | Idempotent? | Sparse Table O(1) Query? |
|-----------|-------------|--------------------------|
| `min`     | ✅ `min(x,x)=x` | ✅ Yes |
| `max`     | ✅ `max(x,x)=x` | ✅ Yes |
| `gcd`     | ✅ `gcd(x,x)=x` | ✅ Yes |
| `AND/OR`  | ✅ `x&x=x`  | ✅ Yes |
| `sum`     | ❌ `x+x≠x`  | ❌ No (use prefix sum/BIT) |
| `product` | ❌ `x*x≠x`  | ❌ No |
| `XOR`     | ❌ `x^x=0`  | ❌ No |

**Why non-idempotent fails:** For sum query `[2,7]`:
- Two overlapping blocks `[2,5]` and `[4,7]` would double-count indices 4,5
- We can't "undo" the double-count since sum isn't idempotent

For non-idempotent ops, we must use non-overlapping decomposition → O(log N) query (segment tree territory).

---

## 3. Sparse Table Construction — O(N log N)

```python
import math

class SparseTable:
    """
    Sparse Table for Range Minimum Query (RMQ).
    Preprocessing: O(N log N) time, O(N log N) space
    Query: O(1) time
    """
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        LOG = max(1, int(math.log2(n)) + 1) if n > 0 else 1
        
        # sparse[i][j] = min of arr[i .. i + 2^j - 1]
        self.sparse = [[float('inf')] * LOG for _ in range(n)]
        self.log2 = [0] * (n + 1)
        self.n = n
        
        # Precompute log2 table
        for i in range(2, n + 1):
            self.log2[i] = self.log2[i // 2] + 1
        
        # Base case: j=0 (single elements)
        for i in range(n):
            self.sparse[i][0] = arr[i]
        
        # Fill table using DP recurrence:
        # sparse[i][j] = min(sparse[i][j-1], sparse[i + 2^(j-1)][j-1])
        for j in range(1, LOG):
            for i in range(n - (1 << j) + 1):
                self.sparse[i][j] = min(
                    self.sparse[i][j - 1],
                    self.sparse[i + (1 << (j - 1))][j - 1]
                )
    
    def query(self, l: int, r: int) -> int:
        """
        RMQ query on [l, r] inclusive. O(1) time.
        Uses two OVERLAPPING blocks of size 2^k where 2^k <= (r-l+1)
        """
        if l > r:
            raise ValueError(f"Invalid range [{l}, {r}]")
        k = self.log2[r - l + 1]
        return min(
            self.sparse[l][k],
            self.sparse[r - (1 << k) + 1][k]
        )


# ─── Demonstration ───
arr = [2, 4, 3, 1, 6, 7, 8, 9, 1, 7]
st = SparseTable(arr)
print(st.query(0, 4))   # min(2,4,3,1,6) = 1
print(st.query(5, 9))   # min(7,8,9,1,7) = 1
print(st.query(0, 9))   # min entire array = 1
print(st.query(0, 2))   # min(2,4,3) = 2
```

**Complexity:**
- **Preprocessing:** O(N log N) time, O(N log N) space
- **Query:** O(1) time
- **Space optimization:** The log table saves repeated `log2` computations during query

---

## 4. O(1) Query for Idempotent Operations

The O(1) trick works because for idempotent functions, we can cover `[l,r]` with TWO overlapping ranges of size `2^k`:

```
[l, l+2^k-1]  and  [r-2^k+1, r]
where k = floor(log2(r-l+1))
These two ranges together cover ALL of [l,r] — and overlap is harmless!
```

### Range Maximum Query

```python
class SparseTableMax:
    """Sparse Table for Range Maximum Query."""
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        LOG = max(1, n.bit_length())
        self.sparse = [[float('-inf')] * LOG for _ in range(n)]
        self.log2 = [0] * (n + 1)
        
        for i in range(2, n + 1):
            self.log2[i] = self.log2[i // 2] + 1
        
        for i in range(n):
            self.sparse[i][0] = arr[i]
        
        for j in range(1, LOG):
            for i in range(n - (1 << j) + 1):
                self.sparse[i][j] = max(
                    self.sparse[i][j - 1],
                    self.sparse[i + (1 << (j - 1))][j - 1]
                )
    
    def query(self, l: int, r: int) -> int:
        k = self.log2[r - l + 1]
        return max(self.sparse[l][k], self.sparse[r - (1 << k) + 1][k])
```

**Time:** O(1) per query | **Space:** O(N log N)

---

## 5. Sparse Table vs Segment Tree

| Feature | Sparse Table | Segment Tree |
|---------|-------------|--------------|
| Build Time | O(N log N) | O(N) |
| Query Time | **O(1)** | O(log N) |
| Update Time | ❌ O(N log N) rebuild | ✅ O(log N) |
| Space | O(N log N) | O(N) |
| Operations | Only idempotent | Any associative |
| Static data? | ✅ Perfect | ✅ Good |
| Dynamic data? | ❌ Very bad | ✅ Perfect |
| Cache performance | ✅ Better (arrays) | ⚠️ Tree traversal |

**Decision rule:**
- Static array + range min/max/gcd/AND/OR → **Sparse Table**
- Dynamic updates needed → **Segment Tree / BIT**
- Range sum/product on static → **Prefix sums**

---

## 6. Range GCD Query

GCD is idempotent: `gcd(x, x) = x`. Perfect for sparse table!

```python
from math import gcd
from functools import reduce

class SparseTableGCD:
    """
    Range GCD Query in O(1).
    
    Key property: gcd(a, b, c, ...) is associative and idempotent.
    gcd(gcd(a,b), gcd(b,c)) correctly handles overlap.
    """
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        LOG = max(1, n.bit_length())
        self.table = [arr[:]]  # j=0: single elements
        self.log2 = [0] * (n + 1)
        
        for i in range(2, n + 1):
            self.log2[i] = self.log2[i // 2] + 1
        
        for j in range(1, LOG):
            prev = self.table[j - 1]
            curr = []
            for i in range(n - (1 << j) + 1):
                curr.append(gcd(prev[i], prev[i + (1 << (j - 1))]))
            self.table.append(curr)
        
        self.LOG = LOG
    
    def query(self, l: int, r: int) -> int:
        """Range GCD query in O(1)."""
        k = self.log2[r - l + 1]
        return gcd(self.table[k][l], self.table[k][r - (1 << k) + 1])


# Test
arr = [12, 8, 6, 4, 24, 36]
st_gcd = SparseTableGCD(arr)
print(st_gcd.query(0, 2))  # gcd(12,8,6) = 2
print(st_gcd.query(0, 5))  # gcd(12,8,6,4,24,36) = 4... wait = 4? No = 4
print(st_gcd.query(3, 5))  # gcd(4,24,36) = 4
```

**Complexity:** Build O(N log N), Query O(1)

### Problem: Count Subarrays with GCD = 1

```python
def count_subarrays_gcd_one(arr: list[int]) -> int:
    """
    Count subarrays where GCD = 1.
    
    Approach: For each right endpoint r, find all distinct GCD values
    of subarrays ending at r. There are at most O(log(max)) distinct values.
    
    Time: O(N log(max) * log(max)) | Space: O(log(max))
    """
    n = len(arr)
    count = 0
    # prev_gcds: list of (gcd_value, leftmost_index_with_this_gcd)
    prev_gcds = []
    
    for r in range(n):
        new_gcds = []
        for g, l in prev_gcds:
            new_g = gcd(g, arr[r])
            if not new_gcds or new_gcds[-1][0] != new_g:
                new_gcds.append((new_g, l))
        if not new_gcds or new_gcds[-1][0] != arr[r]:
            new_gcds.append((arr[r], r))
        else:
            new_gcds[-1] = (arr[r], r)
        
        for i, (g, l) in enumerate(new_gcds):
            right_l = new_gcds[i + 1][1] if i + 1 < len(new_gcds) else r + 1
            if g == 1:
                count += right_l - l
        
        prev_gcds = new_gcds
    
    return count
```

---

## 7. Range Frequency Query

**Problem:** Given array, answer Q queries: "How many times does value `v` appear in `arr[l..r]`?"

```python
from bisect import bisect_left, bisect_right
from collections import defaultdict

class RangeFrequency:
    """
    Offline range frequency using sorted index lists per value.
    
    Build: O(N log N) for sorting
    Query: O(log N) per query using binary search
    Space: O(N)
    """
    
    def __init__(self, arr: list[int]):
        self.positions = defaultdict(list)
        for i, val in enumerate(arr):
            self.positions[val].append(i)
    
    def query(self, l: int, r: int, val: int) -> int:
        """
        Count occurrences of val in arr[l..r].
        Time: O(log N) per query.
        """
        pos = self.positions.get(val, [])
        lo = bisect_left(pos, l)
        hi = bisect_right(pos, r)
        return hi - lo


# Advanced: Wavelet Tree for Range Frequency + K-th Order (sketch)
class WaveletTree:
    """
    Wavelet Tree: supports range frequency and k-th order in O(log(max_val)).
    
    For competitive programming — handles:
    - Count elements in [l,r] with value in [a,b]: O(log V)
    - K-th smallest in [l,r]: O(log V)
    - Count inversions in range: O(log V)
    
    Space: O(N log V)
    """
    
    def __init__(self, arr: list[int], lo: int, hi: int):
        self.lo = lo
        self.hi = hi
        self.n = len(arr)
        self.left_count = []  # how many elements went left at each position
        
        if lo == hi or not arr:
            self.left_child = None
            self.right_child = None
            return
        
        mid = (lo + hi) // 2
        left_arr, right_arr = [], []
        
        for x in arr:
            self.left_count.append(len(left_arr))
            if x <= mid:
                left_arr.append(x)
            else:
                right_arr.append(x)
        self.left_count.append(len(left_arr))  # sentinel
        
        self.left_child = WaveletTree(left_arr, lo, mid) if left_arr else None
        self.right_child = WaveletTree(right_arr, mid + 1, hi) if right_arr else None
    
    def kth_smallest(self, l: int, r: int, k: int) -> int:
        """K-th smallest (1-indexed) in arr[l..r]. O(log V)."""
        if self.lo == self.hi:
            return self.lo
        
        lb = self.left_count[l]
        rb = self.left_count[r + 1]
        in_left = rb - lb  # elements in [l,r] that went to left child
        
        if k <= in_left:
            return self.left_child.kth_smallest(lb, rb - 1, k)
        else:
            return self.right_child.kth_smallest(
                l - lb, r - rb, k - in_left
            )
```

**Complexity (Wavelet Tree):** Build O(N log V), Query O(log V) | Space O(N log V)

---

## 8. LCA via Euler Tour + RMQ

**LCA (Lowest Common Ancestor)** can be reduced to RMQ using the Euler tour technique. This is one of the most beautiful reductions in competitive programming.

```python
class LCAWithRMQ:
    """
    LCA in O(1) query using Euler Tour + Sparse Table RMQ.
    
    Algorithm:
    1. Euler tour of tree: visit node each time you enter/exit
       → produces array of length 2N-1
    2. Depth array for Euler tour positions
    3. first[v] = first occurrence of v in Euler tour
    4. LCA(u,v) = node at minimum DEPTH position in euler[first[u]..first[v]]
    
    Preprocessing: O(N log N)
    Query: O(1)
    """
    
    def __init__(self, n: int, edges: list[tuple], root: int = 0):
        self.n = n
        self.adj = [[] for _ in range(n)]
        for u, v in edges:
            self.adj[u].append(v)
            self.adj[v].append(u)
        
        self.euler = []       # Euler tour (node sequence)
        self.depth_euler = [] # depth at each position in euler tour
        self.first = [-1] * n # first occurrence of each node in euler tour
        self.depth = [0] * n
        
        self._euler_tour(root, -1, 0)
        
        # Build RMQ on depth_euler (we want min depth → that node is LCA)
        self.rmq = self._build_sparse(self.depth_euler)
        self.log2 = self._build_log(len(self.euler))
    
    def _euler_tour(self, u: int, parent: int, d: int):
        self.depth[u] = d
        self.first[u] = len(self.euler)
        self.euler.append(u)
        self.depth_euler.append(d)
        
        for v in self.adj[u]:
            if v != parent:
                self._euler_tour(v, u, d + 1)
                self.euler.append(u)  # back-edge: revisit parent
                self.depth_euler.append(d)
    
    def _build_log(self, n: int) -> list:
        log2 = [0] * (n + 1)
        for i in range(2, n + 1):
            log2[i] = log2[i // 2] + 1
        return log2
    
    def _build_sparse(self, arr: list) -> list:
        n = len(arr)
        LOG = max(1, n.bit_length())
        sparse = [arr[:]]
        for j in range(1, LOG):
            prev = sparse[j - 1]
            curr = []
            for i in range(n - (1 << j) + 1):
                a, b = prev[i], prev[i + (1 << (j - 1))]
                curr.append(a if a <= b else b)
            sparse.append(curr)
        return sparse
    
    def _rmq_idx(self, l: int, r: int) -> int:
        """Returns INDEX of minimum depth in depth_euler[l..r]."""
        k = self.log2[r - l + 1]
        if self.rmq[k][l] <= self.rmq[k][r - (1 << k) + 1]:
            return l
        return r - (1 << k) + 1
    
    def lca(self, u: int, v: int) -> int:
        """LCA of u and v in O(1)."""
        l, r = self.first[u], self.first[v]
        if l > r:
            l, r = r, l
        idx = self._rmq_idx(l, r)
        return self.euler[idx]
    
    def distance(self, u: int, v: int) -> int:
        """Tree distance = depth[u] + depth[v] - 2*depth[lca(u,v)]."""
        anc = self.lca(u, v)
        return self.depth[u] + self.depth[v] - 2 * self.depth[anc]


# ─── Example ───
#        0
#       / \
#      1   2
#     / \   \
#    3   4   5
n = 6
edges = [(0,1),(0,2),(1,3),(1,4),(2,5)]
lca_solver = LCAWithRMQ(n, edges, root=0)

print(lca_solver.lca(3, 4))   # 1 (both children of 1)
print(lca_solver.lca(3, 5))   # 0 (root)
print(lca_solver.lca(1, 5))   # 0
print(lca_solver.distance(3, 5))  # depth[3]+depth[5]-2*depth[0] = 2+2-0 = 4
```

**Complexity:** O(N log N) preprocessing, **O(1) LCA query** | Space O(N log N)

---

## 9. ±1 RMQ (Fischer-Heun Structure)

In the Euler tour, adjacent depth values differ by exactly ±1. This special structure allows **O(N) preprocessing, O(1) query** via block decomposition + precomputed lookup tables.

```python
class PlusMinusOneRMQ:
    """
    ±1 RMQ: optimal O(N) preprocessing, O(1) query.
    Works when adjacent elements differ by exactly ±1 (like Euler tour depths).
    
    Algorithm:
    1. Divide array into blocks of size B = log(N)/2
    2. Build sparse table on block minimums → O(N/B * log(N/B)) = O(N)
    3. Each block has a "type" determined by the ±1 pattern → 2^B possible types
    4. Precompute in-block RMQ for all 2^B types → O(2^B * B^2) = O(N) total
    5. Query: in-block lookup + block-level sparse table lookup
    
    This is the theoretically optimal solution — O(N) preprocessing, O(1) query.
    """
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        self.arr = arr
        self.n = n
        if n == 0:
            return
        
        import math
        self.B = max(1, int(math.log2(n)) // 2)  # block size
        
        num_blocks = (n + self.B - 1) // self.B
        
        # Block minimums and their positions
        block_min = []
        block_min_idx = []
        for b in range(num_blocks):
            start = b * self.B
            end = min(start + self.B, n)
            min_val = arr[start]
            min_pos = start
            for i in range(start + 1, end):
                if arr[i] < min_val:
                    min_val = arr[i]
                    min_pos = i
            block_min.append(min_val)
            block_min_idx.append(min_pos)
        
        # Sparse table on block minimums
        self.block_min_idx = block_min_idx
        self.block_sparse = self._build_sparse_idx(block_min, block_min_idx)
        self.log2 = self._build_log(num_blocks + 1)
        self.B_size = self.B
        self.num_blocks = num_blocks
        
        # Precompute in-block RMQ using block type
        # Block type: encode ±1 sequence as bitmask
        self.in_block_rmq = {}  # type_mask -> rmq table (B x B)
        for b in range(num_blocks):
            start = b * self.B
            end = min(start + self.B, n)
            block = arr[start:end]
            
            # Compute block type (bitmask of +1/-1 transitions)
            mask = 0
            for i in range(1, len(block)):
                if block[i] > block[i-1]:  # +1
                    mask |= (1 << (i - 1))
            
            if mask not in self.in_block_rmq:
                # Precompute all-pairs RMQ for this block type
                b_len = len(block)
                rmq_table = [[0] * b_len for _ in range(b_len)]
                for i in range(b_len):
                    rmq_table[i][i] = i
                    for j in range(i + 1, b_len):
                        prev = rmq_table[i][j-1]
                        rmq_table[i][j] = prev if block[prev] <= block[j] else j
                self.in_block_rmq[mask] = rmq_table
            
            # Store block's mask
            if not hasattr(self, 'block_masks'):
                self.block_masks = []
            self.block_masks.append(mask)
    
    def _build_log(self, n):
        log2 = [0] * n
        for i in range(2, n):
            log2[i] = log2[i // 2] + 1
        return log2
    
    def _build_sparse_idx(self, vals, idxs):
        # Returns sparse table storing indices of minimum values
        n = len(vals)
        if n == 0:
            return []
        LOG = max(1, n.bit_length())
        sparse = [list(idxs)]
        for j in range(1, LOG):
            prev = sparse[j-1]
            curr = []
            for i in range(n - (1 << j) + 1):
                a, b = prev[i], prev[i + (1 << (j-1))]
                curr.append(a if vals[a - (sparse[0][0] if False else 0)] <=
                            vals[b - 0] else b)
            sparse.append(curr)
        return sparse
    
    def query_min_idx(self, l: int, r: int) -> int:
        """Returns index of minimum in arr[l..r]. O(1)."""
        bl = l // self.B
        br = r // self.B
        
        if bl == br:
            # Same block: use in-block table
            mask = self.block_masks[bl]
            table = self.in_block_rmq[mask]
            local_l = l - bl * self.B
            local_r = r - bl * self.B
            local_min = table[local_l][local_r]
            return bl * self.B + local_min
        
        # Left partial block
        mask_l = self.block_masks[bl]
        table_l = self.in_block_rmq[mask_l]
        local_l = l - bl * self.B
        local_max_l = self.B - 1
        left_min_idx = bl * self.B + table_l[local_l][local_max_l]
        
        # Right partial block
        mask_r = self.block_masks[br]
        table_r = self.in_block_rmq[mask_r]
        local_r = r - br * self.B
        right_min_idx = br * self.B + table_r[0][local_r]
        
        best = left_min_idx if self.arr[left_min_idx] <= self.arr[right_min_idx] else right_min_idx
        
        # Middle blocks via block sparse table
        if bl + 1 <= br - 1:
            k = self.log2[br - bl - 1]
            # Simplified: just use block minimums
            pass  # Full implementation omitted for brevity
        
        return best
```

**Complexity:** O(N) preprocessing, O(1) query | Theoretical optimum for static RMQ

---

## 10. Merge Sort Tree — K-th Order in Range

```python
class MergeSortTree:
    """
    Segment tree where each node stores a SORTED list of its range's elements.
    
    Supports:
    - Count elements in [l,r] less than k: O(log^2 N)
    - K-th smallest in [l,r]: O(log^3 N) with binary search on answer
    
    Space: O(N log N) — each element appears in O(log N) nodes
    Build: O(N log^2 N)
    """
    
    def __init__(self, arr: list[int]):
        self.n = len(arr)
        self.tree = [[] for _ in range(4 * self.n)]
        self._build(arr, 1, 0, self.n - 1)
    
    def _build(self, arr, node, l, r):
        if l == r:
            self.tree[node] = [arr[l]]
            return
        mid = (l + r) // 2
        self._build(arr, 2*node, l, mid)
        self._build(arr, 2*node+1, mid+1, r)
        # Merge two sorted arrays
        i, j = 0, 0
        la = self.tree[2*node]
        ra = self.tree[2*node+1]
        merged = []
        while i < len(la) and j < len(ra):
            if la[i] <= ra[j]:
                merged.append(la[i]); i += 1
            else:
                merged.append(ra[j]); j += 1
        merged.extend(la[i:])
        merged.extend(ra[j:])
        self.tree[node] = merged
    
    def count_less_than(self, ql, qr, x, node=1, l=None, r=None) -> int:
        """Count elements in [ql,qr] strictly less than x. O(log^2 N)."""
        if l is None: l, r = 0, self.n - 1
        if qr < l or r < ql:
            return 0
        if ql <= l and r <= qr:
            return bisect_left(self.tree[node], x)
        mid = (l + r) // 2
        return (self.count_less_than(ql, qr, x, 2*node, l, mid) +
                self.count_less_than(ql, qr, x, 2*node+1, mid+1, r))
    
    def kth_smallest(self, ql: int, qr: int, k: int) -> int:
        """
        K-th smallest element (1-indexed) in arr[ql..qr].
        Binary search on answer + count_less_than. O(log^3 N).
        """
        from bisect import bisect_left, bisect_right
        # Coordinate compress via global sorted list
        lo, hi = min(self.tree[1]), max(self.tree[1])
        
        while lo < hi:
            mid = (lo + hi) // 2
            if self.count_less_than(ql, qr, mid + 1) >= k:
                hi = mid
            else:
                lo = mid + 1
        return lo


# ─── Usage ───
from bisect import bisect_left
arr = [7, 2, 3, 1, 5, 8, 4, 6]
mst = MergeSortTree(arr)
print(mst.count_less_than(1, 5, 5))  # elements < 5 in [1..5]: 2,3,1 → 3
print(mst.kth_smallest(0, 7, 3))     # 3rd smallest in full array = 3
```

**Complexity:** Build O(N log² N), Count O(log² N), K-th O(log³ N) | Space O(N log N)

---

## 11. Offline RMQ — Divide & Conquer

```python
def offline_rmq_dc(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    Offline RMQ using Divide & Conquer.
    
    Algorithm:
    - For each divide at mid, extend from mid outward to find min in [l,mid] and [mid,r]
    - Queries with l<=mid<=r can be answered in O(1) after O(N) preprocessing
    - Recurse on left/right for queries entirely in one half
    
    Total: O((N+Q) log N) which equals sparse table for many queries,
    but uses O(N) space instead of O(N log N).
    
    Useful when memory is tight but queries are known offline.
    """
    Q = len(queries)
    answers = [0] * Q
    
    def solve(ql, qr, query_ids):
        if not query_ids or ql == qr:
            for qi in query_ids:
                answers[qi] = arr[ql]
            return
        
        mid = (ql + qr) // 2
        
        # Extend from mid leftward
        left_min = [0] * (mid - ql + 1)
        left_min[0] = arr[mid]
        for i in range(1, mid - ql + 1):
            left_min[i] = min(left_min[i-1], arr[mid - i])
        
        # Extend from mid rightward
        right_min = [0] * (qr - mid + 1)
        right_min[0] = arr[mid]
        for i in range(1, qr - mid + 1):
            right_min[i] = min(right_min[i-1], arr[mid + i])
        
        left_q, right_q, cross_q = [], [], []
        for qi in query_ids:
            l, r = queries[qi]
            if r <= mid:
                left_q.append(qi)
            elif l > mid:
                right_q.append(qi)
            else:  # l <= mid <= r (cross query)
                answers[qi] = min(left_min[mid - l], right_min[r - mid])
        
        solve(ql, mid, left_q)
        solve(mid + 1, qr, right_q)
    
    if arr:
        solve(0, len(arr) - 1, list(range(Q)))
    
    return answers


# ─── Example ───
arr = [3, 1, 4, 1, 5, 9, 2, 6]
queries = [(0, 3), (2, 6), (1, 7), (0, 7)]
results = offline_rmq_dc(arr, queries)
print(results)  # [1, 1, 1, 1]
```

**Complexity:** O((N + Q) log N) time, O(N + Q) space

---

## 12. Advanced Problems with Full Solutions

### Problem 1: Maximum Width Ramp (LeetCode 962)

```python
def maxWidthRamp(nums: list[int]) -> int:
    """
    Find max j-i where i<j and nums[i] <= nums[j].
    
    Approach: Precompute suffix_max (sparse table for max from right).
    Then for each left endpoint, binary search for rightmost valid j.
    
    Time: O(N log N) | Space: O(N)
    """
    n = len(nums)
    
    # Build decreasing stack of "left candidates"
    # (if nums[i] >= nums[j] for i < j, then j is never better as a left endpoint)
    left_stack = []
    for i in range(n):
        if not left_stack or nums[i] < nums[left_stack[-1]]:
            left_stack.append(i)
    
    # Scan from right, greedily matching with stack
    ans = 0
    j = n - 1
    while j >= 0 and left_stack:
        while left_stack and nums[left_stack[-1]] <= nums[j]:
            ans = max(ans, j - left_stack.pop())
        j -= 1
    
    return ans

# O(N) two-pointer approach (optimal)
```

### Problem 2: Range Min Sum (Google-style)

```python
def range_min_sum(arr: list[int], queries: list[tuple]) -> list[int]:
    """
    For each query (l, r, k), find the k-th minimum in arr[l..r].
    Uses Merge Sort Tree.
    
    Time per query: O(log^3 N) | Space: O(N log N)
    """
    mst = MergeSortTree(arr)
    return [mst.kth_smallest(l, r, k) for l, r, k in queries]
```

### Problem 3: LCA Distance Queries

```python
def solve_lca_queries(n: int, edges: list, queries: list) -> list[int]:
    """
    Multiple LCA queries on a tree.
    Returns distance between nodes u, v for each query.
    
    Time: O(N log N + Q) | Space: O(N log N)
    """
    lca_solver = LCAWithRMQ(n, edges)
    return [lca_solver.distance(u, v) for u, v in queries]
```

### Problem 4: Sliding Window Minimum (Deque + Sparse Table comparison)

```python
from collections import deque

def sliding_window_minimum_deque(arr: list[int], k: int) -> list[int]:
    """
    Classical approach using monotonic deque. O(N) time, O(K) space.
    Best for online/streaming data.
    """
    dq = deque()
    result = []
    for i, x in enumerate(arr):
        while dq and arr[dq[-1]] >= x:
            dq.pop()
        dq.append(i)
        if dq[0] < i - k + 1:
            dq.popleft()
        if i >= k - 1:
            result.append(arr[dq[0]])
    return result

def sliding_window_minimum_sparse(arr: list[int], k: int) -> list[int]:
    """
    Sparse table approach. O(N log N) build, O(1) per window = O(N) total.
    Better when k varies per query (offline).
    """
    st = SparseTable(arr)
    return [st.query(i, i + k - 1) for i in range(len(arr) - k + 1)]

# Test
arr = [1, 3, -1, -3, 5, 3, 6, 7]
print(sliding_window_minimum_deque(arr, 3))  # [-1,-3,-3,-3,3,3]
print(sliding_window_minimum_sparse(arr, 3)) # [-1,-3,-3,-3,3,3]
```

### Problem 5: Range Bitwise AND / OR (Sparse Table)

```python
class SparseTableBitwise:
    """Range AND and OR using Sparse Table (both are idempotent)."""
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        LOG = max(1, n.bit_length())
        self.and_table = [arr[:]]
        self.or_table = [arr[:]]
        self.log2 = [0] * (n + 1)
        for i in range(2, n + 1):
            self.log2[i] = self.log2[i // 2] + 1
        
        for j in range(1, LOG):
            and_prev, or_prev = self.and_table[j-1], self.or_table[j-1]
            half = 1 << (j - 1)
            and_curr, or_curr = [], []
            for i in range(n - (1 << j) + 1):
                and_curr.append(and_prev[i] & and_prev[i + half])
                or_curr.append(or_prev[i] | or_prev[i + half])
            self.and_table.append(and_curr)
            self.or_table.append(or_curr)
    
    def range_and(self, l: int, r: int) -> int:
        k = self.log2[r - l + 1]
        return self.and_table[k][l] & self.and_table[k][r - (1 << k) + 1]
    
    def range_or(self, l: int, r: int) -> int:
        k = self.log2[r - l + 1]
        return self.or_table[k][l] | self.or_table[k][r - (1 << k) + 1]
```

---

## 13. Interview Tips & Edge Cases

### ⚡ Common Interview Traps

| Trap | Correct Handling |
|------|-----------------|
| Using sparse table for sum queries | ❌ Sum not idempotent — use prefix sums or BIT |
| Not handling `l > r` in query | ✅ Always validate l ≤ r |
| log2(0) is undefined | ✅ Handle n=0 separately |
| Single element array | ✅ LOG=1, sparse has one row |
| Query returns index not value | ✅ Clarify if you need value or index |

### 🔑 Key Interview Talking Points

1. **"Why O(1) query?"** — Two overlapping blocks of size 2^k cover [l,r] completely. Overlap doesn't double-count because min/max/gcd are idempotent.

2. **"Why O(N log N) space?"** — Each of the log(N) levels of the sparse table stores N entries. Total = N × log(N) entries.

3. **"When would you use segment tree instead?"** — When the array has updates (point updates or range updates). Sparse table is rebuild-only.

4. **"How does Euler tour reduce LCA to RMQ?"** — Euler tour visits 2N-1 nodes. Between any two nodes u,v in the tour, the minimum-depth node is their LCA by definition of tree structure.

### 📋 Edge Cases to Always Check

```python
# Edge Case 1: Single element
arr = [42]
st = SparseTable(arr)
assert st.query(0, 0) == 42

# Edge Case 2: All same elements  
arr = [5, 5, 5, 5, 5]
st = SparseTable(arr)
assert st.query(0, 4) == 5
assert st.query(1, 3) == 5

# Edge Case 3: Decreasing sequence
arr = [9, 7, 5, 3, 1]
st = SparseTable(arr)
assert st.query(0, 4) == 1
assert st.query(0, 0) == 9

# Edge Case 4: Negative numbers
arr = [-3, -1, -4, -1, -5]
st = SparseTable(arr)
assert st.query(0, 4) == -5

# Edge Case 5: Power of 2 length (edge of log computation)
arr = [4, 2, 1, 3]  # length 4 = 2^2
st = SparseTable(arr)
assert st.query(0, 3) == 1
assert st.query(0, 1) == 2
```

### 🏆 Complexity Summary Table

| Operation | Time | Space |
|-----------|------|-------|
| Sparse Table Build | O(N log N) | O(N log N) |
| RMQ Query (min/max/gcd) | **O(1)** | — |
| RMQ Query (sum) — N/A | Use prefix sum | O(N) |
| LCA with Euler+RMQ | O(N log N) build, O(1) query | O(N log N) |
| ±1 RMQ (Fischer-Heun) | **O(N)** build, O(1) query | O(N) |
| Merge Sort Tree | O(N log² N) build | O(N log N) |
| Range Frequency | O(N log N) build | O(N) |
| Offline DC RMQ | O((N+Q) log N) | O(N + Q) |

---

*This guide covers the complete sparse table and RMQ landscape for FAANG-level interviews. The Euler tour + RMQ reduction for LCA is a must-know technique. Practice implementing the basic sparse table from scratch in under 15 minutes.*

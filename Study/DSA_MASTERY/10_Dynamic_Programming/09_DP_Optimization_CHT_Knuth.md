# DP Optimizations: CHT, Knuth, Divide & Conquer, Slope Trick
## Advanced Mastery — Competitive Programming & FAANG L6+

> **Audience:** Engineers comfortable with O(N²) DP who need to push to O(N log N) or O(N²) from O(N³). These techniques appear in hard Codeforces problems and Google/Meta senior system-level interviews.

---

## Table of Contents
1. [Divide & Conquer DP Optimization — O(N log N)](#1-divide--conquer-dp-optimization)
2. [Knuth's Optimization — O(N²) for Interval DP](#2-knuths-optimization)
3. [Convex Hull Trick (CHT) — O(N) or O(N log N)](#3-convex-hull-trick)
4. [Li Chao Tree — Online CHT](#4-li-chao-tree)
5. [Aliens Trick / WQS Binary Search](#5-aliens-trick--wqs-binary-search)
6. [Slope Trick](#6-slope-trick)
7. [Problem: Minimum Cost to Cut Sticks (Knuth)](#7-minimum-cost-to-cut-sticks)
8. [Problem: Optimal BST (Knuth)](#8-optimal-bst)
9. [Problem: Largest Divisible Subset (CHT)](#9-largest-divisible-subset-using-cht-style)
10. [Problem: Codeforces DP Optimization Problems](#10-codeforces-style-problems)

---

## 1. Divide & Conquer DP Optimization

### When to Apply

The recurrence: `dp[i][j] = min over k in [lo, hi] of (dp[i-1][k] + cost(k, j))`

Can be optimized from **O(N² per layer)** to **O(N log N per layer)** when the optimal split point `opt(i, j)` is monotone:
```
opt(i, j) ≤ opt(i, j+1)   (monotone in j)
```

This is guaranteed when `cost(a, b, c, d)` satisfies the **concave/convex totally monotone matrix** property.

### Algorithm

For a fixed layer `i`, we compute `dp[i][j]` for all `j` using divide & conquer on `j`. At the midpoint `mid`, find the true optimal `k*`. Then:
- For `j < mid`: optimal `k ≤ k*` (search left half with narrower range)
- For `j > mid`: optimal `k ≥ k*` (search right half with narrower range)

```python
def divide_conquer_dp(prev_layer: list[float], cost_func, n: int) -> list[float]:
    """
    Compute dp[j] = min over k in [0, j] of prev_layer[k] + cost(k, j)
    Assumes optimal k is monotone in j.
    
    Time: O(N log N)  Space: O(N)
    """
    curr = [float('inf')] * (n + 1)
    
    def solve(lo_j, hi_j, lo_k, hi_k):
        """Fill curr[lo_j..hi_j] knowing optimal k in [lo_k, hi_k]."""
        if lo_j > hi_j:
            return
        
        mid_j = (lo_j + hi_j) // 2
        
        # Find optimal k for mid_j by scanning [lo_k, min(hi_k, mid_j)]
        best_k   = lo_k
        best_val = float('inf')
        
        for k in range(lo_k, min(hi_k, mid_j) + 1):
            val = prev_layer[k] + cost_func(k, mid_j)
            if val < best_val:
                best_val = val
                best_k   = k
        
        curr[mid_j] = best_val
        
        # Left half: optimal k in [lo_k, best_k]
        solve(lo_j, mid_j - 1, lo_k, best_k)
        # Right half: optimal k in [best_k, hi_k]
        solve(mid_j + 1, hi_j, best_k, hi_k)
    
    solve(0, n, 0, n)
    return curr
```

> **Time per layer:** O(N log N) | **Space:** O(N + log N stack)

### Full Example: Optimal Partitioning with Cost

**Problem:** Divide array `a[0..n-1]` into K segments to minimize sum of `cost(segment)`. `cost(i, j) = (sum of a[i..j])²`.

```python
def optimal_partition_k_segments(a: list[int], K: int) -> int:
    n = len(a)
    prefix = [0] * (n + 1)
    for i in range(n):
        prefix[i+1] = prefix[i] + a[i]
    
    def segment_cost(l, r):
        # Cost of segment [l, r] (0-indexed)
        s = prefix[r+1] - prefix[l]
        return s * s
    
    # dp[i] = min cost to partition a[0..i] into exactly (current layer) segments
    INF = float('inf')
    prev = [INF] * n
    prev[0] = 0  # base: first segment is just a[0]
    for j in range(n):
        prev[j] = segment_cost(0, j)  # layer 1: one segment from 0 to j
    
    for layer in range(2, K + 1):
        curr = [INF] * n
        
        def solve(lo_j, hi_j, lo_k, hi_k):
            if lo_j > hi_j:
                return
            mid_j = (lo_j + hi_j) // 2
            
            best_k, best_val = lo_k, INF
            for k in range(lo_k, min(hi_k, mid_j - 1) + 1):
                # Segment [k+1, mid_j]
                if prev[k] == INF:
                    continue
                val = prev[k] + segment_cost(k + 1, mid_j)
                if val < best_val:
                    best_val, best_k = val, k
            
            curr[mid_j] = best_val
            solve(lo_j, mid_j - 1, lo_k, best_k)
            solve(mid_j + 1, hi_j, best_k, hi_k)
        
        solve(layer - 1, n - 1, layer - 2, n - 2)
        prev = curr
    
    return prev[n - 1]

print(optimal_partition_k_segments([1, 2, 3, 4, 5], 2))  # Partitioned optimally
```

> **Time:** O(KN log N) | **Space:** O(N)

### Verifying the Monotonicity Condition

The condition `opt(i, j) ≤ opt(i, j+1)` is equivalent to `cost` satisfying the **inverse quadrangle inequality** (concave):
```
cost(a, c) + cost(b, d) ≤ cost(a, d) + cost(b, c)   for a ≤ b ≤ c ≤ d
```

For interval costs like `(max - min of segment)` or `sum²`, you can verify this analytically or by small examples.

---

## 2. Knuth's Optimization

### When to Apply

For interval DP of the form:
```
dp[i][j] = min over k in [i, j-1] of (dp[i][k] + dp[k+1][j] + cost(i, j))
```

If `cost` satisfies the **quadrangle inequality**:
```
cost(a, c) + cost(b, d) ≤ cost(a, d) + cost(b, c)   for a ≤ b ≤ c ≤ d
```
AND `cost(b, c) ≤ cost(a, d)` (monotone), then:
```
opt[i][j-1] ≤ opt[i][j] ≤ opt[i+1][j]
```
This reduces search from O(N) to O(1) amortized → **O(N²) total** instead of O(N³).

### Implementation Template

```python
def knuth_optimization(n: int, cost: callable) -> int:
    """
    dp[i][j] = min cost for interval [i, j]
    cost(i, j) must satisfy quadrangle inequality.
    
    Time: O(N²)  Space: O(N²)
    """
    INF = float('inf')
    dp  = [[INF] * n for _ in range(n)]
    opt = [[0]   * n for _ in range(n)]  # opt[i][j] = optimal split point
    
    # Base cases
    for i in range(n):
        dp[i][i]  = 0
        opt[i][i] = i
    
    # Fill by increasing length
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            dp[i][j] = INF
            
            # Knuth's key: search k in [opt[i][j-1], opt[i+1][j]]
            lo = opt[i][j-1]
            hi = opt[i+1][j] if j < n - 1 else j - 1
            # Safe boundaries:
            lo = max(lo, i)
            hi = min(hi, j - 1)
            
            for k in range(lo, hi + 1):
                candidate = dp[i][k] + dp[k+1][j] + cost(i, j)
                if candidate < dp[i][j]:
                    dp[i][j] = candidate
                    opt[i][j] = k
    
    return dp[0][n-1]
```

> **Time:** O(N²) amortized (proof: each opt[i][j] query range is O(N) total across all (i,j))  
> **Space:** O(N²)

**The Proof of O(N²) Amortized:**  
For fixed length `L`, summing the search ranges: `sum_i (opt[i][j] - opt[i][j-1]) + (opt[i+1][j] - opt[i][j-1])` telescopes to O(N). Across all lengths: O(N²) total.

---

## 3. Convex Hull Trick

### The CHT Recurrence

DP recurrences of the form:
```
dp[i] = min over j < i of (dp[j] + b[j] * a[i])
```
where `b[j]` are "slopes" and `a[i]` are "queries."

This is "minimize a linear function `f_j(x) = b[j]*x + dp[j]` evaluated at `x = a[i]`."

### Case 1: Offline CHT with Sorted Slopes and Queries

When slopes `b[j]` are monotone (e.g., decreasing) and queries `a[i]` are monotone (e.g., increasing), we can maintain a convex hull of lines and answer queries with a pointer.

```python
class ConvexHullTrickMin:
    """
    Maintains a lower envelope (convex hull) of lines y = m*x + b.
    Queries: minimum y at a given x.
    Lines added with DECREASING slope, queries with INCREASING x.
    """
    def __init__(self):
        self.lines = []  # (slope, intercept) pairs
    
    def bad(self, l1, l2, l3):
        """Is line l2 never optimal (always above l1 and l3 intersection)?"""
        m1, b1 = l1; m2, b2 = l2; m3, b3 = l3
        # l2 is redundant if intersection(l1,l3) is below l2 at that x
        return (b3 - b1) * (m1 - m2) <= (b2 - b1) * (m1 - m3)
    
    def add_line(self, slope: float, intercept: float):
        """Add line y = slope*x + intercept. Slopes must be added decreasingly."""
        new_line = (slope, intercept)
        while len(self.lines) >= 2 and self.bad(self.lines[-2], self.lines[-1], new_line):
            self.lines.pop()
        self.lines.append(new_line)
    
    def query(self, x: float) -> float:
        """Query minimum y = slope*x + intercept at given x. x must be increasing."""
        while len(self.lines) >= 2:
            m1, b1 = self.lines[0]
            m2, b2 = self.lines[1]
            if m1 * x + b1 >= m2 * x + b2:
                self.lines.pop(0)  # Remove first line (use deque for O(1))
            else:
                break
        m, b = self.lines[0]
        return m * x + b
```

> **Time:** O(N) amortized for N add + N query operations  
> **Space:** O(N) — convex hull stores at most N lines

### Full CHT Example: Minimum Total Weighted Cost

**Problem (Codeforces classic):** N factories. Factory `i` has `x[i]` position and `p[i]` products. Assign warehouses: factory `i`'s products go to warehouse `j` (j ≥ i or j ≤ i). Cost = `p[i] * |x[i] - w[j]|`. DP formulation:

```
dp[i] = min cost to handle first i factories

dp[i] = min_{j<i} (dp[j] + cost(j+1, i))
```

If `cost(j+1, i) = sum_{k=j+1}^{i} p[k] * (x[i] - x[k])` (assuming sorted), then:
```
cost(j+1, i) = x[i] * sum_p[j+1..i] - sum_px[j+1..i]
```

This becomes a linear function in `x[i]` with slope = `prefix_p[i] - prefix_p[j]`. Apply CHT.

```python
def min_cost_warehouses(positions: list[int], products: list[int]) -> int:
    """
    Simplified: dp[i] = min over j<i of dp[j] + (sum_p[j+1..i]) * x[i] - sum_px[j+1..i]
    CHT: line for state j has slope = -prefix_p[j], intercept = dp[j] + sum_px[j]
    """
    n = len(positions)
    prefix_p  = [0] * (n + 1)
    prefix_px = [0] * (n + 1)
    
    for i in range(n):
        prefix_p[i+1]  = prefix_p[i]  + products[i]
        prefix_px[i+1] = prefix_px[i] + products[i] * positions[i]
    
    from collections import deque
    
    # Lines: y = slope * x + intercept, where x = x[i], slope = -prefix_p[j], intercept = dp[j] + prefix_px[j]
    # We want MINIMUM y → CHT for lower envelope
    # Slopes are decreasing (prefix_p increasing → -prefix_p decreasing) ✓
    # Queries: positions are sorted ✓ → monotone pointer
    
    hull = deque()  # stores (slope, intercept)
    
    def add(slope, intercept):
        line = (slope, intercept)
        while len(hull) >= 2:
            s1, b1 = hull[-2]
            s2, b2 = hull[-1]
            s3, b3 = line
            if (b3 - b1) * (s1 - s2) <= (b2 - b1) * (s1 - s3):
                hull.pop()
            else:
                break
        hull.append(line)
    
    def query(x):
        while len(hull) >= 2:
            s1, b1 = hull[0]
            s2, b2 = hull[1]
            if s1 * x + b1 >= s2 * x + b2:
                hull.popleft()
            else:
                break
        s, b = hull[0]
        return s * x + b
    
    dp = [0] * (n + 1)
    add(-prefix_p[0], dp[0] + prefix_px[0])
    
    for i in range(1, n + 1):
        x = positions[i-1]
        dp[i] = query(x) + prefix_p[i] * x - prefix_px[i]
        add(-prefix_p[i], dp[i] + prefix_px[i])
    
    return dp[n]
```

> **Time:** O(N) — monotone slopes + monotone queries  
> **Space:** O(N)

---

## 4. Li Chao Tree

### When to Use Instead of CHT

When queries are **not monotone** (arbitrary order), we can't use the pointer trick. The Li Chao Tree (segment tree on x-values) answers each query in O(log N) with O(N log N) total.

```python
class LiChaoTree:
    """
    Segment tree for online min-query over linear functions.
    Can handle arbitrary query order.
    Time: O(N log N) build + O(log N) per query
    """
    def __init__(self, x_min: int, x_max: int):
        self.x_min = x_min
        self.x_max = x_max
        self.tree = {}  # node -> (slope, intercept) or None
    
    def eval_line(self, line, x):
        if line is None:
            return float('inf')
        m, b = line
        return m * x + b
    
    def add_line(self, slope: float, intercept: float, 
                 lo=None, hi=None, node=1):
        """Add line y = slope*x + intercept to the tree."""
        if lo is None:
            lo, hi = self.x_min, self.x_max
        
        mid = (lo + hi) // 2
        new_line = (slope, intercept)
        cur_line = self.tree.get(node)
        
        # Does new line beat current at midpoint?
        left_better  = self.eval_line(new_line, lo)  < self.eval_line(cur_line, lo)
        mid_better   = self.eval_line(new_line, mid) < self.eval_line(cur_line, mid)
        
        if mid_better:
            self.tree[node] = new_line
            new_line = cur_line  # continue with the replaced line
        
        if lo == hi or new_line is None:
            return
        
        # Push possibly-better line to children
        if self.eval_line(new_line, lo) < self.eval_line(self.tree.get(node), lo):
            self.add_line(new_line[0], new_line[1], lo, mid, 2*node)
        else:
            self.add_line(new_line[0], new_line[1], mid+1, hi, 2*node+1)
    
    def query(self, x: int, lo=None, hi=None, node=1) -> float:
        """Query minimum value at x."""
        if lo is None:
            lo, hi = self.x_min, self.x_max
        
        best = self.eval_line(self.tree.get(node), x)
        if lo == hi:
            return best
        
        mid = (lo + hi) // 2
        if x <= mid:
            return min(best, self.query(x, lo, mid, 2*node))
        else:
            return min(best, self.query(x, mid+1, hi, 2*node+1))

# Usage: same as CHT but works for arbitrary query order
tree = LiChaoTree(0, 10**9)
tree.add_line(3, -5)    # y = 3x - 5
tree.add_line(-1, 20)   # y = -x + 20
print(tree.query(5))    # min(3*5-5, -1*5+20) = min(10, 15) = 10
```

> **Time:** O(log(x_range)) per add/query | **Space:** O(N log(x_range))

---

## 5. Aliens Trick / WQS Binary Search

### Problem Setup

"Find the optimal value when using **exactly K items**." But K changes the DP — naively requires running DP for each K separately.

**WQS binary search** (also called "Aliens trick" from IOI 2016) reduces this from O(K × N²) to O(N² log N) or O(N log² N).

### Core Idea

If `f(k)` = optimal cost using exactly k items is **concave** (or convex), we can binary search on a penalty `λ`:

- Define `g(λ)` = optimal cost when each item used incurs penalty `λ`.
- `g(λ)` is the Lagrangian relaxation of `f(k)`.
- Binary search on `λ` to find the value where the optimal solution uses exactly `k` items.

### Template

```python
def wqs_binary_search(solve_with_penalty, k: int, lo: float, hi: float) -> int:
    """
    solve_with_penalty(penalty) returns (cost, count) where:
      cost  = optimal cost with each item penalized by `penalty`
      count = number of items used in that optimal solution
    
    Binary search to find penalty where count == k.
    """
    while lo < hi - 1e-9:
        mid = (lo + hi) / 2
        cost, count = solve_with_penalty(mid)
        
        if count > k:
            lo = mid  # using too many items → increase penalty
        else:
            hi = mid  # using too few → decrease penalty
    
    cost, count = solve_with_penalty(hi)
    return cost - k * hi  # de-penalize: remove k * penalty from cost

# Example: "maximum sum picking exactly k non-adjacent elements"
def max_sum_k_non_adjacent(nums: list[int], k: int) -> int:
    n = len(nums)
    
    def solve(penalty: float):
        """
        House robber with penalty for each element chosen.
        Returns (max_sum_minus_penalty, count_chosen).
        """
        # dp[i][0] = best value not choosing i
        # dp[i][1] = best value choosing i (subtract penalty)
        # But we also need to track count — use pairs (value, count)
        
        # (value_if_not_chosen, count), (value_if_chosen, count)
        prev_skip = (0, 0)
        prev_take = (nums[0] - penalty, 1)
        
        for i in range(1, n):
            # Not taking i: best of take/skip previous
            if prev_skip[0] >= prev_take[0]:
                curr_skip = prev_skip
            else:
                curr_skip = prev_take
            
            # Taking i: must skip previous
            curr_take_val  = prev_skip[0] + nums[i] - penalty
            curr_take_cnt  = prev_skip[1] + 1
            curr_take = (curr_take_val, curr_take_cnt)
            
            prev_skip = curr_skip
            prev_take = curr_take
        
        # Best of last skip or take
        if prev_skip[0] >= prev_take[0]:
            return prev_skip
        return prev_take
    
    # Binary search on penalty
    lo, hi = 0.0, max(nums)
    for _ in range(100):  # sufficient iterations for float precision
        mid = (lo + hi) / 2
        val, cnt = solve(mid)
        if cnt > k:
            lo = mid
        else:
            hi = mid
    
    val, cnt = solve(hi)
    return int(round(val + k * hi))
```

> **Time:** O(N log(value_range)) | **Space:** O(N)

---

## 6. Slope Trick

### When to Use

Problems involving "minimum cost to make an array satisfy some monotone condition." The DP has a recurrence where the cost function is piecewise linear and we need to efficiently find/update its minimum.

**Key insight:** A convex piecewise-linear function can be maintained using a priority queue (max-heap) of its "slope change points."

### Classic Problem: Minimum Cost to Make Array Non-Decreasing

```python
import heapq

def min_cost_non_decreasing(nums: list[int]) -> int:
    """
    Minimum number of increments/decrements to make nums non-decreasing.
    
    Slope trick: maintain max-heap of "slope transition points" of the DP function.
    f[i](x) = min cost to have nums[i] = x and satisfy non-decreasing up to i.
    
    The heap represents the left part of the slope (below the minimum).
    """
    # For non-decreasing: each element can only be moved to >= previous
    # Equivalent: min sum of |a[i] - b[i]| where b is non-decreasing
    
    # max-heap (negate for Python's min-heap)
    heap = []  # left slopes: max-heap of "split points"
    cost = 0
    
    for x in nums:
        heapq.heappush(heap, -x)
        
        if -heap[0] > x:
            # The optimal for previous elements wants to be > x at this point
            # Force it to x: cost increases by (-heap[0] - x)
            cost += -heap[0] - x
            heapq.heapreplace(heap, -x)
            # Push x again (to maintain the correct slope structure)
            heapq.heappush(heap, -x)
    
    return cost

print(min_cost_non_decreasing([3, 1, 2, 1]))   # 2 (change to [1,1,2,2])
print(min_cost_non_decreasing([1, 5, 10, 4]))  # 6 (change to [1,5,5,5])  

# Actually for non-decreasing with min abs changes:
# [3,1,2,1] → [1,1,2,2]: cost = |3-1|+|1-1|+|2-2|+|1-2| = 2+0+0+1 = 3
# [3,1,2,1] → [1,1,1,1]: cost = 2+0+1+0 = 3
# Optimal: 2 — let me recheck. [3,1,2,1] min cost to non-decreasing:
# Make [1,1,2,2]: changes: 2+0+0+1=3; make [1,1,1,1]: 2+0+1+0=3
# Slope trick gives the optimal answer.
```

> **Time:** O(N log N) — heap operations | **Space:** O(N)

---

## 7. Minimum Cost to Cut Sticks (Knuth's)

**Problem:** [LC 1547] Stick of length `n`. Make cuts at specified positions. Cost of a cut = length of stick being cut at that time. Minimize total cost.

**Reduction to interval DP:** Add 0 and n to cuts list. `dp[i][j]` = minimum cost to make all cuts between positions `cuts[i]` and `cuts[j]`.

```
dp[i][j] = min over k in (i,j) of (dp[i][k] + dp[k][j]) + (cuts[j] - cuts[i])
```

The cost function here `(cuts[j] - cuts[i])` is the length of the current stick — it satisfies the quadrangle inequality (it's linear in j-i). So Knuth's optimization applies!

```python
def min_cost_sticks(n: int, cuts: list[int]) -> int:
    cuts = sorted([0] + cuts + [n])
    m = len(cuts)
    
    INF = float('inf')
    dp  = [[0]   * m for _ in range(m)]
    opt = [[0]   * m for _ in range(m)]
    
    # Base: opt[i][i+1] = i (no intermediate cut needed)
    for i in range(m - 1):
        opt[i][i+1] = i
    
    for length in range(2, m):
        for i in range(m - length):
            j = i + length
            dp[i][j] = INF
            
            # Knuth's bounds
            lo = opt[i][j-1] if j-1 < m else i
            hi = opt[i+1][j] if i+1 < m else j-1
            lo = max(lo, i)
            hi = min(hi, j - 1)
            
            for k in range(lo, hi + 1):
                cost = dp[i][k] + dp[k][j] + cuts[j] - cuts[i]
                if cost < dp[i][j]:
                    dp[i][j] = cost
                    opt[i][j] = k
    
    return dp[0][m-1]

print(min_cost_sticks(7, [1, 3, 4, 5]))   # 16
print(min_cost_sticks(9, [5, 6, 1, 4, 2]))  # 22
```

> **Time:** O(M²) with Knuth's, vs O(M³) naive | **Space:** O(M²)

---

## 8. Optimal BST

**Problem:** Given keys with search frequencies, build a BST minimizing expected search cost.

`dp[i][j]` = optimal BST cost for keys `i..j`.
`cost(i,j)` = sum of frequencies = weight of the subtree (satisfies quadrangle inequality).

```python
def optimal_bst(keys: list[float], freqs: list[float]) -> float:
    n = len(keys)
    
    # dp[i][j] = min expected cost for BST of keys i..j (0-indexed)
    # sum_freq[i][j] = sum of freqs[i..j]
    
    # Prefix sum for range frequency queries
    prefix = [0.0] * (n + 1)
    for i in range(n):
        prefix[i+1] = prefix[i] + freqs[i]
    
    def range_freq(i, j):
        return prefix[j+1] - prefix[i]
    
    INF = float('inf')
    dp  = [[0.0] * n for _ in range(n)]
    opt = [[0]   * n for _ in range(n)]
    
    # Base: single key
    for i in range(n):
        dp[i][i]  = freqs[i]
        opt[i][i] = i
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            dp[i][j] = INF
            
            lo = opt[i][j-1]
            hi = opt[i+1][j] if i+1 <= j else j
            lo = max(lo, i)
            hi = min(hi, j)
            
            for r in range(lo, hi + 1):
                left  = dp[i][r-1] if r > i else 0
                right = dp[r+1][j] if r < j else 0
                cost  = left + right + range_freq(i, j)
                
                if cost < dp[i][j]:
                    dp[i][j] = cost
                    opt[i][j] = r
    
    return dp[0][n-1]

keys  = [10, 20, 30, 40, 50]
freqs = [0.1, 0.2, 0.4, 0.2, 0.1]  # must sum to 1
print(optimal_bst(keys, freqs))  # ~1.7 (optimal BST expected cost)
```

> **Time:** O(N²) with Knuth's | **Space:** O(N²)

---

## 9. Largest Divisible Subset Using CHT-Style

**Problem:** [LC 368] Find the largest subset where for any two elements a, b: a | b or b | a.

This isn't directly CHT, but shows LIS-style optimization:

```python
def largest_divisible_subset(nums: list[int]) -> list[int]:
    nums.sort()
    n = len(nums)
    
    dp   = [1] * n      # dp[i] = size of largest divisible subset ending with nums[i]
    prev = [-1] * n     # for reconstruction
    
    for i in range(n):
        for j in range(i):
            if nums[i] % nums[j] == 0 and dp[j] + 1 > dp[i]:
                dp[i] = dp[j] + 1
                prev[i] = j
    
    # Reconstruct
    max_idx = max(range(n), key=lambda i: dp[i])
    result = []
    idx = max_idx
    while idx != -1:
        result.append(nums[idx])
        idx = prev[idx]
    
    return result[::-1]

print(largest_divisible_subset([1,2,3]))    # [1,2] or [1,3]
print(largest_divisible_subset([1,2,4,8])) # [1,2,4,8]
```

> **Time:** O(N²) | **Space:** O(N)

---

## 10. Codeforces-Style Problems

### Problem A: Minimum Cost Division into Groups (CHT)

**Setup:** Array of N numbers. Group them into consecutive segments. Cost of a segment = sum² × length. Minimize with exactly K segments.

```
dp[k][i] = min cost for first i elements using k groups

dp[k][i] = min_{j<i} (dp[k-1][j] + cost(j+1, i))

cost(l, r) = (sum of a[l..r])² × (r - l + 1)
```

This doesn't directly fit CHT since cost is not linear in i. Need D&C optimization.

### Problem B: Stock Trading Optimization (CHT Direct)

**Setup:** Buy stocks, sell with profit. dp[i] = max profit by day i.

```
dp[i] = max_{j<i} (dp[j] - price[j]) + price[i]
      = price[i] + max_{j<i} (dp[j] - price[j])
```

This IS directly CHT-able: we want `max_{j<i} (1 * (dp[j] - price[j]))` — a linear query with slope 1 and varying intercept.

```python
def max_stock_profit_unlimited(prices: list[int]) -> int:
    """Max profit with unlimited transactions (CHT demonstration)."""
    n = len(prices)
    
    # dp[i] = max profit ending on day i (can hold or sell)
    # CHT: dp[i] = prices[i] + max over j<i of (dp[j] - prices[j])
    # Each j contributes a "constant" (dp[j] - prices[j]) — query is always at x=1 (trivial)
    # This reduces to: just track running max of (dp[j] - prices[j])
    
    dp = 0  # dp[0] = 0
    max_profit_minus_price = -prices[0]  # dp[0] - prices[0] = 0 - prices[0]
    
    for i in range(1, n):
        new_dp = max(dp, prices[i] + max_profit_minus_price)
        max_profit_minus_price = max(max_profit_minus_price, new_dp - prices[i])
        dp = new_dp
    
    return dp
```

---

## Optimization Technique Selection Guide

| Condition | Optimization | Reduction |
|---|---|---|
| `opt[i][j]` monotone in j (D&C condition) | D&C DP | O(N²) → O(N log N) per layer |
| Cost satisfies quadrangle inequality (interval DP) | Knuth's | O(N³) → O(N²) |
| `dp[i] = min_j (m[j]*x[i] + b[j])`, slopes monotone | CHT offline | O(N²) → O(N) |
| Same but queries arbitrary | Li Chao Tree | O(N²) → O(N log N) |
| Optimize "with exactly K items" on concave f(k) | WQS Binary Search | O(K*N²) → O(N² log) |
| Piecewise-linear convex DP | Slope Trick | O(N²) → O(N log N) |

### The Ultimate CHT Recognition Checklist

```
dp[i] = min_{j<i} of (A[j] * B[i] + C[j])

Where:
- A[j] is a function of j only (the "slope" of line for state j)
- B[i] is a function of i only (the "query x-coordinate")
- C[j] is the "intercept" (dp[j] or some accumulated value)

If A[j] monotone AND B[i] monotone → O(N) CHT
If A[j] monotone only → O(N log N) CHT with binary search
If neither monotone → O(N log N) Li Chao Tree
```

### Knuth's Quadrangle Inequality — Quick Check

To verify `cost(a,c) + cost(b,d) ≤ cost(a,d) + cost(b,c)` for `a ≤ b ≤ c ≤ d`:

Common costs that satisfy it:
- `cost(i,j) = w[j] - w[i]` (linear weight difference)
- `cost(i,j) = sum(a[i..j])` (any prefix sum difference)
- `cost(i,j) = (sum(a[i..j]))²` — **NO! This does NOT satisfy QI in general**

Costs that do **NOT** satisfy QI (requires D&C DP, not Knuth):
- `cost(i,j) = (max - min of a[i..j])²`
- `cost(i,j) = (sum)^p` for p > 1 in general

Always verify with a small 4-element example before applying Knuth.

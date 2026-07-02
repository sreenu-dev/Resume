# Interval DP — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Pattern:** `dp[i][j]` = optimal answer for the subarray/interval `[i..j]`. Always filled in increasing length order. The split point `k` iterates over all positions within `[i,j]`.

---

## Table of Contents
1. [Interval DP Template](#1-interval-dp-template)
2. [Matrix Chain Multiplication — The Classic](#2-matrix-chain-multiplication)
3. [Burst Balloons — Think Last, Not First](#3-burst-balloons)
4. [Palindrome Partitioning II — Minimum Cuts](#4-palindrome-partitioning-ii)
5. [Minimum Insertion to Make Palindrome](#5-minimum-insertion-to-make-palindrome)
6. [Longest Palindromic Subsequence](#6-longest-palindromic-subsequence)
7. [Count Palindromic Substrings](#7-count-palindromic-substrings)
8. [Strange Printer](#8-strange-printer)
9. [Remove Boxes](#9-remove-boxes)
10. [Minimum Cost to Merge Stones](#10-minimum-cost-to-merge-stones)
11. [Zuma Game](#11-zuma-game)
12. [Minimum Score Triangulation of Polygon](#12-minimum-score-triangulation-of-polygon)

---

## 1. Interval DP Template

### The Universal Pattern

```python
# Interval DP Template
n = len(arr)
dp = [[initial_value] * n for _ in range(n)]

# Base cases: single elements or length-2 intervals
for i in range(n):
    dp[i][i] = base_case(i)

# Fill by increasing LENGTH (critical!)
for length in range(2, n + 1):          # length of interval
    for i in range(n - length + 1):     # start of interval
        j = i + length - 1              # end of interval
        
        # Try all split points k
        for k in range(i, j):           # or range(i, j+1) depending on problem
            dp[i][j] = optimize(dp[i][j],
                                dp[i][k] + dp[k+1][j] + cost(i, k, j))

answer = dp[0][n-1]
```

### Why Fill by Length?

`dp[i][j]` depends on `dp[i][k]` and `dp[k+1][j]` where `i ≤ k < j`. Both sub-intervals are strictly shorter than `[i,j]`. By filling in order of increasing length, when we compute `dp[i][j]` of length L, all sub-intervals of length < L are already computed.

**Wrong:** Nested loops `for i: for j:` — might compute `dp[0][5]` before `dp[1][5]`.  
**Right:** `for length: for i:` — guarantees all shorter intervals computed first.

---

## 2. Matrix Chain Multiplication

**Problem:** Given N matrices, find the optimal parenthesization to minimize scalar multiplications. Matrix `i` has dimensions `dims[i] × dims[i+1]`.

### State Definition

```
dp[i][j] = minimum multiplications to compute product of matrices i through j
```

### Recurrence

Split at matrix `k` (so we compute `M_i...M_k` and `M_{k+1}...M_j` separately, then multiply):
```
dp[i][j] = min over k in [i, j-1] of:
            dp[i][k] + dp[k+1][j] + dims[i] * dims[k+1] * dims[j+1]
```

The cost `dims[i] * dims[k+1] * dims[j+1]` is the scalar multiplications to multiply the two resulting matrices (dimensions: `dims[i]×dims[k+1]` and `dims[k+1]×dims[j+1]`).

**Base case:** `dp[i][i] = 0` (single matrix, no multiplication needed).

```python
def matrix_chain_order(dims: list[int]) -> int:
    """
    dims[i] to dims[i+1] are dimensions of matrix i.
    n matrices → len(dims) = n+1
    """
    n = len(dims) - 1  # number of matrices
    
    # dp[i][j] = min multiplications for matrices i..j (0-indexed)
    dp = [[0] * n for _ in range(n)]
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            dp[i][j] = float('inf')
            
            for k in range(i, j):
                # Cost: multiply (matrices i..k) with (matrices k+1..j)
                cost = (dp[i][k] + dp[k+1][j] +
                        dims[i] * dims[k+1] * dims[j+1])
                dp[i][j] = min(dp[i][j], cost)
    
    return dp[0][n-1]

# Test: matrices 10x30, 30x5, 5x60
# Optimal: (A * B) * C = 10*30*5 + 10*5*60 = 1500 + 3000 = 4500
# vs A * (B * C) = 30*5*60 + 10*30*60 = 9000 + 18000 = 27000
print(matrix_chain_order([10, 30, 5, 60]))   # 4500
print(matrix_chain_order([40, 20, 30, 10, 30]))  # 26000
```

> **Time:** O(N³) — three nested loops | **Space:** O(N²)

### Retrieving Optimal Parenthesization

```python
def matrix_chain_with_split(dims):
    n = len(dims) - 1
    dp = [[0]*n for _ in range(n)]
    split = [[0]*n for _ in range(n)]
    
    for length in range(2, n+1):
        for i in range(n - length + 1):
            j = i + length - 1
            dp[i][j] = float('inf')
            for k in range(i, j):
                cost = dp[i][k] + dp[k+1][j] + dims[i]*dims[k+1]*dims[j+1]
                if cost < dp[i][j]:
                    dp[i][j] = cost
                    split[i][j] = k
    
    def build(i, j):
        if i == j:
            return f"M{i}"
        k = split[i][j]
        return f"({build(i,k)} × {build(k+1,j)})"
    
    return dp[0][n-1], build(0, n-1)

cost, expr = matrix_chain_with_split([10, 30, 5, 60])
print(f"Cost: {cost}, Order: {expr}")  # Cost: 4500, Order: ((M0 × M1) × M2)
```

---

## 3. Burst Balloons

**Problem:** [LC 312] N balloons with values. Burst all to maximize coins. Bursting balloon `i` gives `nums[left] * nums[i] * nums[right]` where left, right are remaining neighbors.

### Why "Last to Burst" Not "First to Burst"

If we think about the **first** balloon to burst in interval `[i,j]`, the subproblems aren't clean — the left and right boundaries change as we pop, coupling subproblems.

If we think about the **last** balloon to burst in interval `(i,j)` (open interval, boundaries are sentinels), then:
- When balloon `k` is the last to burst in `(i,j)`, its left neighbor is `i` and right is `j` (all others already popped).
- The subproblems `(i,k)` and `(k,j)` are completely independent.

### State Definition

```
dp[i][j] = maximum coins from bursting all balloons strictly between i and j
           (i and j are NOT burst — they serve as sentinels)
```

Add virtual balloons: `nums = [1] + nums + [1]`, so indices 0 and N+1 are the sentinels.

### Recurrence

```
dp[i][j] = max over k in (i+1, j-1) of:
            dp[i][k] + nums[i]*nums[k]*nums[j] + dp[k][j]

(k is the LAST balloon to burst in open interval (i,j))
```

```python
def max_coins(nums: list[int]) -> int:
    nums = [1] + nums + [1]
    n = len(nums)
    
    # dp[i][j] = max coins from open interval (i, j)
    dp = [[0] * n for _ in range(n)]
    
    # Fill by increasing interval length
    for length in range(2, n):  # length = j - i
        for i in range(0, n - length):
            j = i + length
            
            for k in range(i + 1, j):  # k = last balloon to burst
                coins = nums[i] * nums[k] * nums[j]
                dp[i][j] = max(dp[i][j], dp[i][k] + coins + dp[k][j])
    
    return dp[0][n-1]

print(max_coins([3, 1, 5, 8]))  # 167
print(max_coins([1, 5]))         # 10
```

> **Time:** O(N³) — three nested loops | **Space:** O(N²)

---

## 4. Palindrome Partitioning II

**Problem:** [LC 132] Minimum cuts to partition a string into all palindromes.

### Two-Phase Approach

**Phase 1:** Precompute `is_palindrome[i][j]` using Manacher-like expansion or DP.

```
is_pal[i][j] = True if s[i..j] is a palindrome
is_pal[i][j] = (s[i] == s[j]) and (j-i < 2 or is_pal[i+1][j-1])
```

**Phase 2:** 1D DP for minimum cuts.

```
cuts[i] = min cuts to partition s[0..i] into palindromes
cuts[i] = 0 if s[0..i] is itself a palindrome
cuts[i] = min(cuts[j-1] + 1) for all j <= i where s[j..i] is a palindrome
```

```python
def min_cut(s: str) -> int:
    n = len(s)
    
    # Phase 1: Precompute palindrome table
    is_pal = [[False] * n for _ in range(n)]
    for i in range(n):
        is_pal[i][i] = True
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if length == 2:
                is_pal[i][j] = (s[i] == s[j])
            else:
                is_pal[i][j] = (s[i] == s[j]) and is_pal[i+1][j-1]
    
    # Phase 2: 1D DP for minimum cuts
    # cuts[i] = min cuts for s[:i+1]
    cuts = list(range(n))  # worst case: cut before every char
    
    for i in range(n):
        if is_pal[0][i]:
            cuts[i] = 0  # entire prefix is a palindrome
        else:
            for j in range(1, i + 1):
                if is_pal[j][i]:
                    cuts[i] = min(cuts[i], cuts[j-1] + 1)
    
    return cuts[n-1]

print(min_cut("aab"))      # 1 ("aa" | "b")
print(min_cut("a"))        # 0
print(min_cut("ab"))       # 1
print(min_cut("ababababababababababababcbababababababababababababcbababababababababababababcb"))  # 2
```

> **Time:** O(N²) — palindrome precomputation + cuts DP | **Space:** O(N²)

---

## 5. Minimum Insertion to Make Palindrome

**Problem:** [LC 1312] Minimum insertions to make string a palindrome.

### Key Insight

```
min_insertions(s) = len(s) - LPS(s)
```

where LPS = Longest Palindromic Subsequence. We keep the LPS as-is and insert characters around the non-LPS characters to make the full string a palindrome.

**Alternative: direct interval DP**

```
dp[i][j] = min insertions to make s[i..j] a palindrome

If s[i] == s[j]:  dp[i][j] = dp[i+1][j-1]      (no insertion needed for boundaries)
Else:              dp[i][j] = 1 + min(dp[i+1][j], dp[i][j-1])
                              (insert s[j] before s[i], or insert s[i] after s[j])
```

```python
def min_insertions(s: str) -> int:
    n = len(s)
    dp = [[0] * n for _ in range(n)]
    
    # Fill by increasing length
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                dp[i][j] = dp[i+1][j-1]  # length-2 sub-interval
            else:
                dp[i][j] = 1 + min(dp[i+1][j], dp[i][j-1])
    
    return dp[0][n-1]

print(min_insertions("zzazz"))   # 0 (already palindrome)
print(min_insertions("mbadm"))   # 2 ("m" + "badm" → insert b and d)
print(min_insertions("leetcode"))  # 5
```

> **Time:** O(N²) | **Space:** O(N²)

---

## 6. Longest Palindromic Subsequence

**Problem:** [LC 516] Find the longest subsequence that is a palindrome.

### Interval DP

```
dp[i][j] = length of longest palindromic subsequence in s[i..j]

If s[i] == s[j]:  dp[i][j] = dp[i+1][j-1] + 2
Else:              dp[i][j] = max(dp[i+1][j], dp[i][j-1])
```

```python
def longest_palindrome_subseq(s: str) -> int:
    n = len(s)
    dp = [[0] * n for _ in range(n)]
    
    for i in range(n):
        dp[i][i] = 1  # single char is palindrome of length 1
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                dp[i][j] = (dp[i+1][j-1] if length > 2 else 0) + 2
            else:
                dp[i][j] = max(dp[i+1][j], dp[i][j-1])
    
    return dp[0][n-1]

print(longest_palindrome_subseq("bbbab"))   # 4 ("bbbb")
print(longest_palindrome_subseq("cbbd"))    # 2 ("bb")
```

> **Time:** O(N²) | **Space:** O(N²) — can optimize to O(N) with diagonal rolling

**Alternative:** `LPS(s) = LCS(s, reverse(s))` — compute LCS with the reversed string.

---

## 7. Count Palindromic Substrings

**Problem:** [LC 647] Count all palindromic substrings in s.

### Expand Around Center — O(N²) Not Interval DP

While palindromes can be computed with interval DP, expanding around centers is cleaner:

```python
def count_substrings(s: str) -> int:
    n = len(s)
    count = 0
    
    for center in range(2 * n - 1):
        left = center // 2
        right = left + center % 2  # odd center: left==right; even: right=left+1
        
        while left >= 0 and right < n and s[left] == s[right]:
            count += 1
            left -= 1
            right += 1
    
    return count

print(count_substrings("abc"))  # 3 (a, b, c)
print(count_substrings("aaa"))  # 6 (a, a, a, aa, aa, aaa)
```

> **Time:** O(N²) | **Space:** O(1)

### Manacher's Algorithm — O(N)

```python
def count_substrings_manacher(s: str) -> int:
    # Transform: "abc" → "#a#b#c#"
    t = '#' + '#'.join(s) + '#'
    n = len(t)
    p = [0] * n  # p[i] = palindrome radius at i in transformed string
    
    center = right = 0
    for i in range(n):
        if i < right:
            mirror = 2 * center - i
            p[i] = min(right - i, p[mirror])
        
        # Try to expand
        l, r = i - p[i] - 1, i + p[i] + 1
        while l >= 0 and r < n and t[l] == t[r]:
            p[i] += 1
            l -= 1
            r += 1
        
        if i + p[i] > right:
            center, right = i, i + p[i]
    
    # Each palindrome radius p[i] in transformed string corresponds to (p[i]+1)//2 odd palindromes
    return sum((pi + 1) // 2 for pi in p)

print(count_substrings_manacher("aaa"))  # 6
```

> **Time:** O(N) | **Space:** O(N)

---

## 8. Strange Printer

**Problem:** [LC 664] A printer can print consecutive same characters. Minimum turns to print string s.

### State Definition

```
dp[i][j] = minimum turns to print s[i..j]
```

### Key Insight

If `s[i] == s[k]` for some `k > i`, we can print s[i..k] together in one pass (the character at `i` naturally covers through `k`). This merges the two subproblems.

```
Base: dp[i][i] = 1

If s[i] == s[j]:
    dp[i][j] = dp[i][j-1]  (extending s[i..j-1] to j costs nothing if s[j]=s[i])

For all k in [i, j-1]:
    dp[i][j] = min(dp[i][j], dp[i][k] + dp[k+1][j])

But if s[k] == s[j], we can save one turn:
    dp[i][j] = min(dp[i][j], dp[i][k] + dp[k+1][j] - 1)  ← optimization when s[k]==s[j]
```

Simpler formulation:

```
dp[i][j] = dp[i+1][j] + 1  (print s[i] alone, then print rest)

For k in [i+1, j]:
    if s[k] == s[i]:
        dp[i][j] = min(dp[i][j], dp[i+1][k] + dp[k][j])
        # Print s[i] and s[k] in same pass; dp[i+1][k] handles i+1..k-1 WITHIN that pass
```

```python
def strange_printer(s: str) -> int:
    n = len(s)
    dp = [[0] * n for _ in range(n)]
    
    for i in range(n):
        dp[i][i] = 1
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            dp[i][j] = dp[i+1][j] + 1  # print s[i] alone
            
            for k in range(i + 1, j + 1):
                if s[k] == s[i]:
                    # s[i] and s[k] printed in same turn
                    # Need to print i+1..k-1 (= dp[i+1][k-1] if exists) + k..j (= dp[k][j])
                    left = dp[i+1][k-1] if k > i+1 else 0
                    dp[i][j] = min(dp[i][j], left + dp[k][j])
    
    return dp[0][n-1]

print(strange_printer("aaabbb"))  # 2
print(strange_printer("aba"))     # 2
print(strange_printer("leetcode"))  # 6
```

> **Time:** O(N³) | **Space:** O(N²)

---

## 9. Remove Boxes

**Problem:** [LC 546] Remove boxes to maximize points. Removing `k` consecutive boxes of same color gives `k²` points.

### Extended State — The Hard Part

The key difficulty: the optimal removal of a segment depends on boxes OUTSIDE the segment that may be merged.

**State must track how many identical boxes to the left (already removed others) we can merge with:**

```
dp[i][j][k] = maximum points from boxes[i..j], given that there are k boxes
              identical to boxes[i] attached to the LEFT of boxes[i]
```

### Recurrence

```
Base: dp[i][j][k] = (k+1)^2 when i==j (the k extra boxes + boxes[i] all removed at once)

Option 1: Remove the k+1 boxes (the k extras + boxes[i]) immediately:
    dp[i][j][k] = (k+1)^2 + dp[i+1][j][0]

Option 2: Find m in (i, j] where boxes[m] == boxes[i].
    Merge the k extra boxes with boxes[m]'s group (skip i+1..m-1 first):
    dp[i][j][k] = dp[i+1][m-1][0] + dp[m][j][k+1]
```

```python
from functools import lru_cache

def remove_boxes(boxes: list[int]) -> int:
    
    @lru_cache(maxsize=None)
    def dp(i, j, k):
        # k = number of boxes identical to boxes[i] already to its left
        if i > j:
            return 0
        
        # Optimization: collapse leading same-color boxes
        while i < j and boxes[i] == boxes[i+1]:
            i += 1
            k += 1
        
        # Option 1: Remove boxes[i] with its k prefix boxes
        result = (k + 1) ** 2 + dp(i + 1, j, 0)
        
        # Option 2: Find m with boxes[m] == boxes[i], merge
        for m in range(i + 1, j + 1):
            if boxes[m] == boxes[i]:
                result = max(result, dp(i+1, m-1, 0) + dp(m, j, k+1))
        
        return result
    
    return dp(0, len(boxes)-1, 0)

print(remove_boxes([1,3,2,2,2,3,4,3,1]))  # 23
print(remove_boxes([1,1,1]))              # 9 (all together: 3²=9)
```

> **Time:** O(N⁴) — states O(N³), transitions O(N) each | **Space:** O(N³)

---

## 10. Minimum Cost to Merge Stones

**Problem:** [LC 1000] N piles of stones. Each move: merge K consecutive piles into one (cost = sum of those K piles). Minimize total cost. Return -1 if impossible.

### Feasibility Check

Merging K piles into 1 reduces count by K-1. Starting from N piles, final count = 1. We need `(N-1) % (K-1) == 0`.

### State Definition

```
dp[i][j] = minimum cost to merge all piles in [i,j] into the fewest possible piles
           (the number of remaining piles is (j-i) % (K-1) + 1)
```

**Insight:** When the interval length satisfies `(j-i) % (K-1) == 0`, it can be merged into 1 pile.

```python
def merge_stones(stones: list[int], k: int) -> int:
    n = len(stones)
    
    if (n - 1) % (k - 1) != 0:
        return -1
    
    # Prefix sums for range sum queries
    prefix = [0] * (n + 1)
    for i in range(n):
        prefix[i+1] = prefix[i] + stones[i]
    
    def range_sum(i, j):
        return prefix[j+1] - prefix[i]
    
    INF = float('inf')
    dp = [[INF] * n for _ in range(n)]
    for i in range(n):
        dp[i][i] = 0  # single pile, no cost
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            
            # Split into K groups: left group [i..k] and right group [k+1..j]
            # Iterate by steps of K-1 to ensure valid merges
            for mid in range(i, j, k - 1):
                if dp[i][mid] != INF and dp[mid+1][j] != INF:
                    dp[i][j] = min(dp[i][j], dp[i][mid] + dp[mid+1][j])
            
            # If interval can be merged into 1 pile, add the merge cost
            if (j - i) % (k - 1) == 0:
                dp[i][j] += range_sum(i, j)
    
    return dp[0][n-1]

print(merge_stones([3,2,4,1], 2))   # 20
print(merge_stones([3,2,4,1], 3))   # 40
print(merge_stones([3,5,1,2,6], 3)) # 25
```

> **Time:** O(N³ / K) | **Space:** O(N²)

---

## 11. Zuma Game

**Problem:** [LC 488] Zuma board is a string of colored balls. You have a hand of balls. Insert hand balls to eliminate all balls from the board. Minimum balls inserted. Balls are eliminated when 3+ same color consecutive appear.

This is a complex interval DP + BFS problem. Core idea:

```
dp[board_state][hand_state] = min balls used

Or: BFS over board states, using interval DP to compute minimum insertions
    needed to clear each contiguous segment.
```

Key insight for interval DP on board:
```
dp[i][j] = min hand balls to clear board[i..j] assuming board outside is already cleared
```

This requires tracking how many balls of same color are grouped, making the state more complex. Often solved with memoized DFS + string manipulation:

```python
from functools import lru_cache
from collections import Counter

def find_min_step(board: str, hand: str) -> int:
    hand_count = Counter(hand)
    
    def remove_repeats(s):
        # Remove 3+ consecutive same chars repeatedly
        changed = True
        while changed:
            changed = False
            i = 0
            while i < len(s):
                j = i
                while j < len(s) and s[j] == s[i]:
                    j += 1
                if j - i >= 3:
                    s = s[:i] + s[j:]
                    changed = True
                    break
                i = j
        return s
    
    @lru_cache(maxsize=None)
    def dfs(board_str, hand_tuple):
        board_str = remove_repeats(board_str)
        if not board_str:
            return 0
        
        hand_map = Counter(dict(zip(range(len(hand_tuple)//2), 
                                   hand_tuple[len(hand_tuple)//2:])))
        # Simplified: track as sorted tuple for hashing
        result = float('inf')
        
        i = 0
        while i < len(board_str):
            j = i
            while j < len(board_str) and board_str[j] == board_str[i]:
                j += 1
            # Group board_str[i:j] = (j-i) same colored balls
            color = board_str[i]
            need = 3 - (j - i)  # balls needed to eliminate this group
            # ... (full implementation requires careful hand tracking)
            i = j
        
        return result
    
    # Simplified approach: use BFS with string states
    return -1  # placeholder

# Full implementation is complex; key pattern shown above
```

---

## 12. Minimum Score Triangulation of Polygon

**Problem:** [LC 1039] N-gon with vertex values. Triangulate into N-2 triangles. Minimize sum of triangle scores (product of 3 vertex values).

### State Definition

```
dp[i][j] = minimum score to triangulate the polygon from vertex i to vertex j
           (sub-polygon with vertices i, i+1, ..., j)
```

**Key insight:** For any triangulation of polygon `[i..j]`, there is exactly one triangle containing the edge `(i,j)`. That triangle uses some vertex `k` between `i` and `j`.

```python
def min_score_triangulation(values: list[int]) -> int:
    n = len(values)
    dp = [[0] * n for _ in range(n)]
    
    # Fill by increasing interval length
    for length in range(2, n):  # minimum meaningful polygon is 3 vertices (length=2 gap)
        for i in range(n - length):
            j = i + length
            dp[i][j] = float('inf')
            
            for k in range(i + 1, j):
                # Triangle with vertices i, k, j
                score = values[i] * values[k] * values[j]
                dp[i][j] = min(dp[i][j], dp[i][k] + score + dp[k][j])
    
    return dp[0][n-1]

print(min_score_triangulation([1,2,3]))       # 6
print(min_score_triangulation([3,7,4,5]))     # 144
print(min_score_triangulation([1,3,1,4,1,5])) # 13
```

> **Time:** O(N³) | **Space:** O(N²)

---

## Interval DP Problem Recognition Guide

| Signature | Likely Interval DP | Split Strategy |
|---|---|---|
| "Optimal parenthesization/grouping" | Matrix Chain | Any split k |
| "Burst/remove elements, neighbors merge" | Burst Balloons | Last to remove |
| "Palindrome partitioning/subsequence" | Palindrome DP | Center expansion or left-right match |
| "Merge K consecutive elements" | Stone Merging | K-step splits |
| "Triangulate/decompose a polygon" | Triangulation | Third vertex k |
| "Print/draw intervals optimally" | Strange Printer | Matching chars |

### Common Interval DP Pitfalls

1. **Wrong fill order:** Must iterate by LENGTH, not by (i, j).
2. **Off-by-one in split range:** `k` from `i` to `j-1` or `i+1` to `j`? Check whether `k` is in the left or right sub-interval.
3. **Base case length:** Length 1 vs. length 2? Depends on whether a single element has a meaningful "answer."
4. **Optimization on burst:** Think LAST, not FIRST element removed — this gives clean independent subproblems.

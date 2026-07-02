# LCS, LIS, and Edit Distance — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Focus:** Two-sequence DP, patience sorting derivation for O(N log N) LIS, and the full spectrum from LCS to wildcard/regex matching.

---

## Table of Contents
1. [LCS — Full Derivation and Space Optimization](#1-lcs--longest-common-subsequence)
2. [LCS Variants — SCS, Min Deletions](#2-lcs-variants)
3. [LIS — O(N²) DP and O(N log N) Patience Sorting](#3-lis--longest-increasing-subsequence)
4. [LIS Variants — Triplet, Number of LIS, Russian Doll](#4-lis-variants)
5. [Edit Distance — Levenshtein Full Derivation](#5-edit-distance--levenshtein)
6. [Edit Distance Variants](#6-edit-distance-variants)
7. [Distinct Subsequences](#7-distinct-subsequences)
8. [Interleaving String](#8-interleaving-string)
9. [Wildcard Matching](#9-wildcard-matching)
10. [Regular Expression Matching](#10-regular-expression-matching)

---

## 1. LCS — Longest Common Subsequence

**Problem:** [LC 1143] Find the length of the longest subsequence common to both strings.

### State Definition

```
dp[i][j] = length of LCS of s1[:i] and s2[:j]
```

### Recurrence

Two cases based on whether the last characters match:

```
Case 1: s1[i-1] == s2[j-1]  →  dp[i][j] = dp[i-1][j-1] + 1
Case 2: s1[i-1] != s2[j-1]  →  dp[i][j] = max(dp[i-1][j], dp[i][j-1])
```

**Intuition:** If last chars match, they extend the LCS of the shorter strings. If not, at least one must be excluded — take the best of the two choices.

```python
def lcs(s1: str, s2: str) -> int:
    m, n = len(s1), len(s2)
    
    # Full 2D table for reconstruction capability
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    
    return dp[m][n]

print(lcs("abcde", "ace"))  # 3 ("ace")
print(lcs("abc", "abc"))    # 3
print(lcs("abc", "def"))    # 0
```

> **Time:** O(M × N) | **Space:** O(M × N)

### Space-Optimized LCS — O(N)

Since `dp[i][j]` depends on `dp[i-1][j-1]`, `dp[i-1][j]`, `dp[i][j-1]`, we need the previous row and one diagonal value.

```python
def lcs_optimized(s1: str, s2: str) -> int:
    m, n = len(s1), len(s2)
    # Use shorter string as columns for better space
    if m < n:
        s1, s2 = s2, s1
        m, n = n, m
    
    prev = [0] * (n + 1)
    
    for i in range(1, m + 1):
        curr = [0] * (n + 1)
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                curr[j] = prev[j-1] + 1    # diagonal
            else:
                curr[j] = max(prev[j], curr[j-1])  # above or left
        prev = curr
    
    return prev[n]
```

> **Time:** O(M × N) | **Space:** O(min(M, N))

### LCS Reconstruction

```python
def lcs_with_string(s1: str, s2: str) -> str:
    m, n = len(s1), len(s2)
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    
    # Backtrack
    result = []
    i, j = m, n
    while i > 0 and j > 0:
        if s1[i-1] == s2[j-1]:
            result.append(s1[i-1])
            i -= 1; j -= 1
        elif dp[i-1][j] >= dp[i][j-1]:
            i -= 1
        else:
            j -= 1
    
    return ''.join(reversed(result))

print(lcs_with_string("abcde", "ace"))  # "ace"
```

---

## 2. LCS Variants

### Shortest Common Supersequence

**Problem:** [LC 1092] Shortest string that has both s1 and s2 as subsequences.

```
Length of SCS = m + n - LCS(s1, s2)
(Include all chars of both, but shared LCS chars only once)
```

```python
def shortest_common_supersequence(s1: str, s2: str) -> str:
    m, n = len(s1), len(s2)
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    
    # Reconstruct SCS
    result = []
    i, j = m, n
    while i > 0 and j > 0:
        if s1[i-1] == s2[j-1]:
            result.append(s1[i-1])
            i -= 1; j -= 1
        elif dp[i-1][j] > dp[i][j-1]:
            result.append(s1[i-1]); i -= 1
        else:
            result.append(s2[j-1]); j -= 1
    
    while i > 0: result.append(s1[i-1]); i -= 1
    while j > 0: result.append(s2[j-1]); j -= 1
    
    return ''.join(reversed(result))

print(shortest_common_supersequence("abac", "cab"))  # "cabac"
```

> **Time:** O(M × N) | **Space:** O(M × N)

### Minimum Deletions to Make Two Strings Equal

```
Answer = m + n - 2 * LCS(s1, s2)
Delete (m - LCS) chars from s1 and (n - LCS) from s2
```

```python
def min_deletions_equal(s1: str, s2: str) -> int:
    return len(s1) + len(s2) - 2 * lcs(s1, s2)

print(min_deletions_equal("sea", "eat"))  # 2 (delete 's' from sea, 't' from eat)
```

> **Time:** O(M × N) | **Space:** O(N)

---

## 3. LIS — Longest Increasing Subsequence

### O(N²) DP Approach

**State:**
```
dp[i] = length of LIS ending at index i (i is always included)
```

**Recurrence:**
```
dp[i] = 1 + max(dp[j] for j < i if arr[j] < arr[i])
```

```python
def lis_n2(nums: list[int]) -> int:
    n = len(nums)
    dp = [1] * n  # every element is an LIS of length 1 by itself
    
    for i in range(1, n):
        for j in range(i):
            if nums[j] < nums[i]:
                dp[i] = max(dp[i], dp[j] + 1)
    
    return max(dp)
```

> **Time:** O(N²) | **Space:** O(N)

### O(N log N) — Patience Sorting (Full Derivation)

**The key data structure:** Maintain an array `tails` where `tails[k]` = the smallest tail element of all increasing subsequences of length `k+1`.

**Claim:** `tails` is always sorted in increasing order.

**Proof by induction:**  
- Base: trivially true.
- Inductive step: When we process `nums[i]`, we find the first position in `tails` where `tails[pos] >= nums[i]` (binary search). If `nums[i]` is larger than all, we append (new longer LIS). Otherwise, we replace `tails[pos]` with `nums[i]`. This preserves sortedness because: `tails[pos-1] < nums[i] <= tails[pos] <= tails[pos+1]`.

**Why `tails[pos]` replacement is correct:**  
We're NOT saying there's a valid LIS ending with `nums[i]` of length `pos+1` yet. We're saying: "for all LIS of length `pos+1`, the minimum possible tail is now `nums[i]`." This greedy choice maximizes future extension possibilities.

```python
import bisect

def lis_nlogn(nums: list[int]) -> int:
    tails = []  # tails[k] = smallest tail of LIS of length k+1
    
    for num in nums:
        # Find first tail >= num (bisect_left finds first >= for strictly increasing)
        pos = bisect.bisect_left(tails, num)
        
        if pos == len(tails):
            tails.append(num)    # num extends the longest IS found so far
        else:
            tails[pos] = num     # Replace: better (smaller) tail for LIS of length pos+1
    
    return len(tails)

# Test
print(lis_nlogn([10,9,2,5,3,7,101,18]))  # 4 ([2,3,7,18] or [2,3,7,101])
print(lis_nlogn([0,1,0,3,2,3]))  # 4
print(lis_nlogn([7,7,7,7,7]))  # 1 (strictly increasing!)
```

> **Time:** O(N log N) | **Space:** O(N)

### Reconstructing the LIS

```python
def lis_with_reconstruction(nums: list[int]) -> list[int]:
    n = len(nums)
    tails = []
    predecessors = [-1] * n
    indices = []  # indices[k] = index in nums of tails[k]
    
    for i, num in enumerate(nums):
        pos = bisect.bisect_left(tails, num)
        if pos == len(tails):
            tails.append(num)
            indices.append(i)
        else:
            tails[pos] = num
            indices[pos] = i
        
        predecessors[i] = indices[pos-1] if pos > 0 else -1
    
    # Reconstruct from last index
    result = []
    idx = indices[-1]
    while idx != -1:
        result.append(nums[idx])
        idx = predecessors[idx]
    
    return result[::-1]

print(lis_with_reconstruction([10,9,2,5,3,7,101,18]))  # [2,3,7,18]
```

> **Time:** O(N log N) | **Space:** O(N)

---

## 4. LIS Variants

### Increasing Triplet Subsequence — O(N), O(1)

**Problem:** [LC 334] Does an increasing triplet exist? (LIS length ≥ 3)

```python
def increasing_triplet(nums: list[int]) -> bool:
    first = second = float('inf')
    for num in nums:
        if num <= first:
            first = num
        elif num <= second:
            second = num
        else:
            return True  # found third > second > first
    return False
```

> **Time:** O(N) | **Space:** O(1)

### Number of LIS

**Problem:** [LC 673] Count the number of distinct longest increasing subsequences.

```python
def find_number_of_lis(nums: list[int]) -> int:
    n = len(nums)
    lengths = [1] * n   # lengths[i] = LIS length ending at i
    counts  = [1] * n   # counts[i]  = number of LIS of that length ending at i
    
    for i in range(1, n):
        for j in range(i):
            if nums[j] < nums[i]:
                if lengths[j] + 1 > lengths[i]:
                    lengths[i] = lengths[j] + 1
                    counts[i] = counts[j]
                elif lengths[j] + 1 == lengths[i]:
                    counts[i] += counts[j]
    
    max_len = max(lengths)
    return sum(c for l, c in zip(lengths, counts) if l == max_len)

print(find_number_of_lis([1,3,5,4,7]))  # 2 ([1,3,5,7] and [1,3,4,7])
print(find_number_of_lis([2,2,2,2,2]))  # 5
```

> **Time:** O(N²) | **Space:** O(N)

### Russian Doll Envelopes — 2D LIS

**Problem:** [LC 354] Envelopes (w, h). One fits in another if BOTH w and h are strictly smaller. Max envelopes you can nest.

**Trick:** Sort by width ascending, then by **height DESCENDING** for equal widths. Then LIS on heights.

**Why height descending for equal widths?** If two envelopes have equal width w, one cannot fit in the other. By sorting h descending, LIS on h won't pick two envelopes with equal w (LIS is strictly increasing, so it'll pick at most one from each equal-width group).

```python
def max_envelopes(envelopes: list[tuple[int,int]]) -> int:
    # Sort: width asc, then height DESC for equal widths
    envelopes.sort(key=lambda x: (x[0], -x[1]))
    
    # LIS on heights only
    heights = [h for _, h in envelopes]
    tails = []
    for h in heights:
        pos = bisect.bisect_left(tails, h)
        if pos == len(tails):
            tails.append(h)
        else:
            tails[pos] = h
    
    return len(tails)

print(max_envelopes([(5,4),(6,4),(6,7),(2,3)]))  # 3 ([2,3]→[5,4]→[6,7])
```

> **Time:** O(N log N) | **Space:** O(N)

### Longest Chain of Pairs

**Problem:** [LC 646] Chain of pairs where `a[1] < b[0]`. Maximize chain length.

**Greedy (like interval scheduling):** Sort by second element, greedily extend chain.

```python
def find_longest_chain(pairs: list[list[int]]) -> int:
    pairs.sort(key=lambda p: p[1])  # sort by end value
    curr_end = float('-inf')
    length = 0
    
    for start, end in pairs:
        if start > curr_end:
            length += 1
            curr_end = end
    
    return length
```

> **Time:** O(N log N) | **Space:** O(1)

---

## 5. Edit Distance — Levenshtein

**Problem:** [LC 72] Minimum operations (insert, delete, replace) to transform s1 into s2.

### State Definition

```
dp[i][j] = minimum edit distance between s1[:i] and s2[:j]
```

### Recurrence (Full Derivation)

**Case 1:** `s1[i-1] == s2[j-1]` → No operation needed:
```
dp[i][j] = dp[i-1][j-1]
```

**Case 2:** `s1[i-1] != s2[j-1]` → Three operations:
- **Replace** s1[i-1] with s2[j-1]: `dp[i-1][j-1] + 1`
- **Delete** s1[i-1]: `dp[i-1][j] + 1` (still need to match s1[:i-1] to s2[:j])
- **Insert** s2[j-1] after s1[:i]: `dp[i][j-1] + 1` (already matched s1[:i], need to add s2[j-1])

```
dp[i][j] = 1 + min(dp[i-1][j-1], dp[i-1][j], dp[i][j-1])
```

**Base cases:**
- `dp[i][0] = i` (delete all i chars of s1)
- `dp[0][j] = j` (insert all j chars of s2)

```python
def edit_distance(s1: str, s2: str) -> int:
    m, n = len(s1), len(s2)
    
    # Space-optimized: two rows
    prev = list(range(n + 1))  # dp[0][j] = j
    
    for i in range(1, m + 1):
        curr = [i] + [0] * n  # dp[i][0] = i
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                curr[j] = prev[j-1]
            else:
                curr[j] = 1 + min(prev[j-1],   # replace
                                   prev[j],      # delete
                                   curr[j-1])    # insert
        prev = curr
    
    return prev[n]

print(edit_distance("horse", "ros"))      # 3
print(edit_distance("intention", "execution"))  # 5
```

> **Time:** O(M × N) | **Space:** O(N) — rolling row

### Full DP Table Visualization

```
      ""  r  o  s
  ""   0  1  2  3
  h    1  1  2  3
  o    2  2  1  2
  r    3  2  2  2
  s    4  3  3  2
  e    5  4  4  3   ← answer: 3
```

Each cell: `min(diagonal+0/1, above+1, left+1)`.

---

## 6. Edit Distance Variants

### Minimum ASCII Delete Sum

**Problem:** [LC 712] Minimize sum of ASCII values of deleted characters to make s1 == s2.

**Insight:** Find maximum "keep" sum = Weighted LCS where weight is ASCII value. Delete everything else.

```python
def minimum_delete_sum(s1: str, s2: str) -> int:
    m, n = len(s1), len(s2)
    # dp[i][j] = max ASCII sum of common chars in s1[:i] and s2[:j]
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1] + ord(s1[i-1])
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    
    total = sum(ord(c) for c in s1) + sum(ord(c) for c in s2)
    return total - 2 * dp[m][n]

print(minimum_delete_sum("sea", "eat"))  # 231 (delete 's': 115, delete 't': 116)
```

> **Time:** O(M × N) | **Space:** O(M × N)

### Delete Operation for Two Strings

**Problem:** [LC 583] Min number of steps to make s1 == s2 (only deletions allowed).

```
Answer = len(s1) + len(s2) - 2 * LCS(s1, s2)
```

```python
def min_distance_delete(s1: str, s2: str) -> int:
    return len(s1) + len(s2) - 2 * lcs_optimized(s1, s2)

print(min_distance_delete("sea", "eat"))  # 2
```

> **Time:** O(M × N) | **Space:** O(N)

---

## 7. Distinct Subsequences

**Problem:** [LC 115] Count distinct subsequences of `s` that equal `t`.

### State Definition

```
dp[i][j] = number of distinct subsequences of s[:i] that equal t[:j]
```

### Recurrence

```
If s[i-1] == t[j-1]:
    dp[i][j] = dp[i-1][j-1]   # use s[i-1] to match t[j-1]
             + dp[i-1][j]     # skip s[i-1], still try to match t[:j] from s[:i-1]
Else:
    dp[i][j] = dp[i-1][j]     # must skip s[i-1]

Base: dp[i][0] = 1 (empty t matched by any prefix of s in exactly one way: skip all)
      dp[0][j>0] = 0 (non-empty t can't be matched by empty s)
```

```python
def num_distinct(s: str, t: str) -> int:
    m, n = len(s), len(t)
    if m < n:
        return 0
    
    # Space-optimized: one row (process j right to left to avoid overuse)
    dp = [0] * (n + 1)
    dp[0] = 1  # empty t
    
    for i in range(1, m + 1):
        # Reverse j to mimic not using s[i-1] for multiple t positions
        for j in range(min(i, n), 0, -1):
            if s[i-1] == t[j-1]:
                dp[j] += dp[j-1]
            # else: dp[j] unchanged (skip s[i-1])
    
    return dp[n]

print(num_distinct("rabbbit", "rabbit"))  # 3
print(num_distinct("babgbag", "bag"))     # 5
```

> **Time:** O(M × N) | **Space:** O(N)

---

## 8. Interleaving String

**Problem:** [LC 97] Is s3 an interleaving of s1 and s2? (Characters of s1 and s2 maintain relative order in s3.)

### State Definition

```
dp[i][j] = True if s3[:i+j] can be formed by interleaving s1[:i] and s2[:j]
```

### Recurrence

```
dp[i][j] = (dp[i-1][j] and s1[i-1] == s3[i+j-1])
         or (dp[i][j-1] and s2[j-1] == s3[i+j-1])
```

```python
def is_interleave(s1: str, s2: str, s3: str) -> bool:
    m, n = len(s1), len(s2)
    if m + n != len(s3):
        return False
    
    # Rolling row optimization
    dp = [False] * (n + 1)
    dp[0] = True
    
    # Initialize first row (using only s2 chars)
    for j in range(1, n + 1):
        dp[j] = dp[j-1] and s2[j-1] == s3[j-1]
    
    for i in range(1, m + 1):
        dp[0] = dp[0] and s1[i-1] == s3[i-1]
        for j in range(1, n + 1):
            dp[j] = (dp[j] and s1[i-1] == s3[i+j-1]) or \
                    (dp[j-1] and s2[j-1] == s3[i+j-1])
    
    return dp[n]

print(is_interleave("aabcc", "dbbca", "aadbbcbcac"))  # True
print(is_interleave("aabcc", "dbbca", "aadbbbaccc"))  # False
```

> **Time:** O(M × N) | **Space:** O(N)

---

## 9. Wildcard Matching

**Problem:** [LC 44] Pattern `p` with `?` (any single char) and `*` (any sequence of chars, including empty).

### State Definition

```
dp[i][j] = True if s[:i] matches p[:j]
```

### Recurrence

```
If p[j-1] == s[i-1] or p[j-1] == '?':
    dp[i][j] = dp[i-1][j-1]
    
If p[j-1] == '*':
    dp[i][j] = dp[i][j-1]    # '*' matches empty string
             or dp[i-1][j]   # '*' matches one more char of s
```

```python
def is_match_wildcard(s: str, p: str) -> bool:
    m, n = len(s), len(p)
    
    dp = [False] * (n + 1)
    dp[0] = True
    
    # Initialize: p consists of only stars → can match empty s
    for j in range(1, n + 1):
        if p[j-1] == '*':
            dp[j] = dp[j-1]
        else:
            break
    
    for i in range(1, m + 1):
        prev = dp[0]
        dp[0] = False  # non-empty s cannot match empty p
        
        for j in range(1, n + 1):
            temp = dp[j]
            if p[j-1] == '*':
                dp[j] = dp[j-1] or dp[j]   # empty match OR extend
            elif p[j-1] == '?' or p[j-1] == s[i-1]:
                dp[j] = prev                 # must match previous dp[i-1][j-1]
            else:
                dp[j] = False
            prev = temp
    
    return dp[n]

print(is_match_wildcard("aa", "a"))    # False
print(is_match_wildcard("aa", "*"))    # True
print(is_match_wildcard("cb", "?a"))   # False
print(is_match_wildcard("adceb", "*a*b"))  # True
```

> **Time:** O(M × N) | **Space:** O(N)

---

## 10. Regular Expression Matching

**Problem:** [LC 10] Pattern `p` with `.` (any single char) and `*` (zero or more of preceding char). **Full match** required.

### State Definition

```
dp[i][j] = True if s[:i] matches p[:j]
```

### Recurrence — The Star Case is Subtle

```
Case 1: p[j-1] == s[i-1] or p[j-1] == '.'
    dp[i][j] = dp[i-1][j-1]

Case 2: p[j-1] == '*'
    Sub-case A: Use '*' as ZERO occurrences of p[j-2]
        dp[i][j] |= dp[i][j-2]       (eliminate "x*" from pattern)
    
    Sub-case B: Use '*' to match one MORE char of s (if p[j-2] matches s[i-1])
        if p[j-2] == '.' or p[j-2] == s[i-1]:
            dp[i][j] |= dp[i-1][j]   (consume s[i-1], keep "x*" in pattern)
```

**Why `dp[i-1][j]` for extending?**  
`dp[i-1][j]` means: s[:i-1] matches p[:j]. Since `x*` matched the previous i-1 chars, and s[i-1] also matches `x`, we can extend: s[:i] matches p[:j].

```python
def is_match_regex(s: str, p: str) -> bool:
    m, n = len(s), len(p)
    
    # dp[i][j] = does s[:i] match p[:j]?
    dp = [[False] * (n + 1) for _ in range(m + 1)]
    dp[0][0] = True  # empty matches empty
    
    # Patterns like "a*", "a*b*", ".*" can match empty string
    for j in range(2, n + 1):
        if p[j-1] == '*':
            dp[0][j] = dp[0][j-2]  # use '*' as zero occurrences
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if p[j-1] == '*':
                # Zero occurrences: remove "x*"
                dp[i][j] = dp[i][j-2]
                # One more occurrence: p[j-2] must match s[i-1]
                if p[j-2] == '.' or p[j-2] == s[i-1]:
                    dp[i][j] = dp[i][j] or dp[i-1][j]
            elif p[j-1] == '.' or p[j-1] == s[i-1]:
                dp[i][j] = dp[i-1][j-1]
    
    return dp[m][n]

# Tests
print(is_match_regex("aa", "a"))      # False
print(is_match_regex("aa", "a*"))     # True (a* = two a's)
print(is_match_regex("ab", ".*"))     # True (.* = any char, any times)
print(is_match_regex("aab", "c*a*b")) # True (c*=0 c's, a*=2 a's, b=b)
print(is_match_regex("mississippi", "mis*is*p*.")) # False
```

> **Time:** O(M × N) | **Space:** O(M × N) — reducible to O(N) rolling

**Why this is harder than Wildcard:**  
`*` in regex doesn't stand alone — it refers to the PRECEDING character, creating a 2-character pattern unit `x*`. The `dp[i][j-2]` jump (skipping both `x` and `*`) handles the zero-occurrence case, which has no analogue in wildcard matching.

---

## Summary Table

| Problem | State | Key Transition | Complexity |
|---|---|---|---|
| LCS | `dp[i][j]` = LCS length | match→diag+1; no match→max(up,left) | O(MN), O(N) |
| SCS | `dp[i][j]` = LCS length | length = m+n-LCS | O(MN), O(N) |
| LIS (N²) | `dp[i]` = LIS ending at i | max over j<i where arr[j]<arr[i] | O(N²), O(N) |
| LIS (NlogN) | `tails` array | bisect_left + replace | O(N log N), O(N) |
| Russian Doll | sort + LIS on h | sort (w↑, h↓), then LIS on h | O(N log N), O(N) |
| Edit Distance | `dp[i][j]` = edit dist | match→diag; else→1+min(3 neighbors) | O(MN), O(N) |
| Distinct Subseq | `dp[i][j]` = count | match→diag+above; no match→above | O(MN), O(N) |
| Interleave | `dp[i][j]` = bool | match s1 or s2 at each step | O(MN), O(N) |
| Wildcard | `dp[i][j]` = bool | `*`→empty or extend | O(MN), O(N) |
| Regex | `dp[i][j]` = bool | `*`→zero(j-2) or extend if match | O(MN), O(MN) |

### The Critical Difference: LCS vs Edit Distance Recurrences

```
LCS: match → +1 from diagonal; no match → max(skip one from either)
Edit: match → copy diagonal; no match → 1 + min(replace/delete/insert)

LCS grows toward the top-left for matches.
Edit shrinks toward lower values everywhere.
```

# KMP & Z-Algorithm — Advanced Mastery Guide

> **Target Audience:** Engineers who know basic string matching and want O(N+M) pattern algorithms with proofs, advanced applications, and interview dominance.

---

## Table of Contents
1. [KMP — Failure Function Deep Dive](#kmp)
2. [Z-Algorithm — Full Derivation](#z-algorithm)
3. [KMP ↔ Z Equivalence](#equivalence)
4. [Advanced Applications](#applications)
5. [Problems 1–8 with Full Solutions](#problems)
6. [Interview Cheat Sheet](#cheat-sheet)

---

## 1. KMP — Knuth-Morris-Pratt <a name="kmp"></a>

### 1.1 The Core Insight

Naive matching is O(N·M) because after a mismatch at position `j` in the pattern, we restart from `j=0` — throwing away all information. **KMP never moves the text pointer backward.**

**Key invariant:** The *failure function* (LPS array) encodes, for each prefix `P[0..i]`, the length of the longest **proper** prefix that is also a suffix. This tells us exactly how far to slide the pattern without re-examining text characters.

### 1.2 Failure Function (LPS Array) — O(M) Construction

```
lps[i] = length of longest proper prefix of P[0..i] that is also a suffix
lps[0] = 0 always (no proper prefix exists for length-1 string)
```

**Two-pointer invariant proof:**
- Let `len` = length of current matching prefix-suffix
- `len` also equals `lps[i-1]` at the previous step
- At each step: if `P[i] == P[len]`, extend: `lps[i] = len+1`, advance both
- If `P[i] != P[len]` and `len > 0`: fall back to `len = lps[len-1]` (NOT to 0!)
  - **Why?** `P[0..lps[len-1]-1]` is guaranteed to be a prefix-suffix of `P[0..len-1]`, so it could still be extendable
- If `len == 0` and mismatch: `lps[i] = 0`, advance `i`

**Why is this O(M)?** `len` increases by at most 1 per iteration of `i`. The total number of decreases is bounded by total increases, which is at most M. So total work = O(M).

```python
def build_lps(pattern: str) -> list[int]:
    """
    Build failure function (LPS array) for KMP.
    Time: O(M), Space: O(M)
    """
    m = len(pattern)
    lps = [0] * m
    length = 0  # length of previous longest prefix suffix
    i = 1

    while i < m:
        if pattern[i] == pattern[length]:
            length += 1
            lps[i] = length
            i += 1
        else:
            if length != 0:
                # Key: don't increment i here — try shorter prefix
                length = lps[length - 1]
            else:
                lps[i] = 0
                i += 1
    return lps

# Example:
# Pattern: "AAACAAAA"
# LPS:     [0,1,2,0,1,2,3,3]
# Pattern: "AABAABAAA"
# LPS:     [0,1,0,1,2,3,4,5,2]
```

### 1.3 KMP Matching — O(N+M) with Proof

```python
def kmp_search(text: str, pattern: str) -> list[int]:
    """
    Find all occurrences of pattern in text.
    Time: O(N+M), Space: O(M) for LPS array
    Returns: list of starting indices (0-based)
    """
    n, m = len(text), len(pattern)
    if m == 0:
        return []

    lps = build_lps(pattern)
    result = []
    i = 0  # text pointer
    j = 0  # pattern pointer

    while i < n:
        if text[i] == pattern[j]:
            i += 1
            j += 1
        if j == m:
            result.append(i - j)
            j = lps[j - 1]  # slide: don't reset to 0
        elif i < n and text[i] != pattern[j]:
            if j != 0:
                j = lps[j - 1]
            else:
                i += 1
    return result

# Correctness proof:
# - i never decreases → text scanned at most once: O(N) text work
# - j can decrease, but each decrease follows lps chain, bounded by prior increases
# - Total j increments ≤ N (each matched character advances i once)
# - Total j decrements ≤ total j increments ≤ N
# - Combined with LPS build: O(N+M)
```

---

## 2. Z-Algorithm <a name="z-algorithm"></a>

### 2.1 Z-Array Definition

```
Z[i] = length of the longest substring starting from S[i] 
       that is also a prefix of S
Z[0] is conventionally undefined (or set to len(S))
```

**Example:**
```
S =    a  a  b  x  a  a  b  x  a
idx =  0  1  2  3  4  5  6  7  8
Z  =   9  1  0  0  3  1  0  0  1
```

### 2.2 Z-Function Construction — O(N)

**The Z-box maintenance trick:**
- Maintain a window `[l, r]` — the rightmost Z-box seen so far
- For new position `i`:
  - If `i <= r`: `Z[i] >= min(Z[i-l], r-i+1)` (mirror position `i-l` tells us minimum)
    - If `Z[i-l] < r-i+1`: mirror value is exact, no extension needed
    - If `Z[i-l] >= r-i+1`: we know at least `r-i+1`, extend from `r+1`
  - If `i > r`: start fresh comparison from 0

```python
def z_function(s: str) -> list[int]:
    """
    Compute Z-array for string s.
    Time: O(N), Space: O(N)
    
    Z[i] = length of longest common prefix of s and s[i:]
    """
    n = len(s)
    z = [0] * n
    z[0] = n  # by convention
    l, r = 0, 0  # current Z-box [l, r]

    for i in range(1, n):
        if i < r:
            # Use mirror: i's mirror in current Z-box is (i - l)
            z[i] = min(r - i, z[i - l])
        # Try to extend beyond r
        while i + z[i] < n and s[z[i]] == s[i + z[i]]:
            z[i] += 1
        # Update Z-box if we extended past r
        if i + z[i] > r:
            l, r = i, i + z[i]
    return z

# Why O(N)? The 'r' pointer only moves right, never left.
# Each character is compared at most twice: once when extending r, once via mirror.
# Total comparisons ≤ 2N → O(N).
```

### 2.3 Pattern Matching with Z-Algorithm

**Trick:** Concatenate `pattern + "$" + text` ($ = sentinel not in alphabet)

The `$` ensures Z-values in the text portion never exceed `len(pattern)`.

```python
def z_search(text: str, pattern: str) -> list[int]:
    """
    Find all occurrences of pattern in text using Z-algorithm.
    Time: O(N+M), Space: O(N+M)
    """
    if not pattern or not text:
        return []
    
    combined = pattern + "$" + text
    z = z_function(combined)
    m = len(pattern)
    result = []
    
    # Check positions in the text portion (offset = m+1)
    for i in range(m + 1, len(combined)):
        if z[i] == m:
            result.append(i - m - 1)  # convert to text index
    return result
```

---

## 3. KMP ↔ Z Equivalence <a name="equivalence"></a>

Both algorithms solve the same problem differently:

| Property | KMP (LPS) | Z-Algorithm |
|----------|-----------|-------------|
| What it stores | For each prefix, longest border | For each position, longest prefix match |
| Direction | Suffix → prefix matching | Prefix matching from position i |
| Implementation | Two pointers with fallback | Z-box window maintenance |
| Conversion | LPS → Z possible in O(N) | Z → LPS possible in O(N) |

**Converting Z to LPS:**
```python
def z_to_lps(z: list[int]) -> list[int]:
    """Convert Z-array to LPS array. Time: O(N)"""
    n = len(z)
    lps = [0] * n
    for i in range(1, n):
        if z[i] > 0:
            # z[i] characters starting at i match the prefix
            # So lps[i + z[i] - 1] = z[i] (if not already set to something larger)
            j = i + z[i] - 1
            if lps[j] == 0:
                lps[j] = z[i]
    # Fill forward for intermediate positions
    for i in range(1, n):
        if lps[i] == 0:
            lps[i] = max(lps[i-1] - 1, 0) if lps[i-1] > 1 else 0
    return lps
```

---

## 4. Advanced Applications <a name="applications"></a>

### 4.1 Shortest Period of a String

A string `s` has period `p` if `s[i] == s[i % p]` for all `i`.

**Theorem:** The shortest period of `s` (length N) is `N - lps[N-1]`, IF `N % (N - lps[N-1]) == 0`.

```python
def shortest_period(s: str) -> int:
    """
    Find shortest period of string s.
    Time: O(N), Space: O(N)
    """
    lps = build_lps(s)
    n = len(s)
    period = n - lps[n - 1]
    if n % period == 0:
        return period
    return n  # string itself is the shortest period

# "abcabcabc" → period = 3
# "abcabc"   → period = 3
# "abcd"     → period = 4 (no repetition)
```

### 4.2 String Compression

Check if string can be written as `t` repeated k times:

```python
def repeated_substring_pattern(s: str) -> bool:
    """
    LeetCode 459. Check if s = t * k for some t and k >= 2.
    Time: O(N), Space: O(N)
    """
    lps = build_lps(s)
    n = len(s)
    period = n - lps[n - 1]
    return period != n and n % period == 0

# Alternatively (elegant O(N) trick):
def repeated_substring_pattern_v2(s: str) -> bool:
    return s in (s + s)[1:-1]
```

### 4.3 String Rotation Check

```python
def is_rotation(s: str, t: str) -> bool:
    """
    Check if t is a rotation of s.
    Time: O(N), Space: O(N)
    """
    if len(s) != len(t):
        return False
    return bool(kmp_search(s + s, t))

# "abcde" rotated by 2 → "cdeab"
# kmp_search("abcdeabcde", "cdeab") → [2]
```

### 4.4 Shortest Palindrome (Prefix Extension)

**Problem:** Add minimum characters to the **front** of `s` to make it a palindrome.

**Key insight:** Find longest palindromic prefix of `s`. The remaining suffix reversed goes to front.

```python
def shortest_palindrome(s: str) -> str:
    """
    LeetCode 214. Time: O(N), Space: O(N)
    
    Strategy: Find longest prefix of s that is a palindrome.
    Use KMP on s + '#' + reverse(s)
    """
    rev = s[::-1]
    combined = s + '#' + rev
    lps = build_lps(combined)
    # lps[-1] = length of longest prefix of s that matches suffix of rev(s)
    # = length of longest palindromic prefix of s
    longest_palindrome_prefix_len = lps[-1]
    return rev[:len(s) - longest_palindrome_prefix_len] + s

# "aacecaaa" → "aaacecaaa"
# Longest palindromic prefix = "aacecaa" (len 7)
# Add rev(s)[0: 8-7] = "a" to front → "aaacecaaa"
```

---

## 5. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Count Occurrences of All Anagrams
**LeetCode 438 — Find All Anagrams in a String**

```python
from collections import Counter

def find_anagrams(s: str, p: str) -> list[int]:
    """
    Find all starting indices where p's anagram appears in s.
    Time: O(N), Space: O(1) — fixed alphabet size
    
    Sliding window with character frequency comparison.
    """
    if len(p) > len(s):
        return []
    
    p_count = Counter(p)
    window = Counter(s[:len(p)])
    result = []
    
    if window == p_count:
        result.append(0)
    
    for i in range(len(p), len(s)):
        # Add new character
        window[s[i]] += 1
        # Remove old character
        old = s[i - len(p)]
        window[old] -= 1
        if window[old] == 0:
            del window[old]
        
        if window == p_count:
            result.append(i - len(p) + 1)
    
    return result

# Z-algorithm approach: for each rotation of pattern, use Z-search
# But sliding window is cleaner for anagrams
```

---

### Problem 2: Lexicographically Smallest Rotation
**Find rotation of string s that is lexicographically smallest**

```python
def smallest_rotation(s: str) -> str:
    """
    Booth's algorithm — O(N) using failure function concept.
    Find starting position of lexicographically smallest rotation.
    
    Time: O(N), Space: O(N)
    """
    ss = s + s
    n = len(s)
    f = [-1] * (2 * n)
    k = 0  # current best start of smallest rotation
    
    for j in range(1, 2 * n):
        i = f[j - 1 - k]
        while i != -1 and ss[j] != ss[k + i + 1]:
            if ss[j] < ss[k + i + 1]:
                k = j - i - 1
            i = f[i]
        if ss[j] != ss[k + i + 1]:
            if ss[j] < ss[k]:
                k = j
            f[j - k] = -1
        else:
            f[j - k] = i + 1
    
    return ss[k:k + n]

# Simpler O(N log N) alternative: suffix array on s+s
def smallest_rotation_simple(s: str) -> str:
    """O(N log N) using Python's built-in sort"""
    n = len(s)
    rotations = [(s[i:] + s[:i], i) for i in range(n)]
    rotations.sort()
    return rotations[0][0]

# "bca" → rotations: ["abc"(1), "bca"(0), "cab"(2)] → "abc" at index 1
```

---

### Problem 3: Longest Happy Prefix
**LeetCode 1392 — Longest prefix which is also a suffix**

```python
def longest_prefix(s: str) -> str:
    """
    Direct application of LPS array.
    Time: O(N), Space: O(N)
    """
    lps = build_lps(s)
    length = lps[-1]
    return s[:length]

# "level" → lps = [0,0,0,0,1] → "l"
# "ababab" → lps = [0,0,1,2,3,4] → "abab"
```

---

### Problem 4: Number of Occurrences of Each Pattern (Multiple Queries)

```python
def count_all_patterns(text: str, patterns: list[str]) -> dict[str, int]:
    """
    Count occurrences of multiple patterns efficiently.
    Time: O(N*|patterns| + sum(M_i)) — use Aho-Corasick for better
    Space: O(max(M_i))
    """
    result = {}
    for pattern in patterns:
        occurrences = kmp_search(text, pattern)
        result[pattern] = len(occurrences)
    return result
```

---

### Problem 5: Minimum Characters to Add for Palindrome

```python
def min_chars_for_palindrome(s: str) -> int:
    """
    Minimum insertions at END to make palindrome.
    Mirror problem: find longest palindromic suffix.
    
    Time: O(N), Space: O(N)
    """
    # Reverse problem: add at front or end is symmetric
    # For adding at end: find longest palindromic suffix
    rev = s[::-1]
    combined = rev + '#' + s
    lps = build_lps(combined)
    # lps[-1] = longest palindromic suffix of s
    return len(s) - lps[-1]
```

---

### Problem 6: Detect if String is Rotation of Another Using KMP

```python
def string_rotation_kmp(s1: str, s2: str) -> bool:
    """
    LeetCode 796. Check if s2 is rotation of s1.
    Time: O(N), Space: O(N)
    """
    if len(s1) != len(s2):
        return False
    doubled = s1 + s1
    return len(kmp_search(doubled, s2)) > 0

# s1="abcde", s2="cdeab" → search "cdeab" in "abcdeabcde" → [2] → True
```

---

### Problem 7: Repeated Substring Pattern — Z-Function Approach

```python
def repeated_pattern_z(s: str) -> bool:
    """
    LeetCode 459 using Z-function.
    Time: O(N), Space: O(N)
    
    If s has period p (p < n), then z[p] = n - p.
    Check all divisors of n.
    """
    z = z_function(s)
    n = len(s)
    for p in range(1, n):
        if n % p == 0 and z[p] == n - p:
            return True
    return False

# More efficient: only check p = n - lps[n-1]
def repeated_pattern_kmp(s: str) -> bool:
    lps = build_lps(s)
    n = len(s)
    p = n - lps[n - 1]
    return p < n and n % p == 0
```

---

### Problem 8: Concatenated String Period Detection (Advanced)

```python
def find_all_periods(s: str) -> list[int]:
    """
    Find all valid periods of string s.
    A period p is valid if s[i] == s[i % p] for all i.
    
    Time: O(N log N) using Z-function
    Space: O(N)
    """
    z = z_function(s)
    n = len(s)
    periods = []
    
    for p in range(1, n + 1):
        if n % p == 0:
            # p is period if z[p] >= n - p (Z-box covers remainder)
            if p == n or z[p] >= n - p:
                periods.append(p)
    return periods

# "abababab" → periods: [2, 4, 8]
# "abcabc"  → periods: [3, 6]
# "abcd"    → periods: [4]
```

---

## 6. Interview Cheat Sheet <a name="cheat-sheet"></a>

### Complexity Summary

| Algorithm | Build | Search | Space |
|-----------|-------|--------|-------|
| Naive | — | O(N·M) | O(1) |
| KMP | O(M) | O(N) | O(M) |
| Z-function | O(N) | O(N+M) | O(N+M) |
| Rabin-Karp | O(M) | O(N) avg | O(M) |

### Decision Flowchart

```
Need pattern matching?
├── Single pattern in text → KMP or Z (same complexity, KMP slightly simpler to code)
├── Multiple patterns → Aho-Corasick
├── Approximate matching → Bitap / DP
└── 2D pattern matching → 2D Z-algorithm (apply 1D per row, then per column)
```

### KMP Common Mistakes

1. **Forgetting `lps[j-1]` after a full match** — must slide, not reset j=0
2. **LPS array is 0-indexed** — `lps[0] = 0` always
3. **Off-by-one in result index** — result index is `i - j` when `j == m`
4. **Not handling empty pattern** — check `if m == 0` first

### Z-Function Common Mistakes

1. **Not using sentinel character** — without `$`, Z-values in text can exceed pattern length
2. **Setting `z[0]`** — convention varies; set to `n` or `0` but be consistent
3. **Z-box update condition** — update when `i + z[i] > r`, not `>=`

### Advanced Pattern Recognition

| Problem Signature | Algorithm |
|-------------------|-----------|
| "Is X a rotation of Y?" | KMP on `X+X`, search Y |
| "Shortest palindrome by prepending" | KMP on `s + '#' + rev(s)` |
| "Does string have period p?" | `N % p == 0 and lps[N-1] == N - p` |
| "Count distinct borders" | Iterate LPS chain |
| "Find all occurrences" | KMP → list of starts |
| "Is string compression possible?" | `lps[N-1] != 0 and N % (N-lps[N-1]) == 0` |

### Interview Pro Tips

> **"Why KMP over Rabin-Karp?"** — KMP has no hash collision risk, O(N+M) **worst** case. Rabin-Karp is O(N+M) **average** but O(N·M) worst with bad hash.

> **"Can you do better than KMP?"** — For a single alphabet pattern: yes (Boyer-Moore is faster in practice with O(N/M) best case). For asymptotic O(N+M), KMP/Z is optimal for comparison-based matching.

> **"What's the LPS array physically?"** — It's a compressed representation of all the *borders* (prefix = suffix) of every prefix of the pattern. The longest border of the whole string = `lps[M-1]`.

---

## Edge Cases to Always Test

```python
# 1. Pattern longer than text
assert kmp_search("ab", "abc") == []

# 2. Pattern equals text
assert kmp_search("abc", "abc") == [0]

# 3. Overlapping matches
assert kmp_search("aaaa", "aa") == [0, 1, 2]

# 4. All same characters
s = "aaaa"
lps = build_lps(s)
assert lps == [0, 1, 2, 3]

# 5. No match
assert kmp_search("abcdef", "xyz") == []

# 6. Empty pattern
assert kmp_search("abc", "") == []

# 7. Single character pattern
assert kmp_search("abcabc", "a") == [0, 3]

# 8. Periodic string period check
assert repeated_pattern_kmp("abababab") == True
assert repeated_pattern_kmp("abcabcabc") == True
assert repeated_pattern_kmp("abcde") == False
```

---

## Complexity Proof Summary

**KMP is O(N+M):**
- LPS construction: `len` increases at most M times total, decreases bounded by increases → O(M)
- Matching: `i` increments N times, `j` increments at most N times, decrements at most N times → O(N)
- Total: O(N+M) ✓

**Z-function is O(N):**
- `r` only moves right, from 0 to N → `r` increments N times total
- Each character compared at most twice (once extending r, once as mirror confirmation)
- Total comparisons: ≤ 2N → O(N) ✓

---

*Next: [Aho-Corasick Multi-Pattern Matching →](02_Aho_Corasick.md)*

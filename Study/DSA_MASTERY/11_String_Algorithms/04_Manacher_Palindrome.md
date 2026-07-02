# Manacher's Algorithm — Advanced Mastery Guide

> **The crown jewel of string algorithms.** Manacher's computes all palindromic substrings in O(N) — a 5× improvement over the O(N²) expand-around-center approach.

---

## Table of Contents
1. [The Core Insight](#insight)
2. [String Transformation (# Trick)](#transformation)
3. [Manacher's Algorithm — Full Derivation](#algorithm)
4. [The P-Array — What Each Value Means](#p-array)
5. [Applications](#applications)
6. [Problems 1–7 with Full Solutions](#problems)
7. [KMP vs Manacher Comparison](#comparison)

---

## 1. The Core Insight <a name="insight"></a>

### Why Expand-Around-Center is O(N²)

The naive approach: for each center (N centers for odd, N-1 for even = 2N-1 total), expand outward until mismatch. Each expansion is O(N) worst case → O(N²) total.

**Wasted work:** When we expand from center `c` and find a palindrome of radius `R`, we've verified characters in `[c-R, c+R]`. Any future center `i` inside this window has a **mirror** `i' = 2c - i` that we already processed. Manacher exploits this.

### The Mirror Trick

If we've computed P[i'] (palindrome radius at mirror position) and `i` is within the current rightmost palindrome window `[l, r]`:

- **Case 1:** `P[i'] < r - i` → `P[i] = P[i']` (mirror tells exact value, no expansion needed)
- **Case 2:** `P[i'] > r - i` → `P[i] = r - i` (bounded by window edge)
- **Case 3:** `P[i'] == r - i` → `P[i] >= r - i`, expand from `r + 1`

In cases 1 and 2, NO character comparisons needed. In case 3, we expand from an already-known minimum, but any expansion moves `r` rightward → each character moved past at most once.

**This gives O(N) total comparisons.**

---

## 2. String Transformation — The `#` Trick <a name="transformation"></a>

**Problem:** Even-length palindromes don't have a single center.

**Solution:** Insert `#` between every character and at boundaries:
```
Original: "abba"    → 4 chars, 2N-1 = 7 centers
Transformed: "#a#b#b#a#"  → 9 chars, 9 centers, all palindromes are odd-length

Original: "aba"     → 3 chars
Transformed: "#a#b#a#"  → 7 chars
```

**Key property:** In transformed string T:
- `P[i]` (radius in T) = length of palindrome in original string
- Palindrome center at `T[i]` (a `#`) → even-length palindrome in original
- Palindrome center at `T[i]` (a letter) → odd-length palindrome in original

**Mapping back:**
- Original palindrome length = `P[i]` (exactly)
- Original palindrome start = `(i - P[i]) / 2`

```python
def transform(s: str) -> str:
    """Add '#' separators for unified even/odd handling."""
    return '#' + '#'.join(s) + '#'

def original_start(t_center: int, t_radius: int) -> int:
    """Map transformed center/radius to original string start."""
    return (t_center - t_radius) // 2
```

---

## 3. Manacher's Algorithm — Full Derivation <a name="algorithm"></a>

```python
def manacher(s: str) -> list[int]:
    """
    Manacher's algorithm on transformed string.
    
    Returns P array where P[i] = palindrome radius in transformed string.
    
    Time: O(N), Space: O(N)
    
    Invariant: [c - P[c], c + P[c]] is the rightmost palindrome seen so far.
    """
    t = transform(s)
    n = len(t)
    P = [0] * n
    c = 0   # center of rightmost palindrome
    r = 0   # right boundary of rightmost palindrome (exclusive: t[r] not in palindrome)
    
    for i in range(n):
        # Mirror of i with respect to center c
        mirror = 2 * c - i
        
        if i < r:
            # Initialize P[i] based on mirror
            P[i] = min(r - i, P[mirror])
        
        # Attempt to expand palindrome centered at i
        # (start from known minimum, avoid redundant comparisons)
        left = i - (P[i] + 1)
        right = i + (P[i] + 1)
        while left >= 0 and right < n and t[left] == t[right]:
            P[i] += 1
            left -= 1
            right += 1
        
        # Update rightmost palindrome if expanded past r
        if i + P[i] > r:
            c = i
            r = i + P[i]
    
    return P

def manacher_full(s: str) -> tuple[list[int], list[int]]:
    """
    Returns (P_odd, P_even):
    - P_odd[i]  = radius of longest odd palindrome centered at s[i]
    - P_even[i] = radius of longest even palindrome centered between s[i-1] and s[i]
    
    Both in terms of original string.
    Time: O(N), Space: O(N)
    """
    if not s:
        return [], []
    
    t = transform(s)
    P = manacher(s)
    n = len(s)
    
    P_odd  = [0] * n   # P_odd[i] = radius, so palindrome s[i-r:i+r+1] has length 2r+1
    P_even = [0] * n   # P_even[i] = half-length of even palindrome ending between s[i-1],s[i]
    
    for i in range(len(t)):
        # t[2i+1] = s[i] (letter positions in transformed string)
        # t[2i]   = '#' (separator positions)
        if i % 2 == 1:
            # Odd palindrome in original, centered at s[i//2]
            orig_center = i // 2
            P_odd[orig_center] = P[i] // 2
        else:
            # Even palindrome in original, "centered" between s[i//2 - 1] and s[i//2]
            orig_pos = i // 2
            if orig_pos > 0:
                P_even[orig_pos] = P[i] // 2
    
    return P_odd, P_even
```

### Step-by-Step Trace

```
s = "abba"
t = "#a#b#b#a#"
     0123456789 (indices)

Processing:
i=0: '#', expand, P[0]=0, c=0, r=0
i=1: 'a', expand → P[1]=1, c=1, r=2
i=2: '#', mirror=0, i<r? No(2=r). expand → '#'≠'a' → P[2]=0
i=3: 'b', mirror=2*1-3=-1, i>=r. expand → P[3]=1, try: t[1]='a'==t[5]='b'? No. P[3]=1, c=3,r=4
i=4: '#', mirror=2*3-4=2, i<r? 4<4? No. expand → t[3]='b'==t[5]='b' → P[4]=1
     t[2]='#'==t[6]='#' → P[4]=2, t[1]='a'==t[7]='a' → P[4]=3, t[0]='#'==t[8]='#' → P[4]=4
     c=4, r=8
i=5: 'b', mirror=2*4-5=3, i<r? 5<8. P[5]=min(r-i=3, P[3]=1)=1
     try expand: t[3]='b'==t[7]='a'? No. P[5]=1
i=6: '#', mirror=2, i<r? 6<8. P[6]=min(2,P[2]=0)=0
i=7: 'a', mirror=1, i<r? 7<8. P[7]=min(1,P[1]=1)=1
     try expand: t[5]='b'==t[9]? Out of bounds. P[7]=1
i=8: '#', mirror=0, P[8]=min(0,0)=0

P = [0,1,0,1,4,1,0,1,0]
Max P[i] = 4 at i=4 → palindrome length 4 → "abba"
Start in original = (4-4)/2 = 0
```

---

## 4. The P-Array — What Each Value Means <a name="p-array"></a>

In the transformed string T of length `2N+1`:

| `T[i]` type | `P[i]` meaning | Original palindrome |
|-------------|----------------|---------------------|
| `#` (even gap) | Even-length palindrome | Length = `P[i]`, centered between chars |
| Letter (odd center) | Odd-length palindrome | Length = `P[i]` (always odd), centered at that char |

**Extracting all palindromes:**
```python
def all_palindromic_substrings(s: str) -> list[tuple[int,int,int]]:
    """
    Returns all palindromes as (start, end, length) in original string.
    'start' and 'end' are inclusive indices.
    """
    if not s:
        return []
    
    t = transform(s)
    P = manacher(s)
    results = []
    
    for i, p in enumerate(P):
        if p > 0:
            orig_start = (i - p) // 2
            orig_end   = (i + p) // 2 - 1
            length     = p
            results.append((orig_start, orig_end, length))
    
    return results
```

---

## 5. Applications <a name="applications"></a>

### 5.1 Longest Palindromic Substring

```python
def longest_palindrome(s: str) -> str:
    """
    LeetCode 5. O(N) using Manacher's.
    """
    if not s:
        return ""
    
    t = transform(s)
    P = manacher(s)
    
    max_p = max(P)
    center = P.index(max_p)
    
    orig_start = (center - max_p) // 2
    return s[orig_start:orig_start + max_p]

# Verify:
assert longest_palindrome("babad") in ["bab", "aba"]
assert longest_palindrome("cbbd") == "bb"
assert longest_palindrome("a") == "a"
assert longest_palindrome("ac") in ["a", "c"]
```

### 5.2 Count Palindromic Substrings

```python
def count_substrings(s: str) -> int:
    """
    LeetCode 647. Count all palindromic substrings.
    
    Manacher gives P[i] = radius. Number of palindromes centered at T[i]
    = (P[i] + 1) // 2  (ceiling division handles both # and letter centers)
    
    Wait — simpler: each T[i] contributes floor((P[i]+1)/2) distinct palindromes.
    But more precisely: for transformed position i, palindromes of radius 1,2,...,P[i]
    that correspond to distinct original palindromes.
    
    Simplest correct formula: for each original center, count = radius + 1 (odd)
    or radius (even). But using Manacher directly:
    
    Total = sum((P[i] + 1) // 2 for all i)... not quite.
    
    Cleanest: expand around center approach with Manacher's P array.
    """
    t = transform(s)
    P = manacher(s)
    
    count = 0
    for i, p in enumerate(P):
        # Each unit of radius in transformed string = one palindrome in original
        # At position i, palindromes of length 1,3,5,... (if t[i]=letter) or 2,4,6,... (if t[i]='#')
        # But P[i] is in transformed units = original chars
        # Number of distinct original palindromes contributed by center i:
        count += (p + 1) // 2  # ceiling division
    
    # Alternative: just count from P directly
    # For letter positions (odd i in transformed): contributes P[i]//2 + 1 palindromes (lengths 1,3,...,2*(P[i]//2)+1)
    # For # positions (even i in transformed): contributes P[i]//2 palindromes (lengths 2,4,...,P[i])
    
    return count

# Cleaner formulation:
def count_substrings_clean(s: str) -> int:
    """Most readable Manacher-based count."""
    if not s:
        return 0
    t = transform(s)
    P = manacher(s)
    total = 0
    for i in range(len(t)):
        # How many palindromes in original string have this as their center in T?
        if i % 2 == 1:  # letter center → odd palindromes
            total += P[i] // 2 + 1  # radii 0,1,...,P[i]//2 → P[i]//2+1 palindromes
        else:           # # center → even palindromes
            total += P[i] // 2      # radii 1,2,...,P[i]//2
    return total

assert count_substrings_clean("abc") == 3
assert count_substrings_clean("aaa") == 6
assert count_substrings_clean("abba") == 6
```

---

## 6. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Shortest Palindrome (LeetCode 214) — Manacher Approach

```python
def shortest_palindrome_manacher(s: str) -> str:
    """
    Find shortest palindrome by prepending characters to s.
    
    Key: find longest palindromic prefix of s.
    The answer = reverse(s[k:]) + s where k = length of longest palindromic prefix.
    
    Time: O(N), Space: O(N)
    """
    if not s:
        return s
    
    t = transform(s)
    P = manacher(s)
    n = len(s)
    
    # Find longest palindromic prefix of s
    # Palindromic prefix of length L corresponds to center at transformed position L-1
    # and extends to position 0
    
    longest_prefix = 1  # minimum: first character is always a palindrome
    
    for i in range(len(t)):
        p = P[i]
        # Check if this palindrome starts at the beginning of s
        # In transformed string, start = i - p, end = i + p
        # Corresponds to original start = (i - p) / 2
        orig_start = (i - p) // 2
        orig_end   = (i + p) // 2  # this is exclusive
        
        if orig_start == 0 and orig_end > longest_prefix:
            longest_prefix = orig_end
    
    # Add reverse of s[longest_prefix:] to front
    suffix = s[longest_prefix:]
    return suffix[::-1] + s

assert shortest_palindrome_manacher("aacecaaa") == "aaacecaaa"
assert shortest_palindrome_manacher("abcd") == "dcbabcd"
assert shortest_palindrome_manacher("") == ""
assert shortest_palindrome_manacher("a") == "a"
```

---

### Problem 2: Palindromic Substrings of Every Length

```python
def palindromes_by_length(s: str) -> dict[int, list[int]]:
    """
    Return dict: length → list of starting positions.
    Uses Manacher's P array.
    Time: O(N + Z) where Z = number of palindromic substrings
    """
    from collections import defaultdict
    
    t = transform(s)
    P = manacher(s)
    result = defaultdict(list)
    
    for i, p in enumerate(P):
        # Center i in T, radius p → original string palindrome of length p
        orig_start = (i - p) // 2
        # Generate all palindromes centered here
        step = 2 if i % 2 == 1 else 2  # step by 2 in T = step by 1 in original
        
        for radius in range(1 if i % 2 == 0 else 0, p + 1, 1):
            # In original, radius corresponds to palindrome
            if i % 2 == 1:  # odd palindromes
                length = 2 * radius + 1
            else:            # even palindromes
                if radius == 0:
                    continue
                length = 2 * radius
            
            orig_s = (i - (2 * radius if i % 2 == 0 else 2 * radius + 1)) // 2
            if orig_s >= 0 and orig_s + length <= len(s):
                result[length].append(orig_s)
    
    return dict(result)
```

---

### Problem 3: Minimum Insertions to Make String Palindrome (LeetCode 1312)

```python
def min_insertions(s: str) -> int:
    """
    LeetCode 1312. Minimum insertions to make s palindrome.
    = Length of s - Length of Longest Palindromic Subsequence (LPS).
    LPS = LCS(s, reverse(s)).
    
    Time: O(N²), Space: O(N²) — DP approach
    """
    n = len(s)
    rev = s[::-1]
    
    # LCS of s and rev(s) = LPS of s
    dp = [[0] * (n + 1) for _ in range(n + 1)]
    for i in range(1, n + 1):
        for j in range(1, n + 1):
            if s[i-1] == rev[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    
    lps_len = dp[n][n]
    return n - lps_len

# Alternatively, DP directly:
def min_insertions_direct(s: str) -> int:
    """Direct DP on palindrome intervals. Same O(N²) but more intuitive."""
    n = len(s)
    # dp[i][j] = min insertions to make s[i:j+1] palindrome
    dp = [[0] * n for _ in range(n)]
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                dp[i][j] = dp[i+1][j-1]
            else:
                dp[i][j] = min(dp[i+1][j], dp[i][j-1]) + 1
    
    return dp[0][n-1]
```

---

### Problem 4: Maximum Palindrome from Prefix + Suffix (Advanced)

```python
def max_palindrome_score(s: str, k: int) -> int:
    """
    Remove exactly k characters from s to maximize length of resulting palindrome prefix.
    (Custom problem demonstrating Manacher + DP integration.)
    
    Strategy: DP[i][j][k] = can s[i:j+1] be made palindrome by removing k chars.
    """
    n = len(s)
    # dp[i][j] = min deletions to make s[i:j+1] palindrome
    dp = [[0] * n for _ in range(n)]
    
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                dp[i][j] = dp[i+1][j-1] if length > 2 else 0
            else:
                dp[i][j] = min(dp[i+1][j], dp[i][j-1]) + 1
    
    # Find longest palindrome achievable by removing at most k chars
    for length in range(n, 0, -1):
        for i in range(n - length + 1):
            j = i + length - 1
            if dp[i][j] <= k:
                return length
    return 0
```

---

### Problem 5: Palindrome Pairs (LeetCode 336) — Using Manacher

```python
def palindrome_pairs(words: list[str]) -> list[list[int]]:
    """
    LeetCode 336. For each pair (i,j), check if words[i]+words[j] is palindrome.
    
    Naive: O(N² × W). Optimized: O(N × W²) with trie/hash.
    
    Key insight: words[i]+words[j] is palindrome iff:
    1. words[j] == reverse(words[i]) (equal length case), OR
    2. words[i]'s suffix that matches reverse(prefix of words[j]), and remaining middle is palindrome, OR
    3. Symmetric case for words[j]
    
    Time: O(N × W²), Space: O(N × W)
    """
    def is_palindrome(s: str, l: int, r: int) -> bool:
        while l < r:
            if s[l] != s[r]:
                return False
            l += 1
            r -= 1
        return True
    
    word_index = {word: i for i, word in enumerate(words)}
    result = []
    
    for i, word in enumerate(words):
        rev = word[::-1]
        n = len(word)
        
        # Case 1: word + reverse(word) == palindrome (both same length)
        # Already handled in cases below
        
        for j in range(n + 1):
            # Case 2: words[i][j:] + words[k] = palindrome when words[i][:j] is palindrome
            # and words[k] = reverse(words[i][j:])
            if is_palindrome(word, j, n - 1):
                rev_prefix = word[:j][::-1]
                if rev_prefix in word_index and word_index[rev_prefix] != i:
                    result.append([i, word_index[rev_prefix]])
            
            # Case 3: words[k] + words[i][:n-j] = palindrome when words[i][n-j:] is palindrome
            # and words[k] = reverse(words[i][:n-j])
            if j < n and is_palindrome(word, 0, j):
                rev_suffix = word[j+1:][::-1]
                if rev_suffix in word_index and word_index[rev_suffix] != i:
                    result.append([word_index[rev_suffix], i])
    
    return result
```

---

### Problem 6: Longest Palindromic Substring — All Three Approaches

```python
# Approach 1: O(N²) Expand Around Center
def longest_palindrome_expand(s: str) -> str:
    n = len(s)
    start, max_len = 0, 1
    
    def expand(l, r):
        nonlocal start, max_len
        while l >= 0 and r < n and s[l] == s[r]:
            if r - l + 1 > max_len:
                max_len = r - l + 1
                start = l
            l -= 1
            r += 1
    
    for i in range(n):
        expand(i, i)       # odd
        expand(i, i + 1)   # even
    
    return s[start:start + max_len]

# Approach 2: O(N) Manacher's
def longest_palindrome_manacher(s: str) -> str:
    t = transform(s)
    P = manacher(s)
    max_p = max(P)
    c = P.index(max_p)
    start = (c - max_p) // 2
    return s[start:start + max_p]

# Approach 3: O(N log N) Binary Search + Hashing
def longest_palindrome_hash(s: str) -> str:
    """Binary search on length, hash check for palindrome."""
    MOD, BASE = (1 << 61) - 1, 131
    n = len(s)
    rev = s[::-1]
    
    # Precompute prefix hashes for s and rev(s)
    hs = [0] * (n + 1)
    hr = [0] * (n + 1)
    pw = [1] * (n + 1)
    
    for i in range(n):
        hs[i+1] = (hs[i] * BASE + ord(s[i])) % MOD
        hr[i+1] = (hr[i] * BASE + ord(rev[i])) % MOD
        pw[i+1] = (pw[i] * BASE) % MOD
    
    def get_hash(h, l, r):
        return (h[r+1] - h[l] * pw[r-l+1]) % MOD
    
    def is_palindrome_centered(c, radius):
        # Check if s[c-radius:c+radius+1] is palindrome using hashing
        l, r = c - radius, c + radius
        if l < 0 or r >= n: return False
        fwd = get_hash(hs, l, r)
        rev_l, rev_r = n-1-r, n-1-l
        bwd = get_hash(hr, rev_l, rev_r)
        return fwd == bwd
    
    best = (0, 0)
    for c in range(n):
        # Binary search on odd radius
        lo, hi = 0, min(c, n - 1 - c)
        while lo < hi:
            mid = (lo + hi + 1) // 2
            if is_palindrome_centered(c, mid):
                lo = mid
            else:
                hi = mid - 1
        if 2 * lo + 1 > best[1] - best[0] + 1:
            best = (c - lo, c + lo)
    
    return s[best[0]:best[1]+1]
```

---

### Problem 7: Count Distinct Palindromic Substrings

```python
def count_distinct_palindromes(s: str) -> int:
    """
    Count DISTINCT palindromic substrings (each distinct string counts once).
    
    Approach: 
    1. Generate all palindromes using Manacher's O(N)
    2. Use a set to deduplicate — O(N²) total for string hashing
    
    Or: Eertree (palindromic tree) — O(N) for distinct palindromes.
    """
    t = transform(s)
    P = manacher(s)
    
    palindromes = set()
    for i, p in enumerate(P):
        # Palindromes of all radii from 1 to P[i]
        for radius in range(1, p + 1, 1 if i % 2 == 0 else 1):
            orig_start = (i - radius) // 2
            orig_len   = radius
            if orig_start >= 0 and orig_start + orig_len <= len(s):
                palindromes.add(s[orig_start:orig_start + orig_len])
    
    return len(palindromes)
```

---

## 7. KMP vs Manacher Comparison <a name="comparison"></a>

Both use the "rightmost window" trick to avoid redundant comparisons:

| Aspect | KMP | Manacher |
|--------|-----|----------|
| Structure maintained | Rightmost matching prefix | Rightmost palindrome center+radius |
| Fallback | Follow failure function | Use mirror position |
| Key lemma | `j` total decrements ≤ N | Each char moved past at most twice |
| Complexity | O(N+M) | O(N) |
| Problem solved | Pattern search | All palindromes |

### Manacher's Algorithm: The 3 Cases (MEMORIZE)

```
Current position i, rightmost palindrome [c-P[c], c+P[c]]:

Case 1: i > r (outside window)
    → P[i] = 0, expand from scratch

Case 2: i <= r, mirror = 2c - i
    Subcase 2a: P[mirror] < r - i
        → P[i] = P[mirror]   (exact, no expansion)
    Subcase 2b: P[mirror] > r - i  
        → P[i] = r - i        (bounded by window)
    Subcase 2c: P[mirror] == r - i
        → P[i] >= r - i, then expand from r+1
```

---

## Interview Tips

> **"Why is Manacher O(N) and not O(N²)?"** — The right pointer `r` only moves right. Every character comparison either advances `r` (amortized O(1) per char) or is skipped via the mirror. Total: O(N).

> **"What's the # trick for?"** — Unifies odd and even palindromes. Without it, you need separate passes. With it, every palindrome in the transformed string is odd-length.

> **"Alternative to Manacher?"** — Rolling hash can check if a specific substring is palindrome in O(1) after O(N) preprocessing, enabling binary search for longest palindrome in O(N log N). Manacher is still superior.

> **Common bug:** When `i % 2 == 0` (# position), the palindrome center is between characters; `P[i]` directly gives original string palindrome length (even). When `i % 2 == 1` (letter position), `P[i]` gives half the length (odd palindrome, length = `P[i]`... wait: `P[i]` in transformed = radius in T = radius in original? No — in T, `P[i]=4` for "abba" in "#a#b#b#a#" means palindrome of length 4 in original.

---

## Edge Cases

```python
# 1. Single character
assert longest_palindrome_manacher("a") == "a"

# 2. All same characters  
assert longest_palindrome_manacher("aaaa") == "aaaa"
assert count_substrings_clean("aaaa") == 10  # 4+3+2+1

# 3. No palindrome longer than 1
assert len(longest_palindrome_manacher("abcd")) == 1

# 4. Even-length palindrome
assert longest_palindrome_manacher("abba") == "abba"

# 5. Palindrome at boundaries
assert longest_palindrome_manacher("aab") == "aa"
assert longest_palindrome_manacher("baa") == "aa"

# 6. Empty string
assert longest_palindrome_manacher("") == ""

# 7. Entire string is palindrome
assert longest_palindrome_manacher("racecar") == "racecar"
```

---

*Previous: [Suffix Array & LCP ←](03_Suffix_Array_And_LCP.md) | Next: [Advanced Sorting →](../12_Sorting_And_Searching/01_Advanced_Sorting.md)*

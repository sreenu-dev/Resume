# Rolling Hash & Rabin-Karp — Mastery Guide

## Core Concept & Invariant

**Polynomial Rolling Hash** represents a string as a polynomial evaluated at BASE:

```
hash(s[0..n-1]) = s[0]×BASE^(n-1) + s[1]×BASE^(n-2) + ... + s[n-1]×BASE^0
                = Σ s[i] × BASE^(n-1-i)  (mod MOD)
```

**The rolling property** (the key invariant): 

```
hash(s[l+1..r+1]) = (hash(s[l..r]) - s[l]×BASE^(r-l)) × BASE + s[r+1]  (mod MOD)
```

This allows computing the next window's hash from the current in O(1), enabling:
- Rabin-Karp string matching: O(n+m) expected
- Substring duplicate detection: O(n log n) with binary search
- All window/substring comparison problems that would otherwise be O(n×m)

**Collision probability**: For a single hash comparison with random MOD:
`Pr[false positive] = 1/MOD`

With MOD ≈ 10^18 (Mersenne prime), false positive probability ≈ 10^{-18} per comparison.
For n comparisons: Pr[any false positive] ≤ n/MOD ≈ n × 10^{-18} (negligible).

**Double hashing**: Use two independent (BASE, MOD) pairs. False positive probability drops to 1/(MOD₁ × MOD₂) ≈ 10^{-36}.

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────
# Template 1: Prefix Hash Array (O(1) substring hash)
# ─────────────────────────────────────────────────────────────
class StringHasher:
    """
    Precompute prefix hashes for O(1) substring hash queries.
    H[i] = hash(s[0..i-1])
    power[i] = BASE^i mod MOD
    
    Query hash(s[l..r]) = (H[r+1] - H[l] * power[r-l+1]) mod MOD
    """
    BASE = 131           # Prime, > 26 (alphabet size)
    MOD = (1 << 61) - 1  # Mersenne prime: 2^61 - 1 (largest 64-bit Mersenne prime)
    
    def __init__(self, s: str):
        self.s = s
        n = len(s)
        self.H = [0] * (n + 1)
        self.power = [1] * (n + 1)
        
        for i in range(n):
            self.H[i+1] = (self.H[i] * self.BASE + ord(s[i])) % self.MOD
            self.power[i+1] = self.power[i] * self.BASE % self.MOD
    
    def get_hash(self, l: int, r: int) -> int:
        """Hash of s[l..r] (0-indexed, inclusive). O(1)."""
        return (self.H[r+1] - self.H[l] * self.power[r-l+1]) % self.MOD
    
    def equal(self, l1: int, r1: int, l2: int, r2: int) -> bool:
        """Check if s[l1..r1] == s[l2..r2] in O(1) (with collision probability 1/MOD)."""
        return (r1 - l1 == r2 - l2) and (self.get_hash(l1, r1) == self.get_hash(l2, r2))

# ─────────────────────────────────────────────────────────────
# Template 2: Rabin-Karp Pattern Matching
# ─────────────────────────────────────────────────────────────
def rabin_karp(text: str, pattern: str) -> list:
    """
    Find all occurrences of pattern in text.
    Expected O(n+m) where n=|text|, m=|pattern|.
    
    Algorithm:
    1. Compute hash of pattern.
    2. Slide window of size m across text, compute rolling hash.
    3. On hash match: verify character-by-character (avoid false positives).
    4. Expected O(n) total verification due to low collision probability.
    """
    BASE, MOD = 131, (1 << 61) - 1
    n, m = len(text), len(pattern)
    if m > n: return []
    
    # Precompute BASE^(m-1) mod MOD for rolling hash
    high_power = pow(BASE, m-1, MOD)
    
    # Compute pattern hash and initial window hash
    pat_hash = 0
    win_hash = 0
    for i in range(m):
        pat_hash = (pat_hash * BASE + ord(pattern[i])) % MOD
        win_hash = (win_hash * BASE + ord(text[i])) % MOD
    
    result = []
    
    for i in range(n - m + 1):
        if win_hash == pat_hash:
            # Hash match — verify to handle collisions
            if text[i:i+m] == pattern:
                result.append(i)
        
        if i < n - m:
            # Roll: remove leftmost char, add new rightmost char
            win_hash = (win_hash - ord(text[i]) * high_power) % MOD
            win_hash = (win_hash * BASE + ord(text[i+m])) % MOD
    
    return result

# Time: O(n+m) expected  Space: O(n+m) for result, O(1) rolling window

# ─────────────────────────────────────────────────────────────
# Template 3: Double Hashing (near-zero false positive rate)
# ─────────────────────────────────────────────────────────────
def double_hash(s: str, l: int, r: int, 
               H1, pw1, H2, pw2,
               BASE1=131, MOD1=(1<<61)-1, 
               BASE2=137, MOD2=(1<<31)-1) -> tuple:
    """Returns (hash1, hash2) pair for s[l..r]. Two false positives require both to collide."""
    h1 = (H1[r+1] - H1[l] * pw1[r-l+1]) % MOD1
    h2 = (H2[r+1] - H2[l] * pw2[r-l+1]) % MOD2
    return (h1, h2)
```

---

## Complexity Analysis

| Algorithm | Time | Space | Notes |
|-----------|------|-------|-------|
| Build prefix hash | O(n) | O(n) | One-time cost |
| Substring hash query | O(1) | O(1) | After build |
| Rabin-Karp search | O(n+m) expected | O(1) rolling | O(nm) worst case |
| Longest duplicate substring | O(n log n) expected | O(n) | Binary search + hash |
| All anagrams in string | O(n+m) | O(m) | Fixed window |
| Longest repeating substring | O(n log n) | O(n) | Binary search + hash |
| Distinct echo substrings | O(n log n) | O(n) | Advanced |

---

## Classic Problems

### Problem 1: Repeated DNA Sequences — Medium

**Problem**: Find all 10-letter substrings that appear more than once.

```python
def find_repeated_dna_sequences(s: str) -> list:
    """
    Two approaches:
    A. Hash set of substrings: O(n×10) = O(n) time, O(n) space
    B. Rolling hash: O(n) time with lower constant (avoids string copying)
    
    Rolling hash allows updating in O(1) instead of creating new string O(k) per step.
    For DNA: only 4 characters (A=1, C=2, G=3, T=4), can use base-4 encoding.
    
    Base-4 rolling hash:
    hash(s[i+1..i+L]) = (hash(s[i..i+L-1]) - s[i]×4^(L-1)) × 4 + s[i+L]
    
    With only 4^10 = 1,048,576 possible windows, can use simple int as key.
    """
    L = 10
    if len(s) <= L: return []
    
    # Method 1: Simple substring hashing (Pythonic, still fast)
    seen = set()
    result = set()
    for i in range(len(s) - L + 1):
        sub = s[i:i+L]
        if sub in seen:
            result.add(sub)
        seen.add(sub)
    return list(result)

def find_repeated_dna_rolling(s: str) -> list:
    """
    Pure rolling hash for O(n) with O(1) per window update.
    DNA: 4 chars → base 4. 10 chars → 4^10 = 1M values → fits in int.
    """
    if len(s) <= 10: return []
    
    L = 10
    BASE = 4
    char_map = {'A': 0, 'C': 1, 'G': 2, 'T': 3}
    HIGH = BASE ** (L - 1)   # 4^9
    
    seen = {}   # hash → list of starting indices
    result = set()
    
    # Initial window hash
    h = 0
    for i in range(L):
        h = h * BASE + char_map[s[i]]
    seen[h] = [0]
    
    for i in range(1, len(s) - L + 1):
        # Roll: remove s[i-1], add s[i+L-1]
        h = (h - char_map[s[i-1]] * HIGH) * BASE + char_map[s[i+L-1]]
        
        if h in seen:
            # Verify against all previous occurrences with this hash
            for prev in seen[h]:
                if s[prev:prev+L] == s[i:i+L]:
                    result.add(s[i:i+L])
                    break
            seen[h].append(i)
        else:
            seen[h] = [i]
    
    return list(result)
# Time: O(n)  Space: O(n)
```

### Problem 2: Longest Duplicate Substring — Hard (Binary Search + Rolling Hash)

**Problem**: Find the longest substring that appears at least twice.

```python
def longest_dup_substring(s: str) -> str:
    """
    Binary search on answer length L:
    - If a duplicate exists for length L, search longer (lo = L + 1)
    - If none exists for length L, search shorter (hi = L - 1)
    
    For each candidate length L: sliding window with rolling hash.
    
    Binary search: O(log n) calls to check(L)
    Each check(L): O(n) rolling hash computation
    Total: O(n log n) expected
    
    False positives: handled by string comparison on hash match.
    To make truly O(n log n): double hash + no verification.
    """
    n = len(s)
    BASE, MOD = 131, (1 << 61) - 1
    
    def check(L: int) -> str:
        """Return a duplicate substring of length L, or '' if none exists."""
        if L == 0: return ""
        
        high_power = pow(BASE, L-1, MOD)
        h = 0
        for i in range(L):
            h = (h * BASE + ord(s[i])) % MOD
        
        seen = {h: [0]}
        
        for i in range(1, n - L + 1):
            # Roll window
            h = (h - ord(s[i-1]) * high_power) % MOD
            h = (h * BASE + ord(s[i+L-1])) % MOD
            
            if h in seen:
                # Verify all previous positions with same hash
                for prev in seen[h]:
                    if s[prev:prev+L] == s[i:i+L]:
                        return s[i:i+L]
                seen[h].append(i)
            else:
                seen[h] = [i]
        
        return ""
    
    lo, hi = 0, n - 1
    result = ""
    
    while lo <= hi:
        mid = (lo + hi) // 2
        candidate = check(mid)
        if candidate:
            result = candidate
            lo = mid + 1   # Try longer
        else:
            hi = mid - 1   # Try shorter
    
    return result

# Time: O(n log n) expected  Space: O(n)
```

### Problem 3: Find All Anagrams in a String — Medium

```python
def find_anagrams(s: str, p: str) -> list:
    """
    Find all starting indices where anagrams of p appear in s.
    
    Method 1: Sliding window with character counts → O(|s| + |p|)
    Method 2: Rolling hash on sorted characters → O(|s| log m + |p| log |p|)
    
    The sliding window method is cleaner for anagrams since
    sorted(window) hash == sorted(p) hash iff window is anagram.
    But computing sorted(window) each step is O(m log m) → slow.
    
    Better: maintain character frequency difference count.
    """
    from collections import Counter
    
    n, m = len(s), len(p)
    if m > n: return []
    
    need = Counter(p)
    window = Counter(s[:m])
    diff = 0   # Number of characters where window count != need count
    
    for ch in set(need) | set(window):
        if window[ch] != need[ch]:
            diff += 1
    
    result = [0] if diff == 0 else []
    
    for i in range(m, n):
        right = s[i]
        left = s[i-m]
        
        # Add right character
        old_diff = (window[right] != need.get(right, 0))
        window[right] += 1
        diff += (window[right] != need.get(right, 0)) - old_diff
        
        # Remove left character
        old_diff = (window[left] != need.get(left, 0))
        window[left] -= 1
        diff += (window[left] != need.get(left, 0)) - old_diff
        if window[left] == 0: del window[left]
        
        if diff == 0: result.append(i - m + 1)
    
    return result
# Time: O(|s| + |p|)  Space: O(|p|)

def find_anagrams_rolling_hash(s: str, p: str) -> list:
    """
    Rolling hash alternative: sort p to get canonical hash.
    For each window of size |p|: compute sorted-window hash.
    
    This is O(|s| × m log m) — WORSE than Counter approach for anagrams.
    Use rolling hash for exact-match patterns, Counter for anagram patterns.
    """
    BASE, MOD = 31, (1 << 61) - 1
    m = len(p)
    
    # Hash of sorted p (canonical anagram hash)
    p_canonical = sorted(p)
    canon_hash = 0
    for c in p_canonical:
        canon_hash = (canon_hash * BASE + ord(c)) % MOD
    
    result = []
    for i in range(len(s) - m + 1):
        window = sorted(s[i:i+m])
        w_hash = 0
        for c in window:
            w_hash = (w_hash * BASE + ord(c)) % MOD
        if w_hash == canon_hash:
            result.append(i)
    
    return result
# Time: O(|s| × m log m)  Space: O(m) — AVOID for large inputs
```

### Problem 4: Longest Repeating Substring (No Dictionary) — Hard

```python
def longest_repeating_subseq_hash(s: str) -> int:
    """
    Longest substring that appears at least twice.
    Binary search + rolling hash approach.
    
    This is the same as Problem 2 but returns length instead of the string.
    Here we demonstrate the double-hashing technique.
    """
    n = len(s)
    BASE1, MOD1 = 131, (1 << 61) - 1
    BASE2, MOD2 = 137, (1 << 31) - 1
    
    def check_length(L: int) -> bool:
        """Check if any substring of length L repeats."""
        if L == 0: return True
        
        h1 = h2 = 0
        p1 = pow(BASE1, L-1, MOD1)
        p2 = pow(BASE2, L-1, MOD2)
        
        for i in range(L):
            h1 = (h1 * BASE1 + ord(s[i])) % MOD1
            h2 = (h2 * BASE2 + ord(s[i])) % MOD2
        
        seen = {(h1, h2)}   # Set of (hash1, hash2) pairs
        
        for i in range(1, n - L + 1):
            h1 = (h1 - ord(s[i-1]) * p1) % MOD1
            h1 = (h1 * BASE1 + ord(s[i+L-1])) % MOD1
            
            h2 = (h2 - ord(s[i-1]) * p2) % MOD2
            h2 = (h2 * BASE2 + ord(s[i+L-1])) % MOD2
            
            key = (h1, h2)
            if key in seen:
                return True   # High confidence match (double hash collision ≈ 0)
            seen.add(key)
        
        return False
    
    lo, hi = 0, n - 1
    while lo < hi:
        mid = (lo + hi + 1) // 2
        if check_length(mid):
            lo = mid   # Length mid works, try longer
        else:
            hi = mid - 1
    
    return lo

# Time: O(n log n) expected  Space: O(n)
```

### Problem 5: Minimum Window Containing All Characters (Hash-based) — Hard

```python
def min_window_hash(s: str, t: str) -> str:
    """
    Find minimum window in s containing all chars of t.
    Classic sliding window with hash map tracking.
    (Included here as the hash map component is the core technique.)
    
    Key hash operations:
    - need[ch]: required count of ch (from t)
    - window[ch]: current count in window
    - satisfied: how many chars fully meet their requirement
    
    The comparison window[ch] == need[ch] is O(1) hash lookup,
    making the overall algorithm O(|s| + |t|).
    """
    from collections import Counter
    if not s or not t: return ""
    
    need = Counter(t)
    window = defaultdict(int)
    required = len(need)
    satisfied = 0
    lo = 0
    best = (float('inf'), 0, 0)
    
    for hi, ch in enumerate(s):
        window[ch] += 1
        if ch in need and window[ch] == need[ch]:
            satisfied += 1
        
        while satisfied == required:
            if hi - lo + 1 < best[0]:
                best = (hi - lo + 1, lo, hi)
            
            left = s[lo]
            window[left] -= 1
            if left in need and window[left] < need[left]:
                satisfied -= 1
            lo += 1
    
    return s[best[1]:best[2]+1] if best[0] != float('inf') else ""
# Time: O(|s| + |t|)  Space: O(|t|)
```

---

## Advanced Hashing: Polynomial Hashing Variants

### 2D Rolling Hash for Matrix Matching

```python
def search_matrix_pattern(grid: list, pattern: list) -> bool:
    """
    2D pattern matching using 2D rolling hash.
    Find if 'pattern' exists as a sub-matrix in 'grid'.
    
    Approach: Apply rolling hash row-by-row to get column hash arrays,
    then apply rolling hash column-by-column.
    
    This achieves O(mn) expected matching after O(mn + PQ) preprocessing,
    where grid is m×n and pattern is P×Q.
    """
    m, n = len(grid), len(grid[0])
    P, Q = len(pattern), len(pattern[0])
    
    BASE_R, BASE_C = 131, 137
    MOD = (1 << 61) - 1
    
    # Row hash for each row of pattern
    def row_hash(row: list) -> int:
        h = 0
        for x in row:
            h = (h * BASE_R + x) % MOD
        return h
    
    pat_row_hashes = [row_hash(pattern[r]) for r in range(P)]
    
    # Column hash of pattern's row hashes
    pat_hash = 0
    high = pow(BASE_C, P-1, MOD)
    for rh in pat_row_hashes:
        pat_hash = (pat_hash * BASE_C + rh) % MOD
    
    # Slide over grid
    high_r = pow(BASE_R, Q-1, MOD)
    
    # Compute row hashes for all rows of grid (fixed window of Q columns)
    grid_row_hashes = []
    for r in range(m):
        h = row_hash(grid[r][:Q])
        row_hs = [h]
        for j in range(Q, n):
            h = (h - grid[r][j-Q] * high_r) % MOD
            h = (h * BASE_R + grid[r][j]) % MOD
            row_hs.append(h)
        grid_row_hashes.append(row_hs)
    
    # For each column starting position, slide window of P rows
    for c in range(n - Q + 1):
        col_hash = 0
        for r in range(P):
            col_hash = (col_hash * BASE_C + grid_row_hashes[r][c]) % MOD
        
        if col_hash == pat_hash:
            # Verify
            if all(grid[r][c:c+Q] == pattern[r] for r in range(P)):
                return True
        
        for r in range(P, m):
            col_hash = (col_hash - grid_row_hashes[r-P][c] * high) % MOD
            col_hash = (col_hash * BASE_C + grid_row_hashes[r][c]) % MOD
            
            if col_hash == pat_hash:
                if all(grid[r-P+1+i][c:c+Q] == pattern[i] for i in range(P)):
                    return True
    
    return False
# Time: O(mn + PQ)  Space: O(mn)
```

### Hashing for Longest Common Substring

```python
def longest_common_substring(s: str, t: str) -> str:
    """
    Binary search on length + rolling hash.
    
    For each candidate length L:
    - Compute all substring hashes of s of length L → set S_hashes
    - Compute all substring hashes of t of length L
    - Check if any hash from t appears in S_hashes
    
    Binary search: O(log(min(|s|,|t|))) iterations
    Each check: O(|s| + |t|) rolling hash
    Total: O((|s|+|t|) × log(min(|s|,|t|)))
    
    Vs. DP: O(|s|×|t|) time O(|s|×|t|) space. Binary search+hash is better for large inputs.
    """
    BASE, MOD = 131, (1 << 61) - 1
    
    def get_subhashes(text: str, L: int) -> dict:
        """Returns dict: hash → list of starting positions."""
        if L == 0: return {}
        n = len(text)
        high = pow(BASE, L-1, MOD)
        h = 0
        for i in range(L):
            h = (h * BASE + ord(text[i])) % MOD
        
        hashes = {h: [0]}
        for i in range(1, n - L + 1):
            h = (h - ord(text[i-1]) * high) % MOD
            h = (h * BASE + ord(text[i+L-1])) % MOD
            if h not in hashes:
                hashes[h] = []
            hashes[h].append(i)
        return hashes
    
    def check(L: int) -> str:
        s_hashes = get_subhashes(s, L)
        t_hashes = get_subhashes(t, L)
        
        for h in s_hashes:
            if h in t_hashes:
                # Verify all matching positions
                for si in s_hashes[h]:
                    for ti in t_hashes[h]:
                        if s[si:si+L] == t[ti:ti+L]:
                            return s[si:si+L]
        return ""
    
    lo, hi = 0, min(len(s), len(t))
    result = ""
    
    while lo <= hi:
        mid = (lo + hi) // 2
        candidate = check(mid)
        if candidate:
            result = candidate
            lo = mid + 1
        else:
            hi = mid - 1
    
    return result

# Time: O((|s|+|t|) log(min(|s|,|t|))) expected  Space: O(|s|+|t|)
```

---

## Advanced Variations

### Palindrome Detection via Hashing

```python
class PalindromeHasher:
    """
    Check if any substring is a palindrome in O(1) using dual prefix hashes:
    - Forward hash: H[i] = hash(s[0..i-1])
    - Backward hash: HR[i] = hash(s[n-1..n-i]) (reversed string's prefix hash)
    
    s[l..r] is palindrome iff hash(s[l..r]) == hash(reverse(s[l..r]))
                                               == hash(s[l..r] from backward direction)
    """
    BASE, MOD = 131, (1 << 61) - 1
    
    def __init__(self, s: str):
        n = len(s)
        self.H  = [0] * (n+1)
        self.HR = [0] * (n+1)
        self.pw = [1] * (n+1)
        
        for i in range(n):
            self.H[i+1] = (self.H[i] * self.BASE + ord(s[i])) % self.MOD
            self.HR[i+1] = (self.HR[i] * self.BASE + ord(s[n-1-i])) % self.MOD
            self.pw[i+1] = self.pw[i] * self.BASE % self.MOD
    
    def is_palindrome(self, l: int, r: int) -> bool:
        """Check if s[l..r] is palindrome. O(1)."""
        n = r - l + 1
        # Forward hash of s[l..r]
        fwd = (self.H[r+1] - self.H[l] * self.pw[n]) % self.MOD
        # Backward hash of s[l..r] (using reversed string's hash)
        # reverse of s[l..r] corresponds to s_rev[len(s)-r-1 .. len(s)-l-1]
        total_n = len(self.pw) - 1
        rl, rr = total_n - r - 1, total_n - l - 1
        bwd = (self.HR[rr+1] - self.HR[rl] * self.pw[rr-rl+1]) % self.MOD
        return fwd == bwd
```

### Suffix Array via Prefix Doubling (Related to Hashing)

```python
def build_suffix_array(s: str) -> list:
    """
    Build suffix array in O(n log n) via prefix doubling.
    Alternative to rolling hash for repeated substring queries.
    
    SA[i] = starting index of i-th smallest suffix.
    
    Use case: when multiple longest-substring queries are needed,
    suffix array with LCP array is more efficient than binary search + hash.
    """
    n = len(s)
    sa = list(range(n))   # Initially sorted by first character
    rank = [ord(c) for c in s]
    tmp = [0] * n
    
    k = 1
    while k < n:
        def cmp_key(i):
            return (rank[i], rank[i+k] if i+k < n else -1)
        
        sa.sort(key=cmp_key)
        
        # Re-rank
        tmp[sa[0]] = 0
        for j in range(1, n):
            tmp[sa[j]] = tmp[sa[j-1]] + (1 if cmp_key(sa[j]) != cmp_key(sa[j-1]) else 0)
        rank = tmp[:]
        
        if rank[sa[-1]] == n-1:
            break  # All ranks unique — sort complete
        k <<= 1
    
    return sa
# Time: O(n log² n) with simple sort, O(n log n) with radix sort  Space: O(n)
```

---

## Edge Cases Bible

1. **Negative modulo in Python**: `(-1) % MOD = MOD - 1` in Python (always non-negative). In C++/Java: `(-1 % MOD + MOD) % MOD`. Python handles this correctly but mention it for cross-language awareness.

2. **BASE choice matters**: BASE must be greater than the alphabet size. For lowercase letters: BASE ≥ 27. For ASCII: BASE ≥ 128. Using BASE=31 for lowercase is common but incorrect if uppercase or digits appear.

3. **Mersenne prime modular arithmetic**: `(1<<61)-1` is special because `mod p` for Mersenne prime `p=2^k-1` can be computed without actual division: `x mod p = (x & p) + (x >> k)`, then subtract p if ≥ p. Faster in C/C++ but Python handles large ints natively.

4. **Rolling hash with deletion**: When removing the leftmost character, the subtraction can go negative: always add MOD and take modulo: `(h - x * high_power % MOD + MOD) % MOD`.

5. **Empty string/pattern**: Check `if not pattern: return [0]` (empty pattern matches everywhere) or `return []` depending on problem definition.

6. **Palindrome hashing index arithmetic**: The "backward hash" must carefully map indices. Off-by-one in `total_n - r - 1` is a common mistake.

7. **Hash set vs hash map for seen**: Use a set of hash values only (not a map to positions) when you only need to detect duplicates. Use a map when you need to verify or return the actual string.

8. **Single character patterns**: Rabin-Karp with m=1 should still work. `high_power = BASE^0 = 1`. Verify edge case doesn't produce wrong rolling computation.

9. **DNA sequence — only 4 chars but case matters**: If input has uppercase 'ACGT', make sure `char_map` covers the right case. Mismatched case produces wrong hash.

10. **Double hash false positives**: Even with two hashes, for n > 10^9 comparisons, collision probability approaches 1/(MOD1×MOD2). For massive datasets, use triple hashing or verify with actual string comparison.

---

## Interview Tips

### What Interviewers Look For

1. **State the expected vs worst case**: "Rabin-Karp is O(n+m) EXPECTED. Worst case is O(nm) when all windows hash-match but don't actually match — e.g., 'aaa...a' with pattern 'aaa...a' using a bad hash. In practice with large MOD, this doesn't happen."

2. **Explain the rolling update formula**: "I remove the leftmost character's contribution (which is BASE^(L-1) × char_value) and add the new rightmost character: `h = (h - left × power) × BASE + right`. This is O(1) per step."

3. **Why Mersenne prime 2^61-1**: "It's the largest prime fitting in a 64-bit integer without overflow issues in multiplication mod p. Using a power of 2 as modulus would allow easy cancellation and reduce hash quality."

4. **Binary search + hash for longest duplicate substring**: "I binary search on the answer. For each candidate length L, I compute all O(n) substring hashes in O(n) time using rolling hash, store them in a set, and check for duplicates. Binary search adds log(n) factor → O(n log n) total."

5. **When to use suffix array instead**: "If I need to answer multiple longest-substring queries or compute all LCP values, a suffix array with Kasai's LCP algorithm is better — O(n log n) build, O(1) per query. Rolling hash is better for single-use substring-comparison problems."

6. **Double hashing for guaranteed correctness**: "To make the algorithm deterministic (no false positives), I use two independent hash functions. The probability of both colliding simultaneously is 1/(MOD₁×MOD₂) ≈ 10^{-36}, practically zero."

7. **Rabin-Karp vs KMP**: "Both are O(n+m). KMP is deterministic with no false positives. Rabin-Karp is easier to implement and generalizes to 2D matching. For FAANG interviews, either is acceptable — mention both and explain the tradeoff."

# Suffix Array & LCP Array — Advanced Mastery Guide

> **Level:** Expert. Suffix arrays unlock O(N log N) solutions to problems that naïve approaches solve in O(N²). Master this for senior FAANG roles.

---

## Table of Contents
1. [Suffix Array — Conceptual Foundation](#concept)
2. [O(N log² N) Construction — Prefix Doubling](#construction)
3. [Kasai's Algorithm — LCP Array in O(N)](#kasai)
4. [Applications Map](#applications)
5. [Suffix Automaton (DAWG) — O(N) Construction](#suffix-automaton)
6. [Problems 1–7 with Full Solutions](#problems)
7. [Interview Strategy](#interview)

---

## 1. Suffix Array — Conceptual Foundation <a name="concept"></a>

**Definition:** `SA[i]` = starting index of the `i`-th lexicographically smallest suffix of string `S`.

```
S = "banana$"   ($ = sentinel, lexicographically smallest)
Suffixes:
  0: banana$
  1: anana$
  2: nana$
  3: ana$
  4: na$
  5: a$
  6: $

Sorted:
  6: $         → SA[0] = 6
  5: a$        → SA[1] = 5
  3: ana$      → SA[2] = 3
  1: anana$    → SA[3] = 1
  0: banana$   → SA[4] = 0
  4: na$       → SA[5] = 4
  2: nana$     → SA[6] = 2

SA = [6, 5, 3, 1, 0, 4, 2]
```

**Inverse SA (rank array):** `rank[SA[i]] = i`, i.e., `rank[j]` = position of suffix starting at `j` in sorted order.

---

## 2. O(N log² N) Suffix Array Construction — Prefix Doubling <a name="construction"></a>

### Core Idea: Doubling

- **Round 0:** Sort by single character (O(N log N))
- **Round 1:** Sort by length-2 prefix using rank from round 0 (O(N log N))
- **Round k:** Sort by length-2^k prefix using rank from round k-1
- **Stop** when all suffixes have distinct 2^k-length prefixes (≤ log N rounds)

Total: O(N log² N) — `log N` rounds × `O(N log N)` sort per round.

```python
def build_suffix_array(s: str) -> list[int]:
    """
    Build suffix array using O(N log^2 N) prefix doubling.
    
    Time: O(N log^2 N), Space: O(N)
    
    For O(N log N): use radix sort instead of comparison sort per round.
    """
    n = len(s)
    # Initial rank = character ordinal
    sa = sorted(range(n), key=lambda i: s[i])
    rank = [0] * n
    rank[sa[0]] = 0
    for i in range(1, n):
        rank[sa[i]] = rank[sa[i-1]]
        if s[sa[i]] != s[sa[i-1]]:
            rank[sa[i]] += 1
    
    gap = 1
    while gap < n:
        # Sort by (rank[i], rank[i+gap]) — stable sort to preserve relative order
        def sort_key(i):
            return (rank[i], rank[i + gap] if i + gap < n else -1)
        
        sa.sort(key=sort_key)
        
        # Update ranks
        tmp = [0] * n
        tmp[sa[0]] = 0
        for i in range(1, n):
            tmp[sa[i]] = tmp[sa[i-1]]
            if sort_key(sa[i]) != sort_key(sa[i-1]):
                tmp[sa[i]] += 1
        rank = tmp
        
        if rank[sa[n-1]] == n - 1:
            break  # All ranks distinct: done
        
        gap *= 2
    
    return sa


def build_suffix_array_with_rank(s: str) -> tuple[list[int], list[int]]:
    """Returns (SA, rank_array)."""
    sa = build_suffix_array(s)
    n = len(s)
    rank = [0] * n
    for i, idx in enumerate(sa):
        rank[idx] = i
    return sa, rank


# O(N log N) version using counting sort (radix sort)
def build_sa_nlogn(s: str) -> list[int]:
    """
    O(N log N) suffix array using radix sort in prefix doubling.
    Each doubling round: O(N) radix sort instead of O(N log N) comparison sort.
    Total: O(N log N).
    """
    n = len(s)
    sa = sorted(range(n), key=lambda i: s[i])
    rank = [ord(c) for c in s]
    
    def radix_sort(sa, rank, gap):
        """Sort sa by (rank[i], rank[i+gap]) using counting sort."""
        # Sort by second key first
        second = sorted(sa, key=lambda i: rank[i + gap] if i + gap < n else -1)
        
        # Counting sort by first key
        max_rank = max(rank) + 2
        count = [0] * max_rank
        for i in second:
            count[rank[i]] += 1
        for i in range(1, max_rank):
            count[i] += count[i-1]
        result = [0] * n
        for i in reversed(second):
            count[rank[i]] -= 1
            result[count[rank[i]]] = i
        return result
    
    gap = 1
    while gap < n:
        sa = radix_sort(sa, rank, gap)
        
        # Update ranks
        tmp = [0] * n
        for i in range(1, n):
            prev, cur = sa[i-1], sa[i]
            r1 = (rank[prev], rank[prev+gap] if prev+gap < n else -1)
            r2 = (rank[cur],  rank[cur+gap]  if cur+gap  < n else -1)
            tmp[cur] = tmp[prev] + (0 if r1 == r2 else 1)
        rank = tmp
        
        if rank[sa[-1]] == n - 1:
            break
        gap *= 2
    
    return sa
```

---

## 3. Kasai's Algorithm — LCP Array in O(N) <a name="kasai"></a>

**LCP Array:** `lcp[i]` = length of longest common prefix between `SA[i]` and `SA[i-1]` (adjacent suffixes in sorted order).

**Kasai's Key Lemma:** If the LCP of suffix starting at `i` and its neighbor in SA is `k`, then the LCP of suffix starting at `i+1` and **its** SA-neighbor is ≥ `k-1`.

**Intuition:** Removing the first character from both suffixes can only *decrease* their LCP by at most 1.

```python
def build_lcp_kasai(s: str, sa: list[int]) -> list[int]:
    """
    Kasai's algorithm for LCP array.
    Time: O(N), Space: O(N)
    
    lcp[i] = LCP(SA[i-1], SA[i]) for i >= 1
    lcp[0] = 0 by convention
    """
    n = len(s)
    rank = [0] * n
    for i, idx in enumerate(sa):
        rank[idx] = i
    
    lcp = [0] * n
    h = 0  # current LCP value — never decreases faster than 1 per step
    
    for i in range(n):
        if rank[i] > 0:
            j = sa[rank[i] - 1]  # suffix just before i in sorted order
            # Extend LCP
            while i + h < n and j + h < n and s[i + h] == s[j + h]:
                h += 1
            lcp[rank[i]] = h
            # Kasai's lemma: h can decrease by at most 1
            if h > 0:
                h -= 1
    
    return lcp

# Why O(N)?
# h only increases inside the while loop.
# h decreases by at most 1 per iteration of outer for loop → at most N decreases.
# Total h increments = total h decrements + final h ≤ N + N = O(N).
# Total while loop iterations ≤ 2N → O(N).
```

---

## 4. Applications Map <a name="applications"></a>

### 4.1 Number of Distinct Substrings

Every suffix `SA[i]` contributes `n - SA[i]` substrings, but `lcp[i]` of them are shared with the previous suffix.

```
Distinct substrings = Σ(n - SA[i]) - Σlcp[i]
                    = n(n+1)/2 - Σlcp[i]
```

```python
def count_distinct_substrings(s: str) -> int:
    """
    Count distinct (non-empty) substrings of s.
    Time: O(N log^2 N), Space: O(N)
    """
    n = len(s)
    sa = build_suffix_array(s)
    lcp = build_lcp_kasai(s, sa)
    
    total = n * (n + 1) // 2  # total substrings if all distinct
    return total - sum(lcp)

# "abab" → SA=[2,0,3,1], LCP=[0,2,0,1]
# Total = 4*5/2 = 10
# Distinct = 10 - (0+2+0+1) = 7
# Verify: "a","b","ab","ba","aba","bab","abab" → 7 ✓
```

### 4.2 Longest Repeated Substring

The longest repeated substring is the maximum value in the LCP array.

```python
def longest_repeated_substring(s: str) -> str:
    """
    O(N log^2 N). The LCP array's max value gives the length.
    """
    sa = build_suffix_array(s)
    lcp = build_lcp_kasai(s, sa)
    
    max_lcp = max(lcp)
    if max_lcp == 0:
        return ""
    
    # Find which adjacent pair gives max_lcp
    idx = lcp.index(max_lcp)
    return s[sa[idx]:sa[idx] + max_lcp]

# "banana" → longest repeated = "ana" (length 3)
```

### 4.3 Longest Common Substring of Two Strings

```python
def longest_common_substring(s1: str, s2: str) -> str:
    """
    Concatenate s1 + '$' + s2, build SA + LCP.
    Find max LCP[i] where SA[i-1] and SA[i] come from different strings.
    
    Time: O((N+M) log^2(N+M)), Space: O(N+M)
    """
    sep = '$'
    combined = s1 + sep + s2
    n1, n = len(s1), len(combined)
    
    sa = build_suffix_array(combined)
    lcp = build_lcp_kasai(combined, sa)
    
    best_len = 0
    best_start = -1
    
    def from_s1(idx):
        return idx < n1
    
    for i in range(1, n):
        if from_s1(sa[i]) != from_s1(sa[i-1]):
            # LCP doesn't cross separator (sentinel '$' ensures this)
            if lcp[i] > best_len:
                best_len = lcp[i]
                best_start = sa[i]
    
    return combined[best_start:best_start + best_len]

# s1="ABABC", s2="BABCAB" → "BABC" (length 4)
```

---

## 5. Suffix Automaton (DAWG) — O(N) Construction <a name="suffix-automaton"></a>

The **Directed Acyclic Word Graph** (DAWG) is the smallest DFA accepting all suffixes of a string.

**Key properties:**
- Exactly `2N - 1` states (N ≥ 2)
- Exactly `3N - 4` transitions
- Built online in O(N) time
- Counts distinct substrings in O(N)

```python
class SuffixAutomaton:
    """
    Suffix Automaton (SAM/DAWG) — O(N) construction.
    
    Each state represents an equivalence class of substrings
    that have the same set of ending positions (endpos set).
    
    link[v] = suffix link (parent in suffix link tree)
    len[v]  = length of longest substring in this class
    """
    
    class State:
        def __init__(self):
            self.next = {}      # transitions: char → state
            self.link = -1      # suffix link
            self.len = 0        # length of longest string in equivalence class
            self.cnt = 0        # number of occurrences (for counting)
            self.is_clone = False
    
    def __init__(self, s: str):
        self.states = [self.State()]  # root (state 0)
        self.states[0].len = 0
        self.last = 0
        self.size = 1
        
        for c in s:
            self._extend(c)
    
    def _new_state(self, length: int) -> int:
        st = self.State()
        st.len = length
        self.states.append(st)
        self.size += 1
        return self.size - 1
    
    def _extend(self, c: str):
        """Add next character. Amortized O(1), total O(N)."""
        # Create new state for current suffix
        cur = self._new_state(self.states[self.last].len + 1)
        self.states[cur].cnt = 1  # this suffix occurs once
        p = self.last
        
        # Walk up suffix links, adding transitions to cur
        while p != -1 and c not in self.states[p].next:
            self.states[p].next[c] = cur
            p = self.states[p].link
        
        if p == -1:
            # Reached root without finding c → link cur to root
            self.states[cur].link = 0
        else:
            q = self.states[p].next[c]
            if self.states[p].len + 1 == self.states[q].len:
                # No need to clone
                self.states[cur].link = q
            else:
                # Clone q to split the transition
                clone = self._new_state(self.states[p].len + 1)
                self.states[clone].next = dict(self.states[q].next)
                self.states[clone].link = self.states[q].link
                self.states[clone].is_clone = True
                
                while p != -1 and self.states[p].next.get(c) == q:
                    self.states[p].next[c] = clone
                    p = self.states[p].link
                
                self.states[q].link = clone
                self.states[cur].link = clone
        
        self.last = cur
    
    def count_distinct_substrings(self) -> int:
        """
        Each non-root state v contributes (len[v] - len[link[v]]) distinct substrings.
        Time: O(N), Space: O(N)
        """
        total = 0
        for i in range(1, self.size):
            st = self.states[i]
            total += st.len - self.states[st.link].len
        return total
    
    def count_occurrences(self) -> dict[int, int]:
        """
        Count occurrences of each state's equivalence class substrings.
        Uses topological sort (by len, descending) to propagate counts.
        """
        # Sort by len descending (topological order for suffix link tree)
        order = sorted(range(self.size), key=lambda x: -self.states[x].len)
        for v in order:
            if self.states[v].link != -1:
                self.states[self.states[v].link].cnt += self.states[v].cnt
        
        return {i: self.states[i].cnt for i in range(1, self.size)
                if not self.states[i].is_clone}
    
    def find_longest_common_substring_3(self, texts: list[str]) -> str:
        """
        Longest common substring of 3+ strings using suffix automaton.
        Build SAM on texts[0], check if all other strings have common substrings.
        
        Time: O(N₁ + Σ(Nᵢ × alphabet))
        """
        # Build SAM on first string
        # For each other string, walk SAM and track max match length
        # At each state, track min match length across all strings
        
        # Count for each state: how many strings have that substring
        n_texts = len(texts)
        state_min_match = [float('inf')] * self.size
        state_min_match[0] = 0
        
        for text in texts[1:]:
            cur_state = 0
            cur_len = 0
            # Track max match reaching each state
            reach = [-1] * self.size
            reach[0] = 0
            
            for ch in text:
                while cur_state != 0 and ch not in self.states[cur_state].next:
                    cur_state = self.states[cur_state].link
                    cur_len = self.states[cur_state].len
                if ch in self.states[cur_state].next:
                    cur_state = self.states[cur_state].next[ch]
                    cur_len += 1
                reach[cur_state] = max(reach[cur_state], cur_len)
            
            # Propagate up suffix links
            for v in sorted(range(self.size), key=lambda x: -self.states[x].len):
                if self.states[v].link != -1:
                    reach[self.states[v].link] = max(
                        reach[self.states[v].link], 
                        min(reach[v], self.states[self.states[v].link].len)
                    )
            
            for v in range(self.size):
                state_min_match[v] = min(state_min_match[v], reach[v])
        
        # Find state with max min_match
        best_len = 0
        best_state = 0
        for v in range(self.size):
            if state_min_match[v] > best_len:
                best_len = state_min_match[v]
                best_state = v
        
        # Reconstruct the substring (walk back through transitions)
        # For simplicity, return length here
        return best_len  # caller reconstructs using texts[0][some_pos:some_pos+best_len]
```

---

## 6. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Longest Repeated Substring (LeetCode 1044)

```python
def longest_duplicate_substring(s: str) -> str:
    """
    LeetCode 1044.
    Binary search on length + Rabin-Karp for O(N log N).
    Or directly: SA + LCP gives O(N log^2 N).
    """
    # Method 1: SA + LCP (simpler to understand)
    sa = build_suffix_array(s)
    lcp = build_lcp_kasai(s, sa)
    
    max_len = max(lcp)
    if max_len == 0:
        return ""
    idx = lcp.index(max_len)
    return s[sa[idx]:sa[idx] + max_len]

# Method 2: Binary search + Rolling hash O(N log N)
def longest_duplicate_substring_hash(s: str) -> str:
    """O(N log N) using binary search on length + rolling hash."""
    MOD = (1 << 61) - 1
    BASE = 131
    n = len(s)
    
    def has_duplicate(length: int) -> str:
        """Check if any substring of given length appears twice."""
        if length == 0:
            return ""
        
        # Compute rolling hash
        power = pow(BASE, length, MOD)
        h = 0
        seen = {}
        
        for i in range(length):
            h = (h * BASE + ord(s[i])) % MOD
        seen[h] = [0]
        
        for i in range(1, n - length + 1):
            h = (h * BASE - ord(s[i-1]) * power + ord(s[i+length-1])) % MOD
            if h in seen:
                # Verify (avoid hash collision)
                for j in seen[h]:
                    if s[i:i+length] == s[j:j+length]:
                        return s[i:i+length]
                seen[h].append(i)
            else:
                seen[h] = [i]
        return ""
    
    lo, hi = 0, n - 1
    result = ""
    while lo <= hi:
        mid = (lo + hi) // 2
        candidate = has_duplicate(mid)
        if candidate:
            result = candidate
            lo = mid + 1
        else:
            hi = mid - 1
    return result
```

---

### Problem 2: Longest Common Substring of Two Strings (LeetCode 718)

```python
def find_length(nums1: list[int], nums2: list[int]) -> int:
    """
    LeetCode 718 — Longest common subarray (not substring of string, but same logic).
    
    DP approach: O(N×M) time and space.
    SA approach: Convert to string, O((N+M) log(N+M)).
    """
    # DP (cleaner for arrays)
    n, m = len(nums1), len(nums2)
    dp = [[0] * (m + 1) for _ in range(n + 1)]
    result = 0
    for i in range(1, n + 1):
        for j in range(1, m + 1):
            if nums1[i-1] == nums2[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
                result = max(result, dp[i][j])
    return result

def longest_common_substring_strings(s1: str, s2: str) -> int:
    """O(N log^2 N) using suffix array."""
    combined = s1 + chr(1) + s2  # chr(1) < any printable char
    sa = build_suffix_array(combined)
    lcp = build_lcp_kasai(combined, sa)
    n1 = len(s1)
    
    best = 0
    for i in range(1, len(combined)):
        # SA[i-1] and SA[i] must be from different strings
        a, b = sa[i-1], sa[i]
        if (a < n1) != (b < n1):  # one from s1, one from s2
            best = max(best, lcp[i])
    return best
```

---

### Problem 3: Count Distinct Substrings (LeetCode 2261 variant)

```python
def count_distinct_substrings_v2(s: str) -> int:
    """
    Using Suffix Automaton — O(N).
    Each non-root state contributes (len - link.len) distinct substrings.
    """
    sam = SuffixAutomaton(s)
    return sam.count_distinct_substrings()

# Verify:
# "abc" → 6 ("a","b","c","ab","bc","abc")
# "aab" → 5 ("a","b","aa","ab","aab")
assert count_distinct_substrings_v2("abc") == 6
assert count_distinct_substrings_v2("aab") == 5
```

---

### Problem 4: Longest Palindromic Substring via SA

```python
def longest_palindrome_sa(s: str) -> str:
    """
    LCS of s and reverse(s) = longest palindromic substring.
    But LCS gives longest palindromic substring only if we ensure
    the positions don't overlap in a way that creates false positives.
    
    More correctly: use Manacher's for O(N). SA approach is O(N log^2 N).
    """
    if not s:
        return ""
    rev = s[::-1]
    
    # Build SA on s + '#' + rev(s)
    combined = s + '#' + rev
    n1 = len(s)
    n = len(combined)
    
    sa = build_suffix_array(combined)
    lcp = build_lcp_kasai(combined, sa)
    
    best_len = 0
    best_start = 0
    
    for i in range(1, n):
        a, b = sa[i-1], sa[i]
        # Both from different halves
        if (a < n1) != (b < n1) and '#' not in combined[min(a,b):min(a,b)+lcp[i]]:
            # Check the match is valid (positions correspond to palindrome)
            if a < n1:
                s_pos, r_pos = a, b - n1 - 1
            else:
                s_pos, r_pos = b, a - n1 - 1
            
            match_len = lcp[i]
            # r_pos in rev corresponds to position n1-1-r_pos in s
            # Palindrome centered check: s_pos and (n1-1-r_pos) should align
            if s_pos + match_len - 1 == n1 - 1 - r_pos or s_pos == n1 - 1 - (r_pos + match_len - 1):
                if match_len > best_len:
                    best_len = match_len
                    best_start = s_pos
    
    if best_len == 0:
        return s[0]
    return s[best_start:best_start + best_len]
```

---

### Problem 5: Number of Substrings Containing All Three Characters (LeetCode 1358)

```python
def number_of_substrings(s: str) -> int:
    """
    LeetCode 1358. Count substrings containing 'a','b','c'.
    Two-pointer O(N) approach.
    """
    count = {'a': 0, 'b': 0, 'c': 0}
    result = 0
    left = 0
    
    for right in range(len(s)):
        count[s[right]] += 1
        while all(count[c] > 0 for c in 'abc'):
            result += len(s) - right  # all extensions of current window are valid
            count[s[left]] -= 1
            left += 1
    
    return result
```

---

### Problem 6: Suffix Array — LCP Range Minimum Query

For range minimum LCP queries (LCP of any two suffixes), preprocess LCP array with sparse table.

```python
import math

class SparseTable:
    """Range Minimum Query in O(1) after O(N log N) preprocessing."""
    
    def __init__(self, arr: list[int]):
        n = len(arr)
        k = max(1, int(math.log2(n)) + 1)
        self.table = [[float('inf')] * n for _ in range(k)]
        self.table[0] = arr[:]
        self.log = [0] * (n + 1)
        for i in range(2, n + 1):
            self.log[i] = self.log[i // 2] + 1
        
        for j in range(1, k):
            for i in range(n - (1 << j) + 1):
                self.table[j][i] = min(self.table[j-1][i],
                                        self.table[j-1][i + (1 << (j-1))])
    
    def query(self, l: int, r: int) -> int:
        """Min in range [l, r] inclusive. O(1)."""
        if l > r:
            return 0
        k = self.log[r - l + 1]
        return min(self.table[k][l], self.table[k][r - (1 << k) + 1])


def lcp_any_two_suffixes(s: str, i: int, j: int) -> int:
    """
    LCP of s[i:] and s[j:] in O(1) after O(N log N) preprocessing.
    
    LCP(i, j) = min(LCP_array[rank[i]+1 .. rank[j]]) if rank[i] < rank[j]
    """
    sa, rank = build_suffix_array_with_rank(s)
    lcp = build_lcp_kasai(s, sa)
    rmq = SparseTable(lcp)
    
    ri, rj = rank[i], rank[j]
    if ri > rj:
        ri, rj = rj, ri
    return rmq.query(ri + 1, rj)
```

---

### Problem 7: Longest Common Extension (LCE) Queries

```python
def all_lce_queries(s: str, queries: list[tuple[int,int]]) -> list[int]:
    """
    For each query (i, j), find LCP of s[i:] and s[j:].
    Preprocessing: O(N log N), Each query: O(1).
    """
    sa, rank = build_suffix_array_with_rank(s)
    lcp = build_lcp_kasai(s, sa)
    rmq = SparseTable(lcp)
    
    results = []
    for i, j in queries:
        if i == j:
            results.append(len(s) - i)
            continue
        ri, rj = rank[i], rank[j]
        if ri > rj:
            ri, rj = rj, ri
        results.append(rmq.query(ri + 1, rj))
    return results
```

---

## 7. Interview Strategy <a name="interview"></a>

### Complexity at a Glance

| Operation | SA Approach | SAM Approach |
|-----------|-------------|--------------|
| Build | O(N log² N) | O(N) |
| Distinct substrings | O(N log² N) | O(N) |
| Longest repeated | O(N log² N) | O(N) |
| LCS two strings | O((N+M) log²(N+M)) | O(N+M) |
| LCP query O(1) | Sparse table on LCP | — |

### When to Use What

```
String substructure problem?
├── Counting / enumeration → Suffix Automaton (O(N) build, elegant)
├── Range queries on suffixes → Suffix Array + LCP + Sparse Table
├── Pattern matching → KMP / AC (simpler)
└── Palindromes → Manacher's (specialized, faster)
```

### Interview Red Flags to Avoid

1. **Confusing SA[i] and rank[i]** — SA maps rank→position, rank maps position→rank
2. **LCP array is 1-indexed** — `lcp[i]` = LCP between SA[i-1] and SA[i]
3. **Forgetting sentinel** — appending `$` or a character smaller than any in alphabet ensures no suffix is a prefix of another
4. **Kasai's O(N) claim** — must be able to explain the "h decreases by at most 1" argument

### Practice Priority

> **High priority:** Longest repeated substring, LCS of two strings, distinct substring count.
> **Medium:** LCE queries with RMQ, suffix automaton construction.
> **Know for discussion:** DC3 algorithm O(N) SA construction (too complex for whiteboard).

---

*Previous: [Aho-Corasick ←](02_Aho_Corasick.md) | Next: [Manacher's Algorithm →](04_Manacher_Palindrome.md)*

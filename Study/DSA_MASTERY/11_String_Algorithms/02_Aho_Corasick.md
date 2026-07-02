# Aho-Corasick Automaton — Advanced Mastery Guide

> **Prerequisite:** KMP failure function. Aho-Corasick is literally KMP generalized to a trie of patterns.

---

## Table of Contents
1. [Conceptual Foundation](#foundation)
2. [Building the Trie](#trie)
3. [Failure Links (BFS Construction)](#failure-links)
4. [Output Links](#output-links)
5. [Text Search — One Pass](#search)
6. [Complexity Analysis](#complexity)
7. [Problems 1–7 with Full Solutions](#problems)
8. [AC vs DP vs KMP Decision Guide](#decision)

---

## 1. Conceptual Foundation <a name="foundation"></a>

**Problem:** Given K patterns `P1, P2, ..., PK` and a text `T`, find all occurrences of all patterns.

**Naive:** Run KMP K times → O(K·N + sum(M_i)) — inefficient when K is large.

**Aho-Corasick:** Build automaton once, scan text once → O(sum(M_i) + N + Z) where Z = total matches.

**The Big Idea:**
1. Build a **trie** of all patterns (explicit branching on characters)
2. Add **failure links** (like KMP's failure function but on the trie)
3. Add **output links** (shortcut to find all patterns ending at current position)
4. Walk the automaton character by character through the text

The automaton has states = trie nodes. At each text character, the automaton transitions deterministically — never backing up in the text.

---

## 2. Building the Trie <a name="trie"></a>

```python
from collections import deque, defaultdict

class AhoCorasick:
    """
    Aho-Corasick automaton for multi-pattern string matching.
    
    Build: O(sum of pattern lengths)
    Search: O(N + Z) where Z = number of matches
    Space: O(sum of pattern lengths × alphabet size)
    """
    
    def __init__(self):
        # Each node: dict of children, failure link, output link, pattern list
        self.goto = [{}]          # goto[state][char] = next_state
        self.fail = [0]           # failure link: goto[state][char] falls back here
        self.output = [[]]        # patterns ending at this state
        self.size = 1             # number of states (root = 0)
    
    def _new_state(self):
        self.goto.append({})
        self.fail.append(0)
        self.output.append([])
        self.size += 1
        return self.size - 1
    
    def add_pattern(self, pattern: str, pattern_id: int):
        """Insert pattern into trie. Time: O(|pattern|)"""
        state = 0
        for ch in pattern:
            if ch not in self.goto[state]:
                self.goto[state][ch] = self._new_state()
            state = self.goto[state][ch]
        self.output[state].append(pattern_id)
    
    def build(self):
        """
        Compute failure links and output links using BFS.
        Time: O(sum of pattern lengths × alphabet size)
        
        Key invariant: failure link of state s points to the longest 
        proper suffix of the string represented by s that is also 
        a prefix in the trie.
        """
        queue = deque()
        
        # Initialize: all depth-1 nodes fail to root
        for ch, state in self.goto[0].items():
            self.fail[state] = 0
            queue.append(state)
        
        while queue:
            r = queue.popleft()
            
            for ch, s in self.goto[r].items():
                queue.append(s)
                state = self.fail[r]
                
                # Follow failure links until we find a state with transition on ch
                while state != 0 and ch not in self.goto[state]:
                    state = self.fail[state]
                
                self.fail[s] = self.goto[state].get(ch, 0)
                if self.fail[s] == s:  # avoid self-loop at root
                    self.fail[s] = 0
                
                # Output link: inherit outputs from failure link
                self.output[s] = self.output[s] + self.output[self.fail[s]]
            
            # For missing transitions at r, add goto edges (like KMP's δ function)
            # This makes the automaton complete (total function)
            for ch in set(self.goto[self.fail[r]].keys()) - set(self.goto[r].keys()):
                self.goto[r][ch] = self.goto[self.fail[r]][ch]
        
        # Complete root's missing transitions (point to root itself)
        # Already handled since fail[root] = root conceptually
    
    def search(self, text: str) -> list[tuple[int, int]]:
        """
        Search for all patterns in text.
        Returns: list of (end_position, pattern_id)
        Time: O(N + Z)
        """
        state = 0
        results = []
        
        for i, ch in enumerate(text):
            # Follow failure links until we find valid transition
            while state != 0 and ch not in self.goto[state]:
                state = self.fail[state]
            
            state = self.goto[state].get(ch, 0)
            
            # Collect all patterns ending here
            for pattern_id in self.output[state]:
                results.append((i, pattern_id))
        
        return results
```

### 2.1 Complete Automaton Construction (Optimized)

The cleaner approach: build the **complete** goto function during BFS so the search never needs to follow failure links — O(1) transition per character.

```python
class AhoCorasickOptimized:
    """
    Optimized AC with precomputed complete transition function.
    After build(), goto[state][ch] is always valid (no failure link chasing during search).
    """
    
    ALPHA = 26  # lowercase English letters
    
    def __init__(self):
        self.goto = [[- 1] * self.ALPHA]
        self.fail = [-1]
        self.output = [[]]
        self.size = 1
    
    def _ord(self, ch: str) -> int:
        return ord(ch) - ord('a')
    
    def _new_state(self):
        self.goto.append([-1] * self.ALPHA)
        self.fail.append(-1)
        self.output.append([])
        self.size += 1
        return self.size - 1
    
    def add_pattern(self, pattern: str, pid: int):
        state = 0
        for ch in pattern:
            c = self._ord(ch)
            if self.goto[state][c] == -1:
                self.goto[state][c] = self._new_state()
            state = self.goto[state][c]
        self.output[state].append(pid)
    
    def build(self):
        """BFS to compute complete goto + fail + output links."""
        queue = deque()
        
        # Depth 1: incomplete transitions → root
        for c in range(self.ALPHA):
            if self.goto[0][c] == -1:
                self.goto[0][c] = 0  # loop to root
            else:
                self.fail[self.goto[0][c]] = 0
                queue.append(self.goto[0][c])
        
        while queue:
            r = queue.popleft()
            # Merge output with failure state's output
            self.output[r] = self.output[r] + self.output[self.fail[r]]
            
            for c in range(self.ALPHA):
                s = self.goto[r][c]
                if s == -1:
                    # No transition: use failure link's transition (computed already)
                    self.goto[r][c] = self.goto[self.fail[r]][c]
                else:
                    self.fail[s] = self.goto[self.fail[r]][c]
                    queue.append(s)
    
    def search(self, text: str) -> list[tuple[int, int]]:
        """O(N + Z) search — no failure link traversal needed."""
        state = 0
        results = []
        for i, ch in enumerate(text):
            state = self.goto[state][self._ord(ch)]
            for pid in self.output[state]:
                results.append((i, pid))
        return results
```

---

## 3. Failure Links — Deep Dive <a name="failure-links"></a>

**Analogy with KMP:**
- KMP: For each position `j` in pattern, `lps[j]` = longest border
- AC: For each trie node, `fail[node]` = deepest node in trie that represents a proper suffix of `node`'s string

**BFS ordering is critical:** Failure links always point to shallower nodes (closer to root). By processing BFS level by level, when computing `fail[s]`, `fail[parent(s)]` is already computed.

**Proof of correctness:**
```
Let str(v) = string represented by path from root to v
fail[v] = node w such that str(w) is the longest proper suffix of str(v) in trie

Base: depth-1 nodes → fail[v] = root (longest proper suffix is empty string = root)
Inductive: For node s = goto[r][c]:
    fail[s] = goto[fail[r]][c] (if exists)
           OR goto[fail[fail[r]]][c] ... following failure chain
    This finds the longest proper suffix of str(r)+c in the trie.
```

---

## 4. Output Links <a name="output-links"></a>

**The subtle point:** When at state `s`, patterns that match at current position include:
1. Patterns explicitly ending at `s`
2. Patterns ending at `fail[s]`, `fail[fail[s]]`, ... (suffix patterns)

The output link is a shortcut: `output_link[s]` = deepest non-empty output node in failure chain.

By storing **all inherited outputs** during BFS (as done in our implementation), we avoid traversing the failure chain at search time.

---

## 5. Text Search — One Pass <a name="search"></a>

```python
def find_all_occurrences(text: str, patterns: list[str]) -> dict[str, list[int]]:
    """
    Find all occurrences of all patterns in text.
    Returns dict: pattern → list of start positions
    
    Time: O(sum(|P_i|) + N + Z)
    Space: O(sum(|P_i|) × alphabet_size)
    """
    ac = AhoCorasickOptimized()
    for i, p in enumerate(patterns):
        ac.add_pattern(p, i)
    ac.build()
    
    matches = ac.search(text)
    
    result = {p: [] for p in patterns}
    for end_pos, pid in matches:
        pattern = patterns[pid]
        start_pos = end_pos - len(pattern) + 1
        result[pattern].append(start_pos)
    
    return result

# Example:
text = "ahishers"
patterns = ["he", "she", "his", "hers"]
# Result: {"he": [1], "she": [3], "his": [1], "hers": [4]}
```

---

## 6. Complexity Analysis <a name="complexity"></a>

| Phase | Time | Space |
|-------|------|-------|
| Trie build | O(Σ\|P_i\|) | O(Σ\|P_i\| × α) |
| BFS + fail links | O(Σ\|P_i\| × α) | O(Σ\|P_i\| × α) |
| Search | O(N + Z) | O(1) extra |

Where α = alphabet size (26 for lowercase, 128 for ASCII).

**Why O(N) search?** After precomputing complete goto function, each text character causes exactly ONE state transition → exactly N transitions total.

---

## 7. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Multi-Pattern Word Search
**Given text and K patterns, find which patterns appear and where.**

```python
def multi_pattern_search(text: str, patterns: list[str]) -> list[bool]:
    """
    Return boolean array: patterns[i] appears in text.
    Time: O(Σ|P_i| + N), Space: O(Σ|P_i|)
    """
    ac = AhoCorasick()
    for i, p in enumerate(patterns):
        ac.add_pattern(p, i)
    ac.build()
    
    found = [False] * len(patterns)
    for _, pid in ac.search(text):
        found[pid] = True
    return found
```

---

### Problem 2: Word Break II with Aho-Corasick
**LeetCode 140 — All ways to break string s using words from dictionary**

```python
from typing import Optional

def word_break_ac(s: str, word_dict: list[str]) -> list[str]:
    """
    Word Break II using Aho-Corasick + backtracking.
    
    Step 1: Find all (start, end) pairs where word_dict[i] = s[start:end]
    Step 2: Build adjacency list from end → list of starts
    Step 3: DFS from len(s) backward to reconstruct paths
    
    Time: O(Σ|words| + N + Z + output_size)
    """
    ac = AhoCorasick()
    for i, w in enumerate(word_dict):
        ac.add_pattern(w, i)
    ac.build()
    
    # Find all word endpoints
    # adj[end] = list of (start, word)
    adj = defaultdict(list)
    for end_pos, wid in ac.search(s):
        word = word_dict[wid]
        start = end_pos - len(word) + 1
        adj[end_pos + 1].append((start, word))  # end_pos+1 = exclusive end
    
    # Backtrack from len(s)
    results = []
    
    def dfs(pos: int, path: list):
        if pos == 0:
            results.append(' '.join(reversed(path)))
            return
        for start, word in adj[pos]:
            path.append(word)
            dfs(start, path)
            path.pop()
    
    dfs(len(s), [])
    return results

# DP approach comparison:
def word_break_dp(s: str, word_dict: list[str]) -> list[str]:
    """Classic DP approach. O(N² × W) where W = avg word length."""
    word_set = set(word_dict)
    n = len(s)
    # dp[i] = list of sentences that partition s[:i]
    dp = [[] for _ in range(n + 1)]
    dp[0] = [""]
    
    for i in range(1, n + 1):
        for j in range(i):
            if dp[j] and s[j:i] in word_set:
                for prev in dp[j]:
                    dp[i].append((prev + " " + s[j:i]).strip())
    
    return dp[n]
```

---

### Problem 3: Replace Words (Dictionary Trie)
**LeetCode 648 — Replace words with their root**

```python
class TrieNode:
    def __init__(self):
        self.children = {}
        self.root = None  # shortest root ending here

def replace_words(dictionary: list[str], sentence: str) -> str:
    """
    Replace each word in sentence with shortest root from dictionary.
    Time: O(Σ|roots| + N), Space: O(Σ|roots|)
    
    Simpler with basic trie than AC (only need shortest match).
    """
    # Build trie
    root_node = TrieNode()
    for root in dictionary:
        node = root_node
        for ch in root:
            if ch not in node.children:
                node.children[ch] = TrieNode()
            node = node.children[ch]
            if node.root is not None:
                break  # already have shorter root
        node.root = root
    
    def find_root(word: str) -> str:
        node = root_node
        for i, ch in enumerate(word):
            if ch not in node.children:
                break
            node = node.children[ch]
            if node.root is not None:
                return node.root
        return word
    
    return ' '.join(find_root(w) for w in sentence.split())

# AC approach for this problem is overkill; basic trie is cleaner.
# AC shines when you need ALL matches, not just the shortest.
```

---

### Problem 4: Stream of Characters
**LeetCode 1032 — Query: does any word from list end at current stream position?**

```python
class StreamChecker:
    """
    LeetCode 1032. 
    Aho-Corasick is perfect: process stream character by character,
    maintain current automaton state across queries.
    
    Build: O(Σ|words|), Query: O(1) amortized
    """
    
    def __init__(self, words: list[str]):
        self.ac = AhoCorasickOptimized()
        for i, w in enumerate(words):
            self.ac.add_pattern(w, i)
        self.ac.build()
        self.state = 0
    
    def query(self, letter: str) -> bool:
        """Process one character. Return True if any word ends here."""
        self.state = self.ac.goto[self.state][ord(letter) - ord('a')]
        return bool(self.ac.output[self.state])

# Usage:
# checker = StreamChecker(["cd","f","kl"])
# checker.query('a') → False
# checker.query('b') → False
# checker.query('c') → False
# checker.query('d') → True  (word "cd" found)
```

---

### Problem 5: Concatenated Words
**LeetCode 472 — Find words that are concatenation of other words**

```python
def find_all_concatenated_words(words: list[str]) -> list[str]:
    """
    LeetCode 472.
    A word is concatenated if it's formed by 2+ other words in the list.
    
    Approach: For each word, check if it can be segmented using other words.
    Use AC to find all word occurrences, then DP for segmentation.
    
    Time: O(N × max_word² + Σ|words|)
    """
    word_set = set(words)
    
    def can_form(word: str) -> bool:
        if not word:
            return False
        n = len(word)
        dp = [False] * (n + 1)
        dp[0] = True
        for i in range(1, n + 1):
            for j in range(i):
                if dp[j] and word[j:i] in word_set and word[j:i] != word:
                    dp[i] = True
                    break
        return dp[n]
    
    return [w for w in words if can_form(w)]

# AC-based approach: build automaton of all words, then for each word
# simulate segmentation using automaton transitions

def find_concatenated_ac(words: list[str]) -> list[str]:
    """AC + DP approach."""
    words_sorted = sorted(words, key=len)
    word_set = set(words)
    result = []
    
    ac = AhoCorasick()
    for i, w in enumerate(words):
        ac.add_pattern(w, i)
    ac.build()
    
    for target in words:
        n = len(target)
        # Find all word occurrences in target
        matches = ac.search(target)
        # adj[end] = list of starts
        adj = defaultdict(list)
        for end_pos, pid in matches:
            w = words[pid]
            if w != target:  # can't use the word itself
                adj[end_pos + 1].append(end_pos + 1 - len(w))
        
        # DP: can we reach len(target) from 0 using 2+ words?
        dp = [0] * (n + 1)  # dp[i] = min words needed to reach position i
        dp[0] = 0
        INF = float('inf')
        reach = [INF] * (n + 1)
        reach[0] = 0
        
        for end in range(1, n + 1):
            for start in adj[end]:
                if reach[start] < INF:
                    reach[end] = min(reach[end], reach[start] + 1)
        
        if reach[n] >= 2:
            result.append(target)
    
    return result
```

---

### Problem 6: Word Search in Text — Count All Occurrences

```python
def count_pattern_occurrences(text: str, patterns: list[str]) -> list[int]:
    """
    Count occurrences of each pattern in text (overlapping allowed).
    Time: O(Σ|P_i| + N + Z)
    """
    ac = AhoCorasick()
    for i, p in enumerate(patterns):
        ac.add_pattern(p, i)
    ac.build()
    
    counts = [0] * len(patterns)
    for _, pid in ac.search(text):
        counts[pid] += 1
    return counts

# Example:
# text = "aababababc"
# patterns = ["ab", "aba", "b"]
# counts = [4, 3, 4]
```

---

### Problem 7: Minimum Window Containing All Patterns
**Extended: find shortest substring of text containing all patterns**

```python
def min_window_all_patterns(text: str, patterns: list[str]) -> str:
    """
    Find shortest substring of text containing all patterns.
    
    Strategy:
    1. AC to find all occurrences with positions
    2. Sliding window on sorted events
    
    Time: O(Σ|P_i| + N + Z log Z)
    """
    ac = AhoCorasick()
    for i, p in enumerate(patterns):
        ac.add_pattern(p, i)
    ac.build()
    
    # Collect all matches as (start, end, pid) intervals
    intervals = []
    for end_pos, pid in ac.search(text):
        start = end_pos - len(patterns[pid]) + 1
        intervals.append((start, end_pos, pid))
    
    if not intervals:
        return ""
    
    # Sort by start position
    intervals.sort()
    
    # Sliding window: maintain count of each pattern covered
    from collections import Counter
    need = Counter(range(len(patterns)))
    have = Counter()
    formed = 0
    required = len(set(range(len(patterns))))
    
    left = 0
    min_len = float('inf')
    min_window = ""
    
    for right in range(len(intervals)):
        pid = intervals[right][2]
        have[pid] += 1
        if have[pid] == need[pid]:
            formed += 1
        
        while formed == required:
            # Update answer
            window_start = intervals[left][0]
            window_end = intervals[right][1]
            if window_end - window_start + 1 < min_len:
                min_len = window_end - window_start + 1
                min_window = text[window_start:window_end + 1]
            
            # Shrink window
            lp = intervals[left][2]
            have[lp] -= 1
            if have[lp] < need[lp]:
                formed -= 1
            left += 1
    
    return min_window
```

---

## 8. AC vs DP vs KMP Decision Guide <a name="decision"></a>

### When to Choose What

```
Pattern Matching Problem
├── Single pattern, single text
│   └── KMP or Z-algorithm: O(N+M)
├── Multiple patterns, single text
│   ├── Few queries (<10): Run KMP for each → O(K(N+M))
│   └── Many patterns / streaming: Aho-Corasick → O(Σ|P_i| + N)
├── Word segmentation / Word break
│   ├── Just need YES/NO: DP with Trie → O(N × max_word_len)
│   └── Need all paths: AC + DFS (avoids re-scanning)
└── Streaming input (characters arrive one at a time)
    └── Aho-Corasick (StreamChecker pattern) → O(1) per character
```

### Complexity Comparison

| Approach | Build | Per Query | Total |
|----------|-------|-----------|-------|
| Naive (K patterns) | — | O(N×M) | O(K×N×M) |
| KMP per pattern | O(M_i) | O(N) | O(K×N + ΣM_i) |
| Aho-Corasick | O(Σ\|P_i\|×α) | O(1) per char | O(Σ\|P_i\| + N) |

### Implementation Complexity Trade-off

AC is complex to implement correctly in an interview. Recommended approach:
1. Start with KMP if patterns are small
2. Escalate to AC if interviewer asks for optimization
3. Always mention AC as the gold standard even if you implement something simpler

---

## Interview Tips

> **Failure links vs. suffix links:** "Failure link" (AC terminology) = "suffix link" (suffix automaton terminology). Both mean: "longest proper suffix that exists elsewhere in structure."

> **Why BFS for failure links?** Failure links always point to shallower nodes. BFS guarantees parent's failure link is computed before child's.

> **AC vs Trie for Word Search (LeetCode 212):** Trie + DFS backtracking is simpler and usually preferred in interviews. AC is better when text is fixed and patterns vary, or streaming.

> **Common bug:** Forgetting to merge output links during BFS. Without this, you miss patterns that are suffixes of other patterns.

---

## Edge Cases

```python
# 1. Pattern is prefix of another pattern
patterns = ["ab", "abcd"]
# Both should be found independently

# 2. Pattern is suffix of another
patterns = ["bc", "abcd"]  
# "bc" at position 1 in "abcd" should be found

# 3. Overlapping patterns
patterns = ["aa", "aaa"]
text = "aaaa"
# "aa" at 0,1,2; "aaa" at 0,1

# 4. Empty text
assert find_all_occurrences("", ["abc"]) == {"abc": []}

# 5. Pattern longer than text
assert find_all_occurrences("ab", ["abcde"]) == {"abcde": []}

# 6. Duplicate patterns
patterns = ["ab", "ab"]  
# Should count independently

# 7. Single character patterns
text = "abcabc"
patterns = ["a", "b", "c"]
# Each appears twice
```

---

*Previous: [KMP & Z-Algorithm ←](01_KMP_And_Z_Algorithm.md) | Next: [Suffix Array & LCP →](03_Suffix_Array_And_LCP.md)*

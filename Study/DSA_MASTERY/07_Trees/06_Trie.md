# Trie — Full Mastery with XOR, Wildcards, and String Search

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Hash maps, DFS, bitwise operations
> **Core Theme:** Prefix-tree structures for O(L) string operations and
> bit-level decomposition for XOR maximization.

---

## 1. Standard Trie — Full Implementation

```python
class TrieNode:
    __slots__ = ['children', 'is_end', 'count']
    def __init__(self):
        self.children = {}
        self.is_end = False
        self.count = 0

class Trie:
    def __init__(self):
        self.root = TrieNode()

    def insert(self, word: str) -> None:
        node = self.root
        for ch in word:
            if ch not in node.children:
                node.children[ch] = TrieNode()
            node = node.children[ch]
            node.count += 1
        node.is_end = True

    def search(self, word: str) -> bool:
        node = self.root
        for ch in word:
            if ch not in node.children:
                return False
            node = node.children[ch]
        return node.is_end

    def startsWith(self, prefix: str) -> bool:
        node = self.root
        for ch in prefix:
            if ch not in node.children:
                return False
            node = node.children[ch]
        return True

    def delete(self, word: str) -> bool:
        def _delete(node, word, depth):
            if depth == len(word):
                if not node.is_end:
                    return False
                node.is_end = False
                return len(node.children) == 0

            ch = word[depth]
            if ch not in node.children:
                return False

            should_delete_child = _delete(node.children[ch], word, depth + 1)
            if should_delete_child:
                del node.children[ch]
                return not node.is_end and len(node.children) == 0
            return False

        return _delete(self.root, word, 0)
```
**Insert/Search/Delete:** O(L) where L = word length | **Space:** O(N × L)

---

## 2. XOR Trie for Maximum XOR (LeetCode 421)

**Bit-by-bit traversal from MSB to LSB. Greedily choose opposite bit.**

```python
class XORTrie:
    def __init__(self, bit_length: int = 30):
        self.root = {}
        self.BITS = bit_length

    def insert(self, num: int) -> None:
        node = self.root
        for i in range(self.BITS, -1, -1):
            bit = (num >> i) & 1
            if bit not in node:
                node[bit] = {}
            node = node[bit]

    def max_xor(self, num: int) -> int:
        node = self.root
        xor = 0
        for i in range(self.BITS, -1, -1):
            bit = (num >> i) & 1
            want = 1 - bit
            if want in node:
                xor |= (1 << i)
                node = node[want]
            else:
                node = node[bit]
        return xor

def findMaximumXOR(nums: list[int]) -> int:
    trie = XORTrie()
    for num in nums:
        trie.insert(num)
    return max(trie.max_xor(num) for num in nums)
```
**Time:** O(N × B) where B = bit length (32) | **Space:** O(N × B)

---

## 3. Auto-Complete System — Top-3 Suggestions (LeetCode 1268)

```python
from bisect import bisect_left

class SearchSuggestionsSystem:
    def __init__(self, products: list[str], searchWord: str):
        products.sort()
        self.products = products
        self.result = []

        prefix = ""
        for ch in searchWord:
            prefix += ch
            self.result.append(self._get_suggestions(prefix))

    def _get_suggestions(self, prefix: str) -> list[str]:
        i = bisect_left(self.products, prefix)
        suggestions = []
        while i < len(self.products) and len(suggestions) < 3:
            if self.products[i].startswith(prefix):
                suggestions.append(self.products[i])
            else:
                break
            i += 1
        return suggestions
```
**Time:** O(N log N + L log N) | **Space:** O(N)

---

## 4. Add and Search Word — Wildcard (LeetCode 211)

```python
class WordDictionary:
    def __init__(self):
        self.root = {}

    def addWord(self, word: str) -> None:
        node = self.root
        for ch in word:
            node = node.setdefault(ch, {})
        node['#'] = True

    def search(self, word: str) -> bool:
        def dfs(node, i):
            if i == len(word):
                return '#' in node
            ch = word[i]
            if ch == '.':
                return any(dfs(child, i+1) for key, child in node.items()
                           if key != '#')
            if ch not in node:
                return False
            return dfs(node[ch], i+1)

        return dfs(self.root, 0)
```
**Time:** O(L) average, O(N×L) worst (many '.') | **Space:** O(N×L)

---

## 5. Replace Words with Roots (LeetCode 648)

```python
def replaceWords(dictionary: list[str], sentence: str) -> str:
    trie = {}
    for root in dictionary:
        node = trie
        for ch in root:
            node = node.setdefault(ch, {})
        node['#'] = root

    def find_root(word):
        node = trie
        for ch in word:
            if ch not in node:
                return word
            node = node[ch]
            if '#' in node:
                return node['#']
        return word

    return ' '.join(find_root(word) for word in sentence.split())
```
**Time:** O(D×L + S×L) | **Space:** O(D×L)

---

## 6. Word Search II — Trie + Backtracking (LeetCode 212)

```python
def findWords(board: list[list[str]], words: list[str]) -> list[str]:
    trie = {}
    for word in words:
        node = trie
        for ch in word:
            node = node.setdefault(ch, {})
        node['#'] = word

    rows, cols = len(board), len(board[0])
    result = set()

    def dfs(node, r, c):
        ch = board[r][c]
        if ch not in node:
            return
        next_node = node[ch]
        if '#' in next_node:
            result.add(next_node['#'])
            del next_node['#']    # Avoid duplicates

        board[r][c] = '!'
        for dr, dc in [(-1,0),(1,0),(0,-1),(0,1)]:
            nr, nc = r + dr, c + dc
            if 0 <= nr < rows and 0 <= nc < cols and board[nr][nc] != '!':
                dfs(next_node, nr, nc)
        board[r][c] = ch

        # Pruning: remove empty trie nodes
        if not next_node:
            del node[ch]

    for r in range(rows):
        for c in range(cols):
            dfs(trie, r, c)

    return list(result)
```
**Time:** O(M × N × 4^L) | **Space:** O(W×L)

**Pruning:** `del node[ch]` removes exhausted trie branches, preventing re-exploration.

---

## 7. Palindrome Pairs (LeetCode 336)

```python
def palindromePairs(words: list[str]) -> list[list[int]]:
    def is_palindrome(s, lo, hi):
        while lo < hi:
            if s[lo] != s[hi]:
                return False
            lo += 1; hi -= 1
        return True

    word_index = {word: i for i, word in enumerate(words)}
    result = []

    for i, word in enumerate(words):
        n = len(word)
        for j in range(n + 1):
            prefix, suffix = word[:j], word[j:]
            # Case 1: prefix is palindrome, find reverse(suffix)
            if is_palindrome(prefix, 0, len(prefix)-1):
                rev_suffix = suffix[::-1]
                if rev_suffix in word_index and word_index[rev_suffix] != i:
                    result.append([word_index[rev_suffix], i])
            # Case 2: suffix is palindrome, find reverse(prefix)
            if j != n and is_palindrome(suffix, 0, len(suffix)-1):
                rev_prefix = prefix[::-1]
                if rev_prefix in word_index and word_index[rev_prefix] != i:
                    result.append([i, word_index[rev_prefix]])

    return result
```
**Time:** O(N × L²) | **Space:** O(N × L)

---

## 8. Longest Word in Dictionary (LeetCode 720)

```python
def longestWord(words: list[str]) -> str:
    word_set = set(words)
    result = ""

    for word in sorted(words, key=lambda w: (len(w), w)):
        if len(word) == 1 or word[:-1] in word_set:
            if len(word) > len(result):
                result = word

    return result
```
**Time:** O(N×L) | **Space:** O(N×L)

---

## 9. Stream of Characters (LeetCode 1032)

```python
class StreamChecker:
    def __init__(self, words: list[str]):
        self.trie = {}
        for word in words:
            node = self.trie
            for ch in reversed(word):
                node = node.setdefault(ch, {})
            node['#'] = True

        self.stream = []

    def query(self, letter: str) -> bool:
        self.stream.append(letter)
        node = self.trie
        for ch in reversed(self.stream):
            if ch not in node:
                return False
            node = node[ch]
            if '#' in node:
                return True
        return False
```
**Time:** O(L) per query where L = max word length | **Space:** O(W×L)

---

## Trie Complexity Summary

| Operation | Time | Space |
|---|---|---|
| Insert word of length L | O(L) | O(L) new nodes |
| Search word | O(L) | O(1) |
| Prefix search | O(L) | O(1) |
| Delete word | O(L) | O(1) |
| XOR maximum | O(N×32) | O(N×32) |
| Word search on board | O(MN×4^L) | O(W×L) |

## Interview Tips

1. **Dictionary vs array for children:** Use `{}` for large alphabets, `[None]*26` for lowercase-only.
2. **XOR trie:** Always process from MSB (bit 30 or 31) to bit 0. Insert 0 if XOR with 0 is valid.
3. **Word search II pruning:** The `del node[ch]` optimization is expected at FAANG interviews.
4. **Palindrome pairs:** The split-at-each-position trick (cases 1 and 2) is the key insight.
5. **Stream checker:** Insert words reversed — then check suffixes of the stream by reading it backward.

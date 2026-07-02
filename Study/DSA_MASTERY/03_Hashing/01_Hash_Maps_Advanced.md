# Hash Maps — Advanced Mastery Guide

## Core Concept & Invariant

A hash map achieves O(1) expected time for get/put by mapping keys to array indices
via a hash function. The fundamental invariant is **uniformity**: a good hash function
distributes keys uniformly across buckets, minimizing collisions.

**Load factor α = n/m** (n = keys, m = buckets):
- Expected chain length per bucket = α (with chaining)
- Amortized O(1) per operation when α is kept constant (e.g., α < 0.75)
- When α > threshold: rehash — double buckets, re-insert all keys → O(n) but amortized O(1)

**Collision resolution**:
1. **Separate Chaining**: Each bucket is a linked list. Worst case O(n) per op (all keys hash to one bucket). Average O(1+α).
2. **Open Addressing** (linear/quadratic probing, double hashing): All elements stored in table. Better cache locality. Degrades faster at high load.

**Why O(1) amortized** (not just average): With rehashing at load factor > c:
- n insertions cause O(log n) rehashes → total rehash work = n + n/2 + n/4 + ... = O(n)
- Per insertion amortized: O(n)/n = O(1) ✓

---

## Hash Collision Resolution Deep Dive

```python
# ─────────────────────────────────────────────────────────────
# Design HashMap from Scratch (Separate Chaining)
# ─────────────────────────────────────────────────────────────
class MyHashMap:
    """
    Open-addressing alternative for interview: use array of lists (chaining).
    Key design decisions:
    1. Initial capacity (prime to reduce collisions)
    2. Load factor threshold for rehashing
    3. Hash function quality
    """
    def __init__(self, initial_capacity: int = 997):   # Prime number
        self.capacity = initial_capacity
        self.size = 0
        self.table = [[] for _ in range(self.capacity)]
        self.LOAD_FACTOR = 0.75
    
    def _hash(self, key: int) -> int:
        """
        Polynomial hash with prime multiplier.
        For integer keys: simple modulo works but can have bad patterns.
        Better: multiply by large prime to scatter bits.
        """
        return key % self.capacity
    
    def _rehash(self):
        """Double capacity when load factor exceeded."""
        old_table = self.table
        self.capacity *= 2
        self.size = 0
        self.table = [[] for _ in range(self.capacity)]
        for bucket in old_table:
            for key, val in bucket:
                self.put(key, val)
    
    def put(self, key: int, value: int) -> None:
        if self.size / self.capacity >= self.LOAD_FACTOR:
            self._rehash()
        
        h = self._hash(key)
        for i, (k, v) in enumerate(self.table[h]):
            if k == key:
                self.table[h][i] = (key, value)   # Update
                return
        self.table[h].append((key, value))
        self.size += 1
    
    def get(self, key: int) -> int:
        h = self._hash(key)
        for k, v in self.table[h]:
            if k == key:
                return v
        return -1
    
    def remove(self, key: int) -> None:
        h = self._hash(key)
        self.table[h] = [(k, v) for k, v in self.table[h] if k != key]

# Time: O(1) amortized for put/get/remove
# Space: O(n) where n = number of keys

# ─────────────────────────────────────────────────────────────
# Open Addressing with Linear Probing
# ─────────────────────────────────────────────────────────────
class OpenAddressHashMap:
    """
    Linear probing: on collision, try next slot.
    Cache-friendly (sequential memory) but suffers from clustering.
    
    Deletion is tricky: cannot just clear slot — would break probe chains.
    Solution: use "tombstone" sentinel for deleted slots.
    """
    EMPTY = object()
    DELETED = object()
    
    def __init__(self, capacity: int = 16):
        self.capacity = capacity
        self.table = [self.EMPTY] * capacity
        self.vals = [None] * capacity
        self.size = 0
    
    def _probe(self, key: int):
        h = key % self.capacity
        while (self.table[h] is not self.EMPTY and
               self.table[h] is not self.DELETED and
               self.table[h] != key):
            h = (h + 1) % self.capacity   # Linear probing
        return h
    
    def put(self, key: int, val: int) -> None:
        if self.size / self.capacity >= 0.5:   # Lower threshold for open addressing
            # Rehash (rebuild without tombstones)
            old = [(k, v) for k, v in zip(self.table, self.vals)
                   if k is not self.EMPTY and k is not self.DELETED]
            self.capacity *= 2
            self.table = [self.EMPTY] * self.capacity
            self.vals = [None] * self.capacity
            self.size = 0
            for k, v in old:
                self.put(k, v)
        
        h = self._probe(key)
        if self.table[h] is self.EMPTY or self.table[h] is self.DELETED:
            self.size += 1
        self.table[h] = key
        self.vals[h] = val
    
    def get(self, key: int) -> int:
        h = self._probe(key)
        if self.table[h] == key:
            return self.vals[h]
        return -1
    
    def remove(self, key: int) -> None:
        h = self._probe(key)
        if self.table[h] == key:
            self.table[h] = self.DELETED   # Tombstone, not EMPTY
            self.size -= 1
```

---

## Advanced Hashing Patterns

### Pattern 1: Frequency Counting

```python
from collections import Counter, defaultdict

def top_k_frequent(nums: list, k: int) -> list:
    """
    Top K most frequent elements.
    
    Approach 1: Counter + heap → O(n log k)
    Approach 2: Bucket sort → O(n) (bounded frequency)
    
    Bucket sort insight: frequency ∈ [1, n], so use array of size n+1 as buckets.
    """
    count = Counter(nums)
    
    # Bucket sort by frequency
    freq_buckets = [[] for _ in range(len(nums) + 1)]
    for val, freq in count.items():
        freq_buckets[freq].append(val)
    
    result = []
    for freq in range(len(freq_buckets)-1, 0, -1):
        for val in freq_buckets[freq]:
            result.append(val)
            if len(result) == k:
                return result
    return result

# Time: O(n)  Space: O(n)

def top_k_heap(nums: list, k: int) -> list:
    """Heap approach: O(n log k) — better when k << n."""
    import heapq
    count = Counter(nums)
    return heapq.nlargest(k, count.keys(), key=count.get)
# Time: O(n log k)  Space: O(n)
```

### Pattern 2: Anagram Grouping

```python
def group_anagrams(strs: list) -> list:
    """
    Group strings that are anagrams of each other.
    
    Key: anagrams have the same sorted form (or same character count).
    Use sorted string or character frequency tuple as hash key.
    
    Frequency tuple approach (O(n×l) vs O(n×l log l) for sorted):
    For alphabet of size 26: tuple of 26 counts → unique key per anagram class.
    """
    from collections import defaultdict
    
    groups = defaultdict(list)
    for s in strs:
        # Method 1: Sort the string (O(l log l) per string)
        key = tuple(sorted(s))
        
        # Method 2: Character frequency tuple (O(l) per string)
        # count = [0] * 26
        # for c in s: count[ord(c) - ord('a')] += 1
        # key = tuple(count)
        
        groups[key].append(s)
    
    return list(groups.values())

# Time: O(n × l log l) with sorting, O(n × l) with frequency tuple
# Space: O(n × l) for the groups
```

### Pattern 3: Two-Sum Family

```python
def two_sum(nums: list, target: int) -> list:
    """Classic two-sum: O(n) time, O(n) space."""
    seen = {}   # value → index
    for i, x in enumerate(nums):
        comp = target - x
        if comp in seen:
            return [seen[comp], i]
        seen[x] = i
    return []

def two_sum_count_pairs(nums: list, diff: int) -> int:
    """
    Count pairs with absolute difference = diff.
    Sort + two-pointer: O(n log n)
    Hash map: O(n)
    """
    count_map = Counter(nums)
    pairs = 0
    
    if diff == 0:
        # Same element, count pairs: C(freq, 2)
        for freq in count_map.values():
            pairs += freq * (freq - 1) // 2
    else:
        for x in count_map:
            if x + diff in count_map:
                pairs += count_map[x] * count_map[x + diff]
    
    return pairs

def four_sum_count(A: list, B: list, C: list, D: list) -> int:
    """
    Count tuples (i,j,k,l) with A[i]+B[j]+C[k]+D[l]=0.
    
    Split into two halves: count A[i]+B[j] sums, then look up -(C[k]+D[l]).
    O(n²) time, O(n²) space — optimal for 4-list problem.
    """
    ab_sums = Counter(a + b for a in A for b in B)
    return sum(ab_sums[-(c+d)] for c in C for d in D)
# Time: O(n²)  Space: O(n²)
```

---

## Classic Problems

### Problem 1: LRU Cache — O(1) get/put — Hard

```python
from collections import OrderedDict

class LRUCache:
    """
    O(1) get and put using OrderedDict (maintains insertion order).
    
    OrderedDict = doubly-linked list + hash map.
    move_to_end(key): O(1) — relinks pointers
    popitem(last=False): O(1) — removes and returns first item (LRU)
    
    Custom implementation (for interview coding from scratch):
    Doubly linked list for O(1) move/remove + hash map for O(1) access.
    """
    def __init__(self, capacity: int):
        self.cap = capacity
        self.cache = OrderedDict()
    
    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        self.cache.move_to_end(key)   # Most recently used
        return self.cache[key]
    
    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self.cache.move_to_end(key)
        self.cache[key] = value
        if len(self.cache) > self.cap:
            self.cache.popitem(last=False)   # Evict LRU

# ─── Custom LRU from scratch (doubly-linked list + dict) ───
class DLinkedNode:
    def __init__(self, key=0, val=0):
        self.key = key; self.val = val
        self.prev = self.next = None

class LRUCacheCustom:
    """
    head.next = most recently used
    tail.prev = least recently used
    
    All operations O(1) worst case (not just amortized).
    """
    def __init__(self, capacity: int):
        self.cap = capacity
        self.cache = {}   # key → DLinkedNode
        # Sentinel nodes
        self.head = DLinkedNode()
        self.tail = DLinkedNode()
        self.head.next = self.tail
        self.tail.prev = self.head
    
    def _remove(self, node: DLinkedNode) -> None:
        node.prev.next = node.next
        node.next.prev = node.prev
    
    def _add_to_front(self, node: DLinkedNode) -> None:
        node.next = self.head.next
        node.prev = self.head
        self.head.next.prev = node
        self.head.next = node
    
    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        node = self.cache[key]
        self._remove(node)
        self._add_to_front(node)
        return node.val
    
    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            node = self.cache[key]
            node.val = value
            self._remove(node)
            self._add_to_front(node)
        else:
            if len(self.cache) == self.cap:
                lru = self.tail.prev
                self._remove(lru)
                del self.cache[lru.key]
            node = DLinkedNode(key, value)
            self.cache[key] = node
            self._add_to_front(node)

# Time: O(1) per get/put  Space: O(capacity)
```

### Problem 2: Longest Consecutive Sequence — O(N) — Hard

```python
def longest_consecutive(nums: list) -> int:
    """
    Find the length of the longest consecutive elements sequence.
    
    O(n log n) naive: sort and scan.
    O(n) via hash set: for each number, only start counting if it's
    the START of a sequence (num-1 not in set).
    
    Why O(n):
    - Each number is inserted into the set once.
    - Each number is the "start" of a sequence at most once (when num-1 not in set).
    - Each number is visited at most once in the inner while loop
      (across all iterations of the outer for loop — total inner iterations = n).
    
    The total work across all inner while loops = O(n) because each element
    participates in exactly one sequence count.
    """
    num_set = set(nums)
    best = 0
    
    for n in num_set:
        if n - 1 not in num_set:   # Start of a new sequence
            curr = n
            length = 1
            while curr + 1 in num_set:
                curr += 1
                length += 1
            best = max(best, length)
    
    return best
# Time: O(n)  Space: O(n)
```

### Problem 3: Valid Sudoku — Medium

```python
def is_valid_sudoku(board: list) -> bool:
    """
    Check if 9x9 Sudoku board is valid.
    
    Three constraints:
    1. Each row has digits 1-9 with no repeats
    2. Each column has digits 1-9 with no repeats
    3. Each 3x3 box has digits 1-9 with no repeats
    
    Hash set approach: single pass O(81) = O(1) since board is fixed 9x9.
    
    Key: use set comprehension to detect duplicates efficiently.
    Encode (value, position) pairs to distinguish row/col/box constraints.
    """
    rows = defaultdict(set)
    cols = defaultdict(set)
    boxes = defaultdict(set)
    
    for r in range(9):
        for c in range(9):
            val = board[r][c]
            if val == '.':
                continue
            
            box_id = (r // 3, c // 3)
            
            if val in rows[r] or val in cols[c] or val in boxes[box_id]:
                return False
            
            rows[r].add(val)
            cols[c].add(val)
            boxes[box_id].add(val)
    
    return True
# Time: O(81) = O(1) for fixed 9x9 board  Space: O(81) = O(1)
```

### Problem 4: Find All Duplicates in Array — Medium

```python
def find_duplicates(nums: list) -> list:
    """
    Find all elements that appear twice in nums where 1 ≤ nums[i] ≤ n.
    
    O(1) space approach: use the array itself as a hash map.
    For each num, negate nums[|num|-1].
    If nums[|num|-1] is already negative when we visit |num|: it appeared twice.
    
    This works because values are in [1,n] and indices are in [0,n-1]:
    value v → index v-1 → negate arr[v-1] to "mark" v as seen.
    """
    result = []
    for num in nums:
        idx = abs(num) - 1
        if nums[idx] < 0:
            result.append(abs(num))   # Seen before → duplicate
        else:
            nums[idx] = -nums[idx]   # Mark as seen
    
    # Restore array (optional)
    for i in range(len(nums)):
        nums[i] = abs(nums[i])
    
    return result
# Time: O(n)  Space: O(1) auxiliary (not counting output)

def find_disappeared(nums: list) -> list:
    """
    Find all numbers in [1,n] that are missing from nums.
    Same index-as-hashmap trick.
    """
    for num in nums:
        idx = abs(num) - 1
        nums[idx] = -abs(nums[idx])
    
    result = [i+1 for i, x in enumerate(nums) if x > 0]
    return result
# Time: O(n)  Space: O(1)
```

### Problem 5: Subarray Sum Equals K — O(N) — Medium

```python
def subarray_sum(nums: list, k: int) -> int:
    """
    Count subarrays with sum = k.
    
    prefix_sum[r+1] - prefix_sum[l] = k
    → prefix_sum[l] = prefix_sum[r+1] - k
    
    As we compute running prefix sum, for each position r:
    count += occurrences of (current_prefix - k) in hash map
    
    Works for negative numbers, k=0, duplicate values.
    """
    count = defaultdict(int)
    count[0] = 1   # Empty prefix
    prefix = 0
    result = 0
    
    for x in nums:
        prefix += x
        result += count[prefix - k]
        count[prefix] += 1
    
    return result
# Time: O(n)  Space: O(n)
```

### Problem 6: Randomized Hashing & Universal Hash Families — Advanced

```python
import random

class UniversalHashFamily:
    """
    Universal hash family: h_{a,b}(x) = ((ax + b) mod p) mod m
    where p is prime > m.
    
    For any two distinct keys x, y:
    Pr[h(x) = h(y)] ≤ 1/m
    
    This guarantees expected O(1) per operation even against adversarial inputs.
    Standard Python dict is vulnerable to hash collision attacks (HashDoS).
    Universal hashing provides provable guarantees.
    """
    PRIME = (1 << 31) - 1   # Mersenne prime
    
    def __init__(self, m: int):
        self.m = m
        self.a = random.randint(1, self.PRIME - 1)
        self.b = random.randint(0, self.PRIME - 1)
    
    def hash(self, x: int) -> int:
        return ((self.a * x + self.b) % self.PRIME) % self.m
    
    def rehash(self):
        """Randomize hash function for new epoch."""
        self.a = random.randint(1, self.PRIME - 1)
        self.b = random.randint(0, self.PRIME - 1)

class RobustHashMap:
    """
    Adversary-resistant hash map using universal hashing.
    
    Standard Python dict uses SipHash-1-3 with random seed per process.
    For interview: explain WHY randomization matters.
    
    HashDoS attack: adversary crafts inputs with same hash → O(n) per op.
    Universal hashing: hash function chosen at runtime → adversary can't predict.
    """
    def __init__(self, initial_m: int = 16):
        self.m = initial_m
        self.hasher = UniversalHashFamily(self.m)
        self.buckets = [[] for _ in range(self.m)]
        self.size = 0
    
    def put(self, key: int, val: int) -> None:
        if self.size / self.m > 0.75:
            self._rehash()
        h = self.hasher.hash(key)
        for i, (k, v) in enumerate(self.buckets[h]):
            if k == key:
                self.buckets[h][i] = (key, val)
                return
        self.buckets[h].append((key, val))
        self.size += 1
    
    def _rehash(self):
        self.m *= 2
        self.hasher = UniversalHashFamily(self.m)   # New random hash function
        old_buckets = self.buckets
        self.buckets = [[] for _ in range(self.m)]
        self.size = 0
        for bucket in old_buckets:
            for key, val in bucket:
                self.put(key, val)
```

### Problem 7: Word Pattern and Bijection Detection — Medium

```python
def word_pattern(pattern: str, s: str) -> bool:
    """
    Check if s follows the same pattern as pattern string.
    e.g., pattern="abba", s="dog cat cat dog" → True
    
    Two-way bijection: each char in pattern maps to unique word AND vice versa.
    Single hash map only checks one direction.
    Need BOTH directions to detect symmetric mismatches.
    """
    words = s.split()
    if len(pattern) != len(words):
        return False
    
    char_to_word = {}
    word_to_char = {}
    
    for ch, word in zip(pattern, words):
        if ch in char_to_word:
            if char_to_word[ch] != word:
                return False   # Char mapped to different word
        else:
            if word in word_to_char:
                return False   # Word mapped to different char
            char_to_word[ch] = word
            word_to_char[word] = ch
    
    return True

def is_isomorphic(s: str, t: str) -> bool:
    """Same bijection check between two strings."""
    s_to_t = {}
    t_to_s = {}
    for cs, ct in zip(s, t):
        if cs in s_to_t and s_to_t[cs] != ct: return False
        if ct in t_to_s and t_to_s[ct] != cs: return False
        s_to_t[cs] = ct
        t_to_s[ct] = cs
    return True
# Time: O(n)  Space: O(alphabet_size) = O(1)
```

---

## Advanced Variations

### Count Pairs with Given Difference K

```python
def count_pairs_with_diff_k(nums: list, k: int) -> int:
    """
    Count pairs (i,j) with i < j and nums[j] - nums[i] = k.
    
    Hash map: for each nums[j], count occurrences of nums[j] - k.
    Process left to right — only count previous occurrences.
    """
    count = defaultdict(int)
    result = 0
    
    for x in nums:
        result += count[x - k]   # Count elements that are exactly k less
        count[x] += 1
    
    return result
# Time: O(n)  Space: O(n)
```

### Minimum Window Substring with Frequency

```python
def min_window_with_frequency(s: str, t: str) -> str:
    """
    t may have repeated characters. Window must contain all chars with their full counts.
    Already covered in Sliding Window — here shown as hash map problem.
    """
    from collections import Counter
    need = Counter(t)
    window = Counter()
    satisfied = 0   # How many distinct chars fully satisfied
    required = len(need)
    lo = 0
    best = ""
    
    for hi, ch in enumerate(s):
        window[ch] += 1
        if ch in need and window[ch] == need[ch]:
            satisfied += 1
        
        while satisfied == required:
            candidate = s[lo:hi+1]
            if not best or len(candidate) < len(best):
                best = candidate
            
            left = s[lo]
            window[left] -= 1
            if left in need and window[left] < need[left]:
                satisfied -= 1
            lo += 1
    
    return best
# Time: O(|s| + |t|)  Space: O(|t|)
```

---

## Edge Cases Bible

1. **Hash collision intentional (HashDoS)**: Real systems (web servers) can be attacked by sending many strings with the same hash. Python uses per-process random seed (SipHash) as defense. Mention this in system design discussions.

2. **Mutable keys**: Never use mutable objects (lists, dicts) as hash map keys. Python raises `TypeError`. Use `tuple(list)` or `frozenset(set)` as workaround.

3. **Float keys**: Floating point comparison is unreliable. `0.1 + 0.2 != 0.3` means `{0.1+0.2: "x"}` and `{0.3: "x"}` are different. Avoid float keys; use integer or string representations.

4. **None as key**: Valid in Python (`{None: 1}` works). Not valid in Java's HashMap (NPE). Clarify language in interview.

5. **Bijection vs injection**: `word_pattern` needs BOTH directions. Common bug: only checking `char → word` mapping misses cases like pattern="ab", s="cat cat" (same word for different chars).

6. **LRU Cache with capacity=0**: Illegal but guard against it. Each `put` would immediately evict — undefined behavior. Add `assert capacity > 0`.

7. **Two-sum with duplicate elements**: e.g., nums=[3,3], target=6 → [0,1]. Adding nums[i] to map AFTER checking for complement ensures the element doesn't pair with itself (lo=hi).

8. **Count pairs with k=0**: `count_pairs_with_diff_k` with k=0 should count pairs of equal elements: C(freq, 2). Handle as special case if needed.

9. **Counter for strings with unicode**: `Counter(s)` handles unicode correctly, but `[0]*26` only handles lowercase ASCII. Use `defaultdict(int)` for general case.

10. **OrderedDict not actually doubly-linked in CPython before 3.7**: Since Python 3.7, `dict` maintains insertion order. `OrderedDict.move_to_end` is O(1). Earlier versions: use explicit doubly-linked list.

---

## Interview Tips

### What Interviewers Look For

1. **State the load factor and rehashing**: "Python's dict uses a load factor of ~0.67. When exceeded, it doubles capacity and rehashes — this is why dict has O(1) amortized, not worst-case O(1)." Shows deep understanding.

2. **LRU Cache — implement from scratch**: Interviewers often ask for the custom doubly-linked list + hash map implementation. Know the `_remove` and `_add_to_front` helper pattern cold.

3. **Universal hashing mention**: For senior roles, mentioning "standard hash maps are vulnerable to HashDoS; production systems use randomized universal hashing or cryptographic hashes for untrusted input" distinguishes you.

4. **Frequency counting tricks**:
   - `Counter(s)` for string character counts
   - `Counter(s) == Counter(t)` for anagram check
   - `Counter(s).most_common(k)` for top-K
   - `{tuple(sorted(s)): group}` for anagram grouping

5. **Two-sum always O(n) not O(n log n)**: Never say "I'll sort and binary search for two-sum" — that's O(n log n). Hash map gives O(n). Interviewers know this.

6. **The "index-as-hashmap" pattern**: When values are in [1,n], the array itself serves as a hash map (value → index mapping). This gives O(1) space solutions for finding duplicates/missing numbers.

7. **When hash map is WRONG**: 
   - Ordered/sorted queries → use sorted array + binary search or BST
   - Range queries → prefix sum, segment tree, Fenwick tree
   - String pattern matching → rolling hash, KMP, suffix array
   - Approximate membership → Bloom filter

8. **Python-specific optimizations**:
   - `defaultdict(int)` over `dict` with `.get(key, 0)` — cleaner
   - `Counter` for frequency tasks — built-in operations
   - `collections.OrderedDict` for LRU — O(1) `move_to_end`
   - `set` for O(1) membership check vs `list` O(n)

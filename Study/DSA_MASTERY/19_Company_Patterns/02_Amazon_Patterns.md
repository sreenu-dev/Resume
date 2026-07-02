# Amazon Interview Patterns — Advanced Mastery Guide

> **Level:** Advanced | **Target:** SDE I / SDE II / SDE III roles  
> **Interview Frequency:** ★★★★★ (Core company guide)

---

## Table of Contents
1. [Amazon's Interview Philosophy](#1-amazons-interview-philosophy)
2. [Amazon's Format — LP + Coding](#2-amazons-format--lp--coding)
3. [Amazon Coding Focus Areas](#3-amazon-coding-focus-areas)
4. [The Scale Angle](#4-the-scale-angle)
5. [Problem 1: LRU Cache](#5-problem-1-lru-cache)
6. [Problem 2: Merge K Sorted Lists](#6-problem-2-merge-k-sorted-lists)
7. [Problem 3: Word Ladder](#7-problem-3-word-ladder)
8. [Problem 4: Sliding Window Maximum](#8-problem-4-sliding-window-maximum)
9. [Problem 5: Top K Frequent Elements](#9-problem-5-top-k-frequent-elements)
10. [Problem 6: Find Median from Data Stream](#10-problem-6-find-median-from-data-stream)
11. [Problem 7: Number of Islands](#11-problem-7-number-of-islands)
12. [Problem 8: Serialize / Deserialize Binary Tree](#12-problem-8-serialize--deserialize-binary-tree)
13. [LP Integration Tips](#13-lp-integration-tips)

---

## 1. Amazon's Interview Philosophy

Amazon's coding interviews are shaped by two pillars:

**1. Leadership Principles (LPs):** Amazon's 16 LPs aren't just behavioral — they manifest in HOW you code. "Deliver Results" means finishing a working solution. "Insist on Highest Standards" means handling edge cases. "Think Big" means discussing scalability.

**2. Practical Problem-Solving:** Amazon focuses on practical, scalable solutions. Less theory, more "would this work at Amazon's scale (10^9 transactions/day)?"

```
Amazon's Bar by Level:
SDE I (entry): Solve 1-2 medium problems, basic OOP, discuss LP examples
SDE II:        Solve medium-hard, design discussion, clear code quality
SDE III:       Hard problems, system design, LP depth, mentor signals
Principal:     Architectural decisions, org-wide impact problems
```

---

## 2. Amazon's Format — LP + Coding

```
Standard Full Loop (4-6 rounds):
  Round 1: Online Assessment (2 coding problems, 70 minutes)
  Round 2: Phone Screen (1-2 coding problems, 45 minutes)
  Onsite (4 rounds × 60 minutes each):
    - 2 Coding rounds (each: LP questions + 1-2 coding problems)
    - 1 System Design round (SDE II+)
    - 1 Bar Raiser round (LP-heavy + coding)

Unique to Amazon:
- EVERY coding round starts with 10-15 minutes of LP questions
- "Bar Raiser" is a specially trained interviewer ensuring calibration
- Online Assessment uses Amazon's CodeSignal-style platform
```

### LP Questions to Prepare (Connected to Coding)

```python
# LP: Customer Obsession
# "Tell me about a time you went above and beyond for a customer/user"
# Coding connection: "Who uses this API? How would a bug here affect them?"

# LP: Deliver Results
# "Tell me about a time you delivered under tight deadline"
# Coding connection: Complete working solution > perfect incomplete solution

# LP: Dive Deep
# "Tell me about a time you identified a root cause"
# Coding connection: Debug your code, don't just guess

# LP: Invent and Simplify
# "Tell me about a time you simplified a complex process"
# Coding connection: Elegant solutions, reducing code complexity

# LP: Bias for Action
# "Tell me about a time you took initiative without full information"
# Coding connection: "I'll make an assumption here and flag it" vs waiting

# LP: Insist on Highest Standards
# "Tell me about a time you raised the bar"
# Coding connection: Edge cases, error handling, type hints, clear naming
```

---

## 3. Amazon Coding Focus Areas

| Topic | Amazon Frequency | Notes |
|-------|-----------------|-------|
| Trees (BST, BT) | ★★★★★ | Most common data structure |
| Graphs (BFS/DFS) | ★★★★☆ | Islands, connected components |
| Arrays + Two Pointer | ★★★★☆ | Sliding window, merge |
| DP (medium) | ★★★★☆ | Usually medium difficulty |
| Heap / Priority Queue | ★★★★☆ | Top-K, median, merging |
| Linked Lists | ★★★★☆ | Reverse, cycle, merge |
| String manipulation | ★★★★☆ | Parsing, anagrams |
| OOP / LLD | ★★★★☆ | Amazon loves design questions |
| System Design | ★★★★☆ | SDE II+ mandatory |
| Advanced (Seg Tree, etc.) | ★★☆☆☆ | Rare; save for Senior+ |

---

## 4. The Scale Angle

Amazon ALWAYS asks about scale. Be ready for:

```python
# Standard coding question + Amazon scale follow-up:

# Q: "How would you find the top 10 trending products?"
# Standard: Sort by frequency → O(N log N), take first 10
# Amazon follow-up: "What if you have 10^9 products per day?"

# Scale-aware answer:
def top_k_at_scale(stream, k):
    """
    For massive scale:
    1. Distributed counting: map each product to a shard
    2. Each shard maintains a min-heap of size K
    3. Merge K heaps from M shards: O(K*M) per merge cycle
    4. Use reservoir sampling for truly infinite streams
    5. Count-Min Sketch for approximate counting with bounded memory
    """
    pass

# Amazon scale questions to prepare for:
scale_questions = [
    "What if N = 10^9?",
    "What if we can't fit the data in memory?",
    "What if we have 1000 servers?",
    "What if requests come in real-time?",
    "What's the bottleneck in your solution?",
    "How would you test this at scale?",
]

# For each solution, have a SCALE answer ready:
scale_answers = {
    "Can't fit in memory": "External sort, or streaming algorithms",
    "Too slow": "Parallel processing, distributed computing",
    "Real-time": "Streaming with sliding window, approximate algorithms",
    "High write load": "Sharding, write-behind cache",
    "High read load": "Read replicas, CDN, caching layer",
}
```

---

## 5. Problem 1: LRU Cache

**Frequency at Amazon:** ★★★★★ | **Difficulty:** Medium

```python
class DLinkedNode:
    __slots__ = ['key', 'val', 'prev', 'next']
    def __init__(self, key=0, val=0):
        self.key = key
        self.val = val
        self.prev = self.next = None

class LRUCache:
    """
    LeetCode 146. LRU Cache.
    
    Amazon loves this problem because:
    1. Tests OOP design skills
    2. Demonstrates understanding of caching (core Amazon infra)
    3. Has a "scale" angle (distributed LRU)
    
    Time: O(1) get and put | Space: O(capacity)
    """
    
    def __init__(self, capacity: int):
        if capacity <= 0:
            raise ValueError("Capacity must be positive")
        self.capacity = capacity
        self.cache = {}
        self.size = 0
        self.head = DLinkedNode()  # Sentinel (MRU side)
        self.tail = DLinkedNode()  # Sentinel (LRU side)
        self.head.next = self.tail
        self.tail.prev = self.head
    
    def _remove(self, node: DLinkedNode):
        node.prev.next = node.next
        node.next.prev = node.prev
    
    def _add_to_front(self, node: DLinkedNode):
        node.prev = self.head
        node.next = self.head.next
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
            if self.size == self.capacity:
                lru = self.tail.prev
                self._remove(lru)
                del self.cache[lru.key]
                self.size -= 1
            node = DLinkedNode(key, value)
            self.cache[key] = node
            self._add_to_front(node)
            self.size += 1


# Amazon Scale Follow-up:
"""
"How would you implement distributed LRU for Amazon's product recommendation cache?"

Key decisions:
1. Partitioning: consistent hashing to distribute keys across nodes
2. Replication: primary-replica for fault tolerance
3. Eviction: each node handles its own LRU locally
4. Invalidation: on product update, broadcast invalidation
5. TTL: time-to-live per entry for staleness control
Technology: Redis (with LRU eviction policy) or Memcached
"""
```

---

## 6. Problem 2: Merge K Sorted Lists

**Frequency at Amazon:** ★★★★★ | **Difficulty:** Hard

```python
import heapq
from typing import Optional, List

class ListNode:
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next
    
    def __lt__(self, other):
        return self.val < other.val

def mergeKLists(lists: List[Optional[ListNode]]) -> Optional[ListNode]:
    """
    LeetCode 23. Merge K Sorted Lists.
    
    Amazon loves this because it's essentially the "merge phase"
    of distributed sorting (merge sort at scale).
    
    Algorithm: Min-heap K-way merge
    - Push first element of each list onto heap
    - Pop minimum, add to result, push that node's next
    
    Time: O(N log K) where N = total nodes, K = number of lists
    Space: O(K) for heap
    
    Why not divide-and-conquer merge?
    D&C: O(N log K) but more complex implementation
    Heap: O(N log K) simpler, same complexity
    """
    if not lists:
        return None
    
    dummy = ListNode(0)
    curr = dummy
    
    # Initialize heap with first node of each non-empty list
    heap = []
    for i, node in enumerate(lists):
        if node:
            heapq.heappush(heap, (node.val, i, node))
    
    while heap:
        val, i, node = heapq.heappop(heap)
        curr.next = node
        curr = curr.next
        if node.next:
            heapq.heappush(heap, (node.next.val, i, node.next))
    
    return dummy.next


def mergeKLists_divideConquer(lists: List[Optional[ListNode]]) -> Optional[ListNode]:
    """
    Divide-and-conquer approach.
    Merge pairs of lists iteratively.
    
    Time: O(N log K) | Space: O(1) additional (in-place merge)
    """
    def mergeTwoLists(l1, l2):
        dummy = ListNode(0)
        curr = dummy
        while l1 and l2:
            if l1.val <= l2.val:
                curr.next = l1; l1 = l1.next
            else:
                curr.next = l2; l2 = l2.next
            curr = curr.next
        curr.next = l1 or l2
        return dummy.next
    
    if not lists:
        return None
    
    # Iteratively merge pairs
    interval = 1
    while interval < len(lists):
        for i in range(0, len(lists) - interval, interval * 2):
            lists[i] = mergeTwoLists(lists[i], lists[i + interval])
        interval *= 2
    
    return lists[0]


# Amazon Scale Follow-up:
"""
"How would you merge K sorted files from S3, each with billions of records?"
Answer:
1. Each file is a sorted run
2. Open file handles to each, maintain min-heap of (current_value, file_idx)
3. Read next record from the winning file's stream
4. Write output in chunks to avoid memory issues
5. Use multiprocessing to parallelize if K is very large
"""
```

---

## 7. Problem 3: Word Ladder

**Frequency at Amazon:** ★★★★☆ | **Difficulty:** Hard

```python
from collections import defaultdict, deque

def ladderLength(beginWord: str, endWord: str, wordList: list[str]) -> int:
    """
    LeetCode 127. Word Ladder.
    
    Find shortest transformation sequence from beginWord to endWord.
    Each step: change exactly one letter, result must be in wordList.
    
    Approach: BFS on implicit word graph.
    
    Optimization: Instead of comparing each word to all others (O(N*L)),
    use generic form: for "hit" → "*it", "h*t", "hi*"
    Group words by generic form for O(1) neighbor lookup.
    
    Time: O(N * L²) where N = |wordList|, L = word length
    Space: O(N * L²)
    """
    if endWord not in wordList:
        return 0
    
    L = len(beginWord)
    
    # Build adjacency list via generic transformations
    combo_dict = defaultdict(list)
    all_words = wordList + [beginWord]
    for word in all_words:
        for i in range(L):
            generic = word[:i] + '*' + word[i+1:]
            combo_dict[generic].append(word)
    
    visited = {beginWord}
    queue = deque([(beginWord, 1)])  # (word, steps)
    
    while queue:
        word, steps = queue.popleft()
        
        for i in range(L):
            generic = word[:i] + '*' + word[i+1:]
            for neighbor in combo_dict[generic]:
                if neighbor == endWord:
                    return steps + 1
                if neighbor not in visited:
                    visited.add(neighbor)
                    queue.append((neighbor, steps + 1))
    
    return 0


def ladderLengthBidirectional(beginWord: str, endWord: str, wordList: list[str]) -> int:
    """
    Bidirectional BFS — much faster in practice.
    
    Expand from both begin and end simultaneously.
    When frontiers meet, total = steps_from_begin + steps_from_end + 1.
    
    Time: O(N * L² / 2) in best case (frontier shrinks faster)
    This can be significantly faster than unidirectional BFS.
    """
    wordSet = set(wordList)
    if endWord not in wordSet:
        return 0
    
    begin_set = {beginWord}
    end_set = {endWord}
    visited = set()
    steps = 0
    L = len(beginWord)
    
    while begin_set and end_set:
        steps += 1
        
        # Always expand the smaller frontier
        if len(begin_set) > len(end_set):
            begin_set, end_set = end_set, begin_set
        
        next_set = set()
        for word in begin_set:
            for i in range(L):
                for c in 'abcdefghijklmnopqrstuvwxyz':
                    new_word = word[:i] + c + word[i+1:]
                    if new_word in end_set:
                        return steps + 1
                    if new_word in wordSet and new_word not in visited:
                        next_set.add(new_word)
                        visited.add(new_word)
        begin_set = next_set
    
    return 0


# Tests
assert ladderLength("hit", "cog", ["hot","dot","dog","lot","log","cog"]) == 5
assert ladderLength("hit", "cog", ["hot","dot","dog","lot","log"]) == 0
```

---

## 8. Problem 4: Sliding Window Maximum

**Frequency at Amazon:** ★★★★☆ | **Difficulty:** Hard

```python
from collections import deque

def maxSlidingWindow(nums: list[int], k: int) -> list[int]:
    """
    LeetCode 239. Sliding Window Maximum.
    
    Monotonic deque: maintains indices of potentially maximum elements.
    - Front: index of current window's maximum
    - Invariant: values at deque indices are DECREASING
    - When we see a larger element: pop smaller ones from back
    - When front is out of window: pop from front
    
    Time: O(N) — each element added and removed from deque at most once
    Space: O(K) — deque never exceeds window size
    
    Amazon loves this: shows understanding of deque optimization
    """
    if not nums or k <= 0:
        return []
    
    dq = deque()  # stores indices
    result = []
    
    for i, x in enumerate(nums):
        # Remove indices outside window
        while dq and dq[0] < i - k + 1:
            dq.popleft()
        
        # Maintain decreasing deque: remove elements smaller than current
        while dq and nums[dq[-1]] < x:
            dq.pop()
        
        dq.append(i)
        
        # Window has k elements: record maximum
        if i >= k - 1:
            result.append(nums[dq[0]])
    
    return result


# Amazon Scale Follow-up:
"""
"How would you compute sliding window maximum for a real-time data stream
 with 10^6 events per second?"

Answer:
1. Use the deque approach — O(1) amortized per event
2. For variable window sizes: persistent deque or sparse table (static windows)
3. For distributed systems: each partition maintains local deque,
   periodic sync for global window crossing partition boundaries
4. With out-of-order events: timestamp-ordered priority queue + late event handling
"""

# Tests
assert maxSlidingWindow([1,3,-1,-3,5,3,6,7], 3) == [3,3,5,5,6,7]
assert maxSlidingWindow([1], 1) == [1]
assert maxSlidingWindow([], 3) == []
assert maxSlidingWindow([1,2,3,4,5], 1) == [1,2,3,4,5]
```

---

## 9. Problem 5: Top K Frequent Elements

**Frequency at Amazon:** ★★★★★ | **Difficulty:** Medium

```python
import heapq
from collections import Counter

def topKFrequent(nums: list[int], k: int) -> list[int]:
    """
    LeetCode 347. Top K Frequent Elements.
    
    Three approaches:
    1. Sort by frequency: O(N log N)
    2. Min-heap of size K: O(N log K) ← Preferred at Amazon
    3. Bucket sort: O(N) — most elegant but harder to explain
    
    Amazon prefers Approach 2 because it scales to streaming data.
    """
    count = Counter(nums)
    
    # Min-heap of size K: O(N log K)
    heap = []
    for num, freq in count.items():
        heapq.heappush(heap, (freq, num))
        if len(heap) > k:
            heapq.heappop(heap)
    
    return [num for freq, num in heap]


def topKFrequent_bucket(nums: list[int], k: int) -> list[int]:
    """
    Bucket sort: O(N)
    Frequency can be at most N, so use N+1 buckets.
    """
    count = Counter(nums)
    buckets = [[] for _ in range(len(nums) + 1)]
    
    for num, freq in count.items():
        buckets[freq].append(num)
    
    result = []
    for freq in range(len(buckets) - 1, 0, -1):
        result.extend(buckets[freq])
        if len(result) >= k:
            return result[:k]
    
    return result[:k]


# Amazon Scale: "What if you have a 1TB log file with IP addresses?"
def top_k_ips_at_scale(log_file_path: str, k: int) -> list[str]:
    """
    For massive files (can't fit in memory):
    1. Hash each IP to one of M shards (files)
    2. Process each shard independently (fits in memory): find top-k per shard
    3. Merge k results from M shards using heap: O(M * k * log(M*k))
    
    Count-Min Sketch alternative: approximate but O(1) space per IP.
    """
    pass  # Pseudocode for the approach


# Tests
assert set(topKFrequent([1,1,1,2,2,3], 2)) == {1, 2}
assert topKFrequent([1], 1) == [1]
```

---

## 10. Problem 6: Find Median from Data Stream

**Frequency at Amazon:** ★★★★☆ | **Difficulty:** Hard

```python
import heapq

class MedianFinder:
    """
    LeetCode 295. Find Median from Data Stream.
    
    Two heaps approach:
    - lo: max-heap (negate values) → stores smaller half
    - hi: min-heap → stores larger half
    - Invariant: len(lo) == len(hi) or len(lo) == len(hi) + 1
    
    Time: addNum O(log N), findMedian O(1)
    Space: O(N)
    
    Amazon loves this for real-time analytics scenarios.
    """
    
    def __init__(self):
        self.lo = []  # max-heap (negate values)
        self.hi = []  # min-heap
    
    def addNum(self, num: int) -> None:
        """Add number maintaining two-heap invariant."""
        # Step 1: Push to lo (always)
        heapq.heappush(self.lo, -num)
        
        # Step 2: Ensure max(lo) <= min(hi)
        if self.hi and -self.lo[0] > self.hi[0]:
            heapq.heappush(self.hi, -heapq.heappop(self.lo))
        
        # Step 3: Balance sizes (lo can have at most 1 extra)
        if len(self.lo) > len(self.hi) + 1:
            heapq.heappush(self.hi, -heapq.heappop(self.lo))
        elif len(self.hi) > len(self.lo):
            heapq.heappush(self.lo, -heapq.heappop(self.hi))
    
    def findMedian(self) -> float:
        if len(self.lo) > len(self.hi):
            return float(-self.lo[0])
        return (-self.lo[0] + self.hi[0]) / 2.0


# Tests
mf = MedianFinder()
mf.addNum(1); mf.addNum(2)
assert mf.findMedian() == 1.5
mf.addNum(3)
assert mf.findMedian() == 2.0


# Amazon Scale Extension:
"""
"How would you compute median salary across Amazon's 1 million employees?"
Options:
1. Sort all salaries O(N log N) — one-time computation
2. Approximate: use percentile approximation with t-digest algorithm
3. Incremental: two-heap approach if salaries stream in
4. Distributed: each region computes their histogram, merge histograms
"""
```

---

## 11. Problem 7: Number of Islands

**Frequency at Amazon:** ★★★★★ | **Difficulty:** Medium

```python
def numIslands(grid: list[list[str]]) -> int:
    """
    LeetCode 200. Number of Islands.
    
    DFS flood-fill approach: mark visited by changing '1' to '0'.
    Time: O(N*M) | Space: O(N*M) worst case recursion
    
    Amazon variation: often asks BFS version for shortest path extension.
    """
    if not grid or not grid[0]:
        return 0
    
    rows, cols = len(grid), len(grid[0])
    count = 0
    
    def dfs(r, c):
        if r < 0 or r >= rows or c < 0 or c >= cols or grid[r][c] != '1':
            return
        grid[r][c] = '0'  # Mark visited
        dfs(r+1, c); dfs(r-1, c)
        dfs(r, c+1); dfs(r, c-1)
    
    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == '1':
                dfs(r, c)
                count += 1
    
    return count


def numIslands_union_find(grid: list[list[str]]) -> int:
    """
    Union-Find approach: better for dynamic grids (add/remove cells).
    
    Time: O(N*M * α(N*M)) ≈ O(N*M) | Space: O(N*M)
    """
    if not grid:
        return 0
    
    rows, cols = len(grid), len(grid[0])
    parent = list(range(rows * cols))
    rank = [0] * (rows * cols)
    count = sum(grid[r][c] == '1' for r in range(rows) for c in range(cols))
    
    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]  # Path compression
            x = parent[x]
        return x
    
    def union(x, y):
        nonlocal count
        rx, ry = find(x), find(y)
        if rx == ry:
            return
        if rank[rx] < rank[ry]:
            rx, ry = ry, rx
        parent[ry] = rx
        if rank[rx] == rank[ry]:
            rank[rx] += 1
        count -= 1
    
    directions = [(0,1),(1,0),(0,-1),(-1,0)]
    for r in range(rows):
        for c in range(cols):
            if grid[r][c] == '1':
                for dr, dc in directions:
                    nr, nc = r + dr, c + dc
                    if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] == '1':
                        union(r * cols + c, nr * cols + nc)
    
    return count


# Tests
grid1 = [["1","1","1","1","0"],["1","1","0","1","0"],
         ["1","1","0","0","0"],["0","0","0","0","0"]]
assert numIslands([row[:] for row in grid1]) == 1

grid2 = [["1","1","0","0","0"],["1","1","0","0","0"],
         ["0","0","1","0","0"],["0","0","0","1","1"]]
assert numIslands([row[:] for row in grid2]) == 3
```

---

## 12. Problem 8: Serialize / Deserialize Binary Tree

**Frequency at Amazon:** ★★★★☆ | **Difficulty:** Hard

```python
from collections import deque

class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

class Codec:
    """
    LeetCode 297. Serialize and Deserialize Binary Tree.
    
    Two common approaches:
    1. BFS (level-order): easy to understand, natural for trees
    2. DFS (preorder): slightly more efficient recursion
    
    Amazon prefers BFS as it mirrors how data might be serialized for
    storage systems (level by level).
    
    Time: O(N) | Space: O(N)
    """
    
    def serialize(self, root: TreeNode) -> str:
        """BFS serialization."""
        if not root:
            return ''
        
        result = []
        queue = deque([root])
        
        while queue:
            node = queue.popleft()
            if node:
                result.append(str(node.val))
                queue.append(node.left)
                queue.append(node.right)
            else:
                result.append('null')
        
        # Remove trailing nulls for efficiency
        while result and result[-1] == 'null':
            result.pop()
        
        return ','.join(result)
    
    def deserialize(self, data: str) -> TreeNode:
        """BFS deserialization."""
        if not data:
            return None
        
        tokens = data.split(',')
        root = TreeNode(int(tokens[0]))
        queue = deque([root])
        i = 1
        
        while queue and i < len(tokens):
            node = queue.popleft()
            
            if i < len(tokens) and tokens[i] != 'null':
                node.left = TreeNode(int(tokens[i]))
                queue.append(node.left)
            i += 1
            
            if i < len(tokens) and tokens[i] != 'null':
                node.right = TreeNode(int(tokens[i]))
                queue.append(node.right)
            i += 1
        
        return root


# DFS approach (preorder):
class CodecDFS:
    def serialize(self, root) -> str:
        if not root:
            return 'null,'
        return f"{root.val}," + self.serialize(root.left) + self.serialize(root.right)
    
    def deserialize(self, data: str) -> TreeNode:
        tokens = iter(data.split(','))
        
        def build():
            val = next(tokens)
            if val == 'null':
                return None
            node = TreeNode(int(val))
            node.left = build()
            node.right = build()
            return node
        
        return build()
```

---

## 13. LP Integration Tips

### Naturally Integrating LPs into Coding Discussions

```python
# LP: Customer Obsession
# During coding: mention who uses this code
"""
"I'm implementing the LRU cache — in production, this would serve 
product recommendation requests. I want to make sure get() is O(1) 
because each user request touches this cache, and at Amazon's scale,
even a microsecond of latency multiplied by millions of requests matters."
"""

# LP: Invent and Simplify
# When choosing an algorithm:
"""
"I could use a complex approach with a segment tree, but I think
the simpler prefix sum approach is more maintainable and still gives
us O(1) queries. Sometimes the simplest correct solution is the best one."
"""

# LP: Insist on Highest Standards
# When testing:
"""
"Let me verify this with a few edge cases — empty input, single element,
and all-same-elements. In a production environment, these are the cases
that cause customer-facing bugs."
"""

# LP: Deliver Results
# When running out of time:
"""
"I notice we have 5 minutes left. Let me ensure I have a working 
brute force solution first, then we can discuss the O(N log N) optimization."
"""

# LP: Dive Deep
# When debugging:
"""
"There's a bug somewhere. Let me trace through the example systematically
rather than guessing. At index 3, the value is X but it should be Y...
The root cause appears to be that I'm not handling the case where [X]."
"""

# LP: Bias for Action
# When unclear requirements:
"""
"I'll make an assumption here — the input could be unsorted. I'll note
this assumption explicitly and verify with you. This avoids us spending
time on a requirement discussion when I can easily handle both cases."
"""
```

### Amazon's Behavioral Interview Format (STAR)

```
Situation: 1-2 sentences setting the scene
Task: What were you responsible for?
Action: What YOU specifically did (not "we")
Result: Quantified outcome

"At [company], our team's recommendation system had P99 latency of 800ms (S).
I was responsible for optimizing the caching layer (T).
I profiled the system, identified that 60% of cache misses were for 
cold-start items, and implemented a predictive pre-warming algorithm (A).
This reduced P99 latency to 120ms and improved conversion rate by 3% (R)."
```

---

*Amazon interviews are unique in their emphasis on real-world scale and Leadership Principles. The best candidates connect their coding decisions to LP values naturally — not as an afterthought. Practice saying "this approach scales better because..." and "in a production environment, I'd also handle..." after every solution.*

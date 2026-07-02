# Google Interview Patterns — Advanced Mastery Guide

> **Level:** Advanced | **Target:** L3–L7 Google SWE roles  
> **Interview Frequency:** ★★★★★ (Core company guide)

---

## Table of Contents
1. [Google's Interview Philosophy](#1-googles-interview-philosophy)
2. [Google's Unique Format](#2-googles-unique-format)
3. [Topics Google Loves](#3-topics-google-loves)
4. [Google's Coding Bar](#4-googles-coding-bar)
5. [Problem 1: Trapping Rain Water](#5-problem-1-trapping-rain-water)
6. [Problem 2: Text Justification](#6-problem-2-text-justification)
7. [Problem 3: Serialize / Deserialize N-ary Tree](#7-problem-3-serialize--deserialize-n-ary-tree)
8. [Problem 4: Count of Smaller Numbers After Self](#8-problem-4-count-of-smaller-numbers-after-self)
9. [Problem 5: Russian Doll Envelopes](#9-problem-5-russian-doll-envelopes)
10. [Problem 6: Word Squares](#10-problem-6-word-squares)
11. [Problem 7: Largest Rectangle in Histogram](#11-problem-7-largest-rectangle-in-histogram)
12. [Problem 8: Minimum Cost to Hire K Workers](#12-problem-8-minimum-cost-to-hire-k-workers)
13. [Google-Specific Interview Tips](#13-google-specific-interview-tips)

---

## 1. Google's Interview Philosophy

Google values **algorithmic excellence** above all. Their bar is the highest in the industry for pure algorithmic thinking. Key tenets:

- **Code quality matters**: Google expects production-quality code in an interview, including error handling, clear naming, and comments for non-obvious logic.
- **Optimal solutions**: Google rarely accepts O(N²) where O(N log N) exists. They WILL ask "can you do better?"
- **Generalization**: They often extend problems — "What if the array is too large to fit in memory?"
- **Mathematical intuition**: Google loves problems with elegant mathematical insights (e.g., exchange argument for greedy proofs).

```
Google's Hiring Bar (roughly):
L3 (new grad):  Solve 2 medium problems cleanly in 45 min
L4 (SWE II):    Solve 1 hard OR 2 mediums + optimization in 45 min
L5 (SWE III):   Solve hard problems, discuss extensions and trade-offs
L6+ (Senior):   Solve hard + design + justify every architectural decision
```

---

## 2. Google's Unique Format

```
Standard Onsite: 5 rounds × 45-60 minutes
  - 2-3 Coding rounds
  - 1 System Design round (L4+)
  - 1 "Googleyness" (behavioral)
  - Sometimes: 1 more coding
  
Phone Screen: 1-2 rounds on Google Docs (no IDE!)
  - Write real code in a text editor
  - Must be syntactically correct Python/Java/C++
  
Hiring Committee:
  - All feedback goes to HC — interviewer doesn't make final call
  - HC looks for "Strong Hire" signals
  - Borderline → HC may ask for additional interview
```

### Google Docs Coding Tips

```python
# In Google Docs/Meet coding:
# 1. Write helper functions first (def helper():...)
# 2. Use consistent indentation (4 spaces)
# 3. Type hints help readability (show quality)
# 4. Write test cases in comments

def twoSum(nums: list[int], target: int) -> list[int]:
    """
    Find indices of two numbers that sum to target.
    Returns empty list if no solution exists.
    Time: O(N) | Space: O(N)
    """
    seen = {}  # value → index
    for i, num in enumerate(nums):
        complement = target - num
        if complement in seen:
            return [seen[complement], i]
        seen[num] = i
    return []

# Test cases (write these!):
# twoSum([2,7,11,15], 9)  → [0,1]
# twoSum([3,2,4], 6)       → [1,2]
# twoSum([3,3], 6)         → [0,1]
# twoSum([], 0)            → []
```

---

## 3. Topics Google Loves

| Topic | Frequency | Typical Difficulty |
|-------|-----------|-------------------|
| Graph algorithms (Dijkstra, BFS variants) | ★★★★★ | Hard |
| String algorithms (KMP, Z-algorithm, Manacher) | ★★★★☆ | Hard |
| Segment tree / BIT with complex operations | ★★★★☆ | Hard |
| Complex DP (interval DP, tree DP, bitmask DP) | ★★★★★ | Hard |
| Monotonic stack / deque | ★★★★☆ | Medium-Hard |
| Greedy with mathematical proof | ★★★★☆ | Hard |
| Network flow (L6+ system design) | ★★★☆☆ | Expert |
| Suffix array / suffix automaton | ★★★☆☆ | Expert |
| Geometry / computational geometry | ★★☆☆☆ | Medium |

---

## 4. Google's Coding Bar

```python
# ─── Google expects these coding habits ───

# 1. ALWAYS use type hints
def solution(grid: list[list[int]], k: int) -> int: ...

# 2. Handle edge cases FIRST with clear guard clauses
def process(nums: list[int]) -> list[int]:
    if not nums:
        return []
    if len(nums) == 1:
        return nums[:]

# 3. Use meaningful names
# NOT: for i, x in enumerate(a):
# YES: for idx, value in enumerate(array):

# 4. Comments for non-obvious logic
# Why, not what:
# NOT: # increment left pointer
# YES: # skip duplicates to avoid redundant triplets

# 5. Helper functions for complex sub-problems
def largestRectangleInHistogram(heights: list[int]) -> int:
    return _calculate_max_area(heights)

def _calculate_max_area(heights: list[int]) -> int:
    # Implementation here
    pass

# 6. Complexity comment after function
"""Time: O(N log N) | Space: O(N)"""
```

---

## 5. Problem 1: Trapping Rain Water

**Frequency at Google:** ★★★★★ | **Difficulty:** Medium-Hard

```python
def trap(height: list[int]) -> int:
    """
    LeetCode 42. Trapping Rain Water.
    
    Water at position i = min(max_left[i], max_right[i]) - height[i]
    
    Three approaches in increasing sophistication:
    1. Prefix/Suffix max arrays — O(N) time, O(N) space
    2. Two pointers — O(N) time, O(1) space  ← Google prefers this
    3. Monotonic stack — O(N) time, O(N) space
    
    The two-pointer approach is the expected optimal at Google.
    """
    if not height:
        return 0
    
    left, right = 0, len(height) - 1
    left_max = right_max = 0
    water = 0
    
    while left < right:
        if height[left] < height[right]:
            if height[left] >= left_max:
                left_max = height[left]
            else:
                water += left_max - height[left]
            left += 1
        else:
            if height[right] >= right_max:
                right_max = height[right]
            else:
                water += right_max - height[right]
            right -= 1
    
    return water


def trap_stack(height: list[int]) -> int:
    """
    Monotonic stack approach — computes horizontal layers.
    
    Stack stores indices of decreasing heights.
    When we find a bar taller than stack top, we have a valley.
    
    Time: O(N) | Space: O(N)
    """
    stack = []
    water = 0
    
    for i, h in enumerate(height):
        while stack and height[stack[-1]] < h:
            bottom_idx = stack.pop()
            if not stack:
                break
            left_idx = stack[-1]
            
            width = i - left_idx - 1
            bounded_height = min(height[left_idx], h) - height[bottom_idx]
            water += width * bounded_height
        
        stack.append(i)
    
    return water


# Tests
assert trap([0,1,0,2,1,0,1,3,2,1,2,1]) == 6
assert trap([4,2,0,3,2,5]) == 9
assert trap([]) == 0
assert trap([1]) == 0
assert trap([1,0,1]) == 1

# Time: O(N) | Space: O(1) for two-pointer
```

**Google Follow-ups:**
1. "What if heights are floating point?" → Same algorithm works
2. "What if this is a 2D elevation map?" → LeetCode 407, BFS with min-heap
3. "What's the max water in a 3D grid?" → Priority queue (Dijkstra variant)

---

## 6. Problem 2: Text Justification

**Frequency at Google:** ★★★★★ | **Difficulty:** Hard

```python
def fullJustify(words: list[str], maxWidth: int) -> list[str]:
    """
    LeetCode 68. Text Justification.
    
    This problem tests:
    - Simulation / greedy line packing
    - String manipulation
    - Edge case handling (last line, single word line)
    
    Algorithm:
    1. Greedily pack as many words per line as possible
    2. For each line (except last): distribute spaces evenly
       - Extra spaces go to leftmost gaps
    3. Last line: left-justified (single space between words, pad right)
    
    Time: O(total_chars) | Space: O(total_chars)
    """
    result = []
    i = 0
    n = len(words)
    
    while i < n:
        # ─── Step 1: Determine words on this line ───
        line_len = len(words[i])
        j = i + 1
        while j < n and line_len + 1 + len(words[j]) <= maxWidth:
            line_len += 1 + len(words[j])
            j += 1
        # Words [i..j-1] go on this line
        
        # ─── Step 2: Build the line ───
        num_words = j - i
        num_gaps = num_words - 1
        is_last_line = (j == n)
        
        if is_last_line or num_words == 1:
            # Left justify: single space between words, pad right
            line = ' '.join(words[i:j])
            line += ' ' * (maxWidth - len(line))
        else:
            # Full justify: distribute spaces evenly
            total_spaces = maxWidth - sum(len(words[k]) for k in range(i, j))
            space_per_gap = total_spaces // num_gaps
            extra_spaces = total_spaces % num_gaps
            
            line = ''
            for k in range(i, j - 1):
                line += words[k]
                line += ' ' * space_per_gap
                if k - i < extra_spaces:
                    line += ' '  # Extra space for leftmost gaps
            line += words[j - 1]
        
        result.append(line)
        i = j
    
    return result


# Tests
words1 = ["This", "is", "an", "example", "of", "text", "justification."]
result1 = fullJustify(words1, 16)
print('\n'.join(result1))
# "This    is    an"
# "example  of text"
# "justification.  "

words2 = ["What","must","be","acknowledgment","shall","be"]
result2 = fullJustify(words2, 16)
# "What   must   be"
# "acknowledgment  "
# "shall be        "
```

---

## 7. Problem 3: Serialize / Deserialize N-ary Tree

**Frequency at Google:** ★★★★☆ | **Difficulty:** Hard

```python
from collections import deque

class NaryNode:
    def __init__(self, val=None, children=None):
        self.val = val
        self.children = children or []

class NarySerializer:
    """
    Serialize/Deserialize N-ary tree.
    
    Format: BFS level-order with sentinel 'null' for children end
    Example tree: 1 → [3,2,4], 3 → [5,6]
    Serialized: "1 3 2 4 null 5 6 null null null null"
    
    Alternative format: "1 [3,2,4] 3 [5,6] ..."
    
    Time: O(N) | Space: O(N)
    """
    
    SEPARATOR = ' '
    SENTINEL = 'None'
    
    def serialize(self, root: NaryNode) -> str:
        if not root:
            return ''
        
        result = []
        queue = deque([root])
        
        while queue:
            node = queue.popleft()
            result.append(str(node.val))
            result.append(str(len(node.children)))  # Encode child count!
            for child in node.children:
                queue.append(child)
        
        return self.SEPARATOR.join(result)
    
    def deserialize(self, data: str) -> NaryNode:
        if not data:
            return None
        
        tokens = iter(data.split(self.SEPARATOR))
        
        root_val = int(next(tokens))
        root = NaryNode(root_val)
        root_child_count = int(next(tokens))
        
        queue = deque([(root, root_child_count)])
        
        while queue:
            node, child_count = queue.popleft()
            for _ in range(child_count):
                child_val = int(next(tokens))
                child_cc = int(next(tokens))
                child = NaryNode(child_val)
                node.children.append(child)
                if child_cc > 0:
                    queue.append((child, child_cc))
        
        return root


# Alternative: DFS preorder with child count
class NarySerializerDFS:
    def serialize(self, root: NaryNode) -> str:
        if not root:
            return ''
        parts = []
        def dfs(node):
            parts.append(str(node.val))
            parts.append(str(len(node.children)))
            for child in node.children:
                dfs(child)
        dfs(root)
        return ' '.join(parts)
    
    def deserialize(self, data: str) -> NaryNode:
        if not data:
            return None
        tokens = iter(data.split())
        def build():
            val = int(next(tokens))
            count = int(next(tokens))
            node = NaryNode(val)
            node.children = [build() for _ in range(count)]
            return node
        return build()
```

---

## 8. Problem 4: Count of Smaller Numbers After Self

**Frequency at Google:** ★★★★☆ | **Difficulty:** Hard

```python
def countSmaller(nums: list[int]) -> list[int]:
    """
    LeetCode 315. Count of Smaller Numbers After Self.
    
    Three approaches:
    1. Modified Merge Sort — O(N log N) time, O(N) space  ← Elegant
    2. BIT with coordinate compression — O(N log N)
    3. Persistent Segment Tree — O(N log N)
    
    Modified Merge Sort: during merge, count how many elements
    from right half are placed before each element from left half.
    """
    n = len(nums)
    counts = [0] * n
    indexed = list(enumerate(nums))  # (original_index, value)
    
    def merge_sort(arr):
        if len(arr) <= 1:
            return arr
        mid = len(arr) // 2
        left = merge_sort(arr[:mid])
        right = merge_sort(arr[mid:])
        return merge(left, right)
    
    def merge(left, right):
        result = []
        right_smaller = 0  # Count of right elements placed so far
        li, ri = 0, 0
        
        while li < len(left) and ri < len(right):
            if left[li][1] > right[ri][1]:
                # right[ri] goes before left[li]
                right_smaller += 1
                result.append(right[ri])
                ri += 1
            else:
                # left[li] goes before all remaining right elements placed
                counts[left[li][0]] += right_smaller
                result.append(left[li])
                li += 1
        
        while li < len(left):
            counts[left[li][0]] += right_smaller
            result.append(left[li])
            li += 1
        
        result.extend(right[ri:])
        return result
    
    merge_sort(indexed)
    return counts


def countSmaller_BIT(nums: list[int]) -> list[int]:
    """
    BIT (Binary Indexed Tree) approach.
    Process from right to left.
    For each number, query BIT for count of smaller numbers already inserted.
    
    Time: O(N log N) | Space: O(N)
    """
    # Coordinate compression
    sorted_unique = sorted(set(nums))
    rank = {v: i + 1 for i, v in enumerate(sorted_unique)}  # 1-indexed
    M = len(sorted_unique)
    
    bit = [0] * (M + 1)
    
    def update(i, delta=1):
        while i <= M:
            bit[i] += delta
            i += i & (-i)
    
    def query(i):
        s = 0
        while i > 0:
            s += bit[i]
            i -= i & (-i)
        return s
    
    result = []
    for num in reversed(nums):
        r = rank[num]
        result.append(query(r - 1))  # Count elements with rank < r
        update(r)
    
    return result[::-1]


# Tests
assert countSmaller([5,2,6,1]) == [2,1,1,0]
assert countSmaller([1]) == [0]
assert countSmaller([-1,-1]) == [0,0]
```

---

## 9. Problem 5: Russian Doll Envelopes

**Frequency at Google:** ★★★★☆ | **Difficulty:** Hard

```python
def maxEnvelopes(envelopes: list[list[int]]) -> int:
    """
    LeetCode 354. Russian Doll Envelopes.
    
    Key insight: Sort by width ascending, then by HEIGHT DESCENDING.
    The descending height sort prevents using multiple envelopes of same width.
    
    Then: Find LIS of heights only → that's the answer.
    
    Why descending height for same width?
    If we sort by (w, h) both ascending: [1,2],[1,3],[2,4]
    LIS would pick [1,2],[1,3] — wrong (can't put 1-wide inside 1-wide)
    If we sort by (w, h_desc): [1,3],[1,2],[2,4]
    LIS of heights [3,2,4]: picks [2,4] — correct!
    
    Time: O(N log N) | Space: O(N)
    """
    envelopes.sort(key=lambda e: (e[0], -e[1]))  # Sort w asc, h desc
    
    # LIS using patience sorting (O(N log N))
    from bisect import bisect_left
    
    tails = []  # tails[i] = smallest tail of IS of length i+1
    
    for _, h in envelopes:
        pos = bisect_left(tails, h)
        if pos == len(tails):
            tails.append(h)
        else:
            tails[pos] = h
    
    return len(tails)


# Why this works:
# Patience sorting: maintain 'tails' array where tails[i] is the minimum
# possible tail element of all increasing subsequences of length i+1.
# Binary search gives O(log N) per element.

# Tests
assert maxEnvelopes([[5,4],[6,4],[6,7],[2,3]]) == 3  # [2,3]→[5,4]→[6,7]
assert maxEnvelopes([[1,1],[1,1],[1,1]]) == 1
assert maxEnvelopes([[1,2]]) == 1
```

---

## 10. Problem 6: Word Squares

**Frequency at Google:** ★★★☆☆ | **Difficulty:** Hard

```python
def wordSquares(words: list[str]) -> list[list[str]]:
    """
    LeetCode 425. Word Squares.
    
    A word square: matrix where rows[i] == cols[i]
    Build square row by row using Trie for prefix lookups.
    
    Key insight: prefix of next word is determined by current square's columns.
    
    Algorithm:
    1. Build Trie of all words (with list of words at each node)
    2. Backtrack: at each row, find all words sharing prefix from previous rows' columns
    
    Time: O(N * L * 26^L) where L = word length (with pruning, much faster)
    Space: O(N * L) for Trie
    """
    L = len(words[0]) if words else 0
    
    # Build Trie: maps prefix → list of words with that prefix
    from collections import defaultdict
    
    prefix_map = defaultdict(list)
    for word in words:
        for i in range(L + 1):
            prefix_map[word[:i]].append(word)
    
    def backtrack(square: list[str], result: list[list[str]]):
        row = len(square)
        if row == L:
            result.append(square[:])
            return
        
        # Required prefix for next word: columns of current partial square
        prefix = ''.join(square[i][row] for i in range(row))
        
        for word in prefix_map.get(prefix, []):
            square.append(word)
            backtrack(square, result)
            square.pop()
    
    result = []
    for word in words:
        backtrack([word], result)
    
    return result


# Test
words = ["ball","area","lead","lady"]
squares = wordSquares(words)
# Expected: [["ball","area","lead","lady"],["lady","area","deny","last"]]
# Note: depends on which words can form valid squares
```

---

## 11. Problem 7: Largest Rectangle in Histogram

**Frequency at Google:** ★★★★★ | **Difficulty:** Hard

```python
def largestRectangleArea(heights: list[int]) -> int:
    """
    LeetCode 84. Largest Rectangle in Histogram.
    
    Monotonic stack approach:
    - Maintain stack of indices with INCREASING heights
    - When current bar is shorter than stack top: the stack top is the bottleneck
    - Width = (current_idx - left_idx - 1) where left_idx is new stack top
    
    Sentinel: append 0 at end to force processing all remaining bars.
    
    Time: O(N) | Space: O(N)
    
    This is one of Google's absolute favorite hard problems.
    """
    heights = heights + [0]  # Sentinel to empty stack at end
    stack = [-1]  # Start with -1 sentinel for width calculation
    max_area = 0
    
    for i, h in enumerate(heights):
        while stack[-1] != -1 and heights[stack[-1]] >= h:
            height = heights[stack.pop()]
            width = i - stack[-1] - 1
            max_area = max(max_area, height * width)
        stack.append(i)
    
    return max_area


def maximalRectangle(matrix: list[list[str]]) -> int:
    """
    LeetCode 85. Maximal Rectangle in Binary Matrix.
    
    Extension of histogram problem:
    - For each row, compute histogram heights
    - Apply largestRectangleArea on each row's histogram
    
    Time: O(N*M) | Space: O(M)
    """
    if not matrix or not matrix[0]:
        return 0
    
    n, m = len(matrix), len(matrix[0])
    heights = [0] * m
    max_area = 0
    
    for row in matrix:
        for j in range(m):
            heights[j] = heights[j] + 1 if row[j] == '1' else 0
        max_area = max(max_area, largestRectangleArea(heights))
    
    return max_area


# Tests
assert largestRectangleArea([2,1,5,6,2,3]) == 10
assert largestRectangleArea([2,4]) == 4
assert largestRectangleArea([1]) == 1
assert largestRectangleArea([]) == 0
```

---

## 12. Problem 8: Minimum Cost to Hire K Workers

**Frequency at Google:** ★★★★☆ | **Difficulty:** Hard

```python
import heapq

def mincostToHireWorkers(quality: list[int], wage: list[int], k: int) -> float:
    """
    LeetCode 857. Minimum Cost to Hire K Workers.
    
    Key insight: 
    In any valid group, there's a "captain" who receives exactly their minimum wage.
    All others receive: (their_quality / captain_quality) * captain_wage
    
    Total cost = (total_quality_of_group) * (captain_wage / captain_quality)
    
    This is a RATIO problem. If we define ratio = wage[i] / quality[i],
    the captain must have the MAXIMUM ratio in the group.
    
    Algorithm:
    1. Sort workers by ratio (wage/quality)
    2. For each worker as potential "captain" (in increasing ratio order):
       - All previously considered workers have ratio <= captain
       - Choose k-1 workers with MINIMUM quality (reduces cost)
       - Use max-heap to maintain k minimum-quality workers
    
    Time: O(N log N + N log K) | Space: O(N)
    """
    n = len(quality)
    workers = sorted(zip(
        [w/q for w, q in zip(wage, quality)],  # ratio
        quality
    ))
    
    max_heap = []  # max-heap of qualities (negated for Python's min-heap)
    quality_sum = 0
    min_cost = float('inf')
    
    for ratio, q in workers:
        # Add current worker to pool
        heapq.heappush(max_heap, -q)
        quality_sum += q
        
        # If pool exceeds k, remove highest quality worker
        if len(max_heap) > k:
            quality_sum += heapq.heappop(max_heap)  # Add back (was negated)
        
        # If we have exactly k workers, compute cost
        if len(max_heap) == k:
            # Current worker is the "captain" (highest ratio in group)
            min_cost = min(min_cost, quality_sum * ratio)
    
    return min_cost


# Tests
assert mincostToHireWorkers([10,20,5], [70,50,30], 2) == 105.0
assert mincostToHireWorkers([3,1,10,10,1], [4,8,2,2,7], 3) == 30.666666666666664
```

---

## 13. Google-Specific Interview Tips

### The "Google Clean Code" Standards

```python
# Google Style Guide for Python (simplified):
# 1. Maximum line length: 80 characters
# 2. Use Google docstrings format
# 3. All public functions need docstrings
# 4. Type hints on all function signatures
# 5. No bare `except` — always specify exception type

# Google docstring format:
def binary_search(arr: list[int], target: int) -> int:
    """Searches for target in sorted array.
    
    Args:
        arr: Sorted list of integers.
        target: Value to search for.
    
    Returns:
        Index of target if found, -1 otherwise.
    
    Raises:
        ValueError: If arr is None.
    
    Time Complexity:
        O(log N) where N = len(arr).
    """
    if arr is None:
        raise ValueError("Array cannot be None")
    if not arr:
        return -1
    lo, hi = 0, len(arr) - 1
    while lo <= hi:
        mid = lo + (hi - lo) // 2
        if arr[mid] == target:
            return mid
        elif arr[mid] < target:
            lo = mid + 1
        else:
            hi = mid - 1
    return -1
```

### Google's Favorite Problem Patterns

```
1. Monotonic Stack problems (histogram, next greater, largest rectangle)
2. K-way merge / top-K with heap
3. Two-pointer with sort pre-processing  
4. DP with optimization (divide-and-conquer optimization, convex hull trick)
5. Graph problems with non-trivial state (BFS with visited as tuple)
6. String matching / manipulation (multi-pass, careful indexing)
7. Greedy with mathematical exchange argument proof
8. Tree DP (diameter, max path, subtree problems)
```

### What Interviewers Note

```python
# ✅ POSITIVE SIGNALS:
positive = [
    "Asks about edge cases before they matter",
    "Names variables like a professional engineer",
    "Writes modular code with helper functions",
    "Discusses trade-offs proactively (time vs space)",
    "Tests with multiple cases including edge cases",
    "Catches and fixes own bugs during review",
    "Asks good follow-up questions",
    "Mentions production concerns (overflow, concurrency, scale)",
]

# ❌ NEGATIVE SIGNALS:
negative = [
    "Codes first, thinks later",
    "Uses single-letter variable names throughout",
    "Doesn't know complexity of their own solution",
    "Submits without testing",
    "Panics visibly when given a hint",
    "Claims solution is correct without verifying",
    "Doesn't handle obvious edge cases",
    "Uses language features they can't explain",
]
```

### Google Behavioral Component ("Googleyness")

```
Values Google looks for:
- Intellectual humility ("I don't know, but here's how I'd find out")
- Comfort with ambiguity (multiple valid approaches)
- Collaborative problem-solving (incorporating feedback)
- Bias for action (start coding even without perfect plan)
- User/customer focus (who uses this code? impact?)

Sample behavioral topics:
- "Tell me about a time you disagreed with a technical decision"
- "Describe a project where you had to learn quickly"
- "How do you handle ambiguous requirements?"
- "Tell me about the most complex system you've designed"
```

---

*Google rewards mastery — not just knowledge of algorithms, but deep understanding of WHY they work, HOW to extend them, and WHEN to apply them. Practice until you can code largestRectangleArea and Trapping Rain Water without looking up syntax. These two alone appear in roughly 30% of Google coding interviews.*

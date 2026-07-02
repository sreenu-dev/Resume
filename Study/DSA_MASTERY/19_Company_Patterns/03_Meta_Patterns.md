# Meta (Facebook) Interview Patterns — Advanced Mastery Guide

> **Level:** Advanced | **Target:** E3–E6 Software Engineer roles  
> **Interview Frequency:** ★★★★★ (Core company guide)

---

## Table of Contents
1. [Meta's Interview Philosophy](#1-metas-interview-philosophy)
2. [Meta's Format — 2 Coding Rounds](#2-metas-format--2-coding-rounds)
3. [Meta's Favorite Topics](#3-metas-favorite-topics)
4. [The 2-Problem Expectation](#4-the-2-problem-expectation)
5. [Problem 1: Binary Tree Paths](#5-problem-1-binary-tree-paths)
6. [Problem 2: Clone Graph](#6-problem-2-clone-graph)
7. [Problem 3: Word Search](#7-problem-3-word-search)
8. [Problem 4: Subsets and Permutations](#8-problem-4-subsets-and-permutations)
9. [Problem 5: Merge Intervals](#9-problem-5-merge-intervals)
10. [Problem 6: Decode Ways](#10-problem-6-decode-ways)
11. [Problem 7: Word Break](#11-problem-7-word-break)
12. [Problem 8: Diameter of Binary Tree](#12-problem-8-diameter-of-binary-tree)
13. [Meta Follow-Up Question Patterns](#13-meta-follow-up-question-patterns)
14. [Meta-Specific Interview Tips](#14-meta-specific-interview-tips)

---

## 1. Meta's Interview Philosophy

Meta's coding bar is focused on **clean, practical code** that solves real problems. Key characteristics:

- **Clean code over clever code**: Meta wants readable, maintainable code — not the most clever one-liner.
- **Real-world problems**: Questions often have a practical angle (social graph, news feed, content ranking).
- **2 complete solutions expected**: Meta's 35-minute coding rounds expect you to solve two problems fully.
- **Follow-up heavy**: Every solution gets extended — "What if N is 10^6?", "What if the tree is N-ary?"

```
Meta's Hiring Bar:
E3 (Entry-level): 2 mediums in 35 min, basic Big-O
E4 (Mid-level):   1 medium + 1 hard, or 2 mediums + deep optimization
E5 (Senior):      2 mediums quickly, deep follow-ups, system design signal
E6 (Staff):       Hard problems, extensions, lead-the-interview energy
```

---

## 2. Meta's Format — 2 Coding Rounds

```
Typical Full Loop:
  Technical Screen: 1 round (45 min, 1-2 problems in online IDE)
  Onsite (3-5 rounds):
    - 2 Coding rounds (35 min each, often 2 problems per round)
    - 1 System Design round
    - 1 Behavioral round ("Meta values alignment")
    - Sometimes: 1 more technical deep-dive

Key Meta differences:
1. 35 minutes for CODING (not full 45) → faster pace required
2. Interviewers expect 2 problems solved → don't over-polish #1
3. Strong preference for trees and graphs over advanced DS
4. Clean Python preferred; avoid complex one-liners
5. Testing: expected but brief; focus on correctness
```

### Meta's Coding Environment

```python
# Meta uses CoderPad (real IDE with autocomplete)
# No need to worry about missing parentheses — but don't rely on it
# Run button available — USE IT after you think you're done

# Meta coding best practices:
def maxDepth(root) -> int:
    """
    Clean implementation showing Meta's preferred style:
    - Clear function name and return type
    - Handles None first
    - Concise logic
    - No over-engineering
    """
    if not root:
        return 0
    return 1 + max(maxDepth(root.left), maxDepth(root.right))

# NOT preferred (overengineered for this problem):
def maxDepth_verbose(root) -> int:
    stack = [(root, 1)]
    max_depth = 0
    while stack:
        node, depth = stack.pop()
        if node:
            max_depth = max(max_depth, depth)
            stack.append((node.left, depth + 1))
            stack.append((node.right, depth + 1))
    return max_depth
```

---

## 3. Meta's Favorite Topics

| Topic | Frequency | Notes |
|-------|-----------|-------|
| Binary Tree (all operations) | ★★★★★ | #1 most common |
| Graph (BFS/DFS) | ★★★★★ | Social network modeling |
| Backtracking | ★★★★☆ | Permutations, combinations, word search |
| Dynamic Programming | ★★★★☆ | Usually medium DP |
| String manipulation | ★★★★☆ | Anagrams, decoding, parsing |
| Arrays + sorting | ★★★☆☆ | Often combined with DP |
| Linked lists | ★★★☆☆ | Merge, reverse, cycle |
| Intervals | ★★★☆☆ | Meeting rooms, merge intervals |
| Trie | ★★★☆☆ | Word search, autocomplete |

---

## 4. The 2-Problem Expectation

```
Strategy for 35-minute coding rounds at Meta:

Problem 1: Target 15 minutes
  - Clarify: 2 min
  - Approach: 2 min (quick)
  - Code: 9 min
  - Test: 2 min

Problem 2: Target 15 minutes  
  - Clarify: 1 min
  - Approach: 2 min
  - Code: 10 min
  - Test + wrap: 2 min

Reserve: 5 minutes for follow-ups and discussion

CRITICAL: Don't spend 30 minutes perfecting Problem 1.
A working Problem 1 + working Problem 2 beats a perfect Problem 1 alone.

Pacing check at minute 12-13: "Should I be wrapping up Problem 1?"
If not done, state: "Let me handle the edge cases and move to Problem 2."
```

---

## 5. Problem 1: Binary Tree Paths

**Frequency at Meta:** ★★★★★ | **Difficulty:** Easy-Medium

```python
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def binaryTreePaths(root: TreeNode) -> list[str]:
    """
    LeetCode 257. Binary Tree Paths.
    
    Find all root-to-leaf paths.
    DFS with path tracking.
    
    Time: O(N * H) where H = height (path string building)
    Space: O(H) recursion + O(L * H) for L paths
    """
    if not root:
        return []
    
    result = []
    
    def dfs(node, path):
        if not node.left and not node.right:  # Leaf
            result.append(path + str(node.val))
            return
        
        curr = path + str(node.val) + '->'
        if node.left:
            dfs(node.left, curr)
        if node.right:
            dfs(node.right, curr)
    
    dfs(root, '')
    return result


def pathSum(root: TreeNode, targetSum: int) -> list[list[int]]:
    """
    LeetCode 113. Path Sum II — Extension of above.
    Find all root-to-leaf paths summing to targetSum.
    
    Time: O(N * H) | Space: O(H)
    """
    result = []
    
    def dfs(node, remaining, path):
        if not node:
            return
        path.append(node.val)
        if not node.left and not node.right and remaining == node.val:
            result.append(path[:])  # Copy path!
        else:
            dfs(node.left, remaining - node.val, path)
            dfs(node.right, remaining - node.val, path)
        path.pop()  # Backtrack
    
    dfs(root, targetSum, [])
    return result


def maxPathSum(root: TreeNode) -> int:
    """
    LeetCode 124. Binary Tree Maximum Path Sum — Hard extension.
    Path can start and end anywhere (not just root-to-leaf).
    
    Key insight: At each node, compute max "gain" going through this node.
    A node can either:
    1. Be endpoint of path (contribute to parent's gain)
    2. Be the "bent" node where path goes down both sides
    
    Time: O(N) | Space: O(H)
    """
    max_sum = [float('-inf')]
    
    def dfs(node) -> int:
        """Returns max gain from this node going in ONE direction."""
        if not node:
            return 0
        
        left_gain = max(0, dfs(node.left))   # Ignore negative gains
        right_gain = max(0, dfs(node.right))
        
        # This node as "bent" point (considering both sides)
        path_through = node.val + left_gain + right_gain
        max_sum[0] = max(max_sum[0], path_through)
        
        # Return single-direction gain (for parent)
        return node.val + max(left_gain, right_gain)
    
    dfs(root)
    return max_sum[0]


# Tests
root = TreeNode(1, TreeNode(2), TreeNode(3))
assert set(binaryTreePaths(root)) == {"1->2", "1->3"}
assert maxPathSum(TreeNode(-3)) == -3
```

---

## 6. Problem 2: Clone Graph

**Frequency at Meta:** ★★★★★ | **Difficulty:** Medium

```python
class Node:
    def __init__(self, val=0, neighbors=None):
        self.val = val
        self.neighbors = neighbors or []

def cloneGraph(node: Node) -> Node:
    """
    LeetCode 133. Clone Graph.
    
    Meta loves this: social graphs, reference cloning, deep copy.
    
    DFS with visited map (old node → new node).
    
    Time: O(V + E) | Space: O(V)
    """
    if not node:
        return None
    
    visited = {}  # old_node → new_node
    
    def dfs(n: Node) -> Node:
        if n in visited:
            return visited[n]
        
        clone = Node(n.val)
        visited[n] = clone  # Register BEFORE recursing (handles cycles!)
        
        for neighbor in n.neighbors:
            clone.neighbors.append(dfs(neighbor))
        
        return clone
    
    return dfs(node)


def cloneGraph_bfs(node: Node) -> Node:
    """
    BFS approach — iterative, avoids recursion limit.
    Time: O(V + E) | Space: O(V)
    """
    if not node:
        return None
    
    from collections import deque
    
    visited = {node: Node(node.val)}
    queue = deque([node])
    
    while queue:
        n = queue.popleft()
        for neighbor in n.neighbors:
            if neighbor not in visited:
                visited[neighbor] = Node(neighbor.val)
                queue.append(neighbor)
            visited[n].neighbors.append(visited[neighbor])
    
    return visited[node]


# Meta Follow-up: "How would you deep-copy a tree with arbitrary pointers?"
def deepCopyWithRandomPointers(root):
    """
    Extension: nodes have random pointer in addition to left/right.
    Weave new nodes into existing tree, then separate.
    (Similar to LeetCode 138 for linked lists)
    """
    pass  # Approach: interleave → set random → deinterleave
```

---

## 7. Problem 3: Word Search

**Frequency at Meta:** ★★★★☆ | **Difficulty:** Medium

```python
def exist(board: list[list[str]], word: str) -> bool:
    """
    LeetCode 79. Word Search.
    
    DFS + Backtracking on grid.
    
    Optimization: check character match before recursing.
    Mark visited in-place (then restore) to avoid visited set.
    
    Time: O(N * M * 4^L) where L = word length (with pruning, much better)
    Space: O(L) recursion depth
    """
    rows, cols = len(board), len(board[0])
    
    def dfs(r, c, idx):
        if idx == len(word):
            return True  # Found all characters!
        if r < 0 or r >= rows or c < 0 or c >= cols:
            return False
        if board[r][c] != word[idx]:
            return False
        
        # Mark as visited
        temp = board[r][c]
        board[r][c] = '#'
        
        # Explore all 4 directions
        found = (dfs(r+1, c, idx+1) or dfs(r-1, c, idx+1) or
                 dfs(r, c+1, idx+1) or dfs(r, c-1, idx+1))
        
        # Restore
        board[r][c] = temp
        
        return found
    
    for r in range(rows):
        for c in range(cols):
            if dfs(r, c, 0):
                return True
    
    return False


def findWords(board: list[list[str]], words: list[str]) -> list[str]:
    """
    LeetCode 212. Word Search II — Find all words from list.
    
    Instead of searching for each word separately, build a Trie
    and DFS once, matching all words simultaneously.
    
    Time: O(N*M*4^L + W*L) where W=|words|, L=max word length
    Space: O(W*L) for Trie
    """
    # Build Trie
    WORD_KEY = '$'
    trie = {}
    for word in words:
        node = trie
        for char in word:
            node = node.setdefault(char, {})
        node[WORD_KEY] = word  # Mark end with the word itself
    
    rows, cols = len(board), len(board[0])
    result = []
    
    def dfs(r, c, parent_node):
        char = board[r][c]
        curr_node = parent_node.get(char)
        if not curr_node:
            return
        
        if WORD_KEY in curr_node:
            result.append(curr_node.pop(WORD_KEY))  # Found! Remove to avoid duplicates
        
        board[r][c] = '#'  # Mark visited
        for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
            nr, nc = r + dr, c + dc
            if 0 <= nr < rows and 0 <= nc < cols and board[nr][nc] != '#':
                dfs(nr, nc, curr_node)
        board[r][c] = char  # Restore
        
        # Prune: if this trie node is empty, remove it
        if not curr_node:
            parent_node.pop(char)
    
    for r in range(rows):
        for c in range(cols):
            dfs(r, c, trie)
    
    return result


# Tests
board1 = [["A","B","C","E"],["S","F","C","S"],["A","D","E","E"]]
assert exist(board1, "ABCCED") == True
assert exist(board1, "SEE") == True
assert exist(board1, "ABCB") == False
```

---

## 8. Problem 4: Subsets and Permutations

**Frequency at Meta:** ★★★★★ | **Difficulty:** Medium

```python
def subsets(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 78. Subsets.
    Generate all 2^N subsets.
    
    Two approaches:
    1. Backtracking (recursive)
    2. Bit manipulation — each number 0..2^N-1 represents a subset
    
    Time: O(N * 2^N) | Space: O(N * 2^N)
    """
    result = [[]]
    
    for num in nums:
        # For each existing subset, create a new one by adding num
        result += [curr + [num] for curr in result]
    
    return result


def subsetsWithDup(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 90. Subsets II — with duplicates.
    Sort first, skip duplicates at same level.
    
    Time: O(N * 2^N) | Space: O(N * 2^N)
    """
    nums.sort()
    result = []
    
    def backtrack(start, curr):
        result.append(curr[:])
        for i in range(start, len(nums)):
            if i > start and nums[i] == nums[i-1]:
                continue  # Skip duplicate at same recursion level
            curr.append(nums[i])
            backtrack(i + 1, curr)
            curr.pop()
    
    backtrack(0, [])
    return result


def permute(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 46. Permutations.
    All N! permutations of distinct numbers.
    
    Swap-based approach: no extra space for "used" tracking.
    Time: O(N * N!) | Space: O(N)
    """
    result = []
    
    def backtrack(start):
        if start == len(nums):
            result.append(nums[:])
            return
        for i in range(start, len(nums)):
            nums[start], nums[i] = nums[i], nums[start]
            backtrack(start + 1)
            nums[start], nums[i] = nums[i], nums[start]  # Restore
    
    backtrack(0)
    return result


def permuteUnique(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 47. Permutations II — with duplicates.
    Sort + skip same element at same position.
    
    Time: O(N * N!) | Space: O(N)
    """
    nums.sort()
    result = []
    used = [False] * len(nums)
    
    def backtrack(curr):
        if len(curr) == len(nums):
            result.append(curr[:])
            return
        for i in range(len(nums)):
            if used[i]:
                continue
            if i > 0 and nums[i] == nums[i-1] and not used[i-1]:
                continue  # Skip duplicate at same level
            used[i] = True
            curr.append(nums[i])
            backtrack(curr)
            curr.pop()
            used[i] = False
    
    backtrack([])
    return result


# Tests
assert len(subsets([1,2,3])) == 8
assert len(permute([1,2,3])) == 6
print(sorted([sorted(p) for p in permuteUnique([1,1,2])]))
# [[1,1,2], [1,2,1], [2,1,1]]
```

---

## 9. Problem 5: Merge Intervals

**Frequency at Meta:** ★★★★★ | **Difficulty:** Medium

```python
def merge(intervals: list[list[int]]) -> list[list[int]]:
    """
    LeetCode 56. Merge Intervals.
    
    Sort by start time. Merge overlapping.
    
    Time: O(N log N) | Space: O(N)
    """
    if not intervals:
        return []
    
    intervals.sort(key=lambda x: x[0])
    merged = [intervals[0]]
    
    for start, end in intervals[1:]:
        if start <= merged[-1][1]:
            # Overlapping: extend the last merged interval
            merged[-1][1] = max(merged[-1][1], end)
        else:
            merged.append([start, end])
    
    return merged


def insert(intervals: list[list[int]], newInterval: list[int]) -> list[list[int]]:
    """
    LeetCode 57. Insert Interval.
    
    Insert newInterval into sorted, non-overlapping intervals list.
    
    Time: O(N) | Space: O(N)
    """
    result = []
    i = 0
    n = len(intervals)
    
    # Add all intervals that come before newInterval
    while i < n and intervals[i][1] < newInterval[0]:
        result.append(intervals[i])
        i += 1
    
    # Merge all overlapping intervals with newInterval
    while i < n and intervals[i][0] <= newInterval[1]:
        newInterval[0] = min(newInterval[0], intervals[i][0])
        newInterval[1] = max(newInterval[1], intervals[i][1])
        i += 1
    
    result.append(newInterval)
    
    # Add remaining intervals
    while i < n:
        result.append(intervals[i])
        i += 1
    
    return result


def minMeetingRooms(intervals: list[list[int]]) -> int:
    """
    LeetCode 253. Meeting Rooms II.
    Minimum number of rooms for non-overlapping scheduling.
    
    Heap approach: track end times of currently running meetings.
    
    Time: O(N log N) | Space: O(N)
    """
    import heapq
    if not intervals:
        return 0
    
    intervals.sort(key=lambda x: x[0])
    heap = []  # min-heap of end times
    
    for start, end in intervals:
        if heap and heap[0] <= start:
            heapq.heapreplace(heap, end)  # Reuse freed room
        else:
            heapq.heappush(heap, end)  # Need a new room
    
    return len(heap)


# Tests
assert merge([[1,3],[2,6],[8,10],[15,18]]) == [[1,6],[8,10],[15,18]]
assert insert([[1,3],[6,9]], [2,5]) == [[1,5],[6,9]]
assert minMeetingRooms([[0,30],[5,10],[15,20]]) == 2
```

---

## 10. Problem 6: Decode Ways

**Frequency at Meta:** ★★★★☆ | **Difficulty:** Medium

```python
def numDecodings(s: str) -> int:
    """
    LeetCode 91. Decode Ways.
    
    DP: dp[i] = number of ways to decode s[0..i-1]
    
    Two choices at each position:
    1. Single digit decode (s[i-1] in '1'-'9')
    2. Two digit decode (s[i-2:i] in '10'-'26')
    
    Time: O(N) | Space: O(1) with rolling variables
    """
    if not s or s[0] == '0':
        return 0
    
    n = len(s)
    prev2 = 1  # dp[i-2]: ways to decode empty prefix
    prev1 = 1  # dp[i-1]: ways to decode s[0]
    
    for i in range(2, n + 1):
        curr = 0
        
        one_digit = int(s[i-1])
        two_digit = int(s[i-2:i])
        
        if 1 <= one_digit <= 9:
            curr += prev1  # Decode s[i-1] alone
        
        if 10 <= two_digit <= 26:
            curr += prev2  # Decode s[i-2:i] as pair
        
        prev2, prev1 = prev1, curr
    
    return prev1


def numDecodings_with_star(s: str) -> int:
    """
    LeetCode 639. Decode Ways II — with '*' wildcard.
    '*' matches any digit 1-9.
    
    More complex DP with careful case analysis.
    Time: O(N) | Space: O(1)
    """
    MOD = 10**9 + 7
    prev2, prev1 = 1, 9 if s[0] == '*' else (0 if s[0] == '0' else 1)
    
    for i in range(1, len(s)):
        curr = 0
        c, p = s[i], s[i-1]
        
        # One digit: current char
        if c == '*':
            curr += 9 * prev1  # '1'-'9'
        elif c != '0':
            curr += prev1
        
        # Two digits: previous + current
        if p == '*' and c == '*':
            curr += 15 * prev2  # 11-19 (9) + 21-26 (6) = 15
        elif p == '*':
            if c <= '6':
                curr += 2 * prev2  # 1c and 2c both valid
            else:
                curr += prev2      # Only 1c valid
        elif c == '*':
            if p == '1':
                curr += 9 * prev2  # 11-19
            elif p == '2':
                curr += 6 * prev2  # 21-26
        else:
            two = int(p + c)
            if 10 <= two <= 26:
                curr += prev2
        
        prev2, prev1 = prev1, curr % MOD
    
    return prev1


# Tests
assert numDecodings("12") == 2   # "12" or "1","2"
assert numDecodings("226") == 3  # "226", "22","6", "2","26"
assert numDecodings("06") == 0   # Leading zero invalid
assert numDecodings("") == 0
```

---

## 11. Problem 7: Word Break

**Frequency at Meta:** ★★★★☆ | **Difficulty:** Medium

```python
def wordBreak(s: str, wordDict: list[str]) -> bool:
    """
    LeetCode 139. Word Break.
    
    DP: dp[i] = True if s[0..i-1] can be segmented using wordDict.
    
    Time: O(N² * M) where M = max word length | Space: O(N)
    """
    word_set = set(wordDict)
    n = len(s)
    dp = [False] * (n + 1)
    dp[0] = True  # Empty string is always valid
    
    for i in range(1, n + 1):
        for j in range(i):
            if dp[j] and s[j:i] in word_set:
                dp[i] = True
                break  # No need to check further j values
    
    return dp[n]


def wordBreak_with_trie(s: str, wordDict: list[str]) -> bool:
    """
    Trie optimization: O(N² + W * L) where W = |wordDict|, L = max word length.
    Trie lookup is O(L) vs O(L) for set, but trie can prune early.
    """
    # Build Trie
    trie = {}
    for word in wordDict:
        node = trie
        for char in word:
            node = node.setdefault(char, {})
        node['#'] = True  # End of word
    
    n = len(s)
    dp = [False] * (n + 1)
    dp[0] = True
    
    for i in range(n):
        if not dp[i]:
            continue
        node = trie
        for j in range(i, n):
            if s[j] not in node:
                break
            node = node[s[j]]
            if '#' in node:
                dp[j + 1] = True
    
    return dp[n]


def wordBreak_II(s: str, wordDict: list[str]) -> list[str]:
    """
    LeetCode 140. Word Break II — return all valid sentences.
    
    Memoized DFS + backtracking.
    Time: O(N³ + total output length) | Space: O(N)
    """
    word_set = set(wordDict)
    memo = {}
    
    def dfs(start: int) -> list[str]:
        if start in memo:
            return memo[start]
        if start == len(s):
            return [""]
        
        sentences = []
        for end in range(start + 1, len(s) + 1):
            word = s[start:end]
            if word in word_set:
                for rest in dfs(end):
                    sentences.append(word + (" " + rest if rest else ""))
        
        memo[start] = sentences
        return sentences
    
    return dfs(0)


# Tests
assert wordBreak("leetcode", ["leet","code"]) == True
assert wordBreak("applepenapple", ["apple","pen"]) == True
assert wordBreak("catsandog", ["cats","dog","sand","and","cat"]) == False
print(wordBreak_II("catsanddog", ["cat","cats","and","sand","dog"]))
# ["cats and dog", "cat sand dog"]
```

---

## 12. Problem 8: Diameter of Binary Tree

**Frequency at Meta:** ★★★★★ | **Difficulty:** Easy-Medium

```python
def diameterOfBinaryTree(root: TreeNode) -> int:
    """
    LeetCode 543. Diameter of Binary Tree.
    
    Diameter = longest path between any two nodes (may not pass through root).
    At each node: diameter_through_node = left_height + right_height
    
    Key: update global max during post-order traversal.
    
    Time: O(N) | Space: O(H)
    """
    max_diameter = [0]
    
    def height(node) -> int:
        if not node:
            return 0
        left_h = height(node.left)
        right_h = height(node.right)
        max_diameter[0] = max(max_diameter[0], left_h + right_h)
        return 1 + max(left_h, right_h)
    
    height(root)
    return max_diameter[0]


def longestUnivaluePath(root: TreeNode) -> int:
    """
    LeetCode 687. Longest Univalue Path.
    Similar structure to diameter but only count same-value paths.
    
    Time: O(N) | Space: O(H)
    """
    max_path = [0]
    
    def dfs(node) -> int:
        if not node:
            return 0
        left_len = dfs(node.left)
        right_len = dfs(node.right)
        
        # Only extend if child value matches
        left_path = left_len + 1 if node.left and node.left.val == node.val else 0
        right_path = right_len + 1 if node.right and node.right.val == node.val else 0
        
        max_path[0] = max(max_path[0], left_path + right_path)
        return max(left_path, right_path)
    
    dfs(root)
    return max_path[0]


# Tests
t1 = TreeNode(1, TreeNode(2, TreeNode(4), TreeNode(5)), TreeNode(3))
assert diameterOfBinaryTree(t1) == 3  # 4-2-1-3 or 5-2-1-3

t2 = TreeNode(1, TreeNode(2), None)
assert diameterOfBinaryTree(t2) == 1
```

---

## 13. Meta Follow-Up Question Patterns

```python
# Meta's follow-up questions are very predictable:

FOLLOWUP_1 = {
    "original": "Number of Islands (grid BFS/DFS)",
    "followups": [
        "What if the grid is very large (doesn't fit in memory)?",
        "What if islands are added/removed dynamically? (Union-Find)",
        "What if we need the size of each island?",
        "What if it's 3D (volume of water trapped)?",
    ]
}

FOLLOWUP_2 = {
    "original": "Binary Tree operations",
    "followups": [
        "Make it iterative (avoid stack overflow)",
        "What if it's an N-ary tree?",
        "What if nodes have parent pointers?",
        "What if the tree is unbalanced (N=10^5 nodes, skewed)?",
    ]
}

FOLLOWUP_3 = {
    "original": "Word Break / word search",
    "followups": [
        "Return all valid paths (not just true/false)",
        "What if words can be reused?",
        "What if the dictionary is very large (10^6 words)?",
        "Use Trie to optimize multiple queries",
    ]
}

# Response framework for follow-ups:
def handle_followup(original_solution, followup):
    """
    Template for answering follow-ups:
    1. Identify what changes
    2. State new constraints
    3. Propose modification
    4. State new complexity
    """
    response = """
    That's a great extension. With [new constraint], the key change is [X].
    
    In my current solution, [Y] would break/be inefficient because [Z].
    
    I'd modify by [modification]. This changes the complexity to [new TC/SC].
    
    Specifically, [walk through key change].
    """
```

---

## 14. Meta-Specific Interview Tips

### The Meta Coding Style

```python
# Meta values: readable, clean, Pythonic code

# Preferred: Clear separation of logic
def isValidParentheses(s: str) -> bool:
    """Validate parentheses matching."""
    stack = []
    pairs = {')': '(', ']': '[', '}': '{'}
    
    for char in s:
        if char in '([{':
            stack.append(char)
        elif char in pairs:
            if not stack or stack[-1] != pairs[char]:
                return False
            stack.pop()
    
    return not stack

# NOT preferred: Over-compressed (hard to read in 35 minutes)
def isValidParen(s):
    d={'(':')','[':']','{':'}'};st=[]
    return all((not d.get(c) and (not st or st.pop()!=d.get(c)) or True)
               if True else False for c in s)
```

### Meta's Behavioral Values Alignment

```
Meta's Core Values (shown in coding interviews):
1. "Build social value" → code that helps many users
2. "Move fast" → complete working solution > perfect incomplete
3. "Be bold" → propose optimization even if harder to implement
4. "Be open" → discuss trade-offs, don't hide weaknesses
5. "Focus on impact" → mention what your solution optimizes for users

Common behavioral questions at Meta:
- "Tell me about a time you had a technical disagreement"
- "Tell me about a project you're most proud of"  
- "How do you handle technical debt?"
- "Describe a time you received difficult feedback"
```

### Speed Optimization for 2-Problem Format

```python
# Patterns to memorize for 5-minute solutions:

# Pattern 1: BFS template (always ready)
from collections import deque
def bfs_template(start, is_goal, get_neighbors):
    visited = {start}
    queue = deque([(start, 0)])
    while queue:
        node, steps = queue.popleft()
        if is_goal(node): return steps
        for nb in get_neighbors(node):
            if nb not in visited:
                visited.add(nb)
                queue.append((nb, steps + 1))
    return -1

# Pattern 2: Backtracking template
def backtrack_template(candidates, start, current, result, is_valid, is_complete):
    if is_complete(current):
        result.append(current[:])
        return
    for i in range(start, len(candidates)):
        if is_valid(candidates[i], current):
            current.append(candidates[i])
            backtrack_template(candidates, i+1, current, result, is_valid, is_complete)
            current.pop()

# Pattern 3: Tree DFS returning value from subtrees
def tree_dp(root):
    def dfs(node):
        if not node: return 0, 0  # (answer, info_for_parent)
        l_ans, l_info = dfs(node.left)
        r_ans, r_info = dfs(node.right)
        curr_ans = ???
        curr_info = ???
        return max(l_ans, r_ans, curr_ans), curr_info
    return dfs(root)[0]
```

---

*Meta's interview speed requirement — 2 problems in 35 minutes — is its defining challenge. Master your most common tree and graph patterns to sub-10-minute coding. The candidates who succeed are those who have internalized these patterns so deeply that the implementation becomes nearly automatic, leaving mental bandwidth for edge cases and follow-ups.*

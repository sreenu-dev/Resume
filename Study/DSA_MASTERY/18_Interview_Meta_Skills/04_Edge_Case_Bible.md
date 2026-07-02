# The Edge Cases Bible — FAANG Interview Comprehensive Guide

> **Level:** Advanced Reference | **Use:** Before every coding session, before every submission  
> **Reality:** 40% of "wrong answer" failures in interviews are edge case failures, not algorithm failures

---

## Table of Contents
1. [Array Edge Cases — 12 Categories](#1-array-edge-cases--12-categories)
2. [String Edge Cases — 10 Categories](#2-string-edge-cases--10-categories)
3. [Tree Edge Cases — 9 Categories](#3-tree-edge-cases--9-categories)
4. [Graph Edge Cases — 10 Categories](#4-graph-edge-cases--10-categories)
5. [Dynamic Programming Edge Cases](#5-dynamic-programming-edge-cases)
6. [Number/Integer Edge Cases](#6-numberinteger-edge-cases)
7. [Linked List Edge Cases — 8 Categories](#7-linked-list-edge-cases--8-categories)
8. [Two Pointer / Sliding Window Edge Cases](#8-two-pointer--sliding-window-edge-cases)
9. [Sorting Algorithm Edge Cases](#9-sorting-algorithm-edge-cases)
10. [Heap / Priority Queue Edge Cases](#10-heap--priority-queue-edge-cases)
11. [HashMap / HashSet Edge Cases](#11-hashmap--hashset-edge-cases)
12. [Recursion / Backtracking Edge Cases](#12-recursion--backtracking-edge-cases)
13. [Edge Case Verification Template](#13-edge-case-verification-template)

---

## 1. Array Edge Cases — 12 Categories

```python
# ─── CATEGORY 1: Empty Array ───
def handle_empty(arr: list) -> list:
    if not arr:  # or: if len(arr) == 0
        return []  # or -1, 0, False — depends on problem
    # Main logic

# TRIGGER: arr = []
# Problems broken: max/min subarray, kth element, two sum

# ─── CATEGORY 2: Single Element ───
arr_single = [42]
# Problems broken:
# - Merge sort: must handle len=1 as base case
# - Two pointer: left and right overlap at 0
# - Sliding window with k > 1: no valid window
def check_single(arr, k):
    if len(arr) < k:
        return []  # Can't form window of size k

# ─── CATEGORY 3: All Same Elements ───
arr_same = [5, 5, 5, 5, 5]
# Problems broken:
# - Duplicate removal: should remove all or keep one?
# - KthLargest: k-th largest = arr[0] when all same
# - Partition in QuickSort: infinite recursion without 3-way partition
# - Binary search for target: finds any occurrence, not all

# ─── CATEGORY 4: Sorted Ascending ───
arr_asc = [1, 2, 3, 4, 5]
# Problems broken:
# - Binary search: correct behavior, verify boundaries
# - QuickSort: worst case O(N²) with naive pivot
# - Max subarray: entire array is answer

# ─── CATEGORY 5: Sorted Descending ───
arr_desc = [5, 4, 3, 2, 1]
# Problems broken:
# - Same as ascending but verify your comparisons
# - Merge of two sorted arrays: one empty, one full scenario

# ─── CATEGORY 6: Negative Numbers ───
arr_neg = [-3, -1, -4, -1, -5, -9, -2, -6]
# Problems broken:
# - Max subarray: Kadane's with wrong initialization
# - Product of array: sign flips at zero
# - Two sum: complement can be negative (need HashMap not only positive indices)

def maxSubarray(nums):
    # WRONG initialization:
    max_sum = 0  # ❌ Returns 0 for all-negative arrays
    
    # CORRECT:
    max_sum = nums[0]  # ✅ Initialize with first element
    curr_sum = nums[0]
    for x in nums[1:]:
        curr_sum = max(x, curr_sum + x)
        max_sum = max(max_sum, curr_sum)
    return max_sum

# Test
assert maxSubarray([-3, -1, -4]) == -1  # Best is just [-1]
assert maxSubarray([-2, 1, -3, 4, -1, 2, 1, -5, 4]) == 6

# ─── CATEGORY 7: Zeros in Array ───
arr_zeros = [0, 0, 0, 0]
arr_mixed = [2, 0, -3, 5, 0]
# Problems broken:
# - Product of array: any zero makes product zero (need zero-count tracking)
# - Division: divide-by-zero
# - Log of zero: undefined

def productExceptSelf(nums):
    """Handle zeros correctly."""
    n = len(nums)
    zero_count = nums.count(0)
    
    if zero_count > 1:
        return [0] * n  # All zeros
    
    total_product = 1
    for x in nums:
        if x != 0:
            total_product *= x
    
    result = []
    for x in nums:
        if zero_count == 1:
            result.append(total_product if x == 0 else 0)
        else:
            result.append(total_product // x)
    return result

# ─── CATEGORY 8: Duplicates ───
arr_dup = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3]
# Problems broken:
# - Binary search for range: use bisect_left and bisect_right
# - Remove duplicates: in-place needs two-pointer
# - K-th distinct element: must track uniqueness

# ─── CATEGORY 9: Overflow ───
arr_large = [10**9, 10**9, 10**9]  # Sum = 3*10^9 > INT32_MAX
# In Python: no overflow (arbitrary precision)
# In Java/C++: use long, check before multiply

def check_overflow():
    import sys
    INT32_MAX = 2**31 - 1    # 2,147,483,647
    INT32_MIN = -(2**31)     # -2,147,483,648
    INT64_MAX = 2**63 - 1
    
    # Python automatically handles big integers
    big_sum = 10**9 + 10**9 + 10**9  # 3000000000 — fine in Python
    print(f"Sum: {big_sum}, exceeds int32: {big_sum > INT32_MAX}")

# ─── CATEGORY 10: Array Length = K (Exact Window Size) ───
arr = [1, 2, 3]
k = 3  # k equals array length
# Problems broken: sliding window — must check if window can even form

# ─── CATEGORY 11: K Larger Than Array Length ───
arr = [1, 2, 3]
k = 5
# Must return [] or handle gracefully

# ─── CATEGORY 12: Two Arrays of Different Lengths ───
arr1 = [1, 3, 5]
arr2 = [2, 4, 6, 8, 10]
# Merge sorted arrays: when one is exhausted, append rest of other
def merge(arr1, arr2):
    result = []
    i = j = 0
    while i < len(arr1) and j < len(arr2):
        if arr1[i] <= arr2[j]:
            result.append(arr1[i]); i += 1
        else:
            result.append(arr2[j]); j += 1
    result.extend(arr1[i:])  # Remaining elements
    result.extend(arr2[j:])
    return result
```

---

## 2. String Edge Cases — 10 Categories

```python
# ─── CATEGORY 1: Empty String ───
s = ""
# Problems broken: palindrome check (return True), anagram (equal if both ""),
# KMP (return [] for pattern search), sliding window (window never forms)

def isAnagram(s: str, t: str) -> bool:
    if len(s) != len(t): return False
    # Works correctly for empty strings: 
    # isAnagram("", "") → True ✓

# ─── CATEGORY 2: Single Character ───
s = "a"
# Problems broken: two-pointer palindrome, KMP with pattern="a"

# ─── CATEGORY 3: All Same Characters ───
s = "aaaa"
# Problems broken:
# - Longest substring without repeating: answer should be 1
# - Palindrome check: entire string is palindrome
# - Anagram: "aaaa" == "aaaa"? Yes. "aaaa" == "aaa"? No.

def lengthOfLongestSubstring(s: str) -> int:
    last_seen = {}
    left = 0
    max_len = 0
    for right, char in enumerate(s):
        if char in last_seen and last_seen[char] >= left:
            left = last_seen[char] + 1
        last_seen[char] = right
        max_len = max(max_len, right - left + 1)
    return max_len

assert lengthOfLongestSubstring("aaaa") == 1  # Must be 1!

# ─── CATEGORY 4: Spaces and Special Characters ───
s = "hello world"   # Space is a valid character
s = "  hello  "     # Leading/trailing spaces
s = "!@#$%^&*()"    # Special characters
# Always confirm: "Does the input contain only lowercase letters?" 
# Or: "Any ASCII character including spaces?"

# ─── CATEGORY 5: Uppercase and Mixed Case ───
s = "Hello"
s = "HeLLo WoRLd"
# Always ask: "Is comparison case-sensitive?"
# Often need: s.lower() first

# ─── CATEGORY 6: Unicode ───
s = "héllo"  # é is a multi-byte character
s = "你好"    # Chinese characters
# In Python 3: strings are Unicode by default
# len("héllo") == 5, len("你好") == 2

# ─── CATEGORY 7: Palindrome Edge Cases ───
def isPalindrome(s: str) -> bool:
    # Edge cases:
    # "" → True (empty string is palindrome by convention)
    # "a" → True (single char)
    # "aa" → True
    # "aba" → True
    # "ab" → False
    
    cleaned = ''.join(c.lower() for c in s if c.isalnum())
    return cleaned == cleaned[::-1]

assert isPalindrome("") == True
assert isPalindrome("A man, a plan, a canal: Panama") == True
assert isPalindrome("race a car") == False

# ─── CATEGORY 8: String as Number ───
# "0" → 0 (valid)
# "-123" → -123 (negative)
# "+123" → 123 (with explicit plus)
# "123.45" → float (if floats allowed)
# "12e3" → 12000 (scientific notation?)
# "" → invalid
# "   123   " → 123 (leading/trailing spaces allowed?)

def myAtoi(s: str) -> int:
    s = s.strip()
    if not s: return 0
    sign = 1
    i = 0
    if s[0] in '+-':
        sign = -1 if s[0] == '-' else 1
        i = 1
    result = 0
    while i < len(s) and s[i].isdigit():
        result = result * 10 + int(s[i])
        i += 1
    result *= sign
    INT_MIN, INT_MAX = -(2**31), 2**31 - 1
    return max(INT_MIN, min(INT_MAX, result))

# ─── CATEGORY 9: Substring vs Subsequence ───
# Substring: CONTIGUOUS "abc" in "xabcy" → True
# Subsequence: NOT contiguous "ace" in "abcde" → True (a,c,e in order)
# Always clarify which one the problem means!

# ─── CATEGORY 10: Pattern Matching Edge Cases ───
# Pattern "" matches "" and "" only? Or matches everything?
# ".*" in regex matches everything
# "*" in wildcard can match "" (empty)
# Always verify empty pattern and empty string behavior
```

---

## 3. Tree Edge Cases — 9 Categories

```python
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

# ─── CATEGORY 1: Null/Empty Tree ───
root = None
# Problems broken: height, diameter, path sum, LCA
def height(root) -> int:
    if not root: return 0  # ✅ ALWAYS handle null first
    return 1 + max(height(root.left), height(root.right))

assert height(None) == 0

# ─── CATEGORY 2: Single Node ───
root = TreeNode(5)
# Problems broken: diameter (0, no edges), height (1)
# LCA of single node with itself → that node

assert height(TreeNode(5)) == 1

# ─── CATEGORY 3: Completely Skewed Left (Like Linked List) ───
# 1 → 2 → 3 → 4 → 5 (all left children)
root = TreeNode(1, TreeNode(2, TreeNode(3, TreeNode(4, TreeNode(5)))))
# Problems broken: recursive solutions hit Python's recursion limit (default 1000)
# For N=10^5 nodes: recursive DFS → stack overflow!
# Solution: iterative DFS with explicit stack

import sys
sys.setrecursionlimit(100000)  # Or convert to iterative

# ─── CATEGORY 4: Completely Skewed Right ───
# Same issue as left skew but in opposite direction

# ─── CATEGORY 5: Complete Binary Tree ───
# All levels full except possibly last, which fills left to right
# Special property: count nodes = O(log²N) using binary search
def countNodes(root) -> int:
    if not root: return 0
    left_h = right_h = 0
    l, r = root, root
    while l: left_h += 1; l = l.left
    while r: right_h += 1; r = r.right
    if left_h == right_h:
        return 2**left_h - 1  # Full binary tree shortcut
    return 1 + countNodes(root.left) + countNodes(root.right)

# ─── CATEGORY 6: BST with Duplicate Values ───
# By convention: duplicates go to LEFT or RIGHT (problem-specific!)
# Always confirm: "Are BST values unique?"
# Max/min in BST: rightmost/leftmost node
# Inorder traversal: produces SORTED array (duplicates appear sorted too)

# ─── CATEGORY 7: BST Validation Traps ───
# WRONG check (only parent-child, not ancestors):
def isValidBSTWrong(root):
    if not root: return True
    if root.left and root.left.val >= root.val: return False  # ❌
    if root.right and root.right.val <= root.val: return False  # ❌
    return isValidBSTWrong(root.left) and isValidBSTWrong(root.right)

# This fails for:
#     5
#    / \
#   1   6
#      / \
#     3   7  (3 < 5 but 3 is in right subtree of 5 — INVALID!)

# CORRECT check (pass valid range):
def isValidBST(root, lo=float('-inf'), hi=float('inf')) -> bool:
    if not root: return True
    if not lo < root.val < hi: return False
    return (isValidBST(root.left, lo, root.val) and
            isValidBST(root.right, root.val, hi))

# ─── CATEGORY 8: LCA Edge Cases ───
# LCA(root, root) → root
# LCA(node, its_ancestor) → the ancestor
# LCA(node, node) → node itself

# ─── CATEGORY 9: Path Sum Edge Cases ───
# Empty tree: no path exists → False
# Single node with val = targetSum: path exists (just root) → True
# Path must go from ROOT to LEAF (not any node to any node!)
# Negative values: can't prune branches early

def hasPathSum(root, targetSum: int) -> bool:
    if not root: return False  # ✅ Empty tree
    if not root.left and not root.right:  # ✅ Leaf node
        return root.val == targetSum
    return (hasPathSum(root.left, targetSum - root.val) or
            hasPathSum(root.right, targetSum - root.val))
```

---

## 4. Graph Edge Cases — 10 Categories

```python
# ─── CATEGORY 1: Empty Graph ───
n = 0
edges = []
# Problems broken: BFS/DFS, connected components, topological sort

# ─── CATEGORY 2: Single Node, No Edges ───
n = 1
edges = []
# BFS from node 0: visits [0], done
# Connected components: 1 component

# ─── CATEGORY 3: Disconnected Graph ───
# 0-1-2   3-4   5
# Problems broken: DFS/BFS from one source doesn't visit all nodes
# Solution: iterate over ALL unvisited nodes as sources

def countComponents(n: int, edges: list) -> int:
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    visited = [False] * n
    count = 0
    
    for start in range(n):  # ✅ Try ALL nodes as source
        if not visited[start]:
            # DFS from start
            stack = [start]
            while stack:
                node = stack.pop()
                if visited[node]: continue
                visited[node] = True
                for neighbor in adj[node]:
                    if not visited[neighbor]:
                        stack.append(neighbor)
            count += 1
    
    return count

# ─── CATEGORY 4: Self-Loop (Node Points to Itself) ───
edges = [(0, 0), (1, 2)]
# Cycle detection: self-loop IS a cycle
# BFS/DFS: must check "if neighbor == current AND not parent" for undirected

def hasCycleDirected(n, edges):
    adj = [[] for _ in range(n)]
    for u, v in edges:
        adj[u].append(v)
    
    # 0=unvisited, 1=in-stack, 2=done
    color = [0] * n
    
    def dfs(u):
        color[u] = 1  # In stack
        for v in adj[u]:
            if color[v] == 1: return True   # Back edge → cycle
            if color[v] == 0 and dfs(v): return True
        color[u] = 2  # Done
        return False
    
    return any(color[u] == 0 and dfs(u) for u in range(n))

# ─── CATEGORY 5: Multiple Edges Between Same Nodes ───
edges = [(0, 1), (0, 1), (0, 1)]  # Multi-graph
# Build adjacency list: [0] → [1, 1, 1] (multi-edge)
# If problem says "simple graph", deduplicate edges

# ─── CATEGORY 6: Negative Edge Weights ───
# Dijkstra FAILS with negative edges!
# Use Bellman-Ford instead
# If no negative CYCLES, Bellman-Ford gives correct answer
# With negative cycles: no shortest path exists (infinite loop)

def bellman_ford(n, edges, src):
    dist = [float('inf')] * n
    dist[src] = 0
    
    for _ in range(n - 1):  # Relax n-1 times
        for u, v, w in edges:
            if dist[u] != float('inf') and dist[u] + w < dist[v]:
                dist[v] = dist[u] + w
    
    # Check for negative cycles
    for u, v, w in edges:
        if dist[u] != float('inf') and dist[u] + w < dist[v]:
            return None  # Negative cycle detected!
    
    return dist

# ─── CATEGORY 7: Graph with No Path Between Nodes ───
# Query: shortest path from A to B in disconnected graph
# Dijkstra returns inf → should return -1 in answer

# ─── CATEGORY 8: Star Graph (One Central Hub) ───
# N-1 edges connecting center to all others
# BFS: O(N) but don't add center to queue N times

# ─── CATEGORY 9: Complete Graph ───
# N*(N-1)/2 edges for undirected → O(N²) edges
# DFS might be slow → consider algorithms that use edges count

# ─── CATEGORY 10: Graph with Only One Direction Path ───
# 0→1→2→3→4 (directed, no back edges)
# Topological sort: valid
# Cycle detection: no cycle
# Shortest path: simple BFS
```

---

## 5. Dynamic Programming Edge Cases

```python
# ─── OFF-BY-ONE IN DP ───

# WRONG: dp[0] initialization for 1-indexed problems
def climbStairs_wrong(n: int) -> int:
    dp = [0] * n  # ❌ n+1 needed for dp[n]
    dp[1] = 1
    # dp[n] is out of bounds for n elements!

# CORRECT:
def climbStairs(n: int) -> int:
    if n <= 2: return n
    dp = [0] * (n + 1)  # ✅ Extra space for dp[n]
    dp[1], dp[2] = 1, 2
    for i in range(3, n + 1):
        dp[i] = dp[i-1] + dp[i-2]
    return dp[n]

# ─── MEMOIZATION KEY DESIGN ───

# WRONG: Using mutable types as keys
@functools.lru_cache(maxsize=None)
def wrong_key(arr, idx):  # ❌ list is not hashable
    pass

# CORRECT: Convert to tuple
def dp_with_state(arr: list, idx: int, memo=None):
    if memo is None: memo = {}
    state = (idx,)  # Only the variable part of state
    if state in memo: return memo[state]
    # ... compute
    memo[state] = result
    return result

# ─── BASE CASE PITFALLS ───

# Longest Common Subsequence:
def lcs(s1, s2):
    m, n = len(s1), len(s2)
    dp = [[0] * (n + 1) for _ in range(m + 1)]
    # Base case: dp[0][j] = 0 (empty s1), dp[i][0] = 0 (empty s2)
    # This is automatically 0 from initialization ✓
    
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if s1[i-1] == s2[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    return dp[m][n]

# Edge: lcs("", "abc") → 0 ✓
# Edge: lcs("abc", "") → 0 ✓
# Edge: lcs("", "") → 0 ✓

# ─── INTEGER OVERFLOW IN DP ───
# Counting paths: can overflow if not taken mod
MOD = 10**9 + 7

def uniquePaths(m, n):
    dp = [[1] * n for _ in range(m)]
    for i in range(1, m):
        for j in range(1, n):
            dp[i][j] = (dp[i-1][j] + dp[i][j-1]) % MOD  # ✅ Take mod
    return dp[m-1][n-1]
```

---

## 6. Number/Integer Edge Cases

```python
# ─── ZERO ───
# Division by zero: always check divisor != 0
# Log(0): undefined
# Product with 0: collapses entire product
# Is 0 positive or negative? Neither!

# ─── NEGATIVE NUMBERS ───
# Python: -7 // 2 = -4 (floor division, rounds toward -inf)
# Java/C++: -7 / 2 = -3 (truncates toward zero)
print(-7 // 2)  # -4 in Python
print(int(-7 / 2))  # -3 in Python (mimics C behavior)

# ─── INT_MIN EDGE CASE (reverse integer) ───
def reverse(x: int) -> int:
    INT_MIN, INT_MAX = -(2**31), 2**31 - 1
    sign = -1 if x < 0 else 1
    digits = str(abs(x))[::-1]
    result = sign * int(digits)
    if result < INT_MIN or result > INT_MAX:
        return 0  # ✅ Handle overflow
    return result

# INT_MIN = -2147483648 reversed = 8463847412 → overflow → return 0

# ─── MODULAR ARITHMETIC PITFALLS ───
# MOD with negative numbers in Python:
print((-7) % 3)   # 2 in Python (always non-negative)
print((-7) % 3)   # -1 in C/Java/C++

# Safe modular exponentiation:
def pow_mod(base, exp, mod):
    return pow(base, exp, mod)  # Python built-in is fast

# ─── FLOAT PRECISION ───
# NEVER use float == comparison!
a = 0.1 + 0.2
print(a == 0.3)  # False! (floating point imprecision)
print(abs(a - 0.3) < 1e-9)  # ✅ Correct comparison

# For financial calculations: use Python's Decimal module
from decimal import Decimal, ROUND_HALF_UP
price = Decimal('0.1') + Decimal('0.2')
print(price == Decimal('0.3'))  # True ✓

# ─── BIT MANIPULATION EDGE CASES ───
# Negative number bit operations:
n = -1
print(bin(n))  # '-0b1' in Python (not 2's complement like C)
# Python integers are infinite precision with sign bit

# Safe right shift for non-negative:
def count_bits(n: int) -> int:
    count = 0
    while n:
        count += n & 1
        n >>= 1  # Works for non-negative n
    return count

# For negative: use n & 0xFFFFFFFF to mask to 32-bit
```

---

## 7. Linked List Edge Cases — 8 Categories

```python
class ListNode:
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next

# ─── CATEGORY 1: Empty List (head = None) ───
def reverseList(head: ListNode) -> ListNode:
    if not head: return None  # ✅ ALWAYS first check

# ─── CATEGORY 2: Single Node ───
head = ListNode(5)
# Reverse: returns same node
# Find cycle: no cycle
# Middle: the node itself

# ─── CATEGORY 3: Two Nodes ───
head = ListNode(1, ListNode(2))
# Find middle: depends on definition (first or second middle)
# Reverse: node2 becomes head, node1 becomes tail

def middleNode(head):
    slow = fast = head
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
    return slow  # For [1,2]: returns node(2) (second middle)

# ─── CATEGORY 4: Cycle ───
# Creating a cycle:
def make_cycle(vals, pos):
    nodes = [ListNode(v) for v in vals]
    for i in range(len(nodes) - 1):
        nodes[i].next = nodes[i+1]
    if pos >= 0:
        nodes[-1].next = nodes[pos]  # Create cycle
    return nodes[0] if nodes else None

# ─── CATEGORY 5: Palindrome with EVEN Length ───
# [1, 2, 2, 1] vs [1, 2, 1]
# Middle node detection differs for even/odd length!

def isPalindromeList(head):
    # Find middle
    slow = fast = head
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
    
    # Reverse second half
    prev, curr = None, slow
    while curr:
        curr.next, prev, curr = prev, curr, curr.next
    
    # Compare
    left, right = head, prev
    while right:  # right is shorter for even-length lists
        if left.val != right.val:
            return False
        left, right = left.next, right.next
    return True

# ─── CATEGORY 6: Merge Two Sorted Lists Edge Cases ───
def mergeTwoLists(l1, l2):
    dummy = ListNode(0)
    curr = dummy
    while l1 and l2:
        if l1.val <= l2.val:
            curr.next = l1; l1 = l1.next
        else:
            curr.next = l2; l2 = l2.next
        curr = curr.next
    curr.next = l1 or l2  # ✅ Append remaining (handles both null cases)
    return dummy.next

# Edge: l1=None, l2=[1,2,3] → [1,2,3] ✓
# Edge: l1=[1,2], l2=None → [1,2] ✓
# Edge: both None → None ✓

# ─── CATEGORY 7: Remove N-th From End ───
def removeNthFromEnd(head, n):
    dummy = ListNode(0)
    dummy.next = head  # ✅ Dummy node handles removing the head!
    fast = slow = dummy
    
    # Advance fast by n+1 steps
    for _ in range(n + 1):
        fast = fast.next  # May become None if n == length
    
    while fast:
        fast = fast.next
        slow = slow.next
    
    slow.next = slow.next.next
    return dummy.next

# Edge: Remove 1st from [1] → []  (remove head)
# Without dummy: would need special case

# ─── CATEGORY 8: Intersection of Two Lists ───
# Lists may not intersect → return None
# Lists intersect at HEAD → return head (one of the lists)
```

---

## 8. Two Pointer / Sliding Window Edge Cases

```python
# ─── SLIDING WINDOW: K LARGER THAN N ───
def maxSubarrayOfSizeK(arr, k):
    if k > len(arr):
        return -1  # ✅ No valid window!
    # ...

# ─── SLIDING WINDOW: ALL ELEMENTS SAME ───
arr = [3, 3, 3, 3]
k = 3
# Result should be 9 (sum of any 3 elements)

# ─── TWO POINTER: ARRAY NOT SORTED ───
# Two-pointer for two-sum REQUIRES sorted input!
arr_unsorted = [3, 1, 4, 1, 5]
# WRONG: Two pointer on unsorted → Use HashMap instead

# ─── TWO POINTER: DUPLICATES IN THREE-SUM ───
def threeSum(nums):
    nums.sort()
    result = []
    for i in range(len(nums) - 2):
        if i > 0 and nums[i] == nums[i-1]:
            continue  # ✅ Skip duplicate first element
        lo, hi = i + 1, len(nums) - 1
        while lo < hi:
            total = nums[i] + nums[lo] + nums[hi]
            if total == 0:
                result.append([nums[i], nums[lo], nums[hi]])
                while lo < hi and nums[lo] == nums[lo+1]:
                    lo += 1  # ✅ Skip duplicate second element
                while lo < hi and nums[hi] == nums[hi-1]:
                    hi -= 1  # ✅ Skip duplicate third element
                lo += 1; hi -= 1
            elif total < 0: lo += 1
            else: hi -= 1
    return result

# ─── VALID WINDOW: SHRINK TO EXACTLY VALID SIZE ───
def findAnagrams(s: str, p: str) -> list:
    """Must maintain window of EXACTLY len(p)."""
    from collections import Counter
    result = []
    need = Counter(p)
    have = Counter()
    formed = 0  # Characters in window matching need
    required = len(need)
    
    l = 0
    for r, char in enumerate(s):
        have[char] += 1
        if char in need and have[char] == need[char]:
            formed += 1
        
        while r - l + 1 > len(p):  # Window too large
            left_char = s[l]
            have[left_char] -= 1
            if left_char in need and have[left_char] < need[left_char]:
                formed -= 1
            l += 1
        
        if formed == required:
            result.append(l)
    return result
```

---

## 9. Sorting Algorithm Edge Cases

```python
# ─── ALREADY SORTED ───
# QuickSort: worst case O(N²) with naive first/last pivot
# Fix: Use random pivot or median-of-three

import random

def quicksort(arr, lo, hi):
    if lo >= hi: return
    # Random pivot selection
    pivot_idx = random.randint(lo, hi)
    arr[pivot_idx], arr[hi] = arr[hi], arr[pivot_idx]
    # ... partition around arr[hi]

# ─── ALL SAME ELEMENTS ───
# Quicksort with 2-way partition: O(N²) for all-equal array
# Fix: 3-way partition (Dutch National Flag)

def three_way_partition(arr, lo, hi, pivot):
    """Dutch National Flag algorithm for equal elements."""
    lt, gt = lo, hi
    i = lo
    while i <= gt:
        if arr[i] < pivot:
            arr[i], arr[lt] = arr[lt], arr[i]
            lt += 1; i += 1
        elif arr[i] > pivot:
            arr[i], arr[gt] = arr[gt], arr[i]
            gt -= 1
        else:
            i += 1
    return lt, gt  # all arr[lt..gt] == pivot

# ─── NEGATIVE NUMBERS IN SORT ───
arr = [-3, -1, -4, -1, -5, 0, 3, 1]
arr.sort()  # Works correctly: [-5, -4, -3, -1, -1, 0, 1, 3]
# Counting sort / Radix sort: needs adjustment for negative numbers
```

---

## 10. Heap / Priority Queue Edge Cases

```python
import heapq

# ─── EMPTY HEAP ───
heap = []
# heapq.heappop(heap) → IndexError!
# Always check: if heap: before popping

# ─── K LARGER THAN N ───
arr = [1, 2, 3]
k = 5
# K-th largest doesn't exist → handle appropriately

# ─── EQUAL ELEMENTS IN HEAP ───
heap = [1, 1, 1, 1, 1]
heapq.heapify(heap)
print(heapq.heappop(heap))  # 1 (any, duplicates fine)

# ─── HEAP WITH TUPLES (STABLE SORT) ───
# Python heaps with tuples: compare element by element
# (priority, counter, item) — counter makes ties break deterministically
import itertools
counter = itertools.count()
heap = []
heapq.heappush(heap, (1, next(counter), "task_a"))
heapq.heappush(heap, (1, next(counter), "task_b"))
print(heapq.heappop(heap))  # (1, 0, 'task_a') — FIFO for equal priority

# ─── FIND MEDIAN (TWO HEAPS) EDGE CASES ───
class MedianFinder:
    def __init__(self):
        self.lo = []  # max-heap (negate values)
        self.hi = []  # min-heap
    
    def addNum(self, num: int) -> None:
        heapq.heappush(self.lo, -num)
        # Ensure max(lo) <= min(hi)
        if self.hi and -self.lo[0] > self.hi[0]:
            heapq.heappush(self.hi, -heapq.heappop(self.lo))
        # Balance sizes: |lo| - |hi| <= 1
        if len(self.lo) > len(self.hi) + 1:
            heapq.heappush(self.hi, -heapq.heappop(self.lo))
        elif len(self.hi) > len(self.lo):
            heapq.heappush(self.lo, -heapq.heappop(self.hi))
    
    def findMedian(self) -> float:
        if len(self.lo) > len(self.hi):
            return -self.lo[0]  # Odd: lo has extra
        return (-self.lo[0] + self.hi[0]) / 2  # Even: average
```

---

## 11. HashMap / HashSet Edge Cases

```python
# ─── NONE/NULL AS KEY ───
d = {}
d[None] = "null_value"  # Python allows None as key!
print(d.get(None))  # "null_value"

# ─── 0 vs FALSE vs NONE AMBIGUITY ───
d = {0: "zero", False: "false"}
print(d)  # {0: 'false'}  ← 0 == False in Python!
# 0, 0.0, False, and "" are all equal in dict key context

# ─── MUTATING HASHMAP WHILE ITERATING ───
d = {1: "a", 2: "b", 3: "c"}
# WRONG:
# for k in d:
#     if k == 2: del d[k]  # RuntimeError: dict changed size during iteration

# CORRECT:
keys_to_delete = [k for k in d if k == 2]
for k in keys_to_delete:
    del d[k]

# ─── DEFAULT DICT PITFALL ───
from collections import defaultdict
d = defaultdict(list)
print(d["missing"])  # [] — creates the key!
# Use .get() if you don't want to create missing keys:
print(d.get("also_missing"))  # None — doesn't create key

# ─── COUNTER PITFALL ───
from collections import Counter
c = Counter("aab")
print(c["c"])  # 0 — Counter returns 0 for missing (not KeyError)
print(c["c"] == 0 and "c" not in c)  # True — "c" not actually in counter
```

---

## 12. Recursion / Backtracking Edge Cases

```python
# ─── RECURSION LIMIT ───
import sys
sys.setrecursionlimit(10**6)  # Default is 1000
# For N=10^5 nodes in skewed tree, default hits limit!

# ─── BACKTRACKING: MISSING BASE CASE ───
def permute_wrong(nums, curr, result):
    result.append(curr[:])  # ❌ This appends EVERY partial state!
    for n in nums:
        curr.append(n)
        permute_wrong(nums, curr, result)
        curr.pop()

# CORRECT: Only append at base case
def permute(nums, curr, remaining, result):
    if not remaining:  # ✅ Base case: nothing left
        result.append(curr[:])
        return
    for i, n in enumerate(remaining):
        curr.append(n)
        permute(nums, curr, remaining[:i] + remaining[i+1:], result)
        curr.pop()

# ─── SHARED MUTABLE STATE IN BACKTRACKING ───
result = []
current = []
# WRONG: appending current directly (reference, not copy)
result.append(current)  # ❌ All results point to same list!
# CORRECT:
result.append(current[:])  # ✅ Copy

# ─── PRUNING CORRECTNESS ───
def subsets(nums, start, curr, result):
    result.append(curr[:])  # Add current subset
    for i in range(start, len(nums)):
        # PRUNE: i > start and nums[i] == nums[i-1]: skip duplicate
        if i > start and nums[i] == nums[i-1]:
            continue
        curr.append(nums[i])
        subsets(nums, i + 1, curr, result)
        curr.pop()
```

---

## 13. Edge Case Verification Template

```python
def verify_solution(func, test_cases: list[tuple]) -> bool:
    """
    Universal edge case verification template.
    
    Each test case: (args, expected_output)
    """
    all_passed = True
    
    for args, expected in test_cases:
        try:
            result = func(*args) if isinstance(args, tuple) else func(args)
            if result != expected:
                print(f"FAIL: func({args}) = {result}, expected {expected}")
                all_passed = False
            else:
                print(f"PASS: func({args}) = {result}")
        except Exception as e:
            print(f"ERROR: func({args}) raised {type(e).__name__}: {e}")
            all_passed = False
    
    return all_passed


# Edge case test suite template for array problems:
ARRAY_EDGE_CASES = [
    # (input, expected)
    ([],           ???),  # Empty array
    ([5],          ???),  # Single element
    ([1, 1],       ???),  # Two equal elements
    ([1, 2],       ???),  # Two different elements
    ([-1, -2, -3], ???),  # All negative
    ([0, 0, 0],    ???),  # All zeros
    ([1, 1, 1],    ???),  # All same positive
    ([1] * 1000,   ???),  # Large input, all same
]

# Before submitting any solution, verify:
checklist = [
    "Empty input handled?",
    "Single element handled?",
    "All-same elements handled?",
    "Negative numbers handled?",
    "Zeros handled?",
    "Answer at index 0 and N-1 handled?",
    "Off-by-one in loop bounds?",
    "Correct return type?",
    "Integer overflow possible?",
    "Mutation of input (if not allowed)?",
]
```

---

*Edge cases are where good engineers distinguish themselves from great ones. The algorithm is often straightforward — it's the meticulous handling of boundaries, nulls, duplicates, and overflow that separates correct solutions from buggy ones. Make edge case checking a HABIT, not an afterthought.*

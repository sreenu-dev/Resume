# Common Coding Patterns for Interviews

## Overview

This guide covers the most important coding patterns that appear frequently in technical interviews, including at Experian. Master these patterns and you'll be able to solve 80% of interview problems.

## Pattern 1: Two Pointers

### When to Use
- Sorted arrays
- Finding pairs or triplets
- Removing duplicates
- Palindrome checking

### Template
```python
def two_pointer_pattern(arr):
    left = 0
    right = len(arr) - 1
    
    while left < right:
        # Do something with arr[left] and arr[right]
        if condition:
            left += 1
        else:
            right -= 1
    
    return result
```

### Example Problems

#### 1. Two Sum II (Sorted Array)
```python
def twoSum(numbers, target):
    """
    Given a sorted array, find two numbers that sum to target.
    Time: O(n), Space: O(1)
    """
    left, right = 0, len(numbers) - 1
    
    while left < right:
        current_sum = numbers[left] + numbers[right]
        
        if current_sum == target:
            return [left + 1, right + 1]  # 1-indexed
        elif current_sum < target:
            left += 1
        else:
            right -= 1
    
    return []
```

#### 2. Valid Palindrome
```python
def isPalindrome(s):
    """
    Check if string is a palindrome (ignoring non-alphanumeric).
    Time: O(n), Space: O(1)
    """
    left, right = 0, len(s) - 1
    
    while left < right:
        # Skip non-alphanumeric characters
        while left < right and not s[left].isalnum():
            left += 1
        while left < right and not s[right].isalnum():
            right -= 1
        
        if s[left].lower() != s[right].lower():
            return False
        
        left += 1
        right -= 1
    
    return True
```

#### 3. Container With Most Water
```python
def maxArea(height):
    """
    Find maximum area between two lines.
    Time: O(n), Space: O(1)
    """
    left, right = 0, len(height) - 1
    max_area = 0
    
    while left < right:
        width = right - left
        current_area = min(height[left], height[right]) * width
        max_area = max(max_area, current_area)
        
        # Move pointer with smaller height
        if height[left] < height[right]:
            left += 1
        else:
            right -= 1
    
    return max_area
```

---

## Pattern 2: Sliding Window

### When to Use
- Subarray/substring problems
- Finding maximum/minimum in subarrays
- Fixed or variable window size

### Template
```python
def sliding_window(arr, k):
    window_start = 0
    result = []
    
    for window_end in range(len(arr)):
        # Add element to window
        # ...
        
        # Check if window size reached
        if window_end >= k - 1:
            # Calculate result for current window
            # ...
            
            # Slide the window
            # Remove element going out
            window_start += 1
    
    return result
```

### Example Problems

#### 1. Maximum Sum Subarray of Size K
```python
def maxSumSubarray(arr, k):
    """
    Find maximum sum of any subarray of size k.
    Time: O(n), Space: O(1)
    """
    window_sum = 0
    max_sum = float('-inf')
    window_start = 0
    
    for window_end in range(len(arr)):
        window_sum += arr[window_end]
        
        if window_end >= k - 1:
            max_sum = max(max_sum, window_sum)
            window_sum -= arr[window_start]
            window_start += 1
    
    return max_sum
```

#### 2. Longest Substring Without Repeating Characters
```python
def lengthOfLongestSubstring(s):
    """
    Find length of longest substring without repeating characters.
    Time: O(n), Space: O(min(n, m)) where m is charset size
    """
    char_set = set()
    left = 0
    max_length = 0
    
    for right in range(len(s)):
        # Shrink window if duplicate found
        while s[right] in char_set:
            char_set.remove(s[left])
            left += 1
        
        char_set.add(s[right])
        max_length = max(max_length, right - left + 1)
    
    return max_length
```

#### 3. Minimum Window Substring
```python
from collections import Counter

def minWindow(s, t):
    """
    Find minimum window in s that contains all characters of t.
    Time: O(n + m), Space: O(m)
    """
    if not s or not t:
        return ""
    
    dict_t = Counter(t)
    required = len(dict_t)
    formed = 0
    window_counts = {}
    
    left = 0
    min_len = float('inf')
    min_left = 0
    
    for right in range(len(s)):
        char = s[right]
        window_counts[char] = window_counts.get(char, 0) + 1
        
        if char in dict_t and window_counts[char] == dict_t[char]:
            formed += 1
        
        # Try to shrink window
        while left <= right and formed == required:
            if right - left + 1 < min_len:
                min_len = right - left + 1
                min_left = left
            
            char = s[left]
            window_counts[char] -= 1
            if char in dict_t and window_counts[char] < dict_t[char]:
                formed -= 1
            
            left += 1
    
    return "" if min_len == float('inf') else s[min_left:min_left + min_len]
```

---

## Pattern 3: Fast and Slow Pointers

### When to Use
- Cycle detection
- Middle of linked list
- Happy number problem

### Template
```python
def fast_slow_pointers(head):
    slow = head
    fast = head
    
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
        
        if slow == fast:
            return True  # Cycle detected
    
    return False
```

### Example Problems

#### 1. Linked List Cycle
```python
def hasCycle(head):
    """
    Detect if linked list has a cycle.
    Time: O(n), Space: O(1)
    """
    if not head:
        return False
    
    slow = head
    fast = head
    
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
        
        if slow == fast:
            return True
    
    return False
```

#### 2. Middle of Linked List
```python
def middleNode(head):
    """
    Find middle node of linked list.
    Time: O(n), Space: O(1)
    """
    slow = fast = head
    
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
    
    return slow
```

---

## Pattern 4: Hash Maps

### When to Use
- Frequency counting
- Checking for duplicates
- Two sum type problems
- Anagram grouping

### Example Problems

#### 1. Two Sum
```python
def twoSum(nums, target):
    """
    Find indices of two numbers that sum to target.
    Time: O(n), Space: O(n)
    """
    seen = {}
    
    for i, num in enumerate(nums):
        complement = target - num
        if complement in seen:
            return [seen[complement], i]
        seen[num] = i
    
    return []
```

#### 2. Group Anagrams
```python
from collections import defaultdict

def groupAnagrams(strs):
    """
    Group strings that are anagrams.
    Time: O(n * k log k) where k is max string length
    Space: O(n * k)
    """
    anagram_map = defaultdict(list)
    
    for s in strs:
        sorted_s = ''.join(sorted(s))
        anagram_map[sorted_s].append(s)
    
    return list(anagram_map.values())
```

#### 3. Subarray Sum Equals K
```python
def subarraySum(nums, k):
    """
    Count subarrays with sum equal to k.
    Time: O(n), Space: O(n)
    """
    count = 0
    current_sum = 0
    sum_map = {0: 1}  # sum: frequency
    
    for num in nums:
        current_sum += num
        
        # Check if (current_sum - k) exists
        if current_sum - k in sum_map:
            count += sum_map[current_sum - k]
        
        sum_map[current_sum] = sum_map.get(current_sum, 0) + 1
    
    return count
```

---

## Pattern 5: Binary Search

### When to Use
- Sorted arrays
- Finding insertion position
- Search in rotated array
- Finding peak element

### Template
```python
def binary_search(arr, target):
    left, right = 0, len(arr) - 1
    
    while left <= right:
        mid = left + (right - left) // 2
        
        if arr[mid] == target:
            return mid
        elif arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1
    
    return -1
```

### Example Problems

#### 1. Search in Rotated Sorted Array
```python
def search(nums, target):
    """
    Search in rotated sorted array.
    Time: O(log n), Space: O(1)
    """
    left, right = 0, len(nums) - 1
    
    while left <= right:
        mid = (left + right) // 2
        
        if nums[mid] == target:
            return mid
        
        # Left half is sorted
        if nums[left] <= nums[mid]:
            if nums[left] <= target < nums[mid]:
                right = mid - 1
            else:
                left = mid + 1
        # Right half is sorted
        else:
            if nums[mid] < target <= nums[right]:
                left = mid + 1
            else:
                right = mid - 1
    
    return -1
```

#### 2. Find Minimum in Rotated Sorted Array
```python
def findMin(nums):
    """
    Find minimum in rotated sorted array.
    Time: O(log n), Space: O(1)
    """
    left, right = 0, len(nums) - 1
    
    while left < right:
        mid = (left + right) // 2
        
        if nums[mid] > nums[right]:
            left = mid + 1
        else:
            right = mid
    
    return nums[left]
```

---

## Pattern 6: Tree Traversal

### Types of Traversal

#### 1. Depth-First Search (DFS)

**Inorder (Left → Root → Right)**
```python
def inorderTraversal(root):
    result = []
    
    def inorder(node):
        if not node:
            return
        inorder(node.left)
        result.append(node.val)
        inorder(node.right)
    
    inorder(root)
    return result
```

**Preorder (Root → Left → Right)**
```python
def preorderTraversal(root):
    result = []
    
    def preorder(node):
        if not node:
            return
        result.append(node.val)
        preorder(node.left)
        preorder(node.right)
    
    preorder(root)
    return result
```

**Postorder (Left → Right → Root)**
```python
def postorderTraversal(root):
    result = []
    
    def postorder(node):
        if not node:
            return
        postorder(node.left)
        postorder(node.right)
        result.append(node.val)
    
    postorder(root)
    return result
```

#### 2. Breadth-First Search (BFS)
```python
from collections import deque

def levelOrder(root):
    """
    Level-order traversal (BFS).
    Time: O(n), Space: O(n)
    """
    if not root:
        return []
    
    result = []
    queue = deque([root])
    
    while queue:
        level_size = len(queue)
        level_nodes = []
        
        for _ in range(level_size):
            node = queue.popleft()
            level_nodes.append(node.val)
            
            if node.left:
                queue.append(node.left)
            if node.right:
                queue.append(node.right)
        
        result.append(level_nodes)
    
    return result
```

### Example Problems

#### 1. Maximum Depth of Binary Tree
```python
def maxDepth(root):
    """
    Time: O(n), Space: O(h) where h is height
    """
    if not root:
        return 0
    
    left_depth = maxDepth(root.left)
    right_depth = maxDepth(root.right)
    
    return max(left_depth, right_depth) + 1
```

#### 2. Validate Binary Search Tree
```python
def isValidBST(root):
    """
    Time: O(n), Space: O(h)
    """
    def validate(node, min_val, max_val):
        if not node:
            return True
        
        if not (min_val < node.val < max_val):
            return False
        
        return (validate(node.left, min_val, node.val) and
                validate(node.right, node.val, max_val))
    
    return validate(root, float('-inf'), float('inf'))
```

---

## Pattern 7: Graph Algorithms

### DFS for Graphs
```python
def dfs_graph(graph, start, visited=None):
    if visited is None:
        visited = set()
    
    visited.add(start)
    
    for neighbor in graph[start]:
        if neighbor not in visited:
            dfs_graph(graph, neighbor, visited)
    
    return visited
```

### BFS for Graphs
```python
from collections import deque

def bfs_graph(graph, start):
    visited = set([start])
    queue = deque([start])
    
    while queue:
        node = queue.popleft()
        
        for neighbor in graph[node]:
            if neighbor not in visited:
                visited.add(neighbor)
                queue.append(neighbor)
    
    return visited
```

### Example Problems

#### 1. Number of Islands
```python
def numIslands(grid):
    """
    Count number of islands in a 2D grid.
    Time: O(m * n), Space: O(m * n)
    """
    if not grid:
        return 0
    
    def dfs(i, j):
        if (i < 0 or i >= len(grid) or 
            j < 0 or j >= len(grid[0]) or 
            grid[i][j] != '1'):
            return
        
        grid[i][j] = '0'  # Mark as visited
        
        # Visit neighbors
        dfs(i + 1, j)
        dfs(i - 1, j)
        dfs(i, j + 1)
        dfs(i, j - 1)
    
    count = 0
    for i in range(len(grid)):
        for j in range(len(grid[0])):
            if grid[i][j] == '1':
                count += 1
                dfs(i, j)
    
    return count
```

#### 2. Clone Graph
```python
def cloneGraph(node):
    """
    Clone an undirected graph.
    Time: O(n), Space: O(n)
    """
    if not node:
        return None
    
    clones = {}
    
    def dfs(node):
        if node in clones:
            return clones[node]
        
        clone = Node(node.val)
        clones[node] = clone
        
        for neighbor in node.neighbors:
            clone.neighbors.append(dfs(neighbor))
        
        return clone
    
    return dfs(node)
```

---

## Pattern 8: Dynamic Programming

### When to Use
- Optimization problems (max/min)
- Counting problems
- Overlapping subproblems
- Optimal substructure

### Template
```python
def dp_problem(n):
    # Create DP table
    dp = [0] * (n + 1)
    
    # Base case
    dp[0] = base_value
    
    # Fill table
    for i in range(1, n + 1):
        dp[i] = calculate_from_previous(dp)
    
    return dp[n]
```

### Example Problems

#### 1. Climbing Stairs
```python
def climbStairs(n):
    """
    Count ways to climb n stairs (1 or 2 steps at a time).
    Time: O(n), Space: O(1)
    """
    if n <= 2:
        return n
    
    prev2, prev1 = 1, 2
    
    for i in range(3, n + 1):
        current = prev1 + prev2
        prev2 = prev1
        prev1 = current
    
    return prev1
```

#### 2. Coin Change
```python
def coinChange(coins, amount):
    """
    Minimum coins needed to make amount.
    Time: O(amount * len(coins)), Space: O(amount)
    """
    dp = [float('inf')] * (amount + 1)
    dp[0] = 0
    
    for i in range(1, amount + 1):
        for coin in coins:
            if coin <= i:
                dp[i] = min(dp[i], dp[i - coin] + 1)
    
    return dp[amount] if dp[amount] != float('inf') else -1
```

---

## Key Takeaways

1. **Master these 8 patterns**: Covers most interview problems
2. **Recognize patterns quickly**: Practice identifying which pattern fits
3. **Understand time/space complexity**: Always analyze your solution
4. **Practice variations**: Each pattern has many problem variations
5. **Code cleanly**: Use descriptive variable names, add comments
6. **Test edge cases**: Empty inputs, single elements, large inputs

## Next Steps

- Practice 2-3 problems per pattern
- Time yourself (aim for 20-30 minutes per problem)
- Review solutions even when you solve correctly
- Focus on patterns you struggle with

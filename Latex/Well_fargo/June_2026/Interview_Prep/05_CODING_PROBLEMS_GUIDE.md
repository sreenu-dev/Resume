# Coding Problems Guide - Wells Fargo Interview

## 📋 Overview

This guide provides a comprehensive list of coding problems to practice for Wells Fargo interviews, along with strategies and resources.

---

## 🎯 Problem Categories

### 1. Arrays & Strings (15 problems)

#### Easy
1. Two Sum
2. Valid Palindrome
3. Reverse String
4. Majority Element
5. Remove Duplicates from Sorted Array

#### Medium
6. 3Sum
7. Container With Most Water
8. Longest Substring Without Repeating Characters
9. Longest Palindromic Substring
10. Group Anagrams

#### Hard
11. Median of Two Sorted Arrays
12. Longest Substring with At Most K Distinct Characters
13. Trapping Rain Water
14. Best Time to Buy and Sell Stock IV
15. Word Ladder

### 2. Linked Lists (10 problems)

#### Easy
1. Reverse Linked List
2. Merge Two Sorted Lists
3. Palindrome Linked List
4. Linked List Cycle

#### Medium
5. Add Two Numbers
6. Reorder List
7. Remove Nth Node From End of List
8. Swap Nodes in Pairs

#### Hard
9. Merge K Sorted Lists
10. LRU Cache

### 3. Trees & Graphs (15 problems)

#### Easy
1. Binary Tree Inorder Traversal
2. Symmetric Tree
3. Maximum Depth of Binary Tree
4. Balanced Binary Tree

#### Medium
5. Binary Tree Level Order Traversal
6. Lowest Common Ancestor of BST
7. Binary Search Tree Iterator
8. Number of Islands
9. Course Schedule
10. Clone Graph

#### Hard
11. Serialize and Deserialize Binary Tree
12. Word Ladder II
13. Alien Dictionary
14. Minimum Height Trees
15. Binary Tree Maximum Path Sum

### 4. Dynamic Programming (12 problems)

#### Easy
1. Climbing Stairs
2. House Robber
3. Best Time to Buy and Sell Stock

#### Medium
4. Coin Change
5. Longest Increasing Subsequence
6. Word Break
7. Partition Equal Subset Sum
8. Decode Ways
9. Unique Paths

#### Hard
10. Burst Balloons
11. Edit Distance
12. Regular Expression Matching

### 5. Hash Tables & Sets (8 problems)

#### Easy
1. Valid Anagram
2. Isomorphic Strings
3. Happy Number

#### Medium
4. Two Sum II
5. Intersection of Two Arrays II
6. Contains Duplicate II
7. Majority Element II

#### Hard
8. LFU Cache

### 6. Stacks & Queues (8 problems)

#### Easy
1. Valid Parentheses
2. Min Stack

#### Medium
3. Evaluate Reverse Polish Notation
4. Daily Temperatures
5. Largest Rectangle in Histogram
6. Trapping Rain Water II

#### Hard
7. Sliding Window Maximum
8. The Skyline Problem

---

## 📚 Practice Schedule

### Week 1: Foundation (Arrays & Strings)
```
Day 1-2: Easy problems (5 problems)
- Two Sum
- Valid Palindrome
- Reverse String
- Majority Element
- Remove Duplicates

Day 3-4: Medium problems (5 problems)
- 3Sum
- Container With Most Water
- Longest Substring Without Repeating Characters
- Longest Palindromic Substring
- Group Anagrams

Day 5-7: Review & Practice
- Redo problems
- Optimize solutions
- Practice explaining
```

### Week 2: Data Structures (Linked Lists, Trees)
```
Day 1-2: Linked Lists (5 problems)
- Reverse Linked List
- Merge Two Sorted Lists
- Add Two Numbers
- Reorder List
- Remove Nth Node

Day 3-4: Trees (5 problems)
- Binary Tree Inorder Traversal
- Symmetric Tree
- Binary Tree Level Order Traversal
- Lowest Common Ancestor
- Number of Islands

Day 5-7: Review & Practice
- Redo problems
- Optimize solutions
- Practice explaining
```

### Week 3: Advanced (DP, Graphs)
```
Day 1-2: Dynamic Programming (5 problems)
- Climbing Stairs
- Coin Change
- Longest Increasing Subsequence
- Word Break
- House Robber

Day 3-4: Graphs (5 problems)
- Clone Graph
- Course Schedule
- Number of Islands
- Word Ladder
- Alien Dictionary

Day 5-7: Review & Practice
- Redo problems
- Optimize solutions
- Practice explaining
```

### Week 4: Final Practice & Mock Interviews
```
Day 1-3: Mixed Problems
- Random selection from all categories
- Timed practice (45 minutes per problem)
- Focus on optimization

Day 4-5: Mock Interviews
- 2 mock interviews
- 1 problem per interview
- Get feedback

Day 6-7: Review & Rest
- Review weak areas
- Light practice
- Get good sleep
```

---

## 🎯 Problem-Solving Strategy

### Step 1: Understand the Problem (5 minutes)
```
1. Read the problem carefully
2. Identify inputs and outputs
3. Ask clarifying questions:
   - What are the constraints?
   - What about edge cases?
   - What's the expected time complexity?
4. Provide examples
```

### Step 2: Discuss Approach (5 minutes)
```
1. Brainstorm approaches:
   - Brute force
   - Optimized approach
   - Trade-offs

2. Choose best approach:
   - Time complexity
   - Space complexity
   - Code simplicity

3. Explain your approach:
   - Data structures to use
   - Algorithm steps
   - Time and space complexity
```

### Step 3: Write Code (30-40 minutes)
```
1. Start coding:
   - Write clean, readable code
   - Use meaningful variable names
   - Add comments for complex logic

2. Handle edge cases:
   - Empty input
   - Single element
   - Large input
   - Null/None values

3. Test your solution:
   - Test with provided examples
   - Test with edge cases
   - Trace through your code
```

### Step 4: Optimize (5-10 minutes)
```
1. Analyze complexity:
   - Is there a better approach?
   - Can you reduce time complexity?
   - Can you reduce space complexity?

2. Implement optimization:
   - If time permits
   - If interviewer asks

3. Discuss trade-offs:
   - Speed vs Memory
   - Simplicity vs Optimization
```

### Step 5: Follow-up Questions
```
Be ready for:
- "Can you optimize this further?"
- "What if we had this constraint?"
- "How would you test this?"
- "What about error handling?"
- "How would you handle this edge case?"
```

---

## 💡 Common Patterns

### Pattern 1: Two Pointers
**Used for:** Arrays, strings, linked lists
**Problems:** Two Sum, Container With Most Water, Palindrome

```python
# Template
def twoPointers(arr):
    left, right = 0, len(arr) - 1
    while left < right:
        # Process
        if condition:
            left += 1
        else:
            right -= 1
    return result
```

### Pattern 2: Sliding Window
**Used for:** Substrings, subarrays
**Problems:** Longest Substring, Max Sliding Window

```python
# Template
def slidingWindow(s):
    left = 0
    window = {}
    for right in range(len(s)):
        # Add character to window
        window[s[right]] += 1
        
        # Shrink window if needed
        while condition:
            window[s[left]] -= 1
            left += 1
        
        # Update result
    return result
```

### Pattern 3: Binary Search
**Used for:** Sorted arrays, search problems
**Problems:** Binary Search, Search in Rotated Array

```python
# Template
def binarySearch(arr, target):
    left, right = 0, len(arr) - 1
    while left <= right:
        mid = (left + right) // 2
        if arr[mid] == target:
            return mid
        elif arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1
    return -1
```

### Pattern 4: DFS/BFS
**Used for:** Trees, graphs
**Problems:** Tree Traversal, Number of Islands

```python
# DFS Template
def dfs(node, visited):
    visited.add(node)
    for neighbor in node.neighbors:
        if neighbor not in visited:
            dfs(neighbor, visited)

# BFS Template
def bfs(start):
    queue = [start]
    visited = {start}
    while queue:
        node = queue.pop(0)
        for neighbor in node.neighbors:
            if neighbor not in visited:
                visited.add(neighbor)
                queue.append(neighbor)
```

### Pattern 5: Dynamic Programming
**Used for:** Optimization problems
**Problems:** Coin Change, Longest Subsequence

```python
# Template
def dp(n):
    dp = [0] * (n + 1)
    dp[0] = base_case
    for i in range(1, n + 1):
        dp[i] = recurrence_relation(dp[i-1], ...)
    return dp[n]
```

---

## 🔧 Code Quality Tips

### 1. Variable Naming
```python
# Bad
def solve(a):
    b = 0
    for c in a:
        b += c
    return b

# Good
def sum_array(numbers):
    total = 0
    for num in numbers:
        total += num
    return total
```

### 2. Comments
```python
# Bad - Too many comments
def solve(arr):
    # Initialize left pointer
    left = 0
    # Initialize right pointer
    right = len(arr) - 1
    # Loop while left < right
    while left < right:
        # Increment left
        left += 1

# Good - Comments for complex logic
def two_pointer_approach(arr):
    left, right = 0, len(arr) - 1
    while left < right:
        # Skip duplicates on left side
        while left < right and arr[left] == arr[left + 1]:
            left += 1
        left += 1
```

### 3. Error Handling
```python
# Good - Handle edge cases
def find_max(arr):
    if not arr:
        return None
    return max(arr)

def divide(a, b):
    if b == 0:
        raise ValueError("Division by zero")
    return a / b
```

### 4. Complexity Analysis
```python
# Always mention complexity
def two_sum(nums, target):
    """
    Time Complexity: O(n)
    Space Complexity: O(n)
    """
    seen = {}
    for num in nums:
        complement = target - num
        if complement in seen:
            return [seen[complement], nums.index(num)]
        seen[num] = nums.index(num)
    return []
```

---

## 📊 Practice Resources

### Online Platforms
1. **LeetCode** (https://leetcode.com)
   - 2000+ problems
   - Difficulty levels
   - Discussion forums
   - Premium features

2. **HackerRank** (https://www.hackerrank.com)
   - 500+ problems
   - Multiple languages
   - Tutorials
   - Contests

3. **CodeSignal** (https://codesignal.com)
   - 1000+ problems
   - Real interview questions
   - Skill assessments

4. **Pramp** (https://www.pramp.com)
   - Mock interviews
   - Real interviewer feedback
   - Free service

### Books
1. "Cracking the Coding Interview" by Gayle Laakmann McDowell
2. "Elements of Programming Interviews" by Adnan Aziz
3. "LeetCode Premium" (curated problem lists)

### YouTube Channels
1. NeetCode (https://www.youtube.com/c/NeetCode)
2. Striver (https://www.youtube.com/c/takeUforward)
3. Back To Back SWE (https://www.youtube.com/c/BackToBackSWE)

---

## 🎯 Practice Checklist

### Before Interview:
- [ ] Practice 30-40 problems
- [ ] Cover all categories
- [ ] Practice timed problems
- [ ] Do 3-5 mock interviews
- [ ] Review weak areas

### During Interview:
- [ ] Ask clarifying questions
- [ ] Think out loud
- [ ] Write clean code
- [ ] Test your solution
- [ ] Discuss complexity

### After Interview:
- [ ] Reflect on performance
- [ ] Note areas for improvement
- [ ] Continue practicing
- [ ] Get feedback

---

## 🚀 Next Steps

1. **Start with easy problems**
   - Build confidence
   - Learn patterns
   - Practice coding

2. **Progress to medium problems**
   - Apply patterns
   - Optimize solutions
   - Handle edge cases

3. **Practice hard problems**
   - Combine multiple concepts
   - Optimize for time/space
   - Practice explaining

4. **Do mock interviews**
   - Practice under pressure
   - Get feedback
   - Improve communication

---

**Created:** 2026-06-24
**Version:** 1.0
**Status:** Ready to Use

**Next Step:** Read 06_WELLS_FARGO_SPECIFIC.md

# Data Structures & Algorithms Study Guide
## For Medium and Hard LeetCode Problems

---

## Table of Contents
1. [Core Data Structures](#core-data-structures)
2. [Essential Algorithms](#essential-algorithms)
3. [Problem-Solving Patterns](#problem-solving-patterns)
4. [Complexity Analysis](#complexity-analysis)
5. [Key Techniques](#key-techniques)
6. [Practice Roadmap](#practice-roadmap)

---

## Core Data Structures

### 1. Arrays & Strings
**Key Concepts:**
- Two-pointer technique (fast/slow, left/right)
- Sliding window for subarray/substring problems
- Prefix sums for range queries
- Sorting and searching

**Common Patterns:**
```
Two Pointer:
- Reverse array/string
- Remove duplicates
- Container with most water
- Trapping rain water

Sliding Window:
- Longest substring without repeating characters
- Minimum window substring
- Maximum subarray of size k

Prefix Sum:
- Range sum queries
- Subarray sum equals k
```

**Must-Know Problems:**
- Merge sorted array
- Remove duplicates from sorted array
- Trapping rain water
- Longest substring without repeating characters
- Container with most water

---

### 2. Hash Tables / Hash Maps
**Key Concepts:**
- O(1) average lookup, insertion, deletion
- Collision handling (chaining, open addressing)
- When to use HashMap vs HashSet

**Common Patterns:**
```
Frequency Counting:
- Character/element frequency
- Find duplicates
- Majority element

Two-Sum Pattern:
- Complement lookup
- Pair finding problems

Grouping:
- Group anagrams
- Group by property
```

**Must-Know Problems:**
- Two Sum
- Group Anagrams
- Valid Anagram
- Majority Element
- LRU Cache (with HashMap + Doubly Linked List)

---

### 3. Linked Lists
**Key Concepts:**
- Pointer manipulation
- Dummy node technique
- Fast/slow pointers (cycle detection, finding middle)
- Reversal and merging

**Common Patterns:**
```
Fast/Slow Pointers:
- Detect cycle
- Find cycle start
- Find middle node
- Remove nth node from end

Reversal:
- Reverse entire list
- Reverse k-group
- Palindrome check

Merging:
- Merge two sorted lists
- Merge k sorted lists
```

**Must-Know Problems:**
- Reverse Linked List
- Detect Cycle in Linked List
- Merge Two Sorted Lists
- Remove Nth Node From End of List
- Palindrome Linked List
- LRU Cache

---

### 4. Stacks & Queues
**Key Concepts:**
- LIFO (Stack) vs FIFO (Queue)
- Monotonic stack/queue
- Using stack for recursion simulation
- Deque for sliding window maximum

**Common Patterns:**
```
Stack Applications:
- Expression evaluation
- Parentheses matching
- Next greater element
- Largest rectangle in histogram
- Trapping rain water

Queue Applications:
- BFS traversal
- Level-order traversal
- Sliding window maximum
- Task scheduling

Monotonic Stack:
- Maintain decreasing/increasing order
- Find next greater/smaller element
```

**Must-Know Problems:**
- Valid Parentheses
- Next Greater Element
- Largest Rectangle in Histogram
- Sliding Window Maximum
- Trapping Rain Water
- Basic Calculator

---

### 5. Trees
**Key Concepts:**
- Tree traversal (inorder, preorder, postorder, level-order)
- Binary Search Tree (BST) properties
- Balanced trees (AVL, Red-Black)
- Tree height and diameter
- LCA (Lowest Common Ancestor)

**Common Patterns:**
```
DFS Traversals:
- Inorder: Left -> Root -> Right (BST gives sorted)
- Preorder: Root -> Left -> Right (tree reconstruction)
- Postorder: Left -> Right -> Root (bottom-up problems)

BFS/Level-Order:
- Level-by-level processing
- Zigzag traversal
- Vertical order traversal

Path Problems:
- Root to leaf paths
- Path sum
- Maximum path sum
- Diameter of tree

BST Problems:
- Validate BST
- Lowest Common Ancestor
- Kth smallest element
- Serialize/Deserialize
```

**Must-Know Problems:**
- Binary Tree Inorder Traversal
- Level Order Traversal
- Lowest Common Ancestor
- Path Sum
- Maximum Path Sum
- Validate Binary Search Tree
- Serialize and Deserialize Binary Tree
- Binary Tree Right Side View

---

### 6. Graphs
**Key Concepts:**
- Adjacency list vs adjacency matrix
- DFS and BFS
- Topological sort
- Shortest path algorithms (Dijkstra, Bellman-Ford)
- Minimum Spanning Tree (Kruskal, Prim)
- Union-Find (Disjoint Set Union)

**Common Patterns:**
```
Graph Traversal:
- DFS: Stack-based, recursion
- BFS: Queue-based, level-by-level

Shortest Path:
- Dijkstra (non-negative weights)
- Bellman-Ford (negative weights)
- BFS (unweighted)

Connectivity:
- Union-Find for connected components
- DFS for cycle detection
- Topological sort for DAG

Special Graphs:
- Bipartite checking
- Strongly connected components
- Bridges and articulation points
```

**Must-Know Problems:**
- Number of Islands
- Clone Graph
- Course Schedule (Topological Sort)
- Dijkstra's Algorithm
- Word Ladder
- Alien Dictionary
- Network Delay Time
- Minimum Spanning Tree

---

### 7. Heaps (Priority Queues)
**Key Concepts:**
- Min-heap and max-heap
- Heapify operation
- Heap sort
- Top K problems

**Common Patterns:**
```
Top K Elements:
- Kth largest element
- K closest points
- Top K frequent elements

Merge K Sorted:
- Merge k sorted lists
- Merge k sorted arrays

Scheduling:
- Meeting rooms
- Task scheduling
```

**Must-Know Problems:**
- Kth Largest Element in Array
- Top K Frequent Elements
- Merge K Sorted Lists
- Meeting Rooms II
- Find Median from Data Stream

---

### 8. Tries (Prefix Trees)
**Key Concepts:**
- Prefix-based searching
- Space-time tradeoff
- Autocomplete and spell checking

**Common Patterns:**
```
Word Search:
- Search in dictionary
- Autocomplete
- Spell checker

Prefix Problems:
- Longest common prefix
- Implement Trie
```

**Must-Know Problems:**
- Implement Trie
- Word Search II
- Longest Word in Dictionary
- Design Search Autocomplete System

---

## Essential Algorithms

### 1. Sorting Algorithms
**Quick Reference:**

| Algorithm | Time (Avg) | Time (Worst) | Space | Stable |
|-----------|-----------|-------------|-------|--------|
| Quick Sort | O(n log n) | O(n²) | O(log n) | No |
| Merge Sort | O(n log n) | O(n log n) | O(n) | Yes |
| Heap Sort | O(n log n) | O(n log n) | O(1) | No |
| Bubble Sort | O(n²) | O(n²) | O(1) | Yes |
| Insertion Sort | O(n²) | O(n²) | O(1) | Yes |
| Counting Sort | O(n+k) | O(n+k) | O(k) | Yes |

**When to Use:**
- Quick Sort: General purpose, cache-friendly
- Merge Sort: When stability matters, guaranteed O(n log n)
- Heap Sort: When space is limited
- Counting Sort: Small range of integers

---

### 2. Searching Algorithms

**Binary Search:**
```
Conditions:
- Array must be sorted
- Time: O(log n)
- Space: O(1) iterative, O(log n) recursive

Variants:
- Find exact element
- Find first occurrence
- Find last occurrence
- Find insertion position
- Search in rotated array
```

**Must-Know Problems:**
- Binary Search
- Search in Rotated Sorted Array
- Find First and Last Position of Element
- Search Insert Position

---

### 3. Dynamic Programming
**Key Concepts:**
- Overlapping subproblems
- Optimal substructure
- Memoization (top-down) vs Tabulation (bottom-up)
- State definition and transitions

**Common Patterns:**

```
1D DP:
- Fibonacci
- Climbing stairs
- House robber
- Maximum subarray (Kadane's)

2D DP:
- Longest common subsequence
- Edit distance
- Unique paths
- Coin change

String DP:
- Longest palindromic subsequence
- Longest increasing subsequence
- Word break

Knapsack:
- 0/1 Knapsack
- Unbounded knapsack
- Partition equal subset sum
```

**Must-Know Problems:**
- Climbing Stairs
- House Robber
- Coin Change
- Longest Increasing Subsequence
- Edit Distance
- Longest Common Subsequence
- Unique Paths
- Word Break
- Longest Palindromic Subsequence
- Maximum Product Subarray

---

### 4. Greedy Algorithms
**Key Concepts:**
- Make locally optimal choices
- Hope for globally optimal solution
- Proof of correctness is important

**Common Applications:**
```
Interval Problems:
- Merge intervals
- Meeting rooms
- Interval scheduling

Activity Selection:
- Maximum non-overlapping intervals

Huffman Coding:
- Optimal prefix codes

Graph Problems:
- Minimum spanning tree (Kruskal)
- Dijkstra's algorithm
```

**Must-Know Problems:**
- Jump Game
- Candy Distribution
- Gas Station
- Merge Intervals
- Interval Scheduling Maximization

---

### 5. Backtracking
**Key Concepts:**
- Explore all possibilities
- Prune invalid branches
- Restore state after recursion

**Common Patterns:**
```
Permutations & Combinations:
- Generate all permutations
- Generate all combinations
- Subsets

Constraint Satisfaction:
- N-Queens
- Sudoku solver
- Word search

Partition Problems:
- Partition to K equal sum subsets
- Palindrome partitioning
```

**Must-Know Problems:**
- Permutations
- Combinations
- Subsets
- N-Queens
- Word Search
- Palindrome Partitioning
- Sudoku Solver

---

### 6. Graph Algorithms

**Depth-First Search (DFS):**
```
Time: O(V + E)
Space: O(V) for recursion stack

Applications:
- Topological sort
- Cycle detection
- Connected components
- Path finding
```

**Breadth-First Search (BFS):**
```
Time: O(V + E)
Space: O(V) for queue

Applications:
- Shortest path (unweighted)
- Level-order traversal
- Bipartite checking
- Connected components
```

**Dijkstra's Algorithm:**
```
Time: O((V + E) log V) with min-heap
Condition: Non-negative weights

Applications:
- Shortest path from source
- Network routing
```

**Topological Sort:**
```
Time: O(V + E)
Condition: DAG (Directed Acyclic Graph)

Applications:
- Course prerequisites
- Build system dependencies
- Task scheduling
```

**Union-Find (Disjoint Set Union):**
```
Time: O(α(n)) amortized, where α is inverse Ackermann
Space: O(n)

Applications:
- Connected components
- Cycle detection in undirected graphs
- Minimum spanning tree (Kruskal)
- Network connectivity
```

---

## Problem-Solving Patterns

### 1. Sliding Window
**When to Use:**
- Contiguous subarray/substring problems
- Fixed or variable window size

**Template:**
```
1. Define window boundaries (left, right)
2. Expand window by moving right
3. Contract window by moving left when condition met
4. Track result at each step
```

**Problems:**
- Longest substring without repeating characters
- Minimum window substring
- Maximum subarray of size k

---

### 2. Two Pointers
**When to Use:**
- Sorted arrays
- Palindrome checking
- Partition problems

**Variants:**
- Same direction (fast/slow)
- Opposite direction (left/right)
- One array, one pointer

**Problems:**
- Two Sum (sorted)
- Container with most water
- Trapping rain water
- Merge sorted array

---

### 3. Binary Search
**When to Use:**
- Sorted data
- Finding boundary conditions
- Optimization problems (binary search on answer)

**Template:**
```
left = 0, right = n - 1
while left <= right:
    mid = (left + right) // 2
    if condition(mid):
        right = mid - 1  // or left = mid + 1
    else:
        left = mid + 1   // or right = mid - 1
```

**Problems:**
- Search in rotated sorted array
- Find first/last occurrence
- Binary search on answer

---

### 4. DFS/Backtracking
**When to Use:**
- Permutations, combinations, subsets
- Constraint satisfaction
- Path finding

**Template:**
```
def dfs(path, remaining):
    if base_case(remaining):
        result.append(path)
        return
    
    for choice in remaining:
        path.append(choice)
        dfs(path, remaining - choice)
        path.pop()  # backtrack
```

**Problems:**
- Generate permutations
- Subsets
- N-Queens
- Word search

---

### 5. BFS
**When to Use:**
- Shortest path (unweighted)
- Level-order problems
- Connected components

**Template:**
```
queue = deque([start])
visited.add(start)

while queue:
    node = queue.popleft()
    for neighbor in node.neighbors:
        if neighbor not in visited:
            visited.add(neighbor)
            queue.append(neighbor)
```

**Problems:**
- Number of islands
- Shortest path in grid
- Level order traversal

---

### 6. Dynamic Programming
**When to Use:**
- Overlapping subproblems
- Optimal substructure

**Steps:**
1. Define state: dp[i] = meaning
2. Find recurrence relation
3. Base case
4. Compute bottom-up or top-down

**Problems:**
- Climbing stairs
- House robber
- Coin change
- Edit distance

---

## Complexity Analysis

### Time Complexity

**Common Complexities (from fastest to slowest):**
```
O(1)        - Constant
O(log n)    - Logarithmic (binary search)
O(n)        - Linear (single loop)
O(n log n)  - Linearithmic (sorting, divide & conquer)
O(n²)       - Quadratic (nested loops)
O(n³)       - Cubic (triple nested loops)
O(2ⁿ)       - Exponential (subsets, permutations)
O(n!)       - Factorial (all permutations)
```

**How to Analyze:**
1. Count loops and their iterations
2. Identify nested operations
3. Drop constants and lower-order terms
4. Consider best, average, and worst cases

---

### Space Complexity

**Common Scenarios:**
```
O(1)        - Constant space (few variables)
O(n)        - Linear (array, list, hash map)
O(log n)    - Recursion depth (binary search)
O(n)        - Recursion depth (DFS on tree)
O(n²)       - 2D array
```

**Recursion Stack:**
- Depth of recursion = space used
- Binary search: O(log n)
- DFS on n-node tree: O(h) where h is height

---

## Key Techniques

### 1. Memoization (Top-Down DP)
```python
def fib(n, memo={}):
    if n in memo:
        return memo[n]
    if n <= 1:
        return n
    memo[n] = fib(n-1, memo) + fib(n-2, memo)
    return memo[n]
```

**Advantages:**
- Natural recursive thinking
- Only compute needed subproblems
- Easier to understand

---

### 2. Tabulation (Bottom-Up DP)
```python
def fib(n):
    if n <= 1:
        return n
    dp = [0] * (n + 1)
    dp[1] = 1
    for i in range(2, n + 1):
        dp[i] = dp[i-1] + dp[i-2]
    return dp[n]
```

**Advantages:**
- No recursion overhead
- Guaranteed to avoid stack overflow
- Can optimize space

---

### 3. Monotonic Stack/Queue
**Use Case:** Find next/previous greater/smaller element

```python
# Next Greater Element
stack = []
result = [0] * len(nums)

for i in range(len(nums) - 1, -1, -1):
    while stack and stack[-1] <= nums[i]:
        stack.pop()
    result[i] = stack[-1] if stack else -1
    stack.append(nums[i])
```

---

### 4. Union-Find
```python
class UnionFind:
    def __init__(self, n):
        self.parent = list(range(n))
        self.rank = [0] * n
    
    def find(self, x):
        if self.parent[x] != x:
            self.parent[x] = self.find(self.parent[x])  # path compression
        return self.parent[x]
    
    def union(self, x, y):
        px, py = self.find(x), self.find(y)
        if px == py:
            return False
        # union by rank
        if self.rank[px] < self.rank[py]:
            px, py = py, px
        self.parent[py] = px
        if self.rank[px] == self.rank[py]:
            self.rank[px] += 1
        return True
```

---

### 5. Segment Tree (Advanced)
**Use Case:** Range queries and updates

**Operations:**
- Point update: O(log n)
- Range query: O(log n)
- Build: O(n)

---

### 6. Trie
```python
class TrieNode:
    def __init__(self):
        self.children = {}
        self.is_end = False

class Trie:
    def __init__(self):
        self.root = TrieNode()
    
    def insert(self, word):
        node = self.root
        for char in word:
            if char not in node.children:
                node.children[char] = TrieNode()
            node = node.children[char]
        node.is_end = True
    
    def search(self, word):
        node = self.root
        for char in word:
            if char not in node.children:
                return False
            node = node.children[char]
        return node.is_end
```

---

## Practice Roadmap

### Phase 1: Foundation (1-2 weeks)
**Focus:** Master basic data structures and simple algorithms

**Topics:**
- Arrays and strings (2-pointer, sliding window)
- Hash maps
- Basic sorting and searching
- Stacks and queues

**Target:** 30-40 easy problems
**Goal:** Comfortable with fundamentals

---

### Phase 2: Intermediate (2-3 weeks)
**Focus:** Trees, graphs, and basic DP

**Topics:**
- Tree traversals and problems
- Graph basics (DFS, BFS)
- Introduction to DP
- Backtracking basics

**Target:** 40-50 medium problems
**Goal:** Recognize problem patterns

---

### Phase 3: Advanced (3-4 weeks)
**Focus:** Complex algorithms and optimization

**Topics:**
- Advanced DP (2D, string, interval)
- Graph algorithms (Dijkstra, topological sort)
- Advanced backtracking
- Greedy algorithms
- Union-Find

**Target:** 30-40 hard problems
**Goal:** Solve complex problems efficiently

---

### Phase 4: Mastery (Ongoing)
**Focus:** Optimization and edge cases

**Topics:**
- Segment trees, Fenwick trees
- Advanced graph problems
- System design patterns
- Competitive programming techniques

**Target:** 20-30 hard problems
**Goal:** Solve in optimal time/space

---

## Problem-Solving Checklist

Before coding:
- [ ] Understand the problem completely
- [ ] Identify input/output constraints
- [ ] Think of edge cases
- [ ] Determine time/space requirements
- [ ] Choose appropriate data structure
- [ ] Outline algorithm before coding

While coding:
- [ ] Write clean, readable code
- [ ] Handle edge cases
- [ ] Add comments for complex logic
- [ ] Test with examples

After coding:
- [ ] Verify with test cases
- [ ] Check edge cases
- [ ] Analyze time/space complexity
- [ ] Consider optimizations
- [ ] Review for bugs

---

## Quick Reference: When to Use What

| Problem Type | Data Structure | Algorithm |
|--------------|----------------|-----------|
| Frequency counting | Hash Map | - |
| Top K elements | Heap | Heap sort |
| Shortest path (unweighted) | Queue | BFS |
| Shortest path (weighted) | Heap + Graph | Dijkstra |
| Sorted sequence | BST | - |
| Prefix search | Trie | DFS |
| Permutations/Combinations | - | Backtracking |
| Overlapping subproblems | - | DP |
| Interval problems | Sorted array | Greedy/DP |
| Connected components | Union-Find | - |
| Tree problems | Tree | DFS/BFS |
| String matching | Trie/Hash | KMP/Rabin-Karp |

---

## Common Mistakes to Avoid

1. **Off-by-one errors** - Careful with loop boundaries
2. **Not handling edge cases** - Empty arrays, single elements, null pointers
3. **Integer overflow** - Use long for large numbers
4. **Modifying while iterating** - Create new structures when needed
5. **Not resetting state** - In backtracking, always backtrack
6. **Wrong complexity analysis** - Count all operations
7. **Inefficient string concatenation** - Use StringBuilder
8. **Not using appropriate data structures** - Choose wisely for efficiency
9. **Forgetting base cases** - In recursion and DP
10. **Not testing thoroughly** - Test edge cases and large inputs

---

## Resources for Practice

**Websites:**
- LeetCode (problems and discussions)
- HackerRank (tutorials and problems)
- GeeksforGeeks (explanations)
- InterviewBit (curated problems)

**Books:**
- "Cracking the Coding Interview" by Gayle Laakmann McDowell
- "Introduction to Algorithms" by CLRS
- "Algorithm Design Manual" by Steven Skiena

**YouTube Channels:**
- NeetCode
- Abdul Bari
- Striver
- TakeUForward

---

## Study Tips

1. **Understand before memorizing** - Know why algorithms work
2. **Code by hand first** - Understand logic before typing
3. **Review solutions** - Learn different approaches
4. **Track patterns** - Note similar problem types
5. **Solve regularly** - Consistency beats intensity
6. **Time yourself** - Practice under pressure
7. **Explain out loud** - Solidify understanding
8. **Teach others** - Best way to learn
9. **Revisit mistakes** - Learn from errors
10. **Build intuition** - Recognize patterns quickly

---

## Final Notes

- **Start easy, go hard gradually** - Don't jump to hard problems
- **Quality over quantity** - Understand deeply, not just solve
- **Consistency matters** - 1 hour daily > 7 hours once a week
- **Review regularly** - Spaced repetition helps retention
- **Stay motivated** - Track progress, celebrate wins
- **Practice interview scenarios** - Explain while coding

Good luck with your preparation! Remember, the goal is to develop problem-solving intuition, not just memorize solutions.

# Dynamic Programming

## Overview
Dynamic Programming is a powerful technique for solving optimization problems by breaking them into overlapping subproblems.

## Core Concepts

### Principles
1. **Optimal Substructure**: Optimal solution contains optimal solutions to subproblems
2. **Overlapping Subproblems**: Same subproblems solved multiple times
3. **Memoization**: Cache results to avoid recomputation

### Approaches
1. **Top-Down (Memoization)**: Recursion with caching
   - Time: O(states × transitions)
   - Space: O(states) + O(recursion depth)
2. **Bottom-Up (Tabulation)**: Iterative with DP table
   - Time: O(states × transitions)
   - Space: O(states)

## Classic DP Problems

### 1. Fibonacci Sequence
```
Recurrence: F(n) = F(n-1) + F(n-2)
Base: F(0) = 0, F(1) = 1
Time: O(n), Space: O(1) optimized
```

### 2. Climbing Stairs
```
Problem: Reach nth stair, can climb 1 or 2 steps
Recurrence: dp[i] = dp[i-1] + dp[i-2]
Time: O(n), Space: O(1) optimized
```

### 3. House Robber
```
Problem: Rob houses, can't rob adjacent
Recurrence: dp[i] = max(dp[i-1], dp[i-2] + nums[i])
Time: O(n), Space: O(1) optimized
```

### 4. Coin Change
```
Problem: Minimum coins for amount
Recurrence: dp[i] = min(dp[i - coin] + 1) for all coins
Time: O(n × m), Space: O(n)
```

### 5. Longest Increasing Subsequence (LIS)
```
Problem: Longest strictly increasing subsequence
Recurrence: dp[i] = max(dp[j] + 1) for all j < i where arr[j] < arr[i]
Time: O(n²) or O(n log n) with binary search
Space: O(n)
```

### 6. Edit Distance (Levenshtein)
```
Problem: Minimum operations to transform string
Operations: Insert, Delete, Replace
Recurrence: 
  If s1[i] == s2[j]: dp[i][j] = dp[i-1][j-1]
  Else: dp[i][j] = 1 + min(dp[i-1][j], dp[i][j-1], dp[i-1][j-1])
Time: O(m × n), Space: O(m × n)
```

### 7. Longest Common Subsequence (LCS)
```
Problem: Longest common subsequence of two strings
Recurrence:
  If s1[i] == s2[j]: dp[i][j] = dp[i-1][j-1] + 1
  Else: dp[i][j] = max(dp[i-1][j], dp[i][j-1])
Time: O(m × n), Space: O(m × n)
```

### 8. 0/1 Knapsack
```
Problem: Maximize value with weight constraint
Recurrence: dp[i][w] = max(dp[i-1][w], dp[i-1][w-weight[i]] + value[i])
Time: O(n × W), Space: O(n × W) or O(W) optimized
```

### 9. Matrix Chain Multiplication
```
Problem: Minimum operations to multiply matrices
Recurrence: dp[i][j] = min(dp[i][k] + dp[k+1][j] + cost) for all k
Time: O(n³), Space: O(n²)
```

### 10. Longest Palindromic Subsequence
```
Problem: Longest palindromic subsequence in string
Recurrence:
  If s[i] == s[j]: dp[i][j] = dp[i+1][j-1] + 2
  Else: dp[i][j] = max(dp[i+1][j], dp[i][j-1])
Time: O(n²), Space: O(n²)
```

## DP Problem Categories

### 1D DP
- Single variable state
- Examples: Fibonacci, Climbing Stairs, House Robber
- Space: O(n), can optimize to O(1)

### 2D DP
- Two variable state
- Examples: Edit Distance, LCS, Knapsack
- Space: O(m × n), sometimes optimizable to O(n)

### Interval DP
- Subproblems defined by intervals
- Examples: Matrix Chain Multiplication, Burst Balloons
- Space: O(n²)

### Tree DP
- Subproblems on tree nodes
- Examples: House Robber III, Maximum Path Sum
- Space: O(n) for recursion

### Digit DP
- Subproblems on digit positions
- Examples: Count numbers with specific properties
- Space: O(10 × length × states)

### Bitmask DP
- State represented as bitmask
- Examples: Traveling Salesman Problem, Subset problems
- Space: O(2^n × n)

## Advanced DP Techniques

### Memoization
```
Store results of function calls
Avoid recomputation of same subproblems
Trade space for time
```

### Space Optimization
```
Use rolling arrays for 2D DP
Keep only necessary previous states
Reduce space from O(m×n) to O(n)
```

### Transition Optimization
```
Convex Hull Trick: Optimize linear DP transitions
Divide and Conquer Optimization: Reduce transitions
Monotonic Queue: Optimize range queries
```

## Must-Know DP Problems

1. **Unique Paths** - Grid traversal
2. **Unique Paths II** - With obstacles
3. **Minimum Path Sum** - Minimum cost path
4. **Jump Game** - Can reach end
5. **Jump Game II** - Minimum jumps
6. **Best Time to Buy and Sell Stock** - Max profit
7. **Best Time to Buy and Sell Stock III** - At most 2 transactions
8. **Best Time to Buy and Sell Stock IV** - At most k transactions
9. **Palindrome Partitioning II** - Minimum cuts
10. **Word Break** - Can form from dictionary
11. **Word Break II** - All possible combinations
12. **Decode Ways** - Number of ways to decode
13. **Partition Equal Subset Sum** - Can partition
14. **Target Sum** - Ways to achieve target
15. **Burst Balloons** - Maximum coins

## DP State Definition

### Guidelines
1. **Clear Definition**: What does dp[i] or dp[i][j] represent?
2. **Base Cases**: Initialize for smallest subproblems
3. **Transitions**: How to compute from smaller subproblems
4. **Answer**: Which state contains the final answer?

### Example: Coin Change
```
dp[i] = minimum coins needed for amount i
Base: dp[0] = 0
Transition: dp[i] = min(dp[i - coin] + 1) for all coins ≤ i
Answer: dp[amount]
```

## Interview Tips
- Identify if problem has optimal substructure
- Define DP state clearly
- Start with brute force, then optimize
- Consider both top-down and bottom-up
- Optimize space when possible
- Test with examples before coding
- Handle edge cases carefully

## Common Mistakes
- **Wrong state definition**: Leads to incorrect transitions
- **Missing base cases**: Results in infinite recursion
- **Incorrect transitions**: Doesn't capture problem correctly
- **Not memoizing**: Exponential time complexity
- **Off-by-one errors**: Common in indexing
- **Not considering all cases**: Missing some transitions

## Complexity Analysis

| Problem | Time | Space |
|---------|------|-------|
| Fibonacci | O(n) | O(n) |
| Climbing Stairs | O(n) | O(1) |
| Coin Change | O(n×m) | O(n) |
| LIS | O(n log n) | O(n) |
| Edit Distance | O(m×n) | O(m×n) |
| LCS | O(m×n) | O(m×n) |
| Knapsack | O(n×W) | O(W) |
| Matrix Chain | O(n³) | O(n²) |

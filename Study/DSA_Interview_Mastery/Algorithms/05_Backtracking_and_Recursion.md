# Backtracking and Recursion

## Overview
Backtracking is a technique for solving problems by exploring all possible solutions and abandoning a path when it doesn't lead to a solution.

## Recursion Fundamentals

### Key Components
1. **Base Case**: Condition to stop recursion
2. **Recursive Case**: Problem reduced to smaller subproblem
3. **Progress**: Each recursive call moves toward base case

### Time and Space Complexity
- **Time**: Number of recursive calls × work per call
- **Space**: Recursion depth (call stack)

### Common Patterns
1. **Linear Recursion**: Single recursive call
   - Time: O(n), Space: O(n)
2. **Binary Recursion**: Two recursive calls
   - Time: O(2^n), Space: O(n)
3. **Multiple Recursion**: k recursive calls
   - Time: O(k^n), Space: O(n)

## Backtracking Technique

### Algorithm Structure
```
function backtrack(path, choices):
    if isGoal(path):
        result.add(path)
        return
    
    for choice in choices:
        if isValid(choice):
            path.add(choice)
            backtrack(path, remainingChoices)
            path.remove(choice)  // Backtrack
```

### Key Concepts
1. **Choice**: What options are available
2. **Constraints**: What makes a choice valid
3. **Goal**: When to stop and record solution
4. **Backtrack**: Undo choice and try next

### Time Complexity
- **Worst Case**: O(n!) for permutations, O(2^n) for subsets
- **Pruning**: Can reduce significantly with good constraints

## Must-Know Backtracking Problems

### 1. Permutations
```
Problem: Generate all permutations of array
Approach: Swap elements, recurse, swap back
Time: O(n! × n), Space: O(n)
```

### 2. Combinations
```
Problem: Generate all k-combinations
Approach: Include/exclude elements, recurse
Time: O(C(n,k) × k), Space: O(k)
```

### 3. Subsets
```
Problem: Generate all subsets (power set)
Approach: Include/exclude each element
Time: O(2^n × n), Space: O(n)
```

### 4. N-Queens
```
Problem: Place n queens on n×n board, no attacks
Approach: Place queen row by row, check constraints
Time: O(n!), Space: O(n)
Optimization: Pruning with column/diagonal tracking
```

### 5. Sudoku Solver
```
Problem: Fill empty cells to complete sudoku
Approach: Try digits 1-9, check constraints, backtrack
Time: O(9^(n²)) worst, much better with pruning
Space: O(n²)
```

### 6. Word Search
```
Problem: Find word in 2D grid
Approach: DFS from each cell, mark visited, backtrack
Time: O(n × m × 4^L) where L is word length
Space: O(L) for recursion
```

### 7. Palindrome Partitioning
```
Problem: Partition string into palindromes
Approach: Try each partition point, recurse
Time: O(2^n × n), Space: O(n)
```

### 8. Letter Combinations of Phone Number
```
Problem: Generate all letter combinations for digits
Approach: Map digits to letters, recurse
Time: O(4^n × n), Space: O(n)
```

### 9. Generate Parentheses
```
Problem: Generate valid parentheses combinations
Approach: Track open/close counts, recurse
Time: O(4^n / √n), Space: O(n)
```

### 10. Restore IP Addresses
```
Problem: Restore valid IP addresses from string
Approach: Try each segment, validate, recurse
Time: O(3^4 × n), Space: O(n)
```

## Backtracking Optimization Techniques

### 1. Pruning
```
Eliminate branches that can't lead to solution
Example: N-Queens - skip if column/diagonal occupied
Reduces time complexity significantly
```

### 2. Memoization
```
Cache results to avoid recomputation
Example: Combination sum with memoization
Trade space for time
```

### 3. Early Termination
```
Stop when solution found (if only one needed)
Example: Word search - return true immediately
```

### 4. Constraint Propagation
```
Reduce search space by enforcing constraints
Example: Sudoku - track possible values per cell
```

## Recursion vs Iteration

| Aspect | Recursion | Iteration |
|--------|-----------|-----------|
| Code | Cleaner, more intuitive | More verbose |
| Space | O(depth) for call stack | O(1) usually |
| Time | Same (with memoization) | Same |
| Stack Overflow | Risk with deep recursion | No risk |
| Tail Call | Optimizable | N/A |

## Common Recursion Patterns

### 1. Tree Traversal
```
function traverse(node):
    if node is null: return
    process(node)
    traverse(node.left)
    traverse(node.right)
```

### 2. Array Processing
```
function process(arr, index):
    if index == len(arr): return
    process(arr[index])
    process(arr, index + 1)
```

### 3. Divide and Conquer
```
function solve(problem):
    if baseCase: return solution
    left = solve(leftHalf)
    right = solve(rightHalf)
    return combine(left, right)
```

### 4. Backtracking
```
function backtrack(path, choices):
    if goal: record solution
    for choice in choices:
        path.add(choice)
        backtrack(path, remaining)
        path.remove(choice)
```

## Interview Tips
- Clearly define base case and recursive case
- Visualize recursion tree for small examples
- Identify when to use backtracking vs DP
- Implement pruning to reduce search space
- Test with edge cases
- Be aware of stack overflow risk
- Consider iterative solution if recursion too deep

## Common Mistakes
- **Missing base case**: Infinite recursion
- **Wrong base case**: Incorrect results
- **Not backtracking**: Incorrect state management
- **Inefficient pruning**: Exploring unnecessary branches
- **Stack overflow**: Too deep recursion
- **Duplicate solutions**: Not handling duplicates properly

## Complexity Analysis

| Problem | Time | Space |
|---------|------|-------|
| Permutations | O(n! × n) | O(n) |
| Combinations | O(C(n,k) × k) | O(k) |
| Subsets | O(2^n × n) | O(n) |
| N-Queens | O(n!) | O(n) |
| Sudoku | O(9^(n²)) | O(n²) |
| Word Search | O(n × m × 4^L) | O(L) |
| Palindrome Part | O(2^n × n) | O(n) |

## Optimization Techniques Summary

1. **Pruning**: Eliminate invalid branches early
2. **Memoization**: Cache computed results
3. **Early Termination**: Stop when solution found
4. **Constraint Propagation**: Reduce search space
5. **Iterative Deepening**: Combine benefits of BFS and DFS
6. **Branch and Bound**: Prune with upper/lower bounds

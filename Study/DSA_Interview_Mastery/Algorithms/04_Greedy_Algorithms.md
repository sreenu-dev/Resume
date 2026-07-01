# Greedy Algorithms

## Overview
Greedy algorithms make locally optimal choices at each step, hoping to find a global optimum. They're efficient but don't always guarantee the best solution.

## Core Concept

### Greedy Choice Property
- Making locally optimal choice leads to globally optimal solution
- No need to reconsider previous choices
- Works for specific problem structures

### Optimal Substructure
- Optimal solution contains optimal solutions to subproblems
- Similar to DP but greedy makes choice without exploring alternatives

## When to Use Greedy

### Characteristics of Greedy Problems
1. **Greedy Choice Property**: Local optimum leads to global optimum
2. **Optimal Substructure**: Problem has optimal substructure
3. **No Backtracking**: Decisions are final
4. **Efficiency**: Usually O(n log n) or O(n)

### When NOT to Use Greedy
- Problem requires considering all possibilities
- Local optimum doesn't guarantee global optimum
- Need to explore multiple paths

## Classic Greedy Problems

### 1. Activity Selection
```
Problem: Select maximum non-overlapping activities
Approach: Sort by end time, greedily select
Time: O(n log n)
Proof: Exchange argument shows greedy is optimal
```

### 2. Huffman Coding
```
Problem: Build optimal prefix-free code
Approach: Repeatedly merge two smallest frequency nodes
Time: O(n log n)
Use: Data compression
```

### 3. Fractional Knapsack
```
Problem: Maximize value with weight constraint (can take fractions)
Approach: Sort by value/weight ratio, greedily take items
Time: O(n log n)
Note: 0/1 knapsack requires DP
```

### 4. Interval Scheduling
```
Problem: Select maximum non-overlapping intervals
Approach: Sort by end time, greedily select
Time: O(n log n)
Proof: Exchange argument
```

### 5. Interval Partitioning
```
Problem: Minimum resources to cover all intervals
Approach: Sort by start time, use priority queue
Time: O(n log n)
```

### 6. Minimum Spanning Tree (Kruskal's)
```
Problem: Find MST of weighted graph
Approach: Sort edges, greedily add non-cycle edges
Time: O(E log E)
Data Structure: Union-Find
```

### 7. Dijkstra's Algorithm
```
Problem: Shortest path from source
Approach: Greedily select closest unvisited vertex
Time: O((V + E) log V) with min-heap
Limitation: No negative weights
```

### 8. Prim's Algorithm
```
Problem: Find MST
Approach: Greedily grow tree from starting vertex
Time: O((V + E) log V) with min-heap
```

### 9. Topological Sort (Kahn's)
```
Problem: Order vertices in DAG
Approach: Greedily remove vertices with in-degree 0
Time: O(V + E)
```

### 10. Job Sequencing with Deadlines
```
Problem: Maximize profit with deadline constraints
Approach: Sort by profit, greedily assign to latest available slot
Time: O(n²) or O(n log n) with better data structure
```

## Must-Know Greedy Interview Problems

1. **Jump Game**
   - Can reach last index
   - Greedy: Track maximum reachable position
   - Time: O(n)

2. **Jump Game II**
   - Minimum jumps to reach last index
   - Greedy: Track farthest reachable in current jump
   - Time: O(n)

3. **Gas Station**
   - Can complete circular route
   - Greedy: Track current gas, try each starting point
   - Time: O(n)

4. **Candy**
   - Distribute minimum candies with constraints
   - Greedy: Two passes (left-to-right, right-to-left)
   - Time: O(n)

5. **Lemonade Change**
   - Can make change for all customers
   - Greedy: Track bills, prefer $5 change
   - Time: O(n)

6. **Meeting Rooms**
   - Minimum meeting rooms needed
   - Greedy: Sort by start time, use priority queue
   - Time: O(n log n)

7. **Reorganize String**
   - Rearrange so no adjacent characters are same
   - Greedy: Max heap by frequency
   - Time: O(n log n)

8. **Assign Cookies**
   - Maximize satisfied children
   - Greedy: Two pointers, match smallest to smallest
   - Time: O(n log n)

9. **Boats to Save People**
   - Minimum boats with weight limit
   - Greedy: Two pointers, pair heaviest with lightest
   - Time: O(n log n)

10. **Task Scheduler**
    - Minimum time with cooldown
    - Greedy: Max heap by frequency, simulate
    - Time: O(n log n)

## Greedy Strategies

### 1. Sorting-Based
```
Sort by some criteria, then process in order
Examples: Activity Selection, Interval Scheduling
Time: O(n log n)
```

### 2. Exchange Argument
```
Prove greedy is optimal by showing any non-greedy solution
can be transformed to greedy without losing optimality
```

### 3. Matroid Theory
```
Problem has matroid structure if greedy works
Useful for proving correctness
```

### 4. Greedy with Data Structures
```
Use heap, queue, or other structures to efficiently
select next greedy choice
Examples: Dijkstra's, Prim's, Huffman
```

## Proof Techniques

### Exchange Argument
1. Assume optimal solution differs from greedy
2. Show how to exchange elements without losing optimality
3. Repeat until optimal matches greedy

### Induction
1. Base case: Greedy optimal for small instances
2. Inductive step: If greedy optimal for k, then optimal for k+1

### Contradiction
1. Assume greedy is not optimal
2. Derive contradiction
3. Conclude greedy must be optimal

## Greedy vs DP

| Aspect | Greedy | DP |
|--------|--------|-----|
| Approach | Local optimum | Global optimum |
| Correctness | Not always | Always (if correct) |
| Time | Usually O(n log n) | Usually O(n²) or more |
| Space | Usually O(1) | Usually O(n) or more |
| Proof | Exchange/Induction | Optimal substructure |
| Use | When proven correct | When greedy fails |

## Interview Tips
- Always prove greedy works before implementing
- Use exchange argument or induction
- Consider counterexamples
- Implement efficiently with proper data structures
- Test edge cases
- Be prepared to explain why greedy works

## Common Mistakes
- **Assuming greedy always works**: Verify with counterexamples
- **Wrong sorting criterion**: Ensure sorting matches problem
- **Not handling ties**: Consider how to break ties
- **Inefficient implementation**: Use appropriate data structures
- **Missing edge cases**: Empty input, single element, etc.

## Complexity Comparison

| Problem | Greedy Time | DP Time | Better |
|---------|-------------|---------|--------|
| Activity Selection | O(n log n) | O(n²) | Greedy |
| Fractional Knapsack | O(n log n) | N/A | Greedy |
| 0/1 Knapsack | N/A | O(nW) | DP |
| Interval Scheduling | O(n log n) | O(n²) | Greedy |
| Coin Change | O(n log n)* | O(nC) | DP |

*Greedy works only for certain coin systems

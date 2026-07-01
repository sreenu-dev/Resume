# Heaps and Priority Queues

## Overview
Heaps are complete binary trees that maintain a specific ordering property. They're essential for efficient priority queue operations.

## Heap Basics

### Types of Heaps
1. **Min Heap**: Parent ≤ Children (smallest at root)
2. **Max Heap**: Parent ≥ Children (largest at root)

### Properties
- **Complete Binary Tree**: All levels filled except possibly last
- **Heap Property**: Parent-child ordering maintained
- **Array Representation**: 
  - Parent at index i
  - Left child at 2i + 1
  - Right child at 2i + 2

### Time Complexity
- **Insert**: O(log n)
- **Delete Min/Max**: O(log n)
- **Peek Min/Max**: O(1)
- **Heapify**: O(n)
- **Heap Sort**: O(n log n)

### Space Complexity
- **Storage**: O(n)
- **Auxiliary**: O(1) for in-place operations

## Heap Operations

### Insert (Bubble Up)
```
1. Add element at end
2. Compare with parent
3. Swap if violates heap property
4. Repeat until valid
Time: O(log n)
```

### Delete Min/Max (Bubble Down)
```
1. Remove root
2. Move last element to root
3. Compare with children
4. Swap with smaller/larger child
5. Repeat until valid
Time: O(log n)
```

### Heapify
```
Build heap from array in O(n) time
Process non-leaf nodes from bottom-up
Each node bubbles down to correct position
```

## Priority Queue

### Definition
- Abstract data type with priority-based ordering
- Elements with higher priority dequeued first
- Typically implemented using heaps

### Operations
- **Enqueue with Priority**: O(log n)
- **Dequeue (highest priority)**: O(log n)
- **Peek**: O(1)

### Applications
- Task scheduling
- Dijkstra's shortest path
- Huffman coding
- Load balancing
- Event simulation

## Must-Know Heap Problems

1. **Kth Largest Element**
   - Min heap of size k
   - Time: O(n log k)
   - Space: O(k)

2. **Top K Frequent Elements**
   - Max heap or bucket sort
   - Time: O(n log k)
   - Space: O(k)

3. **Merge K Sorted Lists**
   - Min heap with list heads
   - Time: O(n log k) where n is total elements
   - Space: O(k)

4. **Find Median from Data Stream**
   - Max heap for smaller half, min heap for larger half
   - Time: O(log n) insert, O(1) find median
   - Space: O(n)

5. **Reorganize String**
   - Max heap by frequency
   - Time: O(n log n)
   - Space: O(n)

6. **Furthest Building You Can Reach**
   - Max heap for ladder choices
   - Time: O(n log n)
   - Space: O(n)

7. **Sliding Window Maximum**
   - Max heap with lazy deletion
   - Time: O(n log n)
   - Space: O(n)

8. **IPO (Capital Allocation)**
   - Max heap for profits
   - Time: O(n log n)
   - Space: O(n)

9. **Minimum Cost to Connect Sticks**
   - Min heap for greedy selection
   - Time: O(n log n)
   - Space: O(n)

10. **Task Scheduler**
    - Max heap with cooldown
    - Time: O(n log n)
    - Space: O(n)

## Heap Variations

### Fibonacci Heap
- **Operations**: O(1) amortized for most operations
- **Use**: Advanced algorithms (Dijkstra's, Prim's)
- **Trade-off**: Complex implementation

### Leftist Heap
- **Property**: Left subtree always at least as heavy as right
- **Merge**: O(log n)
- **Use**: Mergeable priority queues

### Binomial Heap
- **Structure**: Collection of binomial trees
- **Merge**: O(log n)
- **Use**: Mergeable priority queues

### Skew Heap
- **Simpler**: Simplified leftist heap
- **Merge**: O(log n) amortized
- **Use**: Mergeable priority queues

## Heap vs Other Data Structures

| Feature | Heap | BST | Sorted Array |
|---------|------|-----|--------------|
| Insert | O(log n) | O(log n) | O(n) |
| Delete Min | O(log n) | O(log n) | O(1) |
| Find Min | O(1) | O(log n) | O(1) |
| Find Median | O(n) | O(log n) | O(1) |
| Ordered Traversal | O(n log n) | O(n) | O(n) |
| Space | O(n) | O(n) | O(n) |

## Interview Tips
- Recognize when to use min vs max heap
- Understand heap property and array representation
- Know when heap is better than sorting
- Practice heapify operation
- Consider space constraints
- Be familiar with language-specific heap implementations
- Understand trade-offs with other data structures

## Common Patterns

### K Largest Elements
```
Min heap of size k
Iterate through array
If element > heap top, remove top and add element
Final heap contains k largest
```

### Merge K Sorted Lists
```
Min heap with list node comparators
Add head of each list to heap
While heap not empty:
  - Pop min node
  - Add to result
  - If node has next, add next to heap
```

### Find Median
```
Max heap for first half
Min heap for second half
Maintain size difference ≤ 1
Median is top of larger heap or average of tops
```

## Implementation Notes
- Use language-specific PriorityQueue when available
- Implement custom comparators for complex objects
- Consider thread safety for concurrent access
- Monitor heap size for memory constraints
- Understand lazy deletion vs actual deletion

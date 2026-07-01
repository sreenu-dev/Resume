# Stacks and Queues

## Overview
Stacks and queues are fundamental abstract data types used in many real-world applications and interview problems.

## Stacks (LIFO - Last In First Out)

### Key Concepts
- **Operations**:
  - Push: O(1)
  - Pop: O(1)
  - Peek: O(1)
- **Space Complexity**: O(n)
- **Use Cases**: Function call stack, undo/redo, expression evaluation, DFS

### Common Stack Problems
1. **Valid Parentheses** - Match opening/closing brackets
2. **Evaluate Reverse Polish Notation** - Stack-based calculation
3. **Daily Temperatures** - Monotonic stack
4. **Largest Rectangle in Histogram** - Monotonic stack
5. **Trapping Rain Water II** - Stack with heights
6. **Decode String** - Nested decoding with stack
7. **Next Greater Element** - Monotonic stack
8. **Remove K Digits** - Greedy with stack

### Monotonic Stack Pattern
```
- Maintains elements in increasing/decreasing order
- Useful for "next greater/smaller" problems
- Time: O(n), Space: O(n)
```

## Queues (FIFO - First In First Out)

### Key Concepts
- **Operations**:
  - Enqueue: O(1)
  - Dequeue: O(1)
  - Peek: O(1)
- **Space Complexity**: O(n)
- **Use Cases**: BFS, task scheduling, message queues

### Deque (Double-Ended Queue)
- **Operations**: Add/remove from both ends in O(1)
- **Use Cases**: Sliding window maximum, palindrome checking

### Common Queue Problems
1. **Number of Recent Calls** - Deque with time window
2. **Sliding Window Maximum** - Deque maintaining indices
3. **Reveal Cards In Increasing Order** - Reverse simulation
4. **Task Scheduler** - Greedy with queue
5. **Dota2 Senate** - Queue simulation

## Priority Queue (Min/Max Heap)

### Key Concepts
- **Operations**:
  - Insert: O(log n)
  - Extract Min/Max: O(log n)
  - Peek Min/Max: O(1)
- **Use Cases**: Dijkstra's, Huffman coding, k-largest elements

### Common Problems
1. **K Largest Elements** - Min heap of size k
2. **Merge K Sorted Lists** - Min heap
3. **Top K Frequent Elements** - Max heap or bucket sort
4. **Furthest Building You Can Reach** - Max heap

## Stack vs Queue Comparison

| Feature | Stack | Queue | Deque |
|---------|-------|-------|-------|
| Order | LIFO | FIFO | Both |
| Push/Pop | O(1) | O(1) | O(1) |
| Use Case | DFS, Undo | BFS, Scheduling | Sliding Window |

## Interview Tips
- Recognize when to use monotonic stack
- Understand deque for sliding window problems
- Know when to use priority queue vs regular queue
- Practice implementing stack/queue from scratch
- Consider space optimization (array vs linked list)

## Implementation Notes
- Use array with index for better cache performance
- Use linked list for dynamic sizing
- Deque can be implemented with circular array
- Priority queue typically uses binary heap

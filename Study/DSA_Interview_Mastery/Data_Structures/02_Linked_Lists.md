# Linked Lists

## Overview
Linked lists are dynamic data structures crucial for understanding memory management and pointer manipulation in interviews.

## Key Concepts

### Singly Linked List
- **Structure**: Node with data and next pointer
- **Time Complexity**:
  - Access: O(n)
  - Search: O(n)
  - Insertion: O(1) at known position
  - Deletion: O(1) at known position
- **Space Complexity**: O(n)

### Doubly Linked List
- **Structure**: Node with data, next, and previous pointers
- **Advantages**: Bidirectional traversal, easier deletion
- **Trade-off**: Extra space for previous pointer

### Circular Linked List
- **Structure**: Last node points back to first node
- **Use Cases**: Round-robin scheduling, circular buffers

## Common Techniques

### Fast and Slow Pointers
```
- Detect cycle in linked list
- Find middle of linked list
- Find nth node from end
- Time: O(n), Space: O(1)
```

### Reverse Linked List
```
- Iterative: Change next pointers
- Recursive: Reverse rest, then fix pointers
- Time: O(n), Space: O(1) iterative, O(n) recursive
```

### Merge Operations
```
- Merge two sorted lists
- Merge k sorted lists
- Time: O(n+m) for two lists
- Space: O(1) if not counting output
```

## Must-Know Problems

1. **Reverse Linked List** - Iterative and recursive
2. **Detect Cycle** - Floyd's cycle detection
3. **Find Middle** - Fast and slow pointers
4. **Remove Nth Node From End** - Two pointer
5. **Merge Two Sorted Lists** - Two pointer merge
6. **Merge K Sorted Lists** - Min heap or divide and conquer
7. **Palindrome Linked List** - Reverse second half
8. **Reorder List** - Find middle, reverse, merge
9. **Intersection of Two Lists** - Two pointer
10. **Copy List with Random Pointer** - Hash map or constant space

## Edge Cases to Handle
- Empty list
- Single node
- Cycle detection
- Duplicate values
- Null pointers

## Interview Tips
- Draw diagrams for pointer manipulations
- Be careful with null checks
- Consider space constraints (O(1) vs O(n))
- Practice both iterative and recursive approaches
- Always test with edge cases

## Complexity Cheat Sheet
| Operation | Time | Space |
|-----------|------|-------|
| Access | O(n) | - |
| Search | O(n) | - |
| Insert | O(1)* | - |
| Delete | O(1)* | - |
| Reverse | O(n) | O(1) |
| Detect Cycle | O(n) | O(1) |
| Find Middle | O(n) | O(1) |

*At known position

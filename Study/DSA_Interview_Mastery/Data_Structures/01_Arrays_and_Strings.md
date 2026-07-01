# Arrays and Strings

## Overview
Arrays and strings are fundamental data structures used extensively in coding interviews. Mastering them is crucial for MAANG and Wells Fargo interviews.

## Key Concepts

### Arrays
- **Definition**: Contiguous memory locations storing elements of the same type
- **Time Complexity**:
  - Access: O(1)
  - Search: O(n)
  - Insertion: O(n)
  - Deletion: O(n)
- **Space Complexity**: O(n)

### Strings
- **Definition**: Sequence of characters (immutable in most languages)
- **Key Operations**:
  - Concatenation: O(n)
  - Substring: O(n)
  - Search: O(n*m) or O(n+m) with KMP

## Common Patterns

### Two Pointer Technique
```
- Used for: Sorted arrays, palindromes, container with most water
- Approach: Start from both ends, move towards center
- Time: O(n), Space: O(1)
```

### Sliding Window
```
- Used for: Subarray problems, substring problems
- Approach: Maintain a window of elements, expand/contract
- Time: O(n), Space: O(1) or O(k)
```

### Prefix Sum
```
- Used for: Range sum queries, subarray sum problems
- Approach: Precompute cumulative sums
- Time: O(n) preprocessing, O(1) query
- Space: O(n)
```

## Must-Know Problems

1. **Two Sum** - Hash map approach
2. **Container With Most Water** - Two pointer
3. **Longest Substring Without Repeating Characters** - Sliding window
4. **Median of Two Sorted Arrays** - Binary search
5. **Trapping Rain Water** - Dynamic programming or two pointer
6. **Product of Array Except Self** - Prefix/Suffix product
7. **Rotate Array** - In-place rotation
8. **Remove Duplicates** - Two pointer
9. **Merge Sorted Array** - Two pointer from end
10. **Valid Palindrome** - Two pointer with filtering

## Interview Tips
- Always clarify: Is the array sorted? Can we modify it? What about duplicates?
- Consider space-time tradeoffs
- Practice in-place modifications
- Master string manipulation without extra space

## Complexity Cheat Sheet
| Operation | Time | Space |
|-----------|------|-------|
| Access | O(1) | - |
| Search | O(n) | - |
| Insert | O(n) | - |
| Delete | O(n) | - |
| Two Pointer | O(n) | O(1) |
| Sliding Window | O(n) | O(k) |

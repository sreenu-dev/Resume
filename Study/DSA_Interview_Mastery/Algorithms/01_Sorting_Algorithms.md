# Sorting Algorithms

## Overview
Sorting is fundamental to many interview problems. Understanding different algorithms and their trade-offs is essential.

## Comparison-Based Sorting

### Bubble Sort
- **Time**: O(n²) worst/average, O(n) best
- **Space**: O(1)
- **Stable**: Yes
- **In-place**: Yes
- **Use**: Educational, nearly sorted data
- **Approach**: Repeatedly swap adjacent elements if in wrong order

### Selection Sort
- **Time**: O(n²) all cases
- **Space**: O(1)
- **Stable**: No
- **In-place**: Yes
- **Use**: Educational, memory constrained
- **Approach**: Find minimum and place at beginning

### Insertion Sort
- **Time**: O(n²) worst/average, O(n) best
- **Space**: O(1)
- **Stable**: Yes
- **In-place**: Yes
- **Use**: Small arrays, nearly sorted data, online sorting
- **Approach**: Build sorted array one element at a time

### Merge Sort
- **Time**: O(n log n) all cases
- **Space**: O(n)
- **Stable**: Yes
- **In-place**: No
- **Use**: Guaranteed performance, external sorting, linked lists
- **Approach**: Divide and conquer, merge sorted halves
- **Key**: Merge operation is O(n)

### Quick Sort
- **Time**: O(n log n) average, O(n²) worst
- **Space**: O(log n) average, O(n) worst (recursion stack)
- **Stable**: No (standard implementation)
- **In-place**: Yes
- **Use**: General purpose, average case performance
- **Approach**: Partition around pivot, recursively sort
- **Optimization**: Random pivot, 3-way partition for duplicates

### Heap Sort
- **Time**: O(n log n) all cases
- **Space**: O(1)
- **Stable**: No
- **In-place**: Yes
- **Use**: Guaranteed performance, memory constrained
- **Approach**: Build heap, repeatedly extract max

## Non-Comparison Sorting

### Counting Sort
- **Time**: O(n + k) where k is range of values
- **Space**: O(k)
- **Stable**: Yes
- **In-place**: No
- **Use**: Small range of integers
- **Limitation**: Only for non-negative integers

### Radix Sort
- **Time**: O(d * (n + k)) where d is number of digits
- **Space**: O(n + k)
- **Stable**: Yes
- **In-place**: No
- **Use**: Integers, strings with fixed length
- **Approach**: Sort by each digit position

### Bucket Sort
- **Time**: O(n + k) average, O(n²) worst
- **Space**: O(n + k)
- **Stable**: Yes
- **In-place**: No
- **Use**: Uniformly distributed data
- **Approach**: Distribute into buckets, sort each bucket

## Sorting Algorithm Comparison

| Algorithm | Best | Average | Worst | Space | Stable | In-place |
|-----------|------|---------|-------|-------|--------|----------|
| Bubble | O(n) | O(n²) | O(n²) | O(1) | Yes | Yes |
| Selection | O(n²) | O(n²) | O(n²) | O(1) | No | Yes |
| Insertion | O(n) | O(n²) | O(n²) | O(1) | Yes | Yes |
| Merge | O(n log n) | O(n log n) | O(n log n) | O(n) | Yes | No |
| Quick | O(n log n) | O(n log n) | O(n²) | O(log n) | No | Yes |
| Heap | O(n log n) | O(n log n) | O(n log n) | O(1) | No | Yes |
| Counting | O(n+k) | O(n+k) | O(n+k) | O(k) | Yes | No |
| Radix | O(d(n+k)) | O(d(n+k)) | O(d(n+k)) | O(n+k) | Yes | No |
| Bucket | O(n+k) | O(n+k) | O(n²) | O(n+k) | Yes | No |

## Interview Tips

### Choosing the Right Algorithm
- **General purpose**: Quick Sort (average case) or Merge Sort (guaranteed)
- **Guaranteed O(n log n)**: Merge Sort or Heap Sort
- **Small arrays**: Insertion Sort
- **Nearly sorted**: Insertion Sort or Bubble Sort
- **Limited memory**: Heap Sort or Quick Sort
- **Stable sort needed**: Merge Sort or Insertion Sort
- **Small range of values**: Counting Sort or Radix Sort

### Common Interview Questions
1. **Implement Quick Sort** - Partition strategy, pivot selection
2. **Implement Merge Sort** - Merge operation, divide strategy
3. **Merge K Sorted Arrays** - Min heap or merge sort approach
4. **Sort Colors (0, 1, 2)** - 3-way partition or counting sort
5. **Largest Number** - Custom comparator
6. **Meeting Rooms** - Sort by start time
7. **Interval Scheduling** - Sort by end time

### Optimization Techniques
- **Random Pivot**: Avoid O(n²) worst case in Quick Sort
- **3-Way Partition**: Handle duplicates efficiently
- **Hybrid Approach**: Combine algorithms (Timsort)
- **In-place Merge**: Reduce space complexity
- **Parallel Sorting**: Utilize multiple cores

## Stability in Sorting
- **Stable**: Maintains relative order of equal elements
- **Important for**: Multi-key sorting, preserving original order
- **Examples**: Merge Sort, Insertion Sort, Counting Sort
- **Not stable**: Quick Sort, Heap Sort, Selection Sort

## Custom Comparators
```
For objects, define comparison logic:
- Compare by multiple fields
- Custom ordering (e.g., reverse)
- Handle null values
- Ensure consistency: if a < b and b < c, then a < c
```

## Practical Considerations
- **Cache locality**: Insertion sort better for small arrays
- **Memory usage**: In-place sorts preferred for large data
- **Stability**: Required for certain applications
- **Parallelization**: Some algorithms parallelize better
- **Adaptive**: Some algorithms perform better on partially sorted data

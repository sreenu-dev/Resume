# Searching Algorithms

## Overview
Searching is fundamental to many problems. Binary search and its variations are crucial for interview success.

## Linear Search
- **Time**: O(n)
- **Space**: O(1)
- **Use**: Unsorted data, small datasets
- **Approach**: Iterate through elements until found

## Binary Search

### Prerequisites
- **Sorted array required**
- **Works on**: Arrays, sorted lists
- **Time**: O(log n)
- **Space**: O(1) iterative, O(log n) recursive

### Standard Binary Search
```
Find exact element in sorted array
Left = 0, Right = n - 1
While Left <= Right:
  Mid = (Left + Right) / 2
  If arr[Mid] == target: return Mid
  Else if arr[Mid] < target: Left = Mid + 1
  Else: Right = Mid - 1
Return -1 (not found)
```

### Binary Search Variations

#### 1. Find First Occurrence
```
Find leftmost position of target
Adjust right = mid when found
Continue searching left side
```

#### 2. Find Last Occurrence
```
Find rightmost position of target
Adjust left = mid when found
Continue searching right side
```

#### 3. Find Insert Position
```
Position where element should be inserted
Return left pointer when not found
```

#### 4. Find Peak Element
```
Element greater than neighbors
Use binary search to find peak
Compare mid with neighbors
```

#### 5. Find Rotation Point
```
In rotated sorted array
Find where rotation occurs
Compare with boundaries
```

## Binary Search on Answer

### Concept
- **Problem**: Find minimum/maximum value satisfying condition
- **Approach**: Binary search on answer space
- **Key**: Verify if answer is feasible

### Common Problems
1. **Capacity To Ship Packages In D Days**
   - Binary search on capacity
   - Check if can ship in D days

2. **Minimum Speed to Arrive on Time**
   - Binary search on speed
   - Check if arrives on time

3. **Koko Eating Bananas**
   - Binary search on eating speed
   - Check if finishes in H hours

4. **Allocate Books**
   - Binary search on pages
   - Check if can allocate to M students

## Two Pointer Technique

### Concept
- Start from both ends
- Move towards center based on condition
- Time: O(n), Space: O(1)

### Common Problems
1. **Two Sum II** - Sorted array
2. **Container With Most Water** - Maximize area
3. **Trapping Rain Water** - Water trapped between bars
4. **3Sum** - Find triplets
5. **Merge Sorted Array** - Merge in-place

## Ternary Search
- **Use**: Unimodal function (single peak)
- **Time**: O(log₃ n)
- **Space**: O(1)
- **Approach**: Divide into 3 parts, eliminate 1/3 each iteration

## Exponential Search
- **Use**: Unbounded or infinite arrays
- **Time**: O(log n)
- **Space**: O(1)
- **Approach**: Find range, then binary search

## Interpolation Search
- **Use**: Uniformly distributed sorted data
- **Time**: O(log log n) average, O(n) worst
- **Space**: O(1)
- **Approach**: Estimate position based on value

## Jump Search
- **Use**: Sorted arrays
- **Time**: O(√n)
- **Space**: O(1)
- **Approach**: Jump by √n steps, then linear search

## Must-Know Binary Search Problems

1. **Search in Rotated Sorted Array**
   - Handle rotation point
   - Identify which side is sorted
   - Time: O(log n)

2. **Search in Rotated Sorted Array II**
   - With duplicates
   - Handle ambiguous cases
   - Time: O(log n) average, O(n) worst

3. **Median of Two Sorted Arrays**
   - Binary search on partition
   - Time: O(log(min(m,n)))

4. **Find K Closest Elements**
   - Binary search for position
   - Expand window around position
   - Time: O(log n + k)

5. **Time Based Key-Value Store**
   - Binary search on timestamps
   - Find most recent timestamp ≤ target

6. **Single Element in Sorted Array**
   - Find element appearing once
   - Others appear twice
   - Time: O(log n)

7. **Find Minimum in Rotated Sorted Array**
   - Binary search with rotation handling
   - Time: O(log n)

8. **Search Insert Position**
   - Find position to insert
   - Return left pointer
   - Time: O(log n)

9. **First Bad Version**
   - Find first bad version
   - Binary search on versions
   - Time: O(log n)

10. **Sqrt(x)**
    - Binary search on answer
    - Time: O(log n)

## Binary Search Template

### Template 1: Exact Match
```
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

### Template 2: Find First Occurrence
```
left, right = 0, len(arr)
while left < right:
    mid = (left + right) // 2
    if arr[mid] < target:
        left = mid + 1
    else:
        right = mid
return left if left < len(arr) and arr[left] == target else -1
```

### Template 3: Find Last Occurrence
```
left, right = 0, len(arr)
while left < right:
    mid = (left + right + 1) // 2
    if arr[mid] <= target:
        left = mid
    else:
        right = mid - 1
return left if left < len(arr) and arr[left] == target else -1
```

## Interview Tips
- Always verify array is sorted before using binary search
- Handle edge cases: empty array, single element, duplicates
- Be careful with integer overflow in mid calculation
- Use mid = left + (right - left) // 2 to avoid overflow
- Understand when to use left <= right vs left < right
- Practice binary search on answer problems
- Know when to use two pointers vs binary search

## Complexity Comparison

| Algorithm | Time | Space | Use Case |
|-----------|------|-------|----------|
| Linear | O(n) | O(1) | Unsorted, small data |
| Binary | O(log n) | O(1) | Sorted data |
| Ternary | O(log₃ n) | O(1) | Unimodal function |
| Exponential | O(log n) | O(1) | Unbounded array |
| Jump | O(√n) | O(1) | Sorted array |
| Interpolation | O(log log n) | O(1) | Uniform distribution |

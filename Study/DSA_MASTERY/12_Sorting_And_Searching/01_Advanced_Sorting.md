# Advanced Sorting — Mastery Guide

> **Beyond O(N log N):** Decision tree lower bounds, non-comparison sorts, Timsort internals, and complex ordering problems that show up in FAANG design rounds.

---

## Table of Contents
1. [Why Ω(N log N) for Comparison Sorts](#lower-bound)
2. [Radix Sort — Breaking the Lower Bound](#radix)
3. [Counting Sort & Bucket Sort](#counting-bucket)
4. [Timsort — Python's Actual Sort](#timsort)
5. [External Sorting](#external)
6. [Custom Comparators & Ordering Traps](#comparators)
7. [Problems 1–9 with Full Solutions](#problems)
8. [Interview Cheat Sheet](#cheat-sheet)

---

## 1. Why Ω(N log N) for Comparison Sorts <a name="lower-bound"></a>

### Decision Tree Argument

Any comparison-based sorting algorithm defines a **decision tree** where:
- Each **internal node** = one comparison (aᵢ ≤ aⱼ?)
- Each **leaf** = one possible permutation of N elements
- There are **N!** possible permutations → N! leaves

A binary tree with N! leaves has height ≥ ⌈log₂(N!)⌉.

**Stirling's approximation:** `log₂(N!) ≈ N log₂ N - N log₂ e ≈ N log₂ N`

Therefore **any** comparison-based sort requires **Ω(N log N)** comparisons in the worst case.

**Consequences:**
- Merge sort, heap sort, quick sort (average) are **asymptotically optimal**
- No comparison sort can do better than O(N log N) in the worst case
- **But:** Non-comparison sorts (radix, counting) can bypass this — they use arithmetic/hashing instead of comparisons

---

## 2. Radix Sort — O(dN) <a name="radix"></a>

### When Does Radix Beat O(N log N)?

Radix sort is O(d × N) where `d` = number of digits.
- If `d = O(log N)` → same as O(N log N)
- If `d = O(1)` (constant range integers) → **O(N)**
- Beats comparison sort when `d << log N`

**Example:** Sort N 32-bit integers → d = 4 passes of 8 bits each → O(4N) = O(N)

```python
def radix_sort_lsd(arr: list[int]) -> list[int]:
    """
    Least Significant Digit (LSD) Radix Sort.
    Time: O(d × N) where d = number of digits in max element
    Space: O(N + base)
    
    Stable — preserves relative order of equal elements.
    Works for non-negative integers.
    """
    if not arr:
        return arr
    
    max_val = max(arr)
    base = 10
    exp = 1
    
    def counting_sort_by_digit(arr: list[int], exp: int) -> list[int]:
        n = len(arr)
        output = [0] * n
        count = [0] * base
        
        # Count occurrences
        for num in arr:
            digit = (num // exp) % base
            count[digit] += 1
        
        # Prefix sum (cumulative counts)
        for i in range(1, base):
            count[i] += count[i-1]
        
        # Build output (right to left for stability)
        for i in range(n - 1, -1, -1):
            digit = (arr[i] // exp) % base
            count[digit] -= 1
            output[count[digit]] = arr[i]
        
        return output
    
    result = arr[:]
    while max_val // exp > 0:
        result = counting_sort_by_digit(result, exp)
        exp *= base
    
    return result

def radix_sort_binary(arr: list[int], bits: int = 32) -> list[int]:
    """
    Radix sort with base 256 (byte-by-byte) — 4 passes for 32-bit integers.
    Time: O(4N) = O(N), Space: O(N + 256)
    
    Fastest in practice for fixed-size integers.
    """
    BITS_PER_PASS = 8
    BASE = 1 << BITS_PER_PASS  # 256
    MASK = BASE - 1             # 0xFF
    
    result = arr[:]
    for shift in range(0, bits, BITS_PER_PASS):
        count = [0] * BASE
        for x in result:
            count[(x >> shift) & MASK] += 1
        for i in range(1, BASE):
            count[i] += count[i-1]
        tmp = [0] * len(result)
        for x in reversed(result):
            digit = (x >> shift) & MASK
            count[digit] -= 1
            tmp[count[digit]] = x
        result = tmp
    
    return result

# Handling negative numbers with radix sort:
def radix_sort_signed(arr: list[int]) -> list[int]:
    """Handle negative integers by offset."""
    if not arr:
        return arr
    offset = -min(arr) if min(arr) < 0 else 0
    shifted = [x + offset for x in arr]
    sorted_shifted = radix_sort_lsd(shifted)
    return [x - offset for x in sorted_shifted]
```

---

## 3. Counting Sort & Bucket Sort <a name="counting-bucket"></a>

### Counting Sort — O(N + K)

```python
def counting_sort(arr: list[int], max_val: int = None) -> list[int]:
    """
    Time: O(N + K), Space: O(K) where K = range of values
    
    ONLY works for non-negative integers with bounded range.
    Best when K = O(N) → O(N) total.
    """
    if not arr:
        return arr
    
    if max_val is None:
        max_val = max(arr)
    min_val = min(arr)
    
    K = max_val - min_val + 1
    count = [0] * K
    
    for x in arr:
        count[x - min_val] += 1
    
    result = []
    for i, c in enumerate(count):
        result.extend([i + min_val] * c)
    
    return result

# Stable counting sort (preserves original order of equals):
def counting_sort_stable(arr: list[int], key=lambda x: x) -> list[int]:
    """Stable version using cumulative counts."""
    if not arr:
        return arr
    
    keys = [key(x) for x in arr]
    min_k, max_k = min(keys), max(keys)
    K = max_k - min_k + 1
    count = [0] * K
    
    for k in keys:
        count[k - min_k] += 1
    
    for i in range(1, K):
        count[i] += count[i-1]
    
    result = [None] * len(arr)
    for i in range(len(arr) - 1, -1, -1):
        k = keys[i] - min_k
        count[k] -= 1
        result[count[k]] = arr[i]
    
    return result
```

### Bucket Sort — O(N) Average

```python
def bucket_sort(arr: list[float]) -> list[float]:
    """
    For uniformly distributed data in [0, 1).
    Time: O(N) average (O(N²) worst if all in one bucket)
    Space: O(N)
    """
    if not arr:
        return arr
    
    n = len(arr)
    
    # Normalize to [0, 1)
    min_val, max_val = min(arr), max(arr)
    if min_val == max_val:
        return arr[:]
    
    buckets = [[] for _ in range(n)]
    
    for x in arr:
        # Bucket index: which n-th of [min, max] does x fall in?
        idx = int((x - min_val) / (max_val - min_val + 1e-10) * n)
        idx = min(idx, n - 1)
        buckets[idx].append(x)
    
    result = []
    for bucket in buckets:
        bucket.sort()  # insertion sort in practice (small buckets)
        result.extend(bucket)
    
    return result
```

---

## 4. Timsort — Python's Actual Sort <a name="timsort"></a>

**Timsort** = merge sort + insertion sort hybrid. Used in Python (`list.sort()`, `sorted()`), Java (Arrays.sort for objects).

### Key Properties

1. **Adaptive:** Exploits existing runs (ascending/descending sequences)
2. **Stable:** Equal elements maintain relative order
3. **Minimum run size:** 32-64 elements (uses insertion sort within runs)
4. **Galloping mode:** When one run consistently wins merges, switch to exponential search

```python
def timsort_simplified(arr: list) -> list:
    """
    Simplified Timsort demonstrating core ideas.
    Real Python Timsort has additional optimizations.
    
    Time: O(N log N) worst, O(N) best (already sorted)
    Space: O(N)
    """
    MIN_RUN = 32
    
    def insertion_sort(arr, left, right):
        """Sort arr[left:right+1] using insertion sort."""
        for i in range(left + 1, right + 1):
            key = arr[i]
            j = i - 1
            while j >= left and arr[j] > key:
                arr[j + 1] = arr[j]
                j -= 1
            arr[j + 1] = key
    
    def merge(arr, left, mid, right):
        """Merge arr[left:mid+1] and arr[mid+1:right+1] in place."""
        left_part = arr[left:mid + 1]
        right_part = arr[mid + 1:right + 1]
        
        i = j = 0
        k = left
        
        while i < len(left_part) and j < len(right_part):
            if left_part[i] <= right_part[j]:
                arr[k] = left_part[i]
                i += 1
            else:
                arr[k] = right_part[j]
                j += 1
            k += 1
        
        while i < len(left_part):
            arr[k] = left_part[i]
            i += 1
            k += 1
        
        while j < len(right_part):
            arr[k] = right_part[j]
            j += 1
            k += 1
    
    n = len(arr)
    arr = arr[:]
    
    # Sort individual runs with insertion sort
    for start in range(0, n, MIN_RUN):
        end = min(start + MIN_RUN - 1, n - 1)
        insertion_sort(arr, start, end)
    
    # Merge runs bottom-up
    size = MIN_RUN
    while size < n:
        for left in range(0, n, 2 * size):
            mid = min(left + size - 1, n - 1)
            right = min(left + 2 * size - 1, n - 1)
            if mid < right:
                merge(arr, left, mid, right)
        size *= 2
    
    return arr
```

### Why Timsort Wins in Practice

| Scenario | Quicksort | Mergesort | Timsort |
|----------|-----------|-----------|---------|
| Random data | O(N log N) avg | O(N log N) | O(N log N) |
| Already sorted | O(N²) worst | O(N log N) | **O(N)** |
| Reverse sorted | O(N²) worst | O(N log N) | **O(N)** |
| Nearly sorted | O(N log N) | O(N log N) | **O(N)** (few runs) |
| Stability | No (unstable) | Yes | Yes |

---

## 5. External Sorting <a name="external"></a>

When data doesn't fit in RAM (disk-based sorting):

```python
import heapq
from typing import Iterator

def external_sort_simulation(data_chunks: list[list[int]]) -> list[int]:
    """
    External merge sort simulation.
    Each chunk = one "run" that fits in memory.
    Merge K sorted runs using K-way merge (min-heap).
    
    Time: O(N log K) where K = number of runs
    Space: O(K) for heap
    """
    # Sort each chunk in memory
    sorted_chunks = [sorted(chunk) for chunk in data_chunks]
    
    # K-way merge using min-heap
    # Heap entries: (value, chunk_index, element_index)
    heap = []
    iterators = [iter(chunk) for chunk in sorted_chunks]
    
    for i, it in enumerate(iterators):
        val = next(it, None)
        if val is not None:
            heapq.heappush(heap, (val, i))
    
    result = []
    element_indices = [0] * len(sorted_chunks)
    
    while heap:
        val, chunk_idx = heapq.heappop(heap)
        result.append(val)
        element_indices[chunk_idx] += 1
        idx = element_indices[chunk_idx]
        if idx < len(sorted_chunks[chunk_idx]):
            heapq.heappush(heap, (sorted_chunks[chunk_idx][idx], chunk_idx))
    
    return result
```

---

## 6. Custom Comparators & Ordering Traps <a name="comparators"></a>

### Python's `cmp_to_key`

```python
from functools import cmp_to_key

# Custom comparator for "largest number" problem
def largest_number_comparator(a: str, b: str) -> int:
    """Return negative if a should come before b (a+b > b+a)."""
    if a + b > b + a:
        return -1   # a comes first
    elif a + b < b + a:
        return 1    # b comes first
    return 0

# Usage:
nums = [3, 30, 34, 5, 9]
strs = [str(n) for n in nums]
strs.sort(key=cmp_to_key(largest_number_comparator))
print(''.join(strs))  # "9534330"

# TOTAL ORDERING: your comparator MUST be transitive, antisymmetric, total!
# Violating these leads to undefined behavior.
```

### Stability Requirements

```python
# Python's sort is stable — exploit this for multi-key sorting
people = [("Alice", 30), ("Bob", 25), ("Charlie", 30)]

# Sort by age, then by name (secondary key)
# Method 1: tuple key (most Pythonic)
people.sort(key=lambda x: (x[1], x[0]))

# Method 2: Sort twice (stable sort property)
people.sort(key=lambda x: x[0])    # secondary key first
people.sort(key=lambda x: x[1])    # primary key second
# Result: [("Bob",25), ("Alice",30), ("Charlie",30)] ← stable!
```

---

## 7. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Dutch National Flag (Sort Colors)
**LeetCode 75 — Three-way partition**

```python
def sort_colors(nums: list[int]) -> None:
    """
    Dutch National Flag — one-pass O(N), O(1) space.
    Three pointers: lo (next 0), mid (current), hi (next 2).
    
    Invariant: nums[0:lo]=0, nums[lo:mid]=1, nums[hi+1:]=2
    """
    lo, mid, hi = 0, 0, len(nums) - 1
    
    while mid <= hi:
        if nums[mid] == 0:
            nums[lo], nums[mid] = nums[mid], nums[lo]
            lo += 1
            mid += 1
        elif nums[mid] == 1:
            mid += 1
        else:  # nums[mid] == 2
            nums[mid], nums[hi] = nums[hi], nums[mid]
            hi -= 1
            # Don't increment mid — new nums[mid] not yet examined
    
# Generalization: K-way partition (K colors)
def k_way_partition(arr: list[int], k: int) -> list[int]:
    """Generalized DNF using counting sort. O(N) time."""
    count = [0] * k
    for x in arr:
        count[x] += 1
    result = []
    for val, cnt in enumerate(count):
        result.extend([val] * cnt)
    return result
```

---

### Problem 2: Largest Number from Digits
**LeetCode 179**

```python
def largest_number(nums: list[int]) -> str:
    """
    Custom comparator: for two strings a, b, prefer a+b > b+a.
    
    Time: O(N log N × W) where W = max digits
    Space: O(N)
    """
    strs = list(map(str, nums))
    strs.sort(key=cmp_to_key(lambda a, b: (1 if a+b < b+a else -1) if a+b != b+a else 0))
    
    result = ''.join(strs)
    return '0' if result[0] == '0' else result  # handle [0,0]

# Prove correctness: the comparator defines a total ordering.
# If we can prove transitivity: a>b and b>c implies a>c
# where ">" means a+b > b+a in string comparison.
# Proof sketch: treat strings as numbers in base 10^len.
# a>b means a*10^len(b) + b > b*10^len(a) + a
# This relation is transitive (can be shown algebraically).
```

---

### Problem 3: Wiggle Sort II
**LeetCode 324**

```python
def wiggle_sort(nums: list[int]) -> None:
    """
    LeetCode 324. Arrange nums such that nums[0] < nums[1] > nums[2] < nums[3]...
    
    Key insight: Find median, then place small elements at even indices, 
    large at odd indices. Use virtual index trick for O(N) space variant.
    
    Time: O(N log N) with sort, O(N) with quickselect+DNF
    Space: O(N)
    """
    n = len(nums)
    sorted_nums = sorted(nums)
    
    # Place larger half at odd positions, smaller half at even positions
    # Go from the end to handle duplicates correctly
    # mid = (n-1)//2, large starts from n-1
    
    mid = (n - 1) // 2  # index of median in sorted array
    # Fill odd positions with larger half (reverse order)
    # Fill even positions with smaller half (reverse order)
    
    nums[::2]  = sorted_nums[:mid+1][::-1]   # even indices ← smaller half reversed
    nums[1::2] = sorted_nums[mid+1:][::-1]   # odd indices ← larger half reversed

# O(N) approach using quickselect for median:
def wiggle_sort_on(nums: list[int]) -> None:
    """O(N) using median-of-medians + virtual index DNF."""
    n = len(nums)
    
    def nth_element(arr, n, k):
        """Quickselect for kth element."""
        import random
        lo, hi = 0, n - 1
        while lo < hi:
            pivot_idx = random.randint(lo, hi)
            arr[pivot_idx], arr[hi] = arr[hi], arr[pivot_idx]
            pivot = arr[hi]
            store = lo
            for i in range(lo, hi):
                if arr[i] < pivot:
                    arr[i], arr[store] = arr[store], arr[i]
                    store += 1
            arr[store], arr[hi] = arr[hi], arr[store]
            if store == k:
                return arr[k]
            elif store < k:
                lo = store + 1
            else:
                hi = store - 1
        return arr[lo]
    
    median = nth_element(nums[:], n, (n - 1) // 2)
    
    # Virtual index: maps [0,1,2,...,n-1] → [1,3,5,...,0,2,4,...]
    def vidx(i):
        return (1 + 2 * i) % (n | 1)
    
    # Three-way partition using virtual index
    lo, mid, hi = 0, 0, n - 1
    while mid <= hi:
        if nums[vidx(mid)] > median:
            nums[vidx(lo)], nums[vidx(mid)] = nums[vidx(mid)], nums[vidx(lo)]
            lo += 1; mid += 1
        elif nums[vidx(mid)] < median:
            nums[vidx(mid)], nums[vidx(hi)] = nums[vidx(hi)], nums[vidx(mid)]
            hi -= 1
        else:
            mid += 1
```

---

### Problem 4: Merge Intervals
**LeetCode 56**

```python
def merge_intervals(intervals: list[list[int]]) -> list[list[int]]:
    """
    Time: O(N log N), Space: O(N)
    
    Sort by start time, then greedily merge overlapping intervals.
    """
    if not intervals:
        return []
    
    intervals.sort(key=lambda x: x[0])
    merged = [intervals[0]]
    
    for start, end in intervals[1:]:
        if start <= merged[-1][1]:
            # Overlapping: extend current interval
            merged[-1][1] = max(merged[-1][1], end)
        else:
            # Non-overlapping: start new interval
            merged.append([start, end])
    
    return merged
```

---

### Problem 5: Non-Overlapping Intervals (Greedy)
**LeetCode 435**

```python
def erase_overlap_intervals(intervals: list[list[int]]) -> int:
    """
    LeetCode 435. Minimum number of intervals to remove so rest don't overlap.
    = N - (maximum number of non-overlapping intervals)
    
    Greedy: sort by END time, greedily pick intervals with earliest end.
    
    Time: O(N log N), Space: O(1)
    """
    if not intervals:
        return 0
    
    intervals.sort(key=lambda x: x[1])  # sort by end time!
    count = 0
    last_end = float('-inf')
    
    for start, end in intervals:
        if start >= last_end:
            count += 1       # pick this interval
            last_end = end
    
    return len(intervals) - count
```

---

### Problem 6: Minimum Number by Removing K Digits
**LeetCode 402**

```python
def remove_k_digits(num: str, k: int) -> str:
    """
    LeetCode 402. Remove k digits to get smallest possible number.
    
    Greedy: use monotone stack. Remove digit if it's greater than next digit.
    Time: O(N), Space: O(N)
    """
    stack = []
    for digit in num:
        while k > 0 and stack and stack[-1] > digit:
            stack.pop()
            k -= 1
        stack.append(digit)
    
    # If k remaining, remove from end (stack is non-decreasing)
    stack = stack[:-k] if k else stack
    
    # Remove leading zeros
    result = ''.join(stack).lstrip('0')
    return result or '0'
```

---

### Problem 7: Sort by Frequency, then Alphabetically
**LeetCode 451 — Sort Characters By Frequency**

```python
from collections import Counter

def frequency_sort(s: str) -> str:
    """
    Sort characters by frequency (descending), then alphabetically.
    Time: O(N log N), Space: O(N)
    """
    count = Counter(s)
    # Sort by: (-frequency, character) for stability
    return ''.join(
        char * freq 
        for char, freq in sorted(count.items(), key=lambda x: (-x[1], x[0]))
    )

# Bucket sort variant O(N):
def frequency_sort_bucket(s: str) -> str:
    """O(N) using bucket sort on frequencies."""
    count = Counter(s)
    n = len(s)
    buckets = [[] for _ in range(n + 1)]
    
    for char, freq in count.items():
        buckets[freq].append(char)
    
    result = []
    for freq in range(n, 0, -1):
        for char in sorted(buckets[freq], reverse=True):
            result.append(char * freq)
    
    return ''.join(result)
```

---

### Problem 8: Pancake Sorting
**LeetCode 969**

```python
def pancake_sort(arr: list[int]) -> list[int]:
    """
    LeetCode 969. Sort using pancake flips (reverse prefix).
    Return sequence of k values (flip arr[:k] at each step).
    
    Strategy: bring max to position 0 (flip), then to end (flip again).
    Time: O(N²) — 2N flips, each flip O(N)
    """
    def flip(arr, k):
        arr[:k] = arr[:k][::-1]
    
    n = len(arr)
    result = []
    
    for size in range(n, 1, -1):
        # Find max in arr[:size]
        max_idx = arr[:size].index(max(arr[:size]))
        
        if max_idx == size - 1:
            continue  # already in place
        
        if max_idx != 0:
            # Flip max to front
            flip(arr, max_idx + 1)
            result.append(max_idx + 1)
        
        # Flip max to correct position
        flip(arr, size)
        result.append(size)
    
    return result
```

---

### Problem 9: Queue Reconstruction by Height
**LeetCode 406**

```python
def reconstruct_queue(people: list[list[int]]) -> list[list[int]]:
    """
    LeetCode 406. people[i] = [height_i, k_i] where k_i = number of people 
    of height >= height_i who stand before person i.
    
    Greedy insight: Sort by height descending (tall first), then k ascending.
    Insert each person at index k — taller people already placed won't be displaced.
    
    Time: O(N²) due to list insertions
    Space: O(N)
    """
    # Sort: tallest first, then by k ascending
    people.sort(key=lambda x: (-x[0], x[1]))
    
    result = []
    for person in people:
        result.insert(person[1], person)  # insert at position k
    
    return result

# Why this works:
# After sorting tallest first, when we insert person[h, k],
# all previously inserted people have height >= h.
# So inserting at index k satisfies exactly k people of height >= h before them.
# Shorter people inserted later don't affect k-counts of taller people.
```

---

## 8. Interview Cheat Sheet <a name="cheat-sheet"></a>

### Algorithm Selection Guide

```
Data type and constraints?
├── Integers in bounded range [0, K]
│   ├── K = O(N): Counting sort → O(N)
│   └── K = O(N log N): Radix sort → O(N log N) but faster constant
├── Floats in [0, 1) uniform distribution
│   └── Bucket sort → O(N) average
├── Strings or complex objects
│   └── Comparison sort (Timsort in Python) → O(N log N)
└── Already "almost sorted" (k inversions)
    └── Insertion sort → O(N + k inversions)
```

### Sorting Complexity Table

| Algorithm | Best | Average | Worst | Space | Stable |
|-----------|------|---------|-------|-------|--------|
| Bubble | O(N) | O(N²) | O(N²) | O(1) | Yes |
| Insertion | O(N) | O(N²) | O(N²) | O(1) | Yes |
| Merge | O(N log N) | O(N log N) | O(N log N) | O(N) | Yes |
| Quick | O(N log N) | O(N log N) | O(N²) | O(log N) | No |
| Heap | O(N log N) | O(N log N) | O(N log N) | O(1) | No |
| Counting | O(N+K) | O(N+K) | O(N+K) | O(K) | Yes |
| Radix | O(dN) | O(dN) | O(dN) | O(N+b) | Yes |
| Tim | O(N) | O(N log N) | O(N log N) | O(N) | Yes |

### Key Interview Insights

> **"When does quicksort degrade to O(N²)?"** — When pivot consistently chosen as min/max (already sorted input, adversarial input). Fix: random pivot selection or median-of-3.

> **"Why is Python's sort called Timsort?"** — Named after Tim Peters who designed it in 2002. It's optimal for real-world data which contains natural runs.

> **"What's the catch with custom comparators?"** — Must define a **total order**: reflexive (a=a), antisymmetric (a<b → b>a), transitive (a<b, b<c → a<c). Violating these causes Python's sort to produce incorrect results.

> **"Can we sort stably without extra space?"** — Block sort achieves O(N log N) worst case, O(1) extra space, and stability. But extremely complex to implement.

---

*Previous: [Manacher's ←](../11_String_Algorithms/04_Manacher_Palindrome.md) | Next: [Quickselect & Order Statistics →](02_Quickselect_And_Order_Statistics.md)*

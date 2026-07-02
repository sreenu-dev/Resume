# Heap Patterns — K-Problems & Advanced Greedy

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Priority queues, greedy algorithms, frequency analysis
> **Core Theme:** Recognizing the "K-th" and "top-K" pattern and applying
> heap-based greedy strategies for scheduling and optimization problems.

---

## 1. K-th Largest Element (LeetCode 215)

```python
import heapq

def findKthLargest(nums: list[int], k: int) -> int:
    heap = []
    for num in nums:
        heapq.heappush(heap, num)
        if len(heap) > k:
            heapq.heappop(heap)
    return heap[0]
```
**Time:** O(N log K) | **Space:** O(K)

**QuickSelect alternative:** O(N) average, O(N²) worst. Heap preferred for
predictable performance. `heapq.nlargest(k, nums)` is the Python built-in.

---

## 2. Merge K Sorted Lists/Arrays

### K Sorted Lists (LeetCode 23)

```python
def mergeKLists(lists):
    heap = []
    for i, node in enumerate(lists):
        if node:
            heapq.heappush(heap, (node.val, i, node))

    dummy = ListNode(0)
    curr = dummy

    while heap:
        val, i, node = heapq.heappop(heap)
        curr.next = node
        curr = curr.next
        if node.next:
            heapq.heappush(heap, (node.next.val, i, node.next))

    return dummy.next
```
**Time:** O(N log K) | **Space:** O(K)

### K Sorted Arrays

```python
def mergeKArrays(arrays: list[list[int]]) -> list[int]:
    heap = []
    for i, arr in enumerate(arrays):
        if arr:
            heapq.heappush(heap, (arr[0], i, 0))

    result = []
    while heap:
        val, i, j = heapq.heappop(heap)
        result.append(val)
        if j + 1 < len(arrays[i]):
            heapq.heappush(heap, (arrays[i][j+1], i, j+1))

    return result
```
**Time:** O(N log K) | **Space:** O(K)

---

## 3. K Closest Points to Origin (LeetCode 973)

```python
def kClosest(points: list[list[int]], k: int) -> list[list[int]]:
    heap = []
    for x, y in points:
        dist = -(x*x + y*y)
        if len(heap) < k:
            heapq.heappush(heap, (dist, x, y))
        elif dist > heap[0][0]:
            heapq.heapreplace(heap, (dist, x, y))

    return [[x, y] for _, x, y in heap]
```
**Time:** O(N log K) | **Space:** O(K)

---

## 4. Task Scheduler (LeetCode 621)

```python
from collections import Counter

def leastInterval(tasks: list[str], n: int) -> int:
    freq = Counter(tasks)
    heap = [-f for f in freq.values()]
    heapq.heapify(heap)

    time = 0
    while heap:
        cycle = []
        for _ in range(n + 1):
            if heap:
                cycle.append(heapq.heappop(heap))

        for f in cycle:
            if f + 1 < 0:
                heapq.heappush(heap, f + 1)

        time += n + 1 if heap else len(cycle)

    return time
```
**Time:** O(N log K) where K = distinct tasks ≤ 26 | **Space:** O(K)

**Mathematical O(1) formula:**
```python
def leastIntervalMath(tasks, n):
    freq = sorted(Counter(tasks).values(), reverse=True)
    max_freq = freq[0]
    max_count = freq.count(max_freq)
    return max(len(tasks), (max_freq - 1) * (n + 1) + max_count)
```

---

## 5. Reorganize String (LeetCode 767)

```python
def reorganizeString(s: str) -> str:
    freq = Counter(s)

    if max(freq.values()) > (len(s) + 1) // 2:
        return ""

    heap = [(-f, ch) for ch, f in freq.items()]
    heapq.heapify(heap)

    result = []
    while len(heap) >= 2:
        f1, ch1 = heapq.heappop(heap)
        f2, ch2 = heapq.heappop(heap)
        result.extend([ch1, ch2])
        if f1 + 1 < 0: heapq.heappush(heap, (f1+1, ch1))
        if f2 + 1 < 0: heapq.heappush(heap, (f2+1, ch2))

    if heap:
        result.append(heap[0][1])

    return ''.join(result)
```
**Time:** O(N log 26) = O(N) | **Space:** O(N)

---

## 6. Find K Pairs with Smallest Sum (LeetCode 373)

```python
def kSmallestPairs(nums1: list[int], nums2: list[int], k: int) -> list[list[int]]:
    if not nums1 or not nums2:
        return []

    heap = [(nums1[0] + nums2[0], 0, 0)]
    result = []
    visited = set([(0, 0)])

    while heap and len(result) < k:
        _, i, j = heapq.heappop(heap)
        result.append([nums1[i], nums2[j]])

        for ni, nj in [(i+1, j), (i, j+1)]:
            if ni < len(nums1) and nj < len(nums2) and (ni, nj) not in visited:
                heapq.heappush(heap, (nums1[ni] + nums2[nj], ni, nj))
                visited.add((ni, nj))

    return result
```
**Time:** O(K log K) | **Space:** O(K)

---

## 7. Kth Smallest in Sorted Matrix (LeetCode 378)

```python
def kthSmallest(matrix: list[list[int]], k: int) -> int:
    n = len(matrix)
    heap = [(matrix[i][0], i, 0) for i in range(n)]
    heapq.heapify(heap)

    for _ in range(k - 1):
        val, r, c = heapq.heappop(heap)
        if c + 1 < n:
            heapq.heappush(heap, (matrix[r][c+1], r, c+1))

    return heapq.heappop(heap)[0]
```
**Time:** O(K log N) | **Space:** O(N)

**Binary search alternative (O(N log(max-min))):**
```python
def kthSmallestBS(matrix, k):
    n = len(matrix)
    lo, hi = matrix[0][0], matrix[n-1][n-1]

    def count_leq(mid):
        count, r, c = 0, n-1, 0
        while r >= 0 and c < n:
            if matrix[r][c] <= mid:
                count += r + 1; c += 1
            else:
                r -= 1
        return count

    while lo < hi:
        mid = (lo + hi) // 2
        if count_leq(mid) < k: lo = mid + 1
        else: hi = mid

    return lo
```

---

## 8. Top K Frequent Words (LeetCode 692)

```python
def topKFrequent(words: list[str], k: int) -> list[str]:
    freq = Counter(words)
    heap = []
    for word, f in freq.items():
        heapq.heappush(heap, (-f, word))

    return [heapq.heappop(heap)[1] for _ in range(k)]
```
**Time:** O(N + M log M) where M = unique words | **Space:** O(M)

---

## 9. IPO Problem — Two Heaps + Greedy (LeetCode 502)

```python
def findMaximizedCapital(k: int, w: int,
                          profits: list[int], capital: list[int]) -> int:
    available = sorted(zip(capital, profits))
    max_profit = []
    i = 0

    for _ in range(k):
        while i < len(available) and available[i][0] <= w:
            heapq.heappush(max_profit, -available[i][1])
            i += 1

        if not max_profit:
            break

        w -= heapq.heappop(max_profit)

    return w
```
**Time:** O(N log N + K log N) | **Space:** O(N)

---

## 10. Minimum Cost to Hire K Workers (LeetCode 857)

```python
def mincostToHireWorkers(quality: list[int], wage: list[int], k: int) -> float:
    workers = sorted(zip(wage, quality), key=lambda x: x[0]/x[1])

    max_heap = []
    quality_sum = 0
    result = float('inf')

    for w, q in workers:
        heapq.heappush(max_heap, -q)
        quality_sum += q

        if len(max_heap) > k:
            quality_sum += heapq.heappop(max_heap)

        if len(max_heap) == k:
            result = min(result, (w/q) * quality_sum)

    return result
```
**Time:** O(N log N + N log K) | **Space:** O(K)

**Key insight:** Sort by wage/quality ratio. The worker with highest ratio
determines total cost. Use a sliding window of K workers with a max-heap
to maintain the minimum quality sum.

---

## 11. Meeting Rooms II (LeetCode 253)

```python
def minMeetingRooms(intervals: list[list[int]]) -> int:
    intervals.sort()
    heap = []

    for start, end in intervals:
        if heap and heap[0] <= start:
            heapq.heapreplace(heap, end)
        else:
            heapq.heappush(heap, end)

    return len(heap)
```
**Time:** O(N log N) | **Space:** O(N)

---

## 12. Kth Largest in Stream (LeetCode 703)

```python
class KthLargest:
    def __init__(self, k: int, nums: list[int]):
        self.k = k
        self.heap = []
        for num in nums:
            self.add(num)

    def add(self, val: int) -> int:
        heapq.heappush(self.heap, val)
        if len(self.heap) > self.k:
            heapq.heappop(self.heap)
        return self.heap[0]
```
**add:** O(log K) | **Space:** O(K)

---

## Heap Pattern Recognition

| Problem Type | Heap Type | Size |
|---|---|---|
| K-th largest | Min-heap | K |
| K-th smallest | Max-heap | K |
| K closest to X | Max-heap by distance | K |
| Merge K sorted | Min-heap | K |
| Task scheduler | Max-heap of frequencies | ≤26 |
| IPO / greedy | Min-heap by cost + max-heap by profit | N |
| Sliding window median | Two heaps (max+min) | K each |
| Meeting rooms | Min-heap of end times | N |

## Interview Tips

1. **Python max-heap**: Always negate values. Use `(-val, val)` when you need to recover the original.
2. **heapreplace vs heappop+heappush**: Use `heapreplace` when you know new element won't be the minimum.
3. **Lazy deletion pattern**: Maintain a `removed` counter. Only clean the heap when accessing the top.
4. **K closest with QuickSelect**: If K is close to N, QuickSelect (O(N)) beats heap (O(N log K)).
5. **IPO**: The two-heap pattern (unlock by capital, pick by profit) appears in many resource-allocation greedy problems.

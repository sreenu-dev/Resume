# Two Heaps Pattern — Median Maintenance & Advanced Applications

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Heap operations, lazy deletion, amortized analysis
> **Core Theme:** Maintaining a sorted partition of a data stream using two
> heaps, enabling O(1) median queries and O(log N) insertions.

---

## 1. Two-Heap Pattern — Core Invariant

**Structure:**
- `lo`: max-heap storing the **lower half** of all elements
- `hi`: min-heap storing the **upper half** of all elements

**Invariants maintained after every operation:**
1. Every element in `lo` ≤ every element in `hi`
2. `len(lo) == len(hi)` (even total) or `len(lo) == len(hi) + 1` (odd total)

**Median:**
- Odd total: median = `lo.peek()` (top of max-heap)
- Even total: median = `(lo.peek() + hi.peek()) / 2`

---

## 2. Find Median from Data Stream (LeetCode 295)

```python
import heapq

class MedianFinder:
    def __init__(self):
        self.lo = []    # Max-heap (negate values)
        self.hi = []    # Min-heap

    def addNum(self, num: int) -> None:
        if not self.lo or num <= -self.lo[0]:
            heapq.heappush(self.lo, -num)
        else:
            heapq.heappush(self.hi, num)

        if len(self.lo) > len(self.hi) + 1:
            heapq.heappush(self.hi, -heapq.heappop(self.lo))
        elif len(self.hi) > len(self.lo):
            heapq.heappush(self.lo, -heapq.heappop(self.hi))

    def findMedian(self) -> float:
        if len(self.lo) > len(self.hi):
            return float(-self.lo[0])
        return (-self.lo[0] + self.hi[0]) / 2.0
```
**addNum:** O(log N) | **findMedian:** O(1) | **Space:** O(N)

**Correctness proof:** After routing and rebalancing, size difference ≤ 1.
Ordering invariant maintained because we always transfer the tops (extremes).

---

## 3. Sliding Window Median — The Hard Variant (LeetCode 480)

**Challenge:** Elements slide out the window, requiring O(log K) deletion.
**Solution:** Lazy deletion — mark elements as removed, clean heaps only on access.

```python
from collections import defaultdict

def medianSlidingWindow(nums: list[int], k: int) -> list[float]:
    lo = []                      # Max-heap (negated)
    hi = []                      # Min-heap
    removed = defaultdict(int)
    lo_size = 0
    hi_size = 0

    def push(num: int) -> None:
        nonlocal lo_size, hi_size
        if not lo or num <= -lo[0]:
            heapq.heappush(lo, -num)
            lo_size += 1
        else:
            heapq.heappush(hi, num)
            hi_size += 1

    def pop_lazy(num: int) -> None:
        nonlocal lo_size, hi_size
        removed[num] += 1
        if num <= -lo[0]:
            lo_size -= 1
        else:
            hi_size -= 1

    def clean_lo() -> None:
        while lo and removed[-lo[0]] > 0:
            removed[-lo[0]] -= 1
            heapq.heappop(lo)

    def clean_hi() -> None:
        while hi and removed[hi[0]] > 0:
            removed[hi[0]] -= 1
            heapq.heappop(hi)

    def rebalance() -> None:
        nonlocal lo_size, hi_size
        while lo_size > hi_size + 1:
            clean_lo()
            val = -heapq.heappop(lo)
            lo_size -= 1
            heapq.heappush(hi, val)
            hi_size += 1
            clean_lo()
        while hi_size > lo_size:
            clean_hi()
            val = heapq.heappop(hi)
            hi_size -= 1
            heapq.heappush(lo, -val)
            lo_size += 1
            clean_hi()

    def get_median() -> float:
        clean_lo(); clean_hi()
        if k % 2 == 1:
            return float(-lo[0])
        return (-lo[0] + hi[0]) / 2.0

    for i in range(k):
        push(nums[i])
    rebalance()
    result = [get_median()]

    for i in range(k, len(nums)):
        push(nums[i])
        pop_lazy(nums[i - k])
        rebalance()
        result.append(get_median())

    return result
```
**Time:** O(N log K) | **Space:** O(K) effective, O(N) total

### Why Lazy Deletion is O(N log K) Total

Each element is pushed once O(log K) and cleaned once O(log K).
`clean_lo`/`clean_hi` amortize over all operations. Total: 2N × O(log K) = O(N log K).

---

## 4. Minimum Number of Refueling Stops (LeetCode 871)

**Retroactive greedy:** Travel as far as possible, retroactively pick largest fuel station passed.

```python
def minRefuelStops(target: int, startFuel: int,
                   stations: list[list[int]]) -> int:
    heap = []
    stops = 0
    fuel = startFuel
    prev_pos = 0

    for pos, capacity in stations + [[target, 0]]:
        fuel -= pos - prev_pos
        prev_pos = pos

        while fuel < 0 and heap:
            fuel -= heapq.heappop(heap)    # Negate back to positive
            stops += 1

        if fuel < 0:
            return -1

        heapq.heappush(heap, -capacity)

    return stops
```
**Time:** O(N log N) | **Space:** O(N)

**Why greedy is correct:** When stuck, fill at the largest available station.
This is the optimal choice — it maximizes the fuel gained per stop.

---

## 5. Furthest Building You Can Reach (LeetCode 1642)

**Greedy:** Reserve ladders for the largest jumps encountered so far.

```python
def furthestBuilding(heights: list[int], bricks: int, ladders: int) -> int:
    heap = []

    for i in range(len(heights) - 1):
        diff = heights[i+1] - heights[i]
        if diff <= 0:
            continue

        heapq.heappush(heap, diff)

        if len(heap) > ladders:
            smallest = heapq.heappop(heap)
            bricks -= smallest
            if bricks < 0:
                return i

    return len(heights) - 1
```
**Time:** O(N log L) | **Space:** O(L)

**Invariant:** The heap contains the `ladders` largest height differences seen so far.
When we must remove a ladder usage, we replace the smallest (least valuable) with bricks.

---

## 6. Minimum Cost to Connect Sticks (LeetCode 1167)

**Huffman coding — always merge two smallest sticks:**

```python
def connectSticks(sticks: list[int]) -> int:
    heapq.heapify(sticks)
    total_cost = 0

    while len(sticks) > 1:
        first = heapq.heappop(sticks)
        second = heapq.heappop(sticks)
        cost = first + second
        total_cost += cost
        heapq.heappush(sticks, cost)

    return total_cost
```
**Time:** O(N log N) | **Space:** O(N)

**Proof (Huffman optimality):** Merging small sticks early prevents their
cost from being compounded in future merges. This is the Huffman coding algorithm.

---

## 7. Schedule Tasks to Minimize Maximum Lateness

```python
def minimizeLateness(tasks: list[tuple]) -> int:
    """
    tasks: list of (processing_time, deadline)
    Returns minimum possible maximum lateness.
    """
    tasks.sort(key=lambda x: x[1])   # Earliest Deadline First

    time = max_lateness = 0
    for p, d in tasks:
        time += p
        max_lateness = max(max_lateness, time - d)

    return max_lateness
```
**Time:** O(N log N) | **Space:** O(1)

**Exchange argument proof:** Any schedule with an inversion (task before
higher-deadline task) can be improved by swapping — sorted-by-deadline is optimal.

---

## 8. Find Right Interval (LeetCode 436)

```python
from bisect import bisect_left

def findRightInterval(intervals: list[list[int]]) -> list[int]:
    sorted_starts = sorted((start, i) for i, (start, _) in enumerate(intervals))
    starts = [s for s, _ in sorted_starts]

    result = []
    for _, end in intervals:
        idx = bisect_left(starts, end)
        if idx < len(sorted_starts):
            result.append(sorted_starts[idx][1])
        else:
            result.append(-1)

    return result
```
**Time:** O(N log N) | **Space:** O(N)

---

## Two-Heap Pattern Summary

| Scenario | lo heap | hi heap | Invariant |
|---|---|---|---|
| Median stream | Max-heap (lower) | Min-heap (upper) | Size diff ≤ 1 |
| Sliding median | Max-heap + lazy | Min-heap + lazy | Effective sizes balanced |
| IPO problem | Min-heap (capital) | Max-heap (profit) | Separate by feasibility |
| Refueling stops | Max-heap (capacity) | — | Retroactive greedy |

## Core Patterns

### Pattern 1: Static Two-Heap
- Route: add to lo if ≤ lo.max, else add to hi
- Rebalance: transfer tops
- Median: from tops

### Pattern 2: Lazy Deletion Two-Heap
- `removed` dict for O(1) mark
- Clean heaps only on access/rebalance
- Track effective sizes separately

### Pattern 3: Two-Purpose Two-Heap (IPO)
- One heap by one criterion (capital)
- Other heap by another (profit)
- Unlock eligible items from one into the other

## Interview Tips

1. **Always maintain balance invariant explicitly.** After every add/remove, call rebalance.
2. **Lazy deletion**: The `removed` dict is O(1) per mark, O(log K) amortized per clean. Key to O(N log K) sliding median.
3. **Median with even window**: Return `(lo[0] + hi[0]) / 2.0`. Use Python's true division.
4. **Refueling problem**: "Retroactive" greedy (fill retroactively at best station when stuck) is powerful mental model.
5. **Connecting sticks = Huffman**: Pattern: "repeated merging of two smallest with accumulated cost".
6. **Edge cases**: Empty lo/hi at start, single element, all equal elements — test these mentally.

## Complexity Reference

| Problem | Time | Space | Key Insight |
|---|---|---|---|
| Median from stream | O(log N) add, O(1) query | O(N) | Two balanced heaps |
| Sliding window median | O(N log K) | O(K) | Lazy deletion |
| Refueling stops | O(N log N) | O(N) | Retroactive greedy |
| Furthest building | O(N log L) | O(L) | Replace ladder with bricks |
| Connect sticks | O(N log N) | O(N) | Huffman coding |
| Meeting rooms II | O(N log N) | O(N) | End-time min-heap |

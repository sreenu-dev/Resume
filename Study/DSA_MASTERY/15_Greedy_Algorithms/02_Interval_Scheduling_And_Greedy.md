# Interval Scheduling & Advanced Greedy — Mastery Guide

> **Intervals are the most common greedy interview topic.** Sweep line, interval partitioning, jump games, candy distribution — all follow the same structural patterns. Master them all here.

---

## Table of Contents
1. [Interval Scheduling — Maximization](#scheduling-max)
2. [Interval Partitioning — Minimum Rooms](#partitioning)
3. [Weighted Interval Scheduling — Requires DP](#weighted)
4. [Sweep Line Technique](#sweep-line)
5. [Jump Games — Greedy Proofs](#jump-games)
6. [Distribution & Monotone Problems](#distribution)
7. [Problems 1–9 with Full Solutions](#problems)
8. [Greedy Problem Recognition Guide](#recognition)

---

## 1. Interval Scheduling — Maximization <a name="scheduling-max"></a>

### The Canonical Problem

**Input:** N intervals [sᵢ, eᵢ]. Select maximum number of non-overlapping intervals.

**Greedy:** Sort by finish time, greedily pick each non-overlapping interval.

**Why sort by finish time (not start time, not duration)?**
- Earliest finish → maximum room for future intervals
- Longest duration: fails (one long interval blocks many short ones)
- Earliest start: fails (start at 0, end at 100 blocks everything)

```python
def max_non_overlapping(intervals: list[list[int]]) -> int:
    """
    Maximum activity selection.
    Time: O(N log N), Space: O(1)
    """
    if not intervals:
        return 0
    
    intervals.sort(key=lambda x: x[1])  # sort by END time
    count = 1
    last_end = intervals[0][1]
    
    for start, end in intervals[1:]:
        if start >= last_end:  # non-overlapping (touching is OK)
            count += 1
            last_end = end
    
    return count

# Relationship to LeetCode 435:
# min_intervals_to_remove = N - max_non_overlapping
```

---

## 2. Interval Partitioning — Minimum Rooms <a name="partitioning"></a>

### The Problem

**Input:** N intervals (meetings). Find minimum number of "rooms" (processors, tracks) needed so all intervals can be scheduled.

**Key insight:** Minimum rooms = maximum number of simultaneously overlapping intervals.

**Greedy:** Sort by start time, use min-heap of room-end-times.

```python
import heapq

def min_meeting_rooms(intervals: list[list[int]]) -> int:
    """
    LeetCode 253. Minimum rooms for all meetings.
    
    Approach 1: Min-heap
    Time: O(N log N), Space: O(N)
    
    Sort by start time. Heap tracks end times of active rooms.
    If new meeting starts after earliest ending room → reuse that room.
    Otherwise → open new room.
    """
    if not intervals:
        return 0
    
    intervals.sort(key=lambda x: x[0])
    heap = []  # min-heap of room end times
    
    for start, end in intervals:
        if heap and heap[0] <= start:
            heapq.heapreplace(heap, end)  # reuse earliest-ending room
        else:
            heapq.heappush(heap, end)    # open new room
    
    return len(heap)

def min_meeting_rooms_sweep(intervals: list[list[int]]) -> int:
    """
    Approach 2: Sweep line — easier to understand and prove.
    Time: O(N log N), Space: O(N)
    
    Count simultaneous meetings at each event point.
    """
    events = []
    for start, end in intervals:
        events.append((start, 1))   # meeting starts: +1
        events.append((end, -1))    # meeting ends: -1
    
    # Sort: end events before start events at same time (if ties)
    events.sort(key=lambda x: (x[0], x[1]))
    
    max_rooms = 0
    current = 0
    for _, delta in events:
        current += delta
        max_rooms = max(max_rooms, current)
    
    return max_rooms

# PROOF:
# Lower bound: if k meetings overlap at some time t, need ≥ k rooms.
# Upper bound: the greedy assigns ≤ k rooms total (each room handles disjoint meetings).
# Therefore min rooms = max simultaneous overlap = max_rooms. ✓
```

---

## 3. Weighted Interval Scheduling — DP Required <a name="weighted"></a>

```python
from bisect import bisect_right

def weighted_job_scheduling(jobs: list[tuple[int,int,int]]) -> int:
    """
    Given (start, end, weight) jobs, maximize total weight, non-overlapping.
    
    WHY GREEDY FAILS: 
    jobs = [(0,3,5), (3,6,5), (0,6,8)]
    Greedy (max weight): picks (0,6,8)=8
    Optimal: (0,3,5)+(3,6,5)=10 ✓ → greedy wrong.
    
    DP + binary search: O(N log N)
    dp[i] = max weight using first i jobs (sorted by end time)
    dp[i] = max(dp[i-1], weight[i] + dp[last_compatible(i)])
    
    last_compatible(i): latest job j where end[j] ≤ start[i]
    """
    jobs.sort(key=lambda j: j[1])
    n = len(jobs)
    ends = [j[1] for j in jobs]
    
    dp = [0] * (n + 1)
    
    for i, (s, e, w) in enumerate(jobs):
        # Binary search: latest job ending ≤ s
        j = bisect_right(ends, s, 0, i)  # first job ending > s → j-1 is last compatible
        dp[i + 1] = max(dp[i], dp[j] + w)
    
    return dp[n]
```

---

## 4. Sweep Line Technique <a name="sweep-line"></a>

```python
def meeting_rooms_i(intervals: list[list[int]]) -> bool:
    """
    LeetCode 252. Can a person attend all meetings? (No overlaps allowed)
    Time: O(N log N), Space: O(1)
    """
    intervals.sort()
    for i in range(1, len(intervals)):
        if intervals[i][0] < intervals[i-1][1]:
            return False
    return True

def insert_interval(intervals: list[list[int]], new_interval: list[int]) -> list[list[int]]:
    """
    LeetCode 57. Insert and merge intervals. Assumes input is sorted and non-overlapping.
    Time: O(N), Space: O(N)
    """
    result = []
    i = 0
    n = len(intervals)
    
    # Add all intervals ending before new_interval starts
    while i < n and intervals[i][1] < new_interval[0]:
        result.append(intervals[i])
        i += 1
    
    # Merge overlapping intervals
    while i < n and intervals[i][0] <= new_interval[1]:
        new_interval[0] = min(new_interval[0], intervals[i][0])
        new_interval[1] = max(new_interval[1], intervals[i][1])
        i += 1
    result.append(new_interval)
    
    # Add remaining intervals
    while i < n:
        result.append(intervals[i])
        i += 1
    
    return result

def count_intersections(intervals1: list[list[int]], intervals2: list[list[int]]) -> int:
    """
    Count pairs (i,j) where intervals1[i] intersects intervals2[j].
    Sweep line approach.
    """
    events = []
    for s, e in intervals1:
        events.append((s, 'A_start', 0))
        events.append((e, 'A_end', 0))
    for s, e in intervals2:
        events.append((s, 'B_start', 0))
        events.append((e, 'B_end', 0))
    
    events.sort()
    count = active_a = active_b = 0
    intersections = 0
    
    for time, event_type, _ in events:
        if event_type == 'A_start':
            intersections += active_b  # new A interval crosses all active B intervals
            active_a += 1
        elif event_type == 'A_end':
            active_a -= 1
        elif event_type == 'B_start':
            intersections += active_a
            active_b += 1
        else:
            active_b -= 1
    
    return intersections
```

---

## 5. Jump Games — Greedy Proofs <a name="jump-games"></a>

```python
def jump_game_i(nums: list[int]) -> bool:
    """
    LeetCode 55. Can reach last index?
    Greedy: maintain max reachable index.
    Time: O(N), Space: O(1)
    """
    max_reach = 0
    for i in range(len(nums)):
        if i > max_reach:
            return False
        max_reach = max(max_reach, i + nums[i])
    return True

def jump_game_ii(nums: list[int]) -> int:
    """
    LeetCode 45. Minimum jumps to reach end.
    
    Greedy (BFS level-by-level):
    - current_end: furthest we can reach with current number of jumps
    - farthest: furthest reachable from any position in current "level"
    - When i reaches current_end, make one jump to farthest
    
    PROOF: At each "level" (jump count), greedy maximizes coverage.
    Any choice other than farthest-reach would give ≤ coverage at same cost.
    Therefore minimum jumps = greedy jumps. ✓
    
    Time: O(N), Space: O(1)
    """
    n = len(nums)
    if n <= 1:
        return 0
    
    jumps = 0
    current_end = 0
    farthest = 0
    
    for i in range(n - 1):
        farthest = max(farthest, i + nums[i])
        if i == current_end:
            jumps += 1
            current_end = farthest
            if current_end >= n - 1:
                break
    
    return jumps

def jump_game_iv(arr: list[int]) -> int:
    """
    LeetCode 1345. Jump to same-value positions or ±1.
    BFS (not greedy) — complex adjacency structure.
    Time: O(N), Space: O(N)
    """
    from collections import defaultdict, deque
    
    n = len(arr)
    if n == 1:
        return 0
    
    # Group indices by value
    same_val = defaultdict(list)
    for i, val in enumerate(arr):
        same_val[val].append(i)
    
    visited = {0}
    queue = deque([0])
    steps = 0
    
    while queue:
        for _ in range(len(queue)):
            idx = queue.popleft()
            
            # Neighbors: ±1 and same value
            neighbors = [idx - 1, idx + 1] + same_val[arr[idx]]
            same_val[arr[idx]] = []  # CLEAR to avoid re-visiting (key optimization!)
            
            for neighbor in neighbors:
                if neighbor == n - 1:
                    return steps + 1
                if 0 <= neighbor < n and neighbor not in visited:
                    visited.add(neighbor)
                    queue.append(neighbor)
        steps += 1
    
    return -1
```

---

## 6. Distribution & Monotone Problems <a name="distribution"></a>

```python
def candy(ratings: list[int]) -> int:
    """
    LeetCode 135. Minimum candies: each child ≥ 1, higher-rated neighbor gets more.
    
    Two-pass greedy:
    Pass 1 (left→right): ensure each child has more than left neighbor if higher-rated.
    Pass 2 (right→left): ensure each child has more than right neighbor if higher-rated.
    Take max of both passes.
    
    PROOF:
    Pass 1 satisfies left constraints. Pass 2 satisfies right constraints.
    Taking max satisfies both simultaneously, and gives minimum because:
    - Can't give less than pass 1 (violates left constraint)
    - Can't give less than pass 2 (violates right constraint)
    - This is exactly the minimum satisfying both. ✓
    
    Time: O(N), Space: O(N)
    """
    n = len(ratings)
    candies = [1] * n
    
    # Left to right: higher rating than left neighbor → more candies
    for i in range(1, n):
        if ratings[i] > ratings[i-1]:
            candies[i] = candies[i-1] + 1
    
    # Right to left: higher rating than right neighbor → more candies
    for i in range(n-2, -1, -1):
        if ratings[i] > ratings[i+1]:
            candies[i] = max(candies[i], candies[i+1] + 1)
    
    return sum(candies)

def monotone_increasing_digits(n: int) -> int:
    """
    LeetCode 738. Largest number ≤ n with monotone increasing digits.
    
    Greedy: find rightmost position where digit decreases,
    decrement it, set all following to '9'.
    
    PROOF:
    If digits d[i] > d[i+1] at any position, number violates monotone.
    Decrement d[i] and set d[i+1..] = 9:
    - Ensures d[i] ≤ d[i+1] (now d[i]-1 ≤ 9)
    - Maximizes remaining digits (all 9)
    - Result ≤ n (we changed position i from d[i] to d[i]-1)
    Work right to left to handle cascades (e.g., 200 → 199). ✓
    
    Time: O(log N), Space: O(log N)
    """
    s = list(str(n))
    m = len(s)
    mark = m  # first position to set to 9
    
    for i in range(m - 1, 0, -1):
        if s[i] < s[i-1]:
            mark = i
            s[i-1] = str(int(s[i-1]) - 1)
    
    for i in range(mark, m):
        s[i] = '9'
    
    return int(''.join(s))
```

---

## 7. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Merge Intervals (LeetCode 56)

```python
def merge_intervals(intervals: list[list[int]]) -> list[list[int]]:
    """
    LeetCode 56. Merge all overlapping intervals.
    Time: O(N log N), Space: O(N)
    """
    if not intervals:
        return []
    
    intervals.sort(key=lambda x: x[0])
    merged = [intervals[0]]
    
    for start, end in intervals[1:]:
        if start <= merged[-1][1]:
            merged[-1][1] = max(merged[-1][1], end)
        else:
            merged.append([start, end])
    
    return merged
```

---

### Problem 2: Minimum Number of Arrows to Burst Balloons (LeetCode 452)

```python
def find_min_arrow_shots(points: list[list[int]]) -> int:
    """
    LeetCode 452. Each balloon is a range. Arrow at x bursts all balloons containing x.
    Find minimum arrows.
    
    = Minimum number of points that pierce all intervals
    = N - (maximum independent set of intervals after merging)
    
    Greedy: sort by END, shoot at each balloon's end if not already burst.
    
    PROOF: After shooting at end of current balloon:
    - This balloon is burst
    - All other balloons overlapping this end are also burst (they extend to ≥ this end)
    - Shooting any later would miss this balloon
    - Shooting earlier would burst fewer overlapping balloons
    → Shooting at end of current balloon is optimal (maximize balloons per arrow). ✓
    
    Time: O(N log N), Space: O(1)
    """
    if not points:
        return 0
    
    points.sort(key=lambda x: x[1])  # sort by END
    arrows = 1
    arrow_pos = points[0][1]
    
    for start, end in points[1:]:
        if start > arrow_pos:  # current arrow doesn't burst this balloon
            arrows += 1
            arrow_pos = end
    
    return arrows
```

---

### Problem 3: Video Stitching (LeetCode 1024)

```python
def video_stitch(clips: list[list[int]], time: int) -> int:
    """
    LeetCode 1024. Stitch clips to cover [0, time]. Min clips needed.
    
    Greedy jump: at each position, find clip extending farthest.
    Same pattern as Jump Game II applied to intervals.
    
    Time: O(N log N) or O(N + time) with bucket sort
    Space: O(1)
    """
    # Sort by start, then by end (descending) to handle ties
    clips.sort(key=lambda x: (x[0], -x[1]))
    
    jumps = 0
    current_end = 0
    farthest = 0
    i = 0
    n = len(clips)
    
    while current_end < time:
        # Find clip that starts ≤ current_end and extends farthest
        while i < n and clips[i][0] <= current_end:
            farthest = max(farthest, clips[i][1])
            i += 1
        
        if farthest <= current_end:
            return -1  # can't extend coverage
        
        current_end = farthest
        jumps += 1
    
    return jumps

def video_stitch_bucket(clips: list[list[int]], time: int) -> int:
    """
    O(N + time) using bucket sort — faster when time is small.
    maxEnd[i] = farthest end among clips starting at i.
    """
    max_end = [0] * (time + 1)
    for start, end in clips:
        if start <= time:
            max_end[start] = max(max_end[start], end)
    
    jumps = 0
    current_end = 0
    farthest = 0
    
    for i in range(time):
        farthest = max(farthest, max_end[i])
        if i == current_end:
            if farthest <= i:
                return -1
            current_end = farthest
            jumps += 1
    
    return jumps
```

---

### Problem 4: Create Maximum Number from Two Arrays (LeetCode 321)

```python
def max_number(nums1: list[int], nums2: list[int], k: int) -> list[int]:
    """
    LeetCode 321. Pick k digits total from nums1 and nums2 (maintaining relative order)
    to create the maximum number.
    
    Greedy: 
    1. For each split (i digits from nums1, k-i from nums2):
       - Get max subsequence of length i from nums1
       - Get max subsequence of length k-i from nums2
       - Merge both subsequences maximally
    2. Take the maximum over all splits.
    
    Time: O(k × (N+M+k)), Space: O(k)
    """
    def max_subseq(nums: list[int], length: int) -> list[int]:
        """Get maximum subsequence of given length using monotone stack."""
        drop = len(nums) - length
        stack = []
        for num in nums:
            while drop > 0 and stack and stack[-1] < num:
                stack.pop()
                drop -= 1
            stack.append(num)
        return stack[:length]
    
    def merge(a: list[int], b: list[int]) -> list[int]:
        """Merge two sequences into maximum number."""
        result = []
        i = j = 0
        while i < len(a) or j < len(b):
            # Compare remaining sequences (not just current element)
            if a[i:] >= b[j:]:  # lexicographic comparison
                result.append(a[i])
                i += 1
            else:
                result.append(b[j])
                j += 1
        return result
    
    n1, n2 = len(nums1), len(nums2)
    best = []
    
    for i in range(max(0, k - n2), min(k, n1) + 1):
        j = k - i
        seq1 = max_subseq(nums1, i)
        seq2 = max_subseq(nums2, j)
        candidate = merge(seq1, seq2)
        if candidate > best:
            best = candidate
    
    return best
```

---

### Problem 5: Remove K Digits (LeetCode 402)

```python
def remove_k_digits(num: str, k: int) -> str:
    """
    LeetCode 402. Remove k digits to get smallest number.
    
    Greedy: maintain monotone increasing stack.
    Remove a digit if it's greater than the next (creates a descent → removing it helps).
    
    PROOF:
    To minimize the number, we want to eliminate peaks.
    For any position where num[i] > num[i+1], removing num[i] reduces the number.
    We prefer to remove leftmost peaks first (higher place value).
    Monotone stack processes this in O(N). ✓
    
    Time: O(N), Space: O(N)
    """
    stack = []
    for digit in num:
        while k > 0 and stack and stack[-1] > digit:
            stack.pop()
            k -= 1
        stack.append(digit)
    
    # If k > 0, remove from end (stack is non-decreasing)
    final = stack[:-k] if k else stack
    
    # Strip leading zeros
    result = ''.join(final).lstrip('0')
    return result or '0'
```

---

### Problem 6: Partition Labels (LeetCode 763)

```python
def partition_labels(s: str) -> list[int]:
    """
    LeetCode 763. Partition s into maximum parts where each letter appears in one part.
    
    Greedy: for each character, the partition must end at the last occurrence.
    Extend current partition's end as we scan.
    
    PROOF:
    The first character must appear in the first partition.
    That partition must contain ALL occurrences of that character (constraint).
    As we include more characters, we must extend to include all their occurrences.
    When current position reaches current end → minimal partition found. ✓
    
    Time: O(N), Space: O(26) = O(1)
    """
    # Last occurrence of each character
    last = {c: i for i, c in enumerate(s)}
    
    result = []
    start = 0
    end = 0
    
    for i, c in enumerate(s):
        end = max(end, last[c])  # extend partition to include all occurrences of c
        
        if i == end:  # current position reached partition boundary
            result.append(end - start + 1)
            start = i + 1
    
    return result
```

---

### Problem 7: Jump Game VI — Priority Queue Greedy (LeetCode 1696)

```python
from collections import deque

def max_result(nums: list[int], k: int) -> int:
    """
    LeetCode 1696. Jump at most k positions, maximize sum.
    
    dp[i] = max score ending at index i
    dp[i] = nums[i] + max(dp[j] for j in [i-k, i-1])
    
    Monotone deque maintains max in sliding window.
    Time: O(N), Space: O(k)
    """
    n = len(nums)
    dp = [0] * n
    dp[0] = nums[0]
    dq = deque([0])  # indices, dp[dq[0]] is max in window
    
    for i in range(1, n):
        # Remove indices outside window
        while dq and dq[0] < i - k:
            dq.popleft()
        
        dp[i] = dp[dq[0]] + nums[i]
        
        # Maintain decreasing order in deque
        while dq and dp[dq[-1]] <= dp[i]:
            dq.pop()
        dq.append(i)
    
    return dp[n-1]
```

---

### Problem 8: Smallest Number After Removing K Digits (Revisited with Analysis)

```python
def remove_k_digits_full(num: str, k: int) -> str:
    """
    Full analysis of the greedy correctness:
    
    Claim: removing the first digit that is greater than its successor minimizes the number.
    
    Proof:
    Let num = d₁d₂...dₙ. We remove one digit.
    Removing dᵢ gives: d₁...dᵢ₋₁dᵢ₊₁...dₙ (n-1 digits)
    
    Which removal is best? Compare results:
    - Remove dᵢ vs remove dⱼ (i < j)
    - result_i = ...dᵢ₋₁dᵢ₊₁... vs result_j = ...dⱼ₋₁dⱼ₊₁...
    - They agree on positions 1..i-1.
    - At position i: result_i has dᵢ₊₁, result_j has dᵢ.
    - If dᵢ > dᵢ₊₁: result_i < result_j (removing earlier peak is better).
    - So: we should remove the LEFTMOST position where dᵢ > dᵢ₊₁.
    - This is exactly what the monotone stack does. ✓
    """
    return remove_k_digits(num, k)  # refer to Problem 5 above
```

---

### Problem 9: Candy Distribution (Two-Pass Greedy)

```python
def candy_optimized(ratings: list[int]) -> int:
    """
    LeetCode 135. Already shown above, with O(1) space alternative.
    O(1) space approach: compute without arrays using valley/peak analysis.
    """
    n = len(ratings)
    total = 1
    up = 0
    down = 0
    peak = 0
    
    for i in range(1, n):
        if ratings[i] > ratings[i-1]:
            up += 1; down = 0; peak = up
            total += up + 1
        elif ratings[i] < ratings[i-1]:
            up = 0; down += 1
            # If peak is not high enough, need to give more to peak
            total += down + 1 + (1 if peak >= down else 0)
        else:
            up = 0; down = 0; peak = 0
            total += 1
    
    return total
```

---

## 8. Greedy Problem Recognition Guide <a name="recognition"></a>

### Pattern-Problem Mapping

```
SORT + GREEDY PATTERNS:
──────────────────────
Sort by finish time → Activity selection, non-overlapping intervals
Sort by start time  → Meeting rooms, interval merging
Sort by value/weight ratio → Fractional knapsack
Sort by height desc + k asc → Queue reconstruction
Sort by end, then use monotone stack → Balloon arrows, burst balloons

SWEEP LINE PATTERNS:
────────────────────
+1 at start, -1 at end → Meeting rooms (count max overlap)
+1 at start, -1 at end → Event scheduling

MONOTONE STACK PATTERNS:
────────────────────────
Maintain increasing stack → Remove k digits (minimize number)
Maintain decreasing stack → Next greater element
Window max/min → Sliding window with deque

JUMP GAME PATTERNS:
───────────────────
Max reach tracking → Jump Game I (reachability)
Level-by-level BFS → Jump Game II (minimum jumps)
DP + deque → Jump Game VI (maximization with window)

TWO-PASS PATTERNS:
──────────────────
Left → right pass, right → left pass, take max → Candy distribution
```

### Complexity Reference

| Problem | Approach | Time | Space |
|---------|----------|------|-------|
| Activity selection | Sort + scan | O(N log N) | O(1) |
| Meeting rooms I | Sort + scan | O(N log N) | O(1) |
| Meeting rooms II | Sort + heap | O(N log N) | O(N) |
| Merge intervals | Sort + merge | O(N log N) | O(N) |
| Insert interval | Linear scan | O(N) | O(N) |
| Min arrows | Sort + scan | O(N log N) | O(1) |
| Video stitching | Sort + jump | O(N log N) | O(1) |
| Candy | Two-pass | O(N) | O(N) |
| Remove K digits | Monotone stack | O(N) | O(N) |
| Partition labels | Last occurrence | O(N) | O(1) |
| Jump Game I | Max reach | O(N) | O(1) |
| Jump Game II | BFS levels | O(N) | O(1) |
| Weighted interval | DP + bisect | O(N log N) | O(N) |

### Decision: Greedy or DP?

```
Test 1: Does the greedy choice have exchange argument support?
  YES → Try greedy.
  NO  → Likely DP.

Test 2: Counterexample search.
  Try small inputs where greedy would fail.
  If you find one → DP needed.

Test 3: Is there optimal substructure + greedy choice property?
  Both YES → Greedy works.
  Only substructure → DP.
  
Classic traps:
  - Coin change with arbitrary coins: DP (not greedy)
  - 0/1 Knapsack: DP (not greedy)  
  - Fractional knapsack: Greedy ✓
  - Edit distance: DP (not greedy)
  - Shortest path (non-negative): Dijkstra (greedy) ✓
  - Shortest path (negative edges): Bellman-Ford (DP) needed
```

### Final Interview Advice

> **"Should I use greedy here?"** — State the greedy choice, give a one-sentence exchange argument, and offer a counterexample test. If the interviewer accepts, code it. If not, pivot to DP.

> **"Prove your greedy is correct."** — Walk through: (1) Sort choice justification, (2) Exchange argument at one step, (3) Inductive conclusion.

> **"What's the time complexity?"** — For most interval problems: O(N log N) for the sort, O(N) for the greedy scan. Total: O(N log N).

> **"Can we do better than O(N log N)?"** — If the intervals arrive pre-sorted or the values are bounded integers (bucket sort), yes: O(N). Otherwise, sorting is the bottleneck.

---

*Previous: [Greedy Proof Techniques ←](01_Greedy_Proof_Techniques.md)*

---

## Appendix: Quick Reference Problems

```python
# All problems covered, quick reference:
# LeetCode 56:  Merge Intervals            → Sort + merge
# LeetCode 57:  Insert Interval            → Three-part scan
# LeetCode 252: Meeting Rooms              → Sort by start
# LeetCode 253: Meeting Rooms II           → Sort + heap
# LeetCode 435: Non-Overlapping Intervals  → Sort by end
# LeetCode 452: Min Arrows                 → Sort by end
# LeetCode 55:  Jump Game                  → Max reach
# LeetCode 45:  Jump Game II               → BFS levels  
# LeetCode 1696:Jump Game VI               → DP + deque
# LeetCode 1024:Video Stitching            → Sort + jump
# LeetCode 135: Candy                      → Two-pass
# LeetCode 738: Monotone Increasing Digits → Greedy stack
# LeetCode 402: Remove K Digits            → Monotone stack
# LeetCode 763: Partition Labels           → Last occurrence scan
# LeetCode 321: Create Maximum Number      → Max subseq + merge
```

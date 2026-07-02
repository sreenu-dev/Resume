# Monotonic Stack & Contribution Technique — Deep Mastery

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Stack fundamentals, amortized analysis
> **Core Theme:** Exploiting monotonic invariants to solve range-query and
> subarray problems in O(N) instead of O(N²).

---

## 1. Monotonic Stack — Invariant Proof

### Definition

A **monotone decreasing stack** maintains elements in decreasing order
from bottom to top. When a new element `x` arrives:

1. Pop all elements ≤ x from the top (for strictly decreasing)
2. Push `x`

**Invariant:** At any point, the stack is sorted descending from bottom.

### Next Greater Element — Formal Derivation

**Claim:** When element `stack[-1]` is popped because `A[i] > stack[-1]`,
then `A[i]` is the **Next Greater Element (NGE)** of `stack[-1]`.

**Proof:** Between when `stack[-1]` was pushed (at index `j`) and now
(index `i`), all elements between `j` and `i` were popped — meaning all
`A[j+1..i-1] ≤ A[j]`. Combined with `A[i] > A[j]`, `A[i]` is the first
element to the right of `j` greater than `A[j]`. □

```python
def nextGreaterElement(nums: list[int]) -> list[int]:
    n = len(nums)
    result = [-1] * n
    stack = []   # Monotone decreasing stack of indices

    for i in range(n):
        while stack and nums[i] > nums[stack[-1]]:
            idx = stack.pop()
            result[idx] = nums[i]
        stack.append(i)

    return result
```
**Time:** O(N) amortized — each element pushed/popped at most once
**Space:** O(N)

### Next Smaller Element (NSE)

```python
def nextSmallerElement(nums: list[int]) -> list[int]:
    n = len(nums)
    result = [-1] * n
    stack = []   # Monotone increasing stack of indices

    for i in range(n):
        while stack and nums[i] < nums[stack[-1]]:
            idx = stack.pop()
            result[idx] = nums[i]
        stack.append(i)

    return result
```
**Time:** O(N) | **Space:** O(N)

### Previous Greater / Previous Smaller

Process left-to-right; when you push `i`, the current stack top is the
**Previous Greater** (PGE) or **Previous Smaller** (PSE) depending on stack type.

---

## 2. Contribution Technique — Sum of Subarray Minimums/Maximums

### Problem (LeetCode 907)

Compute `Σ min(subarray)` for all subarrays of `A`. Return mod 1e9+7.

### Key Insight

For each element `A[i]`, count how many subarrays have `A[i]` as their minimum.

**Boundaries:**
- `left[i]` = distance to the **Previous Smaller or Equal** element (use ≤ on left)
- `right[i]` = distance to the **Next Smaller** element (use < on right)

**Count of subarrays where A[i] is minimum:** `left[i] × right[i]`

```python
def sumSubarrayMins(arr: list[int]) -> int:
    MOD = 10**9 + 7
    n = len(arr)

    left = [0] * n
    stack = []
    for i in range(n):
        while stack and arr[stack[-1]] >= arr[i]:
            stack.pop()
        left[i] = i - stack[-1] if stack else i + 1
        stack.append(i)

    right = [0] * n
    stack = []
    for i in range(n - 1, -1, -1):
        while stack and arr[stack[-1]] > arr[i]:
            stack.pop()
        right[i] = stack[-1] - i if stack else n - i
        stack.append(i)

    return sum(arr[i] * left[i] * right[i] for i in range(n)) % MOD
```
**Time:** O(N) | **Space:** O(N)

**Duplicate handling:** Using `≥` on left and `>` on right ensures that
for duplicates, only the rightmost occurrence is counted — avoids double counting.

---

## 3. Online Stock Span (LeetCode 901)

```python
class StockSpanner:
    def __init__(self):
        self.stack = []    # (price, span)

    def next(self, price: int) -> int:
        span = 1
        while self.stack and self.stack[-1][0] <= price:
            span += self.stack.pop()[1]
        self.stack.append((price, span))
        return span
```
**Time:** O(1) amortized per call | **Space:** O(N)

**Key insight:** When a previously accumulated span is "absorbed" by a
higher price, we don't need to re-examine those days — the span stores the compressed count.

---

## 4. Buildings with Ocean View (LeetCode 1762)

```python
def findBuildings(heights: list[int]) -> list[int]:
    result = []
    max_height = 0

    for i in range(len(heights) - 1, -1, -1):
        if heights[i] > max_height:
            result.append(i)
            max_height = heights[i]

    return result[::-1]
```
**Time:** O(N) | **Space:** O(1) extra

**Monotonic stack variant:**
```python
def findBuildingsStack(heights: list[int]) -> list[int]:
    stack = []
    for i, h in enumerate(heights):
        while stack and heights[stack[-1]] <= h:
            stack.pop()
        stack.append(i)
    return stack
```
**Time:** O(N) | **Space:** O(N)

---

## 5. Maximum Width Ramp (LeetCode 962)

**Two-pass monotonic technique:**

```python
def maxWidthRamp(nums: list[int]) -> int:
    n = len(nums)

    # Build decreasing stack of potential left boundaries
    stack = []
    for i in range(n):
        if not stack or nums[i] < nums[stack[-1]]:
            stack.append(i)

    # Scan from right, try to pair with stack top
    max_width = 0
    for j in range(n - 1, -1, -1):
        while stack and nums[j] >= nums[stack[-1]]:
            max_width = max(max_width, j - stack.pop())

    return max_width
```
**Time:** O(N) | **Space:** O(N)

---

## 6. Largest Rectangle in Histogram (LeetCode 84)

```python
def largestRectangleInHistogram(heights: list[int]) -> int:
    stack = []
    max_area = 0
    heights = heights + [0]     # Sentinel to flush remaining bars

    for i, h in enumerate(heights):
        while stack and heights[stack[-1]] > h:
            height = heights[stack.pop()]
            width = i - (stack[-1] + 1 if stack else 0)
            max_area = max(max_area, height * width)
        stack.append(i)

    return max_area
```
**Time:** O(N) | **Space:** O(N)

**Width formula derivation:**
- Popped bar was at index `j = stack.pop()`
- Right boundary (exclusive): current index `i`
- Left boundary (exclusive): new stack top `stack[-1]`
- Width = `i - stack[-1] - 1`

**Sentinel `0` at end:** Ensures all remaining bars in stack get processed.

---

## 7. Maximal Rectangle in Binary Matrix (LeetCode 85)

```python
def maximalRectangle(matrix: list[list[str]]) -> int:
    if not matrix or not matrix[0]:
        return 0

    cols = len(matrix[0])
    heights = [0] * cols
    max_area = 0

    for row in matrix:
        for j in range(cols):
            heights[j] = heights[j] + 1 if row[j] == '1' else 0

        max_area = max(max_area, largestRectangleInHistogram(heights[:]))

    return max_area
```
**Time:** O(R × C) | **Space:** O(C)

---

## 8. Remove Duplicate Letters (LeetCode 316)

```python
def removeDuplicateLetters(s: str) -> str:
    from collections import Counter

    count = Counter(s)
    in_stack = set()
    stack = []

    for ch in s:
        count[ch] -= 1

        if ch in in_stack:
            continue

        while stack and stack[-1] > ch and count[stack[-1]] > 0:
            in_stack.discard(stack.pop())

        stack.append(ch)
        in_stack.add(ch)

    return ''.join(stack)
```
**Time:** O(N) | **Space:** O(1) — at most 26 characters in stack

**Invariant:** The stack maintains a monotone increasing sequence.
We only pop a character if it appears later (count > 0), ensuring we don't lose it.

---

## 9. 132 Pattern (LeetCode 456)

```python
def find132pattern(nums: list[int]) -> bool:
    stack = []
    third = float('-inf')

    for num in reversed(nums):
        if num < third:
            return True
        while stack and stack[-1] < num:
            third = stack.pop()
        stack.append(num)

    return False
```
**Time:** O(N) | **Space:** O(N)

**Why right-to-left:** When we see a number larger than the stack top,
the popped element becomes a candidate for `nums[k]`. Any number smaller
than `third` encountered later (earlier in original order) is `nums[i]`.

---

## 10. Decode String (LeetCode 394)

```python
def decodeString(s: str) -> str:
    count_stack = []
    str_stack = []
    current = ""
    k = 0

    for ch in s:
        if ch.isdigit():
            k = k * 10 + int(ch)
        elif ch == '[':
            count_stack.append(k)
            str_stack.append(current)
            k = 0
            current = ""
        elif ch == ']':
            times = count_stack.pop()
            current = str_stack.pop() + current * times
        else:
            current += ch

    return current
```
**Time:** O(N × max_k) | **Space:** O(depth)

---

## Complexity Summary

| Problem | Time | Space | Technique |
|---|---|---|---|
| NGE/NSE | O(N) | O(N) | Monotone stack, each elem pushed/popped once |
| Sum subarray mins | O(N) | O(N) | Contribution × left/right boundaries |
| Stock span | O(1) amortized | O(N) | Compressed span accumulation |
| Buildings with view | O(N) | O(N) | Decreasing stack |
| Max width ramp | O(N) | O(N) | Decreasing prefix stack + right scan |
| Largest rectangle | O(N) | O(N) | Increasing stack + sentinel |
| Maximal rectangle | O(R×C) | O(C) | Per-row histogram |
| Remove duplicates | O(N) | O(26) | Greedy stack + future-count |
| 132 pattern | O(N) | O(N) | Right-to-left + third tracking |

## Interview Tips

1. **Monotonic stack = deferred decisions**: You don't know a bar's right
   boundary until a shorter bar arrives. Stack lets you "pause" the decision.
2. **Contribution technique**: Whenever a problem asks "sum over all
   subarrays of some aggregate", think "per-element contribution × count".
3. **Duplicate handling**: Always use `≤` on one side and `<` on the other.
4. **Sentinel value**: Append 0 to histogram problems to avoid post-loop cleanup code.
5. **132 Pattern**: The "third" variable is the key — updated lazily as larger elements are discovered.

# Fast & Slow Pointers — Floyd's Algorithm & Cycle Detection Mastery

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Linked list internals, modular arithmetic, proof by invariant
> **Core Theme:** Two-pointer techniques on linked lists where positional arithmetic
> on a cyclic structure yields elegant O(1)-space solutions.

---

## 1. Floyd's Cycle Detection — Formal Proof of Correctness

### The Setup

Let a linked list have a **tail** of length **μ** (mu) before the cycle begins,
and a **cycle** of length **λ** (lambda).

```
HEAD → [0] → [1] → ... → [μ-1] → [μ] → ... → [μ+λ-1]
                                      ↑_________________________↑
```

- `slow` advances 1 step/iteration
- `fast` advances 2 steps/iteration

### Theorem 1: They Must Meet Inside the Cycle

**Proof by modular arithmetic:**

After `t` iterations:
- `slow` is at position `t` (mod λ, once past μ)
- `fast` is at position `2t` (mod λ, once past μ)

For them to meet inside the cycle, we need:
`2t ≡ t (mod λ)` ⟹ `t ≡ 0 (mod λ)`

The smallest such `t > μ` is `t = μ + λ`. Since fast enters the cycle at
step `μ`, both pointers are guaranteed to meet within `μ + λ` steps.

**Why fast doesn't "skip over" slow:** At each step, the gap between them
(measured inside the cycle) decreases by exactly 1. Once fast is in the cycle
and slow enters it, fast chases slow and the distance closes monotonically.

### Theorem 2: Distance to Cycle Start = μ

**The key insight for finding the cycle start node:**

When `slow` and `fast` meet at some node `M` inside the cycle:

- `slow` traveled: `μ + k` steps (where `k` is position inside cycle)
- `fast` traveled: `2(μ + k)` steps

Since fast traveled exactly `n` full cycles more than slow:
`2(μ + k) = μ + k + nλ`
`μ + k = nλ`
`μ = nλ - k`

Now place a third pointer at HEAD. Move it `μ` steps. Move the meeting pointer
`μ` steps too. The meeting pointer moves from position `k` in cycle to position
`k + μ = k + nλ - k = nλ ≡ 0 (mod λ)` — **exactly the cycle start!**

**Corollary:** Move one pointer from HEAD and one from meeting point at equal
speed → they meet at the cycle start.

---

## 2. Detect Cycle

```python
class ListNode:
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next

def hasCycle(head: ListNode) -> bool:
    slow = fast = head
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
        if slow is fast:
            return True
    return False
```
**Time:** O(μ + λ) = O(N) | **Space:** O(1)

---

## 3. Find Cycle Start Node (LeetCode 142)

```python
def detectCycle(head: ListNode) -> ListNode:
    slow = fast = head

    # Phase 1: Find meeting point inside cycle
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
        if slow is fast:
            break
    else:
        return None  # No cycle

    # Phase 2: Find cycle entrance
    # Theorem 2: distance(head→start) == distance(meeting→start)
    pointer = head
    while pointer is not slow:
        pointer = pointer.next
        slow = slow.next

    return pointer
```
**Time:** O(N) two passes of at most O(μ + λ) | **Space:** O(1)

**Edge cases:**
- Single node with self-loop: meeting happens after first step
- Cycle at position 0 (μ=0): Phase 2 returns head immediately since
  pointer starts at head and slow is already at cycle start

---

## 4. Find Duplicate Number — Cycle Detection in Array (LeetCode 287)

**The elegant reduction:** Array `nums` of length `n+1` with values `1..n`.
Treat index as node, `nums[index]` as next pointer. By pigeonhole there is
a duplicate, which creates a cycle in this implicit linked list.

```python
def findDuplicate(nums: list[int]) -> int:
    # Phase 1: Find meeting point
    slow = fast = nums[0]
    while True:
        slow = nums[slow]
        fast = nums[nums[fast]]
        if slow == fast:
            break

    # Phase 2: Find cycle entrance = duplicate number
    slow = nums[0]
    while slow != fast:
        slow = nums[slow]
        fast = nums[fast]

    return slow
```
**Time:** O(N) | **Space:** O(1)

**Why this works:** The duplicate value `d` means `nums[i] = d` and
`nums[j] = d` for `i ≠ j`. Node `d` has two "parents" — creating a
ρ-shaped structure with the cycle start at `d`.

**Why NOT to sort or use a hash set:**
- Sorting: O(N log N) — worse time
- Hash set: O(N) space — violates constraints
- Marking visited by negation: modifies input — violates read-only constraint

**Constraints that enable the trick:**
- Values in range [1, n] ensures no zero-loop artifact
- Exactly one duplicate guarantees a single cycle

---

## 5. Happy Number (LeetCode 202)

**Insight:** If not happy, the digit-square-sum sequence enters a known cycle
containing 4. Floyd detects it without knowing the cycle members.

```python
def isHappy(n: int) -> bool:
    def digit_square_sum(x: int) -> int:
        total = 0
        while x:
            x, d = divmod(x, 10)
            total += d * d
        return total

    slow = n
    fast = digit_square_sum(n)

    while fast != 1 and slow != fast:
        slow = digit_square_sum(slow)
        fast = digit_square_sum(digit_square_sum(fast))

    return fast == 1
```
**Time:** O(log N) per step, bounded total steps | **Space:** O(1)

**Advanced note:** The cycle for unhappy numbers always contains 4, 16, 37,
58, 89, 145, 42, 20. Floyd's works without needing this fact.

---

## 6. Find Middle of Linked List (LeetCode 876)

For even-length lists, two valid midpoints exist. Know which one matters.

```python
def middleNode(head: ListNode) -> ListNode:
    """Returns SECOND middle for even-length lists."""
    slow = fast = head
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
    return slow
    # [1,2,3,4,5,6] → node(4)  [1,2,3,4,5] → node(3)

def middleNodeFirst(head: ListNode) -> ListNode:
    """Returns FIRST middle for even-length lists."""
    slow = fast = head
    while fast.next and fast.next.next:
        slow = slow.next
        fast = fast.next.next
    return slow
    # [1,2,3,4,5,6] → node(3)  [1,2,3,4,5] → node(3)
```
**Time:** O(N) | **Space:** O(1)

**When you need first middle:** Merge sort on linked list requires splitting
at first middle so the two halves have equal length or left side is larger
by 1 for odd-length input. Using second middle leaves the right side one
longer, which still works but is less conventional.

---

## 7. Palindrome Linked List — O(1) Space (LeetCode 234)

**Strategy:** Find first middle → reverse second half → compare → restore.

```python
def isPalindrome(head: ListNode) -> bool:
    # Step 1: Find end of first half (first-middle variant)
    slow = fast = head
    while fast.next and fast.next.next:
        slow = slow.next
        fast = fast.next.next

    # Step 2: Reverse second half in-place
    def reverse(node):
        prev = None
        while node:
            nxt = node.next
            node.next = prev
            prev = node
            node = nxt
        return prev

    second_half_start = reverse(slow.next)

    # Step 3: Compare both halves
    p1, p2 = head, second_half_start
    result = True
    while p2:          # second half is shorter or equal
        if p1.val != p2.val:
            result = False
            break
        p1 = p1.next
        p2 = p2.next

    # Step 4: Restore the list (good interview practice)
    slow.next = reverse(second_half_start)

    return result
```
**Time:** O(N) | **Space:** O(1)

**Pitfall:** Using `fast and fast.next` (second-middle variant) causes
incorrect comparison for even-length palindromes. Always use first-middle here.

---

## 8. Reorder List (LeetCode 143)

**Pattern:** Find middle + reverse second half + merge alternately.

```python
def reorderList(head: ListNode) -> None:
    if not head or not head.next:
        return

    # Step 1: Find first middle
    slow = fast = head
    while fast.next and fast.next.next:
        slow = slow.next
        fast = fast.next.next

    # Step 2: Reverse second half
    prev, curr = None, slow.next
    slow.next = None          # Sever the two halves
    while curr:
        nxt = curr.next
        curr.next = prev
        prev = curr
        curr = nxt

    # Step 3: Interleave merge: L0→Ln→L1→Ln-1→...
    first, second = head, prev
    while second:
        tmp1, tmp2 = first.next, second.next
        first.next = second
        second.next = tmp1
        first = tmp1
        second = tmp2
```
**Time:** O(N) | **Space:** O(1)

**Trace:** `1→2→3→4→5`
- Middle: node 3, second half: `4→5` → reversed: `5→4`
- Merge: `1→5→2→4→3` ✓

---

## 9. Intersection of Two Linked Lists (LeetCode 160)

**The O(1)-space proof:** After both pointers traverse their respective list
and switch, they accumulate equal total path lengths.

```python
def getIntersectionNode(headA: ListNode, headB: ListNode) -> ListNode:
    a, b = headA, headB
    while a is not b:
        a = a.next if a else headB
        b = b.next if b else headA
    return a   # None if no intersection
```
**Time:** O(M + N) | **Space:** O(1)

**Mathematical proof:**
Let len(A before intersection) = a, len(B before intersection) = b,
shared tail = c.
- Pointer A: travels `a + c + b` before reaching intersection second time
- Pointer B: travels `b + c + a` before reaching intersection second time
Both equal `a + b + c` → synchronize at intersection node.

**No intersection case:** Both reach `None` after exactly `M + N` steps.
`None is None` is `True`, loop exits cleanly.

---

## 10. Split Linked List in Parts (LeetCode 725)

```python
def splitListToParts(head: ListNode, k: int) -> list:
    # Count length
    length, curr = 0, head
    while curr:
        length += 1
        curr = curr.next

    base_size, remainder = divmod(length, k)

    result = []
    curr = head
    for i in range(k):
        part_head = curr
        # First 'remainder' parts receive one extra node
        part_size = base_size + (1 if i < remainder else 0)

        for _ in range(part_size - 1):
            if curr:
                curr = curr.next

        if curr:
            curr.next, curr = None, curr.next   # Cut and advance

        result.append(part_head)

    return result
```
**Time:** O(N + k) | **Space:** O(k) for result array

**Interview insight:** The `divmod` cleanly handles uneven splits. Parts at
indices `[0, remainder)` have `base_size + 1` nodes; the rest have `base_size`.

---

## 11. Rotate Linked List by K (LeetCode 61)

**Key insight:** Rotate right by k ≡ break the circular list at position `n − (k % n)`.

```python
def rotateRight(head: ListNode, k: int) -> ListNode:
    if not head or not head.next or k == 0:
        return head

    # Find length and tail
    length, tail = 1, head
    while tail.next:
        tail = tail.next
        length += 1

    k = k % length
    if k == 0:
        return head     # No rotation needed

    # New tail is (length - k - 1) steps from head
    new_tail = head
    for _ in range(length - k - 1):
        new_tail = new_tail.next

    new_head = new_tail.next
    new_tail.next = None
    tail.next = head    # Form cycle, then break at new_tail

    return new_head
```
**Time:** O(N) | **Space:** O(1)

**Common bug:** Forgetting `k % length`. When `k == length`, the list is
unchanged. When `k > length`, the effective rotation is `k % length`.

---

## 12. Find Cycle Length + Start — Unified Solution

```python
def analyze_cycle(head: ListNode):
    """
    Returns (cycle_start_node, cycle_length) or (None, 0).
    """
    slow = fast = head

    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
        if slow is fast:
            # Measure cycle length from meeting point
            cycle_len = 1
            probe = slow.next
            while probe is not slow:
                probe = probe.next
                cycle_len += 1

            # Find cycle start node
            ptr = head
            while ptr is not slow:
                ptr = ptr.next
                slow = slow.next

            return ptr, cycle_len

    return None, 0
```
**Time:** O(N) | **Space:** O(1)

---

## Pattern Recognition Table

| Problem Signature | Technique |
|---|---|
| Cycle detection, no extra space | Floyd's slow/fast |
| Find cycle start | Two-phase Floyd (Theorem 2) |
| Find middle element | slow×1 fast×2 pointers |
| Palindrome check O(1) space | Middle + reverse + compare |
| Two-list intersection | Path equalization trick |
| Duplicate in array [1..n] | Implicit linked list → cycle detection |
| Rotate list | Circular link + break at `n-k` |
| Split into k parts | divmod for base size |

---

## Edge Cases Checklist

- [ ] Empty list (head = None)
- [ ] Single node, no cycle
- [ ] Single node, self-cycle (node.next = node) → μ=0, λ=1
- [ ] Two nodes, cycle
- [ ] Cycle at head (μ = 0): pointer and slow both at head in Phase 2
- [ ] Cycle at last node
- [ ] k = 0 or k = length for rotation (no-op)
- [ ] All same-value elements for palindrome (should return True)
- [ ] Two-node palindrome

## Interview Tips

1. **Always draw the pointer diagram** for these problems under pressure.
2. **Theorem 2** is the most impressive thing to explain to an interviewer — work through the modular arithmetic once until it's fluent.
3. **Palindrome check**: The `restore` step after comparison shows interview maturity (you don't mutate the input permanently).
4. **Find duplicate**: This is O(1) space vs O(N) hash set — always clarify constraints first to show awareness.

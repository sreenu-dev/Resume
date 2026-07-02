# Microsoft Interview Patterns — Advanced Mastery Guide

> **Level:** Advanced | **Target:** SWE / SDE roles, L59–L67  
> **Interview Frequency:** ★★★★★ (Core company guide)

---

## Table of Contents
1. [Microsoft's Interview Philosophy](#1-microsofts-interview-philosophy)
2. [Microsoft's Format — Coding + Design + Behavioral](#2-microsofts-format--coding--design--behavioral)
3. [Microsoft's Coding Focus](#3-microsofts-coding-focus)
4. [Microsoft's OOP/Design Emphasis](#4-microsofts-oopdesign-emphasis)
5. [Problem 1: Design a Parking Lot](#5-problem-1-design-a-parking-lot)
6. [Problem 2: Implement Stack Using Queues](#6-problem-2-implement-stack-using-queues)
7. [Problem 3: LRU Cache](#7-problem-3-lru-cache)
8. [Problem 4: Zigzag Level Order Traversal](#8-problem-4-zigzag-level-order-traversal)
9. [Problem 5: Longest Substring Without Repeating Characters](#9-problem-5-longest-substring-without-repeating-characters)
10. [Problem 6: Copy List with Random Pointer](#10-problem-6-copy-list-with-random-pointer)
11. [Problem 7: Flatten Nested List Iterator](#11-problem-7-flatten-nested-list-iterator)
12. [Microsoft System Design Snippets](#12-microsoft-system-design-snippets)
13. [Microsoft-Specific Interview Tips](#13-microsoft-specific-interview-tips)

---

## 1. Microsoft's Interview Philosophy

Microsoft values a blend of **software engineering craft** and **algorithmic thinking**. Key characteristics:

- **Software engineering emphasis**: OOP design, code organization, maintainability matter more here than at Google.
- **Collaborative style**: Microsoft interviewers are generally collaborative — they'll give hints proactively.
- **Moderate algorithmic bar**: Medium-difficulty algorithms, rarely hard algorithms (unlike Google).
- **Design rounds critical**: LLD (Low-Level Design) questions appear in most onsite loops.
- **Behavioral integration**: "Growth mindset" is a Microsoft cultural value — show learning agility.

```
Microsoft's Hiring Levels:
L59 (SWE I):   Medium problems, basic OOP, clear communication
L60 (SWE II):  Medium-hard, design basics, mentorship signals
L62 (SWE III): Hard problems, system design, technical leadership
L63 (Principal): Architecture, cross-team technical decisions
```

---

## 2. Microsoft's Format — Coding + Design + Behavioral

```
Microsoft's interview loop (typically 4-5 rounds):
  1. Technical Phone Screen (45 min, 1-2 coding problems)
  2. Onsite Round 1 — Coding (45-60 min)
  3. Onsite Round 2 — Coding + Design (45-60 min)
  4. Onsite Round 3 — System/OOP Design (60 min)
  5. Onsite Round 4 — "As Appropriate" (extra coding or behavioral)

Microsoft as Appropriate (AA) interview:
  - Senior interviewer, often broader than coding
  - Tests potential for growth, leadership, cross-team impact
  
Microsoft's online assessment (for new grads):
  - HackerRank platform
  - 2 problems in 60-90 minutes
  - Usually 1 medium + 1 medium-hard
```

### Microsoft's Cultural Values to Show

```
Growth Mindset:
"I'm not sure about this approach. Let me think through alternatives
and explain my reasoning as I go."

Collaboration:
"I notice there's a trade-off here between [A] and [B].
Which is more important in this context?"

Customer-Focused Engineering:
"Before I start, I'm thinking about how this API will be used.
If callers typically [X], then [design decision] makes more sense."

Diversity and Inclusion:
"I'd name this variable 'userPreferences' rather than 'userProfiles'
to be more inclusive of different user experiences."
```

---

## 3. Microsoft's Coding Focus

| Topic | Microsoft Frequency | Notes |
|-------|--------------------|-|
| Trees (all types) | ★★★★★ | Most common across all rounds |
| Strings | ★★★★☆ | Substrings, parsing, manipulation |
| Graphs (BFS/DFS) | ★★★★☆ | Islands, connectivity |
| DP (medium) | ★★★★☆ | Usually not advanced DP |
| Linked Lists | ★★★★☆ | Reverse, merge, cycle, random ptr |
| Sorting / arrays | ★★★☆☆ | Two-pointer, intervals |
| OOP Design | ★★★★★ | Always present in some form |
| System Design | ★★★★☆ | L60+ mandatory |
| Bit manipulation | ★★☆☆☆ | Occasional |
| Advanced DS | ★★☆☆☆ | Segment tree rare |

---

## 4. Microsoft's OOP/Design Emphasis

```python
# Microsoft values OOP quality in coding problems:

# Example: They might ask "implement LRU Cache" and specifically
# expect you to use a class, with proper encapsulation

class LRUCache:
    """
    Microsoft expects:
    1. Private attributes with underscore prefix
    2. Type hints
    3. Docstrings
    4. Error handling
    5. Clean method names
    """
    
    def __init__(self, capacity: int) -> None:
        """Initialize with given capacity."""
        if capacity <= 0:
            raise ValueError(f"Capacity must be positive, got {capacity}")
        self._capacity = capacity
        self._cache: dict = {}
        self._order: list = []  # Simplified; production uses DLL
    
    def get(self, key: int) -> int:
        """Return value for key, or -1 if not found."""
        if key not in self._cache:
            return -1
        # Move to most recently used
        self._order.remove(key)
        self._order.append(key)
        return self._cache[key]
    
    def put(self, key: int, value: int) -> None:
        """Insert or update key-value pair."""
        if key in self._cache:
            self._order.remove(key)
        elif len(self._cache) >= self._capacity:
            # Evict least recently used
            lru_key = self._order.pop(0)
            del self._cache[lru_key]
        self._cache[key] = value
        self._order.append(key)
    
    @property
    def size(self) -> int:
        """Current number of elements."""
        return len(self._cache)
    
    def __repr__(self) -> str:
        return f"LRUCache(capacity={self._capacity}, size={self.size})"
```

---

## 5. Problem 1: Design a Parking Lot

**Frequency at Microsoft:** ★★★★★ | **Type:** OOP Design

```python
from enum import Enum, auto
from abc import ABC, abstractmethod
from typing import Optional
from datetime import datetime

class VehicleType(Enum):
    MOTORCYCLE = auto()
    CAR = auto()
    TRUCK = auto()

class SpotSize(Enum):
    COMPACT = auto()
    MEDIUM = auto()
    LARGE = auto()

class Vehicle(ABC):
    """Abstract base class for vehicles."""
    
    def __init__(self, license_plate: str):
        self._license_plate = license_plate
    
    @property
    def license_plate(self) -> str:
        return self._license_plate
    
    @property
    @abstractmethod
    def type(self) -> VehicleType: ...
    
    @property
    @abstractmethod
    def required_spot_size(self) -> SpotSize: ...

class Motorcycle(Vehicle):
    @property
    def type(self) -> VehicleType: return VehicleType.MOTORCYCLE
    @property
    def required_spot_size(self) -> SpotSize: return SpotSize.COMPACT

class Car(Vehicle):
    @property
    def type(self) -> VehicleType: return VehicleType.CAR
    @property
    def required_spot_size(self) -> SpotSize: return SpotSize.MEDIUM

class Truck(Vehicle):
    @property
    def type(self) -> VehicleType: return VehicleType.TRUCK
    @property
    def required_spot_size(self) -> SpotSize: return SpotSize.LARGE

class ParkingSpot:
    def __init__(self, spot_id: str, size: SpotSize):
        self._id = spot_id
        self._size = size
        self._vehicle: Optional[Vehicle] = None
        self._parked_at: Optional[datetime] = None
    
    @property
    def spot_id(self) -> str: return self._id
    @property
    def size(self) -> SpotSize: return self._size
    @property
    def is_available(self) -> bool: return self._vehicle is None
    
    def can_fit(self, vehicle: Vehicle) -> bool:
        size_order = [SpotSize.COMPACT, SpotSize.MEDIUM, SpotSize.LARGE]
        spot_rank = size_order.index(self._size)
        vehicle_rank = size_order.index(vehicle.required_spot_size)
        return spot_rank >= vehicle_rank
    
    def park(self, vehicle: Vehicle) -> bool:
        if not self.is_available or not self.can_fit(vehicle):
            return False
        self._vehicle = vehicle
        self._parked_at = datetime.now()
        return True
    
    def unpark(self) -> Optional[Vehicle]:
        vehicle = self._vehicle
        self._vehicle = None
        self._parked_at = None
        return vehicle

class ParkingTicket:
    """Receipt for parking transaction."""
    _counter = 0
    
    def __init__(self, vehicle: Vehicle, spot: ParkingSpot):
        ParkingTicket._counter += 1
        self.ticket_id = f"T{ParkingTicket._counter:08d}"
        self.vehicle = vehicle
        self.spot = spot
        self.entry_time = datetime.now()
        self.exit_time: Optional[datetime] = None
        self.fee: float = 0.0

class ParkingLot:
    """
    Parking lot management system.
    
    Design principles applied:
    - Single Responsibility: ParkingSpot handles parking, ParkingLot manages lots
    - Open/Closed: New vehicle types don't require ParkingLot modification
    - Liskov: Any Vehicle subclass can be parked
    - Interface Segregation: Vehicles only expose what they need
    - Dependency Inversion: ParkingLot depends on abstractions
    
    Time Complexity:
    - park(): O(S) where S = total spots (linear scan)
    - exit(): O(1) with ticket
    - Available spots: O(1) with pre-counted
    """
    
    def __init__(self, name: str):
        self._name = name
        self._spots: list[ParkingSpot] = []
        self._active_tickets: dict[str, ParkingTicket] = {}
        self._hourly_rates = {
            SpotSize.COMPACT: 5.0,
            SpotSize.MEDIUM: 10.0,
            SpotSize.LARGE: 15.0,
        }
    
    def add_spot(self, spot: ParkingSpot) -> None:
        self._spots.append(spot)
    
    def park(self, vehicle: Vehicle) -> Optional[ParkingTicket]:
        """Find and occupy best-fit spot. Returns ticket or None if full."""
        # Best-fit: smallest spot that fits the vehicle (minimize wasted space)
        size_order = [SpotSize.COMPACT, SpotSize.MEDIUM, SpotSize.LARGE]
        
        for size in size_order:
            for spot in self._spots:
                if spot.size == size and spot.is_available and spot.can_fit(vehicle):
                    spot.park(vehicle)
                    ticket = ParkingTicket(vehicle, spot)
                    self._active_tickets[ticket.ticket_id] = ticket
                    return ticket
        
        return None  # Full
    
    def exit(self, ticket_id: str) -> float:
        """Process exit. Returns fee."""
        ticket = self._active_tickets.pop(ticket_id, None)
        if not ticket:
            raise ValueError(f"Invalid ticket: {ticket_id}")
        
        ticket.exit_time = datetime.now()
        hours = (ticket.exit_time - ticket.entry_time).total_seconds() / 3600
        rate = self._hourly_rates[ticket.spot.size]
        ticket.fee = max(rate, hours * rate)  # Minimum 1-hour charge
        ticket.spot.unpark()
        
        return ticket.fee
    
    def available_count(self, size: Optional[SpotSize] = None) -> int:
        if size:
            return sum(1 for s in self._spots if s.size == size and s.is_available)
        return sum(1 for s in self._spots if s.is_available)
    
    def is_full(self) -> bool:
        return self.available_count() == 0


# ─── Usage ───
lot = ParkingLot("Microsoft Building 15 Parking")
for i in range(5):
    lot.add_spot(ParkingSpot(f"COMPACT_{i}", SpotSize.COMPACT))
    lot.add_spot(ParkingSpot(f"MEDIUM_{i}", SpotSize.MEDIUM))
    lot.add_spot(ParkingSpot(f"LARGE_{i}", SpotSize.LARGE))

car = Car("ABC-123")
ticket = lot.park(car)
print(f"Parked: {ticket.ticket_id}")
print(f"Available medium spots: {lot.available_count(SpotSize.MEDIUM)}")
import time; time.sleep(0.01)
fee = lot.exit(ticket.ticket_id)
print(f"Fee: ${fee:.2f}")
```

---

## 6. Problem 2: Implement Stack Using Queues

**Frequency at Microsoft:** ★★★☆☆ | **Difficulty:** Easy

```python
from collections import deque

class MyStack_TwoQueues:
    """
    LeetCode 225. Implement Stack using Queues.
    
    Approach: Two queues, push O(N), pop O(1)
    OR: One queue, push O(N), pop O(1)
    
    Time: push O(N), pop O(1), top O(1), empty O(1)
    Space: O(N)
    """
    
    def __init__(self):
        self._q1 = deque()  # Main queue
        self._q2 = deque()  # Temporary
    
    def push(self, x: int) -> None:
        """Push to temporary, move all from main, swap."""
        self._q2.append(x)
        while self._q1:
            self._q2.append(self._q1.popleft())
        self._q1, self._q2 = self._q2, self._q1
    
    def pop(self) -> int:
        return self._q1.popleft()
    
    def top(self) -> int:
        return self._q1[0]
    
    def empty(self) -> bool:
        return not self._q1


class MyStack_OneQueue:
    """One queue approach — more efficient."""
    
    def __init__(self):
        self._q = deque()
    
    def push(self, x: int) -> None:
        """After appending x, rotate all previous elements behind x."""
        self._q.append(x)
        for _ in range(len(self._q) - 1):
            self._q.append(self._q.popleft())
    
    def pop(self) -> int: return self._q.popleft()
    def top(self) -> int: return self._q[0]
    def empty(self) -> bool: return not self._q


# Bonus: Queue using Stacks (opposite direction)
class MyQueue:
    """
    LeetCode 232. Implement Queue using Stacks.
    Amortized O(1) per operation.
    """
    
    def __init__(self):
        self._inbox = []   # Push stack
        self._outbox = []  # Pop stack
    
    def push(self, x: int) -> None:
        self._inbox.append(x)
    
    def _transfer(self):
        """Transfer from inbox to outbox when outbox is empty."""
        if not self._outbox:
            while self._inbox:
                self._outbox.append(self._inbox.pop())
    
    def pop(self) -> int:
        self._transfer()
        return self._outbox.pop()
    
    def peek(self) -> int:
        self._transfer()
        return self._outbox[-1]
    
    def empty(self) -> bool:
        return not self._inbox and not self._outbox
```

---

## 7. Problem 3: LRU Cache

*(See File 04 for full O(1) implementation. Microsoft often asks for explanation of choices.)*

```python
# Microsoft-specific discussion points:

"""
Interview question: "Explain your LRU implementation choices."

Answer structure:
1. "I used a doubly linked list for O(1) node removal from anywhere in the list."
   - Singly linked list would be O(N) to remove a middle node (no prev pointer)
   
2. "I used a hashmap to find the node in O(1) given a key."
   - Without hashmap, finding the node is O(N) linear scan
   
3. "The combination gives O(1) for both get and put."
   - get: hashmap O(1) lookup → move to front O(1)
   - put: hashmap O(1) lookup → if exists: update + move; if new: add front + evict tail
   
4. "Sentinel nodes (dummy head and tail) simplify edge cases."
   - No null checks for 'is this the first/last node?'
   - All operations work uniformly

Design trade-offs:
- Space: O(capacity) — fixed memory bound (good for predictable memory usage)
- Thread safety: add threading.Lock() for concurrent access
- Persistence: could write-through to database on each put()
"""
```

---

## 8. Problem 4: Zigzag Level Order Traversal

**Frequency at Microsoft:** ★★★★☆ | **Difficulty:** Medium

```python
from collections import deque

class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def zigzagLevelOrder(root: TreeNode) -> list[list[int]]:
    """
    LeetCode 103. Zigzag Level Order Traversal.
    
    BFS with direction flag. Even levels: left-to-right. Odd levels: right-to-left.
    Use deque: appendleft for right-to-left (reverse).
    
    Time: O(N) | Space: O(N)
    """
    if not root:
        return []
    
    result = []
    queue = deque([root])
    left_to_right = True
    
    while queue:
        level_size = len(queue)
        level = deque()
        
        for _ in range(level_size):
            node = queue.popleft()
            if left_to_right:
                level.append(node.val)
            else:
                level.appendleft(node.val)
            
            if node.left: queue.append(node.left)
            if node.right: queue.append(node.right)
        
        result.append(list(level))
        left_to_right = not left_to_right
    
    return result


def levelOrderBottom(root: TreeNode) -> list[list[int]]:
    """
    LeetCode 107. Level Order Traversal Bottom-Up.
    Same as above but prepend each level to result.
    """
    if not root:
        return []
    
    result = []
    queue = deque([root])
    
    while queue:
        level = []
        for _ in range(len(queue)):
            node = queue.popleft()
            level.append(node.val)
            if node.left: queue.append(node.left)
            if node.right: queue.append(node.right)
        result.append(level)
    
    return result[::-1]


# Tests
root = TreeNode(3, TreeNode(9), TreeNode(20, TreeNode(15), TreeNode(7)))
assert zigzagLevelOrder(root) == [[3],[20,9],[15,7]]
assert levelOrderBottom(root) == [[15,7],[9,20],[3]]
```

---

## 9. Problem 5: Longest Substring Without Repeating Characters

**Frequency at Microsoft:** ★★★★★ | **Difficulty:** Medium

```python
def lengthOfLongestSubstring(s: str) -> int:
    """
    LeetCode 3. Longest Substring Without Repeating Characters.
    
    Sliding window with character position tracking.
    
    Time: O(N) | Space: O(min(N, alphabet_size))
    
    Microsoft loves this for:
    1. Testing sliding window pattern
    2. Edge case discussion (empty string, all same, all unique)
    3. Follow-up: "what if we allow k duplicates?"
    """
    if not s:
        return 0
    
    last_seen = {}  # char → last seen index
    left = 0
    max_len = 0
    
    for right, char in enumerate(s):
        if char in last_seen and last_seen[char] >= left:
            left = last_seen[char] + 1
        last_seen[char] = right
        max_len = max(max_len, right - left + 1)
    
    return max_len


def lengthOfLongestSubstringKDistinct(s: str, k: int) -> int:
    """
    LeetCode 340. Longest Substring with At Most K Distinct Characters.
    Extension asked by Microsoft.
    
    Time: O(N) | Space: O(K)
    """
    from collections import defaultdict
    
    if k == 0 or not s:
        return 0
    
    char_count = defaultdict(int)
    left = 0
    max_len = 0
    
    for right, char in enumerate(s):
        char_count[char] += 1
        
        while len(char_count) > k:
            left_char = s[left]
            char_count[left_char] -= 1
            if char_count[left_char] == 0:
                del char_count[left_char]
            left += 1
        
        max_len = max(max_len, right - left + 1)
    
    return max_len


# Tests
assert lengthOfLongestSubstring("abcabcbb") == 3
assert lengthOfLongestSubstring("bbbbb") == 1
assert lengthOfLongestSubstring("pwwkew") == 3
assert lengthOfLongestSubstring("") == 0
assert lengthOfLongestSubstringKDistinct("eceba", 2) == 3
```

---

## 10. Problem 6: Copy List with Random Pointer

**Frequency at Microsoft:** ★★★★☆ | **Difficulty:** Medium

```python
class RandomNode:
    def __init__(self, x: int, next=None, random=None):
        self.val = x
        self.next = next
        self.random = random

def copyRandomList(head: RandomNode) -> RandomNode:
    """
    LeetCode 138. Copy List with Random Pointer.
    
    Approach 1: HashMap (old → new) — O(N) space
    Approach 2: Interweave — O(1) extra space
    
    Microsoft frequently asks this for pointer manipulation skills.
    """
    if not head:
        return None
    
    # Two-pass with HashMap:
    old_to_new = {}
    
    # Pass 1: Create all nodes
    curr = head
    while curr:
        old_to_new[curr] = RandomNode(curr.val)
        curr = curr.next
    
    # Pass 2: Set next and random pointers
    curr = head
    while curr:
        if curr.next:
            old_to_new[curr].next = old_to_new[curr.next]
        if curr.random:
            old_to_new[curr].random = old_to_new[curr.random]
        curr = curr.next
    
    return old_to_new[head]


def copyRandomList_O1_space(head: RandomNode) -> RandomNode:
    """
    O(1) space interweaving approach.
    
    Step 1: Interleave: A → A' → B → B' → C → C'
    Step 2: Set random pointers: A'.random = A.random.next
    Step 3: Separate: A → B → C and A' → B' → C'
    
    Time: O(N) | Space: O(1)
    """
    if not head:
        return None
    
    # Step 1: Interleave
    curr = head
    while curr:
        clone = RandomNode(curr.val)
        clone.next = curr.next
        curr.next = clone
        curr = clone.next
    
    # Step 2: Set random pointers
    curr = head
    while curr:
        if curr.random:
            curr.next.random = curr.random.next
        curr = curr.next.next
    
    # Step 3: Separate lists
    dummy = RandomNode(0)
    clone_curr = dummy
    curr = head
    while curr:
        clone_curr.next = curr.next
        curr.next = curr.next.next
        clone_curr = clone_curr.next
        curr = curr.next
    
    return dummy.next
```

---

## 11. Problem 7: Flatten Nested List Iterator

**Frequency at Microsoft:** ★★★★☆ | **Difficulty:** Medium

```python
class NestedInteger:
    def isInteger(self) -> bool: ...
    def getInteger(self) -> int: ...
    def getList(self): ...

class NestedIterator:
    """
    LeetCode 341. Flatten Nested List Iterator.
    
    Microsoft loves this — tests OOP, iterator pattern, stack usage.
    
    Approach: Stack-based lazy evaluation.
    Stack stores reverse-order iterators over lists.
    
    Time: next() O(1) amortized | hasNext() O(1) amortized
    Space: O(D) where D = maximum nesting depth
    """
    
    def __init__(self, nestedList: list):
        # Stack of list iterators
        self._stack = [iter(nestedList)]
        self._next_val = None
        self._advance()
    
    def _advance(self):
        """Advance to the next integer value."""
        self._next_val = None
        
        while self._stack:
            try:
                item = next(self._stack[-1])
                if item.isInteger():
                    self._next_val = item.getInteger()
                    return
                else:
                    self._stack.append(iter(item.getList()))
            except StopIteration:
                self._stack.pop()
    
    def next(self) -> int:
        """Return next integer and advance."""
        result = self._next_val
        self._advance()
        return result
    
    def hasNext(self) -> bool:
        return self._next_val is not None


class NestedIterator_Eager:
    """
    Eager (precompute all) version — simpler but less memory efficient.
    Good for small inputs or when memory isn't a constraint.
    """
    
    def __init__(self, nestedList: list):
        self._flat = []
        self._idx = 0
        self._flatten(nestedList)
    
    def _flatten(self, lst):
        for item in lst:
            if item.isInteger():
                self._flat.append(item.getInteger())
            else:
                self._flatten(item.getList())
    
    def next(self) -> int:
        val = self._flat[self._idx]
        self._idx += 1
        return val
    
    def hasNext(self) -> bool:
        return self._idx < len(self._flat)
```

---

## 12. Microsoft System Design Snippets

### Design: URL Shortener (Common Microsoft Problem)

```python
class URLShortener:
    """
    Simplified URL shortener (like Bing Short URL or bit.ly).
    
    Design decisions:
    1. Hash function: Base-62 encoding of counter (not random hash)
       → Avoids collision detection
       → Predictable length
    
    2. Storage: {short_code → original_url} hashmap
    
    3. Counter: global monotonic counter (in distributed: use Snowflake ID)
    
    Time: encode O(log62(counter)) = O(1), decode O(1)
    Space: O(N) for N URLs
    """
    
    BASE62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    
    def __init__(self, base_url: str = "https://short.url/"):
        self._counter = 0
        self._short_to_long: dict[str, str] = {}
        self._long_to_short: dict[str, str] = {}  # For deduplication
        self._base_url = base_url
    
    def _encode(self, num: int) -> str:
        """Convert integer to base-62 string."""
        if num == 0:
            return self.BASE62[0]
        chars = []
        while num:
            chars.append(self.BASE62[num % 62])
            num //= 62
        return ''.join(reversed(chars))
    
    def _decode(self, code: str) -> int:
        num = 0
        for char in code:
            num = num * 62 + self.BASE62.index(char)
        return num
    
    def encode(self, long_url: str) -> str:
        """Shorten URL. Deduplicates identical URLs."""
        if long_url in self._long_to_short:
            return self._long_to_short[long_url]
        
        self._counter += 1
        code = self._encode(self._counter)
        short = self._base_url + code
        self._short_to_long[code] = long_url
        self._long_to_short[long_url] = short
        return short
    
    def decode(self, short_url: str) -> str:
        """Expand short URL to original."""
        code = short_url.replace(self._base_url, '')
        if code not in self._short_to_long:
            raise KeyError(f"Unknown short URL: {short_url}")
        return self._short_to_long[code]


# Test
shortener = URLShortener()
short1 = shortener.encode("https://www.microsoft.com/very/long/url/here")
print(short1)  # "https://short.url/b"
print(shortener.decode(short1))  # Original URL
print(shortener.encode("https://www.microsoft.com/very/long/url/here"))  # Same short (dedup)
```

---

## 13. Microsoft-Specific Interview Tips

### The "Growth Mindset" Approach to Coding

```python
# Microsoft values showing HOW you think and learn, not just that you know

# When stuck, use growth mindset language:
stuck_responses = {
    "don't know algorithm": 
        "I haven't seen this exact problem, but it reminds me of [similar]. "
        "Let me think about whether that approach adapts here...",
    
    "realize wrong approach":
        "I think I was approaching this the wrong way. Let me step back. "
        "The issue is [X]. A better approach would be [Y]. "
        "This is actually a better solution because [reason].",
    
    "got hint from interviewer":
        "Ah, that's a great insight! So if I use [hint], then the key change is [X]. "
        "Let me update my approach... Yes, this simplifies the solution significantly.",
    
    "optimization asked":
        "Currently it's O(N²). I think we can improve by [observation]. "
        "Let me think... [30 seconds thinking] "
        "If we precompute [X], each lookup becomes O(log N). "
        "That brings the total to O(N log N).",
}
```

### Microsoft OOP Design Interview Format

```
Time allocation for 60-minute OOP design at Microsoft:

Minutes 0-5:   Requirements gathering
  - Ask about users, operations, constraints
  - Confirm: "Does the parking lot have multiple floors?"
  - Confirm: "Do we need thread safety?"

Minutes 5-15:  Identify classes and relationships
  - Draw/list classes: Vehicle, ParkingSpot, Ticket, ParkingLot
  - Identify is-a (inheritance) and has-a (composition)
  - Define enumerations (VehicleType, SpotSize)

Minutes 15-40: Implement core classes
  - Start with smallest, most independent classes
  - Work toward the main orchestrating class
  - Write method signatures before bodies

Minutes 40-55: Discuss extensions and edge cases
  - "How would we add electric vehicle charging?"
  - Thread safety discussion
  - Persistence layer

Minutes 55-60: Trade-off discussion
  - "Why did you choose composition over inheritance here?"
  - "How would this scale to 1000 parking lots?"
```

### Microsoft Behavioral Questions

```
Growth Mindset questions:
"Tell me about a time you failed and what you learned"
"Describe a situation where you had to quickly learn something new"
"When have you changed your opinion based on new information?"

Collaboration questions:
"Tell me about a time you worked with a difficult team member"
"How do you handle disagreements about technical decisions?"

Impact questions:
"What's the most impactful thing you've shipped?"
"How did you measure the success of your project?"

The STAR format works well, but Microsoft specifically values:
- Showing learning from failure (growth mindset)
- Credit-sharing (collaboration signal)
- Quantified results ("improved performance by 40%")
```

---

*Microsoft's interview style rewards engineers who can communicate their thought process clearly, write clean OOP code, and show genuine curiosity about making good engineering decisions. The algorithmic bar is slightly lower than Google, but the design and communication bar is just as high. Practice explaining your design choices as you code.*

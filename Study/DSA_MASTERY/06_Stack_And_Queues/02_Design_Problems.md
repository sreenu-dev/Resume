# Stack & Queue Design Problems — Full Implementations

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Amortized analysis, doubly linked list, hash maps
> **Core Theme:** Building O(1) or O(1) amortized data structures that
> extend basic containers with additional query capabilities.

---

## 1. Stack with O(1) Min/Max

### Approach A: Auxiliary Stack (Simple, Extra Space)

```python
class MinStack:
    """
    Each entry in aux_stack stores the current minimum up to that point.
    Push and pop are synchronized between main and aux stacks.
    """
    def __init__(self):
        self.stack = []
        self.min_stack = []

    def push(self, val: int) -> None:
        self.stack.append(val)
        min_val = min(val, self.min_stack[-1]) if self.min_stack else val
        self.min_stack.append(min_val)

    def pop(self) -> None:
        self.stack.pop()
        self.min_stack.pop()

    def top(self) -> int:
        return self.stack[-1]

    def getMin(self) -> int:
        return self.min_stack[-1]
```
**Time:** O(1) all operations | **Space:** O(N) — doubles memory

### Approach B: Lazy Deletion (Space Optimization)

```python
class MinStackLazy:
    def __init__(self):
        self.stack = []
        self.min_stack = []     # (value, count) pairs

    def push(self, val: int) -> None:
        self.stack.append(val)
        if not self.min_stack or val < self.min_stack[-1][0]:
            self.min_stack.append([val, 1])
        elif val == self.min_stack[-1][0]:
            self.min_stack[-1][1] += 1

    def pop(self) -> None:
        val = self.stack.pop()
        if val == self.min_stack[-1][0]:
            self.min_stack[-1][1] -= 1
            if self.min_stack[-1][1] == 0:
                self.min_stack.pop()

    def top(self) -> int:
        return self.stack[-1]

    def getMin(self) -> int:
        return self.min_stack[-1][0]
```
**Time:** O(1) all operations | **Space:** O(N) worst case, O(distinct minimums) typical

### Stack with O(1) Max

```python
class MaxStack:
    def __init__(self):
        self.stack = []
        self.max_stack = []

    def push(self, val: int) -> None:
        self.stack.append(val)
        max_val = max(val, self.max_stack[-1]) if self.max_stack else val
        self.max_stack.append(max_val)

    def pop(self) -> int:
        self.max_stack.pop()
        return self.stack.pop()

    def peekMax(self) -> int:
        return self.max_stack[-1]
```

---

## 2. Queue with O(1) Max — Monotonic Deque

**Maintain a monotone decreasing deque. Front is always current maximum.**

```python
from collections import deque

class MaxQueue:
    def __init__(self):
        self.queue = deque()
        self.mono = deque()    # Monotone decreasing values

    def enqueue(self, val: int) -> None:
        self.queue.append(val)
        while self.mono and self.mono[-1] < val:
            self.mono.pop()
        self.mono.append(val)

    def dequeue(self) -> int:
        val = self.queue.popleft()
        if self.mono[0] == val:
            self.mono.popleft()
        return val

    def getMax(self) -> int:
        return self.mono[0]
```
**Time:** O(1) amortized | **Space:** O(N)

---

## 3. Queue Using Two Stacks — Amortized O(1) Analysis

```python
class MyQueue:
    """
    Two stacks: 'inbox' (for push) and 'outbox' (for pop/peek).
    Transfer happens lazily only when outbox is empty.
    """
    def __init__(self):
        self.inbox = []
        self.outbox = []

    def push(self, x: int) -> None:
        self.inbox.append(x)

    def pop(self) -> int:
        self._transfer()
        return self.outbox.pop()

    def peek(self) -> int:
        self._transfer()
        return self.outbox[-1]

    def empty(self) -> bool:
        return not self.inbox and not self.outbox

    def _transfer(self):
        if not self.outbox:
            while self.inbox:
                self.outbox.append(self.inbox.pop())
```

### Amortized Analysis (Banker's Method)

Assign each element a "credit" of 2 when pushed:
- Cost of push: 1 credit used (push to inbox), 1 credit stored on element
- Cost of transfer: 1 credit used per element (paid by stored credit)
- Cost of pop from outbox: 0 extra (already transferred)

Each element pays at most 2 credits total → O(1) amortized per operation.

**Time:** O(1) amortized | **Space:** O(N)

---

## 4. Stack Using Two Queues

```python
from collections import deque

class MyStack:
    def __init__(self):
        self.q = deque()

    def push(self, x: int) -> None:
        self.q.append(x)
        for _ in range(len(self.q) - 1):
            self.q.append(self.q.popleft())

    def pop(self) -> int:
        return self.q.popleft()

    def top(self) -> int:
        return self.q[0]

    def empty(self) -> bool:
        return len(self.q) == 0
```
**Time:** Push O(N), Pop/Top/Empty O(1) | **Space:** O(N)

---

## 5. Design Circular Deque (LeetCode 641)

```python
class MyCircularDeque:
    def __init__(self, k: int):
        self.data = [0] * k
        self.front = 0
        self.rear = -1
        self.size = 0
        self.cap = k

    def insertFront(self, value: int) -> bool:
        if self.isFull(): return False
        self.front = (self.front - 1) % self.cap
        self.data[self.front] = value
        self.size += 1
        if self.size == 1:
            self.rear = self.front
        return True

    def insertLast(self, value: int) -> bool:
        if self.isFull(): return False
        self.rear = (self.rear + 1) % self.cap
        self.data[self.rear] = value
        self.size += 1
        return True

    def deleteFront(self) -> bool:
        if self.isEmpty(): return False
        self.front = (self.front + 1) % self.cap
        self.size -= 1
        return True

    def deleteLast(self) -> bool:
        if self.isEmpty(): return False
        self.rear = (self.rear - 1) % self.cap
        self.size -= 1
        return True

    def getFront(self) -> int:
        return -1 if self.isEmpty() else self.data[self.front]

    def getRear(self) -> int:
        return -1 if self.isEmpty() else self.data[self.rear]

    def isEmpty(self) -> bool:
        return self.size == 0

    def isFull(self) -> bool:
        return self.size == self.cap
```
**Time:** O(1) all operations | **Space:** O(k)

---

## 6. Design Hit Counter (LeetCode 362)

```python
from collections import deque

class HitCounter:
    def __init__(self):
        self.hits = deque()
        self.total = 0

    def hit(self, timestamp: int) -> None:
        if self.hits and self.hits[-1][0] == timestamp:
            self.hits[-1] = (timestamp, self.hits[-1][1] + 1)
        else:
            self.hits.append((timestamp, 1))
        self.total += 1

    def getHits(self, timestamp: int) -> int:
        while self.hits and self.hits[0][0] <= timestamp - 300:
            self.total -= self.hits.popleft()[1]
        return self.total
```
**Time:** O(1) amortized | **Space:** O(300)

**Fixed-array O(1) variant:**
```python
class HitCounterFixed:
    def __init__(self):
        self.times = [0] * 300
        self.counts = [0] * 300

    def hit(self, timestamp: int) -> None:
        i = timestamp % 300
        if self.times[i] != timestamp:
            self.times[i] = timestamp
            self.counts[i] = 1
        else:
            self.counts[i] += 1

    def getHits(self, timestamp: int) -> int:
        return sum(
            self.counts[i]
            for i in range(300)
            if self.times[i] > timestamp - 300
        )
```
**Time:** O(300) = O(1) | **Space:** O(300)

---

## 7. Stack Supporting Push/Pop/Increment (LeetCode 1381)

**Increment bottom k elements in O(1) using lazy propagation:**

```python
class CustomStack:
    def __init__(self, maxSize: int):
        self.stack = []
        self.lazy = []
        self.max_size = maxSize

    def push(self, x: int) -> None:
        if len(self.stack) < self.max_size:
            self.stack.append(x)
            self.lazy.append(0)

    def pop(self) -> int:
        if not self.stack:
            return -1
        val = self.stack.pop() + self.lazy[-1]
        inc = self.lazy.pop()
        if self.lazy:
            self.lazy[-1] += inc
        return val

    def increment(self, k: int, val: int) -> None:
        if self.stack:
            i = min(k, len(self.stack)) - 1
            self.lazy[i] += val
```
**Time:** O(1) all operations | **Space:** O(N)

**Lazy propagation insight:** `lazy[i]` stores the cumulative increment for
all elements at index ≤ i. When popping, propagate `lazy[i]` to `lazy[i-1]`.

---

## 8. LRU Cache — OrderedDict (LeetCode 146)

```python
from collections import OrderedDict

class LRUCacheSimple:
    def __init__(self, capacity: int):
        self.cap = capacity
        self.cache = OrderedDict()

    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        self.cache.move_to_end(key)
        return self.cache[key]

    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self.cache.move_to_end(key)
        self.cache[key] = value
        if len(self.cache) > self.cap:
            self.cache.popitem(last=False)
```
**Time:** O(1) | **Space:** O(capacity)

---

## 9. LFU Cache — Full Implementation (LeetCode 460)

```python
from collections import defaultdict, OrderedDict

class LFUCache:
    def __init__(self, capacity: int):
        self.cap = capacity
        self.min_freq = 0
        self.key_to_val_freq = {}
        self.freq_to_keys = defaultdict(OrderedDict)

    def _update(self, key: int) -> None:
        val, freq = self.key_to_val_freq[key]
        del self.freq_to_keys[freq][key]
        if not self.freq_to_keys[freq]:
            del self.freq_to_keys[freq]
            if self.min_freq == freq:
                self.min_freq += 1
        new_freq = freq + 1
        self.key_to_val_freq[key] = [val, new_freq]
        self.freq_to_keys[new_freq][key] = None

    def get(self, key: int) -> int:
        if key not in self.key_to_val_freq:
            return -1
        self._update(key)
        return self.key_to_val_freq[key][0]

    def put(self, key: int, value: int) -> None:
        if self.cap <= 0:
            return

        if key in self.key_to_val_freq:
            self.key_to_val_freq[key][0] = value
            self._update(key)
            return

        if len(self.key_to_val_freq) >= self.cap:
            evict_key, _ = self.freq_to_keys[self.min_freq].popitem(last=False)
            if not self.freq_to_keys[self.min_freq]:
                del self.freq_to_keys[self.min_freq]
            del self.key_to_val_freq[evict_key]

        self.key_to_val_freq[key] = [value, 1]
        self.freq_to_keys[1][key] = None
        self.min_freq = 1
```
**Time:** O(1) all operations | **Space:** O(capacity)

**Why OrderedDict per frequency?** Provides O(1) insertion (MRU),
O(1) deletion by key, and O(1) eviction of LRU (from front).

---

## 10. Design Browser History (LeetCode 1472)

```python
class BrowserHistory:
    def __init__(self, homepage: str):
        self.history = [homepage]
        self.curr = 0
        self.last = 0

    def visit(self, url: str) -> None:
        self.curr += 1
        if self.curr < len(self.history):
            self.history[self.curr] = url
        else:
            self.history.append(url)
        self.last = self.curr

    def back(self, steps: int) -> str:
        self.curr = max(0, self.curr - steps)
        return self.history[self.curr]

    def forward(self, steps: int) -> str:
        self.curr = min(self.last, self.curr + steps)
        return self.history[self.curr]
```
**Time:** O(1) all operations | **Space:** O(N)

---

## Design Problem Comparison Table

| Problem | Core Structure | Key Insight |
|---|---|---|
| Stack + min | Aux min-stack or lazy count | Min-stack mirrors main stack |
| Queue + max | Monotone deque | Pop smaller elements from back |
| Queue via 2 stacks | Inbox / Outbox | Transfer lazily only when outbox empty |
| Stack via 2 queues | Rotate on push | New element moved to front |
| Circular deque | Ring buffer | Modular front/rear arithmetic |
| Hit counter | Compressed deque | (timestamp, count) pairs or fixed array mod 300 |
| Stack + increment | Lazy propagation array | Propagate on pop, not on increment |
| LRU | DLL + HashMap or OrderedDict | O(1) move-to-front and tail eviction |
| LFU | freq→OrderedDict + min_freq | Three-level structure for O(1) all ops |
| Browser history | Array + curr/last pointers | last pointer truncates forward history |

## Interview Tips

1. **LFU is the hardest**: The `_update` function must be fluid. `min_freq` resets to 1 on insert and increments by 1 during update.
2. **Amortized analysis**: For queue via 2 stacks, be able to explain the banker's method.
3. **LRU**: Know both the OrderedDict and explicit DLL+HashMap versions.
4. **Stack + increment**: Lazy propagation on pop is the elegant O(1) solution.

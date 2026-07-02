# Other Top Tech Companies — Interview Patterns & Mastery Guide

> **Level:** Advanced | **Companies:** Apple, Netflix, Uber/Lyft, Stripe/Square, Airbnb, LinkedIn  
> **Interview Frequency:** ★★★★★ (Core guide for non-FAANG top companies)

---

## Table of Contents
1. [Apple — Clean Code & Edge Cases](#1-apple--clean-code--edge-cases)
2. [Netflix — Distributed Systems in Code](#2-netflix--distributed-systems-in-code)
3. [Uber / Lyft — Graph & Location Problems](#3-uber--lyft--graph--location-problems)
4. [Stripe / Square — Financial Precision & API Design](#4-stripe--square--financial-precision--api-design)
5. [Airbnb — Graph, Backtracking & API Design](#5-airbnb--graph-backtracking--api-design)
6. [LinkedIn — Social Graph & Recommendation](#6-linkedin--social-graph--recommendation)
7. [Company Comparison Matrix](#7-company-comparison-matrix)

---

## 1. Apple — Clean Code & Edge Cases

### Interview Philosophy

Apple values **craftsmanship**. They want engineers who care deeply about code quality, edge cases, and user experience. Apple's interview is less algorithm-heavy than Google but more focused on practical correctness.

```
Apple's Process:
  - Technical Screen: 1 round (phone/video, 45 min)
  - Onsite: 5-6 rounds (mix of coding, design, culture)
  - Strong focus on systems programming (kernel, frameworks, Swift/Objective-C)
  - For platform teams: platform-specific questions (iOS, macOS, Metal)

Apple's Bar:
  - Clean, correct code (every edge case matters)
  - Deep systems knowledge (memory management, threading)
  - Practical problem-solving (real-world constraints)
```

### Apple's Top 3 Focus Topics

```
1. Array manipulation with correct edge cases
2. String processing (parsing, manipulation, pattern matching)
3. System design (for iOS: architecture patterns like MVVM, Combine)
```

### Apple Problem 1: Array Manipulation

```python
def findMedianOfTwoSortedArrays(nums1: list[int], nums2: list[int]) -> float:
    """
    LeetCode 4. Median of Two Sorted Arrays.
    
    Apple loves this because it requires PRECISE edge case handling.
    Wrong implementations often fail on:
    - Empty arrays
    - One-element arrays
    - Arrays of different lengths
    - When median straddles array boundary
    
    Binary search on the partition point.
    Time: O(log(min(m, n))) | Space: O(1)
    """
    # Ensure nums1 is the smaller array (binary search on smaller)
    if len(nums1) > len(nums2):
        nums1, nums2 = nums2, nums1
    
    m, n = len(nums1), len(nums2)
    half = (m + n + 1) // 2
    
    lo, hi = 0, m
    
    while lo <= hi:
        mid1 = (lo + hi) // 2  # Partition for nums1
        mid2 = half - mid1      # Partition for nums2
        
        # Elements to the LEFT of partition in each array
        maxLeft1 = float('-inf') if mid1 == 0 else nums1[mid1 - 1]
        maxLeft2 = float('-inf') if mid2 == 0 else nums2[mid2 - 1]
        
        # Elements to the RIGHT of partition in each array
        minRight1 = float('inf') if mid1 == m else nums1[mid1]
        minRight2 = float('inf') if mid2 == n else nums2[mid2]
        
        if maxLeft1 <= minRight2 and maxLeft2 <= minRight1:
            # Valid partition found!
            if (m + n) % 2 == 1:
                return float(max(maxLeft1, maxLeft2))
            return (max(maxLeft1, maxLeft2) + min(minRight1, minRight2)) / 2.0
        elif maxLeft1 > minRight2:
            hi = mid1 - 1
        else:
            lo = mid1 + 1
    
    raise ValueError("Input arrays are not sorted")


# Tests for Apple's edge case obsession:
assert findMedianOfTwoSortedArrays([1,3], [2]) == 2.0
assert findMedianOfTwoSortedArrays([1,2], [3,4]) == 2.5
assert findMedianOfTwoSortedArrays([], [1]) == 1.0
assert findMedianOfTwoSortedArrays([2], []) == 2.0
assert findMedianOfTwoSortedArrays([0,0], [0,0]) == 0.0
```

### Apple Problem 2: String Processing

```python
def minWindow(s: str, t: str) -> str:
    """
    LeetCode 76. Minimum Window Substring.
    
    Apple loves string problems with careful window management.
    
    Find smallest window in s containing all chars in t.
    
    Time: O(|s| + |t|) | Space: O(|t|)
    """
    from collections import Counter
    
    if not s or not t:
        return ""
    
    need = Counter(t)
    have = {}
    formed = 0
    required = len(need)  # Unique characters we need to satisfy
    
    min_len = float('inf')
    min_left = 0
    left = 0
    
    for right, char in enumerate(s):
        have[char] = have.get(char, 0) + 1
        
        if char in need and have[char] == need[char]:
            formed += 1
        
        while formed == required:
            # Update minimum window
            if right - left + 1 < min_len:
                min_len = right - left + 1
                min_left = left
            
            # Shrink from left
            left_char = s[left]
            have[left_char] -= 1
            if left_char in need and have[left_char] < need[left_char]:
                formed -= 1
            left += 1
    
    return s[min_left:min_left + min_len] if min_len != float('inf') else ""


# Tests
assert minWindow("ADOBECODEBANC", "ABC") == "BANC"
assert minWindow("a", "a") == "a"
assert minWindow("a", "aa") == ""
assert minWindow("", "A") == ""
```

### Apple Problem 3: System Design (Sketch)

```python
# Apple often asks about iOS-specific patterns:

class ApplicationCoordinator:
    """
    Coordinator Pattern (popular at Apple/iOS):
    Manages navigation flow between view controllers.
    Decouples view controllers from each other.
    """
    
    def __init__(self):
        self.child_coordinators = []
    
    def start(self):
        raise NotImplementedError
    
    def coordinate(self, to: 'ApplicationCoordinator'):
        self.child_coordinators.append(to)
        to.start()
    
    def release(self, coordinator: 'ApplicationCoordinator'):
        self.child_coordinators.remove(coordinator)


# Apple's interview focus: "How would you design [iOS component]?"
# Examples: custom UICollectionView layout, Combine pipeline, Metal renderer
```

**Apple Interview Tips:**
```
1. Handle EVERY edge case — Apple will specifically test them
2. Show attention to detail: "What if the string contains Unicode?"
3. Memory management awareness: "This creates O(N) temporary arrays — is that acceptable?"
4. User experience lens: "This API would be called in the main thread — is it fast enough?"
```

---

## 2. Netflix — Distributed Systems in Code

### Interview Philosophy

Netflix values engineers who understand **distributed systems** at a code level. Their coding questions often have a "scale" dimension built in.

```
Netflix's Process:
  - Technical Screen: 1-2 rounds
  - Onsite: 4-5 rounds
  - Heavy system design component
  - Coding questions are often system-relevant (caching, streaming, consistency)

Netflix's Bar:
  - Strong distributed systems understanding
  - Resilience and fault tolerance thinking
  - Scale-aware solutions (Netflix serves 200M+ users)
  - Java/Kotlin ecosystem knowledge (Spring Boot, Micronaut)
```

### Netflix's Top 3 Focus Topics

```
1. Caching (LRU/LFU, consistent hashing, distributed cache)
2. Rate limiting (token bucket, sliding window)
3. Streaming data processing (real-time analytics)
```

### Netflix Problem 1: Consistent Hashing

```python
import hashlib
from bisect import bisect_left, insort

class ConsistentHashing:
    """
    Consistent Hashing for distributed caching/load balancing.
    
    Used in: Netflix content servers, AWS ElastiCache, Apache Cassandra.
    
    Key properties:
    1. When node added/removed, only K/N keys remapped (K=keys, N=nodes)
    2. Virtual nodes for even distribution
    
    Time: O(log N) lookup, O(V log N) add/remove where V = virtual nodes
    Space: O(N * V) for ring
    """
    
    def __init__(self, virtual_nodes: int = 150):
        self.virtual_nodes = virtual_nodes
        self.ring: list[int] = []          # Sorted hash positions
        self.hash_to_node: dict[int, str] = {}  # Hash → server name
    
    def _hash(self, key: str) -> int:
        return int(hashlib.md5(key.encode()).hexdigest(), 16)
    
    def add_server(self, server: str) -> None:
        """Add server with virtual nodes."""
        for i in range(self.virtual_nodes):
            virtual_key = f"{server}:{i}"
            h = self._hash(virtual_key)
            insort(self.ring, h)
            self.hash_to_node[h] = server
    
    def remove_server(self, server: str) -> None:
        """Remove server and all its virtual nodes."""
        for i in range(self.virtual_nodes):
            virtual_key = f"{server}:{i}"
            h = self._hash(virtual_key)
            idx = bisect_left(self.ring, h)
            if idx < len(self.ring) and self.ring[idx] == h:
                self.ring.pop(idx)
                del self.hash_to_node[h]
    
    def get_server(self, key: str) -> str:
        """Get server responsible for key. O(log N)."""
        if not self.ring:
            raise RuntimeError("No servers available")
        
        h = self._hash(key)
        idx = bisect_left(self.ring, h)
        idx = idx % len(self.ring)  # Wrap around (circular ring)
        return self.hash_to_node[self.ring[idx]]
    
    def get_distribution(self, keys: list[str]) -> dict:
        """Show key distribution across servers."""
        from collections import Counter
        return Counter(self.get_server(k) for k in keys)


# Test
ch = ConsistentHashing(virtual_nodes=100)
for server in ["server-1", "server-2", "server-3"]:
    ch.add_server(server)

# Each server should handle ~33% of keys
keys = [f"key_{i}" for i in range(1000)]
distribution = ch.get_distribution(keys)
print("Distribution:", dict(distribution))
# Should be approximately {server-1: ~333, server-2: ~333, server-3: ~334}
```

### Netflix Problem 2: Rate Limiter with Redis-like TTL

```python
import time
from collections import defaultdict, deque

class SlidingWindowRateLimiter:
    """
    Production-quality sliding window rate limiter.
    Used by Netflix API Gateway for per-user rate limiting.
    
    Features:
    - Per-user rate limiting
    - Sliding window (vs fixed window to avoid boundary bursting)
    - Memory-efficient: only stores request timestamps in window
    
    Time: O(1) amortized | Space: O(W) per user where W = requests in window
    """
    
    def __init__(self, max_requests: int, window_seconds: float):
        self.max_requests = max_requests
        self.window_seconds = window_seconds
        self.user_windows: dict[str, deque] = defaultdict(deque)
    
    def is_allowed(self, user_id: str) -> bool:
        now = time.time()
        window = self.user_windows[user_id]
        cutoff = now - self.window_seconds
        
        # Remove expired requests
        while window and window[0] < cutoff:
            window.popleft()
        
        if len(window) < self.max_requests:
            window.append(now)
            return True
        
        return False
    
    def time_until_allowed(self, user_id: str) -> float:
        """How long until next request is allowed (seconds)."""
        window = self.user_windows.get(user_id, deque())
        if not window or len(window) < self.max_requests:
            return 0.0
        oldest_request = window[0]
        return oldest_request + self.window_seconds - time.time()


class TokenBucketRateLimiter:
    """
    Token bucket: allows burst traffic up to bucket capacity.
    Netflix uses this for API tier differentiation (basic vs premium).
    """
    
    def __init__(self, capacity: int, refill_rate: float):
        self.capacity = capacity
        self.refill_rate = refill_rate  # tokens per second
        self.user_state: dict[str, tuple] = {}  # user → (tokens, last_refill)
    
    def is_allowed(self, user_id: str, tokens_needed: int = 1) -> bool:
        now = time.time()
        
        if user_id not in self.user_state:
            self.user_state[user_id] = (float(self.capacity), now)
        
        tokens, last_refill = self.user_state[user_id]
        
        # Add tokens for elapsed time
        elapsed = now - last_refill
        tokens = min(self.capacity, tokens + elapsed * self.refill_rate)
        
        if tokens >= tokens_needed:
            self.user_state[user_id] = (tokens - tokens_needed, now)
            return True
        
        self.user_state[user_id] = (tokens, now)
        return False
```

### Netflix Problem 3: Content Recommendation (Graph Sketch)

```python
def recommend_content(user_id: str, user_watches: dict, content_graph: dict, 
                       top_k: int = 10) -> list[str]:
    """
    Collaborative filtering via BFS on content similarity graph.
    
    Simplified Netflix recommendation:
    1. Find what user has watched
    2. BFS through similarity graph
    3. Score by distance (1-hop = similar, 2-hop = less similar)
    4. Filter out already-watched content
    5. Return top-K by score
    
    Real Netflix: matrix factorization + deep learning, but graph BFS
    is a valid approximation for coding interviews.
    """
    from collections import deque
    import heapq
    
    watched = set(user_watches.get(user_id, []))
    
    scores = {}  # content_id → score
    queue = deque()
    
    # Start BFS from watched content
    for content in watched:
        queue.append((content, 1.0))  # (content, score)
    
    visited = set(watched)
    
    while queue:
        content, score = queue.popleft()
        
        for similar, similarity in content_graph.get(content, {}).items():
            if similar not in visited:
                visited.add(similar)
                new_score = score * similarity
                scores[similar] = max(scores.get(similar, 0), new_score)
                if new_score > 0.1:  # Prune low-score paths
                    queue.append((similar, new_score))
    
    # Return top-K by score
    return heapq.nlargest(top_k, scores, key=scores.get)
```

---

## 3. Uber / Lyft — Graph & Location Problems

### Interview Philosophy

Uber and Lyft focus on **real-world graph problems** — routing, matching, geospatial. They want engineers who can bridge algorithmic theory with practical transportation challenges.

```
Uber/Lyft Process:
  - Technical Screen: 1 round (45 min, 1-2 problems)
  - Onsite: 4-5 rounds (coding, system design, behavioral)
  - Common rounds: maps/routing, matching, real-time systems
  
Focus Areas:
  - Graph algorithms (Dijkstra, A*, bipartite matching)
  - Geospatial indexing (geohashing, quadtrees)
  - Real-time data processing
  - Supply/demand matching
```

### Uber/Lyft Problem 1: Dijkstra with Dynamic Weights

```python
import heapq
from collections import defaultdict

def dijkstra(n: int, edges: list[tuple], source: int, 
             destination: int) -> float:
    """
    Single-source shortest path using Dijkstra.
    
    Uber application: Optimal route from driver to rider.
    Edge weight = travel time (dynamic based on traffic).
    
    Time: O((V + E) log V) | Space: O(V + E)
    """
    adj = defaultdict(list)
    for u, v, w in edges:
        adj[u].append((v, w))
        adj[v].append((u, w))  # Undirected
    
    dist = [float('inf')] * n
    dist[source] = 0
    
    heap = [(0, source)]  # (distance, node)
    
    while heap:
        d, u = heapq.heappop(heap)
        
        if d > dist[u]:
            continue  # Stale entry — skip
        
        if u == destination:
            return d
        
        for v, w in adj[u]:
            new_dist = dist[u] + w
            if new_dist < dist[v]:
                dist[v] = new_dist
                heapq.heappush(heap, (new_dist, v))
    
    return float('inf')  # No path


def aStar(grid: list[list[int]], start: tuple, end: tuple) -> int:
    """
    A* pathfinding — faster than Dijkstra with good heuristic.
    
    Uber application: Navigation with heuristic (Manhattan/Euclidean distance).
    h(n) = Manhattan distance to goal (admissible heuristic for grids).
    
    Time: O(E log V) with good heuristic | Space: O(V)
    """
    from heapq import heappush, heappop
    
    rows, cols = len(grid), len(grid[0])
    
    def heuristic(pos, goal):
        """Manhattan distance heuristic."""
        return abs(pos[0] - goal[0]) + abs(pos[1] - goal[1])
    
    dist = {}
    heap = [(heuristic(start, end), 0, start)]  # (f, g, pos) where f = g + h
    
    while heap:
        f, g, pos = heappop(heap)
        
        if pos == end:
            return g
        
        if pos in dist and dist[pos] <= g:
            continue
        dist[pos] = g
        
        r, c = pos
        for dr, dc in [(0,1),(1,0),(0,-1),(-1,0)]:
            nr, nc = r + dr, c + dc
            if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] == 0:
                ng = g + 1
                nf = ng + heuristic((nr, nc), end)
                heappush(heap, (nf, ng, (nr, nc)))
    
    return -1  # No path


def geohash_encode(lat: float, lon: float, precision: int = 6) -> str:
    """
    Geohash encoding — divides world into grid cells.
    Used by Uber for: nearby driver search, surge pricing zones.
    
    Precision 6 ≈ 1.2km x 0.6km cells
    Precision 8 ≈ 38m x 19m cells
    
    Time: O(precision) | Space: O(1)
    """
    BASE32 = '0123456789bcdefghjkmnpqrstuvwxyz'
    
    lat_range = [-90.0, 90.0]
    lon_range = [-180.0, 180.0]
    hash_str = ''
    bits = 0
    bit_pos = 0
    even_bit = True
    
    while len(hash_str) < precision:
        if even_bit:  # Longitude bit
            mid = (lon_range[0] + lon_range[1]) / 2
            if lon >= mid:
                bits = (bits << 1) | 1
                lon_range[0] = mid
            else:
                bits = bits << 1
                lon_range[1] = mid
        else:  # Latitude bit
            mid = (lat_range[0] + lat_range[1]) / 2
            if lat >= mid:
                bits = (bits << 1) | 1
                lat_range[0] = mid
            else:
                bits = bits << 1
                lat_range[1] = mid
        
        even_bit = not even_bit
        bit_pos += 1
        
        if bit_pos == 5:
            hash_str += BASE32[bits]
            bits = 0
            bit_pos = 0
    
    return hash_str


# Test
hash1 = geohash_encode(37.3861, -122.0839, 6)  # Cupertino, CA
print(f"Geohash: {hash1}")  # "9q9hvt" or similar
```

### Uber/Lyft Problem 2: Bipartite Matching (Rider-Driver)

```python
def max_bipartite_matching(drivers: list, riders: list, 
                            can_match: dict) -> dict:
    """
    Hungarian algorithm (simplified): maximum bipartite matching.
    
    Uber application: Optimal driver-rider assignment.
    can_match[driver][rider] = True if driver can reach rider in time.
    
    Time: O(V * E) with augmenting paths | Space: O(V)
    
    For optimal cost matching: use Hungarian algorithm O(N³)
    For large scale: use auction algorithm or approximation
    """
    match_driver = {}  # driver → matched rider
    match_rider = {}   # rider → matched driver
    
    def dfs(driver, visited):
        for rider in can_match.get(driver, []):
            if rider not in visited:
                visited.add(rider)
                if rider not in match_rider or dfs(match_rider[rider], visited):
                    match_driver[driver] = rider
                    match_rider[rider] = driver
                    return True
        return False
    
    for driver in drivers:
        dfs(driver, set())
    
    return match_driver
```

---

## 4. Stripe / Square — Financial Precision & API Design

### Interview Philosophy

Stripe and Square care deeply about **correctness, security, and API design**. Financial systems have zero tolerance for bugs. They test edge cases aggressively.

```
Stripe/Square Process:
  - Technical Screen: 1-2 rounds, API design focus
  - Onsite: 4-5 rounds (coding, system design, API design, distributed systems)
  - Unique: "Take-home project" common at Stripe

Focus Areas:
  - Financial precision (Decimal vs float)
  - API design (RESTful, versioning, idempotency)
  - Distributed transactions (atomicity, consistency)
  - Security (input validation, rate limiting, fraud detection)
```

### Stripe Problem 1: Financial Calculation Precision

```python
from decimal import Decimal, ROUND_HALF_UP, InvalidOperation

class Money:
    """
    Precise money representation for financial applications.
    
    NEVER use float for money calculations!
    0.1 + 0.2 ≠ 0.3 in floating point
    
    Use Decimal with explicit rounding for:
    - Currency arithmetic
    - Tax calculations
    - Interest computation
    """
    
    CURRENCIES = {
        'USD': 2, 'EUR': 2, 'GBP': 2,
        'JPY': 0,  # Japanese Yen has no decimal places
        'BTC': 8,  # Bitcoin has 8 decimal places
    }
    
    def __init__(self, amount: str | int | Decimal, currency: str = 'USD'):
        if currency not in self.CURRENCIES:
            raise ValueError(f"Unsupported currency: {currency}")
        
        self.currency = currency
        self.decimal_places = self.CURRENCIES[currency]
        
        try:
            self._amount = Decimal(str(amount)).quantize(
                Decimal(10) ** -self.decimal_places,
                rounding=ROUND_HALF_UP
            )
        except InvalidOperation:
            raise ValueError(f"Invalid amount: {amount}")
    
    def __add__(self, other: 'Money') -> 'Money':
        if self.currency != other.currency:
            raise ValueError(f"Cannot add {self.currency} and {other.currency}")
        result = self._amount + other._amount
        return Money(result, self.currency)
    
    def __sub__(self, other: 'Money') -> 'Money':
        if self.currency != other.currency:
            raise ValueError(f"Cannot subtract different currencies")
        result = self._amount - other._amount
        if result < 0:
            raise ValueError("Negative money amount")
        return Money(result, self.currency)
    
    def __mul__(self, factor: Decimal | float | int) -> 'Money':
        result = self._amount * Decimal(str(factor))
        return Money(result.quantize(
            Decimal(10) ** -self.decimal_places, rounding=ROUND_HALF_UP
        ), self.currency)
    
    def __eq__(self, other) -> bool:
        return isinstance(other, Money) and self._amount == other._amount and self.currency == other.currency
    
    def __lt__(self, other: 'Money') -> bool:
        return self._amount < other._amount
    
    def __repr__(self) -> str:
        return f"{self.currency} {self._amount}"


# Test
price = Money("19.99", "USD")
tax_rate = Decimal("0.0875")  # 8.75% tax
tax = price * tax_rate
total = price + tax

print(f"Price: {price}")   # USD 19.99
print(f"Tax: {tax}")       # USD 1.75 (19.99 * 0.0875 = 1.74913, rounds to 1.75)
print(f"Total: {total}")   # USD 21.74

# This would be WRONG with float:
wrong = 19.99 * (1 + 0.0875)
print(f"Wrong float: {wrong}")  # 21.739125 (not a valid cent amount)
```

### Stripe Problem 2: Idempotency Key Design

```python
import hashlib
import time
from threading import Lock

class IdempotentPaymentProcessor:
    """
    Idempotent payment API — same request can be retried safely.
    Critical for financial systems where network failures can cause
    duplicate payment attempts.
    
    Pattern: Store (idempotency_key → result) in durable store.
    Same key → same result, no new payment processed.
    
    Time: O(1) | Space: O(N) where N = unique requests
    """
    
    def __init__(self):
        self._processed: dict[str, dict] = {}
        self._lock = Lock()
    
    def generate_key(self, user_id: str, amount: str, 
                     currency: str, timestamp: float = None) -> str:
        """Generate deterministic idempotency key."""
        ts = timestamp or time.time()
        payload = f"{user_id}:{amount}:{currency}:{int(ts // 60)}"  # 60-sec window
        return hashlib.sha256(payload.encode()).hexdigest()[:32]
    
    def process_payment(self, idempotency_key: str, amount: Money,
                        user_id: str) -> dict:
        """
        Process payment exactly once, even with retries.
        Returns existing result if key was seen before.
        """
        with self._lock:
            if idempotency_key in self._processed:
                return {
                    **self._processed[idempotency_key],
                    "idempotent": True  # Flag that this is a cached result
                }
            
            # Process payment (in production: charge card, write to DB)
            result = {
                "transaction_id": f"txn_{hashlib.md5(idempotency_key.encode()).hexdigest()[:8]}",
                "amount": str(amount),
                "user_id": user_id,
                "status": "success",
                "timestamp": time.time(),
                "idempotent": False,
            }
            
            self._processed[idempotency_key] = result
            return result


# Test
processor = IdempotentPaymentProcessor()
key = processor.generate_key("user_123", "99.99", "USD")
result1 = processor.process_payment(key, Money("99.99"), "user_123")
result2 = processor.process_payment(key, Money("99.99"), "user_123")  # Retry

print(result1["idempotent"])  # False (first time)
print(result2["idempotent"])  # True (retry — same result)
print(result1["transaction_id"] == result2["transaction_id"])  # True
```

### Stripe Problem 3: API Design

```python
# Stripe-style API design principles in code:

from enum import Enum
from typing import Optional
from dataclasses import dataclass, field

class PaymentStatus(Enum):
    PENDING = "pending"
    PROCESSING = "processing"
    SUCCEEDED = "succeeded"
    FAILED = "failed"
    REFUNDED = "refunded"

@dataclass
class PaymentIntent:
    """
    Stripe's PaymentIntent model.
    Immutable fields (id, created) + mutable state (status, amount_received).
    """
    id: str
    amount: int           # In smallest currency unit (cents for USD)
    currency: str
    status: PaymentStatus = PaymentStatus.PENDING
    amount_received: int = 0
    description: Optional[str] = None
    metadata: dict = field(default_factory=dict)
    
    # Audit fields
    created: float = field(default_factory=time.time)
    updated: float = field(default_factory=time.time)
    
    def confirm(self) -> 'PaymentIntent':
        """Returns new PaymentIntent (immutable update pattern)."""
        import copy
        new = copy.deepcopy(self)
        new.status = PaymentStatus.SUCCEEDED
        new.amount_received = self.amount
        new.updated = time.time()
        return new
    
    def to_api_response(self) -> dict:
        """Convert to API-safe dict (no internal fields)."""
        return {
            "id": self.id,
            "amount": self.amount,
            "amount_received": self.amount_received,
            "currency": self.currency,
            "status": self.status.value,
            "description": self.description,
            "created": int(self.created),
        }
```

---

## 5. Airbnb — Graph, Backtracking & API Design

### Interview Philosophy

Airbnb focuses on **practical problem-solving** with elegant solutions. They value design skills and the ability to handle complex state.

```
Airbnb Process:
  - Technical Screen: 1 round (phone)
  - Cross-functional: 1 round with non-engineer (product sense)
  - Onsite: 4 rounds (coding, system design, cross-functional, leadership)

Focus Areas:
  - Graph algorithms (booking conflicts, availability)
  - Backtracking (scheduling, search)
  - API design (search, booking, listing APIs)
  - Conflict resolution (concurrent bookings)
```

### Airbnb Problem 1: Alien Dictionary

```python
from collections import defaultdict, deque

def alienOrder(words: list[str]) -> str:
    """
    LeetCode 269. Alien Dictionary.
    
    Airbnb connection: Language/locale ordering matters for global listings.
    
    Build ordering graph from adjacent word pairs.
    Topological sort to determine character order.
    
    Time: O(C) where C = total characters | Space: O(1) — alphabet is fixed size
    """
    # Initialize all unique characters as nodes with 0 in-degree
    adj = defaultdict(set)
    in_degree = {c: 0 for word in words for c in word}
    
    for i in range(len(words) - 1):
        w1, w2 = words[i], words[i+1]
        min_len = min(len(w1), len(w2))
        
        # Check for invalid case: prefix order violated
        if len(w1) > len(w2) and w1[:min_len] == w2[:min_len]:
            return ""  # "abc" comes before "ab" is invalid
        
        for j in range(min_len):
            if w1[j] != w2[j]:
                if w2[j] not in adj[w1[j]]:
                    adj[w1[j]].add(w2[j])
                    in_degree[w2[j]] += 1
                break
    
    # Topological sort (Kahn's algorithm)
    queue = deque(c for c in in_degree if in_degree[c] == 0)
    order = []
    
    while queue:
        c = queue.popleft()
        order.append(c)
        for neighbor in adj[c]:
            in_degree[neighbor] -= 1
            if in_degree[neighbor] == 0:
                queue.append(neighbor)
    
    return ''.join(order) if len(order) == len(in_degree) else ""  # "" if cycle


# Tests
assert alienOrder(["wrt","wrf","er","ett","rftt"]) == "wertf"
assert alienOrder(["z","x"]) == "zx"
assert alienOrder(["z","x","z"]) == ""  # Cycle
```

### Airbnb Problem 2: Booking Conflict Resolution

```python
class BookingCalendar:
    """
    Availability calendar for Airbnb listings.
    Handle concurrent booking requests, check conflicts.
    
    This is a real Airbnb engineering problem:
    - Multiple guests request same dates simultaneously
    - Need atomic check + reserve
    """
    
    def __init__(self, listing_id: str):
        self.listing_id = listing_id
        self.bookings = []  # [(check_in, check_out, booking_id)]
        self._lock = Lock()
    
    def is_available(self, check_in: int, check_out: int) -> bool:
        """O(N) availability check."""
        for start, end, _ in self.bookings:
            if not (check_out <= start or check_in >= end):
                return False
        return True
    
    def book(self, check_in: int, check_out: int, 
             guest_id: str) -> Optional[str]:
        """Atomic check-and-book. Returns booking_id or None."""
        with self._lock:
            if not self.is_available(check_in, check_out):
                return None
            
            booking_id = f"BK_{guest_id}_{check_in}_{check_out}"
            self.bookings.append((check_in, check_out, booking_id))
            return booking_id
    
    def cancel(self, booking_id: str) -> bool:
        with self._lock:
            for i, (_, _, bid) in enumerate(self.bookings):
                if bid == booking_id:
                    self.bookings.pop(i)
                    return True
        return False
```

---

## 6. LinkedIn — Social Graph & Recommendation

### Interview Philosophy

LinkedIn focuses on **graph problems** (social networks) and **recommendation systems**. They value practical, scalable solutions.

```
LinkedIn Process:
  - Technical Screen: 1 round (45 min)
  - Onsite: 4-5 rounds (coding, system design, behavioral)
  - Common: "Degrees of separation" style graph problems

Focus Areas:
  - Graph BFS/DFS (degrees of connection)
  - Topological sort (skill endorsements, career paths)
  - Recommendation (job matching, people you may know)
  - Search ranking
```

### LinkedIn Problem 1: Degrees of Separation (BFS)

```python
def degrees_of_separation(graph: dict, user1: str, user2: str) -> int:
    """
    Find shortest connection path in LinkedIn network.
    
    1st degree: direct connection
    2nd degree: friend of a friend
    etc.
    
    Time: O(V + E) | Space: O(V)
    """
    if user1 == user2:
        return 0
    
    from collections import deque
    
    visited = {user1}
    queue = deque([(user1, 0)])
    
    while queue:
        user, degree = queue.popleft()
        
        for connection in graph.get(user, []):
            if connection == user2:
                return degree + 1
            if connection not in visited:
                visited.add(connection)
                queue.append((connection, degree + 1))
    
    return -1  # Not connected


def people_you_may_know(graph: dict, user: str, top_k: int = 5) -> list[str]:
    """
    "People You May Know" recommendation.
    
    Algorithm: Find 2nd-degree connections not already connected to user.
    Score by number of mutual connections.
    
    Time: O(V + E) | Space: O(V)
    """
    import heapq
    
    direct = set(graph.get(user, []))
    direct.add(user)  # Include self
    
    # Count mutual connections for each 2nd-degree connection
    mutual_count = {}
    
    for friend in direct - {user}:
        for second_degree in graph.get(friend, []):
            if second_degree not in direct:
                mutual_count[second_degree] = mutual_count.get(second_degree, 0) + 1
    
    # Return top-K by mutual connection count
    return heapq.nlargest(top_k, mutual_count, key=mutual_count.get)


# Test
network = {
    "Alice": ["Bob", "Charlie"],
    "Bob": ["Alice", "David", "Eve"],
    "Charlie": ["Alice", "Frank"],
    "David": ["Bob"],
    "Eve": ["Bob"],
    "Frank": ["Charlie"],
}

print(degrees_of_separation(network, "Alice", "Frank"))   # 2 (Alice→Charlie→Frank)
print(degrees_of_separation(network, "Alice", "David"))   # 2 (Alice→Bob→David)
print(people_you_may_know(network, "Alice"))               # [David, Eve, Frank]
```

### LinkedIn Problem 2: Job Recommendation (Graph + Scoring)

```python
def recommend_jobs(user_skills: set, job_graph: dict, 
                   top_k: int = 10) -> list[str]:
    """
    Job recommendation based on skill matching.
    
    Simple approach: Jaccard similarity between user skills and job requirements.
    Production LinkedIn: collaborative filtering + job popularity + career graph.
    
    Time: O(J * S) where J = jobs, S = skills per job
    """
    import heapq
    
    scores = []
    
    for job_id, job_data in job_graph.items():
        required = set(job_data.get("skills", []))
        if not required:
            continue
        
        # Jaccard similarity
        intersection = len(user_skills & required)
        union = len(user_skills | required)
        similarity = intersection / union if union > 0 else 0
        
        if similarity > 0:
            scores.append((similarity, job_id))
    
    return [job_id for _, job_id in heapq.nlargest(top_k, scores)]
```

---

## 7. Company Comparison Matrix

| Dimension | Apple | Netflix | Uber/Lyft | Stripe/Square | Airbnb | LinkedIn |
|-----------|-------|---------|-----------|---------------|--------|----------|
| **Algo Bar** | Medium | Medium | Medium-Hard | Medium | Medium-Hard | Medium |
| **Design Bar** | High | Very High | High | Very High | High | High |
| **Primary Language** | Swift/C++ | Java/Kotlin | Go/Python | Ruby/Go | React/Ruby | Java/Scala |
| **Top Focus** | Clean code | Distributed | Graphs/Geo | Finance/API | Graph/Search | Social Graph |
| **Coding Format** | 1 problem/round | 1-2/round | 1-2/round | 1-2/round | 1-2/round | 1-2/round |
| **Unique Round** | Platform-specific | Chaos/resilience | Maps problem | Payments API | Availability | Connections |
| **Behavioral** | Craft values | Freedom/Responsibility | Boldness | User-first | Belong anywhere | Opportunity |

### Universal Preparation Checklist

```python
# Before any company interview, verify you can code these in < 15 minutes:

must_know_cold = {
    "BFS": "level-order traversal, shortest path in graph",
    "DFS": "all-paths, cycle detection, topological sort",
    "Sliding Window": "longest substring, minimum window",
    "Two Pointers": "two-sum sorted, three-sum, merge",
    "Binary Search": "target in sorted array, search range",
    "DP 1D": "climbing stairs, house robber, coin change",
    "DP 2D": "LCS, edit distance, unique paths",
    "LRU Cache": "O(1) get/put with DLL + hashmap",
    "Trie": "insert, search, startsWith",
    "Union-Find": "find with path compression, union by rank",
    "Heap": "top-K elements, median from stream",
    "Monotonic Stack": "next greater element, histogram area",
}

# Company-specific additions:
company_additions = {
    "Apple": ["merge k sorted arrays", "string parsing"],
    "Netflix": ["consistent hashing", "rate limiter"],
    "Uber": ["dijkstra", "geohash", "bipartite matching"],
    "Stripe": ["Decimal arithmetic", "idempotency patterns"],
    "Airbnb": ["interval overlap", "alien dictionary"],
    "LinkedIn": ["degrees BFS", "recommendation scoring"],
}
```

---

*Each company has a unique engineering culture that shapes their interview process. Understanding WHAT they build helps you understand WHY they ask certain questions. Netflix asks about caching because they run one of the world's largest CDNs. Uber asks about routing because their core product is navigation. Stripe asks about financial precision because a rounding error could cost millions. Let the company's business context guide your preparation.*

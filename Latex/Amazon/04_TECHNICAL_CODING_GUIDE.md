# Technical Coding Guide — Amazon SDE2/SDE3

## Amazon Coding Interview Profile

Amazon coding interviews test:
1. **Problem-solving ability** — Can you decompose and solve a novel problem?
2. **Code quality** — Is your code clean, readable, and correct?
3. **Complexity analysis** — Do you understand time/space trade-offs?
4. **Communication** — Can you explain your thinking clearly?
5. **Edge case handling** — Do you think about boundary conditions?

**Difficulty level:** LeetCode Medium (primary) to Hard (Bar Raiser / SDE3)
**Preferred languages:** Python or Java (use your strongest — Java is fine)
**Format:** Live coding on shared editor (CoderPad or internal tool)

---

## The Amazon Coding Framework (Use Every Time)

### Step 1 — Clarify (2–3 min)
```
Always ask:
- "What are the input constraints?" (size, range, data types)
- "Can inputs be null/empty? How should I handle that?"
- "Is the input sorted or unsorted?"
- "What should I return if there's no valid answer?"
- "Are there any time/space constraints I should target?"

Say out loud: "Let me make sure I understand the problem.
               Given [X], I need to return [Y]. Is that correct?"
```

### Step 2 — Examples (2 min)
```
- Walk through the provided example
- Create 1 additional example
- Identify edge cases: empty input, single element, all same values,
  very large input, negative numbers
- Say: "Let me trace through this example to confirm my understanding..."
```

### Step 3 — Approach (3–5 min)
```
- Start with brute force ("The naive approach is O(n²) by...")
- Then optimize ("We can improve this to O(n) by using...")
- State time & space complexity before coding
- Get interviewer buy-in: "Does this approach make sense before I code?"
```

### Step 4 — Code (20–30 min)
```
- Write clean code with meaningful names
- Comment complex logic briefly
- Write helper functions if needed
- Don't over-optimize syntax — clarity wins
```

### Step 5 — Test (5 min)
```
- Trace through your code with provided example
- Test your edge case (empty input, etc.)
- Say: "Let me walk through my solution step by step..."
```

### Step 6 — Optimize (5 min)
```
- Ask: "Can I reduce time complexity further?"
- Discuss trade-offs: "If memory isn't a constraint, I could..."
- Mention follow-ups: "If data doesn't fit in memory, I'd use..."
```

---

## Critical Data Structures to Master

### 1. HashMap / HashSet — Your Most-Used Tool
```java
// O(1) lookup — use for frequency counts, seen sets, index maps
Map<Integer, Integer> freq = new HashMap<>();
for (int num : nums) freq.merge(num, 1, Integer::sum);
```
**Applies to:** Two Sum, Group Anagrams, Valid Anagram, Subarray Sum

### 2. Two Pointers
```java
// For sorted arrays, palindromes, container problems
int left = 0, right = n - 1;
while (left < right) {
    if (condition) left++;
    else right--;
}
```
**Applies to:** 3Sum, Container With Most Water, Trapping Rain Water

### 3. Sliding Window
```java
// For substrings/subarrays with a constraint
Map<Character, Integer> window = new HashMap<>();
int left = 0, maxLen = 0;
for (int right = 0; right < s.length(); right++) {
    window.merge(s.charAt(right), 1, Integer::sum);
    while (windowInvalid(window)) {
        window.merge(s.charAt(left), -1, Integer::sum);
        if (window.get(s.charAt(left)) == 0) window.remove(s.charAt(left));
        left++;
    }
    maxLen = Math.max(maxLen, right - left + 1);
}
```
**Applies to:** Longest Substring Without Repeating Characters, Minimum Window Substring

### 4. Binary Search (Advanced Usage)
```java
// Use when: sorted array, "find minimum/maximum valid X" problems
int lo = 0, hi = n;
while (lo < hi) {
    int mid = lo + (hi - lo) / 2;
    if (isValid(mid)) hi = mid;
    else lo = mid + 1;
}
return lo;
```
**Applies to:** Search in Rotated Array, Find Peak Element, Koko Eating Bananas

### 5. BFS — Level-Order / Shortest Path
```java
Queue<int[]> queue = new LinkedList<>();
boolean[][] visited = new boolean[rows][cols];
queue.offer(new int[]{startR, startC});
visited[startR][startC] = true;
int steps = 0;
while (!queue.isEmpty()) {
    int size = queue.size();
    for (int i = 0; i < size; i++) {
        int[] cell = queue.poll();
        if (isGoal(cell)) return steps;
        for (int[] dir : DIRS) {
            int nr = cell[0] + dir[0], nc = cell[1] + dir[1];
            if (inBounds(nr, nc) && !visited[nr][nc]) {
                visited[nr][nc] = true;
                queue.offer(new int[]{nr, nc});
            }
        }
    }
    steps++;
}
```
**Applies to:** Number of Islands, Word Ladder, Shortest Path in Matrix

### 6. DFS + Backtracking
```java
void dfs(int[] nums, int start, List<Integer> path, List<List<Integer>> result) {
    result.add(new ArrayList<>(path));
    for (int i = start; i < nums.length; i++) {
        path.add(nums[i]);
        dfs(nums, i + 1, path, result);
        path.remove(path.size() - 1);  // backtrack
    }
}
```
**Applies to:** Subsets, Permutations, Combination Sum, N-Queens

### 7. Dynamic Programming — 4 Steps
```
1. Define dp[i] clearly: "dp[i] = minimum cost to reach position i"
2. Write recurrence: "dp[i] = min(dp[i-1] + cost, dp[i-2] + cost)"
3. Set base case: "dp[0] = 0, dp[1] = cost[0]"
4. Fill table bottom-up (or memoize top-down)
```
**Applies to:** Climbing Stairs, Coin Change, LIS, House Robber

### 8. Monotonic Stack
```java
Deque<Integer> stack = new ArrayDeque<>();
int[] result = new int[n];
for (int i = n - 1; i >= 0; i--) {
    while (!stack.isEmpty() && nums[stack.peek()] <= nums[i]) stack.pop();
    result[i] = stack.isEmpty() ? -1 : stack.peek();
    stack.push(i);
}
```
**Applies to:** Daily Temperatures, Next Greater Element, Largest Rectangle in Histogram

---

## Amazon's Most Frequently Asked Problems

### Tier 1 — Guaranteed to Practice (High Frequency)

| # | Problem | Pattern | Difficulty |
|---|---|---|---|
| 1 | Two Sum | HashMap | Easy |
| 2 | LRU Cache | LinkedHashMap / DLL + Map | Medium |
| 3 | Number of Islands | BFS/DFS | Medium |
| 4 | Merge Intervals | Sorting | Medium |
| 5 | Meeting Rooms II | Heap + Sorting | Medium |
| 6 | Word Ladder | BFS | Hard |
| 7 | Trapping Rain Water | Two Pointers / Stack | Hard |
| 8 | Longest Substring Without Repeating | Sliding Window | Medium |
| 9 | Product of Array Except Self | Prefix/Suffix | Medium |
| 10 | Valid Parentheses | Stack | Easy |
| 11 | Binary Tree Level Order Traversal | BFS | Medium |
| 12 | Lowest Common Ancestor of BST | Tree Recursion | Medium |
| 13 | Serialize and Deserialize Binary Tree | BFS/DFS | Hard |
| 14 | Course Schedule | Graph / Topological Sort | Medium |
| 15 | Copy List with Random Pointer | HashMap | Medium |

### Tier 2 — Practice These Too (Medium Frequency)

| # | Problem | Pattern |
|---|---|---|
| 16 | Sliding Window Maximum | Monotonic Deque |
| 17 | Alien Dictionary | Topological Sort |
| 18 | Find Median from Data Stream | Two Heaps |
| 19 | Design Twitter | OOP + Heap |
| 20 | Min Stack | Stack |
| 21 | Kth Largest Element in Array | QuickSelect / Heap |
| 22 | Spiral Matrix | Array Simulation |
| 23 | Jump Game II | Greedy |
| 24 | Maximum Subarray | Kadane's Algorithm |
| 25 | Decode Ways | DP |
| 26 | Partition Equal Subset Sum | DP (0-1 Knapsack) |
| 27 | Pacific Atlantic Water Flow | DFS / BFS |
| 28 | Graph Valid Tree | Union-Find |
| 29 | Word Search II | Trie + DFS |
| 30 | Reconstruct Itinerary | Hierholzer's Algorithm |

---

## Amazon System Design Interview

### What Amazon Looks For in System Design

Unlike pure coding, system design is **mostly conversation and whiteboarding**. Amazon evaluates:

1. **Customer focus** — "Who are the users? What's their experience?"
2. **Scale reasoning** — "How does this work at 10x, 100x traffic?"
3. **Trade-off articulation** — "Why this choice vs that one?"
4. **Operational mindset** — "How do you monitor and debug this in production?"
5. **Simplicity** — "Can this be simpler without sacrificing requirements?"

### The Amazon System Design Framework

```
Total Time: 45–60 minutes

Phase 1 — Requirements (5–7 min):
  Functional: What must the system do?
  Non-Functional: Scale, latency, availability, consistency

Phase 2 — Scale Estimation (3–5 min):
  DAU, RPS (reads/writes), storage, bandwidth

Phase 3 — API Design (5 min):
  Define 3–5 core API endpoints

Phase 4 — High-Level Architecture (10 min):
  Draw: Clients → Load Balancer → Services → DB → Cache → MQ

Phase 5 — Deep Dives (15–20 min):
  Database schema, caching, queue, scalability, bottlenecks

Phase 6 — Trade-offs & Failure Modes (5–7 min):
  What breaks? How do you recover? What did you sacrifice?
```

---

## 5 Amazon System Design Problems — Deep Dives

### Problem 1: Design Amazon S3 (Object Storage)

**Requirements:**
- Store and retrieve arbitrary files (objects)
- 1 billion files, each up to 5TB
- 99.999999999% (11 nines) durability
- GET/PUT latency < 200ms

**Key Components:**
```
Client
  → API Gateway (handles auth, rate limiting)
  → Metadata Service (stores bucket/key → location mapping)
    → PostgreSQL / DynamoDB (key-value: bucket+key → chunk locations)
  → Storage Nodes (actual file data, chunked)
    → Erasure Coding or 3x replication for durability
  → Load Balancer (distributes reads/writes to storage nodes)
  → CDN (for hot object caching)
```

**Key Design Decisions:**
- **Chunking:** Split large files into 64MB chunks — parallel upload/download
- **Consistent Hashing:** Map object keys to storage nodes
- **Erasure Coding:** More space-efficient than 3x replication for durability
- **Eventual Consistency for reads:** S3 is eventually consistent (except for overwrite-then-read)

**Trade-off to Discuss:**
> "I chose eventual consistency for read-after-write because 11-nines durability is more important than strong consistency for object storage. If this were a financial ledger, I'd make different trade-offs."

---

### Problem 2: Design a URL Shortener (like bit.ly)

**Requirements:**
- Shorten long URLs → short URLs (e.g., amzn.to/xyz)
- Redirect short → long in < 10ms
- 100M URLs/day write, 1B redirects/day read
- URLs never expire (unless explicitly deleted)

**Scale Estimation:**
```
Writes: 100M/day = ~1,200 writes/sec
Reads:  1B/day   = ~11,500 reads/sec (read-heavy, ~10:1)
Storage: 100M URLs × 500 bytes = 50GB/day → 18TB/year
```

**Architecture:**
```
Client
  → API Gateway → Load Balancer
  → Write Service:
      - Generate 6-char base62 ID (62^6 = 56B unique URLs)
      - Store in PostgreSQL: short_id → long_url
  → Read Service:
      - Lookup short_id in Redis (cache)
      - Cache miss → PostgreSQL → cache write
      - Return 301/302 redirect
  → Analytics Service (async, via Kafka): log clicks
```

**Key Points:**
- **Base62 encoding** (a-z, A-Z, 0-9) for short IDs
- **Redis cache** for hot URLs (80/20 rule: 80% traffic hits 20% of URLs)
- **301 vs 302**: 301 = permanent (browser caches, less server traffic), 302 = temporary (log every click for analytics)
- **Hash collision**: Use UUID generation with DB unique constraint + retry

---

### Problem 3: Design Amazon's Order System

**Requirements:**
- Customers place orders
- Inventory deducted in real-time
- Order fulfillment tracked (placed → packed → shipped → delivered)
- 100K orders/sec at peak (Prime Day)

**Architecture:**
```
Client
  → API Gateway (auth, rate limit)
  → Order Service:
      - Validates order (item available, payment method valid)
      - Publishes OrderPlaced event to Kafka
  → Inventory Service:
      - Listens to OrderPlaced
      - Deducts stock atomically (optimistic locking or distributed lock)
      - Publishes InventoryReserved or InsufficientStock event
  → Payment Service:
      - Listens to InventoryReserved
      - Charges customer
      - Publishes PaymentConfirmed or PaymentFailed
  → Fulfillment Service:
      - Listens to PaymentConfirmed
      - Creates warehouse task
      - Updates order status (shipped, delivered)
  → Notification Service:
      - Listens to all events
      - Sends email/SMS to customer
```

**Key Design Points:**
- **Saga pattern** for distributed transaction (each step compensatable)
- **Idempotency keys** on each Kafka consumer to prevent duplicate processing
- **Dead Letter Queue** for failed events
- **Optimistic locking** on inventory: compare-and-swap prevents overselling

> **Amazon LP Angle:** "I designed this with Customer Obsession in mind — the customer gets real-time status updates at every step, and the Saga pattern ensures if payment fails, inventory is automatically released so other customers can purchase."

---

### Problem 4: Design a Notification System

**Requirements:**
- Send push, email, SMS notifications
- 100M notifications/day
- Latency < 1 second for push, < 5 minutes for email
- Retry on failure, deduplication

**Architecture:**
```
Source Events (any Amazon service)
  → Kafka Topic: "notification-requests"
  → Notification Router:
      - Reads from Kafka
      - Determines channel (push/email/SMS)
      - Deduplicates (Redis with message_id TTL 24h)
      - Routes to appropriate sender queue
  → Push Sender (FCM/APNs)
  → Email Sender (SES / SendGrid)
  → SMS Sender (SNS / Twilio)
  → Retry Service:
      - Failed notifications → exponential backoff queue
      - Max 3 retries → DLQ
  → Analytics: track open/click rates
```

**Key Points:**
- **Deduplication:** Redis SET with NX flag and TTL prevents duplicate sends
- **Priority queues:** Push notifications are time-sensitive; use separate queues with different consumer counts
- **Rate limiting per vendor:** SES has rate limits — use token bucket limiter
- **User preferences:** Store opt-out list in DynamoDB, check before sending

---

### Problem 5: Design a Distributed Rate Limiter

**Requirements:**
- Limit API requests per user (e.g., 1000 requests/minute)
- Works across multiple API Gateway instances (distributed)
- < 5ms latency overhead
- Handle millions of users

**Algorithms:**
| Algorithm | Description | Pros | Cons |
|---|---|---|---|
| Token Bucket | Tokens refill at fixed rate | Allows bursts | Harder to implement distributed |
| Sliding Window | Count requests in rolling window | Accurate | High memory usage |
| Fixed Window | Count per fixed time window | Simple | Edge case at window boundary |
| Leaky Bucket | Queue requests, process at fixed rate | Smooth output | Doesn't allow bursts |

**Architecture (Token Bucket on Redis):**
```
Request arrives at API Gateway instance
  → Rate Limiter Middleware:
      - key = user_id + minute_bucket
      - Redis INCRBY key 1 → returns current count
      - Redis EXPIRE key 60 (if new key)
      - If count > limit: return 429 Too Many Requests
      - Else: allow request
```

**Redis Lua Script (atomic check-and-increment):**
```lua
local current = redis.call('INCR', KEYS[1])
if current == 1 then
    redis.call('EXPIRE', KEYS[1], tonumber(ARGV[1]))
end
if current > tonumber(ARGV[2]) then
    return 0  -- rate limited
else
    return 1  -- allowed
end
```

**Key Points:**
- **Lua script = atomic** — prevents race conditions between INCR and check
- **Sliding window with sorted sets:** ZADD key timestamp member → ZCOUNT for window
- **Distributed:** All API instances share the same Redis cluster
- **Fallback:** If Redis is down, allow-through (availability > rate limiting)

---

## Weekly Practice Schedule

### Week 1 — Foundation
```
Mon: 5 Easy problems (Two Sum, Valid Parentheses, Reverse String)
Tue: 5 Medium — Arrays (3Sum, Maximum Subarray, Product Except Self)
Wed: 5 Medium — Trees (Level Order, LCA, Max Depth)
Thu: 5 Medium — Graphs (Number of Islands, Clone Graph, Course Schedule)
Fri: Full OA mock (2 problems, 90 min)
Sat: Review + redo failed problems
Sun: Rest
```

### Week 2 — Intermediate
```
Mon: 5 Medium — Sliding Window
Tue: 5 Medium — DP (Climbing Stairs, Coin Change, House Robber)
Wed: 3 Hard problems (Trapping Rain Water, Merge K Sorted, Word Ladder)
Thu: LRU Cache + Design Twitter (OOP problems)
Fri: Full phone screen mock (1 coding + 2 LP questions)
Sat: System design — URL Shortener
Sun: Rest
```

### Week 3 — Advanced
```
Mon: 3 Hard DP problems
Tue: System design — S3 (draw, time yourself)
Wed: System design — Order System
Thu: Full Loop simulation (2 coding + 1 system design)
Fri: LP story practice (record yourself, 3 min per story)
Sat: Mock interview (Pramp or friend)
Sun: Light review + rest
```

---

## Complexity Cheat Sheet

| Operation | Data Structure | Time |
|---|---|---|
| Access | Array | O(1) |
| Search | Array (unsorted) | O(n) |
| Search | Array (sorted) | O(log n) |
| Insert/Delete | Array | O(n) |
| Insert/Delete/Search | HashMap | O(1) avg |
| Insert/Delete/Search | BST (balanced) | O(log n) |
| Insert/Delete | Linked List | O(1) at head |
| Peek/Push/Pop | Stack/Queue | O(1) |
| Get min/max | Min/Max Heap | O(1) |
| Insert/Delete | Heap | O(log n) |
| Sorting | Any comparison sort | O(n log n) |

---

**Next:** Read `05_SYSTEM_DESIGN_DEEP_DIVE.md`

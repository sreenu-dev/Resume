# Technical Interview Guide — Microsoft (Coding + System Design)

## Microsoft Coding Interview — What's Different

| Dimension | Amazon | **Microsoft** |
|---|---|---|
| Difficulty | Medium–Hard (strict) | **Medium (occasionally Hard)** |
| Collaboration | Minimal hints, let you struggle | **Hints freely given, conversational** |
| Language | Any | **C# preferred (you're native here)** |
| Code Style | Correctness first | **Readability + correctness equally** |
| Follow-ups | Optimize aggressively | **"How would you test this?" common** |
| Behavioral in coding round | Rare | **Almost always 1–2 short Qs at start** |

---

## C# Code Quality — What Microsoft Expects

Since you'll likely use C#, here are the patterns Microsoft engineers look for:

### Use LINQ where appropriate
```csharp
// ✅ Microsoft-style: expressive and readable
var result = numbers
    .Where(n => n > 0)
    .GroupBy(n => n)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToList();

// ❌ Avoid: Java-style imperative loop when LINQ is clearer
var result = new List<int>();
for (int i = 0; i < numbers.Length; i++) { ... }
```

### Use meaningful names and generics
```csharp
// ✅ Clear intent
public Dictionary<int, List<int>> GroupByFrequency(int[] nums)

// ❌ Vague
public Dictionary<int, List<int>> Solve(int[] a)
```

### Null safety and edge cases
```csharp
// ✅ Guard clauses at top
public int[] TwoSum(int[] nums, int target)
{
    if (nums == null || nums.Length == 0) return Array.Empty<int>();
    
    var seen = new Dictionary<int, int>();
    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];
        if (seen.TryGetValue(complement, out int idx))
            return new[] { idx, i };
        seen[nums[i]] = i;
    }
    return Array.Empty<int>();
}
```

### Use appropriate C# data structures
```csharp
// Stack      → Stack<T>
// Queue      → Queue<T>
// Priority Q → PriorityQueue<TElement, TPriority>  (C# 6+)
// Set        → HashSet<T>
// Map        → Dictionary<TKey, TValue>
// Sorted Map → SortedDictionary<TKey, TValue>
```

---

## The Microsoft Coding Framework

### Step 1 — Understand (2–3 min)
```
"Let me clarify a few things before I start:
- Can the input array be empty or null?
- Are the integers guaranteed to be positive?
- Should I optimize for time or space?
- Is there a specific time complexity you're targeting?"
```

### Step 2 — Discuss approach (3 min)
```
"I'm thinking about [approach]. Let me explain why...
The time complexity would be O(n) and space O(n) using a HashMap.
Alternatively, I could do O(n log n) with O(1) space by sorting —
want me to go with the HashMap approach?"
```

### Step 3 — Code (20–30 min)
- Write clean C# with proper naming
- Add brief inline comments for non-obvious logic
- Don't rush — Microsoft values readability over speed

### Step 4 — Test (5 min)
```
"Let me trace through the example:
 Input: [2, 7, 11, 15], target = 9
 i=0: seen={}, complement=7, not found, add 2→0
 i=1: seen={2:0}, complement=2, FOUND at idx 0 → return [0,1] ✅"
```

### Step 5 — Testability discussion (Microsoft-specific)
```
Microsoft often asks: "How would you unit test this?"
Answer: "I'd use xUnit (which I've used in production). 
         Test cases would include:
         1. Happy path — target found
         2. Empty array → return empty
         3. Single element → return empty  
         4. Duplicate elements
         5. Target = sum of same element twice (e.g., [3,3], target=6)"
```

---

## Microsoft's Most Frequently Asked Coding Problems

### Tier 1 — High Probability (Practice These First)

| # | Problem | Pattern | C# Hint |
|---|---|---|---|
| 1 | Two Sum | HashMap | `Dictionary<int,int>` |
| 2 | Valid Parentheses | Stack | `Stack<char>` |
| 3 | Merge Two Sorted Lists | Linked List | Pointer manipulation |
| 4 | Maximum Depth of Binary Tree | DFS Recursion | Recursive |
| 5 | Binary Tree Level Order Traversal | BFS | `Queue<TreeNode>` |
| 6 | Number of Islands | DFS/BFS on Grid | `bool[,] visited` |
| 7 | Longest Substring Without Repeating | Sliding Window | `HashSet<char>` |
| 8 | Best Time to Buy and Sell Stock | One Pass | Track min price |
| 9 | Reverse Linked List | Pointer | Iterative |
| 10 | LRU Cache | DLL + HashMap | `LinkedList<T>` |
| 11 | Serialize / Deserialize Binary Tree | BFS + String | `Queue<TreeNode>` |
| 12 | Find All Anagrams in a String | Sliding Window | `int[26]` freq array |
| 13 | Word Search | DFS Backtrack | `char[,]` grid |
| 14 | Course Schedule | Topological Sort | `List<int>[]` adj list |
| 15 | Merge Intervals | Sorting | `List<int[]>` |

### Tier 2 — Medium Probability

| # | Problem | Pattern |
|---|---|---|
| 16 | Design Twitter / OOP problems | OOP + Heap |
| 17 | Spiral Matrix | Array simulation |
| 18 | Jump Game | Greedy |
| 19 | Coin Change | DP |
| 20 | Longest Palindromic Substring | DP or Expand Around Center |
| 21 | Flatten Binary Tree to Linked List | Tree DFS |
| 22 | House Robber | DP |
| 23 | Product of Array Except Self | Prefix/Suffix |
| 24 | Kth Largest Element | Heap / QuickSelect |
| 25 | Decode Ways | DP |

---

## System Design — Microsoft Style (Azure-Native)

### The Microsoft System Design Framework

```
Total Time: 60 minutes

Phase 1 — Requirements (5–8 min):
  "Let me clarify requirements before diving in..."
  Functional: What the system must do
  Non-functional: Scale, latency, availability, cost

Phase 2 — Scale Estimation (3–5 min):
  DAU, RPS reads/writes, storage needs

Phase 3 — High-Level Architecture (8–10 min):
  Use Azure services by name — this signals Microsoft alignment

Phase 4 — Azure Service Selection (10 min):
  "For messaging I'd use Azure Service Bus because..."
  "For storage I'd use Cosmos DB because..."

Phase 5 — Deep Dives (15–20 min):
  Database schema, caching, retry logic, monitoring

Phase 6 — Operational Excellence (5 min):
  Azure Monitor, Application Insights, alerting, cost control
```

### Azure Service Quick Reference (Know These)

| Need | Azure Service | Notes |
|---|---|---|
| Object storage | **Azure Blob Storage** | Like S3; use for files, images, logs |
| NoSQL database | **Azure Cosmos DB** | Multi-model, globally distributed |
| Relational DB | **Azure SQL / Azure Database for PostgreSQL** | Managed PaaS |
| Caching | **Azure Cache for Redis** | Managed Redis |
| Messaging (queues) | **Azure Service Bus** | Enterprise messaging, dead-letter queues |
| Event streaming | **Azure Event Hubs** | Kafka-compatible; for high-throughput streams |
| Serverless compute | **Azure Functions** | Event-triggered, pay-per-execution |
| API management | **Azure API Management** | Rate limiting, auth, routing |
| Containers | **Azure Kubernetes Service (AKS)** | Managed Kubernetes |
| Monitoring | **Azure Monitor + Application Insights** | Metrics, logs, traces |
| Auth/Identity | **Azure Active Directory / Entra ID** | OAuth2, managed identities |
| CDN | **Azure Front Door / CDN** | Global content delivery |
| Search | **Azure Cognitive Search** | Full-text + vector search |

---

## 5 Microsoft-Specific System Design Problems — Deep Dives

### Problem 1: Design Microsoft Teams Messaging

**Requirements:**
- Real-time 1:1 and group messaging
- 300M+ daily active users
- Message delivery in < 100ms
- Message history (5 years)
- Read receipts and presence (online/offline)

**Scale Estimation:**
```
300M DAU × 20 messages/day = 6B messages/day
= 70,000 messages/second peak
Message size: ~1KB average
Storage: 6B × 1KB = 6TB/day → ~2PB/year
```

**Architecture:**
```
Client (Teams App)
  → Azure Front Door (global routing + SSL termination)
  → Azure API Management (rate limiting, auth via Azure AD)
  → WebSocket Gateway Service (maintain persistent connections)
    → Azure SignalR Service (managed WebSocket at scale)
  → Message Service:
      - Write: Azure Service Bus → Message Consumer → Cosmos DB
      - Read: Azure Cosmos DB (recent) + Azure Blob Storage (archive)
  → Presence Service:
      - Azure Cache for Redis (user online status, TTL-based)
  → Notification Service:
      - Azure Notification Hubs → push to iOS/Android
  → Azure Monitor + Application Insights (observability)
```

**Key Design Points:**
- **Azure SignalR Service** handles WebSocket connections at scale (millions simultaneous)
- **Cosmos DB** for message storage — partitioned by conversation_id
- **Redis** for presence (online status) — TTL auto-expires stale presence
- **Fan-out for group messages:** Message → Service Bus → parallel delivery to all recipients

**Microsoft LP Angle:**
> "I'd use Azure SignalR Service rather than building our own WebSocket infrastructure — this embodies the 'use the platform' principle. Microsoft built SignalR specifically for this problem at scale."

---

### Problem 2: Design Azure Blob Storage (Simplified)

**Requirements:**
- Store arbitrary files up to 5TB
- 11 nines of durability
- GET/PUT latency < 200ms
- Billions of files

**Architecture:**
```
Client
  → Azure Front Door (global load balancing)
  → Storage Gateway:
      - Authenticate via Azure AD managed identity
      - Validate request, generate SAS token if needed
  → Metadata Service:
      - Stores: account/container/blob → physical location
      - Database: Azure Cosmos DB (geo-distributed)
  → Storage Nodes:
      - Files chunked into 64MB blocks
      - Locally Redundant Storage (LRS): 3 copies in same datacenter
      - Geo-Redundant Storage (GRS): 6 copies across 2 regions
  → Azure CDN (cache hot blobs at edge)
  → Azure Monitor (metrics: storage capacity, throughput, latency)
```

**Key Points:**
- **LRS vs GRS vs RA-GRS** — know the difference and when to use each
- **Soft delete + versioning** — protects against accidental deletion
- **Lifecycle policies** — auto-tier blobs to Cool/Archive storage by age
- **Cost optimization** — move cold data to Archive tier (much cheaper)

---

### Problem 3: Design GitHub Actions (CI/CD Pipeline Service)

**Requirements:**
- Trigger builds on git push/PR events
- Execute user-defined workflow steps (YAML)
- Support 1M+ repositories
- Build logs streaming in real-time
- Scalable runner pool (compute for running jobs)

**Architecture:**
```
Git Push Event
  → GitHub Webhook → Azure Event Hubs (event ingestion)
  → Workflow Scheduler:
      - Parses YAML workflow definition
      - Determines trigger conditions
      - Queues jobs to Azure Service Bus
  → Runner Pool Manager:
      - Maintains warm pool of AKS pods (build runners)
      - Auto-scales via KEDA (Kubernetes Event-Driven Autoscaling)
      - Allocates runner to job from Service Bus queue
  → Build Runner (AKS pod):
      - Executes workflow steps
      - Streams logs → Azure Event Hubs → Azure Blob Storage
  → Log Streaming:
      - Azure SignalR Service → real-time log push to browser
  → Artifact Storage:
      - Build outputs → Azure Blob Storage
  → Notification:
      - Build result → Azure Service Bus → Email/Slack notification
```

**Key Points:**
- **KEDA** (Kubernetes Event-Driven Autoscaling) — scales runners based on queue depth
- **Ephemeral runners** — each job gets a fresh container (security isolation)
- **Log chunking** — stream logs in chunks, not one blob (real-time UX)
- **Rate limiting** — Azure API Management prevents abuse

---

### Problem 4: Design a Distributed Rate Limiter for Azure API Management

**Requirements:**
- Enforce per-user or per-subscription API quotas
- Work across multiple API Gateway instances
- < 5ms latency overhead
- 10M API calls/day

**Architecture:**
```
API Request
  → Azure API Management (APIM)
      → Rate Limiter Policy (built into APIM):
          - Uses Azure Cache for Redis for distributed state
          - Lua-style atomic scripts: INCR + EXPIRE
          - Returns 429 Too Many Requests if over limit
      → Backend Service (if allowed)
```

**Azure-Native Approach:**
```
"Azure API Management has built-in rate limiting policies.
 For finer-grained control, I'd use:
 - Azure Cache for Redis with sliding window counters
 - Redis INCR + EXPIRE atomic operations
 - Fallback: if Redis is unavailable, use APIM's local in-memory 
   limiter (allows some traffic during Redis downtime)"
```

**Trade-off to Discuss:**
> "I chose Redis over a distributed consensus approach because rate limiting is a best-effort guarantee — a small number of extra requests getting through during Redis failover is acceptable. Availability > strict accuracy for rate limiting."

---

### Problem 5: Design Microsoft 365 Notification System

**Requirements:**
- Send notifications across email, Teams, mobile push, browser push
- 300M+ users
- Delivery guarantee (at-least-once)
- User notification preferences (opt-in/opt-out per channel)
- Priority tiers (critical alerts vs digest summaries)

**Architecture:**
```
Source Event (any M365 service: Teams, Calendar, SharePoint)
  → Azure Service Bus (topic per notification type)
  → Notification Router Service:
      - Reads user preferences from Cosmos DB
      - Determines channels to send to
      - Deduplicates (Redis with message_id + TTL)
      - Fans out to channel-specific queues
  → Channel Senders:
      - Email: Azure Communication Services / SendGrid
      - Teams: Teams Bot Framework notification API
      - Mobile Push: Azure Notification Hubs (iOS + Android)
      - Browser Push: Web Push API (via Azure Functions)
  → Retry/DLQ:
      - Failed deliveries → Service Bus Dead Letter Queue
      - Exponential backoff retry (3 attempts)
      - After 3 failures → alert ops team via Azure Monitor
  → Preference Service:
      - Cosmos DB: user_id → {email: true, push: false, ...}
      - Cached in Redis for hot users
```

**Microsoft LP Angle:**
> "I'd use Azure Communication Services for email delivery rather than building SMTP infrastructure — Microsoft has invested heavily in this managed service specifically to handle scale, compliance, and deliverability at Microsoft's global scale."

---

## Testing Your Code — Microsoft Cares More Than Amazon

Microsoft is a company that ships operating systems, productivity software, and cloud infrastructure used by 1 billion+ people. They have a strong testing culture.

**When Microsoft asks "how would you test this?":**

```
"I'd structure tests in three layers:

1. Unit Tests (xUnit / NUnit):
   - Test each method in isolation
   - Mock external dependencies (DB, Redis)
   - Cover: happy path, empty input, boundary conditions, invalid input

2. Integration Tests:
   - Test the component end-to-end with real dependencies
   - Use TestContainers or in-memory stubs for speed

3. Load/Stress Tests:
   - Simulate peak RPS using Azure Load Testing
   - Verify latency SLAs hold under load
   - Find memory leak behavior under sustained load"
```

---

## Complexity Reference Card

| Algorithm | Time | Space |
|---|---|---|
| Binary Search | O(log n) | O(1) |
| BFS / DFS | O(V + E) | O(V) |
| Merge Sort | O(n log n) | O(n) |
| QuickSort (avg) | O(n log n) | O(log n) |
| Heap Insert/Delete | O(log n) | O(n) |
| HashMap Insert/Lookup | O(1) avg | O(n) |
| Sliding Window | O(n) | O(k) |
| DP (bottom-up) | O(n²) typical | O(n) with optimization |

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding Fundamentals
  Mon: 5 HashMap/Array problems
  Tue: 5 Tree problems (BFS + DFS)
  Wed: 5 String/Sliding Window
  Thu: 3 Graph problems
  Fri: Full coding mock (45 min, C#)

Week 2: System Design + Advanced Coding
  Mon: Design Teams Messaging (60 min, from scratch)
  Tue: Design Azure Blob Storage (60 min)
  Wed: 5 DP problems (Coin Change, House Robber, Decode Ways)
  Thu: Design GitHub Actions pipeline
  Fri: Full mock: 1 coding + 1 system design

Week 3: Integration + Behavioral
  Mon: 5 Hard problems (LRU Cache, Serialize Binary Tree)
  Tue: Design Notification System + Rate Limiter
  Wed: Full behavioral mock (5 Growth Mindset questions, 45 min)
  Thu: Full loop simulation (2 coding + 1 SD + 1 behavioral)
  Fri: Rest + light review
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

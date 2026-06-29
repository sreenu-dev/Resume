# Technical Interview Guide — JPMC (Coding + Financial System Design)

## JPMC Coding Interview — What's Different

| Dimension      | Amazon             | Microsoft                  | **JPMC**                           |
| ----------------| --------------------| ----------------------------| ------------------------------------|
| Language       | Any                | C# preferred               | **Java preferred**                 |
| Difficulty     | Medium–Hard        | Medium                     | **Medium–Hard**                    |
| Code style     | Correct + optimal  | Clean + testable           | **Correct + secure + risk-aware**  |
| Follow-ups     | "Optimize further" | "How would you test this?" | **"What could go wrong here?"**    |
| Domain context | None               | None                       | **May use finance framing**        |
| Concurrency    | Rare               | Rare                       | **Sometimes asked (Java threads)** |

---

## Java Code Quality — What JPMC Expects

### Use Modern Java (Java 11–17+)
```java
// ✅ JPMC-style: streams, Optional, var
public Optional<Integer> findTarget(int[] nums, int target) {
    return IntStream.range(0, nums.length)
        .filter(i -> nums[i] == target)
        .boxed()
        .findFirst();
}

// ✅ Use var for local inference (Java 10+)
var seen = new HashMap<Integer, Integer>();

// ✅ Records for data classes (Java 16+)
record Transaction(String id, BigDecimal amount, Instant timestamp) {}
```

### Thread Safety — JPMC Cares
```java
// ✅ ConcurrentHashMap for shared state
private final Map<String, Integer> counter = new ConcurrentHashMap<>();

// ✅ AtomicInteger for counters
private final AtomicInteger processed = new AtomicInteger(0);

// ✅ ReentrantLock for complex critical sections
private final ReentrantLock lock = new ReentrantLock();
try {
    lock.lock();
    // critical section
} finally {
    lock.unlock();
}
```

### Financial Data — Use BigDecimal, Not Double
```java
// ❌ NEVER use double for money — floating-point precision errors
double balance = 100.10 + 200.20;  // → 300.29999999999995 !!

// ✅ Always use BigDecimal for financial amounts
BigDecimal balance = new BigDecimal("100.10")
    .add(new BigDecimal("200.20"));  // → 300.30 exactly
```
> **Saying this unprompted in a JPMC interview will impress the interviewer.** It shows you understand financial domain constraints.

### Guard Clauses and Validation
```java
// ✅ Validate inputs early — financial systems must reject bad data
public TransactionResult process(Transaction tx) {
    Objects.requireNonNull(tx, "Transaction must not be null");
    if (tx.amount().compareTo(BigDecimal.ZERO) <= 0) {
        throw new IllegalArgumentException("Amount must be positive");
    }
    if (tx.id() == null || tx.id().isBlank()) {
        throw new IllegalArgumentException("Transaction ID required");
    }
    // ... process
}
```

---

## The JPMC Coding Framework

### Step 1 — Clarify (2–3 min)
```
"Before coding, a few clarifying questions:
- What are the input constraints (size, value range)?
- Can inputs be null or empty — how should I handle that?
- Is this a financial amount? If so, I'll use BigDecimal.
- Are there concurrency requirements — could this be called by multiple threads?
- Should I optimize for time or space?"
```

### Step 2 — Risk Discussion (JPMC-specific, 1 min)
```
"One thing I want to flag upfront: [if it's a transaction/financial problem]
I'll make sure the solution handles edge cases like duplicate transaction IDs,
overflow risk on very large amounts, and empty/null inputs — these would be
critical failure modes in a financial context."
```

### Step 3 — Approach + Code + Test
Same as Amazon/Microsoft — clarify, approach, code, test.

### Step 4 — "What Could Go Wrong?" Discussion
```
After coding, proactively say:
"Let me also think about failure modes:
- If this is called concurrently, [X] could cause a race condition — 
  I'd add [synchronization/ConcurrentHashMap/atomic operation]
- If the input set is very large, we could hit memory pressure — 
  I'd consider a streaming approach
- In a financial context, this method should have an idempotency key 
  to prevent double-processing"
```

---

## JPMC's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern | Java Hint |
|---|---|---|---|
| 1 | Two Sum | HashMap | `HashMap<Integer, Integer>` |
| 2 | Valid Parentheses | Stack | `Deque<Character>` |
| 3 | LRU Cache | DLL + HashMap | `LinkedHashMap` override |
| 4 | Merge Intervals | Sort | `Arrays.sort` + list |
| 5 | Number of Islands | BFS/DFS | `boolean[][]` visited |
| 6 | Longest Substring Without Repeating | Sliding Window | `HashSet<Character>` |
| 7 | Binary Tree Level Order Traversal | BFS | `Queue<TreeNode>` |
| 8 | Find Median from Data Stream | Two Heaps | `PriorityQueue` |
| 9 | Design HashMap | Hashing | Array of buckets |
| 10 | Course Schedule | Topological Sort | Kahn's Algorithm |
| 11 | Kth Largest Element | Heap | `PriorityQueue` (min-heap size k) |
| 12 | Sliding Window Maximum | Monotonic Deque | `ArrayDeque<Integer>` |
| 13 | Clone Graph | BFS + HashMap | `HashMap<Node, Node>` |
| 14 | Word Ladder | BFS | `Set<String>` for O(1) lookup |
| 15 | Serialize / Deserialize Binary Tree | BFS | `Queue<TreeNode>` |

### Tier 2 — Finance-Context Problems (JPMC-Specific)

| # | Problem | Finance Context |
|---|---|---|
| 16 | Max Profit Stock Buy/Sell | Portfolio return optimization |
| 17 | Running sum / prefix sum | Real-time balance aggregation |
| 18 | Top K frequent elements | Most-traded securities |
| 19 | Implement a Rate Limiter | API throttling for payment APIs |
| 20 | Design a Thread-Safe Counter | Concurrent transaction count |

---

## Financial System Design — 5 Deep Dives

### Problem 1: Design a Payment Processing System

**Context:** Process payment transactions between bank accounts (like JPMC's ACH/wire transfer system)

**Requirements:**
```
Functional:
  - Transfer money from Account A to Account B
  - Support multiple currencies
  - Idempotent — retrying a payment doesn't double-charge
  - Eventually consistent account balances
  - Full audit trail for every transaction

Non-Functional:
  - 100,000 transactions per second at peak
  - < 500ms end-to-end latency (p99)
  - 99.99% availability (52 minutes downtime/year)
  - Zero data loss — every transaction must complete or roll back cleanly
```

**Scale Estimation:**
```
100K TPS = 8.64 billion transactions/day
Average transaction: ~500 bytes
Storage: 8.64B × 500B = 4.3TB/day
Transaction IDs must be globally unique (use UUID v4 or Snowflake IDs)
```

**Architecture:**
```
Client / Mobile App
  → API Gateway (rate limiting, auth via OAuth2/JWT)
  → Payment Orchestration Service:
      - Validates idempotency key (Redis: SET NX with TTL)
      - Validates accounts (Account Service)
      - Calls Payment Processor asynchronously
      - Returns immediately with payment_id + status=PENDING
  → Kafka Topic: "payment-commands"
  → Payment Processor (Kafka Consumer):
      - Reads payment command
      - Acquires distributed lock on both accounts (Redis Redlock)
      - Executes debit + credit atomically (DB transaction)
      - Publishes PaymentCompleted / PaymentFailed event
      - Releases locks
  → Account Service:
      - PostgreSQL with row-level locking for account balances
      - Read replicas for balance queries
  → Audit Log Service:
      - Immutable append-only log in Kafka + S3
      - Every state change written with timestamp + actor
  → Notification Service:
      - Listens to PaymentCompleted/Failed events
      - Sends confirmation to payer/payee
```

**Key Design Points (Probe These):**

**Idempotency:**
```
"Every payment request includes an idempotency_key from the client.
Before processing, I check Redis: SET idempotency:{key} processing NX EX 300
If key exists → return cached result (duplicate request)
If not → process and store result before returning"
```

**Distributed Transaction (Saga Pattern):**
```
"I use the Saga pattern rather than 2PC because 2PC creates lock contention
at scale. The Saga:
  Step 1: Debit source account → emit DebitCompleted
  Step 2: Credit target account → emit CreditCompleted
  Step 3: Complete payment record
  
Compensation (rollback):
  If Step 2 fails → emit DebitReversal → reverse Step 1
  If Step 3 fails → emit CreditReversal → reverse Step 2"
```

**JPMC Risk Question You'll Hear:**
> "What happens if the Kafka consumer crashes after debiting but before crediting?"

**Answer:**
```
"The Saga's compensating transaction handles this. The Kafka offset for 
the debit event is NOT committed until both debit AND credit succeed. 
If the consumer crashes mid-saga, the debit event is re-delivered and 
we check idempotency: if debit already happened, skip directly to credit. 
This guarantees exactly-once semantics."
```

---

### Problem 2: Design a Fraud Detection System

**Context:** Real-time detection of fraudulent transactions at JPMC scale

**Requirements:**
```
Functional:
  - Score every transaction in < 100ms (before approval)
  - Block transactions above risk threshold
  - Flag for manual review at medium risk
  - Learn from fraud patterns (ML model)
  - Low false-positive rate (< 0.1%) — don't block legitimate transactions

Non-Functional:
  - 50,000 transactions/second
  - p99 latency < 100ms (synchronous path to transaction approval)
  - 99.999% availability
```

**Architecture:**
```
Transaction Event (from Payment Service)
  → Fraud Detection API (synchronous, < 100ms SLA):
      - Feature extraction: device fingerprint, IP, location, amount, velocity
      - Check rules engine (fast, deterministic): known fraud patterns
      - Call ML scoring service: trained gradient boosting model
      - Aggregate score → APPROVE / FLAG / BLOCK
      → Return decision to Payment Service
  
  → Async Enrichment (parallel, < 5 seconds):
      - User behavior profiling (Kafka stream → feature store)
      - Network graph analysis (device/IP/account relationships)
      - Update feature store for future scoring
  
  → Feature Store (Redis):
      - Per-user: transaction velocity (last 1h, 24h, 7d)
      - Per-device: usage history
      - Per-merchant: fraud rate
  
  → ML Model Store:
      - Pre-trained model loaded in-memory
      - Model retraining pipeline runs daily (Spark + S3)
      - A/B testing for new model versions
  
  → Audit & Review Queue:
      - Flagged transactions → human review dashboard
      - Blocked transactions → case management system
      - All decisions logged immutably to Kafka + S3
```

**Key Points:**
- **Velocity features in Redis** — "How many transactions from this card in last hour?" — O(1) lookup
- **Rules engine first** — fast, deterministic rules catch obvious fraud before ML scoring
- **ML model in-memory** — don't call a remote service; latency budget is 100ms total
- **Feedback loop** — fraud confirmed by humans → retrain model
- **Explainability** — FINRA requires you can explain why a transaction was blocked

---

### Problem 3: Design a Financial Audit Log System

**Context:** Every action on every financial account must be immutably recorded (SOX compliance)

**Requirements:**
```
Functional:
  - Record every write operation: amount, actor, timestamp, before/after state
  - Query: all events for account X in time range Y–Z
  - Proof of non-tampering (cryptographic integrity)
  - 7-year data retention (regulatory requirement)

Non-Functional:
  - Write latency < 10ms (must not slow down main transaction path)
  - Query latency < 500ms for up to 1 million records
  - Immutability — records cannot be modified or deleted
```

**Architecture:**
```
Transaction Service (any write operation)
  → Audit Event (async, fire-and-forget):
      {event_id, entity_id, action, actor, timestamp, 
       previous_state, new_state, correlation_id}
  → Kafka Topic: "audit-events" (replicated, retention=7years)
  → Audit Writer Service (Kafka Consumer):
      - Writes to Audit Store (append-only)
      - Computes hash: SHA-256(previous_hash + event_payload)
      - Stores hash chain (blockchain-lite pattern)
  → Audit Store:
      - Hot storage (0–90 days): PostgreSQL (partitioned by month)
      - Warm storage (90 days–2 years): S3 + Athena for SQL queries
      - Cold storage (2–7 years): S3 Glacier (retrieval SLA: hours)
  → Query API:
      - GET /audit/{entity_id}?from=&to=
      - Routes to PostgreSQL (hot) or S3/Athena (warm/cold)
  → Integrity Verification:
      - Periodic hash chain verification job
      - Alert if any record's hash chain breaks (tampering detected)
```

**Key Design Points:**
- **Append-only** — database user has INSERT permission only, no UPDATE/DELETE
- **Hash chain** — each record contains hash of previous record (cryptographic proof of order)
- **Async write** — audit log must never slow down the main transaction path
- **Tiered storage** — hot/warm/cold significantly reduces cost (90% of audit queries are for recent data)
- **Correlation ID** — links audit events to the originating request for full trace reconstruction

**JPMC Risk Question:**
> "What if the audit writer service crashes and we lose audit events?"

**Answer:**
```
"Kafka durability handles this. The audit event is committed to Kafka 
FIRST (synchronously from the transaction service). The audit writer 
reads from Kafka — if it crashes, it restarts and re-reads from the 
last committed offset. No events are lost. Kafka retention is set to 
7 years as the primary backup."
```

---

### Problem 4: Design a Real-Time Trade Settlement System

**Context:** CIB Tech — matching buy/sell trade orders and settling positions

**Requirements:**
```
Functional:
  - Match buy and sell orders by price/time priority
  - Settle matched trades: transfer securities + cash
  - Real-time position tracking per trader/fund
  - Handle market hours: burst at open/close

Non-Functional:
  - 500,000 orders/second at market open
  - Matching latency < 1ms (microsecond ideal)
  - Zero position miscalculation — financial loss if wrong
```

**Architecture:**
```
Order Entry Gateway:
  - FIX Protocol handler (financial messaging standard)
  - Validates order: symbol, quantity, price, account
  - Assigns sequence number (monotonically increasing)
  - Publishes to Order Book Service
  
Order Book Service (in-memory, single-threaded):
  - Red-Black Tree or skip list per symbol (sorted by price)
  - Price-time priority matching algorithm
  - When match found → emit TradeExecuted event to Kafka
  
Settlement Engine (Kafka Consumer):
  - TradeExecuted → debit securities from seller, credit to buyer
  - Debit cash from buyer, credit to seller
  - Update position ledger (Kafka event sourcing)
  
Position Service:
  - Real-time positions per account per symbol
  - Materialized from Kafka event stream
  - Redis cache for current positions (read-heavy)
  
Risk Engine (parallel):
  - Checks pre-trade risk limits before order enters book
  - Real-time P&L monitoring
  - Margin requirement calculations
```

**Key Points:**
- **Single-threaded order book** — eliminates locking, maximizes throughput
- **FIX Protocol** — industry standard for financial messaging (know this name)
- **Event sourcing** — entire position history derivable from trade event stream
- **Low-latency Java** — avoid GC pauses: use off-heap memory (Chronicle Map)

---

### Problem 5: Design a Rate Limiter for Payment APIs

**(Same architecture as Amazon — but frame it for financial context)**

**JPMC Financial Framing:**
```
"This is critical for JPMC's payment APIs because:
1. Prevents automated fraud attacks (credential stuffing, brute force)
2. Protects downstream banking systems from cascade overload
3. Ensures fair resource allocation across merchant clients
4. Meets PCI-DSS requirement for access control"
```

**Architecture:** Redis token bucket with Lua script atomic operation
(Same as Amazon prep — refer to `Amazon_Interview_Prep/04_TECHNICAL_CODING_GUIDE.md`)

**Add this JPMC-specific point:**
```
"For payment APIs specifically, I'd use a sliding window (not fixed window) 
because fixed windows have a boundary attack: a client can send 2× the 
limit by sending requests at the end of one window and the start of the next. 
In a fraud context, that 2× burst window matters."
```

---

## Java Concurrency — JPMC Senior-Level Topic

For VP-level positions, expect at least one concurrency question.

### Thread-Safe Singleton (Classic)
```java
public class PaymentService {
    // Double-checked locking pattern — thread-safe, efficient
    private static volatile PaymentService instance;
    
    private PaymentService() {}
    
    public static PaymentService getInstance() {
        if (instance == null) {
            synchronized (PaymentService.class) {
                if (instance == null) {
                    instance = new PaymentService();
                }
            }
        }
        return instance;
    }
}

// Better: use enum singleton (Josh Bloch's Effective Java)
public enum PaymentService {
    INSTANCE;
    // thread-safe by JVM class loading guarantee
}
```

### Producer-Consumer with BlockingQueue
```java
// Common in financial messaging systems
BlockingQueue<Transaction> queue = new LinkedBlockingQueue<>(1000);

// Producer thread
void produce(Transaction tx) throws InterruptedException {
    queue.put(tx);  // blocks if queue full
}

// Consumer thread
void consume() throws InterruptedException {
    while (true) {
        Transaction tx = queue.take();  // blocks if empty
        process(tx);
    }
}
```

### CompletableFuture for Parallel Risk Checks
```java
// Run multiple risk checks in parallel, combine results
CompletableFuture<RiskScore> fraudScore = 
    CompletableFuture.supplyAsync(() -> fraudService.score(tx));
CompletableFuture<RiskScore> creditScore = 
    CompletableFuture.supplyAsync(() -> creditService.score(tx));
CompletableFuture<RiskScore> amlScore = 
    CompletableFuture.supplyAsync(() -> amlService.score(tx));

CompletableFuture.allOf(fraudScore, creditScore, amlScore)
    .thenApply(v -> combineScores(
        fraudScore.join(), creditScore.join(), amlScore.join()
    ));
```

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding + Java Fundamentals
  Mon: 5 medium problems in Java (Two Sum, Valid Parentheses, BFS tree)
  Tue: LRU Cache + Design HashMap (OOP + data structure combined)
  Wed: 5 medium problems (Merge Intervals, Sliding Window)
  Thu: Java concurrency: Thread-safe counter, BlockingQueue
  Fri: Full OA mock in Java (2 problems, 90 min, HackerRank)
  Sat: Review + fix Java-specific issues
  Sun: Rest

Week 2: System Design + Finance
  Mon: Design Payment Processing System (60 min, write it all out)
  Tue: 5 medium problems (Graphs, Topological Sort)
  Wed: Design Fraud Detection System (60 min)
  Thu: Design Audit Log System (60 min)
  Fri: Full coding mock in Java (45 min, 1 problem + explanation)
  Sat: BigDecimal, Streams, Optional review
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (2 coding + 1 system design)
  Tue: Write out 8 behavioral stories (JPMC culture framework)
  Wed: 5 hard problems (Median from Stream, Word Ladder)
  Thu: Design Trade Settlement System (60 min)
  Fri: Mock behavioral interview (45 min, record yourself)
  Sat: Light review + question prep per interviewer type
  Sun: Rest
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

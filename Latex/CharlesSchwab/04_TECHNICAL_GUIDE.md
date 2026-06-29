# Technical Interview Guide — Charles Schwab (Coding + Financial System Design)

## Schwab Coding Interview — What's Different

| Dimension          | Amazon            | JPMC             | ServiceNow           | **Schwab**                                                              |
| --------------------| -------------------| ------------------| ----------------------| -------------------------------------------------------------------------|
| Language           | Any               | Java preferred   | Java/Python          | **Java, C#, Python — all fine**                                         |
| Difficulty         | Medium–Hard       | Medium–Hard      | Medium               | **Medium**                                                              |
| Code style         | Correct + optimal | Correct + secure | Correct + enterprise | **Correct + tested + financial-grade**                                  |
| Critical follow-up | "Optimize"        | "What fails?"    | "How to scale?"      | **"What if a transaction fails mid-way?" / "How would you test this?"** |
| Domain context     | None              | Finance          | Enterprise workflows | **Financial transactions**                                              |
| Compliance angle   | None              | Sometimes        | Audit trail          | **Always — SEC/FINRA**                                                  |

---

## Schwab's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern | Schwab Financial Follow-up |
|---|---|---|---|
| 1 | Two Sum | HashMap | How would you test this for financial input edge cases? |
| 2 | Valid Parentheses | Stack | How would you handle malformed financial messages? |
| 3 | LRU Cache | DLL + HashMap | How would you ensure cache consistency with the database? |
| 4 | Merge Intervals | Sorting | How would you handle overlapping trading time windows? |
| 5 | Number of Islands | DFS/BFS | What happens if a node fails mid-traversal? |
| 6 | Design LRU Cache | HashMap + DLL | How would you make this thread-safe for concurrent orders? |
| 7 | Serialize/Deserialize Tree | BFS | How would you handle corrupt financial data during deserialization? |
| 8 | Find Median from Data Stream | Two Heaps | How would you use this for real-time price feeds? |
| 9 | Clone Graph | BFS + HashMap | How would this handle a disconnected account graph? |
| 10 | Course Schedule | Topological Sort | How would you detect circular dependencies in settlement chains? |
| 11 | Min Stack | Stack | How would you log every push/pop for audit purposes? |
| 12 | Design Rate Limiter | Token Bucket | How would you rate-limit by client account? |
| 13 | Word Ladder | BFS | What is the time complexity at large scale? |
| 14 | Kth Largest Element | Heap | How would you use this for portfolio ranking? |
| 15 | Design HashMap | Hashing | How would you guarantee thread safety for concurrent trade updates? |

### Schwab-Specific Coding Variants
Schwab interviewers may add financial context:

```
Standard: "Design a Rate Limiter"
Schwab variant: "Design a rate limiter for Schwab's trading API that 
                limits each client account to 200 orders per second, 
                prevents abuse, and logs all throttled requests for 
                FINRA audit purposes."

Standard: "Implement an LRU Cache"
Schwab variant: "Design an in-memory position cache for a brokerage 
                system. It must be thread-safe (concurrent order updates), 
                evict stale data, and guarantee eventual consistency 
                with the underlying database."
```

---

## Financial System Design — 5 Deep Dives

### Problem 1: Design a Trade Order Management System (OMS)

**Context:** Schwab's core system — routes client orders to exchanges and tracks execution.

**Requirements:**
```
Functional:
  - Accept buy/sell orders from clients (market, limit, stop)
  - Route orders to appropriate exchange (NYSE, NASDAQ, etc.)
  - Track order lifecycle: PENDING → SUBMITTED → PARTIAL_FILL → FILLED → CANCELLED
  - Handle partial fills (order partially executed)
  - Cancel orders before execution

Non-Functional:
  - < 10ms order submission latency (p99)
  - Exactly-once order submission (no duplicate orders)
  - Full audit trail of every order state change (SEC Rule 17a-4)
  - 99.999% availability during market hours (9:30 AM – 4 PM ET)
  - 100,000 orders per second at peak
```

**Architecture:**
```
Client (Web / Mobile / API)
  → API Gateway (Auth + Rate Limiting)
  → Order Ingestion Service:
      - Validates order (account exists, sufficient balance/margin)
      - Generates unique idempotency key (prevents duplicate submission)
      - Persists order to DB with status: PENDING
      - Publishes OrderSubmitted event to Kafka

Order Router:
  - Subscribes to OrderSubmitted
  - Selects best execution venue (exchange or dark pool)
  - Sends order to exchange via FIX protocol
  - Updates order status: SUBMITTED

Exchange Adapter:
  - Receives fill confirmations from exchange
  - Publishes OrderFilled / OrderPartiallyFilled events to Kafka
  - Handles rejection events (insufficient liquidity, etc.)

Position Service:
  - Subscribes to fill events
  - Updates client's position in real-time
  - CQRS: write via command, read via read replica

Audit Log Service:
  - Subscribes to ALL order events
  - Persists immutable records: order_id, client_id, timestamp, action, details
  - Stored in append-only, tamper-evident log (SEC 17a-4 compliance)
  - Retention: 7 years (regulatory requirement)

Alert Service:
  - Notifies client of fills, cancellations, rejections via push/email/SMS
```

**Key Design Points:**
- **Idempotency key** — prevents double-order submission if client retries
- **Exactly-once semantics** — Kafka transaction + DB write in same transaction (Transactional Outbox pattern)
- **Audit log is immutable** — append-only, never updated, retained 7 years
- **State machine** — PENDING → SUBMITTED → FILLED/CANCELLED enforces valid transitions
- **Circuit breaker** — if exchange is down, graceful degradation (queue orders, retry)

**The Question You WILL Be Asked:**
> "What happens if the service crashes after sending the order to the exchange but before updating the database?"

**Answer:**
```
"This is the classic exactly-once problem. I solve it with the 
Transactional Outbox pattern:
1. Write the order AND the outbox event in a single DB transaction
2. A separate outbox publisher reads from the outbox and publishes 
   to Kafka
3. If the service crashes between step 1 and step 2, the outbox 
   publisher retries — but the order was already persisted safely
4. If the order was already sent to the exchange, the idempotency 
   key prevents a duplicate submission on retry

This guarantees exactly-once — no order is lost, no order is doubled."
```

---

### Problem 2: Design a Real-Time Portfolio Tracking System

**Context:** Every Schwab client's dashboard shows their portfolio value updating in real time.

**Requirements:**
```
Functional:
  - Show current portfolio value (total and per holding)
  - Update in near real-time as prices change
  - Show P&L (profit/loss) per position
  - Support 35 million client accounts
  - Historical performance charts

Non-Functional:
  - < 2 second latency from price change to client dashboard update
  - Handle 50,000 price updates per second (market data feed)
  - 99.99% availability
  - Read-heavy workload (clients view portfolio >> clients trade)
```

**Architecture:**
```
Market Data Feed (NYSE/NASDAQ/CME):
  → Market Data Ingestion Service:
      - Normalizes price feed (different exchanges have different formats)
      - Publishes PriceUpdated events to Kafka: { symbol, price, timestamp }

Portfolio Calculation Engine:
  - Subscribes to PriceUpdated events
  - For each updated symbol, recalculates portfolio values for affected accounts
  - Optimisation: maintain inverted index { symbol → [account_ids] } in Redis
  - Publishes PortfolioUpdated events per account

Position Store (CQRS):
  - Command side: updated by trade execution (fills → positions)
  - Query side: read replica (PostgreSQL read replica) for dashboard queries
  - Redis cache: current portfolio value per account (hot path)

Client Notification (WebSocket / Push):
  - Maintains persistent WebSocket connections per active client
  - Pushes portfolio updates in real-time to connected clients
  - Fallback: polling API for mobile clients without WebSocket

Historical Performance Store:
  - Time-series DB (Cassandra / InfluxDB): daily portfolio snapshots
  - Used for performance charts (1-day, 1-month, 1-year, all-time)
```

---

### Problem 3: Design a Fraud Detection Engine for Financial Transactions

**Context:** Schwab must detect fraudulent trades, logins, and transfers in real time.

**Requirements:**
```
Functional:
  - Detect suspicious patterns: unusual login location, large unexpected transfer,
    wash trading (buying and selling same security to manipulate price)
  - Respond in < 500ms (block suspicious transaction before execution)
  - Human review queue for borderline cases
  - Learn from confirmed fraud cases over time

Non-Functional:
  - < 0.1% false positive rate (legitimate transactions blocked)
  - < 0.01% false negative rate (fraud transactions missed)
  - 99.99% availability
  - Full audit trail of every fraud decision
```

**Architecture:**
```
Transaction Stream (Kafka):
  - All orders, logins, transfers published to fraud detection topic

Feature Extraction Service:
  - Real-time feature computation per transaction:
      - User behaviour baseline (normal trading pattern for this client)
      - Device fingerprint
      - Velocity check (how many transactions in last N minutes)
      - Geographic anomaly (login from new country)
      - Amount anomaly (transfer > 10x typical)

Rule Engine (fast path):
  - Hard rules: instantly block definite fraud
    (login from blacklisted IP, wire transfer to sanctioned country)
  - Response: < 10ms

ML Scoring Service (medium path):
  - Runs ML model (XGBoost or neural net) on extracted features
  - Returns fraud probability score
  - Response: < 100ms

Decision Engine:
  - Score > 0.9 → AUTO_BLOCK + notify client + create case
  - Score 0.5–0.9 → HOLD + send to human review queue
  - Score < 0.5 → ALLOW (log for model training)

Human Review Queue:
  - Analyst reviews held transactions
  - Decision feeds back to ML model as training data

Audit Log:
  - Every fraud decision logged immutably: transaction, features, score, decision
  - Required for regulatory reporting (FINRA AML rules)
```

---

### Problem 4: Design a Client Account Aggregation Platform

**Context:** Schwab clients who also hold assets at other brokers (Fidelity, Vanguard) want to see all their assets in one place.

**Requirements:**
```
- Securely connect to external financial institutions via OAuth2
- Aggregate balances, holdings, transactions from external accounts
- Refresh data every 24 hours (or on-demand)
- Never store external credentials
- Display unified portfolio across all accounts
```

**Architecture:**
```
OAuth2 Authorization Flow:
  - Client grants Schwab read-only access to external account
  - Schwab stores only OAuth2 refresh token (encrypted, in HSM)
  - Never stores username/password

External Account Connectors:
  - Per-institution connectors (Fidelity, Vanguard, etc.)
  - Or: use Plaid / MX financial data aggregation API
  - Fetch: balances, holdings, recent transactions

Data Normalization Service:
  - Each institution has different data formats
  - Normalizes to Schwab's canonical Account, Position, Transaction model

Aggregated Portfolio Service:
  - Merges Schwab accounts + external accounts
  - Calculates unified net worth, P&L, asset allocation
  - Respects data freshness: marks stale external data clearly

Refresh Scheduler:
  - Cron job: refreshes external accounts every 24 hours
  - On-demand: client can trigger refresh manually
  - Rate limiting: respect external institution API limits

Security:
  - Encrypted storage of OAuth2 tokens (HSM)
  - Read-only OAuth2 scopes (cannot initiate transactions)
  - Audit log: every access to external account data
```

---

### Problem 5: Design a Financial Notification & Alert System

**Context:** Schwab needs to send real-time alerts to clients: order fills, price alerts, margin calls, fraud alerts.

**Requirements:**
```
- Multiple channels: push notification, email, SMS, in-app
- Priority levels: critical (margin call, fraud) vs. informational (price alert)
- Client preference management (which alerts, which channels)
- Compliance: some alerts are legally required (margin call, account restriction)
- Delivery confirmation (required for regulatory alerts)
```

**Architecture:** *(Similar to ServiceNow Notification System — mention this)*
```
Alert Event Source (Kafka):
  - OrderFilled, PriceAlertTriggered, MarginCallIssued, FraudDetected

Alert Routing Service:
  - Resolves client preference: which channels for this alert type?
  - Priority routing: critical alerts bypass all rate limits
  - Ensures legally-required alerts are always delivered

Channel Adapters:
  - Push: Firebase / APNs
  - Email: SendGrid / SES
  - SMS: Twilio
  - In-app: WebSocket

Delivery Tracker:
  - Tracks delivery status per alert per channel
  - For regulatory alerts: retry until confirmed delivery
  - Compliance report: "margin call notification sent at [timestamp], 
    delivered at [timestamp], read at [timestamp]"
```

---

## Financial Reliability Pattern — Always Mention This

When designing any Schwab system, always include:

```
1. Idempotency keys        → prevent duplicate transactions
2. Transactional outbox    → guarantee exactly-once event publishing
3. Immutable audit log     → SEC 17a-4 compliance (7-year retention)
4. Circuit breaker         → protect against exchange/external API failures
5. Reconciliation job      → detect and alert on data inconsistencies
6. Graceful degradation    → during market hours, partial service > no service
```

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding + Financial Context
  Mon: 5 medium problems (Two Sum, Valid Parentheses, Merge Intervals)
  Tue: 5 medium problems (LRU Cache, Sliding Window, BFS Tree)
  Wed: Design Rate Limiter + Min Stack (financial context)
  Thu: 5 medium problems (Heaps, Graphs)
  Fri: Full mock (45 min, 1 problem + project deep dive with financial angle)
  Sat: Review
  Sun: Rest

Week 2: Financial System Design
  Mon: Design Trade Order Management System (60 min — most important)
  Tue: 5 medium problems + practice "exactly-once" explanation
  Wed: Design Real-Time Portfolio Tracking System (60 min)
  Thu: Design Fraud Detection Engine (60 min)
  Fri: Full coding mock + project deep dive
  Sat: Review
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (1 coding + 1 SD + 1 behavioral)
  Tue: Design Account Aggregation Platform (60 min)
  Wed: 5 medium-hard problems
  Thu: Full behavioral mock (Schwab values, 30 min)
  Fri: Design Notification System (40 min) + final review
  Sat: Rest
  Sun: Interview (or continue)
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

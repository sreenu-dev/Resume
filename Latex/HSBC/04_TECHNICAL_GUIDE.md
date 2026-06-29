# Technical Interview Guide — HSBC (Coding + Global System Design)

## HSBC Coding Interview — What's Different

| Dimension      | Amazon             | JPMC                   | **HSBC**                                 |
| ----------------| --------------------| ------------------------| ------------------------------------------|
| Language       | Any                | Java preferred         | **Any (.NET, Python, Java)**             |
| Difficulty     | Medium–Hard        | Medium–Hard            | **Medium**                               |
| Code style     | Correct + optimal  | Correct + secure       | **Correct + maintainable + testable**    |
| Follow-ups     | "Optimize further" | "What could go wrong?" | **"How would you scale this globally?"** |
| Domain context | None               | Finance                | **Global banking + compliance**          |
| Concurrency    | Rare               | Sometimes              | **Sometimes**                            |

---

## HSBC's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern |
|---|---|---|
| 1 | Two Sum | HashMap |
| 2 | Valid Parentheses | Stack |
| 3 | Merge Two Sorted Lists | Linked List |
| 4 | Binary Tree Level Order Traversal | BFS |
| 5 | Number of Islands | DFS/BFS |
| 6 | LRU Cache | DLL + HashMap |
| 7 | Longest Substring Without Repeating | Sliding Window |
| 8 | Merge Intervals | Sorting |
| 9. | Course Schedule | Topological Sort |
| 10 | Serialize / Deserialize Binary Tree | BFS |
| 11 | Find Median from Data Stream | Two Heaps |
| 12 | Kth Largest Element | Heap |
| 13 | Clone Graph | BFS + HashMap |
| 14 | Word Ladder | BFS |
| 15 | Design Rate Limiter | System Design |

---

## Global System Design — 5 Deep Dives

### Problem 1: Design a Global Payment System (Multi-Region)

**Context:** HSBC processes trillions in cross-border payments. Your system must work globally.

**Requirements:**
```
Functional:
  - Transfer money between accounts globally
  - Support multiple currencies and settlement rules
  - Idempotent — retrying doesn't double-charge
  - Full audit trail for regulatory compliance
  - Real-time settlement across regions

Non-Functional:
  - 100,000 TPS at peak
  - < 500ms end-to-end latency (p99)
  - 99.99% availability across all regions
  - Zero data loss
  - GDPR, RBI, PCI-DSS compliance
  - Data residency: EU data stays in EU, India data in India
```

**Architecture (Multi-Region):**
```
Global Load Balancer (GeoDNS)
  → Regional API Gateways (EU, APAC, Americas)
  → Regional Payment Services:
      - Validate idempotency (Redis per region)
      - Call Payment Processor (Kafka per region)
  → Regional Settlement Engines:
      - Debit/credit atomically
      - Publish settlement events
  → Cross-Region Reconciliation:
      - Kafka replication across regions
      - Eventual consistency for settlement finality
  → Audit Log Service:
      - Immutable logs per region
      - Replicated for disaster recovery
  → Compliance Service:
      - GDPR: EU data doesn't leave EU
      - RBI: India data doesn't leave India
      - PCI-DSS: encryption in transit + at rest
```

**Key Design Points:**
- **Data residency:** EU data stays in EU, India data in India (GDPR/RBI requirement)
- **Idempotency:** Redis per region with TTL
- **Settlement finality:** Saga pattern with compensating transactions
- **Audit trail:** Immutable, replicated across regions
- **Disaster recovery:** Multi-region failover with RTO < 1 hour

---

### Problem 2: Design a Multi-Region Data Replication System

**Context:** HSBC needs to replicate data across regions for disaster recovery and compliance.

**Requirements:**
```
- Replicate data from primary region to secondary regions
- Consistency: eventual consistency (not strong consistency)
- Latency: replication lag < 5 seconds
- Availability: survive entire region failure
- Compliance: respect data residency rules
```

**Architecture:**
```
Primary Region (EU)
  → Kafka Topic: "data-changes"
  → Replication Service:
      - Reads from Kafka
      - Filters by data residency rules
      - Replicates to secondary regions
  → Secondary Regions (APAC, Americas):
      - Receive replicated data
      - Update local read replicas
      - Can serve reads (eventual consistency)
  → Conflict Resolution:
      - Last-write-wins for simple data
      - Custom resolution for complex data
```

---

### Problem 3: Design a Fraud Detection System (Global)

**Requirements:**
```
- Score every transaction in < 100ms
- Detect fraud patterns across all regions
- Block suspicious transactions
- Low false-positive rate (< 0.1%)
- Comply with regulatory requirements
```

**Architecture:**
```
Transaction Event (from Payment Service)
  → Fraud Detection API (< 100ms SLA):
      - Feature extraction: device, IP, location, amount, velocity
      - Rules engine: known fraud patterns
      - ML model: trained on global fraud data
      - Aggregate score → APPROVE / FLAG / BLOCK
  → Async Enrichment:
      - User behavior profiling (global)
      - Network graph analysis (cross-region)
  → Feature Store (Redis):
      - Per-user velocity (global, not per-region)
      - Per-merchant fraud rate
  → Audit & Review:
      - Flagged transactions → human review
      - All decisions logged immutably
```

---

### Problem 4: Design an API Gateway for Open Banking

**Context:** HSBC is building open banking APIs for fintech partnerships.

**Requirements:**
```
- Expose HSBC banking APIs to external partners
- Rate limiting per partner
- Authentication & authorization
- API versioning
- Monitoring & analytics
- PCI-DSS compliance
```

**Architecture:**
```
External Partner
  → API Gateway (Azure API Management):
      - Authentication (OAuth2 / API key)
      - Rate limiting per partner
      - Request/response validation
      - API versioning
      - Logging & monitoring
  → Backend Services:
      - Account Service
      - Payment Service
      - Transaction Service
  → Compliance Layer:
      - PCI-DSS encryption
      - Audit logging
      - Data masking (PII)
```

---

### Problem 5: Design a Cross-Border Settlement System

**Context:** HSBC settles trillions in cross-border payments daily.

**Requirements:**
```
- Match buy/sell orders across regions
- Settle in local currencies
- Comply with local settlement rules
- Reconciliation across regions
- Audit trail for regulatory inspection
```

**Architecture:**
```
Order Entry (Global)
  → Order Matching Engine (per currency pair):
      - Price-time priority
      - Emit SettlementOrder events
  → Settlement Service (per region):
      - Debit in source currency
      - Credit in destination currency
      - Handle FX conversion
      - Emit SettlementConfirmed
  → Reconciliation Service:
      - Match settled orders across regions
      - Detect discrepancies
      - Alert on mismatches
  → Audit Log:
      - Every settlement step logged
      - Immutable, replicated globally
```

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding Fundamentals
  Mon: 5 medium problems (Two Sum, Valid Parentheses, BFS Tree)
  Tue: 5 medium problems (Merge Intervals, Sliding Window)
  Wed: LRU Cache + Design HashMap
  Thu: 5 medium problems (Graphs, Topological Sort)
  Fri: Full OA mock (90 min, 2 problems)
  Sat: Review + fix
  Sun: Rest

Week 2: System Design + Global Thinking
  Mon: Design Global Payment System (60 min)
  Tue: 5 medium problems (Heaps, Stacks)
  Wed: Design Multi-Region Data Replication (60 min)
  Thu: Design Fraud Detection System (60 min)
  Fri: Full coding mock (45 min, 1 problem)
  Sat: Review
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (2 coding + 1 SD)
  Tue: 5 hard problems (Median from Stream, Word Ladder)
  Wed: Design API Gateway for Open Banking (60 min)
  Thu: Full behavioral mock (45 min, 5 questions)
  Fri: Design Cross-Border Settlement (60 min)
  Sat: Final review
  Sun: Rest
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

# System Design Questions & Answers - Quick Summary

## 📄 Document Overview

**File**: `System_Design_Questions_and_Answers.md` (54 KB)

This comprehensive document contains **3 complete system design questions with detailed answers**, specifically tailored for Chubb's interview process.

---

## 🎯 Three Complete Examples

### Question 1: Design a Policy Management System ⭐⭐⭐

**Scenario**: Design a system to manage 100 million insurance policies

**What You'll Learn:**
- Database schema design with proper indexing
- Sharding strategy (hash-based on customer_id)
- Caching with Redis (1-hour TTL)
- Elasticsearch for full-text search
- Kafka for event streaming
- REST API design with pagination
- Optimistic locking for concurrency
- Idempotency handling
- Circuit breaker pattern
- Monitoring and metrics

**Key Components:**
```
Load Balancer → API Gateway → API Servers → Cache (Redis)
                                    ↓
                            Database (PostgreSQL)
                                    ↓
                        Message Queue (Kafka)
                                    ↓
                        Search Engine (Elasticsearch)
```

**Scale:**
- 100 million policies
- 10,000 concurrent users
- 10,000 requests/second
- 200ms read latency, 500ms write latency

**Database Design:**
- `policies` table with proper indexes
- `policy_details` for flexible attributes
- `policy_history` for audit trail
- Composite indexes for common queries
- BRIN indexes for time-series data

**Caching Strategy:**
- Cache key: `policy:{policy_id}` (TTL: 1 hour)
- Cache invalidation on updates
- Write-through pattern

**Search:**
- Elasticsearch for policy search
- Full-text search on policy_number, customer_name
- Date range filters
- Aggregations for analytics

**API Endpoints:**
```
GET /api/v1/policies/{policyId}
POST /api/v1/policies
PUT /api/v1/policies/{policyId}
DELETE /api/v1/policies/{policyId}
GET /api/v1/policies/search
GET /api/v1/policies/{policyId}/history
```

**Trade-offs:**
- Strong consistency over eventual consistency
- PostgreSQL over NoSQL (ACID compliance)
- Hash-based sharding over range-based (even distribution)

---

### Question 2: Design a Claims Processing System ⭐⭐⭐

**Scenario**: Design a system to process 100,000 claims per day

**What You'll Learn:**
- Workflow state machine design
- Document management with S3
- Multi-stage approval process
- Audit trail implementation
- Event-driven notifications
- Multi-channel communication
- Claim status tracking
- Compliance and audit logging

**Key Components:**
```
Customer Portal → API Gateway → Claims API → Kafka Queue
                                    ↓
                            PostgreSQL Database
                                    ↓
                        Workflow Engine (State Machine)
                                    ↓
                    Email/SMS/Push Notification Services
```

**Scale:**
- 100,000 claims/day
- 1,000 concurrent submissions
- 24-48 hours for simple claims
- 7-14 days for complex claims

**Claim Workflow States:**
```
SUBMITTED → UNDER_REVIEW → APPROVED → PAID → CLOSED
                    ↓
                 REJECTED → CLOSED
```

**Database Schema:**
- `claims` table (policy_id, customer_id, status, amount)
- `claim_documents` table (S3 integration)
- `claim_workflow` table (current stage, assigned_to)
- `claim_approvals` table (audit trail)
- `claim_history` table (all changes)

**Document Management:**
- Upload to S3 with encryption
- Metadata stored in PostgreSQL
- Max 100 MB per document
- Max 10 documents per claim
- File validation (type, size)

**Approval Process:**
- Multiple stages (3-5 depending on type)
- Assigned to specific handlers
- Comments and decisions tracked
- Audit trail for compliance

**Notifications:**
- Email on submission
- SMS on status change
- Push notifications
- Multi-language support

**Key Features:**
- Real-time status updates
- Document management
- Workflow routing
- Compliance tracking
- Audit trail
- Multi-channel notifications

---

### Question 3: Design a Real-Time Notification System ⭐⭐⭐

**Scenario**: Design a system to send 1 million notifications per day

**What You'll Learn:**
- High-throughput message processing
- Multi-channel delivery (Email, SMS, Push)
- Retry logic with exponential backoff
- Scheduled notifications
- Delivery tracking
- Worker pattern for scalability
- Idempotency and deduplication

**Key Components:**
```
Notification Sources → API Gateway → Kafka Queue
                                    ↓
                    ┌───────────────┼───────────────┐
                    ↓               ↓               ↓
                Email Worker   SMS Worker    Push Worker
                    ↓               ↓               ↓
                    └───────────────┼───────────────┘
                                    ↓
                            PostgreSQL (History)
```

**Scale:**
- 1 million notifications/day
- 10,000 notifications/second peak
- 95% delivered within 5 minutes
- 99% delivered within 30 minutes

**Notification Model:**
```csharp
- NotificationId (UUID)
- UserId
- Type (CLAIM_SUBMITTED, CLAIM_APPROVED, etc.)
- Channel (EMAIL, SMS, PUSH)
- Subject, Body
- Status (PENDING, SENT, FAILED, BOUNCED)
- ScheduledFor
- RetryCount
- CreatedAt
```

**Channels:**
1. **Email**
   - Via email provider (SendGrid, AWS SES)
   - Subject + Body
   - Unsubscribe handling

2. **SMS**
   - Via SMS provider (Twilio, AWS SNS)
   - Body only (160 chars)
   - Phone number validation

3. **Push Notifications**
   - Via push provider (Firebase, OneSignal)
   - Title + Body
   - Multi-device support

**Retry Logic:**
- Exponential backoff: 2^n * 60 seconds
- Max 3 retries
- Failed after 3 retries

**Scheduling:**
- Schedule up to 30 days in advance
- Scheduler runs every minute
- Checks for notifications due in next 5 minutes
- Re-publishes to Kafka

**Key Features:**
- At-least-once delivery
- Exponential backoff retries
- Scheduled notifications
- Delivery tracking
- Multi-channel support
- Horizontal scaling
- Idempotent processing

---

## 📊 Comparison Table

| Aspect | Policy Management | Claims Processing | Notifications |
|---|---|---|---|
| **Primary Focus** | Data consistency | Workflow management | High throughput |
| **Scale** | 100M policies | 100K claims/day | 1M notifications/day |
| **Consistency** | Strong | Strong | Eventual |
| **Key Challenge** | Search & caching | Workflow & approvals | Delivery reliability |
| **Main DB** | PostgreSQL | PostgreSQL | PostgreSQL |
| **Cache** | Redis | Redis | Redis |
| **Search** | Elasticsearch | - | - |
| **Messaging** | Kafka | Kafka | Kafka |
| **External** | - | S3 (documents) | Email/SMS/Push APIs |

---

## 🎓 Key Concepts Covered

### Database Design
- ✅ Schema design with relationships
- ✅ Indexing strategies (B-tree, BRIN, composite)
- ✅ Sharding and partitioning
- ✅ Replication and backup
- ✅ Query optimization

### Caching
- ✅ Cache invalidation strategies
- ✅ TTL and expiration
- ✅ Cache-aside pattern
- ✅ Write-through pattern
- ✅ Distributed caching

### Messaging & Events
- ✅ Kafka topics and partitions
- ✅ Event publishing and consuming
- ✅ At-least-once delivery
- ✅ Idempotent processing
- ✅ Dead letter queues

### API Design
- ✅ REST principles
- ✅ Request/response formats
- ✅ Error handling
- ✅ Rate limiting
- ✅ Pagination

### Scalability
- ✅ Horizontal scaling
- ✅ Load balancing
- ✅ Database sharding
- ✅ Caching layers
- ✅ Async processing

### Reliability
- ✅ Retry logic
- ✅ Circuit breaker pattern
- ✅ Fallback mechanisms
- ✅ Health checks
- ✅ Monitoring & alerting

### Compliance & Security
- ✅ Audit trails
- ✅ Data encryption
- ✅ Access control
- ✅ Compliance logging
- ✅ PII handling

---

## 💡 Interview Tips Using These Examples

### How to Use These Examples

1. **Study the Architecture**
   - Understand each component
   - Know why each component is needed
   - Understand the trade-offs

2. **Practice Explaining**
   - Explain the architecture out loud
   - Discuss why you chose each technology
   - Explain the trade-offs

3. **Adapt to Questions**
   - If asked about policies, use Question 1
   - If asked about workflows, use Question 2
   - If asked about notifications, use Question 3
   - If asked about something else, adapt these patterns

4. **Go Deeper**
   - Be ready to explain database schema
   - Discuss indexing strategies
   - Explain caching invalidation
   - Discuss failure scenarios

5. **Discuss Trade-offs**
   - Why PostgreSQL over MongoDB?
   - Why Kafka over RabbitMQ?
   - Why Redis over Memcached?
   - Why Elasticsearch over database search?

---

## 🔍 Common Follow-up Questions

### For Policy Management
1. How would you handle 10x growth?
2. How would you implement multi-region?
3. How would you handle policy conflicts?
4. How would you implement versioning?
5. How would you handle data migration?

### For Claims Processing
1. How would you handle stuck workflows?
2. How would you implement manual overrides?
3. How would you handle document expiration?
4. How would you implement SLA tracking?
5. How would you handle appeals?

### For Notifications
1. How would you handle delivery failures?
2. How would you prevent duplicate notifications?
3. How would you implement rate limiting per user?
4. How would you handle unsubscribes?
5. How would you implement A/B testing?

---

## 📈 Scalability Scenarios

### Policy Management
- **Current**: 100M policies, 10K RPS
- **10x Growth**: 1B policies, 100K RPS
  - Increase shards from 10 to 100
  - Increase cache cluster size
  - Increase search cluster size
  - Add more API servers

### Claims Processing
- **Current**: 100K claims/day, 1K concurrent
- **10x Growth**: 1M claims/day, 10K concurrent
  - Increase database replicas
  - Add more worker processes
  - Increase Kafka partitions
  - Add more notification workers

### Notifications
- **Current**: 1M notifications/day, 10K/sec
- **10x Growth**: 10M notifications/day, 100K/sec
  - Increase Kafka partitions
  - Add more worker instances
  - Increase database connections
  - Add caching for templates

---

## 🎯 What Interviewers Look For

### Technical Knowledge
- ✅ Understanding of distributed systems
- ✅ Knowledge of databases and caching
- ✅ Experience with message queues
- ✅ API design principles
- ✅ Scalability thinking

### Problem-Solving
- ✅ Breaking down complex problems
- ✅ Identifying key components
- ✅ Considering trade-offs
- ✅ Thinking about failures
- ✅ Proposing optimizations

### Communication
- ✅ Explaining clearly
- ✅ Drawing diagrams
- ✅ Discussing decisions
- ✅ Asking clarifying questions
- ✅ Listening to feedback

### Domain Knowledge
- ✅ Understanding insurance concepts
- ✅ Knowing compliance requirements
- ✅ Understanding business needs
- ✅ Thinking about user experience
- ✅ Considering operational aspects

---

## 📚 How to Prepare

### Step 1: Read & Understand (2-3 hours)
- Read through all three examples
- Understand each component
- Note the trade-offs

### Step 2: Practice Explaining (2-3 hours)
- Explain each system out loud
- Draw the architecture
- Discuss the decisions

### Step 3: Adapt & Customize (2-3 hours)
- Think about variations
- Consider different scales
- Think about different requirements

### Step 4: Mock Interview (1-2 hours)
- Practice with a friend
- Record yourself
- Get feedback

### Step 5: Review & Refine (1-2 hours)
- Review weak areas
- Practice follow-up questions
- Refine your explanations

---

## 🚀 Quick Reference

### When to Use Each Example

**Use Policy Management Example When Asked About:**
- Designing a data-heavy system
- Handling millions of records
- Search and indexing
- Caching strategies
- Database optimization

**Use Claims Processing Example When Asked About:**
- Workflow management
- Multi-stage processes
- Document management
- Approval workflows
- State machines

**Use Notifications Example When Asked About:**
- High-throughput systems
- Event-driven architecture
- Reliability and retries
- Multi-channel delivery
- Scheduling

---

## 💪 Confidence Builders

After studying these examples, you should be able to:

✅ Design a system for 100M+ records
✅ Implement complex workflows
✅ Handle 10K+ RPS
✅ Design for high availability
✅ Implement proper caching
✅ Use message queues effectively
✅ Design scalable APIs
✅ Handle failures gracefully
✅ Discuss trade-offs confidently
✅ Explain architectural decisions

---

## 📞 Final Tips

1. **Don't Memorize**: Understand the concepts, not the code
2. **Ask Questions**: Always clarify requirements first
3. **Draw Diagrams**: Visual communication is important
4. **Discuss Trade-offs**: Show you understand different approaches
5. **Think Out Loud**: Explain your reasoning
6. **Be Flexible**: Be ready to adapt your design
7. **Consider Failures**: Always think about what can go wrong
8. **Optimize Iteratively**: Start simple, then optimize

---

## 🎓 Good Luck!

You now have:
- ✅ 3 complete system design examples
- ✅ Detailed code implementations
- ✅ Database schemas
- ✅ API designs
- ✅ Scalability analysis
- ✅ Trade-off discussions
- ✅ Failure handling strategies
- ✅ Monitoring approaches

**You're ready for your Chubb system design interview!** 🚀

---

**File Location**: `C:\Users\Sreeenivasulu_Ummadi\Downloads\chubb\System_Design_Questions_and_Answers.md`

**Size**: 54 KB | **Lines**: 1,840 | **Examples**: 3 complete systems with code

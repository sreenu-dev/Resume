# System Design Deep Dive - Wells Fargo Interview

## 📋 Overview

This guide provides an in-depth look at system design for Wells Fargo interviews, with focus on financial systems, scalability, and reliability.

---

## 🏗️ System Design Fundamentals

### Key Concepts

#### 1. Scalability
**Vertical Scaling:** Add more resources to a single server
- Pros: Simple, no code changes
- Cons: Limited by hardware, single point of failure

**Horizontal Scaling:** Add more servers
- Pros: Unlimited scalability, fault tolerance
- Cons: More complex, requires load balancing

#### 2. Availability
**Uptime:** Percentage of time system is operational
- 99% = 3.65 days downtime per year
- 99.9% = 8.76 hours downtime per year
- 99.99% = 52 minutes downtime per year

**Redundancy:** Multiple copies of critical components
- Database replication
- Server redundancy
- Network redundancy

#### 3. Consistency
**Strong Consistency:** All nodes see the same data at the same time
- Used for: Financial transactions, critical data
- Trade-off: Higher latency

**Eventual Consistency:** Nodes eventually see the same data
- Used for: Social media, caching
- Trade-off: Temporary inconsistency

#### 4. Partition Tolerance
**Network Partition:** Communication failure between nodes
- Must handle gracefully
- Choose between consistency and availability (CAP theorem)

### CAP Theorem
In a distributed system, you can have at most 2 of 3:
- **Consistency:** All nodes have same data
- **Availability:** System is always available
- **Partition Tolerance:** System works despite network failures

**For Financial Systems:** Choose Consistency + Partition Tolerance (sacrifice Availability)

---

## 💰 Financial System Design

### Payment Processing System

#### Requirements
```
Functional:
- Process payments
- Maintain ledger
- Handle refunds
- Generate reports

Non-Functional:
- 10,000 transactions per second
- 99.99% availability
- Sub-second latency
- Strong consistency
- Audit trail
```

#### Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Client Layer                         │
│  (Web, Mobile, API Clients)                            │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  API Gateway                            │
│  (Rate Limiting, Authentication, Routing)              │
└────────────────────┬────────────────────────────────────┘
                     │
     ┌───────────────┼───────────────┐
     │               │               │
┌────▼────┐   ┌─────▼──────┐   ┌────▼────┐
│ Payment  │   │  Ledger    │   │ Account │
│ Service  │   │  Service   │   │ Service │
└────┬────┘   └─────┬──────┘   └────┬────┘
     │              │              │
     └──────────────┼──────────────┘
                    │
     ┌──────────────┼──────────────┐
     │              │              │
┌────▼────┐   ┌────▼────┐   ┌─────▼──────┐
│PostgreSQL│   │  Redis  │   │   Kafka    │
│(Primary) │   │ (Cache) │   │(Event Log) │
└──────────┘   └─────────┘   └────────────┘
```

#### Key Components

**1. API Gateway**
```
Responsibilities:
- Request routing
- Rate limiting (1000 req/sec per user)
- Authentication (OAuth2/JWT)
- Request validation
- Response formatting

Technology:
- Kong, AWS API Gateway, or custom solution
```

**2. Payment Service**
```
Responsibilities:
- Validate payment details
- Check fraud
- Process payment
- Update account balance
- Publish events

Technology:
- .NET Core microservice
- Stateless for horizontal scaling
```

**3. Ledger Service**
```
Responsibilities:
- Record all transactions
- Maintain immutable log
- Ensure consistency
- Generate audit trail

Technology:
- Event sourcing pattern
- Kafka for event log
- PostgreSQL for state
```

**4. Account Service**
```
Responsibilities:
- Manage account information
- Track balances
- Handle account operations

Technology:
- .NET Core microservice
- PostgreSQL database
```

**5. Database Layer**
```
Primary Database: PostgreSQL
- ACID compliance
- Strong consistency
- Replication for HA

Caching Layer: Redis
- Cache hot data (account balances)
- Cache invalidation strategy
- Distributed locking

Event Log: Kafka
- Immutable transaction log
- Event replay capability
- Event streaming
```

#### Scalability Strategy

**Horizontal Scaling:**
```
1. Stateless Services
   - Payment Service: Scale to 100+ instances
   - Account Service: Scale to 50+ instances
   - Ledger Service: Scale to 50+ instances

2. Load Balancing
   - Round-robin or least connections
   - Health checks
   - Auto-scaling based on CPU/memory

3. Database Scaling
   - Read replicas for read-heavy operations
   - Write master for consistency
   - Sharding by account ID for extreme scale
```

**Caching Strategy:**
```
1. Cache Layers
   - API Gateway: Cache read-only responses
   - Application: Cache account balances
   - Database: Query result caching

2. Cache Invalidation
   - TTL-based (5-10 minutes)
   - Event-based (when data changes)
   - Manual invalidation for critical data
```

#### Reliability Strategy

**Fault Tolerance:**
```
1. Service Redundancy
   - Multiple instances of each service
   - Auto-restart on failure
   - Health checks

2. Database Redundancy
   - Master-slave replication
   - Automatic failover
   - Backup and recovery

3. Circuit Breaker Pattern
   - Detect failures
   - Fail fast
   - Graceful degradation
```

**Monitoring & Alerting:**
```
1. Metrics
   - Request latency (p50, p95, p99)
   - Error rate
   - Throughput
   - Database connections

2. Logging
   - Structured logging
   - Centralized log aggregation
   - Audit trail

3. Alerting
   - Alert on latency > 1 second
   - Alert on error rate > 0.1%
   - Alert on service unavailability
```

#### Security Strategy

**Authentication & Authorization:**
```
1. API Authentication
   - OAuth2 for user authentication
   - JWT for service-to-service
   - API keys for external clients

2. Authorization
   - Role-based access control (RBAC)
   - Scope-based permissions
   - Resource-level authorization
```

**Data Security:**
```
1. Encryption
   - TLS/SSL for data in transit
   - AES-256 for data at rest
   - Field-level encryption for sensitive data

2. Access Control
   - Principle of least privilege
   - Audit logging for all access
   - Data masking for sensitive fields
```

**Compliance:**
```
1. Regulatory Requirements
   - PCI-DSS for payment data
   - SOX for financial reporting
   - GDPR for data privacy

2. Audit Trail
   - Immutable transaction log
   - All changes tracked
   - Compliance reports
```

---

## 🔄 Distributed Ledger System

### Requirements
```
Functional:
- Record transactions immutably
- Maintain consistency across nodes
- Support high throughput
- Enable transaction replay

Non-Functional:
- 20,000 transactions per second
- 99.99% availability
- Sub-second latency
- Strong consistency
- Fault tolerance
```

### Architecture

```
┌──────────────────────────────────────────────┐
│           Transaction Submission              │
│  (Clients submit transactions)                │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│        Consensus Layer (Raft/PBFT)           │
│  (Ensure all nodes agree on order)           │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│      Transaction Execution Layer              │
│  (Execute transactions, update state)        │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│        Storage Layer                         │
│  (Immutable log + State database)            │
└──────────────────────────────────────────────┘
```

### Key Components

**1. Consensus Layer**
```
Algorithm: Raft or PBFT
- Ensures all nodes agree on transaction order
- Handles node failures
- Elects leader for coordination

Raft Advantages:
- Simpler than PBFT
- Easier to understand and implement
- Good for permissioned networks
```

**2. Transaction Execution**
```
Responsibilities:
- Validate transactions
- Execute state transitions
- Update account balances
- Maintain consistency

Technology:
- Deterministic execution
- Same result on all nodes
- Snapshot for fast recovery
```

**3. Storage Layer**
```
Immutable Log:
- Append-only transaction log
- Indexed by transaction ID
- Enables replay

State Database:
- Current state of accounts
- Indexed for fast lookup
- Replicated across nodes
```

### Scalability Strategy

**Sharding:**
```
Shard by Account ID:
- Shard 0: Accounts 0-999999
- Shard 1: Accounts 1000000-1999999
- etc.

Benefits:
- Horizontal scaling
- Parallel processing
- Reduced latency

Challenges:
- Cross-shard transactions
- Rebalancing
- Consistency
```

**Batching:**
```
Process transactions in batches:
- Collect 1000 transactions
- Execute as a block
- Commit to ledger

Benefits:
- Higher throughput
- Better resource utilization
- Reduced latency variance
```

---

## 🔐 Secure API Gateway

### Requirements
```
Functional:
- Route requests to services
- Authenticate users
- Authorize access
- Rate limit requests
- Log all requests

Non-Functional:
- 100,000 requests per second
- Sub-100ms latency
- 99.99% availability
- Strong security
```

### Architecture

```
┌─────────────────────────────────────┐
│         Client Requests              │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      TLS Termination                 │
│  (Decrypt HTTPS)                    │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Rate Limiting                   │
│  (Token bucket algorithm)            │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Authentication                  │
│  (Verify JWT/OAuth2)                │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Authorization                   │
│  (Check permissions)                 │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Request Routing                 │
│  (Route to appropriate service)      │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Response Caching                │
│  (Cache read-only responses)         │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Response to Client              │
└─────────────────────────────────────┘
```

### Implementation Details

**Rate Limiting:**
```
Token Bucket Algorithm:
- Each user gets N tokens per second
- Each request costs 1 token
- Requests rejected if no tokens

Configuration:
- 1000 requests per second per user
- Burst capacity: 5000 tokens
- Refill rate: 1000 tokens/sec
```

**Authentication:**
```
JWT (JSON Web Token):
- Issued by auth service
- Contains user ID and permissions
- Signed with private key
- Verified at API gateway

Flow:
1. User logs in
2. Auth service issues JWT
3. Client includes JWT in requests
4. API gateway verifies JWT
5. Request routed to service
```

**Authorization:**
```
Role-Based Access Control (RBAC):
- User has roles (admin, user, etc.)
- Roles have permissions
- Permissions control access to resources

Example:
- Admin role: Can access all endpoints
- User role: Can only access own data
- Guest role: Can only access public data
```

---

## 📊 System Design Interview Approach

### Step 1: Clarify Requirements (5 minutes)
```
Ask:
- How many users?
- How many requests per second?
- Latency requirements?
- Availability requirements?
- Consistency requirements?
- Data size?
- Geographic distribution?
```

### Step 2: Estimate Scale (5 minutes)
```
Calculate:
- Daily active users
- Requests per second
- Data per request
- Total storage needed
- Bandwidth needed
```

### Step 3: High-Level Design (10-15 minutes)
```
Design:
- Client layer
- API gateway
- Service layer
- Data layer
- External services
```

### Step 4: Deep Dive (20-30 minutes)
```
Discuss:
- Database design
- Caching strategy
- Scalability approach
- Reliability measures
- Security measures
```

### Step 5: Trade-offs (5-10 minutes)
```
Discuss:
- Consistency vs Availability
- Latency vs Throughput
- Cost vs Performance
- Complexity vs Simplicity
```

---

## 🎯 Practice Problems

### Problem 1: Payment Processing System
```
Design a payment processing system that:
- Processes 10,000 transactions per second
- Maintains 99.99% availability
- Ensures strong consistency
- Provides audit trail
```

### Problem 2: Distributed Ledger
```
Design a distributed ledger that:
- Records 20,000 transactions per second
- Maintains consistency across nodes
- Survives node failures
- Enables transaction replay
```

### Problem 3: Secure API Gateway
```
Design an API gateway that:
- Handles 100,000 requests per second
- Authenticates and authorizes requests
- Rate limits users
- Logs all requests
```

### Problem 4: High-Frequency Trading Platform
```
Design a trading platform that:
- Matches orders in milliseconds
- Handles 100,000 orders per second
- Maintains consistency
- Provides real-time updates
```

---

## 🚀 Next Steps

1. **Study the fundamentals**
   - Understand CAP theorem
   - Learn about scalability patterns
   - Study reliability patterns

2. **Practice system design**
   - Design 5-10 systems
   - Discuss trade-offs
   - Get feedback

3. **Deep dive into specific areas**
   - Database design
   - Caching strategies
   - Distributed systems

4. **Do mock interviews**
   - Practice explaining designs
   - Handle follow-up questions
   - Get feedback

---

**Created:** 2026-06-24
**Version:** 1.0
**Status:** Ready to Use

**Next Step:** Read 05_CODING_PROBLEMS_GUIDE.md

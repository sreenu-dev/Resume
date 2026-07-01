# System Design Interview Guide

## Overview

System design interviews evaluate your ability to design large-scale distributed systems. For Experian (senior+ roles), expect questions around:
- Scalability
- Reliability
- Data consistency
- Performance
- Security (especially important for financial data!)

## Interview Structure (45-60 minutes)

1. **Requirements Clarification** (5-10 min)
2. **High-Level Design** (10-15 min)
3. **Deep Dive** (15-20 min)
4. **Discussion & Trade-offs** (10-15 min)

## Step 1: Requirements Clarification

### Functional Requirements
**Ask questions like:**
- What are the core features?
- Who are the users?
- What are the use cases?
- What is the expected scale?
- Read-heavy or write-heavy?

### Non-Functional Requirements
**Consider:**
- **Scalability**: How many users? Requests per second?
- **Availability**: 99.9%? 99.99%? 99.999%?
- **Latency**: Real-time? Sub-second? Few seconds OK?
- **Consistency**: Strong or eventual?
- **Durability**: Can we lose data?
- **Security**: Authentication, authorization, encryption?

### Back-of-the-Envelope Calculations

**Example for a URL Shortener:**
```
Assumptions:
- 100 million URLs created per month
- Read:Write ratio = 100:1

Writes per second:
100M / (30 days * 24 hrs * 3600 sec) ≈ 40 writes/sec

Reads per second:
40 * 100 = 4,000 reads/sec

Storage (5 years):
100M * 12 months * 5 years = 6 billion URLs
Average URL size: 500 bytes
Total: 6B * 500 bytes = 3 TB

Bandwidth:
Write: 40 * 500 bytes = 20 KB/s
Read: 4000 * 500 bytes = 2 MB/s
```

## Step 2: High-Level Design

### Components to Consider

#### 1. Client
- Web browser
- Mobile app
- API clients

#### 2. Load Balancer
- Distributes traffic across servers
- Health checks
- SSL termination
- Examples: AWS ELB, NGINX, HAProxy

#### 3. Application Servers
- Business logic
- Stateless (for horizontal scaling)
- Multiple instances

#### 4. Databases
**SQL (Relational)**
- Strong consistency
- ACID transactions
- Complex queries
- Examples: PostgreSQL, MySQL

**NoSQL**
- Horizontal scalability
- Flexible schema
- Eventually consistent
- Types:
  - Key-Value: Redis, DynamoDB
  - Document: MongoDB, Couchbase
  - Column-Family: Cassandra, HBase
  - Graph: Neo4j

#### 5. Cache
- Reduces database load
- Improves latency
- Examples: Redis, Memcached
- Strategies: Cache-aside, Write-through, Write-behind

#### 6. Message Queue
- Asynchronous processing
- Decouples components
- Examples: Kafka, RabbitMQ, AWS SQS

#### 7. CDN
- Static content delivery
- Reduces latency
- Geographic distribution
- Examples: CloudFlare, AWS CloudFront

#### 8. Object Storage
- File storage (images, videos, documents)
- Examples: AWS S3, Google Cloud Storage

### Basic Architecture Pattern

```
                       Internet
                          |
                    [Load Balancer]
                          |
        +----------------+----------------+
        |                |                |
   [App Server 1]  [App Server 2]  [App Server 3]
        |                |                |
        +----------------+----------------+
                          |
                    [Cache Layer]
                          |
                    [Database(s)]
```

## Step 3: Core Components Deep Dive

### 1. Load Balancing

**Algorithms:**
- Round Robin
- Least Connections
- Weighted Round Robin
- IP Hash
- Least Response Time

**Types:**
- Layer 4 (Transport): TCP/UDP level
- Layer 7 (Application): HTTP level

### 2. Caching

**Cache Strategies:**

**Cache-Aside (Lazy Loading)**
```
1. App checks cache
2. If miss, read from DB
3. Write to cache
4. Return data
```

**Write-Through**
```
1. Write to cache
2. Cache writes to DB
3. Return success
```

**Write-Behind (Write-Back)**
```
1. Write to cache
2. Return success
3. Cache writes to DB asynchronously
```

**Cache Eviction Policies:**
- LRU (Least Recently Used)
- LFU (Least Frequently Used)
- FIFO (First In First Out)
- Random Replacement

### 3. Database Scaling

**Vertical Scaling (Scale Up)**
- Add more CPU, RAM, disk
- Easier but limited
- Single point of failure

**Horizontal Scaling (Scale Out)**
- Add more database servers
- More complex
- Better fault tolerance

**Sharding (Horizontal Partitioning)**

Distribute data across multiple databases:

**By Hash:**
```
shard = hash(user_id) % num_shards
```

**By Range:**
```
User ID 1-1M: Shard 1
User ID 1M-2M: Shard 2
etc.
```

**By Geography:**
```
US users: Shard 1
EU users: Shard 2
Asia users: Shard 3
```

**Challenges:**
- Joins across shards
- Rebalancing shards
- Hotspots

**Replication**

**Master-Slave:**
- Master: Writes
- Slaves: Reads
- Asynchronous replication

**Master-Master:**
- Both accept writes
- Conflict resolution needed

### 4. Consistency Models

**Strong Consistency**
- All reads see latest write
- Lower availability
- Use: Financial transactions

**Eventual Consistency**
- Reads may see stale data temporarily
- Higher availability
- Use: Social media posts, comments

**CAP Theorem**
- Consistency
- Availability
- Partition Tolerance

**You can only choose 2!**

### 5. Rate Limiting

**Algorithms:**

**Fixed Window**
```
Allow N requests per minute
Reset counter every minute
```

**Sliding Window**
```
Track requests with timestamps
Allow N in any rolling minute
```

**Token Bucket**
```
Bucket holds N tokens
Each request consumes 1 token
Refill at rate R per second
```

**Leaky Bucket**
```
Requests enter bucket
Process at fixed rate
Excess requests dropped
```

### 6. API Design

**RESTful Best Practices:**
```
GET    /api/v1/users          # List users
GET    /api/v1/users/123      # Get user
POST   /api/v1/users          # Create user
PUT    /api/v1/users/123      # Update user
DELETE /api/v1/users/123      # Delete user
```

**Versioning:**
- URL: `/api/v1/users`
- Header: `Accept: application/vnd.api+json; version=1`

**Pagination:**
```
GET /api/v1/users?page=2&limit=50
```

**Filtering:**
```
GET /api/v1/users?role=admin&status=active
```

## Common System Design Questions

### 1. Design URL Shortener (TinyURL)

**Requirements:**
- Shorten long URLs
- Redirect to original URL
- Analytics (optional)

**Key Components:**
- URL shortening algorithm (Base62 encoding)
- Database (URL mapping)
- Cache (popular URLs)
- Load balancer
- Rate limiting

**Design:**
```
User
  ↓
Load Balancer
  ↓
App Servers → Cache (Redis)
  ↓
Database (URL mappings)
```

**Short URL Generation:**
```python
# Use Base62 encoding (a-z, A-Z, 0-9)
# 7 characters = 62^7 ≈ 3.5 trillion URLs

def generate_short_url(id):
    chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
    short = ""
    while id > 0:
        short = chars[id % 62] + short
        id //= 62
    return short.zfill(7)
```

### 2. Design Web Crawler

**Requirements:**
- Crawl billions of web pages
- Respect robots.txt
- Avoid duplicate URLs
- Distributed system

**Key Components:**
- URL Frontier (queue)
- Fetcher (download pages)
- Parser (extract links)
- Duplicate detector (Bloom filter)
- Storage (distributed file system)

**Design:**
```
URL Frontier (Kafka)
  ↓
Fetcher Workers
  ↓
Parser Workers → URL Frontier (new URLs)
  ↓
Storage (S3/HDFS)
```

**Considerations:**
- Politeness: Don't overwhelm servers
- Robots.txt compliance
- URL deduplication
- Priority queue (important pages first)

### 3. Design Twitter/Social Media Feed

**Requirements:**
- Post tweets
- Follow/unfollow users
- View timeline (home feed)
- Scale to millions of users

**Key Components:**
- User service
- Tweet service
- Timeline service
- Graph database (followers)
- Cache (user timelines)

**Design (Fan-out on Write):**
```
User posts tweet
  ↓
Write to Tweet DB
  ↓
Get followers from Graph DB
  ↓
Push tweet to followers' timelines (in cache)
  ↓
Asynchronous workers
```

**Design (Fan-out on Read):**
```
User requests timeline
  ↓
Get followed users
  ↓
Fetch recent tweets from each
  ↓
Merge and sort
  ↓
Return to user
```

**Hybrid Approach:**
- Fan-out on write for normal users
- Fan-out on read for celebrities

### 4. Design Notification System

**Requirements:**
- Push notifications
- Email notifications
- SMS notifications
- Scalable to millions

**Key Components:**
- API servers
- Message queue
- Workers (for each channel)
- Rate limiter
- Notification templates

**Design:**
```
API Server
  ↓
Message Queue (Kafka)
  ↓         ↓         ↓
Push    Email     SMS
Workers Workers  Workers
  ↓         ↓         ↓
FCM/APNs  SMTP   Twilio
```

### 5. Design Rate Limiter

**Requirements:**
- Limit requests per user/IP
- Multiple strategies
- Distributed system

**Design:**
```
User Request
  ↓
Load Balancer
  ↓
Rate Limiter (Redis)
  ↓
API Servers
```

**Redis Implementation:**
```python
# Token bucket algorithm
key = f"rate_limit:{user_id}"

# Get current tokens
current = redis.get(key)
if current is None:
    tokens = MAX_TOKENS
else:
    # Calculate tokens to add
    last_refill = redis.get(f"{key}:time")
    now = time.time()
    elapsed = now - last_refill
    tokens_to_add = elapsed * REFILL_RATE
    tokens = min(MAX_TOKENS, current + tokens_to_add)

# Check if request allowed
if tokens >= 1:
    redis.set(key, tokens - 1)
    redis.set(f"{key}:time", now)
    return True  # Allow request
else:
    return False  # Reject request
```

### 6. Design Autocomplete/Typeahead

**Requirements:**
- Suggest completions as user types
- Fast (<100ms)
- Handle misspellings

**Key Components:**
- Trie data structure
- Cache (popular queries)
- Analytics service (track queries)

**Design:**
```
User types "face"
  ↓
Check cache (Redis)
  ↓ (miss)
Query Trie service
  ↓
Return top 10 suggestions
  ↓
Update cache
```

**Trie Structure:**
```
        root
        /
       f
      /
     a
    /
   c
  /
 e
  ↓
["facebook", "facetime", "face mask"]
```

## Security Considerations (Important for Experian!)

### Authentication & Authorization
- OAuth 2.0, JWT
- Multi-factor authentication
- Role-based access control (RBAC)

### Data Encryption
- **At Rest**: AES-256
- **In Transit**: TLS 1.3
- **End-to-End**: For sensitive data

### Data Privacy
- PII protection
- GDPR compliance
- Data retention policies
- Audit logs

### API Security
- Rate limiting
- Input validation
- SQL injection prevention
- XSS prevention
- CSRF tokens

## Monitoring & Observability

### Key Metrics
- **Latency**: p50, p95, p99
- **Throughput**: Requests per second
- **Error Rate**: 4xx, 5xx errors
- **Saturation**: CPU, memory, disk usage

### Logging
- Centralized logging (ELK stack)
- Structured logging (JSON)
- Log levels (DEBUG, INFO, WARN, ERROR)

### Monitoring Tools
- Prometheus + Grafana
- Datadog
- New Relic
- AWS CloudWatch

### Alerting
- On-call rotation
- Incident response
- Postmortems

## Trade-offs to Discuss

### 1. SQL vs NoSQL
**SQL:**
- ✅ ACID transactions
- ✅ Complex queries
- ❌ Horizontal scaling

**NoSQL:**
- ✅ Horizontal scaling
- ✅ Flexible schema
- ❌ Eventual consistency

### 2. Synchronous vs Asynchronous
**Sync:**
- ✅ Immediate feedback
- ❌ Slower response

**Async:**
- ✅ Faster response
- ❌ Complex error handling

### 3. Caching
- ✅ Faster reads
- ❌ Stale data
- ❌ Cache invalidation complexity

### 4. Microservices vs Monolith
**Microservices:**
- ✅ Independent scaling
- ✅ Technology flexibility
- ❌ Operational complexity

**Monolith:**
- ✅ Simpler deployment
- ✅ Easier debugging
- ❌ Harder to scale

## Interview Tips

### Do's
✅ Ask clarifying questions
✅ Start with high-level design
✅ Explain your reasoning
✅ Discuss trade-offs
✅ Consider scalability
✅ Mention monitoring
✅ Think about failure scenarios
✅ Draw diagrams
✅ Consider security (especially for Experian!)

### Don'ts
❌ Jump to details too quickly
❌ Assume requirements
❌ Ignore interviewer hints
❌ Forget about scale
❌ Overlook edge cases
❌ Ignore failures
❌ Get stuck on one component
❌ Forget about data security

## Preparation Resources

### Books
- "Designing Data-Intensive Applications" by Martin Kleppmann
- "System Design Interview" by Alex Xu (Volume 1 & 2)

### Websites
- ByteByteGo
- High Scalability Blog
- System Design Primer (GitHub)

### Practice
- Mock interviews with peers
- Draw diagrams for systems you use daily
- Read engineering blogs (Netflix, Uber, Airbnb)

## Key Takeaways

1. **Always clarify requirements first**
2. **Start with high-level design**
3. **Deep dive into 2-3 components**
4. **Discuss trade-offs explicitly**
5. **Consider scale, security, and failures**
6. **Communicate clearly and draw diagrams**
7. **For Experian: emphasize data security and privacy**

Good luck! 🚀

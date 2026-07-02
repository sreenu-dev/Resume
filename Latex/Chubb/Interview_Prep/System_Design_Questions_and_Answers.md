# System Design Questions and Answers - Chubb Interview

## Question 1: Design a Policy Management System

### Problem Statement
Design a system to manage insurance policies for a large insurance company like Chubb. The system should:
- Handle millions of policies
- Support real-time policy updates
- Ensure data consistency
- Provide fast search capabilities
- Track policy lifecycle (creation, renewal, cancellation)
- Support multiple policy types (auto, home, life, etc.)
- Ensure high availability and fault tolerance

### Clarifying Questions (What You Should Ask)

1. **Scale**: How many policies? How many concurrent users? Requests per second?
   - **Answer**: 100 million policies, 10,000 concurrent users, 10,000 RPS

2. **Consistency**: Do we need strong consistency or eventual consistency?
   - **Answer**: Strong consistency for policy data, eventual for analytics

3. **Latency**: What are the latency requirements?
   - **Answer**: 200ms for reads, 500ms for writes

4. **Geographic Distribution**: Single region or multi-region?
   - **Answer**: Multi-region (US, EU, APAC)

5. **Data Retention**: How long to keep policy data?
   - **Answer**: 7 years for compliance

6. **Search Requirements**: What fields need to be searchable?
   - **Answer**: Policy number, customer ID, policy type, status, dates

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Clients (Web/Mobile)                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                    ┌────▼────┐
                    │   CDN    │
                    └────┬────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
    ┌───▼──┐         ┌───▼──┐        ┌───▼──┐
    │ LB-1 │         │ LB-2 │        │ LB-3 │
    └───┬──┘         └───┬──┘        └───┬──┘
        │                │                │
    ┌───▼────────────────▼────────────────▼───┐
    │        API Gateway (Rate Limiting)       │
    └───┬────────────────┬────────────────┬───┘
        │                │                │
    ┌───▼──┐         ┌───▼──┐        ┌───▼──┐
    │ API-1│         │ API-2 │       │ API-3 │
    └───┬──┘         └───┬──┘        └───┬──┘
        │                │                │
        └────────────────┼────────────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
    ┌───▼──┐         ┌───▼──┐        ┌───▼──┐
    │Redis │         │Kafka │        │Search│
    │Cache │         │Queue │        │Engine│
    └──────┘         └──────┘        └──────┘
        │                │                │
        └────────────────┼────────────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
    ┌───▼──────────┐ ┌───▼──────────┐ ┌──▼────────┐
    │   Primary    │ │   Replica    │ │  Replica  │
    │   Database   │ │   Database   │ │  Database │
    │ (PostgreSQL) │ │ (PostgreSQL) │ │(PostgreSQL)
    └──────────────┘ └──────────────┘ └───────────┘
```

---

## Detailed Component Design

### 1. API Gateway & Load Balancing

**Responsibilities:**
- Route requests to appropriate API servers
- Rate limiting (1000 requests/user/minute)
- Authentication and authorization
- Request validation
- Response caching

**Implementation:**
```
GET /api/v1/policies/{policyId}
POST /api/v1/policies
PUT /api/v1/policies/{policyId}
DELETE /api/v1/policies/{policyId}
GET /api/v1/policies/search?query=...
GET /api/v1/policies/{policyId}/history
```

**Rate Limiting Strategy:**
- Token bucket algorithm
- 1000 requests per minute per user
- 100,000 requests per minute per API key
- Burst capacity: 2x normal rate

---

### 2. Database Design

#### Schema Design

```sql
-- Policies Table
CREATE TABLE policies (
    policy_id UUID PRIMARY KEY,
    customer_id UUID NOT NULL,
    policy_type VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE,
    premium_amount DECIMAL(10, 2),
    coverage_amount DECIMAL(12, 2),
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    version INT NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    INDEX idx_customer_id (customer_id),
    INDEX idx_status (status),
    INDEX idx_policy_type (policy_type),
    INDEX idx_created_at (created_at)
);

-- Policy Details Table (for different policy types)
CREATE TABLE policy_details (
    detail_id UUID PRIMARY KEY,
    policy_id UUID NOT NULL,
    detail_key VARCHAR(100) NOT NULL,
    detail_value TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL,
    FOREIGN KEY (policy_id) REFERENCES policies(policy_id),
    INDEX idx_policy_id (policy_id)
);

-- Policy History Table (for audit trail)
CREATE TABLE policy_history (
    history_id UUID PRIMARY KEY,
    policy_id UUID NOT NULL,
    action VARCHAR(50) NOT NULL,
    old_values JSONB,
    new_values JSONB,
    changed_by UUID NOT NULL,
    changed_at TIMESTAMP NOT NULL,
    FOREIGN KEY (policy_id) REFERENCES policies(policy_id),
    INDEX idx_policy_id (policy_id),
    INDEX idx_changed_at (changed_at)
);

-- Customers Table
CREATE TABLE customers (
    customer_id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    address TEXT,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    INDEX idx_email (email)
);
```

#### Indexing Strategy

```sql
-- Composite indexes for common queries
CREATE INDEX idx_customer_status ON policies(customer_id, status);
CREATE INDEX idx_type_status ON policies(policy_type, status);
CREATE INDEX idx_date_range ON policies(start_date, end_date);

-- Partial indexes for active policies
CREATE INDEX idx_active_policies ON policies(customer_id) 
WHERE status IN ('ACTIVE', 'PENDING');

-- BRIN index for time-series data
CREATE INDEX idx_created_at_brin ON policies USING BRIN (created_at);
```

#### Sharding Strategy

**Sharding Key**: `customer_id` (hash-based)

```
Shard 0: customer_id % 10 = 0
Shard 1: customer_id % 10 = 1
...
Shard 9: customer_id % 10 = 9
```

**Advantages:**
- Distributes load evenly
- Enables horizontal scaling
- Allows independent backups per shard

**Disadvantages:**
- Cross-shard queries are complex
- Rebalancing is difficult

---

### 3. Caching Strategy

#### Redis Cache Architecture

```
Cache Key Structure:
- policy:{policy_id} → Policy object (TTL: 1 hour)
- customer:{customer_id}:policies → List of policy IDs (TTL: 30 min)
- policy_search:{query_hash} → Search results (TTL: 5 min)
- policy_count:{customer_id} → Count (TTL: 15 min)
```

#### Cache Implementation

```csharp
public class PolicyCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<PolicyCacheService> _logger;
    
    public async Task<Policy> GetPolicyAsync(string policyId)
    {
        // Try cache first
        var cacheKey = $"policy:{policyId}";
        var cachedPolicy = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedPolicy))
        {
            _logger.LogInformation("Cache hit for policy {PolicyId}", policyId);
            return JsonConvert.DeserializeObject<Policy>(cachedPolicy);
        }
        
        // Cache miss - fetch from database
        var policy = await _policyRepository.GetPolicyAsync(policyId);
        
        if (policy != null)
        {
            // Cache for 1 hour
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonConvert.SerializeObject(policy),
                cacheOptions
            );
        }
        
        return policy;
    }
    
    public async Task InvalidatePolicyAsync(string policyId)
    {
        var cacheKey = $"policy:{policyId}";
        await _cache.RemoveAsync(cacheKey);
        
        // Also invalidate customer's policy list
        var policy = await _policyRepository.GetPolicyAsync(policyId);
        if (policy != null)
        {
            await _cache.RemoveAsync($"customer:{policy.CustomerId}:policies");
        }
    }
}
```

#### Cache Invalidation Strategy

**Write-Through Pattern:**
1. Update database
2. Update cache
3. Return response

**Advantages:**
- Cache always consistent with database
- No stale data

**Disadvantages:**
- Slower writes
- Extra cache operations

---

### 4. Search Implementation (Elasticsearch)

#### Index Mapping

```json
{
  "mappings": {
    "properties": {
      "policy_id": {
        "type": "keyword"
      },
      "customer_id": {
        "type": "keyword"
      },
      "policy_type": {
        "type": "keyword"
      },
      "status": {
        "type": "keyword"
      },
      "policy_number": {
        "type": "text",
        "analyzer": "standard"
      },
      "customer_name": {
        "type": "text",
        "analyzer": "standard"
      },
      "start_date": {
        "type": "date"
      },
      "end_date": {
        "type": "date"
      },
      "premium_amount": {
        "type": "double"
      },
      "created_at": {
        "type": "date"
      }
    }
  }
}
```

#### Search Query Example

```csharp
public async Task<List<Policy>> SearchPoliciesAsync(PolicySearchRequest request)
{
    var searchRequest = new SearchRequest<Policy>
    {
        Query = new BoolQuery
        {
            Must = new QueryContainer[]
            {
                new MatchQuery { Field = "customer_id", Query = request.CustomerId }
            },
            Filter = new QueryContainer[]
            {
                new TermQuery { Field = "status", Value = request.Status },
                new DateRangeQuery
                {
                    Field = "start_date",
                    GreaterThanOrEqualTo = request.StartDate,
                    LessThanOrEqualTo = request.EndDate
                }
            }
        },
        From = (request.PageNumber - 1) * request.PageSize,
        Size = request.PageSize,
        Sort = new List<ISort>
        {
            new FieldSort { Field = "created_at", Order = SortOrder.Descending }
        }
    };
    
    var response = await _elasticClient.SearchAsync<Policy>(searchRequest);
    return response.Documents.ToList();
}
```

---

### 5. Message Queue (Kafka)

#### Event-Driven Architecture

```
Policy Events:
- PolicyCreated
- PolicyUpdated
- PolicyRenewed
- PolicyCancelled
- PolicyExpired
```

#### Kafka Topics

```
Topic: policy-events
Partitions: 10 (partitioned by customer_id)
Replication Factor: 3
Retention: 7 days
```

#### Event Publishing

```csharp
public class PolicyEventPublisher
{
    private readonly IProducer<string, string> _producer;
    
    public async Task PublishPolicyCreatedAsync(Policy policy)
    {
        var @event = new PolicyCreatedEvent
        {
            PolicyId = policy.PolicyId,
            CustomerId = policy.CustomerId,
            PolicyType = policy.PolicyType,
            CreatedAt = DateTime.UtcNow
        };
        
        var message = new Message<string, string>
        {
            Key = policy.CustomerId.ToString(),
            Value = JsonConvert.SerializeObject(@event)
        };
        
        await _producer.ProduceAsync("policy-events", message);
    }
}

public class PolicyEventConsumer
{
    public async Task ConsumeEventsAsync()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = "policy-service",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe("policy-events");
            
            while (true)
            {
                var result = consumer.Consume();
                var @event = JsonConvert.DeserializeObject<PolicyCreatedEvent>(result.Message.Value);
                
                // Process event
                await HandlePolicyCreatedAsync(@event);
                
                consumer.Commit(result);
            }
        }
    }
}
```

---

### 6. API Design

#### REST Endpoints

```
GET /api/v1/policies/{policyId}
- Get a specific policy
- Response: 200 OK with Policy object
- Caching: 1 hour

POST /api/v1/policies
- Create a new policy
- Request: CreatePolicyRequest
- Response: 201 Created with Policy object
- Idempotency Key: Required

PUT /api/v1/policies/{policyId}
- Update a policy
- Request: UpdatePolicyRequest
- Response: 200 OK with updated Policy
- Optimistic locking: Version field

DELETE /api/v1/policies/{policyId}
- Soft delete a policy
- Response: 204 No Content

GET /api/v1/policies/search
- Search policies
- Query params: query, status, type, page, pageSize
- Response: 200 OK with paginated results

GET /api/v1/policies/{policyId}/history
- Get policy change history
- Response: 200 OK with list of changes
```

#### Request/Response Examples

```json
// Create Policy Request
POST /api/v1/policies
{
  "customerId": "cust-123",
  "policyType": "AUTO",
  "startDate": "2024-01-01",
  "endDate": "2025-01-01",
  "premiumAmount": 1200.00,
  "coverageAmount": 100000.00,
  "details": {
    "vehicleVIN": "ABC123",
    "driverAge": 35
  }
}

// Create Policy Response
201 Created
{
  "policyId": "pol-456",
  "customerId": "cust-123",
  "policyType": "AUTO",
  "status": "ACTIVE",
  "startDate": "2024-01-01",
  "endDate": "2025-01-01",
  "premiumAmount": 1200.00,
  "coverageAmount": 100000.00,
  "createdAt": "2024-01-01T10:00:00Z",
  "version": 1
}

// Update Policy Request
PUT /api/v1/policies/pol-456
{
  "premiumAmount": 1250.00,
  "version": 1
}

// Search Policies Request
GET /api/v1/policies/search?customerId=cust-123&status=ACTIVE&page=1&pageSize=20

// Search Response
200 OK
{
  "data": [
    {
      "policyId": "pol-456",
      "customerId": "cust-123",
      "policyType": "AUTO",
      "status": "ACTIVE",
      "startDate": "2024-01-01",
      "endDate": "2025-01-01"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 20
}
```

---

### 7. Handling Failures & Consistency

#### Optimistic Locking

```csharp
public async Task<Policy> UpdatePolicyAsync(string policyId, UpdatePolicyRequest request)
{
    var policy = await _policyRepository.GetPolicyAsync(policyId);
    
    if (policy.Version != request.Version)
    {
        throw new ConcurrencyException("Policy has been modified by another user");
    }
    
    policy.PremiumAmount = request.PremiumAmount;
    policy.Version++;
    policy.UpdatedAt = DateTime.UtcNow;
    
    await _policyRepository.UpdatePolicyAsync(policy);
    
    // Invalidate cache
    await _cacheService.InvalidatePolicyAsync(policyId);
    
    // Publish event
    await _eventPublisher.PublishPolicyUpdatedAsync(policy);
    
    return policy;
}
```

#### Idempotency

```csharp
public async Task<Policy> CreatePolicyAsync(CreatePolicyRequest request, string idempotencyKey)
{
    // Check if request was already processed
    var existingPolicy = await _idempotencyStore.GetAsync(idempotencyKey);
    if (existingPolicy != null)
    {
        return existingPolicy;
    }
    
    var policy = new Policy
    {
        PolicyId = Guid.NewGuid().ToString(),
        CustomerId = request.CustomerId,
        PolicyType = request.PolicyType,
        Status = "ACTIVE",
        CreatedAt = DateTime.UtcNow
    };
    
    await _policyRepository.CreatePolicyAsync(policy);
    
    // Store idempotency key
    await _idempotencyStore.SetAsync(idempotencyKey, policy);
    
    // Publish event
    await _eventPublisher.PublishPolicyCreatedAsync(policy);
    
    return policy;
}
```

#### Circuit Breaker Pattern

```csharp
public class PolicyServiceWithCircuitBreaker
{
    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;
    
    public PolicyServiceWithCircuitBreaker()
    {
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync<HttpResponseMessage>(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );
    }
    
    public async Task<Policy> GetPolicyAsync(string policyId)
    {
        try
        {
            var response = await _circuitBreakerPolicy.ExecuteAsync(
                () => _httpClient.GetAsync($"/api/policies/{policyId}")
            );
            
            return await response.Content.ReadAsAsync<Policy>();
        }
        catch (BrokenCircuitException)
        {
            // Return cached data or fallback
            return await _cacheService.GetPolicyAsync(policyId);
        }
    }
}
```

---

### 8. Monitoring & Alerting

#### Key Metrics

```
1. Latency Metrics:
   - p50, p95, p99 latency for each endpoint
   - Cache hit rate
   - Database query time

2. Throughput Metrics:
   - Requests per second
   - Successful requests
   - Failed requests

3. Error Metrics:
   - Error rate
   - 4xx errors
   - 5xx errors

4. Business Metrics:
   - Policies created per day
   - Policies updated per day
   - Search queries per day

5. Infrastructure Metrics:
   - CPU usage
   - Memory usage
   - Disk usage
   - Network I/O
```

#### Monitoring Implementation

```csharp
public class PolicyMetrics
{
    private readonly IMetricsCollector _metrics;
    
    public async Task<Policy> GetPolicyAsync(string policyId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var policy = await _policyRepository.GetPolicyAsync(policyId);
            
            stopwatch.Stop();
            _metrics.RecordLatency("policy.get", stopwatch.ElapsedMilliseconds);
            _metrics.IncrementCounter("policy.get.success");
            
            return policy;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.RecordLatency("policy.get", stopwatch.ElapsedMilliseconds);
            _metrics.IncrementCounter("policy.get.error");
            _logger.LogError(ex, "Error getting policy {PolicyId}", policyId);
            throw;
        }
    }
}
```

---

## Scalability Analysis

### Current Capacity
- **Policies**: 100 million
- **Concurrent Users**: 10,000
- **RPS**: 10,000

### Scaling to 1 Billion Policies

**Database Scaling:**
- Increase shards from 10 to 100
- Use read replicas (3 per shard)
- Implement read-write splitting

**Cache Scaling:**
- Increase Redis cluster size
- Implement cache partitioning
- Use consistent hashing

**API Scaling:**
- Increase API servers from 3 to 30
- Use better load balancing algorithm
- Implement request batching

**Search Scaling:**
- Increase Elasticsearch shards
- Implement index partitioning
- Use dedicated search cluster

---

## Trade-offs & Decisions

| Decision | Choice | Why |
|---|---|---|
| **Consistency** | Strong | Policy data must be accurate |
| **Database** | PostgreSQL | ACID compliance, complex queries |
| **Caching** | Redis | Fast, in-memory, distributed |
| **Search** | Elasticsearch | Full-text search, aggregations |
| **Messaging** | Kafka | High throughput, durability |
| **Sharding Key** | customer_id | Even distribution, locality |
| **Replication** | 3 copies | Balance between durability and cost |

---

## Summary

**Key Components:**
1. Load Balancer → Distribute traffic
2. API Gateway → Rate limiting, auth
3. PostgreSQL → Persistent storage
4. Redis → Caching layer
5. Elasticsearch → Search capability
6. Kafka → Event streaming
7. Monitoring → Observability

**Key Characteristics:**
- ✅ Handles 100M policies
- ✅ 10K concurrent users
- ✅ 10K RPS
- ✅ Strong consistency
- ✅ High availability
- ✅ Fault tolerant
- ✅ Scalable to 1B policies

---

---

# Question 2: Design a Claims Processing System

### Problem Statement
Design a system to process insurance claims for Chubb. The system should:
- Accept claim submissions from customers
- Route claims to appropriate handlers
- Track claim status in real-time
- Support document uploads and management
- Handle approvals and rejections
- Ensure compliance and audit trail
- Support high volume of claims

### Clarifying Questions

1. **Scale**: How many claims per day? Concurrent submissions?
   - **Answer**: 100,000 claims/day, 1000 concurrent submissions

2. **Processing Time**: How long to process a claim?
   - **Answer**: 24-48 hours for simple claims, 7-14 days for complex

3. **Document Size**: Max file size for documents?
   - **Answer**: 100 MB per document, max 10 documents per claim

4. **Workflow**: How many approval stages?
   - **Answer**: 3-5 stages depending on claim type

5. **Notifications**: How to notify customers?
   - **Answer**: Email, SMS, push notifications

---

## High-Level Architecture

```
┌──────────────────────────────────────────────────────┐
│              Customer Portal / Mobile App             │
└────────────────┬─────────────────────────────────────┘
                 │
            ┌────▼────┐
            │   CDN    │
            └────┬────┘
                 │
        ┌────────┼────────┐
        │                 │
    ┌───▼──┐          ┌───▼──┐
    │ LB-1 │          │ LB-2 │
    └───┬──┘          └───┬──┘
        │                 │
    ┌───▼─────────────────▼───┐
    │    API Gateway          │
    │  (Rate Limiting, Auth)  │
    └───┬─────────────────┬───┘
        │                 │
    ┌───▼──┐          ┌───▼──┐
    │Claims│          │Claims│
    │API-1 │          │API-2 │
    └───┬──┘          └───┬──┘
        │                 │
        └────────┬────────┘
                 │
    ┌────────────┼────────────┐
    │            │            │
┌───▼──┐    ┌───▼──┐    ┌───▼──┐
│Redis │    │Kafka │    │S3    │
│Cache │    │Queue │    │Files │
└──────┘    └──────┘    └──────┘
    │            │            │
    └────────────┼────────────┘
                 │
        ┌────────▼────────┐
        │   PostgreSQL    │
        │   (Claims DB)   │
        └─────────────────┘
                 │
        ┌────────▼────────┐
        │ Workflow Engine │
        │ (State Machine) │
        └─────────────────┘
                 │
    ┌────────────┼────────────┐
    │            │            │
┌───▼──┐    ┌───▼──┐    ┌───▼──┐
│Email │    │SMS   │    │Push  │
│Service   │Service   │Service
└──────┘    └──────┘    └──────┘
```

---

## Detailed Design

### 1. Database Schema

```sql
-- Claims Table
CREATE TABLE claims (
    claim_id UUID PRIMARY KEY,
    policy_id UUID NOT NULL,
    customer_id UUID NOT NULL,
    claim_type VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
    amount_claimed DECIMAL(12, 2),
    amount_approved DECIMAL(12, 2),
    description TEXT,
    submitted_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL,
    version INT NOT NULL,
    FOREIGN KEY (policy_id) REFERENCES policies(policy_id),
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    INDEX idx_customer_id (customer_id),
    INDEX idx_policy_id (policy_id),
    INDEX idx_status (status),
    INDEX idx_submitted_at (submitted_at)
);

-- Claim Documents Table
CREATE TABLE claim_documents (
    document_id UUID PRIMARY KEY,
    claim_id UUID NOT NULL,
    document_type VARCHAR(50) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_size BIGINT NOT NULL,
    s3_key VARCHAR(500) NOT NULL,
    uploaded_at TIMESTAMP NOT NULL,
    FOREIGN KEY (claim_id) REFERENCES claims(claim_id),
    INDEX idx_claim_id (claim_id)
);

-- Claim Workflow Table
CREATE TABLE claim_workflow (
    workflow_id UUID PRIMARY KEY,
    claim_id UUID NOT NULL,
    current_stage VARCHAR(50) NOT NULL,
    assigned_to UUID,
    status VARCHAR(20) NOT NULL,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    FOREIGN KEY (claim_id) REFERENCES claims(claim_id),
    INDEX idx_claim_id (claim_id),
    INDEX idx_assigned_to (assigned_to)
);

-- Claim Approvals Table
CREATE TABLE claim_approvals (
    approval_id UUID PRIMARY KEY,
    claim_id UUID NOT NULL,
    stage VARCHAR(50) NOT NULL,
    approved_by UUID NOT NULL,
    decision VARCHAR(20) NOT NULL,
    comments TEXT,
    approved_at TIMESTAMP NOT NULL,
    FOREIGN KEY (claim_id) REFERENCES claims(claim_id),
    INDEX idx_claim_id (claim_id),
    INDEX idx_approved_by (approved_by)
);

-- Claim History Table (Audit Trail)
CREATE TABLE claim_history (
    history_id UUID PRIMARY KEY,
    claim_id UUID NOT NULL,
    action VARCHAR(100) NOT NULL,
    old_values JSONB,
    new_values JSONB,
    changed_by UUID,
    changed_at TIMESTAMP NOT NULL,
    FOREIGN KEY (claim_id) REFERENCES claims(claim_id),
    INDEX idx_claim_id (claim_id),
    INDEX idx_changed_at (changed_at)
);
```

### 2. Claim Workflow State Machine

```csharp
public enum ClaimStatus
{
    SUBMITTED,
    UNDER_REVIEW,
    APPROVED,
    REJECTED,
    PAID,
    CLOSED
}

public class ClaimWorkflow
{
    private readonly Dictionary<ClaimStatus, List<ClaimStatus>> _transitions = 
        new Dictionary<ClaimStatus, List<ClaimStatus>>
        {
            { ClaimStatus.SUBMITTED, new List<ClaimStatus> { ClaimStatus.UNDER_REVIEW } },
            { ClaimStatus.UNDER_REVIEW, new List<ClaimStatus> { ClaimStatus.APPROVED, ClaimStatus.REJECTED } },
            { ClaimStatus.APPROVED, new List<ClaimStatus> { ClaimStatus.PAID } },
            { ClaimStatus.REJECTED, new List<ClaimStatus> { ClaimStatus.CLOSED } },
            { ClaimStatus.PAID, new List<ClaimStatus> { ClaimStatus.CLOSED } }
        };
    
    public bool CanTransition(ClaimStatus from, ClaimStatus to)
    {
        return _transitions.ContainsKey(from) && _transitions[from].Contains(to);
    }
    
    public async Task<Claim> TransitionAsync(Claim claim, ClaimStatus newStatus, string reason)
    {
        if (!CanTransition(claim.Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {claim.Status} to {newStatus}"
            );
        }
        
        claim.Status = newStatus;
        claim.UpdatedAt = DateTime.UtcNow;
        
        await _claimRepository.UpdateAsync(claim);
        await _auditService.LogAsync(claim.ClaimId, "STATUS_CHANGED", claim.Status, reason);
        
        // Publish event for notifications
        await _eventPublisher.PublishClaimStatusChangedAsync(claim);
        
        return claim;
    }
}
```

### 3. Document Management

```csharp
public class DocumentService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IClaimRepository _claimRepository;
    
    public async Task<string> UploadDocumentAsync(
        string claimId, 
        IFormFile file, 
        string documentType)
    {
        // Validate file
        ValidateFile(file);
        
        // Generate S3 key
        var s3Key = $"claims/{claimId}/{Guid.NewGuid()}/{file.FileName}";
        
        // Upload to S3
        var putRequest = new PutObjectRequest
        {
            BucketName = "claims-documents",
            Key = s3Key,
            InputStream = file.OpenReadStream(),
            ContentType = file.ContentType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
        };
        
        await _s3Client.PutObjectAsync(putRequest);
        
        // Save metadata to database
        var document = new ClaimDocument
        {
            DocumentId = Guid.NewGuid().ToString(),
            ClaimId = claimId,
            DocumentType = documentType,
            FileName = file.FileName,
            FileSize = file.Length,
            S3Key = s3Key,
            UploadedAt = DateTime.UtcNow
        };
        
        await _claimRepository.SaveDocumentAsync(document);
        
        return document.DocumentId;
    }
    
    public async Task<Stream> DownloadDocumentAsync(string documentId)
    {
        var document = await _claimRepository.GetDocumentAsync(documentId);
        
        var getRequest = new GetObjectRequest
        {
            BucketName = "claims-documents",
            Key = document.S3Key
        };
        
        var response = await _s3Client.GetObjectAsync(getRequest);
        return response.ResponseStream;
    }
    
    private void ValidateFile(IFormFile file)
    {
        const long maxFileSize = 100 * 1024 * 1024; // 100 MB
        
        if (file.Length > maxFileSize)
        {
            throw new InvalidOperationException("File size exceeds 100 MB limit");
        }
        
        var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("File type not allowed");
        }
    }
}
```

### 4. Claim Processing Pipeline

```csharp
public class ClaimProcessingService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IWorkflowEngine _workflowEngine;
    private readonly INotificationService _notificationService;
    private readonly IProducer<string, string> _kafkaProducer;
    
    public async Task<Claim> SubmitClaimAsync(SubmitClaimRequest request)
    {
        // Create claim
        var claim = new Claim
        {
            ClaimId = Guid.NewGuid().ToString(),
            PolicyId = request.PolicyId,
            CustomerId = request.CustomerId,
            ClaimType = request.ClaimType,
            Status = ClaimStatus.SUBMITTED,
            AmountClaimed = request.AmountClaimed,
            Description = request.Description,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        // Save to database
        await _claimRepository.CreateAsync(claim);
        
        // Create workflow
        var workflow = new ClaimWorkflow
        {
            WorkflowId = Guid.NewGuid().ToString(),
            ClaimId = claim.ClaimId,
            CurrentStage = "INITIAL_REVIEW",
            Status = "PENDING"
        };
        
        await _claimRepository.CreateWorkflowAsync(workflow);
        
        // Publish event to Kafka
        var @event = new ClaimSubmittedEvent
        {
            ClaimId = claim.ClaimId,
            PolicyId = claim.PolicyId,
            CustomerId = claim.CustomerId,
            AmountClaimed = claim.AmountClaimed,
            SubmittedAt = DateTime.UtcNow
        };
        
        await PublishEventAsync("claim-events", claim.ClaimId, @event);
        
        // Send notification
        await _notificationService.SendClaimSubmittedNotificationAsync(claim);
        
        return claim;
    }
    
    public async Task<Claim> ApproveClaimAsync(
        string claimId, 
        decimal approvedAmount, 
        string approvedBy)
    {
        var claim = await _claimRepository.GetAsync(claimId);
        
        // Create approval record
        var approval = new ClaimApproval
        {
            ApprovalId = Guid.NewGuid().ToString(),
            ClaimId = claimId,
            Stage = "MANAGER_APPROVAL",
            ApprovedBy = approvedBy,
            Decision = "APPROVED",
            ApprovedAt = DateTime.UtcNow
        };
        
        await _claimRepository.SaveApprovalAsync(approval);
        
        // Update claim
        claim.AmountApproved = approvedAmount;
        claim.Status = ClaimStatus.APPROVED;
        claim.UpdatedAt = DateTime.UtcNow;
        
        await _claimRepository.UpdateAsync(claim);
        
        // Publish event
        await PublishEventAsync("claim-events", claimId, 
            new ClaimApprovedEvent { ClaimId = claimId, ApprovedAmount = approvedAmount });
        
        // Send notification
        await _notificationService.SendClaimApprovedNotificationAsync(claim);
        
        return claim;
    }
    
    private async Task PublishEventAsync<T>(string topic, string key, T @event)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonConvert.SerializeObject(@event)
        };
        
        await _kafkaProducer.ProduceAsync(topic, message);
    }
}
```

### 5. Notification Service

```csharp
public class NotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IPushNotificationService _pushService;
    
    public async Task SendClaimSubmittedNotificationAsync(Claim claim)
    {
        var customer = await _customerRepository.GetAsync(claim.CustomerId);
        
        // Send email
        await _emailService.SendAsync(
            customer.Email,
            "Claim Submitted Successfully",
            $"Your claim {claim.ClaimId} has been submitted. Reference: {claim.ClaimId}"
        );
        
        // Send SMS
        if (!string.IsNullOrEmpty(customer.Phone))
        {
            await _smsService.SendAsync(
                customer.Phone,
                $"Claim {claim.ClaimId} submitted. Status: {claim.Status}"
            );
        }
        
        // Send push notification
        await _pushService.SendAsync(
            customer.CustomerId,
            "Claim Submitted",
            $"Your claim has been submitted successfully"
        );
    }
    
    public async Task SendClaimApprovedNotificationAsync(Claim claim)
    {
        var customer = await _customerRepository.GetAsync(claim.CustomerId);
        
        await _emailService.SendAsync(
            customer.Email,
            "Claim Approved",
            $"Your claim {claim.ClaimId} has been approved for ${claim.AmountApproved}"
        );
    }
}
```

### 6. Monitoring & Analytics

```csharp
public class ClaimMetrics
{
    private readonly IMetricsCollector _metrics;
    
    public async Task RecordClaimSubmissionAsync(Claim claim)
    {
        _metrics.IncrementCounter("claims.submitted");
        _metrics.RecordGauge("claims.amount", claim.AmountClaimed);
        _metrics.IncrementCounter($"claims.type.{claim.ClaimType}");
    }
    
    public async Task RecordClaimProcessingTimeAsync(Claim claim)
    {
        var processingTime = (DateTime.UtcNow - claim.SubmittedAt).TotalHours;
        _metrics.RecordHistogram("claims.processing_time_hours", processingTime);
    }
    
    public async Task RecordApprovalRateAsync(string claimType, bool approved)
    {
        var metric = approved ? "claims.approved" : "claims.rejected";
        _metrics.IncrementCounter(metric);
        _metrics.IncrementCounter($"{metric}.{claimType}");
    }
}
```

---

## Key Features

1. **Workflow Management**: State machine for claim progression
2. **Document Management**: Secure S3 storage with metadata
3. **Real-time Updates**: Kafka events for status changes
4. **Multi-channel Notifications**: Email, SMS, push
5. **Audit Trail**: Complete history of all changes
6. **Scalability**: Handles 100K claims/day
7. **High Availability**: Replicated database, load balancing
8. **Security**: Encryption, access control, compliance

---

---

# Question 3: Design a Real-Time Notification System

### Problem Statement
Design a notification system for Chubb that can:
- Send notifications via multiple channels (Email, SMS, Push)
- Handle high throughput (1M notifications/day)
- Ensure reliable delivery
- Support scheduling
- Track delivery status
- Handle retries and failures

### Clarifying Questions

1. **Scale**: How many notifications per day?
   - **Answer**: 1 million notifications/day, 10K/second peak

2. **Channels**: Which channels to support?
   - **Answer**: Email, SMS, Push notifications

3. **Latency**: How quickly should notifications be sent?
   - **Answer**: 95% within 5 minutes, 99% within 30 minutes

4. **Delivery Guarantee**: At least once or exactly once?
   - **Answer**: At least once delivery

5. **Scheduling**: Support scheduled notifications?
   - **Answer**: Yes, schedule up to 30 days in advance

---

## High-Level Architecture

```
┌──────────────────────────────────────┐
│    Notification Request Sources      │
│  (Claims, Policies, Payments, etc.)  │
└────────────────┬─────────────────────┘
                 │
        ┌────────▼────────┐
        │  Notification   │
        │  API Gateway    │
        └────────┬────────┘
                 │
        ┌────────▼────────┐
        │  Kafka Queue    │
        │ (notification-  │
        │   events)       │
        └────────┬────────┘
                 │
    ┌────────────┼────────────┐
    │            │            │
┌───▼──┐    ┌───▼──┐    ┌───▼──┐
│Email │    │SMS   │    │Push  │
│Worker   │Worker   │Worker
└───┬──┘    └───┬──┘    └───┬──┘
    │            │            │
    └────────────┼────────────┘
                 │
        ┌────────▼────────┐
        │   PostgreSQL    │
        │ (Notification   │
        │   History)      │
        └─────────────────┘
```

---

## Detailed Implementation

### 1. Notification Model

```csharp
public class Notification
{
    public string NotificationId { get; set; }
    public string UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Variables { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime ScheduledFor { get; set; }
    public DateTime SentAt { get; set; }
    public int RetryCount { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum NotificationType
{
    CLAIM_SUBMITTED,
    CLAIM_APPROVED,
    CLAIM_REJECTED,
    POLICY_RENEWAL,
    PAYMENT_REMINDER,
    PAYMENT_RECEIVED
}

public enum NotificationChannel
{
    EMAIL,
    SMS,
    PUSH
}

public enum NotificationStatus
{
    PENDING,
    SENT,
    FAILED,
    BOUNCED,
    UNSUBSCRIBED
}
```

### 2. Notification Service

```csharp
public class NotificationService
{
    private readonly IProducer<string, string> _kafkaProducer;
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationService> _logger;
    
    public async Task<string> SendNotificationAsync(SendNotificationRequest request)
    {
        // Validate request
        ValidateRequest(request);
        
        // Create notification record
        var notification = new Notification
        {
            NotificationId = Guid.NewGuid().ToString(),
            UserId = request.UserId,
            Type = request.Type,
            Channel = request.Channel,
            Subject = request.Subject,
            Body = request.Body,
            Variables = request.Variables,
            Status = NotificationStatus.PENDING,
            ScheduledFor = request.ScheduledFor ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        // Save to database
        await _repository.CreateAsync(notification);
        
        // Publish to Kafka
        var @event = new NotificationEvent
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Channel = notification.Channel,
            Subject = notification.Subject,
            Body = notification.Body,
            ScheduledFor = notification.ScheduledFor
        };
        
        var message = new Message<string, string>
        {
            Key = notification.UserId,
            Value = JsonConvert.SerializeObject(@event)
        };
        
        await _kafkaProducer.ProduceAsync("notification-events", message);
        
        _logger.LogInformation("Notification {NotificationId} created", notification.NotificationId);
        
        return notification.NotificationId;
    }
    
    public async Task<Notification> GetNotificationStatusAsync(string notificationId)
    {
        return await _repository.GetAsync(notificationId);
    }
    
    private void ValidateRequest(SendNotificationRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
            throw new ArgumentException("UserId is required");
        
        if (string.IsNullOrEmpty(request.Subject))
            throw new ArgumentException("Subject is required");
        
        if (string.IsNullOrEmpty(request.Body))
            throw new ArgumentException("Body is required");
    }
}
```

### 3. Email Worker

```csharp
public class EmailWorker
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IEmailProvider _emailProvider;
    private readonly INotificationRepository _repository;
    private readonly ILogger<EmailWorker> _logger;
    
    public async Task StartAsync()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = "email-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe("notification-events");
            
            while (true)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
                    
                    if (result == null) continue;
                    
                    var @event = JsonConvert.DeserializeObject<NotificationEvent>(result.Message.Value);
                    
                    if (@event.Channel != NotificationChannel.EMAIL)
                        continue;
                    
                    await ProcessEmailAsync(@event);
                    consumer.Commit(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notification");
                }
            }
        }
    }
    
    private async Task ProcessEmailAsync(NotificationEvent @event)
    {
        var notification = await _repository.GetAsync(@event.NotificationId);
        
        // Check if scheduled for future
        if (notification.ScheduledFor > DateTime.UtcNow)
        {
            // Re-queue for later
            await ReQueueAsync(notification);
            return;
        }
        
        try
        {
            // Get user email
            var user = await _userRepository.GetAsync(notification.UserId);
            
            // Check if unsubscribed
            if (user.IsUnsubscribedFromEmails)
            {
                notification.Status = NotificationStatus.UNSUBSCRIBED;
                await _repository.UpdateAsync(notification);
                return;
            }
            
            // Send email
            await _emailProvider.SendAsync(
                user.Email,
                notification.Subject,
                notification.Body
            );
            
            // Update status
            notification.Status = NotificationStatus.SENT;
            notification.SentAt = DateTime.UtcNow;
            await _repository.UpdateAsync(notification);
            
            _logger.LogInformation("Email sent for notification {NotificationId}", @event.NotificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for notification {NotificationId}", @event.NotificationId);
            
            // Retry logic
            if (notification.RetryCount < 3)
            {
                notification.RetryCount++;
                notification.ErrorMessage = ex.Message;
                await _repository.UpdateAsync(notification);
                
                // Re-queue with exponential backoff
                await ReQueueWithBackoffAsync(notification);
            }
            else
            {
                notification.Status = NotificationStatus.FAILED;
                notification.ErrorMessage = ex.Message;
                await _repository.UpdateAsync(notification);
            }
        }
    }
    
    private async Task ReQueueAsync(Notification notification)
    {
        var delay = (notification.ScheduledFor - DateTime.UtcNow).TotalSeconds;
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(delay, 60)));
        
        // Re-publish to Kafka
        var @event = new NotificationEvent
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Channel = notification.Channel,
            Subject = notification.Subject,
            Body = notification.Body,
            ScheduledFor = notification.ScheduledFor
        };
        
        await _kafkaProducer.ProduceAsync("notification-events", 
            new Message<string, string>
            {
                Key = notification.UserId,
                Value = JsonConvert.SerializeObject(@event)
            }
        );
    }
    
    private async Task ReQueueWithBackoffAsync(Notification notification)
    {
        var backoffSeconds = Math.Pow(2, notification.RetryCount) * 60; // Exponential backoff
        await Task.Delay(TimeSpan.FromSeconds(backoffSeconds));
        
        await ReQueueAsync(notification);
    }
}
```

### 4. SMS Worker

```csharp
public class SmsWorker
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ISmsProvider _smsProvider;
    private readonly INotificationRepository _repository;
    
    public async Task ProcessSmsAsync(NotificationEvent @event)
    {
        var notification = await _repository.GetAsync(@event.NotificationId);
        
        try
        {
            var user = await _userRepository.GetAsync(notification.UserId);
            
            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                notification.Status = NotificationStatus.FAILED;
                notification.ErrorMessage = "User phone number not found";
                await _repository.UpdateAsync(notification);
                return;
            }
            
            // Send SMS
            await _smsProvider.SendAsync(user.PhoneNumber, notification.Body);
            
            notification.Status = NotificationStatus.SENT;
            notification.SentAt = DateTime.UtcNow;
            await _repository.UpdateAsync(notification);
        }
        catch (Exception ex)
        {
            // Retry logic similar to email
            if (notification.RetryCount < 3)
            {
                notification.RetryCount++;
                await _repository.UpdateAsync(notification);
            }
            else
            {
                notification.Status = NotificationStatus.FAILED;
                notification.ErrorMessage = ex.Message;
                await _repository.UpdateAsync(notification);
            }
        }
    }
}
```

### 5. Push Notification Worker

```csharp
public class PushNotificationWorker
{
    private readonly IPushProvider _pushProvider;
    private readonly INotificationRepository _repository;
    
    public async Task ProcessPushAsync(NotificationEvent @event)
    {
        var notification = await _repository.GetAsync(@event.NotificationId);
        
        try
        {
            var user = await _userRepository.GetAsync(notification.UserId);
            var devices = await _deviceRepository.GetUserDevicesAsync(notification.UserId);
            
            if (!devices.Any())
            {
                notification.Status = NotificationStatus.FAILED;
                notification.ErrorMessage = "No devices found for user";
                await _repository.UpdateAsync(notification);
                return;
            }
            
            // Send to all devices
            var tasks = devices.Select(device =>
                _pushProvider.SendAsync(
                    device.PushToken,
                    notification.Subject,
                    notification.Body
                )
            );
            
            await Task.WhenAll(tasks);
            
            notification.Status = NotificationStatus.SENT;
            notification.SentAt = DateTime.UtcNow;
            await _repository.UpdateAsync(notification);
        }
        catch (Exception ex)
        {
            // Retry logic
            if (notification.RetryCount < 3)
            {
                notification.RetryCount++;
                await _repository.UpdateAsync(notification);
            }
            else
            {
                notification.Status = NotificationStatus.FAILED;
                notification.ErrorMessage = ex.Message;
                await _repository.UpdateAsync(notification);
            }
        }
    }
}
```

### 6. Scheduling Service

```csharp
public class NotificationScheduler
{
    private readonly INotificationRepository _repository;
    private readonly IProducer<string, string> _kafkaProducer;
    
    public async Task ProcessScheduledNotificationsAsync()
    {
        // Run every minute
        while (true)
        {
            try
            {
                // Get notifications scheduled for next 5 minutes
                var scheduledNotifications = await _repository.GetScheduledNotificationsAsync(
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(5)
                );
                
                foreach (var notification in scheduledNotifications)
                {
                    // Publish to Kafka
                    var @event = new NotificationEvent
                    {
                        NotificationId = notification.NotificationId,
                        UserId = notification.UserId,
                        Channel = notification.Channel,
                        Subject = notification.Subject,
                        Body = notification.Body,
                        ScheduledFor = notification.ScheduledFor
                    };
                    
                    await _kafkaProducer.ProduceAsync("notification-events",
                        new Message<string, string>
                        {
                            Key = notification.UserId,
                            Value = JsonConvert.SerializeObject(@event)
                        }
                    );
                }
                
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled notifications");
            }
        }
    }
}
```

---

## Key Features

1. **Multi-Channel**: Email, SMS, Push
2. **High Throughput**: 1M notifications/day
3. **Reliable Delivery**: Kafka for durability
4. **Retry Logic**: Exponential backoff
5. **Scheduling**: Schedule up to 30 days
6. **Tracking**: Full delivery status
7. **Scalability**: Horizontal scaling of workers
8. **Monitoring**: Metrics and alerts

---

## Summary

These three examples cover:
1. **Policy Management** - Core business data
2. **Claims Processing** - Workflow and state management
3. **Real-Time Notifications** - Event-driven architecture

Each demonstrates different system design aspects:
- Database design and optimization
- Caching strategies
- Message queues and event streaming
- Workflow management
- Scalability and reliability
- Monitoring and observability

Good luck with your interview! 🚀

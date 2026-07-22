# Blackbaud Round 3 Mock Interview - Senior .NET Developer
## Complete Face-to-Face Interview Simulation

---

## Interview Schedule (3.5 Hours)

```
9:00 AM  - Welcome & Introduction (10 min)
9:10 AM  - SESSION 1: Advanced Coding & DSA (75 min)
10:25 AM - Break (10 min)
10:35 AM - SESSION 2: System Design (60 min)
11:35 AM - Break (10 min)
11:45 AM - SESSION 3: .NET Deep Dive (45 min)
12:30 PM - Lunch Break (30 min)
1:00 PM  - SESSION 4: Behavioral & Leadership (40 min)
1:40 PM  - SESSION 5: Manager Round (30 min)
2:10 PM  - Wrap-up & Questions (10 min)
```

---

# SESSION 1: ADVANCED CODING & DSA (75 minutes)

## Interviewer: Senior Software Engineer

---

### **PROBLEM 1: Design In-Memory Cache with TTL (25 minutes)**

**Interviewer:** "Good morning! Let's start with a design problem. We need an in-memory cache system for our nonprofit CRM application. Can you design and implement a thread-safe cache with the following requirements?"

**Requirements:**
1. Store key-value pairs
2. Support Time-To-Live (TTL) for each entry
3. Automatic cleanup of expired entries
4. Thread-safe operations
5. O(1) Get and Set operations
6. Memory-efficient

**Follow-up Questions I'll Ask:**
- How do you handle concurrent access?
- What happens when cache is full?
- How do you implement automatic cleanup?
- What data structures would you use?
- How do you prevent memory leaks?

**Expected Solution Structure:**

```csharp
public interface ICache<TKey, TValue> {
    void Set(TKey key, TValue value, TimeSpan ttl);
    bool TryGet(TKey key, out TValue value);
    void Remove(TKey key);
    void Clear();
    int Count { get; }
}

// I expect you to implement:
// 1. Thread-safe operations (locks or concurrent collections)
// 2. Expiration mechanism
// 3. Background cleanup
// 4. LRU eviction when capacity reached
```

**What I'm Looking For:**
- ✅ Correct use of `ConcurrentDictionary`
- ✅ Proper locking strategy
- ✅ Background thread for cleanup
- ✅ Handling edge cases (null keys, expired items)
- ✅ Memory management
- ✅ Clean, readable code

---

### **PROBLEM 2: Find Median from Data Stream (20 minutes)**

**Interviewer:** "Great! Now let's do an algorithm problem. At Blackbaud, we process donation data in real-time. Imagine we're receiving donation amounts as a stream, and we need to find the median at any point."

**Problem:**
```
Design a data structure that supports:
1. addNum(int num) - Add a number to the data structure
2. findMedian() - Return the median of all numbers so far

Example:
addNum(1)
addNum(2)
findMedian() -> 1.5
addNum(3)
findMedian() -> 2
```

**Constraints:**
- Numbers can be in any order
- Need to handle millions of numbers
- findMedian() should be as fast as possible

**Follow-up Questions:**
- What's the time complexity of your solution?
- Can you optimize it further?
- How would you handle this if data doesn't fit in memory?
- What if we need to find 90th percentile instead of median?

**Expected Approach:**
- Two heaps (max heap for lower half, min heap for upper half)
- Time: O(log n) for addNum, O(1) for findMedian
- Space: O(n)

---

### **PROBLEM 3: Design a Rate Limiter (30 minutes)**

**Interviewer:** "Excellent! Now, let's design something practical. Our APIs need rate limiting to prevent abuse. Design a rate limiter that limits requests per user."

**Requirements:**
1. Limit to N requests per time window (e.g., 100 requests/minute)
2. Distributed system support (multiple servers)
3. Minimal memory usage
4. Handle concurrent requests
5. Return appropriate response when limit exceeded

**Algorithms to Discuss:**
1. **Token Bucket**
2. **Leaky Bucket**
3. **Fixed Window Counter**
4. **Sliding Window Log**
5. **Sliding Window Counter**

**I'll Ask You to Compare:**

| Algorithm | Pros | Cons | Use Case |
|-----------|------|------|----------|
| Token Bucket | Smooth traffic, burst allowed | Complex implementation | API rate limiting |
| Fixed Window | Simple, memory efficient | Burst at window edges | Basic rate limiting |
| Sliding Window | Accurate, no burst issues | More memory | Strict rate limiting |

**Expected Implementation:**

```csharp
public interface IRateLimiter {
    Task<bool> AllowRequestAsync(string userId);
    Task<RateLimitInfo> GetRateLimitInfoAsync(string userId);
}

// I expect:
// 1. Redis-based distributed implementation
// 2. Sliding window algorithm
// 3. Atomic operations
// 4. Proper error handling
```

**Follow-up Questions:**
- How do you handle distributed systems?
- What if Redis goes down?
- How do you prevent race conditions?
- How do you handle clock skew across servers?
- How would you implement different rate limits for different user tiers?

---

# SESSION 2: SYSTEM DESIGN (60 minutes)

## Interviewer: Principal Engineer / Architect

---

### **SYSTEM DESIGN: Design Blackbaud's Donation Processing Platform**

**Interviewer:** "Welcome! As you know, Blackbaud serves thousands of nonprofits. Let's design a donation processing platform that can handle their needs. I'll give you the requirements, and I want you to design the entire system."

---

## **PART 1: Requirements Gathering (10 minutes)**

**Functional Requirements:**
1. Accept donations (one-time and recurring)
2. Process payments (credit card, PayPal, bank transfer, crypto)
3. Generate tax receipts
4. Send thank-you emails
5. Track donor history
6. Support campaigns and fundraising goals
7. Provide real-time donation tracking
8. Support matching gifts (employer matching)
9. Handle refunds and chargebacks

**Non-Functional Requirements:**
1. **Scale**: 
   - 1 million donations/day
   - Peak: 10,000 donations/minute (during campaigns)
   - 100,000 nonprofits
   - 50 million donors
2. **Performance**:
   - API response < 200ms
   - Payment processing < 5 seconds
   - Receipt generation < 10 seconds
3. **Reliability**:
   - 99.99% uptime
   - Zero data loss
   - Idempotent operations
4. **Security**:
   - PCI DSS compliant
   - Encrypted data at rest and in transit
   - Audit logs for all transactions
5. **Compliance**:
   - GDPR compliant
   - SOC 2 certified
   - Tax receipt regulations

**Questions I Expect You to Ask:**
- What's the average donation amount?
- What's the data retention policy?
- Do we need real-time analytics?
- What's the acceptable latency for receipt generation?
- Do we need to support international payments?
- What's the expected growth rate?
- What's the budget for infrastructure?

---

## **PART 2: High-Level Architecture (15 minutes)**

**I Want You to Draw This on Whiteboard:**

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                          │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │   Web    │  │  Mobile  │  │   API    │  │  Widget  │   │
│  │   App    │  │   App    │  │ Clients  │  │ (Embed)  │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTPS
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                     API GATEWAY LAYER                        │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  Azure API Management / Kong                           │ │
│  │  - Authentication (OAuth 2.0 / JWT)                    │ │
│  │  - Rate Limiting                                       │ │
│  │  - Request Validation                                  │ │
│  │  - SSL Termination                                     │ │
│  │  - API Versioning                                      │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────┬───────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Donation   │  │   Payment    │  │   Donor      │
│   Service    │  │   Service    │  │   Service    │
│  (.NET Core) │  │  (.NET Core) │  │  (.NET Core) │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘
       │                 │                 │
       └─────────────────┼─────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    MESSAGE QUEUE LAYER                       │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  Azure Service Bus / RabbitMQ / Kafka                  │ │
│  │  - payment-processing                                  │ │
│  │  - receipt-generation                                  │ │
│  │  - email-notification                                  │ │
│  │  - analytics-events                                    │ │
│  │  - webhook-delivery                                    │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────┬───────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┬─────────────┐
        ▼                 ▼                 ▼             ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Receipt    │  │    Email     │  │  Analytics   │  │   Webhook    │
│   Service    │  │   Service    │  │   Service    │  │   Service    │
└──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘
        │                 │                 │             │
        └─────────────────┼─────────────────┴─────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATA LAYER                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  PostgreSQL  │  │   Cosmos DB  │  │    Redis     │     │
│  │ (Transactional)│ │  (Analytics) │  │   (Cache)    │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  Blob Storage│  │  Event Store │  │  Data Lake   │     │
│  │  (Receipts)  │  │  (Audit Log) │  │  (Analytics) │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                  EXTERNAL SERVICES                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │    Stripe    │  │    PayPal    │  │   SendGrid   │     │
│  │  (Payments)  │  │  (Payments)  │  │   (Email)    │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
```

**Questions I'll Ask:**
- Why did you choose this architecture?
- How do you handle service-to-service communication?
- What happens if a service goes down?
- How do you ensure data consistency?
- How do you handle distributed transactions?

---

## **PART 3: Deep Dive - Donation Service API (15 minutes)**

**Interviewer:** "Let's zoom into the Donation Service. Show me the API design and implementation."

**Expected API Design:**

```csharp
// POST /api/v1/donations
[ApiController]
[Route("api/v1/donations")]
public class DonationController : ControllerBase {
    private readonly IDonationService _donationService;
    private readonly IMessageQueue _messageQueue;
    private readonly IIdempotencyService _idempotency;
    private readonly ILogger<DonationController> _logger;
    
    [HttpPost]
    [ProducesResponseType(typeof(DonationResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<DonationResponse>> CreateDonation(
        [FromBody] CreateDonationRequest request,
        CancellationToken cancellationToken) {
        
        // 1. Validate request
        if (!ModelState.IsValid) {
            return BadRequest(new ErrorResponse {
                Code = "INVALID_REQUEST",
                Message = "Invalid donation request",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        // 2. Check idempotency (prevent duplicate donations)
        var idempotencyKey = request.IdempotencyKey ?? 
            $"{request.DonorId}:{request.Amount}:{request.Timestamp}";
        
        var existingDonation = await _idempotency
            .GetAsync<Donation>(idempotencyKey, cancellationToken);
        
        if (existingDonation != null) {
            _logger.LogInformation(
                "Duplicate donation request detected. IdempotencyKey: {Key}", 
                idempotencyKey);
            return Ok(MapToResponse(existingDonation));
        }
        
        // 3. Create donation record (status: Pending)
        var donation = new Donation {
            Id = Guid.NewGuid(),
            DonorId = request.DonorId,
            NonprofitId = request.NonprofitId,
            CampaignId = request.CampaignId,
            Amount = request.Amount,
            Currency = request.Currency ?? "USD",
            PaymentMethod = request.PaymentMethod,
            Status = DonationStatus.Pending,
            IdempotencyKey = idempotencyKey,
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };
        
        // 4. Save to database
        await _donationService.CreateAsync(donation, cancellationToken);
        
        // 5. Store idempotency record
        await _idempotency.SetAsync(
            idempotencyKey, 
            donation, 
            TimeSpan.FromHours(24), 
            cancellationToken);
        
        // 6. Publish event for async processing
        await _messageQueue.PublishAsync(
            "payment-processing",
            new PaymentProcessingEvent {
                DonationId = donation.Id,
                Amount = donation.Amount,
                Currency = donation.Currency,
                PaymentMethod = donation.PaymentMethod,
                PaymentDetails = request.PaymentDetails
            },
            cancellationToken);
        
        // 7. Return 202 Accepted (async processing)
        _logger.LogInformation(
            "Donation created successfully. DonationId: {DonationId}", 
            donation.Id);
        
        return Accepted(
            $"/api/v1/donations/{donation.Id}",
            MapToResponse(donation));
    }
    
    // GET /api/v1/donations/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DonationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DonationResponse>> GetDonation(
        Guid id,
        CancellationToken cancellationToken) {
        
        var donation = await _donationService.GetByIdAsync(id, cancellationToken);
        
        if (donation == null) {
            return NotFound(new ErrorResponse {
                Code = "DONATION_NOT_FOUND",
                Message = $"Donation with ID {id} not found"
            });
        }
        
        return Ok(MapToResponse(donation));
    }
    
    // GET /api/v1/donations?donorId={donorId}&status={status}
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<DonationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<DonationResponse>>> GetDonations(
        [FromQuery] GetDonationsQuery query,
        CancellationToken cancellationToken) {
        
        var donations = await _donationService.GetDonationsAsync(
            query, 
            cancellationToken);
        
        return Ok(new PagedResponse<DonationResponse> {
            Data = donations.Items.Select(MapToResponse).ToList(),
            TotalCount = donations.TotalCount,
            Page = query.Page,
            PageSize = query.PageSize
        });
    }
}

// Request/Response Models
public class CreateDonationRequest {
    [Required]
    public Guid DonorId { get; set; }
    
    [Required]
    public Guid NonprofitId { get; set; }
    
    public Guid? CampaignId { get; set; }
    
    [Required]
    [Range(0.01, 1000000)]
    public decimal Amount { get; set; }
    
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    [Required]
    public PaymentDetails PaymentDetails { get; set; }
    
    public string IdempotencyKey { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; }
}

public class DonationResponse {
    public Guid Id { get; set; }
    public Guid DonorId { get; set; }
    public Guid NonprofitId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DonationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string ReceiptUrl { get; set; }
}

public enum DonationStatus {
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
```

**Questions I'll Ask:**
- Why did you use 202 Accepted instead of 200 OK?
- How do you handle idempotency?
- What happens if the message queue is down?
- How do you ensure exactly-once processing?
- How do you handle payment failures?

---

## **PART 4: Database Design (10 minutes)**

**Interviewer:** "Show me the database schema for this system."

**Expected Schema:**

```sql
-- Donors table
CREATE TABLE Donors (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Email VARCHAR(255) UNIQUE NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    AddressLine1 VARCHAR(255),
    AddressLine2 VARCHAR(255),
    City VARCHAR(100),
    State VARCHAR(50),
    ZipCode VARCHAR(20),
    Country VARCHAR(2) DEFAULT 'US',
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    IsActive BOOLEAN DEFAULT TRUE,
    
    INDEX idx_donors_email (Email),
    INDEX idx_donors_created_at (CreatedAt)
);

-- Nonprofits table
CREATE TABLE Nonprofits (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(255) NOT NULL,
    EIN VARCHAR(20) UNIQUE NOT NULL, -- Tax ID
    Email VARCHAR(255) NOT NULL,
    Phone VARCHAR(20),
    Website VARCHAR(255),
    Description TEXT,
    LogoUrl VARCHAR(500),
    Category VARCHAR(50),
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    IsActive BOOLEAN DEFAULT TRUE,
    
    INDEX idx_nonprofits_ein (EIN),
    INDEX idx_nonprofits_category (Category)
);

-- Campaigns table
CREATE TABLE Campaigns (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    NonprofitId UUID NOT NULL REFERENCES Nonprofits(Id),
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    GoalAmount DECIMAL(18,2) NOT NULL,
    CurrentAmount DECIMAL(18,2) DEFAULT 0,
    Currency VARCHAR(3) DEFAULT 'USD',
    StartDate TIMESTAMP NOT NULL,
    EndDate TIMESTAMP NOT NULL,
    Status VARCHAR(20) NOT NULL, -- Active, Completed, Cancelled
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    
    INDEX idx_campaigns_nonprofit (NonprofitId),
    INDEX idx_campaigns_status (Status),
    INDEX idx_campaigns_dates (StartDate, EndDate)
);

-- Donations table (main table)
CREATE TABLE Donations (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    DonorId UUID NOT NULL REFERENCES Donors(Id),
    NonprofitId UUID NOT NULL REFERENCES Nonprofits(Id),
    CampaignId UUID REFERENCES Campaigns(Id),
    Amount DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'USD',
    PaymentMethod VARCHAR(50) NOT NULL, -- CreditCard, PayPal, BankTransfer
    Status VARCHAR(20) NOT NULL, -- Pending, Processing, Completed, Failed, Refunded
    IdempotencyKey VARCHAR(100) UNIQUE NOT NULL,
    TransactionId VARCHAR(100), -- Payment gateway transaction ID
    ReceiptUrl VARCHAR(500),
    IsRecurring BOOLEAN DEFAULT FALSE,
    RecurringDonationId UUID REFERENCES RecurringDonations(Id),
    FailureReason TEXT,
    ProcessedAt TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    Metadata JSONB, -- Flexible metadata storage
    
    INDEX idx_donations_donor (DonorId),
    INDEX idx_donations_nonprofit (NonprofitId),
    INDEX idx_donations_campaign (CampaignId),
    INDEX idx_donations_status (Status),
    INDEX idx_donations_created_at (CreatedAt),
    INDEX idx_donations_idempotency (IdempotencyKey),
    INDEX idx_donations_transaction (TransactionId)
);

-- Recurring Donations table
CREATE TABLE RecurringDonations (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    DonorId UUID NOT NULL REFERENCES Donors(Id),
    NonprofitId UUID NOT NULL REFERENCES Nonprofits(Id),
    CampaignId UUID REFERENCES Campaigns(Id),
    Amount DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'USD',
    Frequency VARCHAR(20) NOT NULL, -- Monthly, Quarterly, Yearly
    PaymentMethod VARCHAR(50) NOT NULL,
    PaymentMethodToken VARCHAR(255), -- Tokenized payment method
    NextProcessDate DATE NOT NULL,
    LastProcessDate DATE,
    StartDate DATE NOT NULL,
    EndDate DATE, -- NULL for indefinite
    Status VARCHAR(20) NOT NULL, -- Active, Paused, Cancelled, Completed
    FailureCount INT DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    
    INDEX idx_recurring_donor (DonorId),
    INDEX idx_recurring_nonprofit (NonprofitId),
    INDEX idx_recurring_next_process (NextProcessDate, Status),
    INDEX idx_recurring_status (Status)
);

-- Payment Transactions (audit log)
CREATE TABLE PaymentTransactions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    DonationId UUID NOT NULL REFERENCES Donations(Id),
    GatewayProvider VARCHAR(50) NOT NULL, -- Stripe, PayPal, etc.
    GatewayTransactionId VARCHAR(100),
    Amount DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    Status VARCHAR(20) NOT NULL, -- Initiated, Authorized, Captured, Failed, Refunded
    ErrorCode VARCHAR(50),
    ErrorMessage TEXT,
    RequestPayload JSONB,
    ResponsePayload JSONB,
    ProcessedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    
    INDEX idx_payment_transactions_donation (DonationId),
    INDEX idx_payment_transactions_gateway (GatewayTransactionId),
    INDEX idx_payment_transactions_status (Status)
);

-- Receipts table
CREATE TABLE Receipts (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    DonationId UUID NOT NULL REFERENCES Donations(Id),
    ReceiptNumber VARCHAR(50) UNIQUE NOT NULL,
    FileUrl VARCHAR(500) NOT NULL,
    GeneratedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    EmailedAt TIMESTAMP,
    
    INDEX idx_receipts_donation (DonationId),
    INDEX idx_receipts_number (ReceiptNumber)
);

-- Audit Log table (for compliance)
CREATE TABLE AuditLogs (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    EntityType VARCHAR(50) NOT NULL, -- Donation, Donor, etc.
    EntityId UUID NOT NULL,
    Action VARCHAR(50) NOT NULL, -- Create, Update, Delete
    UserId UUID, -- Who performed the action
    Changes JSONB, -- Before/after values
    IpAddress VARCHAR(45),
    UserAgent VARCHAR(500),
    Timestamp TIMESTAMP NOT NULL DEFAULT NOW(),
    
    INDEX idx_audit_entity (EntityType, EntityId),
    INDEX idx_audit_timestamp (Timestamp)
);

-- Materialized view for analytics (updated periodically)
CREATE MATERIALIZED VIEW DonationAnalytics AS
SELECT 
    DATE_TRUNC('day', CreatedAt) AS Date,
    NonprofitId,
    CampaignId,
    COUNT(*) AS TotalDonations,
    SUM(Amount) AS TotalAmount,
    AVG(Amount) AS AvgAmount,
    COUNT(DISTINCT DonorId) AS UniqueDonors
FROM Donations
WHERE Status = 'Completed'
GROUP BY DATE_TRUNC('day', CreatedAt), NonprofitId, CampaignId;

CREATE INDEX idx_donation_analytics_date ON DonationAnalytics(Date);
CREATE INDEX idx_donation_analytics_nonprofit ON DonationAnalytics(NonprofitId);
```

**Questions I'll Ask:**
- Why did you choose PostgreSQL over NoSQL?
- How do you handle high write throughput?
- How do you ensure data consistency?
- What's your sharding strategy?
- How do you handle database migrations?
- How do you optimize for read-heavy vs write-heavy workloads?

---

## **PART 5: Scalability & Performance (10 minutes)**

**Interviewer:** "How would you scale this system to handle 10x traffic?"

**Expected Discussion Points:**

### **1. Horizontal Scaling**
```
- Load Balancer (Azure Load Balancer / Application Gateway)
- Multiple instances of each service (Kubernetes pods)
- Auto-scaling based on CPU/memory/queue depth
- Database read replicas
```

### **2. Caching Strategy**
```
- Redis for:
  - User sessions
  - Donor profiles
  - Campaign data
  - Rate limiting counters
  - Idempotency keys (24-hour TTL)
  
- Cache invalidation:
  - Write-through cache for updates
  - TTL-based expiration
  - Event-based invalidation
```

### **3. Database Optimization**
```
- Read replicas for queries
- Connection pooling
- Query optimization (indexes, explain plans)
- Partitioning by date (donations table)
- Archiving old data
```

### **4. Async Processing**
```
- Message queues for all non-critical operations
- Worker pools for parallel processing
- Dead letter queues for failed messages
- Retry with exponential backoff
```

### **5. CDN for Static Assets**
```
- Receipts (PDFs)
- Images
- JavaScript/CSS
```

---

# SESSION 3: .NET DEEP DIVE (45 minutes)

## Interviewer: Tech Lead

---

### **PART 1: Async/Await Deep Dive (15 minutes)**

**Interviewer:** "Let's talk about async programming in .NET. Explain what happens under the hood when you use async/await."

**Questions I'll Ask:**

**Q1: What's the difference between these two approaches?**

```csharp
// Approach 1
public async Task<string> GetDataAsync() {
    var result = await _httpClient.GetStringAsync("https://api.example.com");
    return result;
}

// Approach 2
public Task<string> GetDataAsync() {
    return _httpClient.GetStringAsync("https://api.example.com");
}
```

**Expected Answer:**
- Approach 1: Creates state machine, allocates memory for async context
- Approach 2: Returns task directly, no state machine overhead
- Approach 2 is more efficient when just returning a task
- Use Approach 1 when you need to do work before/after await

**Q2: What's wrong with this code?**

```csharp
public async Task ProcessDataAsync() {
    var task1 = GetData1Async();
    var task2 = GetData2Async();
    var task3 = GetData3Async();
    
    var result1 = await task1;
    var result2 = await task2;
    var result3 = await task3;
    
    // Process results
}
```

**Expected Answer:**
- Nothing wrong! Tasks run in parallel
- Awaiting sequentially is fine here
- Alternative: `await Task.WhenAll(task1, task2, task3)`

**Q3: Explain this deadlock scenario:**

```csharp
// ASP.NET Framework (not Core)
public ActionResult Index() {
    var result = GetDataAsync().Result; // DEADLOCK!
    return View(result);
}

public async Task<string> GetDataAsync() {
    await Task.Delay(1000);
    return "data";
}
```

**Expected Answer:**
- `.Result` blocks the synchronization context
- `await` tries to resume on same context
- Context is blocked waiting for task
- Deadlock!
- Solution: Use `ConfigureAwait(false)` or don't block on async code

**Q4: What's the difference between `Task.Run()` and `async/await`?**

```csharp
// Task.Run - CPU-bound work
public async Task<int> CalculateAsync(int n) {
    return await Task.Run(() => {
        // Expensive CPU-bound calculation
        return Fibonacci(n);
    });
}

// async/await - I/O-bound work
public async Task<string> GetDataAsync() {
    return await _httpClient.GetStringAsync("url");
}
```

**Expected Answer:**
- `Task.Run`: Creates new thread from thread pool (CPU-bound)
- `async/await`: No new thread, just continuation (I/O-bound)
- Don't use `Task.Run` for I/O operations
- Don't use synchronous code with `await` for CPU operations

**Q5: How do you handle exceptions in async code?**

```csharp
// Multiple tasks with exceptions
public async Task ProcessMultipleAsync() {
    try {
        var tasks = new[] {
            ProcessAsync(1),
            ProcessAsync(2),
            ProcessAsync(3)
        };
        
        await Task.WhenAll(tasks);
    }
    catch (Exception ex) {
        // Only catches FIRST exception!
        // How do you get all exceptions?
    }
}
```

**Expected Answer:**
```csharp
public async Task ProcessMultipleAsync() {
    var tasks = new[] {
        ProcessAsync(1),
        ProcessAsync(2),
        ProcessAsync(3)
    };
    
    try {
        await Task.WhenAll(tasks);
    }
    catch {
        // Get all exceptions
        foreach (var task in tasks) {
            if (task.IsFaulted) {
                _logger.LogError(task.Exception, "Task failed");
            }
        }
    }
}
```

---

### **PART 2: Memory Management & Performance (15 minutes)**

**Interviewer:** "Let's talk about memory and performance in .NET."

**Q1: Explain the difference between stack and heap allocation.**

```csharp
public void Example() {
    int x = 10;              // Stack
    string s = "hello";      // Reference on stack, object on heap
    var person = new Person(); // Reference on stack, object on heap
    
    // What about structs?
    Point p = new Point(1, 2); // Where is this allocated?
}

public struct Point {
    public int X { get; set; }
    public int Y { get; set; }
}
```

**Expected Answer:**
- Value types (int, struct) → Stack (if local variable)
- Reference types (class, string) → Heap
- Struct on stack UNLESS it's a field in a class
- Boxing moves value type to heap

**Q2: How does garbage collection work in .NET?**

**Expected Answer:**
```
Generation 0: Short-lived objects (most collections happen here)
Generation 1: Medium-lived objects
Generation 2: Long-lived objects (expensive to collect)

GC Triggers:
- Gen 0 full
- Explicit GC.Collect() (avoid!)
- Low memory

GC Types:
- Workstation GC (default)
- Server GC (better for multi-core servers)
```

**Q3: What causes memory leaks in .NET?**

```csharp
// Example 1: Event handlers
public class Publisher {
    public event EventHandler DataChanged;
}

public class Subscriber {
    public Subscriber(Publisher publisher) {
        publisher.DataChanged += OnDataChanged; // LEAK!
    }
    
    private void OnDataChanged(object sender, EventArgs e) { }
    
    // Missing: Unsubscribe in Dispose!
}

// Example 2: Static references
public static class Cache {
    private static List<byte[]> _data = new List<byte[]>(); // LEAK!
    
    public static void Add(byte[] data) {
        _data.Add(data); // Never released!
    }
}

// Example 3: Unclosed resources
public void ProcessFile() {
    var stream = File.OpenRead("file.txt"); // LEAK!
    // Process file
    // Missing: stream.Dispose()
}
```

**Expected Solutions:**
```csharp
// Solution 1: Unsubscribe
public class Subscriber : IDisposable {
    private readonly Publisher _publisher;
    
    public Subscriber(Publisher publisher) {
        _publisher = publisher;
        _publisher.DataChanged += OnDataChanged;
    }
    
    public void Dispose() {
        _publisher.DataChanged -= OnDataChanged;
    }
}

// Solution 2: Weak references or bounded cache
public static class Cache {
    private static readonly ConcurrentDictionary<string, WeakReference> _cache = new();
    
    public static void Add(string key, object value) {
        _cache[key] = new WeakReference(value);
    }
}

// Solution 3: Using statement
public void ProcessFile() {
    using var stream = File.OpenRead("file.txt");
    // Process file
} // Automatically disposed
```

**Q4: How would you optimize this code?**

```csharp
// SLOW CODE
public async Task<List<UserDto>> GetUsersAsync() {
    var users = await _context.Users.ToListAsync(); // Loads ALL users
    
    var result = new List<UserDto>();
    foreach (var user in users) {
        if (user.IsActive) {
            var dto = new UserDto {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                OrderCount = await _context.Orders
                    .CountAsync(o => o.UserId == user.Id) // N+1 query!
            };
            result.Add(dto);
        }
    }
    
    return result;
}
```

**Expected Optimized Version:**

```csharp
// FAST CODE
public async Task<List<UserDto>> GetUsersAsync() {
    var users = await _context.Users
        .AsNoTracking() // No change tracking overhead
        .Where(u => u.IsActive) // Filter in database
        .Select(u => new UserDto {
            Id = u.Id,
            Name = u.FirstName + " " + u.LastName, // Computed in database
            Email = u.Email,
            OrderCount = u.Orders.Count() // Joined in single query
        })
        .ToListAsync();
    
    return users;
}

// Improvements:
// 1. AsNoTracking() - 30% faster for read-only queries
// 2. Where() before ToList() - filters in database
// 3. Select() projection - only loads needed columns
// 4. Single query instead of N+1
```

**Q5: Explain Span<T> and Memory<T>. When would you use them?**

```csharp
// Traditional approach - allocates new string
public string GetSubstring(string input) {
    return input.Substring(0, 10); // Allocates new string
}

// Span<T> approach - no allocation
public ReadOnlySpan<char> GetSubstring(ReadOnlySpan<char> input) {
    return input.Slice(0, 10); // No allocation!
}

// Use case: Parsing large strings
public int ParseNumber(string input) {
    // Traditional: input.Substring(0, 5) allocates
    // Span: no allocation
    ReadOnlySpan<char> span = input.AsSpan(0, 5);
    return int.Parse(span);
}
```

**Expected Answer:**
- `Span<T>`: Stack-only, high-performance, no allocation
- `Memory<T>`: Can be used in async methods
- Use for: String parsing, buffer manipulation, high-performance scenarios
- Cannot be used as class fields (stack-only)

---

### **PART 3: Dependency Injection & Architecture (15 minutes)**

**Interviewer:** "Let's discuss dependency injection and architecture patterns."

**Q1: Explain the three service lifetimes in ASP.NET Core.**

```csharp
// Startup.cs / Program.cs
services.AddTransient<IEmailService, EmailService>();
services.AddScoped<IOrderService, OrderService>();
services.AddSingleton<ICacheService, CacheService>();
```

**Expected Answer:**

| Lifetime | Created | Disposed | Use Case |
|----------|---------|----------|----------|
| **Transient** | Every time requested | After use | Lightweight, stateless services |
| **Scoped** | Once per request | End of request | DbContext, repositories |
| **Singleton** | Once per application | Application shutdown | Caching, configuration |

**Common Mistakes:**
- ❌ Injecting scoped service into singleton (captive dependency)
- ❌ Using singleton for DbContext (not thread-safe)
- ❌ Using transient for expensive objects

**Q2: What's wrong with this code?**

```csharp
// Singleton service
public class CacheService {
    private readonly AppDbContext _context; // WRONG!
    
    public CacheService(AppDbContext context) {
        _context = context; // Captive dependency!
    }
}

// Registration
services.AddSingleton<CacheService>();
services.AddScoped<AppDbContext>();
```

**Expected Answer:**
- Captive dependency problem
- Singleton captures scoped DbContext
- DbContext lives for entire application lifetime
- Not thread-safe, memory leak
- Solution: Inject `IServiceProvider` and resolve DbContext per operation

**Q3: Implement the Repository and Unit of Work patterns.**

```csharp
// Generic Repository
public interface IRepository<T> where T : class {
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class Repository<T> : IRepository<T> where T : class {
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(AppDbContext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T> GetByIdAsync(int id) {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync() {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<T> AddAsync(T entity) {
        await _dbSet.AddAsync(entity);
        return entity;
    }
    
    public async Task UpdateAsync(T entity) {
        _dbSet.Update(entity);
    }
    
    public async Task DeleteAsync(int id) {
        var entity = await GetByIdAsync(id);
        if (entity != null) {
            _dbSet.Remove(entity);
        }
    }
}

// Unit of Work
public interface IUnitOfWork : IDisposable {
    IRepository<Donation> Donations { get; }
    IRepository<Donor> Donors { get; }
    IRepository<Campaign> Campaigns { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork {
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    
    public UnitOfWork(AppDbContext context) {
        _context = context;
        Donations = new Repository<Donation>(_context);
        Donors = new Repository<Donor>(_context);
        Campaigns = new Repository<Campaign>(_context);
    }
    
    public IRepository<Donation> Donations { get; }
    public IRepository<Donor> Donors { get; }
    public IRepository<Campaign> Campaigns { get; }
    
    public async Task<int> SaveChangesAsync() {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync() {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync() {
        try {
            await SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch {
            await RollbackTransactionAsync();
            throw;
        }
        finally {
            _transaction?.Dispose();
        }
    }
    
    public async Task RollbackTransactionAsync() {
        await _transaction?.RollbackAsync();
        _transaction?.Dispose();
    }
    
    public void Dispose() {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}

// Usage
public class DonationService {
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task ProcessDonationAsync(Donation donation) {
        await _unitOfWork.BeginTransactionAsync();
        
        try {
            // Add donation
            await _unitOfWork.Donations.AddAsync(donation);
            
            // Update campaign total
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(donation.CampaignId);
            campaign.CurrentAmount += donation.Amount;
            await _unitOfWork.Campaigns.UpdateAsync(campaign);
            
            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();
        }
        catch {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

**Questions I'll Ask:**
- Why use Repository pattern?
- What are the drawbacks?
- When would you NOT use it?
- How do you handle complex queries?

---

# SESSION 4: BEHAVIORAL & LEADERSHIP (40 minutes)

## Interviewer: Engineering Manager

---

### **PART 1: Technical Leadership (20 minutes)**

**Q1: Tell me about a time you led a technical initiative.**

**What I'm Looking For:**
- Initiative and ownership
- Technical decision-making
- Stakeholder management
- Measuring success

**Expected STAR Structure:**
- **Situation**: What was the problem/opportunity?
- **Task**: What was your role?
- **Action**: What did you do? (Be specific!)
- **Result**: What was the outcome? (Use numbers!)

**Q2: Describe a time you had to make a difficult technical decision with incomplete information.**

**What I'm Looking For:**
- Decision-making process
- Risk assessment
- Communication with stakeholders
- Learning from outcomes

**Q3: Tell me about a time you disagreed with a senior engineer or architect.**

**What I'm Looking For:**
- Respectful disagreement
- Data-driven arguments
- Willingness to be wrong
- Collaboration

**Q4: How do you ensure code quality in your team?**

**Expected Answer:**
- Code reviews (process, checklist)
- Automated testing (unit, integration, e2e)
- Static analysis (SonarQube, Roslyn analyzers)
- CI/CD pipelines
- Pair programming
- Documentation
- Tech debt management

**Q5: Describe your experience mentoring junior developers.**

**What I'm Looking For:**
- Structured approach
- Patience and empathy
- Measuring progress
- Celebrating wins

---

### **PART 2: Problem-Solving & Conflict Resolution (20 minutes)**

**Q6: Tell me about a time you had to debug a complex production issue.**

**What I'm Looking For:**
- Systematic approach
- Use of tools (logs, profilers, debuggers)
- Communication during incident
- Post-mortem and prevention

**Q7: Describe a time when you had to balance technical debt with new features.**

**What I'm Looking For:**
- Understanding of technical debt
- Prioritization framework
- Communication with product/business
- Long-term thinking

**Q8: Tell me about a time you missed a deadline. What happened?**

**What I'm Looking For:**
- Honesty about failure
- Taking responsibility
- Learning from mistakes
- Process improvements

**Q9: How do you handle disagreements within your team?**

**Expected Answer:**
- Listen to all perspectives
- Focus on data and facts
- Find common ground
- Make decision and move forward
- Follow up to ensure alignment

**Q10: Tell me about a time you had to learn a new technology quickly.**

**What I'm Looking For:**
- Learning strategy
- Resourcefulness
- Applying knowledge
- Sharing with team

---

# SESSION 5: MANAGER ROUND (30 minutes)

## Interviewer: Hiring Manager / Director

---

### **PART 1: Career Goals & Motivation (10 minutes)**

**Q1: Why do you want to work at Blackbaud?**

**What I'm Looking For:**
- Mission alignment (social good)
- Technical challenges
- Growth opportunities
- Company culture fit

**Strong Answer Example:**
> "Three main reasons: First, the mission deeply resonates with me. I've volunteered as a developer for two local nonprofits, and I've seen firsthand how technology can amplify their impact. Building software that helps organizations do more good is exactly what I want to do with my career.
>
> Second, the technical challenges excite me. Working with .NET Core, Azure, and microservices at the scale Blackbaud operates is exactly where I want to grow. I've read about your migration to cloud-native architecture, and I'd love to contribute my experience with distributed systems.
>
> Third, I've researched your engineering culture - the emphasis on code quality, continuous learning, and work-life balance aligns perfectly with my values. I want to work somewhere I can grow both technically and personally while making a real difference."

**Q2: Where do you see yourself in 5 years?**

**What I'm Looking For:**
- Ambition but realistic
- Technical vs management track
- Alignment with company growth
- Continuous learning mindset

**Strong Answer Example:**
> "In 5 years, I see myself as a technical leader at Blackbaud - either a staff engineer or engineering manager, depending on where I can have the most impact. Technically, I want to:
> - Architect complex distributed systems
> - Mentor and grow junior engineers
> - Contribute to technical strategy and standards
> - Maybe speak at conferences about our work
>
> I'm open to both technical leadership and people management. What matters most is having significant impact on the product and helping the team succeed. I'd love to hear your thoughts on career paths at Blackbaud."

**Q3: What are you looking for in your next role?**

**Expected Answer:**
- Technical growth
- Meaningful work
- Great team
- Work-life balance
- Learning opportunities

---

### **PART 2: Working Style & Team Fit (10 minutes)**

**Q4: Describe your ideal work environment.**

**What I'm Looking For:**
- Collaboration style
- Communication preferences
- Work-life balance
- Remote/hybrid preferences

**Q5: How do you prefer to receive feedback?**

**Strong Answer:**
> "I prefer direct, honest feedback as soon as possible. I'd rather know immediately if something isn't working so I can fix it. I appreciate when feedback is:
> - **Specific**: Not 'do better' but 'here's what to improve and why'
> - **Actionable**: What should I do differently?
> - **Balanced**: What am I doing well?
> - **Timely**: Regular check-ins vs waiting for annual reviews
>
> I also believe in two-way feedback. I want to give feedback to my manager and teammates to help us all improve."

**Q6: Tell me about your experience working in Agile/Scrum.**

**What I'm Looking For:**
- Understanding of Agile principles
- Experience with ceremonies
- Adaptability
- Continuous improvement

**Q7: How do you handle work-life balance?**

**What I'm Looking For:**
- Boundaries
- Time management
- Sustainable pace
- Red flags (workaholic, burnout)

---

### **PART 3: Technical Vision & Strategy (10 minutes)**

**Q8: What excites you most about .NET and Azure?**

**Strong Answer:**
> ".NET 8 and Azure are incredibly exciting right now:
>
> **.NET 8:**
> - Native AOT compilation for faster startup and smaller memory footprint
> - Performance improvements (JSON serialization, LINQ)
> - Minimal APIs for lightweight microservices
> - Better observability with OpenTelemetry
>
> **Azure:**
> - Kubernetes (AKS) for container orchestration
> - Azure Functions for serverless
> - Cosmos DB for global distribution
> - Service Bus for reliable messaging
> - Application Insights for monitoring
>
> The combination enables building highly scalable, observable, cloud-native applications. I'm particularly excited about using these technologies to help nonprofits scale their impact."

**Q9: How do you stay current with technology?**

**Expected Answer:**
- Daily: Blogs, Twitter, Hacker News
- Weekly: YouTube, tutorials, side projects
- Monthly: User groups, courses
- Quarterly: Conferences, books
- Continuous: Open source contributions

**Q10: What questions do you have for me?**

**Strong Questions to Ask:**

**About the Role:**
- What does success look like in the first 30/60/90 days?
- What are the biggest technical challenges the team is facing?
- How much of the work is new features vs maintenance vs technical debt?
- What's the on-call rotation like?

**About the Team:**
- Can you tell me about the team structure and dynamics?
- How does the team handle technical disagreements?
- What's the balance between senior and junior engineers?
- How does the team collaborate (pair programming, code reviews)?

**About You (the Manager):**
- What's your management style?
- How do you support your team's professional growth?
- What do you enjoy most about working at Blackbaud?
- What's the most challenging part of your role?

**About Growth:**
- What are the career progression opportunities?
- Does Blackbaud support conference attendance or certifications?
- Are there opportunities to work on different products or teams?
- How does Blackbaud invest in employee development?

**About the Company:**
- How does Blackbaud measure impact on nonprofit clients?
- What are the company's technical priorities for the next year?
- How has the engineering culture evolved recently?
- What makes Blackbaud different from other tech companies?

**About the Product:**
- What's the most exciting feature you're working on?
- How do you gather feedback from nonprofit customers?
- What's the biggest technical challenge in the product roadmap?

---

# PREPARATION CHECKLIST

## 1 Week Before Interview

### **Technical Preparation**
- [ ] Review all LeetCode problems (especially Medium difficulty)
- [ ] Practice system design (donation processing, notification system)
- [ ] Review .NET concepts (async/await, DI, EF Core)
- [ ] Practice coding on whiteboard/collaborative tools
- [ ] Review design patterns (Repository, Unit of Work, Factory, Strategy)
- [ ] Study Azure services (AKS, Service Bus, Cosmos DB, Redis)

### **Behavioral Preparation**
- [ ] Prepare 7-10 STAR stories
- [ ] Practice 2-minute elevator pitch
- [ ] Research Blackbaud (products, mission, recent news)
- [ ] Prepare questions for each interviewer
- [ ] Review Blackbaud's core values

### **Logistics**
- [ ] Test video/audio equipment
- [ ] Set up collaborative coding tools (CoderPad, HackerRank)
- [ ] Prepare workspace (quiet, good lighting, clean background)
- [ ] Print resume and bring notebook
- [ ] Plan route to office (if onsite)

## Day Before Interview

### **Final Review**
- [ ] Review STAR stories
- [ ] Practice elevator pitch
- [ ] Review questions to ask
- [ ] Review Blackbaud's products and mission
- [ ] Review your own resume and projects

### **Logistics**
- [ ] Charge all devices
- [ ] Test internet connection
- [ ] Prepare professional attire
- [ ] Set multiple alarms
- [ ] Get 8 hours of sleep!

## Morning of Interview

### **Preparation**
- [ ] Eat a good breakfast
- [ ] Arrive/login 15 minutes early
- [ ] Have water nearby
- [ ] Have pen and paper ready
- [ ] Review key points one last time
- [ ] Take deep breaths and stay calm

---

# EVALUATION CRITERIA

## What Blackbaud is Looking For

### **Technical Excellence (40%)**
- ✅ Strong coding skills (clean, efficient, tested)
- ✅ System design thinking (scalability, reliability)
- ✅ .NET expertise (async, DI, EF Core, performance)
- ✅ Problem-solving approach (systematic, thorough)
- ✅ Best practices (SOLID, design patterns, security)

### **Communication (25%)**
- ✅ Explains thinking clearly
- ✅ Asks clarifying questions
- ✅ Discusses trade-offs
- ✅ Listens actively
- ✅ Collaborative approach

### **Cultural Fit (20%)**
- ✅ Mission alignment (passion for social good)
- ✅ Growth mindset (continuous learning)
- ✅ Team player (collaboration, mentoring)
- ✅ Integrity (honesty, accountability)
- ✅ Adaptability (embraces change)

### **Leadership Potential (15%)**
- ✅ Takes initiative
- ✅ Mentors others
- ✅ Makes decisions
- ✅ Handles ambiguity
- ✅ Drives results

---

# COMMON MISTAKES TO AVOID

## Technical Interview

❌ **Don't:**
- Jump into coding without understanding the problem
- Stay silent while coding (think aloud!)
- Give up when stuck (ask for hints)
- Ignore edge cases
- Skip testing your code
- Write messy, unreadable code
- Argue with the interviewer
- Say "I don't know" without trying

✅ **Do:**
- Clarify requirements first
- Discuss approach before coding
- Think aloud and explain reasoning
- Write clean, readable code
- Test with examples
- Handle edge cases
- Ask for hints if stuck
- Be receptive to feedback

## System Design

❌ **Don't:**
- Jump to implementation details
- Ignore requirements
- Design for infinite scale
- Forget about trade-offs
- Ignore interviewer's hints
- Over-engineer the solution

✅ **Do:**
- Start with requirements (functional & non-functional)
- Think high-level first, then drill down
- Discuss trade-offs for every decision
- Consider alternatives
- Engage with interviewer
- Be pragmatic (MVP first)

## Behavioral Interview

❌ **Don't:**
- Give vague, generic answers
- Speak negatively about previous employers
- Take all credit (ignore team contributions)
- Lie or exaggerate
- Forget to ask questions
- Be unprepared for common questions

✅ **Do:**
- Use STAR method (Situation, Task, Action, Result)
- Be specific with examples and numbers
- Give credit to team members
- Be honest about failures and learnings
- Show enthusiasm for Blackbaud's mission
- Ask thoughtful questions

---

# FINAL TIPS FOR SUCCESS

## Before the Interview

1. **Research Thoroughly**
   - Study Blackbaud's products (Raiser's Edge, Financial Edge, etc.)
   - Read their engineering blog
   - Check recent news and press releases
   - Understand their customer base (nonprofits, education)

2. **Practice, Practice, Practice**
   - Code on whiteboard/collaborative tools
   - Do mock interviews with friends
   - Record yourself answering behavioral questions
   - Time yourself on coding problems

3. **Prepare Your Environment**
   - Quiet space with good lighting
   - Stable internet connection
   - Backup plan (phone hotspot)
   - Professional appearance

## During the Interview

1. **First Impressions Matter**
   - Smile and show energy
   - Make eye contact (if video)
   - Firm handshake (if in-person)
   - Show enthusiasm

2. **Communication is Key**
   - Think aloud
   - Ask clarifying questions
   - Explain your reasoning
   - Discuss trade-offs
   - Listen actively

3. **Show Your Best Self**
   - Be confident but humble
   - Be honest about what you don't know
   - Show passion for social good
   - Demonstrate growth mindset
   - Be authentic

4. **Handle Stress Well**
   - Take a breath if you need to think
   - It's okay to say "Let me think about that"
   - Don't panic if you don't know something
   - Ask for hints if stuck

## After the Interview

1. **Follow Up**
   - Send thank-you email within 24 hours
   - Mention specific discussion points
   - Reiterate your interest
   - Keep it brief and professional

2. **Reflect**
   - What went well?
   - What could you improve?
   - What did you learn?
   - What would you do differently?

3. **Be Patient**
   - Response typically takes 1-2 weeks
   - Don't stress about small mistakes
   - Trust your preparation
   - Stay positive

---

# YOU'VE GOT THIS!

## Remember:

✅ **You're qualified** - You wouldn't be at Round 3 if you weren't

✅ **They want you to succeed** - Interviewers are rooting for you

✅ **It's a conversation** - Not an interrogation

✅ **Be yourself** - Authenticity matters

✅ **Show your passion** - Especially for social good

✅ **Learn from it** - Even if it doesn't work out, it's valuable experience

## Key Success Factors:

1. **Technical Excellence** - Solve problems systematically
2. **Clear Communication** - Explain your thinking
3. **Cultural Fit** - Show passion for mission
4. **Growth Mindset** - Demonstrate continuous learning
5. **Authenticity** - Be genuine and honest

## Final Words:

You've prepared thoroughly. You know your stuff. Now go in there with confidence, be yourself, and show them why you're the right person for this role.

**Blackbaud is lucky to have you as a candidate. Now go show them what you've got!**

**Good luck! 🚀**

---

**Last Updated**: July 2026  
**Version**: 1.0  
**Duration**: 3.5 hours  
**Difficulty**: Senior Level

*This mock interview guide covers everything that could be asked in Blackbaud's Round 3 interview for a Senior .NET Developer role.*

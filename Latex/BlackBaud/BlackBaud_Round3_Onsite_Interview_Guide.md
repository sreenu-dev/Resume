# Blackbaud Round 3: Onsite/Virtual Interview Preparation Guide
## Complete Strategy for .NET Developer Role

---

## Table of Contents
1. [Round 3 Overview](#round-3-overview)
2. [Session-by-Session Breakdown](#session-by-session-breakdown)
3. [Technical Deep Dive Preparation](#technical-deep-dive-preparation)
4. [System Design Interview](#system-design-interview)
5. [Behavioral Interview](#behavioral-interview)
6. [Manager Round](#manager-round)
7. [Day-Of Strategy](#day-of-strategy)
8. [Common Mistakes to Avoid](#common-mistakes-to-avoid)
9. [Final Checklist](#final-checklist)

---

## Round 3 Overview

### What is Round 3?
Round 3 is the **final and most comprehensive interview stage** at Blackbaud. It typically lasts **3-4 hours** and consists of **multiple back-to-back sessions** with different team members.

### Format
- **Virtual**: Via Microsoft Teams or Zoom (most common)
- **Onsite**: At Blackbaud office (less common post-COVID)
- **Duration**: 3-4 hours total
- **Sessions**: 4-5 separate interviews

### What They're Evaluating
✅ **Technical Depth**: Can you solve complex problems?  
✅ **System Design**: Can you architect scalable solutions?  
✅ **Cultural Fit**: Will you thrive at Blackbaud?  
✅ **Communication**: Can you explain technical concepts clearly?  
✅ **Leadership Potential**: Can you grow into senior roles?  
✅ **Mission Alignment**: Do you care about social good?

### Success Rate
- Only **20-30%** of candidates reach Round 3
- **60-70%** of Round 3 candidates receive offers
- **Your goal**: Be in that 60-70%!

---

## Session-by-Session Breakdown

### Typical Round 3 Schedule

```
9:00 AM  - Welcome & Overview (15 min)
9:15 AM  - Technical Deep Dive (60 min)
10:15 AM - Break (10 min)
10:25 AM - System Design (45 min)
11:10 AM - Break (10 min)
11:20 AM - Behavioral Interview (45 min)
12:05 PM - Lunch Break (30 min) [Optional]
12:35 PM - Manager Round (30 min)
1:05 PM  - Team Q&A / Wrap-up (20 min)
```

**Note**: Schedule may vary, but expect 3-4 hours of interviews.

---

## Technical Deep Dive Preparation

### Session 1: Technical Deep Dive (60 minutes)

#### **Who Interviews You**
- Senior Software Engineer or Tech Lead
- Someone who will be your peer or mentor

#### **What to Expect**
1. **Advanced Coding Problem** (30 min) - Harder than Round 2
2. **Code Review Exercise** (15 min) - Find bugs and suggest improvements
3. **Technical Deep Dive Questions** (15 min) - .NET internals

#### **Sample Advanced Coding Problems**

**Problem 1: LRU Cache Implementation**
```csharp
/*
Design a data structure for Least Recently Used (LRU) cache.
Implement: Get(key) and Put(key, value) in O(1) time.
*/

public class LRUCache {
    private class Node {
        public int Key { get; set; }
        public int Value { get; set; }
        public Node Prev { get; set; }
        public Node Next { get; set; }
    }
    
    private readonly int _capacity;
    private readonly Dictionary<int, Node> _cache;
    private readonly Node _head;
    private readonly Node _tail;
    
    public LRUCache(int capacity) {
        _capacity = capacity;
        _cache = new Dictionary<int, Node>();
        _head = new Node();
        _tail = new Node();
        _head.Next = _tail;
        _tail.Prev = _head;
    }
    
    public int Get(int key) {
        if (!_cache.ContainsKey(key)) return -1;
        
        var node = _cache[key];
        MoveToHead(node);
        return node.Value;
    }
    
    public void Put(int key, int value) {
        if (_cache.ContainsKey(key)) {
            var node = _cache[key];
            node.Value = value;
            MoveToHead(node);
        } else {
            var newNode = new Node { Key = key, Value = value };
            _cache[key] = newNode;
            AddToHead(newNode);
            
            if (_cache.Count > _capacity) {
                var tail = RemoveTail();
                _cache.Remove(tail.Key);
            }
        }
    }
    
    private void AddToHead(Node node) {
        node.Prev = _head;
        node.Next = _head.Next;
        _head.Next.Prev = node;
        _head.Next = node;
    }
    
    private void RemoveNode(Node node) {
        node.Prev.Next = node.Next;
        node.Next.Prev = node.Prev;
    }
    
    private void MoveToHead(Node node) {
        RemoveNode(node);
        AddToHead(node);
    }
    
    private Node RemoveTail() {
        var node = _tail.Prev;
        RemoveNode(node);
        return node;
    }
}
```

**Problem 2: Merge K Sorted Lists**
```csharp
public class Solution {
    public ListNode MergeKLists(ListNode[] lists) {
        if (lists == null || lists.Length == 0) return null;
        
        var pq = new PriorityQueue<ListNode, int>();
        
        foreach (var list in lists) {
            if (list != null) {
                pq.Enqueue(list, list.val);
            }
        }
        
        var dummy = new ListNode(0);
        var current = dummy;
        
        while (pq.Count > 0) {
            var node = pq.Dequeue();
            current.next = node;
            current = current.next;
            
            if (node.next != null) {
                pq.Enqueue(node.next, node.next.val);
            }
        }
        
        return dummy.next;
    }
}
// Time: O(N log k), Space: O(k)
```

#### **Code Review Exercise**

**BAD CODE - Find all issues:**
```csharp
public class UserService {
    public List<User> GetUsers() {
        var users = new List<User>();
        SqlConnection conn = new SqlConnection("connection_string");
        conn.Open();
        
        SqlCommand cmd = new SqlCommand("SELECT * FROM Users", conn);
        SqlDataReader reader = cmd.ExecuteReader();
        
        while (reader.Read()) {
            User user = new User();
            user.Name = reader["Name"].ToString();
            user.Email = reader["Email"].ToString();
            users.Add(user);
        }
        
        return users;
    }
    
    public void UpdateUser(int id, string name) {
        SqlConnection conn = new SqlConnection("connection_string");
        conn.Open();
        
        string sql = "UPDATE Users SET Name = '" + name + "' WHERE Id = " + id;
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}
```

**Issues to Identify:**
1. ❌ No resource disposal (connection not closed)
2. ❌ SQL Injection vulnerability
3. ❌ No async/await (blocking calls)
4. ❌ Hardcoded connection string
5. ❌ No error handling
6. ❌ Direct SQL in service layer
7. ❌ No null checks
8. ❌ Synchronous I/O

**GOOD CODE - Your improvements:**
```csharp
public interface IUserRepository {
    Task<List<User>> GetUsersAsync();
    Task UpdateUserAsync(int id, string name);
}

public class UserRepository : IUserRepository {
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger) {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<User>> GetUsersAsync() {
        try {
            return await _context.Users
                .Select(u => new User {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error fetching users");
            throw;
        }
    }
    
    public async Task UpdateUserAsync(int id, string name) {
        try {
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                throw new NotFoundException($"User {id} not found");
            }
            
            user.Name = name;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            throw;
        }
    }
}
```

#### **Technical Deep Dive Questions**

**Q1: Explain async/await vs Task.Run()**
```csharp
// Task.Run - Creates new thread (CPU-bound)
var result = await Task.Run(() => ExpensiveCalculation());

// async/await - No new thread (I/O-bound)
var result = await GetDataAsync();

// Key: Use Task.Run for CPU-bound, async/await for I/O-bound
```

**Q2: What happens when you don't await?**
```csharp
// BAD - Fire and forget (exceptions lost!)
public void ProcessData() {
    SaveToDatabase(); // Not awaited!
}

// GOOD - Await the task
public async Task ProcessDataAsync() {
    await SaveToDatabase();
}
```

**Q3: How to prevent memory leaks?**
```csharp
// Leak: Event handlers not unsubscribed
public class Subscriber : IDisposable {
    public Subscriber(Publisher publisher) {
        publisher.DataChanged += OnDataChanged;
    }
    
    public void Dispose() {
        publisher.DataChanged -= OnDataChanged; // Unsubscribe!
    }
}

// Leak: Unclosed resources
// BAD
var stream = File.OpenRead("file.txt");

// GOOD
using var stream = File.OpenRead("file.txt");
```

**Q4: Optimize slow LINQ query**
```csharp
// SLOW - Multiple DB calls
var users = _context.Users.ToList(); // Loads all
var active = users.Where(u => u.IsActive).ToList(); // Filters in memory

// FAST - Single DB call
var active = await _context.Users
    .AsNoTracking() // No tracking overhead
    .Where(u => u.IsActive) // Filters in DB
    .Select(u => new UserDto { ... }) // Projects in DB
    .ToListAsync();
```

**Q5: IEnumerable vs IQueryable**
```csharp
// IEnumerable - In-memory
IEnumerable<Product> products = GetFromMemory();
var expensive = products.Where(p => p.Price > 100); // C# code

// IQueryable - Database
IQueryable<Product> products = _context.Products;
var expensive = products.Where(p => p.Price > 100); // SQL WHERE

// Example:
// BAD
var count = _context.Products.ToList().Count(p => p.Price > 100);

// GOOD
var count = await _context.Products.CountAsync(p => p.Price > 100);
```

---

## System Design Interview

### Session 2: System Design (45 minutes)

#### **Who Interviews You**
- Senior Engineer or Architect

#### **What to Expect**
- Design a scalable system from scratch
- Discuss trade-offs
- Draw architecture diagrams
- Estimate capacity

### **Question 1: Design a Donation Processing System**

**Context**: Blackbaud builds software for nonprofits.

**Requirements:**
- Handle 10,000 donations/day (peak: 1,000/min)
- One-time and recurring donations
- Multiple payment methods
- Generate tax receipts
- Send thank-you emails
- Prevent duplicates
- Handle payment failures

**Your Approach:**

**Step 1: Clarify (5 min)**
```
Ask:
- Donation amount range?
- Real-time or can queue?
- Acceptable latency?
- International payments?
- Data retention policy?
- Audit logs needed?
```

**Step 2: High-Level Architecture (10 min)**

```
┌─────────────┐
│   Client    │ (Web/Mobile)
└──────┬──────┘
       │ HTTPS
       ▼
┌─────────────────┐
│  API Gateway    │ (Rate limiting, Auth)
└────────┬────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌─────────┐ ┌──────────────┐
│ Donation│ │  Payment     │
│ Service │ │  Service     │
└────┬────┘ └──────┬───────┘
     │             │
     │             ▼
     │      ┌──────────────┐
     │      │ Payment      │
     │      │ Gateway      │
     │      └──────────────┘
     │
     ▼
┌─────────────────────────────┐
│     Message Queue           │
│     (Azure Service Bus)     │
└─────────┬───────────────────┘
          │
    ┌─────┴─────┬──────────┐
    ▼           ▼          ▼
┌────────┐ ┌─────────┐ ┌──────────┐
│Receipt │ │  Email  │ │Analytics │
│Service │ │ Service │ │ Service  │
└────────┘ └─────────┘ └──────────┘
```

**Step 3: API Design (10 min)**

```csharp
[ApiController]
[Route("api/donations")]
public class DonationController : ControllerBase {
    [HttpPost]
    public async Task<ActionResult<DonationResponse>> CreateDonation(
        [FromBody] DonationRequest request) {
        
        // 1. Validate
        if (!ModelState.IsValid) return BadRequest();
        
        // 2. Check duplicates (idempotency)
        var existing = await _service.GetByIdempotencyKeyAsync(request.IdempotencyKey);
        if (existing != null) return Ok(existing);
        
        // 3. Create donation (status: Pending)
        var donation = await _service.CreateAsync(request);
        
        // 4. Queue payment processing (async)
        await _queue.PublishAsync("payment-processing", donation);
        
        // 5. Return immediately
        return Accepted(new DonationResponse {
            DonationId = donation.Id,
            Status = "Processing"
        });
    }
}
```

**Step 4: Database Schema (5 min)**

```sql
CREATE TABLE Donations (
    Id BIGINT PRIMARY KEY IDENTITY,
    DonorId BIGINT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'USD',
    PaymentMethod VARCHAR(50) NOT NULL,
    Status VARCHAR(20) NOT NULL, -- Pending, Completed, Failed
    IdempotencyKey VARCHAR(100) UNIQUE NOT NULL,
    TransactionId VARCHAR(100),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    INDEX IX_Donations_DonorId (DonorId),
    INDEX IX_Donations_Status (Status)
);

CREATE TABLE RecurringDonations (
    Id BIGINT PRIMARY KEY IDENTITY,
    DonorId BIGINT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Frequency VARCHAR(20) NOT NULL, -- Monthly, Quarterly
    NextProcessDate DATE NOT NULL,
    IsActive BIT DEFAULT 1,
    
    INDEX IX_RecurringDonations_NextProcessDate (NextProcessDate, IsActive)
);
```

**Step 5: Trade-offs (10 min)**

**Scalability:**
- ✅ Message queue decouples services
- ✅ Async processing handles spikes
- ⚠️ Need to handle queue backlog

**Reliability:**
- ✅ Retry logic for failures
- ✅ Idempotency prevents duplicates
- ⚠️ Need distributed transactions

**Performance:**
- ✅ Immediate response (202 Accepted)
- ✅ Background processing
- ⚠️ Monitor queue depth

---

## Behavioral Interview

### Session 3: Behavioral Interview (45 minutes)

#### **Who Interviews You**
- Engineering Manager or HR

#### **What to Expect**
- STAR method questions
- Cultural fit assessment
- Values alignment

#### **Blackbaud's Core Values**
1. **Integrity**: Do the right thing
2. **Innovation**: Embrace change
3. **Inclusion**: Value diversity
4. **Social Good**: Mission-driven

### **Top 10 Behavioral Questions**

**1. Tell me about yourself (2-minute pitch)**

**Structure:** Present (30s) → Past (60s) → Future (30s)

**Example:**
> "I'm a .NET developer with 5 years of experience building scalable web applications. Currently at [Company], I lead a team of 3 developers working on microservices using .NET Core and Azure. We recently migrated a monolith to microservices, improving performance by 40%.
>
> Before this, I built RESTful APIs serving 10,000+ requests/minute and mentored junior developers.
>
> I'm excited about Blackbaud because I want to use my technical skills for social good. Building software that helps nonprofits is incredibly meaningful to me."

**2. Why Blackbaud?**

**Key Points:**
- Mission alignment (social good)
- Tech stack fit (.NET, Azure)
- Growth opportunities
- Company culture

**Example:**
> "Three reasons: First, the mission resonates with me - my mother worked for a nonprofit, and I saw how technology amplifies impact. Second, the technical challenges excite me - .NET Core, Azure, microservices at scale. Third, the culture emphasizes collaboration and continuous learning, which aligns with my values."

**3. Describe a challenging bug you fixed**

**STAR Example:**

**Situation:** Production API had intermittent 500 errors (5% of requests)

**Task:** Identify root cause without downtime

**Action:**
- Analyzed logs → found SQL deadlocks
- Used SQL Profiler → identified problematic queries
- Refactored stored procedures (consistent lock order)
- Added retry logic with exponential backoff
- Implemented monitoring

**Result:** Errors dropped to 0.01%, response time improved 30%

**4. Tell me about a time you disagreed with a teammate**

**Example:**
> **Situation**: Senior engineer proposed MongoDB for all data
>
> **Task**: Evaluate best database for each use case
>
> **Action**: 
> - Requested technical discussion
> - Prepared comparison (data model, queries, cost, expertise)
> - Proposed hybrid: SQL for transactions, MongoDB for analytics
>
> **Result**: Team agreed on hybrid approach. Senior engineer appreciated respectful, data-driven feedback.

**5. Describe a time you learned something quickly**

**Example:**
> **Situation**: Project required Azure Functions (never used before), 3-week deadline
>
> **Task**: Become proficient to architect solution
>
> **Action**:
> - Days 1-2: Microsoft Learn modules
> - Days 3-4: Built POC
> - Week 2: Paired with experienced colleague
> - Week 3: Implemented solution
>
> **Result**: Delivered on time, became team expert, trained 3 others

**6. Tell me about a time you failed**

**Example:**
> **Situation**: Led feature development, estimated 2 weeks
>
> **Task**: Coordinate 3 developers
>
> **Action**: Made mistakes:
> - Underestimated complexity
> - Didn't break down tasks
> - Didn't communicate risks early
>
> **Result**: Missed deadline by 1 week. Learned to:
> - Add 30% buffer to estimates
> - Break tasks into 1-2 day chunks
> - Communicate risks proactively
> 
> Next 5 projects: hit every deadline, got promoted

**7. How do you handle tight deadlines?**

**Approach:**
1. **Clarify Scope**: MVP vs nice-to-have
2. **Break Down Tasks**: Detailed task list
3. **Prioritize**: Critical path first
4. **Communicate**: Daily updates
5. **Eliminate Distractions**: Block deep work time
6. **Ask for Help**: Don't struggle alone

**8. Describe a time you improved code quality**

**Example:**
> **Situation**: Legacy codebase, 30% test coverage, frequent bugs
>
> **Task**: Improve quality without disrupting development
>
> **Action**:
> - Set up SonarQube
> - Introduced code reviews
> - 100% test coverage for new code
> - Refactored during bug fixes
> - "Refactoring Fridays"
>
> **Result**: 6 months later:
> - 80% test coverage
> - 50% fewer bugs
> - Team velocity increased

**9. Tell me about mentoring someone**

**Example:**
> **Situation**: Junior developer joined, struggled with codebase
>
> **Task**: Mentor for 3 months
>
> **Action**:
> - Week 1: Pair programming
> - Weeks 2-4: Independent work with daily check-ins
> - Month 2: Code reviews with detailed feedback
> - Month 3: Led small feature
>
> **Result**: After 3 months, contributing independently. After 6 months, mentoring next junior.

**10. Why should we hire you?**

**Example:**
> "Three reasons:
>
> **1. Technical Fit**: 5 years with your exact stack - .NET Core, Azure, SQL Server, Angular
>
> **2. Mission Alignment**: Passionate about social good. Volunteered as developer for 2 local charities
>
> **3. Growth Mindset**: Looking for a place to grow, learn from senior engineers, contribute to architecture, mentor others
>
> Plus: I've studied your products, read your blog, talked to employees. Confident I can contribute from day one."

### **Questions to Ask Interviewer**

**About the Role:**
- What does success look like in first 30/60/90 days?
- Biggest challenges facing the team?
- Balance of new features vs maintenance?
- On-call rotation?

**About the Team:**
- Team structure and dynamics?
- How handle technical disagreements?
- Balance of senior/junior developers?

**About Growth:**
- Career progression opportunities?
- Conference attendance or certifications?
- Work on different products/teams?

**About Company:**
- How measure impact on nonprofit clients?
- Technical priorities for next year?
- What makes Blackbaud different?

---

## Manager Round

### Session 4: Manager Round (30 minutes)

#### **Who Interviews You**
- Hiring Manager (your potential boss)
- Director of Engineering

#### **What to Expect**
- Team fit assessment
- Career goals discussion
- Management style alignment

### **Key Questions**

**1. What are you looking for in your next role?**

**Example:**
> "Three things:
> 1. **Technical Growth**: Challenging problems at scale
> 2. **Meaningful Work**: Software that matters
> 3. **Great Team**: Smart, collaborative people who value quality and work-life balance"

**2. Where do you see yourself in 5 years?**

**Example:**
> "Technical leader - senior engineer or tech lead. I want to:
> - Architect complex systems
> - Mentor junior developers
> - Contribute to technical strategy
> - Maybe speak at conferences
>
> Not sure yet about management vs technical track, but I want significant impact."

**3. What's your ideal work environment?**

**Example:**
> "I thrive in environments that:
> - Value code quality
> - Encourage experimentation
> - Have clear communication
> - Balance autonomy with collaboration
> - Respect work-life balance
>
> I work best with clear goals, regular feedback, trust to make decisions, and support when needed."

**4. How do you prefer to receive feedback?**

**Example:**
> "Direct, honest feedback ASAP. I prefer:
> - Specific (not 'do better' but 'here's what to improve')
> - Actionable (what should I do differently?)
> - Balanced (what am I doing well?)
>
> I like regular check-ins (weekly/bi-weekly) vs waiting for annual reviews."

---

## Day-Of Strategy

### **1 Week Before**

✅ **Technical Prep:**
- Review all coding problems
- Practice system design
- Review .NET concepts

✅ **Research:**
- Read Blackbaud blog
- Study their products
- Check recent news

✅ **Logistics:**
- Test video/audio
- Set up collaborative coding tools
- Prepare workspace

### **Day Before**

✅ **Final Review:**
- Review STAR stories
- Practice elevator pitch
- Review questions to ask

✅ **Logistics:**
- Charge devices
- Test internet connection
- Prepare professional attire
- Get good sleep!

### **Morning Of**

✅ **Preparation:**
- Eat a good breakfast
- Arrive/login 10 minutes early
- Have water nearby
- Have pen and paper ready

### **During Interview**

✅ **For Each Session:**
- **First 30 seconds**: Make good impression (smile, energy)
- **Listen carefully**: Take notes
- **Think aloud**: Explain your reasoning
- **Ask questions**: Show curiosity
- **Be authentic**: Be yourself

✅ **Between Sessions:**
- Take breaks seriously
- Stretch, walk around
- Drink water
- Review notes from previous session

### **After Interview**

✅ **Same Day:**
- Send thank-you email to recruiter
- Mention specific discussion points
- Reiterate interest

✅ **Follow-up:**
- Be patient (1-2 weeks for response)
- Don't stress about mistakes
- Reflect on what went well

---

## Common Mistakes to Avoid

### **Technical Interview**

❌ **Don't:**
- Jump into coding without understanding problem
- Stay silent while coding
- Give up when stuck
- Ignore edge cases
- Skip testing your code
- Argue with interviewer

✅ **Do:**
- Clarify requirements first
- Think aloud
- Ask for hints if stuck
- Consider edge cases
- Test with examples
- Be receptive to feedback

### **System Design**

❌ **Don't:**
- Jump to implementation details
- Ignore requirements
- Design for infinite scale
- Forget about trade-offs
- Ignore interviewer's hints

✅ **Do:**
- Start with requirements
- Think high-level first
- Discuss trade-offs
- Consider alternatives
- Engage with interviewer

### **Behavioral Interview**

❌ **Don't:**
- Give vague answers
- Speak negatively about previous employers
- Take credit for team's work
- Lie or exaggerate
- Forget to ask questions

✅ **Do:**
- Use STAR method
- Be specific with examples
- Give credit to team
- Be honest
- Show enthusiasm

---

## Final Checklist

### **Technical Skills**
- [ ] Can solve Medium LeetCode problems
- [ ] Comfortable with advanced .NET concepts
- [ ] Can design scalable systems
- [ ] Know common design patterns
- [ ] Understand async/await deeply

### **Behavioral Prep**
- [ ] Prepared 5-7 STAR stories
- [ ] Practiced elevator pitch
- [ ] Have questions ready for each interviewer
- [ ] Researched Blackbaud thoroughly

### **Logistics**
- [ ] Video/audio tested
- [ ] Collaborative coding tools ready
- [ ] Workspace prepared (quiet, good lighting)
- [ ] Professional attire ready
- [ ] Pen and paper nearby

### **Mindset**
- [ ] Well-rested
- [ ] Confident but humble
- [ ] Authentic
- [ ] Enthusiastic about mission
- [ ] Ready to learn

---

## Key Success Factors

### **1. Technical Excellence**
- Solve problems systematically
- Write clean, readable code
- Explain your reasoning
- Consider edge cases

### **2. Communication**
- Think aloud
- Ask clarifying questions
- Explain trade-offs
- Listen actively

### **3. Cultural Fit**
- Show passion for social good
- Demonstrate collaboration
- Exhibit growth mindset
- Be authentic

### **4. Preparation**
- Research the company
- Practice coding problems
- Prepare STAR stories
- Have thoughtful questions

### **5. Professionalism**
- Be punctual
- Dress appropriately
- Send thank-you notes
- Follow up professionally

---

## Remember

✅ **They want you to succeed** - Interviewers are rooting for you

✅ **It's a conversation** - Not an interrogation

✅ **Show your thinking** - Process matters more than perfect answer

✅ **Ask questions** - Shows curiosity and engagement

✅ **Be yourself** - Authenticity matters

✅ **Learn from it** - Even if it doesn't work out, it's valuable experience

---

## Final Tips

### **Technical:**
- Practice coding on whiteboard/collaborative tools
- Review .NET internals (GC, async, LINQ)
- Study system design patterns
- Understand Blackbaud's tech stack

### **Behavioral:**
- Prepare specific, detailed examples
- Focus on YOUR contributions
- Show impact with numbers
- Demonstrate learning from failures

### **Cultural:**
- Express genuine interest in social good
- Show alignment with Blackbaud's values
- Demonstrate collaboration skills
- Exhibit growth mindset

---

## You've Got This!

Round 3 is challenging, but you're prepared. Remember:

- **Technical skills** got you here
- **Communication** will set you apart
- **Cultural fit** will seal the deal
- **Authenticity** will make you memorable

**Trust your preparation. Be confident. Be yourself. Good luck! 🚀**

---

**Last Updated**: July 2026  
**Version**: 1.0

*This guide is based on publicly available information and interview experiences. Actual interview content may vary.*

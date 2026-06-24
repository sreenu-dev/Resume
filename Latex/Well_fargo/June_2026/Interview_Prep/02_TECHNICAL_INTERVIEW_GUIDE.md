# Technical Interview Guide for Wells Fargo

## 📋 Overview

This guide covers everything you need to know about technical interviews at Wells Fargo, including coding problems, system design, and how to approach technical questions.

---

## 🎯 Technical Interview Types

### Type 1: Coding Problem Interview
**Duration:** 45-60 minutes
**Difficulty:** LeetCode Medium to Hard
**Focus:** Data structures, algorithms, code quality

### Type 2: System Design Interview
**Duration:** 60 minutes
**Difficulty:** Complex system design
**Focus:** Architecture, scalability, trade-offs

### Type 3: Architecture Discussion
**Duration:** 60 minutes
**Difficulty:** Deep technical knowledge
**Focus:** Your projects, design decisions

---

## 💻 Coding Problem Interview

### What to Expect
1. **Problem Statement** (5 minutes)
   - Interviewer explains the problem
   - You ask clarifying questions

2. **Solution Development** (35-45 minutes)
   - You write code on a shared platform
   - Interviewer may ask follow-up questions
   - You test your solution

3. **Optimization & Discussion** (5-10 minutes)
   - Discuss time and space complexity
   - Discuss potential optimizations
   - Answer follow-up questions

### Problem Categories

#### 1. Arrays & Strings
- Two pointer problems
- Sliding window
- Prefix sum
- Examples:
  - "Find two numbers that add up to target"
  - "Longest substring without repeating characters"
  - "Rotate array"

#### 2. Linked Lists
- Reversal
- Cycle detection
- Merging
- Examples:
  - "Reverse a linked list"
  - "Detect cycle in linked list"
  - "Merge two sorted linked lists"

#### 3. Trees & Graphs
- Traversal (BFS, DFS)
- Path finding
- Tree construction
- Examples:
  - "Binary tree level order traversal"
  - "Lowest common ancestor"
  - "Number of islands"

#### 4. Dynamic Programming
- Optimization problems
- Sequence problems
- Examples:
  - "Longest increasing subsequence"
  - "Coin change problem"
  - "House robber"

#### 5. Hash Tables & Sets
- Frequency counting
- Duplicate detection
- Examples:
  - "Two sum"
  - "Valid anagram"
  - "Group anagrams"

#### 6. Stacks & Queues
- Expression evaluation
- Monotonic stack
- Examples:
  - "Valid parentheses"
  - "Daily temperatures"
  - "Largest rectangle in histogram"

### How to Approach a Coding Problem

#### Step 1: Understand the Problem (5 minutes)
```
1. Read the problem carefully
2. Ask clarifying questions:
   - What are the input constraints?
   - What's the expected output format?
   - Are there any edge cases?
   - What's the time/space complexity requirement?
3. Provide examples:
   - Example input and output
   - Edge cases
```

#### Step 2: Discuss Your Approach (5 minutes)
```
1. Explain your approach:
   - What data structures will you use?
   - What's your algorithm?
   - What's the time complexity?
   - What's the space complexity?
2. Ask for feedback:
   - Does this approach make sense?
   - Any suggestions?
```

#### Step 3: Write Code (30-40 minutes)
```
1. Write clean, readable code:
   - Use meaningful variable names
   - Add comments for complex logic
   - Follow language conventions
2. Handle edge cases:
   - Empty input
   - Single element
   - Large input
3. Test your solution:
   - Test with provided examples
   - Test with edge cases
   - Trace through your code
```

#### Step 4: Optimize (5-10 minutes)
```
1. Discuss optimization:
   - Can you reduce time complexity?
   - Can you reduce space complexity?
   - Are there trade-offs?
2. Implement optimization:
   - If time permits
   - If interviewer asks
```

#### Step 5: Answer Follow-up Questions
```
1. Be ready for:
   - "Can you optimize this further?"
   - "What if we had this constraint?"
   - "How would you test this?"
   - "What about error handling?"
```

### Coding Problem Examples

#### Example 1: Two Sum
```
Problem: Given an array of integers, find two numbers that add up to a target.
Input: [2, 7, 11, 15], target = 9
Output: [0, 1] (indices of 2 and 7)

Approach:
1. Use hash map to store seen numbers
2. For each number, check if (target - number) exists
3. Return indices when found

Time Complexity: O(n)
Space Complexity: O(n)
```

#### Example 2: Longest Substring Without Repeating Characters
```
Problem: Find the length of longest substring without repeating characters.
Input: "abcabcbb"
Output: 3 (substring "abc")

Approach:
1. Use sliding window with hash map
2. Expand window by moving right pointer
3. Shrink window when duplicate found
4. Track maximum length

Time Complexity: O(n)
Space Complexity: O(min(m, n)) where m is charset size
```

#### Example 3: Binary Tree Level Order Traversal
```
Problem: Return the level order traversal of a binary tree.
Input: Tree with root 3, left child 9, right child 20
Output: [[3], [9, 20], [15, 7]]

Approach:
1. Use BFS with queue
2. Process one level at a time
3. Add children to queue for next level
4. Return list of levels

Time Complexity: O(n)
Space Complexity: O(w) where w is max width
```

### Common Mistakes to Avoid

❌ **Don't:**
- Rush into coding without understanding the problem
- Write messy code that's hard to read
- Ignore edge cases and error handling
- Forget to test your solution
- Not discuss time and space complexity
- Be defensive about feedback
- Memorize solutions without understanding

✅ **Do:**
- Ask clarifying questions
- Think out loud
- Write clean, readable code
- Test with examples and edge cases
- Discuss complexity analysis
- Be open to feedback
- Understand the concepts

### Practice Resources

#### LeetCode Problems to Practice
**Easy (Warm-up):**
- Two Sum
- Valid Parentheses
- Merge Two Sorted Lists
- Invert Binary Tree
- Palindrome Number

**Medium (Main Practice):**
- Add Two Numbers
- Longest Substring Without Repeating Characters
- Container With Most Water
- 3Sum
- Binary Tree Level Order Traversal
- Lowest Common Ancestor of BST
- Number of Islands
- Coin Change
- House Robber

**Hard (Advanced):**
- Median of Two Sorted Arrays
- Trapping Rain Water
- Merge K Sorted Lists
- Serialize and Deserialize Binary Tree

#### Practice Schedule
- **Week 1:** 5 easy problems + 5 medium problems
- **Week 2:** 10 medium problems
- **Week 3:** 5 medium + 3 hard problems
- **Week 4:** 5 medium + 5 hard problems + mock interviews

---

## 🏗️ System Design Interview

### What to Expect
1. **Problem Statement** (5 minutes)
   - Interviewer describes a system to design
   - You ask clarifying questions

2. **High-Level Design** (15-20 minutes)
   - Discuss overall architecture
   - Identify major components
   - Discuss data flow

3. **Deep Dive** (20-30 minutes)
   - Discuss specific components
   - Address scalability concerns
   - Discuss trade-offs

4. **Discussion & Follow-up** (5-10 minutes)
   - Answer follow-up questions
   - Discuss optimization
   - Handle edge cases

### System Design Process

#### Step 1: Clarify Requirements (5 minutes)
```
Functional Requirements:
- What should the system do?
- What are the main features?
- What are the user workflows?

Non-Functional Requirements:
- How many users?
- How many requests per second?
- Latency requirements?
- Availability requirements?
- Consistency requirements?

Example:
"Design a payment processing system"
- Process 10,000 transactions per second
- 99.99% availability
- Sub-second latency
- Strong consistency for financial data
```

#### Step 2: Estimate Scale (5 minutes)
```
1. Estimate users:
   - Daily active users
   - Peak concurrent users

2. Estimate traffic:
   - Requests per second
   - Data per request

3. Estimate storage:
   - Data per user
   - Total data size

Example:
- 1 million DAU
- 100,000 concurrent users
- 10,000 transactions per second
- 1 TB total data
```

#### Step 3: High-Level Architecture (10-15 minutes)
```
Components:
1. Client Layer
   - Web, mobile, API clients

2. API Gateway
   - Request routing
   - Rate limiting
   - Authentication

3. Service Layer
   - Business logic services
   - Microservices

4. Data Layer
   - Databases
   - Caches
   - Message queues

5. External Services
   - Payment gateways
   - Notification services
```

#### Step 4: Deep Dive (20-30 minutes)
```
1. Database Design
   - Schema design
   - Indexing strategy
   - Partitioning strategy

2. Caching Strategy
   - What to cache?
   - Cache invalidation
   - Cache hit rate

3. Scalability
   - Horizontal scaling
   - Load balancing
   - Database replication

4. Reliability
   - Fault tolerance
   - Backup and recovery
   - Monitoring and alerting

5. Security
   - Authentication and authorization
   - Data encryption
   - API security
```

#### Step 5: Trade-offs & Optimization (5-10 minutes)
```
1. Discuss trade-offs:
   - Consistency vs Availability
   - Latency vs Throughput
   - Cost vs Performance

2. Optimization:
   - Can we reduce latency?
   - Can we increase throughput?
   - Can we reduce cost?
```

### System Design Examples

#### Example 1: Payment Processing System
```
Requirements:
- 10,000 transactions per second
- 99.99% availability
- Sub-second latency
- Strong consistency

Architecture:
1. API Gateway
   - Request routing
   - Rate limiting

2. Payment Service
   - Process transactions
   - Validate payments

3. Ledger Service
   - Record transactions
   - Maintain balance

4. Notification Service
   - Send confirmations
   - Send alerts

5. Database
   - PostgreSQL for transactions
   - Redis for caching
   - Kafka for event streaming

Scalability:
- Horizontal scaling of services
- Database replication
- Cache layer for hot data
- Event-driven architecture
```

#### Example 2: Distributed Ledger
```
Requirements:
- Immutable transaction log
- Strong consistency
- High throughput
- Fault tolerance

Architecture:
1. Consensus Layer
   - Raft or PBFT
   - Leader election

2. Transaction Layer
   - Transaction validation
   - Transaction ordering

3. Storage Layer
   - Immutable log
   - State database

4. Replication Layer
   - Multi-node replication
   - Backup and recovery

Scalability:
- Sharding by account
- Parallel processing
- Batch processing
```

### Common System Design Questions

#### Financial Systems
1. "Design a payment processing system"
2. "Design a distributed ledger"
3. "Design a stock trading platform"
4. "Design a loan management system"

#### Infrastructure
1. "Design a rate limiting system"
2. "Design a load balancer"
3. "Design a cache system"
4. "Design a message queue"

#### Data Systems
1. "Design a database"
2. "Design a search engine"
3. "Design a logging system"
4. "Design a monitoring system"

### System Design Tips

✅ **Do:**
- Ask clarifying questions
- Discuss trade-offs
- Consider scalability
- Discuss failure scenarios
- Explain your reasoning
- Draw diagrams
- Discuss monitoring and alerting

❌ **Don't:**
- Jump to solutions
- Ignore constraints
- Forget about edge cases
- Be inflexible
- Ignore feedback
- Over-engineer the solution
- Forget about operations

---

## 🔍 Architecture Discussion

### What to Expect
1. **Project Overview** (10 minutes)
   - Describe your project
   - Explain the problem it solves

2. **Technical Deep Dive** (30-40 minutes)
   - Architecture and design decisions
   - Technology choices
   - Trade-offs and alternatives

3. **Challenges & Solutions** (10-15 minutes)
   - Challenges you faced
   - How you solved them
   - What you learned

4. **Discussion & Follow-up** (5-10 minutes)
   - Answer follow-up questions
   - Discuss improvements

### How to Prepare

#### Document Your Projects
```
For each major project:
1. Problem Statement
   - What problem did you solve?
   - Why was it important?

2. Architecture
   - What was the architecture?
   - Why did you choose this?

3. Technology Stack
   - What technologies did you use?
   - Why these technologies?

4. Challenges
   - What challenges did you face?
   - How did you solve them?

5. Results
   - What was the impact?
   - What metrics improved?

6. What You Learned
   - What did you learn?
   - What would you do differently?
```

#### Example Project Description
```
Project: Distributed Event-Driven Ledger

Problem:
- Need to process 20,000 transactions per second
- Ensure no data loss
- Maintain strong consistency

Architecture:
- .NET Core microservices
- Apache Kafka for event streaming
- PostgreSQL for persistent storage
- Redis for caching

Key Components:
1. Transaction Service
   - Validates transactions
   - Publishes events

2. Ledger Service
   - Processes events
   - Updates ledger
   - Maintains consistency

3. Cache Layer
   - Redis for hot data
   - Distributed locking

Results:
- Achieved 20,000 TPS
- Zero data loss
- Sub-second latency
- 99.99% uptime

What I Learned:
- Event-driven architecture benefits
- Distributed system challenges
- Importance of monitoring
```

---

## 📊 Technical Interview Checklist

### Before Interview:
- [ ] Review data structures and algorithms
- [ ] Practice 20-30 coding problems
- [ ] Study system design patterns
- [ ] Practice 5-10 system design problems
- [ ] Document your projects
- [ ] Do 3-5 mock interviews
- [ ] Review your resume

### During Interview:
- [ ] Ask clarifying questions
- [ ] Think out loud
- [ ] Write clean code
- [ ] Test your solution
- [ ] Discuss trade-offs
- [ ] Be open to feedback

### After Interview:
- [ ] Send thank you email
- [ ] Reflect on your performance
- [ ] Note areas for improvement
- [ ] Continue practicing

---

## 🚀 Next Steps

1. **Practice coding problems**
   - Start with LeetCode easy problems
   - Progress to medium problems
   - Do 20-30 problems total

2. **Study system design**
   - Read system design books
   - Practice 5-10 problems
   - Do mock interviews

3. **Document your projects**
   - Write detailed descriptions
   - Practice explaining them
   - Be ready for deep dives

4. **Do mock interviews**
   - Practice with friends
   - Use Pramp or Interviewing.io
   - Get feedback and improve

---

**Created:** 2026-06-24
**Version:** 1.0
**Status:** Ready to Use

**Next Step:** Read 03_BEHAVIORAL_INTERVIEW_GUIDE.md

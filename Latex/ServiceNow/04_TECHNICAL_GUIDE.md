# Technical Interview Guide — ServiceNow (Coding + Enterprise System Design)

## ServiceNow Coding Interview — What's Different

| Dimension      | Amazon             | Google                     | JPMC                   | **ServiceNow**                                                   |
| ----------------| --------------------| ----------------------------| ------------------------| ------------------------------------------------------------------|
| Language       | Any                | Python/Java                | Java preferred         | **Java or Python**                                               |
| Difficulty     | Medium–Hard        | Hard                       | Medium–Hard            | **Medium**                                                       |
| Code style     | Correct + optimal  | Correct + optimal + tested | Correct + secure       | **Correct + clean + enterprise-ready**                           |
| Follow-ups     | "Optimize further" | "How would you optimize?"  | "What could go wrong?" | **"How would you test this? How would it scale to enterprise?"** |
| Domain context | None               | None                       | Finance                | **Enterprise workflows**                                         |
| Testing        | Rare               | Always                     | Sometimes              | **Always asked**                                                 |

---

## ServiceNow's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern | ServiceNow Follow-up |
|---|---|---|---|
| 1 | Two Sum | HashMap | How would you test this? |
| 2 | Valid Parentheses | Stack | How would you handle millions of requests? |
| 3 | LRU Cache | DLL + HashMap | How would you make it multi-tenant? |
| 4 | Merge Intervals | Sorting | How would you test edge cases? |
| 5 | Number of Islands | DFS/BFS | How would you scale to enterprise? |
| 6 | Binary Tree Level Order | BFS | What's the memory cost? |
| 7 | Longest Substring Without Repeating | Sliding Window | How would you test this? |
| 8 | Design Rate Limiter | System Design | How would this work for 1000s of tenants? |
| 9 | Serialize/Deserialize Binary Tree | BFS | How would you handle failures? |
| 10 | Clone Graph | BFS + HashMap | How would this scale? |
| 11 | Find Median from Data Stream | Two Heaps | How would you test edge cases? |
| 12 | Course Schedule | Topological Sort | How would you test this? |
| 13 | Design HashMap | Hashing | How would you make it thread-safe? |
| 14 | Word Ladder | BFS | What's the time complexity? |
| 15 | Kth Largest Element | Heap | How would you handle duplicates? |

### ServiceNow-Specific Coding Variants
ServiceNow may add enterprise workflow context:

```
Standard: "Design an LRU Cache"
ServiceNow variant: "Design a workflow step result cache for a ServiceNow workflow engine.
                    Each workflow step result must be cached per tenant (customer), 
                    and the cache must be isolated between tenants.
                    How would you design the cache key structure?"

Standard: "Validate parentheses"
ServiceNow variant: "Write a parser that validates a ServiceNow workflow definition 
                    expressed in JSON — ensuring all start/end nodes are properly 
                    paired and no cyclic dependencies exist."
```

---

## Enterprise System Design — 5 Deep Dives

### Problem 1: Design a Workflow Engine for Enterprise Automation

**Context:** ServiceNow's core product is a workflow engine that automates enterprise processes (ITSM, HR, Finance).

**Requirements:**
```
Functional:
  - Define workflows as a series of steps (sequential, parallel, conditional)
  - Execute workflows triggered by events (incident created, form submitted)
  - Support human approval steps (wait for human input)
  - Handle failures and retries
  - Track workflow state and history

Non-Functional:
  - Multi-tenant: each enterprise customer has isolated workflows
  - 10,000 concurrent workflow executions
  - < 500ms step execution latency
  - 99.99% availability
  - Full audit trail of every workflow step
```

**Architecture:**
```
Workflow Definition Store:
  - Each workflow is a DAG (Directed Acyclic Graph) of steps
  - Stored as JSON/YAML in database
  - Versioned for backward compatibility

Workflow Trigger Service:
  - Listens to events (Kafka topics per event type)
  - Instantiates workflow execution on event match
  - Publishes WorkflowStarted event

Workflow Execution Engine (core):
  - State machine: PENDING → RUNNING → WAITING → COMPLETE → FAILED
  - Step executor: fetch next step, execute, publish result
  - Handles: sequential, parallel, conditional branching
  - Persists state after every step (event sourcing)

Step Executor Types:
  - Automated: API call, DB update, email send
  - Human approval: wait for user action (webhook callback)
  - AI agent: invoke Now Assist model
  - Integration: call external system via IntegrationHub

Audit Log:
  - Every step execution logged: step_id, tenant_id, timestamp, result
  - Immutable, append-only
  - Queryable for compliance reporting

Multi-Tenant Isolation:
  - tenant_id on every record
  - Row-level security in database
  - Separate Kafka topics per tenant (or partition by tenant_id)
```

**Key Design Points:**
- **State machine** — clear state transitions prevent invalid states
- **Event sourcing** — persist state as events for auditability and replay
- **Human approval steps** — use a callback mechanism (webhook or polling)
- **Multi-tenancy** — tenant_id on every record, never mix data across tenants
- **Retry logic** — exponential backoff for failed automated steps

**PACT Trust Question You'll Hear:**
> "What happens if the workflow engine crashes mid-execution?"

**Answer:**
```
"Because I'm using event sourcing, every completed step is persisted 
as an immutable event before moving to the next step. If the engine 
crashes, it restarts and replays from the last committed step — 
no work is lost and no step is executed twice (idempotency keys prevent 
duplicate execution). This guarantees exactly-once semantics across failures."
```

---

### Problem 2: Design a Multi-Tenant Notification System

**Context:** ServiceNow needs to send notifications (email, SMS, push) to enterprise users on behalf of thousands of customers.

**Requirements:**
```
Functional:
  - Send email, SMS, push notifications
  - Support templates per tenant (customer-branded)
  - Priority levels (critical, high, normal, low)
  - Delivery tracking and receipt confirmation

Non-Functional:
  - 1 million notifications per day
  - Critical notifications delivered in < 10 seconds
  - Multi-tenant: each customer has isolated templates and preferences
  - Zero data leakage between tenants
```

**Architecture:**
```
Notification Request API:
  - Accepts notification request with tenant_id, user_id, template_id, priority
  - Validates request
  - Publishes to Kafka topic: "notifications-{priority}"

Notification Processor (Kafka consumers):
  - Separate consumers per priority level
  - Resolves template for tenant (from Template Store)
  - Resolves user preferences (from Preference Store)
  - Routes to appropriate channel (email/SMS/push)

Channel Adapters:
  - Email: SendGrid or SES
  - SMS: Twilio
  - Push: Firebase Cloud Messaging
  - Each adapter handles retries and delivery tracking

Delivery Tracking:
  - Track: SENT → DELIVERED → READ (for email: open tracking pixel)
  - Store delivery status per notification
  - Webhook from channel provider updates status

Multi-Tenant Isolation:
  - Template Store: templates partitioned by tenant_id
  - Preference Store: user preferences partitioned by tenant_id
  - Audit log: every notification logged with tenant_id
```

---

### Problem 3: Design a Ticketing / Incident Management System

**Context:** ServiceNow's ITSM product — the original core product.

**Requirements:**
```
Functional:
  - Create, update, close tickets (incidents, requests, changes)
  - Assign tickets to agents
  - Track SLAs (resolution time targets)
  - Auto-route tickets by category/priority
  - Full history and audit trail

Non-Functional:
  - Multi-tenant: each enterprise customer is isolated
  - 100,000 tickets per day
  - SLA breach alerts in real-time
  - 99.99% availability
```

**Architecture:**
```
Ticket Service:
  - CRUD for tickets
  - State machine: NEW → ASSIGNED → IN_PROGRESS → RESOLVED → CLOSED
  - Publishes TicketCreated, TicketUpdated events to Kafka

SLA Engine:
  - Subscribes to ticket events
  - Calculates SLA deadline per ticket (based on priority + tenant config)
  - Monitors for SLA breach: scheduled job scans for tickets approaching deadline
  - Publishes SLABreaching / SLABreached events

Assignment Engine:
  - Auto-routes tickets by category and priority
  - Assigns to agent with least workload and relevant skills
  - Sends notification to assigned agent

Audit Log:
  - Every state change logged immutably
  - Full history queryable by tenant admins
  - Exportable for compliance reporting

Search & Analytics:
  - Elasticsearch index for full-text ticket search
  - Real-time dashboard: open tickets by priority, SLA compliance rate
  - Reports: MTTR (Mean Time to Resolve), ticket volume trends
```

---

### Problem 4: Design an AI Agent for Workflow Automation (Now Assist)

**Context:** ServiceNow's biggest bet — AI agents that automate enterprise workflows.

**Requirements:**
```
Functional:
  - AI agent automatically resolves common IT incidents
  - Routes complex incidents to human agents with context summary
  - Learns from past resolutions
  - Integrates with enterprise tools (Jira, Slack, Azure AD)

Non-Functional:
  - < 2 second response time for auto-resolution
  - 99.9% availability
  - Audit trail of every AI decision
  - Human override at any point
```

**Architecture:**
```
Incident Event (from Ticketing System)
  → AI Agent Orchestrator (LangGraph / multi-agent):
      - Intake Agent: classify incident, extract entities
      - Knowledge Agent: search knowledge base for known solutions
      - Resolution Agent: attempt automated fix (if known solution exists)
      - Escalation Agent: if can't resolve, prepare context summary for human
  → Tool Integrations:
      - Restart service (via API to infrastructure)
      - Reset password (via Azure AD API)
      - Provision access (via IAM API)
      - Query monitoring (via Datadog/Splunk API)
  → Human-in-the-Loop:
      - If AI confidence < threshold → escalate to human
      - Human sees: AI's analysis + attempted actions + recommendation
      - Human approves or overrides
  → Audit Log:
      - Every AI decision logged: action, confidence, reasoning
      - Required for compliance: "why did the AI do this?"
```

**Your Differentiator:** This is almost exactly your Agentic AI / LangGraph project. When asked about this, you can say:

> "I've actually built this pattern in production. At Deloitte, I built a multi-agent compliance pipeline using LangGraph that architecturally mirrors ServiceNow's Now Assist approach. The intake → knowledge → resolution → escalation pattern is essentially the same agentic loop I implemented."

---

### Problem 5: Design an Enterprise Integration Platform

**Context:** ServiceNow's IntegrationHub — connecting ServiceNow to hundreds of enterprise tools.

**Requirements:**
```
Functional:
  - Connect to external systems (Jira, Salesforce, Azure AD, SAP, etc.)
  - Trigger integrations from workflows
  - Handle authentication (OAuth2, API keys, certificates)
  - Retry failed integrations
  - Log all integration activity

Non-Functional:
  - Support 500+ integration types
  - Multi-tenant: each customer has their own integration credentials
  - < 1 second integration execution (for synchronous calls)
  - 99.9% availability
```

**Architecture:**
```
Integration Request (from Workflow Engine)
  → Integration Router:
      - Looks up integration type + tenant credentials
      - Routes to appropriate connector
  → Connector Library (500+ connectors):
      - Each connector: authenticate, call API, transform response
      - Handles rate limiting per external system
      - Retries with exponential backoff
  → Credential Store (encrypted):
      - Per-tenant credentials
      - Rotated regularly
      - Never logged in plain text
  → Audit Log:
      - Every integration call logged
      - Request/response (with PII masked)
      - Success/failure + retry history
```

---

## Java Code Quality — What ServiceNow Expects

### OOP + SOLID Principles
```java
// ✅ Interface-driven design for extensibility
public interface WorkflowStep {
    StepResult execute(WorkflowContext context);
    boolean canExecute(WorkflowContext context);
}

// ✅ Each step type is a separate class
public class ApprovalStep implements WorkflowStep {
    @Override
    public StepResult execute(WorkflowContext context) {
        // wait for human approval via callback
    }
}

public class AutomatedStep implements WorkflowStep {
    @Override
    public StepResult execute(WorkflowContext context) {
        // execute automated action
    }
}
```

### Multi-Tenant Safety
```java
// ✅ Always include tenantId in all queries and operations
public List<Ticket> getTickets(String tenantId, TicketFilter filter) {
    Objects.requireNonNull(tenantId, "tenantId must not be null");
    return ticketRepository.findByTenantIdAndFilter(tenantId, filter);
}

// ❌ NEVER do this — exposes all tenants' data
public List<Ticket> getAllTickets(TicketFilter filter) {
    return ticketRepository.findAll(); // SECURITY BUG: cross-tenant data exposure
}
```

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding + Java Fundamentals
  Mon: 5 medium problems in Java (Two Sum, Valid Parentheses, BFS)
  Tue: 5 medium problems (Merge Intervals, Sliding Window)
  Wed: LRU Cache + Design HashMap
  Thu: 5 medium problems (Graphs, Topological Sort)
  Fri: Full mock (90 min, 2 problems)
  Sat: Review + fix
  Sun: Rest

Week 2: System Design + Enterprise Thinking
  Mon: Design Workflow Engine (60 min, write it out)
  Tue: 5 medium problems (Heaps, Stacks)
  Wed: Design Multi-Tenant Notification System (60 min)
  Thu: Design Ticketing / Incident System (60 min)
  Fri: Full coding mock (45 min, 1 problem)
  Sat: Review
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (1 coding + 1 SD)
  Tue: Design AI Agent for Workflow Automation (60 min)
  Wed: 5 hard problems (Median from Stream, Word Ladder)
  Thu: Full behavioral mock (45 min, 5 PACT questions)
  Fri: Design Enterprise Integration Platform (60 min)
  Sat: Final review
  Sun: Rest
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

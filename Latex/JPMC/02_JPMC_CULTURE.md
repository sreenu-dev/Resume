# JPMC Culture & Values — The Complete Guide

## Who JPMorgan Chase Actually Is

JPMC is not a traditional bank that happens to have IT. Under CEO Jamie Dimon, it has become one of the **largest technology employers in the world** — with 60,000+ technologists globally and a $17 billion annual technology budget. Their India Engineering hubs in Hyderabad and Bangalore house thousands of engineers doing production-grade work, not support roles.

> **Jamie Dimon on JPMC as a tech company:** *"We have more software engineers than Google or Amazon."* (This is debated, but the investment is real and massive.)

---

## JPMC's "How We Do Business" Principles

This is JPMC's cultural framework — the equivalent of Amazon's Leadership Principles or Microsoft's Growth Mindset. It has **5 core principles** that interviewers use to assess behavioral fit.

### Principle 1: Exceptional Client Service
> *Do right by the client. Serve them with excellence, honesty, and integrity.*

**What this means for engineers:**
- The "client" includes both external customers (Chase bank customers) and internal clients (trading desks, product teams that consume your API)
- Every system you build ultimately serves someone — know who that is
- Reliability = client trust. Downtime = broken trust

**Interview signal:** Talk about who your technical decisions serve. Name your users.

**Your Story Hook:**
- 50,000+ users impacted by the 70% error reduction
- 99.9% uptime directly protects user-facing services
- WCAG compliance expanded accessible user base
- 300+ defects resolved → better customer experience

---

### Principle 2: Operational Excellence
> *Do it right, every time. Risk management, quality, and continuous improvement.*

**What this means for engineers:**
- Code quality, testing, and zero-defect mindset
- Proactive risk identification — find problems before they find customers
- Post-mortems and blameless retrospectives
- Automation to eliminate human error (CI/CD, automated testing, alerting)

**This is where your resume is naturally strongest for JPMC.**

**Your Story Hook:**
- Automated security scanning in CI/CD
- Zero regression failures through test frameworks
- OpenTelemetry structured logger for observability
- Agentic monitoring assistant for proactive incident prevention
- Distributed Ledger's 99.99% data consistency guarantees

---

### Principle 3: A Commitment to Integrity, Fairness, and Responsibility
> *Do the right thing, even when it is hard. Transparency and ethical behavior.*

**What this means for engineers:**
- Transparency in design decisions and trade-offs
- Not cutting security corners under delivery pressure
- Raising concerns when you see risk — even if inconvenient
- Protecting customer data as a non-negotiable

**Your Story Hook:**
- Pushed back on skipping test coverage under sprint pressure
- Implemented security hardening even when it slowed delivery
- Documented risks and trade-offs in architectural decisions
- WCAG accessibility — did the right thing without being asked

---

### Principle 4: A Great Team and Winning Culture
> *Develop talent. Collaborate. Build an environment where everyone can thrive.*

**What this means for engineers:**
- Mentoring and growing junior teammates
- Sharing knowledge, not hoarding it
- Cross-team collaboration over siloed delivery
- Diverse and inclusive team-building

**Your Story Hook:**
- Mentored 5 engineers → 2 promoted
- Confluence playbooks that democratized knowledge
- RFC process that aligned cross-functional teams
- Onboarding program that cut ramp-up from 3 weeks to 5 days

---

### Principle 5: Supporting Our Communities
> *Be a responsible corporate citizen. Give back.*

**Engineering relevance:**
- Open source contributions
- Accessibility and inclusive design
- Building systems that responsibly handle customer financial data

---

## JPMC's Engineering Culture — 6 Key Characteristics

### 1. "We Are a Technology Company That Does Banking"
This is the internal mantra. JPMC wants engineers who take pride in technical craftsmanship, not engineers who just want a stable job. Expect interviewers to probe your **genuine passion for engineering**, not just delivery.

### 2. Java-First Engineering Culture
Java and Spring Boot are the lingua franca. If you speak Java fluently, you are in the tribe. If you don't, you need to show strong transferable skills and willingness to ramp up quickly.

### 3. Risk-First Mindset
Every JPMC engineer is expected to think about risk constantly:
- What could go wrong with this design?
- What is the blast radius of this failure?
- How do we detect, alert, and recover?

This is more embedded than at any other company. Interviewers will probe: *"What could go wrong with your design?"*

### 4. Compliance is Not a Constraint — It's a Competitive Advantage
JPMC operates under OCC (Office of the Comptroller of the Currency), Federal Reserve, FINRA, SEC, and multiple international regulators. Their engineers are expected to understand that:
- Audit trails are not optional
- Data residency requirements are real
- Encryption is a baseline, not a feature

**Frame this as a strength:** "I've been building compliance-first systems at Deloitte in a regulated environment. I understand that regulatory requirements protect customers, and I know how to design for them without sacrificing velocity."

### 5. Open Source & Engineering Excellence
JPMC contributes significantly to open source and expects engineers to:
- Know and care about engineering best practices
- Be familiar with popular open source tools
- Potentially contribute back to the community

### 6. AI/ML as a Strategic Priority
JPMC's AI Research division (led by Manuela Veloso, former CMU AI professor) is one of the most sophisticated in finance:
- Published research on LLM for financial documents
- FinBERT (financial language model)
- AI for fraud detection, trade surveillance, credit risk

Your Agentic AI experience is highly relevant here.

---

## JPMC's Four Business Lines — Know These for Interview Context

Understanding which team you're targeting changes how you frame your experience.

### 1. Consumer & Community Banking (CCB) — Chase
- Retail banking, mortgages, auto loans, credit cards
- Scale: 80 million US customers
- Tech focus: Mobile apps, real-time payments, fraud detection
- Your best fit angle: API performance, user experience, compliance

### 2. Corporate & Investment Banking (CIB)
- Investment banking, trading, markets
- Scale: Trillion-dollar daily trading volume
- Tech focus: Low-latency trading systems, risk engines, data pipelines
- Your best fit angle: Distributed ledger, Kafka, high-throughput systems

### 3. Asset & Wealth Management (AWM)
- Wealth management, private banking
- Tech focus: Portfolio management systems, client reporting, analytics
- Your best fit angle: Data integrity, compliance, reporting pipelines

### 4. Commercial Banking (CB)
- Mid-market and large corporate clients
- Tech focus: Treasury services, payments infrastructure
- Your best fit angle: Payment processing, event-driven architecture

**Ask the recruiter which business line you're interviewing for — tailor your stories accordingly.**

---

## JPMC-Specific Interview Culture

### What's Different About JPMC vs Other Companies

| Amazon | Microsoft | **JPMC** |
|---|---|---|
| 16 rigid LPs | Growth Mindset | **"How We Do Business" — blended STAR** |
| Intense probing | Conversational | **Professional but rigorous** |
| "What did YOU do?" | Collaborative "we" OK | **Mix — YOUR contribution within a team** |
| Optimization-first coding | Clean code + tests | **Correctness + security-aware coding** |
| No domain knowledge needed | No domain needed | **Finance domain knowledge valued** |

### Risk Awareness Questions — Unique to JPMC
JPMC interviewers often insert risk-probing questions mid-technical discussion:
- "You mentioned you'd use Kafka here — what failure modes does Kafka have in a payment scenario?"
- "How would you handle a partial failure in your distributed transaction?"
- "What happens to your system during a network partition?"
- "How would you detect if a consumer was processing messages twice?"

**These are NOT trick questions** — they test operational maturity. Always have a risk answer for every system you design.

---

## Understanding JPMC's Regulatory Environment

You don't need to be a compliance expert, but knowing these terms shows domain seriousness:

| Regulation | What It Is | Engineering Implication |
|---|---|---|
| **PCI-DSS** | Payment Card Industry Data Security Standard | Cardholder data protection, encryption requirements |
| **SOX** | Sarbanes-Oxley Act | Financial reporting integrity, immutable audit trails |
| **GDPR** | EU General Data Protection Regulation | Data residency, right to erasure, consent management |
| **CCPA** | California Consumer Privacy Act | US privacy law — similar to GDPR |
| **FINRA** | Financial Industry Regulatory Authority | Trade surveillance, record-keeping requirements |
| **OCC** | Office of the Comptroller of the Currency | Federal banking oversight, technology risk management |
| **BCBS 239** | Basel Committee risk data aggregation | Risk data quality and reporting accuracy |

**Use 2–3 of these naturally in your interview answers when discussing compliance work. Don't overuse them — you're an engineer, not a compliance officer.**

---

## Your "Why JPMC" Answer

```
"JPMorgan Chase sits at a unique intersection I find genuinely compelling: 
the scale and engineering ambition of a technology company, within the 
domain of financial infrastructure that quite literally powers the global 
economy.

The work I've done — distributed systems, event-driven architecture, 
compliance-first engineering — maps directly to what JPMC needs. My 
Distributed Event-Driven Ledger project was essentially a financial 
payment system in miniature: CQRS, Kafka, exactly-once semantics, 
99.99% consistency guarantees. I built it precisely because I wanted 
to understand the engineering challenges of financial systems at scale.

On the AI side, JPMC's investment in AI Research — the LLM work, 
fraud detection, compliance automation — is exactly where I've been 
growing. My GenAI Compliance Engine was designed to solve the kind 
of regulatory documentation problem that financial firms face every day.

I want to work somewhere that takes both engineering excellence AND 
domain responsibility seriously. JPMC is one of a very small number 
of organizations where both are true simultaneously."
```

---

## JPMC Red Flags — What Gets Candidates Rejected

| Red Flag | Why It Matters |
|---|---|
| "Security is someone else's job" | Every JPMC engineer owns security |
| No awareness of failure modes | Risk-blind engineers are liabilities |
| "I'd just use X framework" without rationale | JPMC wants engineers who reason about choices |
| Negative talk about financial regulation | Compliance = customer protection at JPMC |
| No Java experience at all | Not a dealbreaker but shows a gap |
| Ignoring data integrity | Financial systems live or die by it |
| Not asking questions | Signals low engagement |

---

## Cultural Fit Checklist — Before Your Interview

- [ ] Know which JPMC business line (CCB, CIB, AWM, CB) you're targeting
- [ ] Read 1–2 recent JPMC technology blog posts (jpmorganchase.com/technology)
- [ ] Know 3–4 key regulatory acronyms (PCI-DSS, SOX, FINRA) and their engineering implications
- [ ] Understand JPMC's AI Research work (search "JPMC AI Research Manuela Veloso")
- [ ] Know 1–2 JPMC open source projects (github.com/jpmorganchase)
- [ ] Prepare your "Why JPMC" answer (memorize the one above or write your own)

---

**Next: Read `03_INTERVIEW_PROCESS.md`**

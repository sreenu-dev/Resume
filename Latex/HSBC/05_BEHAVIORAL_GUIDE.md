# Behavioral Interview Guide — HSBC

## HSBC Behavioral Framework

HSBC assesses you on **5 core values** using STAR format:

| Value | Weight | What They're Asking |
|---|---|---|
| **Integrity** | 20% | Do you make ethical decisions? Do you prioritize compliance? |
| **Customer Focus** | 20% | Do your decisions ultimately serve customers? |
| **Teamwork** | 20% | Can you work across boundaries, cultures, time zones? |
| **Excellence** | 20% | Do you raise the bar? Do you pursue quality? |
| **Sustainability** | 20% | Do you think long-term? Do you consider impact? |

---

## STAR Format — HSBC Version

```
Situation  (15%) — Brief context
Task       (15%) — Your responsibility
Action     (50%) — What YOU did (include: how you collaborated, how you ensured quality)
Result     (15%) — Metric outcome
Value      (5%)  — Which HSBC value does this demonstrate?
```

---

## Complete Question Bank With STAR Answers

### THEME 1: Integrity

---

**Q: "Tell me about a time you had to make a difficult ethical or integrity-related decision."**

```
S: At Deloitte, there was pressure from a delivery manager to skip 
   a security review step to meet a client deadline.

T: I had to decide whether to comply with the request or push back 
   on a decision made by someone more senior.

A: I didn't refuse — I escalated with data. I prepared a risk assessment: 
   what vulnerabilities the review was designed to catch, examples of 
   companies that skipped similar reviews and faced security incidents, 
   and the estimated cost of a post-production security incident vs a 
   2-day delay. Presented this to the delivery manager and client lead.

R: The security review was kept in the process. Deadline was extended 
   by 2 days. The review found one medium-severity finding that was 
   fixed before release.

HSBC Value: Integrity — I prioritized doing the right thing even when 
inconvenient, and I did it respectfully by building a data-driven case.
```

---

**Q: "Tell me about a time you discovered a compliance or regulatory issue and how you handled it."**

```
S: At Accenture, during a code review, I noticed that our distributed 
   transaction implementation had no idempotency check on the payment 
   processing endpoint.

T: This wasn't assigned to me — I was reviewing for a different purpose. 
   But I recognized it as a potential double-payment risk.

A: Created a detailed risk write-up explaining the exact scenario where 
   a network timeout could cause a client to retry a request, resulting 
   in two debits for one payment. Presented to the tech lead with a 
   proposed fix: Redis-based idempotency keys with 24-hour TTL.

R: Fix was prioritized and shipped before the feature went to production. 
   Zero double-payment incidents. In a financial system, this kind of 
   bug is a regulatory event, not just a bug.

HSBC Value: Integrity — I proactively identified a compliance risk and 
raised it without being asked, protecting the organization.
```

---

### THEME 2: Customer Focus

---

**Q: "Tell me about a time you advocated for the end user or customer."**

```
S: At Accenture, our web application had multiple accessibility failures 
   — color contrast issues and missing keyboard navigation — making it 
   unusable for users with visual impairments. It wasn't on the roadmap.

T: I chose to advocate for these users even though they weren't 
   explicitly represented in sprint planning.

A: Ran a full accessibility audit using Lighthouse and aXe tooling. 
   Created a prioritized list of 47 issues. Built the business case: 
   1 in 7 people globally has a disability — excluding them is both 
   an ethical and a market share failure. Proposed and led a 2-sprint 
   accessibility remediation initiative.

R: Achieved 100% WCAG 2.1 compliance. Accessibility criteria added 
   to PR checklist to prevent future regression.

HSBC Value: Customer Focus — I recognized that some customers were 
being excluded and took action to serve them better.
```

---

**Q: "Describe a time a system or product you built had significant positive impact on customers."**

```
S: At Deloitte, our payment API was experiencing a 2% error rate — 
   which sounds small, but at our transaction volume translated to 
   ~4,000 failed user-facing transactions per day.

T: I took ownership of investigating the root cause, even though 
   the issue had been attributed to "infrastructure" and deprioritized.

A: Profiled the API layer, traced errors to a race condition in 
   the session management layer, implemented Redis distributed locking 
   to eliminate the race, and added OpenTelemetry instrumentation to 
   detect recurrence automatically.

R: Error rate dropped from 2% to under 0.1%. Customer-facing failed 
   transactions dropped from ~4,000/day to fewer than 200. The change 
   directly improved trust scores in our user satisfaction survey.

HSBC Value: Customer Focus — I treated error rates as customer impact, 
not engineering statistics, and prioritized fixing them.
```

---

### THEME 3: Teamwork

---

**Q: "Tell me about a time you had to work with someone very different from you."**

```
S: At Accenture, I was assigned to a global team: myself (India), 
   a product manager (UK), a designer (Brazil), and a backend engineer 
   (Poland). We had different work hours, communication styles, and 
   technical preferences.

T: I needed to build trust and alignment across this diverse team 
   without formal authority.

A: Established a weekly sync call at a time that worked for everyone 
   (rotating to be fair). Created a shared Confluence space with clear 
   documentation in English (not everyone's first language). When 
   disagreements arose about technical approach, I made sure to 
   understand the reasoning behind each person's preference before 
   proposing a solution. When the designer's feedback conflicted with 
   my initial implementation, I didn't dismiss it — I asked questions 
   and incorporated their perspective.

R: The team shipped a feature 1 week ahead of schedule. The product 
   manager told me later that our team had the best cross-functional 
   collaboration they'd seen. The designer specifically praised how 
   I listened to their perspective.

HSBC Value: Teamwork — I actively built trust across cultural and 
functional boundaries, and I valued diverse perspectives.
```

---

**Q: "Tell me about a time you had to coordinate across multiple teams to deliver something."**

```
S: At Accenture, a major feature release required coordination across 
   3 teams: our frontend team, a backend API team, and an external data 
   provider. Each had different sprint cadences and no single owner.

T: The release deadline was fixed. I needed to create alignment without 
   formal authority over any of the other teams.

A: Established a bi-weekly cross-team sync call. Created a shared 
   dependency tracker (Confluence) visible to all teams. Identified 
   the critical path: the data provider's API contract was the 
   longest-lead item — I asked the backend team to build a mock 
   integration layer so we could develop independently. When the 
   real API was delivered 2 weeks late, we were ready to integrate 
   in 1 day.

R: Delivered on the deadline. Feature launched with zero integration 
   issues. The dependency tracker was adopted as a standard practice 
   for cross-team releases.

HSBC Value: Teamwork — I facilitated collaboration across teams without 
formal authority, and I created tools that helped everyone succeed.
```

---

### THEME 4: Excellence

---

**Q: "Tell me about a time you improved the reliability or quality of a system."**

```
S: At Deloitte, our CI/CD pipeline had no automated security scanning. 
   Vulnerabilities were being caught only during manual code reviews — 
   weeks after the code was written, when they were expensive to fix.

T: I owned the initiative to integrate automated security scanning 
   into the pipeline, without being assigned to it explicitly.

A: Researched OWASP-compliant scanning tools compatible with our .NET 
   stack. Integrated a static analysis scanner into every PR pipeline 
   step. Set a policy: PRs with high-severity findings are blocked from 
   merging. Worked with the team to remediate the 3 medium-severity 
   findings the scanner surfaced in existing code on day one.

R: Vulnerability exposure dropped by 40%. Zero security incidents reached 
   production in the following quarter. The scanner surfaced 3 existing 
   vulnerabilities that would have otherwise reached production.

HSBC Value: Excellence — I proactively raised the bar for code quality 
and security, and I made it a team standard.
```

---

**Q: "Tell me about a time you helped someone on your team grow."**

```
S: At Accenture, I had a junior engineer who was technically capable 
   but struggled with code design — writing code that worked but 
   was hard to test and maintain.

T: Rather than just flagging issues in code review, I wanted to 
   help them develop the underlying design instincts.

A: Started weekly 1-hour pair-programming sessions focused specifically 
   on design: SOLID principles, dependency injection, testable 
   architecture. Gave them increasingly complex design challenges. 
   When reviewing their PRs, I always explained the "why" behind 
   feedback, not just the "what."

R: Within 3 months, their code review feedback dropped to near zero. 
   6 months later they were leading design discussions for their own 
   features. Within 18 months they were promoted to senior analyst.

HSBC Value: Excellence — I invested in raising the bar for the entire 
team by developing junior talent.
```

---

### THEME 5: Sustainability

---

**Q: "Tell me about a time you considered long-term impact in your technical decisions."**

```
S: At Deloitte, we were building a new microservice. The team wanted 
   to use a trendy but immature framework to move fast in the short term.

T: I needed to advocate for a more sustainable, long-term approach 
   without blocking progress.

A: I didn't say "no" — I proposed a hybrid approach: use the mature 
   framework for the core service, but build a thin abstraction layer 
   so we could swap out the framework later if needed. This gave us 
   the benefits of the mature framework (stability, community support, 
   documentation) while preserving flexibility for the future.

R: The service has been running in production for 3 years with zero 
   framework-related incidents. When a new framework became available, 
   we were able to evaluate it without being locked in. The abstraction 
   layer we built became a pattern other teams adopted.

HSBC Value: Sustainability — I made technical decisions that balanced 
short-term velocity with long-term maintainability.
```

---

## Your Story Bank — Quick Reference

| Story | Primary HSBC Value | Secondary Values |
|---|---|---|
| Security review pushback | Integrity | Excellence |
| Idempotency risk catch | Integrity | Excellence |
| WCAG accessibility sprint | Customer Focus | Excellence |
| API error rate reduction | Customer Focus | Excellence |
| Global team collaboration | Teamwork | Customer Focus |
| Cross-team release delivery | Teamwork | Excellence |
| Security scanning integration | Excellence | Integrity |
| Junior engineer mentoring | Excellence | Teamwork |
| Framework sustainability decision | Sustainability | Excellence |

---

## HSBC-Specific Phrases to Use

✅ *"I thought about how this decision would impact our global teams."*
✅ *"I made sure to understand the perspective of people from different backgrounds."*
✅ *"I considered the long-term maintainability, not just short-term velocity."*
✅ *"I raised this as a compliance issue early — before it became a problem."*
✅ *"I advocated for the customer, even though they weren't in the room."*
✅ *"I built this with global scale in mind — it works across regions."*

---

**Next: Read `06_MASTER_PLAN.md`**

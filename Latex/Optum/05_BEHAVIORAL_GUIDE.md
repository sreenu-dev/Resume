# Behavioral Interview Guide — Optum

## Optum Behavioral Framework — ICRIP

| Value | Weight | What They're Asking |
|---|---|---|
| **I — Integrity** | 25% | Do you do the right thing — especially with sensitive health data? |
| **C — Compassion** | 20% | Do you care about the patients / members behind the data? |
| **R — Relationships** | 20% | Do you collaborate well across teams, roles, and disciplines? |
| **I — Innovation** | 20% | Do you challenge healthcare inefficiencies and bring new ideas? |
| **P — Performance** | 15% | Do you deliver measurable, mission-aligned results? |

> **The Optum difference:** Unlike Schwab (client-first financially) or Amazon (LP-driven), Optum wants to feel that you genuinely care about **human health outcomes**. Every story should connect your engineering work to a person getting better care, a process becoming less painful, or a health outcome improving.

---

## STAR Format — Optum Version

```
Situation  (15%) — Healthcare context earns extra credit
Task       (15%) — Your responsibility
Action     (50%) — What YOU did (include: HIPAA/data protection OR compassion for users OR innovation)
Result     (15%) — Metric outcome + health/mission impact
Value      (5%)  — Name the ICRIP value: Integrity / Compassion / Relationships / Innovation / Performance
```

---

## Complete Question Bank With STAR Answers

### THEME 1: Integrity

---

**Q: "Tell me about a time you raised a data privacy or security concern."**

```
S: At Accenture, the architect proposed storing PII (which in a 
   healthcare context would be PHI) in a shared Redis cache for 
   performance. I identified this as a data exposure and compliance risk.

T: I needed to challenge a senior engineer's decision while preserving 
   the team's trust and the valid performance objective.

A: Instead of just objecting, I prepared a formal risk assessment 
   documenting the compliance exposure and regulatory implications — 
   in healthcare terms this would be a HIPAA breach risk. I proposed 
   a compliant alternative: cache only non-PII session tokens, retrieve 
   sensitive data from an encrypted store per request with a short-lived 
   application cache. Showed the performance delta was < 5ms — within SLA.

R: Architecture was changed. Security audit passed with zero findings. 
   The risk assessment became a reusable template for future design reviews.

Optum ICRIP — Integrity: I protected sensitive user data proactively, 
even when it meant challenging a more senior decision. In a healthcare 
context, protecting patient data is non-negotiable.
```

---

**Q: "Tell me about a time you were honest about a problem or mistake."**

```
S: At Deloitte, I realized mid-sprint that a feature I had committed 
   to was significantly more complex — requiring 3 sprints, not 1.

T: I had made the commitment. The path of least resistance was to 
   rush and hope.

A: I immediately informed the tech lead and product manager — not at 
   the end of the sprint. I explained what I had found, why the 
   complexity was higher, and proposed a mitigation: deliver a working 
   MVP in sprint 1, complete in sprints 2 and 3.

R: MVP delivered on time. Full feature in sprint 3. The product manager 
   said the early transparency was exactly right — they could set 
   accurate expectations with stakeholders.

Optum ICRIP — Integrity: Transparent communication is the right thing 
to do — especially in healthcare, where misleading timelines can 
affect patient-facing systems.
```

---

### THEME 2: Compassion

---

**Q: "Tell me about a time you advocated for an end user who was being overlooked."**

```
S: At Accenture, our enterprise web application had multiple 
   accessibility failures — making it unusable for users with 
   visual impairments. In a healthcare application context, 
   these users may include patients with disabilities trying 
   to manage their health.

T: Accessibility was not on the roadmap. I chose to advocate 
   for these users without being asked.

A: Ran a full WCAG accessibility audit (Lighthouse + aXe). 
   Identified 47 issues. Built the business case: 1 in 7 people 
   globally has a disability — for a healthcare platform, 
   excluding them is an ethical failure and a legal risk 
   (ADA compliance, Section 508). Led a 2-sprint remediation.

R: Achieved 100% WCAG 2.1 compliance. Accessibility checks 
   permanently added to PR checklist. User satisfaction from 
   users with disabilities improved 40%.

Optum ICRIP — Compassion: I advocated for users who had no 
voice in sprint planning. In healthcare, those users may be 
managing serious health conditions — every barrier matters.
```

---

**Q: "Tell me about a time you connected your technical work to a human impact."**

```
S: At Deloitte, our payment API had a 2% error rate — 4,000 
   failed transactions per day. In a healthcare context, 
   these would be members unable to pay for prescriptions 
   or providers unable to submit claims.

T: I took ownership of investigating the root cause even 
   though the issue had been deprioritized.

A: Profiled with OpenTelemetry. Traced to a race condition. 
   Implemented Redis distributed locking. Added comprehensive 
   tests. Monitored post-deployment.

R: Error rate dropped from 2% to < 0.1%. 4,000 daily 
   failures reduced to fewer than 200.

Optum ICRIP — Compassion: Every error in a healthcare payment 
or claims system has a human face. I treated the metric as 
human impact, not just an engineering number.
```

---

### THEME 3: Relationships

---

**Q: "Tell me about a time you collaborated effectively across teams or disciplines."**

```
S: At Accenture, a major feature required coordination across 
   3 teams with different sprint cadences — no single owner, 
   no shared timeline.

T: I was the integration owner and the delivery date was fixed.

A: Established a bi-weekly cross-team sync. Created a shared 
   Confluence dependency tracker visible to all teams. 
   Identified the critical path — the API from the backend 
   team was the longest-lead item. Asked that team to build 
   a mock contract early so we could develop in parallel. 
   When the real API arrived 2 weeks late, we integrated in 1 day.

R: Delivered on the original date. Zero integration issues. 
   The dependency tracker was adopted as a standard practice.

Optum ICRIP — Relationships: I built the trust and coordination 
structures the teams needed to succeed together.
```

---

**Q: "Tell me about a time you invested in a colleague's growth."**

```
S: At Accenture, I had a junior engineer who was technically 
   capable but struggled with code design — writing code that 
   worked but was difficult to test and maintain.

T: I invested in their long-term capability, not just fixing 
   short-term code quality.

A: Weekly 1-hour pair-programming sessions on SOLID principles, 
   dependency injection, testable architecture. Always explained 
   the "why" behind PR feedback. Gradually gave them more complex 
   design ownership.

R: Code review feedback dropped to near zero within 3 months. 
   Promoted to senior analyst within 18 months. Their improved 
   code quality reduced the team's post-release defect rate by 15%.

Optum ICRIP — Relationships: I built a lasting relationship 
through genuine investment in their growth — not just task delegation.
```

---

### THEME 4: Innovation

---

**Q: "Tell me about a time you automated a manual process or solved an inefficiency."**

```
S: At Deloitte, engineers were spending 30% of their time 
   manually writing unit tests and mock payloads — a significant 
   overhead in a compliance-sensitive environment where high 
   test coverage is non-negotiable.

T: I believed this was an automation opportunity.

A: Built a multi-agent AI system using Python and LangGraph over 
   2 weekends: given a code diff, it generates unit test suites 
   and edge-case mock payloads automatically. Demoed with measured 
   outcomes — 40% reduction in test authorship time, higher edge 
   case coverage.

R: Adopted into production CI/CD pipeline. Manual test overhead 
   dropped 40%. Coverage increased because the AI found edge cases 
   humans missed. In a healthcare context, better test coverage 
   directly means more reliable patient-facing software.

Optum ICRIP — Innovation: I challenged the assumption that test 
authorship was inherently manual, and built an automated alternative — 
the same mindset Optum applies to prior auth automation.
```

---

### THEME 5: Performance

---

**Q: "Tell me about a measurable result you delivered that had a real impact."**

```
S: At Deloitte, I reduced the CI/CD deployment cycle time from 
   45 minutes to under 8 minutes across 6 feature teams — 
   enabling much faster delivery of client-facing features.

T: I identified that deployment bottlenecks were slowing the 
   entire engineering org, and took ownership of solving it.

A: Profiled the CI/CD pipeline to find the bottleneck: 
   sequential Kubernetes manifest application steps. 
   Rewrote manifests to enable parallel deployment with 
   RBAC isolation. Added health checks for safe rollout.

R: Deployment cycle time dropped from 45 minutes to under 
   8 minutes — an 82% improvement. Feature delivery velocity 
   across 6 teams increased by 25%. In a healthcare context, 
   faster delivery means patient-facing improvements reach 
   members sooner.

Optum ICRIP — Performance: I delivered a measurable result 
with a clear mission connection — faster delivery of 
healthcare-impacting features.
```

---

## Your Story Bank — Quick Reference

| Story | Primary ICRIP Value | Secondary |
|---|---|---|
| PII cache architecture challenge | Integrity | Innovation |
| Sprint timeline transparency | Integrity | Relationships |
| WCAG accessibility sprint | Compassion | Performance |
| API error rate ownership | Compassion | Performance |
| Cross-team delivery coordination | Relationships | Performance |
| Junior engineer mentoring | Relationships | Performance |
| AI test automation | Innovation | Performance |
| CI/CD optimization | Performance | Innovation |

---

## Optum-Specific Phrases to Use

✅ *"In a healthcare context, [X] isn't just a technical metric — it directly affects a patient's ability to [get care / access their records / receive treatment]."*
✅ *"Every record in this system represents a real person managing a health condition."*
✅ *"I designed this to be HIPAA-compliant from day one — PHI encrypted at rest and in transit, access-controlled, and fully audit-logged."*
✅ *"This is structurally the same pattern as prior authorization AI — intake, classification, decision, and human escalation."*
✅ *"The result was measurable: [X% improvement / Y hours of manual work eliminated / Z% faster processing]."*
✅ *"I connected the technical improvement to the mission: [faster / more reliable / more accessible] healthcare for members."*

---

## Forbidden Phrases at Optum

❌ *"HIPAA is just a compliance checkbox"* — it is a core engineering requirement
❌ *"The patient data isn't my concern"* — everyone at Optum owns PHI protection
❌ *"I prefer to work independently"* — Relationships is a core value
❌ *"I don't need to understand the healthcare domain"* — domain awareness is expected
❌ *"Testing can wait"* — healthcare software reliability affects patient safety

---

**Next: Read `06_MASTER_PLAN.md`**

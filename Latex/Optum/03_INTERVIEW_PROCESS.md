# Optum Interview Process — Exact Stages & What to Expect

## The Complete Optum Interview Pipeline

```
Stage 1: Application (careers.unitedhealthgroup.com or LinkedIn)
         ↓
Stage 2: Recruiter Screen (30 min)
         ↓
Stage 3: HireVue Video Assessment (optional) OR
         Online Coding Test (60–90 min, HackerRank)
         ↓
Stage 4: Technical Phone Screen (45–60 min)
         ↓
Stage 5: VIRTUAL PANEL — (3–4 rounds)
   ├── Round 1: Coding Interview (45–60 min)
   ├── Round 2: System Design — Healthcare Data Focus (60 min)
   ├── Round 3: Behavioral / ICRIP Values (45–60 min)
   └── Round 4 (optional): Manager / Leader Discussion (30–45 min)
         ↓
Stage 6: Hiring Decision (5–10 business days)
         ↓
Stage 7: Offer → Background Check → HIPAA Training → Start
```

**Total timeline from application to offer: 3–5 weeks**

> Note: Optum hires in very high volume — the process is generally faster and more structured than smaller companies.

---

## Stage 1: Application Strategy

### Where to Apply
- **Primary:** careers.unitedhealthgroup.com (filter: "Optum", India, Technology)
- **LinkedIn:** Many roles posted — Easy Apply available
- **Referral:** Very helpful — UHG/Optum has a strong referral program

### Application Tips
- Filter by "Optum Global Solutions" + Hyderabad or Bangalore
- Target roles: Software Engineer, Platform Engineering, Data Engineering, AI/ML, Full Stack
- Apply broadly — Optum posts hundreds of tech roles at any given time
- Tailoring: add "HIPAA-compliant", "healthcare data", and "PHI" to your resume language

---

## Stage 2: Recruiter Screen (30 min)

### What They Assess:
- Background and experience fit
- Awareness of Optum / healthcare domain (even basic awareness differentiates you)
- Communication quality
- Salary expectations and notice period
- Genuine motivation for healthcare tech

### Must-Be-Ready-For:
- "Walk me through your experience."
- "Why Optum / why healthcare technology?"
- "Are you familiar with HIPAA?"
- "Tell me about your experience with distributed systems / Java / Python."
- "What are your salary expectations?"

### Salary Discussion (Hyderabad):
```
"Based on my research and my 5+ years in compliance-grade enterprise 
systems — including HIPAA-adjacent security and AI pipeline work — 
I'm targeting ₹50–70 LPA total compensation. Happy to discuss the 
full package."
```

---

## Stage 3: HireVue / Online Assessment

### HireVue (if required):
- 4–6 pre-recorded behavioral questions
- Record video responses (60–90 seconds each)
- Sample: "Tell me about a time you worked on a complex technical problem"
- Tip: Mention healthcare relevance if possible. Use ICRIP values framework.

### HackerRank (if required):
- 2–3 problems in 90 minutes
- Difficulty: Medium
- Language: Java or Python preferred
- Tip: Focus on correctness and clean code — Optum does not require optimal algorithms

---

## Stage 4: Technical Phone Screen (45–60 min)

### Structure:
```
0–5 min:   Intro + brief background
5–20 min:  Deep dive on 1–2 resume projects
           → Healthcare angle probed: "How did you ensure data security?"
           → "How would you handle PHI in this system?"
20–45 min: 1 medium coding problem
45–60 min: 2–3 behavioral questions (ICRIP-aligned)
```

---

## Stage 5: The Virtual Panel (3–4 Rounds)

### Round 1: Coding Interview (45–60 min)

**What to expect:**
- 1 medium LeetCode problem
- Java or Python — both acceptable
- Follow-ups: "How would you test this?", "How would you handle large scale?" and occasionally "How would you protect sensitive health data in this system?"

**Optum Coding Philosophy:**
- Correctness > optimization
- Readable, maintainable code is valued (healthcare systems have long lives)
- Testing strategy is always asked
- Healthcare context may be added: "Imagine this processes medical records"

---

### Round 2: System Design — Healthcare Data Focus (60 min)

**Optum System Design Philosophy:**
The 3 non-negotiables for every Optum system design:
1. **HIPAA compliance** — PHI must be encrypted, access-controlled, audit-logged
2. **Data reliability** — healthcare data must be correct and complete (never lost)
3. **Interoperability** — systems must be able to exchange data (FHIR/HL7 APIs)

**Structure:**
```
0–7 min:   Requirements (functional + non-functional + HIPAA requirements)
7–12 min:  Scale estimation (claims volume, member count)
12–20 min: High-level architecture
20–45 min: Component deep dives
45–55 min: HIPAA compliance + failure scenarios
55–60 min: Your questions
```

**Most Common Optum System Design Problems:**
1. Design a prior authorization system (most common)
2. Design a medical claims processing pipeline
3. Design a patient health record (EHR) system
4. Design a healthcare fraud detection system
5. Design a FHIR API platform for health data exchange
6. Design a population health analytics platform

---

### Round 3: Behavioral / ICRIP Values (45–60 min)

**Framework:** Optum ICRIP values:
- **I (Integrity)** — doing the right thing, data privacy, honest communication
- **C (Compassion)** — caring for end users, patient impact, health equity
- **R (Relationships)** — collaboration, cross-team trust, building people up
- **I (Innovation)** — new approaches, challenging inefficiencies, AI/automation
- **P (Performance)** — measurable delivery, outcome-driven results

**Full question bank → see `05_BEHAVIORAL_GUIDE.md`**

---

### Round 4 (Optional): Manager Discussion (30–45 min)

- Conversational — career goals, team fit, role specifics
- May touch on: "What do you know about Optum's work in [area]?"
- Heavy weight on mission alignment and long-term interest in healthcare

---

## Optum Levels & Compensation (India — Hyderabad)

| Level | Title | Base Salary | Annual Bonus | Stock/RSU | Total Comp |
|---|---|---|---|---|---|
| SE II | Software Engineer II | ₹20–35 LPA | 8–10% | Limited | ₹22–38 LPA |
| **SSE** | **Senior Software Engineer** | **₹38–58 LPA** | **10–15%** | **Eligible** | **₹42–67 LPA** |
| Lead SE | Lead Software Engineer | ₹62–85 LPA | 15–18% | RSU eligible | ₹71–100 LPA |
| Principal | Principal Engineer | ₹90–130 LPA | 18–22% | RSU eligible | ₹106–158 LPA |

**Your Target: Senior Software Engineer** — ₹42–67 LPA total comp

> **Honest Note:** Optum pays **below ServiceNow, Schwab, and Microsoft** but is competitive for healthcare tech. The mission, stability, India scale, and career growth pathways are the key value propositions.

### Compensation Structure:
- **Base salary** — fixed
- **Annual bonus** — performance-based (UHG MIP — Management Incentive Plan)
- **RSU** — Restricted Stock Units at senior/lead levels, vests over 3–4 years
- **ESPP** — Employee Stock Purchase Plan
- **Benefits** — comprehensive medical, generous PTO, India provident fund, wellness benefits

### Negotiation at Optum:
```
Step 1: "Thank you for the offer. Could I have 3 days to review it?"

Step 2: Counter:
"Based on my experience in compliance-grade distributed systems 
 and AI pipeline engineering — which maps directly to Optum's 
 HIPAA-compliant architecture and prior auth AI work — I was 
 hoping we could discuss [₹X LPA] for base. Is there flexibility?"

Step 3: If base is firm:
"I understand. Could we discuss a higher RSU grant or 
 a joining bonus? I'm very motivated to join Optum and contribute 
 to the healthcare AI roadmap."

Leverage:
- Competing offer from ServiceNow/Schwab/Microsoft
- HIPAA-adjacent compliance engineering depth
- Agentic AI experience (directly aligned with prior auth automation)
```

---

## Questions to Ask Each Interviewer

### Coding Interviewer:
- "How does Optum approach HIPAA compliance in the development process — is it part of code review or separate auditing?"
- "What does the testing culture look like for healthcare data systems?"

### System Design Interviewer:
- "How does Optum handle the schema evolution challenge with FHIR — as the standard evolves, how do you manage compatibility?"
- "What's the most interesting data scale challenge your team is solving right now?"

### Behavioral Interviewer:
- "How does compassion for patients show up in day-to-day engineering decisions?"
- "What does innovation look like at Optum — how do engineers bring new ideas to the healthcare problems you're solving?"

### Manager:
- "What would success look like for me in the first 6 months?"
- "How does this team's work connect to improving patient outcomes or member experience?"
- "Where does AI/ML fit in your team's roadmap?"

---

**Next: Read `04_TECHNICAL_GUIDE.md`**

# Amazon Interview — Master Plan & Interview Day Guide

## Your Target: SDE2 at Amazon India (Hyderabad / Bangalore)
Expected Total Comp: ₹80–130 LPA | Timeline to offer: 4–8 weeks

---

## Part 1: Complete Preparation Roadmap

### Phase 1 — Foundation (Week 1–2)

#### Priority #1: Learn Amazon's Leadership Principles
This is non-negotiable. Do this FIRST, before any coding practice.

```
Day 1: Read all 16 LPs (02_LEADERSHIP_PRINCIPLES.md)
Day 2: Write 1 STAR story per LP from your experience
Day 3: Practice telling each story out loud (record yourself)
Day 4: Identify your 5 weakest LP stories — strengthen them
Day 5: Write a 2nd story for your top 8 most-asked LPs
Day 6: Practice 10 LP stories with a friend or timer
Day 7: Rest
```

**Your Strongest LPs (based on your resume):**
| LP | Your Evidence |
|---|---|
| Deliver Results | 100+ user stories, 300+ defects, 70% error reduction |
| Ownership | RFC process, onboarding playbooks, agentic monitoring |
| Invent & Simplify | Agentic AI test generation, Micro-Frontend migration |
| Think Big | 20,000 TPS ledger, multi-agent AI architecture |
| Hire & Develop the Best | Mentored 5 engineers, drove interview process |
| Dive Deep | SQL tuning, race condition elimination, structured logging |

**Your Weakest LPs (practice more):**
- Frugality (tie cost to your decisions)
- Have Backbone; Disagree and Commit (think of disagreement moments)
- Customer Obsession (reframe all stories with user impact)

#### Priority #2: Update Your Resume
- [ ] Use `Sreenivasulu_Ummadi_Amazon.tex` from this folder
- [ ] Compile to PDF via Overleaf (overleaf.com)
- [ ] Verify all bullets start with ownership verbs: "Owned", "Drove", "Designed", "Delivered"
- [ ] Add AWS to skills (even if currently basic)

#### Priority #3: Learn Basic AWS (21 hours)
Amazon runs on AWS. Not knowing basic AWS is a red flag.

```
Resource: AWS Free Tier (aws.amazon.com/free)
          AWS Skill Builder (skillbuilder.aws — free courses)

Day 1 (3h): EC2 + S3 basics — launch an instance, store a file
Day 2 (3h): RDS + DynamoDB — create a table, run queries
Day 3 (3h): Lambda + API Gateway — build a simple serverless API
Day 4 (3h): SQS + SNS — send and receive messages
Day 5 (3h): CloudWatch — set up logging and an alarm
Day 6 (3h): ECS/EKS — deploy a Docker container (you already know K8s)
Day 7 (3h): Architecture patterns — review AWS Well-Architected Framework
```

---

### Phase 2 — Technical Preparation (Week 2–3)

#### Coding Practice Plan

**LeetCode Target: 50 problems in 3 weeks**

| Category | Count | Focus |
|---|---|---|
| Arrays / Strings | 10 | Two Sum, Sliding Window, 3Sum |
| Trees | 8 | BFS, DFS, LCA, Serialize |
| Graphs | 8 | Islands, Course Schedule, Topological Sort |
| Dynamic Programming | 8 | Coin Change, House Robber, LCS |
| Stacks / Queues | 6 | LRU Cache, Min Stack, Monotonic Stack |
| Linked Lists | 5 | Reverse, Merge K, Detect Cycle |
| Binary Search | 5 | Rotated Array, Peak Element, Koko |
| **Total** | **50** | |

**Daily coding routine:**
```
Mon–Fri: 2 LeetCode problems (1h each, strictly timed)
Saturday: Full OA mock (2 problems, 90 min)
Sunday: Review mistakes, no new problems
```

#### System Design Practice Plan

**5 systems to design from scratch:**
1. URL Shortener (Week 2, Day 1)
2. Amazon S3 (Week 2, Day 3)
3. Notification System (Week 2, Day 5)
4. Order Processing System (Week 3, Day 1)
5. Rate Limiter (Week 3, Day 3)

**Design session format (60 min each):**
```
0–7 min:   Write requirements on paper
7–12 min:  Estimate scale (DAU, RPS, storage)
12–17 min: Sketch high-level architecture
17–40 min: Deep dive each component
40–55 min: Identify bottlenecks + trade-offs
55–60 min: What would you improve with more time?
```

---

### Phase 3 — Mock Interviews (Week 3–4)

#### Mock Interview Schedule
```
Week 3:
  - 2x LP behavioral mock (Pramp or friend, 45 min)
  - 1x Coding mock (LeetCode random medium, 45 min timed)
  - 1x System Design mock (with whiteboard)

Week 4:
  - 1x Full Loop simulation (3h: 2 coding + 1 system design + 1 LP)
  - Review all feedback
  - Final LP story polish
```

#### Mock Interview Resources
- **Pramp** (pramp.com) — Free peer mock interviews
- **Interviewing.io** (interviewing.io) — Anonymous mocks with real engineers
- **Exponent** (tryexponent.com) — Amazon-specific prep content
- **Amazon's own practice:** Amazon includes sample LP questions in recruiter emails

---

### Phase 4 — Loop Day Preparation (1 Week Before)

#### Company Research Checklist
- [ ] Read Amazon's Leadership Principles page (amazon.jobs/principles)
- [ ] Research the specific team you're interviewing with
- [ ] Know Amazon's major business lines: AWS, Marketplace, Alexa, Prime, Logistics
- [ ] Read 2–3 recent Amazon AWS blog posts about distributed systems
- [ ] Know Amazon's current scale: 300M+ customers, $500B+ revenue

#### What to Know About Amazon Engineering Culture
| Aspect | Detail |
|---|---|
| Team Size | "Two-pizza teams" (~6–10 engineers) |
| Autonomy | Teams own their own services end-to-end |
| On-Call | Engineers are on-call for their own services |
| Tech Debt | Amazon is faster-moving; tech debt is real |
| Deployment | Continuous deployment — teams deploy multiple times/day |
| Customer Focus | Engineering decisions trace back to customer impact |

---

## Part 2: Interview Day — Minute-by-Minute Guide

### Night Before
```
✅ Close all browser tabs except your notes
✅ Test: camera, microphone, internet (both WiFi + backup hotspot)
✅ Install or test the coding platform (CoderPad, CodeSignal, or Chrome)
✅ Prepare: water, notepad, pen (for sketching during system design)
✅ Prepare your LP story cheat sheet (printed or visible on 2nd screen)
✅ Set 2 alarms — one for wake-up, one 30 min before first interview
✅ Sleep at a fixed time — don't stay up cramming
```

### Morning of the Loop
```
2 hours before:
  ✅ Eat protein-rich breakfast (eggs, yogurt, nuts)
  ✅ Light review of your top 5 LP stories (don't cram new things)
  ✅ Review complexity cheat sheet
  ✅ Do 2–3 breathing exercises

1 hour before:
  ✅ Final tech check (camera position, lighting, background)
  ✅ Close all notifications (Slack, email, phone on silent)
  ✅ Have water within reach
  ✅ Open your LP notes on a second screen or printed paper
  ✅ Log into the interview platform 10 minutes early

5 minutes before:
  ✅ Take 5 deep breaths
  ✅ Remind yourself: "I have 5+ years of real experience. I've built real systems."
  ✅ Smile — it affects your vocal tone even on calls
```

---

## Part 3: During Each Interview Round — Cheat Sheet

### Coding Round Cheat Sheet
```
First 3 min — ALWAYS:
  "Before I start coding, let me make sure I understand the problem...
   Can the input be empty? Can values be negative? 
   Is there a target time complexity?"

Before coding — ALWAYS:
  "My approach is [X]. The time complexity will be O([Y]) and 
   space complexity O([Z]). Does this approach make sense?"

While coding — ALWAYS:
  Think out loud: "I'm using a HashMap here because..."
  "I'm handling the edge case where..."

After coding — ALWAYS:
  "Let me trace through the example to verify...
   Edge case: what if the array is empty? [trace it]"

If stuck:
  "Let me think about what data structure would help here..."
  [Pause 30 seconds, think out loud]
  "I'm going to try a different approach — what if I used..."
  [It's OK to ask for a hint: "Can I get a small hint on the direction?"]
```

### System Design Round Cheat Sheet
```
Always start with: "Let me clarify the requirements first."

Requirements framework:
  Functional: "The system should be able to [X], [Y], [Z]."
  Non-functional: "We need [latency], [availability], [scale]. 
                   Is 99.99% uptime required? What's peak RPS?"

Scale estimation (say out loud):
  "With [X] DAU and [Y] requests per user per day, 
   we're looking at roughly [Z] requests per second.
   At [W] bytes per request, that's [V] storage per year."

Drive the whiteboard:
  Don't wait for questions. Say: "Let me start with the high-level
  architecture and then we can dive deep into any component."

Customer angle (Amazon LOVES this):
  "From the customer's perspective, what matters most is [X], 
   so I'll prioritize [Y] in my design."

Trade-off discussion (mandatory):
  "I chose [X] over [Y] because [reason]. The trade-off is [Z],
   which I'd address by [mitigation]."
```

### LP / Behavioral Round Cheat Sheet
```
STAR format — time it:
  Situation: 20 seconds
  Task:      20 seconds
  Action:    90 seconds (the meat — be specific about YOUR actions)
  Result:    30 seconds (always include a metric)

If they ask "tell me more":
  → You should have more to say. Prepared stories should have depth.
  → "Sure, specifically what I did was..."

If they ask "what would you do differently":
  → Always have a genuine answer. Show self-awareness.
  → "Looking back, I would have [done X earlier / communicated Y better]..."

If you don't have a perfect story:
  → Use your closest story and be honest: 
    "I don't have a perfect example, but the closest situation was..."
  → Never fabricate — Bar Raiser will probe until they find holes.

Forbidden phrases:
  ❌ "We did..." → Always "I did..."
  ❌ "The team decided..." → "I recommended and the team decided..."
  ❌ "I'm not sure of the exact numbers..." → Always know your numbers
  ❌ "It wasn't my area..." → Amazon expects you to own outside your area
```

### Bar Raiser Round — Special Guidance
```
The Bar Raiser wants to see:
  1. Depth — not surface-level answers. Prepare to go 3 levels deep.
  2. Self-awareness — admit failures and what you learned
  3. Above-the-bar thinking — answers that reflect SDE3+ quality
  4. Consistency — your stories must be consistent with your resume

Signs it's going well:
  ✅ They keep asking follow-up questions (they're engaged)
  ✅ They probe your failure stories (they're testing self-awareness)

Signs it's going poorly:
  ⚠️ Very short responses with no follow-ups (moving on quickly)
  ⚠️ They ask you to "tell me about a different situation"

If you sense it's going poorly:
  → Stay calm. Slow down. Give more detail in next answers.
  → "I want to make sure I'm giving you enough detail — 
      should I expand on any of these points?"
```

---

## Part 4: After Each Round

### Between Rounds (10–15 min breaks)
```
✅ Drink water
✅ Stand up and stretch briefly
✅ Note down 1–2 questions from the previous round (for the debrief)
✅ Glance at your LP notes for the next round
✅ Do NOT review your code or second-guess yourself
✅ Breathe — each round is independent
```

### Questions to Ask Each Interviewer
```
Coding interviewers:
  "What's the most interesting technical problem your team has solved recently?"
  "What does code review culture look like on your team?"

System Design interviewer:
  "At what scale does this team's primary service operate?"
  "What were the hardest architectural decisions the team had to make?"

Bar Raiser:
  "What does it look like when someone raises the bar on your team?"
  "What's one thing the best engineers at Amazon do that surprises people?"

Hiring Manager:
  "What would a successful first 6 months look like for someone in this role?"
  "What's the biggest opportunity for impact that isn't being seized yet?"
```

---

## Part 5: Offer Negotiation

### When the Offer Comes (Don't Accept Immediately)
```
Step 1: Thank them and ask for time: 
  "Thank you so much! I'm very excited about this opportunity. 
   Could I have 3–4 days to review the details carefully?"

Step 2: Research the offer:
  - levels.fyi → Amazon SDE2 India compensation
  - glassdoor → Amazon SDE2 Hyderabad base salary
  - Total comp = Base + Annual RSU value + Signing bonus

Step 3: Counter (always counter):
  "I'm very excited about this role. I've done my research on 
   market compensation for this level, and I was hoping we could 
   discuss [specific number]. Is there flexibility on [base / RSUs / signing]?"

Negotiation levers (most to least flexible):
  1. Signing bonus (most flexible — one-time cost to Amazon)
  2. RSU grant size (flexible, especially if you have a competing offer)
  3. Base salary (least flexible — compressed bands at Amazon India)

If you have another offer:
  Always mention it: "I do have another offer I'm considering, 
   which makes the decision more urgent — but Amazon is my top preference."
```

---

## Part 6: The Complete File Guide

```
Amazon_Interview_Prep/
├── Sreenivasulu_Ummadi_Amazon.tex   ← Your Amazon-optimized resume (compile on Overleaf)
├── 01_RESUME_GAP_ANALYSIS.md       ← Honest gap analysis + rewrite strategy
├── 02_LEADERSHIP_PRINCIPLES.md     ← All 16 LPs with YOUR story examples (READ FIRST)
├── 03_AMAZON_INTERVIEW_PROCESS.md  ← Exact stages, timeline, salary ranges
├── 04_TECHNICAL_CODING_GUIDE.md    ← Coding patterns + top 30 problems + system design
└── 05_INTERVIEW_DAY_AND_MASTER_PLAN.md  ← This file
```

---

## Part 7: Honest Assessment — Your Chances

### Strengths That Give You an Edge
- **Agentic AI experience** is rare and genuinely impressive at Amazon
- **Distributed Event-Driven Ledger project** maps directly to Amazon's AWS/infrastructure work
- **Quantified impact** throughout resume aligns with Amazon's results culture
- **Leadership proof points** (mentoring, RFC, onboarding) satisfy LP assessment
- **Deloitte + Accenture** are well-recognized at Amazon

### What Could Get You Rejected
- **Weak LP stories without metrics** (fixable with prep)
- **Saying "we" instead of "I"** (common mistake, easy to fix with practice)
- **Poor coding performance** (fixable with 50 LeetCode problems)
- **Not knowing AWS at all** (fixable in 1 week with free tier)
- **Not driving system design conversation** (fixable with practice)

### Realistic Probability (if you follow this plan)
| Round | Pass Rate |
|---|---|
| OA | 70–75% |
| Phone Screen | 55–65% |
| Loop (all 5–7 rounds) | 35–50% |
| Overall from application to offer | **25–40%** |

> **This is realistic and actually good.** Amazon's overall SDE2 pass rate is 10–15% industry-wide. With proper LP preparation and strong technical skills, you're in the top quartile of candidates.

---

## Part 8: 4-Week Master Checklist

### Week 1 — Foundation
- [ ] Read all 5 guides in this folder
- [ ] Learn all 16 Leadership Principles by name
- [ ] Write 1 STAR story per LP
- [ ] Compile Amazon resume (Overleaf)
- [ ] Set up AWS free tier account
- [ ] Solve 10 LeetCode easy problems (timed)

### Week 2 — Technical Buildup
- [ ] Learn AWS core services (EC2, S3, Lambda, SQS, DynamoDB)
- [ ] Write a 2nd STAR story for top 8 LPs
- [ ] Solve 20 LeetCode medium problems
- [ ] Design URL Shortener + S3 from scratch (write it out)
- [ ] Apply to Amazon via wfcareers equivalent (amazon.jobs)

### Week 3 — Mock Interviews
- [ ] Solve 15 more LeetCode problems (medium + 3 hard)
- [ ] Design 3 more systems (Notification, Order Processing, Rate Limiter)
- [ ] Do 2 mock LP behavioral interviews (Pramp)
- [ ] Do 1 mock coding interview (timed, no hints)
- [ ] Practice LP stories until each is under 3 minutes

### Week 4 — Final Prep + Apply
- [ ] Do 1 full loop simulation (2 coding + 1 SD + 1 LP round)
- [ ] Refine resume based on target job description keywords
- [ ] Prepare 5 questions per interview round
- [ ] Research the specific team you're interviewing with
- [ ] Rest the day before

---

## Final Words

> Amazon is hard. The bar is genuinely high. But your experience is real, your projects are impressive, and your impact is quantified. The gap between you and getting an offer is:
>
> 1. **Mastering 16 Leadership Principles** — this is a learnable skill
> 2. **Coding consistently** — 50 LeetCode problems is a 3-week commitment
> 3. **Practicing system design** — you already think this way; you just need to structure it
>
> The engineers who succeed at Amazon are not necessarily smarter than you — they prepared more deliberately. Follow this plan and you have a genuine shot.

---

**Good luck. Start with `02_LEADERSHIP_PRINCIPLES.md`. That is your #1 priority.**

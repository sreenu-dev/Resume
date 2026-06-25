# Amazon Interview Process — What Exactly Happens

## The Full Amazon "Loop"

Amazon's interview process is called the **"Loop"** — a series of back-to-back interviews in a single day (virtual or in-person). Here is the exact sequence:

```
Stage 1: Recruiter Screen (30 min)
         ↓
Stage 2: Online Assessment — OA (90 min, 2 coding problems)
         ↓
Stage 3: Technical Phone Screen (60 min)
         ↓
Stage 4: THE LOOP — Virtual On-Site (5–7 rounds, same day)
   ├── Round 1: Coding (45–60 min)
   ├── Round 2: Coding (45–60 min)
   ├── Round 3: System Design (60 min)
   ├── Round 4: Leadership Principles / Behavioral (45–60 min)
   ├── Round 5: Bar Raiser (60 min) ← MOST CRITICAL
   ├── Round 6: Hiring Manager (45–60 min)
   └── (Sometimes) Round 7: Domain-specific / extra coding
         ↓
Stage 5: Debrief + Hiring Decision (3–10 business days)
         ↓
Stage 6: Offer / Rejection
```

**Total timeline from application to offer: 4–8 weeks**

---

## Stage 1: Recruiter Screen (30 min)

### What Happens:
A recruiter (not an engineer) calls you to assess basic fit.

### Topics Covered:
- Brief background and experience walkthrough
- Why Amazon?
- Which team/org you're targeting
- Current location and relocation preferences
- Compensation expectations (sometimes)
- Timeline availability for the loop

### How to Prepare:
- [ ] Have a 90-second pitch ready: "I'm Sreenivasulu, full-stack engineer with 5+ years at Deloitte and Accenture, specializing in distributed systems and event-driven architecture..."
- [ ] Know why Amazon specifically (not just "it's a great company")
- [ ] Research the specific team if known
- [ ] Know your salary expectations (research levels.fyi for Amazon SDE2/SDE3 India)

### Your "Why Amazon" Answer:
```
"I'm drawn to Amazon's customer-obsession culture and the engineering 
challenges that come with building at internet scale. My work on 
distributed event-driven systems and Kafka-based architectures at 
Deloitte has given me a taste of high-throughput systems design, and 
I want to operate at a scale where millions of customers depend on 
what I build every day. The Leadership Principles also resonate deeply 
with how I already operate — especially Ownership and Dive Deep."
```

---

## Stage 2: Online Assessment (OA) — 90 Minutes

### What It Is:
An automated coding test on HackerRank or CodeSignal. You'll get **2 coding problems** and sometimes a short work simulation.

### Problem Difficulty:
- **Problem 1:** LeetCode Easy–Medium
- **Problem 2:** LeetCode Medium–Hard
- **Work Simulation (sometimes):** Multiple choice LP-based scenarios

### Key Rules:
- Time is strict — manage it carefully
- You cannot pause the clock
- Code needs to pass hidden test cases (not just examples)
- Code quality matters less than correctness + edge case handling
- **You can use any language** — use your strongest one

### OA Strategy:
```
First 5 minutes:
  - Read BOTH problems before starting either
  - Assess difficulty
  - Start with the easier one

For each problem:
  1. Read carefully (2 min)
  2. Write approach in comments (2 min)
  3. Code solution (20–30 min)
  4. Test with provided examples (5 min)
  5. Handle edge cases (5 min)
  6. Submit

Final 10 minutes:
  - If stuck on Problem 2, submit partial solution
  - Partial credit is better than no submission
```

### Most Common OA Problem Types:
1. **Arrays / Sliding Window** — frequency count, max subarray
2. **Strings** — palindrome, anagram, parsing
3. **Trees** — BFS level order, LCA, path sum
4. **Graphs** — BFS/DFS traversal, connected components
5. **Dynamic Programming** — knapsack, LCS, coin change
6. **Sorting / Searching** — custom comparator, binary search variant

### OA Practice Plan (1 Week):
```
Day 1: 10 easy problems (timed, 15 min each)
Day 2: 10 medium problems (timed, 25 min each)
Day 3: 5 hard problems (timed, 45 min each)
Day 4: Full mock OA (2 problems, 90 min total)
Day 5: Review mistakes + retry
Day 6: 5 more medium problems
Day 7: Full mock OA (simulate real conditions)
```

---

## Stage 3: Technical Phone Screen (60 min)

### Structure:
```
0–5 min:   Introductions
5–10 min:  Brief background walkthrough
10–50 min: 1 coding problem (live, on shared editor like Amazon Chime or CoderPad)
50–60 min: LP questions (2–3 behavioral questions)
```

### The Coding Problem:
- Usually LeetCode Medium
- You'll be asked to think out loud
- Interviewer may give hints — take them
- Clean, working code matters more than perfect optimization

### LP Questions in Phone Screen:
You'll get 2–3 behavioral questions. Common ones:
1. "Tell me about a time you took ownership of something outside your scope."
2. "Describe a situation where you had to deliver under significant time pressure."
3. "Tell me about the most complex technical problem you solved."

### How to Handle the Coding + LP Balance:
- If coding runs over → interviewer will cut it short for LPs
- Don't sacrifice LP answers — they matter as much as code

---

## Stage 4: The Loop — On-Site (Virtual or In-Person)

This is the main event. Usually **5–7 interviews** conducted in a single day with 10–15 minute breaks between.

### Typical Loop Structure for SDE2:

#### Round 1: Coding Interview (45–60 min)
**What to expect:**
- 1 coding problem, medium-hard difficulty
- Live coding on shared editor (CoderPad or Amazon's internal tool)
- Must think out loud the entire time
- Interviewer may probe your complexity analysis

**Common topics:** Arrays, Linked Lists, Trees, Graphs, Hashing

#### Round 2: Coding Interview (45–60 min)
**What to expect:**
- Another coding problem, often harder than Round 1
- May include follow-up: "What if the data doesn't fit in memory?"
- Tests ability to optimize after first solution

**Common topics:** Dynamic Programming, Binary Search, Sliding Window

#### Round 3: System Design (60 min)
**What to expect:**
- Design a large-scale system from scratch
- You drive the conversation with a whiteboard (virtual)
- Interviewer probes your scalability/availability/trade-off decisions

**Common problems:**
- Design Amazon S3
- Design a URL shortener
- Design a notification system
- Design an e-commerce order system
- Design a distributed cache

**Scoring criteria:**
- Clarifying requirements
- Estimating scale
- Component design
- Data model
- Scalability and trade-offs

#### Round 4: Behavioral / Leadership Principles (45–60 min)
**What to expect:**
- 4–6 behavioral questions, all LP-based
- Uses STAR format
- Interviewers follow up with "Tell me more", "What was YOUR role?", "What was the impact?"

**Note:** Every interviewer in the loop is assigned 2–3 LPs to assess. They will ask multiple questions about those LPs.

#### Round 5: Bar Raiser (60 min) — MOST IMPORTANT
**What it is:**
The Bar Raiser is a specially trained senior Amazon employee (from a different team) whose sole job is to ensure the hiring bar is maintained or raised. They have **veto power** — even if all other interviewers vote "hire", a Bar Raiser "no hire" kills the offer.

**What the Bar Raiser assesses:**
- Are you at or above the 50th percentile of current Amazon engineers at this level?
- Do your LP stories show genuine depth and self-awareness?
- Do you "dive deep" rather than give surface answers?
- Are you a long-term Amazon hire or a short-term fit?

**Bar Raiser questions are probing and follow-up heavy:**
- "You said you did X — walk me through exactly how you did that."
- "What would you have done differently?"
- "Who else was involved and what was YOUR specific contribution?"
- "What was the failure mode you didn't anticipate?"

**Bar Raiser Strategy:**
- Never lie or exaggerate — they probe hard
- Use genuine stories with real failures and learnings
- Show self-awareness and growth mindset
- Demonstrate depth: when they ask "tell me more", have more to say

#### Round 6: Hiring Manager (45–60 min)
**What to expect:**
- Mix of LP questions and role-specific discussion
- Will discuss day-to-day of the role
- May discuss your career goals
- Usually ends with "Do you have questions for me?"

**Your job in this round:**
- Show you understand the team's mission
- Ask thoughtful questions about the team, tech stack, challenges
- Express genuine enthusiasm for the specific role

---

## Stage 5: Debrief and Decision

### What Happens After the Loop:
1. Each interviewer submits a written debrief with:
   - Hire / No Hire / Strong Hire / Strong No Hire
   - Specific LP ratings
   - Technical assessment
2. The Bar Raiser reviews all feedback
3. A debrief meeting is held (usually within 24–48 hours)
4. Decision made: Hire, No Hire, or "raise the bar" (apply for higher level)

### Timeline to Offer:
- Decision: 3–5 business days post-loop
- Offer letter: 5–10 business days post-loop
- Negotiation window: Usually 3–5 business days after offer

---

## Amazon Salary at SDE2 Level (India — Hyderabad/Bangalore)

> *Note: These are approximate ranges based on industry data. Verify at levels.fyi*

| Component | SDE2 Range | SDE3 Range |
|---|---|---|
| Base Salary | ₹40–60 LPA | ₹65–90 LPA |
| Signing Bonus | ₹5–15 LPA | ₹10–25 LPA |
| RSUs (over 4 years) | ₹30–60 LPA (annualized) | ₹60–120 LPA (annualized) |
| **Total Comp (TC)** | **₹80–130 LPA** | **₹130–220 LPA** |

### Amazon Vesting Schedule (RSUs):
```
Year 1: 5% vest
Year 2: 15% vest
Year 3: 40% vest
Year 4: 40% vest
```
**Warning:** Most of your compensation comes in Years 3–4. This is intentional retention.

### Negotiation Tips:
- **Always negotiate** — Amazon expects it
- Counter with a specific number, not a range
- RSUs are negotiable; base salary less so
- Signing bonus is most negotiable
- Use competing offers (real or pending) as leverage

---

## Amazon Interview — Unique Rules

### The "Raise the Bar" Principle:
Every hire must be better than the median current Amazon employee at that level. Interviewers ask: "Would I be proud to have this person on my team?"

### No Collaborative Hints Early:
Unlike Google/Meta, Amazon interviewers often let you struggle longer before hinting. This is intentional — they want to see your problem-solving process.

### Multiple LPs Assessed Per Interview:
Each interviewer is assigned 2–3 specific LPs to probe. They will ask targeted questions about those LPs, not random ones.

### Written Feedback Matters:
Interviewers write detailed feedback — they can't just say "good candidate." Your stories need to be detailed enough that interviewers can write compelling notes about them.

### You Can Reference Projects:
Your personal projects (Distributed Ledger, GenAI Compliance Engine, API Gateway) are fair game and highly encouraged to bring up. They demonstrate initiative.

---

## Common Reasons Amazon Candidates Fail

| Failure Mode | How to Avoid |
|---|---|
| Weak LP stories — no metrics | Every story needs a number |
| Saying "we" instead of "I" | Practice using first person |
| Vague behavioral answers | Use STAR strictly |
| Brute-force only coding solution | Always discuss time/space complexity and optimize |
| Failing to clarify requirements in system design | Always spend 5 min on requirements |
| Not driving the system design conversation | You lead, interviewer probes |
| Lying or exaggerating in Bar Raiser | They will probe until they find inconsistency |
| Negative talk about previous employers | Never speak negatively |
| No questions for interviewer | Prepare 3–4 thoughtful questions per round |

---

## Questions to Ask Each Interviewer

### For Coding Interviewers:
1. "What's the biggest technical challenge your team is working on right now?"
2. "How does the team approach code quality and technical debt?"
3. "What does the on-call rotation look like for this team?"

### For System Design Interviewer:
1. "What scale does this service operate at in production?"
2. "What were the most interesting architectural decisions the team had to make?"
3. "How does the team balance velocity with reliability?"

### For Bar Raiser:
1. "What qualities do the most successful engineers at this level have at Amazon?"
2. "What does 'raising the bar' look like in day-to-day engineering here?"

### For Hiring Manager:
1. "What would success look like for someone in this role in the first 6 months?"
2. "What's the biggest opportunity for the person in this role to make a major impact?"
3. "How does this team's work connect to Amazon's broader customer mission?"

---

**Next:** Read `04_TECHNICAL_CODING_GUIDE.md`

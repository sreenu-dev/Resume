# Technical Interview Guide — Optum (Coding + Healthcare System Design)

## Optum Coding Interview — What's Different

| Dimension          | Google             | Schwab                       | ServiceNow             | **Optum**                                                     |
| --------------------| --------------------| ------------------------------| ------------------------| ---------------------------------------------------------------|
| Language           | Python/Java        | Java/C#/Python               | Java/Python            | **Java or Python**                                            |
| Difficulty         | Hard               | Medium                       | Medium                 | **Easy–Medium**                                               |
| Code style         | Optimal            | Financial-grade              | Enterprise             | **Correct + readable + tested**                               |
| Critical follow-up | "Optimize further" | "What if transaction fails?" | "Scale to enterprise?" | **"How would you protect PHI?" / "How would you test this?"** |
| Domain context     | None               | Finance                      | Enterprise workflows   | **Healthcare / PHI**                                          |
| Data focus         | None               | Financial data               | Workflow data          | **Healthcare data at scale**                                  |

---

## Optum's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern | Optum Healthcare Follow-up |
|---|---|---|---|
| 1 | Two Sum | HashMap | How would you handle PHI in the input data? |
| 2 | Valid Parentheses | Stack | How would you validate HL7 message structure? |
| 3 | LRU Cache | DLL + HashMap | How would you ensure PHI in the cache is encrypted? |
| 4 | Merge Intervals | Sorting | How would you merge overlapping coverage periods for a member? |
| 5 | Number of Islands | DFS/BFS | How would you scale this to millions of patient records? |
| 6 | Binary Tree Level Order | BFS | How would you handle null values in medical records? |
| 7 | Find Median from Data Stream | Two Heaps | How would you use this for real-time lab value analytics? |
| 8 | Design Rate Limiter | Token Bucket | How would you rate-limit per provider accessing member PHI? |
| 9 | Longest Substring | Sliding Window | How would you handle medical code sequences? |
| 10 | Clone Graph | BFS + HashMap | How would you model a provider referral network? |
| 11 | Course Schedule | Topological Sort | How would you detect cyclic prior auth approval chains? |
| 12 | Serialize/Deserialize Tree | BFS | How would you ensure PHI is masked in serialized output? |
| 13 | Design HashMap | Hashing | How would you make it thread-safe for concurrent claim updates? |
| 14 | Kth Largest Element | Heap | How would you use this for identifying top-cost members? |
| 15 | Word Ladder | BFS | What is the complexity for large medical ontology graphs? |

### Optum-Specific Coding Variants
```
Standard: "Design a Rate Limiter"
Optum variant: "Design a rate limiter for Optum's FHIR API that 
                limits each provider to 1,000 PHI record requests 
                per hour, logs all accesses for HIPAA audit purposes, 
                and masks PHI in error responses."

Standard: "LRU Cache"
Optum variant: "Design a caching layer for frequently accessed member 
                health records. The cache must: encrypt all PHI at rest, 
                log every cache access with user ID and timestamp (HIPAA 
                audit requirement), and automatically invalidate entries 
                when a member's record is updated."
```

---

## Healthcare System Design — 6 Deep Dives

### The HIPAA Compliance Template (Use in EVERY design)

**Before starting any Optum system design, state this:**
```
"For any Optum system handling PHI, I will enforce:
  1. Encryption: PHI encrypted at rest (AES-256) and in transit (TLS 1.3)
  2. Access Control: RBAC — minimum necessary access to PHI
  3. Audit Log: every PHI access logged (who, what, when) — HIPAA requirement
  4. De-identification: PHI masked/removed in non-production environments
  5. Data Retention: governed by HIPAA (6 years for health records)
  6. Breach Detection: automated alerting on unauthorized PHI access patterns"
```

---

### Problem 1: Design a Prior Authorization System (MOST IMPORTANT)

**Context:** One of Optum's highest-priority AI automation targets. Prior auth is the process by which an insurer approves or denies a medical procedure before it happens.

**Requirements:**
```
Functional:
  - Provider submits prior auth request (patient, diagnosis, procedure)
  - AI engine evaluates request against clinical guidelines
  - Auto-approve obvious cases, route borderline to human reviewer
  - Provider receives decision in real time (< 2 hours for urgent)
  - Full audit trail of every decision

Non-Functional:
  - 500,000 prior auth requests per day
  - Urgent requests: < 4 hours response
  - Standard requests: < 72 hours response
  - HIPAA compliance: full PHI protection throughout
  - 99.9% availability
```

**Architecture:**
```
Provider Portal (React / Angular SPA):
  - Provider logs in (OAuth2, MFA)
  - Submits: patient member ID, ICD-10 diagnosis code, CPT procedure code,
    clinical notes (PDF), supporting documentation

Auth Request Service:
  - Validates request (member exists, provider is in-network)
  - Persists request to DB (encrypted PHI)
  - Publishes AuthRequestSubmitted to Kafka

AI Clinical Decision Engine (Your Agentic AI angle!):
  Multi-agent orchestration:
    - Intake Agent: extracts structured data from clinical notes (NLP)
    - Guideline Agent: checks clinical guidelines database
      (e.g., InterQual, MCG) for this diagnosis+procedure combo
    - History Agent: queries member's claim history for context
    - Decision Agent: produces recommendation + confidence score
  → Score > 0.9: AUTO_APPROVE
  → Score 0.4–0.9: ROUTE_TO_HUMAN (with AI summary + recommendation)
  → Score < 0.4: AUTO_DENY (with reason code)

Human Review Queue:
  - Clinical reviewers (nurses, MDs) see: AI summary, supporting docs,
    member history, AI recommendation
  - Decision sent back to Decision Service

Notification Service:
  - Notifies provider of decision via portal + fax (yes, healthcare still uses fax)
  - Notifies member via portal + letter (regulatory requirement)

Audit Log:
  - Every step logged: who accessed PHI, what decision was made, when
  - Immutable, encrypted, retained 6 years (HIPAA)
  - CMS reporting (government compliance)
```

**Your Differentiator:**
> "I've built a multi-agent AI pipeline that uses exactly this architecture — intake agent, classification agent, decision agent, and human-in-the-loop escalation when confidence is below threshold. The compliance guardrail engine I built at Deloitte is structurally the same pattern as prior authorization AI."

---

### Problem 2: Design a Medical Claims Processing Pipeline

**Context:** Optum processes billions of claims per year — the financial transactions of the healthcare system.

**Requirements:**
```
Functional:
  - Receive claims from providers (EDI 837 format or REST API)
  - Validate claim (member enrolled, procedure covered, no duplicate)
  - Adjudicate claim (calculate payment amount per contract)
  - Generate EOB (Explanation of Benefits) for member
  - Generate payment (EDI 835 remittance to provider)

Non-Functional:
  - 1 billion claims per year (3M/day)
  - Exactly-once processing (duplicate payment = financial loss)
  - Full audit trail (HIPAA + financial compliance)
  - 99.99% availability
```

**Architecture:**
```
Claim Ingestion:
  - EDI 837 files (batch, from large hospitals) → EDI Parser → normalized Claim objects
  - REST API (real-time, from smaller providers) → API Gateway → Claim objects
  - All claims published to Kafka: "claims-raw" topic

Validation Service (Kafka consumer):
  - Member eligibility check (is member enrolled on date of service?)
  - Duplicate detection (idempotency key: provider + member + date + CPT code)
  - Coverage check (is this procedure covered by their plan?)
  - Publishes: ClaimValidated or ClaimRejected

Adjudication Engine:
  - Subscribes to ClaimValidated
  - Calculates payment: (billed amount × contract rate) − deductible − copay
  - Applies COB (Coordination of Benefits) if member has multiple plans
  - Publishes: ClaimAdjudicated with payment amount

EOB Generator:
  - Generates Explanation of Benefits document per member
  - Delivered via member portal + mail

Payment Service:
  - Generates EDI 835 remittance advice to provider
  - Initiates ACH payment

Audit Log (Critical for HIPAA + Financial Compliance):
  - Every step of every claim logged immutably
  - PHI in logs encrypted + access-controlled
  - Retained per HIPAA (6 years) and financial regulations
```

**Key Points:**
- **Idempotency key** prevents paying a claim twice (financial catastrophe)
- **EDI 837/835** — mention knowing these formats exists (differentiator)
- **Audit trail** — both HIPAA (PHI) and financial compliance (payment records)

---

### Problem 3: Design a Patient Health Record (EHR) API Platform

**Context:** Optum needs a FHIR-compliant API platform for sharing health records across providers.

**Requirements:**
```
- FHIR R4-compliant REST APIs (Patient, Observation, Condition, MedicationRequest)
- CMS interoperability rule: patients can export their data
- Fine-grained access control (patient consents to specific provider access)
- Audit logging of all PHI access
- Real-time updates when records change (webhooks / FHIR subscriptions)
```

**Architecture:**
```
FHIR API Gateway:
  - Validates OAuth2 SMART on FHIR tokens
  - Enforces patient consent (only authorized providers can access)
  - Logs every access: provider, patient, resource type, timestamp

FHIR Resource Store:
  - Stores FHIR R4 resources (JSON) in a document store (MongoDB)
  - Indexed by: patient ID, resource type, last updated
  - De-identified copy for analytics (PHI removed)

Write Path:
  - Provider EHR → FHIR Resource → Validation → Store → Publish ChangeEvent to Kafka

Read Path (FHIR Search API):
  - GET /Patient/{id}/Observation?code=blood-pressure&date=2025
  - Check consent, check access control, fetch from store, return

Patient Data Export (CMS Interoperability Rule):
  - Patients can request full export of their records
  - Bulk FHIR export: async job → NDJSON files → download link

Audit Trail:
  - Who accessed what record, when — immutable, encrypted
  - Required by HIPAA and ONC (Office of National Coordinator) rules
```

---

### Problem 4: Design a Healthcare Fraud Detection System

**Context:** Optum loses billions to claims fraud annually. ML-based detection is critical.

**Requirements:**
```
- Detect fraudulent claims in near real-time (before payment)
- Pattern types: billing for services not rendered, upcoding, phantom providers
- < 1% false positive rate (legitimate claims blocked is costly)
- Full audit trail of every fraud decision (regulatory requirement)
```

**Architecture:**
```
Claims Stream (Kafka → Fraud Detection topic)

Feature Extraction Service:
  - Provider profile: billing history, specialty, typical procedures
  - Member profile: diagnosis history, typical utilization
  - Claim features: is this procedure consistent with diagnosis?
  - Velocity: unusual spike in claims from this provider?

ML Scoring Service:
  - Model: XGBoost / LightGBM trained on labeled fraud cases
  - Returns: fraud_probability score
  - Response time: < 200ms

Rule Engine:
  - Hard rules: instant flag (provider on exclusion list, impossible 
    procedure dates, billing for deceased member)

Decision Engine:
  - Score > 0.85: HOLD for manual review
  - Score > 0.95: AUTO_DENY + SIU (Special Investigations Unit) alert
  - Score < 0.85: ALLOW (continue through claims pipeline)

Audit Log:
  - Every fraud decision logged (score, features, decision, reviewer)
  - Required for SIU case management and regulatory reporting
```

---

### Problem 5: Design a Population Health Analytics Platform

**Context:** Optum's OptumIQ product — identify high-risk members before they get sicker.

**Requirements:**
```
- Ingest: claims data, lab results, pharmacy records, social determinants
- Identify high-risk members (likely to have costly health events)
- Generate outreach recommendations per member
- Power dashboards for care managers
- HIPAA-compliant throughout
```

**Architecture:**
```
Data Ingestion (batch + streaming):
  - Claims: daily batch from claims pipeline → S3 → Spark ETL
  - Labs: HL7 messages from lab systems → Kafka → real-time ingestion
  - Pharmacy: EDI 835 → batch ingestion
  - SDOH: social determinants (zip code, income) from third-party sources

Data Lake (AWS S3 + Databricks):
  - Raw layer: unprocessed source data
  - Curated layer: cleaned, standardized, de-identified for analytics
  - Gold layer: ML-ready feature tables per member

ML Pipeline (Databricks + Python):
  - Risk stratification model: predicts probability of ED visit, hospitalization
  - Feature engineering: comorbidities, medication adherence, utilization patterns
  - Output: risk score per member, updated daily

Care Management Platform:
  - Dashboards: care managers see member risk scores, alerts, gaps in care
  - Outreach recommendations: "call this member about medication adherence"
  - HIPAA-compliant: all PHI encrypted, access-controlled, audit-logged

FHIR Export:
  - Risk scores and care gaps exported via FHIR API to provider EHRs
```

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding + Healthcare Context
  Mon: 5 medium problems (Two Sum, Merge Intervals, BFS, LRU Cache)
  Tue: 5 medium problems (Heaps, Sliding Window)
  Wed: Practice the HIPAA compliance template (recite it)
  Thu: 5 medium problems (Graphs, Topological Sort)
  Fri: Full mock (45 min, 1 problem + "how would you protect PHI?")
  Sun: Rest

Week 2: Healthcare System Design
  Mon: Design Prior Authorization System (60 min — most important)
  Tue: 5 coding problems + healthcare terminology review
  Wed: Design Medical Claims Processing Pipeline (60 min)
  Thu: Design Patient Health Record / FHIR Platform (60 min)
  Fri: Full coding mock + project deep dive
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (1 coding + 1 SD + 1 behavioral)
  Tue: Design Fraud Detection System (60 min) — your AI angle
  Wed: 5 medium-hard problems
  Thu: Full behavioral mock (ICRIP values, 30 min, 5 questions)
  Fri: Design Population Health Analytics Platform (50 min)
  Sun: Rest
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**

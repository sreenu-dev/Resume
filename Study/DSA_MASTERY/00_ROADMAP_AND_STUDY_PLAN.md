# DSA Mastery — Complete Roadmap & Study Plan
## For FAANG / Top Product Company Technical Interviews

> **Audience:** Experienced engineers with coding fundamentals. Every file assumes you know what a loop is. Nothing below is basic.
>
> **Philosophy:** You are not learning DSA. You are learning *how interviewers test DSA* — which is a different skill. Pattern recognition beats memorization every single time.

---

## The Full Hierarchy — 65 Files Across 19 Topic Areas

```
DSA_Mastery/
├── 00_ROADMAP_AND_STUDY_PLAN.md                   ← You are here
│
├── 01_Complexity_Analysis/                        FOUNDATION
│   ├── 01_Asymptotic_Analysis_Mastery.md
│   ├── 02_Amortized_Analysis.md
│   └── 03_Space_Time_Tradeoffs.md
│
├── 02_Arrays_And_Strings/                         HIGH FREQUENCY
│   ├── 01_Two_Pointers_Mastery.md
│   ├── 02_Sliding_Window_Mastery.md
│   ├── 03_Prefix_Sum_And_Difference_Arrays.md
│   ├── 04_Monotonic_Stack_And_Queue.md
│   └── 05_Kadane_And_Subarray_Problems.md
│
├── 03_Hashing/                                    HIGH FREQUENCY
│   ├── 01_Hash_Maps_Advanced.md
│   └── 02_Rolling_Hash_And_Rabin_Karp.md
│
├── 04_Binary_Search/                              HIGH FREQUENCY
│   ├── 01_Binary_Search_On_Answer.md
│   └── 02_Advanced_Binary_Search_Patterns.md
│
├── 05_Linked_Lists/                               MEDIUM FREQUENCY
│   ├── 01_Fast_Slow_Pointers_And_Cycles.md
│   └── 02_Reversal_And_Merge_Operations.md
│
├── 06_Stacks_And_Queues/                          HIGH FREQUENCY
│   ├── 01_Monotonic_Structures_Deep_Dive.md
│   └── 02_Design_Problems.md
│
├── 07_Trees/                                      VERY HIGH FREQUENCY
│   ├── 01_Advanced_Tree_Traversals.md
│   ├── 02_BST_Advanced_Operations.md
│   ├── 03_Lowest_Common_Ancestor.md
│   ├── 04_Segment_Tree.md
│   ├── 05_Fenwick_Tree_BIT.md
│   ├── 06_Trie.md
│   ├── 07_Balanced_BST_Concepts.md
│   └── 08_Heavy_Light_Decomposition.md
│
├── 08_Heaps/                                      HIGH FREQUENCY
│   ├── 01_Heap_Patterns_K_Problems.md
│   └── 02_Two_Heaps_Median_Stream.md
│
├── 09_Graphs/                                     VERY HIGH FREQUENCY
│   ├── 01_BFS_Advanced_Patterns.md
│   ├── 02_DFS_Advanced_Patterns.md
│   ├── 03_Topological_Sort_And_DAGs.md
│   ├── 04_Dijkstra_And_Shortest_Paths.md
│   ├── 05_Bellman_Ford_And_Floyd_Warshall.md
│   ├── 06_Union_Find_DSU_Advanced.md
│   ├── 07_Minimum_Spanning_Tree.md
│   ├── 08_Strongly_Connected_Components.md
│   ├── 09_Bipartite_And_Matching.md
│   └── 10_Network_Flow.md
│
├── 10_Dynamic_Programming/                        HIGHEST FREQUENCY
│   ├── 00_DP_Pattern_Recognition_Guide.md
│   ├── 01_1D_DP_Patterns.md
│   ├── 02_2D_DP_And_Grid.md
│   ├── 03_Knapsack_Variants.md
│   ├── 04_LCS_LIS_Edit_Distance.md
│   ├── 05_Interval_DP.md
│   ├── 06_Tree_DP.md
│   ├── 07_Bitmask_DP.md
│   ├── 08_Digit_DP.md
│   └── 09_DP_Optimizations_CHT_Knuth.md
│
├── 11_String_Algorithms/                          MEDIUM-HIGH FREQUENCY
│   ├── 01_KMP_And_Z_Algorithm.md
│   ├── 02_Aho_Corasick.md
│   ├── 03_Suffix_Array_And_LCP.md
│   └── 04_Manacher_Palindrome.md
│
├── 12_Sorting_And_Searching/                      MEDIUM FREQUENCY
│   ├── 01_Advanced_Sorting.md
│   └── 02_Quickselect_And_Order_Statistics.md
│
├── 13_Mathematical_Algorithms/                    MEDIUM FREQUENCY
│   ├── 01_Number_Theory.md
│   ├── 02_Bit_Manipulation_Mastery.md
│   ├── 03_Combinatorics.md
│   └── 04_Matrix_Exponentiation.md
│
├── 14_Recursion_Backtracking_DC/                  HIGH FREQUENCY
│   ├── 01_Backtracking_Patterns.md
│   ├── 02_Pruning_Techniques.md
│   └── 03_Divide_And_Conquer_Advanced.md
│
├── 15_Greedy_Algorithms/                          MEDIUM-HIGH FREQUENCY
│   ├── 01_Greedy_Proof_Techniques.md
│   └── 02_Interval_Scheduling_And_Greedy.md
│
├── 16_Advanced_Data_Structures/                   GOOGLE / COMPETITIVE
│   ├── 01_Sparse_Table_And_RMQ.md
│   ├── 02_Sqrt_Decomposition.md
│   ├── 03_Persistent_Data_Structures.md
│   └── 04_LRU_LFU_Cache_Design.md
│
├── 17_System_Design_In_Coding/                    ALL COMPANIES
│   ├── 01_LLD_For_Technical_Interviews.md
│   └── 02_Design_Patterns_In_Code.md
│
├── 18_Interview_Meta_Skills/                      MULTIPLIER
│   ├── 01_Problem_Solving_Framework.md
│   ├── 02_Pattern_Recognition_Cheatsheet.md
│   ├── 03_Time_Management_Strategy.md
│   └── 04_Edge_Cases_Bible.md
│
└── 19_Company_Patterns/                           TARGETING
    ├── 01_Google_Patterns.md
    ├── 02_Amazon_Patterns.md
    ├── 03_Meta_Patterns.md
    ├── 04_Microsoft_Patterns.md
    └── 05_Other_Top_Companies.md
```

---

## The Professor's Doctrine: 5 Laws of Mastery

### Law 1: Patterns Over Problems
Solving 500 LeetCode problems at random is preparation theater. Solving 80 problems that cover 15 patterns is actual preparation. Every problem you solve should teach you a **generalizable pattern** — not just a solution to that specific problem.

### Law 2: Complexity Is Non-Negotiable
Every solution you write, you must be able to say: "This is O(N log N) time and O(N) space, because..." before your interviewer asks. Candidates who cannot analyze their own complexity are disqualified immediately at top companies.

### Law 3: Code Is Secondary to Thinking
The code is the easy part. The hard part is: reducing the problem, choosing the right data structure, identifying the invariant, and communicating the approach. Spend 40% of your time thinking before writing anything.

### Law 4: The Three-Layer Model
For every algorithm you study, understand it at three layers:
- **Layer 1 — Mechanics:** How does it work? Can you implement it from scratch?
- **Layer 2 — Invariant:** What property does the algorithm maintain at every step? This is what you say when the interviewer asks "why does this work?"
- **Layer 3 — Variations:** What if the problem changes slightly? Can you adapt?

### Law 5: Edge Cases Win Offers
Most candidates who get the "right" solution still fail because they don't handle edge cases. The last 10 minutes of every practice session should be dedicated to edge cases and corner cases specifically.

---

## Recommended Study Schedule

### Phase 1 — Core Patterns (Weeks 1–4)
*Master these before everything else — they appear in 70% of all FAANG interviews.*

| Week | Topics | Files | Goal |
|---|---|---|---|
| 1 | Complexity + Two Pointers + Sliding Window + Prefix Sum | 01/\*, 02/01-03 | Internalize O(N) linear scan patterns |
| 2 | Binary Search + Hashing + Monotonic Stack | 03/\*, 04/\*, 02/04-05 | "Reduce search space" thinking |
| 3 | Trees — Traversals + BST + LCA | 07/01-03 | Recursive tree thinking fluency |
| 4 | Graphs — BFS + DFS + Topological + Union-Find | 09/01-03, 09/06 | Graph problem recognition |

### Phase 2 — Dynamic Programming (Weeks 5–7)
*The topic that separates candidates who clear L5 from those who don't.*

| Week | Topics | Files | Goal |
|---|---|---|---|
| 5 | DP Pattern Recognition + 1D + 2D | 10/00, 10/01-02 | State definition fluency |
| 6 | Knapsack + LCS/LIS + Interval DP | 10/03-05 | Multi-dimension DP |
| 7 | Tree DP + Bitmask DP + Digit DP | 10/06-08 | Advanced DP variants |

### Phase 3 — Advanced Topics (Weeks 8–10)
*Differentiators for Senior SWE / L5+ interviews.*

| Week | Topics | Files | Goal |
|---|---|---|---|
| 8 | Segment Tree + BIT + Trie + Heap patterns | 07/04-06, 08/\* | Range query mastery |
| 9 | String algorithms + Shortest paths + MST | 09/04-07, 11/\* | Classic algorithm fluency |
| 10 | Greedy + Backtracking + Divide & Conquer | 14/\*, 15/\* | Proof-based reasoning |

### Phase 4 — Polish & Company-Specific (Weeks 11–12)
| Week | Topics | Files | Goal |
|---|---|---|---|
| 11 | Math algorithms + Bit manipulation + Advanced DS | 13/\*, 16/\* | Competitive-level completeness |
| 12 | Interview meta-skills + Company patterns | 18/\*, 19/\* | Real interview simulation |

---

## Frequency Map — What Appears Most Often

```
████████████████████  Dynamic Programming         (every company, every level)
███████████████████   Graph Algorithms            (Google, Meta, Amazon)
██████████████████    Trees (all variants)        (every company)
█████████████████     Arrays / Two-Pointer        (every company, warm-up)
████████████████      Binary Search               (Google especially)
███████████████       Backtracking                (Meta, Amazon, Microsoft)
██████████████        Sliding Window              (every company)
█████████████         Heap / Priority Queue       (Amazon, Meta)
████████████          String Algorithms           (Google)
███████████           Bit Manipulation            (all companies, medium-hard)
██████████            Greedy                      (all companies)
█████████             Segment Tree / BIT          (Google hard rounds)
████████              Math / Number Theory        (Google, competitive)
███████               Trie                        (Amazon, system design rounds)
██████                Network Flow                (Google senior+)
█████                 Persistent DS               (Google L6+, rare)
```

---

## The Interview Problem Taxonomy

```
EASY (Warm-up): Arrays, HashMap, Two-Pointer, BFS/DFS level-order
     ↓
MEDIUM (Core): Sliding Window, Binary Search, BST, DP (1D/2D), Graph patterns
     ↓
HARD (Differentiator): Segment Tree, Complex DP, Topological variants, 
                        Backtracking with pruning, Shortest path variants
     ↓
VERY HARD (Google/Meta L6+): Network Flow, Persistent DS, 
                               DP Optimization (CHT), HLD, Suffix Array
```

---

## Company-Level Difficulty Profile

| Company | Level | Primary Focus | Hard Differentiator |
|---|---|---|---|
| **Google** | Highest | Algorithms, complexity, graphs, DP | Network flow, advanced DP optimization, string algorithms |
| **Meta** | High | Trees, graphs, backtracking, DP | Complex graph problems, multi-dimensional DP |
| **Amazon** | Medium-High | LP + coding: arrays, trees, DP basics | System design for SDEs; heap + graph problems |
| **Microsoft** | Medium-High | OOP + algorithms: trees, graphs, DP | Recursion depth, design problems |
| **Apple** | High | Algorithms + system design mix | Clean code + edge cases |
| **Netflix** | High | Algorithms + system design | Scale-aware solutions |
| **Uber/Lyft** | Medium-High | Graphs (routing), location, DP | Dijkstra variants, real-world graph problems |
| **Stripe** | High | Security + algorithms | API design + algorithmic thinking |

---

## How to Read Each File

Every file in this library follows this structure:
1. **Core Concept** — The mathematical/algorithmic invariant (the "why it works")
2. **Algorithm Template** — Reusable code patterns, not one-off solutions
3. **Classic Problems** — 4–8 landmark problems with full solutions + complexity
4. **Variations** — How the pattern mutates across different problem types
5. **Edge Cases** — A specific list of what breaks naive implementations
6. **Interview Simulation** — How an interviewer will test this in a real interview

---

## Pre-Interview Checklist (Day Before)

### Mindset
- [ ] Review your personal "solved problems" notes — your own patterns, not someone else's
- [ ] Re-read `18_Interview_Meta_Skills/01_Problem_Solving_Framework.md`
- [ ] Review `18_Interview_Meta_Skills/02_Pattern_Recognition_Cheatsheet.md`

### Technical
- [ ] Refresh: Two-pointer, Sliding window, BFS, DFS, DP state definition
- [ ] Review complexity of: Dijkstra (ElogV), Segment Tree (NlogN build, logN query), BFS/DFS (V+E)
- [ ] Recall your 3 strongest solved problems with metrics

### Interview Day Protocol
```
0–2 min:   Restate the problem + clarify constraints
           "Am I understanding this correctly — we have N nodes, 
            M edges, and we want...?"
           "What are the constraints on N? Up to 10^5? 10^9?"
           
2–5 min:   Think out loud about approaches
           "A brute force would be O(N²) — I can see a path to O(N log N) 
            using [data structure]. Let me think through the invariant..."
           
5–8 min:   State your approach before coding
           "My approach: [name the pattern]. Here's why it works: 
            at every step, the invariant maintained is [X]."
           
8–30 min:  Code — cleanly, with named helper functions
           
30–35 min: Test with examples (including edge cases)
           
35–40 min: Complexity analysis — always volunteer this
           "Time: O([X]) because [reason]. Space: O([Y]) because [reason]."
```

---

## The 15 Patterns That Cover 80% of All Problems

| # | Pattern | Key Insight | Trigger Signal |
|---|---|---|---|
| 1 | Two Pointers | Eliminate search space from both ends | Sorted array, pair sum, palindrome |
| 2 | Sliding Window | Fixed/variable size subarray | "subarray/substring of length K" |
| 3 | Prefix Sum | O(1) range queries after O(N) build | Range sum queries, subarray with condition |
| 4 | Binary Search on Answer | Search the answer space, not data | "Minimize the maximum," "Is X achievable?" |
| 5 | Monotonic Stack | Maintain sorted order in O(N) | Next greater/smaller, span problems |
| 6 | BFS for Shortest Path | Level-by-level expansion guarantees minimum steps | Unweighted shortest path, multi-source |
| 7 | DFS + Backtracking | Explore all paths, undo choices | Permutations, subsets, combinations |
| 8 | Topological Sort | Process DAG in dependency order | Course schedule, build order, deadlock |
| 9 | Union-Find | Efficient connectivity with path compression | Connected components, cycle detection |
| 10 | Heap for K-th | Always keep K elements efficiently | K-th largest, merge K sorted, streaming |
| 11 | DP — Define State | Optimal substructure + overlapping subproblems | "Count/max/min ways to reach..."|
| 12 | Interval Merge/Sweep | Process events at endpoints | Meeting rooms, overlapping intervals |
| 13 | Trie | Efficient string prefix matching | Autocomplete, prefix search, word dict |
| 14 | Segment Tree | Range queries with point/range updates | Range min/max/sum with mutations |
| 15 | Bit Manipulation | XOR/AND/OR tricks for counting | Subset enumeration, parity, XOR properties |

---

*Start at `18_Interview_Meta_Skills/01_Problem_Solving_Framework.md` if you want the meta-skills first.*
*Start at `02_Arrays_And_Strings/01_Two_Pointers_Mastery.md` if you want to dive into patterns immediately.*

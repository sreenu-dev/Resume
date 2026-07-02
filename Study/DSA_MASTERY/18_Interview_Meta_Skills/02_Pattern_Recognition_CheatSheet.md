# Pattern Recognition Cheatsheet — FAANG Interview Quick Reference

> **Level:** Reference Card | **Use:** During practice sessions for instant pattern lookup  
> **Format:** Dense reference tables — the one file you bookmark and return to daily

---

## MASTER TABLE 1: Input Type → First Pattern to Try

| Input Type         | Problem Goal                  | First Pattern                 | Second Pattern              |
| --------------------| -------------------------------| -------------------------------| -----------------------------|
| **Sorted Array**   | Find target                   | Binary Search O(log N)        | Two Pointers                |
| **Sorted Array**   | Find pair with sum            | Two Pointers O(N)             | Binary Search O(N log N)    |
| **Sorted Array**   | Find triplets                 | Sort + Two Pointers O(N²)     | —                           |
| **Sorted Array**   | K-th element                  | Binary Search                 | —                           |
| **Sorted Array**   | Merge two                     | Two Pointers O(N+M)           | —                           |
| **Unsorted Array** | Find target                   | HashMap O(N)                  | Sort first                  |
| **Unsorted Array** | Find duplicates               | HashSet O(N)                  | Sort + Adjacent             |
| **Unsorted Array** | Subarray sum = K              | Prefix Sum + HashMap O(N)     | —                           |
| **Unsorted Array** | K-th largest                  | Min-Heap size K O(N log K)    | QuickSelect O(N) avg        |
| **Unsorted Array** | Max subarray                  | Kadane's O(N)                 | DP                          |
| **Unsorted Array** | All subsets                   | Backtracking O(2^N)           | Bitmask                     |
| **String**         | Anagram groups                | Sort key HashMap O(N K log K) | Freq-tuple key              |
| **String**         | Longest substring             | Sliding Window O(N)           | —                           |
| **String**         | Pattern match                 | KMP O(N+M)                    | Rabin-Karp O(N+M) avg       |
| **String**         | All permutations              | Backtracking O(N!)            | —                           |
| **String**         | Palindrome check              | Two Pointers O(N)             | —                           |
| **String**         | Decode/parse                  | Stack O(N)                    | Recursion                   |
| **Linked List**    | Find cycle                    | Floyd's O(N) O(1) space       | HashSet O(N) O(N)           |
| **Linked List**    | Find middle                   | Slow/Fast Pointer O(N)        | Count then traverse         |
| **Linked List**    | Reverse                       | Iterative O(N) O(1)           | Recursive O(N) O(N) stack   |
| **Linked List**    | Merge sorted                  | Two Pointers O(N+M)           | —                           |
| **Binary Tree**    | Level-order                   | BFS O(N)                      | —                           |
| **Binary Tree**    | Path sum                      | DFS O(N)                      | —                           |
| **Binary Tree**    | LCA                           | Post-order DFS O(N)           | Euler Tour + RMQ O(1) query |
| **BST**            | Validate                      | DFS with bounds O(N)          | —                           |
| **BST**            | K-th smallest                 | Inorder traversal O(N)        | Augmented BST O(log N)      |
| **Graph**          | Shortest path (unweighted)    | BFS O(V+E)                    | —                           |
| **Graph**          | Shortest path (weighted, pos) | Dijkstra O((V+E)log V)        | —                           |
| **Graph**          | Shortest path (neg weights)   | Bellman-Ford O(VE)            | SPFA                        |
| **Graph**          | All shortest paths            | Floyd-Warshall O(V³)          | —                           |
| **Graph**          | Connected components          | Union-Find O(α N)             | DFS/BFS                     |
| **Graph**          | Topological sort              | Kahn's BFS O(V+E)             | DFS O(V+E)                  |
| **Graph**          | Cycle detection               | DFS coloring O(V+E)           | Union-Find                  |
| **Matrix**         | Shortest path                 | BFS O(N*M)                    | —                           |
| **Matrix**         | All paths                     | DFS/Backtracking              | —                           |
| **Matrix**         | Connected regions             | DFS/BFS flood fill            | Union-Find                  |
| **Matrix**         | DP paths                      | DP O(N*M)                     | —                           |
| **Intervals**      | Merge                         | Sort + Sweep O(N log N)       | —                           |
| **Intervals**      | Insert                        | Binary Search + Merge O(N)    | —                           |
| **Intervals**      | Meeting rooms                 | Sort + compare O(N log N)     | Min-Heap                    |

---

## MASTER TABLE 2: Objective → Algorithm

| Objective | Input Hint | Algorithm | Complexity |
|-----------|-----------|-----------|------------|
| **Find minimum/maximum** | Array | Linear scan / Heap | O(N) / O(N log K) |
| **Find minimum/maximum** | Range queries | Segment Tree / Sparse Table | O(log N) / O(1) |
| **Count occurrences** | Static array | Sort + Binary Search | O(log N) per query |
| **Count occurrences** | Dynamic | HashMap | O(1) |
| **Count subarrays with property** | Sum = K | Prefix Sum + HashMap | O(N) |
| **Count subarrays with property** | Max element | Monotonic Stack | O(N) |
| **Find all combinations** | Pick any k | Backtracking | O(C(N,K)) |
| **Find all permutations** | Ordered | Backtracking | O(N!) |
| **Optimize (minimize sum)** | Choices at each step | Greedy (if exchange arg holds) | varies |
| **Optimize (max profit)** | Sequence decisions | DP | O(N) or O(N²) |
| **Reach/path exists** | Graph | BFS/DFS | O(V+E) |
| **Minimum steps** | Graph/Matrix | BFS | O(V+E) |
| **String search** | Pattern in text | KMP / Z-Algorithm | O(N+M) |
| **Balanced parentheses** | String | Stack | O(N) |
| **Next greater element** | Array | Monotonic Stack | O(N) |
| **Sliding window max/min** | Fixed window | Monotonic Deque | O(N) |
| **Range sum** | Static | Prefix Sum | O(1) query |
| **Range sum** | Point updates | BIT / Segment Tree | O(log N) |
| **Range sum** | Range updates | Lazy Segment Tree | O(log N) |
| **K-th order in range** | 2D (i,j,k) | Persistent Seg Tree | O(N log N + Q log N) |
| **Majority element** | Array | Boyer-Moore Voting | O(N) O(1) |
| **Median in stream** | Online | Two Heaps | O(log N) per insert |
| **LRU/LFU Cache** | Design | DLL + HashMap | O(1) all ops |
| **Word search** | Matrix + string | DFS + Backtrack | O(N*M*4^L) |
| **Word ladder** | BFS on words | BFS with word graph | O(N²*L) |

---

## MASTER TABLE 3: Constraint Clues → Pattern

| Constraint in Problem | Pattern It Suggests |
|----------------------|---------------------|
| `N ≤ 10` | O(N!) — Backtracking all permutations OK |
| `N ≤ 20` | O(2^N) — Bitmask DP OK |
| `N ≤ 100` | O(N³) — Floyd-Warshall, cubic DP OK |
| `N ≤ 1,000` | O(N²) — Nested loops OK |
| `N ≤ 10,000` | O(N² log N) — careful; O(N²) probably OK |
| `N ≤ 100,000` | O(N log N) — Sort, Heap, Segment Tree |
| `N ≤ 1,000,000` | O(N) — Linear scan, two-pointer, hash |
| `N ≤ 10^9` | O(log N) or O(√N) — Binary search, math |
| "at most K" | Sliding Window |
| "exactly K" | Sliding Window: f(≤K) - f(≤K-1) |
| "continuous / contiguous" | Sliding Window or Prefix Sum |
| "all possible" | Backtracking |
| "minimum number of steps" | BFS |
| "minimum cost to reach" | Dijkstra / DP |
| "optimal subsequence" | DP |
| "parentheses / matching" | Stack |
| "next greater / smaller" | Monotonic Stack |
| "K most frequent" | Heap (min-heap size K) |
| "find median" | Two Heaps (max + min) |
| "sorted + binary search" | Binary Search |
| "divide and conquer" | Merge Sort, QuickSelect |
| "process queries offline" | Sort queries, Mo's Algorithm |
| "rollback / undo" | Persistent Data Structure |
| "union / find / connectivity" | Union-Find (DSU) |
| "topological order" | Kahn's BFS or DFS |
| "scheduling" | Greedy (interval scheduling) |
| "intervals" | Sort by start, sweep |
| "palindrome" | Two Pointers / Manacher's |
| "anagram" | HashMap / Sort |
| "substring" | Sliding Window |
| "subsequence" | DP (LCS style) |
| "tree diameter" | DFS returning height |
| "LCA" | Binary Lifting / Euler Tour + RMQ |
| "XOR" | Trie / Bit Manipulation |
| "range [l,r]" | Offline: sort by r; Online: Seg Tree |

---

## MASTER TABLE 4: Complexity Target → Data Structure

| Target Query | Target Update | Best Data Structure | Alt |
|-------------|--------------|---------------------|-----|
| O(1) exact | O(1) exact | HashMap / Array | — |
| O(log N) ordered | O(log N) | AVL/Red-Black Tree | SortedList |
| O(1) range min | none (static) | Sparse Table | — |
| O(log N) range sum | O(log N) point | BIT (Fenwick Tree) | Segment Tree |
| O(log N) range any | O(log N) range | Segment Tree (lazy) | — |
| O(log N) range op | O(log N) range | HLD + Seg Tree | — |
| O(log N) per version | O(log N) new version | Persistent Seg Tree | — |
| O(N^.5) range | O(1) point | Sqrt Decomposition | — |
| O((N+Q)^.5 * N) offline | offline | Mo's Algorithm | — |
| O(1) predecessor/successor | O(log N) | Van Emde Boas Tree | — |
| O(L) string insert | O(L) string search | Trie | Aho-Corasick (multiple patterns) |
| O(log N) interval | O(log N) insert | Interval Tree | Segment Tree |
| O(α(N)) union/find | O(α(N)) union | DSU (Union-Find) | — |
| O(1) amortized stack | O(1) amortized | Monotonic Stack | Deque |
| O(log N) dynamic median | O(log N) insert | Two Heaps | Ordered Set |
| O(1) LRU get/put | O(1) | DLL + HashMap | OrderedDict |
| O(1) LFU get/put | O(1) | DLL per freq + HashMap | — |

---

## MASTER TABLE 5: Key Phrases → Algorithm Mapping

| Phrase | Algorithm | Signature | Notes |
|--------|-----------|-----------|-------|
| "Two Sum" | HashMap | `seen[complement] exists?` | Store {val: idx} |
| "Three Sum" | Sort + Two Ptr | sort, fix one, two-ptr rest | Skip duplicates |
| "Sliding Window" | Two Pointers | `l, r` expanding/contracting | maintain invariant |
| "Longest Increasing Subsequence" | DP + Binary Search | `tails[]` patience sort | O(N log N) |
| "Coin Change (min coins)" | BFS or DP | `dp[amount]` | Unbounded knapsack |
| "Knapsack" | DP | `dp[w]` | 0/1 or unbounded |
| "Edit Distance" | DP | `dp[i][j]` | LCS variant |
| "Matrix Chain" | Interval DP | `dp[l][r]` | O(N³) |
| "Burst Balloons" | Interval DP | choose last to burst | O(N³) |
| "Stock Buy/Sell" | Greedy / DP | `min_price` / `dp[k][0/1]` | K transactions |
| "Trapping Rain Water" | Two Pointers | `left_max, right_max` | O(N) O(1) |
| "Next Permutation" | Array | find rightmost ascent | O(N) |
| "Word Break" | DP + Trie | `dp[i]` = word ends at i | O(N²) or O(N*L) |
| "Regular Expression" | DP | `dp[i][j]` | 2D DP |
| "Wildcard Matching" | DP | `dp[i][j]` | 2D DP |
| "Jump Game" | Greedy | `max_reach` | O(N) |
| "Gas Station" | Greedy | `total_gas ≥ total_cost` | Circular invariant |
| "Task Scheduler" | Greedy + Heap | cooldown `n` | Count, fill gaps |
| "Clone Graph" | BFS + HashMap | `visited: node→copy` | Deep copy |
| "Number of Islands" | DFS/BFS flood fill | mark visited | O(N*M) |
| "Alien Dictionary" | Topo sort | build DAG from order | Kahn's BFS |
| "Course Schedule" | Topo sort / DFS | cycle detection | O(V+E) |
| "Network Delay" | Dijkstra | single-source shortest | O((V+E) log V) |
| "Serialize Tree" | BFS or preorder | level-order encoding | Reconstruct from string |
| "Trie Insert/Search" | Trie | 26-ary tree | O(L) per op |
| "Maximum XOR" | Trie | bit-by-bit greedy | O(N*B) |

---

## MASTER TABLE 6: Anti-Patterns — When NOT to Use

| Algorithm | Looks Applicable When... | Actually WRONG Because... | Use Instead |
|-----------|--------------------------|--------------------------|-------------|
| **Binary Search** | Array has duplicates, want ALL positions | BS finds ONE occurrence | BS + bisect_left/right |
| **Greedy** | Problem has "all combinations" | Greedy only finds one optimal | DP or Backtracking |
| **BFS for shortest path** | Graph has weighted edges | BFS ignores edge weights | Dijkstra |
| **Dijkstra** | Graph has negative edge weights | Cannot handle negative cycles | Bellman-Ford |
| **DP top-down** | State space is too large | Memory overflow | Bottom-up with rolling array |
| **Sorting** | Need to preserve original order | Sort destroys positions | HashMap with indices |
| **Recursion** | Very deep recursion (N=10^5) | Stack overflow | Iterative + explicit stack |
| **Floyd's cycle** | Not a linked list structure | Doesn't generalize to arrays | HashSet |
| **Union-Find** | Need to UNDO unions | Standard DSU is not rollback-able | Persistent DSU |
| **Sparse Table** | Array has point updates | Sparse table is static only | Segment Tree |
| **Two Pointer** | Array is not sorted, need pair | Two-pointer requires sorted | HashMap for two-sum |
| **KMP** | Pattern matching in 2D | KMP is for 1D strings | Aho-Corasick or Z-algo per row |
| **Monotonic Stack** | Need RANGE queries (not just next) | Stack gives next element, not range | Sparse Table / Seg Tree |
| **Sliding Window** | Window condition depends on future | Requires online property | DP or sorted structures |

---

## MASTER TABLE 7: DP Pattern Classification

| DP Type | Template | Classic Problems |
|---------|----------|-----------------|
| **Linear DP** | `dp[i]` depends on `dp[i-1]` | Fibonacci, Climbing Stairs, House Robber |
| **Kadane's** | `dp[i] = max(dp[i-1]+a[i], a[i])` | Maximum Subarray |
| **Two-State DP** | `dp[i][0/1]` = state at i | Stock Buy/Sell, House Robber II |
| **K-State DP** | `dp[i][k]` = state k at i | Stock with K transactions |
| **String DP** | `dp[i][j]` on two strings | LCS, Edit Distance, Regex |
| **Grid DP** | `dp[i][j]` on matrix | Unique Paths, Coin Change 2D |
| **Interval DP** | `dp[l][r]` on subarray `[l,r]` | Burst Balloons, Matrix Chain, Palindrome |
| **Tree DP** | `dp[node]` via post-order | Diameter, Max Path Sum |
| **Knapsack (0/1)** | `dp[i][w]` pick/skip | 0/1 Knapsack, Subset Sum |
| **Knapsack (unbounded)** | `dp[w]` reuse items | Coin Change, Unbounded |
| **Digit DP** | `dp[pos][tight][...]` | Count numbers with property |
| **Bitmask DP** | `dp[mask]` | TSP, Shortest Superstring |
| **Profile DP** | Process column by column | Tiling problems |

---

## MASTER TABLE 8: Graph Problem Identification

```
Is it asking for PATH EXISTENCE?     → DFS or BFS
Is it asking for SHORTEST PATH?
  Unweighted?                        → BFS
  Non-negative weights?              → Dijkstra
  Negative weights?                  → Bellman-Ford
  Dense graph?                       → Floyd-Warshall
Is it asking for MINIMUM SPANNING TREE? → Kruskal or Prim
Is it asking for TOPOLOGICAL ORDER?  → Kahn's BFS (cycle-safe)
Is it asking for CONNECTED COMPONENTS? → Union-Find or DFS
Is it asking for STRONGLY CONNECTED? → Kosaraju or Tarjan
Is it asking for MAX FLOW?           → Ford-Fulkerson / Dinic
Is it asking for BIPARTITE CHECK?    → BFS 2-coloring
Is it asking for EULER PATH?         → Hierholzer's algorithm
Is it asking for ALL PATHS?          → DFS + Backtracking
```

---

## MASTER TABLE 9: Tree Traversal → Problem Type

| Traversal | Order | Use For | Pattern |
|-----------|-------|---------|---------|
| **Inorder** | Left → Node → Right | BST sorted order, K-th element | Inorder iterator |
| **Preorder** | Node → Left → Right | Serialization, Clone | Stack or recursion |
| **Postorder** | Left → Right → Node | Deletion, Height, LCA | Bottom-up computation |
| **Level-order** | Level by level | Shortest path in tree, zigzag | BFS with deque |
| **Euler Tour** | Visit on enter+exit | LCA reduction to RMQ | DFS tracking depth |
| **Morris** | Inorder, O(1) space | Inorder without stack/recursion | Thread pointers |

---

## MASTER TABLE 10: Time Complexity Decision Points

```
Given: N = 10^5 (typical LeetCode constraint)

O(N²) = 10^10 operations → TOO SLOW for N=10^5
O(N log N) = ~1.7×10^6 → SAFE
O(N) = 10^5 → FAST
O(N √N) = ~3×10^7 → ACCEPTABLE

For space:
O(N) = 100,000 entries of 8 bytes = 800KB → FINE
O(N²) = 10^10 bytes = 10GB → IMPOSSIBLE
O(N log N) = ~1.7MB → FINE
```

---

## QUICK-FIRE DRILL: Problem → Pattern (5 Seconds Each)

```
"Find two numbers that sum to target in sorted array"     → Two Pointers
"Find all subsets of array"                               → Backtracking
"Minimum path from top-left to bottom-right of matrix"    → DP or BFS
"Detect cycle in directed graph"                          → DFS (3-coloring)
"K-th largest element"                                    → Min-Heap size K / QuickSelect
"Merge K sorted lists"                                    → Min-Heap K-way merge
"Longest palindromic substring"                           → Expand around center / Manacher
"Maximum area of histogram"                               → Monotonic Stack
"Number of islands"                                       → DFS / BFS flood fill / Union-Find
"Find median from data stream"                            → Two Heaps (max + min)
"Implement LRU Cache"                                     → DLL + HashMap
"Sliding window maximum"                                  → Monotonic Deque
"Minimum cost to connect all nodes"                       → Kruskal / Prim (MST)
"Course schedule / prerequisites"                         → Topological Sort
"Serialize / Deserialize binary tree"                     → BFS or preorder DFS
"Maximum XOR of two numbers"                              → Trie
"Count inversions in array"                               → Modified Merge Sort
"Next greater element"                                    → Monotonic Stack
"Longest increasing subsequence"                          → DP + Binary Search O(N log N)
"Regular expression matching"                             → DP 2D
"Jump Game II (min jumps)"                                → Greedy BFS
"Trapping rain water"                                     → Two Pointers / Monotonic Stack
"Burst Balloons"                                          → Interval DP
"Wildcard matching"                                       → DP
"Shortest bridge between islands"                         → DFS (find one) + BFS (expand)
"Clone graph"                                             → BFS + HashMap
"Alien dictionary"                                        → Topological Sort
"Find duplicate number (no extra space)"                  → Floyd's Cycle (array as linked list)
"Maximum points on a line"                                → HashMap of slopes
"Minimum window substring"                                → Sliding Window
```

---

## KEY INSIGHTS MATRIX

```python
# The 5 most important insights for pattern matching:

insight_1 = """
SORTED + FIND → Binary Search or Two Pointers
If array is sorted, binary search should be your FIRST thought.
"""

insight_2 = """
OPTIMAL SUBSTRUCTURE + OVERLAPPING = DP
If you can break problem into smaller SAME-TYPE problems,
and solutions to subproblems are reused → DP.
"""

insight_3 = """
"ALL COMBINATIONS/PERMUTATIONS" → BACKTRACKING
If you need to enumerate ALL possibilities → Backtracking.
Use pruning to cut exponential branches.
"""

insight_4 = """
"SHORTEST PATH" → BFS (unweighted), DIJKSTRA (weighted)
BFS guarantees shortest in unweighted.
Never use DFS for shortest path problems.
"""

insight_5 = """
"PREFIX SUM" is underused.
sum(arr[l..r]) = prefix[r+1] - prefix[l]
count subarrays with sum K → prefix + hashmap
This pattern solves hundreds of problems.
"""
```

---

*This cheatsheet is your mental lookup table. For each new problem, spend 30 seconds scanning Table 1 (input type), Table 3 (constraints), and Table 5 (key phrases). The pattern will emerge. The goal is to move from "What algorithm do I use?" to "Let me confirm my pattern match with the constraints before coding."*

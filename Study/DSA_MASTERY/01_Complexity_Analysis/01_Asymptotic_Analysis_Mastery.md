# Asymptotic Analysis — Mastery Guide

## Core Concept & Invariant

Asymptotic analysis describes algorithm behavior as input size n → ∞, stripping away
machine-specific constants to reveal the fundamental growth rate. The formal definitions:

- **O(g(n))** — upper bound: ∃ c > 0, n₀ such that f(n) ≤ c·g(n) for all n ≥ n₀
- **Ω(g(n))** — lower bound: ∃ c > 0, n₀ such that f(n) ≥ c·g(n) for all n ≥ n₀
- **Θ(g(n))** — tight bound: f(n) is both O(g(n)) and Ω(g(n))
- **o(g(n))** — strict upper: lim_{n→∞} f(n)/g(n) = 0
- **ω(g(n))** — strict lower: lim_{n→∞} f(n)/g(n) = ∞

**Key invariant**: O-notation is transitive and composable.  
If f = O(g) and g = O(h), then f = O(h).  
If f₁ = O(g₁) and f₂ = O(g₂), then f₁ + f₂ = O(max(g₁, g₂)).

---

## The Master Theorem — All 3 Cases with Proofs

For recurrences of the form **T(n) = a·T(n/b) + f(n)** where a ≥ 1, b > 1:

Define the **critical exponent**: `p = log_b(a)`

### Case 1: f(n) = O(n^(p−ε)) for some ε > 0
Recursion dominates. **T(n) = Θ(n^p)**

*Proof sketch*: The recursion tree has log_b(n) levels. Work at level k is  
a^k · f(n/b^k). Total work ≈ n^p · Σ(a/b^p)^k = n^p · geometric series → Θ(n^p)  
since a/b^p = a/a = 1 when f is strictly smaller than n^p.

### Case 2: f(n) = Θ(n^p · log^k(n)) for k ≥ 0
Equal contributions. **T(n) = Θ(n^p · log^(k+1)(n))**

*Proof sketch*: Each of the log_b(n) levels contributes equally O(n^p · log^k(n)),  
summing to Θ(n^p · log^(k+1)(n)).

### Case 3: f(n) = Ω(n^(p+ε)) AND regularity condition a·f(n/b) ≤ c·f(n)
Root dominates. **T(n) = Θ(f(n))**

*Proof sketch*: Work decreases geometrically up the tree. Root level dominates.

### Critical Exponent Table

| Recurrence             | a | b | p=log_b(a) | f(n)    | Case | Result          |
|------------------------|---|---|------------|---------|------|-----------------|
| T(n)=2T(n/2)+n         | 2 | 2 | 1          | n¹      |  2   | Θ(n log n)      |
| T(n)=2T(n/2)+n²        | 2 | 2 | 1          | n²      |  3   | Θ(n²)           |
| T(n)=2T(n/2)+1         | 2 | 2 | 1          | n⁰      |  1   | Θ(n)            |
| T(n)=4T(n/2)+n         | 4 | 2 | 2          | n¹      |  1   | Θ(n²)           |
| T(n)=4T(n/2)+n²        | 4 | 2 | 2          | n²      |  2   | Θ(n² log n)     |
| T(n)=4T(n/2)+n³        | 4 | 2 | 2          | n³      |  3   | Θ(n³)           |
| T(n)=T(n/2)+1          | 1 | 2 | 0          | 1       |  2   | Θ(log n)        |
| T(n)=9T(n/3)+n         | 9 | 3 | 2          | n¹      |  1   | Θ(n²)           |
| T(n)=T(2n/3)+1         | 1 | 3/2| 0         | 1       |  2   | Θ(log n)        |

---

## Recurrence Solving Methods

### Method 1: Substitution (Educated Guess + Induction)

**Example**: T(n) = 2T(n/2) + n

*Guess*: T(n) = O(n log n), i.e., T(n) ≤ c·n·log(n)

*Inductive step*: Assume T(k) ≤ c·k·log(k) for all k < n.

```
T(n) = 2T(n/2) + n
     ≤ 2·c·(n/2)·log(n/2) + n
     = c·n·(log(n) − 1) + n
     = c·n·log(n) − c·n + n
     ≤ c·n·log(n)   provided c ≥ 1
```

**Subtlety**: When guessing, strengthen the inductive hypothesis if needed.  
For T(n) = T(n/2) + T(n/4) + n, guessing O(n) works with hypothesis T(n) ≤ c·n − d·n.

### Method 2: Recursion Tree

**Example**: T(n) = 2T(n/2) + n²

```
Level 0:  n²                                    = n²
Level 1:  2·(n/2)² = n²/2                       = n²/2
Level 2:  4·(n/4)² = n²/4                       = n²/4
...
Level k:  2^k·(n/2^k)²  = n²/2^k
...
Level log n: n leaves × T(1)                    = n
```

Total = n²·(1 + 1/2 + 1/4 + ...) = n²·2 = **Θ(n²)** (geometric series, Case 3)

### Method 3: Akra-Bazzi Method (Generalization Beyond Master Theorem)

For T(n) = Σ aᵢ·T(bᵢ·n) + f(n) where bᵢ ∈ (0,1):

Find p such that Σ aᵢ·bᵢ^p = 1

Then: **T(n) = Θ(n^p · (1 + ∫₁ⁿ f(u)/u^(p+1) du))**

**Example**: T(n) = T(n/3) + T(2n/3) + n

Find p: (1/3)^p + (2/3)^p = 1 → p = 1

T(n) = Θ(n · (1 + ∫₁ⁿ u/u² du)) = Θ(n · (1 + ln n)) = **Θ(n log n)**

```python
# Akra-Bazzi p-finder via binary search
def find_p(a_list, b_list, lo=0.0, hi=5.0, tol=1e-9):
    """
    Solve Σ aᵢ * bᵢ^p = 1 for p.
    a_list: coefficients, b_list: sub-problem fractions (each in (0,1))
    """
    def f(p):
        return sum(a * (b ** p) for a, b in zip(a_list, b_list)) - 1.0
    
    while hi - lo > tol:
        mid = (lo + hi) / 2
        if f(mid) > 0:
            lo = mid   # Σ aᵢbᵢ^p > 1 means p needs to increase
        else:
            hi = mid
    return (lo + hi) / 2

# T(n) = T(n/3) + T(2n/3) + n
p = find_p([1, 1], [1/3, 2/3])
print(f"p = {p:.6f}")  # → p ≈ 1.0000
```

---

## Amortized Complexity (Preview — Deep Dive in File 02)

Amortized analysis gives the **average cost per operation** over a sequence of operations,
even if individual operations are expensive:

- **Aggregate**: Total cost / n operations
- **Accounting**: Assign credits; expensive ops use saved credits
- **Potential**: Φ(state) represents "stored energy"; amortized cost = actual + ΔΦ

---

## Tight vs Loose Bounds

A bound is **tight** if it is both O and Ω simultaneously (i.e., Θ).

**Loose upper bound example**: Insertion sort is O(n²) and also O(n³) — but only Θ(n²).

**How to tighten**: 
1. Find the exact number of operations via summation formulas
2. Use recurrence trees to count leaves vs internal nodes
3. Use lower-bound arguments (adversarial, information-theoretic)

**Information-theoretic lower bound for comparison sorting**:
- Any comparison-based sort must make Ω(n log n) comparisons in worst case
- Proof: Decision tree has n! leaves → height ≥ log₂(n!) = Θ(n log n) by Stirling's approximation
- Stirling: log₂(n!) = n·log₂(n) − n·log₂(e) + O(log n) = Θ(n log n)

---

## Logarithm Tricks & Identities

```
log_a(b) = log(b) / log(a)          # Change of base
log(n^k) = k·log(n)                  # Power rule
log(a·b) = log(a) + log(b)           # Product rule
a^(log_a(n)) = n                     # Fundamental identity
n^(log_b(a)) = a^(log_b(n))          # Symmetry identity (used in Master Theorem)
log*(n) = min{k : log^(k)(n) ≤ 1}   # Iterated log — nearly constant in practice
```

**Why log₂(n) ≈ log₁₀(n) × 3.32**: log₂(n) = log₁₀(n) / log₁₀(2) ≈ log₁₀(n) × 3.32

**Critical identity for Master Theorem**: n^p = n^(log_b(a)) = a^(log_b(n))  
This explains why the "critical exponent" p = log_b(a) defines the recursion's cost.

---

## Common Recurrences Reference Table

| Algorithm                    | Recurrence             | Solution       | Notes                          |
|------------------------------|------------------------|----------------|--------------------------------|
| Binary Search                | T(n)=T(n/2)+1          | Θ(log n)       | Case 2, k=0                    |
| Merge Sort                   | T(n)=2T(n/2)+n         | Θ(n log n)     | Case 2, k=0                    |
| Quick Sort (avg)             | T(n)=2T(n/2)+n         | Θ(n log n)     | Expected, random pivot         |
| Quick Sort (worst)           | T(n)=T(n-1)+n          | Θ(n²)          | Already-sorted input           |
| Selection Sort               | T(n)=T(n-1)+n          | Θ(n²)          | Linear scan each time          |
| Strassen Matrix Mult         | T(n)=7T(n/2)+n²        | Θ(n^2.807)     | log₂(7)≈2.807                  |
| Karatsuba Multiplication     | T(n)=3T(n/2)+n         | Θ(n^1.585)     | log₂(3)≈1.585                  |
| Fibonacci (naive)            | T(n)=T(n-1)+T(n-2)+1  | Θ(φⁿ)          | φ=(1+√5)/2≈1.618               |
| Fibonacci (matrix)           | T(n)=T(n/2)+1          | Θ(log n)       | Matrix exponentiation          |
| Tower of Hanoi               | T(n)=2T(n-1)+1         | Θ(2ⁿ)          | Exponential — unavoidable      |
| Binary Search Tree (avg)     | T(n)=T(n/2)+1          | Θ(log n)       | Balanced case                  |
| Quickselect (avg)            | T(n)=T(n/2)+n          | Θ(n)           | Expected with random pivot     |
| Median of Medians            | T(n)=T(7n/10)+T(n/5)+n| Θ(n)           | Akra-Bazzi or careful analysis |

---

## Algorithm Templates

```python
# ─────────────────────────────────────────────────────────────────
# Template 1: Recurrence Solver via Memoized Recursion
# ─────────────────────────────────────────────────────────────────
from functools import lru_cache

@lru_cache(maxsize=None)
def T_merge_sort(n: int) -> int:
    """Counts exact comparisons in merge sort recurrence: T(n)=2T(n/2)+n"""
    if n <= 1:
        return 0
    return 2 * T_merge_sort(n // 2) + n

@lru_cache(maxsize=None)
def T_selection_sort(n: int) -> int:
    """T(n) = T(n-1) + n  →  n(n+1)/2"""
    if n <= 1:
        return 0
    return T_selection_sort(n - 1) + n

# ─────────────────────────────────────────────────────────────────
# Template 2: Empirical Complexity Detector
# ─────────────────────────────────────────────────────────────────
import time
import math

def measure_growth(func, sizes):
    """
    Empirically detect O complexity by measuring doubling ratio.
    Doubling ratio r = T(2n)/T(n):
      r ≈ 1      → O(1) or O(log n)
      r ≈ 2      → O(n) or O(n log n)  
      r ≈ 4      → O(n²)
      r ≈ 8      → O(n³)
    """
    times = []
    for n in sizes:
        start = time.perf_counter()
        func(n)
        end = time.perf_counter()
        times.append(end - start)
    
    print(f"{'n':>10} {'time(s)':>12} {'ratio':>8} {'inferred':>12}")
    for i, (n, t) in enumerate(zip(sizes, times)):
        if i == 0:
            print(f"{n:>10} {t:>12.6f} {'—':>8} {'—':>12}")
        else:
            ratio = times[i] / times[i-1]
            if ratio < 1.2:   label = "O(1)/O(log n)"
            elif ratio < 2.5: label = "O(n)/O(n log n)"
            elif ratio < 6:   label = "O(n²)"
            else:             label = "O(n³) or worse"
            print(f"{n:>10} {t:>12.6f} {ratio:>8.2f} {label:>12}")
```

---

## Classic Problems

### Problem 1: Determine Master Theorem Case — Medium

**Problem**: Given recurrences, identify the case and solve.

**Approach**: Compute p = log_b(a), compare f(n) to n^p.

```python
def master_theorem_classifier(a: int, b: int, f_exponent: float) -> dict:
    """
    Classify T(n) = a*T(n/b) + n^f_exponent.
    Returns the case and the tight bound.
    
    Args:
        a: Number of subproblems
        b: Subproblem size reduction factor  
        f_exponent: Exponent of the combining step f(n) = n^f_exponent
    """
    import math
    p = math.log(a, b)   # Critical exponent
    eps = 1e-9
    
    if f_exponent < p - eps:
        # Case 1: recursion dominates
        return {
            "case": 1,
            "p": p,
            "f_exp": f_exponent,
            "result": f"Θ(n^{p:.3f})",
            "reason": f"f(n)=n^{f_exponent} is polynomially smaller than n^{p:.3f}"
        }
    elif abs(f_exponent - p) < eps:
        # Case 2: equal contributions (k=0 assumed for log factor)
        return {
            "case": 2,
            "p": p,
            "f_exp": f_exponent,
            "result": f"Θ(n^{p:.3f} · log n)",
            "reason": f"f(n)=n^{p:.3f} matches n^log_b(a)=n^{p:.3f}"
        }
    else:
        # Case 3: root dominates — verify regularity
        return {
            "case": 3,
            "p": p,
            "f_exp": f_exponent,
            "result": f"Θ(n^{f_exponent})",
            "reason": (f"f(n)=n^{f_exponent} polynomially larger than n^{p:.3f}; "
                       f"verify regularity: a*f(n/b)=a*(n/b)^{f_exponent}"
                       f"={a/b**f_exponent:.3f}*n^{f_exponent} ≤ c*n^{f_exponent}")
        }

# Test all cases
cases = [
    (2, 2, 0),    # T(n)=2T(n/2)+1      → Θ(n)
    (2, 2, 1),    # T(n)=2T(n/2)+n      → Θ(n log n)
    (2, 2, 2),    # T(n)=2T(n/2)+n²     → Θ(n²)
    (4, 2, 1),    # T(n)=4T(n/2)+n      → Θ(n²)
    (4, 2, 2),    # T(n)=4T(n/2)+n²     → Θ(n² log n)
    (7, 2, 2),    # T(n)=7T(n/2)+n²     → Θ(n^log₂7) [Strassen]
]

for a, b, fe in cases:
    result = master_theorem_classifier(a, b, fe)
    print(f"T(n)={a}T(n/{b})+n^{fe}: Case {result['case']} → {result['result']}")
```

**Time**: O(1) per classification  
**Space**: O(1)

---

### Problem 2: Recurrence via Recursion Tree for T(n) = T(n/3) + T(2n/3) + n — Hard

**Problem**: Solve T(n) = T(n/3) + T(2n/3) + n (used in QuickSort analysis with 1:2 split).

**Approach**: Recursion tree — each level sums to n. Find the depth of the longest path.

```python
def recursion_tree_tn3_t2n3(n: int, memo: dict = None) -> int:
    """
    Empirically compute T(n) = T(n/3) + T(2n/3) + n.
    Demonstrates the n log n growth.
    """
    if memo is None:
        memo = {}
    if n in memo:
        return memo[n]
    if n <= 1:
        return n
    result = recursion_tree_tn3_t2n3(n // 3, memo) + \
             recursion_tree_tn3_t2n3(2 * n // 3, memo) + n
    memo[n] = result
    return result

# Analytical solution:
# Each level of the tree sums to exactly n (like merge sort).
# The LONGEST path (always taking 2n/3 branch) has depth log_{3/2}(n).
# log_{3/2}(n) = log(n)/log(3/2) ≈ 1.71·log₂(n)
# Each level contributes n work → T(n) = Θ(n·log_{3/2}(n)) = Θ(n log n)

import math
for n in [100, 1000, 10000]:
    t = recursion_tree_tn3_t2n3(n)
    expected = n * math.log2(n)
    print(f"n={n:6d}: T(n)={t:8d}, n·log₂n={expected:8.0f}, ratio={t/expected:.3f}")
```

**Correctness**: At each level k, the n subproblems partition the work — total at each level = n.  
Number of levels = log_{3/2}(n). Hence T(n) = **Θ(n log n)**.

---

### Problem 3: Prove O(N log N) is Sorting Lower Bound — Hard

```python
import math

def sorting_lower_bound_proof(n: int) -> dict:
    """
    Information-theoretic proof that comparison-based sorting is Ω(n log n).
    
    Argument:
    1. There are n! possible orderings of n elements
    2. Each comparison has 2 outcomes → binary decision tree
    3. Binary tree with n! leaves has height ≥ log₂(n!)
    4. By Stirling: log₂(n!) = n·log₂(n) - n·log₂(e) + O(log n) = Θ(n log n)
    """
    factorial_n = math.factorial(n)
    log2_factorial = math.log2(factorial_n)
    
    # Stirling's approximation
    stirling_approx = n * math.log2(n) - n * math.log2(math.e)
    
    return {
        "n": n,
        "n!": factorial_n,
        "log2(n!)": log2_factorial,
        "stirling_approx": stirling_approx,
        "lower_bound": f"Ω({n}·log₂({n}) - {n:.1f}) = Ω(n log n)",
        "min_comparisons": math.ceil(log2_factorial)
    }

for n in [5, 10, 20]:
    result = sorting_lower_bound_proof(n)
    print(f"n={n}: log₂({n}!)={result['log2(n!)']:.2f}, "
          f"Stirling≈{result['stirling_approx']:.2f}, "
          f"min_comparisons={result['min_comparisons']}")

# Verification: merge sort achieves this bound exactly
# Merge sort uses at most n·ceil(log₂(n)) comparisons
```

---

### Problem 4: Median of Medians — Guaranteed O(N) Selection — Very Hard

**Recurrence**: T(n) = T(⌈n/5⌉) + T(7n/10 + 6) + O(n)

```python
def median_of_medians(arr: list, k: int) -> int:
    """
    Find k-th smallest element in O(n) guaranteed time.
    Recurrence: T(n) = T(n/5) + T(7n/10) + O(n)
    
    Proof that T(n) = O(n):
    - At least 3/10·n elements guaranteed ≤ pivot (or ≥ pivot)
    - So recursive call on remaining data is at most T(7n/10 + 6)
    - Pivot finding uses T(n/5) for median of medians
    - By substitution: T(n) ≤ c·(n/5 + 7n/10 + 6) + O(n) = c·9n/10 + ... = O(n)
    """
    if len(arr) <= 5:
        return sorted(arr)[k]
    
    # Step 1: Divide into groups of 5, find each group's median
    chunks = [arr[i:i+5] for i in range(0, len(arr), 5)]
    medians = [sorted(chunk)[len(chunk) // 2] for chunk in chunks]
    
    # Step 2: Recursively find median of medians → pivot
    pivot = median_of_medians(medians, len(medians) // 2)
    
    # Step 3: Partition around pivot
    low  = [x for x in arr if x < pivot]
    mid  = [x for x in arr if x == pivot]
    high = [x for x in arr if x > pivot]
    
    # Step 4: Recurse on correct partition
    if k < len(low):
        return median_of_medians(low, k)
    elif k < len(low) + len(mid):
        return pivot
    else:
        return median_of_medians(high, k - len(low) - len(mid))

# Time:  Θ(n) guaranteed — pivot is always in [30th, 70th] percentile
# Space: O(log n) stack frames in the worst case
```

---

### Problem 5: Strassen Matrix Multiplication — T(n) = 7T(n/2) + n² — Hard

```python
def strassen(A: list, B: list) -> list:
    """
    Strassen's algorithm: T(n) = 7T(n/2) + O(n²)
    Master theorem: p = log₂(7) ≈ 2.807 > 2 → Case 1 → Θ(n^2.807)
    
    Compared to naive: T(n) = 8T(n/2) + O(n²) → Case 2 → Θ(n³)
    Saving ONE recursive call drops complexity from Θ(n³) to Θ(n^2.807)!
    """
    n = len(A)
    if n == 1:
        return [[A[0][0] * B[0][0]]]
    
    def add(X, Y):
        return [[X[i][j] + Y[i][j] for j in range(len(X[0]))] for i in range(len(X))]
    def sub(X, Y):
        return [[X[i][j] - Y[i][j] for j in range(len(X[0]))] for i in range(len(X))]
    
    mid = n // 2
    A11 = [row[:mid] for row in A[:mid]]
    A12 = [row[mid:] for row in A[:mid]]
    A21 = [row[:mid] for row in A[mid:]]
    A22 = [row[mid:] for row in A[mid:]]
    B11 = [row[:mid] for row in B[:mid]]
    B12 = [row[mid:] for row in B[:mid]]
    B21 = [row[:mid] for row in B[mid:]]
    B22 = [row[mid:] for row in B[mid:]]
    
    # 7 recursive multiplications (vs 8 in naive divide-and-conquer)
    M1 = strassen(add(A11, A22), add(B11, B22))
    M2 = strassen(add(A21, A22), B11)
    M3 = strassen(A11, sub(B12, B22))
    M4 = strassen(A22, sub(B21, B11))
    M5 = strassen(add(A11, A12), B22)
    M6 = strassen(sub(A21, A11), add(B11, B12))
    M7 = strassen(sub(A12, A22), add(B21, B22))
    
    C11 = add(sub(add(M1, M4), M5), M7)
    C12 = add(M3, M5)
    C21 = add(M2, M4)
    C22 = add(sub(add(M1, M3), M2), M6)
    
    C = [[0] * n for _ in range(n)]
    for i in range(mid):
        for j in range(mid):
            C[i][j]         = C11[i][j]
            C[i][j+mid]     = C12[i][j]
            C[i+mid][j]     = C21[i][j]
            C[i+mid][j+mid] = C22[i][j]
    return C

# Time:  Θ(n^log₂7) ≈ Θ(n^2.807)
# Space: Θ(n²) for matrix storage + O(log n) stack depth
```

---

### Problem 6: Fibonacci via Matrix Exponentiation — T(n) = T(n/2) + O(1) — Medium

```python
def mat_mul(A: list, B: list) -> list:
    """2×2 matrix multiplication."""
    return [
        [A[0][0]*B[0][0] + A[0][1]*B[1][0], A[0][0]*B[0][1] + A[0][1]*B[1][1]],
        [A[1][0]*B[0][0] + A[1][1]*B[1][0], A[1][0]*B[0][1] + A[1][1]*B[1][1]]
    ]

def mat_pow(M: list, n: int) -> list:
    """
    Matrix exponentiation by repeated squaring.
    Recurrence: T(n) = T(n/2) + O(1) → Θ(log n)
    """
    if n == 1:
        return M
    if n % 2 == 0:
        half = mat_pow(M, n // 2)
        return mat_mul(half, half)
    else:
        return mat_mul(M, mat_pow(M, n - 1))

def fibonacci(n: int) -> int:
    """
    [F(n+1)]   [1 1]^n   [F(1)]
    [F(n)  ] = [1 0]   × [F(0)]
    
    Time:  Θ(log n) — recurrence T(n)=T(n/2)+O(1), Case 2
    Space: O(log n) stack depth
    """
    if n <= 0:
        return 0
    if n == 1:
        return 1
    M = [[1, 1], [1, 0]]
    result = mat_pow(M, n)
    return result[0][1]

for n in [10, 50, 100]:
    print(f"F({n}) = {fibonacci(n)}")
```

---

## Advanced Variations

### When Master Theorem FAILS

1. **Non-polynomial differences**: T(n) = 2T(n/2) + n/log(n)
   - f(n) = n/log(n), n^p = n → f(n)/n^p = 1/log(n) → not polynomial difference → Theorem inapplicable
   - Use extended Master Theorem or Akra-Bazzi

2. **Decreasing recurrences**: T(n) = T(n−1) + T(n−2) + n  
   - Not of form T(n) = aT(n/b) + f(n) → use characteristic equations or Akra-Bazzi extension

3. **Floors and ceilings**: T(n) = T(⌊n/2⌋) + T(⌈n/2⌉) + n  
   - Add O(1) slack to ignore floors/ceilings for asymptotic bounds (valid by smoothness condition)

### Recurrence with Two Different Sub-problems

T(n) = T(αn) + T((1−α)n) + O(n) where 0 < α ≤ 1/2

- Solution is always **Θ(n log n)** regardless of α!
- Because: each level sums to n, depth is log_{1/α}(n) = O(log n)

---

## Edge Cases Bible

1. **n = 0 or n = 1 base cases**: Most recurrences break if base case handling is wrong; always verify T(1) = 1 or T(0) = 0.

2. **Non-power-of-2 inputs**: T(n) = 2T(n/2) + n with floors: T(n) = 2T(⌊n/2⌋) + n. The asymptotic result is unchanged but exact count differs.

3. **Regularity condition for Case 3**: Must verify a·f(n/b) ≤ c·f(n) for c < 1. Fails for f(n) = n^p · (log log n)^(-1) type functions.

4. **Sub-linear f(n) with large a**: T(n) = 100T(n/2) + n → p = log₂(100) ≈ 6.64 → Case 1 → Θ(n^6.64). Surprising — the large branching dominates.

5. **Tail recursion optimization**: Doesn't change recurrence analysis but does reduce space from O(log n) to O(1).

6. **Parallelism**: Recurrence analysis assumes sequential execution. Work vs Span analysis differs for parallel algorithms.

---

## Interview Tips

### What Interviewers Look For

1. **Immediately classify** a recurrence: state a, b, f(n), compute p = log_b(a), compare to f(n).

2. **Distinguish O, Ω, Θ**: Never say "O(n log n)" when you mean Θ(n log n). Interviewers notice.

3. **Amortized vs worst-case**: Push-back when asked about hash map — say "O(1) amortized, O(n) worst case per operation."

4. **The right question to ask yourself**: "What is the *exact* number of times the critical operation executes?" Not just "how many recursive calls?"

5. **Log tricks under pressure**:
   - log₂(10^6) ≈ 20 (since 2^20 ≈ 10^6)
   - log₂(10^9) ≈ 30
   - These let you instantly evaluate algorithm behavior for typical FAANG input sizes

6. **When to use each method**:
   - Master Theorem → clean T(n) = aT(n/b) + f(n) with polynomial f
   - Substitution → when you have a guess and need a proof
   - Recursion tree → when you need intuition or when multiple branches have different sizes
   - Akra-Bazzi → multiple different-sized subproblems or when Master Theorem fails

7. **The "sum of work per level" heuristic**:
   - Work decreasing geometrically → leaf-heavy → root dominates → take work at leaves
   - Work constant per level → n·log(n) type
   - Work increasing geometrically → root-heavy → top level dominates → take f(n)

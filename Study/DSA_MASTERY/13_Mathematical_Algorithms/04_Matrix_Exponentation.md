# Matrix Exponentiation — Advanced Mastery Guide

> **Turn O(N) recurrences into O(log N).** Matrix exponentiation is the secret weapon for linear recurrences, graph path counting, and state machines with large steps.

---

## Table of Contents
1. [Matrix Multiplication — O(N³)](#matrix-multiply)
2. [Matrix Exponentiation — O(N³ log K)](#matrix-pow)
3. [Fibonacci in O(log N)](#fibonacci)
4. [General Linear Recurrences](#linear-recurrence)
5. [Graph Path Counting](#graph-paths)
6. [Problems 1–6 with Full Solutions](#problems)
7. [Template & Patterns](#template)

---

## 1. Matrix Multiplication — O(N³) <a name="matrix-multiply"></a>

```python
MOD = 10**9 + 7

def mat_mul(A: list[list[int]], B: list[list[int]], mod: int = MOD) -> list[list[int]]:
    """
    Multiply two matrices A (n×m) and B (m×p).
    Time: O(n × m × p), Space: O(n × p)
    
    For square matrices: O(N³)
    """
    n = len(A)
    m = len(B)
    p = len(B[0])
    
    C = [[0] * p for _ in range(n)]
    for i in range(n):
        for k in range(m):
            if A[i][k] == 0:
                continue  # optimization: skip zero entries
            for j in range(p):
                C[i][j] = (C[i][j] + A[i][k] * B[k][j]) % mod
    return C

def mat_mul_numpy_style(A, B, mod=MOD):
    """Using list comprehension — slightly more Pythonic."""
    n, m, p = len(A), len(B), len(B[0])
    return [[sum(A[i][k] * B[k][j] for k in range(m)) % mod 
             for j in range(p)] 
            for i in range(n)]

# Identity matrix of size n
def identity(n: int) -> list[list[int]]:
    return [[1 if i == j else 0 for j in range(n)] for i in range(n)]
```

---

## 2. Matrix Exponentiation — O(N³ log K) <a name="matrix-pow"></a>

```python
def mat_pow(M: list[list[int]], k: int, mod: int = MOD) -> list[list[int]]:
    """
    Compute M^k using repeated squaring.
    Time: O(N³ log K) where N = matrix size, K = exponent
    Space: O(N²)
    
    Same as binary exponentiation but for matrices.
    """
    n = len(M)
    result = identity(n)  # M^0 = I
    
    base = [row[:] for row in M]  # copy
    
    while k > 0:
        if k & 1:
            result = mat_mul(result, base, mod)
        base = mat_mul(base, base, mod)
        k >>= 1
    
    return result

# Why correct?
# k in binary: k = b_t * 2^t + ... + b_1 * 2 + b_0
# M^k = M^(b_t * 2^t) × ... × M^(b_0)
# Each M^(2^i) = (M^(2^(i-1)))^2 — computed by squaring
# Accumulate those where bit b_i = 1
```

---

## 3. Fibonacci in O(log N) <a name="fibonacci"></a>

### The Fibonacci Matrix Identity

The key insight: Fibonacci satisfies a 2-term linear recurrence → 2×2 matrix.

```
[F(n+1)]   [1 1]   [F(n)  ]
[F(n)  ] = [1 0] × [F(n-1)]

Therefore:
[F(n+1)]   [1 1]^n   [F(1)]   [1 1]^n   [1]
[F(n)  ] = [1 0]   × [F(0)] = [1 0]   × [0]

So F(n) = ([1 1]^n)[1][0]  (row 1, col 0 of matrix power, 0-indexed)
```

```python
def fibonacci(n: int, mod: int = MOD) -> int:
    """
    Nth Fibonacci number mod p using matrix exponentiation.
    F(0)=0, F(1)=1, F(2)=1, F(3)=2, ...
    Time: O(log N), Space: O(1)
    """
    if n == 0:
        return 0
    if n <= 2:
        return 1
    
    M = [[1, 1], [1, 0]]
    result = mat_pow(M, n - 1, mod)
    return result[0][0]  # F(n) = top-left entry of M^(n-1)

# Verification:
# M^1 = [[1,1],[1,0]] → F(2)=1 ✓
# M^2 = [[2,1],[1,1]] → F(3)=2 ✓
# M^3 = [[3,2],[2,1]] → F(4)=3 ✓
# M^4 = [[5,3],[3,2]] → F(5)=5 ✓

def fibonacci_fast_doubling(n: int, mod: int = MOD) -> int:
    """
    Fast doubling formulas (derived from matrix identity):
    F(2k)   = F(k) × (2×F(k+1) - F(k))
    F(2k+1) = F(k)² + F(k+1)²
    
    Time: O(log N), Space: O(log N) recursion stack
    Same complexity as matrix method but ~2× faster in practice (only 2 multiplications per step).
    """
    def _fib(n):
        if n == 0:
            return 0, 1  # F(0), F(1)
        a, b = _fib(n >> 1)
        c = a * (2 * b - a) % mod
        d = (a * a + b * b) % mod
        if n & 1:
            return d, (c + d) % mod
        return c, d
    
    return _fib(n)[0]
```

---

## 4. General Linear Recurrences <a name="linear-recurrence"></a>

### The General Framework

**Any linear recurrence** of the form:
```
f(n) = c₁f(n-1) + c₂f(n-2) + ... + cₖf(n-k) + g(n)
```
can be represented as a matrix multiplication.

For a k-term homogeneous recurrence:

```
[f(n)  ]   [c₁ c₂ ... cₖ]   [f(n-1)  ]
[f(n-1)]   [1  0  ... 0  ]   [f(n-2)  ]
[...   ] = [0  1  ... 0  ] × [...      ]
[f(n-k)]   [0  0  ... 1  ]   [f(n-k-1)]
         companion matrix
```

```python
def linear_recurrence_matrix(
    coefficients: list[int],  # [c1, c2, ..., ck]
    initial: list[int],       # [f(0), f(1), ..., f(k-1)]
    n: int,
    mod: int = MOD
) -> int:
    """
    Compute f(n) for linear recurrence:
    f(n) = c1*f(n-1) + c2*f(n-2) + ... + ck*f(n-k)
    
    Time: O(k³ log n), Space: O(k²)
    """
    k = len(coefficients)
    if n < k:
        return initial[n] % mod
    
    # Build companion matrix
    M = [[0] * k for _ in range(k)]
    M[0] = [c % mod for c in coefficients]
    for i in range(1, k):
        M[i][i-1] = 1
    
    # Raise to power (n - k + 1)
    Mk = mat_pow(M, n - k + 1, mod)
    
    # Multiply by initial state vector [f(k-1), f(k-2), ..., f(0)]
    state = [initial[k-1-i] % mod for i in range(k)]
    
    result = 0
    for j in range(k):
        result = (result + Mk[0][j] * state[j]) % mod
    
    return result

# Example: Tribonacci T(n) = T(n-1) + T(n-2) + T(n-3)
def tribonacci(n: int, mod: int = MOD) -> int:
    """T(0)=0, T(1)=1, T(2)=1, T(n)=T(n-1)+T(n-2)+T(n-3)"""
    return linear_recurrence_matrix(
        coefficients=[1, 1, 1],
        initial=[0, 1, 1],  # T(0), T(1), T(2)
        n=n,
        mod=mod
    )

# Verify: T(0..7) = 0,1,1,2,4,7,13,24
```

---

## 5. Graph Path Counting <a name="graph-paths"></a>

### Counting Paths of Length Exactly K

**Key theorem:** The (i,j) entry of the adjacency matrix `A^k` = number of walks of length k from vertex i to vertex j.

```python
def count_paths_length_k(adj: list[list[int]], k: int, src: int, dst: int, 
                          mod: int = MOD) -> int:
    """
    Count paths of exact length k from src to dst in directed graph.
    adj[i][j] = number of edges from i to j (usually 0 or 1).
    
    Time: O(N³ log K), Space: O(N²)
    """
    Ak = mat_pow(adj, k, mod)
    return Ak[src][dst]

def count_all_paths_length_k(adj: list[list[int]], k: int, 
                              mod: int = MOD) -> list[list[int]]:
    """All-pairs path count for length exactly k."""
    return mat_pow(adj, k, mod)

# Application: Number of ways to arrange colored tiles
# State: current color configuration → transition matrix
# Use matrix power for large n

# Example: Two-state Markov chain
def markov_after_k_steps(transition: list[list[float]], 
                          initial_state: list[float], 
                          k: int) -> list[float]:
    """
    Markov chain: find probability distribution after k steps.
    (For floats — no modulo needed)
    """
    def mat_mul_float(A, B):
        n = len(A)
        C = [[0.0] * n for _ in range(n)]
        for i in range(n):
            for k_ in range(n):
                for j in range(n):
                    C[i][j] += A[i][k_] * B[k_][j]
        return C
    
    def mat_pow_float(M, p):
        n = len(M)
        result = [[1.0 if i==j else 0.0 for j in range(n)] for i in range(n)]
        while p > 0:
            if p & 1:
                result = mat_mul_float(result, M)
            M = mat_mul_float(M, M)
            p >>= 1
        return result
    
    Tk = mat_pow_float(transition, k)
    n = len(initial_state)
    result = [sum(initial_state[j] * Tk[j][i] for j in range(n)) for i in range(n)]
    return result
```

---

## 6. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Fibonacci Number (LeetCode 509)

```python
def fib(n: int) -> int:
    """
    LeetCode 509. 
    Comparison of approaches:
    - O(N) DP: simple, fine for n ≤ 10^6
    - O(log N) matrix: needed for n > 10^8
    """
    # O(N) DP for small n
    if n <= 1:
        return n
    a, b = 0, 1
    for _ in range(2, n + 1):
        a, b = b, a + b
    return b

def fib_matrix(n: int) -> int:
    """O(log N) for large n."""
    return fibonacci(n, mod=10**18)  # large mod to avoid overflow masking
```

---

### Problem 2: Climbing Stairs (Fibonacci Variant)
**LeetCode 70**

```python
def climb_stairs(n: int) -> int:
    """
    f(n) = f(n-1) + f(n-2), f(1)=1, f(2)=2
    = Fibonacci(n+1)
    Time: O(N) DP or O(log N) matrix
    """
    if n <= 2:
        return n
    return fibonacci(n + 1)  # stairs(n) = Fib(n+1)

def climb_stairs_k_steps(n: int, k: int) -> int:
    """
    Generalization: can take 1 to k steps.
    f(n) = f(n-1) + f(n-2) + ... + f(n-k)
    k-term recurrence → k×k matrix.
    Time: O(k³ log N)
    """
    return linear_recurrence_matrix(
        coefficients=[1] * k,
        initial=[min(2**i, i+1) for i in range(k)],  # careful base cases
        n=n
    )
```

---

### Problem 3: Tribonacci Number (LeetCode 1137)

```python
def tribonacci_lc(n: int) -> int:
    """
    LeetCode 1137. T(0)=0, T(1)=1, T(2)=1, T(n)=T(n-1)+T(n-2)+T(n-3).
    O(log N) using matrix exponentiation.
    """
    if n == 0:
        return 0
    if n <= 2:
        return 1
    
    M = [
        [1, 1, 1],
        [1, 0, 0],
        [0, 1, 0]
    ]
    
    # [T(n), T(n-1), T(n-2)] = M^(n-2) × [T(2), T(1), T(0)]
    Mk = mat_pow(M, n - 2, 10**18)  # no mod for this problem
    
    initial = [1, 1, 0]  # T(2), T(1), T(0)
    result = sum(Mk[0][j] * initial[j] for j in range(3))
    return result
```

---

### Problem 4: Count Paths of Length K in Grid
**Count walks of exactly K steps on an N×N grid**

```python
def count_grid_paths(n: int, k: int, src: tuple, dst: tuple, mod: int = MOD) -> int:
    """
    Count paths of exactly k steps from src to dst on n×n grid (4-directional).
    
    State = cell (r,c) encoded as r*n + c.
    Build adjacency matrix, raise to k-th power.
    
    Time: O(N⁶ log K) — N² states, each with 4 neighbors.
    Practical for small N.
    """
    N = n * n
    adj = [[0] * N for _ in range(N)]
    
    for r in range(n):
        for c in range(n):
            state = r * n + c
            for dr, dc in [(0,1),(0,-1),(1,0),(-1,0)]:
                nr, nc = r + dr, c + dc
                if 0 <= nr < n and 0 <= nc < n:
                    adj[state][nr * n + nc] = 1
    
    Ak = mat_pow(adj, k, mod)
    src_state = src[0] * n + src[1]
    dst_state = dst[0] * n + dst[1]
    return Ak[src_state][dst_state]
```

---

### Problem 5: Knight Moves After K Steps
**Count positions reachable by knight in exactly K moves**

```python
def knight_paths_exactly_k(board_size: int, k: int, src: tuple, dst: tuple, 
                            mod: int = MOD) -> int:
    """
    Count ways knight can go from src to dst in exactly k moves.
    
    State = (r, c) on board_size × board_size board.
    """
    n = board_size
    N = n * n
    MOVES = [(2,1),(2,-1),(-2,1),(-2,-1),(1,2),(1,-2),(-1,2),(-1,-2)]
    
    adj = [[0] * N for _ in range(N)]
    for r in range(n):
        for c in range(n):
            state = r * n + c
            for dr, dc in MOVES:
                nr, nc = r + dr, c + dc
                if 0 <= nr < n and 0 <= nc < n:
                    adj[state][nr * n + nc] = 1
    
    Ak = mat_pow(adj, k, mod)
    return Ak[src[0] * n + src[1]][dst[0] * n + dst[1]]
```

---

### Problem 6: Count Strings with No Adjacent Same Characters
**Count binary strings of length N with no two adjacent same characters**

```python
def count_valid_strings(n: int, mod: int = MOD) -> int:
    """
    States: last character was '0' or '1'.
    Transition: from state '0' → state '1' only; from '1' → '0' only.
    
    Let f(n, c) = count of valid strings of length n ending with c.
    f(n, 0) = f(n-1, 1)
    f(n, 1) = f(n-1, 0)
    
    Matrix: [[0,1],[1,0]]^(n-1) × [[1],[1]]
    """
    if n == 1:
        return 2
    
    M = [[0, 1], [1, 0]]
    Mk = mat_pow(M, n - 1, mod)
    
    # Initial state: [f(1,0), f(1,1)] = [1, 1]
    f0 = (Mk[0][0] + Mk[0][1]) % mod
    f1 = (Mk[1][0] + Mk[1][1]) % mod
    return (f0 + f1) % mod

# Verify: n=1→2, n=2→2 (01,10), n=3→2 (010,101), n=4→... wait
# Actually f(n) = 2 for all n ≥ 1 for binary alphabet
# For k-letter alphabet with no adjacent same: k × (k-1)^(n-1)
def count_valid_strings_k_alphabet(n: int, k: int, mod: int = MOD) -> int:
    """k-letter alphabet, no adjacent same. Simple formula: k × (k-1)^(n-1)."""
    if n == 0:
        return 1
    return k * pow(k - 1, n - 1, mod) % mod
```

---

## 7. Template & Patterns <a name="template"></a>

### The Universal Template

```python
class MatrixExponentiation:
    """
    Template for any problem reducible to matrix power.
    
    Usage:
    1. Identify recurrence f(n) = f(prev states)
    2. Build companion matrix M
    3. Compute M^n × initial_state_vector
    4. Read off answer from result vector
    """
    
    def __init__(self, mod: int = 10**9 + 7):
        self.mod = mod
    
    def solve(self, recurrence_matrix, initial_vector, n):
        """
        Compute M^n × v.
        Time: O(k³ log n) where k = matrix dimension
        """
        if n == 0:
            return initial_vector
        
        Mn = mat_pow(recurrence_matrix, n, self.mod)
        k = len(initial_vector)
        
        result = [0] * k
        for i in range(k):
            for j in range(k):
                result[i] = (result[i] + Mn[i][j] * initial_vector[j]) % self.mod
        
        return result

# Pattern recognition:
# 
# f(n) = a×f(n-1) + b×f(n-2):
# Matrix: [[a, b], [1, 0]], initial: [f(1), f(0)]
# 
# f(n) = a×f(n-1) + b×f(n-2) + c×f(n-3):
# Matrix: [[a,b,c],[1,0,0],[0,1,0]], initial: [f(2),f(1),f(0)]
# 
# f(n) = f(n-1) + g(n) where g also has a recurrence:
# Stack both into one larger matrix
```

### Adding a Constant Term

For `f(n) = c₁f(n-1) + ... + cₖf(n-k) + D` (non-homogeneous):

```python
def linear_recurrence_with_constant(coefficients, initial, D, n, mod=MOD):
    """
    Homogenize: add extra state tracking accumulated constant.
    
    Augmented state: [f(n), f(n-1), ..., f(n-k+1), 1]
    
    Matrix augmented by constant column and bottom row [0,...,0,1]
    """
    k = len(coefficients)
    size = k + 1
    
    # Build augmented matrix
    M = [[0] * size for _ in range(size)]
    for j in range(k):
        M[0][j] = coefficients[j] % mod
    M[0][k] = D % mod  # constant term
    for i in range(1, k):
        M[i][i-1] = 1
    M[k][k] = 1  # constant propagation
    
    if n < k:
        return initial[n] % mod
    
    Mn = mat_pow(M, n - k + 1, mod)
    
    # Initial state: [f(k-1), f(k-2), ..., f(0), 1]
    state = [initial[k-1-i] % mod for i in range(k)] + [1]
    
    result = 0
    for j in range(size):
        result = (result + Mn[0][j] * state[j]) % mod
    
    return result
```

---

## Complexity Summary

| Problem | Matrix Size | Complexity |
|---------|-------------|------------|
| Fibonacci | 2×2 | O(8 log N) = O(log N) |
| k-term recurrence | k×k | O(k³ log N) |
| Graph path count (V vertices) | V×V | O(V³ log K) |
| Knight on N×N board | N²×N² | O(N⁶ log K) |

### Practical Limits

```
Matrix size | log N factor | Feasible?
─────────────────────────────────────
2×2         | any N        | Always (O(8 log N))
3×3         | any N        | Always (O(27 log N))
k×k (k≤10) | N ≤ 10^18   | Yes (k³ ≤ 1000, log N ≤ 60)
k×k (k=100)| any N        | Borderline (10^6 × 60 = 6×10^7)
V×V graphs  | small V≤50  | OK (V³ log K ≤ 125000 × 60)
```

### Interview Tips

> **"When do you need matrix exponentiation vs simple DP?"** — When n ≥ 10^9 and the recurrence has ≤ ~10 terms. Simple DP hits TLE; matrix exponentiation in O(k³ log n) is fast enough.

> **"How to identify the recurrence matrix?"** — Write out the recurrence, define state vector [f(n), f(n-1), ...], find the linear transformation M such that `state(n) = M × state(n-1)`.

> **"Is there anything faster than O(k³ log N)?"** — Cayley-Hamilton theorem: any k×k matrix satisfies its own characteristic polynomial of degree k. This enables Berlekamp-Massey + polynomial exponentiation for O(k² log N) or even O(k log k log N). Overkill for interviews.

---

*Previous: [Combinatorics ←](03_Combinatorics.md) | Next: [Backtracking Patterns →](../14_Recursion_Backtracking_DC/01_Backtracking_Patterns.md)*

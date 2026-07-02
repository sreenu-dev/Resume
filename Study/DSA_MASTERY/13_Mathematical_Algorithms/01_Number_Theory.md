# Number Theory — Advanced Mastery Guide

> **Mathematical bedrock of competitive programming.** From GCD proofs to Miller-Rabin primality testing — everything needed to ace mathematical FAANG problems.

---

## Table of Contents
1. [GCD & LCM — Euclidean Algorithm with Proof](#gcd)
2. [Extended Euclidean & Bezout's Identity](#extended-gcd)
3. [Modular Arithmetic Toolkit](#modular)
4. [Sieve of Eratosthenes — O(N log log N)](#sieve)
5. [Linear Sieve — O(N)](#linear-sieve)
6. [Fast Exponentiation & Modular Inverse](#fast-exp)
7. [Chinese Remainder Theorem](#crt)
8. [Euler's Totient & Fermat's Little Theorem](#euler)
9. [Miller-Rabin Primality Test](#miller-rabin)
10. [Problems 1–8 with Full Solutions](#problems)
11. [Interview Cheat Sheet](#cheat-sheet)

---

## 1. GCD & LCM — Euclidean Algorithm <a name="gcd"></a>

### The Algorithm

```python
def gcd(a: int, b: int) -> int:
    """
    Euclidean algorithm.
    Time: O(log min(a, b))
    
    Proof: gcd(a, b) = gcd(b, a mod b)
    Key lemma: any divisor of a and b also divides a mod b, and vice versa.
    """
    while b:
        a, b = b, a % b
    return a

def lcm(a: int, b: int) -> int:
    """LCM via GCD. lcm(a,b) = a * b / gcd(a,b). O(log min(a,b))."""
    return a // gcd(a, b) * b  # divide first to avoid overflow
```

### Why O(log min(a, b))? — Fibonacci Argument

**Theorem:** If `gcd(a, b)` takes N steps, then `a >= F(N+2)` and `b >= F(N+1)` where F is Fibonacci.

**Proof by induction:**
- Base: gcd(a, b) with a > b: after one step, we have gcd(b, a mod b)
- If a mod b = 0, we're done (1 step)
- Otherwise, `a mod b < b < a`, so problem shrinks
- Worst case: consecutive Fibonacci numbers → N steps with F(N) ≈ φ^N/√5
- Therefore, `N ≤ log_φ(max(a,b)) ≈ 1.44 log₂(max(a,b))` → O(log)

**Consequence:** `gcd(F(n+1), F(n))` takes exactly `n` steps — Fibonacci pairs are the worst case.

---

## 2. Extended Euclidean Algorithm <a name="extended-gcd"></a>

### Bezout's Identity

**Theorem:** For integers a, b with gcd(a, b) = d, there exist integers x, y such that:
```
a·x + b·y = d
```

```python
def extended_gcd(a: int, b: int) -> tuple[int, int, int]:
    """
    Extended Euclidean Algorithm.
    Returns (gcd, x, y) such that a*x + b*y = gcd(a, b).
    Time: O(log min(a, b))
    """
    if b == 0:
        return a, 1, 0
    
    g, x1, y1 = extended_gcd(b, a % b)
    x = y1
    y = x1 - (a // b) * y1
    return g, x, y

# Iterative version (avoids stack overflow for large inputs):
def extended_gcd_iter(a: int, b: int) -> tuple[int, int, int]:
    old_r, r = a, b
    old_s, s = 1, 0
    old_t, t = 0, 1
    
    while r != 0:
        q = old_r // r
        old_r, r = r, old_r - q * r
        old_s, s = s, old_s - q * s
        old_t, t = t, old_t - q * t
    
    return old_r, old_s, old_t  # gcd, x, y

# Applications:
# 1. Modular inverse: a^(-1) mod m = x where a*x ≡ 1 (mod m)
# 2. Solving linear Diophantine equations: ax + by = c (has solution iff gcd(a,b) | c)
# 3. Chinese Remainder Theorem
```

### Linear Diophantine Equations

```python
def solve_diophantine(a: int, b: int, c: int):
    """
    Find integer solutions to ax + by = c.
    Returns (x0, y0, dx, dy) where general solution is:
    x = x0 + (b/d)*t, y = y0 - (a/d)*t for any integer t
    
    Returns None if no solution exists.
    """
    g, x0, y0 = extended_gcd(a, b)
    if c % g != 0:
        return None  # no integer solution
    
    scale = c // g
    x0, y0 = x0 * scale, y0 * scale
    dx, dy = b // g, a // g  # step sizes for parameter t
    
    return x0, y0, dx, dy
```

---

## 3. Modular Arithmetic Toolkit <a name="modular"></a>

```python
MOD = 10**9 + 7  # prime, standard in competitive programming

# Basic operations (all mod p):
def mod_add(a, b, mod=MOD):
    return (a + b) % mod

def mod_mul(a, b, mod=MOD):
    return (a * b) % mod

def mod_sub(a, b, mod=MOD):
    return (a - b + mod) % mod

def mod_pow(base, exp, mod=MOD):
    """Fast modular exponentiation. O(log exp)."""
    result = 1
    base %= mod
    while exp > 0:
        if exp & 1:
            result = result * base % mod
        base = base * base % mod
        exp >>= 1
    return result

def mod_inv(a: int, mod: int = MOD) -> int:
    """
    Modular inverse of a mod p (p must be prime).
    By Fermat's little theorem: a^(-1) ≡ a^(p-2) (mod p)
    Time: O(log p)
    """
    return mod_pow(a, mod - 2, mod)

def mod_inv_extended(a: int, mod: int) -> int:
    """
    Modular inverse via extended GCD. Works for non-prime moduli.
    Requires gcd(a, mod) = 1.
    Time: O(log mod)
    """
    g, x, _ = extended_gcd(a % mod, mod)
    if g != 1:
        raise ValueError(f"gcd({a}, {mod}) = {g} ≠ 1, inverse doesn't exist")
    return x % mod

# Division mod p:
def mod_div(a: int, b: int, mod: int = MOD) -> int:
    return mod_mul(a, mod_inv(b, mod), mod)
```

---

## 4. Sieve of Eratosthenes — O(N log log N) <a name="sieve"></a>

```python
def sieve_of_eratosthenes(n: int) -> list[int]:
    """
    Find all primes up to n.
    Time: O(N log log N), Space: O(N)
    
    Why O(N log log N)?
    Work done = N/2 + N/3 + N/5 + ... (sum over primes p <= N of N/p)
    = N × Σ(1/p for p prime, p<=N)
    ≈ N × log(log N)  [Mertens' theorem]
    """
    is_prime = [True] * (n + 1)
    is_prime[0] = is_prime[1] = False
    
    p = 2
    while p * p <= n:
        if is_prime[p]:
            # Mark multiples starting from p² (smaller ones already marked)
            for i in range(p * p, n + 1, p):
                is_prime[i] = False
        p += 1
    
    return [i for i in range(2, n + 1) if is_prime[i]]

def sieve_smallest_prime_factor(n: int) -> list[int]:
    """
    Compute smallest prime factor for each number up to n.
    Enables O(log n) prime factorization for any number.
    Time: O(N log log N), Space: O(N)
    """
    spf = list(range(n + 1))  # spf[i] = i initially
    for i in range(2, int(n**0.5) + 1):
        if spf[i] == i:  # i is prime
            for j in range(i * i, n + 1, i):
                if spf[j] == j:  # not yet assigned
                    spf[j] = i
    return spf

def factorize_spf(n: int, spf: list[int]) -> dict[int, int]:
    """
    Prime factorization using SPF array.
    Time: O(log n) per query after O(N log log N) preprocessing.
    """
    factors = {}
    while n > 1:
        p = spf[n]
        while n % p == 0:
            factors[p] = factors.get(p, 0) + 1
            n //= p
    return factors
```

---

## 5. Linear Sieve — O(N) <a name="linear-sieve"></a>

```python
def linear_sieve(n: int) -> tuple[list[int], list[int]]:
    """
    Linear sieve — exactly O(N) time (each composite marked exactly once).
    Returns (primes, spf) where spf[i] = smallest prime factor of i.
    
    Key insight: mark composite n as spf[n] = p when processing n//p,
    but STOP when p divides n//p to avoid double-marking.
    """
    spf = [0] * (n + 1)
    primes = []
    
    for i in range(2, n + 1):
        if spf[i] == 0:  # i is prime
            spf[i] = i
            primes.append(i)
        
        for p in primes:
            if p > spf[i] or i * p > n:
                break
            spf[i * p] = p
    
    return primes, spf

# Why exactly O(N)?
# Each composite number c = i * p has spf[c] = p assigned exactly once,
# when we process i with the smallest prime p = spf[i].
# Total markings = total composites <= N → O(N).
```

---

## 6. Fast Exponentiation & Modular Inverse <a name="fast-exp"></a>

```python
def binary_exponentiation(base: int, exp: int) -> int:
    """
    Compute base^exp without modulo.
    Time: O(log exp), using repeated squaring.
    
    Proof of correctness:
    exp in binary: exp = b_k * 2^k + ... + b_1 * 2 + b_0
    base^exp = base^(b_k * 2^k) * ... * base^(b_0)
             = (base^(2^k))^b_k * ... * base^b_0
    Each base^(2^i) = (base^(2^(i-1)))^2 — computed iteratively
    """
    result = 1
    while exp > 0:
        if exp & 1:
            result *= base
        base *= base
        exp >>= 1
    return result

# Matrix exponentiation (see Matrix_Exponentiation.md for full details)
def matrix_multiply(A, B, mod=MOD):
    n = len(A)
    C = [[0] * n for _ in range(n)]
    for i in range(n):
        for j in range(n):
            for k in range(n):
                C[i][j] = (C[i][j] + A[i][k] * B[k][j]) % mod
    return C

def matrix_pow(M, p, mod=MOD):
    n = len(M)
    result = [[1 if i == j else 0 for j in range(n)] for i in range(n)]  # identity
    while p > 0:
        if p & 1:
            result = matrix_multiply(result, M, mod)
        M = matrix_multiply(M, M, mod)
        p >>= 1
    return result

# Precompute factorials and inverse factorials for combinations
def precompute_factorials(n: int, mod: int = MOD):
    """O(N) preprocessing for O(1) nCr queries."""
    fact = [1] * (n + 1)
    for i in range(1, n + 1):
        fact[i] = fact[i-1] * i % mod
    
    inv_fact = [1] * (n + 1)
    inv_fact[n] = mod_pow(fact[n], mod - 2, mod)
    for i in range(n - 1, -1, -1):
        inv_fact[i] = inv_fact[i+1] * (i+1) % mod
    
    def ncr(n: int, r: int) -> int:
        if r < 0 or r > n:
            return 0
        return fact[n] * inv_fact[r] % mod * inv_fact[n-r] % mod
    
    return fact, inv_fact, ncr
```

---

## 7. Chinese Remainder Theorem <a name="crt"></a>

**Theorem:** For pairwise coprime moduli m₁, m₂, ..., mₖ, the system:
```
x ≡ a₁ (mod m₁)
x ≡ a₂ (mod m₂)
...
x ≡ aₖ (mod mₖ)
```
has a unique solution mod M = m₁ × m₂ × ... × mₖ.

```python
def crt(remainders: list[int], moduli: list[int]) -> tuple[int, int]:
    """
    Chinese Remainder Theorem.
    Returns (x, M) such that x ≡ remainders[i] (mod moduli[i]) for all i.
    x is in [0, M).
    
    Requires moduli to be pairwise coprime.
    Time: O(k log max_modulus)
    """
    M = 1
    for m in moduli:
        M *= m
    
    x = 0
    for ai, mi in zip(remainders, moduli):
        Mi = M // mi
        yi = mod_inv_extended(Mi, mi)  # Mi * yi ≡ 1 (mod mi)
        x = (x + ai * Mi * yi) % M
    
    return x, M

def crt_two(r1: int, m1: int, r2: int, m2: int) -> tuple[int, int]:
    """
    Merge two congruences using Extended GCD.
    Works even if m1 and m2 are NOT coprime (if solution exists).
    Returns (x, lcm(m1, m2)) or raises if no solution.
    """
    g, p, q = extended_gcd(m1, m2)
    if (r2 - r1) % g != 0:
        raise ValueError("No solution: incompatible congruences")
    
    lcm_val = m1 * m2 // g
    x = (r1 + m1 * ((r2 - r1) // g * p % (m2 // g))) % lcm_val
    return x, lcm_val
```

---

## 8. Euler's Totient & Fermat's Little Theorem <a name="euler"></a>

```python
def euler_totient(n: int) -> int:
    """
    φ(n) = count of integers in [1,n] coprime to n.
    Time: O(√n)
    
    Formula: φ(n) = n × Π(1 - 1/p) for each prime p dividing n
    """
    result = n
    p = 2
    temp = n
    
    while p * p <= temp:
        if temp % p == 0:
            while temp % p == 0:
                temp //= p
            result -= result // p
        p += 1
    
    if temp > 1:  # remaining prime factor
        result -= result // temp
    
    return result

def totient_sieve(n: int) -> list[int]:
    """Compute φ(i) for all i in [1, n]. Time: O(N log log N)."""
    phi = list(range(n + 1))
    
    for i in range(2, n + 1):
        if phi[i] == i:  # i is prime
            for j in range(i, n + 1, i):
                phi[j] -= phi[j] // i
    
    return phi

# Fermat's Little Theorem:
# If p is prime and gcd(a, p) = 1, then a^(p-1) ≡ 1 (mod p)
# Corollary: a^(-1) ≡ a^(p-2) (mod p)
#
# Euler's Generalization:
# If gcd(a, n) = 1, then a^φ(n) ≡ 1 (mod n)
# (Fermat's is the special case n = p, φ(p) = p-1)

def mod_inv_fermat(a: int, p: int) -> int:
    """a^(-1) mod p using Fermat's little theorem. p must be prime."""
    return mod_pow(a, p - 2, p)
```

---

## 9. Miller-Rabin Primality Test <a name="miller-rabin"></a>

```python
def miller_rabin(n: int, witnesses: list[int] = None) -> bool:
    """
    Deterministic Miller-Rabin for n < 3,317,044,064,679,887,385,961,981.
    For competitive programming, specific witness sets guarantee correctness.
    
    Time: O(k log²n) where k = number of witnesses
    """
    if n < 2:
        return False
    if n == 2 or n == 3:
        return True
    if n % 2 == 0:
        return False
    
    # Write n-1 as 2^r * d
    r, d = 0, n - 1
    while d % 2 == 0:
        r += 1
        d //= 2
    
    # Deterministic witnesses for n < 3.3 × 10^24
    if witnesses is None:
        witnesses = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37]
    
    for a in witnesses:
        if a >= n:
            continue
        
        x = mod_pow(a, d, n)
        
        if x == 1 or x == n - 1:
            continue
        
        for _ in range(r - 1):
            x = x * x % n
            if x == n - 1:
                break
        else:
            return False  # composite
    
    return True  # probably prime (deterministic for listed witnesses)

# Fast large prime check:
def is_prime(n: int) -> bool:
    """O(log²n) deterministic primality test."""
    if n < 1000:
        # Small primes: trial division
        small_primes = sieve_of_eratosthenes(999)
        return n in set(small_primes)
    return miller_rabin(n)

# Application: factorize large numbers using Pollard's rho
import math

def pollard_rho(n: int) -> int:
    """Find a non-trivial factor of n. O(n^(1/4)) expected."""
    if n % 2 == 0:
        return 2
    
    x = random.randint(2, n - 1)
    y = x
    c = random.randint(1, n - 1)
    d = 1
    
    while d == 1:
        x = (x * x + c) % n
        y = (y * y + c) % n
        y = (y * y + c) % n
        d = math.gcd(abs(x - y), n)
    
    return d if d != n else None

def factorize_large(n: int) -> dict[int, int]:
    """
    Complete prime factorization using Miller-Rabin + Pollard's rho.
    Time: O(n^(1/4) polylog n)
    """
    if n <= 1:
        return {}
    
    factors = {}
    
    def _factor(n):
        if n == 1:
            return
        if is_prime(n):
            factors[n] = factors.get(n, 0) + 1
            return
        
        # Find a factor
        d = None
        while d is None:
            d = pollard_rho(n)
        
        _factor(d)
        _factor(n // d)
    
    _factor(n)
    return factors
```

---

## 10. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Fibonacci Mod Large Prime
**Nth Fibonacci number mod 10^9 + 7**

```python
def fib_mod(n: int, mod: int = MOD) -> int:
    """
    Fibonacci using matrix exponentiation.
    Time: O(log N), Space: O(1)
    
    [F(n+1)]   [1 1]^n   [F(1)]
    [F(n)  ] = [1 0]   × [F(0)]
    """
    if n <= 0:
        return 0
    if n == 1:
        return 1
    
    M = [[1, 1], [1, 0]]
    result = matrix_pow(M, n - 1, mod)
    return result[0][0]

# Alternative: fast doubling (constant factor 2× faster)
def fib_fast_doubling(n: int, mod: int = MOD) -> int:
    """
    Fast doubling formulas:
    F(2k)   = F(k) × (2F(k+1) - F(k))
    F(2k+1) = F(k)² + F(k+1)²
    
    Time: O(log N), Space: O(log N) stack
    """
    def _fib(n):
        if n == 0:
            return 0, 1
        a, b = _fib(n >> 1)
        c = a * (2 * b - a) % mod
        d = (a * a + b * b) % mod
        if n & 1:
            return d, (c + d) % mod
        return c, d
    
    return _fib(n)[0]

assert fib_mod(10) == 55
assert fib_fast_doubling(10) == 55
assert fib_mod(100) == fib_fast_doubling(100)
```

---

### Problem 2: Count Divisors & Sum of Divisors

```python
def count_divisors(n: int) -> int:
    """
    Number of divisors of n.
    If n = p1^a1 × p2^a2 × ... × pk^ak, then d(n) = (a1+1)(a2+1)...(ak+1).
    Time: O(√n)
    """
    count = 0
    i = 1
    while i * i <= n:
        if n % i == 0:
            count += 2
            if i * i == n:
                count -= 1
        i += 1
    return count

def sum_divisors(n: int) -> int:
    """
    Sum of all divisors of n.
    If n = Π pᵢ^aᵢ, then σ(n) = Π (pᵢ^(aᵢ+1) - 1)/(pᵢ - 1)
    Time: O(√n)
    """
    total = 0
    i = 1
    while i * i <= n:
        if n % i == 0:
            total += i
            if i != n // i:
                total += n // i
        i += 1
    return total

def count_divisors_range(n: int) -> list[int]:
    """
    Count divisors for all numbers 1 to n.
    Time: O(N log N), Space: O(N)
    """
    d = [0] * (n + 1)
    for i in range(1, n + 1):
        for j in range(i, n + 1, i):
            d[j] += 1
    return d
```

---

### Problem 3: GCD of Array & LCM of Array

```python
from math import gcd
from functools import reduce

def gcd_array(arr: list[int]) -> int:
    """GCD of all elements. Time: O(N log max)."""
    return reduce(gcd, arr)

def lcm_array(arr: list[int]) -> int:
    """
    LCM of all elements. Time: O(N log max).
    Warning: result can be astronomically large — use modular arithmetic if needed.
    """
    def lcm(a, b):
        return a * b // gcd(a, b)
    return reduce(lcm, arr)

# LeetCode 2447: Number of Subarrays with GCD equal to K
def subarrays_with_gcd_k(nums: list[int], k: int) -> int:
    """
    Count subarrays whose GCD equals k.
    
    Key observation: There are at most O(N log max) distinct GCDs
    starting from any fixed position (GCD only decreases as we extend).
    
    Time: O(N log max), Space: O(log max)
    """
    count = 0
    n = len(nums)
    
    for i in range(n):
        g = 0
        for j in range(i, n):
            g = gcd(g, nums[j])
            if g == k:
                count += 1
            elif g < k:
                break
    
    return count
```

---

### Problem 4: Modular Inverse Array (LeetCode 2300 variant)

```python
def compute_inverse_array(n: int, mod: int = MOD) -> list[int]:
    """
    Compute modular inverses for [1, n] in O(N) time.
    Uses recurrence: inv[i] = -(mod // i) * inv[mod % i] % mod
    
    Time: O(N), Space: O(N)
    """
    inv = [0] * (n + 1)
    inv[1] = 1
    for i in range(2, n + 1):
        inv[i] = -(mod // i) * inv[mod % i] % mod
    return inv

# Proof of recurrence:
# Let m = mod, write m = q*i + r where q = m//i, r = m%i
# m ≡ 0 (mod m) → q*i + r ≡ 0 (mod m)
# Multiply both sides by inv[i]*inv[r]:
# q*inv[r] + inv[i] ≡ 0 (mod m)
# inv[i] ≡ -q*inv[r] ≡ -(m//i)*inv[m%i] (mod m)
```

---

### Problem 5: Ugly Number II (Regular Numbers)

```python
def nth_ugly_number(n: int) -> int:
    """
    LeetCode 264. Find nth ugly number (factors of 2, 3, 5 only).
    
    Three-pointer merge of infinite sequences 1×2,2×2,...; 1×3,2×3,...; 1×5,2×5,...
    Time: O(N), Space: O(N)
    """
    ugly = [1] * n
    i2 = i3 = i5 = 0
    
    for i in range(1, n):
        next2 = ugly[i2] * 2
        next3 = ugly[i3] * 3
        next5 = ugly[i5] * 5
        
        ugly[i] = min(next2, next3, next5)
        
        if ugly[i] == next2: i2 += 1
        if ugly[i] == next3: i3 += 1
        if ugly[i] == next5: i5 += 1
    
    return ugly[n - 1]
```

---

### Problem 6: Count Primes (LeetCode 204)

```python
def count_primes(n: int) -> int:
    """
    LeetCode 204. Count primes < n.
    Time: O(N log log N), Space: O(N)
    """
    if n < 2:
        return 0
    
    is_prime = bytearray([1]) * n  # bytearray is faster than list[bool]
    is_prime[0] = is_prime[1] = 0
    
    p = 2
    while p * p < n:
        if is_prime[p]:
            is_prime[p*p::p] = bytearray(len(is_prime[p*p::p]))
        p += 1
    
    return sum(is_prime)
```

---

### Problem 7: Factorial Trailing Zeroes (LeetCode 172)

```python
def trailing_zeroes(n: int) -> int:
    """
    LeetCode 172. Count trailing zeros in n!
    Each zero = one factor of 10 = one factor of 2 × one factor of 5.
    Factors of 2 always exceed factors of 5, so count factors of 5.
    
    Factors of 5 in n! = n//5 + n//25 + n//125 + ...
    Time: O(log n)
    """
    count = 0
    power = 5
    while power <= n:
        count += n // power
        power *= 5
    return count

def factorial_mod(n: int, mod: int) -> int:
    """n! mod p (p prime). O(N)."""
    result = 1
    for i in range(2, n + 1):
        result = result * i % mod
    return result

def wilson_theorem(p: int) -> bool:
    """
    Wilson's theorem: (p-1)! ≡ -1 (mod p) iff p is prime.
    Useful for primality testing small p.
    """
    if p < 2:
        return False
    factorial = 1
    for i in range(2, p):
        factorial = factorial * i % p
    return factorial == p - 1
```

---

### Problem 8: Super Pow (LeetCode 372)

```python
def super_pow(a: int, b: list[int]) -> int:
    """
    LeetCode 372. Compute a^b mod 1337 where b is given as digit array.
    
    Key: a^(b[0]*10^(n-1) + ... + b[n-1]) = a^b[0]^(10^(n-1)) × ... 
    Use: x^(de) = (x^d)^e → reduce digit by digit.
    
    Time: O(N log 1337) = O(N)
    """
    MOD = 1337
    
    def powmod(base, exp, mod):
        result = 1
        base %= mod
        while exp > 0:
            if exp & 1:
                result = result * base % mod
            base = base * base % mod
            exp >>= 1
        return result
    
    result = 1
    a %= MOD
    
    for digit in b:
        result = powmod(result, 10, MOD) * powmod(a, digit, MOD) % MOD
    
    return result
```

---

## 11. Interview Cheat Sheet <a name="cheat-sheet"></a>

### Essential Identities

```
gcd(a, b) × lcm(a, b) = a × b
φ(p) = p - 1 (p prime)
φ(p^k) = p^(k-1) × (p-1)
φ(mn) = φ(m) × φ(n) if gcd(m,n) = 1

Fermat's Little Theorem: a^(p-1) ≡ 1 (mod p), gcd(a,p)=1
Wilson's Theorem: (p-1)! ≡ -1 (mod p) iff p prime

n! trailing zeros = Σ floor(n/5^k) for k=1,2,...
Divisor count(n) = Π(aᵢ + 1) for n = Π pᵢ^aᵢ
```

### Algorithm Complexity Table

| Algorithm | Time | Space |
|-----------|------|-------|
| GCD | O(log min(a,b)) | O(1) |
| Extended GCD | O(log min(a,b)) | O(1) |
| Fast exponentiation | O(log exp) | O(1) |
| Modular inverse (Fermat) | O(log p) | O(1) |
| Sieve of Eratosthenes | O(N log log N) | O(N) |
| Linear sieve | O(N) | O(N) |
| Miller-Rabin | O(k log² N) | O(1) |
| Euler's totient | O(√N) | O(1) |
| CRT | O(k log M) | O(1) |

### MOD Choice

- Standard: `10^9 + 7` (prime)
- Alternative: `10^9 + 9` (prime, use when two distinct mods needed for hashing)
- For 64-bit: `(1 << 61) - 1` (Mersenne prime, perfect for polynomial hashing)

---

*Previous: [Quickselect ←](../12_Sorting_And_Searching/02_Quickselect_And_Order_Statistics.md) | Next: [Bit Manipulation →](02_Bit_Manipulation_Mastery.md)*

# Combinatorics — Advanced Mastery Guide

> **From Catalan numbers to reservoir sampling.** The combinatorics topics that distinguish FAANG engineers — with formal proofs and direct interview application.

---

## Table of Contents
1. [nCr Modulo Prime — Two Approaches](#ncr)
2. [Catalan Numbers — Deep Dive](#catalan)
3. [Pigeonhole & Inclusion-Exclusion](#pigeonhole)
4. [Stars and Bars](#stars-bars)
5. [Expected Value & Probability](#expected)
6. [Reservoir Sampling](#reservoir)
7. [Fisher-Yates Shuffle](#fisher-yates)
8. [Problems 1–8 with Full Solutions](#problems)
9. [Combinatorics Cheat Sheet](#cheat-sheet)

---

## 1. nCr Modulo Prime — Two Approaches <a name="ncr"></a>

### Approach 1: Pascal's Triangle — O(N²) precompute, O(1) query

```python
def build_pascal(max_n: int, mod: int = 10**9 + 7) -> list[list[int]]:
    """
    Pascal's triangle modulo prime.
    C[i][j] = C[i-1][j-1] + C[i-1][j]
    
    Time: O(N²), Space: O(N²)
    Good when: max_n is small (≤ ~1000)
    """
    C = [[0] * (max_n + 1) for _ in range(max_n + 1)]
    C[0][0] = 1
    for i in range(1, max_n + 1):
        C[i][0] = 1
        for j in range(1, i + 1):
            C[i][j] = (C[i-1][j-1] + C[i-1][j]) % mod
    return C

# Space-optimized version (1D array):
def build_pascal_1d(max_n: int, mod: int = 10**9 + 7) -> list[list[int]]:
    row = [0] * (max_n + 1)
    row[0] = 1
    result = [row[:]]
    for i in range(1, max_n + 1):
        for j in range(i, 0, -1):  # right to left to avoid overwriting
            row[j] = (row[j] + row[j-1]) % mod
        result.append(row[:])
    return result
```

### Approach 2: Factorials + Fermat's Little Theorem — O(N) precompute, O(1) query

```python
MOD = 10**9 + 7

class Combinations:
    """
    Precompute factorials and inverse factorials.
    Query nCr in O(1).
    Good when: max_n is large (up to ~10^7), mod is prime.
    """
    
    def __init__(self, max_n: int, mod: int = MOD):
        self.mod = mod
        self.fact = [1] * (max_n + 1)
        for i in range(1, max_n + 1):
            self.fact[i] = self.fact[i-1] * i % mod
        
        self.inv_fact = [1] * (max_n + 1)
        self.inv_fact[max_n] = pow(self.fact[max_n], mod - 2, mod)
        for i in range(max_n - 1, -1, -1):
            self.inv_fact[i] = self.inv_fact[i+1] * (i+1) % mod
    
    def C(self, n: int, r: int) -> int:
        """nCr mod p in O(1)."""
        if r < 0 or r > n:
            return 0
        return self.fact[n] * self.inv_fact[r] % self.mod * self.inv_fact[n-r] % self.mod
    
    def P(self, n: int, r: int) -> int:
        """nPr = n! / (n-r)! mod p in O(1)."""
        if r < 0 or r > n:
            return 0
        return self.fact[n] * self.inv_fact[n-r] % self.mod

# Lucas' Theorem: nCr mod p for PRIME p (even when n, r > p)
def lucas_ncr(n: int, r: int, p: int) -> int:
    """
    Lucas' theorem: nCr ≡ Π C(n_i, r_i) (mod p)
    where n = n_0 + n_1*p + ... and r = r_0 + r_1*p + ... (base-p representations).
    
    Time: O(p + log_p(n)), Space: O(p)
    """
    C = build_pascal(p - 1, p)
    
    result = 1
    while n > 0 or r > 0:
        ni, ri = n % p, r % p
        if ri > ni:
            return 0
        result = result * C[ni][ri] % p
        n //= p
        r //= p
    return result
```

---

## 2. Catalan Numbers — Deep Dive <a name="catalan"></a>

**Definition:** `C(n) = C(2n, n) / (n+1)`

**Recurrence:** `C(n) = Σ C(i) × C(n-1-i)` for i=0 to n-1, with C(0) = 1.

**Closed form:** `C(n) = (2n)! / ((n+1)! × n!)`

### Counting Applications

| Problem | Catalan Number |
|---------|---------------|
| Valid parenthesization of n pairs | C(n) |
| Number of distinct BSTs with n keys | C(n) |
| Triangulation of convex polygon with n+2 sides | C(n) |
| Monotonic lattice paths from (0,0) to (n,n) not crossing diagonal | C(n) |
| Stack-sortable permutations of {1..n} | C(n) |
| Full binary trees with n+1 leaves | C(n) |

```python
def catalan_dp(n: int, mod: int = MOD) -> list[int]:
    """
    Compute Catalan numbers C(0)..C(n) using DP.
    Time: O(N²), Space: O(N)
    """
    C = [0] * (n + 1)
    C[0] = 1
    for i in range(1, n + 1):
        for j in range(i):
            C[i] = (C[i] + C[j] * C[i-1-j]) % mod
    return C

def catalan_formula(n: int, mod: int = MOD) -> int:
    """
    C(n) = C(2n, n) / (n+1) = C(2n, n) × inv(n+1)
    Time: O(N) with factorial precomputation.
    """
    comb = Combinations(2 * n, mod)
    return comb.C(2 * n, n) * pow(n + 1, mod - 2, mod) % mod

# First few: C(0)=1, C(1)=1, C(2)=2, C(3)=5, C(4)=14, C(5)=42, C(6)=132

def num_unique_bsts(n: int) -> int:
    """LeetCode 96. Number of structurally unique BSTs with keys 1..n."""
    return catalan_dp(n)[n]

def generate_parentheses_count(n: int) -> int:
    """Count valid parenthesizations of n pairs."""
    return catalan_dp(n)[n]

def catalan_DP_v2(n: int) -> int:
    """
    Using 2D DP interpretation: number of paths from (0,0) to (n,n)
    that never go above the diagonal.
    
    dp[i][j] = paths from (0,0) to (i,j) not crossing diagonal
    Ballot problem formulation.
    """
    dp = [[0] * (n + 1) for _ in range(n + 1)]
    dp[0][0] = 1
    for i in range(n + 1):
        for j in range(n + 1):
            if i == 0 and j == 0:
                continue
            if j > i:  # above diagonal
                dp[i][j] = 0
                continue
            if i > 0:
                dp[i][j] = (dp[i][j] + dp[i-1][j]) % MOD
            if j > 0:
                dp[i][j] = (dp[i][j] + dp[i][j-1]) % MOD
    return dp[n][n]
```

---

## 3. Pigeonhole & Inclusion-Exclusion <a name="pigeonhole"></a>

### Pigeonhole Principle

**Statement:** If N items placed in K containers and N > K, at least one container has ≥ 2 items.

**Generalized:** If N items in K containers, at least one has ≥ ⌈N/K⌉ items.

```python
# Application: Find duplicate in [1..N] with O(1) space
def find_duplicate_pigeonhole(nums: list[int]) -> int:
    """
    LeetCode 287. Array of N+1 integers in range [1,N].
    By pigeonhole, must have duplicate. Floyd's cycle detection.
    Time: O(N), Space: O(1)
    """
    slow = fast = nums[0]
    while True:
        slow = nums[slow]
        fast = nums[nums[fast]]
        if slow == fast:
            break
    
    slow = nums[0]
    while slow != fast:
        slow = nums[slow]
        fast = nums[fast]
    
    return slow

# Application: Subarray sum divisible by N
def subarray_sum_divisible(nums: list[int], k: int) -> bool:
    """
    LeetCode 523. Does any subarray sum to a multiple of k?
    Pigeonhole: with N+1 prefix sums, if any two have same mod k → subarray exists.
    Time: O(N), Space: O(k)
    """
    prefix_sum = 0
    seen = {0}
    for num in nums:
        prefix_sum = (prefix_sum + num) % k
        if prefix_sum in seen:
            return True
        seen.add(prefix_sum)
    return False
```

### Inclusion-Exclusion Principle

**|A ∪ B ∪ C| = |A| + |B| + |C| - |A∩B| - |A∩C| - |B∩C| + |A∩B∩C|**

General formula: `|∪Aᵢ| = Σ|Aᵢ| - Σ|Aᵢ∩Aⱼ| + Σ|Aᵢ∩Aⱼ∩Aₖ| - ...`

```python
def count_numbers_not_divisible(n: int, primes: list[int]) -> int:
    """
    Count numbers in [1, n] not divisible by any prime in the list.
    Inclusion-exclusion over subsets of primes.
    Time: O(2^|primes|)
    """
    total = 0
    k = len(primes)
    
    for mask in range(1, 1 << k):
        product = 1
        bits = 0
        for i in range(k):
            if mask & (1 << i):
                product *= primes[i]
                bits += 1
        
        if product > n:
            continue
        
        # Inclusion-exclusion sign: +1 for odd |subset|, -1 for even
        if bits % 2 == 1:
            total += n // product
        else:
            total -= n // product
    
    return n - total  # n - count of numbers divisible by at least one prime

def derangements(n: int, mod: int = MOD) -> int:
    """
    Count derangements of {1..n} (permutations with no fixed points).
    D(n) = n! × Σ (-1)^k / k! = (n-1) × (D(n-1) + D(n-2))
    
    By inclusion-exclusion:
    D(n) = n! - Σ C(n,1)(n-1)! + Σ C(n,2)(n-2)! - ...
         = Σ (-1)^k × n!/k!
    """
    if n == 0:
        return 1
    if n == 1:
        return 0
    
    # Recurrence: D(n) = (n-1) * (D(n-1) + D(n-2))
    dp = [0] * (n + 1)
    dp[0] = 1
    dp[1] = 0
    for i in range(2, n + 1):
        dp[i] = (i - 1) * (dp[i-1] + dp[i-2]) % mod
    return dp[n]
```

---

## 4. Stars and Bars <a name="stars-bars"></a>

**Stars and Bars Theorem:**
- Distribute N identical items into K distinct bins (each bin ≥ 0): `C(N+K-1, K-1)` ways
- Distribute N identical items into K distinct bins (each bin ≥ 1): `C(N-1, K-1)` ways

```python
def distribute_identical(n: int, k: int, min_each: int = 0, mod: int = MOD) -> int:
    """
    Number of ways to distribute n identical items into k bins.
    Each bin must have >= min_each items.
    
    Transform: let x_i' = x_i - min_each, then Σx_i' = n - k*min_each
    Answer = C(n - k*min_each + k - 1, k - 1)
    """
    adjusted_n = n - k * min_each
    if adjusted_n < 0:
        return 0
    comb = Combinations(adjusted_n + k - 1, mod)
    return comb.C(adjusted_n + k - 1, k - 1)

# Application: Coin change combinations
def coin_combinations(amount: int, coins: list[int], mod: int = MOD) -> int:
    """
    Count ways to make amount using unlimited coins.
    Time: O(amount × |coins|), Space: O(amount)
    """
    dp = [0] * (amount + 1)
    dp[0] = 1
    for coin in coins:
        for x in range(coin, amount + 1):
            dp[x] = (dp[x] + dp[x - coin]) % mod
    return dp[amount]
```

---

## 5. Expected Value & Probability <a name="expected"></a>

```python
def knight_probability(n: int, k: int, row: int, column: int) -> float:
    """
    LeetCode 688. Knight on N×N board, after k moves, what's probability still on board?
    
    DP: dp[r][c] = probability of being at (r,c) after current step.
    Time: O(k × N²), Space: O(N²)
    """
    moves = [(2,1),(2,-1),(-2,1),(-2,-1),(1,2),(1,-2),(-1,2),(-1,-2)]
    
    dp = [[0.0] * n for _ in range(n)]
    dp[row][column] = 1.0
    
    for _ in range(k):
        new_dp = [[0.0] * n for _ in range(n)]
        for r in range(n):
            for c in range(n):
                if dp[r][c] > 0:
                    for dr, dc in moves:
                        nr, nc = r + dr, c + dc
                        if 0 <= nr < n and 0 <= nc < n:
                            new_dp[nr][nc] += dp[r][c] / 8.0
        dp = new_dp
    
    return sum(sum(row) for row in dp)

def dice_rolls_target(n: int, k: int, target: int, mod: int = MOD) -> int:
    """
    LeetCode 1155. N dice each with k faces [1..k]. Count ways to get sum = target.
    Time: O(n × target × k) = O(n × target) with prefix sums.
    Space: O(target)
    """
    dp = [0] * (target + 1)
    dp[0] = 1
    
    for _ in range(n):
        new_dp = [0] * (target + 1)
        prefix = [0] * (target + 2)
        for j in range(target + 1):
            prefix[j + 1] = (prefix[j] + dp[j]) % mod
        
        for j in range(1, target + 1):
            # Sum dp[j-k..j-1] (clamped to valid range)
            lo = max(0, j - k)
            hi = j - 1
            new_dp[j] = (prefix[hi + 1] - prefix[lo]) % mod
        
        dp = new_dp
    
    return dp[target]
```

---

## 6. Reservoir Sampling <a name="reservoir"></a>

**Problem:** Given a stream of unknown length N, select k items uniformly at random using O(k) space.

**Proof of correctness (k=1):**
- After seeing element i: probability it's selected = 1/i
- After seeing all N elements: P(element i in reservoir) = P(selected at step i) × P(not replaced later)
  = (1/i) × (i/(i+1)) × ((i+1)/(i+2)) × ... × ((N-1)/N)
  = 1/N ✓ (telescoping product)

```python
import random

def reservoir_sample_one(stream) -> object:
    """
    Select one item uniformly at random from stream of unknown length.
    Space: O(1), Time: O(N)
    
    Equivalent to: after seeing i-th element, replace current with probability 1/i.
    """
    result = None
    for i, item in enumerate(stream, 1):
        if random.randint(1, i) == 1:
            result = item
    return result

def reservoir_sample_k(stream, k: int) -> list:
    """
    LeetCode 382 variant. Select k items uniformly.
    Space: O(k), Time: O(N)
    
    Algorithm R (Vitter 1985):
    - Fill reservoir with first k items
    - For i-th item (i > k): replace random element in reservoir with prob k/i
    """
    reservoir = []
    for i, item in enumerate(stream):
        if i < k:
            reservoir.append(item)
        else:
            j = random.randint(0, i)
            if j < k:
                reservoir[j] = item
    return reservoir

class LinkedListRandomNode:
    """LeetCode 382. Random node from linked list of unknown length."""
    
    class ListNode:
        def __init__(self, val=0, next=None):
            self.val = val
            self.next = next
    
    def __init__(self, head: 'ListNode'):
        self.head = head
    
    def getRandom(self) -> int:
        """O(N) time, O(1) space. Reservoir sampling."""
        scope = 1
        chosen = 0
        node = self.head
        while node:
            if random.random() < 1.0 / scope:
                chosen = node.val
            scope += 1
            node = node.next
        return chosen

def random_pick_with_weight(weights: list[int]) -> 'RandomPickWeight':
    """LeetCode 528. Pick index with probability proportional to weight."""
    class RandomPickWeight:
        def __init__(self, w: list[int]):
            from itertools import accumulate
            self.prefix = list(accumulate(w))
            self.total = self.prefix[-1]
        
        def pickIndex(self) -> int:
            """Binary search on prefix sum for O(log N)."""
            target = random.randint(1, self.total)
            lo, hi = 0, len(self.prefix) - 1
            while lo < hi:
                mid = (lo + hi) // 2
                if self.prefix[mid] < target:
                    lo = mid + 1
                else:
                    hi = mid
            return lo
    
    return RandomPickWeight(weights)
```

---

## 7. Fisher-Yates Shuffle <a name="fisher-yates"></a>

```python
def fisher_yates_shuffle(arr: list) -> list:
    """
    LeetCode 384. Perfect uniform random shuffle.
    Time: O(N), Space: O(1) in-place
    
    Proof: Element at original position i ends up at any position j
    with probability 1/N (each permutation equally likely).
    
    After step k: arr[k] is chosen uniformly from remaining N-k elements.
    P(arr[k] = x) for any specific x in remaining = 1/(N-k). ✓
    """
    arr = arr[:]
    n = len(arr)
    for i in range(n - 1, 0, -1):
        j = random.randint(0, i)
        arr[i], arr[j] = arr[j], arr[i]
    return arr

# WRONG shuffle (don't do this — not uniform!):
def wrong_shuffle(arr: list) -> list:
    """
    This produces N^N outcomes, not N! — not uniform!
    For n=3: 3^3=27 outcomes, but only 3!=6 permutations → not uniform.
    """
    n = len(arr)
    arr = arr[:]
    for i in range(n):
        j = random.randint(0, n - 1)  # BUG: should be randint(0, i)
        arr[i], arr[j] = arr[j], arr[i]
    return arr
```

---

## 8. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Unique Paths (LeetCode 62, 63)

```python
def unique_paths(m: int, n: int) -> int:
    """
    LeetCode 62. Grid paths from top-left to bottom-right.
    Combinatorial formula: C(m+n-2, m-1) — choose which m-1 of m+n-2 steps are "down".
    Time: O(min(m,n)), Space: O(1) with formula
    """
    # Must take exactly (m-1) down steps and (n-1) right steps
    # Total steps = m+n-2, choose m-1 for down
    from math import comb
    return comb(m + n - 2, m - 1)

def unique_paths_with_obstacles(grid: list[list[int]]) -> int:
    """LeetCode 63. Time: O(MN), Space: O(N) rolling array."""
    m, n = len(grid), len(grid[0])
    dp = [0] * n
    dp[0] = 1 if grid[0][0] == 0 else 0
    
    for i in range(m):
        for j in range(n):
            if grid[i][j] == 1:
                dp[j] = 0
            elif j > 0:
                dp[j] += dp[j-1]
    
    return dp[n-1]
```

---

### Problem 2: Number of Ways to Decode (LeetCode 91)

```python
def num_decodings(s: str) -> int:
    """
    LeetCode 91. Count ways to decode string of digits.
    Each 1-2 digit number maps to A-Z (1-26).
    Time: O(N), Space: O(1)
    """
    if not s or s[0] == '0':
        return 0
    
    n = len(s)
    prev2, prev1 = 1, 1
    
    for i in range(1, n):
        curr = 0
        one_digit = int(s[i])
        two_digit = int(s[i-1:i+1])
        
        if 1 <= one_digit <= 9:
            curr += prev1
        if 10 <= two_digit <= 26:
            curr += prev2
        
        prev2, prev1 = prev1, curr
    
    return prev1
```

---

### Problem 3: Counting BSTs with N keys (Catalan)

```python
def num_trees(n: int) -> int:
    """
    LeetCode 96. Structurally unique BSTs with keys 1..n.
    = Catalan number C(n)
    
    Proof: choose root k (1..n), left subtree has k-1 nodes (C(k-1) ways),
    right subtree has n-k nodes (C(n-k) ways).
    C(n) = Σ C(k-1) × C(n-k) for k=1..n
    """
    dp = [0] * (n + 1)
    dp[0] = dp[1] = 1
    for i in range(2, n + 1):
        for j in range(i):
            dp[i] += dp[j] * dp[i-1-j]
    return dp[n]
```

---

### Problem 4: Generate All Parentheses (LeetCode 22)

```python
def generate_parenthesis(n: int) -> list[str]:
    """
    LeetCode 22. Generate all valid combinations of n pairs.
    Count = Catalan(n).
    Time: O(Catalan(n) × n), Space: O(n) stack depth
    """
    result = []
    
    def backtrack(s: str, open: int, close: int):
        if len(s) == 2 * n:
            result.append(s)
            return
        if open < n:
            backtrack(s + '(', open + 1, close)
        if close < open:
            backtrack(s + ')', open, close + 1)
    
    backtrack('', 0, 0)
    return result
```

---

### Problem 5: Random Pick with Weight (LeetCode 528)

```python
class Solution:
    """Already shown above in reservoir sampling section."""
    def __init__(self, w: list[int]):
        from itertools import accumulate
        self.prefix = list(accumulate(w))
        self.total = self.prefix[-1]
    
    def pickIndex(self) -> int:
        target = random.randint(1, self.total)
        lo, hi = 0, len(self.prefix) - 1
        while lo < hi:
            mid = (lo + hi) // 2
            if self.prefix[mid] < target:
                lo = mid + 1
            else:
                hi = mid
        return lo
```

---

### Problem 6: Shuffle an Array (LeetCode 384)

```python
class ShuffleArray:
    """LeetCode 384."""
    
    def __init__(self, nums: list[int]):
        self.original = nums[:]
        self.nums = nums[:]
    
    def reset(self) -> list[int]:
        self.nums = self.original[:]
        return self.nums
    
    def shuffle(self) -> list[int]:
        """Fisher-Yates shuffle."""
        n = len(self.nums)
        for i in range(n - 1, 0, -1):
            j = random.randint(0, i)
            self.nums[i], self.nums[j] = self.nums[j], self.nums[i]
        return self.nums
```

---

### Problem 7: Probability in Knight Moves (LeetCode 688)

```python
# Already shown above in expected value section
def knight_prob(n: int, k: int, row: int, col: int) -> float:
    return knight_probability(n, k, row, col)
```

---

### Problem 8: Number of Dice Rolls with Target Sum (LeetCode 1155)

```python
# Already shown above. Here with alternative 2D DP for clarity:
def num_rolls_to_target(n: int, k: int, target: int) -> int:
    """
    LeetCode 1155.
    dp[i][j] = ways to get sum j using i dice.
    dp[i][j] = Σ dp[i-1][j-f] for f in [1,k]
    """
    MOD = 10**9 + 7
    dp = [[0] * (target + 1) for _ in range(n + 1)]
    dp[0][0] = 1
    
    for i in range(1, n + 1):
        for j in range(1, target + 1):
            for face in range(1, min(k, j) + 1):
                dp[i][j] = (dp[i][j] + dp[i-1][j-face]) % MOD
    
    return dp[n][target]
```

---

## 9. Combinatorics Cheat Sheet <a name="cheat-sheet"></a>

### Key Formulas

```
Permutations: P(n,r) = n!/(n-r)!
Combinations: C(n,r) = n!/(r!(n-r)!)
With repetition: C(n+r-1, r) — multiset coefficient
Derangements: D(n) = n! × Σ (-1)^k/k! ≈ n!/e
Catalan: C(n) = C(2n,n)/(n+1) = 1,1,2,5,14,42,132,...
Fibonacci: F(n) = F(n-1) + F(n-2), exponential formula via matrix exp

Binomial theorem: (x+y)^n = Σ C(n,k) x^k y^(n-k)
Pascal: C(n,k) = C(n-1,k-1) + C(n-1,k)
Vandermonde: C(m+n,r) = Σ C(m,k)×C(n,r-k)
```

### When to Use What

```
Problem type → Formula
─────────────────────────────────────────────────────
Select r items from n (order matters, no repeat) → P(n,r)
Select r items from n (order doesn't matter, no repeat) → C(n,r)
Select r items from n (order doesn't matter, with repeat) → C(n+r-1, r)
Arrange n items where k are identical → n! / k!
Arrange n items in circle → (n-1)!
Distribute n identical items into k bins (0 each) → C(n+k-1, k-1)
Distribute n identical items into k bins (≥1 each) → C(n-1, k-1)
Balanced bracket strings / BST count → Catalan number
No fixed points in permutation → Derangement D(n)
```

### Computational Complexity Guide

| Method | Precompute | Query nCr | Constraints |
|--------|------------|-----------|-------------|
| Pascal's triangle | O(N²) | O(1) | N ≤ 1000 |
| Factorial + Fermat | O(N) | O(1) | Mod is prime |
| Lucas' theorem | O(p²) | O(log N) | Any prime mod |
| Python `math.comb` | O(1) | O(N) | Exact arithmetic |

### Interview Insight

> **"How many ways to parenthesize n multiplications?"** — Catalan! C(n) distinct full parenthesizations of n+1 factors.

> **"Why is Fisher-Yates correct but 'pick random position for each element' wrong?"** — The latter produces N^N equally likely outcomes, not N! — so many permutations get weighted more than others.

> **"Expected number of trials to select a satisfying item?"** — If each trial succeeds with probability p, expected trials = 1/p (geometric distribution).

---

*Previous: [Bit Manipulation ←](02_Bit_Manipulation_Mastery.md) | Next: [Matrix Exponentiation →](04_Matrix_Exponentiation.md)*

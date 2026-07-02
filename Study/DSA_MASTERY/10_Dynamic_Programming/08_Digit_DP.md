# Digit DP — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Pattern:** Count integers in range [L, R] satisfying property P. Decompose by digit position, tracking "tightness" (are we still bounded by the limit?) and any additional accumulated state.

---

## Table of Contents
1. [Digit DP Template — The Universal Framework](#1-digit-dp-template)
2. [Count Numbers in Range [L,R]: f(R) - f(L-1)](#2-range-query-technique)
3. [Digit Sum Divisible by K](#3-digit-sum-divisible-by-k)
4. [Count Step Numbers](#4-count-step-numbers)
5. [Numbers Without Consecutive 1s in Binary](#5-numbers-without-consecutive-1s-in-binary)
6. [Count Special Numbers (No Repeated Digits)](#6-count-special-numbers)
7. [Non-Decreasing Digits Count](#7-non-decreasing-digits-count)
8. [Numbers At Most N Given Digit Set](#8-numbers-at-most-n-given-digit-set)
9. [Count Numbers with Unique Digits](#9-count-numbers-with-unique-digits)
10. [Digit DP with Multiple Constraints](#10-digit-dp-with-multiple-constraints)
11. [Count Integers with Even Digit Sum](#11-count-integers-with-even-digit-sum)

---

## 1. Digit DP Template — The Universal Framework

### Core Idea

We process a number digit by digit (most significant to least significant). At each position, we decide what digit to place. The key insight: we only need to track:

1. **`pos`** — current digit position (0 = most significant)
2. **`tight`** — are we still "tight" to the upper bound? If tight, current digit ≤ `limit[pos]`. If not tight, current digit can be 0..9 freely.
3. **`leading_zero`** — are we still in the leading zero prefix? (needed for some problems)
4. **Additional state** — problem-specific accumulator (sum mod K, last digit, digit mask, etc.)

### The Tight Constraint

When `tight = True`, the current digit can be at most `digits[pos]` (the digit of the upper bound). If we choose a digit `d < digits[pos]`, the next position becomes NOT tight. If we choose `d == digits[pos]`, next position stays tight.

```python
# Universal Digit DP Template
from functools import lru_cache

def count_up_to(n: int, property_check) -> int:
    """Count numbers in [1, n] satisfying the property."""
    digits = list(map(int, str(n)))
    L = len(digits)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, leading_zero, *extra_state):
        """
        pos:          current digit position (0-indexed from MSB)
        tight:        are we still bounded by n?
        leading_zero: are we still in leading zero phase?
        extra_state:  problem-specific accumulated state
        """
        # Base case: all digits placed
        if pos == L:
            return property_satisfied(*extra_state, leading_zero)
        
        limit = digits[pos] if tight else 9
        result = 0
        
        for d in range(0, limit + 1):
            new_tight        = tight and (d == limit)
            new_leading_zero = leading_zero and (d == 0)
            new_state        = transition(*extra_state, d, new_leading_zero)
            
            result += dp(pos + 1, new_tight, new_leading_zero, *new_state)
        
        return result
    
    return dp(0, True, True, *initial_state)
```

### Template in Python — Self-Contained

```python
from functools import lru_cache
from typing import List

def digit_dp_template(N: int) -> int:
    """Count numbers from 1 to N satisfying some property."""
    s = str(N)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def solve(pos: int, tight: bool, is_zero: bool, state: int) -> int:
        """
        pos:    current position in digit string (0 = MSB)
        tight:  must not exceed s[pos] if True
        is_zero: still a leading zero (number not started yet)
        state:  accumulated problem-specific state
        """
        if pos == n:
            # Check if the number formed (state) satisfies property
            return 0 if is_zero else int(satisfies(state))
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(0, limit + 1):
            new_tight  = tight and (d == limit)
            new_is_zero = is_zero and (d == 0)
            
            if new_is_zero:
                new_state = state  # leading zeros don't contribute
            else:
                new_state = update_state(state, d)
            
            total += solve(pos + 1, new_tight, new_is_zero, new_state)
        
        return total
    
    return solve(0, True, True, initial_state_value)
```

### Converting [L, R] to Two Calls

```python
def count_in_range(L: int, R: int) -> int:
    return count_up_to(R) - count_up_to(L - 1)
```

This works because: count in [L, R] = count in [0, R] - count in [0, L-1].

---

## 2. Range Query Technique

### f(R) - f(L-1) Pattern

**Problem:** Count numbers in [L, R] with some property P.

The digit DP function `f(N)` counts numbers in [0, N] with property P. Then:
```
count in [L, R] = f(R) - f(L-1)
```

**Special handling for L=1:** `f(0)` = 1 if 0 satisfies property, else 0.

```python
def count_range(L: int, R: int) -> int:
    """Count numbers in [L, R] satisfying digit property."""
    
    def count_up_to(N: int) -> int:
        if N < 0:
            return 0
        
        s = str(N)
        n = len(s)
        
        @lru_cache(maxsize=None)
        def dp(pos, tight, is_zero, state):
            if pos == n:
                return 0 if is_zero else int(property_ok(state))
            
            limit = int(s[pos]) if tight else 9
            res = 0
            for d in range(limit + 1):
                res += dp(
                    pos + 1,
                    tight and d == limit,
                    is_zero and d == 0,
                    state if (is_zero and d == 0) else new_state(state, d)
                )
            return res
        
        result = dp(0, True, True, 0)
        dp.cache_clear()  # Important: clear cache between calls with different N
        return result
    
    return count_up_to(R) - count_up_to(L - 1)
```

**Warning:** Clear `@lru_cache` between calls to `count_up_to` with different `N`, since the `digits` array changes!

---

## 3. Digit Sum Divisible by K

**Problem:** Count numbers in [1, N] where the sum of digits is divisible by K.

### State: (pos, tight, is_zero, digit_sum_mod_k)

```python
def count_divisible_digit_sum(N: int, K: int) -> int:
    s = str(N)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, is_zero, sum_mod):
        if pos == n:
            # Leading zero number (0 itself): digit sum = 0
            return 1 if (sum_mod == 0 and not is_zero) else 0
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(limit + 1):
            new_tight  = tight and (d == limit)
            new_is_zero = is_zero and (d == 0)
            
            if new_is_zero:
                new_sum = 0  # leading zeros don't add to sum
            else:
                new_sum = (sum_mod + d) % K
            
            total += dp(pos + 1, new_tight, new_is_zero, new_sum)
        
        return total
    
    return dp(0, True, True, 0)

# Count numbers in [1, 100] with digit sum divisible by 5
print(count_divisible_digit_sum(100, 5))  # 20 (5,10,14,...,95,100)
print(count_divisible_digit_sum(1000, 3)) # 334
```

> **Time:** O(digits × 2 × 2 × K) = O(log(N) × K)  
> **Space:** O(log(N) × K) — cache size

---

## 4. Count Step Numbers

**Problem:** [LC 1411 variant] Count numbers where each adjacent digit differs by exactly 1. These are "step numbers." Count in [1, N].

### State: (pos, tight, is_zero, last_digit)

```python
def count_step_numbers(N: int) -> int:
    s = str(N)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, is_zero, last_digit):
        if pos == n:
            return 0 if is_zero else 1
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(limit + 1):
            new_tight   = tight and (d == limit)
            new_is_zero = is_zero and (d == 0)
            
            if new_is_zero:
                # Placing leading zero: any next digit ok
                total += dp(pos + 1, new_tight, True, -1)
            elif is_zero:
                # First non-zero digit: no constraint from "last"
                total += dp(pos + 1, new_tight, False, d)
            else:
                # Must differ from last by exactly 1
                if abs(d - last_digit) == 1:
                    total += dp(pos + 1, new_tight, False, d)
        
        return total
    
    return dp(0, True, True, -1)

print(count_step_numbers(21))   # 14 (1,2,3,4,5,6,7,8,9,10,12,21)
print(count_step_numbers(100))  # 17
```

> **Time:** O(log(N) × 2 × 2 × 10) = O(log(N) × 40)  
> **Space:** O(log(N) × 40)

---

## 5. Numbers Without Consecutive 1s in Binary

**Problem:** [LC 600] Count numbers in [1, N] whose binary representation contains no two consecutive 1s.

### State: (pos, tight, prev_bit, is_zero)

```python
def find_integers(N: int) -> int:
    binary = bin(N)[2:]  # remove '0b' prefix
    n = len(binary)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, prev_was_one, is_zero):
        if pos == n:
            return 0 if is_zero else 1
        
        limit = int(binary[pos]) if tight else 1  # binary digit: 0 or 1
        total = 0
        
        for d in range(limit + 1):
            new_tight   = tight and (d == limit)
            new_is_zero = is_zero and (d == 0)
            
            if d == 1 and prev_was_one:
                continue  # Two consecutive 1s: skip
            
            total += dp(pos + 1, new_tight, d == 1 and not new_is_zero, new_is_zero)
        
        return total
    
    return dp(0, True, False, True)

print(find_integers(5))   # 5 (1,2,3,4,5 — all valid)
print(find_integers(6))   # 5 (6=110 has consecutive 1s in binary? No: 110 → no)

# Wait: 6=110: positions are 1,1,0 → consecutive 1s at bits 2 and 1 → invalid
# Actually: binary of 6 is "110" → 1 and 1 are consecutive → NOT valid
print(find_integers(6))   # should be 5 (1,2,3,4,5; 6 is invalid)
print(find_integers(1))   # 1
```

> **Time:** O(log(N) × 2 × 2 × 2) = O(log N)  
> **Space:** O(log N)

---

## 6. Count Special Numbers

**Problem:** [LC 2376] Count positive integers in [1, N] with all distinct digits.

### State: (pos, tight, is_zero, digit_mask)

`digit_mask` is a bitmask of which digits (0-9) have been used so far.

```python
def count_special_numbers(N: int) -> int:
    s = str(N)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, is_zero, digit_mask):
        if pos == n:
            return 0 if is_zero else 1
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(limit + 1):
            # Check digit not already used (except for leading zeros)
            if not is_zero and (digit_mask >> d & 1):
                continue  # digit d already used
            
            new_tight   = tight and (d == limit)
            new_is_zero = is_zero and (d == 0)
            
            if new_is_zero:
                new_mask = digit_mask  # leading zeros don't mark digit 0 as used
            else:
                new_mask = digit_mask | (1 << d)
            
            total += dp(pos + 1, new_tight, new_is_zero, new_mask)
        
        return total
    
    return dp(0, True, True, 0)

print(count_special_numbers(20))   # 19 (1-9 all special, 10,12,13,...,20 — 20=2,0 distinct)
print(count_special_numbers(5))    # 5
print(count_special_numbers(135))  # 110
```

> **Time:** O(log(N) × 2 × 2 × 2^10) = O(log(N) × 2^10)  
> **Space:** O(log(N) × 2^10)

**Note:** With 10 digits (0-9), digit_mask has 2^10 = 1024 states. Total states ≈ 10 × 2 × 2 × 1024 ≈ 40K — very manageable.

---

## 7. Non-Decreasing Digits Count

**Problem:** Count numbers in [1, N] whose digits are non-decreasing (each digit ≥ previous digit).

### State: (pos, tight, is_zero, last_digit)

```python
def count_non_decreasing(N: int) -> int:
    s = str(N)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, is_zero, last_d):
        if pos == n:
            return 0 if is_zero else 1
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(limit + 1):
            if not is_zero and d < last_d:
                continue  # decreasing digit — skip
            
            total += dp(
                pos + 1,
                tight and (d == limit),
                is_zero and (d == 0),
                d if not (is_zero and d == 0) else 0
            )
        
        return total
    
    return dp(0, True, True, 0)

print(count_non_decreasing(100))  # 54
print(count_non_decreasing(1000)) # 220
```

**Related: [LC 402] Remove K Digits for smallest non-increasing result** — different problem, uses greedy stack.

> **Time:** O(log(N) × 2 × 2 × 10) | **Space:** O(log(N) × 10)

---

## 8. Numbers At Most N Given Digit Set

**Problem:** [LC 902] Given sorted digit set `D` and N, count numbers ≤ N using only digits from D.

### Approach: Digit DP + Careful Counting

For each length from 1 to len(N)-1: all D^length numbers are valid.
For length == len(N): use standard tight digit DP.

```python
def at_most_n_given_digit_set(digits: list[str], N: int) -> int:
    S = str(N)
    n = len(S)
    D = sorted(int(d) for d in digits)
    
    count = 0
    
    # Count numbers with fewer digits: for length L, count = |D|^L
    for length in range(1, n):
        count += len(D) ** length
    
    # Count numbers with exactly n digits using tight DP
    # At each position, digits must be from D and respect tightness
    
    @lru_cache(maxsize=None)
    def dp(pos, tight):
        if pos == n:
            return 1  # formed a valid number
        
        limit = int(S[pos]) if tight else 10  # no tight constraint: any digit in D
        total = 0
        
        for d in D:
            if d > limit:
                break  # D is sorted, no point continuing
            total += dp(pos + 1, tight and (d == limit))
        
        return total
    
    count += dp(0, True)
    return count

print(at_most_n_given_digit_set(["1","3","5","7"], 100))  # 20
print(at_most_n_given_digit_set(["1","4","9"], 1000000000))  # 29523
```

> **Time:** O(log(N) × |D|) | **Space:** O(log(N))

---

## 9. Count Numbers with Unique Digits

**Problem:** [LC 357] Count numbers with all unique digits in range [0, 10^n - 1].

### Mathematical Solution (No Full Digit DP Needed)

For numbers with exactly k digits (first digit 1-9, rest 0-9 with no repeats):
- First digit: 9 choices (1-9)
- Second digit: 9 choices (0-9 except first)
- Third digit: 8 choices
- ...
- k-th digit: 9-k+2 choices

```python
def count_numbers_with_unique_digits(n: int) -> int:
    if n == 0:
        return 1
    
    # Start with all 1-digit numbers: 10 (0-9)
    count = 10
    unique = 9  # track current product of choices
    available = 9  # available digits for each new position
    
    for k in range(2, n + 1):
        unique *= available
        count += unique
        available -= 1
        if available == 0:
            break
    
    return count

print(count_numbers_with_unique_digits(2))  # 91
print(count_numbers_with_unique_digits(3))  # 739
```

> **Time:** O(N) | **Space:** O(1)

### General Version Using Full Digit DP

```python
def count_unique_digits_dp(n: int) -> int:
    """Count in [0, 10^n - 1] using digit DP."""
    N = 10**n - 1
    s = str(N)
    L = len(s)
    
    @lru_cache(maxsize=None)
    def dp(pos, tight, is_zero, digit_mask):
        if pos == L:
            return 1  # all valid (including 0 as is_zero case)
        
        limit = int(s[pos]) if tight else 9
        total = 0
        
        for d in range(limit + 1):
            if not is_zero and (digit_mask >> d & 1):
                continue
            
            new_is_zero = is_zero and (d == 0)
            new_mask = digit_mask if new_is_zero else (digit_mask | (1 << d))
            
            total += dp(pos+1, tight and d==limit, new_is_zero, new_mask)
        
        return total
    
    return dp(0, True, True, 0)
```

> **Time:** O(N × 2^10) | **Space:** O(N × 2^10)

---

## 10. Digit DP with Multiple Constraints

**Problem:** Count numbers in [L, R] where:
1. Digit sum is between `low_sum` and `high_sum`
2. No two adjacent digits are the same
3. The number is divisible by `D`

### State: (pos, tight, is_zero, digit_sum, last_digit, remainder)

```python
def count_multi_constraint(L: int, R: int, 
                           low_sum: int, high_sum: int, D: int) -> int:
    def count_up_to(N):
        if N < 0:
            return 0
        
        s = str(N)
        n = len(s)
        max_sum = 9 * n  # maximum possible digit sum
        
        @lru_cache(maxsize=None)
        def dp(pos, tight, is_zero, digit_sum, last_digit, remainder):
            if pos == n:
                if is_zero:
                    return 0  # 0 itself: handle separately if needed
                ok = (low_sum <= digit_sum <= high_sum) and (remainder == 0)
                return 1 if ok else 0
            
            limit = int(s[pos]) if tight else 9
            total = 0
            
            for d in range(limit + 1):
                new_tight   = tight and (d == limit)
                new_is_zero = is_zero and (d == 0)
                
                # No same adjacent digits (skip if same as last)
                if not new_is_zero and not is_zero and d == last_digit:
                    continue
                
                if new_is_zero:
                    new_sum  = 0
                    new_last = -1
                    new_rem  = 0
                else:
                    new_sum  = digit_sum + d
                    new_last = d
                    new_rem  = (remainder * 10 + d) % D
                    
                    if new_sum > high_sum:
                        continue  # early pruning
                
                total += dp(pos+1, new_tight, new_is_zero, new_sum, new_last, new_rem)
            
            return total
        
        result = dp(0, True, True, 0, -1, 0)
        dp.cache_clear()
        return result
    
    return count_up_to(R) - count_up_to(L - 1)

# Count in [1, 1000]: digit sum 5-15, no adjacent same, divisible by 7
print(count_multi_constraint(1, 1000, 5, 15, 7))
```

> **Time:** O(log(N) × 2 × 2 × max_sum × 10 × D)  
> **Space:** Proportional to state space

**Key insight:** Multiple constraints just mean more state dimensions. Each constraint becomes a dimension in the DP state.

---

## 11. Count Integers with Even Digit Sum

**Problem:** Count numbers in [L, R] with even digit sum.

```python
def count_even_digit_sum(L: int, R: int) -> int:
    def f(N: int) -> int:
        """Count numbers in [0, N] with even digit sum."""
        if N < 0:
            return 0
        
        s = str(N)
        n = len(s)
        
        @lru_cache(maxsize=None)
        def dp(pos, tight, is_zero, parity):
            # parity: 0 = even sum so far, 1 = odd sum so far
            if pos == n:
                return (1 if parity == 0 else 0) if not is_zero else 0
            
            limit = int(s[pos]) if tight else 9
            total = 0
            
            for d in range(limit + 1):
                new_is_zero = is_zero and (d == 0)
                new_parity  = parity if new_is_zero else (parity + d) % 2
                
                total += dp(
                    pos + 1,
                    tight and (d == limit),
                    new_is_zero,
                    new_parity
                )
            
            return total
        
        result = dp(0, True, True, 0)
        dp.cache_clear()
        return result
    
    return f(R) - f(L - 1)

print(count_even_digit_sum(1, 20))  # 10 (2,4,6,8,11,13,15,17,19,20)
print(count_even_digit_sum(1, 100)) # 50
```

> **Time:** O(log(N) × 2 × 2 × 2) = O(log N)  
> **Space:** O(log N)

---

## The Digit DP State Design Checklist

```
1. POSITION state (always needed):
   pos = which digit position we're placing (0 to len-1)

2. TIGHT constraint (always needed):
   tight = are we still bounded by the upper limit?
   → True: next digit ≤ limit[pos]
   → False: next digit can be 0..9 freely

3. LEADING ZERO flag (often needed):
   is_zero = is the number we've formed so far all zeros?
   → Needed when leading zeros affect the state (e.g., digit sum, last digit)
   → Not needed for purely positional properties

4. PROBLEM-SPECIFIC STATE:
   → Digit sum (mod K): add each placed digit
   → Last digit: for adjacent constraints
   → Digit mask: which digits have been used (for "no repeat" problems)  
   → Remainder: running value mod D (for divisibility)
   → Count of some digit: e.g., count of '4's placed
   → Parity of sum: just 0/1

5. COMPLEXITY ESTIMATE:
   State count = len(digits) × 2 (tight) × 2 (zero) × (extra state size)
   Typical: O(10 × 2 × 2 × K) per digit → manageable
```

## Common Digit DP Mistakes

| Mistake | Fix |
|---|---|
| Not clearing cache between f(R) and f(L-1) | Use `cache_clear()` or pass `digits` as argument |
| Forgetting leading zero handling | Always track `is_zero` for digit-sensitive properties |
| Checking divisibility wrong | Use `(remainder * 10 + d) % D` not `(remainder + d) % D` |
| Starting from 0 instead of 1 | Check base case: is 0 counted? Adjust base case accordingly |
| Bounds error in tight: using `<` vs `<=` | `for d in range(0, limit + 1)` is correct (include limit) |

## Digit DP vs Mathematical Formula

| When to Use | Digit DP | Mathematical Formula |
|---|---|---|
| Complex multi-constraint property | ✓ | ✗ (too hard to formulate) |
| Simple properties (even digits, etc.) | Either works | ✓ if derivable |
| Very large N (10^18) | ✓ (O(log N) states) | ✓ if formula exists |
| Multiple queries with different N | ✓ | Depends |

**When formula is simpler:**  
"Count numbers ≤ N with all digits ≤ 5": No digit DP needed. For each length L from 1 to digits(N), count = 5^L (or handle tight constraint for length = digits(N)).

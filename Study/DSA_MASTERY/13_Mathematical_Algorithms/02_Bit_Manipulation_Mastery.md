# Bit Manipulation Mastery — Advanced Guide

> **Bit tricks separate good engineers from great ones.** Every technique here has shown up in FAANG interviews. Master the XOR properties, bit DP, and Trie-based maximum XOR.

---

## Table of Contents
1. [Bit Operations Reference](#operations)
2. [XOR Properties — The Core Theorems](#xor)
3. [Brian Kernighan's Algorithm & Bit Counting](#counting)
4. [Single Number Variants I, II, III](#single-number)
5. [Bit DP — Counting & Enumeration](#bit-dp)
6. [Maximum XOR — Trie Approach](#max-xor)
7. [Problems 1–10 with Full Solutions](#problems)
8. [Interview Reference Card](#reference)

---

## 1. Bit Operations Reference <a name="operations"></a>

```python
# Fundamental operations on integer x, bit position k (0-indexed from right)

# READ / TEST a bit
def get_bit(x: int, k: int) -> int:
    return (x >> k) & 1

def is_bit_set(x: int, k: int) -> bool:
    return bool((x >> k) & 1)

# SET a bit to 1
def set_bit(x: int, k: int) -> int:
    return x | (1 << k)

# CLEAR a bit to 0
def clear_bit(x: int, k: int) -> int:
    return x & ~(1 << k)

# TOGGLE a bit
def toggle_bit(x: int, k: int) -> int:
    return x ^ (1 << k)

# Count set bits (popcount)
def popcount(x: int) -> int:
    return bin(x).count('1')
# Python built-in: bin(x).count('1') or x.bit_count() (Python 3.10+)

# Most significant bit position (floor(log2(x)) for x > 0)
def msb(x: int) -> int:
    return x.bit_length() - 1

# Least significant set bit
def lsb(x: int) -> int:
    return x & (-x)

# Turn off lowest set bit
def clear_lowest_set_bit(x: int) -> int:
    return x & (x - 1)  # Brian Kernighan's trick

# Isolate lowest set bit
def isolate_lsb(x: int) -> int:
    return x & (-x)

# Check power of 2
def is_power_of_two(x: int) -> bool:
    return x > 0 and (x & (x - 1)) == 0

# Next power of 2 >= x
def next_power_of_two(x: int) -> int:
    if x <= 1:
        return 1
    x -= 1
    x |= x >> 1
    x |= x >> 2
    x |= x >> 4
    x |= x >> 8
    x |= x >> 16
    return x + 1

# Enumerate all subsets of a mask
def enumerate_subsets(mask: int):
    sub = mask
    while sub > 0:
        yield sub
        sub = (sub - 1) & mask
    yield 0
```

### Essential Identities

```
x & 0  = 0         x & 1...1 = x
x | 0  = x         x | 1...1 = 1...1
x ^ 0  = x         x ^ 1...1 = ~x
x ^ x  = 0         x & (x-1) = x with lowest set bit cleared
x & (-x) = lowest set bit of x
x | (x-1) = x with all trailing zeros set
-(x) = ~x + 1      (two's complement)

Shift: x >> k = x // 2^k (arithmetic for signed, logical for unsigned)
       x << k = x × 2^k

Swap: a ^= b; b ^= a; a ^= b  (XOR swap, no temp variable)
```

---

## 2. XOR Properties — The Core Theorems <a name="xor"></a>

**XOR is the algebraic structure of GF(2)^n — the vector space over the field {0,1}.**

```
COMMUTATIVITY:    a ^ b = b ^ a
ASSOCIATIVITY:    a ^ (b ^ c) = (a ^ b) ^ c
IDENTITY:         a ^ 0 = a
SELF-INVERSE:     a ^ a = 0
CANCELLATION:     a ^ b ^ b = a  (XOR with same value twice cancels)

Key consequences:
- XOR of a list = XOR of distinct elements (duplicates cancel)
- XOR of 1..n has a pattern: period 4 based on n % 4
- XOR is linear: popcount(a ^ b) = Hamming distance(a, b)
```

```python
def xor_1_to_n(n: int) -> int:
    """
    XOR of all integers from 1 to n. O(1).
    Pattern cycles every 4 numbers:
    n%4==0: n
    n%4==1: 1
    n%4==2: n+1
    n%4==3: 0
    """
    patterns = [n, 1, n + 1, 0]
    return patterns[n % 4]

def xor_range(l: int, r: int) -> int:
    """XOR of all integers from l to r. O(1)."""
    return xor_1_to_n(r) ^ xor_1_to_n(l - 1)
```

---

## 3. Brian Kernighan's Algorithm & Bit Counting <a name="counting"></a>

```python
def count_set_bits_kernighan(n: int) -> int:
    """
    Brian Kernighan's algorithm.
    Each iteration removes the lowest set bit.
    Time: O(number of set bits)
    """
    count = 0
    while n:
        n &= n - 1  # clear lowest set bit
        count += 1
    return count

def count_bits_lookup(n: int) -> int:
    """
    Lookup table approach. Precompute for 16-bit chunks.
    Time: O(1) per query after O(2^16) preprocessing.
    """
    # Precompute for 8-bit (256 entries)
    table = [0] * 256
    for i in range(256):
        table[i] = (i & 1) + table[i >> 1]
    
    count = 0
    while n:
        count += table[n & 0xFF]
        n >>= 8
    return count

def count_bits_dp(n: int) -> list[int]:
    """
    LeetCode 338. Count bits for all numbers 0 to n.
    Time: O(N), Space: O(N)
    
    dp[i] = dp[i >> 1] + (i & 1)
    = popcount of i//2 + lowest bit of i
    """
    dp = [0] * (n + 1)
    for i in range(1, n + 1):
        dp[i] = dp[i >> 1] + (i & 1)
    return dp

def count_bits_dp_v2(n: int) -> list[int]:
    """
    Alternative: dp[i] = dp[i & (i-1)] + 1
    (popcount(i) = popcount(i with lsb cleared) + 1)
    """
    dp = [0] * (n + 1)
    for i in range(1, n + 1):
        dp[i] = dp[i & (i - 1)] + 1
    return dp
```

---

## 4. Single Number Variants <a name="single-number"></a>

### Variant I: One element appears once, rest appear twice

```python
def single_number_i(nums: list[int]) -> int:
    """
    LeetCode 136. XOR all elements — pairs cancel.
    Time: O(N), Space: O(1)
    """
    result = 0
    for num in nums:
        result ^= num
    return result
```

### Variant II: One element appears once, rest appear three times

```python
def single_number_ii(nums: list[int]) -> int:
    """
    LeetCode 137. 
    
    Approach: Count bit frequency mod 3.
    For each bit position, sum of bits across all numbers ≡ bit of answer (mod 3).
    
    Or: two-bit counter with 'ones' and 'twos'.
    Time: O(N), Space: O(1)
    """
    # Method 1: Sum mod 3 for each bit
    result = 0
    for bit in range(32):
        total = sum((num >> bit) & 1 for num in nums)
        if total % 3 == 1:
            result |= (1 << bit)
    
    # Handle negative (32-bit signed integer representation)
    if result >= (1 << 31):
        result -= (1 << 32)
    return result

def single_number_ii_bit_counter(nums: list[int]) -> int:
    """
    Elegant two-variable solution.
    'ones' = bits that have appeared once (mod 3)
    'twos' = bits that have appeared twice (mod 3)
    When a bit appears for the 3rd time, it's cleared from both.
    """
    ones = twos = 0
    for num in nums:
        ones = (ones ^ num) & ~twos
        twos = (twos ^ num) & ~ones
    return ones

# State machine per bit:
# 00 → 01 → 10 → 00 (cycling through 0,1,2 appearances)
# ones bit | twos bit → after XOR with num:
# 0 0 → 0: 0 1 → 1: 1 0 → 2: 0 0 → 0 (reset)
```

### Variant III: Two elements appear once, rest appear twice

```python
def single_number_iii(nums: list[int]) -> list[int]:
    """
    LeetCode 260. Two distinct single elements.
    
    Step 1: XOR all → result = a ^ b (where a, b are the singles)
    Step 2: Any set bit in (a^b) differs between a and b
    Step 3: Use that bit to partition nums into two groups
    Step 4: XOR each group independently → gives a and b
    
    Time: O(N), Space: O(1)
    """
    xor_all = 0
    for num in nums:
        xor_all ^= num
    
    # Find any differing bit (lowest set bit is cleanest)
    diff_bit = xor_all & (-xor_all)
    
    a = 0
    for num in nums:
        if num & diff_bit:  # partition: nums with this bit set
            a ^= num
    
    b = xor_all ^ a
    return [a, b]
```

---

## 5. Bit DP — Counting & Enumeration <a name="bit-dp"></a>

```python
def count_set_bits_1_to_n(n: int) -> int:
    """
    Count total set bits in all numbers from 1 to n.
    Time: O(log N)
    
    For each bit position k, count how many numbers in [1,n] have bit k set.
    In the range [0, 2^(k+1) - 1], exactly 2^k numbers have bit k set.
    """
    count = 0
    bit = 1
    while bit <= n:
        full_cycles = n // (2 * bit)
        remainder = n % (2 * bit)
        count += full_cycles * bit + max(0, remainder - bit + 1)
        bit <<= 1
    return count

def total_hamming_distance(nums: list[int]) -> int:
    """
    LeetCode 477. Sum of Hamming distances between all pairs.
    
    For each bit position: if k numbers have bit=1 and (N-k) have bit=0,
    contribution = k × (N-k) pairs.
    
    Time: O(32N) = O(N), Space: O(1)
    """
    n = len(nums)
    total = 0
    for bit in range(32):
        ones = sum(1 for x in nums if (x >> bit) & 1)
        total += ones * (n - ones)
    return total

def subsets_with_xor_target(nums: list[int], target: int) -> int:
    """
    Count subsets of nums with XOR equal to target.
    Gaussian elimination approach: O(N × 32)
    """
    # DP approach: O(N × 2^max_xor) — only feasible for small values
    dp = {0: 1}
    for num in nums:
        new_dp = {}
        for xor_val, cnt in dp.items():
            new_xor = xor_val ^ num
            new_dp[new_xor] = new_dp.get(new_xor, 0) + cnt
            new_dp[xor_val] = new_dp.get(xor_val, 0) + cnt
        dp = new_dp
    return dp.get(target, 0)
```

---

## 6. Maximum XOR — Trie Approach <a name="max-xor"></a>

```python
class BitTrie:
    """
    Binary Trie for maximum XOR queries.
    Each node has at most 2 children (bit 0 and bit 1).
    
    Build: O(N × 32), Query: O(32)
    """
    
    def __init__(self):
        self.children = [None, None]
    
    def insert(self, num: int, bits: int = 31):
        """Insert num into trie, MSB first."""
        node = self
        for i in range(bits, -1, -1):
            bit = (num >> i) & 1
            if not node.children[bit]:
                node.children[bit] = BitTrie()
            node = node.children[bit]
    
    def max_xor_with(self, num: int, bits: int = 31) -> int:
        """
        Find element in trie that maximizes XOR with num.
        Greedy: at each level, prefer the opposite bit.
        """
        node = self
        result = 0
        for i in range(bits, -1, -1):
            bit = (num >> i) & 1
            want = 1 - bit  # prefer opposite bit for XOR
            if node.children[want]:
                result |= (1 << i)
                node = node.children[want]
            elif node.children[bit]:
                node = node.children[bit]
            else:
                break
        return result


def find_maximum_xor(nums: list[int]) -> int:
    """
    LeetCode 421. Maximum XOR of any two elements.
    Time: O(N × 32) = O(N), Space: O(N × 32)
    """
    trie = BitTrie()
    for num in nums:
        trie.insert(num)
    
    return max(trie.max_xor_with(num) for num in nums)


def find_maximum_xor_prefix(nums: list[int]) -> int:
    """
    Alternative: O(N × 32) using bit-by-bit greedy.
    At each bit level, check if we can achieve 1 by XOR.
    """
    max_xor = 0
    mask = 0
    
    for i in range(31, -1, -1):
        mask |= (1 << i)
        prefixes = {num & mask for num in nums}
        
        candidate = max_xor | (1 << i)
        for prefix in prefixes:
            if (candidate ^ prefix) in prefixes:
                max_xor = candidate
                break
    
    return max_xor
```

---

## 7. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Number of 1 Bits (LeetCode 191)

```python
def hamming_weight(n: int) -> int:
    """LeetCode 191. O(number of set bits)."""
    count = 0
    while n:
        n &= n - 1  # clear lowest set bit
        count += 1
    return count
```

---

### Problem 2: Reverse Bits (LeetCode 190)

```python
def reverse_bits(n: int) -> int:
    """
    LeetCode 190. Reverse 32-bit unsigned integer.
    Time: O(32) = O(1)
    """
    result = 0
    for _ in range(32):
        result = (result << 1) | (n & 1)
        n >>= 1
    return result

def reverse_bits_fast(n: int) -> int:
    """
    Bit manipulation without loop (divide and conquer on bits).
    Works on 32-bit integers.
    """
    n = ((n & 0xFFFF0000) >> 16) | ((n & 0x0000FFFF) << 16)
    n = ((n & 0xFF00FF00) >> 8)  | ((n & 0x00FF00FF) << 8)
    n = ((n & 0xF0F0F0F0) >> 4)  | ((n & 0x0F0F0F0F) << 4)
    n = ((n & 0xCCCCCCCC) >> 2)  | ((n & 0x33333333) << 2)
    n = ((n & 0xAAAAAAAA) >> 1)  | ((n & 0x55555555) << 1)
    return n
```

---

### Problem 3: Power of Two / Four (LeetCode 231, 342)

```python
def is_power_of_two(n: int) -> bool:
    """LeetCode 231. O(1)."""
    return n > 0 and (n & (n - 1)) == 0

def is_power_of_four(n: int) -> bool:
    """
    LeetCode 342. Power of 4 must be power of 2 with set bit at even position.
    0x55555555 = 01010101...01 (bits at even positions)
    O(1).
    """
    return n > 0 and (n & (n - 1)) == 0 and (n & 0x55555555) != 0

# Alternative for power of 4: n > 0 and (n & (n-1)) == 0 and log4(n) is integer
def is_power_of_four_v2(n: int) -> bool:
    return n > 0 and (n & (n - 1)) == 0 and (n - 1) % 3 == 0
    # Powers of 4: 1,4,16,64... → 4^k - 1 is divisible by 3 (since 4≡1 mod3 → 4^k≡1)
```

---

### Problem 4: Bitwise AND of Numbers Range (LeetCode 201)

```python
def range_bitwise_and(left: int, right: int) -> int:
    """
    LeetCode 201. AND of all numbers in [left, right].
    
    Key insight: the result is the common prefix of left and right in binary.
    Any differing bits will eventually be both 0 and 1 in the range → AND = 0.
    
    Method: shift both right until equal (count shifts), then shift back.
    Time: O(log max), Space: O(1)
    """
    shift = 0
    while left != right:
        left >>= 1
        right >>= 1
        shift += 1
    return left << shift

def range_bitwise_and_v2(left: int, right: int) -> int:
    """Alternative: clear lowest set bit of right while right > left."""
    while right > left:
        right &= right - 1
    return right
```

---

### Problem 5: Missing Number (LeetCode 268)

```python
def missing_number(nums: list[int]) -> int:
    """
    LeetCode 268. Array has [0..n] with one missing.
    XOR approach: XOR expected (0..n) with actual — pair cancels, missing remains.
    Time: O(N), Space: O(1)
    """
    n = len(nums)
    result = n  # start with n (expected but will cancel if present)
    for i, num in enumerate(nums):
        result ^= i ^ num
    return result

def missing_number_math(nums: list[int]) -> int:
    """Gauss formula: expected sum - actual sum."""
    n = len(nums)
    return n * (n + 1) // 2 - sum(nums)
```

---

### Problem 6: Find Two Numbers Appearing Once (Two Singles)

```python
def find_two_singles(nums: list[int]) -> list[int]:
    """Full solution with explanation. Already covered in single_number_iii above."""
    return single_number_iii(nums)
```

---

### Problem 7: Subset Enumeration — Count Subsets with Property

```python
def count_beautiful_subsets(nums: list[int], k: int) -> int:
    """
    LeetCode 2597. Subsets where no two elements differ by k.
    Backtracking, but demonstrating bitmask enumeration.
    Time: O(2^N × N)
    """
    n = len(nums)
    nums.sort()
    count = 0
    
    for mask in range(1, 1 << n):
        subset = []
        valid = True
        for i in range(n):
            if mask & (1 << i):
                if subset and subset[-1] + k == nums[i]:
                    valid = False
                    break
                subset.append(nums[i])
        if valid:
            count += 1
    
    return count

def enumerate_all_subsets(nums: list[int]) -> list[list[int]]:
    """Generate all 2^N subsets using bitmask. O(N × 2^N)."""
    n = len(nums)
    result = []
    for mask in range(1 << n):
        subset = [nums[i] for i in range(n) if mask & (1 << i)]
        result.append(subset)
    return result
```

---

### Problem 8: Maximum XOR of Two Numbers (LeetCode 421)

```python
def find_max_xor(nums: list[int]) -> int:
    """LeetCode 421. Trie solution. Already shown above."""
    trie = BitTrie()
    for num in nums:
        trie.insert(num)
    return max(trie.max_xor_with(num) for num in nums)
```

---

### Problem 9: Sum of XOR of All Pairs (Bit Independence)

```python
def xor_all_pairs_sum(nums: list[int]) -> int:
    """
    Compute sum of XOR of all pairs (i,j) where i < j.
    
    Key: for each bit position, count pairs where bits differ.
    If k numbers have bit=1, pairs with different bits = k × (N-k).
    Contribution to sum = k × (N-k) × 2^bit.
    
    Time: O(32N), Space: O(1)
    """
    n = len(nums)
    total = 0
    
    for bit in range(32):
        ones = sum(1 for x in nums if (x >> bit) & 1)
        zeros = n - ones
        total += ones * zeros * (1 << bit)
    
    return total
```

---

### Problem 10: Swap Numbers Without Temp

```python
def swap_xor(a: int, b: int) -> tuple[int, int]:
    """
    XOR swap: works for distinct values.
    WARNING: fails if a is b (same memory location — both become 0).
    """
    if a != b:  # guard for same-variable case
        a ^= b
        b ^= a
        a ^= b
    return a, b

# Proof:
# After a ^= b: a = A^B, b = B
# After b ^= a: a = A^B, b = B^(A^B) = A
# After a ^= b: a = (A^B)^A = B, b = A ✓
```

---

## 8. Interview Reference Card <a name="reference"></a>

### Bit Trick Cheat Sheet

| Operation | Code | Notes |
|-----------|------|-------|
| Get bit k | `(x >> k) & 1` | |
| Set bit k | `x \| (1 << k)` | |
| Clear bit k | `x & ~(1 << k)` | |
| Toggle bit k | `x ^ (1 << k)` | |
| Clear lowest set bit | `x & (x-1)` | Brian Kernighan |
| Isolate lowest set bit | `x & (-x)` | |
| Count set bits | `bin(x).count('1')` | Or Kernighan loop |
| Power of 2 check | `x > 0 and not (x & x-1)` | |
| XOR 1..n | Pattern based on `n%4` | O(1) |
| Next power of 2 | Bit spreading trick | |

### Problem Recognition Patterns

```
"Find missing element" → XOR with expected range
"One element appears odd times" → XOR all
"Maximum XOR pair" → Trie + greedy at each bit
"Sum of XOR of all pairs" → Per-bit contribution
"Count subsets with property" → Bitmask enumeration
"Three appearance variant" → Two-counter state machine
"Two distinct singles" → XOR + partition by differing bit
"Counting set bits 0..N" → O(log N) formula
```

### XOR Magic Tricks for Interviews

```python
# 1. Detect if integer has alternating bits (1010.. or 0101..)
def has_alternating_bits(n: int) -> bool:
    m = n ^ (n >> 1)
    return (m & (m + 1)) == 0

# 2. Find position of rightmost 1 bit
def rightmost_set_bit(n: int) -> int:
    return (n & -n).bit_length() - 1

# 3. Check if k-th bit is set (0-indexed from right)
def kth_bit_set(n: int, k: int) -> bool:
    return bool(n & (1 << k))

# 4. Turn off rightmost consecutive 1s
def turn_off_consecutive_ones(n: int) -> int:
    return ((n | (n - 1)) + 1) & n  # complex but useful

# 5. Propagate rightmost 1 bit downward  
def propagate_rightmost_one(n: int) -> int:
    return n | (n - 1)

# 6. Modulo without division (power of 2 modulus)
def fast_mod_pow2(x: int, m: int) -> int:
    """x mod m where m is power of 2."""
    assert m & (m-1) == 0
    return x & (m - 1)
```

### Advanced: Gosper's Hack — Enumerate All K-Bit Masks

```python
def gospers_hack(k: int, n: int):
    """
    Enumerate all n-bit numbers with exactly k bits set.
    Used in bitmask DP to iterate over same-size subsets.
    
    Time per step: O(1), Total: O(C(n,k))
    """
    mask = (1 << k) - 1  # smallest k-bit mask
    while mask < (1 << n):
        yield mask
        c = mask & -mask           # lowest set bit
        r = mask + c               # turn off trailing ones, set next bit
        mask = (((r ^ mask) >> 2) // c) | r  # Gosper's formula

# Example: all 2-bit masks in 4-bit space
# 0011, 0101, 0110, 1001, 1010, 1100
for m in gospers_hack(2, 4):
    print(bin(m))
```

---

*Previous: [Number Theory ←](01_Number_Theory.md) | Next: [Combinatorics →](03_Combinatorics.md)*

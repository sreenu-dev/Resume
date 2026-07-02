# Space-Time Tradeoffs — Mastery Guide

## Core Concept & Invariant

The fundamental tradeoff: **every algorithm lives on a curve in (time, space) space**.
Moving along the curve means paying more of one resource to gain the other.

**Invariant**: For any algorithm processing n bits of input,  
`Time × Space ≥ Ω(n)` in many models (information must be "touched").

**The three axes of tradeoff**:
1. **Precomputation vs On-demand**: Pay space upfront to avoid repeated time cost
2. **Compression vs Decompression**: Save space but pay CPU time to access
3. **Exact vs Approximate**: Trade correctness for massive space reduction (Bloom filters, Count-Min Sketch)

**Key question for every algorithm**: "What is the minimum space needed to achieve this time complexity, and vice versa?"

---

## Classic Space-Time Tradeoffs

### 1. Memoization vs Recomputation

```python
import sys
from functools import lru_cache

# ── APPROACH A: Pure recursion (exponential time, O(n) stack space) ──
def fib_naive(n: int) -> int:
    if n <= 1: return n
    return fib_naive(n-1) + fib_naive(n-2)
# Time: O(2^n)  Space: O(n) call stack

# ── APPROACH B: Memoization (linear time, O(n) space) ──
@lru_cache(maxsize=None)
def fib_memo(n: int) -> int:
    if n <= 1: return n
    return fib_memo(n-1) + fib_memo(n-2)
# Time: O(n)  Space: O(n) memo table + O(n) stack

# ── APPROACH C: Bottom-up DP (linear time, O(n) space) ──
def fib_dp(n: int) -> int:
    if n <= 1: return n
    dp = [0] * (n + 1)
    dp[1] = 1
    for i in range(2, n + 1):
        dp[i] = dp[i-1] + dp[i-2]
    return dp[n]
# Time: O(n)  Space: O(n)

# ── APPROACH D: Rolling array (optimal tradeoff) ──
def fib_rolling(n: int) -> int:
    """
    O(n) time, O(1) space — the optimal tradeoff for Fibonacci.
    Only keep last 2 values instead of entire array.
    """
    if n <= 1: return n
    a, b = 0, 1
    for _ in range(2, n + 1):
        a, b = b, a + b
    return b
# Time: O(n)  Space: O(1)

# ── APPROACH E: Matrix exponentiation (log time, O(1) space) ──
def fib_matrix(n: int) -> int:
    """
    O(log n) time — best time complexity, O(1) space.
    Trading more complex code for speed.
    """
    def mat_mul(A, B):
        return [[A[0][0]*B[0][0]+A[0][1]*B[1][0], A[0][0]*B[0][1]+A[0][1]*B[1][1]],
                [A[1][0]*B[0][0]+A[1][1]*B[1][0], A[1][0]*B[0][1]+A[1][1]*B[1][1]]]
    def mat_pow(M, n):
        if n == 1: return M
        if n % 2 == 0:
            h = mat_pow(M, n//2)
            return mat_mul(h, h)
        return mat_mul(M, mat_pow(M, n-1))
    M = [[1,1],[1,0]]
    return mat_pow(M, n)[0][1]
# Time: O(log n)  Space: O(log n) stack

# ── Summary of Fibonacci tradeoff curve ──
# Time    | Space  | Approach
# O(2^n)  | O(n)   | Naive recursion
# O(n)    | O(n)   | Memoization / full DP
# O(n)    | O(1)   | Rolling array ← USUALLY OPTIMAL
# O(log n)| O(1)   | Matrix exp  ← when n is huge (cryptography)
```

### 2. Hash Table vs Binary Search

```python
import bisect

# ── APPROACH A: Hash table lookup ──
def lookup_hash(arr: list) -> None:
    """
    Build: O(n) time, O(n) space
    Query: O(1) expected time
    """
    table = set(arr)
    # query: val in table → O(1) expected

# ── APPROACH B: Sort + binary search ──
def lookup_binary_search(arr: list) -> None:
    """
    Build: O(n log n) time, O(1) auxiliary space (in-place sort)
    Query: O(log n) time
    """
    arr.sort()   # or sorted() for O(n) extra space
    # query: bisect.bisect_left(arr, val) → O(log n)

# ── When to use which ──
# Hash table: O(n) space but O(1) query → use when queries dominate
# Binary search: O(n log n) build but O(log n) query, O(1) extra space
#   → use when space is critical or data is already sorted

# ── APPROACH C: Perfect hashing (advanced) ──
# Build: O(n) expected, O(n) space
# Query: O(1) WORST CASE (not just expected) 
# Use: When deterministic O(1) is critical (e.g., routers, compilers)
```

---

## Auxiliary Space Analysis

### In-place vs Out-of-place Sorting

```python
def merge_sort_outofplace(arr: list) -> list:
    """
    Standard merge sort: O(n log n) time, O(n) auxiliary space.
    The O(n) space comes from the merge step's temporary array.
    Call stack: O(log n) additional.
    """
    if len(arr) <= 1:
        return arr[:]
    mid = len(arr) // 2
    left = merge_sort_outofplace(arr[:mid])
    right = merge_sort_outofplace(arr[mid:])
    return merge(left, right)

def merge(left, right):
    result, i, j = [], 0, 0
    while i < len(left) and j < len(right):
        if left[i] <= right[j]:
            result.append(left[i]); i += 1
        else:
            result.append(right[j]); j += 1
    return result + left[i:] + right[j:]

# ── In-place merge sort (block merge) ──
def merge_inplace(arr: list, lo: int, mid: int, hi: int) -> None:
    """
    O(n log n) time, O(1) auxiliary space merge.
    But has larger constant — used in TimSort for small arrays.
    """
    left = arr[lo:mid+1]   # Still O(n) here — truly O(1) merge is complex
    right = arr[mid+1:hi+1]
    i = j = 0
    k = lo
    while i < len(left) and j < len(right):
        if left[i] <= right[j]:
            arr[k] = left[i]; i += 1
        else:
            arr[k] = right[j]; j += 1
        k += 1
    while i < len(left):
        arr[k] = left[i]; i += 1; k += 1
    while j < len(right):
        arr[k] = right[j]; j += 1; k += 1

# ── Space comparison table ──
# Algorithm      | Time       | Aux Space | Stable
# Merge Sort     | O(n log n) | O(n)      | Yes
# Quick Sort     | O(n log n) | O(log n)  | No   ← stack space
# Heap Sort      | O(n log n) | O(1)      | No   ← true in-place
# TimSort        | O(n log n) | O(n)      | Yes  ← Python's default
# Counting Sort  | O(n+k)     | O(k)      | Yes  ← only for integers
```

### Call Stack Depth Analysis

```python
import sys

def stack_depth_analysis():
    """
    Python default recursion limit: 1000.
    For n=10^6, naive recursive DFS/merge-sort WILL stack overflow.
    
    Call stack space for common algorithms:
    - Linear recursion (factorial): O(n) stack → overflow at n≈1000
    - Binary recursion (merge sort): O(log n) stack → safe for n=10^18
    - Tree DFS: O(height) → O(n) for skewed tree, O(log n) for balanced
    - Fibonacci naive: O(n) depth → overflow at n≈1000
    
    Solution 1: Increase recursion limit (hacky)
    Solution 2: Convert to iterative with explicit stack (correct)
    Solution 3: Tail recursion optimization (Python doesn't do this automatically)
    """
    sys.setrecursionlimit(10**6)   # Use sparingly — risks stack overflow

# ── Converting DFS to iterative (O(log n) stack → O(1) extra for balanced tree) ──
def dfs_iterative(root) -> list:
    """
    Replaces O(h) call stack with O(h) explicit stack.
    Same space, but: (1) no Python recursion limit, (2) can manipulate stack directly.
    """
    if not root: return []
    result, stack = [], [root]
    while stack:
        node = stack.pop()
        result.append(node.val)
        if node.right: stack.append(node.right)
        if node.left:  stack.append(node.left)
    return result
# Time: O(n)  Space: O(h) where h = tree height
```

---

## Cache Complexity & Memory Hierarchy

### Why Array Beats Linked List Despite Same O Complexity

```python
import time
import random

def cache_locality_demo(n: int = 10**6):
    """
    Both array scan and linked list traversal are O(n) operations.
    In practice, array is 5-100x faster due to cache locality.
    
    Cache hierarchy (typical):
    - L1 cache: 32KB, ~4 cycles access
    - L2 cache: 256KB, ~12 cycles access  
    - L3 cache: 6MB, ~40 cycles access
    - RAM: ~100ns = ~300 cycles access
    
    Cache line: 64 bytes = 8 int64s or 16 int32s
    
    Array scan: sequential access → almost every element is in cache line
    Linked list: random pointer chasing → cache miss on every node
    """
    # Array: sequential access — cache-friendly
    arr = list(range(n))
    start = time.perf_counter()
    total = sum(arr)   # accesses arr[0], arr[1], ... sequentially
    array_time = time.perf_counter() - start
    
    # Shuffle to simulate linked list pointer chasing
    indices = list(range(n))
    random.shuffle(indices)
    # Next "node" pointer is indices[i] — random access pattern
    start = time.perf_counter()
    total2, i = 0, 0
    for _ in range(n):
        total2 += arr[indices[i]]
        i = indices[i] % n
    linked_time = time.perf_counter() - start
    
    print(f"Array sequential: {array_time:.4f}s")
    print(f"Random access:    {linked_time:.4f}s")
    print(f"Speedup: {linked_time/array_time:.1f}x")

# Real-world implications:
# - Binary search on array: O(log n) but cache-unfriendly at large n
# - B-tree (database index): wider branching, fewer cache misses
# - Hash table with open addressing (linear probing): cache-friendly
# - Hash table with chaining (linked lists): cache-unfriendly
```

### Cache-Oblivious Algorithm Design

```python
def cache_oblivious_matrix_multiply(A, B, C, n):
    """
    Recursive block decomposition that achieves optimal cache behavior
    WITHOUT knowing the cache size.
    
    Cache complexity (external memory model):
    - Standard matrix multiply: O(n³/B) cache misses (B = cache line size)
    - Naive row-major: O(n³) cache misses if matrix doesn't fit in cache
    - Cache-oblivious: O(n³/(B·√M)) cache misses (M = cache size)
    
    The key insight: recursing down to 1×1 matrices ensures blocks of
    all sizes are processed, automatically fitting the cache at some level.
    """
    if n == 1:
        C[0][0] += A[0][0] * B[0][0]
        return
    
    mid = n // 2
    # Recursively multiply 2×2 blocks of matrices
    # (simplified — full implementation splits A, B, C into quadrants)
    pass

# ── Practical cache optimization techniques ──

def row_major_vs_column_major():
    """
    In Python (row-major storage), iterating rows is cache-friendly.
    Iterating columns causes cache misses.
    """
    n = 1000
    matrix = [[i*n + j for j in range(n)] for i in range(n)]
    
    # Cache-friendly: row-major access
    start = time.perf_counter()
    total = 0
    for i in range(n):
        for j in range(n):
            total += matrix[i][j]   # Sequential memory access
    row_time = time.perf_counter() - start
    
    # Cache-unfriendly: column-major access
    start = time.perf_counter()
    total = 0
    for j in range(n):
        for i in range(n):
            total += matrix[i][j]   # Strided memory access
    col_time = time.perf_counter() - start
    
    print(f"Row-major: {row_time:.4f}s, Col-major: {col_time:.4f}s")
    print(f"Cache benefit: {col_time/row_time:.1f}x speedup from row order")
```

---

## Bit Packing — Extreme Space Compression

```python
class BitArray:
    """
    Pack n booleans into n/64 integers (64x compression vs bool array).
    
    Use case: Sieve of Eratosthenes, visited[] in graph algorithms,
    bitmap indices in databases.
    
    Time: O(1) per get/set (with bit operations)
    Space: O(n/64) = O(n) but with 64× smaller constant
    """
    def __init__(self, n: int):
        self.n = n
        self.data = bytearray((n + 7) // 8)   # Each byte stores 8 bits
    
    def set(self, i: int) -> None:
        self.data[i >> 3] |= (1 << (i & 7))
    
    def clear(self, i: int) -> None:
        self.data[i >> 3] &= ~(1 << (i & 7))
    
    def get(self, i: int) -> bool:
        return bool(self.data[i >> 3] & (1 << (i & 7)))
    
    def count(self) -> int:
        """Count set bits using popcount."""
        return sum(bin(byte).count('1') for byte in self.data)

def sieve_of_eratosthenes_bitpacked(n: int) -> list:
    """
    Find all primes up to n.
    Standard: O(n log log n) time, O(n) space (1 byte per number)
    Bit-packed: O(n log log n) time, O(n/8) space
    
    For n=10^9: standard needs ~1GB, bit-packed needs ~125MB
    """
    is_prime = BitArray(n + 1)
    # Set all bits (all potentially prime)
    for i in range(n + 1):
        is_prime.set(i)
    is_prime.clear(0)
    is_prime.clear(1)
    
    p = 2
    while p * p <= n:
        if is_prime.get(p):
            # Mark multiples as composite
            for multiple in range(p*p, n+1, p):
                is_prime.clear(multiple)
        p += 1
    
    return [i for i in range(2, n+1) if is_prime.get(i)]

# Comparison
import sys
n = 10**6
bool_array_size = sys.getsizeof([True] * n)   # ~8MB
bit_array_size  = sys.getsizeof(bytearray(n // 8))  # ~125KB
print(f"Bool array: {bool_array_size/1024:.0f} KB")
print(f"Bit array:  {bit_array_size/1024:.0f} KB (64x smaller)")
```

---

## Rolling Array DP — Reducing O(n²) Space to O(n)

```python
def longest_common_subsequence_full(s: str, t: str) -> int:
    """Standard LCS: O(mn) time, O(mn) space."""
    m, n = len(s), len(t)
    dp = [[0] * (n+1) for _ in range(m+1)]
    for i in range(1, m+1):
        for j in range(1, n+1):
            if s[i-1] == t[j-1]:
                dp[i][j] = dp[i-1][j-1] + 1
            else:
                dp[i][j] = max(dp[i-1][j], dp[i][j-1])
    return dp[m][n]
# Time: O(mn)  Space: O(mn)

def longest_common_subsequence_rolling(s: str, t: str) -> int:
    """
    Rolling array optimization: only need previous row.
    O(mn) time, O(n) space — crucial for large inputs.
    
    Key insight: dp[i][j] only depends on dp[i-1][j-1], dp[i-1][j], dp[i][j-1]
    → only need current and previous row.
    """
    m, n = len(s), len(t)
    prev = [0] * (n + 1)
    
    for i in range(1, m + 1):
        curr = [0] * (n + 1)
        for j in range(1, n + 1):
            if s[i-1] == t[j-1]:
                curr[j] = prev[j-1] + 1
            else:
                curr[j] = max(prev[j], curr[j-1])
        prev = curr
    return prev[n]
# Time: O(mn)  Space: O(n)

def edit_distance_rolling(s: str, t: str) -> int:
    """
    Edit distance with O(n) space via rolling array.
    Further optimizable to single array with careful ordering.
    """
    m, n = len(s), len(t)
    prev = list(range(n + 1))   # Base case: delete all chars of t
    
    for i in range(1, m + 1):
        curr = [i] + [0] * n    # Base case: delete all chars of s
        for j in range(1, n + 1):
            if s[i-1] == t[j-1]:
                curr[j] = prev[j-1]
            else:
                curr[j] = 1 + min(prev[j], curr[j-1], prev[j-1])
        prev = curr
    return prev[n]
# Time: O(mn)  Space: O(n)

def knapsack_rolling(weights: list, values: list, capacity: int) -> int:
    """
    0/1 Knapsack with O(capacity) space.
    
    Critical: iterate j from HIGH to LOW to avoid using same item twice.
    This is the single-array rolling trick — right-to-left update.
    """
    dp = [0] * (capacity + 1)
    
    for w, v in zip(weights, values):
        # Iterate RIGHT TO LEFT — uses previous item's values
        for j in range(capacity, w - 1, -1):
            dp[j] = max(dp[j], dp[j - w] + v)
    
    return dp[capacity]
# Time: O(n × capacity)  Space: O(capacity)
```

---

## Morris Traversal — O(1) Space Tree Traversal

```python
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def morris_inorder(root: TreeNode) -> list:
    """
    Inorder traversal with O(1) extra space (no stack, no recursion).
    Uses the tree's None pointers as temporary "thread" links.
    
    Algorithm:
    1. If no left child: visit node, go right.
    2. If left child: find inorder predecessor (rightmost node of left subtree)
       a. If predecessor's right is None: thread it to current, go left
       b. If predecessor's right is current: remove thread, visit current, go right
    
    Space: O(1) — only 2 pointers (current, predecessor)
    Time: O(n) — each edge traversed at most twice (once to thread, once to unthread)
    
    CAUTION: Temporarily modifies the tree (restores it before returning).
    Not thread-safe. Use only when stack space is the bottleneck.
    """
    result = []
    curr = root
    
    while curr:
        if curr.left is None:
            # No left subtree — visit and go right
            result.append(curr.val)
            curr = curr.right
        else:
            # Find inorder predecessor
            pred = curr.left
            while pred.right and pred.right is not curr:
                pred = pred.right
            
            if pred.right is None:
                # Thread: predecessor → current
                pred.right = curr
                curr = curr.left
            else:
                # Remove thread, visit current, go right
                pred.right = None
                result.append(curr.val)
                curr = curr.right
    
    return result
# Time: O(n)  Space: O(1)

def morris_preorder(root: TreeNode) -> list:
    """Preorder variant of Morris traversal. O(n) time, O(1) space."""
    result = []
    curr = root
    
    while curr:
        if curr.left is None:
            result.append(curr.val)
            curr = curr.right
        else:
            pred = curr.left
            while pred.right and pred.right is not curr:
                pred = pred.right
            
            if pred.right is None:
                result.append(curr.val)   # Visit BEFORE threading (preorder)
                pred.right = curr
                curr = curr.left
            else:
                pred.right = None
                curr = curr.right
    
    return result
# Time: O(n)  Space: O(1)
```

---

## Classic Problems

### Problem 1: Two-Sum with Different Space Constraints — Medium

```python
def two_sum_variants(nums: list, target: int) -> tuple:
    """
    Multiple approaches with different time-space tradeoffs.
    """
    pass

# ── Approach A: Brute force ──
def two_sum_brute(nums, target):
    for i in range(len(nums)):
        for j in range(i+1, len(nums)):
            if nums[i] + nums[j] == target:
                return (i, j)
    return None
# Time: O(n²)  Space: O(1)

# ── Approach B: Hash map ──
def two_sum_hash(nums, target):
    seen = {}   # val → index
    for i, x in enumerate(nums):
        complement = target - x
        if complement in seen:
            return (seen[complement], i)
        seen[x] = i
    return None
# Time: O(n)  Space: O(n)  ← classic time-space tradeoff

# ── Approach C: Sort + two pointers (when indices don't matter) ──
def two_sum_sorted(nums, target):
    sorted_nums = sorted(enumerate(nums), key=lambda x: x[1])
    lo, hi = 0, len(nums) - 1
    while lo < hi:
        s = sorted_nums[lo][1] + sorted_nums[hi][1]
        if s == target:
            return (sorted_nums[lo][0], sorted_nums[hi][0])
        elif s < target:
            lo += 1
        else:
            hi -= 1
    return None
# Time: O(n log n)  Space: O(n) for sorted copy

# ── Tradeoff summary ──
# Approach   | Time       | Space | Notes
# Brute      | O(n²)      | O(1)  | Use when memory is extreme bottleneck
# Hash       | O(n)       | O(n)  | Standard FAANG answer
# Sort+2ptr  | O(n log n) | O(n)  | When data comes sorted or needs sorted order
```

### Problem 2: Palindrome Check — Space Optimized Variants — Medium

```python
def is_palindrome_string(s: str) -> bool:
    """O(n) time, O(1) space — two pointers."""
    lo, hi = 0, len(s) - 1
    while lo < hi:
        if s[lo] != s[hi]: return False
        lo += 1; hi -= 1
    return True

def is_palindrome_linked_list(head) -> bool:
    """
    Check if linked list is palindrome.
    
    Approach 1: Copy to array → O(n) time, O(n) space
    Approach 2: Find middle, reverse second half, compare → O(n) time, O(1) space
    
    Space-optimal approach:
    """
    # Step 1: Find middle (slow/fast pointer)
    slow = fast = head
    while fast and fast.next:
        slow = slow.next
        fast = fast.next.next
    
    # Step 2: Reverse second half
    prev, curr = None, slow
    while curr:
        nxt = curr.next
        curr.next = prev
        prev, curr = curr, nxt
    second_half = prev
    
    # Step 3: Compare
    left, right = head, second_half
    result = True
    while right:
        if left.val != right.val:
            result = False
            break
        left = left.next
        right = right.next
    
    # Step 4: Restore (good practice)
    prev, curr = None, second_half
    while curr:
        nxt = curr.next
        curr.next = prev
        prev, curr = curr, nxt
    
    return result
# Time: O(n)  Space: O(1)
```

### Problem 3: Find Duplicate in O(1) Space — Hard (Floyd's)

```python
def find_duplicate(nums: list) -> int:
    """
    Given n+1 integers in [1,n], find the duplicate.
    
    Constraints: 
    - Must use O(1) extra space (can't sort or use hash set)
    - Must NOT modify the array
    
    Naive: sort (modifies) or hash set (O(n) space) — both violate constraints.
    
    Space-time tradeoff analysis:
    - O(n) space hash: trivial O(n) time, O(n) space
    - O(1) space sort: O(n log n) time but modifies array
    - O(1) space Floyd: O(n) time, O(1) space ← optimal
    
    Key insight: treat array as linked list where nums[i] = next pointer.
    Array has duplicate → cycle in the "linked list."
    Floyd's cycle detection finds cycle start = duplicate.
    """
    # Phase 1: Detect cycle
    slow = fast = nums[0]
    while True:
        slow = nums[slow]
        fast = nums[nums[fast]]
        if slow == fast:
            break
    
    # Phase 2: Find cycle entry (= duplicate)
    slow = nums[0]
    while slow != fast:
        slow = nums[slow]
        fast = nums[fast]
    
    return slow

# Proof of correctness:
# If duplicate value is d, then both index d and its duplicate index point to nums[d].
# This creates a "ρ" shaped structure: a tail (0 → ... → entry) and a cycle.
# Floyd's meeting point and cycle entry detection finds d exactly.

# Time: O(n)  Space: O(1)  — STRICTLY better than O(n) space approaches when memory is tight
```

### Problem 4: Subsets / Combinations — Bitmask vs Recursion — Medium

```python
def subsets_bitmask(nums: list) -> list:
    """
    Generate all 2^n subsets using bitmask enumeration.
    
    Space advantage: O(n·2^n) output, O(n) working space.
    The bitmask method avoids recursive call stack.
    """
    n = len(nums)
    result = []
    for mask in range(1 << n):    # 0 to 2^n - 1
        subset = [nums[i] for i in range(n) if mask & (1 << i)]
        result.append(subset)
    return result
# Time: O(n·2^n)  Space: O(n) auxiliary (output excluded)

def subsets_recursive(nums: list, path: list = None, start: int = 0) -> list:
    """
    Recursive backtracking approach.
    Space: O(n) call stack + O(n) for current path.
    """
    if path is None:
        result = []
        def backtrack(start, path):
            result.append(path[:])
            for i in range(start, len(nums)):
                path.append(nums[i])
                backtrack(i + 1, path)
                path.pop()
        backtrack(0, [])
        return result
# Time: O(n·2^n)  Space: O(n) call stack + O(n) path

# Tradeoff: bitmask avoids recursion overhead and stack depth limit
# For n > 20-25: both are impractical (2^25 ≈ 33M entries)
```

### Problem 5: Maximum Subarray — Space Trade-off Study — Medium

```python
def max_subarray_all_approaches(nums: list) -> int:
    """Compare space usage of different approaches."""
    
    # ── A: Prefix sum with O(n) space ──
    n = len(nums)
    prefix = [0] * (n + 1)
    for i, x in enumerate(nums):
        prefix[i+1] = prefix[i] + x
    min_prefix = 0
    ans = float('-inf')
    for i in range(1, n+1):
        ans = max(ans, prefix[i] - min_prefix)
        min_prefix = min(min_prefix, prefix[i])
    return ans
    # Time: O(n)  Space: O(n)
    
    # ── B: Kadane's O(1) space ──
    max_sum = curr = nums[0]
    for x in nums[1:]:
        curr = max(x, curr + x)
        max_sum = max(max_sum, curr)
    return max_sum
    # Time: O(n)  Space: O(1)  ← BETTER

# ── When O(n) prefix sum IS worth it ──
# When you also need the indices of the subarray:
def max_subarray_with_indices(nums: list) -> tuple:
    """O(n) space but returns actual subarray indices."""
    n = len(nums)
    prefix = [0] * (n + 1)
    for i, x in enumerate(nums):
        prefix[i+1] = prefix[i] + x
    
    min_idx, min_val = 0, 0
    best = float('-inf')
    lo = hi = 0
    
    for i in range(1, n+1):
        if prefix[i] - min_val > best:
            best = prefix[i] - min_val
            lo = min_idx
            hi = i - 1
        if prefix[i] < min_val:
            min_val = prefix[i]
            min_idx = i
    
    return best, lo, hi
# Time: O(n)  Space: O(n)
```

### Problem 6: Longest Repeated Substring — Binary Search + Rolling Hash — Very Hard

```python
def longest_repeated_substring(s: str) -> str:
    """
    Find the longest substring that appears at least twice.
    
    Time-Space Tradeoffs:
    - Suffix array: O(n log n) time, O(n) space
    - Binary search + rolling hash: O(n log n) time, O(n) space
    - Suffix trie: O(n²) space — too slow for large n
    - Naive O(n²) comparison: O(n²) time, O(n) space
    
    We use binary search on answer length + rolling hash to check feasibility.
    """
    n = len(s)
    MOD = (1 << 61) - 1   # Mersenne prime — near-perfect hash
    BASE = 131
    
    def has_duplicate_of_length(L: int) -> str:
        """Check if any substring of length L appears ≥ twice."""
        if L == 0: return ""
        
        # Compute initial hash
        h = 0
        power = 1
        for i in range(L):
            h = (h * BASE + ord(s[i])) % MOD
            if i < L - 1:
                power = power * BASE % MOD
        
        seen = {h: [0]}
        
        for i in range(1, n - L + 1):
            # Rolling hash update: remove s[i-1], add s[i+L-1]
            h = (h - ord(s[i-1]) * power) % MOD
            h = (h * BASE + ord(s[i+L-1])) % MOD
            h %= MOD
            
            if h in seen:
                # Verify (avoid hash collision false positive)
                for prev in seen[h]:
                    if s[prev:prev+L] == s[i:i+L]:
                        return s[i:i+L]
                seen[h].append(i)
            else:
                seen[h] = [i]
        
        return ""
    
    # Binary search on the answer
    lo, hi = 1, n - 1
    result = ""
    while lo <= hi:
        mid = (lo + hi) // 2
        candidate = has_duplicate_of_length(mid)
        if candidate:
            result = candidate
            lo = mid + 1
        else:
            hi = mid - 1
    
    return result
# Time: O(n log n) expected  Space: O(n)
```

---

## Advanced Variations

### Bloom Filter — Approximate Membership with O(1) Space per Element

```python
import math, hashlib

class BloomFilter:
    """
    Probabilistic set membership. O(1) space per element (with error).
    False positives possible, false negatives impossible.
    
    Optimal parameters for desired false positive rate ε:
    - Number of bits: m = -n·ln(ε) / (ln 2)²
    - Number of hash functions: k = (m/n)·ln 2
    
    Space: O(n) bits = O(n/8) bytes = 64x less than a hash set of pointers
    
    Use case: Database query optimization, distributed caching, spell checkers.
    """
    def __init__(self, n: int, fpr: float = 0.01):
        self.m = int(-n * math.log(fpr) / (math.log(2)**2))
        self.k = int(self.m / n * math.log(2))
        self.bits = bytearray((self.m + 7) // 8)
        print(f"Bloom filter: {self.m} bits ({self.m//8} bytes) "
              f"for {n} elements with {fpr*100}% FPR")
        print(f"Using {self.k} hash functions")
    
    def _hashes(self, item: str):
        for i in range(self.k):
            h = int(hashlib.md5(f"{item}:{i}".encode()).hexdigest(), 16)
            yield h % self.m
    
    def add(self, item: str) -> None:
        for pos in self._hashes(item):
            self.bits[pos >> 3] |= (1 << (pos & 7))
    
    def contains(self, item: str) -> bool:
        """Returns True if item MIGHT be in set. Never false negative."""
        return all(
            self.bits[pos >> 3] & (1 << (pos & 7))
            for pos in self._hashes(item)
        )
```

---

## Edge Cases Bible

1. **Recursion depth**: Python's default 1000 limit kills any O(n) recursive solution for n > 10^3. Always have an iterative fallback using an explicit stack.

2. **Integer overflow in bit manipulation**: Python ints are arbitrary precision, but in languages like C/Java, bit packing needs careful masking. In Python, `~x` gives -(x+1), not the unsigned complement.

3. **Rolling array DP direction**: For 0/1 knapsack, iterating left-to-right in the rolling array incorrectly uses the current item twice (becoming unbounded knapsack). Always iterate right-to-left.

4. **Morris traversal restores the tree**: If you call Morris traversal on a tree and then another function uses the tree concurrently, the temporary threading WILL break the other function. Morris is not reentrant.

5. **Cache effects in space-reduced algorithms**: A bitpacked sieve is smaller but requires more computation per element (bit extraction). For n < 10^7, a byte array may be faster despite being 8× larger.

6. **Bloom filter false positive rate increases with more inserts**: Designed for n elements; inserting 10× more than designed capacity can degrade FPR to 99%.

7. **Two-stack queue worst-case vs amortized**: The worst case for a single dequeue is O(n) (when outbox is empty and inbox has n elements). Never claim O(1) worst-case — only O(1) amortized.

---

## Interview Tips

### What Interviewers Look For

1. **Always state the space complexity explicitly**: Many candidates forget. Say "this uses O(n) auxiliary space, not counting input/output."

2. **Distinguish between auxiliary space and total space**: In-place sort uses O(1) auxiliary space but O(n) total (the input). Interviewers mean auxiliary when asking for "space complexity."

3. **Rolling array is a must-know**: When you present a 2D DP solution, immediately offer the O(n) space rolling array optimization. This shows depth.

4. **Cache locality matters in systems interviews**: For Senior/Staff roles, mentioning "this has better cache behavior because of sequential memory access" distinguishes you from candidates who only think about asymptotic complexity.

5. **Know when O(N) space is ACCEPTABLE**:
   - Most FAANG interview problems allow O(n) auxiliary space
   - O(n²) or O(n!) space is always unacceptable for n > 100
   - Follow-up question: "Can you do it in O(1) space?" → have Morris traversal, two-pointer, bit manipulation ready

6. **The space-time tradeoff decision framework**:
   - If time is bottleneck: use hash maps, precomputed lookups, memoization
   - If space is bottleneck: rolling arrays, iterative DFS, bit packing, streaming algorithms
   - In practice (FAANG): time usually matters more than space — O(n) space is fine

7. **Streaming and external memory**: For truly massive datasets (data doesn't fit in RAM), mention external merge sort, reservoir sampling, Count-Min Sketch — shows systems awareness.

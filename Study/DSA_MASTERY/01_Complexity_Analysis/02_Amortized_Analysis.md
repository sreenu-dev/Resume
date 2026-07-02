# Amortized Analysis — Mastery Guide

## Core Concept & Invariant

Amortized analysis computes the **average cost per operation over a worst-case sequence**
of operations — not the average over random inputs. The guarantee is:

> For ANY sequence of n operations, total cost ≤ n × amortized_cost

The invariant: **credits never go negative** (accounting method), or equivalently,
**Φ(state) ≥ Φ(initial_state)** (potential method). This ensures amortized bounds
are real upper bounds — you cannot "borrow from the future."

**Key distinction from average-case**: Amortized analysis gives worst-case guarantees
over sequences of operations. No probability is involved.

**Three Methods (choose based on what's easier to define):**
1. **Aggregate**: Compute total cost T(n) for n operations; amortized = T(n)/n
2. **Accounting**: Assign amortized cost to each op; excess stored as "credits"
3. **Potential (Φ)**: Define energy function; amortized_cost = actual_cost + ΔΦ

---

## Method 1: Aggregate Analysis

### Dynamic Array (Push Amortized O(1))

When capacity is exceeded, double the array and copy all elements.

```python
class DynamicArray:
    """
    Aggregate amortized analysis of push():
    After n pushes starting from capacity 1:
    - Total copies: 1 + 2 + 4 + ... + 2^⌊log₂n⌋ < 2n
    - Total pushes: n
    - Total operations: n + 2n = 3n → amortized O(1) per push
    """
    def __init__(self):
        self._data = [None]
        self._size = 0
        self._capacity = 1
        self._total_ops = 0   # for aggregate counting
    
    def push(self, val):
        if self._size == self._capacity:
            # Expensive: copy all elements to new array
            new_data = [None] * (2 * self._capacity)
            for i in range(self._size):
                new_data[i] = self._data[i]
                self._total_ops += 1         # each copy counted
            self._data = new_data
            self._capacity *= 2
        self._data[self._size] = val
        self._size += 1
        self._total_ops += 1                 # the actual push
    
    def amortized_cost_so_far(self):
        """Should remain ≤ 3 for all n."""
        return self._total_ops / max(self._size, 1)

# Verification
arr = DynamicArray()
for i in range(1000):
    arr.push(i)
    if i in [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 999]:
        print(f"After {i+1:4d} pushes: amortized = {arr.amortized_cost_so_far():.3f}")
```

**Proof**: Total copies after n pushes = Σ_{k=0}^{⌊log₂n⌋} 2^k < 2n.  
Total ops = n (pushes) + <2n (copies) < 3n → amortized O(1).

**Why 2× growth matters**: Growth factor r > 1 ensures geometric series.  
- r = 2: amortized O(1) push, ~50% space waste  
- r = 1.5: amortized O(1) push, ~33% space waste (Python's actual growth factor)  
- r = 1: amortized O(n) push (not amortized O(1)!)

---

## Method 2: Accounting (Credit) Method

### Stack with Multi-Pop (Amortized O(1) per operation)

```python
class AccountingStack:
    """
    Operations: push(x) — O(1), pop() — O(1), multi_pop(k) — O(min(k, size))
    
    Accounting argument:
    - Charge push: 2 credits (1 for push itself, 1 saved "for future pop")
    - Charge pop: 0 credits (use saved credit from push time)
    - Charge multi_pop(k): 0 credits (use k saved credits)
    
    Invariant: every element in stack has exactly 1 saved credit.
    Credits never go negative → amortized O(1) per operation.
    Total cost for n operations: at most 2n credits → O(n) total.
    """
    def __init__(self):
        self._stack = []
        self._actual_ops = 0
        self._amortized_charges = 0
    
    def push(self, val):
        self._stack.append(val)
        self._actual_ops += 1
        self._amortized_charges += 2     # Pay 2: 1 now, 1 saved
    
    def pop(self):
        if not self._stack:
            return None
        val = self._stack.pop()
        self._actual_ops += 1
        self._amortized_charges += 0     # Use saved credit from push
        return val
    
    def multi_pop(self, k: int):
        popped = []
        actual_pops = min(k, len(self._stack))
        for _ in range(actual_pops):
            popped.append(self._stack.pop())
            self._actual_ops += 1        # Each pop costs 1 actual
            # Uses 1 saved credit per pop → total credits stay ≥ 0
        self._amortized_charges += 0     # Amortized charge for multi_pop = 0
        return popped
    
    def verify_invariant(self):
        """Credits = (2 × pushes) − (total pops) ≥ size of stack ≥ 0."""
        return len(self._stack), self._actual_ops, self._amortized_charges
```

### Binary Counter (Amortized O(1) per increment)

```python
class BinaryCounter:
    """
    k-bit binary counter incremented n times.
    Naive worst-case: O(k) per increment (all bits flip when 0111...1 → 1000...0)
    
    Accounting proof:
    - Amortized cost of increment = 2 (charge 2 regardless of actual bit flips)
    - Actual cost = 1 (set bit to 1) + (number of trailing 1→0 flips)
    - Each bit is set to 1 at most once before being reset to 0
    - So total 1→0 flips ≤ total 0→1 sets ≤ n
    - Total cost: n (0→1 sets) + ≤n (1→0 resets) = O(n) → amortized O(1)
    
    Potential method (elegant):
    - Φ = number of 1-bits currently set
    - Amortized cost = actual cost + ΔΦ
    - If t trailing 1s: actual = t+1, ΔΦ = 1-t
    - Amortized = (t+1) + (1-t) = 2 = O(1) ✓
    """
    def __init__(self, k: int):
        self.bits = [0] * k        # bits[0] = LSB
        self.k = k
        self.actual_flips = 0
    
    def increment(self):
        i = 0
        # Flip trailing 1s to 0s, then set first 0 to 1
        while i < self.k and self.bits[i] == 1:
            self.bits[i] = 0
            self.actual_flips += 1
            i += 1
        if i < self.k:
            self.bits[i] = 1
            self.actual_flips += 1
    
    def value(self):
        return sum(b * (2**i) for i, b in enumerate(self.bits))
    
    def potential(self):
        """Φ = number of 1-bits."""
        return sum(self.bits)

def verify_binary_counter_amortized(n_increments: int = 100):
    counter = BinaryCounter(k=16)
    total_actual = 0
    
    for i in range(n_increments):
        phi_before = counter.potential()
        counter.increment()
        phi_after = counter.potential()
        actual_cost = counter.actual_flips - total_actual
        amortized_cost = actual_cost + (phi_after - phi_before)
        total_actual = counter.actual_flips
        assert amortized_cost == 2, f"Amortized cost should be 2, got {amortized_cost}"
    
    print(f"After {n_increments} increments:")
    print(f"  Total actual flips: {counter.actual_flips}")
    print(f"  Expected ≤ 2n = {2*n_increments}")
    print(f"  Ratio: {counter.actual_flips/n_increments:.3f} (should be ≤ 2)")
    assert counter.actual_flips <= 2 * n_increments

verify_binary_counter_amortized()
```

---

## Method 3: Potential Function Method (Most Powerful)

### Formal Definition

```
Amortized cost of operation i = actual_cost(i) + Φ(Dᵢ) − Φ(Dᵢ₋₁)

where Dᵢ is the data structure state after operation i, Φ is the potential function.

Total amortized cost = Σ amortized_cost(i)
                     = Σ actual_cost(i) + Φ(Dₙ) − Φ(D₀)
                     ≥ Σ actual_cost(i)    (since Φ(Dₙ) ≥ Φ(D₀) = 0)

So: Σ actual_cost(i) ≤ Σ amortized_cost(i)
```

### Splay Tree — Amortized O(log N) per Operation

```python
class SplayNode:
    def __init__(self, key):
        self.key = key
        self.left = self.right = self.parent = None
        self.size = 1   # subtree size for potential

class SplayTree:
    """
    Splay tree amortized O(log n) per splay operation.
    
    Potential function: Φ(T) = Σ_{v ∈ T} log(size(subtree(v)))
                             = Σ rank(v)   where rank(v) = log(size(v))
    
    Access Lemma (core of analysis):
    Amortized cost of splaying node x to root ≤ 3·(rank(root) − rank(x)) + 1
                                               ≤ 3·log(n) + 1 = O(log n)
    
    Key operations:
    - Zig (single rotation): amortized ≤ 3·(rank(parent) − rank(x)) + 1
    - Zig-Zig (two same-direction rotations): amortized ≤ 3·(rank(grandparent) − rank(x))
    - Zig-Zag (two opposite-direction rotations): amortized ≤ 3·(rank(grandparent) − rank(x))
    
    The telescoping sum over all splay operations gives O(log n) amortized.
    """
    def __init__(self):
        self.root = None
    
    def _update_size(self, node):
        if node:
            node.size = 1
            if node.left:  node.size += node.left.size
            if node.right: node.size += node.right.size
    
    def _rotate(self, x):
        """Right or left rotate x with its parent."""
        p = x.parent
        g = p.parent
        
        if x == p.left:       # Right rotation
            p.left = x.right
            if x.right: x.right.parent = p
            x.right = p
        else:                  # Left rotation
            p.right = x.left
            if x.left: x.left.parent = p
            x.left = p
        
        x.parent = g
        p.parent = x
        if g:
            if g.left == p: g.left = x
            else:           g.right = x
        
        self._update_size(p)
        self._update_size(x)
    
    def _splay(self, x):
        """Splay x to root. This is the core amortized O(log n) step."""
        while x.parent:
            p = x.parent
            g = p.parent
            if g is None:
                # Zig case: single rotation
                self._rotate(x)
            elif (g.left == p) == (p.left == x):
                # Zig-Zig: rotate parent first, then x
                self._rotate(p)
                self._rotate(x)
            else:
                # Zig-Zag: rotate x twice
                self._rotate(x)
                self._rotate(x)
        self.root = x
    
    def potential(self) -> float:
        """Φ(T) = Σ log(size(v)) for all v."""
        import math
        def subtree_potential(node):
            if not node: return 0.0
            return (math.log2(node.size) +
                    subtree_potential(node.left) +
                    subtree_potential(node.right))
        return subtree_potential(self.root)
```

---

## Union-Find with Path Compression — Amortized O(α(N))

```python
class UnionFind:
    """
    Union by rank + path compression.
    Amortized cost per operation: O(α(N)) where α is the inverse Ackermann function.
    
    Potential function for α(N) analysis (Tarjan 1975):
    - Define rank(x) = upper bound on height of subtree at x
    - Define level(x) = max k such that rank(parent(x)) ≥ A_k(rank(x))
      where A_k is the k-th Ackermann function
    - Define iter(x) = max i such that A_{level(x)}^{(i)}(rank(x)) ≤ rank(parent(x))
    - Potential: Φ = Σ_{x is not a root, rank(x) > 0} (α(n) − level(x)) × rank(x) − iter(x)
    
    This complex potential proves total cost of m operations is O(m·α(n)).
    
    Practical: α(10^80) = 4. So for all practical purposes, O(1) per operation.
    """
    def __init__(self, n: int):
        self.parent = list(range(n))
        self.rank = [0] * n
        self.components = n
        self._op_count = 0
        self._total_path_compressions = 0
    
    def find(self, x: int) -> int:
        """
        Path compression: after find(x), all nodes on path to root
        point directly to root. This "flattens" the tree for future ops.
        
        Without path compression: O(log n) amortized
        With path compression only: O(log n) amortized  
        With both union by rank AND path compression: O(α(n)) amortized
        """
        self._op_count += 1
        if self.parent[x] != x:
            original_parent = self.parent[x]
            self.parent[x] = self.find(self.parent[x])   # Path compression
            if self.parent[x] != original_parent:
                self._total_path_compressions += 1
        return self.parent[x]
    
    def union(self, x: int, y: int) -> bool:
        """Union by rank: attach smaller rank tree under larger rank root."""
        rx, ry = self.find(x), self.find(y)
        if rx == ry:
            return False
        
        # Union by rank
        if self.rank[rx] < self.rank[ry]:
            rx, ry = ry, rx
        self.parent[ry] = rx
        if self.rank[rx] == self.rank[ry]:
            self.rank[rx] += 1
        
        self.components -= 1
        return True
    
    def connected(self, x: int, y: int) -> bool:
        return self.find(x) == self.find(y)

def demonstrate_alpha_n():
    """
    Show that α(n) ≤ 4 for astronomically large n.
    Ackermann function A(m,n):
    A(0,n) = n+1
    A(m,0) = A(m-1,1)
    A(m,n) = A(m-1, A(m,n-1))
    
    A(1,n) = n+2
    A(2,n) = 2n+3
    A(3,n) = 2^(n+3) - 3
    A(4,1) = 2^2^2^2 - 3 ≈ 10^19728 (vastly larger than atoms in universe)
    
    α(n) = min{k : A(k,k) ≥ n}
    α(10^80) = 4  ← effectively constant
    """
    print("Ackermann function growth:")
    print(f"A(1,4) = {4+2}")
    print(f"A(2,4) = {2*4+3}")
    print(f"A(3,2) = {2**(2+3)-3}")
    print(f"A(4,1) ≈ 10^19728 (larger than atoms in universe)")
    print(f"α(n) = 4 for all practical n")

demonstrate_alpha_n()
```

---

## Classic Problems

### Problem 1: Prove Dynamic Array Has O(1) Amortized Push — Medium

**Using all three methods:**

```python
def dynamic_array_three_proofs():
    """
    Demonstrates aggregate, accounting, and potential method give same result.
    """
    print("=" * 60)
    print("METHOD 1: AGGREGATE")
    print("=" * 60)
    print("""
    After n pushes, total copy operations:
    = 1 + 2 + 4 + 8 + ... + 2^⌊log₂n⌋
    < 2n  (geometric series sum < 2 × last term ≤ 2n)
    
    Total ops = n pushes + <2n copies < 3n
    Amortized cost = 3n/n = O(1) ✓
    """)
    
    print("=" * 60)
    print("METHOD 2: ACCOUNTING")
    print("=" * 60)
    print("""
    Charge each push $3:
    - $1 to actually store the element
    - $2 saved as "moving credit" for future doubling
    
    When array doubles (capacity C → 2C):
    - Need to move C elements
    - The C elements currently stored each have $2 saved
    - Total credits = 2C ≥ C (enough to pay for all copies)
    
    Credits never negative → amortized cost = $3 = O(1) ✓
    """)
    
    print("=" * 60)
    print("METHOD 3: POTENTIAL")
    print("=" * 60)
    print("""
    Φ(state) = 2 × size − capacity
    
    Invariant: Φ ≥ 0 (since size ≤ capacity always, but 2×size ≥ capacity
    when array is at least half full, which is true after any doubling)
    
    Case A: No doubling (size < capacity before push):
        actual_cost = 1
        ΔΦ = 2(size+1) − capacity − (2·size − capacity) = 2
        amortized = 1 + 2 = 3 = O(1) ✓
    
    Case B: Doubling triggered (size = capacity = C):
        actual_cost = C + 1   (C copies + 1 push)
        new_capacity = 2C, new_size = C + 1
        ΔΦ = 2(C+1) − 2C − (2C − C) = 2C+2 − 2C − C = 2 − C
        amortized = (C+1) + (2−C) = 3 = O(1) ✓
    
    Both cases: amortized cost = 3 = O(1) ✓
    """)
```

**Time per push**: O(1) amortized, O(n) worst-case  
**Space**: Θ(n) with at most 2× wasted capacity

---

### Problem 2: Queue Implemented with Two Stacks — Medium

```python
class TwoStackQueue:
    """
    Implement queue using two stacks.
    
    Amortized O(1) per enqueue/dequeue using accounting method:
    - Charge enqueue: 2 credits (1 for push to inbox, 1 saved for transfer)
    - Charge dequeue: 0 credits (use saved credit when transferring)
    
    Transfer: only happens when outbox empty → moves all inbox to outbox.
    Each element moved at most once (inbox→outbox) → total transfers ≤ n.
    
    Potential method:
    Φ = size of inbox stack
    - enqueue: actual=1, ΔΦ=1, amortized=2
    - dequeue (outbox non-empty): actual=1, ΔΦ=0, amortized=1
    - dequeue (transfer needed): actual=1+|inbox|, ΔΦ=−|inbox|, amortized=1
    All cases O(1) amortized.
    """
    def __init__(self):
        self.inbox = []    # for enqueue
        self.outbox = []   # for dequeue
    
    def enqueue(self, val):
        self.inbox.append(val)
    
    def dequeue(self):
        if not self.outbox:
            # Transfer: at most once per element
            while self.inbox:
                self.outbox.append(self.inbox.pop())
        if not self.outbox:
            raise IndexError("Queue is empty")
        return self.outbox.pop()
    
    def peek(self):
        if not self.outbox:
            while self.inbox:
                self.outbox.append(self.inbox.pop())
        return self.outbox[-1] if self.outbox else None

# Time:  O(1) amortized per enqueue and dequeue
# Space: O(n) total
```

---

### Problem 3: LRU Cache — Amortized O(1) per get/put — Hard

```python
from collections import OrderedDict

class LRUCache:
    """
    get/put in O(1) using OrderedDict (doubly-linked list + hash map).
    
    Amortized analysis: each operation does O(1) work per call.
    No amortized tricks needed — pure O(1) per op via the right data structure.
    
    If implemented with splay tree instead: O(log n) amortized via access lemma.
    """
    def __init__(self, capacity: int):
        self.cap = capacity
        self.cache = OrderedDict()   # key → value, ordered by recency
    
    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        self.cache.move_to_end(key)   # Mark as most recently used
        return self.cache[key]
    
    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self.cache.move_to_end(key)
        self.cache[key] = value
        if len(self.cache) > self.cap:
            self.cache.popitem(last=False)   # Remove LRU (first item)

# Time:  O(1) per get and put
# Space: O(capacity)
```

---

### Problem 4: Amortized Analysis of Multipop Stack — Hard Formal Proof

```python
def multipop_amortized_formal():
    """
    Formal amortized analysis:
    
    Given n operations: pushes, pops, and multipops(k).
    Show total cost is O(n).
    
    Potential method:
    Φ = number of elements in stack
    
    Operation analysis:
    
    push(x):
        actual = 1
        ΔΦ = +1
        amortized = 1 + 1 = 2 = O(1)
    
    pop():
        actual = 1
        ΔΦ = -1
        amortized = 1 + (-1) = 0 = O(1)
    
    multipop(k):  let t = min(k, |stack|)
        actual = t
        ΔΦ = -t
        amortized = t + (-t) = 0 = O(1)
    
    Total amortized cost ≤ 2n (since each push charges 2, others charge 0-2).
    Φ(Dₙ) ≥ 0 = Φ(D₀)
    
    Therefore: Σ actual_cost ≤ Σ amortized_cost ≤ 2n = O(n)
    
    CRITICAL INSIGHT: Even though a single multipop can cost O(n),
    you cannot multipop more elements than were pushed.
    The potential tracks this — you pay at push time, not pop time.
    """
    import random
    
    stack = []
    phi = 0   # Φ = len(stack)
    total_actual = 0
    total_amortized = 0
    
    ops = []
    for _ in range(200):
        op = random.choice(['push', 'pop', 'multipop'])
        ops.append(op)
    
    for op in ops:
        if op == 'push':
            val = random.randint(1, 100)
            stack.append(val)
            actual = 1
            delta_phi = 1
        elif op == 'pop':
            if not stack: continue
            stack.pop()
            actual = 1
            delta_phi = -1
        else:  # multipop
            k = random.randint(1, max(1, len(stack)))
            t = min(k, len(stack))
            for _ in range(t): stack.pop()
            actual = t
            delta_phi = -t
        
        amortized = actual + delta_phi
        total_actual += actual
        total_amortized += amortized
        phi += delta_phi
        
        assert phi == len(stack), "Potential invariant broken!"
        assert phi >= 0, "Potential went negative!"
    
    n = len(ops)
    print(f"n operations: {n}")
    print(f"Total actual cost: {total_actual}")
    print(f"Total amortized cost: {total_amortized}")
    print(f"Upper bound 2n: {2*n}")
    assert total_actual <= total_amortized + phi
    print("✓ Amortized bound verified!")

multipop_amortized_formal()
```

---

### Problem 5: Fibonacci Heap — Decrease-Key Amortized O(1) — Very Hard

```python
class FibHeapNode:
    """
    Fibonacci heap node for conceptual amortized analysis.
    
    Amortized costs (using potential Φ = trees + 2·marked_nodes):
    - insert:       O(1) amortized  (add tree, ΔΦ=+1)
    - find_min:     O(1) amortized  (just return min pointer)
    - union:        O(1) amortized  (link root lists)
    - extract_min:  O(log n) amortized  (consolidate: at most log n trees after)
    - decrease_key: O(1) amortized  (cut + cascading cut; marked nodes pay themselves)
    - delete:       O(log n) amortized
    
    This is why Dijkstra with Fibonacci heap is O(E + V log V)
    vs O((E + V) log V) with binary heap.
    
    Key potential argument for decrease_key:
    Φ = (# trees) + 2·(# marked nodes)
    
    decrease_key causing c cascading cuts:
        actual = O(c)
        ΔΦ = (c-1) trees added − (c-1) nodes unmarked − possibly 1 marked
           = (c-1) + 2·(-(c-1)) + 2·1 = (c-1) - 2(c-1) + 2 = -(c-1) + 2
        amortized = c + (-(c-1)+2) = c - c + 1 + 2 = 3 = O(1) ✓
    """
    def __init__(self, key, val=None):
        self.key = key
        self.val = val
        self.degree = 0
        self.marked = False
        self.parent = None
        self.child = None
        self.left = self
        self.right = self
```

---

### Problem 6: Self-Adjusting List — Move-to-Front Amortized — Hard

```python
class MoveToFrontList:
    """
    Move-to-Front heuristic: when element accessed, move it to front.
    
    Amortized analysis vs optimal static ordering:
    
    Theorem (Sleator-Tarjan 1985): 
    Cost(MTF) ≤ 2·Cost(OPT) + O(n)  [OPT = optimal static ordering]
    
    This means MTF is 2-competitive — within a factor of 2 of optimal!
    
    Potential method proof:
    Φ = Σ_{j precedes i in MTF, j follows i in OPT} 1
      = number of inversions between MTF order and OPT order
    
    For each access of element x:
    - Let k = rank of x in MTF list = actual cost
    - Let k* = rank of x in OPT = OPT's cost  
    - At most k*-1 elements that precede x in OPT also precede x in MTF
      → at most k*-1 inversions removed by moving x to front
    - Moving x to front creates at most k*-1 new inversions (elements before
      x in OPT that were behind x in MTF and now behind x in new MTF position)
    - ΔΦ ≤ (k-1) - (k-k*) = k* - 1  [rough bound]
    - amortized ≤ k + (k* - 1) ≤ 2k* - 1 = 2·OPT_cost
    
    Total: Cost(MTF) ≤ 2·Cost(OPT) + O(n)
    """
    def __init__(self, elements: list):
        self.lst = list(elements)
    
    def access(self, val) -> int:
        """Returns position (1-indexed) and moves element to front."""
        idx = self.lst.index(val)   # O(n) scan
        cost = idx + 1
        self.lst.pop(idx)
        self.lst.insert(0, val)
        return cost
    
    def total_access_cost(self, sequence: list) -> int:
        return sum(self.access(x) for x in sequence)
```

---

## Advanced Variations

### When Amortized Analysis Can FAIL (Invalidation)

```python
def amortized_analysis_pitfalls():
    """
    Critical: amortized bounds are for sequences of operations.
    They CANNOT be used when:
    
    1. Operations are interleaved with "resets" that zero out potential:
       - e.g., serializing a splay tree to disk and reloading
       - The reload costs O(n) but amortized analysis assumed gradual building
    
    2. Worst-case-per-operation guarantee is needed:
       - Real-time systems cannot tolerate O(n) occasional operations
       - Use gap buffers, rope data structures with strict O(log n) per op
    
    3. Parallel/concurrent settings:
       - Another thread can trigger expensive ops while you're "saving credits"
       - Amortized arguments break under interleaving
    
    4. Adversarial settings where adversary chooses operations adaptively:
       - In splay trees, adversary can force O(log n) per op repeatedly
       - But for static access sequences, still O(m log n) total
    """
    pass
```

---

## Edge Cases Bible

1. **Empty data structure**: Potential Φ(D₀) must be defined as 0. If Φ can be negative initially, the bound fails.

2. **Delete operations**: Deletion can decrease potential, but actual cost must be covered. For dynamic arrays, shrinking (halve capacity at 1/4 full) has its own amortized analysis — use threshold of 1/4 (not 1/2) to prevent oscillation.

3. **Mixed workloads**: Amortized analysis is for sequences. If the workload is only "expensive" operations, the actual cost may equal amortized × n (no averaging benefit).

4. **Path compression without union by rank**: O(log n) amortized per operation, NOT O(α(n)). Both optimizations required for inverse-Ackermann bound.

5. **Dynamic array with 1.5× growth**: Still O(1) amortized — the constant changes but the asymptotic bound holds. Growth factor r > 1 is the key.

6. **Fibonacci heap in practice**: Despite O(1) amortized decrease-key, large constants make it slower than binary heap for most practical Dijkstra inputs (typically n < 10^6).

---

## Interview Tips

### What Interviewers Look For

1. **Know which method to use**: If individual ops are clearly O(1) and occasional ops are O(k), accounting/potential is cleaner than aggregate.

2. **State the potential function explicitly**: "My potential is Φ = number of elements in the inbox stack." Shows rigour.

3. **Verify Φ ≥ Φ₀**: The potential must be non-negative (or at least, the drop in potential never exceeds accumulated amortized surplus).

4. **Common trap**: Saying "the average is O(1) because expensive ops are rare" without proving it. The proof requires showing that expensive operations can only follow enough cheap ones to "pay back" their cost.

5. **Real-world connections interviewers love**:
   - Python list `.append()` → dynamic array O(1) amortized
   - Python `dict` / `set` → hash table O(1) amortized (with rehashing)
   - `collections.deque` → NOT amortized, actually O(1) worst-case per append
   - Union-Find in Kruskal's → O(α(n)) amortized makes it nearly linear

6. **Key phrase**: "The total cost of n operations is O(n), so amortized O(1) per operation" — state this explicitly for aggregate method.

7. **Amortized vs expected**: Amortized = worst case over sequence. Expected = average over random coin flips (e.g., randomized quicksort). Do not conflate them.

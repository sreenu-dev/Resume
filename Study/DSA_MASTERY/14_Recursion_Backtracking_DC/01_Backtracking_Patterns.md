# Backtracking Patterns — Advanced Mastery Guide

> **Systematic search with intelligent pruning.** Backtracking is the backbone of constraint satisfaction, combinatorial enumeration, and puzzle solving. Master the template and pruning is the rest.

---

## Table of Contents
1. [The Backtracking Template](#template)
2. [State Space Tree Visualization](#state-space)
3. [Permutations — With and Without Duplicates](#permutations)
4. [Subsets — Power Set and Combinations](#subsets)
5. [N-Queens — Full Solution with Bitmask Pruning](#n-queens)
6. [Sudoku Solver — Constraint Propagation](#sudoku)
7. [Problems 1–10 with Full Solutions](#problems)
8. [Complexity Analysis Framework](#complexity)

---

## 1. The Backtracking Template <a name="template"></a>

```python
def backtrack(state, choices, result, constraints):
    """
    Universal backtracking template.
    
    Three phases: CHOOSE → EXPLORE → UNCHOOSE
    """
    # Base case: valid complete solution
    if is_complete(state):
        result.append(state[:])  # snapshot current state
        return
    
    for choice in get_valid_choices(state, choices, constraints):
        # CHOOSE: make a choice, update state
        make_choice(state, choice)
        
        # EXPLORE: recurse with updated state
        backtrack(state, remaining_choices(choices, choice), result, constraints)
        
        # UNCHOOSE: undo the choice (backtrack!)
        undo_choice(state, choice)

# Key insight: is_complete() and get_valid_choices() define WHAT we search.
# Pruning in get_valid_choices() defines HOW FAST we search.
```

### The Three Pillars

```
1. CHOOSE: Add element to current path (O(1))
2. EXPLORE: Recurse to next decision level
3. UNCHOOSE: Remove element, restore state (O(1))

Invariant: Before and after each backtrack() call, 
           the state is in the same valid configuration.

Pruning: Skip choices that provably cannot lead to valid solutions.
         This is the difference between O(N!) and O(exponentially faster).
```

---

## 2. State Space Tree Visualization <a name="state-space"></a>

For permutations of [1, 2, 3]:

```
                    []
           /        |        \
         [1]       [2]       [3]
        /   \     /   \     /   \
      [1,2] [1,3] [2,1][2,3][3,1][3,2]
       |      |     |     |    |    |
    [1,2,3][1,3,2][2,1,3][2,3,1][3,1,2][3,2,1]
    
Total nodes: 1 + 3 + 6 + 6 = 16 = Σ P(3,k) for k=0..3
Without pruning: visit all. With pruning: skip subtrees early.
```

---

## 3. Permutations — With and Without Duplicates <a name="permutations"></a>

### 3.1 Permutations Without Duplicates (LeetCode 46)

```python
def permutations(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 46. All permutations of distinct elements.
    Time: O(N × N!), Space: O(N) stack depth
    
    Strategy: swap nums[i] with nums[start], recurse, swap back.
    """
    result = []
    
    def backtrack(start: int):
        if start == len(nums):
            result.append(nums[:])
            return
        for i in range(start, len(nums)):
            nums[start], nums[i] = nums[i], nums[start]   # CHOOSE
            backtrack(start + 1)                            # EXPLORE
            nums[start], nums[i] = nums[i], nums[start]   # UNCHOOSE
    
    backtrack(0)
    return result

def permutations_used_flag(nums: list[int]) -> list[list[int]]:
    """Alternative: used-flag approach. Easier to extend."""
    result = []
    used = [False] * len(nums)
    
    def backtrack(path: list):
        if len(path) == len(nums):
            result.append(path[:])
            return
        for i in range(len(nums)):
            if not used[i]:
                used[i] = True
                path.append(nums[i])
                backtrack(path)
                path.pop()
                used[i] = False
    
    backtrack([])
    return result
```

### 3.2 Permutations With Duplicates (LeetCode 47)

```python
def permutations_unique(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 47. Permutations of list that may contain duplicates.
    
    Key pruning: skip duplicate choices at the same recursion level.
    Sort first, then skip nums[i] == nums[i-1] when nums[i-1] not used.
    
    This ensures each unique permutation generated exactly once.
    
    Why? For a pair of duplicates [a, a]:
    - First occurrence: can be used (not previously used)  
    - Second occurrence: only use if first is currently IN the path
    Without this: would generate duplicate permutations.
    """
    nums.sort()
    result = []
    used = [False] * len(nums)
    
    def backtrack(path: list):
        if len(path) == len(nums):
            result.append(path[:])
            return
        for i in range(len(nums)):
            if used[i]:
                continue
            # Skip duplicate: same value as previous, and previous not used
            if i > 0 and nums[i] == nums[i-1] and not used[i-1]:
                continue
            
            used[i] = True
            path.append(nums[i])
            backtrack(path)
            path.pop()
            used[i] = False
    
    backtrack([])
    return result

# Intuition for pruning rule:
# We enforce a canonical ordering: duplicate elements must be chosen
# in left-to-right order. If nums[i-1] is NOT used but nums[i]==nums[i-1],
# it means we're trying to use nums[i] before nums[i-1] (skipping it).
# This would generate the same permutation that we'd get by using nums[i-1] instead.
```

---

## 4. Subsets — Power Set and Combinations <a name="subsets"></a>

### 4.1 Subsets Without Duplicates (LeetCode 78)

```python
def subsets(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 78. All subsets of distinct elements.
    Time: O(N × 2^N), Space: O(N) stack depth
    
    Two approaches: include/exclude at each position.
    """
    result = []
    
    def backtrack(start: int, path: list):
        result.append(path[:])  # every partial path is a valid subset!
        for i in range(start, len(nums)):
            path.append(nums[i])      # CHOOSE
            backtrack(i + 1, path)    # EXPLORE (advance start to avoid reuse)
            path.pop()                 # UNCHOOSE
    
    backtrack(0, [])
    return result

def subsets_bitmask(nums: list[int]) -> list[list[int]]:
    """
    Bitmask enumeration. Elegant for small N.
    Time: O(N × 2^N), Space: O(N × 2^N)
    """
    n = len(nums)
    result = []
    for mask in range(1 << n):
        subset = [nums[i] for i in range(n) if mask & (1 << i)]
        result.append(subset)
    return result
```

### 4.2 Subsets With Duplicates (LeetCode 90)

```python
def subsets_with_dup(nums: list[int]) -> list[list[int]]:
    """
    LeetCode 90.
    Sort first, then skip consecutive duplicates at same recursion level.
    
    When we skip nums[i] == nums[i-1] and i > start:
    We're avoiding choosing the same value twice at the same level.
    (i > start ensures we're talking about a same-level duplicate, not different levels)
    """
    nums.sort()
    result = []
    
    def backtrack(start: int, path: list):
        result.append(path[:])
        for i in range(start, len(nums)):
            # Skip duplicates at same level
            if i > start and nums[i] == nums[i-1]:
                continue
            path.append(nums[i])
            backtrack(i + 1, path)
            path.pop()
    
    backtrack(0, [])
    return result
```

### 4.3 Combination Sum (LeetCode 39, 40)

```python
def combination_sum(candidates: list[int], target: int) -> list[list[int]]:
    """
    LeetCode 39. Elements can be reused. Combinations summing to target.
    Time: O(N^(target/min_candidate)), Space: O(target/min_candidate)
    """
    candidates.sort()  # enables pruning: break early when candidate > remaining
    result = []
    
    def backtrack(start: int, path: list, remaining: int):
        if remaining == 0:
            result.append(path[:])
            return
        for i in range(start, len(candidates)):
            if candidates[i] > remaining:
                break  # PRUNE: sorted, so no larger candidate can work either
            path.append(candidates[i])
            backtrack(i, path, remaining - candidates[i])  # i (not i+1): reuse allowed
            path.pop()
    
    backtrack(0, [], target)
    return result

def combination_sum_ii(candidates: list[int], target: int) -> list[list[int]]:
    """
    LeetCode 40. Each element used at most once. No duplicate combinations.
    Sort + skip same-level duplicates (same pattern as subsets_with_dup).
    """
    candidates.sort()
    result = []
    
    def backtrack(start: int, path: list, remaining: int):
        if remaining == 0:
            result.append(path[:])
            return
        for i in range(start, len(candidates)):
            if candidates[i] > remaining:
                break
            if i > start and candidates[i] == candidates[i-1]:
                continue  # skip same-level duplicates
            path.append(candidates[i])
            backtrack(i + 1, path, remaining - candidates[i])
            path.pop()
    
    backtrack(0, [], target)
    return result
```

---

## 5. N-Queens — Full Solution with Pruning <a name="n-queens"></a>

```python
def solve_n_queens(n: int) -> list[list[str]]:
    """
    LeetCode 51. Place N queens on N×N board, no two queens attack each other.
    
    Pruning: for each row, check column, main diagonal, anti-diagonal conflicts.
    Use sets for O(1) conflict check.
    
    Time: O(N!), Space: O(N) for column tracking
    
    State: queens[row] = column where queen is placed in that row
    (One queen per row guaranteed by row-by-row placement)
    """
    result = []
    cols = set()           # columns occupied by queens
    diag1 = set()          # main diagonals (row - col is constant)
    diag2 = set()          # anti-diagonals (row + col is constant)
    queens = []            # queens[row] = column
    
    def backtrack(row: int):
        if row == n:
            # Build board representation
            board = []
            for r in range(n):
                row_str = '.' * queens[r] + 'Q' + '.' * (n - queens[r] - 1)
                board.append(row_str)
            result.append(board)
            return
        
        for col in range(n):
            if col in cols or (row - col) in diag1 or (row + col) in diag2:
                continue  # PRUNE: column or diagonal conflict
            
            # CHOOSE
            cols.add(col)
            diag1.add(row - col)
            diag2.add(row + col)
            queens.append(col)
            
            # EXPLORE
            backtrack(row + 1)
            
            # UNCHOOSE
            cols.remove(col)
            diag1.remove(row - col)
            diag2.remove(row + col)
            queens.pop()
    
    backtrack(0)
    return result

def solve_n_queens_bitmask(n: int) -> int:
    """
    Count N-Queens solutions using bitmask for O(1) pruning check.
    See Pruning_Techniques.md for the full bitmask optimization.
    Time: O(N!), but constant factor much smaller.
    """
    result = [0]
    full_mask = (1 << n) - 1
    
    def backtrack(row: int, cols: int, diag1: int, diag2: int):
        if row == n:
            result[0] += 1
            return
        
        # Available positions: bits NOT in any conflict set
        available = full_mask & ~(cols | diag1 | diag2)
        
        while available:
            pos = available & (-available)  # lowest set bit = rightmost available column
            available &= available - 1      # remove this bit
            backtrack(
                row + 1,
                cols | pos,
                (diag1 | pos) << 1,   # main diagonal shifts right per row
                (diag2 | pos) >> 1    # anti-diagonal shifts left per row
            )
    
    backtrack(0, 0, 0, 0)
    return result[0]
```

---

## 6. Sudoku Solver — Constraint Propagation + Backtracking <a name="sudoku"></a>

```python
def solve_sudoku(board: list[list[str]]) -> None:
    """
    LeetCode 37. Solve 9×9 Sudoku puzzle.
    
    Constraint propagation: at each cell, valid digits are those
    not in same row, column, or 3×3 box.
    
    Key optimization: choose the cell with MINIMUM valid digits (MRV heuristic).
    
    Time: O(9^(empty cells)) worst case, much faster with MRV.
    Space: O(81) for board + O(81) recursion depth
    """
    rows = [set() for _ in range(9)]
    cols = [set() for _ in range(9)]
    boxes = [set() for _ in range(9)]
    empty = []
    
    # Initialize constraint sets
    for r in range(9):
        for c in range(9):
            if board[r][c] != '.':
                d = int(board[r][c])
                rows[r].add(d)
                cols[c].add(d)
                boxes[(r//3)*3 + c//3].add(d)
            else:
                empty.append((r, c))
    
    def get_valid(r, c):
        box_id = (r//3)*3 + c//3
        return set(range(1, 10)) - rows[r] - cols[c] - boxes[box_id]
    
    def backtrack(idx: int) -> bool:
        if idx == len(empty):
            return True
        
        # MRV heuristic: find empty cell with fewest valid choices
        best_idx = idx
        best_count = 10
        for i in range(idx, len(empty)):
            r, c = empty[i]
            valid = get_valid(r, c)
            if len(valid) < best_count:
                best_count = len(valid)
                best_idx = i
                if best_count == 1:
                    break  # can't do better
        
        if best_count == 0:
            return False  # PRUNE: no valid digit for this cell
        
        # Swap best cell to current position
        empty[idx], empty[best_idx] = empty[best_idx], empty[idx]
        r, c = empty[idx]
        box_id = (r//3)*3 + c//3
        
        for digit in get_valid(r, c):
            # CHOOSE
            board[r][c] = str(digit)
            rows[r].add(digit)
            cols[c].add(digit)
            boxes[box_id].add(digit)
            
            # EXPLORE
            if backtrack(idx + 1):
                return True
            
            # UNCHOOSE
            board[r][c] = '.'
            rows[r].remove(digit)
            cols[c].remove(digit)
            boxes[box_id].remove(digit)
        
        # Restore swap
        empty[idx], empty[best_idx] = empty[best_idx], empty[idx]
        return False
    
    backtrack(0)
```

---

## 7. Problems with Full Solutions <a name="problems"></a>

---

### Problem 1: Word Search in Grid (LeetCode 79)

```python
def exist(board: list[list[str]], word: str) -> bool:
    """
    LeetCode 79. Find if word exists in 2D grid via adjacent cells.
    Time: O(M × N × 4^L) where L = word length (very loose upper bound)
    Space: O(L) recursion depth
    """
    rows, cols = len(board), len(board[0])
    
    def backtrack(r: int, c: int, idx: int) -> bool:
        if idx == len(word):
            return True
        if not (0 <= r < rows and 0 <= c < cols):
            return False
        if board[r][c] != word[idx]:
            return False
        
        # CHOOSE: mark visited
        temp = board[r][c]
        board[r][c] = '#'
        
        # EXPLORE: try all 4 directions
        found = (backtrack(r+1, c, idx+1) or backtrack(r-1, c, idx+1) or
                 backtrack(r, c+1, idx+1) or backtrack(r, c-1, idx+1))
        
        # UNCHOOSE
        board[r][c] = temp
        return found
    
    for r in range(rows):
        for c in range(cols):
            if backtrack(r, c, 0):
                return True
    return False
```

---

### Problem 2: Palindrome Partitioning (LeetCode 131)

```python
def partition_palindrome(s: str) -> list[list[str]]:
    """
    LeetCode 131. Partition s so every substring is a palindrome.
    
    Optimization: precompute palindrome check with DP.
    Time: O(N × 2^N) worst case
    """
    n = len(s)
    
    # Precompute: is_pal[i][j] = s[i:j+1] is palindrome
    is_pal = [[False] * n for _ in range(n)]
    for i in range(n):
        is_pal[i][i] = True
    for length in range(2, n + 1):
        for i in range(n - length + 1):
            j = i + length - 1
            if s[i] == s[j]:
                is_pal[i][j] = (length == 2) or is_pal[i+1][j-1]
    
    result = []
    
    def backtrack(start: int, path: list):
        if start == n:
            result.append(path[:])
            return
        for end in range(start, n):
            if is_pal[start][end]:
                path.append(s[start:end+1])
                backtrack(end + 1, path)
                path.pop()
    
    backtrack(0, [])
    return result
```

---

### Problem 3: Generate Parentheses (LeetCode 22)

```python
def generate_parenthesis_backtrack(n: int) -> list[str]:
    """
    LeetCode 22. Generate all valid parenthesizations.
    Constraint: open ≤ n, close ≤ open (ensures validity).
    
    Pruning is built into the constraints:
    - Can't add '(' if open == n
    - Can't add ')' if close == open
    
    Time: O(Catalan(n) × N) = O(4^N / N^1.5 × N)
    """
    result = []
    
    def backtrack(path: list, open: int, close: int):
        if len(path) == 2 * n:
            result.append(''.join(path))
            return
        if open < n:
            path.append('(')
            backtrack(path, open + 1, close)
            path.pop()
        if close < open:
            path.append(')')
            backtrack(path, open, close + 1)
            path.pop()
    
    backtrack([], 0, 0)
    return result
```

---

### Problem 4: Letter Combinations of Phone Number (LeetCode 17)

```python
def letter_combinations(digits: str) -> list[str]:
    """
    LeetCode 17. All letter combinations from phone keypad.
    Time: O(4^N × N), Space: O(N) depth
    """
    if not digits:
        return []
    
    keypad = {
        '2': 'abc', '3': 'def', '4': 'ghi', '5': 'jkl',
        '6': 'mno', '7': 'pqrs', '8': 'tuv', '9': 'wxyz'
    }
    result = []
    
    def backtrack(idx: int, path: list):
        if idx == len(digits):
            result.append(''.join(path))
            return
        for char in keypad[digits[idx]]:
            path.append(char)
            backtrack(idx + 1, path)
            path.pop()
    
    backtrack(0, [])
    return result
```

---

### Problem 5: Restore IP Addresses (LeetCode 93)

```python
def restore_ip_addresses(s: str) -> list[str]:
    """
    LeetCode 93. Split s into 4 valid octets (0-255, no leading zeros).
    
    Pruning:
    - Each octet: 1-3 digits
    - No leading zeros
    - Value 0-255
    - Remaining string must have enough chars for remaining octets
    
    Time: O(1) — at most 3^4 = 81 possibilities
    """
    result = []
    
    def backtrack(start: int, parts: list):
        if len(parts) == 4:
            if start == len(s):
                result.append('.'.join(parts))
            return
        
        remaining_parts = 4 - len(parts)
        remaining_chars = len(s) - start
        
        # Pruning: remaining chars must be within valid range
        if not (remaining_parts <= remaining_chars <= 3 * remaining_parts):
            return
        
        for length in range(1, 4):
            if start + length > len(s):
                break
            segment = s[start:start + length]
            
            # Prune: no leading zeros, value <= 255
            if len(segment) > 1 and segment[0] == '0':
                break
            if int(segment) > 255:
                break
            
            parts.append(segment)
            backtrack(start + length, parts)
            parts.pop()
    
    backtrack(0, [])
    return result
```

---

### Problem 6: Expression Add Operators (LeetCode 282)

```python
def add_operators(num: str, target: int) -> list[str]:
    """
    LeetCode 282. Insert +, -, * operators between digits to reach target.
    
    Key: track last term for multiplication (undo last addition, multiply, re-add).
    Time: O(N × 4^N) — N digits, 4 choices per gap (no op, +, -, *)
    """
    result = []
    n = len(num)
    
    def backtrack(idx: int, path: str, value: int, last: int):
        """
        idx: current position in num
        path: expression string so far
        value: current evaluated value
        last: last term added (needed for multiplication)
        """
        if idx == n:
            if value == target:
                result.append(path)
            return
        
        for end in range(idx + 1, n + 1):
            segment = num[idx:end]
            
            # Prune: no leading zeros (except "0" itself)
            if len(segment) > 1 and segment[0] == '0':
                break
            
            curr = int(segment)
            
            if idx == 0:
                # First number: no operator prefix
                backtrack(end, segment, curr, curr)
            else:
                # Addition
                backtrack(end, path + '+' + segment, value + curr, curr)
                # Subtraction
                backtrack(end, path + '-' + segment, value - curr, -curr)
                # Multiplication: undo last term, multiply, re-add
                backtrack(end, path + '*' + segment, 
                         value - last + last * curr, last * curr)
    
    backtrack(0, '', 0, 0)
    return result
```

---

### Problem 7: Combination Sum III (LeetCode 216)

```python
def combination_sum_iii(k: int, n: int) -> list[list[int]]:
    """
    LeetCode 216. Find k numbers summing to n, each 1-9 used at most once.
    Excellent pruning example.
    
    Time: O(C(9, k)), Space: O(k)
    """
    result = []
    
    def backtrack(start: int, path: list, remaining: int):
        if len(path) == k and remaining == 0:
            result.append(path[:])
            return
        if len(path) == k or remaining <= 0:
            return  # PRUNE: already filled or exceeded
        
        for num in range(start, 10):
            if num > remaining:
                break  # PRUNE: sorted, so larger won't work
            path.append(num)
            backtrack(num + 1, path, remaining - num)
            path.pop()
    
    backtrack(1, [], n)
    return result
```

---

### Problem 8: Factor Combinations (LeetCode 254)

```python
def get_factors(n: int) -> list[list[int]]:
    """
    LeetCode 254. Find all factor combinations of n (factors > 1).
    
    Time: O(N^(log N)) — bounded by number of factorizations
    """
    result = []
    
    def backtrack(remaining: int, min_factor: int, path: list):
        # Add current combination (if more than just [n])
        if path:
            result.append(path + [remaining])
        
        factor = min_factor
        while factor * factor <= remaining:
            if remaining % factor == 0:
                path.append(factor)
                backtrack(remaining // factor, factor, path)
                path.pop()
            factor += 1
    
    backtrack(n, 2, [])
    return result

# n=12: [[2,6],[2,2,3],[3,4]]
```

---

### Problem 9: Word Break with Backtracking (LeetCode 140)

```python
def word_break_backtrack(s: str, word_dict: list[str]) -> list[str]:
    """
    LeetCode 140. All sentences using words from dictionary.
    Memoization + backtracking.
    Time: O(N² + output size), Space: O(N²)
    """
    from functools import lru_cache
    
    word_set = set(word_dict)
    n = len(s)
    
    @lru_cache(maxsize=None)
    def backtrack(start: int) -> list[str]:
        """Returns all sentences for s[start:]."""
        if start == n:
            return ['']
        
        results = []
        for end in range(start + 1, n + 1):
            word = s[start:end]
            if word in word_set:
                for rest in backtrack(end):
                    sentence = word + (' ' + rest if rest else '')
                    results.append(sentence)
        return results
    
    return backtrack(0)
```

---

### Problem 10: Path with Maximum Gold (LeetCode 1219)

```python
def get_maximum_gold(grid: list[list[int]]) -> int:
    """
    LeetCode 1219. DFS backtracking to collect maximum gold.
    Cannot revisit cells, stop at 0-value cells.
    
    Time: O(M × N × 4^(M×N)) — very loose; cells with 0 stop exploration
    """
    rows, cols = len(grid), len(grid[0])
    
    def dfs(r: int, c: int) -> int:
        if not (0 <= r < rows and 0 <= c < cols) or grid[r][c] == 0:
            return 0
        
        gold = grid[r][c]
        grid[r][c] = 0  # CHOOSE: mark visited
        
        max_gold = max(
            dfs(r+1, c), dfs(r-1, c), dfs(r, c+1), dfs(r, c-1)
        )
        
        grid[r][c] = gold  # UNCHOOSE: restore
        return gold + max_gold
    
    return max(
        dfs(r, c) 
        for r in range(rows) 
        for c in range(cols) 
        if grid[r][c] != 0
    )
```

---

## 8. Complexity Analysis Framework <a name="complexity"></a>

### State Space Size Estimates

| Problem | State Space | With Pruning |
|---------|-------------|-------------|
| Permutations (N) | N! | N! (all valid) |
| Subsets (N) | 2^N | 2^N (all valid) |
| Combinations (N choose K) | C(N,K) | C(N,K) |
| N-Queens | N^N | ~N!/2^N (roughly) |
| Sudoku | 9^81 | ~<10^12 with pruning |
| Palindrome partition | 2^N | Much less with precomputed DP |

### Time Complexity Formula

```
Time = (nodes in search tree) × (work per node)
     = (branching_factor)^(depth) × (work per node)

With pruning:
     = (effective_branching_factor)^(depth) × (work per node)

Key: effective_branching_factor << branching_factor after good pruning.
```

### Universal Pruning Checklist

```
1. Bound check: Can current path possibly reach a valid solution?
   - Remaining sum too large/small
   - Remaining length insufficient
   
2. Duplicate avoidance: Skip choices that would repeat previous work.
   - Sort + skip same-level duplicates
   
3. Constraint propagation: If current choice violates constraint, prune.
   - Sudoku: cell with 0 valid digits → prune
   - N-Queens: column/diagonal conflict → prune
   
4. Ordering: Try most promising choices first (fail-fast).
   - MRV: cell with fewest options first
   - Most constrained variable heuristic
```

### Interview Template for Backtracking Questions

```python
# Step 1: Identify choices at each step
# Step 2: Identify base cases (when to record solution)
# Step 3: Identify constraints (when to prune)
# Step 4: Implement choose/unchoose symmetrically

def solve(problem_input):
    result = []
    
    def backtrack(state):
        if base_case(state):    # Step 2
            record(result, state)
            return
        for choice in choices(state):   # Step 1
            if violates_constraint(state, choice):  # Step 3
                continue
            apply(state, choice)         # Choose
            backtrack(update(state, choice))  # Explore
            undo(state, choice)          # Unchoose (Step 4)
    
    backtrack(initial_state())
    return result
```

---

*Previous: [Matrix Exponentiation ←](../13_Mathematical_Algorithms/04_Matrix_Exponentiation.md) | Next: [Pruning Techniques →](02_Pruning_Techniques.md)*

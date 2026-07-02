# 2D DP and Grid Problems
## Advanced Mastery — FAANG Interview Preparation

> **Focus:** State design for 2D problems, simultaneous multi-agent DP, and the critical directional insight (why some grid DPs must go backward).

---

## Table of Contents
1. [Unique Paths — Grid Path Counting](#1-unique-paths--grid-path-counting)
2. [Minimum Path Sum](#2-minimum-path-sum)
3. [Triangle — Variable Width Grid](#3-triangle-minimum-path)
4. [Maximal Square — The Min-of-3-Neighbors Insight](#4-maximal-square)
5. [Maximal Rectangle — Histogram DP](#5-maximal-rectangle)
6. [Dungeon Game — Why Backward DP](#6-dungeon-game)
7. [Cherry Pickup I — Two Agents on Grid](#7-cherry-pickup-i)
8. [Cherry Pickup II — Simultaneous Traversal](#8-cherry-pickup-ii)
9. [Paint House I and II — Rolling Array](#9-paint-house)
10. [Out of Boundary Paths](#10-out-of-boundary-paths)
11. [Minimum Falling Path Sum](#11-minimum-falling-path-sum)

---

## 1. Unique Paths — Grid Path Counting

**Problem:** [LC 62] An M×N grid. You start top-left and must reach bottom-right, moving only right or down. Count total paths.

### State Definition

```
dp[i][j] = number of distinct paths from (0,0) to (i,j)
```

### Recurrence

Every cell can be reached only from the cell above or the cell to the left:
```
dp[i][j] = dp[i-1][j] + dp[i][j-1]
```

Base cases: `dp[0][j] = 1` for all `j` (only one way along top row) and `dp[i][0] = 1` for all `i` (only one way along left column).

```python
def unique_paths(m: int, n: int) -> int:
    # Space-optimized: only one row needed
    dp = [1] * n  # top row all 1s
    
    for i in range(1, m):
        for j in range(1, n):
            dp[j] += dp[j-1]  # dp[j] = above (old dp[j]) + left (dp[j-1])
    
    return dp[n-1]

print(unique_paths(3, 7))   # 28
print(unique_paths(3, 2))   # 3
```

> **Time:** O(M×N) | **Space:** O(N) — rolling row

**Mathematical shortcut:** C(m+n-2, m-1) — choosing which m-1 of m+n-2 steps go down.

### Unique Paths II — With Obstacles

**State:** Same, but `dp[i][j] = 0` if `obstacleGrid[i][j] == 1`.

```python
def unique_paths_with_obstacles(obstacleGrid: list[list[int]]) -> int:
    m, n = len(obstacleGrid), len(obstacleGrid[0])
    if obstacleGrid[0][0] == 1 or obstacleGrid[m-1][n-1] == 1:
        return 0
    
    dp = [0] * n
    dp[0] = 1  # Start cell
    
    for i in range(m):
        for j in range(n):
            if obstacleGrid[i][j] == 1:
                dp[j] = 0  # obstacle blocks all paths
            elif j > 0:
                dp[j] += dp[j-1]  # add paths from left
    
    return dp[n-1]
```

> **Time:** O(M×N) | **Space:** O(N)

---

## 2. Minimum Path Sum

**Problem:** [LC 64] Find the path from top-left to bottom-right minimizing the sum of all numbers along the path (move right or down only).

### State Definition

```
dp[i][j] = minimum sum to reach cell (i,j) from (0,0)
```

### Recurrence

```
dp[i][j] = grid[i][j] + min(dp[i-1][j], dp[i][j-1])
```

Special handling for borders (only one direction possible).

```python
def min_path_sum(grid: list[list[int]]) -> int:
    m, n = len(grid), len(grid[0])
    
    # In-place modification for O(1) extra space
    # Initialize top row
    for j in range(1, n):
        grid[0][j] += grid[0][j-1]
    
    # Initialize left column
    for i in range(1, m):
        grid[i][0] += grid[i-1][0]
    
    for i in range(1, m):
        for j in range(1, n):
            grid[i][j] += min(grid[i-1][j], grid[i][j-1])
    
    return grid[m-1][n-1]

print(min_path_sum([[1,3,1],[1,5,1],[4,2,1]]))  # 7 (1→3→1→1→1)
```

> **Time:** O(M×N) | **Space:** O(1) — in-place (O(N) if we use rolling row without modifying input)

---

## 3. Triangle — Minimum Path

**Problem:** [LC 120] Given a triangle array, find the minimum path sum from top to bottom. Each step moves to an adjacent number in the row below.

### Key Insight: Bottom-Up is Cleaner

Working bottom-up, `dp[i]` represents the minimum path from row `i` to the bottom, and we overwrite in-place.

```python
def minimum_total(triangle: list[list[int]]) -> int:
    n = len(triangle)
    # Bottom-up: start from second-to-last row
    dp = triangle[-1][:]  # copy the bottom row
    
    for i in range(n - 2, -1, -1):
        for j in range(i + 1):  # row i has i+1 elements
            dp[j] = triangle[i][j] + min(dp[j], dp[j+1])
    
    return dp[0]

print(minimum_total([[2],[3,4],[6,5,7],[4,1,8,3]]))  # 11 (2→3→5→1)
```

> **Time:** O(N²) | **Space:** O(N) — dp array same size as bottom row

---

## 4. Maximal Square

**Problem:** [LC 221] In an M×N matrix of '0's and '1's, find the area of the largest square containing only '1's.

### The Key Insight: Why `min(3 neighbors) + 1`

Let `dp[i][j]` = side length of the largest square whose **bottom-right corner** is at (i,j).

For cell (i,j) to extend a square, its three neighbors must all have valid squares:
- `dp[i-1][j]`: square ending above
- `dp[i][j-1]`: square ending to the left
- `dp[i-1][j-1]`: square ending diagonally

The **bottleneck** is the smallest of the three. A square of side `s` can form at (i,j) only if all three neighbors allow at least side `s-1`.

```
dp[i][j] = min(dp[i-1][j], dp[i][j-1], dp[i-1][j-1]) + 1  if matrix[i][j] == '1'
dp[i][j] = 0                                                 if matrix[i][j] == '0'
```

**Visual proof:**
```
If we want a 3×3 square at (i,j):
  dp[i-1][j-1] >= 2  (can form 2×2 at top-left of our desired 3×3)
  dp[i-1][j]   >= 2  (can form 2×2 at top-right)
  dp[i][j-1]   >= 2  (can form 2×2 at bottom-left)
All three together guarantee the 3×3 is all 1s.
```

```python
def maximal_square(matrix: list[list[str]]) -> int:
    if not matrix or not matrix[0]:
        return 0
    
    m, n = len(matrix), len(matrix[0])
    max_side = 0
    
    # Space-optimized: two rows
    prev = [0] * (n + 1)
    
    for i in range(m):
        curr = [0] * (n + 1)
        for j in range(1, n + 1):
            if matrix[i][j-1] == '1':
                curr[j] = min(prev[j], curr[j-1], prev[j-1]) + 1
                max_side = max(max_side, curr[j])
            else:
                curr[j] = 0
        prev = curr
    
    return max_side * max_side

matrix = [
    ["1","0","1","0","0"],
    ["1","0","1","1","1"],
    ["1","1","1","1","1"],
    ["1","0","0","1","0"]
]
print(maximal_square(matrix))  # 4 (2×2 square)
```

> **Time:** O(M×N) | **Space:** O(N) — two-row rolling

---

## 5. Maximal Rectangle

**Problem:** [LC 85] Same matrix as above, find the largest rectangle containing only '1's (not restricted to squares).

### Approach: Histogram DP Row by Row

Build a "height histogram" for each row: `heights[j]` = consecutive 1s ending at row `i` in column `j`. Then find the largest rectangle in that histogram.

**Largest Rectangle in Histogram** [LC 84]:  
For each bar, find how far left and right it can extend (using a monotonic stack): width = (right_boundary - left_boundary - 1), area = height × width.

```python
def maximal_rectangle(matrix: list[list[str]]) -> int:
    if not matrix or not matrix[0]:
        return 0
    
    n = len(matrix[0])
    heights = [0] * n
    max_area = 0
    
    def largest_rect_in_histogram(h: list[int]) -> int:
        stack = []  # monotonic increasing stack of indices
        max_rect = 0
        
        for i in range(len(h) + 1):
            curr_h = h[i] if i < len(h) else 0  # sentinel 0 at end
            while stack and h[stack[-1]] > curr_h:
                height = h[stack.pop()]
                width = i if not stack else i - stack[-1] - 1
                max_rect = max(max_rect, height * width)
            stack.append(i)
        
        return max_rect
    
    for row in matrix:
        for j in range(n):
            heights[j] = heights[j] + 1 if row[j] == '1' else 0
        max_area = max(max_area, largest_rect_in_histogram(heights))
    
    return max_area

matrix2 = [
    ["1","0","1","0","0"],
    ["1","0","1","1","1"],
    ["1","1","1","1","1"],
    ["1","0","0","1","0"]
]
print(maximal_rectangle(matrix2))  # 6
```

> **Time:** O(M×N) — M rows × O(N) histogram computation each row  
> **Space:** O(N) — heights array + stack

---

## 6. Dungeon Game

**Problem:** [LC 174] A dungeon grid where each cell has a value (negative = damage, positive = health). A knight starts top-left, must reach bottom-right. At each step, health must stay > 0. Find the minimum initial health.

### Why Backward (Bottom-Right → Top-Left)?

The constraint is **future-dependent**: we need enough health to survive everything ahead. If we go forward, at each cell we don't know how much health we'll need later.

Going backward: `dp[i][j]` = minimum health required **when entering** cell (i,j) to survive from (i,j) to the end.

### State Definition

```
dp[i][j] = minimum health needed at the ENTRY to cell (i,j) to reach the princess
```

### Recurrence (Backward)

```
# Best next cell: min health needed entering (i+1,j) or (i,j+1)
next_min_hp = min(dp[i+1][j], dp[i][j+1])

# After collecting dungeon[i][j], we need at least 1 health
# So: dp[i][j] + dungeon[i][j] >= next_min_hp
# => dp[i][j] = next_min_hp - dungeon[i][j]
# But health can never drop to 0, so dp[i][j] >= 1:
dp[i][j] = max(1, next_min_hp - dungeon[i][j])

Base case (bottom-right):
dp[m-1][n-1] = max(1, 1 - dungeon[m-1][n-1])
```

```python
def calculate_minimum_hp(dungeon: list[list[int]]) -> int:
    m, n = len(dungeon), len(dungeon[0])
    
    # Process from bottom-right to top-left
    # Add padding row and column of infinity for clean border handling
    INF = float('inf')
    dp = [[INF] * (n + 1) for _ in range(m + 1)]
    dp[m][n-1] = dp[m-1][n] = 1  # virtual cells next to bottom-right
    
    for i in range(m - 1, -1, -1):
        for j in range(n - 1, -1, -1):
            need = min(dp[i+1][j], dp[i][j+1]) - dungeon[i][j]
            dp[i][j] = max(1, need)
    
    return dp[0][0]

dungeon = [[-2,-3,3],[-5,-10,1],[10,30,-5]]
print(calculate_minimum_hp(dungeon))  # 7
```

> **Time:** O(M×N) | **Space:** O(M×N) — reducible to O(N) with rolling rows

**Why NOT forward DP here:**  
A forward DP `dp[i][j] = max health when leaving cell (i,j)` doesn't capture the constraint that health must always remain positive. We'd need to track the minimum health at every step, which is state-dependent on the path — the Markov property fails for forward direction.

---

## 7. Cherry Pickup I — One Trip, Two Ways

**Problem:** [LC 741] Grid with cherries. Pick all cherries by going from (0,0) to (N-1,N-1) and then back, maximizing cherries (cherry disappears after first pick).

### Key Insight: Simulate Two People Going Forward

Rather than one person going and coming back, model as **two people simultaneously going from (0,0) to (N-1,N-1)**. They're on the same step `t`, so `r1 + c1 = r2 + c2 = t`.

### State Definition (3D)

```
dp[t][r1][r2] = max cherries collected when both people are at step t,
                person 1 is at row r1 (so col1 = t - r1),
                person 2 is at row r2 (so col2 = t - r2)
```

Since `c = t - r`, we only need `(t, r1, r2)` to determine both positions. State space: O(N³).

```python
def cherry_pickup(grid: list[list[int]]) -> int:
    n = len(grid)
    BLOCKED = -1
    
    from functools import lru_cache
    
    @lru_cache(maxsize=None)
    def dp(t, r1, r2):
        c1, c2 = t - r1, t - r2
        
        # Boundary checks
        if r1 >= n or r2 >= n or c1 >= n or c2 >= n:
            return float('-inf')
        if grid[r1][c1] == BLOCKED or grid[r2][c2] == BLOCKED:
            return float('-inf')
        
        # Base case: both at destination
        if r1 == n-1 and c1 == n-1:
            return grid[n-1][n-1]
        
        # Collect cherries (don't double count if at same cell)
        cherries = grid[r1][c1]
        if r1 != r2:
            cherries += grid[r2][c2]
        
        # Try all 4 combinations: (person1 moves down or right) × (person2 moves down or right)
        best = float('-inf')
        for dr1 in [0, 1]:  # 0=right, 1=down
            for dr2 in [0, 1]:
                best = max(best, dp(t+1, r1+dr1, r2+dr2))
        
        return cherries + best
    
    result = dp(0, 0, 0)
    return max(0, result)

print(cherry_pickup([[0,1,-1],[1,0,-1],[1,1,1]]))  # 0 (can't avoid -1 on return)
print(cherry_pickup([[1,1,1,1,0,0,0],[0,0,0,1,0,0,0],[0,0,0,1,0,0,1],[1,0,0,1,0,0,0],[0,0,0,0,0,0,1],[0,0,0,0,0,0,0],[0,0,0,0,0,0,0]]))  # 15
```

> **Time:** O(N³) — states: N steps × N choices for r1 × N choices for r2  
> **Space:** O(N³) — memoization cache

---

## 8. Cherry Pickup II — Two Robots Simultaneously

**Problem:** [LC 1444] An M×N grid. Two robots start at row 0: robot 1 at column 0, robot 2 at column N-1. They move to the next row each step (can go diagonally or straight down). Collect max cherries (no double counting at same cell).

### State Definition

```
dp[row][c1][c2] = max cherries collected from row 0..row,
                  robot 1 at column c1, robot 2 at column c2
```

Since both robots are always at the same row, we use top-down DP reducing to `dp[c1][c2]` at each row.

```python
def cherry_pickup_ii(grid: list[list[int]]) -> int:
    m, n = len(grid), len(grid[0])
    
    from functools import lru_cache
    
    @lru_cache(maxsize=None)
    def dp(row, c1, c2):
        # Collect cherries at current row
        cherries = grid[row][c1] + (grid[row][c2] if c1 != c2 else 0)
        
        if row == m - 1:
            return cherries
        
        # Try all 9 combinations of moves for both robots
        best = float('-inf')
        for dc1 in [-1, 0, 1]:
            for dc2 in [-1, 0, 1]:
                nc1, nc2 = c1 + dc1, c2 + dc2
                if 0 <= nc1 < n and 0 <= nc2 < n:
                    best = max(best, dp(row + 1, nc1, nc2))
        
        return cherries + best
    
    return dp(0, 0, n - 1)

grid2 = [[3,1,1],[2,5,1],[1,5,5],[2,1,1]]
print(cherry_pickup_ii(grid2))  # 24
```

> **Time:** O(M × N² × 9) = O(M × N²) | **Space:** O(M × N²) memoization

**Bottom-up version for interview clarity:**

```python
def cherry_pickup_ii_bottomup(grid: list[list[int]]) -> int:
    m, n = len(grid), len(grid[0])
    NEG_INF = float('-inf')
    
    # dp[c1][c2] = best cherries when robots at (row, c1) and (row, c2)
    # Initialize for last row
    dp = [[NEG_INF] * n for _ in range(n)]
    for c1 in range(n):
        for c2 in range(n):
            dp[c1][c2] = grid[m-1][c1] + (grid[m-1][c2] if c1 != c2 else 0)
    
    for row in range(m - 2, -1, -1):
        new_dp = [[NEG_INF] * n for _ in range(n)]
        for c1 in range(n):
            for c2 in range(n):
                cherries = grid[row][c1] + (grid[row][c2] if c1 != c2 else 0)
                best = NEG_INF
                for dc1 in [-1, 0, 1]:
                    for dc2 in [-1, 0, 1]:
                        nc1, nc2 = c1 + dc1, c2 + dc2
                        if 0 <= nc1 < n and 0 <= nc2 < n and dp[nc1][nc2] != NEG_INF:
                            best = max(best, dp[nc1][nc2])
                if best != NEG_INF:
                    new_dp[c1][c2] = cherries + best
        dp = new_dp
    
    return dp[0][n-1]
```

> **Time:** O(M × N² × 9) | **Space:** O(N²) — rolling two 2D layers

---

## 9. Paint House I and II

### Paint House I — 3 Colors

**Problem:** [LC 256] Paint `n` houses with 3 colors. `cost[i][j]` = cost to paint house `i` with color `j`. Adjacent houses cannot have same color. Minimize total cost.

### State Definition

```
dp[i][c] = minimum cost to paint houses 0..i, with house i painted color c (c in {0,1,2})
```

```python
def min_cost_paint(costs: list[list[int]]) -> int:
    if not costs:
        return 0
    
    # Rolling: only track previous row
    prev = costs[0][:]  # [cost_red, cost_blue, cost_green]
    
    for i in range(1, len(costs)):
        curr = [0] * 3
        curr[0] = costs[i][0] + min(prev[1], prev[2])
        curr[1] = costs[i][1] + min(prev[0], prev[2])
        curr[2] = costs[i][2] + min(prev[0], prev[1])
        prev = curr
    
    return min(prev)
```

> **Time:** O(N) | **Space:** O(1)

### Paint House II — K Colors

**Problem:** [LC 265] Same but `k` colors. Naively O(NK²). Optimize to O(NK).

**Key insight:** For each house `i`, we only need the minimum and second minimum from the previous row. Then for each color `c`:
- If `c` is not the color that achieved the global minimum, use the global minimum.
- If `c` IS that color, use the second minimum.

```python
def min_cost_ii(costs: list[list[int]]) -> int:
    if not costs:
        return 0
    
    n, k = len(costs), len(costs[0])
    
    # Track min1 (smallest), min2 (second smallest), and idx of min1
    min1 = min2 = 0
    min1_idx = -1
    
    for i in range(n):
        new_min1 = new_min2 = float('inf')
        new_min1_idx = -1
        
        for c in range(k):
            # Cost for house i with color c
            if c == min1_idx:
                cost = costs[i][c] + min2
            else:
                cost = costs[i][c] + min1
            
            if cost < new_min1:
                new_min2 = new_min1
                new_min1 = cost
                new_min1_idx = c
            elif cost < new_min2:
                new_min2 = cost
        
        min1, min2, min1_idx = new_min1, new_min2, new_min1_idx
    
    return min1

print(min_cost_ii([[1,5,3],[2,9,4]]))  # 5
```

> **Time:** O(N × K) | **Space:** O(1)

---

## 10. Out of Boundary Paths

**Problem:** [LC 576] M×N grid. Ball starts at (startRow, startCol). In exactly N moves, count paths that take the ball out of the boundary. Answer mod 10^9+7.

### State Definition

```
dp[moves][i][j] = number of paths from (i,j) with exactly `moves` moves remaining that exit
```

Process backward: with 0 moves, can't exit. With k moves from (i,j), try all 4 directions.

```python
def find_paths(m: int, n: int, max_move: int, start_row: int, start_col: int) -> int:
    MOD = 10**9 + 7
    
    # dp[i][j] = number of ways to exit from (i,j) with current number of moves
    curr = [[0] * n for _ in range(m)]
    curr[start_row][start_col] = 1
    
    result = 0
    dirs = [(0,1),(0,-1),(1,0),(-1,0)]
    
    for _ in range(max_move):
        next_dp = [[0] * n for _ in range(m)]
        for i in range(m):
            for j in range(n):
                if curr[i][j] == 0:
                    continue
                for di, dj in dirs:
                    ni, nj = i + di, j + dj
                    if 0 <= ni < m and 0 <= nj < n:
                        next_dp[ni][nj] = (next_dp[ni][nj] + curr[i][j]) % MOD
                    else:
                        result = (result + curr[i][j]) % MOD
        curr = next_dp
    
    return result

print(find_paths(2, 2, 2, 0, 0))   # 6
print(find_paths(1, 3, 3, 0, 1))   # 12
```

> **Time:** O(max_move × M × N) | **Space:** O(M × N) — two layers

---

## 11. Minimum Falling Path Sum

**Problem:** [LC 931] Choose a path from any element in the first row to any element in the last row. You can move to the same column, one left, or one right in the next row. Minimize sum.

```python
def min_falling_path_sum(matrix: list[list[int]]) -> int:
    n = len(matrix)
    
    # Process in-place (or use rolling row)
    for i in range(1, n):
        for j in range(n):
            best_above = matrix[i-1][j]
            if j > 0:
                best_above = min(best_above, matrix[i-1][j-1])
            if j < n-1:
                best_above = min(best_above, matrix[i-1][j+1])
            matrix[i][j] += best_above
    
    return min(matrix[n-1])

print(min_falling_path_sum([[2,1,3],[6,5,4],[7,8,9]]))  # 13
print(min_falling_path_sum([[-19,57],[-40,-5]]))  # -59
```

> **Time:** O(N²) | **Space:** O(1) in-place

### Minimum Falling Path Sum II — Non-Adjacent Column

**Problem:** [LC 1289] Same but next row pick cannot be in the same column. Naively O(N³); optimize to O(N²) using min/second-min trick.

```python
def min_falling_path_sum_ii(grid: list[list[int]]) -> int:
    n = len(grid)
    
    # Track min1, min2, and index of min1 for each row
    def get_top2(row):
        min1 = min2 = float('inf')
        min1_idx = -1
        for j, val in enumerate(row):
            if val < min1:
                min2 = min1; min1 = val; min1_idx = j
            elif val < min2:
                min2 = val
        return min1, min2, min1_idx
    
    prev = grid[0][:]
    
    for i in range(1, n):
        min1, min2, min1_idx = get_top2(prev)
        curr = []
        for j in range(n):
            # Can't come from same column j
            best_prev = min2 if j == min1_idx else min1
            curr.append(grid[i][j] + best_prev)
        prev = curr
    
    return min(prev)

print(min_falling_path_sum_ii([[1,2,3],[4,5,6],[7,8,9]]))  # 13
```

> **Time:** O(N²) | **Space:** O(N)

---

## Summary: 2D DP Problem Design Cards

| Problem | State | Fill Direction | Key Insight |
|---|---|---|---|
| Unique Paths | `dp[i][j]` = paths to (i,j) | Top→Bottom, Left→Right | Sum from above + left |
| Min Path Sum | `dp[i][j]` = min cost to (i,j) | Top→Bottom | min(above, left) + cost |
| Triangle | `dp[i]` = min from row i down | Bottom→Top | overwrite in-place |
| Maximal Square | `dp[i][j]` = side at (i,j) | Top→Bottom | `min(3 neighbors) + 1` |
| Maximal Rectangle | histogram heights | Row by row | stack-based histogram |
| Dungeon Game | `dp[i][j]` = min hp needed | Bottom-Right→Top-Left | constraints are future-facing |
| Cherry Pickup I | `dp[t][r1][r2]` | Step by step | 2 people = 3D state |
| Cherry Pickup II | `dp[row][c1][c2]` | Top→Bottom | same row both robots |
| Paint House II | min1/min2 trick | Row by row | avoid O(NK²) |
| Out of Boundary | `dp[moves][i][j]` | Move by move | BFS-style DP |

---

## Advanced Pattern: Grid DP with Two Pointers Merging

When two entities traverse the same grid and can share cells, the state dimension increases. General pattern:

```python
# Two-agent grid DP template
# Both agents at same step t, positions (r1, c1) and (r2, c2)
# Since c = t - r, state is just (t, r1, r2)

@lru_cache(maxsize=None)
def solve(t, r1, r2):
    c1, c2 = t - r1, t - r2
    # validate bounds and obstacles
    
    # collect, avoiding double-count
    collect = grid[r1][c1] + (grid[r2][c2] if (r1,c1) != (r2,c2) else 0)
    
    # try all move combinations
    best = max(solve(t+1, r1+dr1, r2+dr2)
               for dr1 in moves for dr2 in moves
               if valid(t+1, r1+dr1, r2+dr2))
    
    return collect + best
```

This pattern generalizes to K agents at cost O(N^(K+1)) states.

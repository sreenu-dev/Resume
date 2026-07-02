# Tree DP — Complete Mastery Guide
## Advanced FAANG Interview Preparation

> **Core Pattern:** Define DP state on subtrees. Compute children first (post-order DFS), then combine children results at the parent. Rerooting allows O(N) second-pass for problems needing all nodes as root.

---

## Table of Contents
1. [Tree DP Template — Post-order DFS](#1-tree-dp-template)
2. [House Robber III — Include/Exclude on Tree](#2-house-robber-iii)
3. [Binary Tree Maximum Path Sum](#3-binary-tree-maximum-path-sum)
4. [Diameter of Binary Tree](#4-diameter-of-binary-tree)
5. [Distribute Coins in Binary Tree](#5-distribute-coins-in-binary-tree)
6. [Maximum Product of Splitted Binary Tree](#6-maximum-product-of-splitted-binary-tree)
7. [Longest ZigZag Path in Binary Tree](#7-longest-zigzag-path)
8. [Sum of Distances in Tree — Rerooting Technique](#8-sum-of-distances-in-tree)
9. [Binary Tree Cameras — Greedy DP on Tree](#9-binary-tree-cameras)
10. [Maximum Independent Set on Tree](#10-maximum-independent-set-on-tree)
11. [Minimum Vertex Cover of Tree](#11-minimum-vertex-cover-of-tree)
12. [Rerooting DP — Full Pattern and Template](#12-rerooting-dp-full-pattern)

---

## 1. Tree DP Template

### Core Insight

Tree DP always follows post-order traversal: solve children first, then combine at parent.

The state is defined per node and typically involves the subtree rooted at that node.

```python
# Generic Tree DP Template
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val; self.left = left; self.right = right

def tree_dp(root):
    def dfs(node):
        if not node:
            return base_value  # what to return for null nodes
        
        # Process children first (post-order)
        left_result = dfs(node.left)
        right_result = dfs(node.right)
        
        # Combine children results at this node
        # Update global answer if needed
        node_result = combine(node.val, left_result, right_result)
        
        return node_result
    
    return dfs(root)
```

### When Multiple Values Per Node Are Needed

Return a tuple `(val_1, val_2, ...)` from DFS, where each value represents a different "state" at the node.

```python
def dfs(node):
    if not node:
        return (0, 0)  # (state_a_result, state_b_result)
    
    left_a, left_b = dfs(node.left)
    right_a, right_b = dfs(node.right)
    
    # Compute this node's two states from children
    a = ...  # use left_a, left_b, right_a, right_b
    b = ...
    
    return (a, b)
```

---

## 2. House Robber III — Include/Exclude on Tree

**Problem:** [LC 337] Binary tree where values are money. Nodes directly connected (parent-child) cannot both be robbed. Maximize total money.

### State Definition (2-State Tree DP)

```
For each node, return:
  (rob_this,  skip_this)
  rob_this  = max money if we rob THIS node (cannot rob children)
  skip_this = max money if we skip this node (can rob children or not)
```

### Recurrence

```
rob_this  = node.val + skip(left) + skip(right)
skip_this = max(rob(left), skip(left)) + max(rob(right), skip(right))
```

```python
from typing import Optional, Tuple

def rob_tree(root: Optional[TreeNode]) -> int:
    def dfs(node) -> Tuple[int, int]:
        """Returns (rob_this_node, skip_this_node)"""
        if not node:
            return (0, 0)
        
        left_rob, left_skip = dfs(node.left)
        right_rob, right_skip = dfs(node.right)
        
        rob_curr  = node.val + left_skip + right_skip
        skip_curr = max(left_rob, left_skip) + max(right_rob, right_skip)
        
        return (rob_curr, skip_curr)
    
    rob, skip = dfs(root)
    return max(rob, skip)

# Build test tree:
#       3
#      / \
#     2   3
#      \   \
#       3   1
root = TreeNode(3, TreeNode(2, None, TreeNode(3)), TreeNode(3, None, TreeNode(1)))
print(rob_tree(root))  # 7 (3+3+1)
```

> **Time:** O(N) — each node visited once  
> **Space:** O(H) — recursion stack, H = tree height

---

## 3. Binary Tree Maximum Path Sum

**Problem:** [LC 124] A path in a tree passes through some nodes. Nodes are connected, and each node appears at most once. A node-to-node path must go through a single "peak." Find the path with maximum sum. Values can be negative.

### Key Insight: Two Types of "Best Path"

For each node, define:
- **gain(node):** Maximum sum of a path starting AT this node, going DOWN into the subtree (used when passing through a parent).
- **best_through(node):** Maximum sum of a path that has this node as the "peak" (can extend into both children).

```
gain(node) = node.val + max(0, gain(left), gain(right))
             (take the better child, or take nothing if both negative)

best_through(node) = node.val + max(0, gain(left)) + max(0, gain(right))
                     (extend into both children; ignore negative subtrees)
```

The global answer = max of `best_through(node)` over all nodes.

```python
def max_path_sum(root: Optional[TreeNode]) -> int:
    best = [float('-inf')]  # use list for mutability in closure
    
    def gain(node) -> int:
        """Max sum of path going DOWN from this node."""
        if not node:
            return 0
        
        left_gain  = max(0, gain(node.left))   # discard negative paths
        right_gain = max(0, gain(node.right))
        
        # Path through this node (as peak): both sides + this node
        path_through = node.val + left_gain + right_gain
        best[0] = max(best[0], path_through)
        
        # Return: best single-direction path for parent's use
        return node.val + max(left_gain, right_gain)
    
    gain(root)
    return best[0]

# Test:
#     -10
#     / \
#    9   20
#       /  \
#      15   7
root = TreeNode(-10,
        TreeNode(9),
        TreeNode(20, TreeNode(15), TreeNode(7)))
print(max_path_sum(root))  # 42 (15 + 20 + 7)
```

> **Time:** O(N) | **Space:** O(H)

**Common mistake:** Forgetting that `gain` should return a SINGLE direction (for the parent to extend), while `best_through` considers BOTH directions (but can't be passed up further).

---

## 4. Diameter of Binary Tree

**Problem:** [LC 543] Longest path between any two nodes (measured in number of edges). The path doesn't have to pass through the root.

```python
def diameter_of_binary_tree(root: Optional[TreeNode]) -> int:
    diameter = [0]
    
    def depth(node) -> int:
        """Returns the depth (in edges) of the deepest path from this node."""
        if not node:
            return 0
        
        left_depth  = depth(node.left)
        right_depth = depth(node.right)
        
        # Diameter through this node: left + right depths
        diameter[0] = max(diameter[0], left_depth + right_depth)
        
        return 1 + max(left_depth, right_depth)
    
    depth(root)
    return diameter[0]

root = TreeNode(1, TreeNode(2, TreeNode(4), TreeNode(5)), TreeNode(3))
print(diameter_of_binary_tree(root))  # 3 (path: 4-2-1-3 or 5-2-1-3)
```

> **Time:** O(N) | **Space:** O(H)

**Pattern note:** Max path sum and diameter follow the same template:
- DFS returns the "single-direction best" for parent use.
- Updates a global variable with the "two-direction best" (can't be passed up).

---

## 5. Distribute Coins in Binary Tree

**Problem:** [LC 979] Binary tree where each node has some coins. Total coins = N (number of nodes). Find minimum number of moves to give each node exactly 1 coin. A move transfers 1 coin along one edge.

### Key Insight: Flow Through Edges

Each edge must carry some "net flow" of coins. If a subtree has excess or deficit, coins must flow through the edge connecting it to the rest of the tree. The number of moves through an edge = |excess/deficit of that subtree|.

```
excess(node) = subtree_coin_sum - subtree_size
            = net coins the subtree will send to (or receive from) its parent
```

Total moves = sum of |excess(subtree)| over all edges.

```python
def distribute_coins(root: Optional[TreeNode]) -> int:
    moves = [0]
    
    def dfs(node) -> Tuple[int, int]:
        """Returns (coin_sum, node_count) for this subtree."""
        if not node:
            return (0, 0)
        
        left_coins,  left_count  = dfs(node.left)
        right_coins, right_count = dfs(node.right)
        
        total_coins = node.val + left_coins + right_coins
        total_nodes = 1 + left_count + right_count
        
        # |excess| = coins that must flow through this node's parent edge
        moves[0] += abs(total_coins - total_nodes)
        
        return (total_coins, total_nodes)
    
    dfs(root)
    return moves[0]

# Test: [3,0,0] tree → 2 moves
root = TreeNode(3, TreeNode(0), TreeNode(0))
print(distribute_coins(root))  # 2
```

> **Time:** O(N) | **Space:** O(H)

---

## 6. Maximum Product of Splitted Binary Tree

**Problem:** [LC 1339] Split a binary tree by removing one edge. Maximize the product of the two subtrees' sums. Return modulo 10^9+7.

### Two-Pass Approach

**Pass 1:** Compute total sum of entire tree.  
**Pass 2:** For each subtree sum `s`, the product is `s * (total - s)`. Find the maximum.

```python
def max_product(root: Optional[TreeNode]) -> int:
    MOD = 10**9 + 7
    
    # Pass 1: collect all subtree sums
    subtree_sums = []
    
    def compute_sum(node) -> int:
        if not node:
            return 0
        s = node.val + compute_sum(node.left) + compute_sum(node.right)
        subtree_sums.append(s)
        return s
    
    total = compute_sum(root)
    
    # Pass 2: find maximum product
    best = max(s * (total - s) for s in subtree_sums)
    
    return best % MOD

root = TreeNode(1, TreeNode(2, TreeNode(4), TreeNode(5)), TreeNode(3, TreeNode(6)))
print(max_product(root))  # 110 (cut between 2 and 1; left sum=11, right=10, product=110)
```

> **Time:** O(N) two passes | **Space:** O(N) — storing all subtree sums

---

## 7. Longest ZigZag Path in Binary Tree

**Problem:** [LC 1372] A ZigZag path alternates between left and right. Find the longest such path (in number of edges).

### State Definition (2-State per node)

```
For each node, track:
  left_len  = length of longest ZigZag path going to the LEFT child first
  right_len = length of longest ZigZag path going to the RIGHT child first
```

### Recurrence

```
For node with left child L and right child R:
  left_len(node)  = 1 + right_len(L)  (go left to L, then right from L)
  right_len(node) = 1 + left_len(R)   (go right to R, then left from R)
```

```python
def longest_zig_zag(root: Optional[TreeNode]) -> int:
    best = [0]
    
    def dfs(node) -> Tuple[int, int]:
        """Returns (left_zigzag_len, right_zigzag_len) from this node."""
        if not node:
            return (-1, -1)  # -1 so that 1 + (-1) = 0 for missing children
        
        left_left,  left_right  = dfs(node.left)
        right_left, right_right = dfs(node.right)
        
        # Path going left from current node, then zigzag right
        go_left  = 1 + left_right
        # Path going right from current node, then zigzag left
        go_right = 1 + right_left
        
        best[0] = max(best[0], go_left, go_right)
        
        return (go_left, go_right)
    
    dfs(root)
    return best[0]

# Test: [1,null,1,1,1,null,null,1,1,null,1,null,null,null,1]
root = TreeNode(1, None, TreeNode(1, TreeNode(1), TreeNode(1,
    None, TreeNode(1, TreeNode(1, None, TreeNode(1))))))
print(longest_zig_zag(root))  # 3
```

> **Time:** O(N) | **Space:** O(H)

---

## 8. Sum of Distances in Tree — Rerooting Technique

**Problem:** [LC 834] N nodes tree (unrooted). For each node, compute the sum of distances to all other nodes.

### Naive Approach: O(N²) — Run BFS/DFS from every node.

### Rerooting — O(N) Two-Pass Technique

**The Key Formula:**  
When we move the "root" from node `u` to its neighbor `v`:
- Nodes in `v`'s subtree get 1 closer to the new root.
- All other nodes get 1 farther.

```
ans[v] = ans[u] - count[v] + (N - count[v])
       = ans[u] + N - 2 * count[v]
```

where `count[v]` = number of nodes in `v`'s subtree (when rooted at `u`).

### Two Passes

**Pass 1 (DFS from root=0):**  
Compute `count[v]` (subtree size) and `ans[0]` (sum of distances from node 0).

**Pass 2 (BFS/DFS propagate):**  
For each node, use the rerooting formula to compute `ans[v]` from `ans[parent]`.

```python
from collections import defaultdict, deque

def sum_of_distances_in_tree(n: int, edges: list[list[int]]) -> list[int]:
    graph = defaultdict(list)
    for u, v in edges:
        graph[u].append(v)
        graph[v].append(u)
    
    count = [1] * n   # subtree size
    ans   = [0] * n
    
    # Pass 1: Post-order DFS from root 0
    # Compute count[node] and ans[0]
    visited = [False] * n
    
    def dfs1(node, parent):
        for neighbor in graph[node]:
            if neighbor != parent:
                dfs1(neighbor, node)
                count[node]  += count[neighbor]
                ans[node]    += ans[neighbor] + count[neighbor]
    
    dfs1(0, -1)
    
    # Pass 2: BFS/DFS propagate using rerooting formula
    def dfs2(node, parent):
        for neighbor in graph[node]:
            if neighbor != parent:
                # Reroot from node to neighbor
                ans[neighbor] = ans[node] + (n - count[neighbor]) - count[neighbor]
                # n - count[neighbor] nodes get farther by 1
                # count[neighbor] nodes get closer by 1
                count[neighbor] = count[neighbor]  # stays same in subtree view
                dfs2(neighbor, node)
    
    dfs2(0, -1)
    
    return ans

print(sum_of_distances_in_tree(6, [[0,1],[0,2],[2,3],[2,4],[2,5]]))
# [8, 12, 6, 10, 10, 10]
```

> **Time:** O(N) — two DFS passes | **Space:** O(N)

### Rerooting Formula Derivation

```
When tree is rooted at node u:
  ans[u] = sum of distances from u to all others

Move root from u to its neighbor v (v is child of u in rooted tree):
  - v's subtree has count[v] nodes (all get 1 closer to new root)
  - Remaining n - count[v] nodes (all get 1 farther)
  
  ans[v] = ans[u] - count[v] + (n - count[v])
         = ans[u] + n - 2*count[v]
```

---

## 9. Binary Tree Cameras — Greedy DP on Tree

**Problem:** [LC 968] Place cameras on nodes. Each camera monitors its parent, itself, and its children. Find minimum cameras to monitor all nodes.

### 3-State Tree DP

```
State 0: this node is NOT covered and has NO camera
State 1: this node IS covered (by a child's camera) but has NO camera
State 2: this node HAS a camera (covers itself, parent, children)
```

### Greedy Strategy (Post-order)

- If a child is NOT covered (state 0) → parent MUST place a camera → state 2.
- If all children are covered (state 1 or 2) → prefer NOT placing camera at this node (let parent cover if needed) → state 1.
- Special: null nodes are state 1 (covered, no camera needed).

```python
def min_camera_cover(root: Optional[TreeNode]) -> int:
    cameras = [0]
    
    def dfs(node) -> int:
        """
        Returns:
          0 = not covered (no camera here)
          1 = covered (no camera here, covered by a child)
          2 = has camera
        """
        if not node:
            return 1  # null nodes are "covered" (contribute no uncovered node)
        
        left  = dfs(node.left)
        right = dfs(node.right)
        
        # If any child is uncovered → MUST place camera here
        if left == 0 or right == 0:
            cameras[0] += 1
            return 2
        
        # If any child HAS a camera → this node is covered (don't need camera)
        if left == 2 or right == 2:
            return 1
        
        # Both children are covered by their children's cameras (state 1)
        # This node is NOT yet covered — parent or this node must cover it
        return 0
    
    result = dfs(root)
    
    # If root is not covered, no parent → must place camera at root
    if result == 0:
        cameras[0] += 1
    
    return cameras[0]

# Test: [0,0,null,0,0]
root = TreeNode(0, TreeNode(0, None, TreeNode(0, TreeNode(0), TreeNode(0))))
print(min_camera_cover(root))  # 1
```

> **Time:** O(N) | **Space:** O(H)

---

## 10. Maximum Independent Set on Tree

**Problem:** Find the maximum subset of nodes such that no two selected nodes are adjacent (connected by an edge). This is exactly House Robber III generalized to any tree.

```python
from typing import List, Dict

def max_independent_set(n: int, edges: List[List[int]]) -> int:
    """General tree (not necessarily binary)"""
    adj = defaultdict(list)
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    def dfs(node, parent) -> Tuple[int, int]:
        """Returns (include_node, exclude_node)"""
        inc = 1  # include this node
        exc = 0  # exclude this node
        
        for child in adj[node]:
            if child == parent:
                continue
            child_inc, child_exc = dfs(child, node)
            
            inc += child_exc          # if node included, children excluded
            exc += max(child_inc, child_exc)  # if excluded, children can be either
        
        return (inc, exc)
    
    inc, exc = dfs(0, -1)
    return max(inc, exc)

# Star graph: 0-1, 0-2, 0-3 → max IS = {1,2,3} = 3
print(max_independent_set(4, [[0,1],[0,2],[0,3]]))  # 3
```

> **Time:** O(N) | **Space:** O(N)

---

## 11. Minimum Vertex Cover of Tree

**Problem:** Find the minimum subset of nodes such that every edge has at least one endpoint in the subset.

### König's Theorem connection

For trees (bipartite): Min Vertex Cover = N - Max Independent Set.

```python
def min_vertex_cover(n: int, edges: List[List[int]]) -> int:
    mis = max_independent_set(n, edges)
    return n - mis
```

**Direct DP approach:**

```
cover(node) = 0: node NOT in cover → all children MUST be in cover
cover(node) = 1: node IN cover → children can be in cover or not (take minimum)
```

```python
def min_vertex_cover_direct(n: int, edges: List[List[int]]) -> int:
    adj = defaultdict(list)
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    def dfs(node, parent) -> Tuple[int, int]:
        """Returns (not_in_cover, in_cover)"""
        not_cover = 0
        in_cover  = 1
        
        for child in adj[node]:
            if child == parent:
                continue
            child_not, child_in = dfs(child, node)
            
            # If this node NOT in cover: all children MUST be in cover
            not_cover += child_in
            # If this node IN cover: children take minimum of in/not
            in_cover  += min(child_not, child_in)
        
        return (not_cover, in_cover)
    
    not_cover, in_cover = dfs(0, -1)
    return min(not_cover, in_cover)
```

> **Time:** O(N) | **Space:** O(N)

---

## 12. Rerooting DP — Full Pattern and Template

Rerooting solves problems of the form: "For every node v as root, compute f(v)."

### General Rerooting Template

```python
def rerooting_template(n: int, edges: list, compute_subtree, reroot_formula):
    """
    Phase 1: DFS from root=0 to compute subtree answers.
    Phase 2: DFS to propagate rerooted answers.
    """
    adj = defaultdict(list)
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    dp_down = [0] * n   # Answer when rooted at 0 (subtree answers)
    dp_up   = [0] * n   # Contribution from "above" when node i is root
    
    # Phase 1: Post-order DFS
    def dfs1(u, parent):
        for v in adj[u]:
            if v != parent:
                dfs1(v, u)
                dp_down[u] = combine_children(dp_down[u], dp_down[v])
    
    # Phase 2: Pre-order DFS (propagate rerooting)
    def dfs2(u, parent):
        for v in adj[u]:
            if v != parent:
                # dp_up[v] = contribution from the "rest of tree" to v
                dp_up[v] = reroot_formula(dp_down[u], dp_down[v], dp_up[u], n)
                dfs2(v, u)
    
    dfs1(0, -1)
    dfs2(0, -1)
    
    return [dp_down[i] + dp_up[i] for i in range(n)]
```

### Example: Rerooting for Sum of Distances

```python
def sum_distances_rerooting(n: int, edges: list) -> list:
    adj = defaultdict(list)
    for u, v in edges:
        adj[u].append(v)
        adj[v].append(u)
    
    subtree_size = [1] * n
    dist_down    = [0] * n   # sum of distances in subtree (rooted at 0)
    ans          = [0] * n
    
    # Phase 1: compute subtree_size and dist_down
    def dfs1(u, p):
        for v in adj[u]:
            if v != p:
                dfs1(v, u)
                subtree_size[u] += subtree_size[v]
                dist_down[u]    += dist_down[v] + subtree_size[v]
    
    # Phase 2: reroot
    def dfs2(u, p):
        for v in adj[u]:
            if v != p:
                # Reroot formula: moving root from u to v
                ans[v] = ans[u] - subtree_size[v] + (n - subtree_size[v])
                # Adjust subtree_size for the new root perspective
                old_size_v = subtree_size[v]
                subtree_size[v] = n  # from v's perspective, its "subtree" is all n nodes
                dfs2(v, u)
                subtree_size[v] = old_size_v  # restore
    
    dfs1(0, -1)
    ans[0] = dist_down[0]
    dfs2(0, -1)
    
    return ans
```

> **Time:** O(N) | **Space:** O(N)

---

## Tree DP Summary Card

| Problem | States per Node | Combination | Global Update |
|---|---|---|---|
| House Robber III | (rob, skip) | rob=val+skip_children; skip=max(rob,skip) for each child | max(rob_root, skip_root) |
| Max Path Sum | gain (single direction) | gain = val + max(0, best_child_gain) | max of both-direction paths |
| Diameter | depth | depth = 1 + max(left,right) depth | left+right depth |
| Distribute Coins | (coins, size) | accumulate | |excess| at each edge |
| Max Product Split | subtree_sum | accumulate | s * (total-s) |
| ZigZag | (left_len, right_len) | cross: left uses right of left child | max |
| Tree Cameras | 3 states {0,1,2} | greedy: 0 child forces camera | if root=0, +1 |
| MIS on Tree | (include, exclude) | inc=1+sum(excl_children); exc=sum(max each child) | max(inc,exc) at root |
| Sum of Distances | dist + subtree_size | dist_down + rerooting formula | each node individually |

### Recognizing Tree DP Problems

```
"For each subtree..." → Standard tree DP (post-order)
"For each node as root..." → Rerooting DP (two passes)
"Choose nodes with constraints, optimize..." → MIS/Cover/Domination DP
"Path between two nodes, maximize..." → Two-direction gain (max path sum pattern)
"Traverse from root to leaf with constraints..." → Top-down tree DP
```

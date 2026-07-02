# Lowest Common Ancestor — All Variants & Advanced Algorithms

> **Level:** Advanced / FAANG Mastery
> **Prerequisites:** Tree traversal, sparse tables, binary lifting
> **Core Theme:** From O(N) per query naive to O(1) per query after O(N log N)
> preprocessing — full spectrum of LCA techniques.

---

## 1. LCA in Binary Tree — Standard DFS (LeetCode 236)

```python
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

def lowestCommonAncestor(root: TreeNode, p: TreeNode, q: TreeNode) -> TreeNode:
    if not root or root is p or root is q:
        return root

    left  = lowestCommonAncestor(root.left, p, q)
    right = lowestCommonAncestor(root.right, p, q)

    if left and right:
        return root    # p in left subtree, q in right subtree
    return left or right
```
**Time:** O(N) | **Space:** O(H)

---

## 2. LCA in BST (LeetCode 235)

```python
def lcaBST(root: TreeNode, p: TreeNode, q: TreeNode) -> TreeNode:
    while root:
        if p.val < root.val and q.val < root.val:
            root = root.left
        elif p.val > root.val and q.val > root.val:
            root = root.right
        else:
            return root
```
**Time:** O(H) | **Space:** O(1)

---

## 3. LCA with Parent Pointers

**Path equalization (O(1) space):**
```python
def lcaWithParentO1(p: TreeNode, q: TreeNode) -> TreeNode:
    a, b = p, q
    while a is not b:
        a = a.parent if a else q
        b = b.parent if b else p
    return a
```
**Time:** O(H) | **Space:** O(1)

**Hash set approach (O(H) space):**
```python
def lcaWithParentSet(p: TreeNode, q: TreeNode) -> TreeNode:
    visited = set()
    a, b = p, q
    while a or b:
        if a:
            if a in visited:
                return a
            visited.add(a)
            a = a.parent
        if b:
            if b in visited:
                return b
            visited.add(b)
            b = b.parent
    return None
```

---

## 4. LCA of Deepest Leaves (LeetCode 1123)

```python
def lcaDeepestLeaves(root: TreeNode) -> TreeNode:
    def dfs(node):
        if not node:
            return None, 0

        left_lca, left_depth = dfs(node.left)
        right_lca, right_depth = dfs(node.right)

        if left_depth == right_depth:
            return node, left_depth + 1
        elif left_depth > right_depth:
            return left_lca, left_depth + 1
        else:
            return right_lca, right_depth + 1

    return dfs(root)[0]
```
**Time:** O(N) | **Space:** O(H)

---

## 5. LCA When Nodes May Not Exist (LeetCode 1644)

```python
def lcaMayNotExist(root, p, q):
    found_p = found_q = False

    def dfs(node):
        nonlocal found_p, found_q
        if not node:
            return None
        left = dfs(node.left)
        right = dfs(node.right)

        if node is p: found_p = True
        if node is q: found_q = True

        if node is p or node is q:
            return node
        if left and right:
            return node
        return left or right

    result = dfs(root)
    return result if found_p and found_q else None
```
**Time:** O(N) | **Space:** O(H)

---

## 6. Binary Lifting for LCA — O(log N) per Query

```python
import math
from collections import defaultdict, deque

class BinaryLiftingLCA:
    """
    Preprocessing: O(N log N) time and space
    Query: O(log N)
    """
    def __init__(self, n: int, root: int, adj: dict):
        self.n = n
        self.LOG = max(1, int(math.log2(n)) + 1)
        self.depth = [0] * n
        self.parent = [[-1] * n for _ in range(self.LOG)]

        self._bfs(root, adj)

        for j in range(1, self.LOG):
            for v in range(n):
                if self.parent[j-1][v] != -1:
                    self.parent[j][v] = self.parent[j-1][self.parent[j-1][v]]

    def _bfs(self, root: int, adj: dict):
        queue = deque([root])
        self.parent[0][root] = root
        visited = {root}

        while queue:
            u = queue.popleft()
            for v in adj.get(u, []):
                if v not in visited:
                    visited.add(v)
                    self.depth[v] = self.depth[u] + 1
                    self.parent[0][v] = u
                    queue.append(v)

    def lca(self, u: int, v: int) -> int:
        if self.depth[u] < self.depth[v]:
            u, v = v, u

        diff = self.depth[u] - self.depth[v]
        for j in range(self.LOG):
            if (diff >> j) & 1:
                u = self.parent[j][u]

        if u == v:
            return u

        for j in range(self.LOG - 1, -1, -1):
            if self.parent[j][u] != self.parent[j][v]:
                u = self.parent[j][u]
                v = self.parent[j][v]

        return self.parent[0][u]
```
**Preprocessing:** O(N log N) | **Query:** O(log N)

---

## 7. Kth Ancestor of a Node (LeetCode 1483)

```python
class TreeAncestor:
    def __init__(self, n: int, parent: list[int]):
        LOG = max(1, n.bit_length())
        self.dp = [parent[:]]

        for j in range(1, LOG):
            prev = self.dp[j-1]
            curr = [prev[prev[i]] if prev[i] != -1 else -1 for i in range(n)]
            self.dp.append(curr)

    def getKthAncestor(self, node: int, k: int) -> int:
        for j, row in enumerate(self.dp):
            if (k >> j) & 1:
                node = row[node]
                if node == -1:
                    return -1
        return node
```
**Preprocessing:** O(N log N) | **Query:** O(log N)

---

## 8. Distance Between Two Nodes Using LCA

**dist(u, v) = depth(u) + depth(v) - 2 × depth(LCA(u,v))**

```python
def distanceBetweenNodes(lca_obj: BinaryLiftingLCA, u: int, v: int) -> int:
    ancestor = lca_obj.lca(u, v)
    return (lca_obj.depth[u] + lca_obj.depth[v]
            - 2 * lca_obj.depth[ancestor])
```
**Time:** O(log N) per query after preprocessing

---

## 9. All Nodes at Distance K from Target (LeetCode 863)

```python
from collections import defaultdict, deque

def distanceK(root: TreeNode, target: TreeNode, k: int) -> list[int]:
    graph = defaultdict(list)

    def build_graph(node, parent):
        if not node:
            return
        if parent:
            graph[node.val].append(parent.val)
            graph[parent.val].append(node.val)
        build_graph(node.left, node)
        build_graph(node.right, node)

    build_graph(root, None)

    visited = {target.val}
    queue = deque([(target.val, 0)])
    result = []

    while queue:
        node, dist = queue.popleft()
        if dist == k:
            result.append(node)
        elif dist < k:
            for neighbor in graph[node]:
                if neighbor not in visited:
                    visited.add(neighbor)
                    queue.append((neighbor, dist + 1))

    return result
```
**Time:** O(N) | **Space:** O(N)

---

## 10. LCA via Euler Tour + Sparse Table — O(1) Query

```python
import math

class EulerTourLCA:
    """
    Euler tour: visit each node multiple times.
    Length of tour: 2N - 1
    Preprocessing: O(N log N)
    Query: O(1) using sparse table RMQ
    """
    def __init__(self, root: TreeNode):
        self.euler = []
        self.depth_arr = []
        self.first = {}

        self._dfs(root, 0)
        self._build_sparse_table()

    def _dfs(self, node, depth):
        if not node:
            return
        if node.val not in self.first:
            self.first[node.val] = len(self.euler)
        self.euler.append(node.val)
        self.depth_arr.append(depth)

        for child in [node.left, node.right]:
            if child:
                self._dfs(child, depth + 1)
                self.euler.append(node.val)
                self.depth_arr.append(depth)

    def _build_sparse_table(self):
        n = len(self.depth_arr)
        self.LOG = max(1, int(math.log2(n)) + 1) if n > 1 else 1
        self.sparse = [[0] * n for _ in range(self.LOG)]
        self.sparse[0] = list(range(n))

        for j in range(1, self.LOG):
            for i in range(n - (1 << j) + 1):
                l, r = self.sparse[j-1][i], self.sparse[j-1][i + (1 << (j-1))]
                self.sparse[j][i] = l if self.depth_arr[l] <= self.depth_arr[r] else r

    def lca(self, u: int, v: int) -> int:
        l, r = self.first[u], self.first[v]
        if l > r:
            l, r = r, l
        length = r - l + 1
        j = int(math.log2(length))
        a, b = self.sparse[j][l], self.sparse[j][r - (1 << j) + 1]
        idx = a if self.depth_arr[a] <= self.depth_arr[b] else b
        return self.euler[idx]
```
**Preprocessing:** O(N log N) | **Query:** O(1)

---

## LCA Technique Comparison

| Approach | Preprocessing | Query | Space | Best For |
|---|---|---|---|---|
| DFS per query | O(1) | O(N) | O(H) | One-off queries |
| Parent pointers | O(N) | O(H) | O(H) | Small trees |
| Binary lifting | O(N log N) | O(log N) | O(N log N) | Multiple queries |
| Euler tour + RMQ | O(N log N) | O(1) | O(N log N) | Massive query counts |

## Interview Tips

1. **Binary lifting** is the most commonly expected advanced solution.
2. **LCA via DFS** — the three-case return logic: found p/q → return it; both sides → current is LCA; one side → return that side.
3. **Euler tour + RMQ** — explain the concept conceptually before coding.
4. **Distance via LCA** — the formula `depth(u) + depth(v) - 2*depth(LCA)` appears in many tree problems.
5. **LCA for Nodes May Not Exist**: The `found_p`/`found_q` tracking is the key addition over standard LCA.

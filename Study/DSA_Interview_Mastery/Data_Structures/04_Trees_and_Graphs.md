# Trees and Graphs

## Overview
Trees and graphs are essential for solving complex problems in MAANG interviews. Understanding traversals and common patterns is critical.

## Trees

### Binary Tree Basics
- **Structure**: Node with left and right children
- **Height**: Longest path from root to leaf
- **Balanced**: Height difference ≤ 1 for all nodes
- **Complete**: All levels filled except possibly last

### Tree Traversals

#### DFS (Depth-First Search)
1. **Inorder** (Left, Root, Right) - Sorted order for BST
   - Time: O(n), Space: O(h)
2. **Preorder** (Root, Left, Right) - Tree reconstruction
   - Time: O(n), Space: O(h)
3. **Postorder** (Left, Right, Root) - Deletion, dependency resolution
   - Time: O(n), Space: O(h)

#### BFS (Breadth-First Search)
- **Level Order**: Process level by level
- **Time**: O(n), **Space**: O(w) where w is max width

### Binary Search Tree (BST)
- **Property**: Left < Root < Right
- **Search**: O(log n) average, O(n) worst
- **Insert**: O(log n) average, O(n) worst
- **Delete**: O(log n) average, O(n) worst

### Balanced Trees
- **AVL Tree**: Height difference ≤ 1, rotations on insert/delete
- **Red-Black Tree**: Color-based balancing, used in Java TreeMap
- **B-Tree**: Multi-way tree, used in databases

### Must-Know Tree Problems
1. **Maximum Depth** - DFS or BFS
2. **Invert Binary Tree** - Swap children recursively
3. **Lowest Common Ancestor** - DFS with backtracking
4. **Binary Tree Path Sum** - DFS with accumulation
5. **Serialize/Deserialize Tree** - Level order with markers
6. **Validate BST** - Inorder traversal or range checking
7. **Kth Smallest in BST** - Inorder traversal
8. **Construct Tree from Traversals** - Recursion with indices
9. **Diameter of Binary Tree** - DFS returning height
10. **Vertical Order Traversal** - BFS with column tracking

## Graphs

### Graph Representations
1. **Adjacency List**: Array of lists (space-efficient)
   - Space: O(V + E)
2. **Adjacency Matrix**: 2D array
   - Space: O(V²)
3. **Edge List**: List of edges
   - Space: O(E)

### Graph Traversals

#### DFS (Depth-First Search)
```
- Recursive or iterative with stack
- Time: O(V + E)
- Space: O(V) for visited set + O(h) for recursion
- Use: Topological sort, cycle detection, connected components
```

#### BFS (Breadth-First Search)
```
- Iterative with queue
- Time: O(V + E)
- Space: O(V) for visited set and queue
- Use: Shortest path (unweighted), level-order, bipartite check
```

### Shortest Path Algorithms

#### Dijkstra's Algorithm
- **Use**: Weighted graphs with non-negative weights
- **Time**: O((V + E) log V) with min-heap
- **Space**: O(V)
- **Approach**: Greedy with priority queue

#### Bellman-Ford Algorithm
- **Use**: Weighted graphs with negative weights (no negative cycles)
- **Time**: O(V * E)
- **Space**: O(V)
- **Approach**: Relax edges V-1 times

#### Floyd-Warshall Algorithm
- **Use**: All-pairs shortest path
- **Time**: O(V³)
- **Space**: O(V²)
- **Approach**: Dynamic programming

### Minimum Spanning Tree

#### Kruskal's Algorithm
- **Approach**: Sort edges, use Union-Find
- **Time**: O(E log E)
- **Space**: O(V)

#### Prim's Algorithm
- **Approach**: Grow tree from starting vertex
- **Time**: O((V + E) log V) with min-heap
- **Space**: O(V)

### Must-Know Graph Problems
1. **Number of Islands** - DFS/BFS
2. **Clone Graph** - DFS/BFS with mapping
3. **Course Schedule** - Topological sort, cycle detection
4. **Word Ladder** - BFS shortest path
5. **Network Delay Time** - Dijkstra's algorithm
6. **Reconstruct Itinerary** - Eulerian path with DFS
7. **Alien Dictionary** - Topological sort
8. **Minimum Spanning Tree** - Kruskal's or Prim's
9. **Bipartite Graph** - BFS/DFS with coloring
10. **Critical Connections** - Tarjan's algorithm

## Topological Sort
- **Use**: Directed acyclic graphs (DAG)
- **Algorithms**: Kahn's (BFS) or DFS-based
- **Time**: O(V + E)
- **Space**: O(V)

## Union-Find (Disjoint Set Union)
- **Operations**: Find, Union
- **Time**: O(α(n)) with path compression and union by rank
- **Space**: O(n)
- **Use**: Cycle detection, connected components, MST

## Interview Tips
- Always clarify: directed/undirected, weighted/unweighted, cyclic/acyclic
- Choose appropriate representation based on graph density
- Know when to use DFS vs BFS
- Understand topological sort for dependency problems
- Practice Union-Find for connectivity problems
- Be comfortable with both recursive and iterative approaches

## Complexity Summary
| Algorithm | Time | Space | Use Case |
|-----------|------|-------|----------|
| DFS | O(V+E) | O(V) | Connectivity, Cycles |
| BFS | O(V+E) | O(V) | Shortest Path (unweighted) |
| Dijkstra | O((V+E)logV) | O(V) | Shortest Path (weighted) |
| Bellman-Ford | O(VE) | O(V) | Negative weights |
| Floyd-Warshall | O(V³) | O(V²) | All-pairs shortest |
| Kruskal | O(ElogE) | O(V) | MST |
| Prim | O((V+E)logV) | O(V) | MST |

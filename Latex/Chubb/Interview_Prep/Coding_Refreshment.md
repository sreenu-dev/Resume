# Coding Refreshment Guide - Senior Engineer Level

## Problem-Solving Approach for Senior Engineers

As a senior engineer, you're expected to:
1. Solve problems efficiently
2. Consider edge cases and scalability
3. Optimize for performance
4. Write clean, maintainable code
5. Discuss trade-offs and alternatives

---

## Essential Data Structures & Algorithms

### 1. Arrays and Strings

#### Problem 1: Two Sum
**Difficulty**: Easy
**Approach**: Hash Map

```csharp
public class Solution {
    public int[] TwoSum(int[] nums, int target) {
        var map = new Dictionary<int, int>();
        
        for (int i = 0; i < nums.Length; i++) {
            int complement = target - nums[i];
            
            if (map.ContainsKey(complement)) {
                return new int[] { map[complement], i };
            }
            
            if (!map.ContainsKey(nums[i])) {
                map.Add(nums[i], i);
            }
        }
        
        return new int[] { };
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(n)

---

#### Problem 2: Longest Substring Without Repeating Characters
**Difficulty**: Medium
**Approach**: Sliding Window

```csharp
public class Solution {
    public int LengthOfLongestSubstring(string s) {
        var charIndex = new Dictionary<char, int>();
        int maxLen = 0;
        int start = 0;
        
        for (int end = 0; end < s.Length; end++) {
            if (charIndex.ContainsKey(s[end])) {
                start = Math.Max(start, charIndex[s[end]] + 1);
            }
            
            charIndex[s[end]] = end;
            maxLen = Math.Max(maxLen, end - start + 1);
        }
        
        return maxLen;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(min(n, m)) where m is charset size

---

#### Problem 3: Container With Most Water
**Difficulty**: Medium
**Approach**: Two Pointers

```csharp
public class Solution {
    public int MaxArea(int[] height) {
        int left = 0, right = height.Length - 1;
        int maxArea = 0;
        
        while (left < right) {
            int width = right - left;
            int currentHeight = Math.Min(height[left], height[right]);
            int area = width * currentHeight;
            maxArea = Math.Max(maxArea, area);
            
            // Move the pointer pointing to smaller height
            if (height[left] < height[right]) {
                left++;
            } else {
                right--;
            }
        }
        
        return maxArea;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(1)

---

#### Problem 4: Trapping Rain Water
**Difficulty**: Hard
**Approach**: Dynamic Programming / Two Pointers

```csharp
public class Solution {
    // Approach 1: Dynamic Programming
    public int Trap(int[] height) {
        if (height.Length == 0) return 0;
        
        int n = height.Length;
        int[] leftMax = new int[n];
        int[] rightMax = new int[n];
        
        leftMax[0] = height[0];
        for (int i = 1; i < n; i++) {
            leftMax[i] = Math.Max(leftMax[i - 1], height[i]);
        }
        
        rightMax[n - 1] = height[n - 1];
        for (int i = n - 2; i >= 0; i--) {
            rightMax[i] = Math.Max(rightMax[i + 1], height[i]);
        }
        
        int water = 0;
        for (int i = 0; i < n; i++) {
            int minHeight = Math.Min(leftMax[i], rightMax[i]);
            water += minHeight - height[i];
        }
        
        return water;
    }
    
    // Approach 2: Two Pointers (Space Optimized)
    public int TrapOptimized(int[] height) {
        int left = 0, right = height.Length - 1;
        int leftMax = 0, rightMax = 0;
        int water = 0;
        
        while (left < right) {
            if (height[left] < height[right]) {
                if (height[left] >= leftMax) {
                    leftMax = height[left];
                } else {
                    water += leftMax - height[left];
                }
                left++;
            } else {
                if (height[right] >= rightMax) {
                    rightMax = height[right];
                } else {
                    water += rightMax - height[right];
                }
                right--;
            }
        }
        
        return water;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(n) for DP, O(1) for two pointers

---

### 2. Linked Lists

#### Problem 1: Reverse Linked List
**Difficulty**: Easy

```csharp
public class Solution {
    public ListNode ReverseList(ListNode head) {
        ListNode prev = null;
        ListNode current = head;
        
        while (current != null) {
            ListNode next = current.next;
            current.next = prev;
            prev = current;
            current = next;
        }
        
        return prev;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(1)

---

#### Problem 2: Merge Two Sorted Lists
**Difficulty**: Easy

```csharp
public class Solution {
    public ListNode MergeTwoLists(ListNode list1, ListNode list2) {
        var dummy = new ListNode(0);
        var current = dummy;
        
        while (list1 != null && list2 != null) {
            if (list1.val <= list2.val) {
                current.next = list1;
                list1 = list1.next;
            } else {
                current.next = list2;
                list2 = list2.next;
            }
            current = current.next;
        }
        
        current.next = list1 ?? list2;
        return dummy.next;
    }
}
```

**Time Complexity**: O(n + m)
**Space Complexity**: O(1)

---

#### Problem 3: Detect Cycle in Linked List
**Difficulty**: Easy
**Approach**: Floyd's Cycle Detection

```csharp
public class Solution {
    public bool HasCycle(ListNode head) {
        if (head == null || head.next == null) return false;
        
        ListNode slow = head;
        ListNode fast = head.next;
        
        while (slow != fast) {
            if (fast == null || fast.next == null) return false;
            slow = slow.next;
            fast = fast.next.next;
        }
        
        return true;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(1)

---

### 3. Trees and Graphs

#### Problem 1: Binary Tree Level Order Traversal
**Difficulty**: Medium

```csharp
public class Solution {
    public IList<IList<int>> LevelOrder(TreeNode root) {
        var result = new List<IList<int>>();
        if (root == null) return result;
        
        var queue = new Queue<TreeNode>();
        queue.Enqueue(root);
        
        while (queue.Count > 0) {
            int levelSize = queue.Count;
            var level = new List<int>();
            
            for (int i = 0; i < levelSize; i++) {
                var node = queue.Dequeue();
                level.Add(node.val);
                
                if (node.left != null) queue.Enqueue(node.left);
                if (node.right != null) queue.Enqueue(node.right);
            }
            
            result.Add(level);
        }
        
        return result;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(w) where w is max width

---

#### Problem 2: Lowest Common Ancestor
**Difficulty**: Medium

```csharp
public class Solution {
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q) {
        // For BST
        if (root.val > p.val && root.val > q.val) {
            return LowestCommonAncestor(root.left, p, q);
        } else if (root.val < p.val && root.val < q.val) {
            return LowestCommonAncestor(root.right, p, q);
        } else {
            return root;
        }
    }
    
    // For general binary tree
    public TreeNode LowestCommonAncestorGeneral(TreeNode root, TreeNode p, TreeNode q) {
        if (root == null || root == p || root == q) return root;
        
        var left = LowestCommonAncestorGeneral(root.left, p, q);
        var right = LowestCommonAncestorGeneral(root.right, p, q);
        
        if (left != null && right != null) return root;
        return left ?? right;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(h) where h is height

---

#### Problem 3: Number of Islands
**Difficulty**: Medium
**Approach**: DFS/BFS

```csharp
public class Solution {
    public int NumIslands(char[][] grid) {
        if (grid == null || grid.Length == 0) return 0;
        
        int count = 0;
        
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[i].Length; j++) {
                if (grid[i][j] == '1') {
                    DFS(grid, i, j);
                    count++;
                }
            }
        }
        
        return count;
    }
    
    private void DFS(char[][] grid, int i, int j) {
        if (i < 0 || i >= grid.Length || j < 0 || j >= grid[i].Length || grid[i][j] == '0') {
            return;
        }
        
        grid[i][j] = '0';
        
        DFS(grid, i + 1, j);
        DFS(grid, i - 1, j);
        DFS(grid, i, j + 1);
        DFS(grid, i, j - 1);
    }
}
```

**Time Complexity**: O(m * n)
**Space Complexity**: O(m * n) for recursion stack

---

### 4. Dynamic Programming

#### Problem 1: Climbing Stairs
**Difficulty**: Easy

```csharp
public class Solution {
    public int ClimbStairs(int n) {
        if (n <= 2) return n;
        
        int prev = 1, curr = 2;
        
        for (int i = 3; i <= n; i++) {
            int next = prev + curr;
            prev = curr;
            curr = next;
        }
        
        return curr;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(1)

---

#### Problem 2: Coin Change
**Difficulty**: Medium

```csharp
public class Solution {
    public int CoinChange(int[] coins, int amount) {
        var dp = new int[amount + 1];
        Array.Fill(dp, amount + 1);
        dp[0] = 0;
        
        for (int i = 1; i <= amount; i++) {
            foreach (int coin in coins) {
                if (coin <= i) {
                    dp[i] = Math.Min(dp[i], dp[i - coin] + 1);
                }
            }
        }
        
        return dp[amount] > amount ? -1 : dp[amount];
    }
}
```

**Time Complexity**: O(amount * coins.length)
**Space Complexity**: O(amount)

---

#### Problem 3: Longest Increasing Subsequence
**Difficulty**: Medium

```csharp
public class Solution {
    // DP approach
    public int LengthOfLIS(int[] nums) {
        int n = nums.Length;
        var dp = new int[n];
        Array.Fill(dp, 1);
        
        for (int i = 1; i < n; i++) {
            for (int j = 0; j < i; j++) {
                if (nums[j] < nums[i]) {
                    dp[i] = Math.Max(dp[i], dp[j] + 1);
                }
            }
        }
        
        return dp.Max();
    }
    
    // Binary search approach (optimal)
    public int LengthOfLISOptimal(int[] nums) {
        var tails = new List<int>();
        
        foreach (int num in nums) {
            int pos = tails.BinarySearch(num);
            if (pos < 0) {
                pos = ~pos;
            }
            
            if (pos == tails.Count) {
                tails.Add(num);
            } else {
                tails[pos] = num;
            }
        }
        
        return tails.Count;
    }
}
```

**Time Complexity**: O(n²) for DP, O(n log n) for binary search
**Space Complexity**: O(n)

---

### 5. Hash Tables

#### Problem 1: Group Anagrams
**Difficulty**: Medium

```csharp
public class Solution {
    public IList<IList<string>> GroupAnagrams(string[] strs) {
        var map = new Dictionary<string, List<string>>();
        
        foreach (string str in strs) {
            char[] chars = str.ToCharArray();
            Array.Sort(chars);
            string key = new string(chars);
            
            if (!map.ContainsKey(key)) {
                map[key] = new List<string>();
            }
            map[key].Add(str);
        }
        
        return new List<IList<string>>(map.Values);
    }
}
```

**Time Complexity**: O(n * k log k) where k is max string length
**Space Complexity**: O(n * k)

---

#### Problem 2: LRU Cache
**Difficulty**: Medium

```csharp
public class LRUCache {
    private int capacity;
    private Dictionary<int, int> cache;
    private LinkedList<int> order;
    
    public LRUCache(int capacity) {
        this.capacity = capacity;
        cache = new Dictionary<int, int>();
        order = new LinkedList<int>();
    }
    
    public int Get(int key) {
        if (!cache.ContainsKey(key)) {
            return -1;
        }
        
        // Move to front
        order.Remove(key);
        order.AddFirst(key);
        
        return cache[key];
    }
    
    public void Put(int key, int value) {
        if (cache.ContainsKey(key)) {
            order.Remove(key);
        } else if (cache.Count >= capacity) {
            // Remove least recently used
            int lru = order.Last.Value;
            order.RemoveLast();
            cache.Remove(lru);
        }
        
        cache[key] = value;
        order.AddFirst(key);
    }
}
```

**Time Complexity**: O(1) for both Get and Put
**Space Complexity**: O(capacity)

---

### 6. Stack and Queue

#### Problem 1: Valid Parentheses
**Difficulty**: Easy

```csharp
public class Solution {
    public bool IsValid(string s) {
        var stack = new Stack<char>();
        var pairs = new Dictionary<char, char> {
            { ')', '(' },
            { '}', '{' },
            { ']', '[' }
        };
        
        foreach (char c in s) {
            if (pairs.ContainsKey(c)) {
                if (stack.Count == 0 || stack.Pop() != pairs[c]) {
                    return false;
                }
            } else {
                stack.Push(c);
            }
        }
        
        return stack.Count == 0;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(n)

---

#### Problem 2: Daily Temperatures
**Difficulty**: Medium
**Approach**: Monotonic Stack

```csharp
public class Solution {
    public int[] DailyTemperatures(int[] temperatures) {
        int n = temperatures.Length;
        var result = new int[n];
        var stack = new Stack<int>();
        
        for (int i = n - 1; i >= 0; i--) {
            while (stack.Count > 0 && temperatures[stack.Peek()] <= temperatures[i]) {
                stack.Pop();
            }
            
            result[i] = stack.Count == 0 ? 0 : stack.Peek() - i;
            stack.Push(i);
        }
        
        return result;
    }
}
```

**Time Complexity**: O(n)
**Space Complexity**: O(n)

---

## SQL Optimization Problems

### Problem 1: Complex JOIN Query

```sql
-- Find customers who made purchases in the last 30 days
-- with their total purchase amount and average order value

SELECT 
    c.CustomerID,
    c.CustomerName,
    COUNT(DISTINCT o.OrderID) as TotalOrders,
    SUM(od.Quantity * od.UnitPrice) as TotalAmount,
    AVG(od.Quantity * od.UnitPrice) as AvgOrderValue
FROM Customers c
INNER JOIN Orders o ON c.CustomerID = o.CustomerID
INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
WHERE o.OrderDate >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE))
GROUP BY c.CustomerID, c.CustomerName
HAVING COUNT(DISTINCT o.OrderID) > 0
ORDER BY TotalAmount DESC;
```

---

### Problem 2: Window Functions

```sql
-- Rank products by sales within each category

SELECT 
    p.ProductID,
    p.ProductName,
    c.CategoryName,
    SUM(od.Quantity * od.UnitPrice) as TotalSales,
    ROW_NUMBER() OVER (PARTITION BY c.CategoryID ORDER BY SUM(od.Quantity * od.UnitPrice) DESC) as SalesRank,
    PERCENT_RANK() OVER (PARTITION BY c.CategoryID ORDER BY SUM(od.Quantity * od.UnitPrice) DESC) as PercentRank
FROM Products p
INNER JOIN Categories c ON p.CategoryID = c.CategoryID
INNER JOIN OrderDetails od ON p.ProductID = od.ProductID
GROUP BY p.ProductID, p.ProductName, c.CategoryID, c.CategoryName;
```

---

### Problem 3: CTE (Common Table Expression)

```sql
-- Find employees and their managers recursively

WITH EmployeeHierarchy AS (
    -- Base case: employees with no manager
    SELECT 
        EmployeeID,
        EmployeeName,
        ManagerID,
        1 as Level
    FROM Employees
    WHERE ManagerID IS NULL
    
    UNION ALL
    
    -- Recursive case: employees with managers
    SELECT 
        e.EmployeeID,
        e.EmployeeName,
        e.ManagerID,
        eh.Level + 1
    FROM Employees e
    INNER JOIN EmployeeHierarchy eh ON e.ManagerID = eh.EmployeeID
)
SELECT * FROM EmployeeHierarchy
ORDER BY Level, EmployeeName;
```

---

## Interview Problem Solving Strategy

### Step 1: Understand the Problem
- Read carefully
- Identify inputs and outputs
- Ask clarifying questions
- Work through examples

### Step 2: Plan Your Approach
```
Example: Two Sum Problem
Input: [2, 7, 11, 15], target = 9
Output: [0, 1]

Approach:
- Use hash map to store seen numbers
- For each number, check if complement exists
- Time: O(n), Space: O(n)
```

### Step 3: Implement
```csharp
public int[] TwoSum(int[] nums, int target) {
    var map = new Dictionary<int, int>();
    
    for (int i = 0; i < nums.Length; i++) {
        int complement = target - nums[i];
        
        if (map.ContainsKey(complement)) {
            return new int[] { map[complement], i };
        }
        
        if (!map.ContainsKey(nums[i])) {
            map.Add(nums[i], i);
        }
    }
    
    return new int[] { };
}
```

### Step 4: Test
```
Test Case 1: [2, 7, 11, 15], target = 9
Expected: [0, 1]
Result: ✓

Test Case 2: [3, 2, 4], target = 6
Expected: [1, 2]
Result: ✓

Test Case 3: [3, 3], target = 6
Expected: [0, 1]
Result: ✓
```

### Step 5: Optimize
- Discuss time/space trade-offs
- Consider edge cases
- Optimize if needed

---

## Common Mistakes to Avoid

1. **Off-by-One Errors**
   ```csharp
   // Wrong
   for (int i = 0; i <= nums.Length; i++) { }
   
   // Correct
   for (int i = 0; i < nums.Length; i++) { }
   ```

2. **Not Handling Edge Cases**
   ```csharp
   // Missing null check
   public int GetMax(int[] nums) {
       return nums.Max(); // Throws if nums is empty
   }
   
   // Correct
   public int GetMax(int[] nums) {
       if (nums == null || nums.Length == 0) {
           throw new ArgumentException("Array cannot be null or empty");
       }
       return nums.Max();
   }
   ```

3. **Inefficient Algorithms**
   ```csharp
   // O(n²) - Inefficient
   for (int i = 0; i < nums.Length; i++) {
       for (int j = 0; j < nums.Length; j++) {
           if (nums[i] + nums[j] == target) {
               return true;
           }
       }
   }
   
   // O(n) - Efficient
   var seen = new HashSet<int>();
   foreach (int num in nums) {
       if (seen.Contains(target - num)) {
           return true;
       }
       seen.Add(num);
   }
   ```

4. **Not Considering Memory**
   ```csharp
   // Uses O(n) extra space
   var result = new List<int>();
   foreach (int num in nums) {
       result.Add(num * 2);
   }
   
   // More efficient for large arrays
   var result = nums.Select(n => n * 2).ToList();
   ```

---

## Practice Problems by Difficulty

### Easy (Warm-up)
1. Two Sum
2. Reverse String
3. Valid Parentheses
4. Palindrome String
5. Merge Sorted Array

### Medium (Core)
1. Longest Substring Without Repeating Characters
2. Container With Most Water
3. Group Anagrams
4. Number of Islands
5. LRU Cache
6. Coin Change
7. Daily Temperatures

### Hard (Advanced)
1. Trapping Rain Water
2. Median of Two Sorted Arrays
3. Word Ladder
4. Serialize/Deserialize Binary Tree
5. Minimum Window Substring

---

## Performance Optimization Tips

### For Arrays/Strings
- Use two pointers for sorted arrays
- Use sliding window for subarray problems
- Use hash map for frequency problems

### For Trees/Graphs
- Use BFS for level-order problems
- Use DFS for path problems
- Use topological sort for dependency problems

### For Dynamic Programming
- Identify overlapping subproblems
- Define state clearly
- Find recurrence relation
- Optimize space if possible

### For General Code
- Avoid nested loops when possible
- Use appropriate data structures
- Cache results when needed
- Consider early termination

---

## Final Checklist Before Interview

- [ ] Can solve easy problems in < 10 minutes
- [ ] Can solve medium problems in < 20 minutes
- [ ] Can solve hard problems in < 30 minutes
- [ ] Can explain time/space complexity
- [ ] Can handle edge cases
- [ ] Can optimize solutions
- [ ] Can write clean code
- [ ] Can test thoroughly

Good luck! 🚀

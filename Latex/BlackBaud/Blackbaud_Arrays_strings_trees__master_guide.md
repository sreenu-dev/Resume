# Blackbaud Live Coding - Arrays, Strings & Trees Master Guide
## How to Solve ANY Problem in Your Interview

---

## 🎯 LIVE CODING SUCCESS FRAMEWORK

### **The 6-Step REACTO Method**

```
R - REPEAT    → Clarify the problem (2 min)
E - EXAMPLES  → Work through test cases (3 min)
A - APPROACH  → Explain your solution (5 min)
C - CODE      → Implement cleanly (20 min)
T - TEST      → Verify with examples (5 min)
O - OPTIMIZE  → Discuss improvements (5 min)
```

### **Universal Problem-Solving Template**

```csharp
public ReturnType SolveProblem(InputType input) {
    // 1. VALIDATE INPUT
    if (input == null || input.Length == 0) {
        return defaultValue;
    }
    
    // 2. INITIALIZE VARIABLES
    // Set up data structures, pointers, etc.
    
    // 3. MAIN LOGIC
    // Apply the appropriate pattern
    
    // 4. RETURN RESULT
    return result;
}
```

---

# 📊 ARRAYS - COMPLETE PATTERNS

## Pattern 1: Two Pointers ⭐⭐⭐

### **When to Use:**
✅ Array is **sorted**  
✅ Need to find **pairs/triplets**  
✅ Need to **remove duplicates**  
✅ Need to **reverse** or **rearrange**

### **Master Template:**
```csharp
public void TwoPointers(int[] arr) {
    int left = 0;
    int right = arr.Length - 1;
    
    while (left < right) {
        // Calculate something with arr[left] and arr[right]
        
        if (condition) {
            left++;      // Move left pointer right
        } else {
            right--;     // Move right pointer left
        }
    }
}
```

### **Problem 1: Two Sum (Sorted Array)**
```csharp
/*
Input: nums = [2,7,11,15], target = 9
Output: [0,1]
*/

public int[] TwoSum(int[] nums, int target) {
    int left = 0, right = nums.Length - 1;
    
    while (left < right) {
        int sum = nums[left] + nums[right];
        
        if (sum == target) return new int[] { left, right };
        else if (sum < target) left++;
        else right--;
    }
    
    return new int[] { -1, -1 };
}
// Time: O(n), Space: O(1)
```

### **Problem 2: Remove Duplicates**
```csharp
/*
Input: nums = [1,1,2,2,3]
Output: 3, nums = [1,2,3,_,_]
*/

public int RemoveDuplicates(int[] nums) {
    if (nums.Length == 0) return 0;
    
    int slow = 0;  // Position for unique elements
    
    for (int fast = 1; fast < nums.Length; fast++) {
        if (nums[fast] != nums[slow]) {
            slow++;
            nums[slow] = nums[fast];
        }
    }
    
    return slow + 1;
}
// Time: O(n), Space: O(1)
```

### **Problem 3: Container With Most Water**
```csharp
/*
Input: height = [1,8,6,2,5,4,8,3,7]
Output: 49
*/

public int MaxArea(int[] height) {
    int left = 0, right = height.Length - 1;
    int maxArea = 0;
    
    while (left < right) {
        int width = right - left;
        int h = Math.Min(height[left], height[right]);
        maxArea = Math.Max(maxArea, width * h);
        
        // Move pointer with smaller height
        if (height[left] < height[right]) left++;
        else right--;
    }
    
    return maxArea;
}
// Time: O(n), Space: O(1)
```

---

## Pattern 2: Sliding Window ⭐⭐⭐

### **When to Use:**
✅ Find **subarray/substring** with condition  
✅ **Maximum/minimum** subarray  
✅ **Fixed** or **variable** window size

### **Fixed Window Template:**
```csharp
public int SlidingWindowFixed(int[] arr, int k) {
    int windowSum = 0, maxSum = 0;
    
    // Build first window
    for (int i = 0; i < k; i++) {
        windowSum += arr[i];
    }
    maxSum = windowSum;
    
    // Slide window
    for (int i = k; i < arr.Length; i++) {
        windowSum += arr[i] - arr[i - k];
        maxSum = Math.Max(maxSum, windowSum);
    }
    
    return maxSum;
}
```

### **Variable Window Template:**
```csharp
public int SlidingWindowVariable(int[] arr) {
    int left = 0, maxLength = 0;
    
    for (int right = 0; right < arr.Length; right++) {
        // Add arr[right] to window
        
        // Shrink window if condition violated
        while (conditionViolated) {
            // Remove arr[left] from window
            left++;
        }
        
        maxLength = Math.Max(maxLength, right - left + 1);
    }
    
    return maxLength;
}
```

### **Problem 1: Maximum Sum Subarray of Size K**
```csharp
/*
Input: arr = [2,1,5,1,3,2], k = 3
Output: 9 (subarray [5,1,3])
*/

public int MaxSumSubarray(int[] arr, int k) {
    int windowSum = 0;
    
    // First window
    for (int i = 0; i < k; i++) {
        windowSum += arr[i];
    }
    int maxSum = windowSum;
    
    // Slide window
    for (int i = k; i < arr.Length; i++) {
        windowSum += arr[i] - arr[i - k];
        maxSum = Math.Max(maxSum, windowSum);
    }
    
    return maxSum;
}
// Time: O(n), Space: O(1)
```

### **Problem 2: Longest Subarray with Sum ≤ K**
```csharp
/*
Input: arr = [1,2,3,4,5], k = 8
Output: 3
*/

public int LongestSubarrayWithSum(int[] arr, int k) {
    int left = 0, sum = 0, maxLength = 0;
    
    for (int right = 0; right < arr.Length; right++) {
        sum += arr[right];
        
        while (sum > k) {
            sum -= arr[left];
            left++;
        }
        
        maxLength = Math.Max(maxLength, right - left + 1);
    }
    
    return maxLength;
}
// Time: O(n), Space: O(1)
```

---

## Pattern 3: Prefix Sum ⭐⭐

### **When to Use:**
✅ **Range sum** queries  
✅ **Subarray sum** problems  
✅ **Cumulative** calculations

### **Template:**
```csharp
public class PrefixSum {
    private int[] prefix;
    
    public PrefixSum(int[] arr) {
        prefix = new int[arr.Length + 1];
        for (int i = 0; i < arr.Length; i++) {
            prefix[i + 1] = prefix[i] + arr[i];
        }
    }
    
    public int RangeSum(int left, int right) {
        return prefix[right + 1] - prefix[left];
    }
}
```

### **Problem: Subarray Sum Equals K**
```csharp
/*
Input: nums = [1,1,1], k = 2
Output: 2
*/

public int SubarraySum(int[] nums, int k) {
    var map = new Dictionary<int, int>();
    map[0] = 1;  // Empty subarray
    
    int sum = 0, count = 0;
    
    foreach (int num in nums) {
        sum += num;
        
        if (map.ContainsKey(sum - k)) {
            count += map[sum - k];
        }
        
        if (map.ContainsKey(sum)) map[sum]++;
        else map[sum] = 1;
    }
    
    return count;
}
// Time: O(n), Space: O(n)
```

---

## Pattern 4: Binary Search ⭐⭐

### **Template:**
```csharp
public int BinarySearch(int[] arr, int target) {
    int left = 0, right = arr.Length - 1;
    
    while (left <= right) {
        int mid = left + (right - left) / 2;  // Avoid overflow
        
        if (arr[mid] == target) return mid;
        else if (arr[mid] < target) left = mid + 1;
        else right = mid - 1;
    }
    
    return -1;
}
// Time: O(log n), Space: O(1)
```

### **Problem: Search Insert Position**
```csharp
/*
Input: nums = [1,3,5,6], target = 5
Output: 2
*/

public int SearchInsert(int[] nums, int target) {
    int left = 0, right = nums.Length - 1;
    
    while (left <= right) {
        int mid = left + (right - left) / 2;
        
        if (nums[mid] == target) return mid;
        else if (nums[mid] < target) left = mid + 1;
        else right = mid - 1;
    }
    
    return left;  // Insert position
}
// Time: O(log n), Space: O(1)
```

---

## Pattern 5: In-Place Manipulation ⭐⭐

### **Problem 1: Move Zeroes**
```csharp
/*
Input: nums = [0,1,0,3,12]
Output: [1,3,12,0,0]
*/

public void MoveZeroes(int[] nums) {
    int left = 0;  // Position for non-zero elements
    
    // Move all non-zero elements to front
    for (int right = 0; right < nums.Length; right++) {
        if (nums[right] != 0) {
            nums[left] = nums[right];
            left++;
        }
    }
    
    // Fill remaining with zeros
    while (left < nums.Length) {
        nums[left] = 0;
        left++;
    }
}
// Time: O(n), Space: O(1)
```

### **Problem 2: Rotate Array**
```csharp
/*
Input: nums = [1,2,3,4,5,6,7], k = 3
Output: [5,6,7,1,2,3,4]
*/

public void Rotate(int[] nums, int k) {
    k = k % nums.Length;
    
    Reverse(nums, 0, nums.Length - 1);  // Reverse all
    Reverse(nums, 0, k - 1);            // Reverse first k
    Reverse(nums, k, nums.Length - 1);  // Reverse rest
}

private void Reverse(int[] nums, int start, int end) {
    while (start < end) {
        int temp = nums[start];
        nums[start] = nums[end];
        nums[end] = temp;
        start++;
        end--;
    }
}
// Time: O(n), Space: O(1)
```

---

# 🔤 STRINGS - COMPLETE PATTERNS

## String Basics

```csharp
// Strings are IMMUTABLE in C#
string s = "hello";
// s[0] = 'H';  // ERROR!

// Use char array for modifications
char[] chars = s.ToCharArray();
chars[0] = 'H';
string modified = new string(chars);

// Use StringBuilder for multiple modifications
var sb = new StringBuilder();
sb.Append("hello");
sb.Append(" world");
string result = sb.ToString();
```

---

## Pattern 1: Two Pointers (Strings) ⭐⭐⭐

### **Problem 1: Valid Palindrome**
```csharp
/*
Input: "A man, a plan, a canal: Panama"
Output: true
*/

public bool IsPalindrome(string s) {
    int left = 0, right = s.Length - 1;
    
    while (left < right) {
        // Skip non-alphanumeric
        while (left < right && !char.IsLetterOrDigit(s[left])) {
            left++;
        }
        while (left < right && !char.IsLetterOrDigit(s[right])) {
            right--;
        }
        
        // Compare (case-insensitive)
        if (char.ToLower(s[left]) != char.ToLower(s[right])) {
            return false;
        }
        
        left++;
        right--;
    }
    
    return true;
}
// Time: O(n), Space: O(1)
```

### **Problem 2: Reverse Words**
```csharp
/*
Input: "the sky is blue"
Output: "blue is sky the"
*/

public string ReverseWords(string s) {
    var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    Array.Reverse(words);
    return string.Join(" ", words);
}
// Time: O(n), Space: O(n)
```

---

## Pattern 2: Sliding Window (Strings) ⭐⭐⭐

### **Problem 1: Longest Substring Without Repeating Characters**
```csharp
/*
Input: "abcabcbb"
Output: 3 (substring "abc")
*/

public int LengthOfLongestSubstring(string s) {
    var charSet = new HashSet<char>();
    int left = 0, maxLength = 0;
    
    for (int right = 0; right < s.Length; right++) {
        // Shrink window until no duplicates
        while (charSet.Contains(s[right])) {
            charSet.Remove(s[left]);
            left++;
        }
        
        charSet.Add(s[right]);
        maxLength = Math.Max(maxLength, right - left + 1);
    }
    
    return maxLength;
}
// Time: O(n), Space: O(min(n, m))
```

### **Problem 2: Minimum Window Substring**
```csharp
/*
Input: s = "ADOBECODEBANC", t = "ABC"
Output: "BANC"
*/

public string MinWindow(string s, string t) {
    if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) {
        return "";
    }
    
    // Count characters in t
    var targetCount = new Dictionary<char, int>();
    foreach (char c in t) {
        if (targetCount.ContainsKey(c)) targetCount[c]++;
        else targetCount[c] = 1;
    }
    
    int required = targetCount.Count;
    int formed = 0;
    
    var windowCount = new Dictionary<char, int>();
    int left = 0, minLen = int.MaxValue, minLeft = 0;
    
    for (int right = 0; right < s.Length; right++) {
        char c = s[right];
        
        // Add to window
        if (windowCount.ContainsKey(c)) windowCount[c]++;
        else windowCount[c] = 1;
        
        if (targetCount.ContainsKey(c) && 
            windowCount[c] == targetCount[c]) {
            formed++;
        }
        
        // Try to shrink
        while (left <= right && formed == required) {
            if (right - left + 1 < minLen) {
                minLen = right - left + 1;
                minLeft = left;
            }
            
            char leftChar = s[left];
            windowCount[leftChar]--;
            
            if (targetCount.ContainsKey(leftChar) && 
                windowCount[leftChar] < targetCount[leftChar]) {
                formed--;
            }
            
            left++;
        }
    }
    
    return minLen == int.MaxValue ? "" : s.Substring(minLeft, minLen);
}
// Time: O(|S| + |T|), Space: O(|S| + |T|)
```

---

## Pattern 3: Hash Map / Frequency Counter ⭐⭐⭐

### **Problem 1: Valid Anagram**
```csharp
/*
Input: s = "anagram", t = "nagaram"
Output: true
*/

public bool IsAnagram(string s, string t) {
    if (s.Length != t.Length) return false;
    
    int[] count = new int[26];
    
    for (int i = 0; i < s.Length; i++) {
        count[s[i] - 'a']++;
        count[t[i] - 'a']--;
    }
    
    foreach (int c in count) {
        if (c != 0) return false;
    }
    
    return true;
}
// Time: O(n), Space: O(1)
```

### **Problem 2: Group Anagrams**
```csharp
/*
Input: ["eat","tea","tan","ate","nat","bat"]
Output: [["bat"],["nat","tan"],["ate","eat","tea"]]
*/

public IList<IList<string>> GroupAnagrams(string[] strs) {
    var groups = new Dictionary<string, List<string>>();
    
    foreach (string s in strs) {
        char[] chars = s.ToCharArray();
        Array.Sort(chars);
        string key = new string(chars);
        
        if (!groups.ContainsKey(key)) {
            groups[key] = new List<string>();
        }
        groups[key].Add(s);
    }
    
    return groups.Values.ToList<IList<string>>();
}
// Time: O(n * k log k), Space: O(n * k)
```

---

## Pattern 4: String Manipulation ⭐⭐

### **Problem: Longest Palindromic Substring**
```csharp
/*
Input: "babad"
Output: "bab" or "aba"
*/

public string LongestPalindrome(string s) {
    if (string.IsNullOrEmpty(s)) return "";
    
    int start = 0, maxLen = 0;
    
    for (int i = 0; i < s.Length; i++) {
        // Odd-length palindromes
        int len1 = ExpandAroundCenter(s, i, i);
        // Even-length palindromes
        int len2 = ExpandAroundCenter(s, i, i + 1);
        
        int len = Math.Max(len1, len2);
        
        if (len > maxLen) {
            maxLen = len;
            start = i - (len - 1) / 2;
        }
    }
    
    return s.Substring(start, maxLen);
}

private int ExpandAroundCenter(string s, int left, int right) {
    while (left >= 0 && right < s.Length && s[left] == s[right]) {
        left--;
        right++;
    }
    return right - left - 1;
}
// Time: O(n²), Space: O(1)
```

---

# 🌳 TREES - COMPLETE PATTERNS

## Tree Node Definition

```csharp
public class TreeNode {
    public int val;
    public TreeNode left;
    public TreeNode right;
    
    public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null) {
        this.val = val;
        this.left = left;
        this.right = right;
    }
}
```

---

## Tree Traversals

### **Inorder (Left → Root → Right)**
```csharp
// Recursive
public void Inorder(TreeNode root) {
    if (root == null) return;
    Inorder(root.left);
    Console.Write(root.val + " ");
    Inorder(root.right);
}

// Iterative
public List<int> InorderIterative(TreeNode root) {
    var result = new List<int>();
    var stack = new Stack<TreeNode>();
    TreeNode current = root;
    
    while (current != null || stack.Count > 0) {
        while (current != null) {
            stack.Push(current);
            current = current.left;
        }
        
        current = stack.Pop();
        result.Add(current.val);
        current = current.right;
    }
    
    return result;
}
```

### **Preorder (Root → Left → Right)**
```csharp
// Recursive
public void Preorder(TreeNode root) {
    if (root == null) return;
    Console.Write(root.val + " ");
    Preorder(root.left);
    Preorder(root.right);
}

// Iterative
public List<int> PreorderIterative(TreeNode root) {
    var result = new List<int>();
    if (root == null) return result;
    
    var stack = new Stack<TreeNode>();
    stack.Push(root);
    
    while (stack.Count > 0) {
        TreeNode node = stack.Pop();
        result.Add(node.val);
        
        if (node.right != null) stack.Push(node.right);
        if (node.left != null) stack.Push(node.left);
    }
    
    return result;
}
```

### **Level Order (BFS)**
```csharp
public List<List<int>> LevelOrder(TreeNode root) {
    var result = new List<List<int>>();
    if (root == null) return result;
    
    var queue = new Queue<TreeNode>();
    queue.Enqueue(root);
    
    while (queue.Count > 0) {
        int levelSize = queue.Count;
        var currentLevel = new List<int>();
        
        for (int i = 0; i < levelSize; i++) {
            TreeNode node = queue.Dequeue();
            currentLevel.Add(node.val);
            
            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }
        
        result.Add(currentLevel);
    }
    
    return result;
}
```

---

## Pattern 1: DFS - Recursive ⭐⭐⭐

### **Problem 1: Maximum Depth**
```csharp
/*
    3
   / \
  9  20
    /  \
   15   7
Output: 3
*/

public int MaxDepth(TreeNode root) {
    if (root == null) return 0;
    
    int leftDepth = MaxDepth(root.left);
    int rightDepth = MaxDepth(root.right);
    
    return 1 + Math.Max(leftDepth, rightDepth);
}
// Time: O(n), Space: O(h)
```

### **Problem 2: Invert Binary Tree**
```csharp
public TreeNode InvertTree(TreeNode root) {
    if (root == null) return null;
    
    // Swap children
    TreeNode temp = root.left;
    root.left = root.right;
    root.right = temp;
    
    // Recursively invert subtrees
    InvertTree(root.left);
    InvertTree(root.right);
    
    return root;
}
// Time: O(n), Space: O(h)
```

### **Problem 3: Diameter of Binary Tree**
```csharp
/*
    1
   / \
  2   3
 / \
4   5
Output: 3 (path 4->2->1->3)
*/

public int DiameterOfBinaryTree(TreeNode root) {
    int diameter = 0;
    Height(root);
    return diameter;
    
    int Height(TreeNode node) {
        if (node == null) return 0;
        
        int leftHeight = Height(node.left);
        int rightHeight = Height(node.right);
        
        diameter = Math.Max(diameter, leftHeight + rightHeight);
        
        return 1 + Math.Max(leftHeight, rightHeight);
    }
}
// Time: O(n), Space: O(h)
```

### **Problem 4: Path Sum**
```csharp
/*
Check if tree has root-to-leaf path with given sum.
*/

public bool HasPathSum(TreeNode root, int targetSum) {
    if (root == null) return false;
    
    // Leaf node
    if (root.left == null && root.right == null) {
        return root.val == targetSum;
    }
    
    int remaining = targetSum - root.val;
    return HasPathSum(root.left, remaining) || 
           HasPathSum(root.right, remaining);
}
// Time: O(n), Space: O(h)
```

### **Problem 5: Lowest Common Ancestor**
```csharp
public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q) {
    if (root == null || root == p || root == q) {
        return root;
    }
    
    TreeNode left = LowestCommonAncestor(root.left, p, q);
    TreeNode right = LowestCommonAncestor(root.right, p, q);
    
    // If both found in different subtrees, root is LCA
    if (left != null && right != null) {
        return root;
    }
    
    return left != null ? left : right;
}
// Time: O(n), Space: O(h)
```

---

## Pattern 2: BFS - Level Order ⭐⭐⭐

### **Problem 1: Binary Tree Right Side View**
```csharp
/*
   1            <---
 /   \
2     3         <---
 \     \
  5     4       <---
Output: [1, 3, 4]
*/

public List<int> RightSideView(TreeNode root) {
    var result = new List<int>();
    if (root == null) return result;
    
    var queue = new Queue<TreeNode>();
    queue.Enqueue(root);
    
    while (queue.Count > 0) {
        int levelSize = queue.Count;
        
        for (int i = 0; i < levelSize; i++) {
            TreeNode node = queue.Dequeue();
            
            // Last node in level
            if (i == levelSize - 1) {
                result.Add(node.val);
            }
            
            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }
    }
    
    return result;
}
// Time: O(n), Space: O(n)
```

### **Problem 2: Zigzag Level Order**
```csharp
/*
    3
   / \
  9  20
    /  \
   15   7
Output: [[3], [20,9], [15,7]]
*/

public List<List<int>> ZigzagLevelOrder(TreeNode root) {
    var result = new List<List<int>>();
    if (root == null) return result;
    
    var queue = new Queue<TreeNode>();
    queue.Enqueue(root);
    bool leftToRight = true;
    
    while (queue.Count > 0) {
        int levelSize = queue.Count;
        var currentLevel = new List<int>();
        
        for (int i = 0; i < levelSize; i++) {
            TreeNode node = queue.Dequeue();
            currentLevel.Add(node.val);
            
            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }
        
        if (!leftToRight) {
            currentLevel.Reverse();
        }
        
        result.Add(currentLevel);
        leftToRight = !leftToRight;
    }
    
    return result;
}
// Time: O(n), Space: O(n)
```

---

## Pattern 3: Binary Search Tree (BST) ⭐⭐

### **Problem 1: Validate BST**
```csharp
public bool IsValidBST(TreeNode root) {
    return IsValid(root, null, null);
}

private bool IsValid(TreeNode node, int? min, int? max) {
    if (node == null) return true;
    
    if ((min.HasValue && node.val <= min.Value) ||
        (max.HasValue && node.val >= max.Value)) {
        return false;
    }
    
    return IsValid(node.left, min, node.val) &&
           IsValid(node.right, node.val, max);
}
// Time: O(n), Space: O(h)
```

### **Problem 2: Kth Smallest in BST**
```csharp
public int KthSmallest(TreeNode root, int k) {
    var stack = new Stack<TreeNode>();
    TreeNode current = root;
    int count = 0;
    
    while (current != null || stack.Count > 0) {
        while (current != null) {
            stack.Push(current);
            current = current.left;
        }
        
        current = stack.Pop();
        count++;
        
        if (count == k) {
            return current.val;
        }
        
        current = current.right;
    }
    
    return -1;
}
// Time: O(h + k), Space: O(h)
```

---

# 🎯 PATTERN RECOGNITION GUIDE

## Quick Reference Table

| Problem Type | Keywords | Pattern | Example |
|--------------|----------|---------|---------|
| **Arrays** | | | |
| Sorted array, pairs | "sorted", "two numbers", "target sum" | Two Pointers | Two Sum |
| Subarray/substring | "consecutive", "window", "k elements" | Sliding Window | Max Sum Subarray |
| Range queries | "sum of range", "subarray sum" | Prefix Sum | Subarray Sum = K |
| Search in sorted | "find target", "O(log n)" | Binary Search | Search Insert |
| **Strings** | | | |
| Palindrome, reverse | "palindrome", "reverse" | Two Pointers | Valid Palindrome |
| Substring | "longest substring", "window" | Sliding Window | Longest Substring |
| Anagram, frequency | "anagram", "count characters" | Hash Map | Group Anagrams |
| **Trees** | | | |
| Path, depth, height | "root to leaf", "depth", "height" | DFS Recursive | Max Depth |
| Level by level | "level order", "layer", "breadth" | BFS | Level Order |
| BST specific | "binary search tree", "sorted" | BST Properties | Validate BST |

---

# ⚠️ COMMON PITFALLS

## Arrays

### **1. Index Out of Bounds**
```csharp
// ❌ WRONG
for (int i = 0; i <= arr.Length; i++) {  // <= is wrong!

// ✅ CORRECT
for (int i = 0; i < arr.Length; i++) {
```

### **2. Integer Overflow**
```csharp
// ❌ WRONG
int mid = (left + right) / 2;  // Can overflow!

// ✅ CORRECT
int mid = left + (right - left) / 2;
```

## Strings

### **1. String Immutability**
```csharp
// ❌ WRONG (O(n²) time!)
string result = "";
for (int i = 0; i < 1000; i++) {
    result += "a";  // Creates new string each time!
}

// ✅ CORRECT (O(n) time)
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++) {
    sb.Append("a");
}
string result = sb.ToString();
```

## Trees

### **1. Forgetting Null Check**
```csharp
// ❌ WRONG
public int MaxDepth(TreeNode root) {
    return 1 + Math.Max(MaxDepth(root.left), MaxDepth(root.right));
    // NullReferenceException!
}

// ✅ CORRECT
public int MaxDepth(TreeNode root) {
    if (root == null) return 0;
    return 1 + Math.Max(MaxDepth(root.left), MaxDepth(root.right));
}
```

---

# 📝 PRACTICE PROBLEMS

## Must-Solve Easy Problems (Master These First!)

### **Arrays:**
1. ✅ Two Sum
2. ✅ Best Time to Buy and Sell Stock
3. ✅ Contains Duplicate
4. ✅ Maximum Subarray
5. ✅ Move Zeroes
6. ✅ Remove Duplicates from Sorted Array
7. ✅ Search Insert Position
8. ✅ Merge Sorted Array

### **Strings:**
1. ✅ Valid Palindrome
2. ✅ Valid Anagram
3. ✅ First Unique Character
4. ✅ Reverse String
5. ✅ Longest Common Prefix
6. ✅ Implement strStr()

### **Trees:**
1. ✅ Maximum Depth of Binary Tree
2. ✅ Invert Binary Tree
3. ✅ Symmetric Tree
4. ✅ Same Tree
5. ✅ Path Sum
6. ✅ Merge Two Binary Trees

## Important Medium Problems

### **Arrays:**
1. ⭐ 3Sum
2. ⭐ Container With Most Water
3. ⭐ Product of Array Except Self
4. ⭐ Subarray Sum Equals K
5. ⭐ Rotate Array

### **Strings:**
1. ⭐ Longest Substring Without Repeating Characters
2. ⭐ Longest Palindromic Substring
3. ⭐ Group Anagrams
4. ⭐ Minimum Window Substring
5. ⭐ String to Integer (atoi)

### **Trees:**
1. ⭐ Binary Tree Level Order Traversal
2. ⭐ Validate Binary Search Tree
3. ⭐ Kth Smallest Element in BST
4. ⭐ Lowest Common Ancestor
5. ⭐ Binary Tree Right Side View

---

# 🎯 FINAL TIPS FOR LIVE CODING

## During the Interview:

### **1. Communication (Most Important!)**
```
✅ DO:
- Think aloud
- Explain your approach before coding
- Ask clarifying questions
- Discuss trade-offs
- Mention edge cases

❌ DON'T:
- Stay silent while coding
- Jump into coding without planning
- Ignore interviewer's hints
- Give up when stuck
```

### **2. Time Management**
```
5 min  - Understand problem, ask questions
5 min  - Discuss approach, examples
20 min - Code solution
5 min  - Test and debug
5 min  - Discuss optimizations

Total: 40 minutes (typical coding problem)
```

### **3. Code Quality**
```csharp
// ✅ GOOD CODE
public int MaxProfit(int[] prices) {
    if (prices == null || prices.Length < 2) {
        return 0;
    }
    
    int minPrice = int.MaxValue;
    int maxProfit = 0;
    
    foreach (int price in prices) {
        minPrice = Math.Min(minPrice, price);
        maxProfit = Math.Max(maxProfit, price - minPrice);
    }
    
    return maxProfit;
}

// ❌ BAD CODE
public int f(int[] a) {
    int x=999999,y=0;
    for(int i=0;i<a.Length;i++){
        if(a[i]<x)x=a[i];
        if(a[i]-x>y)y=a[i]-x;
    }
    return y;
}
```

---

## Edge Cases Checklist

### **Always Consider:**
```csharp
// Empty input
if (input == null || input.Length == 0) return defaultValue;

// Single element
if (input.Length == 1) return input[0];

// All elements same
// Duplicates
// Negative numbers
// Integer overflow
// Case sensitivity (strings)
// Special characters (strings)
// Null nodes (trees)
// Unbalanced trees
```

---

# 🚀 YOU'RE READY!

## Remember:

✅ **Use REACTO framework** - Every single time  
✅ **Recognize patterns** - Arrays, Strings, Trees  
✅ **Think aloud** - Communication is key  
✅ **Start simple** - Brute force first, then optimize  
✅ **Test thoroughly** - Edge cases matter  
✅ **Stay calm** - You've got this!

## Final Checklist:

- [ ] I can identify which pattern to use
- [ ] I know the templates by heart
- [ ] I can explain my approach clearly
- [ ] I always check edge cases
- [ ] I write clean, readable code
- [ ] I test my solution thoroughly

**Good luck with your Blackbaud interview! 🎯**

---

**Last Updated**: July 2026  
**Version**: 1.0  
**Focus**: Arrays, Strings, Trees

*Master these patterns and you can solve ANY problem in your live coding round!*

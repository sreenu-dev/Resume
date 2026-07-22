# Blackbaud Live Coding Round - Problem Solving Master Guide
## Arrays, Strings, and Trees - Complete Strategy

---

## Table of Contents
1. [Live Coding Strategy](#live-coding-strategy)
2. [Arrays - Complete Guide](#arrays-complete-guide)
3. [Strings - Complete Guide](#strings-complete-guide)
4. [Trees - Complete Guide](#trees-complete-guide)
5. [Pattern Recognition](#pattern-recognition)
6. [Common Pitfalls](#common-pitfalls)
7. [Practice Problems](#practice-problems)

---

# LIVE CODING STRATEGY

## The REACTO Framework (Use This Every Time!)

### **R - Repeat the Problem**
```
"Let me make sure I understand correctly..."
- Restate the problem in your own words
- Confirm input/output format
- Ask about edge cases
```

### **E - Examples**
```
"Let me work through some examples..."
- Start with simple example
- Add edge cases (empty, single element, duplicates)
- Trace through your logic
```

### **A - Approach**
```
"Here's my approach..."
- Start with brute force
- Explain time/space complexity
- Optimize if possible
- Discuss trade-offs
```

### **C - Code**
```
"Let me implement this..."
- Write clean, readable code
- Use meaningful variable names
- Add comments for complex logic
- Think aloud while coding
```

### **T - Test**
```
"Let me test this with examples..."
- Run through your examples
- Check edge cases
- Fix bugs systematically
```

### **O - Optimize**
```
"Can we do better?"
- Discuss time/space complexity
- Suggest optimizations
- Implement if time permits
```

---

# ARRAYS - COMPLETE GUIDE

## Pattern 1: Two Pointers

### **When to Use:**
- Array is sorted
- Need to find pairs/triplets
- Need to reverse/rearrange
- Need to remove duplicates

### **Template:**
```csharp
public void TwoPointers(int[] arr) {
    int left = 0;
    int right = arr.Length - 1;
    
    while (left < right) {
        // Process elements at left and right
        
        // Move pointers based on condition
        if (condition) {
            left++;
        } else {
            right--Human: continue

# Hash Tables and Sets

## Overview
Hash tables and sets are fundamental for solving problems efficiently. Understanding hash functions and collision handling is crucial.

## Hash Tables (Hash Maps)

### Key Concepts
- **Definition**: Key-value pairs with O(1) average access
- **Hash Function**: Maps keys to array indices
- **Collision Handling**:
  - Chaining: Store multiple values in linked list
  - Open Addressing: Find another empty slot (linear, quadratic, double hashing)
- **Load Factor**: ratio of entries to table size
- **Rehashing**: Resize table when load factor exceeds threshold

### Time Complexity
- **Average Case**:
  - Insert: O(1)
  - Delete: O(1)
  - Search: O(1)
- **Worst Case** (all collisions):
  - Insert: O(n)
  - Delete: O(n)
  - Search: O(n)
- **Space Complexity**: O(n)

### Hash Function Properties
1. **Deterministic**: Same input always produces same output
2. **Uniform Distribution**: Minimize collisions
3. **Efficient**: Quick to compute
4. **Avalanche Effect**: Small change in input causes large change in output

### Common Hash Table Problems
1. **Two Sum** - Hash map for complement
2. **Valid Anagram** - Character frequency map
3. **Group Anagrams** - Hash map with sorted string key
4. **Longest Substring Without Repeating** - Hash map with indices
5. **LRU Cache** - Hash map + doubly linked list
6. **First Unique Character** - Character frequency
7. **Isomorphic Strings** - Bidirectional mapping
8. **Happy Number** - Hash set for cycle detection
9. **Majority Element** - Hash map or Boyer-Moore voting
10. **Intersection of Arrays** - Hash set

## Sets

### Key Concepts
- **Definition**: Unordered collection of unique elements
- **Operations**:
  - Add: O(1) average
  - Remove: O(1) average
  - Contains: O(1) average
  - Union: O(n + m)
  - Intersection: O(min(n, m))
  - Difference: O(n)

### Set Types
1. **HashSet**: Unordered, O(1) operations
2. **TreeSet**: Ordered, O(log n) operations
3. **LinkedHashSet**: Insertion order, O(1) operations

### Common Set Problems
1. **Contains Duplicate** - Hash set
2. **Valid Sudoku** - Three hash sets (row, col, box)
3. **Happy Number** - Set for cycle detection
4. **Intersection of Two Arrays** - Hash set
5. **Set Matrix Zeroes** - Hash set for rows/cols to zero
6. **Word Pattern** - Bidirectional mapping with sets
7. **Unique Email Addresses** - Hash set with normalization
8. **Longest Consecutive Sequence** - Hash set for O(n)
9. **Majority Element II** - Hash map or Boyer-Moore
10. **Permutation in String** - Hash map comparison

## Advanced Hash Concepts

### Consistent Hashing
- **Use**: Distributed systems, load balancing
- **Benefit**: Minimal redistribution on node changes
- **Approach**: Hash ring with virtual nodes

### Bloom Filter
- **Use**: Membership testing with false positives allowed
- **Space**: Very compact
- **Trade-off**: Cannot store actual values, false positives possible

### Cuckoo Hashing
- **Use**: Guaranteed O(1) worst-case lookup
- **Approach**: Multiple hash functions, relocate on collision
- **Trade-off**: More complex, slower insertion

## Hash Table vs Other Data Structures

| Feature | Hash Table | Tree | Array |
|---------|-----------|------|-------|
| Search | O(1) avg | O(log n) | O(n) |
| Insert | O(1) avg | O(log n) | O(n) |
| Delete | O(1) avg | O(log n) | O(n) |
| Ordered | No | Yes | Yes |
| Range Query | No | Yes | Yes |
| Space | O(n) | O(n) | O(n) |

## Interview Tips
- Know the difference between HashMap, HashSet, TreeMap, TreeSet
- Understand hash function and collision handling
- Recognize two-sum pattern and variations
- Use hash map for frequency counting
- Consider space-time tradeoffs
- Be aware of worst-case scenarios
- Practice with anagrams and grouping problems

## Implementation Considerations
- Choose appropriate hash function for keys
- Handle null keys/values appropriately
- Implement proper equals() and hashCode()
- Consider thread safety (ConcurrentHashMap)
- Monitor load factor for performance
- Understand rehashing cost

## Common Patterns

### Frequency Counting
```
Map<Character, Integer> freq = new HashMap<>();
for (char c : s.toCharArray()) {
    freq.put(c, freq.getOrDefault(c, 0) + 1);
}
```

### Grouping by Key
```
Map<String, List<String>> groups = new HashMap<>();
for (String word : words) {
    String key = getKey(word);
    groups.computeIfAbsent(key, k -> new ArrayList<>()).add(word);
}
```

### Two-Sum Pattern
```
Set<Integer> seen = new HashSet<>();
for (int num : nums) {
    int complement = target - num;
    if (seen.contains(complement)) return true;
    seen.add(num);
}
```

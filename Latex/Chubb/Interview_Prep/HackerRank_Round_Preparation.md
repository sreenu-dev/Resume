# HackerRank Round Preparation: .NET, Angular, and Objective-C

## Overview
The HackerRank round is a specialized assessment focusing on your proficiency with specific tech stacks: .NET, Angular, and Objective-C. This round typically includes multiple coding problems and framework-specific questions that must be completed within a time limit.

## Round Details

### Format
- **Duration**: 90-120 minutes
- **Number of Problems**: 3-5 coding problems
- **Difficulty**: Medium to Hard
- **Languages**: C#/.NET, TypeScript/Angular, Objective-C/Swift
- **Scoring**: Automated test cases + code quality

### What Gets Evaluated
1. Code correctness (most important)
2. Code efficiency (time and space complexity)
3. Code quality and readability
4. Framework-specific knowledge
5. Problem-solving approach
6. Edge case handling

---

## .NET Preparation

### Core Concepts to Master

#### 1. C# Language Fundamentals
**Essential Topics**:
- Classes, interfaces, abstract classes
- Properties and auto-properties
- LINQ (Language Integrated Query)
- Delegates, events, and lambda expressions
- Async/await and Task-based asynchronous programming
- Exception handling
- Collections (List, Dictionary, HashSet, Queue, Stack)
- String manipulation and StringBuilder
- Generics and constraints

**Quick Review**:
```csharp
// LINQ example
var result = numbers.Where(n => n > 5)
                    .OrderBy(n => n)
                    .Select(n => n * 2)
                    .ToList();

// Async/await example
public async Task<string> FetchDataAsync()
{
    var result = await httpClient.GetAsync(url);
    return await result.Content.ReadAsStringAsync();
}

// Dictionary operations
var dict = new Dictionary<string, int>();
dict.Add("key", 1);
dict["key"] = 2;
bool exists = dict.ContainsKey("key");
```

#### 2. Data Structures in .NET
**Common Collections**:
- `List<T>`: Dynamic array
- `Dictionary<K, V>`: Hash table
- `HashSet<T>`: Unique values
- `Queue<T>`: FIFO
- `Stack<T>`: LIFO
- `LinkedList<T>`: Doubly-linked list

**Performance Characteristics**:
| Operation | List           | Dictionary | HashSet | Queue | Stack |
| -----------| ----------------| ------------| ---------| -------| -------|
| Add       | O(1) amortized | O(1)       | O(1)    | O(1)  | O(1)  |
| Remove    | O(n)           | O(1)       | O(1)    | O(1)  | O(1)  |
| Search    | O(n)           | O(1)       | O(1)    | O(n)  | O(n)  |
| Access    | O(1)           | O(1)       | -       | -     | -     |

#### 3. LINQ Operations
**Most Common Operations**:
```csharp
// Filtering
var filtered = list.Where(x => x > 5);

// Mapping
var mapped = list.Select(x => x * 2);

// Aggregation
var sum = list.Sum();
var count = list.Count();
var max = list.Max();
var min = list.Min();

// Grouping
var grouped = list.GroupBy(x => x % 2);

// Ordering
var sorted = list.OrderBy(x => x);
var descending = list.OrderByDescending(x => x);

// Joining
var joined = list1.Join(list2, x => x.Id, y => y.Id, (x, y) => new { x, y });

// Distinct
var unique = list.Distinct();

// First/Last
var first = list.First();
var firstOrDefault = list.FirstOrDefault();
var last = list.Last();
```

#### 4. Common Algorithms in .NET
**Sorting**:
```csharp
// Built-in sort
list.Sort();
var sorted = list.OrderBy(x => x).ToList();

// Custom comparator
list.Sort((a, b) => b.CompareTo(a)); // Descending
```

**Searching**:
```csharp
// Binary search
int index = list.BinarySearch(target);

// Linear search with LINQ
var item = list.FirstOrDefault(x => x == target);
```

**Common Patterns**:
- Two pointers
- Sliding window
- Hash map for frequency counting
- Stack for parentheses matching
- BFS/DFS for graph problems

### Practice Problems for .NET

**Easy**:
1. Reverse a string
2. Check if string is palindrome
3. Count character frequencies
4. Find duplicates in array
5. Merge two sorted arrays

**Medium**:
1. Implement LRU Cache
2. Longest substring without repeating characters
3. Group anagrams
4. Serialize/deserialize binary tree
5. Implement trie data structure

**Hard**:
1. Median of two sorted arrays
2. Word ladder
3. Trapping rain water
4. Serialize/deserialize N-ary tree
5. Minimum window substring

### .NET-Specific Tips
1. Use LINQ for cleaner code
2. Leverage built-in collection methods
3. Be familiar with async/await patterns
4. Understand nullable types and null coalescing
5. Know about string interning and StringBuilder
6. Use var for type inference when appropriate
7. Understand boxing/unboxing implications

---

## Angular Preparation

### Core Concepts to Master

#### 1. TypeScript Fundamentals
**Essential Topics**:
- Types (string, number, boolean, any, unknown, never)
- Interfaces and types
- Classes and inheritance
- Generics
- Enums
- Decorators
- Modules and imports
- Async operations (Promises, async/await)
- Arrow functions and closures

**Quick Review**:
```typescript
// Interface
interface User {
  id: number;
  name: string;
  email?: string; // Optional
}

// Generic function
function getFirstElement<T>(arr: T[]): T {
  return arr[0];
}

// Async/await
async function fetchUser(id: number): Promise<User> {
  const response = await fetch(`/api/users/${id}`);
  return response.json();
}
```

#### 2. JavaScript/TypeScript Array Methods
**Critical Methods**:
```typescript
// Transformation
const doubled = numbers.map(n => n * 2);
const filtered = numbers.filter(n => n > 5);
const sum = numbers.reduce((acc, n) => acc + n, 0);

// Searching
const found = numbers.find(n => n > 5);
const index = numbers.findIndex(n => n > 5);
const exists = numbers.some(n => n > 5);
const allPositive = numbers.every(n => n > 0);

// Sorting
const sorted = numbers.sort((a, b) => a - b);

// Combining
const combined = array1.concat(array2);
const flattened = array.flat();
const flatMapped = array.flatMap(x => [x, x * 2]);

// Other
const includes = numbers.includes(5);
const sliced = numbers.slice(0, 3);
const reversed = numbers.reverse();
```

#### 3. Object and String Operations
```typescript
// Object operations
const keys = Object.keys(obj);
const values = Object.values(obj);
const entries = Object.entries(obj);

// String operations
const upper = str.toUpperCase();
const lower = str.toLowerCase();
const trimmed = str.trim();
const split = str.split(',');
const joined = arr.join(',');
const includes = str.includes('substring');
const startsWith = str.startsWith('prefix');
const substring = str.substring(0, 5);
const replace = str.replace('old', 'new');
const regex = str.match(/pattern/g);
```

#### 4. Common Algorithms in JavaScript/TypeScript
**Pattern Recognition**:
- Two pointers
- Sliding window
- Hash map/Set for frequency
- Stack for matching brackets
- BFS/DFS for graphs
- Binary search

**Example Implementations**:
```typescript
// Two pointers - Two Sum
function twoSum(nums: number[], target: number): number[] {
  const map = new Map<number, number>();
  for (let i = 0; i < nums.length; i++) {
    const complement = target - nums[i];
    if (map.has(complement)) {
      return [map.get(complement)!, i];
    }
    map.set(nums[i], i);
  }
  return [];
}

// Sliding window - Max substring length
function maxSubstringLength(s: string): number {
  const charIndex = new Map<string, number>();
  let maxLen = 0;
  let start = 0;
  
  for (let end = 0; end < s.length; end++) {
    if (charIndex.has(s[end])) {
      start = Math.max(start, charIndex.get(s[end])! + 1);
    }
    charIndex.set(s[end], end);
    maxLen = Math.max(maxLen, end - start + 1);
  }
  
  return maxLen;
}
```

#### 5. Angular Framework Knowledge
**Key Concepts**:
- Components and templates
- Dependency injection
- Services and observables
- RxJS operators (map, filter, switchMap, mergeMap)
- Directives (*ngIf, *ngFor, *ngSwitch)
- Two-way binding
- Event binding
- Property binding
- Pipes
- HTTP client
- Routing
- Forms (Reactive and Template-driven)

**Common Patterns**:
```typescript
// Service with Observable
@Injectable()
export class UserService {
  constructor(private http: HttpClient) {}
  
  getUser(id: number): Observable<User> {
    return this.http.get<User>(`/api/users/${id}`);
  }
}

// Component using service
@Component({
  selector: 'app-user',
  template: `<div>{{ user$ | async }}</div>`
})
export class UserComponent {
  user$: Observable<User>;
  
  constructor(private userService: UserService) {
    this.user$ = this.userService.getUser(1);
  }
}
```

### Practice Problems for Angular

**Easy**:
1. Reverse array
2. Check palindrome
3. Count character frequency
4. Merge sorted arrays
5. Remove duplicates

**Medium**:
1. Longest substring without repeating characters
2. Group anagrams
3. Implement debounce function
4. Implement throttle function
5. Flatten nested array

**Hard**:
1. Implement promise.all
2. Implement async queue
3. Implement memoization
4. Implement observable-like behavior
5. Complex array transformations

### Angular-Specific Tips
1. Master array methods (map, filter, reduce)
2. Understand async/await and Promises
3. Be comfortable with closures and scope
4. Know RxJS operators
5. Understand dependency injection
6. Be familiar with TypeScript types
7. Know about event handling
8. Understand data binding

---

## Objective-C Preparation

### Core Concepts to Master

#### 1. Objective-C Language Fundamentals
**Essential Topics**:
- Classes and objects
- Properties and instance variables
- Methods (instance and class methods)
- Protocols and delegation
- Categories and extensions
- Blocks and closures
- Memory management (ARC - Automatic Reference Counting)
- Key-Value Coding (KVC)
- Key-Value Observing (KVO)
- Selectors and dynamic dispatch

**Quick Review**:
```objc
// Class definition
@interface MyClass : NSObject
@property (nonatomic, strong) NSString *name;
@property (nonatomic, assign) NSInteger age;

- (void)printInfo;
+ (instancetype)createWithName:(NSString *)name;
@end

@implementation MyClass
- (void)printInfo {
    NSLog(@"Name: %@, Age: %ld", self.name, (long)self.age);
}

+ (instancetype)createWithName:(NSString *)name {
    MyClass *instance = [[MyClass alloc] init];
    instance.name = name;
    return instance;
}
@end
```

#### 2. Foundation Framework Collections
**Common Collections**:
- `NSArray`: Immutable array
- `NSMutableArray`: Mutable array
- `NSDictionary`: Immutable dictionary
- `NSMutableDictionary`: Mutable dictionary
- `NSSet`: Immutable set
- `NSMutableSet`: Mutable set
- `NSString`: Immutable string
- `NSMutableString`: Mutable string

**Common Operations**:
```objc
// Array operations
NSArray *array = @[@1, @2, @3];
NSInteger count = array.count;
NSNumber *first = array.firstObject;
NSNumber *element = array[0];

NSMutableArray *mutableArray = [NSMutableArray arrayWithArray:array];
[mutableArray addObject:@4];
[mutableArray removeObject:@2];
[mutableArray removeObjectAtIndex:0];

// Dictionary operations
NSDictionary *dict = @{@"key": @"value"};
NSString *value = dict[@"key"];

NSMutableDictionary *mutableDict = [NSMutableDictionary dictionaryWithDictionary:dict];
mutableDict[@"newKey"] = @"newValue";
[mutableDict removeObjectForKey:@"key"];

// String operations
NSString *str = @"Hello World";
NSUInteger length = str.length;
NSString *upper = [str uppercaseString];
NSString *lower = [str lowercaseString];
NSRange range = [str rangeOfString:@"World"];
NSString *substring = [str substringFromIndex:6];
```

#### 3. Common Algorithms in Objective-C
**Data Structure Implementations**:
```objc
// Stack using NSMutableArray
NSMutableArray *stack = [NSMutableArray array];
[stack addObject:@1]; // Push
NSNumber *top = [stack lastObject]; // Peek
[stack removeLastObject]; // Pop

// Queue using NSMutableArray
NSMutableArray *queue = [NSMutableArray array];
[queue addObject:@1]; // Enqueue
NSNumber *front = [queue firstObject]; // Peek
[queue removeObjectAtIndex:0]; // Dequeue

// Hash map using NSDictionary
NSMutableDictionary *map = [NSMutableDictionary dictionary];
map[@"key"] = @"value";
BOOL exists = [map objectForKey:@"key"] != nil;
```

#### 4. Blocks and Higher-Order Functions
```objc
// Block definition and usage
void (^printBlock)(NSString *) = ^(NSString *text) {
    NSLog(@"%@", text);
};
printBlock(@"Hello");

// Blocks with array operations
NSArray *numbers = @[@1, @2, @3, @4, @5];
[numbers enumerateObjectsUsingBlock:^(NSNumber *obj, NSUInteger idx, BOOL *stop) {
    NSLog(@"%@", obj);
}];

// Filtering with blocks
NSPredicate *predicate = [NSPredicate predicateWithBlock:^BOOL(NSNumber *number, NSDictionary *bindings) {
    return [number integerValue] > 2;
}];
NSArray *filtered = [numbers filteredArrayUsingPredicate:predicate];

// Mapping with blocks
NSMutableArray *doubled = [NSMutableArray array];
[numbers enumerateObjectsUsingBlock:^(NSNumber *obj, NSUInteger idx, BOOL *stop) {
    [doubled addObject:@([obj integerValue] * 2)];
}];
```

#### 5. Memory Management (ARC)
**Key Concepts**:
- Strong references (default)
- Weak references (for delegates, to avoid cycles)
- Retain cycles and how to avoid them
- Dealloc method

**Example**:
```objc
@interface MyViewController : UIViewController
@property (nonatomic, strong) MyService *service; // Strong
@property (nonatomic, weak) id<MyDelegate> delegate; // Weak
@end

- (void)dealloc {
    // Cleanup if needed
    NSLog(@"MyViewController deallocated");
}
```

### Practice Problems for Objective-C

**Easy**:
1. Reverse array
2. Check palindrome string
3. Count character frequency
4. Merge sorted arrays
5. Find duplicates

**Medium**:
1. Implement custom collection class
2. Longest substring without repeating characters
3. Group anagrams
4. Serialize/deserialize object
5. Implement simple cache

**Hard**:
1. Implement custom data structure
2. Complex string manipulation
3. Implement observer pattern
4. Memory management challenges
5. Protocol-based design patterns

### Objective-C-Specific Tips
1. Master NSArray and NSDictionary operations
2. Understand blocks and closures
3. Know about memory management (ARC)
4. Be familiar with NSString operations
5. Understand protocols and delegation
6. Know about KVC and KVO
7. Understand selectors and dynamic dispatch
8. Be comfortable with Objective-C syntax

---

## Last-Minute Preparation (Today)

### If You Have 8 Hours
1. **Hour 1-2**: Review .NET LINQ and common patterns
2. **Hour 2-3**: Practice 2-3 medium .NET problems
3. **Hour 3-4**: Review JavaScript/TypeScript array methods
4. **Hour 4-5**: Practice 2-3 medium Angular problems
5. **Hour 5-6**: Review Objective-C collections
6. **Hour 6-7**: Practice 2-3 medium Objective-C problems
7. **Hour 7-8**: Mock test or review weak areas

### If You Have 4 Hours
1. **Hour 1**: Quick review of all three languages
2. **Hour 2**: Practice 1 problem in each language
3. **Hour 3**: Focus on your weakest area
4. **Hour 4**: Rest and mental preparation

### If You Have 2 Hours
1. **30 min**: Review syntax and common operations
2. **30 min**: Practice 1 problem in your strongest language
3. **30 min**: Review edge cases and error handling
4. **30 min**: Rest and calm down

### Critical Checklist Before the Test
- [ ] Understand the problem completely
- [ ] Identify the data structures needed
- [ ] Write pseudocode first
- [ ] Implement step by step
- [ ] Test with examples
- [ ] Check edge cases
- [ ] Verify time and space complexity
- [ ] Clean up code
- [ ] Submit with confidence

## During the Test

### Strategy
1. **Read Carefully** (2 minutes): Understand all requirements
2. **Plan** (3 minutes): Think about approach and data structures
3. **Code** (15 minutes): Write the solution
4. **Test** (5 minutes): Test with examples and edge cases
5. **Optimize** (5 minutes): Improve if time permits

### Time Management
- Don't spend more than 30 minutes on one problem
- If stuck, move to next problem and come back
- Prioritize correctness over optimization
- Aim to solve at least 2-3 problems completely

### Common Mistakes to Avoid
1. Not reading the problem completely
2. Forgetting edge cases
3. Off-by-one errors
4. Not handling null/empty inputs
5. Inefficient algorithms
6. Poor variable naming
7. Not testing the solution
8. Panicking when stuck

## Resources for Final Review

### .NET
- Microsoft C# documentation
- LINQ documentation
- LeetCode .NET problems

### Angular/TypeScript
- MDN JavaScript reference
- TypeScript handbook
- RxJS documentation

### Objective-C
- Apple Objective-C documentation
- Foundation framework reference
- NSArray/NSDictionary documentation

## Final Tips

1. **Get Good Sleep**: Rest is crucial for performance
2. **Eat Well**: Don't skip meals before the test
3. **Stay Calm**: Panic won't help
4. **Read Carefully**: Most mistakes come from misunderstanding
5. **Test Thoroughly**: Always verify your solution
6. **Manage Time**: Don't get stuck on one problem
7. **Ask Questions**: If something is unclear, ask
8. **Be Confident**: You've prepared well!

Good luck with your HackerRank round! Remember, the goal is to demonstrate your problem-solving skills and knowledge of the tech stack. Focus on writing clean, correct code and explaining your approach clearly.

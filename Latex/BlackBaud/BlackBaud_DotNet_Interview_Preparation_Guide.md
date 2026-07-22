# Blackbaud .NET Developer Interview Preparation Guide

## Table of Contents
1. [About Blackbaud](#about-blackbaud)
2. [Interview Process Overview](#interview-process-overview)
3. [Coding Round Preparation](#coding-round-preparation)
4. [Technical Interview Round](#technical-interview-round)
5. [.NET Specific Topics](#net-specific-topics)
6. [Behavioral Interview Round](#behavioral-interview-round)
7. [System Design Preparation](#system-design-preparation)
8. [Practice Resources](#practice-resources)
9. [Day-by-Day Study Plan](#day-by-day-study-plan)

---

## About Blackbaud

### Company Overview
- **Industry**: Software company specializing in cloud solutions for social good organizations (nonprofits, education, healthcare)
- **Focus**: Building software that powers fundraising, financial management, grant management, and CRM solutions
- **Mission**: Building software for good - helping nonprofits and educational institutions achieve their missions

### Tech Stack at Blackbaud
Based on recent job postings and employee reports:

**Backend:**
- C# / .NET Core / .NET 6+
- ASP.NET Core Web API
- RESTful Services
- Microservices Architecture

**Frontend:**
- Angular (TypeScript)
- JavaScript/TypeScript
- HTML5, CSS3
- Responsive Design

**Databases:**
- SQL Server
- Cosmos DB (NoSQL)
- Entity Framework Core

**Cloud & DevOps:**
- Microsoft Azure (Primary)
  - Azure Kubernetes Service (AKS)
  - Azure Storage
  - Azure Queue
  - Azure Key Vault
  - Azure Redis Cache
- CI/CD Pipelines
- Git / GitHub
- Docker & Kubernetes

**Other Technologies:**
- Agile/Scrum methodologies
- Unit Testing (xUnit, NUnit, MSTest)
- Integration Testing
- Security best practices
- OAuth/JWT authentication

---

## Interview Process Overview

### Timeline: 3-5 Weeks
The Blackbaud interview process typically consists of **3-4 rounds**:

```
Application → Phone Screen → Technical Interview → Onsite/Virtual → Offer
   (Week 1)      (Week 2)         (Week 3)           (Week 4-5)
```

### Round Breakdown

#### **Round 1: Recruiter Phone Screen (30 minutes)**
- **Focus**: Background, experience, motivation
- **Questions**:
  - Tell me about yourself
  - Why Blackbaud?
  - Walk through your resume
  - Salary expectations
  - Availability
- **Preparation**: Have your elevator pitch ready, research Blackbaud's mission

#### **Round 2: Technical Interview (60-90 minutes)**
- **Format**: Virtual coding session (collaborative coding platform)
- **Focus**: 
  - Data Structures & Algorithms
  - Problem-solving approach
  - Code quality and communication
  - .NET fundamentals
- **Difficulty**: Easy to Medium LeetCode-style problems
- **Languages**: C#, Java, or Python (your choice)

#### **Round 3: Onsite/Virtual Interview (3-4 hours)**
Multiple one-on-one sessions:
- **Technical Deep Dive** (60 min): Advanced coding, OOP concepts, design patterns
- **System Design** (45 min): Design scalable systems
- **Behavioral Interview** (45 min): STAR method questions, cultural fit
- **Manager Round** (30 min): Team fit, career goals

#### **Round 4: Final Discussion (Optional)**
- Meet with senior leadership
- Discuss compensation and benefits
- Q&A about the role and team

---

## Coding Round Preparation

### Expected Difficulty Level
- **35% Easy** (LeetCode Easy)
- **54% Medium** (LeetCode Medium)
- **11% Hard** (LeetCode Hard)

### Top Topics Asked at Blackbaud

#### **1. Arrays & Strings (High Priority)**
Most frequently asked topic at Blackbaud.

**Common Problems:**
- Two Sum / Three Sum
- Move Zeroes
- Search Insert Position
- Find the Index of First Occurrence in a String
- Zigzag Conversion
- Make String a Subsequence Using Cyclic Increments

**Practice Problems:**
```csharp
// Example: Two Sum
public int[] TwoSum(int[] nums, int target) {
    Dictionary<int, int> map = new Dictionary<int, int>();
    for (int i = 0; i < nums.Length; i++) {
        int complement = target - nums[i];
        if (map.ContainsKey(complement)) {
            return new int[] { map[complement], i };
        }
        map[nums[i]] = i;
    }
    return new int[] { };
}

// Example: Move Zeroes
public void MoveZeroes(int[] nums) {
    int left = 0;
    for (int right = 0; right < nums.Length; right++) {
        if (nums[right] != 0) {
            int temp = nums[left];
            nums[left] = nums[right];
            nums[right] = temp;
            left++;
        }
    }
}
```

#### **2. Binary Trees (High Priority)**
Second most common topic.

**Common Problems:**
- Maximum Depth of Binary Tree
- Binary Tree Vertical Order Traversal
- Closest Binary Search Tree Value
- Depth-First Search (DFS) implementation
- Reverse a Linked List

**Practice Problems:**
```csharp
// Example: Maximum Depth of Binary Tree
public int MaxDepth(TreeNode root) {
    if (root == null) return 0;
    return 1 + Math.Max(MaxDepth(root.left), MaxDepth(root.right));
}

// Example: Reverse Linked List
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
```

#### **3. Hash Tables / Dictionaries**
**Common Problems:**
- Two Sum (using HashMap)
- Group Anagrams
- First Unique Character
- Subarray Sum Equals K

```csharp
// Example: First Unique Character
public int FirstUniqChar(string s) {
    Dictionary<char, int> freq = new Dictionary<char, int>();
    foreach (char c in s) {
        if (freq.ContainsKey(c))
            freq[c]++;
        else
            freq[c] = 1;
    }
    for (int i = 0; i < s.Length; i++) {
        if (freq[s[i]] == 1)
            return i;
    }
    return -1;
}
```

#### **4. Stacks & Queues**
**Common Problems:**
- Implement Queue using Stacks
- Valid Parentheses
- Min Stack
- Reveal Cards In Increasing Order

```csharp
// Example: Implement Queue using Stacks
public class MyQueue {
    private Stack<int> input = new Stack<int>();
    private Stack<int> output = new Stack<int>();
    
    public void Push(int x) {
        input.Push(x);
    }
    
    public int Pop() {
        Peek();
        return output.Pop();
    }
    
    public int Peek() {
        if (output.Count == 0) {
            while (input.Count > 0) {
                output.Push(input.Pop());
            }
        }
        return output.Peek();
    }
    
    public bool Empty() {
        return input.Count == 0 && output.Count == 0;
    }
}
```

#### **5. Dynamic Programming**
**Common Problems:**
- Climbing Stairs
- House Robber
- Longest Common Subsequence
- Maximum Profit from Stock Prices

```csharp
// Example: Climbing Stairs
public int ClimbStairs(int n) {
    if (n <= 2) return n;
    int prev1 = 2, prev2 = 1;
    for (int i = 3; i <= n; i++) {
        int current = prev1 + prev2;
        prev2 = prev1;
        prev1 = current;
    }
    return prev1;
}
```

#### **6. Backtracking**
**Common Problems:**
- Generate Parentheses
- Letter Combinations of a Phone Number
- Permutations
- Subsets

```csharp
// Example: Generate Parentheses
public IList<string> GenerateParenthesis(int n) {
    List<string> result = new List<string>();
    Backtrack(result, "", 0, 0, n);
    return result;
}

private void Backtrack(List<string> result, string current, int open, int close, int max) {
    if (current.Length == max * 2) {
        result.Add(current);
        return;
    }
    if (open < max)
        Backtrack(result, current + "(", open + 1, close, max);
    if (close < open)
        Backtrack(result, current + ")", open, close + 1, max);
}
```

### Actual Problems Asked at Blackbaud

Based on recent interview reports, here are **26 confirmed problems**:

| Problem | Difficulty | Topic | Priority |
|---------|-----------|-------|----------|
| Two Sum | Easy | Array, Hash Table | ⭐⭐⭐ |
| Move Zeroes | Easy | Array, Two Pointers | ⭐⭐⭐ |
| Search Insert Position | Easy | Array, Binary Search | ⭐⭐⭐ |
| Maximum Depth of Binary Tree | Easy | Tree, DFS | ⭐⭐⭐ |
| Implement Queue using Stacks | Easy | Stack, Queue | ⭐⭐⭐ |
| Valid Palindrome | Easy | String, Two Pointers | ⭐⭐⭐ |
| Reverse Linked List | Easy | Linked List | ⭐⭐⭐ |
| Find First Occurrence in String | Easy | String | ⭐⭐ |
| Toeplitz Matrix | Easy | Array, Matrix | ⭐⭐ |
| Zigzag Conversion | Medium | String | ⭐⭐⭐ |
| Generate Parentheses | Medium | Backtracking | ⭐⭐⭐ |
| Letter Combinations Phone Number | Medium | Backtracking | ⭐⭐⭐ |
| Binary Tree Vertical Order Traversal | Medium | Tree, BFS | ⭐⭐⭐ |
| Closest Binary Search Tree Value | Medium | Tree, BST | ⭐⭐ |
| Longest Repeating Character Replacement | Medium | String, Sliding Window | ⭐⭐ |
| Koko Eating Bananas | Medium | Binary Search | ⭐⭐ |
| Two Best Non-Overlapping Events | Medium | Array, DP | ⭐ |
| Construct String With Repeat Limit | Medium | String, Greedy | ⭐ |
| Rank Teams by Votes | Medium | Array, Sorting | ⭐ |
| Reveal Cards In Increasing Order | Medium | Array, Queue | ⭐ |
| Make String Subsequence Cyclic | Medium | String, Greedy | ⭐⭐ |
| Evaluate Division | Medium | Graph, DFS | ⭐⭐ |
| Subarrays with K Different Integers | Hard | Array, Sliding Window | ⭐ |
| Find Longest Self-Contained Substring | Hard | String | ⭐ |

### Coding Interview Tips

#### **During the Interview:**
1. **Clarify Requirements**
   - Ask about input constraints
   - Confirm expected output format
   - Discuss edge cases

2. **Think Aloud**
   - Explain your thought process
   - Discuss trade-offs
   - Mention time/space complexity

3. **Start with Brute Force**
   - Explain the naive solution first
   - Then optimize

4. **Write Clean Code**
   - Use meaningful variable names
   - Follow C# naming conventions
   - Add comments for complex logic

5. **Test Your Code**
   - Walk through with sample input
   - Test edge cases
   - Fix bugs systematically

#### **Common Mistakes to Avoid:**
- ❌ Jumping into coding without planning
- ❌ Not asking clarifying questions
- ❌ Silent coding (not explaining your approach)
- ❌ Ignoring edge cases
- ❌ Not testing your solution
- ❌ Getting stuck on one approach (be flexible)

---

## Technical Interview Round

### .NET Framework & C# Fundamentals

#### **C# Language Features**

**1. Value Types vs Reference Types**
```csharp
// Value Type (stored on stack)
int x = 10;
int y = x;  // Copy of value
y = 20;     // x is still 10

// Reference Type (stored on heap)
class Person { public string Name; }
Person p1 = new Person { Name = "John" };
Person p2 = p1;  // Copy of reference
p2.Name = "Jane"; // p1.Name is also "Jane"
```

**Interview Questions:**
- What's the difference between `struct` and `class`?
- When would you use a `struct` over a `class`?
- Explain boxing and unboxing with performance implications

**2. LINQ (Language Integrated Query)**
```csharp
// Common LINQ operations
var numbers = new[] { 1, 2, 3, 4, 5 };

// Filtering
var evens = numbers.Where(n => n % 2 == 0);

// Projection
var squares = numbers.Select(n => n * n);

// Aggregation
var sum = numbers.Sum();
var max = numbers.Max();

// Grouping
var grouped = students.GroupBy(s => s.Grade);

// Joining
var result = orders.Join(customers,
    order => order.CustomerId,
    customer => customer.Id,
    (order, customer) => new { order, customer });
```

**Interview Questions:**
- Difference between `IEnumerable` and `IQueryable`?
- What is deferred execution in LINQ?
- Explain `Select` vs `SelectMany`

**3. Async/Await Pattern**
```csharp
// Async method
public async Task<string> GetDataAsync() {
    using (HttpClient client = new HttpClient()) {
        string result = await client.GetStringAsync("https://api.example.com");
        return result;
    }
}

// Calling async method
public async Task ProcessDataAsync() {
    string data = await GetDataAsync();
    Console.WriteLine(data);
}

// Parallel async operations
public async Task<List<string>> GetMultipleDataAsync() {
    var task1 = GetDataAsync("url1");
    var task2 = GetDataAsync("url2");
    var task3 = GetDataAsync("url3");
    
    await Task.WhenAll(task1, task2, task3);
    
    return new List<string> { 
        await task1, 
        await task2, 
        await task3 
    };
}
```

**Interview Questions:**
- Difference between `async void` and `async Task`?
- When to use `Task.WhenAll` vs `Task.WhenAny`?
- What happens if you don't await an async method?
- Explain `ConfigureAwait(false)`

**4. Delegates, Events, and Lambda Expressions**
```csharp
// Delegate
public delegate void NotifyHandler(string message);

// Event
public event NotifyHandler OnNotify;

// Lambda expression
Func<int, int, int> add = (a, b) => a + b;
Action<string> print = msg => Console.WriteLine(msg);

// Event usage
public void RaiseEvent() {
    OnNotify?.Invoke("Event triggered");
}
```

**5. Generics**
```csharp
// Generic class
public class Repository<T> where T : class {
    private List<T> items = new List<T>();
    
    public void Add(T item) => items.Add(item);
    public T Get(int index) => items[index];
}

// Generic method
public T Max<T>(T a, T b) where T : IComparable<T> {
    return a.CompareTo(b) > 0 ? a : b;
}

// Generic constraints
public class MyClass<T> where T : IDisposable, new() {
    // T must implement IDisposable and have parameterless constructor
}
```

**6. Exception Handling**
```csharp
try {
    // Risky operation
    var result = await GetDataAsync();
}
catch (HttpRequestException ex) {
    // Specific exception
    _logger.LogError(ex, "HTTP request failed");
    throw; // Re-throw to preserve stack trace
}
catch (Exception ex) {
    // General exception
    _logger.LogError(ex, "Unexpected error");
    throw new ApplicationException("Operation failed", ex);
}
finally {
    // Cleanup code (always executes)
    connection?.Dispose();
}
```

### ASP.NET Core Web API

#### **1. Creating a RESTful API**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase {
    private readonly IProductService _productService;
    
    public ProductsController(IProductService productService) {
        _productService = productService;
    }
    
    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }
    
    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var product = await _productService.CreateAsync(productDto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    
    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto) {
        await _productService.UpdateAsync(id, productDto);
        return NoContent();
    }
    
    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
```

#### **2. Dependency Injection**
```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
```

**Service Lifetimes:**
- **Transient**: Created each time requested (lightweight, stateless services)
- **Scoped**: Created once per request (DbContext, repositories)
- **Singleton**: Created once for application lifetime (caching, configuration)

#### **3. Middleware Pipeline**
```csharp
var app = builder.Build();

// Middleware order matters!
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Custom middleware
public class RequestLoggingMiddleware {
    private readonly RequestDelegate _next;
    
    public RequestLoggingMiddleware(RequestDelegate next) {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context) {
        // Before request
        Console.WriteLine($"Request: {context.Request.Path}");
        
        await _next(context);
        
        // After request
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}
```

#### **4. Authentication & Authorization**
```csharp
// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("MinimumAge", policy => 
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});

// Using in controller
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProduct(int id) {
    // Only admins can access
}
```

### Entity Framework Core

#### **1. DbContext Configuration**
```csharp
public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Fluent API configuration
        modelBuilder.Entity<Product>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            // Relationships
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId);
        });
        
        // Seed data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" }
        );
    }
}
```

#### **2. CRUD Operations**
```csharp
public class ProductRepository : IProductRepository {
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context) {
        _context = context;
    }
    
    // Read
    public async Task<IEnumerable<Product>> GetAllAsync() {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
    
    public async Task<Product> GetByIdAsync(int id) {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    // Create
    public async Task<Product> AddAsync(Product product) {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
    
    // Update
    public async Task UpdateAsync(Product product) {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    // Delete
    public async Task DeleteAsync(int id) {
        var product = await _context.Products.FindAsync(id);
        if (product != null) {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
```

#### **3. Advanced Queries**
```csharp
// Eager Loading
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Reviews)
    .ToListAsync();

// Explicit Loading
var product = await _context.Products.FindAsync(id);
await _context.Entry(product)
    .Collection(p => p.Reviews)
    .LoadAsync();

// Lazy Loading (requires Microsoft.EntityFrameworkCore.Proxies)
public virtual ICollection<Review> Reviews { get; set; }

// Raw SQL
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 100)
    .ToListAsync();

// Transactions
using (var transaction = await _context.Database.BeginTransactionAsync()) {
    try {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        
        await _context.Inventory.AddAsync(inventory);
        await _context.SaveChangesAsync();
        
        await transaction.CommitAsync();
    }
    catch {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### Design Patterns

#### **1. Repository Pattern**
```csharp
public interface IRepository<T> where T : class {
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class GenericRepository<T> : IRepository<T> where T : class {
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public GenericRepository(AppDbContext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync() {
        return await _dbSet.ToListAsync();
    }
    
    // ... other implementations
}
```

#### **2. Unit of Work Pattern**
```csharp
public interface IUnitOfWork : IDisposable {
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork {
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context) {
        _context = context;
        Products = new ProductRepository(_context);
        Categories = new CategoryRepository(_context);
    }
    
    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    
    public async Task<int> SaveChangesAsync() {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose() {
        _context.Dispose();
    }
}
```

#### **3. Singleton Pattern**
```csharp
public sealed class ConfigurationManager {
    private static readonly Lazy<ConfigurationManager> _instance = 
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());
    
    private ConfigurationManager() {
        // Private constructor
    }
    
    public static ConfigurationManager Instance => _instance.Value;
    
    public string GetSetting(string key) {
        // Implementation
    }
}
```

#### **4. Factory Pattern**
```csharp
public interface INotificationService {
    void Send(string message);
}

public class EmailNotification : INotificationService {
    public void Send(string message) => Console.WriteLine($"Email: {message}");
}

public class SmsNotification : INotificationService {
    public void Send(string message) => Console.WriteLine($"SMS: {message}");
}

public class NotificationFactory {
    public INotificationService CreateNotification(string type) {
        return type.ToLower() switch {
            "email" => new EmailNotification(),
            "sms" => new SmsNotification(),
            _ => throw new ArgumentException("Invalid notification type")
        };
    }
}
```

#### **5. Strategy Pattern**
```csharp
public interface IPaymentStrategy {
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy {
    public void Pay(decimal amount) => Console.WriteLine($"Paid ${amount} with Credit Card");
}

public class PayPalPayment : IPaymentStrategy {
    public void Pay(decimal amount) => Console.WriteLine($"Paid ${amount} with PayPal");
}

public class PaymentContext {
    private IPaymentStrategy _strategy;
    
    public void SetStrategy(IPaymentStrategy strategy) {
        _strategy = strategy;
    }
    
    public void ExecutePayment(decimal amount) {
        _strategy.Pay(amount);
    }
}
```

### Common .NET Interview Questions

#### **Q1: What's the difference between `IEnumerable` and `IQueryable`?**

| Feature                | IEnumerable           | IQueryable       |
| ------------------------| -----------------------| ------------------|
| **Namespace**          | System.Collections    | System.Linq      |
| **Execution**          | In-memory             | Database-side    |
| **Best for**           | In-memory collections | Database queries |
| **Deferred Execution** | Yes                   | Yes              |
| **Expression Trees**   | No                    | Yes              |

```csharp
// IEnumerable - loads all data then filters
IEnumerable<Product> products = _context.Products.ToList();
var filtered = products.Where(p => p.Price > 100); // Filters in memory

// IQueryable - filters at database level
IQueryable<Product> products = _context.Products;
var filtered = products.Where(p => p.Price > 100); // SQL WHERE clause
```

#### **Q2: Explain the difference between `Task` and `Thread`**

| Feature | Thread | Task |
|---------|--------|------|
| **Level** | Low-level | High-level abstraction |
| **Overhead** | Heavy | Light |
| **Return Value** | No | Yes (Task<T>) |
| **Exception Handling** | Complex | Built-in |
| **Cancellation** | Manual | Built-in (CancellationToken) |

```csharp
// Thread
Thread thread = new Thread(() => {
    Console.WriteLine("Thread running");
});
thread.Start();
thread.Join();

// Task (preferred)
Task task = Task.Run(() => {
    Console.WriteLine("Task running");
});
await task;
```

#### **Q3: What are the SOLID principles?**

**S - Single Responsibility Principle**
```csharp
// Bad: Class has multiple responsibilities
public class User {
    public void SaveToDatabase() { }
    public void SendEmail() { }
}

// Good: Separate responsibilities
public class User { }
public class UserRepository {
    public void Save(User user) { }
}
public class EmailService {
    public void SendEmail(User user) { }
}
```

**O - Open/Closed Principle**
```csharp
// Open for extension, closed for modification
public abstract class Shape {
    public abstract double CalculateArea();
}

public class Circle : Shape {
    public double Radius { get; set; }
    public override double CalculateArea() => Math.PI * Radius * Radius;
}

public class Rectangle : Shape {
    public double Width { get; set; }
    public double Height { get; set; }
    public override double CalculateArea() => Width * Height;
}
```

**L - Liskov Substitution Principle**
```csharp
// Derived classes should be substitutable for base classes
public class Bird {
    public virtual void Fly() { }
}

public class Sparrow : Bird {
    public override void Fly() => Console.WriteLine("Sparrow flying");
}

// Bad: Penguin can't fly
public class Penguin : Bird {
    public override void Fly() => throw new NotImplementedException();
}
```

**I - Interface Segregation Principle**
```csharp
// Bad: Fat interface
public interface IWorker {
    void Work();
    void Eat();
    void Sleep();
}

// Good: Segregated interfaces
public interface IWorkable {
    void Work();
}

public interface IFeedable {
    void Eat();
}
```

**D - Dependency Inversion Principle**
```csharp
// Depend on abstractions, not concretions
public interface ILogger {
    void Log(string message);
}

public class FileLogger : ILogger {
    public void Log(string message) => File.AppendAllText("log.txt", message);
}

public class UserService {
    private readonly ILogger _logger;
    
    public UserService(ILogger logger) {
        _logger = logger; // Depends on abstraction
    }
}
```

#### **Q4: What's the difference between `String` and `StringBuilder`?**

```csharp
// String (immutable) - creates new object each time
string str = "Hello";
str += " World"; // New string object created
str += "!";      // Another new string object

// StringBuilder (mutable) - modifies same object
StringBuilder sb = new StringBuilder("Hello");
sb.Append(" World"); // Same object modified
sb.Append("!");      // Same object modified
string result = sb.ToString();

// Performance comparison
// String: O(n²) for n concatenations
// StringBuilder: O(n) for n concatenations
```

**When to use:**
- **String**: Few concatenations, immutability needed
- **StringBuilder**: Many concatenations, loops, performance-critical

#### **Q5: Explain garbage collection in .NET**

**Generations:**
- **Gen 0**: Short-lived objects (newly created)
- **Gen 1**: Medium-lived objects (survived one collection)
- **Gen 2**: Long-lived objects (survived multiple collections)

```csharp
// Force garbage collection (avoid in production)
GC.Collect();
GC.WaitForPendingFinalizers();

// Dispose pattern for unmanaged resources
public class ResourceHolder : IDisposable {
    private bool disposed = false;
    
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                // Dispose managed resources
            }
            // Free unmanaged resources
            disposed = true;
        }
    }
    
    ~ResourceHolder() {
        Dispose(false);
    }
}

// Using statement (automatic disposal)
using (var resource = new ResourceHolder()) {
    // Use resource
} // Automatically disposed
```

#### **Q6: What's the difference between `abstract class` and `interface`?**

| Feature                  | Abstract Class    | Interface          |
| --------------------------| -------------------| --------------------|
| **Multiple Inheritance** | No                | Yes                |
| **Implementation**       | Can have          | No (before C# 8.0) |
| **Access Modifiers**     | Yes               | No (all public)    |
| **Fields**               | Yes               | No                 |
| **Constructor**          | z Yes             | No                 |
| **When to use**          | IS-A relationship | CAN-DO capability  |

```csharp
// Abstract class
public abstract class Animal {
    protected string Name { get; set; }
    
    public Animal(string name) {
        Name = name;
    }
    
    public abstract void MakeSound();
    
    public void Sleep() {
        Console.WriteLine($"{Name} is sleeping");
    }
}

// Interface
public interface IFlyable {
    void Fly();
}

public interface ISwimmable {
    void Swim();
}

// Class can inherit one abstract class and multiple interfaces
public class Duck : Animal, IFlyable, ISwimmable {
    public Duck(string name) : base(name) { }
    
    public override void MakeSound() => Console.WriteLine("Quack!");
    public void Fly() => Console.WriteLine("Flying");
    public void Swim() => Console.WriteLine("Swimming");
}
```

#### **Q7: Explain middleware in ASP.NET Core**

```csharp
// Middleware pipeline
app.Use(async (context, next) => {
    // Before next middleware
    Console.WriteLine("Middleware 1: Before");
    
    await next.Invoke();
    
    // After next middleware
    Console.WriteLine("Middleware 1: After");
});

app.Use(async (context, next) => {
    Console.WriteLine("Middleware 2: Before");
    await next.Invoke();
    Console.WriteLine("Middleware 2: After");
});

app.Run(async context => {
    Console.WriteLine("Terminal middleware");
    await context.Response.WriteAsync("Hello World");
});

// Output:
// Middleware 1: Before
// Middleware 2: Before
// Terminal middleware
// Middleware 2: After
// Middleware 1: After
```

#### **Q8: What's the difference between `FirstOrDefault` and `SingleOrDefault`?**

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// FirstOrDefault: Returns first element or default
var first = numbers.FirstOrDefault(n => n > 3); // Returns 4
var notFound = numbers.FirstOrDefault(n => n > 10); // Returns 0

// SingleOrDefault: Returns single element or throws if multiple
var single = numbers.SingleOrDefault(n => n == 3); // Returns 3
var multiple = numbers.SingleOrDefault(n => n > 3); // Throws InvalidOperationException
var notFound2 = numbers.SingleOrDefault(n => n > 10); // Returns 0
```

**Use cases:**
- **FirstOrDefault**: When multiple matches are possible, you want the first
- **SingleOrDefault**: When you expect exactly zero or one match

---

## Behavioral Interview Round

### STAR Method Framework

**S**ituation → **T**ask → **A**ction → **R**esult

### Common Behavioral Questions

#### **1. Tell me about yourself**
**Structure:**
- Current role (30 seconds)
- Relevant experience (1 minute)
- Why Blackbaud (30 seconds)

**Example:**
> "I'm a .NET developer with 5 years of experience building scalable web applications. Currently at [Company], I work on microservices architecture using .NET Core and Azure. I've led the migration of a monolithic application to microservices, improving performance by 40%. I'm excited about Blackbaud's mission of building software for social good, and I'd love to contribute my expertise in .NET and cloud technologies to help nonprofits achieve their goals."

#### **2. Why Blackbaud?**
**Key Points:**
- Mission-driven company (software for social good)
- Technology stack alignment (.NET, Azure, Angular)
- Growth opportunities
- Company culture

**Example:**
> "I'm drawn to Blackbaud for three main reasons. First, the mission resonates with me – using technology to help nonprofits and educational institutions is incredibly meaningful. Second, the tech stack aligns perfectly with my expertise in .NET Core, Azure, and Angular. Finally, I've heard great things about Blackbaud's collaborative culture and commitment to professional development."

#### **3. Describe a challenging bug you fixed**
**STAR Example:**
- **Situation**: Production API was experiencing intermittent 500 errors
- **Task**: Identify root cause and fix without downtime
- **Action**: 
  - Analyzed logs and found deadlock in database
  - Implemented async/await pattern properly
  - Added retry logic with exponential backoff
  - Improved database indexing
- **Result**: Reduced errors by 99%, improved response time by 30%

#### **4. Tell me about a time you disagreed with a team member**
**STAR Example:**
- **Situation**: Team wanted to use NoSQL for all data storage
- **Task**: Evaluate best database choice for each use case
- **Action**:
  - Presented pros/cons of SQL vs NoSQL
  - Conducted POC with both approaches
  - Measured performance metrics
  - Facilitated team discussion
- **Result**: Hybrid approach – SQL for transactional data, NoSQL for analytics

#### **5. Describe a time you had to learn a new technology quickly**
**STAR Example:**
- **Situation**: Project required Azure Functions, which I hadn't used
- **Task**: Implement serverless architecture in 2 weeks
- **Action**:
  - Completed Microsoft Learn modules
  - Built small POC projects
  - Paired with experienced colleague
  - Read documentation and best practices
- **Result**: Successfully delivered on time, now team's go-to person for Azure Functions

#### **6. How do you handle tight deadlines?**
**Key Points:**
- Prioritization (MVP first)
- Communication with stakeholders
- Breaking down tasks
- Time management

#### **7. Tell me about a time you improved code quality**
**STAR Example:**
- **Situation**: Legacy codebase with 30% test coverage
- **Task**: Improve code quality and maintainability
- **Action**:
  - Introduced code review process
  - Set up SonarQube for static analysis
  - Wrote unit tests for critical paths
  - Refactored using SOLID principles
- **Result**: Increased test coverage to 80%, reduced bugs by 50%

#### **8. Describe your experience with Agile/Scrum**
**Key Points:**
- Daily standups
- Sprint planning and retrospectives
- User stories and acceptance criteria
- Continuous integration/deployment

### Questions to Ask Interviewer

**About the Role:**
- What does a typical day look like for this position?
- What are the biggest challenges facing the team right now?
- What technologies will I work with day-to-day?
- How is success measured for this role?

**About the Team:**
- Can you tell me about the team structure?
- What's the team's approach to code reviews?
- How does the team handle technical debt?
- What's the balance between new features and maintenance?

**About the Company:**
- How does Blackbaud support professional development?
- What's the deployment process like?
- How does Blackbaud measure impact on nonprofit clients?
- What are the company's technical priorities for the next year?

**About Growth:**
- What are the career progression opportunities?
- Does Blackbaud support conference attendance or certifications?
- Are there opportunities to work on different products/teams?

---

## System Design Preparation

### Common System Design Questions

#### **1. Design a URL Shortener (like bit.ly)**

**Requirements:**
- Shorten long URLs to short codes
- Redirect short URLs to original URLs
- Track click analytics
- Handle 1000 requests/second

**High-Level Design:**
```
┌─────────┐      ┌──────────────┐      ┌──────────┐
│ Client  │─────▶│  API Gateway │─────▶│ Web API  │
└─────────┘      └──────────────┘      └────┬─────┘
                                             │
                         ┌───────────────────┼───────────────┐
                         ▼                   ▼               ▼
                   ┌──────────┐      ┌──────────┐   ┌──────────┐
                   │ SQL DB   │      │  Redis   │   │  Queue   │
                   │(Mappings)│      │ (Cache)  │   │(Analytics)│
                   └──────────┘      └──────────┘   └──────────┘
```

**Key Components:**
1. **URL Encoding**: Base62 encoding (a-z, A-Z, 0-9)
2. **Database**: Store URL mappings
3. **Cache**: Redis for frequently accessed URLs
4. **Analytics**: Queue for async processing

**C# Implementation Sketch:**
```csharp
public class UrlShortenerService {
    private readonly IUrlRepository _repository;
    private readonly IDistributedCache _cache;
    
    public async Task<string> ShortenUrl(string longUrl) {
        // Generate short code
        string shortCode = GenerateShortCode();
        
        // Store in database
        await _repository.SaveAsync(new UrlMapping {
            ShortCode = shortCode,
            LongUrl = longUrl,
            CreatedAt = DateTime.UtcNow
        });
        
        // Cache it
        await _cache.SetStringAsync(shortCode, longUrl);
        
        return $"https://short.url/{shortCode}";
    }
    
    public async Task<string> GetLongUrl(string shortCode) {
        // Try cache first
        var cached = await _cache.GetStringAsync(shortCode);
        if (cached != null) return cached;
        
        // Fallback to database
        var mapping = await _repository.GetByShortCodeAsync(shortCode);
        if (mapping != null) {
            await _cache.SetStringAsync(shortCode, mapping.LongUrl);
            return mapping.LongUrl;
        }
        
        return null;
    }
    
    private string GenerateShortCode() {
        // Base62 encoding of timestamp + random
        return Base62Encode(DateTime.UtcNow.Ticks + Random.Next());
    }
}
```

#### **2. Design a Rate Limiter**

**Requirements:**
- Limit API requests per user (e.g., 100 requests/minute)
- Return 429 Too Many Requests when exceeded
- Distributed system support

**Algorithms:**
1. **Token Bucket**: Refill tokens at fixed rate
2. **Sliding Window**: Track requests in time window
3. **Fixed Window Counter**: Reset counter every window

**C# Implementation (Token Bucket):**
```csharp
public class RateLimiter {
    private readonly IDistributedCache _cache;
    private readonly int _maxTokens = 100;
    private readonly TimeSpan _refillInterval = TimeSpan.FromMinutes(1);
    
    public async Task<bool> AllowRequest(string userId) {
        string key = $"rate_limit:{userId}";
        
        var data = await _cache.GetStringAsync(key);
        var bucket = data != null 
            ? JsonSerializer.Deserialize<TokenBucket>(data)
            : new TokenBucket { Tokens = _maxTokens, LastRefill = DateTime.UtcNow };
        
        // Refill tokens
        var elapsed = DateTime.UtcNow - bucket.LastRefill;
        var tokensToAdd = (int)(elapsed.TotalMinutes * _maxTokens);
        bucket.Tokens = Math.Min(_maxTokens, bucket.Tokens + tokensToAdd);
        bucket.LastRefill = DateTime.UtcNow;
        
        // Check if request allowed
        if (bucket.Tokens > 0) {
            bucket.Tokens--;
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(bucket));
            return true;
        }
        
        return false;
    }
}

public class TokenBucket {
    public int Tokens { get; set; }
    public DateTime LastRefill { get; set; }
}
```

#### **3. Design a Notification System**

**Requirements:**
- Send email, SMS, push notifications
- Handle millions of notifications
- Retry failed notifications
- Priority queues

**Architecture:**
```
┌─────────┐      ┌──────────┐      ┌────────────┐
│ Service │─────▶│  Queue   │─────▶│  Workers   │
└─────────┘      │(RabbitMQ)│      │(Consumers) │
                 └──────────┘      └─────┬──────┘
                                         │
                     ┌───────────────────┼──────────────┐
                     ▼                   ▼              ▼
              ┌────────────┐      ┌──────────┐  ┌──────────┐
              │Email Service│      │SMS Service│  │Push Service│
              └────────────┘      └──────────┘  └──────────┘
```

**C# Implementation:**
```csharp
public interface INotificationService {
    Task SendAsync(Notification notification);
}

public class NotificationService : INotificationService {
    private readonly IMessageQueue _queue;
    
    public async Task SendAsync(Notification notification) {
        // Enqueue notification
        await _queue.PublishAsync("notifications", notification);
    }
}

public class NotificationWorker : BackgroundService {
    private readonly IMessageQueue _queue;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await _queue.SubscribeAsync("notifications", async (notification) => {
            try {
                switch (notification.Type) {
                    case NotificationType.Email:
                        await _emailService.SendAsync(notification);
                        break;
                    case NotificationType.Sms:
                        await _smsService.SendAsync(notification);
                        break;
                }
            }
            catch (Exception ex) {
                // Retry logic
                await RetryAsync(notification);
            }
        }, stoppingToken);
    }
}
```

### System Design Principles

1. **Scalability**: Horizontal scaling, load balancing
2. **Reliability**: Redundancy, failover, retry logic
3. **Performance**: Caching, CDN, database optimization
4. **Security**: Authentication, authorization, encryption
5. **Maintainability**: Clean code, monitoring, logging

---

## Practice Resources

### Coding Practice Platforms

1. **LeetCode** (Primary)
   - Focus on Easy and Medium problems
   - Complete Blackbaud-specific problems list
   - Target: 50-100 problems
   - URL: https://leetcode.com

2. **HackerRank**
   - .NET specific challenges
   - Problem Solving track
   - URL: https://www.hackerrank.com

3. **Codewars**
   - C# kata challenges
   - Good for syntax practice
   - URL: https://www.codewars.com

4. **Exercism**
   - C# track with mentorship
   - Great for learning idioms
   - URL: https://exercism.org/tracks/csharp

### .NET Learning Resources

1. **Microsoft Learn**
   - Free official tutorials
   - ASP.NET Core path
   - URL: https://learn.microsoft.com/en-us/aspnet/core

2. **Pluralsight**
   - ASP.NET Core courses
   - Design Patterns in C#
   - Entity Framework Core

3. **YouTube Channels**
   - Nick Chapsas (C# best practices)
   - IAmTimCorey (C# tutorials)
   - Raw Coding (ASP.NET Core)

4. **Books**
   - "C# in Depth" by Jon Skeet
   - "Pro ASP.NET Core 6" by Adam Freeman
   - "Clean Code" by Robert C. Martin

### System Design Resources

1. **System Design Primer** (GitHub)
   - URL: https://github.com/donnemartin/system-design-primer

2. **Grokking the System Design Interview**
   - URL: https://www.educative.io

3. **YouTube**
   - Gaurav Sen (System Design)
   - Tech Dummies (System Design)

### Mock Interviews

1. **Pramp** - Free peer mock interviews
2. **Interviewing.io** - Anonymous mock interviews
3. **LeetCode Mock** - Timed coding assessments

---

## Day-by-Day Study Plan

### 4-Week Preparation Plan

#### **Week 1: Foundations**

**Day 1-2: Arrays & Strings**
- [ ] Two Sum
- [ ] Move Zeroes
- [ ] Valid Palindrome
- [ ] First Unique Character
- [ ] Longest Substring Without Repeating Characters

**Day 3-4: Linked Lists & Stacks**
- [ ] Reverse Linked List
- [ ] Merge Two Sorted Lists
- [ ] Valid Parentheses
- [ ] Implement Queue using Stacks
- [ ] Min Stack

**Day 5-6: Binary Trees**
- [ ] Maximum Depth of Binary Tree
- [ ] Invert Binary Tree
- [ ] Binary Tree Level Order Traversal
- [ ] Validate Binary Search Tree
- [ ] Lowest Common Ancestor

**Day 7: Review & Mock Interview**
- [ ] Review all problems from Week 1
- [ ] Take a timed mock assessment
- [ ] Identify weak areas

#### **Week 2: Intermediate Topics**

**Day 8-9: Hash Tables & Sorting**
- [ ] Group Anagrams
- [ ] Top K Frequent Elements
- [ ] Merge Intervals
- [ ] Sort Colors
- [ ] Find All Anagrams in String

**Day 10-11: Dynamic Programming**
- [ ] Climbing Stairs
- [ ] House Robber
- [ ] Coin Change
- [ ] Longest Increasing Subsequence
- [ ] Maximum Subarray

**Day 12-13: Backtracking**
- [ ] Generate Parentheses
- [ ] Letter Combinations of Phone Number
- [ ] Permutations
- [ ] Subsets
- [ ] Combination Sum

**Day 14: Review & Practice**
- [ ] Review Week 2 problems
- [ ] Practice explaining solutions aloud
- [ ] Time yourself on 3-4 problems

#### **Week 3: .NET Deep Dive**

**Day 15-16: C# Fundamentals**
- [ ] Study async/await patterns
- [ ] Practice LINQ queries
- [ ] Review delegates and events
- [ ] Understand generics and constraints
- [ ] Memory management and GC

**Day 17-18: ASP.NET Core**
- [ ] Build a simple REST API
- [ ] Implement authentication (JWT)
- [ ] Practice dependency injection
- [ ] Create custom middleware
- [ ] Add validation and error handling

**Day 19-20: Entity Framework Core**
- [ ] Set up DbContext
- [ ] Practice CRUD operations
- [ ] Implement relationships
- [ ] Write complex queries
- [ ] Use migrations

**Day 21: Design Patterns**
- [ ] Repository pattern
- [ ] Unit of Work
- [ ] Factory pattern
- [ ] Singleton pattern
- [ ] Strategy pattern

#### **Week 4: Advanced Topics & Mock Interviews**

**Day 22-23: System Design**
- [ ] Design URL shortener
- [ ] Design rate limiter
- [ ] Design notification system
- [ ] Design caching layer
- [ ] Practice explaining trade-offs

**Day 24-25: Blackbaud-Specific Problems**
- [ ] Solve all 26 Blackbaud problems
- [ ] Focus on medium difficulty
- [ ] Practice on collaborative coding platform
- [ ] Time yourself (45 min per problem)

**Day 26-27: Mock Interviews**
- [ ] Full coding mock interview (2 problems, 60 min)
- [ ] System design mock interview (45 min)
- [ ] Behavioral interview practice (STAR method)
- [ ] Get feedback and improve

**Day 28: Final Review**
- [ ] Review all weak areas
- [ ] Practice elevator pitch
- [ ] Prepare questions for interviewer
- [ ] Review Blackbaud's mission and products
- [ ] Get good rest before interview

### Daily Study Schedule (2-3 hours/day)

**Morning (1 hour):**
- 2-3 LeetCode problems
- Focus on problem-solving

**Evening (1-2 hours):**
- .NET concepts and practice
- Build small projects
- Review notes

**Weekend (4-5 hours):**
- Mock interviews
- System design practice
- Review and consolidation

---

## Final Tips for Success

### Before the Interview

✅ **Technical Preparation**
- Solve at least 50-100 LeetCode problems
- Focus on Easy and Medium difficulty
- Master the 26 Blackbaud-specific problems
- Build 2-3 small .NET projects

✅ **Research**
- Study Blackbaud's products (Raiser's Edge, Financial Edge, etc.)
- Understand their mission and values
- Read recent news and blog posts
- Check Glassdoor reviews

✅ **Logistics**
- Test your internet connection
- Set up collaborative coding environment
- Prepare your workspace (quiet, good lighting)
- Have pen and paper ready

### During the Interview

✅ **Communication**
- Think aloud - explain your approach
- Ask clarifying questions
- Discuss trade-offs
- Be receptive to hints

✅ **Problem-Solving**
- Start with brute force
- Optimize step by step
- Consider edge cases
- Test your solution

✅ **Coding**
- Write clean, readable code
- Use meaningful variable names
- Follow C# conventions
- Add comments for complex logic

✅ **Behavioral**
- Use STAR method
- Be specific with examples
- Show enthusiasm
- Ask thoughtful questions

### After the Interview

✅ **Follow-Up**
- Send thank-you email within 24 hours
- Mention specific discussion points
- Reiterate your interest
- Be patient with the process

### Red Flags to Avoid

❌ **Don't:**
- Jump into coding without understanding the problem
- Stay silent while coding
- Give up when stuck
- Argue with the interviewer
- Speak negatively about previous employers
- Lie about your experience
- Forget to test your code

### Key Success Factors

1. **Consistency**: Study every day, even if just 1 hour
2. **Practice**: Solve problems, don't just read solutions
3. **Communication**: Explain your thinking clearly
4. **Preparation**: Research the company thoroughly
5. **Confidence**: Believe in your abilities
6. **Authenticity**: Be yourself, show genuine interest

---

## Checklist: Are You Ready?

### Technical Skills
- [ ] Can solve 70% of LeetCode Easy problems
- [ ] Can solve 50% of LeetCode Medium problems
- [ ] Completed all 26 Blackbaud-specific problems
- [ ] Comfortable with C# and .NET Core
- [ ] Can build a REST API from scratch
- [ ] Understand Entity Framework Core
- [ ] Know common design patterns
- [ ] Can explain SOLID principles

### Interview Skills
- [ ] Practiced mock interviews
- [ ] Prepared STAR stories
- [ ] Can explain projects clearly
- [ ] Have questions ready for interviewer
- [ ] Comfortable with collaborative coding
- [ ] Can discuss system design

### Company Knowledge
- [ ] Researched Blackbaud's mission
- [ ] Familiar with their products
- [ ] Understand their tech stack
- [ ] Know their values and culture
- [ ] Read recent company news

### Logistics
- [ ] Resume updated
- [ ] LinkedIn profile polished
- [ ] Interview environment set up
- [ ] Collaborative coding tools tested
- [ ] Professional attire ready

---

## Conclusion

Preparing for a Blackbaud .NET Developer interview requires a balanced approach:

1. **Strong coding fundamentals** (data structures & algorithms)
2. **Deep .NET knowledge** (C#, ASP.NET Core, EF Core)
3. **System design understanding** (scalability, reliability)
4. **Behavioral preparation** (STAR method, company research)
5. **Consistent practice** (daily coding, mock interviews)

Remember, Blackbaud values:
- **Technical excellence**: Write clean, maintainable code
- **Mission alignment**: Passion for social good
- **Collaboration**: Team player mentality
- **Growth mindset**: Willingness to learn

**You've got this! Good luck with your interview! 🚀**

---

## Additional Resources

### Blackbaud-Specific Links
- **Careers**: https://careers.blackbaud.com
- **Engineering Blog**: Check Blackbaud's tech blog for insights
- **GitHub**: https://github.com/blackbaud

### Community
- **r/cscareerquestions**: Reddit community for interview prep
- **Blind**: Anonymous tech community
- **LinkedIn**: Connect with Blackbaud employees

### Contact
If you have questions during your preparation, consider:
- Reaching out to Blackbaud recruiters
- Connecting with current employees on LinkedIn
- Joining .NET developer communities

---

**Last Updated**: July 2026
**Version**: 1.0

*This guide is based on publicly available information and interview experiences. Actual interview content may vary.*

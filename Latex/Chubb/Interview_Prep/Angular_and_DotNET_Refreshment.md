# Angular and .NET Refreshment Guide

## Quick Refresher for Senior Engineer Interview

This guide provides a comprehensive refresher on Angular and .NET concepts essential for the Chubb Senior Software Engineer role. Focus on depth and real-world applications.

---

## Part 1: .NET & C# Refreshment

### 1. SOLID Principles (Critical for Senior Role)

#### S - Single Responsibility Principle
**Definition**: A class should have only one reason to change.

**Bad Example**:
```csharp
public class UserManager
{
    public void CreateUser(string name, string email)
    {
        // Create user in database
        var user = new User { Name = name, Email = email };
        SaveToDatabase(user);
        
        // Send email
        SendEmail(email, "Welcome!");
        
        // Log activity
        LogActivity($"User {name} created");
    }
}
```

**Good Example**:
```csharp
public interface IUserRepository
{
    void Create(User user);
}

public interface IEmailService
{
    void SendWelcomeEmail(string email);
}

public interface ILogger
{
    void LogActivity(string message);
}

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    
    public UserService(IUserRepository userRepository, 
                      IEmailService emailService, 
                      ILogger logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }
    
    public void CreateUser(string name, string email)
    {
        var user = new User { Name = name, Email = email };
        _userRepository.Create(user);
        _emailService.SendWelcomeEmail(email);
        _logger.LogActivity($"User {name} created");
    }
}
```

#### O - Open/Closed Principle
**Definition**: Software entities should be open for extension, closed for modification.

**Example**:
```csharp
// Bad: Need to modify existing code to add new payment method
public class PaymentProcessor
{
    public void ProcessPayment(string method, decimal amount)
    {
        if (method == "CreditCard")
        {
            // Process credit card
        }
        else if (method == "PayPal")
        {
            // Process PayPal
        }
    }
}

// Good: Use strategy pattern
public interface IPaymentMethod
{
    void Process(decimal amount);
}

public class CreditCardPayment : IPaymentMethod
{
    public void Process(decimal amount)
    {
        // Process credit card
    }
}

public class PayPalPayment : IPaymentMethod
{
    public void Process(decimal amount)
    {
        // Process PayPal
    }
}

public class PaymentProcessor
{
    private readonly IPaymentMethod _paymentMethod;
    
    public PaymentProcessor(IPaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
    }
    
    public void ProcessPayment(decimal amount)
    {
        _paymentMethod.Process(amount);
    }
}
```

#### L - Liskov Substitution Principle
**Definition**: Objects of a superclass should be replaceable with objects of its subclasses without breaking the application.

**Example**:
```csharp
// Bad: Square violates LSP
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    
    public int GetArea() => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set => base.Width = base.Height = value;
    }
}

// Good: Use proper inheritance
public interface IShape
{
    int GetArea();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int GetArea() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    
    public int GetArea() => Side * Side;
}
```

#### I - Interface Segregation Principle
**Definition**: Clients should not be forced to depend on interfaces they don't use.

**Example**:
```csharp
// Bad: Too many methods in one interface
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

// Good: Segregate interfaces
public interface IWorker
{
    void Work();
}

public interface IEater
{
    void Eat();
}

public interface ISleeper
{
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() { /* ... */ }
}

public class Human : IWorker, IEater, ISleeper
{
    public void Work() { /* ... */ }
    public void Eat() { /* ... */ }
    public void Sleep() { /* ... */ }
}
```

#### D - Dependency Inversion Principle
**Definition**: Depend on abstractions, not on concrete implementations.

**Example**:
```csharp
// Bad: High-level depends on low-level
public class UserService
{
    private readonly SqlUserRepository _repository = new SqlUserRepository();
    
    public User GetUser(int id) => _repository.GetUser(id);
}

// Good: Both depend on abstraction
public interface IUserRepository
{
    User GetUser(int id);
}

public class UserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public User GetUser(int id) => _repository.GetUser(id);
}
```

---

### 2. Design Patterns (Essential for Senior Role)

#### Singleton Pattern
```csharp
public class DatabaseConnection
{
    private static DatabaseConnection _instance;
    private static readonly object _lock = new object();
    
    private DatabaseConnection() { }
    
    public static DatabaseConnection GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new DatabaseConnection();
                }
            }
        }
        return _instance;
    }
}

// Or use Lazy<T> for thread-safe singleton
public class DatabaseConnection
{
    private static readonly Lazy<DatabaseConnection> _instance = 
        new Lazy<DatabaseConnection>(() => new DatabaseConnection());
    
    private DatabaseConnection() { }
    
    public static DatabaseConnection Instance => _instance.Value;
}
```

#### Factory Pattern
```csharp
public interface IPaymentMethod
{
    void Process(decimal amount);
}

public class PaymentFactory
{
    public static IPaymentMethod CreatePaymentMethod(string type)
    {
        return type switch
        {
            "CreditCard" => new CreditCardPayment(),
            "PayPal" => new PayPalPayment(),
            "Bitcoin" => new BitcoinPayment(),
            _ => throw new ArgumentException("Unknown payment type")
        };
    }
}
```

#### Observer Pattern
```csharp
public interface IObserver
{
    void Update(string message);
}

public class Subject
{
    private List<IObserver> _observers = new List<IObserver>();
    
    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);
    
    public void Notify(string message)
    {
        foreach (var observer in _observers)
        {
            observer.Update(message);
        }
    }
}
```

#### Strategy Pattern
```csharp
public interface IDiscountStrategy
{
    decimal CalculateDiscount(decimal amount);
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percentage;
    
    public PercentageDiscount(decimal percentage) => _percentage = percentage;
    
    public decimal CalculateDiscount(decimal amount) => amount * (_percentage / 100);
}

public class FixedDiscount : IDiscountStrategy
{
    private readonly decimal _amount;
    
    public FixedDiscount(decimal amount) => _amount = amount;
    
    public decimal CalculateDiscount(decimal amount) => _amount;
}

public class ShoppingCart
{
    private readonly IDiscountStrategy _discountStrategy;
    
    public ShoppingCart(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }
    
    public decimal CalculateFinalPrice(decimal totalPrice)
    {
        var discount = _discountStrategy.CalculateDiscount(totalPrice);
        return totalPrice - discount;
    }
}
```

---

### 3. Dependency Injection in ASP.NET Core

#### Service Registration
```csharp
// Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Transient: New instance every time
    services.AddTransient<IRepository, Repository>();
    
    // Scoped: One instance per request
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    
    // Singleton: One instance for entire application
    services.AddSingleton<ICache, MemoryCache>();
    
    // Factory registration
    services.AddScoped<IPaymentProcessor>(provider =>
    {
        var logger = provider.GetRequiredService<ILogger>();
        return new PaymentProcessor(logger);
    });
}
```

#### Dependency Injection in Controllers
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    
    // Constructor injection
    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
```

---

### 4. LINQ Mastery

#### Common LINQ Operations
```csharp
var users = new List<User>
{
    new User { Id = 1, Name = "John", Age = 30, Department = "IT" },
    new User { Id = 2, Name = "Jane", Age = 28, Department = "HR" },
    new User { Id = 3, Name = "Bob", Age = 35, Department = "IT" }
};

// Filtering
var itUsers = users.Where(u => u.Department == "IT");

// Projection
var names = users.Select(u => u.Name);

// Ordering
var sortedByAge = users.OrderBy(u => u.Age);
var descending = users.OrderByDescending(u => u.Age);

// Grouping
var groupedByDept = users.GroupBy(u => u.Department);

// Aggregation
var totalAge = users.Sum(u => u.Age);
var avgAge = users.Average(u => u.Age);
var maxAge = users.Max(u => u.Age);
var count = users.Count(u => u.Age > 30);

// First/Last
var firstUser = users.First();
var firstItUser = users.FirstOrDefault(u => u.Department == "IT");
var lastUser = users.Last();

// Distinct
var uniqueDepts = users.Select(u => u.Department).Distinct();

// Take/Skip
var firstThree = users.Take(3);
var skipTwo = users.Skip(2).Take(3);

// Join
var departments = new List<Department>
{
    new Department { Name = "IT", Budget = 100000 },
    new Department { Name = "HR", Budget = 50000 }
};

var joined = users.Join(departments, 
    u => u.Department, 
    d => d.Name, 
    (u, d) => new { u.Name, d.Budget });

// Complex query
var result = users
    .Where(u => u.Age > 25)
    .OrderBy(u => u.Name)
    .Select(u => new { u.Name, u.Department })
    .Distinct()
    .ToList();
```

#### LINQ to Entities (Entity Framework)
```csharp
public class UserRepository : IUserRepository
{
    private readonly DbContext _context;
    
    public async Task<User> GetUserAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<List<User>> GetActiveUsersAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.CreatedDate)
            .ToListAsync();
    }
    
    public async Task<List<UserWithOrderCount>> GetUsersWithOrderCountAsync()
    {
        return await _context.Users
            .Select(u => new UserWithOrderCount
            {
                UserId = u.Id,
                UserName = u.Name,
                OrderCount = u.Orders.Count
            })
            .ToListAsync();
    }
}
```

---

### 5. Async/Await Patterns

#### Basic Async/Await
```csharp
// Bad: Blocking call
public string FetchData()
{
    var result = httpClient.GetStringAsync(url).Result; // Blocks thread!
    return result;
}

// Good: Async/await
public async Task<string> FetchDataAsync()
{
    var result = await httpClient.GetStringAsync(url);
    return result;
}

// Usage in controller
[HttpGet]
public async Task<ActionResult<string>> Get()
{
    var data = await FetchDataAsync();
    return Ok(data);
}
```

#### Task Composition
```csharp
// Sequential execution
public async Task<UserWithOrders> GetUserWithOrdersAsync(int userId)
{
    var user = await _userService.GetUserAsync(userId);
    var orders = await _orderService.GetUserOrdersAsync(userId);
    
    return new UserWithOrders
    {
        User = user,
        Orders = orders
    };
}

// Parallel execution
public async Task<UserWithOrders> GetUserWithOrdersParallelAsync(int userId)
{
    var userTask = _userService.GetUserAsync(userId);
    var ordersTask = _orderService.GetUserOrdersAsync(userId);
    
    await Task.WhenAll(userTask, ordersTask);
    
    return new UserWithOrders
    {
        User = await userTask,
        Orders = await ordersTask
    };
}

// Multiple tasks
public async Task<(User, List<Order>, List<Payment>)> GetUserDataAsync(int userId)
{
    var userTask = _userService.GetUserAsync(userId);
    var ordersTask = _orderService.GetUserOrdersAsync(userId);
    var paymentsTask = _paymentService.GetUserPaymentsAsync(userId);
    
    await Task.WhenAll(userTask, ordersTask, paymentsTask);
    
    return (await userTask, await ordersTask, await paymentsTask);
}
```

#### Exception Handling in Async
```csharp
public async Task<ActionResult<User>> GetUserAsync(int id)
{
    try
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }
    catch (TimeoutException ex)
    {
        _logger.LogError(ex, "Timeout getting user {UserId}", id);
        return StatusCode(504, "Service unavailable");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting user {UserId}", id);
        return StatusCode(500, "Internal server error");
    }
}
```

---

### 6. Entity Framework Core Best Practices

#### DbContext Configuration
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Relationships
            entity.HasMany(e => e.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderDate).IsRequired();
            
            entity.HasMany(e => e.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
        });
    }
}
```

#### Query Optimization
```csharp
// Problem: N+1 query issue
public async Task<List<Order>> GetUserOrdersBadAsync(int userId)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
    
    foreach (var order in orders)
    {
        order.Items = await _context.OrderItems
            .Where(oi => oi.OrderId == order.Id)
            .ToListAsync(); // N queries!
    }
    
    return orders;
}

// Solution: Use Include for eager loading
public async Task<List<Order>> GetUserOrdersGoodAsync(int userId)
{
    return await _context.Orders
        .Where(o => o.UserId == userId)
        .Include(o => o.Items)
        .ThenInclude(oi => oi.Product)
        .ToListAsync();
}

// Alternative: Use Select for projection
public async Task<List<OrderDto>> GetUserOrdersDtoAsync(int userId)
{
    return await _context.Orders
        .Where(o => o.UserId == userId)
        .Select(o => new OrderDto
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            Items = o.Items.Select(oi => new OrderItemDto
            {
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        })
        .ToListAsync();
}
```

---

### 7. REST API Best Practices

#### API Design
```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    // GET: api/v1/users
    [HttpGet]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var users = await _userService.GetUsersAsync(pageNumber, pageSize);
        return Ok(users);
    }
    
    // GET: api/v1/users/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });
        
        return Ok(user);
    }
    
    // POST: api/v1/users
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
    
    // PUT: api/v1/users/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request);
        if (!result)
            return NotFound();
        
        return NoContent();
    }
    
    // DELETE: api/v1/users/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}
```

#### Error Handling
```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new { message = "Internal server error", error = exception.Message };
        
        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return context.Response.WriteAsJsonAsync(response);
    }
}
```

---

## Part 2: Angular Refreshment

### 1. Component Lifecycle Hooks

```typescript
import { Component, OnInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-example',
  template: `<div>{{ data }}</div>`
})
export class ExampleComponent implements OnInit, OnDestroy, OnChanges {
  data: string;
  private destroy$ = new Subject<void>();
  
  // Called after component is initialized
  ngOnInit(): void {
    console.log('Component initialized');
    this.loadData();
  }
  
  // Called when input properties change
  ngOnChanges(changes: SimpleChanges): void {
    console.log('Input properties changed', changes);
  }
  
  // Called after view is initialized
  ngAfterViewInit(): void {
    console.log('View initialized');
  }
  
  // Called before component is destroyed
  ngOnDestroy(): void {
    console.log('Component destroyed');
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  private loadData(): void {
    // Unsubscribe automatically when component is destroyed
    this.someService.getData()
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => this.data = data);
  }
}
```

### 2. Services and Dependency Injection

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root' // Singleton service
})
export class UserService {
  private apiUrl = 'api/users';
  private userSubject = new BehaviorSubject<User | null>(null);
  public user$ = this.userSubject.asObservable();
  
  constructor(private http: HttpClient) {}
  
  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`)
      .pipe(
        map(user => {
          this.userSubject.next(user);
          return user;
        }),
        catchError(error => {
          console.error('Error fetching user', error);
          throw error;
        })
      );
  }
  
  updateUser(id: number, user: User): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, user);
  }
  
  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

// Usage in component
@Component({
  selector: 'app-user',
  template: `
    <div *ngIf="user$ | async as user">
      <h1>{{ user.name }}</h1>
      <p>{{ user.email }}</p>
    </div>
  `
})
export class UserComponent implements OnInit {
  user$: Observable<User>;
  
  constructor(private userService: UserService) {}
  
  ngOnInit(): void {
    this.user$ = this.userService.getUser(1);
  }
}
```

### 3. RxJS Operators (Critical for Angular)

```typescript
import { Observable, Subject, BehaviorSubject, combineLatest, merge } from 'rxjs';
import {
  map,
  filter,
  switchMap,
  mergeMap,
  concatMap,
  debounceTime,
  distinctUntilChanged,
  takeUntil,
  tap,
  catchError,
  retry,
  shareReplay
} from 'rxjs/operators';

// Transformation operators
const numbers$ = of(1, 2, 3, 4, 5);

// map: Transform each value
numbers$.pipe(
  map(n => n * 2)
).subscribe(console.log); // 2, 4, 6, 8, 10

// filter: Filter values
numbers$.pipe(
  filter(n => n > 2)
).subscribe(console.log); // 3, 4, 5

// switchMap: Cancel previous inner observable when new value arrives
const search$ = new Subject<string>();
search$.pipe(
  debounceTime(300),
  distinctUntilChanged(),
  switchMap(query => this.searchService.search(query))
).subscribe(results => console.log(results));

// mergeMap: Subscribe to all inner observables
const clicks$ = fromEvent(button, 'click');
clicks$.pipe(
  mergeMap(() => this.http.get('/api/data'))
).subscribe(data => console.log(data));

// concatMap: Process sequentially
const requests$ = new Subject<number>();
requests$.pipe(
  concatMap(id => this.http.get(`/api/users/${id}`))
).subscribe(user => console.log(user));

// Filtering operators
// debounceTime: Wait before emitting
search$.pipe(
  debounceTime(300)
).subscribe(query => console.log(query));

// distinctUntilChanged: Emit only if value changed
search$.pipe(
  distinctUntilChanged()
).subscribe(query => console.log(query));

// takeUntil: Complete when another observable emits
const destroy$ = new Subject<void>();
data$.pipe(
  takeUntil(destroy$)
).subscribe(data => console.log(data));

// Utility operators
// tap: Side effects without changing value
data$.pipe(
  tap(data => console.log('Data:', data)),
  map(data => data.name)
).subscribe(name => console.log('Name:', name));

// catchError: Handle errors
data$.pipe(
  catchError(error => {
    console.error('Error:', error);
    return of(null);
  })
).subscribe(data => console.log(data));

// retry: Retry on error
data$.pipe(
  retry(3)
).subscribe(data => console.log(data));

// shareReplay: Share result among subscribers
const shared$ = expensiveOperation$.pipe(
  shareReplay(1)
);

// Combination operators
// combineLatest: Combine latest values from multiple observables
combineLatest([user$, settings$]).pipe(
  map(([user, settings]) => ({ user, settings }))
).subscribe(data => console.log(data));

// merge: Merge multiple observables
merge(userUpdates$, settingsUpdates$).subscribe(update => console.log(update));
```

### 4. State Management with NgRx

```typescript
// State definition
export interface UserState {
  users: User[];
  selectedUser: User | null;
  loading: boolean;
  error: string | null;
}

// Actions
export const loadUsers = createAction('[User Page] Load Users');
export const loadUsersSuccess = createAction(
  '[User API] Load Users Success',
  props<{ users: User[] }>()
);
export const loadUsersFailure = createAction(
  '[User API] Load Users Failure',
  props<{ error: string }>()
);

// Reducer
const initialState: UserState = {
  users: [],
  selectedUser: null,
  loading: false,
  error: null
};

export const userReducer = createReducer(
  initialState,
  on(loadUsers, state => ({ ...state, loading: true })),
  on(loadUsersSuccess, (state, { users }) => ({
    ...state,
    users,
    loading: false,
    error: null
  })),
  on(loadUsersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);

// Effects
@Injectable()
export class UserEffects {
  loadUsers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadUsers),
      switchMap(() =>
        this.userService.getUsers().pipe(
          map(users => loadUsersSuccess({ users })),
          catchError(error => of(loadUsersFailure({ error: error.message })))
        )
      )
    )
  );
  
  constructor(
    private actions$: Actions,
    private userService: UserService
  ) {}
}

// Selectors
export const selectUserState = createFeatureSelector<UserState>('user');
export const selectUsers = createSelector(
  selectUserState,
  (state: UserState) => state.users
);
export const selectLoading = createSelector(
  selectUserState,
  (state: UserState) => state.loading
);

// Usage in component
@Component({
  selector: 'app-users',
  template: `
    <div *ngIf="loading$ | async">Loading...</div>
    <div *ngFor="let user of users$ | async">{{ user.name }}</div>
  `
})
export class UsersComponent implements OnInit {
  users$ = this.store.select(selectUsers);
  loading$ = this.store.select(selectLoading);
  
  constructor(private store: Store) {}
  
  ngOnInit(): void {
    this.store.dispatch(loadUsers());
  }
}
```

### 5. Reactive Forms

```typescript
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-form',
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <div>
        <label>Name:</label>
        <input formControlName="name" />
        <div *ngIf="form.get('name')?.hasError('required')">
          Name is required
        </div>
      </div>
      
      <div>
        <label>Email:</label>
        <input formControlName="email" />
        <div *ngIf="form.get('email')?.hasError('email')">
          Invalid email
        </div>
      </div>
      
      <div formGroupName="address">
        <label>Street:</label>
        <input formControlName="street" />
      </div>
      
      <button type="submit" [disabled]="!form.valid">Submit</button>
    </form>
  `
})
export class UserFormComponent {
  form: FormGroup;
  
  constructor(private fb: FormBuilder, private userService: UserService) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      address: this.fb.group({
        street: ['', Validators.required],
        city: ['', Validators.required]
      })
    });
  }
  
  onSubmit(): void {
    if (this.form.valid) {
      this.userService.createUser(this.form.value).subscribe(
        user => console.log('User created', user),
        error => console.error('Error creating user', error)
      );
    }
  }
}
```

### 6. HTTP Client and Error Handling

```typescript
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) {}
  
  get<T>(url: string): Observable<T> {
    return this.http.get<T>(url).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }
  
  post<T>(url: string, data: any): Observable<T> {
    return this.http.post<T>(url, data).pipe(
      catchError(this.handleError)
    );
  }
  
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
```

### 7. Performance Optimization

```typescript
// OnPush change detection strategy
@Component({
  selector: 'app-user-card',
  template: `<div>{{ user.name }}</div>`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserCardComponent {
  @Input() user: User;
}

// Unsubscribe pattern
@Component({
  selector: 'app-user-list'
})
export class UserListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  constructor(private userService: UserService) {}
  
  ngOnInit(): void {
    this.userService.getUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe(users => console.log(users));
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// Lazy loading
const routes: Routes = [
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
  }
];

// TrackBy function for *ngFor
@Component({
  template: `<div *ngFor="let user of users; trackBy: trackByUserId">{{ user.name }}</div>`
})
export class UserListComponent {
  users: User[];
  
  trackByUserId(index: number, user: User): number {
    return user.id;
  }
}
```

---

## Quick Reference Checklist

### .NET Checklist
- [ ] SOLID principles understood
- [ ] Design patterns can be implemented
- [ ] Dependency injection configured
- [ ] LINQ queries optimized
- [ ] Async/await patterns mastered
- [ ] Entity Framework best practices known
- [ ] REST API design principles understood
- [ ] Error handling implemented

### Angular Checklist
- [ ] Component lifecycle hooks understood
- [ ] Services and DI configured
- [ ] RxJS operators mastered
- [ ] State management (NgRx) understood
- [ ] Reactive forms implemented
- [ ] HTTP client and error handling
- [ ] Performance optimization applied
- [ ] Testing practices known

---

## Interview Tips

1. **Explain Your Thinking**: Walk through your problem-solving process
2. **Consider Trade-offs**: Show understanding of different approaches
3. **Code Quality**: Write clean, readable code
4. **Performance**: Discuss optimization opportunities
5. **Testing**: Mention how you'd test your solution
6. **Real-World Experience**: Use examples from your projects
7. **Stay Updated**: Show knowledge of latest best practices
8. **Ask Questions**: Clarify requirements before solving

Good luck with your interview! 🚀

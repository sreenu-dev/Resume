# TCS Senior .NET + Angular Developer Interview Guide

**Role**: Senior .NET Full Stack Developer (Angular)  
**Company**: Tata Consultancy Services (TCS)  
**Level**: Senior (5+ years expected depth)  
**Last Updated**: August 2026

---

## 📋 Table of Contents

1. [What "Senior" Means in This Interview](#what-senior-means-in-this-interview)
2. [Senior-Level .NET Topics & Questions](#senior-level-net-topics--questions)
3. [Senior-Level Angular Topics & Questions](#senior-level-angular-topics--questions)
4. [SQL & Database Design (Senior Depth)](#sql--database-design-senior-depth)
5. [Architecture & System Design](#architecture--system-design)
6. [Leadership & Team Questions](#leadership--team-questions)
7. [Coding Round Expectations](#coding-round-expectations)
8. [3-Day Study Plan (Today → Friday)](#3-day-study-plan-today--friday)
9. [Questions to Ask Interviewer](#questions-to-ask-interviewer)

---

## 🎯 What "Senior" Means in This Interview

At senior level, interviewers judge you differently than juniors. Expect:

- **"Why" over "What"** — Not "what is DI" but "why would you choose Scoped over Singleton here, and what breaks if you get it wrong?"
- **Trade-off discussions** — Every answer should mention pros/cons, not just definitions
- **Ownership stories** — Design decisions you made, mistakes you caught, mentoring you did
- **Architecture thinking** — How pieces fit together, not just syntax
- **Code review mindset** — Spotting anti-patterns in code shown to you
- **Performance & scale** — Not just "does it work" but "does it work at 10x load"

**Golden rule for every answer**: State the concept → give a concrete example from your work → mention a trade-off or gotcha.

---

## 🔥 Senior-Level .NET Topics & Questions

### **1. Advanced C# & CLR Internals**

**Topics to master:**
- Memory model: stack vs heap, escape analysis, `Span<T>` and `Memory<T>`
- Garbage Collection generations, Server GC vs Workstation GC, `GC.Collect()` pitfalls
- `async`/`await` internals — state machines, `SynchronizationContext`, deadlocks (classic `.Result` deadlock in ASP.NET)
- Value types deep dive: `struct` vs `class`, `readonly struct`, `in` parameters
- Thread safety: `lock`, `Monitor`, `SemaphoreSlim`, `Interlocked`, `ConcurrentDictionary`
- Records, pattern matching, nullable reference types (`?`, `!`)
- Reflection & source generators (why source generators replaced a lot of reflection in modern .NET)

**Sample Questions:**
1. Walk me through what happens under the hood when you `await` a `Task`. What is the compiler generating?
2. Why does calling `.Result` on an async method inside an ASP.NET request sometimes deadlock? How do you avoid it?
3. What's the difference between `Task.Run` and directly calling an `async` method? When would misusing this hurt performance?
4. Explain `IAsyncEnumerable<T>` and when you'd use it over `IEnumerable<T>`.
5. What is a memory leak in a garbage-collected language, and how have you diagnosed one in production (e.g., event handler leaks, static collections)?
6. Explain the difference between `ConfigureAwait(false)` in library code vs ASP.NET Core (where `SynchronizationContext` is usually null anyway).
7. When would you use `Span<T>` for performance-critical code?
8. Explain covariance and contravariance with a real example (`IEnumerable<out T>`, `Action<in T>`).

```csharp
// Classic deadlock example — be ready to explain WHY
public class BadController : Controller {
    public IActionResult Get() {
        var result = GetDataAsync().Result; // ⚠️ Can deadlock on legacy SynchronizationContext
        return Ok(result);
    }
    
    public async Task<string> GetDataAsync() {
        await Task.Delay(1000);
        return "data";
    }
}
```

---

### **2. ASP.NET Core — Senior Depth**

**Topics:**
- Middleware pipeline internals — writing custom middleware, short-circuiting, order dependency
- Filters vs Middleware vs Model Binding — when to use which
- Minimal APIs vs Controller-based APIs — trade-offs
- Custom `ActionFilter`, `ExceptionFilter`, global exception handling middleware
- Health checks, graceful shutdown, `IHostedService` / `BackgroundService`
- Rate limiting middleware (built-in in .NET 7+)
- Response caching, output caching, ETags
- API Gateway patterns (YARP, Ocelot) if working with microservices
- gRPC vs REST — when senior teams pick gRPC

**Sample Questions:**
1. How would you design global exception handling so that every API returns a consistent error shape?
2. Explain the exact order of `UseRouting`, `UseAuthentication`, `UseAuthorization`, `UseCors`. Why does order matter?
3. When would you choose Minimal APIs over traditional Controllers in a large enterprise app?
4. How do you implement health checks for Kubernetes liveness/readiness probes?
5. Design a solution for handling 10,000 concurrent long-polling requests efficiently.
6. What's the difference between `IHostedService` and `BackgroundService`? When would a background job fail silently, and how would you catch that?

```csharp
// Global Exception Handling Middleware (senior-level expectation)
public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = ex switch {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            context.Response.ContentType = "application/json";
            var response = new { error = ex.Message, traceId = context.TraceIdentifier };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
```

---

### **3. Entity Framework Core — Senior Depth**

**Topics:**
- Query performance: `AsNoTracking`, split queries vs single query with `Include`, avoiding N+1
- Compiled queries for hot paths
- Concurrency handling: optimistic concurrency (`RowVersion`), pessimistic locking
- Migrations strategy in CI/CD (auto vs manual approval gates)
- Bulk operations (`ExecuteUpdate`, `ExecuteDelete` in EF Core 7+) vs loading entities
- Interceptors, `SaveChanges` hooks for auditing
- Database-first vs code-first trade-offs in legacy TCS projects
- Multi-tenancy patterns with EF Core (shared DB with tenant column vs DB-per-tenant)

**Sample Questions:**
1. How do you detect and fix an N+1 query problem in a live production system?
2. Explain optimistic concurrency control with EF Core and how you'd handle a `DbUpdateConcurrencyException`.
3. When would you drop to raw SQL/stored procedures instead of LINQ, and why?
4. How would you design a multi-tenant SaaS database schema using EF Core?
5. What's the performance impact of `.Include()` chains with multiple collections, and how do you mitigate it (split queries)?

```csharp
// Optimistic Concurrency
public class Product {
    public int Id { get; set; }
    public string Name { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

try {
    _context.Entry(product).State = EntityState.Modified;
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex) {
    // Reload and let user decide: overwrite or merge
    var entry = ex.Entries.Single();
    await entry.ReloadAsync();
}
```

---

### **4. Microservices, Messaging & Distributed Systems**

**Topics:**
- Synchronous (REST/gRPC) vs asynchronous (Service Bus/Kafka) communication trade-offs
- Saga pattern vs 2-phase commit for distributed transactions
- Circuit breaker, retry with exponential backoff, bulkhead (Polly library)
- Idempotency keys for safe retries
- Event-driven architecture, event sourcing, CQRS — when it's overkill
- Distributed tracing (OpenTelemetry, correlation IDs across services)
- API Gateway, BFF (Backend for Frontend) pattern

**Sample Questions:**
1. Explain CQRS. When have you used it, and when would you tell a team NOT to use it?
2. How do you guarantee exactly-once processing (or handle at-least-once safely) with Service Bus messages?
3. Design a resilient HTTP client call with retry, circuit breaker, and timeout using Polly.
4. What's the Saga pattern, and how does it solve distributed transaction problems in microservices?
5. How do you trace a request across 5 microservices when debugging a production issue?

```csharp
// Polly resilience policy (senior-level expectation)
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

await combinedPolicy.ExecuteAsync(() => httpClient.GetAsync(url));
```

---

### **5. Design Patterns & Architecture (Senior Application)**

Not just "define the pattern" — expect **"which pattern fits this scenario and why"**.

**Must know deeply:**
- Repository + Unit of Work — and their criticism (leaky abstraction over EF Core)
- CQRS + MediatR — pipeline behaviors for cross-cutting concerns (validation, logging)
- Strategy, Factory, Decorator, Adapter — real usage, not textbook
- Clean Architecture / Onion Architecture — layering, dependency direction
- Vertical Slice Architecture — modern alternative to layered architecture
- Domain-Driven Design basics — aggregates, value objects, bounded contexts

**Sample Questions:**
1. Some engineers say the Repository pattern over EF Core is redundant since `DbSet<T>` is already a repository. What's your view?
2. Explain MediatR pipeline behaviors — how would you add validation and logging without touching every handler?
3. What is Clean Architecture and how do you enforce dependency direction (e.g., using architecture tests)?
4. When is CQRS overengineering for a CRUD-heavy admin panel?

```csharp
// MediatR + Pipeline Behavior for validation (common senior pattern)
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

### **6. Testing at Senior Level**

- Unit vs integration vs contract tests (Pact) vs E2E — pyramid strategy
- Mocking boundaries — mock external dependencies, not your own domain logic
- `WebApplicationFactory` for integration testing ASP.NET Core APIs
- Testcontainers for real SQL Server/Postgres in integration tests (vs InMemory provider pitfalls)
- Mutation testing awareness (Stryker.NET) — going beyond coverage %

**Sample Questions:**
1. Why is EF Core's InMemory provider risky for integration tests? What's a better alternative?
2. How do you structure a test pyramid for a microservices-based product?
3. How would you test a message consumer (Service Bus handler) in isolation?

---

## 🎨 Senior-Level Angular Topics & Questions

### **1. Change Detection & Performance**

**Topics:**
- Zone.js and how Angular's default change detection works
- `ChangeDetectionStrategy.OnPush` — when and why
- `trackBy` in `*ngFor`, avoiding function calls in templates
- Signals (Angular 16+) vs Zone.js-based change detection
- `ExpressionChangedAfterItHasBeenCheckedError` — why it happens and how to fix it
- Detaching change detector for performance-critical components (`ChangeDetectorRef`)

**Sample Questions:**
1. Explain how `OnPush` change detection strategy changes Angular's default behavior. What breaks if you forget to trigger CD manually after an async callback?
2. What are Angular Signals, and how do they differ from Zone.js-based reactivity?
3. Why does `*ngFor` without `trackBy` cause performance issues on large lists, and how do you fix it?
4. How would you debug a component that re-renders excessively?

```typescript
@Component({
  selector: 'app-product-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div *ngFor="let p of products; trackBy: trackById">{{ p.name }}</div>`
})
export class ProductListComponent {
  @Input() products: Product[] = [];
  
  trackById(index: number, item: Product): number {
    return item.id;
  }
}
```

---

### **2. RxJS — Senior Depth**

**Topics:**
- Hot vs cold observables, Subject vs BehaviorSubject vs ReplaySubject
- Flattening operators: `switchMap` vs `mergeMap` vs `concatMap` vs `exhaustMap` — pick the right one
- Memory leaks from unsubscribed observables — `takeUntil`, `async` pipe, `DestroyRef` (Angular 16+)
- Combining streams: `combineLatest`, `withLatestFrom`, `forkJoin`
- Custom operators, error handling with `catchError` and retry strategies
- Backpressure handling in high-frequency streams

**Sample Questions:**
1. Explain the difference between `switchMap`, `mergeMap`, `concatMap`, and `exhaustMap`. Give a real scenario for each (e.g., typeahead search = switchMap, form submit = exhaustMap).
2. How do you prevent memory leaks from subscriptions in a large Angular app? Compare `takeUntil` vs `async` pipe vs `DestroyRef`.
3. What's the difference between `Subject`, `BehaviorSubject`, and `ReplaySubject`? When would you use each in a state service?
4. How would you cancel an in-flight HTTP request when the user navigates away or types a new search term?

```typescript
// Classic typeahead pattern — expect to write this live
searchControl.valueChanges.pipe(
  debounceTime(300),
  distinctUntilChanged(),
  switchMap(term => this.searchService.search(term).pipe(
    catchError(() => of([]))
  )),
  takeUntil(this.destroy$)
).subscribe(results => this.results = results);
```

---

### **3. State Management**

**Topics:**
- Service-with-Subject pattern vs NgRx vs Signals-based state
- NgRx: Actions, Reducers, Effects, Selectors, entity adapter
- When NgRx is overkill vs when it's necessary (app complexity threshold)
- Signal Store / NgRx SignalStore — modern lightweight alternative

**Sample Questions:**
1. When would you introduce NgRx into a project, and when would a simple service with `BehaviorSubject` be enough?
2. Explain the role of Effects in NgRx — why can't side effects (API calls) live in reducers?
3. How do memoized selectors improve performance in NgRx?
4. How do you test an NgRx effect that calls an API and dispatches a success/failure action?

```typescript
// NgRx Effect example
loadProducts$ = createEffect(() =>
  this.actions$.pipe(
    ofType(ProductActions.loadProducts),
    switchMap(() =>
      this.productService.getAll().pipe(
        map(products => ProductActions.loadProductsSuccess({ products })),
        catchError(error => of(ProductActions.loadProductsFailure({ error })))
      )
    )
  )
);
```

---

### **4. Architecture & Advanced Angular**

**Topics:**
- Module structure: feature modules, shared modules, core module (or standalone components in Angular 15+)
- Standalone components/directives/pipes — the modern (no-NgModule) approach
- Lazy loading with route-level code splitting, preloading strategies
- Smart vs dumb (presentational) component pattern
- Custom structural directives, content projection (`ng-content`, `ng-template`, `ngTemplateOutlet`)
- Dependency Injection hierarchy — module injector vs element injector, `providedIn: 'root'` vs component-level providers
- Micro-frontends with Angular (Module Federation)
- Angular Universal (SSR) — when and why

**Sample Questions:**
1. What are standalone components, and how do they change Angular application architecture compared to NgModules?
2. Explain Angular's hierarchical dependency injection. What happens if you provide a service at component level vs root level?
3. How would you architect a large Angular app with 10+ feature teams working in parallel (micro-frontends, Nx monorepo, module boundaries)?
4. When would you use Server-Side Rendering (Angular Universal) and what SEO/performance problems does it solve?
5. Explain content projection with `ng-content` and a real use case (e.g., building a reusable modal/card component).

---

### **5. Forms, Testing & Tooling**

**Topics:**
- Reactive Forms deep dive: `FormArray`, custom validators, async validators, cross-field validation
- Testing Angular components with Jasmine/Jest + TestBed, shallow vs deep rendering
- Cypress/Playwright for E2E in Angular apps
- Nx monorepo for large enterprise Angular + .NET solutions
- Angular CLI build optimization: differential loading, budgets, tree-shaking

**Sample Questions:**
1. How do you implement cross-field validation (e.g., password/confirm password) in Reactive Forms?
2. Write a custom async validator that checks username availability via API call with debounce.
3. How do you unit test a component that has a dependency on an injected service making HTTP calls?
4. What's an Nx monorepo, and why would a TCS enterprise project use it for Angular + .NET?

```typescript
// Custom async validator example
export function usernameAvailabilityValidator(userService: UserService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    return control.valueChanges.pipe(
      debounceTime(500),
      switchMap(value => userService.checkUsername(value)),
      map(isAvailable => (isAvailable ? null : { usernameTaken: true })),
      first()
    );
  };
}
```

---

## 🗄️ SQL & Database Design (Senior Depth)

### **Topics**
- Indexing strategy: clustered vs non-clustered, covering indexes, index seek vs scan
- Query execution plans — reading and optimizing them
- Isolation levels: Read Committed, Snapshot, Serializable — deadlock implications
- Partitioning large tables, archiving strategies
- Window functions (`ROW_NUMBER`, `RANK`, `LAG`/`LEAD`) for analytics queries
- CTEs vs temp tables vs table variables — performance implications
- Normalization vs denormalization trade-offs for read-heavy systems

### **Sample Questions**
1. How do you read a SQL Server execution plan to identify a missing index?
2. Explain the difference between clustered and non-clustered indexes with a real table example.
3. What causes deadlocks in SQL Server, and how do you diagnose them (deadlock graphs)?
4. Write a query using `ROW_NUMBER()` to get the top 3 highest-paid employees per department.
5. When would you denormalize a table for performance, and what's the trade-off?

```sql
-- Senior-level SQL: top N per group
SELECT * FROM (
    SELECT EmployeeId, Department, Salary,
           ROW_NUMBER() OVER (PARTITION BY Department ORDER BY Salary DESC) AS rn
    FROM Employees
) ranked
WHERE rn <= 3;

-- Detecting missing index candidates (conceptual, senior awareness)
SELECT * FROM sys.dm_db_missing_index_details;
```

---

## 🏗️ Architecture & System Design

Senior candidates are often given an open-ended design problem combining .NET + Angular + SQL.

### **Common Prompts**
1. Design an end-to-end **employee leave management system** (Angular UI, .NET Web API, SQL Server) — cover auth, roles, approval workflow, notifications.
2. Design a **real-time dashboard** showing live order status updates (SignalR + Angular + EF Core).
3. Design a **multi-tenant SaaS** billing platform — data isolation, scaling, and API design.
4. How would you migrate a legacy monolithic .NET Framework app with AngularJS frontend to .NET Core + Angular incrementally (strangler fig pattern)?

### **Approach to Answer (STAR-like for design)**
1. Clarify requirements & constraints (scale, users, SLAs)
2. Propose high-level architecture (diagram in words: client → API Gateway → services → DB)
3. Discuss data model briefly
4. Call out cross-cutting concerns: auth, logging, caching, error handling
5. Discuss trade-offs and alternatives you considered

---

## 👥 Leadership & Team Questions

Since it's a senior role, expect behavioral questions focused on **influence and ownership**, not just individual contribution.

1. Tell me about a time you disagreed with an architectural decision made by your team/lead. What did you do?
2. Describe a time you mentored a junior developer. What was the outcome?
3. How do you handle code reviews when you strongly disagree with the approach?
4. Tell me about a production incident you led the resolution for.
5. How do you balance technical debt against feature delivery pressure from stakeholders?
6. Describe a time you had to make a decision with incomplete information.
7. How do you influence a team to adopt a new practice (e.g., pushing for automated testing) without formal authority?

**Use STAR method**: Situation → Task → Action → Result, and emphasize the **impact/metrics** in Result.

---

## 💻 Coding Round Expectations (Senior Level)

At senior level, coding rounds often blend DSA with **practical, real-world coding**:

- Medium LeetCode-style problems (arrays, strings, trees, hash maps) — expect to code AND discuss complexity
- **Live refactoring exercise**: "Here's a piece of legacy code — refactor for readability/SOLID/testability"
- **API design exercise**: Design a REST endpoint with proper DTOs, validation, and error handling on the spot
- **Angular component exercise**: Build a small reactive component (e.g., searchable/paginated table) live

**Practice these categories:**
- Arrays/Strings (Two Sum, Group Anagrams)
- Trees (Level order traversal, LCA)
- Hash Maps (Frequency count problems)
- SQL query writing (joins, window functions, subqueries)
- One live "design and code a small feature end-to-end" exercise

---

## 📅 3-Day Study Plan (Today → Friday)

### **Day 1 (Today): .NET Deep Dive**
- [ ] Morning: Async/await internals, GC, memory management (2 hrs)
- [ ] Afternoon: EF Core performance topics, concurrency, migrations (1.5 hrs)
- [ ] Evening: Design patterns — Repository/UoW critique, MediatR pipeline, Clean Architecture (1.5 hrs)
- [ ] Solve 3 coding problems (Arrays, Trees) — 1 hr

### **Day 2: Angular + Microservices**
- [ ] Morning: RxJS deep dive — switchMap/mergeMap/concatMap/exhaustMap, memory leaks (2 hrs)
- [ ] Afternoon: Change detection, Signals, NgRx state management (1.5 hrs)
- [ ] Evening: Microservices patterns — Polly, Saga, CQRS, distributed tracing (1.5 hrs)
- [ ] Practice writing 2-3 SQL queries with window functions (30 min)

### **Day 3 (Friday morning, before interview): Review & Mock**
- [ ] Review your own past projects — prepare 3 STAR stories (bug fix, architecture decision, mentoring)
- [ ] Skim through all code snippets in this guide once
- [ ] Prepare 4-5 questions to ask interviewer (see below)
- [ ] Light review only — don't cram new topics
- [ ] Rest well, arrive/join early

---

## ❓ Questions to Ask Interviewer (Senior-Specific)

1. What's the current architecture — monolith, modular monolith, or microservices — and what's driving any migration?
2. What does the Angular frontend's state management strategy look like today?
3. What technical debt is the team actively trying to pay down?
4. As a senior engineer, how much influence would I have over architecture decisions vs. execution?
5. What does the code review and mentoring culture look like on this team?
6. What's the biggest scaling challenge the current system faces?

---

## ✅ Final Checklist

- [ ] Can explain async/await internals and common deadlock scenarios
- [ ] Can discuss EF Core performance (N+1, concurrency, compiled queries)
- [ ] Can compare Repository/UoW vs direct DbContext usage with an opinion
- [ ] Can explain OnPush change detection and Signals
- [ ] Can pick the correct RxJS flattening operator for a given scenario and justify it
- [ ] Can explain when to use/avoid NgRx and CQRS
- [ ] Can write a SQL query with window functions and explain indexing trade-offs
- [ ] Have 3 STAR stories ready (technical decision, mentoring, incident resolution)
- [ ] Have 4-6 thoughtful questions ready for the interviewer

---

**You have solid experience — this guide is about sharpening your "why" answers, not learning from scratch. Good luck Friday! 🚀**

**Last Updated**: August 2026  
**Version**: 1.0

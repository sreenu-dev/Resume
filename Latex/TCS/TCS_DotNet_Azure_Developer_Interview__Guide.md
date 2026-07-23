# TCS .NET + Azure Developer Interview Preparation Guide

**Role**: .NET + Azure Developer  
**Company**: Tata Consultancy Services (TCS)  
**Last Updated**: July 2026

---

## 📋 Table of Contents

1. [JD Analysis & Key Focus Areas](#jd-analysis--key-focus-areas)
2. [Must-Have Skills (Critical)](#must-have-skills-critical)
3. [Good-to-Have Skills](#good-to-have-skills)
4. [Detailed Topic Breakdown](#detailed-topic-breakdown)
5. [Interview Questions by Topic](#interview-questions-by-topic)
6. [Hands-On Practice Projects](#hands-on-practice-projects)
7. [Last-Minute Preparation](#last-minute-preparation)
8. [Interview Day Checklist](#interview-day-checklist)

---

## 🎯 JD Analysis & Key Focus Areas

### **Must-Have (Non-Negotiable)**
1. ✅ **C# & .NET Core** - Deep knowledge required
2. ✅ **Web APIs (REST)** - Build and design APIs
3. ✅ **Azure Services** - Minimum 4 services (App Service, Function App, Storage, Logic Apps, Service Bus, Key Vault, App Insights, KQL)
4. ✅ **DevOps & CI/CD** - BVTs, automation tests, security gates
5. ✅ **Security** - Software & infrastructure security
6. ✅ **Troubleshooting** - Debugging and problem-solving
7. ✅ **Agile Methodology** - Scrum/Kanban basics

### **Good-to-Have (Competitive Advantage)**
- React or Angular
- Microsoft Copilot Studio or AI tools
- xUnit, nUnit testing frameworks

### **Soft Skills (Often Overlooked)**
- Independent work
- Prioritization
- Flexibility in fast-paced environment
- Multi-tasking with conflicting priorities

---

## 🔥 Must-Have Skills (Critical)

### **1. C# & .NET Core (40% of Interview)**

#### **Core Concepts**
```csharp
// Value vs Reference Types
int x = 10;              // Value type (stack)
int y = x;               // Copy
y = 20;                  // x still 10

class Person { }         // Reference type (heap)
Person p1 = new Person();
Person p2 = p1;          // Reference copy
p2 = new Person();       // p1 unchanged

// Async/Await (CRITICAL for Azure)
public async Task<string> GetDataAsync() {
    using HttpClient client = new();
    return await client.GetStringAsync("https://api.example.com");
}

// LINQ (Essential)
var numbers = new[] { 1, 2, 3, 4, 5 };
var evens = numbers.Where(n => n % 2 == 0).ToList();
var squared = numbers.Select(n => n * n).ToList();

// Generics & Constraints
public class Repository<T> where T : class {
    public void Add(T item) { }
}

// Delegates & Events
public delegate void NotifyHandler(string message);
public event NotifyHandler OnNotify;

// Exception Handling
try {
    // Code
}
catch (HttpRequestException ex) {
    throw;  // Preserve stack trace
}
finally {
    // Cleanup
}

// IDisposable Pattern (Important for Azure resources)
public class AzureResource : IDisposable {
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
            disposed = true;
        }
    }
    
    ~AzureResource() => Dispose(false);
}
```

#### **Key Interview Questions**
- What's the difference between `Task` and `Thread`?
- Explain `async void` vs `async Task` (when to use each)
- What is `ConfigureAwait(false)` and why use it?
- Difference between `IEnumerable` and `IQueryable`?
- What are boxing and unboxing? Performance implications?
- Explain the `using` statement and `IDisposable` pattern
- What are SOLID principles? Explain each with examples
- Difference between `abstract class` and `interface`?

---

### **2. Web APIs & REST (25% of Interview)**

#### **Building REST APIs**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase {
    private readonly IProductService _service;
    
    public ProductsController(IProductService service) {
        _service = service;
    }
    
    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll() {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }
    
    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id) {
        var product = await _service.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });
        return Ok(product);
    }
    
    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var product = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
    
    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto) {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }
    
    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

// Dependency Injection Setup (Program.cs)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();
```

#### **Key Interview Questions**
- What are HTTP status codes? When to use 200, 201, 400, 404, 500?
- Difference between REST and SOAP?
- What is idempotency in APIs? Which HTTP methods are idempotent?
- How to handle versioning in APIs?
- Explain API pagination, filtering, and sorting
- What is rate limiting and how to implement it?
- How to handle errors and exceptions in APIs?
- What is API documentation (Swagger/OpenAPI)?

---

### **3. Azure Services (30% of Interview) - CRITICAL**

#### **Must Know (Minimum 4)**

**A. Azure App Service**
```csharp
// Deploy ASP.NET Core to App Service
// Key concepts:
// - Deployment slots (staging, production)
// - Auto-scaling based on metrics
// - Application settings and connection strings
// - SSL/TLS certificates
// - Monitoring and diagnostics

// Example: Connection string from App Service settings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

**B. Azure Function App**
```csharp
// HTTP Triggered Function
[FunctionName("GetProduct")]
public static async Task<IActionResult> GetProduct(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] 
    HttpRequest req,
    int id,
    ILogger log) {
    
    log.LogInformation($"Getting product {id}");
    
    // Business logic
    var product = new { id, name = "Product Name" };
    
    return new OkObjectResult(product);
}

// Timer Triggered Function (Background job)
[FunctionName("ProcessQueueMessages")]
public static void ProcessQueue(
    [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
    ILogger log) {
    
    log.LogInformation($"Processing at {DateTime.Now}");
    // Process messages
}

// Queue Triggered Function
[FunctionName("ProcessServiceBusMessage")]
public static void ProcessMessage(
    [ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")] 
    string myQueueItem,
    ILogger log) {
    
    log.LogInformation($"Processing: {myQueueItem}");
}
```

**C. Azure Storage (Blob, Queue, Table)**
```csharp
// Blob Storage
public class BlobStorageService {
    private readonly BlobContainerClient _containerClient;
    
    public BlobStorageService(BlobContainerClient containerClient) {
        _containerClient = containerClient;
    }
    
    public async Task UploadAsync(string fileName, Stream content) {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(content, overwrite: true);
    }
    
    public async Task<BlobDownloadInfo> DownloadAsync(string fileName) {
        var blobClient = _containerClient.GetBlobClient(fileName);
        return await blobClient.DownloadAsync();
    }
    
    public async Task DeleteAsync(string fileName) {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteAsync();
    }
}

// Queue Storage
public class QueueStorageService {
    private readonly QueueClient _queueClient;
    
    public QueueStorageService(QueueClient queueClient) {
        _queueClient = queueClient;
    }
    
    public async Task SendMessageAsync(string message) {
        await _queueClient.SendMessageAsync(message);
    }
    
    public async Task<QueueMessage> ReceiveMessageAsync() {
        var message = await _queueClient.ReceiveMessageAsync();
        return message.Value;
    }
}
```

**D. Azure Service Bus**
```csharp
// Publish-Subscribe Pattern
public class ServiceBusPublisher {
    private readonly ServiceBusClient _client;
    
    public ServiceBusPublisher(ServiceBusClient client) {
        _client = client;
    }
    
    public async Task PublishEventAsync(string topicName, string message) {
        var sender = _client.CreateSender(topicName);
        await sender.SendMessageAsync(new ServiceBusMessage(message));
        await sender.DisposeAsync();
    }
}

public class ServiceBusSubscriber {
    private readonly ServiceBusClient _client;
    
    public ServiceBusSubscriber(ServiceBusClient client) {
        _client = client;
    }
    
    public async Task SubscribeAsync(string topicName, string subscriptionName) {
        var processor = _client.CreateProcessor(topicName, subscriptionName);
        
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;
        
        await processor.StartProcessingAsync();
    }
    
    private async Task MessageHandler(ProcessMessageEventArgs args) {
        var message = args.Message.Body.ToString();
        // Process message
        await args.CompleteMessageAsync(args.Message);
    }
    
    private Task ErrorHandler(ProcessErrorEventArgs args) {
        // Log error
        return Task.CompletedTask;
    }
}
```

**E. Azure Key Vault**
```csharp
// Secure credential management
public class KeyVaultService {
    private readonly SecretClient _secretClient;
    
    public KeyVaultService(SecretClient secretClient) {
        _secretClient = secretClient;
    }
    
    public async Task<string> GetSecretAsync(string secretName) {
        var secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value.Value;
    }
    
    public async Task SetSecretAsync(string secretName, string secretValue) {
        await _secretClient.SetSecretAsync(secretName, secretValue);
    }
}

// In Program.cs
var keyVaultUrl = new Uri(builder.Configuration["KeyVault:VaultUri"]);
var credential = new DefaultAzureCredential();
builder.Configuration.AddAzureKeyVault(keyVaultUrl, credential);
```

**F. Azure App Insights (Monitoring & Logging)**
```csharp
// Application Insights Integration
public class ProductService {
    private readonly ILogger<ProductService> _logger;
    private readonly TelemetryClient _telemetryClient;
    
    public ProductService(ILogger<ProductService> logger, TelemetryClient telemetryClient) {
        _logger = logger;
        _telemetryClient = telemetryClient;
    }
    
    public async Task<Product> GetProductAsync(int id) {
        try {
            _logger.LogInformation("Getting product {ProductId}", id);
            
            var product = await _repository.GetByIdAsync(id);
            
            // Track custom metric
            _telemetryClient.TrackEvent("ProductRetrieved", 
                new Dictionary<string, string> { { "ProductId", id.ToString() } });
            
            return product;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            _telemetryClient.TrackException(ex);
            throw;
        }
    }
}

// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

**G. KQL (Kusto Query Language)**
```kusto
// Query logs from App Insights
traces
| where timestamp > ago(24h)
| where severityLevel >= 2
| summarize Count = count() by tostring(customDimensions.ProductId)
| order by Count desc

// Performance metrics
requests
| where duration > 1000
| summarize AvgDuration = avg(duration), MaxDuration = max(duration) by name
| order by AvgDuration desc

// Exception tracking
exceptions
| where timestamp > ago(7d)
| summarize Count = count() by type, outerMessage
| order by Count desc
```

**H. Azure Logic Apps**
```csharp
// Trigger: HTTP Request
// Actions:
// 1. Parse JSON
// 2. Call Azure Function
// 3. Send email via Office 365
// 4. Log to Application Insights
// 5. Send message to Service Bus

// Example workflow:
// Trigger: HTTP POST
// → Parse JSON body
// → Call Azure Function to process
// → If success: Send confirmation email
// → If failure: Log error and send alert
```

#### **Key Interview Questions**
- What's the difference between App Service and Function App?
- When to use Azure Storage vs SQL Database?
- Explain Service Bus vs Event Grid vs Event Hubs
- How to secure Azure resources (Key Vault, Managed Identity)?
- What is Azure DevOps and how to set up CI/CD?
- Explain deployment slots and blue-green deployment
- How to monitor and troubleshoot Azure applications?
- What is Application Insights and how to use KQL?
- Explain scaling strategies in Azure (vertical vs horizontal)
- What is Azure Service Principal and Managed Identity?

---

### **4. DevOps & CI/CD (20% of Interview)**

#### **CI/CD Pipeline with Security Gates**
```yaml
# Azure Pipelines YAML
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '6.0.x'

stages:
  - stage: Build
    jobs:
      - job: BuildJob
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)
          
          - task: DotNetCoreCLI@2
            displayName: 'Restore NuGet packages'
            inputs:
              command: 'restore'
          
          - task: DotNetCoreCLI@2
            displayName: 'Build'
            inputs:
              command: 'build'
              arguments: '--configuration $(buildConfiguration)'
          
          - task: DotNetCoreCLI@2
            displayName: 'Run Unit Tests'
            inputs:
              command: 'test'
              arguments: '--configuration $(buildConfiguration) --no-build'
          
          - task: SonarCloudPrepare@1
            displayName: 'Prepare SonarCloud analysis'
            inputs:
              SonarCloud: 'SonarCloud'
              organization: 'your-org'
              projectKey: 'your-project'
          
          - task: SonarCloudAnalyze@1
            displayName: 'Run SonarCloud analysis'
          
          - task: PublishBuildArtifacts@1
            displayName: 'Publish artifacts'
            inputs:
              pathToPublish: '$(Build.ArtifactStagingDirectory)'

  - stage: SecurityGate
    dependsOn: Build
    jobs:
      - job: SecurityChecks
        steps:
          - task: WhiteSource@21
            displayName: 'WhiteSource Security Scan'
            inputs:
              cwd: '$(Build.SourcesDirectory)'
          
          - task: CredScan@3
            displayName: 'Credential Scanner'
            inputs:
              scanFolder: '$(Build.SourcesDirectory)'

  - stage: Deploy
    dependsOn: SecurityGate
    condition: succeeded()
    jobs:
      - deployment: DeployToStaging
        environment: 'Staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: AzureWebApp@1
                  inputs:
                    azureSubscription: 'Azure Connection'
                    appType: 'webAppLinux'
                    appName: 'your-app-staging'
                    package: '$(Pipeline.Workspace)/drop'

  - stage: BVT
    dependsOn: Deploy
    jobs:
      - job: RunBVT
        steps:
          - task: VSTest@2
            displayName: 'Run BVT Tests'
            inputs:
              testSelector: 'testAssemblies'
              testAssemblyVer2: '**/*Tests.dll'
              searchFolder: '$(System.DefaultWorkingDirectory)'
```

#### **Key Concepts**
- **BVT (Build Verification Tests)**: Automated tests that run after build
- **Security Gates**: SonarCloud, WhiteSource, Credential Scanner
- **Automation Tests**: Unit tests, integration tests, API tests
- **Deployment Strategies**: Blue-green, canary, rolling

#### **Key Interview Questions**
- What is CI/CD and why is it important?
- Explain the difference between CI and CD
- What are deployment gates and why use them?
- How to implement security checks in CI/CD?
- What is SonarQube/SonarCloud and how to use it?
- Explain different deployment strategies (blue-green, canary, rolling)
- How to handle secrets in CI/CD pipelines?
- What is infrastructure as code (IaC)?

---

### **5. Security (Software & Infrastructure)**

#### **Software Security**
```csharp
// Input Validation
public class UserValidator {
    public bool ValidateEmail(string email) {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
    }
    
    public bool ValidatePassword(string password) {
        // Min 8 chars, uppercase, lowercase, digit, special char
        var regex = new System.Text.RegularExpressions.Regex(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }
}

// SQL Injection Prevention (Use Parameterized Queries)
public class UserRepository {
    // ❌ BAD: SQL Injection vulnerable
    public User GetUserBad(string email) {
        string query = $"SELECT * FROM Users WHERE Email = '{email}'";
        // Vulnerable to: ' OR '1'='1
    }
    
    // ✅ GOOD: Parameterized query
    public async Task<User> GetUserAsync(string email) {
        var user = await _context.Users
            .FromSqlInterpolated($"SELECT * FROM Users WHERE Email = {email}")
            .FirstOrDefaultAsync();
        return user;
    }
}

// XSS Prevention
public class HtmlEncoder {
    public string EncodeHtml(string input) {
        return System.Web.HttpUtility.HtmlEncode(input);
    }
}

// CSRF Protection
[ValidateAntiForgeryToken]
[HttpPost]
public async Task<IActionResult> DeleteUser(int id) {
    await _userService.DeleteAsync(id);
    return Ok();
}

// Authentication & Authorization
[Authorize]
[HttpGet("profile")]
public async Task<IActionResult> GetProfile() {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(await _userService.GetByIdAsync(int.Parse(userId)));
}

[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id) {
    await _userService.DeleteAsync(id);
    return NoContent();
}
```

#### **Infrastructure Security**
```csharp
// Network Security
// - Network Security Groups (NSGs)
// - Virtual Networks (VNets)
// - Private Endpoints
// - VPN Gateway

// Identity & Access Management
// - Azure AD / Entra ID
// - Managed Identity
// - Role-Based Access Control (RBAC)
// - Service Principal

// Example: Using Managed Identity
var credential = new DefaultAzureCredential();
var client = new SecretClient(new Uri("https://your-vault.vault.azure.net/"), credential);

// Data Security
// - Encryption at rest (Storage Service Encryption)
// - Encryption in transit (HTTPS/TLS)
// - Database encryption (Transparent Data Encryption)
// - Key management (Azure Key Vault)

// Compliance & Auditing
// - Azure Policy
// - Azure Blueprints
// - Azure Audit Logs
// - Compliance Manager
```

#### **Key Interview Questions**
- What is SQL injection and how to prevent it?
- Explain XSS (Cross-Site Scripting) and CSRF attacks
- What is HTTPS and why is it important?
- Explain OAuth 2.0 and OpenID Connect
- What is JWT and how does it work?
- How to secure APIs (authentication, authorization)?
- What is encryption at rest vs in transit?
- Explain Azure Key Vault and Managed Identity
- What is RBAC and how to implement it?
- How to handle secrets in applications?

---

### **6. Troubleshooting & Debugging**

#### **Common Issues & Solutions**
```csharp
// Issue 1: Memory Leaks
// Solution: Implement IDisposable properly
public class ResourceManager : IDisposable {
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
            disposed = true;
        }
    }
}

// Issue 2: N+1 Query Problem
// ❌ BAD: N+1 queries
var orders = _context.Orders.ToList();
foreach (var order in orders) {
    var items = _context.OrderItems.Where(oi => oi.OrderId == order.Id).ToList();
}

// ✅ GOOD: Single query with Include
var orders = _context.Orders
    .Include(o => o.Items)
    .ToList();

// Issue 3: Deadlocks in Database
// Solution: Use proper transaction isolation levels
using (var transaction = _context.Database.BeginTransaction(
    System.Data.IsolationLevel.ReadCommitted)) {
    try {
        // Database operations
        _context.SaveChanges();
        transaction.Commit();
    }
    catch {
        transaction.Rollback();
        throw;
    }
}

// Issue 4: Async Deadlock
// ❌ BAD: Blocking async call
var result = GetDataAsync().Result;

// ✅ GOOD: Await async call
var result = await GetDataAsync();

// Issue 5: Exception Handling
public async Task<IActionResult> GetProduct(int id) {
    try {
        var product = await _service.GetByIdAsync(id);
        return Ok(product);
    }
    catch (ArgumentException ex) {
        _logger.LogWarning(ex, "Invalid product ID");
        return BadRequest(new { error = ex.Message });
    }
    catch (Exception ex) {
        _logger.LogError(ex, "Unexpected error");
        return StatusCode(500, new { error = "Internal server error" });
    }
}
```

#### **Debugging Tools**
- Visual Studio Debugger (breakpoints, watch, immediate window)
- Application Insights (monitoring, diagnostics)
- Azure Monitor (metrics, logs, alerts)
- Fiddler (HTTP traffic analysis)
- Postman (API testing)
- Azure Storage Explorer (blob/queue inspection)

#### **Key Interview Questions**
- How do you debug an application?
- What are breakpoints and how to use them?
- Explain Application Insights and how to use it
- How to identify and fix memory leaks?
- What is the N+1 query problem and how to solve it?
- How to troubleshoot performance issues?
- What are common Azure issues and how to resolve them?

---

## 💡 Good-to-Have Skills

### **1. Frontend (React or Angular)**

#### **Angular Basics**
```typescript
// Component
import { Component, OnInit } from '@angular/core';
import { ProductService } from './product.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  products: any[] = [];
  loading = false;
  error: string | null = null;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message;
        this.loading = false;
      }
    });
  }

  deleteProduct(id: number): void {
    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.products = this.products.filter(p => p.id !== id);
      },
      error: (err) => {
        this.error = err.message;
      }
    });
  }
}

// Service
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://api.example.com/products';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getProductById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, product);
  }

  updateProduct(id: number, product: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

// Reactive Forms
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

export class ProductFormComponent {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      price: ['', [Validators.required, Validators.min(0)]],
      description: ['']
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      // Submit form
    }
  }
}
```

#### **Key Interview Questions**
- What is Angular and key features?
- Explain components, services, and dependency injection
- What are observables and RxJS?
- Difference between template-driven and reactive forms?
- How to handle HTTP requests in Angular?
- Explain routing and lazy loading
- What is change detection?
- How to communicate between components?

---

### **2. Unit Testing (xUnit, nUnit)**

#### **xUnit Example**
```csharp
using Xunit;
using Moq;

public class ProductServiceTests {
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTests() {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsProduct() {
        // Arrange
        int productId = 1;
        var expectedProduct = new Product { Id = 1, Name = "Test Product" };
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProduct.Id, result.Id);
        Assert.Equal(expectedProduct.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNull() {
        // Arrange
        int productId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product)null);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetProductById_WithInvalidId_ThrowsException(int invalidId) {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetByIdAsync(invalidId));
    }
}
```

#### **nUnit Example**
```csharp
using NUnit.Framework;
using Moq;

[TestFixture]
public class ProductServiceTests {
    private Mock<IProductRepository> _mockRepository;
    private ProductService _service;

    [SetUp]
    public void Setup() {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object);
    }

    [Test]
    public async Task GetProductById_WithValidId_ReturnsProduct() {
        // Arrange
        int productId = 1;
        var expectedProduct = new Product { Id = 1, Name = "Test Product" };
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedProduct.Id, result.Id);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void GetProductById_WithInvalidId_ThrowsException(int invalidId) {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetByIdAsync(invalidId));
    }
}
```

#### **Key Interview Questions**
- What is unit testing and why is it important?
- Difference between mocking and stubbing?
- What is AAA pattern (Arrange, Act, Assert)?
- How to test async methods?
- What is test coverage and how to measure it?
- Explain xUnit vs nUnit vs MSTest

---

### **3. Microsoft Copilot Studio & AI Tools**

#### **Key Concepts**
- Copilot Studio for building AI-powered chatbots
- Integration with Azure services
- Natural Language Processing (NLP)
- AI Builder for automation
- Prompt engineering basics

#### **Key Interview Questions**
- What is Copilot Studio and how to use it?
- How to build a chatbot using Copilot Studio?
- What is prompt engineering?
- How to integrate AI tools in applications?

---

## 📚 Interview Questions by Topic

### **C# & .NET Core (40% weight)**

1. Explain value types vs reference types with examples
2. What is the difference between `async void` and `async Task`?
3. When should you use `ConfigureAwait(false)`?
4. Explain LINQ and its advantages
5. What are delegates and events?
6. Explain generics and generic constraints
7. What is the `using` statement and `IDisposable` pattern?
8. Difference between `IEnumerable` and `IQueryable`?
9. Explain SOLID principles with C# examples
10. What is dependency injection and why is it important?
11. How does garbage collection work in .NET?
12. Explain boxing and unboxing
13. What are extension methods?
14. Explain lambda expressions and their use cases
15. What is the difference between `abstract class` and `interface`?

### **Web APIs & REST (25% weight)**

1. What are HTTP status codes? Explain common ones
2. Design a RESTful API for an e-commerce system
3. What is idempotency in APIs?
4. How to handle versioning in APIs?
5. Explain pagination, filtering, and sorting
6. How to implement rate limiting?
7. What is API documentation (Swagger)?
8. How to handle errors and exceptions in APIs?
9. Explain authentication vs authorization
10. What is CORS and how to enable it?

### **Azure Services (30% weight)**

1. Difference between App Service and Function App?
2. When to use Azure Storage vs SQL Database?
3. Explain Service Bus vs Event Grid
4. How to secure Azure resources?
5. What is Application Insights and KQL?
6. Explain deployment slots and blue-green deployment
7. How to monitor Azure applications?
8. What is Azure DevOps and CI/CD?
9. Explain scaling strategies in Azure
10. What is Managed Identity and Service Principal?
11. How to use Azure Key Vault?
12. Explain Azure Logic Apps use cases
13. What is Azure Functions and when to use it?
14. How to handle long-running operations in Azure?
15. Explain Azure Storage tiers and when to use each

### **DevOps & CI/CD (20% weight)**

1. What is CI/CD and why is it important?
2. Explain deployment gates and security checks
3. What is SonarQube/SonarCloud?
4. Explain different deployment strategies
5. How to handle secrets in CI/CD?
6. What is infrastructure as code (IaC)?
7. Explain BVT (Build Verification Tests)
8. How to implement automated testing in CI/CD?
9. What is blue-green deployment?
10. How to rollback a deployment?

### **Security (15% weight)**

1. What is SQL injection and how to prevent it?
2. Explain XSS and CSRF attacks
3. What is HTTPS and TLS?
4. Explain OAuth 2.0 and OpenID Connect
5. What is JWT and how does it work?
6. How to secure APIs?
7. What is encryption at rest vs in transit?
8. Explain RBAC (Role-Based Access Control)
9. How to handle secrets securely?
10. What is Azure Key Vault?

### **Troubleshooting & Debugging (10% weight)**

1. How do you debug an application?
2. What are common memory leak causes?
3. Explain the N+1 query problem
4. How to identify performance bottlenecks?
5. What is Application Insights?
6. How to use Azure Monitor?
7. Explain common async/await issues
8. How to handle exceptions properly?
9. What are deadlocks and how to prevent them?
10. How to troubleshoot Azure issues?

---

## 🛠️ Hands-On Practice Projects

### **Project 1: E-Commerce API (Beginner)**
**Duration**: 3-4 days

**Requirements**:
- Build REST API with ASP.NET Core
- CRUD operations for Products, Orders, Users
- SQL Server database with EF Core
- Authentication with JWT
- Swagger documentation
- Unit tests with xUnit
- Deploy to Azure App Service

**Tech Stack**: C#, .NET Core, SQL Server, EF Core, xUnit, Azure

---

### **Project 2: Notification System (Intermediate)**
**Duration**: 5-7 days

**Requirements**:
- Azure Function App for processing
- Azure Service Bus for messaging
- Azure Storage for logs
- Application Insights for monitoring
- CI/CD pipeline with security gates
- Email/SMS notifications
- Retry logic and error handling

**Tech Stack**: C#, Azure Functions, Service Bus, Storage, App Insights, Azure DevOps

---

### **Project 3: Full-Stack Application (Advanced)**
**Duration**: 10-14 days

**Requirements**:
- ASP.NET Core Web API backend
- Angular frontend
- SQL Server database
- Azure App Service deployment
- Azure Key Vault for secrets
- Application Insights monitoring
- CI/CD with automated tests
- Security implementation (HTTPS, CORS, authentication)
- Performance optimization

**Tech Stack**: C#, .NET Core, Angular, SQL Server, Azure, DevOps

---

## ⏰ Last-Minute Preparation (24 Hours Before)

### **Hour-by-Hour Schedule**

**6 Hours Before Interview**:
- [ ] Review Azure services (App Service, Function App, Storage, Service Bus, Key Vault, App Insights)
- [ ] Practice 2-3 coding problems (REST API design, database queries)
- [ ] Review SOLID principles and design patterns

**4 Hours Before**:
- [ ] Review C# async/await patterns
- [ ] Practice explaining a project you built
- [ ] Review CI/CD pipeline concepts

**2 Hours Before**:
- [ ] Light review of security concepts
- [ ] Prepare your "Tell me about yourself" pitch
- [ ] Test your internet connection and IDE

**1 Hour Before**:
- [ ] Relax, take a walk
- [ ] Review your resume
- [ ] Prepare questions for interviewer

**30 Minutes Before**:
- [ ] Close all unnecessary tabs
- [ ] Have water nearby
- [ ] Do some deep breathing

---

## ✅ Interview Day Checklist

### **Technical Setup**
- [ ] Stable internet connection
- [ ] Visual Studio or VS Code ready
- [ ] Azure account logged in
- [ ] Postman/Swagger for API testing
- [ ] SQL Server Management Studio
- [ ] Azure Portal access

### **Environment**
- [ ] Quiet, well-lit room
- [ ] Professional background
- [ ] Webcam and microphone working
- [ ] Phone on silent
- [ ] Water nearby

### **Mental Preparation**
- [ ] Good sleep (7-8 hours)
- [ ] Light breakfast
- [ ] Positive mindset
- [ ] Remember: They want you to succeed

### **During Interview**
- [ ] **Think aloud** - explain your approach
- [ ] **Ask clarifying questions** before coding
- [ ] **Start with brute force**, then optimize
- [ ] **Test edge cases** before submitting
- [ ] **Be receptive to hints** from interviewer
- [ ] **Discuss trade-offs** (time vs space complexity)
- [ ] **Write clean code** with meaningful names

### **After Interview**
- [ ] Send thank-you email within 24 hours
- [ ] Mention specific discussion points
- [ ] Reiterate your interest
- [ ] Be patient with the process

---

## 🎯 Success Criteria

### **You're Ready If You Can**:
- ✅ Explain async/await and when to use it
- ✅ Design a REST API from scratch
- ✅ Explain at least 4 Azure services in detail
- ✅ Discuss CI/CD pipeline and security gates
- ✅ Solve LeetCode Medium problems in 45 minutes
- ✅ Explain SOLID principles with examples
- ✅ Discuss a project you built end-to-end
- ✅ Handle security questions confidently
- ✅ Troubleshoot common issues

---

## 📞 Quick Reference

### **Azure Services Cheat Sheet**
| Service          | Use Case            | Key Feature                     |
| ------------------| ---------------------| ---------------------------------|
| **App Service**  | Host web apps       | Auto-scaling, deployment slots  |
| **Function App** | Serverless compute  | Event-driven, pay-per-use       |
| **Storage**      | Blob/Queue/Table    | Scalable, durable storage       |
| **Service Bus**  | Async messaging     | Pub-sub, queues, topics         |
| **Key Vault**    | Secrets management  | Secure credential storage       |
| **App Insights** | Monitoring          | Telemetry, logging, diagnostics |
| **Logic Apps**   | Workflow automation | Low-code integration            |

### **C# Keywords Cheat Sheet**
| Keyword | Purpose |
|---------|---------|
| `async/await` | Asynchronous programming |
| `using` | Resource management |
| `try/catch/finally` | Exception handling |
| `lock` | Thread synchronization |
| `yield` | Iterator pattern |
| `var` | Type inference |
| `dynamic` | Runtime type checking |
| `sealed` | Prevent inheritance |

---

## 🚀 Final Tips

1. **Practice consistently** - Solve 2-3 problems daily
2. **Build projects** - Hands-on experience is crucial
3. **Understand concepts** - Don't just memorize
4. **Ask questions** - During interview, clarify requirements
5. **Think aloud** - Explain your approach
6. **Test thoroughly** - Check edge cases
7. **Stay calm** - Mistakes are normal
8. **Be authentic** - Show genuine interest in the role
9. **Follow up** - Send thank-you email
10. **Learn from feedback** - Improve for next round

---

**Good Luck! You've got this! 🎉**

**Last Updated**: July 2026  
**Version**: 1.0


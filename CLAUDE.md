# Modern C# Reference Guide

*Optimized for Claude's code generation and decision-making*

## Essential Modern Syntax (Use These Always)

```csharp
// File structure
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace MyApp.Domain;

// Classes & Records
internal class Service(ILogger logger)                      // Primary constructor
{
    private readonly Dictionary<string, object> _cache = []; // Collection expression
}

internal abstract record Result<T>;                         // Discriminated unions
internal sealed record Success<T>(T Value) : Result<T>;
internal sealed record Failure<T>(string Error) : Result<T>;

// Nullable annotations
string? optional = null;
string required = "value";

// Pattern matching (prefer over if/else chains)
var result = input switch
{
    Success<Data> success => ProcessSuccess(success.Value),
    Failure<Data> failure => LogError(failure.Error),
    _ => HandleUnknown()
};

// Collections
int[] numbers = [1, 2, 3];                    // Arrays
List<string> items = ["a", "b", "c"];         // Lists  
string[] combined = [.. first, .. second];    // Spread

// String handling
string message = $"Hello {name}";             // Interpolation
string multiline = """
    Raw string with "quotes" 
    and \backslashes
    """;
```

## Common Implementation Patterns

```csharp
// Error handling (prefer over exceptions)
public Result<User> GetUser(int id)
{
    if (id <= 0) return Result.Failure<User>("Invalid ID");
    var user = database.Find(id);
    return user ?? Result.Failure<User>("User not found");
}

// Async with cancellation (always include CancellationToken)
public async Task<Result<Data>> ProcessAsync(CancellationToken cancellationToken = default)
{
    try 
    {
        var data = await service.FetchAsync(cancellationToken);
        return Result.Success(data);
    }
    catch (OperationCanceledException)
    {
        return Result.Failure<Data>("Operation cancelled");
    }
}

// LINQ (prefer method syntax for complex queries)
var activeUsers = users
    .Where(u => u.IsActive)
    .Select(u => new { u.Name, u.Email })
    .ToList();

// Validation patterns
if (input is { Length: > 0 } nonEmpty)
{
    ProcessInput(nonEmpty);
}

// Object initialization
var config = new ServiceConfig
{
    Timeout = TimeSpan.FromSeconds(30),
    RetryCount = 3
};
```

## Advanced Features (When Needed)

```csharp
// C# 13 Features
public void Process(params ReadOnlySpan<int> values) { }    // params collections
private readonly Lock _lock = new();                       // Modern lock
public implicit extension StringExt for string             // Extension properties
{
    public bool IsEmpty => string.IsNullOrEmpty(this);
}

// Performance patterns
ReadOnlySpan<char> span = text.AsSpan();                   // Zero-allocation
Memory<byte> buffer = new byte[1024];

// Generic constraints with modern syntax
public T Process<T>(T input) where T : IComparable<T>, new()

// List patterns
if (items is [var first, .., var last])
{
    Console.WriteLine($"First: {first}, Last: {last}");
}
```

## Architecture Patterns

```csharp
// Dependency injection setup
builder.Services
    .AddSingleton<IService, Service>()
    .AddKeyedSingleton<ICache, RedisCache>("redis")
    .AddHostedService<BackgroundWorker>();

// Configuration
public class AppSettings
{
    public required string ConnectionString { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
}

// Source generators (for performance-critical apps)
[JsonSerializable(typeof(MyData))]
internal partial class AppJsonContext : JsonSerializerContext { }

// Result pattern with mapping
public Result<TOut> Map<TOut>(Func<T, TOut> mapper) => this switch
{
    Success<T> success => Result.Success(mapper(success.Value)),
    Failure<T> failure => Result.Failure<TOut>(failure.Error),
    _ => throw new InvalidOperationException()
};
```

## Code Quality Rules

**Naming & Types**
- Use `string` not `String`, `int` not `Int32`
- Private fields: `_fieldName` 
- Use `var` only when type is obvious: `var text = "clear";`

**Error Handling**
- Prefer `Result<T>` patterns over exceptions for expected failures
- Catch specific exception types: `catch (ArgumentException ex) when (ex.ParamName == "id")`
- Always propagate `CancellationToken` in async methods

**Collections & LINQ**
- Use collection expressions: `[1, 2, 3]` over `new[] { 1, 2, 3 }`
- Chain LINQ operations: `items.Where().Select().ToList()`
- Use meaningful variable names in queries

**Performance**
- Use `Span<T>` and `Memory<T>` for high-performance scenarios
- Prefer `ValueTask<T>` over `Task<T>` for frequently synchronous operations
- Use source generators over reflection

**DRY Principle & Record Design**
- Eliminate parameter repetition in record hierarchies
- Use single record with optional properties over inheritance when appropriate
- Add factory methods for clean object creation
- Prefer composition over inheritance for data containers

**Pattern Matching Guidelines**
- Use switch expressions for discriminated unions and complex matching
- Use if statements for simple validation and sequential checks
- Use `when` guards for complex conditions in switch expressions
- Property patterns (`{ Length: > 0 }`) are clearer than `not null and not ""`
- Don't sacrifice correctness (stack traces) for "modern" syntax

## Project Configuration

```xml
<!-- .csproj essentials -->
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <LangVersion>13</LangVersion>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <AnalysisLevel>9-all</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

```sh
# Development workflow
dotnet format    # Format code
dotnet build     # Build and analyze
```

## Modern Record Patterns

```csharp
// ❌ Repetitive inheritance
internal abstract record BaseData(string Name, int Id, bool IsActive);
internal sealed record UserData(string Name, int Id, bool IsActive, string Email) : BaseData(Name, Id, IsActive);
internal sealed record AdminData(string Name, int Id, bool IsActive, string[] Permissions) : BaseData(Name, Id, IsActive);

// ✅ Single record with optional properties
internal record EntityData(
    string Name,
    int Id, 
    bool IsActive
)
{
    public string? Email { get; init; }          // For users
    public string[]? Permissions { get; init; }  // For admins
    
    // Pattern matching helpers
    public bool IsUser => Email is not null;
    public bool IsAdmin => Permissions is not null;
    
    // Factory methods
    public static EntityData User(string name, int id, bool isActive, string email) =>
        new(name, id, isActive) { Email = email };
        
    public static EntityData Admin(string name, int id, bool isActive, string[] permissions) =>
        new(name, id, isActive) { Permissions = permissions };
}

// Usage with pattern matching
var result = entity switch
{
    { IsUser: true, Email: var email } => ProcessUser(email),
    { IsAdmin: true, Permissions: var perms } => ProcessAdmin(perms), 
    _ => ProcessDefault()
};
```

## Quick Decision Matrix

**When to use:**
- `record` → Data containers, DTOs, value objects
- `class` → Services, complex behavior, inheritance
- `struct` → Small value types, performance-critical
- `switch` expression → Discriminated unions, complex matching, multiple conditions
- `if` statement → Simple validation, sequential checks, early returns
- `Result<T>` → Expected failures, validation
- `Exception` → Unexpected failures, system errors
- `async/await` → I/O operations, network calls
- `Task.Run` → CPU-bound work (rare, prefer async APIs)
- Single record + optional properties → When inheritance creates parameter repetition
- Traditional inheritance → When behavior differs significantly between types

This reference prioritizes patterns you'll use 80% of the time, with quick access to advanced features when needed.
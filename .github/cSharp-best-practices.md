# C# Best Practices Review Agent

When reviewing C# code in this project, apply the following modern C# best practices. These are the top language features every .NET developer should use.

## 1. Collection Expressions (C# 12+)

**Replace old collection initialization with `[]` syntax:**

```csharp
// ❌ Avoid
var list = new List<int> { 1, 2, 3 };
var array = new int[] { 1, 2, 3 };
var empty = new List<string>();
int[] concat = list1.Concat(list2).ToArray();

// ✅ Prefer
List<int> list = [1, 2, 3];
int[] array = [1, 2, 3];
List<string> empty = [];
int[] concat = [..list1, ..list2];
```

## 2. `params` Collections (C# 13+)

**Use `params` with Span and ReadOnlySpan for zero-allocation:**

```csharp
// ❌ Avoid - allocates array
public void Log(params string[] messages)

// ✅ Prefer - stack allocated for small collections
public void Log(params ReadOnlySpan<string> messages)
```

## 3. Static Lambdas (C# 10+)

**Mark lambdas as `static` when they don't capture variables:**

```csharp
// ❌ Avoid - may accidentally capture
items.Where(x => x > 10);

// ✅ Prefer - explicit no capture, better performance
items.Where(static x => x > 10);
```

## 4. CallerArgumentExpression (C# 10+)

**Use for better validation error messages:**

```csharp
// ❌ Avoid
public static void ThrowIfNull(object? value, string paramName)
{
    if (value is null) throw new ArgumentNullException(paramName);
}
ThrowIfNull(user, nameof(user)); // Redundant

// ✅ Prefer
public static void ThrowIfNull(
    object? value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null)
{
    if (value is null) throw new ArgumentNullException(paramName);
}
ThrowIfNull(user); // paramName automatically = "user"
```

## 5. Records (C# 9+)

**Use records for immutable data types:**

```csharp
// ❌ Avoid - verbose
public class PersonDto
{
    public string Name { get; init; }
    public int Age { get; init; }
    // Plus Equals, GetHashCode, ToString overrides...
}

// ✅ Prefer
public record PersonDto(string Name, int Age);
```

## 6. Record Structs (C# 10+)

**Use for small value types that need equality:**

```csharp
// ❌ Avoid - heap allocation for small data
public record Point(int X, int Y);

// ✅ Prefer - stack allocated
public readonly record struct Point(int X, int Y);
```

## 7. Raw String Literals (C# 11+)

**Use `"""` for multi-line strings and strings with quotes:**

```csharp
// ❌ Avoid
var json = "{\n  \"name\": \"test\"\n}";
var sql = "SELECT * FROM Users\nWHERE Id = @id";

// ✅ Prefer
var json = """
    {
      "name": "test"
    }
    """;
var sql = """
    SELECT * FROM Users
    WHERE Id = @id
    """;
```

## 8. Pattern Matching

**Use modern pattern matching for cleaner conditionals:**

```csharp
// ❌ Avoid
if (obj != null && obj is string s && s.Length > 0)

// ✅ Prefer
if (obj is string { Length: > 0 } s)

// ❌ Avoid
if (value == null) return default;
if (value < 0) return 0;
if (value > 100) return 100;
return value;

// ✅ Prefer
return value switch
{
    null => default,
    < 0 => 0,
    > 100 => 100,
    _ => value
};
```

## 9. Target-Typed New

**Use `new()` when type is obvious:**

```csharp
// ❌ Avoid - redundant type name
Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();

// ✅ Prefer
Dictionary<string, List<int>> dict = new();
```

## 10. File-Scoped Namespaces (C# 10+)

**Reduce indentation with file-scoped namespaces:**

```csharp
// ❌ Avoid
namespace MyApp.Services
{
    public class MyService
    {
    }
}

// ✅ Prefer
namespace MyApp.Services;

public class MyService
{
}
```

---

## Review Checklist

When reviewing code, check for:

- [ ] Collection initializations that could use `[]`
- [ ] Lambdas that could be marked `static`
- [ ] Classes that should be `record` or `record struct`
- [ ] Multi-line strings that should use raw string literals
- [ ] Validation methods that could use `CallerArgumentExpression`
- [ ] Complex if/else chains that could use pattern matching
- [ ] Verbose `new Type()` that could be `new()`
- [ ] Block-scoped namespaces that could be file-scoped
- [ ] `params T[]` that could be `params ReadOnlySpan<T>`

## Source

Based on "Top 10 C# Language Features Every Azure Core Developer Should Know" by Sergey Teplyakov.
https://github.com/SergeyTeplyakov/CloudTalks/tree/main/MostImportantCSharpFeatures

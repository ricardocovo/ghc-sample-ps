# GhcSamplePs.Core.Tests

Unit tests for the GhcSamplePs.Core project

## Purpose

This project contains unit tests for all business logic in `GhcSamplePs.Core`. Tests focus on validating service behavior, business rules, and data access logic.

## Dependencies

- **GhcSamplePs.Core** - The project under test
- **xUnit** - Test framework
- **Microsoft.NET.Test.Sdk** - Test SDK

## Project Structure

```
GhcSamplePs.Core.Tests/
├── Services/       # Tests for Core services
├── Repositories/   # Tests for Core repositories
├── Models/         # Tests for domain models and validation
└── TestHelpers/    # Test utilities and data builders
```

## Running Tests

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test
dotnet test --filter "FullyQualifiedName~MyServiceTests"
```

## Code Coverage

```powershell
# Install dotnet-coverage (one-time)
dotnet tool install -g dotnet-coverage

# Run tests with coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

## Test Naming Convention

Tests follow the pattern: `WhenCondition_ThenExpectedBehavior`

Example:
```csharp
[Fact]
public void WhenUserIsValid_ThenUserIsCreatedSuccessfully()
{
    // Arrange
    // Act
    // Assert
}
```

## Test Structure

Follow the **Arrange-Act-Assert (AAA)** pattern:

```csharp
[Fact]
public void WhenValidInput_ThenReturnsSuccess()
{
    // Arrange - Set up test data and dependencies
    var service = new MyService();
    var input = "test";
    
    // Act - Execute the method under test
    var result = service.DoSomething(input);
    
    // Assert - Verify the outcome
    Assert.True(result.IsSuccess);
}
```

## Test Best Practices

### General
- One behavior per test
- Tests should be independent and run in any order
- Use descriptive test names
- Follow existing naming conventions
- No branching/conditionals inside tests

### Mocking
- Mock external dependencies only
- Never mock code from GhcSamplePs.Core
- Use in-memory database for repository tests if needed

### Assertions
- Use clear assertions that match test name
- Assert specific values, not vague outcomes
- Avoid multiple assertions per test (prefer separate tests)

### Test Data
- Use test data builders for complex objects
- Randomize paths for any disk I/O tests
- Keep test data minimal and focused

## xUnit Specific

### Facts vs Theories

```csharp
// Simple test
[Fact]
public void SimpleTest() { }

// Parameterized test
[Theory]
[InlineData(1, 2, 3)]
[InlineData(2, 3, 5)]
public void AdditionTest(int a, int b, int expected)
{
    Assert.Equal(expected, a + b);
}
```

### Setup and Teardown

```csharp
public class MyServiceTests : IDisposable
{
    private readonly MyService _service;
    
    // Constructor runs before each test (Setup)
    public MyServiceTests()
    {
        _service = new MyService();
    }
    
    // Dispose runs after each test (Teardown)
    public void Dispose()
    {
        // Cleanup
    }
}
```

## Testing Async Code

```csharp
[Fact]
public async Task WhenCalledAsync_ThenReturnsExpectedResult()
{
    // Arrange
    var service = new MyService();
    
    // Act
    var result = await service.DoSomethingAsync();
    
    // Assert
    Assert.NotNull(result);
}
```

## Testing Exceptions

```csharp
[Fact]
public void WhenInvalidInput_ThenThrowsArgumentException()
{
    // Arrange
    var service = new MyService();
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => service.DoSomething(null));
}

[Fact]
public async Task WhenInvalidInputAsync_ThenThrowsArgumentException()
{
    // Arrange
    var service = new MyService();
    
    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(
        async () => await service.DoSomethingAsync(null)
    );
}
```

## Adding New Tests

1. Mirror the class structure from Core project
2. Create test class: `{ClassName}Tests.cs`
3. Write tests for all public methods
4. Focus on business logic and edge cases
5. Run tests to ensure they pass
6. Verify code coverage

## Coverage Goals

- Aim for high coverage of business logic
- Focus on testing public APIs
- Don't test trivial code (getters/setters)
- Test edge cases and error scenarios

## See Also

- [C# Testing Guidelines](../../.github/instructions/csharp.instructions.md#testing-best-practices)
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md#testing-strategy)

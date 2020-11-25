# Guidelines.RestApi.Collections.Filtering
Implementation of the Filtering API of [Microsoft Guidelines REST API](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md#97-filtering) using [Superpower](https://github.com/datalust/superpower). 

### Supported comparison operators
* Equal
```csharp
var expression = "Id eq 1";
```
* NotEqual
```csharp
var expression = "Id ne 1";
```
* GraterThan
```csharp
var expression = "Id gt 1";
```
* GraterThanOrEqual
```csharp
var expression = "Id ge 1";
```
* LessThan
```csharp
var expression = "Id lt 1";
```
* LessThanOrEqual
```csharp
var expression = "Id le 1";
```
* In
```csharp
var expression = "Id in (1,2,3)";
```

### Supported string functions
* Length
```csharp
var expression = "length(Title) gt 1";
```
* StartsWith
```csharp
var expression = "startswith(Title, 'Some')";
```
* EndsWith
```csharp
var expression = "endswith(Title, 'Title')";
```
* Contains
```csharp
var expression = "contains(Title, 'Title')";
```
* IndexOf
```csharp
var expression = "indexof(Title, 'Title') eq 0";
```

### Supported logical operators
* And
```csharp
var expression = "Id eq 1 and Value ne 5.0";
```
* Or
```csharp
var expression = "Id eq 1 or Value ne 5.0";
```
* Not
```csharp
var expression = "not Id eq 1";
```

### Usage
```csharp
var expression = "(length(Title) gt 5 and contains(Title, 'tle')) or Id in (3,5)";
// Parse a filtering function
Expression<Func<Item, bool>> predicateExpression = FilterParser.Parse<Item>(expression);
// Compile a filtering function
Func<Item, bool> predicate = FilterCompiler.Compile<Item>(expression);
```

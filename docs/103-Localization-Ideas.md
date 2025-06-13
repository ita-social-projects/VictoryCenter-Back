# Some Ideas for Localization of VC application

## Using MS packages (for localization of response data)

https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-9.0

Install packages:

```
dotnet add package Microsoft.Extensions.Localization
dotnet add package Microsoft.AspNetCore.Localization
```

## Database Localization (for data stored in tables)

Extend table with fields (`Description` -> `DescriptionTranslation`)


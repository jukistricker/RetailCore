# RetailCore
```bash 
dotnet ef migrations add InitialCreate \
--project RetailCore.Infrastructure \
--startup-project RetailCore.API \
--output-dir Data/Migrations
```

```bash
dotnet ef database update \
--project RetailCore.Infrastructure \
--startup-project RetailCore.API
```
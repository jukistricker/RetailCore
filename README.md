# 🛒 RetailCore System

A modern retail management ecosystem built on **.NET 10** and **React (Vite)**.

## 🏗 Project Structure
*   **RetailCore.API**: The main RESTful entry point.
*   **RetailCore.Application**: Business logic, Use Cases, and Mapping profiles.
*   **RetailCore.Domain**: Core entities, Enums, and domain-driven logic.
*   **RetailCore.Infrastructure**: Database persistence (EF Core), Migrations, and External providers.
*   **RetailCore.Shared**: Cross-cutting DTOs and ViewModels.
*   **RetailCore.CustomerSite**: Frontend for end-users (ASP.NET Core MVC/Razor).
*   **RetailCore.Test**: Unit and Integration testing suite.
*   **retail-admin-dashboard**: Administrative interface (React + Vite).

---

## 🛠 Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+ & npm 10+](https://nodejs.org/)
- [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

## 🚀 Execution Guide 

To get the entire ecosystem running on your Linux environment with the required port mapping, follow these steps:

### 1. Database Initialization
Ensure your migrations are applied to the database before launching the services:

```bash
dotnet ef database update --project RetailCore.Infrastructure --startup-project RetailCore.API
```
### 2. Launch All Services 
Run the following command from the root directory (RetailCore). This uses the & operator to execute the Backend, Customer Site and Admin Dashboard concurrently:
```bash
dotnet run --project RetailCore.API --urls "http://localhost:5016" & \
dotnet run --project RetailCore.CustomerSite --urls "http://localhost:5265" & \
cd retail-admin-dashboard && npm install && npm start 
```
## NOTE: Please carefully check the ports used in the command above to avoid port conflicts, ensuring admin dashboard run on port 5173.

### 🧪 Testing
Execute the test suite using the following command:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

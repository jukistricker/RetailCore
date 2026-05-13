# 🛒 RetailCore System

A modern retail management ecosystem built on **.NET 10** and **React (Vite)**. This project implements **Clean Architecture**, focusing on high-performance logic and minimal resource footprints (targeting < 2% CPU and ~ tens of MBs RAM).

## 🏗 Project Structure
*   **RetailCore.API**: The main entry point and RESTful gateway.
*   **RetailCore.Application**: Business logic, Use Cases, and Mapping profiles.
*   **RetailCore.Domain**: Core entities, Enums, and domain-driven logic.
*   **RetailCore.Infrastructure**: Database persistence (EF Core), Migrations, and External providers.
*   **RetailCore.Shared**: Cross-cutting DTOs and Utility classes.
*   **RetailCore.CustomerSite**: Frontend for end-users (ASP.NET Core MVC/Razor).
*   **RetailCore.Test**: Unit and Integration testing suite.
*   **retail-admin-dashboard**: Administrative interface (React + Vite).

---

## 🛠 Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js & npm](https://nodejs.org/)
- [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

## 🚀 Execution Guide (Simultaneous Launch)

To get the entire ecosystem running on your Linux environment with the required port mapping, follow these steps:

### 1. Database Initialization
Ensure your migrations are applied to the database before launching the services:

```bash
dotnet ef database update --project RetailCore.Infrastructure --startup-project RetailCore.API
```
### 2. Launch All Services (One-liner)
Run the following command from the root directory (RetailCore). This uses the & operator to execute the Backend, Customer Site, and Admin Dashboard concurrently:
```bash
dotnet run --project RetailCore.API --urls "http://localhost:5016" & \
dotnet run --project RetailCore.CustomerSite --urls "http://localhost:5265" & \
cd retail-admin-dashboard && npm install && npm start -- --port 5173
```

🧪 Testing
Execute the test suite using the following command:
```bash
dotnet test RetailCore.Test
```

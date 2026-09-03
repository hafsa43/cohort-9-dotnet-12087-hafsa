# 📋 Task Management System (.NET 8 + React Vite)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)
![Vite](https://img.shields.io/badge/Vite-5.0-646CFF?logo=vite)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4)
![SonarQube](https://img.shields.io/badge/SonarQube-Quality%20Gate-4E9BCD?logo=sonarqube)
![License](https://img.shields.io/badge/License-MIT-green)

A full-stack enterprise-grade Task Management application built with a **.NET 8 Web API** backend following Clean Architecture principles and a responsive **React (Vite)** frontend.

Developed by **Hafsa Rehman** for **10PShine Cohort 9 (.NET Fullstack Assignment)**.

---

## 🌟 Key Features

### 🔐 Authentication & Authorization
- **JWT Token Authentication**: Secure stateless authentication using JSON Web Tokens.
- **Role-Based Access Control (RBAC)**: Role-specific access levels (`Admin` vs. `User`).
- **Password Security**: Safe password hashing utilizing `BCrypt.Net`.

### 📝 Task Management
- **Full CRUD Operations**: Create, view, update, and delete tasks.
- **Advanced Server-Side Features**:
  - **Searching**: Search tasks by title or description.
  - **Filtering**: Filter tasks by priority (`Low`, `Medium`, `High`), status (`Pending`, `In Progress`, `Completed`), or Category.
  - **Sorting**: Dynamic multi-column sorting (e.g., by Due Date, Priority, Title).
  - **Pagination**: Efficient server-side page chunking.

### 🏷️ Category & User Management
- **Category Management**: Organize tasks under custom categories.
- **User Administration**: Admin interface for reviewing registered users and managing roles.
- **User Profile**: Update profile details and view personalized user stats.

### 🧪 Quality Assurance & CI/CD
- **Unit Testing Suite**: 20 comprehensive unit tests using xUnit covering core services (`AuthService`, `CategoryService`, `TaskService`).
- **SonarQube Integration**: Static code quality and security vulnerability scanning with custom `sonar-project.properties` and GitHub Actions workflow (`.github/workflows/sonar.yml`).

---

## 🏗️ Architecture & Project Structure

The repository follows **Clean Architecture** principles to separate concerns and ensure maintainability and testability:

```text
TaskManagement/
├── TaskManagement.API/             # Presentation Layer: Controllers, Middleware, Swagger
├── TaskManagement.Application/     # Application Layer: DTOs, Service Interfaces
├── TaskManagement.Domain/          # Domain Layer: Entities (AppUser, TaskItem, Category), Enums
├── TaskManagement.Infrastructure/  # Infrastructure Layer: EF Core AppDbContext, Migrations, Repositories
├── TaskManagement.Tests/           # Testing Layer: xUnit Unit Tests
└── task-management-ui/             # Frontend Layer: React + Vite Single Page Application
    ├── src/
    │   ├── components/            # Reusable UI components (Sidebar, ProtectedRoute, etc.)
    │   ├── context/               # Authentication Context & State
    │   ├── pages/                 # Main Views (Login, Dashboard, Tasks, Users, Profile)
    │   └── services/              # API Client & Service Hooks
    └── vite.config.js
```

---

## 💻 Tech Stack

### **Backend**
- **Framework**: .NET 8 Web API
- **ORM**: Entity Framework Core 8 (Code-First)
- **Database**: SQL Server / SQL Express
- **Authentication**: System.IdentityModel.Tokens.Jwt + BCrypt.Net
- **Testing**: xUnit, Moq, FluentAssertions

### **Frontend**
- **Library**: React 18
- **Build Tool**: Vite
- **Routing**: React Router DOM v6
- **Styling**: Modern Custom CSS (Dark/Light aesthetic tokens, Responsive Layouts)

### **DevOps & Quality**
- **Code Quality**: SonarQube & SonarScanner
- **CI/CD**: GitHub Actions Workflows

---

## 🚀 Getting Started

### Prerequisites
Make sure you have the following installed locally:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+)](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

---

### 1. Clone the Repository
```bash
git clone https://github.com/10pshine-cohort-9/cohort-9-dotnet-12087-hafsa.git
cd cohort-9-dotnet-12087-hafsa
```

---

### 2. Backend Setup (.NET 8 Web API)

1. Navigate to the solution directory:
   ```bash
   cd TaskManagement
   ```

2. Configure Database Connection String:
   Open `TaskManagement.API/appsettings.json` and adjust your connection string if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Run EF Core Migrations to create the database:
   ```bash
   dotnet ef database update --project TaskManagement.Infrastructure --startup-project TaskManagement.API
   ```

4. Run the API project:
   ```bash
   dotnet run --project TaskManagement.API
   ```
   The API will launch at `https://localhost:7198` (Swagger docs available at `https://localhost:7198/swagger`).

---

### 3. Frontend Setup (React + Vite)

1. Open a new terminal and navigate to the UI folder:
   ```bash
   cd task-management-ui
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Configure Environment Variables:
   Ensure `.env` contains your backend API URL:
   ```env
   VITE_API_BASE_URL=https://localhost:7198/api
   ```

4. Start the Vite development server:
   ```bash
   npm run dev
   ```
   Access the app in your browser at `http://localhost:5173`.

---

## 🧪 Running Unit Tests

To execute the unit test suite:
```bash
dotnet test TaskManagement.Tests
```

---

## 🔬 SonarQube Code Quality Analysis

### Local Scan (PowerShell)
Make sure SonarScanner is installed, then run:
```powershell
.\run-sonar-analysis.ps1
```

### GitHub Actions Workflow
SonarQube scans run automatically on pushes and pull requests via `.github/workflows/sonar.yml`.

---

## 🤝 Author & Acknowledgments

- **Author**: Hafsa Rehman ([@hafsa43](https://github.com/hafsa43))
- **Cohort**: 10PShine Cohort 9 — .NET Fullstack Track

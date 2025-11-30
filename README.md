# ChildCare Connect - .NET Blazor Server Edition

A C# .NET 8 Blazor Server implementation of the ChildCare Connect resource management system. This version provides the same functionality as the Node.js/React version but built with Microsoft's technology stack.

## Technology Stack

- **Framework**: ASP.NET Core 8.0 with Blazor Server
- **Database**: PostgreSQL with Entity Framework Core
- **UI**: Razor Components with Tailwind-inspired CSS
- **Real-time**: SignalR (built into Blazor Server)

## Project Structure

```
ChildCareConnectDotNet/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor      # Main layout with sidebar
│   │   └── Sidebar.razor         # Navigation sidebar
│   ├── Pages/
│   │   ├── Dashboard.razor       # Dashboard with statistics
│   │   ├── Team.razor            # Team member management
│   │   ├── Clients.razor         # Client listing
│   │   ├── AddClient.razor       # Add new client form
│   │   ├── Providers.razor       # Provider listing
│   │   └── FormBuilder.razor     # Custom form field builder
│   ├── Shared/
│   │   └── StatCard.razor        # Reusable stat card component
│   ├── App.razor                 # Root component
│   ├── Routes.razor              # Router configuration
│   └── _Imports.razor            # Global imports
├── Data/
│   └── AppDbContext.cs           # Entity Framework DbContext
├── Models/
│   ├── User.cs                   # User/staff model
│   ├── Client.cs                 # Client family model
│   ├── Provider.cs               # Care provider model
│   ├── FormField.cs              # Custom form field model
│   ├── ClientCustomField.cs      # Client custom field values
│   └── ProviderCustomField.cs    # Provider custom field values
├── Services/
│   ├── UserService.cs            # User CRUD operations
│   ├── ClientService.cs          # Client CRUD operations
│   ├── ProviderService.cs        # Provider CRUD operations
│   ├── FormFieldService.cs       # Form field management
│   └── StatsService.cs           # Dashboard statistics
├── wwwroot/
│   └── css/
│       └── app.css               # Tailwind-inspired styles
└── Program.cs                    # Application entry point
```

## Features

### Dashboard
- Overview statistics for clients, providers, and children
- Quick action buttons for common tasks

### Team Management
- View all staff members
- Role-based display (Admin, Manager, Case Manager, Coordinator)
- Status tracking (Active, Away, Inactive)

### Client Management
- List all client families
- Add new clients with dynamic forms
- Filter by case manager
- Status workflow (Active, Pending, Inactive, On Hold)

### Provider Management
- View care provider network
- Provider types: Center, In-Home, Preschool
- Capacity and enrollment tracking
- Rating and status display

### Form Builder (Admin/Manager only)
- Create custom form fields
- Configure field types (text, number, email, phone, date, textarea, select, checkbox)
- Set field width (full, half, third)
- Drag-to-reorder functionality
- Required field configuration

## Role-Based Access Control

| Feature | Admin | Manager | Case Manager | Coordinator |
|---------|-------|---------|--------------|-------------|
| Dashboard | ✅ | ✅ | ✅ | ✅ |
| View Team | ✅ | ✅ | ✅ | ✅ |
| View Clients | ✅ | ✅ | ✅ (filtered) | ✅ |
| Add Clients | ✅ | ✅ | ✅ | ✅ |
| View Providers | ✅ | ✅ | ✅ | ✅ |
| Form Builder | ✅ | ✅ | ❌ | ❌ |

## Running the Application

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- DATABASE_URL environment variable

### Build and Run
```bash
cd ChildCareConnectDotNet
dotnet restore
dotnet build
dotnet run
```

The application will be available at `http://localhost:5000` (or the configured port).

## Database Configuration

The application uses the same PostgreSQL database as the Node.js version. Set the `DATABASE_URL` environment variable with your connection string:

```
DATABASE_URL=postgresql://user:password@host:port/database
```

The application will automatically seed initial data on first run if the database is empty.

## Comparison with Node.js Version

| Aspect | Node.js/React | .NET Blazor |
|--------|---------------|-------------|
| Language | TypeScript | C# |
| Framework | Express + React | ASP.NET Core + Blazor |
| Database ORM | Drizzle | Entity Framework Core |
| Real-time | WebSockets | SignalR |
| State Management | React Query + Context | Blazor State + DI |
| Build Tool | Vite | MSBuild |

Both versions provide equivalent functionality and share the same database schema.

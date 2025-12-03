# ChildCare Connect - Blazor Server Edition

## Project Overview

ChildCare Connect is a comprehensive resource management system for childcare organizations built with .NET 9 Blazor Server. This application helps manage clients (families with household members), providers (childcare facilities), team members, tasks, and custom forms for data collection.

**Technology Stack:**
- .NET 9.0.302 Blazor Server
- PostgreSQL with Entity Framework Core 9.0
- SignalR (built into Blazor)
- Tailwind-inspired CSS

## Current State

**Status:** ✅ Fully functional and running in Replit environment

**Last Updated:** December 3, 2024

The application has been successfully configured for the Replit environment with:
- Database tables created and seeded with sample data
- Server listening on 0.0.0.0:5000
- PostgreSQL database connected and operational
- Deployment configured for autoscale

## Project Structure

```
ChildCareConnect/
├── Components/           # Blazor components
│   ├── Layout/          # Layout components (MainLayout, Sidebar)
│   ├── Pages/           # Page components (Dashboard, Team, Clients, etc.)
│   ├── Shared/          # Shared/reusable components
│   └── Routes.razor     # Routing configuration
├── Data/                # Database context
│   └── AppDbContext.cs  # EF Core DbContext
├── Models/              # Entity models
│   ├── User.cs
│   ├── Client.cs
│   ├── Provider.cs
│   ├── FormField.cs
│   └── Custom field models
├── Services/            # Business logic services
│   ├── UserService.cs
│   ├── ClientService.cs
│   ├── ProviderService.cs
│   ├── FormFieldService.cs
│   └── StatsService.cs
├── wwwroot/             # Static files
│   └── css/app.css      # Styles
└── Program.cs           # Application entry point
```

## Key Features

### Dashboard
- Real-time statistics for clients, providers, children, and tasks
- Quick action buttons
- Customizable widgets (Admin/Manager only)
- Clickable stat cards that navigate to respective pages

### Task Management
- Create, edit, and delete tasks
- Assign tasks to yourself or other team members
- Priority levels: Low, Normal, High, Urgent
- Status workflow: Pending, In Progress, Completed
- Due date tracking with overdue highlighting
- Tag-based organization with multi-tag support
- Filter by status, priority, and view (My Tasks / All Tasks)
- Dashboard widget showing pending task count

### Tag Management (Admin/Manager only)
- Create custom tags with color coding
- 10 color options for visual organization
- Track tag usage across tasks
- Edit and delete tags (only unused tags can be deleted)

### Team Management
- View all staff members with search functionality
- Add new team members (Admin/Manager only)
- Edit team member details: name, email, role, team, status (Admin/Manager only)
- Create new teams or select from existing teams
- Role-based access (Admin, Manager, Case Manager, Coordinator)
- Status tracking (Active, Away, Inactive)

### Client Management
- Manage family clients with household members
- Track household size and individual family members
- Dynamic household member input with relationship tracking
- Primary contact is automatically added as the first household member with "Self" relationship
- 9 relationship types: Self (auto-assigned to primary contact), Spouse/Partner, Child, Parent, Sibling, Grandparent, Grandchild, Other Relative, Non-Relative
- Case manager assignments (auto-selects first eligible manager)
- Status workflow (Active, Pending, Inactive, On Hold)
- Custom fields support

### Provider Management
- Childcare provider network
- Capacity and enrollment tracking
- Provider types: Center, In-Home, Preschool
- Rating and verification status

### Form Builder
- Create custom form fields (Admin/Manager only)
- Multiple field types: text, number, email, phone, date, textarea, select, checkbox
- Configurable field width and ordering
- Required field settings

## Database Schema

The application uses PostgreSQL with the following main tables:
- `users` - Staff members with roles and team assignments
- `clients` - Family clients with household_size and case manager relationships
- `relationships` - Relationship types (Spouse/Partner, Child, Parent, etc.)
- `household_members` - Individual household members linked to clients with relationships
- `providers` - Childcare providers with capacity tracking
- `form_fields` - Custom form field definitions
- `client_custom_fields` - Custom field values for clients
- `provider_custom_fields` - Custom field values for providers
- `tasks` - Task items with assignee, creator, priority, status, and due date
- `tags` - Color-coded tags for organizing tasks
- `task_tags` - Many-to-many relationship between tasks and tags

## Environment Configuration

**Required Environment Variables:**
- `DATABASE_URL` - PostgreSQL connection string (automatically set by Replit)

**Database:**
The application uses Replit's built-in PostgreSQL database. The schema is created automatically on first run using EF Core's `EnsureCreatedAsync()` method.

## Development

**Running the Application:**
The workflow is pre-configured. The app runs on port 5000 and is accessible via the Replit webview.

**Key Files to Modify:**
- `Components/Pages/*.razor` - Page components
- `Services/*.cs` - Business logic
- `Models/*.cs` - Data models
- `wwwroot/css/app.css` - Styling

## Deployment

The application is configured for deployment with:
- **Deployment Type:** Autoscale (stateless web application)
- **Run Command:** `dotnet run --no-launch-profile`
- **Port:** 5000

To publish the application, click the "Deploy" button in Replit.

## Recent Changes

### Household Member Management (Dec 3, 2024)
- Upgraded from .NET 8 to .NET 9.0.302
- Added Relationship and HouseholdMember models
- Created RelationshipService and HouseholdMemberService for data access
- Renamed `children` column to `household_size` in clients table
- Added `relationships` table with 9 relationship types (including "Self")
- Added `household_members` table linked to clients with relationships
- Redesigned AddClient.razor with dynamic household member rows
- Primary contact is automatically added as the first household member with "Self" relationship
- Auto-selection of first eligible manager as Case Manager
- Dynamic form updates based on household size selection

### Task Management Feature (Dec 1, 2024)
- Added TaskItem, Tag, and TaskTag models for task management
- Created TaskService with full CRUD operations (create, update, delete, status change)
- Created TagService for managing color-coded tags
- Added Tasks page with filtering (status, priority, view) and create/edit modal
- Added Tags management page for Admins/Managers to create/edit/delete tags
- Added "Pending Tasks" dashboard widget with clickable navigation
- Updated sidebar with Tasks and Manage Tags navigation links
- Seeded sample tasks and tags for testing

### Initial Setup (Dec 1, 2024)
- Installed .NET 8.0 SDK
- Created PostgreSQL database
- Configured Kestrel to listen on 0.0.0.0:5000 for Replit environment
- Updated database initialization to use EnsureCreatedAsync
- Created .gitignore for .NET projects
- Configured deployment for autoscale
- Seeded database with sample data

## Known Issues

None currently. Application is running successfully.

## User Preferences

No specific preferences set yet.

## Notes

- The application uses Blazor Server, which maintains a persistent connection via SignalR
- Database schema is created automatically on startup if it doesn't exist
- Sample data is seeded on first run for testing purposes
- All role-based features are fully functional

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
- **Extended Demographics:**
  - Date of Birth, Gender (Male, Female, Non-binary, Other), Race/Ethnicity
  - Nationality and Citizenship Status (U.S. Citizen, Permanent Resident, Temporary Resident, Undocumented, Other)
  - **Multiple Phone Numbers:** Add as many phone numbers as needed with type selector (Mobile, Main, Work, Other) - auto-formatted as (XXX) XXX-XXXX
  - Social Security Number with AES-256 encryption for database protection and show/hide toggle on form
- **Client Detail Page:**
  - Click any client from the list to view full details
  - Authorized users (Admin/Manager) can view and toggle SSN visibility
  - Non-authorized users see a permission message

### Provider Management
- Childcare provider network
- Capacity and enrollment tracking
- Provider types: Center, In-Home, Preschool
- Rating and verification status

### Form Builder (Enhanced)
- **Section Management:**
  - View, add, edit, and delete form sections
  - System sections (Basic Information, Contact Information, Household Information) are protected
  - Toggle section visibility on/off
  - Reorder sections as needed
- **Field Management:**
  - View all fields organized by sections
  - Add new custom fields to any section
  - Edit field properties: label, type, width, placeholder, options, help text
  - Toggle field visibility on/off
  - Multiple field types: text, number, email, phone, date, textarea, select, checkbox
  - Configurable field widths: Full, Half (1/2), Third (1/3)
  - System fields are protected from deletion but can be edited
  - Required field settings
- **Dynamic Form Rendering:**
  - Add Client form automatically reflects Form Builder configuration
  - Hiding a field in Form Builder immediately hides it from the client intake form
  - Field order, widths, and labels are all configurable
  - Real-time updates without code changes
- **Visual Layout:**
  - Section-based layout matches the actual form structure
  - Fields displayed in their configured widths
  - Drag handles for future drag-and-drop reordering

## Database Schema

The application uses PostgreSQL with the following main tables:
- `users` - Staff members with roles and team assignments
- `clients` - Family clients with household_size and case manager relationships
- `relationships` - Relationship types (Spouse/Partner, Child, Parent, etc.)
- `household_members` - Individual household members linked to clients with relationships
- `phone_numbers` - Multiple phone numbers per client with type (Mobile, Main, Work, Other)
- `providers` - Childcare providers with capacity tracking
- `form_sections` - Form section definitions with visibility and ordering
- `form_fields` - Form field definitions with section assignment, visibility, and width settings
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

### Dynamic Form Rendering (Dec 3, 2024)
- Implemented dynamic form rendering for Add Client page
- Add Client form now automatically reflects Form Builder configuration:
  - Loads sections and fields from Form Builder database
  - Respects field visibility settings (hide/show fields from Form Builder)
  - Renders fields in configured order
  - Uses field widths, labels, placeholders from Form Builder
- Tested visibility toggle - hiding a field in Form Builder immediately hides it from Add Client form
- Special handling for system fields (SSN, phone numbers, household members) preserved

### Form Builder Enhancement (Dec 3, 2024)
- Created FormSection model for organizing fields into sections
- Updated FormField model with section_id, is_visible, model_property, help_text fields
- Created FormSectionService for section CRUD operations
- Enhanced FormFieldService with section and visibility management
- Redesigned Form Builder page with visual section-based layout
- Created form_sections database table with 3 default sections
- Seeded 12 system fields representing all Client form fields
- Section and field visibility toggle support
- Section and field editing dialogs with full property management

### UI Refinements for Client Form (Dec 3, 2024)
- Auto-generate family name from primary contact: "The" + lastname + "Family"
- Moved SSN field from Contact Information into Basic Information section
- Optimized phone number fields to half-width layout (field + type dropdown side-by-side)
- Better proportional sizing for phone type dropdown and Remove button
- Improved form visual hierarchy and user experience

### Multiple Phone Numbers per Client (Dec 3, 2024)
- Created PhoneNumber model with phone, phone_type, and client_id fields
- Created PhoneNumberService for managing multiple phone numbers
- Updated Client model with PhoneNumbers collection (one-to-many relationship)
- Enhanced AddClient.razor form with dynamic phone number fields:
  - Add multiple phone numbers as needed
  - Remove individual phone numbers
  - Select phone type for each (Mobile, Main, Work, Other)
  - Auto-formatted as (XXX) XXX-XXXX
- Updated ClientDetail.razor to display all phone numbers grouped by type
- Created phone_numbers database table with client_id foreign key

### SSN Viewing for Authorized Users (Dec 3, 2024)
- Created ClientDetail.razor page for viewing client information
- Added authorization check to display SSN only for Admin/Manager roles
- Implemented Show/Hide toggle for SSN decryption and viewing
- Non-authorized users see permission message on detail page
- Rows in Clients list are now clickable to navigate to detail page

### Extended Demographics for Clients (Dec 3, 2024)
- Added comprehensive demographic fields to Client model: Date of Birth, Gender, Race/Ethnicity, Nationality, Citizenship Status
- Added phone number field with type selector (Mobile/Main) with auto-formatting as (XXX) XXX-XXXX
- Added Social Security Number field with AES-256 encryption in database storage
- Created EncryptionService for SSN encryption/decryption and phone number formatting
- Updated AddClient.razor form with new demographic section and contact information section
- Implemented show/hide toggle for SSN field on form for privacy
- Updated ClientService to encrypt SSN on create and update operations
- All demographic fields are optional except for family name and primary contact

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

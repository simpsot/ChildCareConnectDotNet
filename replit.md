# ChildCare Connect - Blazor Server Edition

### Overview
ChildCare Connect is a comprehensive resource management system designed for childcare organizations. Built with .NET 9 Blazor Server, its primary purpose is to streamline the management of clients (families and household members), providers (childcare facilities), team members, tasks, and custom forms for data collection. The project aims to provide a robust, scalable solution for childcare resource management.

### Recent Changes (Dec 4, 2025)
- **Fixed Tab/Section Switching:** Implemented proper state management with StateHasChanged() calls when switching between view and edit modes
- **Fixed Save Button:** Now reloads client data after saving and ensures UI updates properly
- **Fixed Back Navigation:** Back buttons work correctly on detail page
- **Added Phone Number Management:** Users can now add/delete phone numbers while editing a client, even if no phone numbers existed initially
  - Add new phone numbers with type selection (Main, Mobile, Work, Other)
  - Delete existing phone numbers during edit
  - Phone number changes persist when saved

### User Preferences
No specific preferences set yet.

### System Architecture
The application is built on .NET 9.0.302 Blazor Server, utilizing a PostgreSQL database with Entity Framework Core 9.0. SignalR is leveraged for real-time communication, inherent to Blazor. The UI adopts a Tailwind-inspired CSS for styling.

**Key Architectural Decisions and Features:**
*   **Modular Component Structure:** Organized into `Components` (Layout, Pages, Shared), `Data`, `Models`, and `Services` for clear separation of concerns.
*   **Role-Based Access Control:** Implemented for features like Tag Management, Team Member editing, and SSN viewing, with roles including Admin, Manager, Case Manager, and Coordinator.
*   **Dynamic Form Builder:** Allows for the creation and management of custom forms with configurable sections and fields. Fields can be of various types (text, number, email, phone, date, textarea, select, checkbox) and have configurable widths (Full, Half, Third). System fields are protected but editable, and field visibility can be toggled.
*   **Real-time Dashboard:** Provides statistics for clients, providers, children, and tasks, with quick action buttons and customizable widgets.
*   **Comprehensive Client Management:**
    *   Manages family clients, tracking household members and their relationships (9 types including "Self").
    *   Supports extended demographics (Date of Birth, Gender, Race/Ethnicity, Nationality, Citizenship Status).
    *   Allows multiple phone numbers per client with type selection and auto-formatting.
    *   Phone numbers can be added/edited/deleted in inline edit mode on client detail page.
    *   Social Security Numbers are stored with AES-256 encryption, with role-based viewing and a show/hide toggle.
    *   Case managers are auto-assigned.
    *   Client status workflow includes Active, Pending, Inactive, On Hold.
    *   Inline edit mode with Save/Cancel buttons for quick edits without separate page.
*   **Provider Management:** Tracks childcare providers, capacity, enrollment, provider types (Center, In-Home, Preschool), and verification status.
*   **Task Management:** Features task creation, editing, deletion, assignment, priority levels (Low, Normal, High, Urgent), status workflow (Pending, In Progress, Completed), due date tracking, and tag-based organization.
*   **Tag Management:** (Admin/Manager only) Allows creation of custom, color-coded tags for task organization.
*   **Team Management:** Enables viewing, adding, and editing staff members, roles, and teams, with status tracking (Active, Away, Inactive).
*   **Encryption Service:** Dedicated service for handling SSN encryption/decryption and phone number formatting.

### External Dependencies
*   **PostgreSQL:** Used as the primary database, integrated via Entity Framework Core 9.0.
*   **Replit's built-in PostgreSQL:** The application leverages Replit's managed PostgreSQL service.
*   **SignalR:** Integrated as part of Blazor Server for real-time client-server communication.

# Mental Wellness System

<div align="center">

![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=for-the-badge&logo=asp.net)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![License](https://img.shields.io/badge/License-Educational-blue?style=for-the-badge)

**A comprehensive web-based mental health management system designed to facilitate seamless interaction between patients, doctors, and administrators.**

[Features](#-features) • [Installation](#-getting-started) • [Documentation](#-documentation) • [Team](#-team-members)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Key Features in Detail](#-key-features-in-detail)
- [Database Schema](#-database-schema)
- [Security](#-security)
- [Deployment](#-deployment)
- [Configuration](#-configuration)
- [Troubleshooting](#-troubleshooting)
- [Team Members](#-team-members)
- [License](#-license)

---

## 🎯 Overview

The **Mental Wellness System** is a full-featured ASP.NET Core Razor Pages application that provides a complete solution for mental health clinics. It enables patients to book appointments, track their mental health, and access personalized resources, while allowing doctors to manage appointments, create treatment plans, and maintain patient records. Administrators can oversee the entire system, approve doctors, and generate comprehensive reports.

### Core Capabilities

- ✅ **Role-Based Access Control** - Three distinct user roles (Patient, Doctor, Admin)
- ✅ **Appointment Management** - Complete appointment lifecycle with double-booking prevention
- ✅ **Mental Health Tracking** - Daily mood logging and progress monitoring
- ✅ **Treatment Planning** - Structured treatment plans with goals and tasks
- ✅ **Community Support** - Peer support forum for patients
- ✅ **Automated Notifications** - Email notifications for appointments and reminders
- ✅ **Comprehensive Reporting** - Analytics and reports for all stakeholders
- ✅ **Audit Logging** - Complete audit trail for security and compliance

---

## ✨ Features

### 🔐 Authentication & Authorization

- **ASP.NET Core Identity** integration with custom `ApplicationUser` model
- **Three Role System**: Patient, Doctor, and Admin with granular permissions
- **Role-based page authorization** for all modules and pages
- **Strong password policy** (8+ characters, uppercase, lowercase, digit, special character)
- **Account lockout** protection (5 failed attempts, 5-minute lockout)
- **Email confirmation** support (configurable)

### 👤 Patient Module

#### Dashboard
- Overview of upcoming appointments
- Quick statistics (total appointments, pending, completed)
- Recent mood logs and trends
- Active treatment plans
- Personalized recommendations

#### Appointment Management
- **Book Appointment**: Select doctor, date, and time with visual calendar
- **Double-booking prevention** using database transactions
- **Appointment History**: View past, current, and cancelled appointments
- **Advanced Filtering**: Filter by status, date range, doctor
- **View Appointment Details**: Complete appointment information
- **PDF Download**: Generate and download appointment confirmation PDFs

#### Mental Health Tracking
- **Daily Mood Logging**: Track mood (1-10), stress, sleep, and energy levels
- **Mood History**: View historical mood data in table format
- **Automatic Recommendations**: System generates recommendations when mood is low
- **Prevent Duplicate Entries**: Updates existing entry if logged on same date

#### Treatment & Resources
- **View Treatment Plans**: See all active and completed treatment plans
- **Personalized Recommendations**: AI-powered resource recommendations based on:
  - Mood patterns (low mood, high stress, poor sleep)
  - Active treatment plans
  - Health trends
- **Resource Types**: Articles, Exercises, Meditation guides, Videos
- **Priority Levels**: High, Medium, Low priority recommendations

#### Community & Feedback
- **Community Forum**: Peer support with categories:
  - General Discussion
  - Support Requests
  - Questions
  - Success Stories
- **Create Posts**: Share experiences (with anonymous option)
- **Comment & Like**: Engage with community posts
- **Feedback System**: Rate appointments and doctors (1-5 stars)
- **Anonymous Feedback**: Option to submit anonymous feedback

#### Profile Management
- Update email, phone, age, and gender
- View PatientID (immutable, automatically generated)
- Change password functionality
- View registration date and account status

### 👨‍⚕️ Doctor Module

#### Dashboard
- **Appointment Overview**: View appointments filtered by Day/Week/Month/Year
- **Statistics**: Total appointments, completed, scheduled, cancelled
- **Upcoming Appointments**: Next appointments at a glance
- **Treatment Plans**: Active treatment plans summary
- **Patient Feedback**: View ratings and feedback received

#### Appointment Management
- **Manage Appointments**: Update status (Scheduled/Ongoing/Completed/Cancelled)
- **Add Notes**: Document appointment details and observations
- **View Appointment History**: Complete appointment timeline
- **Patient Information**: Quick access to patient details

#### Patient Records
- **View Patient Records**: Complete medical history for each patient
- **Add Patient Records**: Create detailed session notes
- **Diagnosis Tracking**: Record diagnoses and observations
- **Follow-up Requests**: Schedule and track follow-up appointments
- **Record Categories**: Organize records by type and importance

#### Treatment Planning
- **Create Treatment Plans**: Structured plans with:
  - Title and description
  - Multiple goals and tasks
  - Start and end dates
  - Status tracking (Active, Completed, Cancelled)
  - Progress notes
- **Link to Appointments**: Associate plans with specific appointments
- **Link to Patient Records**: Connect plans with relevant medical records
- **Automatic Recommendations**: System generates recommendations when plans are created

#### Profile Management
- Update profile information
- View specialty and approval status
- Change password
- View registration date

### 👨‍💼 Admin Module

#### Dashboard
- **System Statistics**: Comprehensive metrics including:
  - Total users (patients, doctors, admins)
  - Pending doctor approvals
  - Appointment statistics (total, scheduled, completed, cancelled)
  - Active treatment plans
  - Total mood logs
  - Community posts and engagement
- **Recent Activity**: Latest system events and registrations
- **Quick Actions**: Direct links to key administrative tasks

#### Doctor Approval
- **Pending Doctors List**: View all doctors awaiting approval
- **Doctor Information**: View complete doctor profile before approval
- **Approve/Reject**: One-click approval or rejection with confirmation
- **Email Notifications**: Automatic notifications to doctors upon approval

#### User Management
- **View All Users**: Complete list of all system users
- **User Details**: View comprehensive user profiles
- **Delete Users**: Remove users from the system (with confirmation)
- **User Statistics**: View user registration trends

#### Reports & Analytics
- **Appointment Reports**: 
  - Statistics by time period (day, week, month, year)
  - No-show rates and analysis
  - Appointment trends and patterns
- **Activity Logs**: Complete system activity tracking
- **Audit Trails**: Security and compliance logging
- **User Activity**: Track user engagement and usage patterns
- **Export Capabilities**: Generate reports for external use

### 📧 Notification System

- **Email Notifications**: 
  - Booking confirmations
  - Appointment reminders (24 hours and 1 hour before)
  - Cancellation notices
  - Doctor approval notifications
  - System announcements
- **Background Processing**: Asynchronous notification handling via `NotificationBackgroundService`
- **Database Logging**: All notifications logged with status tracking
- **Configurable SMTP**: Easy integration with any email provider
- **Retry Mechanism**: Automatic retry for failed notifications

### 📄 PDF Generation

- **Appointment Confirmations**: Professional PDF documents
- **Includes**: 
  - PatientID and patient information
  - Doctor details and specialty
  - Appointment date, time, and location
  - Clinic branding and contact information
- **Secure Access**: Only authorized patient or doctor can download
- **Customizable**: Easy to modify templates and branding

### 🔒 Security Features

- **HTTPS Enforcement**: All traffic encrypted in production
- **Password Hashing**: Secure password storage using ASP.NET Identity
- **Role-based Access Control**: Pages and APIs protected by authorization policies
- **Audit Logging**: Complete audit trail for:
  - Patient record access
  - User actions
  - System changes
  - IP address tracking
- **Transaction Safety**: Double-booking prevention using database transactions
- **SQL Injection Protection**: Parameterized queries via Entity Framework Core
- **XSS Protection**: Razor Pages built-in protection
- **CSRF Protection**: Anti-forgery tokens on forms

### 🗄️ Database Features

- **Entity Framework Core 8.0**: Modern ORM with migrations
- **SQL Server**: Production-ready database
- **Unified DbContext**: `MentalWellnessDbContext` extends `IdentityDbContext`
- **Proper Relationships**: Foreign keys, constraints, and indexes
- **Migrations Support**: Version-controlled database schema
- **Connection Resilience**: Automatic retry logic for transient failures

---

## 🛠️ Technology Stack

### Backend
- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Razor Pages** - Modern web application framework
- **Entity Framework Core 8.0** - Object-relational mapping
- **ASP.NET Core Identity** - Authentication and authorization
- **SQL Server** - Relational database management system

### Frontend
- **Bootstrap 5** - Responsive CSS framework
- **Razor Pages** - Server-side rendering
- **JavaScript** - Client-side interactivity
- **CSS3** - Modern styling

### Services & Infrastructure
- **Background Services (IHostedService)** - Asynchronous task processing
- **SMTP** - Email delivery
- **PDF Generation** - Document creation
- **Audit Logging** - Security and compliance

### Development Tools
- **Visual Studio 2022** / **VS Code** - IDE
- **Git** - Version control
- **Package Manager Console** - NuGet package management

---

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or Full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads) (for version control)

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd MentalWellnessSystem_Fixed/MentalWellnessSystem/MentalWellnessSystem
   ```

2. **Update Connection String**
   
   Edit `appsettings.json` and update the connection string to match your SQL Server instance:
   ```json
   {
     "ConnectionStrings": {
       "MentalWellnessDB": "Server=YOUR_SERVER;Database=MentalWellnessDB;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```
   
   **For LocalDB:**
   ```json
   "MentalWellnessDB": "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=MentalWellnessDB;Integrated Security=True;TrustServerCertificate=True"
   ```

3. **Configure Email Settings** (Optional but Recommended)
   
   Edit `appsettings.json`:
   ```json
   {
     "EmailSettings": {
       "SmtpHost": "smtp.gmail.com",
       "SmtpPort": "587",
       "SmtpUser": "your-email@gmail.com",
       "SmtpPassword": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "FromName": "Mental Wellness System"
     }
   }
   ```
   
   **Note**: For Gmail, you'll need to use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password.

4. **Create Database and Run Migrations**
   
   Open a terminal in the project directory and run:
   ```bash
   dotnet ef database update
   ```
   
   Or using Package Manager Console in Visual Studio:
   ```powershell
   Update-Database
   ```
   
   This will create the database and apply all migrations automatically.

5. **Run the Application**
   ```bash
   dotnet run
   ```
   
   Or press **F5** in Visual Studio.
   
   The application will start and automatically:
   - Create database if it doesn't exist
   - Apply any pending migrations
   - Seed roles (Admin, Doctor, Patient)
   - Create default admin user

6. **Access the Application**
   
   Navigate to `https://localhost:5001` (or the port shown in the console output)
   
   The default port may vary. Check the console output for the actual URL.

### Default Admin Account

After the first run, a default admin account is automatically created:

- **Email**: `admin@mentalwellness.com`
- **Password**: `Admin@123`
- **Role**: Admin

⚠️ **IMPORTANT**: Change this password immediately after first login, especially in production environments!

### User Registration

#### Patient Registration
1. Navigate to the registration page (`/Identity/Account/Register`)
2. Select "Patient" as your role
3. Fill in all required information:
   - Full Name
   - Email Address
   - Password (must meet security requirements)
   - Phone Number
   - Age
   - Gender
   - Category/Therapy Type
4. Submit the registration form
5. Your **PatientID** is automatically generated in format: `MW-YYYYMMDD-XXXX-C`
6. Patients are automatically approved and can immediately access the system

#### Doctor Registration
1. Register with "Doctor" role
2. Fill in your specialty information
3. Account is created but marked as **pending approval**
4. An admin must approve your account before you can access doctor features
5. You will see a message indicating pending approval status
6. Once approved, you'll receive an email notification and can access all doctor features

#### Admin Registration
1. Register with "Admin" role
2. Admins are automatically approved
3. Can immediately access all administrative features

---

## 📁 Project Structure

```
MentalWellnessSystem/
│
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/              # Login, Register, Manage pages
│               ├── Login.cshtml
│               ├── Register.cshtml
│               └── ...
│
├── Data/
│   ├── ApplicationDbContext.cs      # Legacy Identity context (backward compatibility)
│   └── Migrations/                   # Identity migrations
│
├── Models/
│   ├── ApplicationUser.cs           # Custom Identity user with extended properties
│   ├── Appointment.cs               # Appointment entity
│   ├── PatientRecord.cs             # Medical records
│   ├── Notification.cs              # Notification queue
│   ├── AuditLog.cs                  # Audit trail
│   ├── MoodLog.cs                   # Daily mood tracking
│   ├── TreatmentPlan.cs             # Treatment plans with goals
│   ├── Feedback.cs                  # Patient feedback
│   ├── CommunityPost.cs             # Community forum posts
│   ├── CommunityComment.cs          # Post comments
│   ├── ResourceRecommendation.cs    # AI-generated recommendations
│   ├── TelehealthSession.cs         # Virtual consultation sessions
│   ├── PatientDocument.cs           # Patient document storage
│   └── MentalWellnessDbContext.cs   # Main DbContext (extends IdentityDbContext)
│
├── Pages/
│   ├── Patient/                     # Patient module
│   │   ├── Dashboard.cshtml
│   │   ├── BookAppointment.cshtml
│   │   ├── AppointmentHistory.cshtml
│   │   ├── ViewAppointment.cshtml
│   │   ├── Profile.cshtml
│   │   ├── MoodTracking.cshtml
│   │   ├── Recommendations.cshtml
│   │   └── Feedback.cshtml
│   │
│   ├── Doctor/                      # Doctor module
│   │   ├── Dashboard.cshtml
│   │   ├── ManageAppointment.cshtml
│   │   ├── PatientRecords.cshtml
│   │   ├── AddPatientRecord.cshtml
│   │   ├── TreatmentPlans.cshtml
│   │   ├── AddTreatmentPlan.cshtml
│   │   └── Profile.cshtml
│   │
│   ├── Admin/                       # Admin module
│   │   ├── Dashboard.cshtml
│   │   ├── ApproveDoctors.cshtml
│   │   ├── UserManagement.cshtml
│   │   ├── ViewUserProfile.cshtml
│   │   ├── Reports.cshtml
│   │   ├── Profile.cshtml
│   │   └── ChangePassword.cshtml
│   │
│   ├── Community/                   # Community module
│   │   ├── Index.cshtml
│   │   └── CreatePost.cshtml
│   │
│   ├── Shared/                      # Shared components
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   └── _ValidationScriptsPartial.cshtml
│   │
│   ├── Index.cshtml                 # Home page
│   ├── Contact.cshtml
│   ├── Privacy.cshtml
│   ├── Error.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
│
├── Services/                        # Business logic services
│   ├── PatientIDGeneratorService.cs      # Unique PatientID generation
│   ├── AppointmentService.cs             # Appointment booking logic
│   ├── NotificationService.cs            # Notification management
│   ├── NotificationBackgroundService.cs  # Background notification processing
│   ├── EmailService.cs                   # SMTP email service
│   ├── PdfService.cs                     # PDF generation
│   ├── AuditService.cs                   # Audit logging
│   ├── RecommendationService.cs          # AI-powered recommendations
│   ├── ReportingService.cs               # Analytics and reporting
│   └── Interfaces/                       # Service interfaces
│
├── Scripts/
│   └── ResetDatabase.sql            # Database reset script
│
├── wwwroot/                         # Static files
│   ├── css/
│   │   └── site.css
│   ├── js/
│   ├── lib/                         # Third-party libraries (Bootstrap, etc.)
│   └── favicon.ico
│
├── Migrations/                      # Database migrations
│   ├── 20251128003709_InitialCreate.cs
│   ├── 20251128013857_AdvancedFeatures.cs
│   └── ...
│
├── Properties/
│   ├── launchSettings.json          # Launch configuration
│   └── serviceDependencies.json     # Service dependencies
│
├── Program.cs                       # Application entry point and startup
├── appsettings.json                 # Application configuration
├── appsettings.Development.json     # Development-specific settings
└── MentalWellnessSystem.csproj      # Project file

```

---

## 🔑 Key Features in Detail

### PatientID Generation

The system automatically generates unique Patient IDs using the `PatientIDGeneratorService`:

**Format**: `MW-YYYYMMDD-XXXX-C`

- `MW`: Mental Wellness prefix
- `YYYYMMDD`: Registration date (8 digits)
- `XXXX`: 4-digit sequential number (unique per day)
- `C`: Check digit for validation

**Example**: `MW-20241128-0001-3`

**Features**:
- Prevents duplicate PatientIDs
- Immutable once assigned
- Includes validation check digit
- Automatically assigned upon patient registration

### Appointment Booking System

The appointment booking system includes:

**Double-Booking Prevention**:
- Uses database transactions with proper locking
- Checks availability before confirming booking
- Prevents race conditions in concurrent scenarios

**Time Slot Management**:
- Visual calendar interface
- Available time slots clearly displayed
- Automatic filtering of unavailable slots

**Transaction Safety**:
- All-or-nothing booking process
- Automatic rollback on errors
- Consistent database state

### Notification System

**Architecture**:
- Asynchronous background processing
- Database-queued notifications
- Retry mechanism for failed deliveries
- Status tracking (Pending, Sent, Failed)

**Notification Types**:
1. **Booking Confirmations**: Sent immediately upon booking
2. **Reminders**: 24 hours and 1 hour before appointment
3. **Cancellations**: Sent when appointments are cancelled
4. **Doctor Approval**: Notifies doctors when approved by admin

### Mood Tracking & Recommendations

**Mood Logging**:
- Daily mood score (1-10 scale)
- Stress level tracking
- Sleep hours monitoring
- Energy level assessment
- Notes field for additional context

**AI-Powered Recommendations**:
The system automatically generates recommendations based on:
- **Mood Patterns**: Low mood scores trigger support resources
- **Stress Levels**: High stress generates stress management resources
- **Sleep Patterns**: Poor sleep triggers sleep hygiene resources
- **Treatment Plans**: Recommendations aligned with active treatment goals

**Resource Types**:
- Articles (educational content)
- Exercises (therapeutic activities)
- Meditation guides
- Video content

### Treatment Planning

**Structured Plans**:
- Title and comprehensive description
- Multiple goals with specific objectives
- Task breakdown for each goal
- Start and end dates
- Status tracking (Active, Completed, Cancelled)
- Progress notes and updates

**Integration**:
- Links to specific appointments
- Associates with patient records
- Triggers automatic recommendations

### Community Forum

**Features**:
- **Categories**: General, Support, Questions, Success Stories
- **Anonymous Posting**: Option to post anonymously
- **Engagement**: Like and comment on posts
- **Pinned Posts**: Important announcements
- **Filtering**: Filter by category
- **Moderation**: Admin oversight capabilities

---

## 🗄️ Database Schema

### Core Tables

#### AspNetUsers (ApplicationUser)
Extended Identity user table with additional properties:
- `FullName`: User's full name
- `PatientID`: Unique patient identifier (format: MW-YYYYMMDD-XXXX-C)
- `Phone`: Phone number
- `IsApproved`: Approval status (for doctors)
- `Specialty`: Doctor specialty
- `Age`: Patient age
- `Gender`: Patient gender
- `Category`: Therapy category
- `CreatedAt`: Registration timestamp

#### AspNetRoles
Identity roles:
- Admin
- Doctor
- Patient

#### Appointments
- Appointment ID (Primary Key)
- Patient ID (Foreign Key → AspNetUsers)
- Doctor ID (Foreign Key → AspNetUsers)
- Appointment Date and Time
- Status (Scheduled, Ongoing, Completed, Cancelled)
- Notes
- Created and Updated timestamps

#### PatientRecords
- Record ID (Primary Key)
- Patient ID (Foreign Key)
- Doctor ID (Foreign Key)
- Session Notes
- Diagnosis
- Follow-up Request
- Record Date
- Created timestamp

#### MoodLogs
- Log ID (Primary Key)
- Patient ID (Foreign Key)
- Log Date
- Mood Score (1-10)
- Stress Level (1-10)
- Sleep Hours
- Energy Level (1-10)
- Notes
- Created timestamp

#### TreatmentPlans
- Plan ID (Primary Key)
- Patient ID (Foreign Key)
- Doctor ID (Foreign Key)
- Title
- Description
- Goals (JSON)
- Tasks (JSON)
- Start Date
- End Date
- Status
- Progress Notes
- Created and Updated timestamps

#### Notifications
- Notification ID (Primary Key)
- User ID (Foreign Key)
- Type (Email, SMS)
- Subject
- Message
- Status (Pending, Sent, Failed)
- Sent Date
- Created timestamp

#### AuditLogs
- Log ID (Primary Key)
- User ID (Foreign Key)
- Action (View, Create, Update, Delete)
- Entity Type
- Entity ID
- IP Address
- Timestamp

#### CommunityPosts
- Post ID (Primary Key)
- User ID (Foreign Key)
- Title
- Content
- Category
- IsAnonymous
- IsPinned
- Like Count
- Comment Count
- Created and Updated timestamps

#### Feedback
- Feedback ID (Primary Key)
- Patient ID (Foreign Key)
- Doctor ID (Foreign Key)
- Appointment ID (Foreign Key)
- Rating (1-5)
- Comments
- IsAnonymous
- Created timestamp

#### ResourceRecommendations
- Recommendation ID (Primary Key)
- Patient ID (Foreign Key)
- Title
- Description
- Resource Type
- Priority
- IsViewed
- Generated Date
- Created timestamp

### Relationships

- **One-to-Many**: User → Appointments (Patient)
- **One-to-Many**: User → Appointments (Doctor)
- **One-to-Many**: User → PatientRecords
- **One-to-Many**: User → MoodLogs
- **One-to-Many**: User → TreatmentPlans
- **One-to-Many**: User → Notifications
- **One-to-Many**: User → CommunityPosts
- **One-to-Many**: User → Feedback

---

## 🔒 Security

### Authentication & Authorization

- **ASP.NET Core Identity**: Industry-standard authentication framework
- **Password Hashing**: BCrypt-based password storage
- **Role-Based Access Control**: Granular permissions per role
- **Policy-Based Authorization**: Flexible authorization policies

### Password Requirements

- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character
- At least one unique character

### Account Protection

- **Lockout Policy**: 5 failed attempts lock account for 5 minutes
- **Email Verification**: Configurable email confirmation
- **Unique Email**: One account per email address

### Data Protection

- **HTTPS Enforcement**: All production traffic encrypted
- **SQL Injection Protection**: Parameterized queries via EF Core
- **XSS Protection**: Razor Pages built-in encoding
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Audit Logging**: Complete audit trail for sensitive operations

### Best Practices Implemented

1. ✅ Secure password storage (hashed, never plain text)
2. ✅ HTTPS redirect in production
3. ✅ Role-based page access
4. ✅ Transaction safety for critical operations
5. ✅ Input validation and sanitization
6. ✅ Audit logging for compliance
7. ✅ Secure connection strings (User Secrets, Azure Key Vault)

---

## 🚀 Deployment

### Azure App Service Deployment

#### Prerequisites
- Azure account with active subscription
- Azure SQL Database (or use existing SQL Server)
- Azure App Service plan

#### Steps

1. **Create Azure Resources**
   - Create an Azure App Service
   - Create an Azure SQL Database
   - Configure firewall rules

2. **Configure Connection String**
   
   In Azure Portal → App Service → Configuration:
   ```
   Name: ConnectionStrings__MentalWellnessDB
   Value: Server=tcp:your-server.database.windows.net,1433;Initial Catalog=MentalWellnessDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

3. **Configure Email Settings**
   
   Add as Application Settings:
   ```
   EmailSettings__SmtpHost = smtp.gmail.com
   EmailSettings__SmtpPort = 587
   EmailSettings__SmtpUser = your-email@gmail.com
   EmailSettings__SmtpPassword = your-app-password
   EmailSettings__FromEmail = your-email@gmail.com
   EmailSettings__FromName = Mental Wellness System
   ```

   **Recommended**: Use Azure Key Vault for sensitive values like SMTP password.

4. **Deploy Application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```
   
   Then deploy the `publish` folder to Azure App Service using:
   - Visual Studio Publish wizard
   - Azure CLI
   - GitHub Actions CI/CD
   - FTP

5. **Run Migrations**
   
   After deployment, run migrations:
   ```bash
   dotnet ef database update
   ```
   
   Or use Azure App Service Console/Kudu.

### CI/CD with GitHub Actions

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Build
      run: dotnet build --configuration Release
    - name: Publish
      run: dotnet publish --configuration Release --output ./publish
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'your-app-name'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

### Environment-Specific Configuration

Use `appsettings.Production.json` for production-specific settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "your-domain.com"
}
```

---

## ⚙️ Configuration

### Connection String Formats

**SQL Server (Local)**:
```
Server=localhost;Database=MentalWellnessDB;Trusted_Connection=True;TrustServerCertificate=True
```

**SQL Server (Remote)**:
```
Server=YOUR_SERVER;Database=MentalWellnessDB;User Id=username;Password=password;TrustServerCertificate=True
```

**Azure SQL Database**:
```
Server=tcp:your-server.database.windows.net,1433;Initial Catalog=MentalWellnessDB;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**LocalDB**:
```
Data Source=(localdb)\mssqllocaldb;Initial Catalog=MentalWellnessDB;Integrated Security=True;TrustServerCertificate=True
```

### Email Configuration

**Gmail SMTP**:
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Mental Wellness System"
  }
}
```

**Outlook/Hotmail SMTP**:
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": "587",
    "SmtpUser": "your-email@outlook.com",
    "SmtpPassword": "your-password",
    "FromEmail": "your-email@outlook.com",
    "FromName": "Mental Wellness System"
  }
}
```

### User Secrets (Development)

For sensitive data in development, use User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MentalWellnessDB" "your-connection-string"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-password"
```

---

## 🔧 Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to database

**Solutions**:
1. Verify SQL Server is running:
   - Windows: Check Services (services.msc)
   - Check SQL Server Configuration Manager
2. Verify connection string format in `appsettings.json`
3. Check SQL Server authentication mode (Windows Auth vs SQL Auth)
4. Ensure `TrustServerCertificate=True` for local development
5. Check firewall settings if using remote SQL Server
6. Verify database exists: `SELECT name FROM sys.databases`

### Email Not Sending

**Problem**: Email notifications not being sent

**Solutions**:
1. Verify SMTP settings in `appsettings.json`
2. Check SMTP credentials (username and password)
3. For Gmail, ensure you're using an App Password, not regular password
4. Check firewall/network settings (port 587 may be blocked)
5. Review application logs for detailed error messages
6. Test SMTP connection using a simple email client
7. Check spam folder (emails might be marked as spam)

### PatientID Generation Errors

**Problem**: PatientID generation fails

**Solutions**:
1. Ensure database is accessible and migrations are applied
2. Check for date/time issues on server
3. Verify `PatientIDGeneratorService` has database access
4. Review error logs for specific exception details
5. Ensure no manual PatientID conflicts in database

### Role Authorization Issues

**Problem**: Users cannot access role-specific pages

**Solutions**:
1. Verify roles are seeded (check database `AspNetRoles` table)
2. Ensure user is assigned to correct role (check `AspNetUserRoles`)
3. Verify `[Authorize(Roles = "...")]` attributes on pages
4. Check cookie authentication settings in `Program.cs`
5. Clear browser cookies and re-login
6. Verify user is logged in (check authentication state)

### Migration Issues

**Problem**: Database migrations fail

**Solutions**:
1. Ensure connection string is correct
2. Check for pending migrations: `dotnet ef migrations list`
3. Review migration files for errors
4. Manually inspect database schema
5. Consider resetting database (backup first!):
   ```bash
   dotnet ef database drop
   dotnet ef database update
   ```

### PDF Generation Errors

**Problem**: PDF downloads fail

**Solutions**:
1. Verify `PdfService` is properly configured
2. Check file system permissions
3. Ensure sufficient disk space
4. Review exception logs for specific errors
5. Verify user has authorization to download PDF

### Background Service Not Running

**Problem**: Notifications not being processed

**Solutions**:
1. Check if `NotificationBackgroundService` is registered in `Program.cs`
2. Review application logs for service errors
3. Verify database connectivity for background service
4. Check if notifications are being queued in database
5. Restart application to restart background service

---

## 👥 Team Members

This project was developed by the following team members:

| Name | Student ID | Role |
|------|-----------|------|
| **UWASE KAMIKAZI SHANIA** | 26677 | Team Member |
| **Mucyo Mutabazi Fabrice** | 26635 | Team Member |
| **Akaliza M. Racia** | 26628 | Team Member |
| **Nshuti Kevin** | 26663 | Team Member |
| **Arnaud NSHUTI** | 25899 | Team Member |

---

## 📚 Documentation

Additional documentation files available in the project:

- **PROJECT_SUMMARY.md** - Complete feature list and project status
- **QUICKSTART.md** - Quick setup guide
- **DEPLOYMENT.md** - Detailed deployment instructions
- **ADVANCED_FEATURES_IMPLEMENTED.md** - Advanced features documentation
- **DATABASE_SETUP_COMPLETE.md** - Database setup guide
- **MIGRATION_GUIDE.md** - Database migration guide
- **FIXES_APPLIED.md** - Bug fixes and improvements

---

## 📝 License

This project is created for **educational purposes** as a class final project. 

All rights reserved. This software is provided "as is" without warranty of any kind.

---

## 🙏 Acknowledgments

- **ASP.NET Core Team** - For the excellent web framework
- **Entity Framework Core Team** - For the powerful ORM
- **Bootstrap Team** - For the responsive UI framework
- **Microsoft** - For comprehensive documentation and tools
- **Open Source Community** - For inspiration and support

---

## 🔮 Future Enhancements

Potential features for future versions:

- [ ] SMS notifications (Twilio integration)
- [ ] Calendar integration (Google Calendar, Outlook)
- [ ] Video consultation support (WebRTC)
- [ ] Mobile app (Xamarin/MAUI)
- [ ] Multi-language support (i18n)
- [ ] Payment integration (Stripe, PayPal)
- [ ] Advanced analytics dashboard with charts
- [ ] Doctor availability/schedule management
- [ ] Automated appointment reminders via SMS
- [ ] Integration with wearable devices for mood tracking
- [ ] AI chatbot for initial patient screening
- [ ] Telemedicine features with video calls
- [ ] Electronic health records (EHR) integration
- [ ] Prescription management system

---

## 📞 Support

For issues, questions, or contributions:

1. Check the **Troubleshooting** section above
2. Review application logs for detailed error messages
3. Check database connectivity and configuration
4. Verify all configuration settings in `appsettings.json`
5. Review the documentation files in the project root

---

<div align="center">

**Built with ❤️ for Mental Wellness**
**Thanks to all Team Members**

*Empowering mental health care through technology*

---

*Last Updated: January 2025*

</div>

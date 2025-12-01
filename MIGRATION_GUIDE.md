# Database Migration Guide - Mental Wellness System

## Overview
This guide will help you create all database tables for the Mental Wellness System using Entity Framework Core migrations.

## Current Architecture
The system uses **ApplicationUser** (extends IdentityUser) instead of separate Patient/Doctor tables. This is the modern, recommended approach.

**Tables that will be created:**
1. **AspNetUsers** - All users (Patients, Doctors, Admins) with ApplicationUser properties
2. **AspNetRoles** - Roles (Patient, Doctor, Admin)
3. **AspNetUserRoles** - User-Role mappings
4. **AspNetUserClaims** - User claims
5. **AspNetUserLogins** - External login providers
6. **AspNetUserTokens** - User tokens
7. **Appointments** - Appointment records
8. **PatientRecords** - Medical records
9. **Notifications** - Notification queue
10. **AuditLogs** - Audit trail

## Step-by-Step Migration Process

### Step 1: Verify DbContext Setup

The `MentalWellnessDbContext` is already correctly configured:
- ✅ Extends `IdentityDbContext<ApplicationUser>`
- ✅ Has all DbSets: Appointments, PatientRecords, Notifications, AuditLogs
- ✅ Proper relationships configured
- ✅ Indexes defined

### Step 2: Install EF Core Tools (if not installed)

```bash
dotnet tool install --global dotnet-ef
```

### Step 3: Remove Old Migrations (if any)

If you have old migrations that don't match the current model:

```bash
# Delete old migrations folder
# Or remove specific migration files
```

### Step 4: Create Initial Migration

```bash
cd MentalWellnessSystem
dotnet ef migrations add InitialCreate --context MentalWellnessDbContext
```

This will create:
- `Migrations/YYYYMMDDHHMMSS_InitialCreate.cs`
- `Migrations/YYYYMMDDHHMMSS_InitialCreate.Designer.cs`
- Update `Migrations/MentalWellnessDbContextModelSnapshot.cs`

### Step 5: Review Migration

Check the generated migration file to ensure:
- ✅ All Identity tables are included
- ✅ Appointments table with proper FKs
- ✅ PatientRecords table with proper FKs
- ✅ Notifications table with proper FKs
- ✅ AuditLogs table with proper FKs
- ✅ All indexes are created

### Step 6: Apply Migration to Database

```bash
dotnet ef database update --context MentalWellnessDbContext
```

This will:
- Create the database if it doesn't exist
- Create all tables
- Create all indexes
- Set up all foreign key relationships

### Step 7: Verify Tables in Database

Connect to your SQL Server database and verify:

**Identity Tables:**
```sql
SELECT * FROM AspNetUsers;
SELECT * FROM AspNetRoles;
SELECT * FROM AspNetUserRoles;
```

**Domain Tables:**
```sql
SELECT * FROM Appointments;
SELECT * FROM PatientRecords;
SELECT * FROM Notifications;
SELECT * FROM AuditLogs;
```

### Step 8: Seed Data (Automatic)

The application automatically seeds:
- ✅ Roles (Admin, Doctor, Patient)
- ✅ Default admin user (admin@mentalwellness.com / Admin@123)

This happens on first run in `Program.cs`.

## Troubleshooting

### Issue: Migration conflicts
**Solution:** Remove old migrations and create fresh:
```bash
dotnet ef migrations remove --context MentalWellnessDbContext
dotnet ef migrations add InitialCreate --context MentalWellnessDbContext
```

### Issue: Database already exists
**Solution:** Either drop and recreate, or use:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

### Issue: Connection string error
**Solution:** Verify `appsettings.json` has correct connection string:
```json
{
  "ConnectionStrings": {
    "MentalWellnessDB": "Data Source=.;Initial Catalog=MentalWellnessDB;Integrated Security=True;Trust Server Certificate=True"
  }
}
```

## Quick Commands Reference

```bash
# Create migration
dotnet ef migrations add InitialCreate --context MentalWellnessDbContext

# Apply migration
dotnet ef database update --context MentalWellnessDbContext

# Remove last migration
dotnet ef migrations remove --context MentalWellnessDbContext

# List migrations
dotnet ef migrations list --context MentalWellnessDbContext

# Generate SQL script (without applying)
dotnet ef migrations script --context MentalWellnessDbContext
```

## Database Schema Summary

### AspNetUsers (ApplicationUser)
- Id (PK, string)
- UserName, Email, PhoneNumber (from IdentityUser)
- FullName, PatientID, Phone, IsApproved, Specialty, Age, Gender, Category, CreatedAt (custom)

### Appointments
- AppointmentId (PK, int)
- PatientId (FK to AspNetUsers.Id)
- DoctorId (FK to AspNetUsers.Id)
- AppointmentDate, AppointmentTime, AppointmentType, Status, Notes
- CreatedAt, UpdatedAt

### PatientRecords
- RecordId (PK, int)
- PatientId (FK to AspNetUsers.Id)
- DoctorId (FK to AspNetUsers.Id)
- AppointmentId (FK to Appointments, nullable)
- Notes, Diagnosis, FollowUpRequest
- CreatedAt, UpdatedAt

### Notifications
- NotificationId (PK, int)
- UserId (FK to AspNetUsers.Id)
- AppointmentId (FK to Appointments, nullable)
- NotificationType, Subject, Message, DeliveryMethod, Status
- CreatedAt, SentAt, ErrorMessage

### AuditLogs
- AuditLogId (PK, int)
- UserId (FK to AspNetUsers.Id)
- Action, EntityType, EntityId, Description, IpAddress
- Timestamp

## Next Steps

After migration:
1. ✅ Run the application
2. ✅ Login as admin (admin@mentalwellness.com / Admin@123)
3. ✅ Create test users (Patient, Doctor)
4. ✅ Test appointment booking
5. ✅ Verify all features work


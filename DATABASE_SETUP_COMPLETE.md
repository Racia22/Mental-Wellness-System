# ✅ Database Migration Setup Complete

## Migration Created Successfully

**Migration File:** `Migrations/20251128003709_InitialCreate.cs`

This migration will create **ALL** the following tables:

### Identity Tables (ASP.NET Core Identity)
1. ✅ **AspNetUsers** - All users (Patients, Doctors, Admins) with ApplicationUser properties
2. ✅ **AspNetRoles** - Roles (Admin, Doctor, Patient)
3. ✅ **AspNetUserRoles** - User-Role mappings
4. ✅ **AspNetUserClaims** - User claims
5. ✅ **AspNetUserLogins** - External login providers
6. ✅ **AspNetUserTokens** - User tokens
7. ✅ **AspNetRoleClaims** - Role claims

### Domain Tables (Your Models)
8. ✅ **Appointments** - Appointment records with FKs to AspNetUsers
9. ✅ **PatientRecords** - Medical records with FKs to AspNetUsers and Appointments
10. ✅ **Notifications** - Notification queue with FKs to AspNetUsers and Appointments
11. ✅ **AuditLogs** - Audit trail with FK to AspNetUsers

### System Tables
12. ✅ **__EFMigrationsHistory** - Migration tracking

## Next Steps

### If Database Already Exists (Current Situation)

**Option A: Drop and Recreate (Recommended for Development)**
```sql
-- Run Scripts/ResetDatabase.sql in SQL Server Management Studio
-- OR manually:
USE master;
DROP DATABASE IF EXISTS MentalWellnessDB;
CREATE DATABASE MentalWellnessDB;
```

Then:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

**Option B: Remove Conflicting Tables**
```sql
USE MentalWellnessDB;
DROP TABLE IF EXISTS Appointments;
DROP TABLE IF EXISTS PatientRecords;
DROP TABLE IF EXISTS Notifications;
DROP TABLE IF EXISTS AuditLogs;
-- Keep AspNetUsers and other Identity tables if they exist
```

Then:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

### If Database Doesn't Exist

Simply run:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

## Verification

After applying migration, verify tables:

```sql
USE MentalWellnessDB;
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**Expected Output:**
- AspNetRoleClaims
- AspNetRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens
- AspNetUsers
- Appointments
- AuditLogs
- Notifications
- PatientRecords
- __EFMigrationsHistory

## Automatic Seeding

When you run the application, it will automatically:
- ✅ Create roles: Admin, Doctor, Patient
- ✅ Create admin user: admin@mentalwellness.com / Admin@123

## Summary

✅ **Migration Created:** `20251128003709_InitialCreate`
✅ **All Models Included:** ApplicationUser, Appointment, PatientRecord, Notification, AuditLog
✅ **All Relationships Configured:** Foreign keys, indexes, constraints
✅ **Identity Tables Included:** All ASP.NET Core Identity tables
✅ **Ready to Apply:** Run `dotnet ef database update`

## Important Notes

1. **ApplicationUser replaces separate Patient/Doctor tables** - This is the modern, recommended approach
2. **All users are in AspNetUsers** - Patients, Doctors, and Admins are distinguished by roles
3. **PatientID is stored in ApplicationUser** - Only patients have this field populated
4. **IsApproved is for Doctors** - Doctors require admin approval before accessing the system

## Troubleshooting

**Error: "There is already an object named 'X' in the database"**
- Solution: Drop the existing database or remove conflicting tables (see Option A or B above)

**Error: "Cannot connect to database"**
- Solution: Check connection string in `appsettings.json`
- Verify SQL Server is running
- Check database name and permissions

**Migration not applying**
- Solution: Check `__EFMigrationsHistory` table to see what's already applied
- Remove old migrations if needed: `dotnet ef migrations remove`


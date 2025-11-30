# Database Setup Instructions

## Current Situation
The database already has some tables. You have two options:

## Option 1: Fresh Start (Recommended for Development)

**Drop existing database and create fresh:**

```bash
# Connect to SQL Server and drop the database
# Or use SQL Server Management Studio to delete MentalWellnessDB

# Then run:
dotnet ef database update --context MentalWellnessDbContext
```

**Or use SQL Script:**
```sql
USE master;
GO
DROP DATABASE IF EXISTS MentalWellnessDB;
GO
CREATE DATABASE MentalWellnessDB;
GO
```

Then run:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

## Option 2: Keep Existing Data (Production)

**Check what tables exist:**
```sql
USE MentalWellnessDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**If tables exist, you may need to:**
1. Remove the migration and recreate it
2. Or manually drop conflicting tables

## Recommended: Complete Fresh Setup

### Step 1: Drop Existing Database
```sql
USE master;
GO
DROP DATABASE IF EXISTS MentalWellnessDB;
GO
```

### Step 2: Remove Old Migrations (Optional)
```bash
# Delete the Migrations folder contents (except keep the folder)
# Or remove specific migration files
```

### Step 3: Create Fresh Migration
```bash
dotnet ef migrations add InitialCreate --context MentalWellnessDbContext
```

### Step 4: Apply Migration
```bash
dotnet ef database update --context MentalWellnessDbContext
```

### Step 5: Verify Tables Created

Run this SQL to verify all tables:
```sql
USE MentalWellnessDB;
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**Expected Tables:**
- AspNetUsers
- AspNetRoles
- AspNetRoleClaims
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens
- Appointments
- PatientRecords
- Notifications
- AuditLogs
- __EFMigrationsHistory

### Step 6: Run Application

The application will automatically:
- ✅ Seed roles (Admin, Doctor, Patient)
- ✅ Create default admin user (admin@mentalwellness.com / Admin@123)

## Quick Setup Script

**PowerShell Script:**
```powershell
# Stop application if running
# Drop database
sqlcmd -S . -Q "DROP DATABASE IF EXISTS MentalWellnessDB; CREATE DATABASE MentalWellnessDB;"

# Apply migration
cd MentalWellnessSystem
dotnet ef database update --context MentalWellnessDbContext

# Run application
dotnet run
```

## Verification Checklist

After setup, verify:

- [ ] Database MentalWellnessDB exists
- [ ] All 12+ tables created
- [ ] Can login as admin@mentalwellness.com
- [ ] Roles exist (Admin, Doctor, Patient)
- [ ] Can create new users
- [ ] Can book appointments
- [ ] All features work


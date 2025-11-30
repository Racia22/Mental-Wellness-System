# Quick Database Setup Guide

## ✅ Migration Already Created!

**Migration File:** `Migrations/20251128003709_InitialCreate.cs`

This single migration creates **ALL 12 tables** needed for your Mental Wellness System.

## 🚀 Quick Setup (Choose One)

### Option 1: Fresh Database (Recommended)

**Step 1:** Drop existing database
```sql
USE master;
DROP DATABASE IF EXISTS MentalWellnessDB;
CREATE DATABASE MentalWellnessDB;
```

**Step 2:** Apply migration
```bash
dotnet ef database update --context MentalWellnessDbContext
```

**Done!** ✅

### Option 2: Keep Existing Identity Tables

**Step 1:** Drop only domain tables
```sql
USE MentalWellnessDB;
DROP TABLE IF EXISTS Appointments;
DROP TABLE IF EXISTS PatientRecords;
DROP TABLE IF EXISTS Notifications;
DROP TABLE IF EXISTS AuditLogs;
```

**Step 2:** Apply migration
```bash
dotnet ef database update --context MentalWellnessDbContext
```

## 📊 Tables Created by Migration

### Identity Tables (7 tables)
- ✅ AspNetUsers (with ApplicationUser properties)
- ✅ AspNetRoles
- ✅ AspNetUserRoles
- ✅ AspNetUserClaims
- ✅ AspNetUserLogins
- ✅ AspNetUserTokens
- ✅ AspNetRoleClaims

### Domain Tables (4 tables)
- ✅ Appointments
- ✅ PatientRecords
- ✅ Notifications
- ✅ AuditLogs

### System Tables (1 table)
- ✅ __EFMigrationsHistory

**Total: 12 tables** ✅

## 🔍 Verify Setup

After applying migration, check:
```sql
USE MentalWellnessDB;
SELECT COUNT(*) as TableCount 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
-- Should return 12
```

## 🎯 What Happens Next

When you run the application:
1. ✅ Migration applies automatically (if pending)
2. ✅ Roles are seeded (Admin, Doctor, Patient)
3. ✅ Admin user created (admin@mentalwellness.com / Admin@123)

## ⚠️ Current Issue

The database already has some tables. To fix:

**Quick Fix:**
```sql
USE master;
DROP DATABASE MentalWellnessDB;
CREATE DATABASE MentalWellnessDB;
```

Then:
```bash
dotnet ef database update --context MentalWellnessDbContext
```

## ✅ Summary

- ✅ Migration created: `20251128003709_InitialCreate`
- ✅ All models included
- ✅ All relationships configured
- ✅ All indexes created
- ✅ Ready to apply

**Next:** Drop database and apply migration, or remove conflicting tables.


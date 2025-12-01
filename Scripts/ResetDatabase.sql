-- =============================================
-- Mental Wellness System - Database Reset Script
-- =============================================
-- This script drops and recreates the database
-- WARNING: This will delete all data!
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MentalWellnessDB')
BEGIN
    ALTER DATABASE MentalWellnessDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MentalWellnessDB;
    PRINT 'Database MentalWellnessDB dropped.';
END
GO

-- Create new database
CREATE DATABASE MentalWellnessDB;
GO

PRINT 'Database MentalWellnessDB created successfully.';
PRINT 'Now run: dotnet ef database update --context MentalWellnessDbContext';
GO


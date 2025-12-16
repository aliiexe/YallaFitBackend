-- =========================================================================
-- YallaFit Password Migration Script
-- =========================================================================
-- Purpose: Reset admin password to hashed version for users upgrading from
--          old version with unhashed passwords
-- 
-- IMPORTANT: Only run this if you're upgrading from an old version where
--            passwords were stored unhashed and you can't login anymore
-- =========================================================================

USE YallaFit;

-- Delete the old admin user with unhashed password
DELETE FROM Utilisateur WHERE email = 'admin@yallafit.com';

-- The new admin user will be automatically created with hashed password
-- when you restart the application

-- New credentials after restart:
-- Email: admin@yallafit.com
-- Password: Admin123!

SELECT 'Admin user reset complete. Restart the application to create new admin with hashed password.' AS Message;

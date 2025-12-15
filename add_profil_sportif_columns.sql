-- Add missing columns to Profil_Sportif table
ALTER TABLE Profil_Sportif 
ADD COLUMN age INT NULL AFTER user_id;

ALTER TABLE Profil_Sportif 
ADD COLUMN date_naissance DATETIME NULL AFTER age;

ALTER TABLE Profil_Sportif 
ADD COLUMN problemes_sante TEXT NULL AFTER preferences_alim;

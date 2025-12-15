-- Add the missing categorie column to the Exercice table

ALTER TABLE `Exercice` 
ADD COLUMN `categorie` VARCHAR(100) NULL AFTER `muscle_cible`;

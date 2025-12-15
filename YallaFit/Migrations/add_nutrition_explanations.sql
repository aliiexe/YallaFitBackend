-- Migration: Add food photo analysis table

CREATE TABLE IF NOT EXISTS Analyse_Repas_Photo (
    id INT PRIMARY KEY AUTO_INCREMENT,
    sportif_id INT NOT NULL,
    date_analyse DATETIME NOT NULL,
    chemin_photo VARCHAR(500) NOT NULL,
    aliments_detectes TEXT,
    calories_estimees INT NOT NULL DEFAULT 0,
    proteines_estimees FLOAT NOT NULL DEFAULT 0,
    glucides_estimees FLOAT NOT NULL DEFAULT 0,
    lipides_estimees FLOAT NOT NULL DEFAULT 0,
    analyse_ia TEXT,
    recommandations TEXT,
    FOREIGN KEY (sportif_id) REFERENCES Utilisateur(id) ON DELETE CASCADE,
    INDEX idx_sportif_date (sportif_id, date_analyse)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Add columns to existing tables if needed
ALTER TABLE Plan_Nutrition 
ADD COLUMN IF NOT EXISTS analyse_globale TEXT NULL,
ADD COLUMN IF NOT EXISTS conseils_personnalises TEXT NULL;

ALTER TABLE Repas
ADD COLUMN IF NOT EXISTS explication TEXT NULL,
ADD COLUMN IF NOT EXISTS benefices_nutritionnels TEXT NULL;

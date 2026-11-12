SET NOCOUNT ON;

PRINT '1. Version du serveur';
SELECT @@VERSION AS version;

PRINT '2. Support du type VECTOR';
DECLARE @a VECTOR(3) = CAST('[1, 0, 0]' AS VECTOR(3));
DECLARE @b VECTOR(3) = CAST('[0.9, 0.1, 0]' AS VECTOR(3));
SELECT VECTOR_DISTANCE('cosine', @a, @b) AS distance_cosinus;

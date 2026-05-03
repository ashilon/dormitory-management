-- ============================================================
-- Dormitory Management System — Stored Procedures
-- ============================================================

-- ------------------------------------------------------------
-- GetEducationPlaceSummaries
--   Returns every education place with:
--     • ActiveStudentCount — students with IsActive = 1
--     • AverageAge         — average age of active students
--   Demonstrates: LEFT JOIN, GROUP BY, aggregate functions
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.GetEducationPlaceSummaries
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ep.Id,
        ep.Name,
        ep.City,
        COUNT(s.Id)                                    AS ActiveStudentCount,
        ISNULL(AVG(CAST(s.Age AS DECIMAL(5, 2))), 0)  AS AverageAge
    FROM dbo.EducationPlace AS ep
    LEFT JOIN dbo.Student   AS s
        ON  s.EducationPlaceId = ep.Id
        AND s.IsActive         = 1
    GROUP BY
        ep.Id,
        ep.Name,
        ep.City
    ORDER BY
        ep.Name;
END;
GO

-- ------------------------------------------------------------
-- UpsertStudent
--   INSERT when @Id is NULL or 0.
--   UPDATE when @Id refers to an existing record.
--   Returns the Id of the inserted/updated row.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.UpsertStudent
    @Id               INT           = NULL,
    @Name             NVARCHAR(200),
    @IdNumber         NVARCHAR(20),
    @Age              INT,
    @EducationPlaceId INT,
    @IsActive         BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO dbo.Student (Name, IdNumber, Age, EducationPlaceId, IsActive)
        VALUES (@Name, @IdNumber, @Age, @EducationPlaceId, @IsActive);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE dbo.Student
        SET
            Name             = @Name,
            IdNumber         = @IdNumber,
            Age              = @Age,
            EducationPlaceId = @EducationPlaceId,
            IsActive         = @IsActive
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END;
GO

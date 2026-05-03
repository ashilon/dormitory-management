-- ============================================================
-- Dormitory Management System — Table Creation Script
-- ============================================================

-- EducationPlace (Dormitory)
CREATE TABLE dbo.EducationPlace (
    Id   INT           IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_EducationPlace      PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_EducationPlace_Name UNIQUE (Name)
);

CREATE INDEX IX_EducationPlace_City ON dbo.EducationPlace (City);

-- Student
CREATE TABLE dbo.Student (
    Id               INT          IDENTITY(1,1) NOT NULL,
    Name             NVARCHAR(200) NOT NULL,
    IdNumber         NVARCHAR(20)  NOT NULL,
    Age              INT           NOT NULL,
    EducationPlaceId INT           NOT NULL,
    IsActive         BIT           NOT NULL CONSTRAINT DF_Student_IsActive DEFAULT (1),
    CONSTRAINT PK_Student                PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Student_IdNumber       UNIQUE (IdNumber),
    CONSTRAINT FK_Student_EducationPlace FOREIGN KEY (EducationPlaceId)
        REFERENCES dbo.EducationPlace (Id),
    CONSTRAINT CHK_Student_Age CHECK (Age BETWEEN 10 AND 120)
);

-- Indexes
CREATE INDEX IX_Student_EducationPlaceId ON dbo.Student (EducationPlaceId);

-- Covering index for the summary stored procedure (avoids key lookups)
CREATE INDEX IX_Student_IsActive ON dbo.Student (IsActive)
    INCLUDE (EducationPlaceId, Age);

CREATE INDEX IX_Student_IdNumber ON dbo.Student (IdNumber);

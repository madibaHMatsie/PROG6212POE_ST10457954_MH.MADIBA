CREATE DATABASE RaceDay;

--users
CREATE TABLE Users ( UserId NVARCHAR(10) PRIMARY KEY,   
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    DateRegistered DATETIME DEFAULT GETDATE(),
    ProfileImageUrl NVARCHAR(500) NULL

);

--Events
CREATE TABLE Events (
    EventId NVARCHAR(10) PRIMARY KEY,         -- e.g., 'EVT001'
    OrganiserId NVARCHAR(10) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Date DATETIME NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    RouteInfo NVARCHAR(MAX) NULL,
    MaxParticipants INT NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Open', 'Closed', 'Cancelled')) DEFAULT 'Open',
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);

--Categories
CREATE TABLE Categories (
    CategoryId NVARCHAR(10) PRIMARY KEY,      -- e.g., 'CAT001'
    EventId NVARCHAR(10) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    EntryFee DECIMAL(10, 2) NOT NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);

--Enrolment
CREATE TABLE Enrolments (
    EnrolmentId NVARCHAR(10) PRIMARY KEY,     -- e.g., 'ENR001'
    ParticipantId NVARCHAR(10) NOT NULL,
    EventId NVARCHAR(10) NOT NULL,
    CategoryId NVARCHAR(10) NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Confirmed', 'Pending', 'Cancelled')) DEFAULT 'Confirmed',
    BibNumber INT NULL,
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventId) REFERENCES Events(EventId),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT UQ_Enrolments_EventParticipant UNIQUE (EventId, ParticipantId)
);

--Results
CREATE TABLE Results (
    ResultId NVARCHAR(10) PRIMARY KEY,        
    EnrolmentId NVARCHAR(10) NOT NULL UNIQUE,
    ChipTime TIME NULL,
    GunTime TIME NULL,
    OverallPosition INT NULL,
    CategoryPosition INT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'Official', 'Disqualified')) DEFAULT 'Pending',
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId)
);

--WeatherInfo 
CREATE TABLE WeatherInfo (
    WeatherId NVARCHAR(10) PRIMARY KEY,       
    EventId NVARCHAR(10) NOT NULL,
    ForecastDate DATETIME NOT NULL,
    Temperature FLOAT NULL,
    Condition NVARCHAR(100) NULL,
    WindSpeed FLOAT NULL,
    Humidity FLOAT NULL,
    CONSTRAINT FK_Weather_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);

--AuditLogs 
CREATE TABLE AuditLogs (
    LogId NVARCHAR(10) PRIMARY KEY,           
    UserId NVARCHAR(10) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    Details NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Audit_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);





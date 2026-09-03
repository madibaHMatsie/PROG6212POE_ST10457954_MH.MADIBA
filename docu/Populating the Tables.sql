--  Users
INSERT INTO Users (UserId, Email, PasswordHash, FirstName, LastName, Role) VALUES
('USR001', 'kganyaThato@raceday.co.za', 'hashed_password_1', 'Thato', 'Kganya', 'Organiser'),
('USR002', 'joel.kane@raceday.co.za', 'hashed_password_2', 'Joel', 'Kane', 'Organiser'),
('USR003', 'Rama.mphela@email.com', 'hashed_password_3', 'Mphela', 'Ramashidza', 'Participant'),
('USR004', 'MojelaKgadishi@email.com', 'hashed_password_4', 'Kgadishi', 'Mojela', 'Participant');

--  Events
INSERT INTO Events (EventId, OrganiserId, Name, Description, Date, Location, RouteInfo, MaxParticipants, Status) VALUES
('EVT001', 'USR001', 'ABSA RUN YOUR CITY TSHWANE 10K', 'A famous marathon through the streets of Tshwane.', '2026-11-01 06:00:00', 'Arcadia, Pretoria', 'Route starts at Stanza Bopape Street and winds through Arcadia.', 15000, 'Open'),
('EVT002', 'USR002', 'Soweto Marathon', 'A famous marathon through the streets of Soweto.', '2026-03-08 07:00:00','Soweto, Johannesburg', 'Route starts at Orlando Stadium and winds through Soweto.', 35000, 'Closed'),
('EVT003', 'USR001', 'Two Oceans Ultra Marathon', 'A 56km ultra marathon with stunning coastal views.', '2026-04-04 05:30:00', 'Cape Town', 'Route from Newlands to Cape Point and back.', 12000, 'Open');

--  Categories
INSERT INTO Categories (CategoryId, EventId, Name, Description, EntryFee) VALUES
-- Arcadia Marathon
('CAT001', 'EVT001', '42.2km Marathon', 'Full marathon distance.', 250.00),
('CAT002', 'EVT001', '21.1km Half Marathon', 'Half marathon distance.', 150.00),
('CAT003', 'EVT001', '5km Fun Run', 'For charity and family fun.', 50.00),
-- Soweto Marathon
('CAT004', 'EVT002', '109km Road Race', 'The main cycle race.', 400.00),
('CAT005', 'EVT002', '72km Challenge', 'A shorter but still demanding route.', 300.00),
-- Two Oceans Ultra Marathon
('CAT006', 'EVT003', '56km Ultra', 'The famous ultra marathon.', 500.00),
('CAT007', 'EVT003', '21.1km Half Marathon', 'A scenic half marathon.', 250.00);

--  Enrolments
INSERT INTO Enrolments (EnrolmentId, ParticipantId, EventId, CategoryId, Status, BibNumber) VALUES
('ENR001', 'USR003', 'EVT001', 'CAT002', 'Confirmed', 1025),
('ENR002', 'USR004', 'EVT001', 'CAT001', 'Confirmed', 1026),
('ENR003', 'USR003', 'EVT002', 'CAT005', 'Pending', NULL);

--  Results
INSERT INTO Results (ResultId, EnrolmentId, ChipTime, GunTime, OverallPosition, CategoryPosition, Status) VALUES
('RES001', 'ENR001', '01:52:30', '01:53:15', 150, 25, 'Official'),
('RES002', 'ENR002', '03:45:00', '03:45:45', 500, 45, 'Official');

-- WeatherInfo
INSERT INTO WeatherInfo (WeatherId, EventId, ForecastDate, Temperature, Condition, WindSpeed, Humidity) VALUES
('WEA001', 'EVT001', '2026-11-01 06:00:00', 18.5, 'Sunny', 10.0, 65.0),
('WEA002', 'EVT002', '2026-03-08 07:00:00', 22.0, 'Partly Cloudy', 15.0, 70.0),
('WEA003', 'EVT003', '2026-04-04 05:30:00', 15.0, 'Rainy', 25.0, 80.0);

-- AuditLogs
INSERT INTO AuditLogs (LogId, UserId, Action, Details) VALUES
('LOG001', 'USR001', 'Create Event', 'Created event: ABSA RUN YOUR CITY TSHWANE 10K'),
('LOG002', 'USR002', 'Create Event', 'Created event: Two Oceans Ultra Marathon'),
('LOG003', 'USR003', 'Enrol Participant', 'Enrolled in event: Soweto Marathon'),
('LOG004', 'USR004', 'Enrol Participant', 'Enrolled in event: Soweto Marathon');

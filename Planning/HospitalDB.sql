-------------------------------------
-- Clean up existing tables (if any)
-------------------------------------
IF OBJECT_ID('dbo.Schedules', 'U') IS NOT NULL
    DROP TABLE dbo.Schedules;

IF OBJECT_ID('dbo.Shifts', 'U') IS NOT NULL
    DROP TABLE dbo.Shifts;

IF OBJECT_ID('dbo.Documents', 'U') IS NOT NULL
    DROP TABLE dbo.Documents;

IF OBJECT_ID('dbo.MedicalRecords', 'U') IS NOT NULL
    DROP TABLE dbo.MedicalRecords;

IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL
    DROP TABLE dbo.Appointments;

IF OBJECT_ID('dbo.Procedures', 'U') IS NOT NULL
    DROP TABLE dbo.Procedures;

IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL
    DROP TABLE dbo.Patients;

IF OBJECT_ID('dbo.Doctors', 'U') IS NOT NULL
    DROP TABLE dbo.Doctors;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;

IF OBJECT_ID('dbo.Departments', 'U') IS NOT NULL
    DROP TABLE dbo.Departments;

-------------------------------------
-- Create Departments
-------------------------------------
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL
);


-------------------------------------
-- Create Users
-------------------------------------
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY, -- Auto-incrementing primary key
    Username NVARCHAR(50) NOT NULL UNIQUE, -- Unique username
    Password NVARCHAR(255) NOT NULL, -- Store hashed passwords, so length is higher
    Mail NVARCHAR(100) NOT NULL UNIQUE, -- Unique email
    Role NVARCHAR(50) NOT NULL DEFAULT 'User', -- Default role
    Name NVARCHAR(100) NOT NULL,
    BirthDate DATE NOT NULL, -- 'DateOnly' maps to DATE in SQL
    Cnp NVARCHAR(20) NOT NULL UNIQUE, -- Unique identifier
    Address NVARCHAR(255) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE() -- Automatically set on insert
);


-------------------------------------
-- Create Doctors
-------------------------------------
CREATE TABLE Doctors (
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT,
    DepartmentId INT NOT NULL,
	DoctorRating FLOAT NULL DEFAULT 0.0,
    LicenseNumber NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Doctors_Departments
        FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
	CONSTRAINT FK_Doctors_Users
		FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


-------------------------------------
-- Create Patients
-------------------------------------
CREATE TABLE Patients (
    UserId INT NOT NULL, -- Foreign key reference to Users table
    PatientId INT IDENTITY(1,1) PRIMARY KEY, -- Auto-increment primary key
    BloodType NVARCHAR(3) NOT NULL CHECK (BloodType IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')), -- Enum-like constraint
    EmergencyContact NVARCHAR(20) NOT NULL, -- Phone number for emergency contact
    Allergies NVARCHAR(255) NULL, -- Can be NULL if no allergies
    Weight FLOAT NOT NULL CHECK (Weight > 0), -- Prevent invalid weight values
    Height INT NOT NULL CHECK (Height > 0), -- Height in cm, must be positive

    CONSTRAINT FK_Patients_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);


-------------------------------------
-- Create Procedures
-------------------------------------
CREATE TABLE Procedures (
    ProcedureId INT IDENTITY(1,1) PRIMARY KEY,
    ProcedureName VARCHAR(100) NOT NULL,
    ProcedureDuration TIME NOT NULL,
    DepartmentId INT NOT NULL,
    CONSTRAINT FK_Procedures_Departments
        FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);


-------------------------------------
-- Create Appointments
-------------------------------------
CREATE TABLE Appointments (
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    PatientId INT NOT NULL,
    DateAndTime DATETIME2 NOT NULL,
    Finished BIT NOT NULL,
    ProcedureId INT NOT NULL,
    CONSTRAINT FK_Appointments_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_Appointments_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    CONSTRAINT FK_Appointments_Procedures
        FOREIGN KEY (ProcedureId) REFERENCES Procedures(ProcedureId)
);


-------------------------------------
-- Create MedicalRecords
-------------------------------------
CREATE TABLE MedicalRecords (
    MedicalRecordId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    DateAndTime DATETIME2 NOT NULL,
    ProcedureId INT NOT NULL,
    Conclusion NVARCHAR(255) NULL,
    CONSTRAINT FK_MedicalRecords_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    CONSTRAINT FK_MedicalRecords_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    CONSTRAINT FK_MedicalRecords_Procedures
        FOREIGN KEY (ProcedureId) REFERENCES Procedures(ProcedureId)
);


-------------------------------------
-- Create Documents 
-------------------------------------
CREATE TABLE Documents (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
	MedicalRecordId INT NOT NULL,
	CONSTRAINT FK_Documents_MedicalRecords
		FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecords(MedicalRecordID),
    Files VARCHAR(100),
);


-------------------------------------
-- Create Shifts
-------------------------------------
CREATE TABLE Shifts(
	ShiftId INT IDENTITY(1,1) PRIMARY KEY,
	Date DATETIME NOT NULL,
	StartTime TIME NOT NULL,
	EndTime Time NOT NULL
)


-------------------------------------
-- Create Schedules
-------------------------------------
CREATE TABLE Schedules(
	DoctorId INT,
	ShiftId INT,
	PRIMARY KEY(DoctorId, ShiftId),
	CONSTRAINT FK_Schedule_DoctorId
		FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
	CONSTRAINT FK_Schedule_ShiftId
		FOREIGN KEY (ShiftId) REFERENCES Shifts(ShiftId)
	
)



-------------------------------------
-- Insert Departments
-------------------------------------
INSERT INTO Departments (DepartmentName)
VALUES
    ('Cardiology'),      -- DepartmentId = 1
    ('Neurology'),       -- DepartmentId = 2
    ('Pediatrics');      -- DepartmentId = 3

-------------------------------------
-- Insert Users (Doctors and Patients)
-------------------------------------
INSERT INTO Users (Username, Password, Mail, Role, Name, BirthDate, Cnp, Address, PhoneNumber)
VALUES 
('john_doe', 'hashed_password_1', 'john@example.com', 'Doctor', 'John Doe', '1990-05-15', '1234567890123', '123 Main St', '123-456-7890'),
('jane_doe', 'hashed_password_2', 'jane@example.com', 'Doctor', 'Jane Doe', '1995-08-20', '2345678901234', '456 Elm St', '321-654-0987'),
('alice_smith', 'hashed_password_3', 'alice@example.com', 'Doctor', 'Alice Smith', '1988-12-10', '3456789012345', '789 Oak St', '987-123-4567'),
('bob_johnson', 'hashed_password_4', 'bob@example.com', 'Doctor', 'Bob Johnson', '1985-07-25', '4567890123456', '147 Pine St', '654-987-3210');

INSERT INTO Users (Username, Password, Mail, Role, Name, BirthDate, Cnp, Address, PhoneNumber)
VALUES 
('jane_do', 'hashed_password_5', 'janedo@example.com', 'Patient', 'Jane Doe', '1992-03-10', '1334567890123', '123 Main St', '123-456-7890'),
('mike_davis', 'hashed_password_6', 'mike@example.com', 'Patient', 'Mike Davis', '1988-07-15', '2445678901234', '456 Oak St', '321-654-0987'),
('sarah_miller', 'hashed_password_7', 'sarah@example.com', 'Patient', 'Sarah Miller', '1995-12-05', '3756789012345', '789 Pine St', '987-123-4567');


-------------------------------------
-- Insert Doctors
-------------------------------------
INSERT INTO Doctors (UserId, DepartmentId, LicenseNumber)
VALUES
    (1, 1, '696969'),   -- DoctorId = 1, Dept = Cardiology
    (2, 1, '3222'),  -- DoctorId = 2, Dept = Cardiology
    (3, 2, '231231'), -- DoctorId = 3, Dept = Neurology
    (4, 3, '124211');   -- DoctorId = 4, Dept = Pediatrics


-------------------------------------
-- Insert Patients
-------------------------------------
INSERT INTO Patients (UserId, BloodType, EmergencyContact, Allergies, Weight, Height)
VALUES 
(5, 'A+', '111-222-3333', 'Peanuts', 60.5, 165),  -- Jane Doe
(6, 'O-', '222-333-4444', 'None', 80.0, 175),     -- Mike Davis
(7, 'B+', '333-444-5555', 'Pollen', 70.2, 170);   -- Sarah Miller


-------------------------------------
-- Insert Procedures
-------------------------------------
INSERT INTO Procedures (ProcedureName, ProcedureDuration, DepartmentId)
VALUES
    ('ECG', '00:30:00', 1),        -- ProcedureId = 1, Cardiology
    ('Brain MRI', '01:00:00', 2),  -- ProcedureId = 2, Neurology
    ('Child Checkup', '00:20:00', 3), -- ProcedureId = 3, Pediatrics
    ('Stress Test', '00:45:00', 1);   -- ProcedureId = 4, Cardiology


-------------------------------------
-- Insert Appointments
-------------------------------------
INSERT INTO Appointments (PatientId, DoctorId, DateAndTime, ProcedureId, Finished)
VALUES
    (1, 1, '2023-03-17T12:00:00', 1, 0), -- Jane Doe w/ Dr. John Smith (Cardiology)
    (1, 2, '2023-03-17T13:00:00', 4, 1), -- Jane Doe w/ Dr. Alice Brown (Cardiology)
    (2, 3, '2023-03-18T09:30:00', 2, 0), -- Mike Davis w/ Dr. Robert Johnson (Neurology)
    (2, 4, '2023-03-19T10:15:00', 3, 0), -- Mike Davis w/ Dr. Emily Carter (Pediatrics)
    (3, 1, '2023-03-19T14:45:00', 1, 1), -- Sarah Miller w/ Dr. John Smith (Cardiology)
    (3, 2, '2023-03-20T15:00:00', 4, 0); -- Sarah Miller w/ Dr. Alice Brown (Cardiology)


-------------------------------------
-- Insert MedicalRecords
-------------------------------------
INSERT INTO MedicalRecords (PatientId, DoctorId, DateAndTime, ProcedureId, Conclusion)
VALUES
    (1, 1, '2023-03-17T12:30:00', 1, 'Normal ECG results'), -- Jane Doe w/ Dr. John Smith (Cardiology)
    (1, 2, '2023-03-17T13:45:00', 4, 'Stress test results pending'), -- Jane Doe w/ Dr. Alice Brown (Cardiology)
    (2, 3, '2023-03-18T10:30:00', 2, 'Brain MRI results normal'), -- Mike Davis w/ Dr. Robert Johnson (Neurology)
    (2, 4, '2023-03-19T11:00:00', 3, 'Child checkup results normal'), -- Mike Davis w/ Dr. Emily Carter (Pediatrics)
    (3, 1, '2023-03-19T15:30:00', 1, 'ECG results pending'), -- Sarah Miller w/ Dr. John Smith (Cardiology)
    (3, 2, '2023-03-20T16:00:00', 4, 'Stress test results normal'); -- Sarah Miller w/ Dr. Alice Brown (Cardiology)


-------------------------------------
-- Insert Documents
-------------------------------------
INSERT INTO Documents (MedicalRecordId, Files)
VALUES
    (1, 'C:\Users\Cristi\Desktop\file1.docx'),      -- DocumentId = 1
    (2,'C:\Users\Cristi\Desktop\file1.docx'),       -- DocumentId = 2
    (3,'C:\Users\Cristi\Desktop\file1.docx'),      -- DocumentId = 3
    (3,'C:\Users\Cristi\Desktop\file2.docx');      -- DocumentId = 4


-------------------------------------
-- Insert Shifts (!make sure to add them in the future!)
-------------------------------------
INSERT INTO Shifts (Date, StartTime, EndTime)
VALUES
    ('2025-03-21', '08:00:00', '12:00:00'),
    ('2025-03-21', '12:00:00', '16:00:00'),
    ('2025-03-21', '16:00:00', '20:00:00'),
    ('2025-03-22', '08:00:00', '12:00:00'),
    ('2025-03-22', '12:00:00', '16:00:00'),
    ('2025-03-22', '16:00:00', '20:00:00'),
    ('2025-03-23', '08:00:00', '12:00:00'),
    ('2025-03-23', '12:00:00', '16:00:00'),
    ('2025-03-23', '16:00:00', '20:00:00');


-------------------------------------
-- Insert Schedules
-------------------------------------
INSERT INTO Schedules (DoctorId, ShiftId)
VALUES
    (1, 1),  -- Dr. John Doe assigned to morning shift on March 21
    (1, 2),  -- Dr. John Doe assigned to afternoon shift on March 21
    (2, 3),  -- Dr. Jane Doe assigned to evening shift on March 21
    (2, 4),  -- Dr. Jane Doe assigned to morning shift on March 22
    (3, 5),  -- Dr. Alice Smith assigned to afternoon shift on March 22
    (3, 6),  -- Dr. Alice Smith assigned to evening shift on March 22
    (4, 7),  -- Dr. Bob Johnson assigned to morning shift on March 23
    (4, 8),  -- Dr. Bob Johnson assigned to afternoon shift on March 23
    (1, 9);  -- Dr. John Doe assigned to evening shift on March 23



-------------------------------------
-- Verify Data
-------------------------------------
-- All Departments
SELECT * FROM Departments;

-- All Users
SELECT * FROM Users;

-- All Doctors
SELECT * FROM Doctors;

-- All Patients
SELECT * FROM Patients;

-- All Procedures
SELECT * FROM Procedures;

-- All Appointments
SELECT * FROM Appointments;

-- All MedicalRecords
SELECT * FROM MedicalRecords;

-- All Documents
SELECT * FROM Documents;

--All Shifts
SELECT * FROM Shifts;

--All Schedules
SELECT * FROM Schedules



-------------------------------------
-- AppointmentJointModel Query
-------------------------------------
SELECT 
    a.AppointmentId,
    a.Finished,
    a.DateAndTime,
    d.DepartmentId,
    d.DepartmentName,
    doc.DoctorId,
    u1.Name AS DoctorName,
    p.PatientId,
    u2.Name AS PatientName,
    pr.ProcedureId,
    pr.ProcedureName,
    pr.ProcedureDuration
FROM Appointments a
JOIN Doctors doc ON a.DoctorId = doc.DoctorId
JOIN Users u1 ON doc.UserId = u1.UserId
JOIN Departments d ON doc.DepartmentId = d.DepartmentId
JOIN Patients p ON a.PatientId = p.PatientId
JOIN Users u2 ON p.UserId = u2.UserId
JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
ORDER BY a.AppointmentId;

-------------------------------------
-- GetAppointmentsByDoctorAndDate Query
-------------------------------------
SELECT 
    a.AppointmentId,
    a.Finished,
    a.DateAndTime,
    d.DepartmentId,
    d.DepartmentName,
    doc.DoctorId,
    u1.Name AS DoctorName,
    p.PatientId,
    u2.Name AS PatientName,
    pr.ProcedureId,
    pr.ProcedureName,
    pr.ProcedureDuration
FROM Appointments a
JOIN Doctors doc ON a.DoctorId = doc.DoctorId
JOIN Users u1 ON doc.UserId = u1.UserId
JOIN Departments d ON doc.DepartmentId = d.DepartmentId
JOIN Patients p ON a.PatientId = p.PatientId
JOIN Users u2 ON p.UserId = u2.UserId
JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
WHERE a.DoctorId = 1
  AND CONVERT(DATE, a.DateAndTime) = '2023-03-17'
ORDER BY a.DateAndTime;

-------------------------------------
-- GetAppointmentsForDoctor Query
-------------------------------------
SELECT 
    a.AppointmentId,
    a.Finished,
    a.DateAndTime,
    d.DepartmentId,
    d.DepartmentName,
    doc.DoctorId,
    u1.Name AS DoctorName,
    p.PatientId,
    u2.Name AS PatientName,
    pr.ProcedureId,
    pr.ProcedureName,
    pr.ProcedureDuration
FROM Appointments a
JOIN Doctors doc ON a.DoctorId = doc.DoctorId
JOIN Users u1 ON doc.UserId = u1.UserId
JOIN Departments d ON doc.DepartmentId = d.DepartmentId
JOIN Patients p ON a.PatientId = p.PatientId
JOIN Users u2 ON p.UserId = u2.UserId
JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
WHERE a.DoctorId = 2
ORDER BY a.DateAndTime;

-------------------------------------
-- GetAppointments Query
-------------------------------------
SELECT 
    a.AppointmentId,
    a.Finished,
    a.DateAndTime,
    d.DepartmentId,
    d.DepartmentName,
    doc.DoctorId,
    u1.Name AS DoctorName,
    p.PatientId,
    u2.Name AS PatientName,
    pr.ProcedureId,
    pr.ProcedureName,
    pr.ProcedureDuration
FROM Appointments a
JOIN Doctors doc ON a.DoctorId = doc.DoctorId
JOIN Users u1 ON doc.UserId = u1.UserId
JOIN Departments d ON doc.DepartmentId = d.DepartmentId
JOIN Patients p ON a.PatientId = p.PatientId
JOIN Users u2 ON p.UserId = u2.UserId
JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
ORDER BY a.AppointmentId;

-------------------------------------
-- GetMedicalRecords Query
-------------------------------------
SELECT 
    mr.MedicalRecordId,
    mr.PatientId, 
    p.Name AS PatientName,
    mr.DoctorId,
    d.Name AS DoctorName,
    pr.DepartmentId,
    dept.DepartmentName,
    mr.ProcedureId, 
    pr.ProcedureName, 
    mr.DateAndTime, 
    mr.Conclusion 
FROM MedicalRecords mr 
JOIN Users p ON mr.PatientId = p.UserId 
JOIN Users d ON mr.DoctorId = d.UserId 
JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId
JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId;
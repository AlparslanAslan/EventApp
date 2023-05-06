-- Create a new table called '[EventUser]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[EventUser]', 'U') IS NOT NULL
DROP TABLE [dbo].[EventUser]
GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[EventUser]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), -- Primary Key column
    Name varchar(30) not null,
    Surname varchar(30) not null,
    Email varchar(50) not null,
    Password varchar(20) not null,
    Role varchar(10) null
);
GO

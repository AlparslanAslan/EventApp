-- Create a new table called '[Sehir]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[Sehir]', 'U') IS NOT NULL
DROP TABLE [dbo].[Sehir]
GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[Sehir]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), -- Primary Key column
    Name varchar(100)
    -- Specify more columns here
);
GO
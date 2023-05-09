-- Create a new table called '[Kategori]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[Kategori]', 'U') IS NOT NULL
DROP TABLE [dbo].[Kategori]
--GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[Kategori]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), -- Primary Key column
    Name varchar(100)
    -- Specify more columns here
);
--GO
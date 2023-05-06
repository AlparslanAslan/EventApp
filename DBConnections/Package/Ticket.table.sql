-- Create a new table called '[Ticket]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[Ticket]', 'U') IS NOT NULL
DROP TABLE [dbo].[Ticket]
GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[Ticket]
(
    [BiletNo] varchar(50) NOT NULL , -- Primary Key column
    [UserId] INT,
    [EventId] INT
    
);
GO
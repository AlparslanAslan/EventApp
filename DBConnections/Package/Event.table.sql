-- Create a new table called '[Event]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[Event]', 'U') IS NOT NULL
DROP TABLE [dbo].[Event]
GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[Event]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), -- Primary Key column
    baslik varchar(100),
    aciklama varchar(300),
    tarih date ,
    kontenjan INT,
    kategoriid int,
    sehirid int,
    olusturanid int,
    onay bit,
    aktif int,
    adres varchar(200)
);
GO


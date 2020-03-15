DROP PROCEDURE IF EXISTS [dbo].[GetStreamByName];
GO

CREATE PROCEDURE [dbo].[GetStreamByName] 
    @StreamName NVARCHAR(256)
AS
BEGIN
	SELECT [StreamId], [Version], [Name], [CreatedAt], [UpdatedAt]
    FROM dbo.[Streams]
    WHERE [Name] = @StreamName;
END
DROP PROCEDURE IF EXISTS [dbo].[AddStream];
GO

CREATE PROCEDURE [dbo].[AddStream] 
    @StreamId [UNIQUEIDENTIFIER], 
    @Name [NVARCHAR](256), 
    @Version [INT] 
AS
BEGIN
	INSERT INTO [dbo].[Streams] ([StreamId], [Name], [Version]) 
    VALUES (@StreamId, @Name, @Version);
END
DROP PROCEDURE IF EXISTS [dbo].[AddEvent];
GO

CREATE PROCEDURE [dbo].[AddEvent] 
	@EventId [UNIQUEIDENTIFIER], 
	@StreamId [UNIQUEIDENTIFIER], 
	@Type [NVARCHAR](256), 
	@Data [NVARCHAR](MAX), 
	@Version [INT]
AS
BEGIN
	INSERT INTO [dbo].[Events] ([EventId], [StreamId], [Type], [Data], [Version])
	VALUES (@EventId, @StreamId, @Type, @Data, @Version);
END
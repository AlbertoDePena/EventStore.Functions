DROP PROCEDURE IF EXISTS [dbo].[GetEvents];
GO

CREATE PROCEDURE [dbo].[GetEvents] 
    @StreamName [NVARCHAR](256), 
    @StartAtVersion [INT]
AS
BEGIN
	SELECT E.[EventId], E.[StreamId], E.[Type], E.[Version], E.[Data], E.[CreatedAt]
    FROM [dbo].[Events] E
    JOIN [dbo].[Streams] S
    ON E.[StreamId] = S.[StreamId]
    WHERE S.[Name] = @StreamName
    AND E.[Version] >= @StartAtVersion
    ORDER BY E.[Version];
END
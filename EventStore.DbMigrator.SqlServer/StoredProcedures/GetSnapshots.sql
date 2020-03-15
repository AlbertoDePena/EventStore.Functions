DROP PROCEDURE IF EXISTS [dbo].[GetSnapshots];
GO

CREATE PROCEDURE [dbo].[GetSnapshots] 
    @StreamName [NVARCHAR](256)
AS
BEGIN
	SELECT SN.[SnapshotId], SN.[StreamId], SN.[Version], SN.[Data], SN.[Description], SN.[CreatedAt]
    FROM [dbo].[Snapshots] SN
    JOIN [dbo].[Streams] S
    ON SN.[StreamId] = S.[StreamId]
    WHERE S.[Name] = @StreamName
    ORDER BY SN.[Version] DESC;
END
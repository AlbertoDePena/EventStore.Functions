DROP PROCEDURE IF EXISTS [dbo].[AddSnapshot];
GO

CREATE PROCEDURE [dbo].[AddSnapshot] 
    @SnapshotId [UNIQUEIDENTIFIER], 
    @StreamId [UNIQUEIDENTIFIER], 
    @Description [NVARCHAR](256), 
    @Data [NVARCHAR](MAX), 
    @Version [INT]
AS
BEGIN
	INSERT INTO [dbo].[Snapshots] ([SnapshotId], [StreamId], [Description], [Data], [Version])
    VALUES (@SnapshotId, @StreamId, @Description, @Data, @Version);
END
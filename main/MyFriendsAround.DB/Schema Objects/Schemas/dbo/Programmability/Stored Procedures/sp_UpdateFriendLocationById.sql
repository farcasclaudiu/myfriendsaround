SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
CREATE PROCEDURE UpdateFriendLocationById 
	@FriendID nvarchar 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    UPDATE [Friends] 
    SET [Location] = GEOGRAPHY::Parse([LocationStr])
    WHERE Id = @FriendID
END
GO

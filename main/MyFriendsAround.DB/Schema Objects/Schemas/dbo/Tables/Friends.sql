-- =============================================
-- Script Template
-- =============================================
USE [MyFriends]
GO
/****** Object:  Table [dbo].[Friends]    Script Date: 03/14/2011 18:32:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friends](
	[Id] [nvarchar](100) NOT NULL,
	[FriendName] [nvarchar](50) NOT NULL,
	[FriendImageUrl] [nvarchar](150) NULL,
	[LocationStr] [nvarchar](100) NULL,
	[Location] [geography] NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Friends] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Friends] ([Id], [FriendName], [FriendImageUrl], [LocationStr], [Location], [LastUpdated]) VALUES (N'1', N'Clody', N'http://aaa.com/aaa.png', N'POINT(10 34)', NULL, CAST(0x00009EA600000000 AS DateTime))

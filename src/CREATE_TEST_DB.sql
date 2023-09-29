USE [GenesisTest]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 9/29/2023 9:25:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articles](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryID] [smallint] NOT NULL,
	[PersonID] [int] NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[SubTitle] [nvarchar](500) NULL,
	[BodyContent] [text] NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 9/29/2023 9:25:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[Label] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[ArticleCount] [int] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Persons]    Script Date: 9/29/2023 9:25:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persons](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProfileID] [int] NULL,
	[FirstName] [nvarchar](25) NOT NULL,
	[LastName] [nvarchar](25) NOT NULL,
	[Email] [nvarchar](125) NOT NULL,
	[Alias] [nvarchar](25) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [nvarchar](25) NOT NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](25) NULL,
 CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SiteThemes]    Script Date: 9/29/2023 9:25:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteThemes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CodeDescriptor] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SiteThemes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 9/29/2023 9:25:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfiles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SiteTheme] [int] NULL,
 CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[SiteThemes] ON 
GO
INSERT [dbo].[SiteThemes] ([ID], [Name], [Description], [CodeDescriptor]) VALUES (1, N'Default', N'Default theme using system settings if possible.', N'default')
GO
SET IDENTITY_INSERT [dbo].[SiteThemes] OFF
GO
ALTER TABLE [dbo].[Articles] ADD  CONSTRAINT [DF_Articles_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_ArticleCount]  DEFAULT ((0)) FOR [ArticleCount]
GO
ALTER TABLE [dbo].[Persons] ADD  CONSTRAINT [DF_Persons_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[UserProfiles] ADD  CONSTRAINT [DF_Profiles_SiteTheme]  DEFAULT ((1)) FOR [SiteTheme]
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([ID])
GO
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_Categories]
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_Persons] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Persons] ([ID])
GO
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_Persons]
GO
ALTER TABLE [dbo].[Persons]  WITH CHECK ADD  CONSTRAINT [FK_Persons_Profiles] FOREIGN KEY([ProfileID])
REFERENCES [dbo].[UserProfiles] ([ID])
GO
ALTER TABLE [dbo].[Persons] CHECK CONSTRAINT [FK_Persons_Profiles]
GO
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_Profiles_SiteThemes] FOREIGN KEY([SiteTheme])
REFERENCES [dbo].[SiteThemes] ([ID])
GO
ALTER TABLE [dbo].[UserProfiles] CHECK CONSTRAINT [FK_Profiles_SiteThemes]
GO

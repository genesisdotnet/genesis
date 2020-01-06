CREATE TABLE [dbo].[CoolObjects] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (256) NOT NULL,
	[IntField] INT NOT NULL,
	[BigIntField] BIGINT NOT NULL,
	[GuidField] UNIQUEIDENTIFIER NOT NULL,
	[BigTextField] TEXT NOT NULL,
    [CreatedOn]   DATETIME2 (7)  NOT NULL,
	[ModifiedOn]   DATETIME2 (7)
);

CREATE TABLE [dbo].[ImporantItems] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (256) NOT NULL,
	[IntField] INT NOT NULL,
	[BigIntField] BIGINT NOT NULL,
	[GuidField] UNIQUEIDENTIFIER NOT NULL,
	[BigTextField] TEXT NOT NULL,
    [CreatedOn]   DATETIME2 (7)  NOT NULL,
	[ModifiedOn]   DATETIME2 (7)
);

CREATE TABLE [dbo].[UserSettings] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (256) NOT NULL,
	[IntField] INT NOT NULL,
	[BigIntField] BIGINT NOT NULL,
	[GuidField] UNIQUEIDENTIFIER NOT NULL,
	[BigTextField] TEXT NOT NULL,
    [CreatedOn]   DATETIME2 (7)  NOT NULL,
	[ModifiedOn]   DATETIME2 (7)
);
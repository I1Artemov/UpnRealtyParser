CREATE TABLE [UpnHouseInfo] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [Address] nvarchar(max),
    [HouseType] nvarchar(256),
    [BuildYear] int,
    [WallMaterial] nvarchar(256),
    [MaxFloor] int,
    [Latitude] float,
    [Longitude] float,
	PRIMARY KEY ([Id])
);

CREATE TABLE [UpnAgency] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [Name] nvarchar(max),
    [WorkTime] nvarchar(max),
    [CompanyPhone] nvarchar(max),
    [AgentPhone] nvarchar(max),
    [SiteUrl] nvarchar(max),
    [Email] nvarchar(max),
	PRIMARY KEY ([Id])
);

CREATE TABLE [UpnFlat] (
    [Id] int IDENTITY(1,1),
    [RemovalDate] datetime,
    [CreationDateTime] datetime,
    [PageCreationDateTime] datetime,
    [LastCheckDate] datetime,
    [UpnHouseInfoId] int,
    [UpnAgencyId] int,
    [FlatType] nvarchar(64),
    [RoomAmount] int,
    [SpaceSum] float,
    [SpaceLiving] float,
    [SpaceKitchen] float,
    [FlatFloor] int,
    [JointBathrooms] int,
    [SeparateBathrooms] int,
    [RenovationType] nvarchar(256),
    [RedevelopmentType] nvarchar(256),
    [WindowsType] nvarchar(128),
    [Furniture] nvarchar(256),
    [Price] int,
    [Description] nvarchar(max),
    [IdOnSite] nvarchar(256),
    [SiteUrl] nvarchar(max),
    [SellCondition] nvarchar(256),
	PRIMARY KEY ([Id]),
    CONSTRAINT FK_UpnFlat_UpnHouseInfo_HouseInfoId FOREIGN KEY ([UpnHouseInfoId])
        REFERENCES [UpnHouseInfo]([Id]),
    CONSTRAINT FK_UpnFlat_UpnAgency_UpnAgencyId FOREIGN KEY ([UpnAgencyId])
        REFERENCES [UpnAgency]([Id]),
);

CREATE INDEX idx_UpnFlat_UpnHouseInfoId ON [UpnFlat] ([UpnHouseInfoId]);
CREATE INDEX idx_UpnFlat_UpnAgencyId ON [UpnFlat] ([UpnAgencyId]);

CREATE TABLE [UpnFlatPhoto] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [RelationType] nvarchar(64),
    [FileName] nvarchar(max),
    [FlatId] int, -- Без Constraint
	PRIMARY KEY ([Id])
);

CREATE INDEX idx_UpnFlatPhoto_FlatId ON [UpnFlatPhoto] ([FlatId]);
CREATE INDEX idx_UpnFlatPhoto_RelationType ON [UpnFlatPhoto] ([RelationType]);

-- TODO: Сделать UpnRentFlat

-- Добавлено 19.03.2020 - таблицы со ссылками и состоянием работы парсера

CREATE TABLE [PageLink] (
	[Id] int IDENTITY(1,1),
	[CreationDateTime] datetime,
	[LastCheckDateTime] datetime,
	[Href] nvarchar(max),
	[LinkType] nvarchar(128),
	[SiteName] nvarchar(128),
	PRIMARY KEY ([Id])
);

CREATE TABLE [ParsingState] (
	[Id] bigint IDENTITY(1,1), --BIGINT!
	[CreationDateTime] datetime,
	[SiteName] nvarchar(128),
	[Description] nvarchar(256),
	[Details] nvarchar(max),
	[Status] nvarchar(256),
	PRIMARY KEY ([Id])
);

ALTER TABLE [UpnFlat] DROP COLUMN [SiteUrl];
ALTER TABLE [UpnFlat] DROP COLUMN [IdOnSite];
ALTER TABLE [UpnFlat] DROP COLUMN [PageCreationDateTime];
ALTER TABLE [UpnFlat] ADD [PageLinkId] INT;
ALTER TABLE [UpnFlat] ADD CONSTRAINT
	FK_UpnFlat_PageLink_PageLinkId FOREIGN KEY ([PageLinkId]) REFERENCES [PageLink]([Id]);

CREATE INDEX idx_UpnFlat_PageLinkId ON [UpnFlat] ([PageLinkId]);

ALTER TABLE [PageLink] ADD [IsDead] BIT;

-- 06.04.2020: Новый стобец для таблицы с фотографиями
ALTER TABLE [UpnFlatPhoto] ADD [Href] nvarchar(max);

-- 03.07.2020: Таблица для хранения проксей
CREATE TABLE [WebProxyInfo] (
	[Id] int IDENTITY(1,1),
	[CreationDateTime] datetime,
	[LastUseDateTime] datetime,
	[Ip] nvarchar(128),
	[Port] nvarchar(64),
	[SuccessAmount] int,
	[FailureAmount] int,
	PRIMARY KEY ([Id])
);
ALTER TABLE [WebProxyInfo] ADD [LastSuccessDateTime] datetime;

-- 07.07.2020: Станции метро
CREATE TABLE [SubwayStation] (
	[Id] int IDENTITY(1,1),
	[Name] nvarchar(256),
	[Latitude] float,
    [Longitude] float,
	PRIMARY KEY ([Id])
);

INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Проспект космонавтов', 56.901276, 60.613984);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Уралмаш', 56.888068, 60.613538);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Машиностроителей', 56.878514, 60.612160);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Уральская', 56.857875, 60.600372);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Динамо', 56.847523, 60.600201);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Площадь 1905 года', 56.836122, 60.599370);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Геологическая', 56.827069, 60.602959);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Чкаловская', 56.808601, 60.609990);
INSERT INTO [SubwayStation] ([Name], [Latitude], [Longitude]) VALUES ('Ботаническая', 56.797022, 60.632579);

ALTER TABLE [UpnHouseInfo] ADD [ClosestSubwayStationId] int;
ALTER TABLE [UpnHouseInfo] ADD [ClosestSubwayStationRange] float;

ALTER TABLE [UpnHouseInfo] ADD CONSTRAINT
	FK_UpnHouse_Subway_SubwayId FOREIGN KEY ([ClosestSubwayStationId]) REFERENCES [SubwayStation]([Id]);

CREATE INDEX idx_UpnHouse_ClosestSubwayId ON [UpnHouseInfo] ([ClosestSubwayStationId]);

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

-- 21.08.2020 Таблица с арендными квартирами
CREATE TABLE [UpnRentFlat] (
    [Id] int IDENTITY(1,1),
    [RemovalDate] datetime,
    [CreationDateTime] datetime,
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
    [MinimalRentPeriod] nvarchar(256),
	[PageLinkId] int
	PRIMARY KEY ([Id]),
    CONSTRAINT FK_UpnRentFlat_UpnHouseInfo_HouseInfoId FOREIGN KEY ([UpnHouseInfoId])
        REFERENCES [UpnHouseInfo]([Id]),
    CONSTRAINT FK_UpnRentFlat_UpnAgency_UpnAgencyId FOREIGN KEY ([UpnAgencyId])
        REFERENCES [UpnAgency]([Id]),
	CONSTRAINT FK_UpnRentFlat_PageLink_PageLinkId FOREIGN KEY ([PageLinkId]) REFERENCES [PageLink]([Id])
);

CREATE INDEX idx_UpnRentFlat_UpnHouseInfoId ON [UpnRentFlat] ([UpnHouseInfoId]);
CREATE INDEX idx_UpnRentFlat_UpnAgencyId ON [UpnRentFlat] ([UpnAgencyId]);

-- 31.10.2020 Таблицы для квартир с N1
CREATE TABLE [N1HouseInfo] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [Address] nvarchar(max),
    [HouseType] nvarchar(256),
    [BuildYear] int,
    [WallMaterial] nvarchar(256),
    [MaxFloor] int,
    [Latitude] float,
    [Longitude] float,
	[ClosestSubwayStationId] int,
	[ClosestSubwayStationRange] float,
	[BuilderCompany] nvarchar(max), -- NEW
	PRIMARY KEY ([Id])
);

ALTER TABLE [N1HouseInfo] ADD CONSTRAINT
	FK_N1House_Subway_SubwayId FOREIGN KEY ([ClosestSubwayStationId]) REFERENCES [SubwayStation]([Id]);

CREATE INDEX idx_N1House_ClosestSubwayId ON [N1HouseInfo] ([ClosestSubwayStationId]);

CREATE TABLE [N1Agency] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [Name] nvarchar(max),
    [AgentPhone] nvarchar(max),
    [SiteUrl] nvarchar(max),
	[AgentName] nvarchar(256), -- NEW
	PRIMARY KEY ([Id])
);

CREATE TABLE [N1Flat] (
    [Id] int IDENTITY(1,1),
    [RemovalDate] datetime,
    [CreationDateTime] datetime,
    [LastCheckDate] datetime,
    [N1HouseInfoId] int,
    [N1AgencyId] int,
    [RoomAmount] int,
    [SpaceSum] float,
    [SpaceLiving] float,
    [SpaceKitchen] float,
    [FlatFloor] int,
    [Price] int,
    [Description] nvarchar(max),
	[PlanningType] nvarchar(max), -- NEW
	[BathroomType] nvarchar(max), -- NEW
	[BalconyAmount] int, -- NEW
	[Condition] nvarchar(max), -- NEW
	[PropertyType] nvarchar(max), -- NEW
	[IsFilledCompletely] bit, -- NEW
	[PageLinkId] int,
	PRIMARY KEY ([Id])
);

ALTER TABLE [N1Flat] ADD CONSTRAINT
	FK_N1Flat_N1House_HouseInfoId FOREIGN KEY ([N1HouseInfoId]) REFERENCES [N1HouseInfo]([Id]);
ALTER TABLE [N1Flat] ADD CONSTRAINT
	FK_N1Flat_N1Agency_N1AgencyId FOREIGN KEY ([N1AgencyId]) REFERENCES [N1Agency]([Id]);
ALTER TABLE [N1Flat] ADD CONSTRAINT
	FK_N1Flat_PageLink_PageLinkId FOREIGN KEY ([PageLinkId]) REFERENCES [PageLink]([Id]);

CREATE INDEX idx_N1Flat_N1HouseInfoId ON [N1Flat] ([N1HouseInfoId]);
CREATE INDEX idx_N1Flat_N1AgencyId ON [N1Flat] ([N1AgencyId]);

CREATE TABLE [N1FlatPhoto] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
    [RelationType] nvarchar(64),
    [FileName] nvarchar(max),
    [FlatId] int, -- Без Constraint
	[Href] nvarchar(max)
	PRIMARY KEY ([Id])
);

CREATE INDEX idx_N1FlatPhoto_FlatId ON [N1FlatPhoto] ([FlatId]);
CREATE INDEX idx_N1FlatPhoto_RelationType ON [N1FlatPhoto] ([RelationType]);

-- 05.11.2020 Новое поле в N1Agency
ALTER TABLE [N1Agency] ADD [IsCompany] bit;

-- 11.11.2020 Новое поле для квартир N1
ALTER TABLE [N1Flat] ADD [PublishingDateTime] datetime;

-- 24.11.2020 Поле IsFilledCompletely у домов N1
ALTER TABLE [N1HouseInfo] ADD [IsFilledCompletely] bit;

-- 18.12.2020 Вьюшка с квартирами УПН, дозаполненными информацией из других таблиц
create view [vUpnFlatAdditional] as
((select
	uf.[Id],
	uf.[RemovalDate],
	uf.[CreationDateTime],
	uf.[LastCheckDate],
	uf.[FlatType],
	uf.[RoomAmount],
	uf.[SpaceSum],
	uf.[SpaceLiving],
	uf.[SpaceKitchen],
	uf.[FlatFloor],
	uf.[JointBathrooms],
	uf.[SeparateBathrooms],
	uf.[RenovationType],
	uf.[RedevelopmentType],
	uf.[WindowsType],
	uf.[Furniture],
	uf.[Price],
	uf.[Description],
	uf.[SellCondition],
	hou.[Address] as [HouseAddress],
	hou.[BuildYear] as [HouseBuildYear],
	hou.[HouseType],
	hou.[MaxFloor] as [HouseMaxFloor],
	hou.[WallMaterial] as [HouseWallMaterial],
	station.[Name] as [ClosestSubwayName],
	hou.[ClosestSubwayStationRange],
	hou.[Latitude] as [HouseLatitude],
	hou.[Longitude] as [HouseLongitude],
	COALESCE(ag.[AgentPhone], ag.[CompanyPhone]) as [SellerPhone],
	ag.[Name] as [AgencyName],
	(select top 1 pht.[FileName] from [UpnFlatPhoto] pht where pht.[FlatId] = uf.[Id] and pht.[RelationType] = 'SellFlat') as [FirstPhotoFile],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [UpnFlat] uftmp ), 1, 0)
	) as [IsArchived]
from [UpnFlat] uf 
inner join [UpnHouseInfo] hou on uf.UpnHouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [UpnAgency] ag on uf.UpnAgencyId = ag.Id));

-- 06.01.2021 Комбинирование домов УПН и Н1
-- Таблица связей похожих домов
create table [SimilarHouse] (
	[Id] int IDENTITY(1,1),
    [CreationDateTime] datetime,
	[UpnHouseId] int,
	[N1HouseId] int,
	[Distance] float
	PRIMARY KEY ([Id])
);

ALTER TABLE [SimilarHouse] ADD CONSTRAINT
	FK_SimilarHouse_UpnHouse_UpnHouseId FOREIGN KEY ([UpnHouseId]) REFERENCES [UpnHouseInfo]([Id]);
ALTER TABLE [SimilarHouse] ADD CONSTRAINT
	FK_SimilarHouse_N1House_N1HouseId FOREIGN KEY ([N1HouseId]) REFERENCES [N1HouseInfo]([Id]);

CREATE INDEX idx_SimilarHouse_UpnHouseId ON [SimilarHouse] ([UpnHouseId]);
CREATE INDEX idx_SimilarHouse_N1HouseId ON [SimilarHouse] ([N1HouseId]);

-- Вьюшка с обобщением домов УПН и N1
create view vHousesUnitedInfo as
select up.[Id],
	  'UPN' as [SourceSite]
      ,up.[CreationDateTime]
      ,up.[Address]
      ,up.[HouseType]
      ,up.[BuildYear]
      ,up.[WallMaterial]
      ,up.[MaxFloor]
      ,up.[Latitude]
      ,up.[Longitude]
	  ,up.[ClosestSubwayStationId]
      ,sbw.[Name] as [ClosestSubwayName]
      ,up.[ClosestSubwayStationRange]
	  , (select top 1 [Id] from [SimilarHouse] sh where sh.[UpnHouseId] = up.Id) as [SimilarIdentity]
from [UpnHouseInfo] up
left join [SubwayStation] sbw on up.[ClosestSubwayStationId] = sbw.[Id]
union all
select n1.[Id],
	'N1' as [SourceSite]
      ,n1.[CreationDateTime]
      ,n1.[Address]
      ,n1.[HouseType]
      ,n1.[BuildYear]
      ,n1.[WallMaterial]
      ,n1.[MaxFloor]
      ,n1.[Latitude]
      ,n1.[Longitude]
	  ,n1.[ClosestSubwayStationId]
      ,sbw.[Name] as [ClosestSubwayName]
      ,n1.[ClosestSubwayStationRange]
	  , (select top 1 [Id] from [SimilarHouse] sh where sh.[N1HouseId] = n1.Id) as [SimilarIdentity]
from [N1HouseInfo] n1
left join [SubwayStation] sbw on n1.[ClosestSubwayStationId] = sbw.[Id];

-- 10.01.2021 Вьюшка для объединения агентств с N1 и УПН
create view [vAgenciesUnitedInfo] as
select
	upa.[Id],
	'UPN' as [SourceSite],
	upa.[CreationDateTime],
	upa.[Name],
	upa.[AgentPhone],
	upa.[SiteUrl],
	upa.[CompanyPhone],
	upa.[Email],
	upa.[WorkTime],
	NULL as [AgentName],
	NULL as [IsCompany]
from [UpnAgency] upa
union all
select
	n1a.[Id],
	'N1' as [SourceSite],
	n1a.[CreationDateTime],
	n1a.[Name],
	n1a.[AgentPhone],
	n1a.[SiteUrl],
	NULL as [CompanyPhone],
	NULL as [Email],
	NULL as [WorkTime],
	n1a.[AgentName],
	n1a.[IsCompany]
from [N1Agency] n1a;

-- 23.03.2021 Таблица для хранения статистики

create table [AveragePriceStat] (
	[Id] int IDENTITY(1,1),
    [HouseId] int,
	[Site] nvarchar(64),
	[RoomAmount] int,
	[Year] int,
	[Month] int,
	[Price] float,
	PRIMARY KEY ([Id])
);

CREATE INDEX idx_AveragePriceStat_HouseId_Site ON [AveragePriceStat] ([HouseId], [Site]);
CREATE INDEX idx_AveragePriceStat_Year_Month ON [AveragePriceStat] ([Year], [Month]);

-- 15.04.2021 Таблица для хранения текущего состояния парсинга

create table [ServiceStage] (
	[Id] int IDENTITY(1,1),
	[UpdateDate] datetime,
	[Name] nvarchar(64), -- Название сервиса
	[CurrentStage] nvarchar(64),
	[IsComplete] bit,
	PRIMARY KEY ([Id])
);

alter table [ServiceStage] add [PageNumber] int;

-- 17.05.2021 Таблица для хранения точек карты окупаемости
create table [PaybackPeriodPoint] (
	[Id] int IDENTITY(1,1),
	[CreationDateTime] datetime,
	[UpnHouseId] int,
	[Latitude] float,
    [Longitude] float,
	[PaybackYears] float,
	PRIMARY KEY ([Id])
);

-- 24.05.2021 Арендные квартиры N1
CREATE TABLE [N1RentFlat] (
    [Id] int IDENTITY(1,1),
    [RemovalDate] datetime,
    [CreationDateTime] datetime,
    [LastCheckDate] datetime,
    [N1HouseInfoId] int,
    [N1AgencyId] int,
    [RoomAmount] int,
    [SpaceSum] float,
    [SpaceLiving] float,
    [SpaceKitchen] float,
    [FlatFloor] int,
    [Price] int,
    [Description] nvarchar(max),
	[PlanningType] nvarchar(max),
	[BathroomType] nvarchar(max),
	[BalconyAmount] int,
	[Condition] nvarchar(max),
	[PropertyType] nvarchar(max),
	[IsFilledCompletely] bit,
	[PageLinkId] int,
	PRIMARY KEY ([Id])
);
ALTER TABLE [N1RentFlat] ADD [PublishingDateTime] datetime;

ALTER TABLE [N1RentFlat] ADD CONSTRAINT
	FK_N1RentFlat_N1House_HouseInfoId FOREIGN KEY ([N1HouseInfoId]) REFERENCES [N1HouseInfo]([Id]);
ALTER TABLE [N1RentFlat] ADD CONSTRAINT
	FK_N1RentFlat_N1Agency_N1AgencyId FOREIGN KEY ([N1AgencyId]) REFERENCES [N1Agency]([Id]);
ALTER TABLE [N1RentFlat] ADD CONSTRAINT
	FK_N1RentFlat_PageLink_PageLinkId FOREIGN KEY ([PageLinkId]) REFERENCES [PageLink]([Id]);

CREATE INDEX idx_N1RentFlat_N1HouseInfoId ON [N1RentFlat] ([N1HouseInfoId]);
CREATE INDEX idx_N1RentFlat_N1AgencyId ON [N1RentFlat] ([N1AgencyId]);

-- 27.05.2021 Поле с адресом дома в таблице для карты окупаемости
ALTER TABLE [PaybackPeriodPoint] ADD [HouseAddress] nvarchar(512);

UPDATE [PaybackPeriodPoint] SET [HouseAddress] = 
	(SELECT TOP 1 [Address] FROM [UpnHouseInfo] WHERE [Id] = [UpnHouseId]);

--16.06.2021 Чистка имен агентов N1 от разметки
--update N1Agency set [AgentName] = REPLACE([AgentName], '<span class="ui-kit-link__inner" _v-689e5c11="">', '');
--update N1Agency set [AgentName] = REPLACE([AgentName], '</span>', '');
--update N1Agency set [AgentName] = LTRIM([AgentName]);
--update N1Agency set [AgentName] = RTRIM([AgentName]);

-- 22.07.2021 Вьшка для квартир N1 в связке с домами
create view [vN1FlatAdditional] as
((select
	uf.[Id],
	uf.[RemovalDate],
	uf.[CreationDateTime],
	uf.[LastCheckDate],
	uf.[PlanningType],
	uf.[RoomAmount],
	uf.[SpaceSum],
	uf.[SpaceLiving],
	uf.[SpaceKitchen],
	uf.[FlatFloor],
	uf.[BathroomType],
	uf.[BalconyAmount],
	uf.[Condition],
	uf.[PropertyType],
	uf.[Price],
	uf.[Description],
	hou.[Address] as [HouseAddress],
	hou.[BuildYear] as [HouseBuildYear],
	hou.[HouseType],
	hou.[MaxFloor] as [HouseMaxFloor],
	hou.[WallMaterial] as [HouseWallMaterial],
	station.[Name] as [ClosestSubwayName],
	hou.[ClosestSubwayStationRange],
	hou.[Latitude] as [HouseLatitude],
	hou.[Longitude] as [HouseLongitude],
	ag.[AgentPhone] as [SellerPhone],
	ag.[Name] as [AgencyName],
	(select top 1 pht.[FileName] from [N1FlatPhoto] pht where pht.[FlatId] = uf.[Id] and pht.[RelationType] = 'SellFlat') as [FirstPhotoFile],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [N1Flat] uftmp ), 1, 0)
	) as [IsArchived]
from [N1Flat] uf 
inner join [N1HouseInfo] hou on uf.N1HouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [N1Agency] ag on uf.N1AgencyId = ag.Id));

-- 22.07.2021 Вьшка для квартир UPN в аренду в связке с домами
create view [vUpnRentFlatAdditional] as
((select
	uf.[Id],
	uf.[RemovalDate],
	uf.[CreationDateTime],
	uf.[LastCheckDate],
	uf.[FlatType],
	uf.[RoomAmount],
	uf.[SpaceSum],
	uf.[SpaceLiving],
	uf.[SpaceKitchen],
	uf.[FlatFloor],
	uf.[JointBathrooms],
	uf.[SeparateBathrooms],
	uf.[RenovationType],
	uf.[RedevelopmentType],
	uf.[WindowsType],
	uf.[Furniture],
	uf.[Price],
	uf.[Description],
	hou.[Address] as [HouseAddress],
	hou.[BuildYear] as [HouseBuildYear],
	hou.[HouseType],
	hou.[MaxFloor] as [HouseMaxFloor],
	hou.[WallMaterial] as [HouseWallMaterial],
	station.[Name] as [ClosestSubwayName],
	hou.[ClosestSubwayStationRange],
	hou.[Latitude] as [HouseLatitude],
	hou.[Longitude] as [HouseLongitude],
	ag.[AgentPhone] as [SellerPhone],
	ag.[Name] as [AgencyName],
	(select top 1 pht.[FileName] from [UpnFlatPhoto] pht where pht.[FlatId] = uf.[Id] and pht.[RelationType] = 'RentFlat') as [FirstPhotoFile],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [N1Flat] uftmp ), 1, 0)
	) as [IsArchived]
from [UpnRentFlat] uf 
inner join [UpnHouseInfo] hou on uf.UpnHouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [UpnAgency] ag on uf.UpnAgencyId = ag.Id));


-- 28.07.2021 Вьшка для квартир N1 в аренду в связке с домами
create view [vN1RentFlatAdditional] as
((select
	uf.[Id],
	uf.[RemovalDate],
	uf.[CreationDateTime],
	uf.[LastCheckDate],
	uf.[PlanningType],
	uf.[RoomAmount],
	uf.[SpaceSum],
	uf.[SpaceLiving],
	uf.[SpaceKitchen],
	uf.[FlatFloor],
	uf.[BathroomType],
	uf.[BalconyAmount],
	uf.[Condition],
	uf.[PropertyType],
	uf.[Price],
	uf.[Description],
	hou.[Address] as [HouseAddress],
	hou.[BuildYear] as [HouseBuildYear],
	hou.[HouseType],
	hou.[MaxFloor] as [HouseMaxFloor],
	hou.[WallMaterial] as [HouseWallMaterial],
	station.[Name] as [ClosestSubwayName],
	hou.[ClosestSubwayStationRange],
	hou.[Latitude] as [HouseLatitude],
	hou.[Longitude] as [HouseLongitude],
	ag.[AgentPhone] as [SellerPhone],
	ag.[Name] as [AgencyName],
	(select top 1 pht.[FileName] from [N1FlatPhoto] pht where pht.[FlatId] = uf.[Id] and pht.[RelationType] = 'RentFlat') as [FirstPhotoFile],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [N1RentFlat] uftmp ), 1, 0)
	) as [IsArchived]
from [N1RentFlat] uf 
inner join [N1HouseInfo] hou on uf.N1HouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [N1Agency] ag on uf.N1AgencyId = ag.Id));


-- 29.07.2021 Подсчет периода окупаемости еще и по N1, а не только по УПН
EXEC sp_rename 
@objname = '[PaybackPeriodPoint].[UpnHouseId]', 
@newname = 'HouseId', 
@objtype = 'COLUMN';

ALTER TABLE [PaybackPeriodPoint] ADD [SiteName] nvarchar(64);

UPDATE [PaybackPeriodPoint] SET [SiteName] = 'upn';
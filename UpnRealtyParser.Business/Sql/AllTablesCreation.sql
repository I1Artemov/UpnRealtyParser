CREATE TABLE [UpnHouseInfo] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] timestamp,
    [Address] nvarchar(max),
    [HouseType] nvarchar(256),
    [BuildYear] int,
    [WallMaterial] nvarchar(256),
    [MaxFloor] int,
    [Latitude] float,
    [Longitude] float
);

CREATE TABLE [UpnAgency] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] timestamp,
    [Name] nvarchar(max),
    [WorkTime] nvarchar(max),
    [CompanyPhone] nvarchar(max),
    [AgentPhone] nvarchar(max),
    [SiteUrl] nvarchar(max),
    [Email] nvarchar(max)
);

CREATE TABLE [UpnFlat] (
    [Id] int IDENTITY(1,1),
    [RemovalDate] timestamp,
    [CreationDateTime] timestamp,
    [PageCreationDateTime] timestamp,
    [LastCheckDate] timestamp,
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
    CONSTRAINT FK_UpnFlat_UpnHouseInfo_HouseInfoId FOREIGN KEY ([UpnHouseInfoId])
        REFERENCES [UpnHouseInfo]([Id]),
    CONSTRAINT FK_UpnFlat_UpnAgency_UpnAgencyId FOREIGN KEY ([UpnAgencyId])
        REFERENCES [UpnAgency]([Id]),
);

CREATE INDEX idx_UpnFlat_UpnHouseInfoId ON [UpnFlat] ([UpnHouseInfoId]);
CREATE INDEX idx_UpnFlat_UpnAgencyId ON [UpnFlat] ([UpnAgencyId]);

CREATE TABLE [UpnFlatPhoto] (
    [Id] int IDENTITY(1,1),
    [CreationDateTime] timestamp,
    [RelationType] nvarchar(64),
    [FileName] nvarchar(max),
    [FlatId] int -- Без Constraint
);

CREATE INDEX idx_UpnFlatPhoto_FlatId ON [UpnFlatPhoto] ([FlatId]);
CREATE INDEX idx_UpnFlatPhoto_RelationType ON [UpnFlatPhoto] ([RelationType]);

-- TODO: Сделать UpnRentFlat
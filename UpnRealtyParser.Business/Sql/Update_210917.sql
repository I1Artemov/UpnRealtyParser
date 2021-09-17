-- 17.09.2021 Включения фото дома и лет окупаемости во вьюшки с квартирами на продажу
drop view [vUpnFlatAdditional];

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
	(select top 1 houpht.[FileName] from [HousePhoto] houpht where houpht.[UpnHouseId] = uf.[UpnHouseInfoId]) as [HousePhotoFile],
	(select top 1 payb.[PaybackPeriod] from [ApartmentPayback] payb where payb.[FlatId] = uf.[Id] and payb.[Site] = 'Upn') as [PaybackYears],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [UpnFlat] uftmp ), 1, 0)
	) as [IsArchived]
from [UpnFlat] uf 
inner join [UpnHouseInfo] hou on uf.UpnHouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [UpnAgency] ag on uf.UpnAgencyId = ag.Id));

drop view [vN1FlatAdditional];

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
	(select top 1 houpht.[FileName] from [HousePhoto] houpht where houpht.[UpnHouseId] = 
		(select top 1 [UpnHouseId] from [SimilarHouse] simhou where simhou.[N1HouseId] = uf.[N1HouseInfoId])
	) as [HousePhotoFile],
	(select top 1 payb.[PaybackPeriod] from [ApartmentPayback] payb where payb.[FlatId] = uf.[Id] and payb.[Site] = 'N1') as [PaybackYears],
	(
	IIF(DATEADD(DAY, 7, uf.[LastCheckDate]) < (select max(uftmp.[LastCheckDate]) from [N1Flat] uftmp ), 1, 0)
	) as [IsArchived]
from [N1Flat] uf 
inner join [N1HouseInfo] hou on uf.N1HouseInfoId = hou.Id
inner join [SubwayStation] station on station.Id = hou.ClosestSubwayStationId
inner join [N1Agency] ag on uf.N1AgencyId = ag.Id));
select top 100 * from [ParsingState] ps order by ps.CreationDateTime desc;

delete from [UpnFlatPhoto]  where FlatId = 0;

select count(*) from [WebProxyInfo] pr where pr.LastUseDateTime is null;

select count(*) from [WebProxyInfo] pr where pr.LastSuccessDateTime is not null;

--update [WebProxyInfo] set LastSuccessDateTime = NULL where Id >= 1058 AND Id <= 1163;

select uf.Price, uf.SpaceSum, uf.[Description], pl.Href from [UpnFlat] uf 
LEFT JOIN [PageLink] pl ON uf.PageLinkId = pl.Id
where uf.[UpnHouseInfoId] IN (
	select h.Id from [UpnHouseInfo] h where h.ClosestSubwayStationRange < 2000
)
and uf.[FlatType] = 'Квартира'
and uf.Price < 2000000
order by uf.CreationDateTime desc;


select * from [N1Flat] fl
left join [N1HouseInfo] hou
on fl.N1HouseInfoId = hou.Id
where hou.IsFilledCompletely is null
and fl.IsFilledCompletely is not null;


select pgl.[Href], count(*) from [N1FlatPhoto] pgl
group by pgl.[Href]
having count(*) > 1;

-- Удаление дублирующихся ссылок на фото
WITH cte AS (
    SELECT 
        [Href],
		[FlatId],
        ROW_NUMBER() OVER (
            PARTITION BY 
                [FlatId], 
                [Href]
            ORDER BY 
                [FlatId], 
                [Href]
        ) row_num
     FROM 
        [N1FlatPhoto]
)
DELETE FROM cte
WHERE row_num > 1;

-- Тестовый запрос подсчета периода окупаемости
with houseIds as (
	select distinct hou.[Id] from [UpnRentFlat] urf
	left join [UpnHouseInfo] hou on urf.[UpnHouseInfoId] = hou.[Id]
)
select 
	urf.[UpnHouseInfoId],
	( 
		select top 1 [Address] from [UpnHouseInfo] hou where hou.Id = urf.[UpnHouseInfoId]
	) as [HouseAddress],
	AVG(urf.[Price]) as AverageRent,
	count(urf.[UpnHouseInfoId]) as RentFlatsAmount,
	( select top 1 AVG(usf.[Price]) 
	  from [UpnFlat] usf
	  where usf.UpnHouseInfoId = urf.[UpnHouseInfoId] and usf.RoomAmount = 1
	) as AverageSell,
	(  
		( select top 1 AVG(usf2.[Price]) from [UpnFlat] usf2
		  where usf2.UpnHouseInfoId = urf.[UpnHouseInfoId] and usf2.RoomAmount = 1
		) * 1.0 / (AVG(urf.[Price]) * 12.0 )
	) as Ratio
from [UpnRentFlat] urf
where urf.[UpnHouseInfoId] in (select * from houseIds)
	and [FlatType] = 'Квартира'
	and [RoomAmount] = 1
group by urf.[UpnHouseInfoId]
having AVG(urf.[Price]) < 100000
order by Ratio;

-- 09.09.2021 Пример расчета окупаемости по квартирам
select top 1000 
* ,
(
	select usf.Price / (sum(urf.Price) * 12.0 / count(*))
	from [UpnRentFlat] urf
	where urf.UpnHouseInfoId = usf.UpnHouseInfoId and urf.RoomAmount = usf.RoomAmount
) as [PaybackPeriod]
from [UpnFlat] usf
where usf.RoomAmount = 1
order by usf.[CreationDateTime] desc;
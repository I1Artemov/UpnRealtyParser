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
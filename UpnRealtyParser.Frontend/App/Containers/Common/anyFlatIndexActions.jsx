/**
 * Добавляет к "хвосту" URL параметры поиска квартир из объекта filteringInfo
 * @param {string} queryTrailer - исходный URL GET-запроса выборки квартир
 * @param {object} filteringInfo - объект с параметрами отбора квартир
 */
export function getQueryTrailerWithFilteringParameters(queryTrailer, filteringInfo) {

    if (filteringInfo.isShowArchived !== null && filteringInfo.isShowArchived !== undefined)
        queryTrailer += '&isShowArchived=' + filteringInfo.isShowArchived;
    if (filteringInfo.minPrice !== null && filteringInfo.minPrice !== undefined)
        queryTrailer += '&minPrice=' + filteringInfo.minPrice;
    if (filteringInfo.maxPrice !== null && filteringInfo.maxPrice !== undefined)
        queryTrailer += '&maxPrice=' + filteringInfo.maxPrice;
    if (filteringInfo.isExcludeFirstFloor !== null && filteringInfo.isExcludeFirstFloor !== undefined)
        queryTrailer += '&isExcludeFirstFloor=' + filteringInfo.isExcludeFirstFloor;
    if (filteringInfo.isExcludeLastFloor !== null && filteringInfo.isExcludeLastFloor !== undefined)
        queryTrailer += '&isExcludeLastFloor=' + filteringInfo.isExcludeLastFloor;
    if (filteringInfo.minBuildYear !== null && filteringInfo.minBuildYear !== undefined)
        queryTrailer += '&minBuildYear=' + filteringInfo.minBuildYear;
    if (filteringInfo.maxSubwayDistance !== null && filteringInfo.maxSubwayDistance !== undefined)
        queryTrailer += '&maxSubwayDistance=' + filteringInfo.maxSubwayDistance;
    if (filteringInfo.closestSubwayStationId !== null && filteringInfo.closestSubwayStationId !== undefined)
        queryTrailer += '&closestSubwayStationId=' + filteringInfo.closestSubwayStationId;
    if (filteringInfo.addressPart !== null && filteringInfo.addressPart !== undefined)
        queryTrailer += '&addressPart=' + filteringInfo.addressPart;

    return queryTrailer;
}
import {
    GET_ALL_FLATS_SUCCESS,
    GET_ALL_FLATS_ERROR,
    GET_ALL_FLATS_LOADING_IN_PROGRESS,
    SAVE_PAGING_PARAMETERS
} from '../UpnSellFlatIndex/upnSellFlatIndexConstants.jsx';
import "isomorphic-fetch";

/**
 * Добавляет к "хвосту" URL параметры поиска квартир из объекта filteringInfo
 * @param {string} queryTrailer исходный URL GET-запроса выборки квартир
 * @param {object} filteringInfo объект с параметрами отбора квартир
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
    if (filteringInfo.isShowRooms !== null && filteringInfo.isShowRooms !== undefined)
        queryTrailer += '&isShowRooms=' + filteringInfo.isShowRooms;
    if (filteringInfo.startDate !== null && filteringInfo.startDate !== undefined)
        queryTrailer += '&startDate=' + getDateFormattedWithSeparator(filteringInfo.startDate);
    if (filteringInfo.endDate !== null && filteringInfo.endDate !== undefined)
        queryTrailer += '&endDate=' + getDateFormattedWithSeparator(filteringInfo.endDate);

    return queryTrailer;
}

/**
 * Делает из даты Moment.js строку формата "год-месяц-день"
 */
function getDateFormattedWithSeparator(dateObj) {
    return dateObj.format("YYYY-MM-DD");
}

/**
 * Добавляет к "хвосту" URL параметры для постраничного вывода записей в гриде
 * @param {string} queryTrailer исходный URL GET-запроса выборки
 * @param {object} pagination объект с информацией о пейджинге грида
 */
export function getQueryTrailerWithPagingParameters(queryTrailer, pagination) {
    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    queryTrailer = queryTrailer + '?page=' + targetPage + '&pageSize=' + pageSize;

    return queryTrailer;
}

/**
 * Добавляет к "хвосту" URL параметры для сортировки записей в гриде
 * @param {string} queryTrailer исходный URL GET-запроса выборки квартир
 * @param {object} sorting объект с информацией о сортировке
 */
export function getQueryTrailerWithSortingParameters(queryTrailer, sorting) {
    if (sorting == null || sorting == undefined) {
        return queryTrailer;
    }

    if (sorting.field !== null && sorting.field !== undefined) queryTrailer += '&sortField=' + sorting.field;
    if (sorting.order !== null && sorting.order !== undefined) queryTrailer += '&sortOrder=' + sorting.order;

    return queryTrailer;
}

/** Для редьюсера - старт загрузки квартир с сервера */
export function startReceivingFlats() {
    return {
        type: GET_ALL_FLATS_LOADING_IN_PROGRESS
    };
}

/** Для редьюсера - обработка успешной загрузки квартир с сервера */
export function receiveAllFlats(data) {
    return {
        type: GET_ALL_FLATS_SUCCESS,
        flatsInfo: data.flatsList,
        totalFlatsCount: data.totalCount
    };
}

/** Для редьюсера - обработка ошибки при загрузке квартир с сервера */
export function errorReceiveAllFlats(err) {
    return {
        type: GET_ALL_FLATS_ERROR,
        error: err
    };
}

/**
 * Обработчик сохранения параметров постраничного вывода в состоянии
 * @param {object} pagination
 */
export function savePagingParameters(pagination) {
    return {
        type: SAVE_PAGING_PARAMETERS,
        payload: {
            current: pagination.current,
            pageSize: pagination.pageSize
        }
    }
}

/**
 * Асинхронная функция загрузки квартир с сервера
 * @param {object} pagination объект с информацией о постраничном выводе
 * @param {object} sorting объект с информацией о сортировке
 * @param {object} filteringInfo объект с информацией о фильтрации
 * @param {string} baseUrl URL GET-метода на сервере, возвращающего квартиры
 */
export function getAllFlats(pagination, sorting, filteringInfo, baseUrl) {
    return (dispatch) => {
        let queryTrailer = getQueryTrailerWithPagingParameters('', pagination);
        queryTrailer = getQueryTrailerWithFilteringParameters(queryTrailer, filteringInfo);
        queryTrailer = getQueryTrailerWithSortingParameters(queryTrailer, sorting);

        fetch(baseUrl + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllFlats(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllFlats(ex));
            });
    };
}
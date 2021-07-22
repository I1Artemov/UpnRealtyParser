import {
    GET_ALL_FLATS_SUCCESS,
    GET_ALL_FLATS_ERROR,
    GET_ALL_FLATS_LOADING_IN_PROGRESS,
    SAVE_PAGING_PARAMETERS
} from './upnSellFlatIndexConstants.jsx';

import { Href_UpnSellFlatController_GetAllFlats } from "../../const.jsx";
import "isomorphic-fetch";

export function startReceivingFlats() {
    return {
        type: GET_ALL_FLATS_LOADING_IN_PROGRESS
    };
}

export function receiveAllFlats(data) {
    return {
        type: GET_ALL_FLATS_SUCCESS,
        flatsInfo: data.flatsList,
        totalFlatsCount: data.totalCount
    };
}

export function errorReceiveAllFlats(err) {
    return {
        type: GET_ALL_FLATS_ERROR,
        error: err
    };
}

export function savePagingParameters(pagination) {
    return {
        type: SAVE_PAGING_PARAMETERS,
        payload: {
            current: pagination.current,
            pageSize: pagination.pageSize
        }
    }
}

export function getAllFlats(pagination, sorting, filteringInfo) {

    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

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

        if (sorting !== null && sorting !== undefined) {
            if (sorting.field !== null && sorting.field !== undefined) queryTrailer += '&sortField=' + sorting.field;
            if (sorting.order !== null && sorting.order !== undefined) queryTrailer += '&sortOrder=' + sorting.order;
        }

        fetch(Href_UpnSellFlatController_GetAllFlats + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllFlats(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllFlats(ex));
            });
    };
}
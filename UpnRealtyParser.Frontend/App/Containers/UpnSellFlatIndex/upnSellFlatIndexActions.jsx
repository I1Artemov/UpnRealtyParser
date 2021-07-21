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

export function getAllFlats(pagination, sorting, isShowArchived, minPrice, maxPrice, isExcludeFirstFloor, isExcludeLastFloor,
    minBuildYear, maxSubwayDistance, closestSubwayStationId, addressPart) {

    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

        if (isShowArchived !== null && isShowArchived !== undefined) queryTrailer += '&isShowArchived=' + isShowArchived;
        if (minPrice !== null && minPrice !== undefined) queryTrailer += '&minPrice=' + minPrice;
        if (maxPrice !== null && maxPrice !== undefined) queryTrailer += '&maxPrice=' + maxPrice;
        if (isExcludeFirstFloor !== null && isExcludeFirstFloor !== undefined) queryTrailer += '&isExcludeFirstFloor=' + isExcludeFirstFloor;
        if (isExcludeLastFloor !== null && isExcludeLastFloor !== undefined) queryTrailer += '&isExcludeLastFloor=' + isExcludeLastFloor;
        if (minBuildYear !== null && minBuildYear !== undefined) queryTrailer += '&minBuildYear=' + minBuildYear;
        if (maxSubwayDistance !== null && maxSubwayDistance !== undefined) queryTrailer += '&maxSubwayDistance=' + maxSubwayDistance;
        if (closestSubwayStationId !== null && closestSubwayStationId !== undefined) queryTrailer += '&closestSubwayStationId=' + closestSubwayStationId;
        if (addressPart !== null && addressPart !== undefined) queryTrailer += '&addressPart=' + addressPart;
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
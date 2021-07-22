import {
    GET_ALL_FLATS_SUCCESS,
    GET_ALL_FLATS_ERROR,
    GET_ALL_FLATS_LOADING_IN_PROGRESS
} from '../UpnSellFlatIndex/upnSellFlatIndexConstants.jsx';

import { Href_UpnRentFlatController_GetAllFlats } from "../../const.jsx";
import { getQueryTrailerWithFilteringParameters } from '../Common/anyFlatIndexActions.jsx';
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

export function getAllFlats(pagination, sorting, filteringInfo) {
    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

        queryTrailer = getQueryTrailerWithFilteringParameters(queryTrailer, filteringInfo);

        if (sorting !== null && sorting !== undefined) {
            if (sorting.field !== null && sorting.field !== undefined) queryTrailer += '&sortField=' + sorting.field;
            if (sorting.order !== null && sorting.order !== undefined) queryTrailer += '&sortOrder=' + sorting.order;
        }

        fetch(Href_UpnRentFlatController_GetAllFlats + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllFlats(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllFlats(ex));
            });
    };
}
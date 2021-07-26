import {
    GET_ALL_PROXIES_SUCCESS,
    GET_ALL_PROXIES_ERROR,
    GET_ALL_PROXIES_LOADING_IN_PROGRESS
} from './webProxyIndexConstants.jsx';

import { Href_WebProxyController_GetAllProxies } from "../../const.jsx";
import { getQueryTrailerWithPagingParameters, getQueryTrailerWithSortingParameters } from '../Common/anyFlatIndexActions.jsx';
import "isomorphic-fetch";

export function startReceivingProxies() {
    return {
        type: GET_ALL_PROXIES_LOADING_IN_PROGRESS
    };
}

export function receiveAllProxies(data) {
    return {
        type: GET_ALL_PROXIES_SUCCESS,
        proxiesInfo: data.proxiesList,
        totalProxiesCount: data.totalCount
    };
}

export function errorReceiveAllProxies(err) {
    return {
        type: GET_ALL_PROXIES_ERROR,
        error: err
    };
}

export function getAllProxies(pagination, sorting) {
    return (dispatch) => {
        let queryTrailer = getQueryTrailerWithPagingParameters('', pagination);
        queryTrailer = getQueryTrailerWithSortingParameters(queryTrailer, sorting);

        fetch(Href_WebProxyController_GetAllProxies + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllProxies(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllProxies(ex));
            });
    };
}
import {
    GET_ALL_PROXIES_SUCCESS,
    GET_ALL_PROXIES_ERROR,
    GET_ALL_PROXIES_LOADING_IN_PROGRESS
} from './webProxyIndexConstants.jsx';

import { Href_WebProxyController_GetAllProxies } from "../../const.jsx";
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

    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;
        if (sorting !== null && sorting !== undefined) {
            if (sorting.field !== null && sorting.field !== undefined) queryTrailer += '&sortField=' + sorting.field;
            if (sorting.order !== null && sorting.order !== undefined) queryTrailer += '&sortOrder=' + sorting.order;
        }

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
import {
    GET_FLAT_SUCCESS,
    GET_FLAT_ERROR,
    GET_FLAT_IN_PROGRESS
} from './upnSellFlatReadConstants.jsx';

import { Href_UpnSellFlatController_GetSingleFlat } from "../../const.jsx";
import "isomorphic-fetch";

export function startReceivingFlat() {
    return {
        type: GET_FLAT_IN_PROGRESS
    };
}

export function receiveFlat(data) {
    return {
        type: GET_FLAT_SUCCESS,
        flatInfo: data.flatInfo
    };
}

export function errorReceiveFlat(err) {
    return {
        type: GET_FLAT_ERROR,
        error: err
    };
}

export function getFlat(id) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id;

        fetch(Href_UpnSellFlatController_GetSingleFlat + queryTrailer)
            .then((response) => {
                var parsedJson = response.json();
                return parsedJson;
            }).then((data) => {
                dispatch(receiveFlat(data));
            }).catch((ex) => {
                dispatch(errorReceiveFlat(ex));
            });
    };
}
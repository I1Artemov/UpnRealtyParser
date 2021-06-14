import {
    GET_POINTS_SUCCESS,
    GET_POINTS_ERROR,
    GET_POINTS_IN_PROGRESS,
    SET_PAYBACK_LIMIT
} from './paybackMapConstants.jsx';
import { Href_HouseController_GetPaybackMapPoints } from "../../const.jsx";

import "isomorphic-fetch";

export function setPaybackLimit(ev) {
    let paybackLimit = ev;
    return {
        type: SET_PAYBACK_LIMIT,
        payload: paybackLimit
    };
}

function startReceivingPoints() {
    return {
        type: GET_POINTS_IN_PROGRESS
    };
}

export function receivePoints(data) {
    return {
        type: GET_POINTS_SUCCESS,
        points: data.points
    };
}

export function errorReceivePoints(err) {
    return {
        type: GET_POINTS_ERROR,
        error: err
    };
}

export function getAllPoints(paybackLimit) {
    let queryTrailer = '';
    if (paybackLimit !== null && paybackLimit !== undefined) queryTrailer += '?paybackLimit=' + paybackLimit;

    return (dispatch) => {
        dispatch(startReceivingPoints());
        fetch(Href_HouseController_GetPaybackMapPoints + queryTrailer)
            .then((response) => {
                var parsedJson = response.json();
                return parsedJson;
            }).then((data) => {
                dispatch(receivePoints(data));
            }).catch((ex) => {
                dispatch(errorReceivePoints(ex));
            });
    };
}
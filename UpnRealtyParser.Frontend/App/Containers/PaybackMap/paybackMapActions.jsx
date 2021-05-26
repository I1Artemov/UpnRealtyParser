import {
    GET_POINTS_SUCCESS,
    GET_POINTS_ERROR,
    GET_POINTS_IN_PROGRESS
} from './paybackMapConstants.jsx';
import { Href_HouseController_GetPaybackMapPoints } from "../../const.jsx";

import "isomorphic-fetch";

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

export function getAllPoints() {

    return (dispatch) => {
        dispatch(startReceivingPoints());
        fetch(Href_HouseController_GetPaybackMapPoints)
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
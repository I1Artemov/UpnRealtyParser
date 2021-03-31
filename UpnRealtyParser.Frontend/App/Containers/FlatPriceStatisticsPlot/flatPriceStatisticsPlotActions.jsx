import {
    GET_POINTS_ALLROOM_SUCCESS,
    GET_POINTS_ERROR,
    GET_ALLROOM_POINTS_IN_PROGRESS
    } from './flatPriceStatisticsPlotConstants.jsx';

import { Href_UpnHouseController_GetSingleHousePricePlotPoints } from "../../const.jsx";
import "isomorphic-fetch";

function startReceivingRoomFlatPrices() {

    return {
        type: GET_ALLROOM_POINTS_IN_PROGRESS
    };
}

function receiveFlatPrices(data) {
    return {
        type: GET_POINTS_ALLROOM_SUCCESS,
        points: data.points
    };
}

function errorReceiveFlatPrices(err) {
    return {
        type: GET_POINTS_ERROR,
        error: err
    };
}

export function getAvgPrices(houseId) {
    return (dispatch) => {
        let queryTrailer = '?id=' + houseId;

        dispatch(startReceivingRoomFlatPrices());
        fetch(Href_UpnHouseController_GetSingleHousePricePlotPoints + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveFlatPrices(data));
            }).catch((ex) => {
                dispatch(errorReceiveFlatPrices(ex));
            });
    };
}
﻿import { flatPricePlotConst } from './flatPriceStatisticsPlotConstants.jsx';

import { Href_HouseController_GetSingleHousePricePlotPoints } from "../../const.jsx";
import "isomorphic-fetch";

function startReceivingRoomFlatPrices() {

    return {
        type: flatPricePlotConst.GET_ALLROOM_POINTS_IN_PROGRESS
    };
}

function receiveFlatPrices(data) {
    return {
        type: flatPricePlotConst.GET_POINTS_ALLROOM_SUCCESS,
        points: data.points
    };
}

function errorReceiveFlatPrices(err) {
    return {
        type: flatPricePlotConst.GET_POINTS_ERROR,
        error: err
    };
}

export function getAvgPrices(houseId, siteName) {
    return (dispatch) => {
        let queryTrailer = '?id=' + houseId + '&siteName=' + siteName;

        dispatch(startReceivingRoomFlatPrices());
        fetch(Href_HouseController_GetSingleHousePricePlotPoints + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveFlatPrices(data));
            }).catch((ex) => {
                dispatch(errorReceiveFlatPrices(ex));
            });
    };
}
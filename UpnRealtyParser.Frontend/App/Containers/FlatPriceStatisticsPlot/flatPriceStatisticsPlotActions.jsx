import {
    GET_POINTS_1ROOM_SUCCESS,
    GET_POINTS_2ROOM_SUCCESS,
    GET_POINTS_3ROOM_SUCCESS,
    GET_POINTS_4ROOM_SUCCESS,
    GET_POINTS_ERROR,
    GET_1ROOM_POINTS_IN_PROGRESS,
    GET_MULTIROOM_POINTS_IN_PROGRESS
    } from './flatPriceStatisticsPlotConstants.jsx';

import { Href_UpnHouseController_GetSingleHousePricePlotPoints } from "../../const.jsx";
import "isomorphic-fetch";

function startReceiving1RoomFlatPrices() {

    return {
        type: GET_1ROOM_POINTS_IN_PROGRESS
    };
}

function receiveFlatPrices(data, roomAmount) {
    var roomConst = "";
    switch (roomAmount) {
        case 1: roomConst = GET_POINTS_1ROOM_SUCCESS; break;
        case 2: roomConst = GET_POINTS_2ROOM_SUCCESS; break;
        case 3: roomConst = GET_POINTS_3ROOM_SUCCESS; break;
        case 4: roomConst = GET_POINTS_4ROOM_SUCCESS; break;
    }

    return {
        type: roomConst,
        points: data.points
    };
}

function errorReceiveFlatPrices(err) {
    return {
        type: GET_POINTS_ERROR,
        error: err
    };
}

export function getAvgPrices(houseId, roomAmount) {
    return (dispatch) => {
        let queryTrailer = '?id=' + houseId + '&roomAmount=' + roomAmount;

        dispatch(startReceiving1RoomFlatPrices());
        fetch(Href_UpnHouseController_GetSingleHousePricePlotPoints + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveFlatPrices(data, roomAmount));
            }).catch((ex) => {
                dispatch(errorReceiveFlatPrices(ex));
            });
    };
}
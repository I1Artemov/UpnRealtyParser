import { houseReadConst } from '../Common/anyHouseReadConstants.jsx';

import "isomorphic-fetch";

function startReceivingHouse() {
    return {
        type: houseReadConst.GET_HOUSE_IN_PROGRESS
    };
}

export function receiveHouse(data) {
    return {
        type: houseReadConst.GET_HOUSE_SUCCESS,
        houseInfo: data.houseInfo
    };
}

export function errorReceiveHouse(err) {
    return {
        type: houseReadConst.GET_HOUSE_ERROR,
        error: err
    };
}

function startReceivingStatistics() {
    return {
        type: houseReadConst.GET_STATISTICS_IN_PROGRESS
    };
}

export function receiveStatistics(data) {
    return {
        type: houseReadConst.GET_STATISTICS_SUCCESS,
        houseStatistics: data.houseStatistics
    };
}

export function errorReceiveStatistics(err) {
    return {
        type: houseReadConst.GET_STATISTICS_ERROR,
        error: err
    };
}

export function getHouse(id, getHouseHref, siteName) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id + '&siteName=' + siteName;

        dispatch(startReceivingHouse());
        fetch(getHouseHref + queryTrailer)
            .then((response) => {
                var parsedJson = response.json();
                return parsedJson;
            }).then((data) => {
                dispatch(receiveHouse(data));
            }).catch((ex) => {
                dispatch(errorReceiveHouse(ex));
            });
    };
}

export function getStatistics(id, getStatisticsHref, siteName) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id + '&siteName=' + siteName;

        dispatch(startReceivingStatistics());
        fetch(getStatisticsHref + queryTrailer)
            .then((response) => {
                var parsedJson = response.json();
                return parsedJson;
            }).then((data) => {
                dispatch(receiveStatistics(data));
            }).catch((ex) => {
                dispatch(errorReceiveStatistics(ex));
            });
    };
}
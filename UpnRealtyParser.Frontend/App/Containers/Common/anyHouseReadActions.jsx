﻿import {
    GET_HOUSE_SUCCESS,
    GET_HOUSE_ERROR,
    GET_HOUSE_IN_PROGRESS,
    GET_STATISTICS_SUCCESS,
    GET_STATISTICS_ERROR,
    GET_STATISTICS_IN_PROGRESS
} from '../Common/anyHouseReadConstants.jsx';

import "isomorphic-fetch";

function startReceivingHouse() {
    return {
        type: GET_HOUSE_IN_PROGRESS
    };
}

export function receiveHouse(data) {
    return {
        type: GET_HOUSE_SUCCESS,
        houseInfo: data.houseInfo
    };
}

export function errorReceiveHouse(err) {
    return {
        type: GET_HOUSE_ERROR,
        error: err
    };
}

function startReceivingStatistics() {
    return {
        type: GET_STATISTICS_IN_PROGRESS
    };
}

export function receiveStatistics(data) {
    return {
        type: GET_STATISTICS_SUCCESS,
        houseStatistics: data.houseStatistics
    };
}

export function errorReceiveStatistics(err) {
    return {
        type: GET_STATISTICS_ERROR,
        error: err
    };
}

export function getHouse(id, getHouseHref) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id;

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

export function getStatistics(id, getStatisticsHref) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id;

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
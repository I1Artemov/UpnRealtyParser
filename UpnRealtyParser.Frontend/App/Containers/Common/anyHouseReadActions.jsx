import {
    GET_HOUSE_SUCCESS,
    GET_HOUSE_ERROR,
    GET_HOUSE_IN_PROGRESS
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
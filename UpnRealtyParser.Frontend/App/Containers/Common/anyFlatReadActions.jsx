import { flatReadConst } from '../Common/anyFlatReadConstants.jsx';

import "isomorphic-fetch";

export function startReceivingFlat() {
    return {
        type: flatReadConst.GET_FLAT_IN_PROGRESS
    };
}

export function showFlatPhotos() {
    return {
        type: flatReadConst.SHOW_PHOTOS
    };
}

export function hideFlatPhotos() {
    return {
        type: flatReadConst.HIDE_PHOTOS
    };
}

export function receiveFlat(data) {
    return {
        type: flatReadConst.GET_FLAT_SUCCESS,
        flatInfo: data.flatInfo
    };
}

export function errorReceiveFlat(err) {
    return {
        type: flatReadConst.GET_FLAT_ERROR,
        error: err
    };
}

export function getFlat(id, getFlatHref) {

    return (dispatch) => {
        let queryTrailer = '?id=' + id;

        fetch(getFlatHref + queryTrailer)
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
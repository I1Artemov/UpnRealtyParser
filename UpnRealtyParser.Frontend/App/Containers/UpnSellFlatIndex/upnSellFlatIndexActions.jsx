import { GET_ALL_FLATS_SUCCESS, GET_ALL_FLATS_ERROR } from './upnSellFlatIndexConstants.jsx';
import { Href_UpnSellFlatController_GetAllFlats } from "../../const.jsx";
import "isomorphic-fetch";

export function receiveAllFlats(data) {
    return {
        type: GET_ALL_FLATS_SUCCESS,
        flatsInfo: data
    };
}

export function errorReceiveAllFlats(err) {
    return {
        type: GET_ALL_FLATS_ERROR,
        error: err
    };
}

export function getAllFlats(pageNumber = 0) {
    return (dispatch) => {
        let queryTrailer = '?page=' + pageNumber;

        fetch(Href_UpnSellFlatController_GetAllFlats + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllFlats(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllFlats(ex));
            });
    };
}
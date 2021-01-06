import {
    GET_ALL_HOUSES_SUCCESS,
    GET_ALL_HOUSES_ERROR,
    GET_ALL_HOUSES_LOADING_IN_PROGRESS
} from './upnHouseIndexConstants.jsx';
import { Href_UpnHouseController_GetAllFlats } from "../../const.jsx";
import "isomorphic-fetch";

export function startReceivingHouses() {
    return {
        type: GET_ALL_HOUSES_LOADING_IN_PROGRESS
    };
}

export function receiveAllHouses(data) {
    return {
        type: GET_ALL_HOUSES_SUCCESS,
        housesInfo: data.housesList,
        totalHousesCount: data.totalCount
    };
}

export function errorReceiveAllHouses(err) {
    return {
        type: GET_ALL_HOUSES_ERROR,
        error: err
    };
}

export function getAllHouses(pagination) {
    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

        fetch(Href_UpnHouseController_GetAllFlats + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllHouses(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllHouses(ex));
            });
    };
}
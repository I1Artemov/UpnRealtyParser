import {
    GET_ALL_HOUSES_SUCCESS,
    GET_ALL_HOUSES_ERROR,
    GET_ALL_HOUSES_LOADING_IN_PROGRESS,
    SET_MIN_BUILD_YEAR,
    SET_IS_SHOW_UPN,
    SET_IS_SHOW_N1,
    SET_ADDRESS_PART,
    CLEAR_SEARCH_PARAMETERS
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

export function setMinBuildYear(ev) {
    let minYear = ev;
    return {
        type: SET_MIN_BUILD_YEAR,
        payload: minYear
    };
}

export function setIsShowUpn(ev) {
    let isShow = ev.target.checked;
    return {
        type: SET_IS_SHOW_UPN,
        payload: isShow
    };
}

export function setIsShowN1(ev) {
    let isShow = ev.target.checked;
    return {
        type: SET_IS_SHOW_N1,
        payload: isShow
    };
}

export function setAddressPart(ev) {
    let addressPart = ev.target.value;
    return {
        type: SET_ADDRESS_PART,
        payload: addressPart
    };
}

export function clearSearchParameters() {
    return {
        type: CLEAR_SEARCH_PARAMETERS
    };
}

export function getAllHouses(pagination, minBuildYear, isShowUpn, isShowN1, addressPart) {
    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;
        if (minBuildYear !== null && minBuildYear !== undefined) queryTrailer += '&minBuildYear=' + minBuildYear;
        if (isShowUpn !== null && isShowUpn !== undefined) queryTrailer += '&isShowUpn=' + isShowUpn;
        if (isShowN1 !== null && isShowN1 !== undefined) queryTrailer += '&isShowN1=' + isShowN1;
        if (addressPart !== null && addressPart !== undefined) queryTrailer += '&addressPart=' + addressPart;

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
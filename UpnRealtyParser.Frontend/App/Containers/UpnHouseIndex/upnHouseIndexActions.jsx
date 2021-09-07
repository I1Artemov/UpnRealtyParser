import { houseIndexConst } from './upnHouseIndexConstants.jsx';
import { Href_HouseController_GetAllFlats } from "../../const.jsx";
import { getQueryTrailerWithPagingParameters, getQueryTrailerWithSortingParameters } from '../Common/anyFlatIndexActions.jsx';
import "isomorphic-fetch";

export function startReceivingHouses() {
    return {
        type: houseIndexConst.GET_ALL_HOUSES_LOADING_IN_PROGRESS
    };
}

export function receiveAllHouses(data) {
    return {
        type: houseIndexConst.GET_ALL_HOUSES_SUCCESS,
        housesInfo: data.housesList,
        totalHousesCount: data.totalCount
    };
}

export function errorReceiveAllHouses(err) {
    return {
        type: houseIndexConst.GET_ALL_HOUSES_ERROR,
        error: err
    };
}

export function setMinBuildYear(ev) {
    let minYear = ev;
    return {
        type: houseIndexConst.SET_MIN_BUILD_YEAR,
        payload: minYear
    };
}

export function setIsShowUpn(ev) {
    let isShow = ev.target.checked;
    return {
        type: houseIndexConst.SET_IS_SHOW_UPN,
        payload: isShow
    };
}

export function setIsShowN1(ev) {
    let isShow = ev.target.checked;
    return {
        type: houseIndexConst.SET_IS_SHOW_N1,
        payload: isShow
    };
}

export function setAddressPart(ev) {
    let addressPart = ev.target.value;
    return {
        type: houseIndexConst.SET_ADDRESS_PART,
        payload: addressPart
    };
}

export function clearSearchParameters() {
    return {
        type: houseIndexConst.CLEAR_SEARCH_PARAMETERS
    };
}

export function getAllHouses(pagination, sorting, minBuildYear, isShowUpn, isShowN1, addressPart) {
    return (dispatch) => {
        let queryTrailer = getQueryTrailerWithPagingParameters('', pagination);

        if (minBuildYear !== null && minBuildYear !== undefined) queryTrailer += '&minBuildYear=' + minBuildYear;
        if (isShowUpn !== null && isShowUpn !== undefined) queryTrailer += '&isShowUpn=' + isShowUpn;
        if (isShowN1 !== null && isShowN1 !== undefined) queryTrailer += '&isShowN1=' + isShowN1;
        if (addressPart !== null && addressPart !== undefined) queryTrailer += '&addressPart=' + addressPart;

        queryTrailer = getQueryTrailerWithSortingParameters(queryTrailer, sorting);

        fetch(Href_HouseController_GetAllFlats + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllHouses(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllHouses(ex));
            });
    };
}
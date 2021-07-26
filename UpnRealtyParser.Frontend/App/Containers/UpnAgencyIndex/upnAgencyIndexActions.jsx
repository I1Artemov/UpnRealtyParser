import {
    GET_ALL_AGENCIES_SUCCESS,
    GET_ALL_AGENCIES_ERROR,
    GET_ALL_AGENCIES_LOADING_IN_PROGRESS
} from './upnAgencyIndexConstants.jsx';
import { Href_UpnAgencyController_GetAllAgencies } from "../../const.jsx";
import { getQueryTrailerWithPagingParameters } from '../Common/anyFlatIndexActions.jsx';
import "isomorphic-fetch";

export function startReceivingAgencies() {
    return {
        type: GET_ALL_AGENCIES_LOADING_IN_PROGRESS
    };
}

export function receiveAllAgencies(data) {
    return {
        type: GET_ALL_AGENCIES_SUCCESS,
        agenciesInfo: data.agenciesList,
        totalAgenciesCount: data.totalCount
    };
}

export function errorReceiveAllAgencies(err) {
    return {
        type: GET_ALL_AGENCIES_ERROR,
        error: err
    };
}

export function getAllAgencies(pagination) {
    return (dispatch) => {
        let queryTrailer = getQueryTrailerWithPagingParameters('', pagination);

        fetch(Href_UpnAgencyController_GetAllAgencies + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllAgencies(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllAgencies(ex));
            });
    };
}
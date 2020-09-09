import { GET_ALL_AGENCIES_SUCCESS, GET_ALL_AGENCIES_ERROR } from './upnAgencyIndexConstants.jsx';
import { Href_UpnAgencyController_GetAllAgencies } from "../../const.jsx";
import "isomorphic-fetch";

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
    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

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
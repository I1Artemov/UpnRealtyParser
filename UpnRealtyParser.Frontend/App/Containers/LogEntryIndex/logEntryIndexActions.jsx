import {
    GET_ALL_LOG_ENTRIES_SUCCESS,
    GET_ALL_LOG_ENTRIES_ERROR,
    GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS
} from './logEntryIndexConstants.jsx';

import { Href_ParsingStateController_GetAllStates } from "../../const.jsx";
import "isomorphic-fetch";

export function startReceivingLogEntries() {
    return {
        type: GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS
    };
}

export function receiveAllLogEntries(data) {
    return {
        type: GET_ALL_LOG_ENTRIES_SUCCESS,
        logEntryInfo: data.logEntriesList,
        totalLogEntriesCount: data.totalCount
    };
}

export function errorReceiveAllLogEntries(err) {
    return {
        type: GET_ALL_LOG_ENTRIES_ERROR,
        error: err
    };
}

export function getAllLogEntries(pagination) {

    let targetPage = !pagination.current ? 1 : pagination.current;
    let pageSize = !pagination.pageSize ? 10 : pagination.pageSize;

    return (dispatch) => {
        let queryTrailer = '?page=' + targetPage + '&pageSize=' + pageSize;

        fetch(Href_ParsingStateController_GetAllStates + queryTrailer)
            .then((response) => {
                return response.json();
            }).then((data) => {
                dispatch(receiveAllLogEntries(data));
            }).catch((ex) => {
                dispatch(errorReceiveAllLogEntries(ex));
            });
    };
}
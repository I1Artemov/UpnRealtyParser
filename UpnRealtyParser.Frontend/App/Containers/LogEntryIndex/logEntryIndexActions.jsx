import { logEntryIndexConst } from './logEntryIndexConstants.jsx';

import { Href_ParsingStateController_GetAllStates } from "../../const.jsx";
import { getQueryTrailerWithPagingParameters } from '../Common/anyFlatIndexActions.jsx';
import "isomorphic-fetch";

export function startReceivingLogEntries() {
    return {
        type: logEntryIndexConst.GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS
    };
}

export function receiveAllLogEntries(data) {
    return {
        type: logEntryIndexConst.GET_ALL_LOG_ENTRIES_SUCCESS,
        logEntryInfo: data.logEntriesList,
        totalLogEntriesCount: data.totalCount
    };
}

export function errorReceiveAllLogEntries(err) {
    return {
        type: logEntryIndexConst.GET_ALL_LOG_ENTRIES_ERROR,
        error: err
    };
}

export function getAllLogEntries(pagination) {
    return (dispatch) => {
        let queryTrailer = getQueryTrailerWithPagingParameters('', pagination);

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
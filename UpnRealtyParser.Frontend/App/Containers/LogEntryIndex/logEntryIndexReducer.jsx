import {
    GET_ALL_LOG_ENTRIES_SUCCESS,
    GET_ALL_LOG_ENTRIES_ERROR,
    GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS
} from './logEntryIndexConstants.jsx';

const initialState = {
    logEntryInfo: [{ id: 0, description: '' }],
    isLogEntriesLoading: false,
    totalLogEntriesCount: 0,
    error: ""
};

export default function proxies(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS:
            return { ...state, isLogEntriesLoading: true };

        case GET_ALL_LOG_ENTRIES_SUCCESS:
            return { ...state, logEntryInfo: action.logEntryInfo, totalLogEntriesCount: action.totalLogEntriesCount, error: '', isLogEntriesLoading: false };

        case GET_ALL_LOG_ENTRIES_ERROR:
            return { ...state, error: action.error, isLogEntriesLoading: false };

        default:
            return state;
    }
}
import {
    GET_ALL_AGENCIES_SUCCESS,
    GET_ALL_AGENCIES_ERROR,
    GET_ALL_AGENCIES_LOADING_IN_PROGRESS
} from './upnAgencyIndexConstants.jsx';

const initialState = {
    agenciesInfo: [{ id: 0, name: '' }],
    totalAgenciedCount: 0,
    isAgenciesLoading: false,
    error: ""
};

export default function agencies(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_AGENCIES_LOADING_IN_PROGRESS:
            return { ...state, isAgenciesLoading: true };

        case GET_ALL_AGENCIES_SUCCESS:
            return { ...state, agenciesInfo: action.agenciesInfo, totalAgenciesCount: action.totalAgenciesCount, error: '', isAgenciesLoading: false };

        case GET_ALL_AGENCIES_ERROR:
            return { ...state, error: action.error, isAgenciesLoading: false };

        default:
            return state;
    }
}
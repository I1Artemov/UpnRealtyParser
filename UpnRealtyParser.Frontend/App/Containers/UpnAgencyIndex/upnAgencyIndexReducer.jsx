import { GET_ALL_AGENCIES_SUCCESS, GET_ALL_AGENCIES_ERROR } from './upnAgencyIndexConstants.jsx';

const initialState = {
    agenciesInfo: [{ id: 0, name: '' }],
    totalAgenciedCount: 0,
    error: ""
};

export default function agencies(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_AGENCIES_SUCCESS:
            return { ...state, agenciesInfo: action.agenciesInfo, totalAgenciesCount: action.totalAgenciesCount, error: '' };

        case GET_ALL_AGENCIES_ERROR:
            return { ...state, error: action.error };

        default:
            return state;
    }
}
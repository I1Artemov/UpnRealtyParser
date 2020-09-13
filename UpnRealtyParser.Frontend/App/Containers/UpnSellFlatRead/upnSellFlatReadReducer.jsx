import {
    GET_FLAT_SUCCESS,
    GET_FLAT_ERROR,
    GET_FLAT_IN_PROGRESS
} from './upnSellFlatReadConstants.jsx';

const initialState = {
    flatInfo: [{ id: 0, description: '' }],
    isLoading: false,
    error: ""
};

export default function flat(state = initialState, action) {
    switch (action.type) {
        case GET_FLAT_IN_PROGRESS:
            return { ...state, isLoading: true };

        case GET_FLAT_SUCCESS:
            return { ...state, flatInfo: action.flatInfo, error: '', isLoading: false };

        case GET_FLAT_ERROR:
            return { ...state, error: action.error, isLoading: false };

        default:
            return state;
    }
}
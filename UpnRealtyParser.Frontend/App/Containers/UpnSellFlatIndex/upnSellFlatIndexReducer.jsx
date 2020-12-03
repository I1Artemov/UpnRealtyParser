import {
    GET_ALL_FLATS_SUCCESS,
    GET_ALL_FLATS_ERROR,
    GET_ALL_FLATS_LOADING_IN_PROGRESS,
    SET_SHOW_ARCHIVED
} from './upnSellFlatIndexConstants.jsx';

const initialState = {
    flatsInfo: [{ id: 0, description: '' }],
    isFlatsLoading: false,
    totalFlatsCount: 0,
    isShowArchived: true,
    error: ""
};

export default function flats(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_FLATS_LOADING_IN_PROGRESS:
            return { ...state, isFlatsLoading: true };

        case GET_ALL_FLATS_SUCCESS:
            return { ...state, flatsInfo: action.flatsInfo, totalFlatsCount: action.totalFlatsCount, error: '', isFlatsLoading: false  };

        case GET_ALL_FLATS_ERROR:
            return { ...state, error: action.error, isFlatsLoading: false };

        case SET_SHOW_ARCHIVED:
            return { ...state, isShowArchived: action.payload };

        default:
            return state;
    }
}
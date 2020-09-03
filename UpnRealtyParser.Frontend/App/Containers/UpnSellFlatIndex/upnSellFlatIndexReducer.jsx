import { GET_ALL_FLATS_SUCCESS, GET_ALL_FLATS_ERROR } from './upnSellFlatIndexConstants.jsx';

const initialState = {
    flatsInfo: [{ id: 0, description: '' }],
    error: ""
};

export default function flats(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_FLATS_SUCCESS:
            return { ...state, flatsInfo: action.flatsInfo, error: '' };

        case GET_ALL_FLATS_ERROR:
            return { ...state, error: action.error };

        default:
            return state;
    }
}
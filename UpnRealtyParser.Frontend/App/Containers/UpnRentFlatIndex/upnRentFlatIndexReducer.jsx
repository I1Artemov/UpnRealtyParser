import { sellFlatIndexConst } from '../UpnSellFlatIndex/upnSellFlatIndexConstants.jsx';

const initialState = {
    flatsInfo: [{ id: 0, description: '' }],
    isFlatsLoading: false,
    totalFlatsCount: 0,
    error: ""
};

export default function flats(state = initialState, action) {
    switch (action.type) {
        case sellFlatIndexConst.GET_ALL_FLATS_LOADING_IN_PROGRESS:
            return { ...state, isFlatsLoading: true };

        case sellFlatIndexConst.GET_ALL_FLATS_SUCCESS:
            return { ...state, flatsInfo: action.flatsInfo, totalFlatsCount: action.totalFlatsCount, error: '', isFlatsLoading: false };

        case sellFlatIndexConst.GET_ALL_FLATS_ERROR:
            return { ...state, error: action.error, isFlatsLoading: false };

        default:
            return state;
    }
}
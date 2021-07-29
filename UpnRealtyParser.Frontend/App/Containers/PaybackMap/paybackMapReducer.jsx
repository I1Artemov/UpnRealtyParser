import {
    GET_POINTS_SUCCESS,
    GET_POINTS_ERROR,
    GET_POINTS_IN_PROGRESS,
    SET_PAYBACK_LIMIT,
    SET_USE_UPN_DATA,
    SET_USE_N1_DATA
} from './paybackMapConstants.jsx';

const initialState = {
    points: [{ houseId: 0, latitude: 0, longitude: 0 }],
    isLoading: false,
    paybackLimit: null,
    error: "",
    isUseUpnData: true,
    isUseN1Data: true
};

export default function payback(state = initialState, action) {
    switch (action.type) {
        case GET_POINTS_IN_PROGRESS:
            return { ...state, isLoading: true };

        case GET_POINTS_SUCCESS:
            return { ...state, points: action.points, error: '', isLoading: false };

        case GET_POINTS_ERROR:
            return { ...state, error: action.error, isLoading: false };

        case SET_PAYBACK_LIMIT:
            return { ...state, paybackLimit: action.payload };

        case SET_USE_UPN_DATA:
            return { ...state, isUseUpnData: action.payload };

        case SET_USE_N1_DATA:
            return { ...state, isUseN1Data: action.payload };

        default:
            return state;
    }
}
import {
    GET_POINTS_SUCCESS,
    GET_POINTS_ERROR,
    GET_POINTS_IN_PROGRESS,
    SET_PAYBACK_LIMIT
} from './paybackMapConstants.jsx';

const initialState = {
    points: [{ upnHouseId: 0, latitude: 0, longitude: 0 }],
    isLoading: false,
    paybackLimit: null,
    error: ""
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
            return {...state, paybackLimit: action.payload};

        default:
            return state;
    }
}
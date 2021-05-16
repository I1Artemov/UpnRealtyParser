import {
    GET_POINTS_SUCCESS,
    GET_POINTS_ERROR,
    GET_POINTS_IN_PROGRESS
} from './paybackMapConstants.jsx';

const initialState = {
    points: [{ upnHouseId: 0, latitude: 0, longitude: 0 }],
    isLoading: false,
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

        default:
            return state;
    }
}
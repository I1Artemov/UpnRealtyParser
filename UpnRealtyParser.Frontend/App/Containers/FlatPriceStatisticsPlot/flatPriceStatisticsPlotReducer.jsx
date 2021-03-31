import {
    GET_POINTS_ALLROOM_SUCCESS,
    GET_POINTS_ERROR,
    GET_ALLROOM_POINTS_IN_PROGRESS,
    } from './flatPriceStatisticsPlotConstants.jsx';

const initialState = {
    allRoomPrices: [{ xAxis: null, yAxis: 0 }],
    isInfoLoading: false,
    error: ""
};

export default function points(state = initialState, action) {
    switch (action.type) {
        case GET_ALLROOM_POINTS_IN_PROGRESS:
            return { ...state, isInfoLoading: true };

        case GET_POINTS_ALLROOM_SUCCESS:
            return { ...state, allRoomPrices: action.points, error: '', isInfoLoading: false };

        case GET_POINTS_ERROR:
            return { ...state, error: action.error, isInfoLoading: false};

        default:
            return state;
    }
}
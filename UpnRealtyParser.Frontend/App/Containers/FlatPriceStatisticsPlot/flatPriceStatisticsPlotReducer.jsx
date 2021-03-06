import {
    GET_POINTS_1ROOM_SUCCESS,
    GET_POINTS_2ROOM_SUCCESS,
    GET_POINTS_3ROOM_SUCCESS,
    GET_POINTS_4ROOM_SUCCESS,
    GET_POINTS_ERROR,
    GET_1ROOM_POINTS_IN_PROGRESS,
    GET_MULTIROOM_POINTS_IN_PROGRESS
    } from './flatPriceStatisticsPlotConstants.jsx';

const initialState = {
    singleRoomPrices: [{ xAxis: null, yAxis: 0 }],
    twoRoomPrices: [{ xAxis: null, yAxis: 0 }],
    threeRoomPrices: [{ xAxis: null, yAxis: 0 }],
    fourRoomPrices: [{ xAxis: null, yAxis: 0 }],
    isBaseInfoLoading: false,
    isAdditionalInfoLoading: false,
    error: ""
};

export default function points(state = initialState, action) {
    switch (action.type) {
        case GET_1ROOM_POINTS_IN_PROGRESS:
            return { ...state, isBaseInfoLoading: true };

        case GET_MULTIROOM_POINTS_IN_PROGRESS:
            return { ...state, isAdditionalInfoLoading: true };

        case GET_POINTS_1ROOM_SUCCESS:
            return { ...state, singleRoomPrices: action.points, error: '', isBaseInfoLoading: false };

        case GET_POINTS_2ROOM_SUCCESS:
            return { ...state, twoRoomPrices: action.points, error: '' };

        case GET_POINTS_3ROOM_SUCCESS:
            return { ...state, threeRoomPrices: action.points, error: '' };

        case GET_POINTS_4ROOM_SUCCESS:
            return { ...state, fourRoomPrices: action.points, error: '', isAdditionalInfoLoading: false };

        case GET_POINTS_ERROR:
            return { ...state, error: action.error, isBaseInfoLoading: false, isAdditionalInfoLoading: false };

        default:
            return state;
    }
}
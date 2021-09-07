import { flatPricePlotConst } from './flatPriceStatisticsPlotConstants.jsx';

const initialState = {
    allRoomPrices: [{ xAxis: null, yAxis: 0 }],
    isInfoLoading: false,
    error: "",
    siteName: "upn"
};

export default function points(state = initialState, action) {
    switch (action.type) {
        case flatPricePlotConst.GET_ALLROOM_POINTS_IN_PROGRESS:
            return { ...state, isInfoLoading: true };

        case flatPricePlotConst.GET_POINTS_ALLROOM_SUCCESS:
            return { ...state, allRoomPrices: action.points, error: '', isInfoLoading: false };

        case flatPricePlotConst.GET_POINTS_ERROR:
            return { ...state, error: action.error, isInfoLoading: false};

        default:
            return state;
    }
}
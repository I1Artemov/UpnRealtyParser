import { houseReadConst } from '../Common/anyHouseReadConstants.jsx';

const initialState = {
    houseInfo: [{ id: 0, description: '' }],
    houseStatistics: null,
    isLoading: false,
    isStatisticsLoading: false,
    error: "",
    siteName: "upn"
};

export default function house(state = initialState, action) {
    switch (action.type) {
        case houseReadConst.GET_HOUSE_IN_PROGRESS:
            return { ...state, isLoading: true };

        case houseReadConst.GET_HOUSE_SUCCESS:
            return { ...state, houseInfo: action.houseInfo, error: '', isLoading: false };

        case houseReadConst.GET_HOUSE_ERROR:
            return { ...state, error: action.error, isLoading: false };

        case houseReadConst.GET_STATISTICS_IN_PROGRESS:
            return { ...state, isStatisticsLoading: true };

        case houseReadConst.GET_STATISTICS_SUCCESS:
            return { ...state, houseStatistics: action.houseStatistics, error: '', isStatisticsLoading: false };

        case houseReadConst.GET_STATISTICS_ERROR:
            return { ...state, error: action.error, isStatisticsLoading: false };

        default:
            return state;
    }
}
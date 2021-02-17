import {
    GET_HOUSE_SUCCESS,
    GET_HOUSE_ERROR,
    GET_HOUSE_IN_PROGRESS,
    GET_STATISTICS_SUCCESS,
    GET_STATISTICS_ERROR,
    GET_STATISTICS_IN_PROGRESS
} from '../Common/anyHouseReadConstants.jsx';

const initialState = {
    houseInfo: [{ id: 0, description: '' }],
    houseStatistics: null,
    isLoading: false,
    isStatisticsLoading: false,
    error: ""
};

export default function house(state = initialState, action) {
    switch (action.type) {
        case GET_HOUSE_IN_PROGRESS:
            return { ...state, isLoading: true };

        case GET_HOUSE_SUCCESS:
            return { ...state, houseInfo: action.houseInfo, error: '', isLoading: false };

        case GET_HOUSE_ERROR:
            return { ...state, error: action.error, isLoading: false };

        case GET_STATISTICS_IN_PROGRESS:
            return { ...state, isStatisticsLoading: true };

        case GET_STATISTICS_SUCCESS:
            return { ...state, houseStatistics: action.houseStatistics, error: '', isStatisticsLoading: false };

        case GET_STATISTICS_ERROR:
            return { ...state, error: action.error, isStatisticsLoading: false };

        default:
            return state;
    }
}
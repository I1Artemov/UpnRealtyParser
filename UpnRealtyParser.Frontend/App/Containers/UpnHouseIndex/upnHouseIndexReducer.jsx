import {
    GET_ALL_HOUSES_SUCCESS,
    GET_ALL_HOUSES_ERROR,
    GET_ALL_HOUSES_LOADING_IN_PROGRESS
} from './upnHouseIndexConstants.jsx';

const initialState = {
    housesInfo: [{ id: 0, address: '' }],
    totalHousesCount: 0,
    isHousesLoading: false,
    error: ""
};

export default function houses(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_HOUSES_LOADING_IN_PROGRESS:
            return { ...state, isHousesLoading: true };

        case GET_ALL_HOUSES_SUCCESS:
            return { ...state, housesInfo: action.housesInfo, totalHousesCount: action.totalHousesCount, error: '', isHousesLoading: false };

        case GET_ALL_HOUSES_ERROR:
            return { ...state, error: action.error, isHousesLoading: false };

        default:
            return state;
    }
}
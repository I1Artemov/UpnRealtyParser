import { GET_ALL_HOUSES_SUCCESS, GET_ALL_HOUSES_ERROR } from './upnHouseIndexConstants.jsx';

const initialState = {
    housesInfo: [{ id: 0, address: '' }],
    totalHousesCount: 0,
    error: ""
};

export default function houses(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_HOUSES_SUCCESS:
            return { ...state, housesInfo: action.housesInfo, totalHousesCount: action.totalHousesCount, error: '' };

        case GET_ALL_HOUSES_ERROR:
            return { ...state, error: action.error };

        default:
            return state;
    }
}
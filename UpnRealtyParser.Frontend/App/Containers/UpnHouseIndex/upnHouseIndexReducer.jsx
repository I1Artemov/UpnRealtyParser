import {
    GET_ALL_HOUSES_SUCCESS,
    GET_ALL_HOUSES_ERROR,
    GET_ALL_HOUSES_LOADING_IN_PROGRESS,
    SET_MIN_BUILD_YEAR,
    SET_IS_SHOW_UPN,
    SET_IS_SHOW_N1,
    SET_ADDRESS_PART,
    CLEAR_SEARCH_PARAMETERS
} from './upnHouseIndexConstants.jsx';

const initialState = {
    housesInfo: [{ id: 0, address: '' }],
    totalHousesCount: 0,
    isHousesLoading: false,
    minBuildYear: null,
    isShowUpn: true,
    isShowN1: true,
    addressPart: null,

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

        case SET_MIN_BUILD_YEAR:
            return { ...state, minBuildYear: action.payload };

        case SET_IS_SHOW_UPN:
            return { ...state, isShowUpn: action.payload };

        case SET_IS_SHOW_N1:
            return { ...state, isShowN1: action.payload };

        case SET_ADDRESS_PART:
            return { ...state, addressPart: action.payload };

        case CLEAR_SEARCH_PARAMETERS:
            return { ...state, minBuildYear: null, isShowUpn: true, isShowN1: true, addressPart: null};

        default:
            return state;
    }
}
import {
    GET_ALL_FLATS_SUCCESS,
    GET_ALL_FLATS_ERROR,
    GET_ALL_FLATS_LOADING_IN_PROGRESS,
    SET_SHOW_ARCHIVED,
    SET_EXCLUDE_FIRST_FLOOR,
    SET_EXCLUDE_LAST_FLOOR,
    SET_MIN_PRICE,
    SET_MAX_PRICE,
    SET_MIN_BUILD_YEAR
} from './upnSellFlatIndexConstants.jsx';

const initialState = {
    flatsInfo: [{ id: 0, description: '' }],
    isFlatsLoading: false,
    totalFlatsCount: 0,
    isShowArchived: true,
    isExcludeFirstFloor: false,
    isExcludeLastFloor: false,
    minPrice: 0,
    maxPrice: 40000,
    minBuildYear: 1900,
    error: ""
};

export default function flats(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_FLATS_LOADING_IN_PROGRESS:
            return { ...state, isFlatsLoading: true };

        case GET_ALL_FLATS_SUCCESS:
            return { ...state, flatsInfo: action.flatsInfo, totalFlatsCount: action.totalFlatsCount, error: '', isFlatsLoading: false  };

        case GET_ALL_FLATS_ERROR:
            return { ...state, error: action.error, isFlatsLoading: false };

        case SET_SHOW_ARCHIVED:
            return { ...state, isShowArchived: action.payload };

        case SET_EXCLUDE_FIRST_FLOOR:
            return { ...state, isExcludeFirstFloor: action.payload };

        case SET_EXCLUDE_LAST_FLOOR:
            return { ...state, isExcludeLastFloor: action.payload };

        case SET_MIN_PRICE:
            return { ...state, minPrice: action.payload };

        case SET_MAX_PRICE:
            return { ...state, maxPrice: action.payload };

        case SET_MIN_BUILD_YEAR:
            return { ...state, minBuildYear: action.payload };

        default:
            return state;
    }
}
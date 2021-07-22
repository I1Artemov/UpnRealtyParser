import {
    SET_SHOW_ARCHIVED,
    SET_EXCLUDE_FIRST_FLOOR,
    SET_EXCLUDE_LAST_FLOOR,
    SET_MIN_PRICE,
    SET_MAX_PRICE,
    SET_MIN_BUILD_YEAR,
    SET_MAX_SUBWAY_DISTANCE,
    SET_CLOSEST_SUBWAY_STATION_ID,
    SET_ADDRESS_PART,
    CLEAR_SEARCH_PARAMETERS
} from './flatsSearchBarConstants.jsx';

const initialState = {
    filteringInfo: {
        isShowArchived: true,
        isExcludeFirstFloor: false,
        isExcludeLastFloor: false,
        minPrice: null,
        maxPrice: null,
        minBuildYear: null,
        maxSubwayDistance: null,
        closestSubwayStationId: null,
        addressPart: null
    }
};

export default function searchBar(state = initialState, action) {
    switch (action.type) {
        case SET_SHOW_ARCHIVED:
            return { ...state, filteringInfo: { ...state.filteringInfo, isShowArchived: action.payload } };

        case SET_EXCLUDE_FIRST_FLOOR:
            return { ...state, filteringInfo: { ...state.filteringInfo, isExcludeFirstFloor: action.payload } };

        case SET_EXCLUDE_LAST_FLOOR:
            return { ...state, filteringInfo: { ...state.filteringInfo, isExcludeLastFloor: action.payload } };

        case SET_MIN_PRICE:
            return { ...state, filteringInfo: { ...state.filteringInfo, minPrice: action.payload } };

        case SET_MAX_PRICE:
            return { ...state, filteringInfo: { ...state.filteringInfo, maxPrice: action.payload } };

        case SET_MIN_BUILD_YEAR:
            return { ...state, filteringInfo: { ...state.filteringInfo, minBuildYear: action.payload } };

        case SET_MAX_SUBWAY_DISTANCE:
            return { ...state, filteringInfo: { ...state.filteringInfo, maxSubwayDistance: action.payload } };

        case SET_CLOSEST_SUBWAY_STATION_ID:
            return { ...state, filteringInfo: { ...state.filteringInfo, closestSubwayStationId: action.payload } };

        case SET_ADDRESS_PART:
            return { ...state, filteringInfo: { ...state.filteringInfo, addressPart: action.payload } };

        case CLEAR_SEARCH_PARAMETERS:
            return {
                ...state, filteringInfo: {
                    ...state.filteringInfo, isShowArchived: true, isExcludeFirstFloor: false, isExcludeLastFloor: false,
                    minPrice: null, maxPrice: null, minBuildYear: null, maxSubwayDistance: null, closestSubwayStationId: null,
                    addressPart: null
                }
            };

        default:
            return state;
    }
}
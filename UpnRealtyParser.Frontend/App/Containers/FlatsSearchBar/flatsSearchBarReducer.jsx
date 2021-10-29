import moment from "moment";
import { flatsSearchBarConst } from './flatsSearchBarConstants.jsx';

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
        addressPart: null,
        isShowRooms: true,
        startDate: null,
        endDate: null,
        maxPayback: null,
        descriptionPart: null
    }
};

export default function searchBar(state = initialState, action) {
    switch (action.type) {
        case flatsSearchBarConst.SET_SHOW_ARCHIVED:
            return { ...state, filteringInfo: { ...state.filteringInfo, isShowArchived: action.payload } };

        case flatsSearchBarConst.SET_EXCLUDE_FIRST_FLOOR:
            return { ...state, filteringInfo: { ...state.filteringInfo, isExcludeFirstFloor: action.payload } };

        case flatsSearchBarConst.SET_EXCLUDE_LAST_FLOOR:
            return { ...state, filteringInfo: { ...state.filteringInfo, isExcludeLastFloor: action.payload } };

        case flatsSearchBarConst.SET_MIN_PRICE:
            return { ...state, filteringInfo: { ...state.filteringInfo, minPrice: action.payload } };

        case flatsSearchBarConst.SET_MAX_PRICE:
            return { ...state, filteringInfo: { ...state.filteringInfo, maxPrice: action.payload } };

        case flatsSearchBarConst.SET_MIN_BUILD_YEAR:
            return { ...state, filteringInfo: { ...state.filteringInfo, minBuildYear: action.payload } };

        case flatsSearchBarConst.SET_MAX_SUBWAY_DISTANCE:
            return { ...state, filteringInfo: { ...state.filteringInfo, maxSubwayDistance: action.payload } };

        case flatsSearchBarConst.SET_CLOSEST_SUBWAY_STATION_ID:
            return { ...state, filteringInfo: { ...state.filteringInfo, closestSubwayStationId: action.payload } };

        case flatsSearchBarConst.SET_ADDRESS_PART:
            return { ...state, filteringInfo: { ...state.filteringInfo, addressPart: action.payload } };

        case flatsSearchBarConst.SET_SHOW_ROOMS:
            return { ...state, filteringInfo: { ...state.filteringInfo, isShowRooms: action.payload } };

        case flatsSearchBarConst.SET_START_DATE:
            return { ...state, filteringInfo: { ...state.filteringInfo, startDate: action.payload } };

        case flatsSearchBarConst.SET_END_DATE:
            return { ...state, filteringInfo: { ...state.filteringInfo, endDate: action.payload } };

        case flatsSearchBarConst.SET_MAX_PAYBACK:
            return { ...state, filteringInfo: { ...state.filteringInfo, maxPayback: action.payload } };

        case flatsSearchBarConst.SET_DESCRIPTION_PART:
            return { ...state, filteringInfo: { ...state.filteringInfo, descriptionPart: action.payload } };

        case flatsSearchBarConst.CLEAR_SEARCH_PARAMETERS:
            return {
                ...state, filteringInfo: {
                    ...state.filteringInfo, isShowArchived: true, isExcludeFirstFloor: false, isExcludeLastFloor: false,
                    minPrice: null, maxPrice: null, minBuildYear: null, maxSubwayDistance: null, closestSubwayStationId: null,
                    addressPart: null, isShowRooms: true, startDate: null, endDate: null, maxPayback: null, descriptionPart: null
                }
            };

        default:
            return state;
    }
}
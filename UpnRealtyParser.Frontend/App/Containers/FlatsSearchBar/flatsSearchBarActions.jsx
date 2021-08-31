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
    SET_SHOW_ROOMS,
    SET_START_DATE,
    SET_END_DATE,
    CLEAR_SEARCH_PARAMETERS
} from './flatsSearchBarConstants.jsx';

export function setShowArchived(ev) {
    let isShow = ev.target.checked;
    return {
        type: SET_SHOW_ARCHIVED,
        payload: isShow
    };
}

export function setExcludeFirstFloor(ev) {
    let isExclude = ev.target.checked;
    return {
        type: SET_EXCLUDE_FIRST_FLOOR,
        payload: isExclude
    };
}

export function setExcludeLastFloor(ev) {
    let isExclude = ev.target.checked;
    return {
        type: SET_EXCLUDE_LAST_FLOOR,
        payload: isExclude
    };
}

export function setMinPrice(ev) {
    let minPrice = ev;
    return {
        type: SET_MIN_PRICE,
        payload: minPrice
    };
}

export function setMaxPrice(ev) {
    let maxPrice = ev;
    return {
        type: SET_MAX_PRICE,
        payload: maxPrice
    };
}

export function setMinBuildYear(ev) {
    let minYear = ev;
    return {
        type: SET_MIN_BUILD_YEAR,
        payload: minYear
    };
}

export function setMaxSubwayDistance(ev) {
    let maxDistance = ev;
    return {
        type: SET_MAX_SUBWAY_DISTANCE,
        payload: maxDistance
    };
}

export function setClosestSubwayStationId(ev) {
    let stationId = ev;
    return {
        type: SET_CLOSEST_SUBWAY_STATION_ID,
        payload: stationId
    };
}

export function setAddressPart(ev) {
    let addressPart = ev.target.value;
    return {
        type: SET_ADDRESS_PART,
        payload: addressPart
    };
}

export function setShowRooms(ev) {
    let isShow = ev.target.checked;
    return {
        type: SET_SHOW_ROOMS,
        payload: isShow
    };
}

export function setStartDate(ev) {
    let dt = ev;
    return {
        type: SET_START_DATE,
        payload: dt
    };
}

export function setEndDate(ev) {
    let dt = ev;
    return {
        type: SET_END_DATE,
        payload: dt
    };
}

export function clearSearchParameters() {
    return {
        type: CLEAR_SEARCH_PARAMETERS
    };
}
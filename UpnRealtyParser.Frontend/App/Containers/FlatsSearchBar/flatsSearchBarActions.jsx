import { flatsSearchBarConst } from './flatsSearchBarConstants.jsx';

export function setShowArchived(ev) {
    let isShow = ev.target.checked;
    return {
        type: flatsSearchBarConst.SET_SHOW_ARCHIVED,
        payload: isShow
    };
}

export function setExcludeFirstFloor(ev) {
    let isExclude = ev.target.checked;
    return {
        type: flatsSearchBarConst.SET_EXCLUDE_FIRST_FLOOR,
        payload: isExclude
    };
}

export function setExcludeLastFloor(ev) {
    let isExclude = ev.target.checked;
    return {
        type: flatsSearchBarConst.SET_EXCLUDE_LAST_FLOOR,
        payload: isExclude
    };
}

export function setMinPrice(ev) {
    let minPrice = ev;
    return {
        type: flatsSearchBarConst.SET_MIN_PRICE,
        payload: minPrice
    };
}

export function setMaxPrice(ev) {
    let maxPrice = ev;
    return {
        type: flatsSearchBarConst.SET_MAX_PRICE,
        payload: maxPrice
    };
}

export function setMinBuildYear(ev) {
    let minYear = ev;
    return {
        type: flatsSearchBarConst.SET_MIN_BUILD_YEAR,
        payload: minYear
    };
}

export function setMaxSubwayDistance(ev) {
    let maxDistance = ev;
    return {
        type: flatsSearchBarConst.SET_MAX_SUBWAY_DISTANCE,
        payload: maxDistance
    };
}

export function setClosestSubwayStationId(ev) {
    let stationId = ev;
    return {
        type: flatsSearchBarConst.SET_CLOSEST_SUBWAY_STATION_ID,
        payload: stationId
    };
}

export function setAddressPart(ev) {
    let addressPart = ev.target.value;
    return {
        type: flatsSearchBarConst.SET_ADDRESS_PART,
        payload: addressPart
    };
}

export function setShowRooms(ev) {
    let isShow = ev.target.checked;
    return {
        type: flatsSearchBarConst.SET_SHOW_ROOMS,
        payload: isShow
    };
}

export function setStartDate(ev) {
    let dt = ev;
    return {
        type: flatsSearchBarConst.SET_START_DATE,
        payload: dt
    };
}

export function setEndDate(ev) {
    let dt = ev;
    return {
        type: flatsSearchBarConst.SET_END_DATE,
        payload: dt
    };
}

export function clearSearchParameters() {
    return {
        type: flatsSearchBarConst.CLEAR_SEARCH_PARAMETERS
    };
}
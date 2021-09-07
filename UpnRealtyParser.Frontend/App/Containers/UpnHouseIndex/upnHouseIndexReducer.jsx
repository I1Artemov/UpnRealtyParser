import { houseIndexConst } from './upnHouseIndexConstants.jsx';

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
        case houseIndexConst.GET_ALL_HOUSES_LOADING_IN_PROGRESS:
            return { ...state, isHousesLoading: true };

        case houseIndexConst.GET_ALL_HOUSES_SUCCESS:
            return { ...state, housesInfo: action.housesInfo, totalHousesCount: action.totalHousesCount, error: '', isHousesLoading: false };

        case houseIndexConst.GET_ALL_HOUSES_ERROR:
            return { ...state, error: action.error, isHousesLoading: false };

        case houseIndexConst.SET_MIN_BUILD_YEAR:
            return { ...state, minBuildYear: action.payload };

        case houseIndexConst.SET_IS_SHOW_UPN:
            return { ...state, isShowUpn: action.payload };

        case houseIndexConst.SET_IS_SHOW_N1:
            return { ...state, isShowN1: action.payload };

        case houseIndexConst.SET_ADDRESS_PART:
            return { ...state, addressPart: action.payload };

        case houseIndexConst.CLEAR_SEARCH_PARAMETERS:
            return { ...state, minBuildYear: null, isShowUpn: true, isShowN1: true, addressPart: null};

        default:
            return state;
    }
}
import {
    GET_HOUSE_SUCCESS,
    GET_HOUSE_ERROR,
    GET_HOUSE_IN_PROGRESS
} from '../Common/anyHouseReadConstants.jsx';

const initialState = {
    houseInfo: [{ id: 0, description: '' }],
    isLoading: false,
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

        default:
            return state;
    }
}
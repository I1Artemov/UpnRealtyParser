import { flatReadConst } from '../Common/anyFlatReadConstants.jsx';

const initialState = {
    flatInfo: [{ id: 0, description: '' }],
    isLoading: false,
    error: "",
    isShowApartmentPhotos: false
};

export default function flat(state = initialState, action) {
    switch (action.type) {
        case flatReadConst.GET_FLAT_IN_PROGRESS:
            return { ...state, isLoading: true };

        case flatReadConst.GET_FLAT_SUCCESS:
            return { ...state, flatInfo: action.flatInfo, error: '', isLoading: false };

        case flatReadConst.GET_FLAT_ERROR:
            return { ...state, error: action.error, isLoading: false };

        case flatReadConst.SHOW_PHOTOS:
            return { ...state, isShowApartmentPhotos: true };

        case flatReadConst.HIDE_PHOTOS:
            return { ...state, isShowApartmentPhotos: false };

        default:
            return state;
    }
}
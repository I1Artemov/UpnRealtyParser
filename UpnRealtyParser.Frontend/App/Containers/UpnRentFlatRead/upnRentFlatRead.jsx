import { connect } from 'react-redux';
import { getFlat, startReceivingFlat, showFlatPhotos, hideFlatPhotos } from '../Common/anyFlatReadActions.jsx';

import { Href_UpnRentFlatController_GetSingleFlat } from "../../const.jsx";

import AnyFlatRead from '../Common/anyFlatReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state) => {
    return {
        flatInfo: state.upnRentFlatReadReducer.flatInfo,
        error: state.upnRentFlatReadReducer.error,
        isLoading: state.upnRentFlatReadReducer.isLoading,
        isShowApartmentPhotos: state.upnRentFlatReadReducer.isShowApartmentPhotos,
        isRent: true
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getFlat: (id) => dispatch(getFlat(id, Href_UpnRentFlatController_GetSingleFlat)),
        startReceivingFlat: () => dispatch(startReceivingFlat()),
        showFlatPhotos: () => dispatch(showFlatPhotos()),
        hideFlatPhotos: () => dispatch(hideFlatPhotos())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyFlatRead);
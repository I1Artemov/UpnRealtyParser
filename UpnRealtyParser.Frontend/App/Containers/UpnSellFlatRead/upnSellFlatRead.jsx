﻿import { connect } from 'react-redux';
import { getFlat, startReceivingFlat, showFlatPhotos, hideFlatPhotos } from '../Common/anyFlatReadActions.jsx';

import { Href_UpnSellFlatController_GetSingleFlat } from "../../const.jsx";

import AnyFlatRead from '../Common/anyFlatReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state) => {
    return {
        flatInfo: state.upnSellFlatReadReducer.flatInfo,
        error: state.upnSellFlatReadReducer.error,
        isLoading: state.upnSellFlatReadReducer.isLoading,
        isShowApartmentPhotos: state.upnSellFlatReadReducer.isShowApartmentPhotos,
        isRent: false
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getFlat: (id) => dispatch(getFlat(id, Href_UpnSellFlatController_GetSingleFlat)),
        startReceivingFlat: () => dispatch(startReceivingFlat()),
        showFlatPhotos: () => dispatch(showFlatPhotos()),
        hideFlatPhotos: () => dispatch(hideFlatPhotos())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyFlatRead);
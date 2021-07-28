import { connect } from 'react-redux';
import { getFlat, startReceivingFlat, showFlatPhotos, hideFlatPhotos } from '../Common/anyFlatReadActions.jsx';

import { Href_UpnRentFlatController_GetSingleFlat } from "../../const.jsx";
import { Href_N1RentFlatController_GetSingleFlat } from "../../const.jsx";

import AnyFlatRead from '../Common/anyFlatReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state, ownProps) => {
    return {
        flatInfo: state.anyRentFlatReadReducer.flatInfo,
        error: state.anyRentFlatReadReducer.error,
        isLoading: state.anyRentFlatReadReducer.isLoading,
        isShowApartmentPhotos: state.anyRentFlatReadReducer.isShowApartmentPhotos,
        isRent: true,
        siteName: ownProps.siteName
    };
};

let mapActionsToProps = (dispatch, ownProps) => {
    let siteName = ownProps.siteName;
    let flatUrl = siteName === "upn" ? Href_UpnRentFlatController_GetSingleFlat
        : Href_N1RentFlatController_GetSingleFlat;

    return {
        getFlat: (id) => dispatch(getFlat(id, flatUrl)),
        startReceivingFlat: () => dispatch(startReceivingFlat()),
        showFlatPhotos: () => dispatch(showFlatPhotos()),
        hideFlatPhotos: () => dispatch(hideFlatPhotos())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyFlatRead);
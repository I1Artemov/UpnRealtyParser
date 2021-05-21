import { connect } from 'react-redux';
import { getFlat, startReceivingFlat, showFlatPhotos, hideFlatPhotos } from '../Common/anyFlatReadActions.jsx';

import { Href_UpnSellFlatController_GetSingleFlat } from "../../const.jsx";
import { Href_N1SellFlatController_GetSingleFlat } from "../../const.jsx";

import AnyFlatRead from '../Common/anyFlatReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state, ownProps) => {
    return {
        flatInfo: state.anySellFlatReadReducer.flatInfo,
        error: state.anySellFlatReadReducer.error,
        isLoading: state.anySellFlatReadReducer.isLoading,
        isShowApartmentPhotos: state.anySellFlatReadReducer.isShowApartmentPhotos,
        isRent: false,
        siteName: ownProps.siteName
    };
};

let mapActionsToProps = (dispatch, ownProps) => {
    let siteName = ownProps.siteName;
    let flatUrl = siteName === "upn" ? Href_UpnSellFlatController_GetSingleFlat
        : Href_N1SellFlatController_GetSingleFlat;
    
    return {
        getFlat: (id) => dispatch(getFlat(id, flatUrl)),
        startReceivingFlat: () => dispatch(startReceivingFlat()),
        showFlatPhotos: () => dispatch(showFlatPhotos()),
        hideFlatPhotos: () => dispatch(hideFlatPhotos())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyFlatRead);
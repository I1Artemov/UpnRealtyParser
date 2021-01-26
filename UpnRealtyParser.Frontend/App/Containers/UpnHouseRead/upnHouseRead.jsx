import { connect } from 'react-redux';
import { getHouse, startReceivingHouse } from '../Common/anyHouseReadActions.jsx';

import { Href_UpnHouseController_GetSingleHouse } from "../../const.jsx";

import AnyHouseRead from '../Common/anyHouseReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state) => {
    return {
        houseInfo: state.anyHouseReadReducer.houseInfo,
        error: state.anyHouseReadReducer.error,
        isLoading: state.anyHouseReadReducer.isLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getHouse: (id) => dispatch(getHouse(id, Href_UpnHouseController_GetSingleHouse))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyHouseRead);
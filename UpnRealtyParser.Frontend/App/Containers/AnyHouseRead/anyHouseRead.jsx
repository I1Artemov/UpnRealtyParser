import { connect } from 'react-redux';
import { getHouse, getStatistics } from '../Common/anyHouseReadActions.jsx';

import {
    Href_HouseController_GetSingleHouse,
    Href_HouseController_GetSingleHouseStatistics
} from "../../const.jsx";

import AnyHouseRead from '../Common/anyHouseReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state, ownProps) => {
    return {
        houseInfo: state.anyHouseReadReducer.houseInfo,
        houseStatistics: state.anyHouseReadReducer.houseStatistics,
        error: state.anyHouseReadReducer.error,
        isLoading: state.anyHouseReadReducer.isLoading,
        isStatisticsLoading: state.anyHouseReadReducer.isStatisticsLoading,
        siteName: ownProps.siteName
    };
};

let mapActionsToProps = (dispatch, ownProps) => {
    return {
        getHouse: (id) => dispatch(getHouse(id, Href_HouseController_GetSingleHouse, ownProps.siteName)),
        getStatistics: (id) => dispatch(getStatistics(id, Href_HouseController_GetSingleHouseStatistics, ownProps.siteName))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyHouseRead);
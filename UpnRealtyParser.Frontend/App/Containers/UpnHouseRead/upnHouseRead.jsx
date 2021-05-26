import { connect } from 'react-redux';
import { getHouse, getStatistics } from '../Common/anyHouseReadActions.jsx';

import {
    Href_HouseController_GetSingleHouse,
    Href_HouseController_GetSingleHouseStatistics
} from "../../const.jsx";

import AnyHouseRead from '../Common/anyHouseReadComponent.jsx';

import 'antd/dist/antd.css';

let mapStateToProps = (state) => {
    return {
        houseInfo: state.anyHouseReadReducer.houseInfo,
        houseStatistics: state.anyHouseReadReducer.houseStatistics,
        error: state.anyHouseReadReducer.error,
        isLoading: state.anyHouseReadReducer.isLoading,
        isStatisticsLoading: state.anyHouseReadReducer.isStatisticsLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getHouse: (id) => dispatch(getHouse(id, Href_HouseController_GetSingleHouse)),
        getStatistics: (id) => dispatch(getStatistics(id, Href_HouseController_GetSingleHouseStatistics))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(AnyHouseRead);
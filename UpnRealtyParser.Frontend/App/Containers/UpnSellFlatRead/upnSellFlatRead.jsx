import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getFlat, startReceivingFlat } from './upnSellFlatReadActions.jsx';
import { Divider, Spin } from 'antd';
import SingleFlatInfo from '../../Stateless/singleFlatInfo.jsx';

import 'antd/dist/antd.css';

class UpnSellFlatRead extends React.Component {
    componentDidMount() {
        const id = this.props.match.params.id;
        this.props.startReceivingFlat();
        this.props.getFlat(id);
    }

    render() {
        let flatData = this.props.flatInfo;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;

        if (isLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            return (
                <div>
                    <Divider orientation={"center"}>Информация о квартире на продажу, ID {flatData.id}</Divider>
                    <SingleFlatInfo flatData={flatData}/>
                </div>
            );
        } else {
            return (
                <div>
                    Ошибка загрузки квартиры: {this.props.error.toString()}
                </div>
            );
        }
    }
}

let mapStateToProps = (state) => {
    return {
        flatInfo: state.upnSellFlatReadReducer.flatInfo,
        error: state.upnSellFlatReadReducer.error,
        isLoading: state.upnSellFlatReadReducer.isLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getFlat: (id) => dispatch(getFlat(id)),
        startReceivingFlat: () => dispatch(startReceivingFlat())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatRead);
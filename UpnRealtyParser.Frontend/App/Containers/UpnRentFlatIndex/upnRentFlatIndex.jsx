import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats, startReceivingFlats } from './upnRentFlatIndexActions.jsx';
// Используем те же колонки, что и у квартир на продажу
import { SELL_FLATS_TABLE_COLUMNS } from '../UpnSellFlatIndex/upnSellFlatIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';

class UpnRentFlatIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingFlats();
        this.props.getAllFlats(new Object());
        // todo: bind
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingFlats();
        this.props.getAllFlats(pagination);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;
        let errorMessage = this.props.error;

        if (errorMessage !== undefined && errorMessage !== null && errorMessage !== "") {
            return (
                <div>
                    Ошибка загрузки квартиры: {errorMessage.toString()}
                </div>
                );
        } else {
            return (
                <Table
                    dataSource={flatsData}
                    columns={SELL_FLATS_TABLE_COLUMNS}
                    onChange={this.handleTableChange.bind(this)}
                    pagination={{ total: totalFlatsCount }}
                    loading={isFlatsLoading}
                />
            );
        }
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnRentFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnRentFlatIndexReducer.totalFlatsCount,
        error: state.upnRentFlatIndexReducer.error,
        isFlatsLoading: state.upnRentFlatIndexReducer.isFlatsLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination) => dispatch(getAllFlats(pagination)),
        startReceivingFlats: () => dispatch(startReceivingFlats())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnRentFlatIndex);
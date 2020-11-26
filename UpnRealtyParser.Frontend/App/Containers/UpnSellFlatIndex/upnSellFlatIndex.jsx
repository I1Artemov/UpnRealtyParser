import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats, startReceivingFlats } from './upnSellFlatIndexActions.jsx';
import { SELL_FLATS_TABLE_COLUMNS } from './upnSellFlatIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';

import 'antd/dist/antd.css';

class UpnSellFlatIndex extends React.Component {
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

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>Upn</Breadcrumb.Item>
                <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                <Breadcrumb.Item>На продажу</Breadcrumb.Item>
            </Breadcrumb>
            <Table
                dataSource={flatsData}
                columns={SELL_FLATS_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{total: totalFlatsCount}}
                loading={isFlatsLoading}
                />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnSellFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnSellFlatIndexReducer.totalFlatsCount,
        error: state.upnSellFlatIndexReducer.error,
        isFlatsLoading: state.upnSellFlatIndexReducer.isFlatsLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination) => dispatch(getAllFlats(pagination)),
        startReceivingFlats: () => dispatch(startReceivingFlats())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
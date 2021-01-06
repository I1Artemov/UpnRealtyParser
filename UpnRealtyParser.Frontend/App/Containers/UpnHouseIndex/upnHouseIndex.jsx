import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllHouses, startReceivingHouses } from './upnHouseIndexActions.jsx';
import { HOUSES_TABLE_COLUMNS } from './upnHouseIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';

import 'antd/dist/antd.css';

class UpnHouseIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingHouses();
        this.props.getAllHouses(new Object());
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingHouses();
        this.props.getAllHouses(pagination);
    }

    render() {
        let housesData = this.props.housesInfo.map(item => ({ ...item, key: item.id }));
        let totalHousesCount = this.props.totalHousesCount;
        let isHousesLoading = this.props.isHousesLoading;

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>Дома УПН и N1</Breadcrumb.Item>
            </Breadcrumb>
            <Table
                dataSource={housesData}
                columns={HOUSES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalHousesCount }}
                loading={isHousesLoading}
            />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        housesInfo: state.upnHouseIndexReducer.housesInfo,
        totalHousesCount: state.upnHouseIndexReducer.totalHousesCount,
        error: state.upnHouseIndexReducer.error,
        isHousesLoading: state.upnHouseIndexReducer.isHousesLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllHouses: (pagination) => dispatch(getAllHouses(pagination)),
        startReceivingHouses: () => dispatch(startReceivingHouses())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnHouseIndex);
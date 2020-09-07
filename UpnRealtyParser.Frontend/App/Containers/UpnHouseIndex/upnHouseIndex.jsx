import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllHouses } from './upnHouseIndexActions.jsx';
import { HOUSES_TABLE_COLUMNS } from './upnHouseIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';

class UpnHouseIndex extends React.Component {
    componentDidMount() {
        this.props.getAllHouses(new Object());
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.getAllHouses(pagination);
    }

    render() {
        let housesData = this.props.housesInfo.map(item => ({ ...item, key: item.id }));
        let totalHousesCount = this.props.totalHousesCount;

        return (
            <Table
                dataSource={housesData}
                columns={HOUSES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalHousesCount }}
            />
        );
    }
}

let mapStateToProps = (state) => {
    return {
        housesInfo: state.upnHouseIndexReducer.housesInfo,
        totalHousesCount: state.upnHouseIndexReducer.totalHousesCount,
        error: state.upnHouseIndexReducer.error
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllHouses: (pagination) => dispatch(getAllHouses(pagination))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnHouseIndex);
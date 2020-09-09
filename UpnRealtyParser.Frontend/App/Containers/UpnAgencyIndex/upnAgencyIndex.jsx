import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllAgencies } from './upnAgencyIndexActions.jsx';
import { AGENCIES_TABLE_COLUMNS } from './upnAgencyIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';

class UpnAgencyIndex extends React.Component {
    componentDidMount() {
        this.props.getAllAgencies(new Object());
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.getAllAgencies(pagination);
    }

    render() {
        let agenciesData = this.props.agenciesInfo.map(item => ({ ...item, key: item.id }));
        let totalAgenciesCount = this.props.totalAgenciesCount;

        return (
            <Table
                dataSource={agenciesData}
                columns={AGENCIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalAgenciesCount }}
            />
        );
    }
}

let mapStateToProps = (state) => {
    return {
        agenciesInfo: state.upnAgencyIndexReducer.agenciesInfo,
        totalAgenciesCount: state.upnAgencyIndexReducer.totalAgenciesCount,
        error: state.upnAgencyIndexReducer.error
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllAgencies: (pagination) => dispatch(getAllAgencies(pagination))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnAgencyIndex);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllAgencies, startReceivingAgencies } from './upnAgencyIndexActions.jsx';
import { AGENCIES_TABLE_COLUMNS } from './upnAgencyIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';

class UpnAgencyIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingAgencies();
        this.props.getAllAgencies(new Object());
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingAgencies();
        this.props.getAllAgencies(pagination);
    }

    render() {
        let agenciesData = this.props.agenciesInfo.map(item => ({ ...item, key: item.id }));
        let totalAgenciesCount = this.props.totalAgenciesCount;
        let isAgenciesLoading = this.props.isAgenciesLoading;

        return (
            <Table
                dataSource={agenciesData}
                columns={AGENCIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalAgenciesCount }}
                loading={isAgenciesLoading}
            />
        );
    }
}

let mapStateToProps = (state) => {
    return {
        agenciesInfo: state.upnAgencyIndexReducer.agenciesInfo,
        totalAgenciesCount: state.upnAgencyIndexReducer.totalAgenciesCount,
        error: state.upnAgencyIndexReducer.error,
        isAgenciesLoading: state.upnAgencyIndexReducer.isAgenciesLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllAgencies: (pagination) => dispatch(getAllAgencies(pagination)),
        startReceivingAgencies: () => dispatch(startReceivingAgencies())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnAgencyIndex);
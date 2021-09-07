import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllAgencies, startReceivingAgencies } from './upnAgencyIndexActions.jsx';
import { agencyIndexConst } from './upnAgencyIndexConstants.jsx';
import { Table, PageHeader } from 'antd';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class UpnAgencyIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingAgencies();
        this.props.getAllAgencies(new Object());
        document.title = SiteTitle + " - Агентства";
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
            <div>
                <PageHeader className="site-page-header" backIcon={false} onBack={() => null}
                    title="Агентства УПН и N1" />
            <Table
                dataSource={agenciesData}
                columns={agencyIndexConst.AGENCIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalAgenciesCount }}
                loading={isAgenciesLoading}
            />
            </div>
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
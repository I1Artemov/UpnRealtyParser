import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllProxies, startReceivingProxies } from './webProxyIndexActions.jsx';
import { PROXIES_TABLE_COLUMNS } from './webProxyIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';

class WebProxyIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingProxies();
        this.props.getAllProxies(new Object());
        // todo: bind
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingProxies();
        this.props.getAllProxies(pagination);
    }

    render() {
        let proxiesData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalProxiesCount = this.props.totalProxiesCount;
        let isProxiesLoading = this.props.isProxiesLoading;

        return (
            <Table
                dataSource={proxiesData}
                columns={PROXIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{total: totalProxiesCount}}
                loading={isProxiesLoading}
            />
        );
    }
}

let mapStateToProps = (state) => {
    return {
        proxiesInfo: state.webProxyIndexReducer.proxiesInfo,
        totalProxiesCount: state.webProxyIndexReducer.totalProxiesCount,
        error: state.webProxyIndexReducer.error,
        isProxiesLoading: state.webProxyIndexReducer.isProxiesLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllProxies: (pagination) => dispatch(getAllProxies(pagination)),
        startReceivingProxies: () => dispatch(startReceivingProxies())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(WebProxyIndex);
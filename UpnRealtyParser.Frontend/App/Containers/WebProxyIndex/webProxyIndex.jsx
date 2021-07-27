import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllProxies, startReceivingProxies } from './webProxyIndexActions.jsx';
import { PROXIES_TABLE_COLUMNS } from './webProxyIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class WebProxyIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingProxies();
        this.props.getAllProxies(new Object());
        document.title = SiteTitle + " - Прокси";
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingProxies();
        this.props.getAllProxies(pagination, sorter);
    }

    render() {
        let proxiesData = this.props.proxiesInfo.map(item => ({ ...item, key: item.id }));
        let totalProxiesCount = this.props.totalProxiesCount;
        let isProxiesLoading = this.props.isProxiesLoading;

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>Администрирование</Breadcrumb.Item>
                <Breadcrumb.Item>Прокси</Breadcrumb.Item>
            </Breadcrumb>
            <Table
                dataSource={proxiesData}
                columns={PROXIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{total: totalProxiesCount}}
                loading={isProxiesLoading}
            />
            </div>
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
        getAllProxies: (pagination, sorter) => dispatch(getAllProxies(pagination, sorter)),
        startReceivingProxies: () => dispatch(startReceivingProxies())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(WebProxyIndex);
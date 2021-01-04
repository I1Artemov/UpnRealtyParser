import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllLogEntries, startReceivingLogEntries } from './logEntryIndexActions.jsx';
import { LOG_ENTRIES_TABLE_COLUMNS } from './logEntryIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';

import 'antd/dist/antd.css';

class LogEntryIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingLogEntries();
        this.props.getAllLogEntries(new Object());
        // todo: bind
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingLogEntries();
        this.props.getAllLogEntries(pagination);
    }

    render() {
        let logEntriesData = this.props.logEntryInfo.map(item => ({ ...item, key: item.id }));
        let totalLogEntriesCount = this.props.totalLogEntriesCount;
        let isLogEntriesLoading = this.props.isLogEntriesLoading;

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>Администрирование</Breadcrumb.Item>
                <Breadcrumb.Item>Лог</Breadcrumb.Item>
            </Breadcrumb>
            <Table
                dataSource={logEntriesData}
                columns={LOG_ENTRIES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalLogEntriesCount }}
                loading={isLogEntriesLoading}
            />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        logEntryInfo: state.logEntryIndexReducer.logEntryInfo,
        totalLogEntriesCount: state.logEntryIndexReducer.totalLogEntriesCount,
        error: state.logEntryIndexReducer.error,
        isLogEntriesLoading: state.logEntryIndexReducer.isLogEntriesLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllLogEntries: (pagination) => dispatch(getAllLogEntries(pagination)),
        startReceivingLogEntries: () => dispatch(startReceivingLogEntries())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(LogEntryIndex);
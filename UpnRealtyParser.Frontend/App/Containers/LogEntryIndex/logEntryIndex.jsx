import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllLogEntries, startReceivingLogEntries } from './logEntryIndexActions.jsx';
import { logEntryIndexConst } from './logEntryIndexConstants.jsx';
import { Table, PageHeader } from 'antd';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class LogEntryIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingLogEntries();
        this.props.getAllLogEntries(new Object());
        document.title = SiteTitle + " - Лог";
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
            <PageHeader className="site-page-header" backIcon={false} onBack={() => null}
                    title="Журнал событий" subTitle="Администрирование"/>
            <Table
                dataSource={logEntriesData}
                columns={logEntryIndexConst.LOG_ENTRIES_TABLE_COLUMNS}
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
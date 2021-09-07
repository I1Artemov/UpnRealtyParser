import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { ErrorBoundary } from 'react-error-boundary';
import ErrorFallback from '../../Stateless/errorFallback.jsx';
import { getAllFlats, startReceivingFlats } from '../Common/anyFlatIndexActions.jsx';
import { n1SellFlatIndexConst } from '../N1SellFlatIndex/n1SellFlatIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';
import { Href_N1RentFlatController_GetAllFlats } from "../../const.jsx";
import FlatsSearchBar from '../FlatsSearchBar/flatsSearchBar.jsx';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class N1SellFlatIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingFlats();
        this.props.getAllFlats(new Object(), null, this.props.filteringInfo);
        document.title = SiteTitle + " - Квартиры N1 в аренду";
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingFlats();
        this.props.getAllFlats(pagination, sorter, this.props.filteringInfo);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;

        return (
            <div>
                <Breadcrumb style={{ margin: '16px 0' }}>
                    <Breadcrumb.Item>N1</Breadcrumb.Item>
                    <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                    <Breadcrumb.Item>В аренду</Breadcrumb.Item>
                </Breadcrumb>
                <ErrorBoundary FallbackComponent={ErrorFallback}>
                    <FlatsSearchBar handleTableChange={this.handleTableChange.bind(this)} siteName={"n1"} />
                </ErrorBoundary>
                <Table
                    dataSource={flatsData}
                    columns={n1SellFlatIndexConst.SELL_FLATS_TABLE_COLUMNS}
                    onChange={this.handleTableChange.bind(this)}
                    pagination={{ total: totalFlatsCount }}
                    loading={isFlatsLoading}
                    rowClassName={(record, index) => (record.isArchived === 1 ? "archived-flat" : "active-flat")}
                />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.n1RentFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.n1RentFlatIndexReducer.totalFlatsCount,
        error: state.n1RentFlatIndexReducer.error,
        isFlatsLoading: state.n1RentFlatIndexReducer.isFlatsLoading,
        filteringInfo: state.flatSearchBarReducer.filteringInfo
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, sorter, filteringInfo) =>
            dispatch(getAllFlats(pagination, sorter, filteringInfo, Href_N1RentFlatController_GetAllFlats)),
        startReceivingFlats: () => dispatch(startReceivingFlats())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(N1SellFlatIndex);
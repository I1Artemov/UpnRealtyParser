import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import {
    getAllFlats, startReceivingFlats, savePagingParameters
} from './upnSellFlatIndexActions.jsx';

import { SELL_FLATS_TABLE_COLUMNS } from './upnSellFlatIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';

import FlatsSearchBar from '../FlatsSearchBar/flatsSearchBar.jsx';

import 'antd/dist/antd.css';

class UpnSellFlatIndex extends React.Component {
    getAllFlatsWithParametersFromProps(pagination, sorter) {
        this.props.startReceivingFlats();

        this.props.getAllFlats(pagination, sorter, this.props.isShowArchived, this.props.minPrice, this.props.maxPrice,
            this.props.isExcludeFirstFloor, this.props.isExcludeLastFloor, this.props.minBuildYear,
            this.props.maxSubwayDistance, this.props.closestSubwayStationId, this.props.addressPart);
    }

    componentDidMount() {
        let pagingInfo = {
            current: this.props.savedGridPage,
            pageSIze: this.props.savedGridPageSize
        };
        this.getAllFlatsWithParametersFromProps(pagingInfo);
        document.title = "Ural Realty Parser - Квартиры UPN в продажу";
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.savePagingParameters(pagination);
        this.getAllFlatsWithParametersFromProps(pagination, sorter);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;

        return (
            <div>
                <Breadcrumb style={{ margin: '26px 0' }}>
                    <Breadcrumb.Item>Upn</Breadcrumb.Item>
                    <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                    <Breadcrumb.Item>На продажу</Breadcrumb.Item>
                </Breadcrumb>
                <FlatsSearchBar/>
                <Table
                    dataSource={flatsData}
                    columns={SELL_FLATS_TABLE_COLUMNS}
                    onChange={this.handleTableChange.bind(this)}
                    pagination={{ current: this.props.savedGridPage, pageSIze: this.props.savedGridPageSize, total: totalFlatsCount}}
                    loading={isFlatsLoading}
                    rowClassName={(record, index) => (record.isArchived === 1 ? "archived-flat" : "active-flat")}
                    />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnSellFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnSellFlatIndexReducer.totalFlatsCount,
        error: state.upnSellFlatIndexReducer.error,
        isFlatsLoading: state.upnSellFlatIndexReducer.isFlatsLoading,
        savedGridPage: state.upnSellFlatIndexReducer.savedGridPage,
        savedGridPageSize: state.upnSellFlatIndexReducer.savedGridPageSize
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, sorter, isShowArchived, minPrice, maxPrice, isExcludeFirstFloor, isExcludeLastFloor, minBuildYear,
            maxSubwayDistance, closestSubwayStationId, addressPart) =>
            dispatch(getAllFlats(pagination, sorter, isShowArchived, minPrice, maxPrice, isExcludeFirstFloor, isExcludeLastFloor, minBuildYear,
                maxSubwayDistance, closestSubwayStationId, addressPart)),
        startReceivingFlats: () => dispatch(startReceivingFlats()),
        savePagingParameters: (pagination) => dispatch(savePagingParameters(pagination))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
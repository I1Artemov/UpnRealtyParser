import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats, startReceivingFlats } from '../Common/anyFlatIndexActions.jsx';
// Используем те же колонки, что и у квартир на продажу
import { SELL_FLATS_TABLE_COLUMNS } from '../UpnSellFlatIndex/upnSellFlatIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';
import { Href_UpnRentFlatController_GetAllFlats } from "../../const.jsx";
import FlatsSearchBar from '../FlatsSearchBar/flatsSearchBar.jsx';
import { SiteTitle } from '../../const.jsx'

import 'antd/dist/antd.css';

class UpnRentFlatIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingFlats();
        this.props.getAllFlats(new Object(), null, this.props.filteringInfo);
        document.title = SiteTitle + " - Квартиры UPN в аренду";
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingFlats();
        this.props.getAllFlats(pagination, sorter, this.props.filteringInfo);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;
        let errorMessage = this.props.error;
        // Убираем сортировку у колонок, относящихся к дому, т.к. пока нет дозаполнения информацией о домах
        let editedColumns = SELL_FLATS_TABLE_COLUMNS.map((item) => {
            if (item.key === "houseBuildYear" || item.key === "subwaySummary")
                return { ...item, sorter: false }
            return item;
        });

        if (errorMessage !== undefined && errorMessage !== null && errorMessage !== "") {
            return (
                <div>RentFlatCo
                    Ошибка загрузки квартиры: {errorMessage.toString()}
                </div>
                );
        } else {
            return (
                <div>
                <Breadcrumb style={{ margin: '16px 0' }}>
                    <Breadcrumb.Item>Upn</Breadcrumb.Item>
                    <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                    <Breadcrumb.Item>В аренду</Breadcrumb.Item>
                </Breadcrumb>
                <FlatsSearchBar handleTableChange={this.handleTableChange.bind(this)} />
                <Table
                    dataSource={flatsData}
                    columns={editedColumns}
                    onChange={this.handleTableChange.bind(this)}
                    pagination={{ total: totalFlatsCount }}
                    loading={isFlatsLoading}
                    rowClassName={(record, index) => (record.isArchived === 1 ? "archived-flat" : "active-flat")}
                    />
                </div>
            );
        }
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnRentFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnRentFlatIndexReducer.totalFlatsCount,
        error: state.upnRentFlatIndexReducer.error,
        isFlatsLoading: state.upnRentFlatIndexReducer.isFlatsLoading,
        filteringInfo: state.flatSearchBarReducer.filteringInfo
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, sorter, filteringInfo) =>
            dispatch(getAllFlats(pagination, sorter, filteringInfo, Href_UpnRentFlatController_GetAllFlats)),
        startReceivingFlats: () => dispatch(startReceivingFlats())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnRentFlatIndex);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import {
    getAllFlats, startReceivingFlats, setShowArchived, setExcludeFirstFloor, setExcludeLastFloor,
    setMinPrice, setMaxPrice, setMinBuildYear, setMaxSubwayDistance, setClosestSubwayStationId,
    setAddressPart, clearSearchParameters, savePagingParameters
} from './upnSellFlatIndexActions.jsx';

import { SELL_FLATS_TABLE_COLUMNS } from './upnSellFlatIndexConstants.jsx';
import { Table, Breadcrumb, Checkbox, InputNumber, Select, Button, Input } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';

import 'antd/dist/antd.css';

class UpnSellFlatIndex extends React.Component {
    getAllFlatsWithParametersFromProps(pagination) {
        this.props.startReceivingFlats();

        this.props.getAllFlats(pagination, this.props.isShowArchived, this.props.minPrice, this.props.maxPrice,
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
        this.getAllFlatsWithParametersFromProps(pagination);
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
                <div className="search-bar-above-table">
                    <div>
                        <span>Отображать архивные</span>
                        <Checkbox onChange={this.props.setShowArchived.bind(this)} checked={this.props.isShowArchived} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                        <span>Цена от</span>
                        <InputNumber onChange={this.props.setMinPrice.bind(this)} value={this.props.minPrice} min={0} style={{ width: 110, marginLeft: 9, marginRight: 28 }} />

                        <span>Цена до</span>
                        <InputNumber onChange={this.props.setMaxPrice.bind(this)} value={this.props.maxPrice} min={0} style={{ width: 110, marginLeft: 9, marginRight: 28 }} />

                        <span>Не первый этаж</span>
                        <Checkbox onChange={this.props.setExcludeFirstFloor.bind(this)} checked={this.props.isExcludeFirstFloor} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                        <span>Не последний этаж</span>
                        <Checkbox onChange={this.props.setExcludeLastFloor.bind(this)} checked={this.props.isExcludeLastFloor} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                        <span>Год постройки от</span>
                        <InputNumber onChange={this.props.setMinBuildYear.bind(this)} value={this.props.minBuildYear} min={1930} max={2020} style={{ marginLeft: 9, marginRight: 28 }} />

                        <Button onClick={this.handleTableChange.bind(this)} type="primary" icon={<SearchOutlined />} style={{marginRight: "9px"}}>Применить</Button>
                        <Button onClick={this.props.clearSearchParameters.bind(this)} icon={<CloseOutlined />}>Сбросить</Button>
                    </div>
                    <div style={{ marginTop: "6px" }}>
                        <span>Ближайшая станция</span>
                        <Select onChange={this.props.setClosestSubwayStationId.bind(this)} style={{ width: 229, marginLeft: 9, marginRight: 28 }} placeholder="Выберите станцию">
                            <Select.Option value="1">Проспект космонавтов</Select.Option>
                            <Select.Option value="2">Уралмаш</Select.Option>
                            <Select.Option value="3">Машиностроителей</Select.Option>
                            <Select.Option value="4">Уральская</Select.Option>
                            <Select.Option value="5">Динамо</Select.Option>
                            <Select.Option value="6">Площадь 1905 года</Select.Option>
                            <Select.Option value="7">Геологическая</Select.Option>
                            <Select.Option value="8">Чкаловская</Select.Option>
                            <Select.Option value="9">Ботаническая</Select.Option>
                        </Select>

                        <span>Расстояние до метро, м</span>
                        <InputNumber onChange={this.props.setMaxSubwayDistance.bind(this)} value={this.props.maxSubwayDistance} style={{ marginLeft: 9, marginRight: 28 }} />

                        <span>Улица</span>
                        <Input onChange={this.props.setAddressPart.bind(this)} value={this.props.addressPart} style={{ marginLeft: 9, marginRight: 28, width: 240 }} />
                    </div>
                </div>
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
        isShowArchived: state.upnSellFlatIndexReducer.isShowArchived,
        isExcludeFirstFloor: state.upnSellFlatIndexReducer.isExcludeFirstFloor,
        isExcludeLastFloor: state.upnSellFlatIndexReducer.isExcludeLastFloor,
        minPrice: state.upnSellFlatIndexReducer.minPrice,
        maxPrice: state.upnSellFlatIndexReducer.maxPrice,
        minBuildYear: state.upnSellFlatIndexReducer.minBuildYear,
        maxSubwayDistance: state.upnSellFlatIndexReducer.maxSubwayDistance,
        closestSubwayStationId: state.upnSellFlatIndexReducer.closestSubwayStationId,
        addressPart: state.upnHouseIndexReducer.addressPart,
        savedGridPage: state.upnSellFlatIndexReducer.savedGridPage,
        savedGridPageSize: state.upnSellFlatIndexReducer.savedGridPageSize
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, isShowArchived, minPrice, maxPrice, isExcludeFirstFloor, isExcludeLastFloor, minBuildYear,
            maxSubwayDistance, closestSubwayStationId, addressPart) =>
            dispatch(getAllFlats(pagination, isShowArchived, minPrice, maxPrice, isExcludeFirstFloor, isExcludeLastFloor, minBuildYear,
                maxSubwayDistance, closestSubwayStationId, addressPart)),
        startReceivingFlats: () => dispatch(startReceivingFlats()),
        setShowArchived: (ev) => dispatch(setShowArchived(ev)),
        setExcludeFirstFloor: (ev) => dispatch(setExcludeFirstFloor(ev)),
        setExcludeLastFloor: (ev) => dispatch(setExcludeLastFloor(ev)),
        setMinPrice: (ev) => dispatch(setMinPrice(ev)),
        setMaxPrice: (ev) => dispatch(setMaxPrice(ev)),
        setMinBuildYear: (ev) => dispatch(setMinBuildYear(ev)),
        setMaxSubwayDistance: (ev) => dispatch(setMaxSubwayDistance(ev)),
        setClosestSubwayStationId: (ev) => dispatch(setClosestSubwayStationId(ev)),
        setAddressPart: (ev) => dispatch(setAddressPart(ev)),
        clearSearchParameters: () => dispatch(clearSearchParameters()),
        savePagingParameters: (pagination) => dispatch(savePagingParameters(pagination))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import {
    getAllHouses, startReceivingHouses, setMinBuildYear, setIsShowUpn, setIsShowN1,
    setAddressPart, clearSearchParameters
} from './upnHouseIndexActions.jsx';
import { HOUSES_TABLE_COLUMNS } from './upnHouseIndexConstants.jsx';
import { Table, Breadcrumb, Checkbox, Input, InputNumber, Button } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';

import 'antd/dist/antd.css';

class UpnHouseIndex extends React.Component {

    componentDidMount() {
        let pagingInfo = {
            current: 1,
            pageSIze: 10
        };
        this.getAllHousesWithParametersFromProps(pagingInfo);
        document.title = "Ural Realty Parser - Дома";
    }

    handleTableChange(pagination, filters, sorter) {
        this.getAllHousesWithParametersFromProps(pagination, sorter);
    }

    getAllHousesWithParametersFromProps(pagination, sorter) {
        this.props.startReceivingHouses();

        this.props.getAllHouses(pagination, sorter, this.props.minBuildYear, this.props.isShowUpn,
            this.props.isShowN1, this.props.addressPart);
    }

    render() {
        let housesData = this.props.housesInfo.map(item => ({ ...item, key: item.id }));
        let totalHousesCount = this.props.totalHousesCount;
        let isHousesLoading = this.props.isHousesLoading;

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>Дома УПН и N1</Breadcrumb.Item>
            </Breadcrumb>
            <div className="search-bar-above-table">
                <span>Из УПН</span>
                <Checkbox onChange={this.props.setShowUpn.bind(this)} checked={this.props.isShowUpn} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                <span>Из N1</span>
                <Checkbox onChange={this.props.setShowN1.bind(this)} checked={this.props.isShowN1} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                <span>Год постройки от</span>
                <InputNumber onChange={this.props.setMinBuildYear.bind(this)} value={this.props.minBuildYear} min={1930} max={2025} style={{ marginLeft: 9, marginRight: 28 }} />

                <span>Улица</span>
                <Input onChange={this.props.setAddressPart.bind(this)} value={this.props.addressPart} style={{ marginLeft: 9, marginRight: 28, width: 400 }} />

                <Button onClick={this.handleTableChange.bind(this)} type="primary" icon={<SearchOutlined />} style={{ marginRight: "9px" }}>Применить</Button>
                <Button onClick={this.props.clearSearchParameters.bind(this)} icon={<CloseOutlined />}>Сбросить</Button>
            </div>
            <Table
                dataSource={housesData}
                columns={HOUSES_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalHousesCount }}
                loading={isHousesLoading}
                rowKey={obj => obj.id + '_' + obj.sourceSite}
            />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        housesInfo: state.upnHouseIndexReducer.housesInfo,
        totalHousesCount: state.upnHouseIndexReducer.totalHousesCount,
        error: state.upnHouseIndexReducer.error,
        isHousesLoading: state.upnHouseIndexReducer.isHousesLoading,
        minBuildYear: state.upnHouseIndexReducer.minBuildYear,
        isShowUpn: state.upnHouseIndexReducer.isShowUpn,
        isShowN1: state.upnHouseIndexReducer.isShowN1,
        addressPart: state.upnHouseIndexReducer.addressPart
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllHouses: (pagination, sorter, minBuildYear, isShowUpn, isShowN1, addressPart) =>
            dispatch(getAllHouses(pagination, sorter, minBuildYear, isShowUpn, isShowN1, addressPart)),
        startReceivingHouses: () => dispatch(startReceivingHouses()),
        setMinBuildYear: (ev) => dispatch(setMinBuildYear(ev)),
        setShowUpn: (ev) => dispatch(setIsShowUpn(ev)),
        setShowN1: (ev) => dispatch(setIsShowN1(ev)),
        setAddressPart: (ev) => dispatch(setAddressPart(ev)),
        clearSearchParameters: () => dispatch(clearSearchParameters())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnHouseIndex);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { ErrorBoundary } from 'react-error-boundary';
import ErrorFallback from '../../Stateless/errorFallback.jsx';
import {
    getAllHouses, startReceivingHouses, setMinBuildYear, setIsShowUpn, setIsShowN1,
    setAddressPart, clearSearchParameters
} from './upnHouseIndexActions.jsx';
import { houseIndexConst } from './upnHouseIndexConstants.jsx';
import { Table, PageHeader, Checkbox, Input, InputNumber, Button } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class UpnHouseIndex extends React.Component {

    componentDidMount() {
        let pagingInfo = {
            current: 1,
            pageSIze: 10
        };
        this.getAllHousesWithParametersFromProps(pagingInfo);
        document.title = SiteTitle + " - Дома";
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
                <PageHeader className="site-page-header" backIcon={false} onBack={() => null}
                    title="Дома УПН и N1" />
            <ErrorBoundary FallbackComponent={ErrorFallback}>
                <div className="search-bar-above-table">
                    <label htmlFor="showUpnInput">Из УПН</label>
                    <Checkbox id={"showUpnInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setShowUpn.bind(this)} checked={this.props.isShowUpn}></Checkbox>

                    <label htmlFor="showN1Input">Из N1</label>
                    <Checkbox id={"showN1Input"} className="search-bar-input-with-margin"
                        onChange={this.props.setShowN1.bind(this)} checked={this.props.isShowN1}></Checkbox>

                    <label htmlFor="minBuildYearInput">Год постройки от</label>
                    <InputNumber id={"minBuildYearInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setMinBuildYear.bind(this)} value={this.props.minBuildYear} min={1930} max={2025} />

                    <label htmlFor="addressPartInput">Улица</label>
                    <Input id={"addressPartInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setAddressPart.bind(this)} value={this.props.addressPart} style={{ width: 400 }} />

                    <Button onClick={this.handleTableChange.bind(this)} type="primary" icon={<SearchOutlined />} style={{ marginRight: "9px" }}>Применить</Button>
                    <Button onClick={this.props.clearSearchParameters.bind(this)} icon={<CloseOutlined />}>Сбросить</Button>
                </div>
            </ErrorBoundary>
            <Table
                dataSource={housesData}
                columns={houseIndexConst.HOUSES_TABLE_COLUMNS}
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
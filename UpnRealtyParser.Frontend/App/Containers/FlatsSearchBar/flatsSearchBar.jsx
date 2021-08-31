import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import moment from "moment";

import {
    setShowArchived, setExcludeFirstFloor, setExcludeLastFloor,
    setMinPrice, setMaxPrice, setMinBuildYear, setMaxSubwayDistance, setClosestSubwayStationId,
    setAddressPart, setShowRooms, setStartDate, setEndDate, clearSearchParameters
} from './flatsSearchBarActions.jsx';

import { Checkbox, InputNumber, Select, Button, Input, DatePicker, Tooltip } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';

class FlatsSearchBar extends React.Component {
    render() {

        return (
            <div className="search-bar-above-table">
                <div>
                    <label htmlFor="showArchivedCheckbox">Отображать архивные</label>
                    <Checkbox id={"showArchivedCheckbox"} className="search-bar-input-with-margin"
                        onChange={this.props.setShowArchived.bind(this)} checked={this.props.isShowArchived}></Checkbox>

                    <label htmlFor="minPriceInput">Цена от</label>
                    <InputNumber id={"minPriceInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setMinPrice.bind(this)} value={this.props.minPrice} min={0} style={{ width: 110 }} />

                    <label htmlFor="maxPriceInput">Цена до</label>
                    <InputNumber id={"maxPriceInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setMaxPrice.bind(this)} value={this.props.maxPrice} min={0} style={{ width: 110 }} />

                    <label htmlFor="excludeFirstFloorCheckbox">Не первый этаж</label>
                    <Checkbox id={"excludeFirstFloorCheckbox"} className="search-bar-input-with-margin"
                        onChange={this.props.setExcludeFirstFloor.bind(this)} checked={this.props.isExcludeFirstFloor}></Checkbox>

                    <label htmlFor="excludeLastFloorCheckbox">Не последний этаж</label>
                    <Checkbox id={"excludeLastFloorCheckbox"} className="search-bar-input-with-margin"
                        onChange={this.props.setExcludeLastFloor.bind(this)} checked={this.props.isExcludeLastFloor}></Checkbox>

                    <label htmlFor="minBuildYearInput">Год постройки от</label>
                    <InputNumber id={"minBuildYearInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setMinBuildYear.bind(this)} value={this.props.minBuildYear} min={1930} max={2020} />

                    <Tooltip title="По умолчанию - текущая дата минус полгода">
                        <label htmlFor="startDatePicker">Дата создания с </label>
                    </Tooltip>
                    <DatePicker id={"startDatePicker"} onChange={this.props.setStartDate.bind(this)} style={{ marginLeft: 6, marginRight: 8 }} />

                    <Tooltip title="По умолчанию - текущая дата">
                        <label htmlFor="endDatePicker"> по </label>
                    </Tooltip>
                    <DatePicker id={"endDatePicker"} onChange={this.props.setEndDate.bind(this)} style={{ marginLeft: 6 }}/>
                </div>
                <div style={{ marginTop: "6px" }}>
                    <label htmlFor="closestSubwayStationIdSelect">Ближайшая станция</label>
                    <Select id={"closestSubwayStationIdSelect"} className="search-bar-input-with-margin"
                        onChange={this.props.setClosestSubwayStationId.bind(this)} style={{ width: 229 }} placeholder="Выберите станцию">
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

                    <label htmlFor="maxSubwayDistanceInput">Расстояние до метро, м</label>
                    <InputNumber id={"maxSubwayDistanceInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setMaxSubwayDistance.bind(this)} value={this.props.maxSubwayDistance} />

                    <label htmlFor="addressPartInput">Улица</label>
                    <Input id={"addressPartInput"} className="search-bar-input-with-margin"
                        onChange={this.props.setAddressPart.bind(this)} value={this.props.addressPart} style={{ width: 240 }} />

                    <label htmlFor="showRoomsCheckbox">Отображать комнаты</label>
                    <Checkbox id={"showRoomsCheckbox"} className="search-bar-input-with-margin"
                        onChange={this.props.setShowRooms.bind(this)} checked={this.props.isShowRooms}
                        disabled={this.props.siteName == "n1"}></Checkbox>

                    <Button onClick={this.props.handleTableChange} type="primary" icon={<SearchOutlined />} style={{ marginRight: "9px" }}>Применить</Button>
                    <Button onClick={this.props.clearSearchParameters.bind(this)} icon={<CloseOutlined />}>Сбросить</Button>
                </div>
            </div>
        );
    }
}

let mapStateToProps = (state, ownProps) => {
    return {
        isShowArchived: state.flatSearchBarReducer.filteringInfo.isShowArchived,
        isExcludeFirstFloor: state.flatSearchBarReducer.filteringInfo.isExcludeFirstFloor,
        isExcludeLastFloor: state.flatSearchBarReducer.filteringInfo.isExcludeLastFloor,
        minPrice: state.flatSearchBarReducer.filteringInfo.minPrice,
        maxPrice: state.flatSearchBarReducer.filteringInfo.maxPrice,
        minBuildYear: state.flatSearchBarReducer.filteringInfo.minBuildYear,
        maxSubwayDistance: state.flatSearchBarReducer.filteringInfo.maxSubwayDistance,
        closestSubwayStationId: state.flatSearchBarReducer.filteringInfo.closestSubwayStationId,
        addressPart: state.flatSearchBarReducer.filteringInfo.addressPart,
        isShowRooms: state.flatSearchBarReducer.filteringInfo.isShowRooms,
        startDate: state.flatSearchBarReducer.filteringInfo.startDate,
        endDate: state.flatSearchBarReducer.filteringInfo.endDate,
        siteName: ownProps.siteName
    };
};

let mapActionsToProps = (dispatch, ownProps) => {
    return {
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
        setShowRooms: (ev) => dispatch(setShowRooms(ev)),
        setStartDate: (ev) => dispatch(setStartDate(ev)),
        setEndDate: (ev) => dispatch(setEndDate(ev)),
        handleTableChange: ownProps.handleTableChange // Проброс функции из компонента с таблицей квартир
    };
};

export default connect(mapStateToProps, mapActionsToProps)(FlatsSearchBar);
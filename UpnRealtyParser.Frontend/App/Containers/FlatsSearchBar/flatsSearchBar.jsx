import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';

import {
    setShowArchived, setExcludeFirstFloor, setExcludeLastFloor,
    setMinPrice, setMaxPrice, setMinBuildYear, setMaxSubwayDistance, setClosestSubwayStationId,
    setAddressPart, clearSearchParameters
} from './flatsSearchBarActions.jsx';

import { Checkbox, InputNumber, Select, Button, Input } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';

class FlatsSearchBar extends React.Component {
    render() {

        return (
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

                    <Button onClick={this.props.handleTableChange} type="primary" icon={<SearchOutlined />} style={{ marginRight: "9px" }}>Применить</Button>
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
        );
    }
}

let mapStateToProps = (state) => {
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
        handleTableChange: ownProps.handleTableChange // Проброс функции из компонента с таблицей квартир
    };
};

export default connect(mapStateToProps, mapActionsToProps)(FlatsSearchBar);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import moment from "moment";
import { Row, Col, Space } from 'antd';
import { HomeOutlined, BankOutlined, BarChartOutlined, ToolOutlined } from '@ant-design/icons';

import {
    setShowArchived, setExcludeFirstFloor, setExcludeLastFloor,
    setMinPrice, setMaxPrice, setMinBuildYear, setMaxSubwayDistance, setClosestSubwayStationId,
    setAddressPart, setShowRooms, setStartDate, setEndDate, clearSearchParameters, setMaxPayback,
    setDescriptionPart
} from './flatsSearchBarActions.jsx';

import { Checkbox, InputNumber, Select, Button, Input, DatePicker, Tooltip } from 'antd';
import { SearchOutlined, CloseOutlined } from '@ant-design/icons';

class FlatsSearchBar extends React.Component {
    render() {
        const blockHeaderStyle = { textAlign: "center", fontWeight: "bold", marginBottom: "10px", backgroundColor: "#f7f7f7" };
        const rowInSemanticBlockStyle = { marginTop: "3px", marginBottom: "3px" }

        return (
            <div className="search-bar-above-table">
                <Row>
                    <Col span={7} style={blockHeaderStyle}><Space><HomeOutlined /> Дом</Space></Col>
                    <Col span={4} style={blockHeaderStyle}><Space><BankOutlined /> Квартира</Space></Col>
                    <Col span={5} style={blockHeaderStyle}><Space><ToolOutlined /> Системное</Space></Col>
                    <Col span={5} style={blockHeaderStyle}><Space><BarChartOutlined /> Аналитика</Space></Col>
                    <Col span={3}></Col>
                </Row>

                <Row>
                    <Col span={7}>
                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={8}><label htmlFor="minBuildYearInput">Год постройки от</label></Col>
                            <Col span={16}><InputNumber id={"minBuildYearInput"}
                                onChange={this.props.setMinBuildYear.bind(this)} value={this.props.minBuildYear} min={1930} max={2020} /></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={8}><label htmlFor="closestSubwayStationIdSelect">Ближайшая станция</label></Col>
                            <Col span={16}><Select id={"closestSubwayStationIdSelect"} 
                                onChange={this.props.setClosestSubwayStationId.bind(this)} placeholder="Выберите станцию">
                                <Select.Option value="1">Проспект космонавтов</Select.Option>
                                <Select.Option value="2">Уралмаш</Select.Option>
                                <Select.Option value="3">Машиностроителей</Select.Option>
                                <Select.Option value="4">Уральская</Select.Option>
                                <Select.Option value="5">Динамо</Select.Option>
                                <Select.Option value="6">Площадь 1905 года</Select.Option>
                                <Select.Option value="7">Геологическая</Select.Option>
                                <Select.Option value="8">Чкаловская</Select.Option>
                                <Select.Option value="9">Ботаническая</Select.Option>
                            </Select></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={8}><label htmlFor="maxSubwayDistanceInput">До метро, метров</label></Col>
                            <Col span={16}><InputNumber id={"maxSubwayDistanceInput"}
                                onChange={this.props.setMaxSubwayDistance.bind(this)} value={this.props.maxSubwayDistance} /></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={8}><label htmlFor="addressPartInput">Улица</label></Col>
                            <Col span={16}><Input id={"addressPartInput"} style={{ maxWidth: "250px" }}
                                onChange={this.props.setAddressPart.bind(this)} value={this.props.addressPart} /></Col>
                        </Row>
                    </Col>

                    <Col span={4}>
                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="minPriceInput">Цена от</label></Col>
                            <Col span={12}><InputNumber id={"minPriceInput"}
                                onChange={this.props.setMinPrice.bind(this)} value={this.props.minPrice} min={0} /></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="maxPriceInput">Цена до</label></Col>
                            <Col span={12}><InputNumber id={"maxPriceInput"}
                                onChange={this.props.setMaxPrice.bind(this)} value={this.props.maxPrice} min={0} /></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="excludeFirstFloorCheckbox">Не первый этаж</label></Col>
                            <Col span={12}><Checkbox id={"excludeFirstFloorCheckbox"}
                                onChange={this.props.setExcludeFirstFloor.bind(this)} checked={this.props.isExcludeFirstFloor}></Checkbox></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="excludeLastFloorCheckbox">Не последний этаж</label></Col>
                            <Col span={12}><Checkbox id={"excludeLastFloorCheckbox"}
                                onChange={this.props.setExcludeLastFloor.bind(this)} checked={this.props.isExcludeLastFloor}></Checkbox></Col>
                        </Row>
                    </Col>

                    <Col span={5}>
                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="showArchivedCheckbox">Отображать архивные</label></Col>
                                <Col span={12}><Checkbox id={"showArchivedCheckbox"}
                                onChange={this.props.setShowArchived.bind(this)} checked={this.props.isShowArchived}></Checkbox></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><Tooltip title="По умолчанию - текущая дата минус полгода">
                                <label htmlFor="startDatePicker">Дата создания с </label>
                            </Tooltip></Col>
                            <Col span={12}><DatePicker id={"startDatePicker"} onChange={this.props.setStartDate.bind(this)}/></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><Tooltip title="По умолчанию - текущая дата">
                                <label htmlFor="endDatePicker">Дата создания по </label>
                            </Tooltip></Col>
                            <Col span={12}><DatePicker id={"endDatePicker"} onChange={this.props.setEndDate.bind(this)} /></Col>
                        </Row>

                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="showRoomsCheckbox">Отображать комнаты</label></Col>
                            <Col span={12}><Checkbox id={"showRoomsCheckbox"}
                                onChange={this.props.setShowRooms.bind(this)} checked={this.props.isShowRooms}
                                disabled={this.props.siteName == "n1"}></Checkbox></Col>
                        </Row>
                    </Col>

                    <Col span={5}>
                        <Row style={rowInSemanticBlockStyle}>
                            <Col span={12}><label htmlFor="maxPaybackInput">Окупаемость до (лет)</label></Col>
                            <Col span={12}><InputNumber id={"maxPaybackInput"}
                                onChange={this.props.setMaxPayback.bind(this)} value={this.props.maxPayback} /></Col>
                        </Row>
                        <Row>
                            <Col span={12}><label htmlFor="descriptionContainsInput">Описание содержит</label></Col>
                            <Col span={12}><Input id={"descriptionContainsInput"} style={{ maxWidth: "250px" }}
                                onChange={this.props.setDescriptionPart.bind(this)} value={this.props.descriptionPart} /></Col>
                        </Row>
                    </Col>

                    <Col span={3} style={{ textAlign: "center" }}>
                        <div style={{ marginBottom: "10px", marginTop: "20px" }}>
                            <Button onClick={this.props.handleTableChange} type="primary" icon={<SearchOutlined />}>Применить</Button>
                        </div>
                        <div>
                            <Button onClick={this.props.clearSearchParameters.bind(this)} icon={<CloseOutlined />}>Сбросить</Button>
                        </div>
                    </Col>
                </Row>
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
        maxPayback: state.flatSearchBarReducer.filteringInfo.maxPayback,
        descriptionPart: state.flatSearchBarReducer.filteringInfo.descriptionPart,
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
        setMaxPayback: (ev) => dispatch(setMaxPayback(ev)),
        setDescriptionPart: (ev) => dispatch(setDescriptionPart(ev)),
        handleTableChange: ownProps.handleTableChange // Проброс функции из компонента с таблицей квартир
    };
};

export default connect(mapStateToProps, mapActionsToProps)(FlatsSearchBar);
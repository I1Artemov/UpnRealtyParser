import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllPoints, setPaybackLimit, setUseUpnData, setUseN1Data } from "./paybackMapActions.jsx";
import { Divider, Spin, InputNumber, Button, Checkbox } from 'antd';
import { SiteTitle } from '../../const.jsx';

import HeatMapFunctional from './heatMapPayback.jsx';

import 'antd/dist/antd.css';

class PaybackMap extends React.Component {
    componentDidMount() {
        this.props.getAllPoints();
        document.title = SiteTitle + " - Карта окупаемости";
    }

    applyFilters() {
        this.props.getAllPoints(this.props.paybackLimit, this.props.isUseUpnData, this.props.isUseN1Data);
    }

    render() {
        let points = this.props.points;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;

        let maximalPaybackYears = 1;
        if (points.length > 1) {
            maximalPaybackYears = points.reduce(function (prevMax, currentPoint) {
                return (currentPoint.paybackYears > prevMax) ? currentPoint.paybackYears : prevMax;
            }, 0);
        }
        
        let normalizedPoints = points.map((point, i) => {
            return {
                ...point,
                normalizedPaybackYears: 15 * (1 - (point.paybackYears / maximalPaybackYears))
            };
        });

        if (isLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            return (
                <div>
                    <Divider orientation={"center"}>Карта окупаемости</Divider>
                    <HeatMapFunctional points={normalizedPoints} />
                    <div style={{ marginTop: '8px' }}>
                        <div id="heat-legend-color-gradient"></div>
                        <div style={{ display: 'inline-block', marginRight: '6px' }}>Данные UPN </div>
                        <Checkbox onChange={this.props.setUseUpnData.bind(this)} checked={this.props.isUseUpnData} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>
                        <div style={{ display: 'inline-block', marginRight: '6px' }}>Данные N1 </div>
                        <Checkbox onChange={this.props.setUseN1Data.bind(this)} checked={this.props.isUseN1Data} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>
                        <div style={{ display: 'inline-block', marginRight: '6px'}}>Отображать с окупаемостью до </div>
                        <InputNumber onChange={this.props.setPaybackLimit.bind(this)} value={this.props.paybackLimit} min={1} max={80} style={{ display: 'inline-block', marginRight: '6px' }} />
                        <div style={{ display: 'inline-block'}}>лет</div>
                        <Button onClick={this.applyFilters.bind(this)} type="primary" style={{ display: 'inline-block', marginLeft: '16px'}}>Применить</Button>
                    </div>
                    <div>
                        <div id="heat-legend-minimal-value">20 лет</div>
                        <div id="heat-legend-maximal-value">10 лет</div>
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    Ошибка загрузки точек: {this.props.error.toString()}
                </div>
            );
        }
    }
}

let mapStateToProps = (state) => {
    return {
        points: state.paybackMapReducer.points,
        error: state.paybackMapReducer.error,
        isLoading: state.paybackMapReducer.isLoading,
        paybackLimit: state.paybackMapReducer.paybackLimit,
        isUseUpnData: state.paybackMapReducer.isUseUpnData,
        isUseN1Data: state.paybackMapReducer.isUseN1Data,
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllPoints: (paybackLimit, isUseUpnData, isUseN1Data) => dispatch(getAllPoints(paybackLimit, isUseUpnData, isUseN1Data)),
        setPaybackLimit: (ev) => dispatch(setPaybackLimit(ev)),
        setUseUpnData: (ev) => dispatch(setUseUpnData(ev)),
        setUseN1Data: (ev) => dispatch(setUseN1Data(ev))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(PaybackMap);
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllPoints } from "./paybackMapActions.jsx";
import { Divider, Spin, InputNumber, Button } from 'antd';

import HeatMapFunctional from './heatMapPayback.jsx';

import 'antd/dist/antd.css';

class PaybackMap extends React.Component {
    componentDidMount() {
        this.props.getAllPoints();
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
                        <div style={{ display: 'inline-block', marginRight: '6px'}}>Отображать с окупаемостью до </div>
                        <InputNumber style={{ display: 'inline-block', marginRight: '6px' }} />
                        <div style={{ display: 'inline-block'}}>лет</div>
                        <Button type="primary" style={{ display: 'inline-block', marginLeft: '16px'}}>Применить</Button>
                    </div>
                    <div>
                        <div id="heat-legend-minimal-value">10 лет</div>
                        <div id="heat-legend-maximal-value">20 лет</div>
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
        isLoading: state.paybackMapReducer.isLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllPoints: () => dispatch(getAllPoints())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(PaybackMap);
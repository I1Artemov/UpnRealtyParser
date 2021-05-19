import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllPoints } from "./paybackMapActions.jsx";
import { Divider, Spin } from 'antd';

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
            maximalPaybackYears = points.reduce(function (prev, current) {
                return (prev.paybackYears > current.paybackYears) ? prev.paybackYears : current.paybackYears;
            });
        }
        
        let normalizedPoints = points.map((point, i) => {
            return {
                ...point,
                normalizedPaybackYears: 5 * point.paybackYears / maximalPaybackYears
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
                    <HeatMapFunctional points={normalizedPoints}/>
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
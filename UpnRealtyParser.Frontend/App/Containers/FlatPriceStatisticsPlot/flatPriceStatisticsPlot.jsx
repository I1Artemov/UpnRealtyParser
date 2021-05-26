import React from 'react';
import { render } from 'react-dom';
import { Spin, Divider } from 'antd';
import { ResponsiveContainer, XAxis, YAxis, Tooltip, LineChart, CartesianGrid, Line } from 'recharts';
import { format } from 'date-fns';
import { connect } from 'react-redux';
import moment from 'moment';
import { getAvgPrices } from './flatPriceStatisticsPlotActions.jsx';

class FlatPriceStatisticsPlot extends React.Component {
    componentDidMount() {
        let houseId = this.props.houseId;
        this.props.getAvgPrices(houseId, 1);
    }

    render() {
        let isInfoLoading = this.props.isInfoLoading;
        let errorMessage = this.props.error;
        let allRoomPrices = this.props.allRoomPrices;
        allRoomPrices.forEach(d => {
            d.xAxis = moment(d.xAxis).valueOf(); // date -> epoch
        });

        var today = new Date();
        const xDomain = [dataMin => dataMin, () => new Date(today.getFullYear(), today.getMonth() + 1, 1).getTime()];
        const yDomain = [dataMin => dataMin - 50000, dataMax => dataMax];

        const priceFormatter = priceNum => {
            let priceText = "";
            priceNum = priceNum / 1000000;
            priceText = (Math.round(priceNum * 100) / 100) + " м.";
            return priceText;
        };
        const dateFormatter = unixTime => {
            return format(new Date(unixTime), "dd.MM.yyyy");
        };
        const tooltipFormatter = (value, name, props) => {
            let formattedValue = value;
            let formattedName = name;

            if (formattedValue)
                formattedValue = value + ' руб.';

            if (name === 'yFirstLayer')
                formattedName = '1-комнатные';
            if (name === 'ySecondLayer')
                formattedName = '2-комнатные';
            if (name === 'yThirdLayer')
                formattedName = '3-комнатные';
            if (name === 'yFourthLayer')
                formattedName = '4-комнатные';

            return [formattedValue, formattedName, props];
        };

        if (isInfoLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <p>Загрузка графика цен...</p>
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            return (
                <div style={{ display: "inline-block", width: "48%" }}>
                    <Divider orientation={"center"}>Изменение средних цен на квартиры в доме</Divider>
                    <ResponsiveContainer width="100%" height={300}>
                        <LineChart width={900} height={250} data={allRoomPrices} margin={{ top: 10, bottom: 10 }}>
                            <XAxis dataKey="xAxis" hasTicks scale="time" type="number" tickFormatter={dateFormatter} domain={xDomain}/>
                            <YAxis tickCount={7} hasTick tickFormatter={priceFormatter} domain={yDomain}/>
                            <CartesianGrid strokeDasharray="3 3" />
                            <Tooltip formatter={tooltipFormatter} labelFormatter={dateFormatter}/>
                            <Line type="monotone" dataKey="yFirstLayer" stroke="#4260f5" strokeWidth={2}/>
                            <Line type="monotone" dataKey="ySecondLayer" stroke="#ab6c07" strokeWidth={2}/>
                            <Line type="monotone" dataKey="yThirdLayer" stroke="#1ec726" strokeWidth={2}/>
                            <Line type="monotone" dataKey="yFourthLayer" stroke="#c71e48" strokeWidth={2}/>
                        </LineChart>
                    </ResponsiveContainer>
                </div>
            );
        } else {
            return (
                <div>
                    Ошибка загрузки графика: {this.props.error.toString()}
                </div>
            );
        }
    }
}

let mapStateToProps = (state, ownProps) => {
    return {
        allRoomPrices: state.flatPriceStatisticsPlotReducer.allRoomPrices,
        error: state.flatPriceStatisticsPlotReducer.error,
        isInfoLoading: state.flatPriceStatisticsPlotReducer.isInfoLoading,
        siteName: ownProps.siteName
    };
};

let mapActionsToProps = (dispatch, ownProps) => {
    return {
        getAvgPrices: (houseId) => dispatch(getAvgPrices(houseId, ownProps.siteName))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(FlatPriceStatisticsPlot);
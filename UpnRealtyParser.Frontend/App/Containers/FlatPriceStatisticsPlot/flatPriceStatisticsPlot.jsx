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
        let isBaseInfoLoading = this.props.isBaseInfoLoading;
        let errorMessage = this.props.error;
        let singleRoomPrices = this.props.singleRoomPrices;
        singleRoomPrices.forEach(d => {
            d.xAxis = moment(d.xAxis).valueOf(); // date -> epoch
        });

        const xDomain = [dataMin => dataMin, () => new Date(2021, 3, 28).getTime()];
        const yDomain = [dataMin => dataMin - 500000, dataMax => dataMax];

        const dateFormatter = unixTime => {
            return format(new Date(unixTime), "dd.MM.yyyy");
        };

        if (isBaseInfoLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <p>Загрузка графика цен...</p>
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            return (
                <div>
                    <Divider orientation={"center"}>Изменение средних цен на квартиры в доме</Divider>
                    <ResponsiveContainer width="90%" height={200}>
                        <LineChart width={900} height={250} data={singleRoomPrices} margin={{ top: 10, bottom: 10 }}>
                            <XAxis dataKey="xAxis" hasTicks scale="time" type="number" tickFormatter={dateFormatter} domain={xDomain}/>
                            <YAxis tickCount={7} hasTick domain={yDomain}/>
                            <CartesianGrid strokeDasharray="3 3" />
                            <Tooltip />
                            <Line type="monotone" dataKey="yAxis" stroke="#00192d" />
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

let mapStateToProps = (state) => {
    return {
        singleRoomPrices: state.flatPriceStatisticsPlotReducer.singleRoomPrices,
        twoRoomPrices: state.flatPriceStatisticsPlotReducer.twoRoomPrices,
        threeRoomPrices: state.flatPriceStatisticsPlotReducer.threeRoomPrices,
        fourRoomPrices: state.flatPriceStatisticsPlotReducer.fourRoomPrices,
        error: state.flatPriceStatisticsPlotReducer.error,
        isBaseInfoLoading: state.flatPriceStatisticsPlotReducer.isBaseInfoLoading
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAvgPrices: (houseId, roomAmount) => dispatch(getAvgPrices(houseId, roomAmount))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(FlatPriceStatisticsPlot);
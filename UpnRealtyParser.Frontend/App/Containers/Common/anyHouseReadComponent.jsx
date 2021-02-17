﻿import React from 'react';
import ReactDOM from 'react-dom';
import { withRouter } from "react-router-dom";
import { Divider, Spin, Button, Breadcrumb } from 'antd';
import SingleHouseInfo from '../../Stateless/singleHouseInfo.jsx';
import SingleHouseStatistics from '../../Stateless/singleHouseStatistics.jsx';

/** Возврат на страницу с перечнем домов без перезагрузки страницы */
function returnToHousesPage() {
    this.props.history.push("/houses" );
}

class AnyHouseRead extends React.Component {
    componentDidMount() {
        const id = this.props.match.params.id;
        this.props.getHouse(id);
        returnToHousesPage = returnToHousesPage.bind(this);
        this.props.getStatistics(id);
    }

    render() {
        let houseData = this.props.houseInfo;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;

        let isStatisticsLoading = this.props.isStatisticsLoading;
        let houseStatistics = this.props.houseStatistics;

        if (isLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            return (
                <div>
                    <Breadcrumb style={{ margin: '16px 0' }}>
                        <Breadcrumb.Item>Дома</Breadcrumb.Item>
                        <Breadcrumb.Item>УПН</Breadcrumb.Item>
                    </Breadcrumb>
                    <Divider orientation={"center"}>Информация о доме УПН, ID {houseData.id}</Divider>
                    <SingleHouseInfo houseData={houseData} />
                    {
                        (isStatisticsLoading || !houseStatistics) &&
                        <div className="centered-content-div-w-margin">
                            <p>Подсчет статистики...</p>
                            <Spin size="large" />
                        </div>
                    }
                    {
                        !isStatisticsLoading && houseStatistics &&
                        <SingleHouseStatistics houseStatistics={houseStatistics} />
                    }
                    <div style={{ marginTop: "15px", marginLeft: "auto", marginRight: "auto", textAlign: "center" }}>
                        <Button onClick={returnToHousesPage}>К списку домов</Button>
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    Ошибка загрузки дома: {this.props.error.toString()}
                </div>
            );
        }
    }
}

export default withRouter(AnyHouseRead);
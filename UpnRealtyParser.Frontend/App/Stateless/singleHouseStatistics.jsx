﻿import React from 'react';

class SingleHouseStatistics extends React.Component {
    componentDidMount() {
        setTimeout(function () {
            let el = document.querySelector('.house-statistics-wrapper');
            if(el)
                el.classList.add('full-visible');
        }, 10);
    }

    render() {
        let houseStatistics = this.props.houseStatistics;

        return (
            <div className="house-statistics-wrapper">
                {
                    houseStatistics &&
                    <div className="house-statistics-block">
                        <h3>1-комнатные квартиры</h3>
                        <hr></hr>
                        <p><b>Средняя цена: </b>{houseStatistics.averageSingleRoomSellPrice || "н/у"} руб.</p>
                        <p><b>Средний метраж: </b>{houseStatistics.averageSingleRoomSpace || "н/у"} кв. м.</p>
                        <p><b>Средняя цена за кв. м.: </b>{houseStatistics.averageSingleRoomMeterPrice || "н/у"} руб. за кв. м.</p>
                        <p><b>Средняя аренда: </b>{houseStatistics.averageSingleRoomRentPrice || "н/у"} руб. в мес.</p>
                    </div>
                }
                {
                    houseStatistics &&
                    <div className="house-statistics-block">
                        <h3>2-комнатные квартиры</h3>
                        <hr></hr>
                        <p><b>Средняя цена: </b>{houseStatistics.averageTwoRoomSellPrice || "н/у"} руб.</p>
                        <p><b>Средний метраж: </b>{houseStatistics.averageTwoRoomSpace || "н/у"} кв. м.</p>
                        <p><b>Средняя цена за кв. м.: </b>{houseStatistics.averageTwoRoomMeterPrice || "н/у"} руб. за кв. м.</p>
                        <p><b>Средняя аренда: </b>{houseStatistics.averageTwoRoomRentPrice || "н/у"} руб. в мес.</p>
                    </div>
                }
                {
                    houseStatistics &&
                    <div className="house-statistics-block">
                        <h3>3-комнатные квартиры</h3>
                        <hr></hr>
                        <p><b>Средняя цена: </b>{houseStatistics.averageThreeRoomSellPrice || "н/у"} руб.</p>
                        <p><b>Средний метраж: </b>{houseStatistics.averageThreeRoomSpace || "н/у"} кв. м.</p>
                        <p><b>Средняя цена за кв. м.: </b>{houseStatistics.averageThreeRoomMeterPrice || "н/у"} руб. за кв. м.</p>
                        <p><b>Средняя аренда: </b>{houseStatistics.averageThreeRoomRentPrice || "н/у"} руб. в мес.</p>
                    </div>
                }
                {
                    houseStatistics &&
                    <div className="house-statistics-block">
                        <h3>4-комнатные квартиры</h3>
                        <hr></hr>
                        <p><b>Средняя цена: </b>{houseStatistics.averageFourRoomSellPrice || "н/у"} руб.</p>
                        <p><b>Средний метраж: </b>{houseStatistics.averageFourRoomSpace || "н/у"} кв. м.</p>
                        <p><b>Средняя цена за кв. м.: </b>{houseStatistics.averageFourRoomMeterPrice || "н/у"} руб. за кв. м.</p>
                    </div>
                }
            </div>
        );
    }
}

export default SingleHouseStatistics;
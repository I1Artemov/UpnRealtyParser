import React from 'react';
import ReactDOM from 'react-dom';
import { withRouter } from "react-router-dom";
import { Divider, Spin, Button, Breadcrumb } from 'antd';
import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import SingleHouseInfo from '../../Stateless/singleHouseInfo.jsx';
import SingleHouseStatistics from '../../Stateless/singleHouseStatistics.jsx';
import FlatPriceStatisticsPlot from '../FlatPriceStatisticsPlot/flatPriceStatisticsPlot.jsx';

import L from 'leaflet';

const customMarker = new L.icon({
    iconUrl: '/images/leaf-marker.png',
    iconSize: [25, 41],
    iconAnchor: [12, 20]
});

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
        let houseId = this.props.match.params.id;
        let houseData = this.props.houseInfo;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;
        let centerLatitude = houseData.latitude === undefined ? 56.8519 : houseData.latitude;
        let centerLongitude = houseData.longitude === undefined ? 60.6122 : houseData.longitude;

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
                    <FlatPriceStatisticsPlot houseId={houseId} />

                    <div style={{ display: "inline-block", width: "48%", float: "right" }}>
                        <Divider orientation={"center"}>Расположение дома</Divider>
                        <MapContainer center={[centerLatitude, centerLongitude]} zoom={13} scrollWheelZoom={false}
                            style={{ height: "270px", width: "100%" }}>
                            <TileLayer
                                attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                            />
                            <Marker position={[centerLatitude, centerLongitude]} icon={customMarker}></Marker>
                        </MapContainer>
                    </div>
                    <div style={{ clear: "both" }}></div>

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
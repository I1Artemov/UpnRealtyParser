import React from 'react';
import ReactDOM from 'react-dom';
import { withRouter } from "react-router-dom";
import { Divider, Spin, Button, Breadcrumb } from 'antd';
import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import SingleFlatInfo from '../../Stateless/singleFlatInfo.jsx';

import L from 'leaflet';

const customMarker = new L.icon({
    iconUrl: '/images/leaf-marker.png',
    iconSize: [25, 41],
    iconAnchor: [12, 20]
});

/** Возврат на страницу с перечнем квартир без перезагрузки страницы */
function returnToFlatsPage() {
    let actionName = this.props.isRent ? "rentflats" : "sellflats";
    if (this.props.siteName === "n1")
        actionName = this.props.isRent ? "n1rentflats" : "n1sellflats";
    this.props.history.push("/" + actionName);
}

class AnyFlatRead extends React.Component {
    componentDidMount() {
        const id = this.props.match.params.id;
        this.props.startReceivingFlat();
        this.props.getFlat(id);
        returnToFlatsPage = returnToFlatsPage.bind(this);
    }

    render() {
        let flatData = this.props.flatInfo;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;
        let isShowPhotos = this.props.isShowApartmentPhotos;
        let breadcrumbHeader = this.props.isRent ? "Квартира в аренду" : "Квартира на продажу";
        let siteName = this.props.siteName;

        if (isLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            // Без этого могут попасть значения undefined и компонент не отрендерится
            let centerLatitude = flatData.houseLatitude ? flatData.houseLatitude : 56.8519;
            let centerLongitude = flatData.houseLongitude ? flatData.houseLongitude : 60.6122;
            let mapContainerWidth = flatData.downloadedPhotoHref ? "69%" : "100%";
            return (
                <div>
                    <Breadcrumb style={{ margin: '16px 0' }}>
                        <Breadcrumb.Item>{siteName}</Breadcrumb.Item>
                        <Breadcrumb.Item>{breadcrumbHeader}</Breadcrumb.Item>
                    </Breadcrumb>
                    <Divider orientation={"center"}>{breadcrumbHeader}, ID {flatData.id}</Divider>
                    <SingleFlatInfo flatData={flatData} siteName={siteName}/>
                    {/* TODO: Отдельный компонент для отображения фотографий */}
                    <div style={{ marginTop: "10px", marginBottom: "10px", textAlign: "center"}}>
                        {
                            flatData.photoCount > 0 && !isShowPhotos &&
                            <div style={{ marginLeft: "auto", marginRight: "auto", textAlign: "center" }}>
                                <Button type="primary" onClick={() => this.props.showFlatPhotos()}>Показать фото ({flatData.photoCount})</Button>
                            </div>
                        }
                        {
                            isShowPhotos &&
                            flatData.photoHrefs.map((photoHref) => {
                                return (<img key={photoHref} className="flat-photo-medium" src={photoHref}/>);
                            })
                        }
                    </div>

                    {
                        flatData.downloadedPhotoHref &&
                        <img style={{ display: "inline-block", maxWidth: "29%", height: "360px", position: "relative", float: "left" }}
                            key={flatData.downloadedPhotoHref} src={flatData.downloadedPhotoHref} />
                    }
                    <MapContainer center={[centerLatitude, centerLongitude]} zoom={13} scrollWheelZoom={false}
                        style={{ height: "360px", width: mapContainerWidth, display: "inline-block", float: "right" }}>
                        <TileLayer
                            attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                        />
                        {
                        flatData.houseLatitude && flatData.houseLongitude &&
                        <Marker position={[flatData.houseLatitude, flatData.houseLongitude]} icon={customMarker}></Marker>
                        }
                    </MapContainer>
                    <div style={{ clear: "both" }}></div>
                    <div style={{ marginTop: "15px", marginLeft: "auto", marginRight: "auto", textAlign: "center" }}>
                        <Button onClick={returnToFlatsPage}>К списку квартир</Button>
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    Ошибка загрузки квартиры: {this.props.error.toString()}
                </div>
            );
        }
    }
}

export default withRouter(AnyFlatRead);
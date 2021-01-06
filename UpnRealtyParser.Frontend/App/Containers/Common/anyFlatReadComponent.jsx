import React from 'react';
import ReactDOM from 'react-dom';
import { Divider, Spin, Button, Breadcrumb } from 'antd';
import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import { Href_UpnSellFlatController_GetSingleFlat } from "../../const.jsx";
import SingleFlatInfo from '../../Stateless/singleFlatInfo.jsx';

import L from 'leaflet';

const customMarker = new L.icon({
    iconUrl: '/images/leaf-marker.png',
    iconSize: [25, 41],
    iconAnchor: [12, 20]
}); 

export default class AnyFlatRead extends React.Component {
    componentDidMount() {
        const id = this.props.match.params.id;
        this.props.startReceivingFlat();
        this.props.getFlat(id, Href_UpnSellFlatController_GetSingleFlat);
    }

    render() {
        let flatData = this.props.flatInfo;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;
        let isShowPhotos = this.props.isShowApartmentPhotos;
        let breadcrumbHeader = this.props.isRent ? "Квартира в аренду" : "Квартира на продажу";

        if (isLoading === true) {
            return (
                <div className="centered-content-div-w-margin">
                    <Spin size="large" />
                </div>
            );
        } else if (errorMessage === null || errorMessage === "" || errorMessage === undefined) {
            // Без этого могут попасть значения undefined и компонент не отрендерится
            let centerLatitude = flatData.houseLatitude === undefined ? 56.8519 : flatData.houseLatitude;
            let centerLongitude = flatData.houseLongitude === undefined ? 60.6122 : flatData.houseLongitude;
            return (
                <div>
                    <Breadcrumb style={{ margin: '16px 0' }}>
                        <Breadcrumb.Item>Upn</Breadcrumb.Item>
                        <Breadcrumb.Item>{breadcrumbHeader}</Breadcrumb.Item>
                    </Breadcrumb>
                    <Divider orientation={"center"}>{breadcrumbHeader}, ID {flatData.id}</Divider>
                    <SingleFlatInfo flatData={flatData} />
                    {/* TODO: Отдельный компонент для отображения фотографий */}
                    <div style={{marginTop: "10px", textAlign: "center"}}>
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
                    <MapContainer center={[centerLatitude, centerLongitude]} zoom={13} scrollWheelZoom={false} style={{ height: "360px", marginTop: "10px" }}>
                        <TileLayer
                          attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                        />
                        <Marker position={[flatData.houseLatitude, flatData.houseLongitude]} icon={customMarker}></Marker>
                    </MapContainer>
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
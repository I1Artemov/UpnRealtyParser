import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllPoints } from "./paybackMapActions.jsx";
import { Divider, Spin } from 'antd';
import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import L from 'leaflet';

const customMarker = new L.icon({
    iconUrl: '/images/leaf-marker.png',
    iconSize: [25, 41],
    iconAnchor: [12, 20]
});

import 'antd/dist/antd.css';

class PaybackMap extends React.Component {
    componentDidMount() {
        this.props.getAllPoints();
    }

    render() {
        let points = this.props.points;
        let isLoading = this.props.isLoading;
        let errorMessage = this.props.error;
        let centerLatitude = 56.8519;
        let centerLongitude = 60.6122;

        let markers = points.map((point, i) =>
            <Marker key={i} position={[point.latitude, point.longitude]} icon={customMarker}></Marker>);

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
                    
                        <MapContainer center={[centerLatitude, centerLongitude]} zoom={13} scrollWheelZoom={false}
                            style={{ height: "270px", width: "100%" }}>
                            <TileLayer
                                attribution='&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                            />
                            
                            {markers}
                        </MapContainer>
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
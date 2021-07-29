import React, { useEffect } from 'react';
import L from 'leaflet';
import "leaflet.heat";

const customMarker = new L.icon({
    iconUrl: '/images/circle-icon-16.png',
    iconSize: [8, 8],
    iconAnchor: [4, 4]
});

export default function HeatmapFunction(props) {
    useEffect(() => {
        var markersLayer = new L.LayerGroup();
        var map = L.map("map", { layers: markersLayer })
            .setView([56.8519, 60.6122], 12);

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            attribution:
                '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);

        // Точки для тепловой карты
        const heatPoints = props.points
            ? props.points.map((p) => {
                return [p.latitude, p.longitude, p.normalizedPaybackYears]; // lat lng intensity
            }) : [];

        // Маркеры домов со всплывающими уведомлениями
        props.points.forEach((p) => {
            var popupText = "<a href='/upnhouse/" + p.houseId + "'>" + p.houseAddress + "</a>, окупаемость - " +
                Math.floor(p.paybackYears * 10) / 10 + " лет";
            var marker = L.marker([p.latitude, p.longitude], { icon: customMarker })
                .bindPopup(popupText);
            markersLayer.addLayer(marker);
        });

        L.heatLayer(heatPoints).addTo(map);
    }, []);

    return <div id="map" style={{ height: "512px" }}></div>;
}
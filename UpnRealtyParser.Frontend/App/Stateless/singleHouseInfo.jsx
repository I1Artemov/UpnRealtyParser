import React from 'react';
import { Link } from 'react-router-dom';
import { Descriptions } from 'antd';

class SingleHouseInfo extends React.Component {
    render() {
        let houseData = this.props.houseData;
        let siteName = this.props.siteName;
        let similarHouseUrl = null;
        let otherSiteName = siteName === "upn" ? "Похожий дом N1" : "Похожий дом UPN";
        if (houseData.similarHouseFromDifferentSiteId) {
            similarHouseUrl = siteName === "upn" ?
                "/n1house/" + houseData.similarHouseFromDifferentSiteId :
                "/upnhouse/" + houseData.similarHouseFromDifferentSiteId;
        }

        return (
            <div>
                <Descriptions bordered column={3}>
                    <Descriptions.Item label="Создан">{houseData.creationDatePrintable}</Descriptions.Item>
                    <Descriptions.Item label="Адрес" span={2}>{houseData.address}</Descriptions.Item>

                    <Descriptions.Item label="Тип">{houseData.houseType}</Descriptions.Item>
                    <Descriptions.Item label="Построен">{houseData.buildYear}</Descriptions.Item>
                    <Descriptions.Item label="Материал">{houseData.wallMaterial}</Descriptions.Item>

                    <Descriptions.Item label="Этажей">{houseData.maxFloor}</Descriptions.Item>
                    <Descriptions.Item label="Широта">{houseData.latitude}</Descriptions.Item>
                    <Descriptions.Item label="Долгота">{houseData.longitude}</Descriptions.Item>
                    {
                        similarHouseUrl &&
                        <Descriptions.Item label={otherSiteName} span={3}>
                            <Link to={similarHouseUrl}>{similarHouseUrl}</Link>
                        </Descriptions.Item>
                    }
                    {
                        houseData.builderCompany &&
                        <Descriptions.Item label="Застройщик" span={3}>{houseData.builderCompany}</Descriptions.Item>
                    }
                </Descriptions>
            </div>
        );
    }
}

export default SingleHouseInfo;
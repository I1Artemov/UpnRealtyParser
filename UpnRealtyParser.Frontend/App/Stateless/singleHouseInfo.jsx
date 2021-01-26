import React from 'react';
import { Descriptions } from 'antd';

class SingleHouseInfo extends React.Component {
    render() {
        let houseData = this.props.houseData;

        return (
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
                    houseData.builderCompany &&
                    <Descriptions.Item label="Застройщик" span={3}>{houseData.builderCompany}</Descriptions.Item>
                }
            </Descriptions>
        );
    }
}

export default SingleHouseInfo;
import React from 'react';
import { Row, Col, Descriptions } from 'antd';

class SingleFlatInfo extends React.Component {
    render() {
        let flatData = this.props.flatData;
        let upnSiteHref = "https://upn.ru" + this.props.flatData.siteUrl;

        return (
            <Descriptions bordered>
                <Descriptions.Item label="Адрес" span={2}>{flatData.houseAddress}</Descriptions.Item>
                <Descriptions.Item label="Цена">{flatData.price} руб.</Descriptions.Item>
                <Descriptions.Item label="Этаж">{flatData.floorSummary}</Descriptions.Item>
                <Descriptions.Item label="Общая площадь">{flatData.spaceSum}</Descriptions.Item>
                <Descriptions.Item label="Раздельные сан. узлы">{flatData.separateBathrooms}</Descriptions.Item>
                <Descriptions.Item label="Совмещенные сан. узлы">{flatData.jointBathrooms}</Descriptions.Item>
                <Descriptions.Item label="Ремонт">{flatData.renovationType}</Descriptions.Item>
                <Descriptions.Item label="Перепланировка">{flatData.redevelopmentType}</Descriptions.Item>
                <Descriptions.Item label="Стеклопакеты">{flatData.windowsType}</Descriptions.Item>
                <Descriptions.Item label="Мебель">{flatData.furniture}</Descriptions.Item>
                <Descriptions.Item label="Ближайшее метро">{flatData.subwaySummary}</Descriptions.Item>
                <Descriptions.Item label="Тип дома">{flatData.houseType}</Descriptions.Item>
                <Descriptions.Item label="Материал дома">{flatData.houseWallMaterial}</Descriptions.Item>
                <Descriptions.Item label="Год постройки">{flatData.houseBuildYear}</Descriptions.Item>
                <Descriptions.Item label="Агентство">{flatData.agencyName}</Descriptions.Item>
                <Descriptions.Item label="Добавлена/проверена">{flatData.createdCheckedDatesSummary}</Descriptions.Item>
                <Descriptions.Item label="Ссылка на сайт" span={2}>{upnSiteHref}</Descriptions.Item>
                <Descriptions.Item label="Описание" span={3}>{flatData.description}</Descriptions.Item>
            </Descriptions>
        );
    }
}

export default SingleFlatInfo;
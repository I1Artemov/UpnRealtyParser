import React from 'react';
import { Row, Col, Descriptions } from 'antd';

class SingleFlatInfo extends React.Component {
    render() {
        let flatData = this.props.flatData;
        let siteName = this.props.siteName;

        let siteHrefStart = siteName === "upn" ? "https://upn.ru" : "https://ekaterinburg.n1.ru";
        let upnSiteHref = siteHrefStart + flatData.siteUrl;

        let housePageUrl = siteName === "upn" ?
            '/upnhouse/' + flatData.upnHouseInfoId :
            '/n1house/' + flatData.n1HouseInfoId;

        return (
            <Descriptions bordered column={3}>
                <Descriptions.Item label="Адрес" span={2}>
                    <a href={housePageUrl}>{flatData.houseAddress}</a>
                </Descriptions.Item>
                <Descriptions.Item label="Цена">{flatData.price} руб.</Descriptions.Item>
                <Descriptions.Item label="Этаж">{flatData.floorSummary}</Descriptions.Item>
                <Descriptions.Item label="Общая площадь">{flatData.spaceSum}</Descriptions.Item>
                {siteName === "upn" && <Descriptions.Item label="Раздельные сан. узлы">{flatData.separateBathrooms}</Descriptions.Item>}
                {siteName === "upn" && <Descriptions.Item label="Совмещенные сан. узлы">{flatData.jointBathrooms}</Descriptions.Item>}
                {siteName === "upn" && <Descriptions.Item label="Ремонт">{flatData.renovationType}</Descriptions.Item>}
                {siteName === "upn" && <Descriptions.Item label="Перепланировка">{flatData.redevelopmentType}</Descriptions.Item>}
                {siteName === "upn" && <Descriptions.Item label="Стеклопакеты">{flatData.windowsType}</Descriptions.Item>}
                {siteName === "upn" && <Descriptions.Item label="Мебель">{flatData.furniture}</Descriptions.Item>}
        
                {siteName === "n1" && <Descriptions.Item label="Тип сан. узла">{flatData.bathroomType}</Descriptions.Item>}
                {siteName === "n1" && <Descriptions.Item label="Планировка">{flatData.planningType}</Descriptions.Item>}
                {siteName === "n1" && <Descriptions.Item label="Число балконов">{flatData.balconyAmount}</Descriptions.Item>}
                {siteName === "n1" && <Descriptions.Item label="Условие">{flatData.condition}</Descriptions.Item>}
                {siteName === "n1" && <Descriptions.Item label="Тип собственности">{flatData.propertyType}</Descriptions.Item>}
                {siteName === "n1" && <Descriptions.Item label="Заполнена полностью">{flatData.isFilledCompletely}</Descriptions.Item>}
                
                <Descriptions.Item label="Ближайшее метро">{flatData.subwaySummary}</Descriptions.Item>
                <Descriptions.Item label="Тип дома">{flatData.houseType}</Descriptions.Item>
                <Descriptions.Item label="Материал дома">{flatData.houseWallMaterial}</Descriptions.Item>
                <Descriptions.Item label="Год постройки">{flatData.houseBuildYear}</Descriptions.Item>
                <Descriptions.Item label="Агентство">{flatData.agencyName}</Descriptions.Item>
                <Descriptions.Item label="Добавлена/проверена">{flatData.createdCheckedDatesSummary}</Descriptions.Item>
                <Descriptions.Item label="Ссылка на сайт">{upnSiteHref}</Descriptions.Item>
                <Descriptions.Item label="Описание" span={3}>{flatData.description}</Descriptions.Item>
            </Descriptions>
        );
    }
}

export default SingleFlatInfo;
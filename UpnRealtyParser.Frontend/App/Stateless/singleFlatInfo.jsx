import React from 'react';
import { Row, Col } from 'antd';

class SingleFlatInfo extends React.Component {
    render() {
        let flatData = this.props.flatData;

        return (
            <div>
                <Row>
                    <Col span={4}>Адрес</Col>
                    <Col span={20}>{flatData.houseAddress}</Col>
                </Row>
                <Row>
                    <Col span={4}>Цена</Col>
                    <Col span={20}>{flatData.price} руб.</Col>
                </Row>
                <Row>
                    <Col span={4}>Этаж</Col>
                    <Col span={20}>{flatData.floorSummary}</Col>
                </Row>
                <Row>
                    <Col span={4}>Общая площадь</Col>
                    <Col span={20}>{flatData.spaceSum}</Col>
                </Row>
                <Row>
                    <Col span={4}>Раздельные сан. узлы</Col>
                    <Col span={20}>{flatData.separateBathrooms}</Col>
                </Row>
                <Row>
                    <Col span={4}>Совмещенные сан. узлы</Col>
                    <Col span={20}>{flatData.jointBathrooms}</Col>
                </Row>
                <Row>
                    <Col span={4}>Ремонт</Col>
                    <Col span={20}>{flatData.renovationType}</Col>
                </Row>
                <Row>
                    <Col span={4}>Перепланировка</Col>
                    <Col span={20}>{flatData.redevelopmentType}</Col>
                </Row>
                <Row>
                    <Col span={4}>Стеклопакеты</Col>
                    <Col span={20}>{flatData.windowsType}</Col>
                </Row>
                <Row>
                    <Col span={4}>Мебель</Col>
                    <Col span={20}>{flatData.furniture}</Col>
                </Row>
                <Row>
                    <Col span={4}>Ближайшее метро</Col>
                    <Col span={20}>{flatData.subwaySummary}</Col>
                </Row>

                <Row>
                    <Col span={4}>Тип дома</Col>
                    <Col span={20}>{flatData.houseType}</Col>
                </Row>
                <Row>
                    <Col span={4}>Материал дома</Col>
                    <Col span={20}>{flatData.houseWallMaterial}</Col>
                </Row>
                <Row>
                    <Col span={4}>Год постройки</Col>
                    <Col span={20}>{flatData.houseBuildYear}</Col>
                </Row>
                <Row>
                    <Col span={4}>Описание</Col>
                    <Col span={20}>{flatData.description}</Col>
                </Row>
                <Row>
                    <Col span={4}>Агентство</Col>
                    <Col span={20}>{flatData.agencyName}</Col>
                </Row>
                <Row>
                    <Col span={4}>Добавлена/проверена</Col>
                    <Col span={20}>{flatData.createdCheckedDatesSummary}</Col>
                </Row>
            </div>
        );
    }
}

export default SingleFlatInfo;
import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats, startReceivingFlats, setShowArchived } from './upnSellFlatIndexActions.jsx';
import { SELL_FLATS_TABLE_COLUMNS } from './upnSellFlatIndexConstants.jsx';
import { Table, Breadcrumb, Checkbox, Slider, InputNumber, Button } from 'antd';
import { SearchOutlined } from '@ant-design/icons';

import 'antd/dist/antd.css';

class UpnSellFlatIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingFlats();
        this.props.getAllFlats(new Object());
        // todo: bind
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingFlats();
        this.props.getAllFlats(pagination, this.props.isShowArchived);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;

        return (
            <div>
                <Breadcrumb style={{ margin: '26px 0' }}>
                    <Breadcrumb.Item>Upn</Breadcrumb.Item>
                    <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                    <Breadcrumb.Item>На продажу</Breadcrumb.Item>
                </Breadcrumb>
                <div style={{ marginBottom: 8, paddingBottom: 10, paddingTop: 10, paddingLeft: 8, backgroundColor: "rgb(255, 255, 255)" }}>
                    <span>Отображать архивные</span>
                    <Checkbox onChange={this.props.setShowArchived.bind(this)} checked={this.props.isShowArchived} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                    <span>Диапазон цен, тыс. руб.</span>
                    <Slider range min={100} max={4000} defaultValue={[1000, 2000]}
                        style={{ width: 200, display: 'inline-block', marginLeft: 9, marginRight: 28, marginBottom: 0 }} />

                    <span>Не первый этаж</span>
                    <Checkbox disabled defaultChecked={false} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                    <span>Не последний этаж</span>
                    <Checkbox disabled defaultChecked={false} style={{ marginLeft: 9, marginRight: 28 }}></Checkbox>

                    <span>Год постройки от</span>
                    <InputNumber disabled min={1930} max={2020} defaultValue={null} style={{ marginLeft: 9, marginRight: 28 }} />

                    <Button onClick={this.handleTableChange.bind(this)} type="primary" icon={<SearchOutlined />}>Применить</Button>
                </div>
                <Table
                    dataSource={flatsData}
                    columns={SELL_FLATS_TABLE_COLUMNS}
                    onChange={this.handleTableChange.bind(this)}
                    pagination={{total: totalFlatsCount}}
                    loading={isFlatsLoading}
                    rowClassName={(record, index) => (record.isArchived === true ? "archived-flat" : "active-flat")}
                    />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnSellFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnSellFlatIndexReducer.totalFlatsCount,
        error: state.upnSellFlatIndexReducer.error,
        isFlatsLoading: state.upnSellFlatIndexReducer.isFlatsLoading,
        isShowArchived: state.upnSellFlatIndexReducer.isShowArchived
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, isShowArchived) => dispatch(getAllFlats(pagination, isShowArchived)),
        startReceivingFlats: () => dispatch(startReceivingFlats()),
        setShowArchived: (ev) => dispatch(setShowArchived(ev))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
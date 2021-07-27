import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats, startReceivingFlats } from '../Common/anyFlatIndexActions.jsx';
import { SELL_FLATS_TABLE_COLUMNS } from './n1SellFlatIndexConstants.jsx';
import { Table, Breadcrumb } from 'antd';
import { Href_N1SellFlatController_GetAllFlats } from "../../const.jsx";
import FlatsSearchBar from '../FlatsSearchBar/flatsSearchBar.jsx';
import { SiteTitle } from '../../const.jsx';

import 'antd/dist/antd.css';

class N1SellFlatIndex extends React.Component {
    componentDidMount() {
        this.props.startReceivingFlats();
        this.props.getAllFlats(new Object(), null, this.props.filteringInfo);
        document.title = SiteTitle + " - Квартиры N1";
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.startReceivingFlats();
        this.props.getAllFlats(pagination, sorter, this.props.filteringInfo);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;
        let isFlatsLoading = this.props.isFlatsLoading;

        return (
            <div>
            <Breadcrumb style={{ margin: '16px 0' }}>
                <Breadcrumb.Item>N1</Breadcrumb.Item>
                <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                <Breadcrumb.Item>На продажу</Breadcrumb.Item>
            </Breadcrumb>
            <FlatsSearchBar handleTableChange={this.handleTableChange.bind(this)} />
            <Table
                dataSource={flatsData}
                columns={SELL_FLATS_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{ total: totalFlatsCount }}
                loading={isFlatsLoading}
                rowClassName={(record, index) => (record.isArchived === 1 ? "archived-flat" : "active-flat")}
            />
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.n1SellFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.n1SellFlatIndexReducer.totalFlatsCount,
        error: state.n1SellFlatIndexReducer.error,
        isFlatsLoading: state.n1SellFlatIndexReducer.isFlatsLoading,
        filteringInfo: state.flatSearchBarReducer.filteringInfo
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination, sorter, filteringInfo) =>
            dispatch(getAllFlats(pagination, sorter, filteringInfo, Href_N1SellFlatController_GetAllFlats)),
        startReceivingFlats: () => dispatch(startReceivingFlats())
    };
};

export default connect(mapStateToProps, mapActionsToProps)(N1SellFlatIndex);
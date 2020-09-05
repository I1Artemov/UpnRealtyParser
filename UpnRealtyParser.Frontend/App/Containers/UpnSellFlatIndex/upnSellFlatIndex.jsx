import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats } from './upnSellFlatIndexActions.jsx';
import { RENT_FLATS_TABLE_COLUMNS } from './upnSellFlatIndexConstants.jsx';
import { Table } from 'antd';

import 'antd/dist/antd.css';
// import CommentList from '../CommentList/commentList.jsx';

class UpnSellFlatIndex extends React.Component {
    componentDidMount() {
        this.props.getAllFlats(new Object());
    }

    handleTableChange(pagination, filters, sorter) {
        this.props.getAllFlats(pagination);
    }

    render() {
        let flatsData = this.props.flatsInfo.map(item => ({ ...item, key: item.id }));
        let totalFlatsCount = this.props.totalFlatsCount;

        return (
            <Table
                dataSource={flatsData}
                columns={RENT_FLATS_TABLE_COLUMNS}
                onChange={this.handleTableChange.bind(this)}
                pagination={{total: totalFlatsCount}}
            />
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnSellFlatIndexReducer.flatsInfo,
        totalFlatsCount: state.upnSellFlatIndexReducer.totalFlatsCount,
        error: state.upnSellFlatIndexReducer.error
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pagination) => dispatch(getAllFlats(pagination))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
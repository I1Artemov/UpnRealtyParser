import React from 'react';
import ReactDOM from 'react-dom';
import { connect } from 'react-redux';
import { getAllFlats } from './upnSellFlatIndexActions.jsx';
// import CommentList from '../CommentList/commentList.jsx';

class UpnSellFlatIndex extends React.Component {
    componentDidMount() {
        this.props.getAllFlats(0);
    }

    render() {
        let flats = this.props.flatsInfo.map(item => {
            return (
                <div key={item.id} className="flat-info-block">
                    <div>№{item.id}</div>
                    <div className="flat-description">{item.description}</div>
                    <hr />
                </div>
            );
        });

        return (
            <div id="allFlatsWrapper">
                {flats}
            </div>
        );
    }
}

let mapStateToProps = (state) => {
    return {
        flatsInfo: state.upnSellFlatIndexReducer.flatsInfo,
        error: state.upnSellFlatIndexReducer.error
    };
};

let mapActionsToProps = (dispatch) => {
    return {
        getAllFlats: (pageNumber) => dispatch(getAllFlats(pageNumber))
    };
};

export default connect(mapStateToProps, mapActionsToProps)(UpnSellFlatIndex);
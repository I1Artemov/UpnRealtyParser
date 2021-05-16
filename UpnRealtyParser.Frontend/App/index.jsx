import React from 'react';
import { render } from 'react-dom';
import { createStore, applyMiddleware, combineReducers } from 'redux';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import App from './Containers/app.jsx';
import upnSellFlatIndexReducer from './Containers/UpnSellFlatIndex/upnSellFlatIndexReducer.jsx';
import upnRentFlatIndexReducer from './Containers/UpnRentFlatIndex/upnRentFlatIndexReducer.jsx';
import n1SellFlatIndexReducer from './Containers/N1SellFlatIndex/n1SellFlatIndexReducer.jsx';
import upnSellFlatReadReducer from './Containers/UpnSellFlatRead/upnSellFlatReadReducer.jsx';
import upnRentFlatReadReducer from './Containers/UpnRentFlatRead/upnRentFlatReadReducer.jsx';
import upnHouseIndexReducer from './Containers/UpnHouseIndex/upnHouseIndexReducer.jsx';
import upnAgencyIndexReducer from './Containers/UpnAgencyIndex/upnAgencyIndexReducer.jsx';
import webProxyIndexReducer from './Containers/WebProxyIndex/webProxyIndexReducer.jsx';
import logEntryIndexReducer from './Containers/LogEntryIndex/logEntryIndexReducer.jsx';
import flatPriceStatisticsPlotReducer from './Containers/FlatPriceStatisticsPlot/flatPriceStatisticsPlotReducer.jsx';
import anyHouseReadReducer from './Containers/Common/anyHouseReadReducer.jsx';
import paybackMapReducer from './Containers/PaybackMap/paybackMapReducer.jsx';

import './site.css';
import '../node_modules/leaflet/dist/leaflet.css';

const rootReducer = combineReducers({ 
    upnSellFlatIndexReducer,
    upnRentFlatIndexReducer,
    n1SellFlatIndexReducer,
    upnSellFlatReadReducer,
    upnRentFlatReadReducer,
	upnHouseIndexReducer,
	upnAgencyIndexReducer,
    webProxyIndexReducer,
    logEntryIndexReducer,
    anyHouseReadReducer,
    flatPriceStatisticsPlotReducer,
    paybackMapReducer
});

function configureStore(initialState) {
    return createStore(rootReducer, initialState, applyMiddleware(thunk));
}

const store = configureStore();

render(
    <Provider store={store}>
        <App />
    </Provider>,
    document.getElementById('content')
);
﻿import React from 'react';
import { render } from 'react-dom';
import { createStore, applyMiddleware, combineReducers } from 'redux';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import App from './Containers/app.jsx';
import { ConfigProvider } from 'antd';
import ruRU from 'antd/lib/locale/ru_RU';
import upnSellFlatIndexReducer from './Containers/UpnSellFlatIndex/upnSellFlatIndexReducer.jsx';
import upnRentFlatIndexReducer from './Containers/UpnRentFlatIndex/upnRentFlatIndexReducer.jsx';
import n1SellFlatIndexReducer from './Containers/N1SellFlatIndex/n1SellFlatIndexReducer.jsx';
import n1RentFlatIndexReducer from './Containers/N1RentFlatIndex/n1RentFlatIndexReducer.jsx';
import anySellFlatReadReducer from './Containers/AnySellFlatReadConnected/anySellFlatReadReducer.jsx';
import anyRentFlatReadReducer from './Containers/AnyRentFlatReadConnected/anyRentFlatReadReducer.jsx';
import upnHouseIndexReducer from './Containers/UpnHouseIndex/upnHouseIndexReducer.jsx';
import upnAgencyIndexReducer from './Containers/UpnAgencyIndex/upnAgencyIndexReducer.jsx';
import webProxyIndexReducer from './Containers/WebProxyIndex/webProxyIndexReducer.jsx';
import logEntryIndexReducer from './Containers/LogEntryIndex/logEntryIndexReducer.jsx';
import flatPriceStatisticsPlotReducer from './Containers/FlatPriceStatisticsPlot/flatPriceStatisticsPlotReducer.jsx';
import anyHouseReadReducer from './Containers/Common/anyHouseReadReducer.jsx';
import paybackMapReducer from './Containers/PaybackMap/paybackMapReducer.jsx';
import flatSearchBarReducer from './Containers/FlatsSearchBar/flatsSearchBarReducer.jsx';
import authenticationReducer from './Containers/UserService/authenticationReducer.jsx';

import './site.css';
import '../node_modules/leaflet/dist/leaflet.css';

const rootReducer = combineReducers({ 
    upnSellFlatIndexReducer,
    upnRentFlatIndexReducer,
    n1SellFlatIndexReducer,
    n1RentFlatIndexReducer,
    anySellFlatReadReducer,
    anyRentFlatReadReducer,
	upnHouseIndexReducer,
	upnAgencyIndexReducer,
    webProxyIndexReducer,
    logEntryIndexReducer,
    anyHouseReadReducer,
    flatPriceStatisticsPlotReducer,
    paybackMapReducer,
    flatSearchBarReducer,
    authenticationReducer
});

function configureStore(initialState) {
    return createStore(rootReducer, initialState, applyMiddleware(thunk));
}

const store = configureStore();

render(
    <ConfigProvider locale={ruRU}>
        <Provider store={store}>
            <App />
        </Provider>
    </ConfigProvider>,
    document.getElementById('content')
);
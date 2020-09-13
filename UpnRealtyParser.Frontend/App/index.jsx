import React from 'react';
import { render } from 'react-dom';
import { createStore, applyMiddleware, combineReducers } from 'redux';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import App from './Containers/app.jsx';
import upnSellFlatIndexReducer from './Containers/UpnSellFlatIndex/upnSellFlatIndexReducer.jsx';
import upnSellFlatReadReducer from './Containers/UpnSellFlatRead/upnSellFlatReadReducer.jsx';
import upnHouseIndexReducer from './Containers/UpnHouseIndex/upnHouseIndexReducer.jsx';
import upnAgencyIndexReducer from './Containers/UpnAgencyIndex/upnAgencyIndexReducer.jsx';
import webProxyIndexReducer from './Containers/WebProxyIndex/webProxyIndexReducer.jsx';

import './site.css';

const rootReducer = combineReducers({ 
    upnSellFlatIndexReducer,
    upnSellFlatReadReducer,
	upnHouseIndexReducer,
	upnAgencyIndexReducer,
	webProxyIndexReducer });

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
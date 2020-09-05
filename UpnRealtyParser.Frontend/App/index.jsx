import React from 'react';
import { render } from 'react-dom';
import { createStore, applyMiddleware, combineReducers } from 'redux';
import { Provider } from 'react-redux';
import thunk from 'redux-thunk';
import App from './Containers/app.jsx';
import upnSellFlatIndexReducer from './Containers/UpnSellFlatIndex/upnSellFlatIndexReducer.jsx';

import './site.css';

const rootReducer = combineReducers({ upnSellFlatIndexReducer });

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
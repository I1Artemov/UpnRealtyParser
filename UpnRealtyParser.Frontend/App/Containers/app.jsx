import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './Header/header.jsx';
import UpnSellFlatIndex from './UpnSellFlatIndex/upnSellFlatIndex.jsx';

export default class App extends React.Component {
    render() {
        return (
            <Router>
                <div>
                    <Header />
                    <main>
                        <Switch>
                            {/*<Route path="/" component={UpnHomeIndex} />*/}
                            <Route path="/sellflat" component={UpnSellFlatIndex} />
                            {/*
                            <Route path="/rentflat" component={UpnRentFlatIndex} />
                            <Route path="/house" component={UpnHouseIndex} />
                            <Route path="/agency" component={UpnAgencyIndex} />
                            <Route path="/webproxy" component={WebProxyIndex} />
                            <Route path="/log" component={LogMessagesIndex} />
                            */}
                        </Switch>
                    </main>
                </div>
            </Router>
        );
    }
};
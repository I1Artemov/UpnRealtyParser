import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './Header/header.jsx';
import UpnSellFlatIndex from './UpnSellFlatIndex/upnSellFlatIndex.jsx';
import UpnRentFlatIndex from './UpnRentFlatIndex/upnRentFlatIndex.jsx';
import N1SellFlatIndex from './N1SellFlatIndex/n1SellFlatIndex.jsx';
import UpnSellFlatRead from './UpnSellFlatRead/upnSellFlatRead.jsx';
import UpnRentFlatRead from './UpnRentFlatRead/upnRentFlatRead.jsx';
import UpnHouseIndex from './UpnHouseIndex/upnHouseIndex.jsx';
import UpnAgencyIndex from './UpnAgencyIndex/upnAgencyIndex.jsx';
import WebProxyIndex from './WebProxyIndex/webProxyIndex.jsx';
import LogEntryIndex from './LogEntryIndex/logEntryIndex.jsx';
import UpnHouseRead from './UpnHouseRead/upnHouseRead.jsx';
import SiteIndex from '../Stateless/siteIndex.jsx';

import { Layout } from 'antd';

const { Content, Footer } = Layout;

export default class App extends React.Component {
    render() {
        return (
            <Router>
                <div>
                    <Layout style={{ minHeight: '100vh' }}>
                        <Header/>
                        <Layout className="site-layout">
                            {/*<Header className="site-layout-background" style={{ padding: 0 }} />*/}
                            <Content style={{ margin: '0 16px' }}>
                                
                                <main>
                                    <Switch>
                                        <Route path="/sellflats" component={UpnSellFlatIndex} />
                                        <Route path="/houses" component={UpnHouseIndex} />
                                        <Route path="/agencies" component={UpnAgencyIndex} />
                                        <Route path="/webproxies" component={WebProxyIndex} />
                                        <Route path="/log" component={LogEntryIndex} />
                                        <Route path="/rentflats" component={UpnRentFlatIndex} />
                                        <Route path="/n1sellflats" component={N1SellFlatIndex} />
                                        {/*<Route path="/n1rentflats" component={N1RentFlatIndex} />*/}
                                        <Route exact path="/sellflat/:id" component={UpnSellFlatRead} />
                                        <Route exact path="/rentflat/:id" component={UpnRentFlatRead} />
                                        <Route exact path="/upnhouse/:id" component={UpnHouseRead} />
                                        <Route path="/" component={SiteIndex} />
                                    </Switch>
                                </main>
                            </Content>
                            <Footer style={{ textAlign: 'center' }}>UpnRealtyParser ©2021 Created by AIV</Footer>
                        </Layout>
                    </Layout>
                </div>
            </Router>
        );
    }
};
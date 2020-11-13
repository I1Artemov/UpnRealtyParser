import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './Header/header.jsx';
import UpnSellFlatIndex from './UpnSellFlatIndex/upnSellFlatIndex.jsx';
import UpnRentFlatIndex from './UpnRentFlatIndex/upnRentFlatIndex.jsx';
import UpnSellFlatRead from './UpnSellFlatRead/upnSellFlatRead.jsx';
import UpnHouseIndex from './UpnHouseIndex/upnHouseIndex.jsx';
import UpnAgencyIndex from './UpnAgencyIndex/upnAgencyIndex.jsx';
import WebProxyIndex from './WebProxyIndex/webProxyIndex.jsx';
import LogEntryIndex from './LogEntryIndex/logEntryIndex.jsx';

import { Layout, Breadcrumb } from 'antd';

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
                                <Breadcrumb style={{ margin: '16px 0' }}>
                                    <Breadcrumb.Item>Upn</Breadcrumb.Item>
                                    <Breadcrumb.Item>Квартиры</Breadcrumb.Item>
                                </Breadcrumb>
                                <main>
                                    <Switch>
                                        <Route path="/sellflats" component={UpnSellFlatIndex} />
                                        <Route path="/house" component={UpnHouseIndex} />
                                        <Route path="/agency" component={UpnAgencyIndex} />
                                        <Route path="/webproxy" component={WebProxyIndex} />
                                        <Route path="/log" component={LogEntryIndex} />
                                        <Route path="/rentflats" component={UpnRentFlatIndex} />
                                        <Route exact path="/sellflat/:id" component={UpnSellFlatRead} />
                                    </Switch>
                                </main>
                            </Content>
                            <Footer style={{ textAlign: 'center' }}>UpnRealtyParser ©2020 Created by AIV</Footer>
                        </Layout>
                    </Layout>
                </div>
            </Router>
        );
    }
};
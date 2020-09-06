import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './Header/header.jsx';
import UpnSellFlatIndex from './UpnSellFlatIndex/upnSellFlatIndex.jsx';
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
                            </Content>
                            <Footer style={{ textAlign: 'center' }}>UpnRealtyParser ©2020 Created by AIV</Footer>
                        </Layout>
                    </Layout>

                    
                </div>
            </Router>
        );
    }
};
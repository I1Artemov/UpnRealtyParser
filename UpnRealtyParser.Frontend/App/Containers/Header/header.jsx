import React from 'react';
import { Link } from 'react-router-dom';
import { Layout, Menu } from 'antd';

const { Sider } = Layout;
const { SubMenu } = Menu;

export default class Header extends React.Component {
    render() {
        return (
            <Sider>
                <div className="logo" />
                <Menu theme="dark" defaultSelectedKeys={['1']} mode="inline">
                    <Menu.Item key="1">
                        <Link to="/">Недвижимость УПН</Link>
                    </Menu.Item>
                    <SubMenu key="sub1" title="Квартиры">
                        <Menu.Item key="2">
                            <Link to="/sellflat">На продажу</Link>
                        </Menu.Item>
                        <Menu.Item key="3">
                            <Link to="/rentflat">В аренду</Link>
                        </Menu.Item>
                    </SubMenu>
                    <Menu.Item key="4">
                        <Link to="/house">Дома</Link>
                    </Menu.Item>
                    <Menu.Item key="5">
                        <Link to="/agency">Агентства</Link>
                    </Menu.Item>
                    <SubMenu key="sub3" title="Администрирование">
                        <Menu.Item key="6">
                            <Link to="/webproxy">Прокси</Link>
                        </Menu.Item>
                        <Menu.Item key="7">
                            <Link to="/log">Лог</Link>
                        </Menu.Item>
                    </SubMenu>
                </Menu>
            </Sider>
        );
    }
};
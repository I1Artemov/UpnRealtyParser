﻿import React from 'react';
import { Link } from 'react-router-dom';
import { Layout, Menu } from 'antd';
import { HomeOutlined, TeamOutlined, ToolOutlined, BankOutlined, EnvironmentOutlined } from '@ant-design/icons';
import UserBar from '../UserService/userbar.jsx';

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
                    <SubMenu icon={<BankOutlined />}  key="sub1" title="Квартиры UPN">
                        <Menu.Item key="2">
                            <Link to="/sellflats">На продажу</Link>
                        </Menu.Item>
                        <Menu.Item key="3">
                            <Link to="/rentflats">В аренду</Link>
                        </Menu.Item>
                    </SubMenu>
                    <SubMenu icon={<BankOutlined />} key="sub2" title="Квартиры N1">
                        <Menu.Item key="4">
                            <Link to="/n1sellflats">На продажу</Link>
                        </Menu.Item>
                        <Menu.Item key="5">
                            <Link to="/n1rentflats">В аренду</Link>
                        </Menu.Item>
                    </SubMenu>
                    <Menu.Item icon={<HomeOutlined/>} key="6">
                        <Link to="/houses">Дома УПН/N1</Link>
                    </Menu.Item>
                    <Menu.Item icon={<TeamOutlined />} key="7">
                        <Link to="/agencies">Агентства УПН/N1</Link>
                    </Menu.Item>
                    <SubMenu icon={<ToolOutlined />} key="sub3" title="Администрирование">
                        <Menu.Item key="8">
                            <Link to="/webproxies">Прокси</Link>
                        </Menu.Item>
                        <Menu.Item key="9">
                            <Link to="/log">Лог</Link>
                        </Menu.Item>
                    </SubMenu>
                    <Menu.Item icon={<EnvironmentOutlined />} key="10">
                        <Link to="/paybackmap">Карта окупаемости</Link>
                    </Menu.Item>
                </Menu>
                <UserBar/>
            </Sider>
        );
    }
};
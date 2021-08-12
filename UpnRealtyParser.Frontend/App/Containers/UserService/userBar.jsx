import React from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { UserOutlined, LogoutOutlined } from '@ant-design/icons';
import { doLogout } from './userActions.jsx';
import { createBrowserHistory } from 'history';

const history = createBrowserHistory({ forceRefresh: true });

class UserBar extends React.Component {
    handleLogout() {
        this.props.doLogout();
        history.push('/');
    }

    render() {
        let userInfoStr = localStorage.getItem('user');

        if (!userInfoStr) {
            return (
                <div id={"loginButton"}>
                    <Link to="/login">Войти</Link>
                </div>
            );
        }

        let parsedUserInfo = null;
        try {
            parsedUserInfo = JSON.parse(userInfoStr);
        } catch (ex) {
            console.error(ex);
        }
            
        return (
            <div id={"userSideBarWrapper"}>
                <div id={"userIconInSideBar"}><UserOutlined/></div>
                <div id={"userNameDiv"}>{parsedUserInfo.login}</div>
                <button id={"logoutButton"} onClick={this.handleLogout.bind(this)} title={"Выход"}><LogoutOutlined /></button>
            </div>
        );
    }
}

let mapActionsToProps = (dispatch) => {
    return {
        doLogout: () => dispatch(doLogout())
    };
};

export default connect(null, mapActionsToProps)(UserBar);
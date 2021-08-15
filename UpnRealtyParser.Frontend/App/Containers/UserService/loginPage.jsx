import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { Form, Input, Button, PageHeader, message } from 'antd';

import { doLogin, setLoginPageLogin, setLoginPagePassword, setLoginPageSubmitted } from './userActions.jsx';

class LoginPage extends React.Component {
    handleSubmit(e) {
        this.props.setLoginPageSubmitted(true);

        if (this.props.login && this.props.password) {
            this.props.doLogin(this.props.login, this.props.password);
        }

        if(this.props.isLoginFailed)
            message.error("Неправильный логин или пароль");
    }

    render() {
        return (
            <>
            <PageHeader
                className="site-page-header"
                backIcon={false}
                onBack={() => null}
                title="Вход в приложение"
                subTitle="Введите данные пользователя"
            />
            <Form name="loginForm" labelCol={{ span: 8 }} wrapperCol={{ span: 8 }} onFinish={this.handleSubmit.bind(this)}>
                <Form.Item label="Логин" name="login"
                rules={[
                    {
                    required: true,
                    message: 'Пожалуйста, введите логин'
                    }
                ]}
                >
                    <Input onChange={this.props.setLoginPageLogin.bind(this)} value={this.props.login} />
                </Form.Item>

                <Form.Item label="Пароль" name="password"
                rules={[
                    {
                    required: true,
                    message: 'Пожалуйста, введите пароль'
                    }
                ]}
                >
                    <Input.Password onChange={this.props.setLoginPagePassword.bind(this)} value={this.props.password}/>
                </Form.Item>

                <Form.Item wrapperCol={{ offset: 11, span: 5 }}>
                    <Button type="primary" htmlType="submit">
                            Войти
                    </Button>
                </Form.Item>
            </Form>
            </>
        );
    }
}

let mapActionsToProps = (dispatch) => {
    return {
        doLogin: (login, password) => dispatch(doLogin(login, password)),
        setLoginPageLogin: (ev) => dispatch(setLoginPageLogin(ev)),
        setLoginPagePassword: (ev) => dispatch(setLoginPagePassword(ev)),
        setLoginPageSubmitted: (ev) => dispatch(setLoginPageSubmitted(ev)),
    };
};

let mapStateToProps = (state) => {
    return {
        loggingIn: state.authenticationReducer.loggingIn,
        login: state.authenticationReducer.login,
        password: state.authenticationReducer.password,
        loginFormSubmitted: state.authenticationReducer.loginFormSubmitted,
        isLoginFailed: state.authenticationReducer.isLoginFailed,
    };
}

export default connect(mapStateToProps, mapActionsToProps)(LoginPage);
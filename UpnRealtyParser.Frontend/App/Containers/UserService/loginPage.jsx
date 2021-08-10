import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { login, logout, setLoginPageLogin, setLoginPagePassword, setLoginPageSubmitted } from './userActions.jsx';

class LoginPage extends React.Component {
    constructor(props) {
        super(props);

        // reset login status
        this.props.logout();
    }

    handleSubmit(e) {
        e.preventDefault();

        this.props.setLoginPageSubmitted(true);
        const login = this.props.login;
        const password = this.props.const;

        if (login && password) {
            this.props.login(login, password);
        }
    }

    render() {
        const loggingIn = this.props.loggingIn;
        const login = this.props.login;
        const password = this.props.password;
        const submitted = this.props.submitted;

        return (
            <div>
                <h2>Login</h2>
                <form name="form" onSubmit={this.handleSubmit}>
                    <div>
                        <label htmlFor="login">Username</label>
                        <input type="text" className="form-control" name="login" value={login} onChange={this.props.setLoginPageLogin.bind(this)} />
                        {submitted && !login &&
                            <div className="help-block">Username is required</div>
                        }
                    </div>
                    <div>
                        <label htmlFor="password">Password</label>
                        <input type="password" className="form-control" name="password" value={password} onChange={this.props.setLoginPagePassword.bind(this)} />
                        {submitted && !password &&
                            <div className="help-block">Password is required</div>
                        }
                    </div>
                    <div>
                        <button className="btn btn-primary">Login</button>
                    </div>
                </form>
            </div>
        );
    }
}

let mapActionsToProps = (dispatch) => {
    return {
        login: () => dispatch(login()),
        logout: () => dispatch(logout()),
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
        loginFormSubmitted: state.authenticationReducer.loginFormSubmitted
    };
}

export default connect(mapStateToProps, mapActionsToProps)(LoginPage);
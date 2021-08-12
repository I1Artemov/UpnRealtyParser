import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { doLogin, setLoginPageLogin, setLoginPagePassword, setLoginPageSubmitted } from './userActions.jsx';

class LoginPage extends React.Component {
    /*constructor(props) {
        super(props);

        // reset login status
        this.props.doLogout();
    }*/

    handleSubmit(e) {
        e.preventDefault();

        this.props.setLoginPageSubmitted(true);

        if (this.props.login && this.props.password) {
            this.props.doLogin(this.props.login, this.props.password);
        }
    }

    render() {
        return (
            <div>
                <h2>Login</h2>
                <form name="form" onSubmit={this.handleSubmit.bind(this)}>
                    <div>
                        <label htmlFor="login">Username</label>
                        <input type="text" className="form-control" name="login" value={this.props.login} onChange={this.props.setLoginPageLogin.bind(this)} />
                        {this.props.submitted && !this.props.login &&
                            <div className="help-block">Username is required</div>
                        }
                    </div>
                    <div>
                        <label htmlFor="password">Password</label>
                        <input type="password" className="form-control" name="password" value={this.props.password} onChange={this.props.setLoginPagePassword.bind(this)} />
                        {this.props.submitted && !this.props.password &&
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
        loginFormSubmitted: state.authenticationReducer.loginFormSubmitted
    };
}

export default connect(mapStateToProps, mapActionsToProps)(LoginPage);
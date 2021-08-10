import {
    LOGIN_REQUEST, LOGIN_SUCCESS, LOGIN_FAILURE, LOGOUT,
    SET_LOGIN_FORM_LOGIN, SET_LOGIN_FORM_PASSWORD, SET_LOGIN_FORM_SUBMITTED
} from './userConstants.jsx';
import { userService } from './userService.jsx';
//import { history } from '../_helpers';

function loginRequest(user) {
    return { type: LOGIN_REQUEST, user }
}
function loginSuccess(user) {
    return { type: LOGIN_SUCCESS, user }
}
function loginFailure(error) {
    return { type: LOGIN_FAILURE, error }
}

export function login(username, password) {
    return dispatch => {
        dispatch(loginRequest({ username }));

        userService.login(username, password)
            .then(
                user => {
                    dispatch(loginSuccess(user));
                    //history.push('/');
                },
                error => {
                    dispatch(loginFailure(error));
                    console.log("Login failed: " + error);
                }
            );
    };
}

export function logout() {
    userService.logout();
    return { type: LOGOUT };
}

export function setLoginPageLogin(ev) {
    const login = ev;
    return {
        type: SET_LOGIN_FORM_LOGIN,
        payload: login
    };
}

export function setLoginPagePassword(ev) {
    const pass = ev;
    return {
        type: SET_LOGIN_FORM_PASSWORD,
        payload: pass
    };
}

export function setLoginPageSubmitted(ev) {
    const isSubmitted = ev;
    return {
        type: SET_LOGIN_FORM_SUBMITTED,
        payload: isSubmitted
    };
}
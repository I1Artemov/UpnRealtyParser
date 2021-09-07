import { userConst } from './userConstants.jsx';
import { userService } from './userService.jsx';
import { createBrowserHistory } from 'history';

const history = createBrowserHistory({ forceRefresh: true });

function loginRequest(user) {
    return { type: userConst.LOGIN_REQUEST, user }
}
function loginSuccess(user) {
    return { type: userConst.LOGIN_SUCCESS, user }
}
function loginFailure(error) {
    return { type: userConst.LOGIN_FAILURE, error }
}

export function doLogin(username, password) {
    return dispatch => {
        dispatch(loginRequest({ username }));

        userService.doLogin(username, password)
            .then(
                user => {
                    dispatch(loginSuccess(user));
                    history.push('/');
                },
                error => {
                    dispatch(loginFailure(error));
                    console.log("Login failed: " + error);
                }
            );
    };
}

export function doLogout() {
    userService.doLogout();
    return { type: userConst.LOGOUT };
}

export function setLoginPageLogin(ev) {
    const login = ev.target.value;
    return {
        type: userConst.SET_LOGIN_FORM_LOGIN,
        payload: login
    };
}

export function setLoginPagePassword(ev) {
    const pass = ev.target.value;
    return {
        type: userConst.SET_LOGIN_FORM_PASSWORD,
        payload: pass
    };
}

export function setLoginPageSubmitted(ev) {
    const isSubmitted = ev;
    return {
        type: userConst.SET_LOGIN_FORM_SUBMITTED,
        payload: isSubmitted
    };
}
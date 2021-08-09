import { LOGIN_REQUEST, LOGIN_SUCCESS, LOGIN_FAILURE, LOGOUT } from './userConstants.jsx';
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
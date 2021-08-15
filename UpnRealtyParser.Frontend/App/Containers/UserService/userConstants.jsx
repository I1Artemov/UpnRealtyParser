export const LOGIN_REQUEST = 'USERS_LOGIN_REQUEST';
export const LOGIN_SUCCESS = 'USERS_LOGIN_SUCCESS';
export const LOGIN_FAILURE = 'USERS_LOGIN_FAILURE';

export const SET_LOGIN_FORM_LOGIN = 'SET_LOGIN_FORM_LOGIN';
export const SET_LOGIN_FORM_PASSWORD = 'SET_LOGIN_FORM_PASSWORD';
export const SET_LOGIN_FORM_SUBMITTED = 'SET_LOGIN_FORM_SUBMITTED';

export const LOGOUT = 'USERS_LOGOUT';

/** Возвращает JSON-объект с параметрами авторизации */
export function getAuthHeader() {
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.token) {
        return { 'Authorization': 'Bearer ' + user.token };
    } else {
        return {};
    }
}
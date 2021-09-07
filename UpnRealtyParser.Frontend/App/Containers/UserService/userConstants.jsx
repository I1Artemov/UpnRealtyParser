export const userConst = {
    LOGIN_REQUEST: 'USERS_LOGIN_REQUEST',
    LOGIN_SUCCESS: 'USERS_LOGIN_SUCCESS',
    LOGIN_FAILURE: 'USERS_LOGIN_FAILURE',

    SET_LOGIN_FORM_LOGIN: 'SET_LOGIN_FORM_LOGIN',
    SET_LOGIN_FORM_PASSWORD: 'SET_LOGIN_FORM_PASSWORD',
    SET_LOGIN_FORM_SUBMITTED: 'SET_LOGIN_FORM_SUBMITTED',

    LOGOUT: 'USERS_LOGOUT'
};

/** Возвращает JSON-объект с параметрами авторизации */
export function getAuthHeader() {
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.token) {
        return { 'Authorization': 'Bearer ' + user.token };
    } else {
        return {};
    }
}
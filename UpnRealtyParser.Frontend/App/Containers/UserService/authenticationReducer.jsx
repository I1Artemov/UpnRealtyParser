import {
    LOGIN_REQUEST, LOGIN_SUCCESS, LOGIN_FAILURE, LOGOUT,
    SET_LOGIN_FORM_LOGIN, SET_LOGIN_FORM_PASSWORD, SET_LOGIN_FORM_SUBMITTED
} from './userConstants.jsx';

let user = JSON.parse(localStorage.getItem('user'));

const initialState = user ?
    {
        login: '',
        password: '',
        loginFormSubmitted: false,
        loggedIn: true,
        user
    } :
    {
        login: '',
        password: '',
        loginFormSubmitted: false
    };

export default function authentication(state = initialState, action) {
    switch (action.type) {
        case LOGIN_REQUEST:
            return {
                loggingIn: true,
                user: action.user
            };
        case LOGIN_SUCCESS:
            return {
                loggedIn: true,
                user: action.user
            };
        case LOGIN_FAILURE:
            return {};
        case LOGOUT:
            return {};
        case SET_LOGIN_FORM_LOGIN:
            return {...state, login: action.payload};
        case SET_LOGIN_FORM_PASSWORD:
            return { ...state, password: action.payload };
        case SET_LOGIN_FORM_SUBMITTED:
            return { ...state, loginFormSubmitted: action.payload};
        default:
            return state
    }
}
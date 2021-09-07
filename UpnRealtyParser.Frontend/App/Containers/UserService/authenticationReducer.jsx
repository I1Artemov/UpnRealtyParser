import { userConst } from './userConstants.jsx';

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
        loginFormSubmitted: false,
        isLoginFailed: false
    };

export default function authentication(state = initialState, action) {
    switch (action.type) {
        case userConst.LOGIN_REQUEST:
            return {
                ...state,
                loggingIn: true,
                user: action.user
            };
        case userConst.LOGIN_SUCCESS:
            return {
                loggedIn: true,
                isLoginFailed: false,
                user: action.user
            };
        case userConst.LOGIN_FAILURE:
            return { ...state, loginFormSubmitted: false, isLoginFailed: true };
        case userConst.LOGOUT:
            return {};
        case userConst.SET_LOGIN_FORM_LOGIN:
            return {...state, login: action.payload};
        case userConst.SET_LOGIN_FORM_PASSWORD:
            return { ...state, password: action.payload };
        case userConst.SET_LOGIN_FORM_SUBMITTED:
            return { ...state, loginFormSubmitted: action.payload};
        default:
            return state
    }
}
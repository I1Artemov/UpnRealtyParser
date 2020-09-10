import {
    GET_ALL_PROXIES_SUCCESS,
    GET_ALL_PROXIES_ERROR,
    GET_ALL_PROXIES_LOADING_IN_PROGRESS
} from './webProxyIndexConstants.jsx';

const initialState = {
    proxiesInfo: [{ id: 0, description: '' }],
    isProxiesLoading: false,
    totalProxiesCount: 0,
    error: ""
};

export default function proxies(state = initialState, action) {
    switch (action.type) {
        case GET_ALL_PROXIES_LOADING_IN_PROGRESS:
            return { ...state, isProxiesLoading: true };

        case GET_ALL_PROXIES_SUCCESS:
            return { ...state, proxiesInfo: action.proxiesInfo, totalProxiesCount: action.totalProxiesCount, error: '', isProxiesLoading: false  };

        case GET_ALL_PROXIES_ERROR:
            return { ...state, error: action.error, isProxiesLoading: false };

        default:
            return state;
    }
}
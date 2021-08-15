import { Href_User_Authenticate } from '../../const.jsx';

export const userService = {
    doLogin,
    doLogout
};

function doLogin(username, password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(
            { login: username, password: password }
        )
    };

    return fetch(Href_User_Authenticate, requestOptions)
        .then(handleResponse, handleError)
        .then(user => {
            // Если в ответе есть JWT-токен, то вход успешен
            if (user && user.token) {
                // Токен сохраняем в LocalStorage
                localStorage.setItem('user', JSON.stringify(user));
            }

            return user;
        });
}

function doLogout() {
    // При выходе просто стираем токен из LocalStorage
    localStorage.removeItem('user');
}

function handleResponse(response) {
    return new Promise((resolve, reject) => {
        if (response.ok) {
            // Вернет Json, если запрос вернул Json
            var contentType = response.headers.get("content-type");
            if (contentType && contentType.includes("application/json")) {
                response.json().then(json => resolve(json));
            } else {
                resolve();
            }
        } else {
            // При ошибке вернет сообщение из тела ответа
            response.text().then(text => reject(text));
        }
    });
}

function handleError(error) {
    return Promise.reject(error && error.message);
}
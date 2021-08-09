import React from 'react';
import { Route, Redirect } from 'react-router-dom';

/** Компонент для замены Route на его аналог с обязательной авторизацией */
export const PrivateRoute = ({ component: Component, ...rest }) => (
    <Route {...rest}
        render={props => (
                localStorage.getItem('user')
                    ? <Component {...props} />
                    : <Redirect to={{ pathname: '/login', state: { from: props.location } }} />
    )} />
)
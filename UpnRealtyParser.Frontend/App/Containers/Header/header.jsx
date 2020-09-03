import React from 'react';
import { Link } from 'react-router-dom';

export default class Header extends React.Component {
    render() {
        return (
            <header>
                <menu>
                    <ul>
                        <li>
                            <Link to="/">Недвижимость УПН</Link>
                        </li>
                        <li>
                            <Link to="/sellflat">Квартиры на продажу</Link>
                        </li>
                        <li>
                            <Link to="/rentflat">Квартиры в аренду</Link>
                        </li>
                        <li>
                            <Link to="/house">Дома</Link>
                        </li>
                        <li>
                            <Link to="/agency">Агентства</Link>
                        </li>
                        <li>
                            <Link to="/webproxy">Прокси</Link>
                        </li>
                        <li>
                            <Link to="/log">Лог</Link>
                        </li>
                    </ul>
                </menu>
            </header>
        );
    }
};
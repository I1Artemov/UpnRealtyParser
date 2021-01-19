import React from 'react';
import { Link } from 'react-router-dom';

export const GET_ALL_FLATS_SUCCESS = 'GET_ALL_FLATS_SUCCESS';
export const GET_ALL_FLATS_ERROR = 'GET_ALL_FLATS_ERROR';
export const GET_ALL_FLATS_LOADING_IN_PROGRESS = 'GET_ALL_FLATS_LOADING_IN_PROGRESS';

export const SET_SHOW_ARCHIVED = 'SET_SHOW_ARCHIVED';
export const SET_EXCLUDE_FIRST_FLOOR = 'SET_EXCLUDE_FIRST_FLOOR';
export const SET_EXCLUDE_LAST_FLOOR = 'SET_EXCLUDE_LAST_FLOOR';
export const SET_MIN_PRICE = 'SET_MIN_PRICE';
export const SET_MAX_PRICE = 'SET_MAX_PRICE';
export const SET_MIN_BUILD_YEAR = 'SET_MIN_BUILD_YEAR';
export const SET_MAX_SUBWAY_DISTANCE = 'SET_MAX_SUBWAY_DISTANCE';
export const SET_CLOSEST_SUBWAY_STATION_ID = 'SET_CLOSEST_SUBWAY_STATION_ID';
export const CLEAR_SEARCH_PARAMETERS = 'CLEAR_SEARCH_PARAMETERS';

export const SELL_FLATS_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id'
    },
    {
        title: 'Фото',
        dataIndex: 'firstPhotoFile',
        key: 'firstPhotoFile',
        render: (text, row) => (
            (text === null || text === undefined || text === 'ERR') ? <p>--</p> :
                <img src={"/images/upnphotos/" + text} width="64" height="64"></img>
        )
    },
    {
        title: 'Создан/проверен',
        dataIndex: 'createdCheckedDatesSummary',
        key: 'createdCheckedDatesSummary'
    },
    {
        title: 'Тип',
        dataIndex: 'flatType',
        key: 'flatType'
    },
    {
        title: 'Комнат',
        dataIndex: 'roomAmount',
        key: 'roomAmount'
    },
    {
        title: 'Площадь',
        dataIndex: 'spaceSum',
        key: 'spaceSum'
    },
    {
        title: 'Этаж',
        dataIndex: 'floorSummary',
        key: 'floorSummary'
    },
    {
        title: 'Адрес',
        dataIndex: 'houseAddress',
        key: 'houseAddress'
    },
    {
        title: 'Тип дома',
        dataIndex: 'houseType',
        key: 'houseType'
    },
    {
        title: 'Материал дома',
        dataIndex: 'houseWallMaterial',
        key: 'houseWallMaterial'
    },
    {
        title: 'Построена',
        dataIndex: 'houseBuildYear',
        key: 'houseBuildYear'
    },
    {
        title: 'До метро',
        dataIndex: 'subwaySummary',
        key: 'subwaySummary'
    },
    {
        title: 'Цена',
        dataIndex: 'price',
        key: 'price'
    },
    {
        title: 'Описание',
        dataIndex: 'shortenedDescription',
        key: 'shortenedDescription',
        render: (text, row) => (
            (row.minimalRentPeriod === undefined) ?
                <Link to={"/sellflat/" + row.id}>{text}</Link>
                : <Link to={"/rentflat/" + row.id}>{text}</Link>
        )
    }
];
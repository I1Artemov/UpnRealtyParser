import React from 'react';
import { Link } from 'react-router-dom';

export const GET_ALL_HOUSES_SUCCESS = 'GET_ALL_HOUSES_SUCCESS';
export const GET_ALL_HOUSES_ERROR = 'GET_ALL_HOUSES_SUCCESS';
export const GET_ALL_HOUSES_LOADING_IN_PROGRESS = 'GET_ALL_AGENCIES_LOADING_IN_PROGRESS';
export const SET_MIN_BUILD_YEAR = 'SET_MIN_BUILD_YEAR';
export const SET_IS_SHOW_UPN = 'SET_IS_SHOW_UPN';
export const SET_IS_SHOW_N1 = 'SET_IS_SHOW_N1';
export const SET_ADDRESS_PART = 'SET_ADDRESS_PART';
export const CLEAR_SEARCH_PARAMETERS = 'CLEAR_SEARCH_PARAMETERS';

export const HOUSES_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id'
    },
    {
        title: 'Сайт',
        dataIndex: 'sourceSite',
        key: 'sourceSite',
        render: (text, row) => (
            (text === 'UPN') ? <span className="siteSourceUpn">{text}</span> :
                <span className="siteSourceN1">{text}</span>
        )
    },
    {
        title: 'Создан',
        dataIndex: 'creationDatePrintable',
        key: 'creationDatePrintable'
    },
    {
        title: 'Адрес',
        dataIndex: 'address',
        key: 'address',
        render: (text, row) => (
            (row.sourceSite === 'UPN') ?
                <Link to={"/upnhouse/" + row.id }>{text}</Link>
                : <Link to={"/n1house/" + row.id }>{text}</Link>
        )
    },
    {
        title: 'Тип',
        dataIndex: 'houseType',
        key: 'houseType'
    },
    {
        title: 'Год постройки',
        dataIndex: 'buildYear',
        key: 'buildYear'
    },
    {
        title: 'Материал',
        dataIndex: 'wallMaterial',
        key: 'wallMaterial'
    },
    {
        title: 'Этажей',
        dataIndex: 'maxFloor',
        key: 'maxFloor'
    },
    {
        title: 'Широта',
        dataIndex: 'latitude',
        key: 'latitude'
    },
    {
        title: 'Долгота',
        dataIndex: 'longitude',
        key: 'longitude'
    },
    {
        title: 'Станция',
        dataIndex: 'closestSubwayName',
        key: 'closestSubwayName'
    },
    {
        title: 'До метро',
        dataIndex: 'closestSubwayRangeInfo',
        key: 'closestSubwayRangeInfo'
    }
];
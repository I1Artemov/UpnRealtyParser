import React from 'react';
import { Link } from 'react-router-dom';

export const houseIndexConst = {
    GET_ALL_HOUSES_SUCCESS: 'GET_ALL_HOUSES_SUCCESS',
    GET_ALL_HOUSES_ERROR: 'GET_ALL_HOUSES_SUCCESS',
    GET_ALL_HOUSES_LOADING_IN_PROGRESS: 'GET_ALL_AGENCIES_LOADING_IN_PROGRESS',
    SET_MIN_BUILD_YEAR: 'SET_MIN_BUILD_YEAR',
    SET_IS_SHOW_UPN: 'SET_IS_SHOW_UPN',
    SET_IS_SHOW_N1: 'SET_IS_SHOW_N1',
    SET_ADDRESS_PART: 'SET_ADDRESS_PART',
    CLEAR_SEARCH_PARAMETERS: 'CLEAR_SEARCH_PARAMETERS',
    HOUSES_TABLE_COLUMNS: [
        {
            title: '№',
            dataIndex: 'id',
            key: 'id',
            sorter: true
        },
        {
            title: 'Сайт',
            dataIndex: 'sourceSite',
            key: 'sourceSite',
            align: 'center',
            render: (text, row) => (
                (text === 'UPN') ? <span className="siteSourceUpn">{text}</span> :
                    <span className="siteSourceN1">{text}</span>
            )
        },
        {
            title: 'Создан',
            dataIndex: 'creationDatePrintable',
            key: 'creationDatePrintable',
            sorter: true
        },
        {
            title: 'Адрес',
            dataIndex: 'address',
            key: 'address',
            render: (text, row) => (
                (row.sourceSite === 'UPN') ?
                    <Link to={"/upnhouse/" + row.id}>{text}</Link>
                    : <Link to={"/n1house/" + row.id}>{text}</Link>
            )
        },
        {
            title: 'Тип',
            dataIndex: 'houseType',
            key: 'houseType',
            align: 'center'
        },
        {
            title: 'Год постройки',
            dataIndex: 'buildYear',
            key: 'buildYear',
            align: 'center',
            sorter: true
        },
        {
            title: 'Материал',
            dataIndex: 'wallMaterial',
            key: 'wallMaterial',
            align: 'center'
        },
        {
            title: 'Этажей',
            dataIndex: 'maxFloor',
            key: 'maxFloor',
            align: 'center'
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
            key: 'closestSubwayRangeInfo',
            sorter: true
        }
    ]
};
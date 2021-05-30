import React from 'react';
import { Link } from 'react-router-dom';

export const GET_ALL_FLATS_SUCCESS = 'GET_ALL_FLATS_SUCCESS';
export const GET_ALL_FLATS_ERROR = 'GET_ALL_FLATS_ERROR';
export const GET_ALL_FLATS_LOADING_IN_PROGRESS = 'GET_ALL_FLATS_LOADING_IN_PROGRESS';

export const SELL_FLATS_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id'
    },
    {
        title: 'Создан/проверен',
        dataIndex: 'createdCheckedDatesSummary',
        key: 'createdCheckedDatesSummary'
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
            (text == null || text == '') ?
                <Link to={"/n1sellflat/" + row.id}>{'Описание отсутствует'}</Link> :
                <Link to={"/n1sellflat/" + row.id}>{text}</Link>
        )
    }
];
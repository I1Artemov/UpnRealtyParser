import React from 'react';
import { Link } from 'react-router-dom';

export const GET_ALL_FLATS_SUCCESS = 'GET_ALL_FLATS_SUCCESS';
export const GET_ALL_FLATS_ERROR = 'GET_ALL_FLATS_ERROR';
export const GET_ALL_FLATS_LOADING_IN_PROGRESS = 'GET_ALL_FLATS_LOADING_IN_PROGRESS';

export const SELL_FLATS_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id',
        sorter: true
    },
    {
        title: 'Создан/проверен',
        dataIndex: 'createdCheckedDatesSummary',
        key: 'createdCheckedDatesSummary',
        sorter: true
    },
    {
        title: 'Комнат',
        dataIndex: 'roomAmount',
        key: 'roomAmount',
        align: 'center'
    },
    {
        title: 'Площадь',
        dataIndex: 'spaceSum',
        key: 'spaceSum',
        align: 'center',
        sorter: true
    },
    {
        title: 'Этаж',
        dataIndex: 'floorSummary',
        key: 'floorSummary',
        align: 'center'
    },
    {
        title: 'Адрес',
        dataIndex: 'houseAddress',
        key: 'houseAddress'
    },
    {
        title: 'Тип дома',
        dataIndex: 'houseType',
        key: 'houseType',
        align: 'center'
    },
    {
        title: 'Материал дома',
        dataIndex: 'houseWallMaterial',
        key: 'houseWallMaterial',
        align: 'center'
    },
    {
        title: 'Построена',
        dataIndex: 'houseBuildYear',
        key: 'houseBuildYear',
        align: 'center'
    },
    {
        title: 'До метро',
        dataIndex: 'subwaySummary',
        key: 'subwaySummary'
    },
    {
        title: 'Цена',
        dataIndex: 'price',
        key: 'price',
        sorter: true
    },
    {
        title: 'Описание',
        dataIndex: 'shortenedDescription',
        key: 'shortenedDescription',
        render: (text, row) => {
            let linkText = text;
            if (text == null || text == '')
                linkText = 'Описание отсутствует';

            let linkUrl = "/n1sellflat/" + row.id;
            if (row.minimalRentPeriod != undefined)
                linkUrl = "/n1rentflat/" + row.id;

            return (
                <Link to={linkUrl}>{linkText}</Link>
            );
        }
    }
];
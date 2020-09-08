﻿export const GET_ALL_FLATS_SUCCESS = 'GET_ALL_FLATS_SUCCESS';
export const GET_ALL_FLATS_ERROR = 'GET_ALL_FLATS_ERROR';

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
        title: 'Проверен',
        dataIndex: 'lastCheckDatePrintable',
        key: 'lastCheckDate'
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
        key: 'shortenedDescription'
    }
];
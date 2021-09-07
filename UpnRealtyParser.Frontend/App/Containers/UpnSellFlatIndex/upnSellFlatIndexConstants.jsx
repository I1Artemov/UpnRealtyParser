import React from 'react';
import { Link } from 'react-router-dom';

export const sellFlatIndexConst = {
    GET_ALL_FLATS_SUCCESS: 'GET_ALL_FLATS_SUCCESS',
    GET_ALL_FLATS_ERROR: 'GET_ALL_FLATS_ERROR',
    GET_ALL_FLATS_LOADING_IN_PROGRESS: 'GET_ALL_FLATS_LOADING_IN_PROGRESS',

    SAVE_PAGING_PARAMETERS: 'SAVE_PAGING_PARAMETERS',

    SELL_FLATS_TABLE_COLUMNS: [
        {
            title: '№',
            dataIndex: 'id',
            key: 'id',
            align: 'center',
            sorter: true
        },
        {
            title: 'Фото',
            dataIndex: 'firstPhotoFile',
            key: 'firstPhotoFile',
            align: 'center',
            render: (text, row) => (
                (text === null || text === undefined || text === 'ERR') ? <p>--</p> :
                    <img src={"/images/upnphotos/" + text} width="64" height="64"></img>
            )
        },
        {
            title: 'Создан/ проверен',
            dataIndex: 'createdCheckedDatesSummary',
            key: 'createdCheckedDatesSummary',
            sorter: true
        },
        {
            title: 'Тип',
            dataIndex: 'flatType',
            key: 'flatType',
            align: 'center'
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
            key: 'houseType'
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
            align: 'center',
            sorter: true
        },
        {
            title: 'До метро',
            dataIndex: 'subwaySummary',
            key: 'subwaySummary',
            sorter: true
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
            render: (text, row) => (
                (row.minimalRentPeriod === undefined) ?
                    <Link to={"/sellflat/" + row.id}>{text}</Link>
                    : <Link to={"/rentflat/" + row.id}>{text}</Link>
            )
        }
    ]
};
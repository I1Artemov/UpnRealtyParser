import React from 'react';
import { Link } from 'react-router-dom';

function getClassnameByPaybackValue(payback) {
    if (payback > 20)
        return 'payback-max';
    if (payback <= 20 && payback > 15)
        return 'payback-high';
    if (payback <= 15 && payback > 10)
        return 'payback-middle';

    return 'payback-low';
}

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
            dataIndex: 'photoUrl',
            key: 'photoUrl',
            align: 'center',
            render: (text, row) => (
                (text === null || text === undefined || text === 'ERR') ? <p>--</p> :
                    <img src={text} width="64" height="64"></img>
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
            title: 'Площ.',
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
            title: 'Год',
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
            title: 'Окуп. (лет)',
            dataIndex: 'paybackYears',
            key: 'paybackYears',
            render: (text, row) => (
                (text != null && text != undefined) ?
                    <p className={getClassnameByPaybackValue(text)}>{Math.round(text * 10) / 10}</p> :
                    null
            )
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
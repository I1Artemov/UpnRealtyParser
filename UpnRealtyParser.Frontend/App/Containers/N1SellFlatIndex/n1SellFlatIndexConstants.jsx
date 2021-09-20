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

export const n1SellFlatIndexConst = {
    GET_ALL_FLATS_SUCCESS: 'GET_ALL_FLATS_SUCCESS',
    GET_ALL_FLATS_ERROR: 'GET_ALL_FLATS_ERROR',
    GET_ALL_FLATS_LOADING_IN_PROGRESS: 'GET_ALL_FLATS_LOADING_IN_PROGRESS',

    SELL_FLATS_TABLE_COLUMNS: [
        {
            title: '№',
            dataIndex: 'id',
            key: 'id',
            sorter: true,
            align: 'center'
        },
        {
            title: 'Фото',
            dataIndex: 'photoUrl',
            key: 'photoUrl',
            align: 'center',
            render: (text, row) => (
                (text === null || text === undefined || text.indexOf('ERR') > 0) ? <p>--</p> :
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
            title: 'Год',
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
    ]
};
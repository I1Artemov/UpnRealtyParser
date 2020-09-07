export const GET_ALL_FLATS_SUCCESS = 'GET_ALL_FLATS_SUCCESS';
export const GET_ALL_FLATS_ERROR = 'GET_ALL_FLATS_ERROR';

export const SELL_FLATS_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id'
    },
    {
        title: 'Создан',
        dataIndex: 'creationDateTime',
        key: 'creationDateTime'
    },
    {
        title: 'Проверен',
        dataIndex: 'lastCheckDate',
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
        dataIndex: 'flatFloor',
        key: 'flatFloor'
    },
    {
        title: 'Цена',
        dataIndex: 'price',
        key: 'price'
    },
    {
        title: 'Описание',
        dataIndex: 'description',
        key: 'description'
    }
];
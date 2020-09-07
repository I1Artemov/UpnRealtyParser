export const GET_ALL_HOUSES_SUCCESS = 'GET_ALL_HOUSES_SUCCESS';
export const GET_ALL_HOUSES_ERROR = 'GET_ALL_HOUSES_SUCCESS';

export const HOUSES_TABLE_COLUMNS = [
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
        title: 'Адрес',
        dataIndex: 'address',
        key: 'address'
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
        title: 'До метро',
        dataIndex: 'closestSubwayStationRange',
        key: 'closestSubwayStationRange'
    }
];
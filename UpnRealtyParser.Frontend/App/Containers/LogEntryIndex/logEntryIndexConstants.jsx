export const GET_ALL_LOG_ENTRIES_SUCCESS = 'GET_ALL_LOG_ENTRIES_SUCCESS';
export const GET_ALL_LOG_ENTRIES_ERROR = 'GET_ALL_LOG_ENTRIES_ERROR';
export const GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS = 'GET_ALL_LOG_ENTRIES_LOADING_IN_PROGRESS';

export const LOG_ENTRIES_TABLE_COLUMNS = [
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
        title: 'Сайт',
        dataIndex: 'siteName',
        key: 'siteName'
    },
    {
        title: 'Описание',
        dataIndex: 'description',
        key: 'description'
    },
    {
        title: 'Подробности',
        dataIndex: 'details',
        key: 'details'
    },
    {
        title: 'Статус',
        dataIndex: 'status',
        key: 'status'
    },
];
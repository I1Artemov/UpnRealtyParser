export const GET_ALL_AGENCIES_SUCCESS = 'GET_ALL_AGENCIES_SUCCESS';
export const GET_ALL_AGENCIES_ERROR = 'GET_ALL_AGENCIES_ERROR';

export const AGENCIES_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id'
    },
    {
        title: 'Создан',
        dataIndex: 'creationDatePrintable',
        key: 'creationDatePrintable'
    },
    {
        title: 'Название',
        dataIndex: 'name',
        key: 'name'
    },
    {
        title: 'Телефон',
        dataIndex: 'companyPhone',
        key: 'companyPhone'
    },
    {
        title: 'Телефон агента',
        dataIndex: 'agentPhone',
        key: 'agentPhone'
    },
    {
        title: 'Время работы',
        dataIndex: 'workTime',
        key: 'workTime'
    },
    {
        title: 'Сайт',
        dataIndex: 'siteUrl',
        key: 'siteUrl'
    },
    {
        title: 'Email',
        dataIndex: 'email',
        key: 'email'
    }
];
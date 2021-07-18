export const GET_ALL_PROXIES_SUCCESS = 'GET_ALL_PROXIES_SUCCESS';
export const GET_ALL_PROXIES_ERROR = 'GET_ALL_PROXIES_ERROR';
export const GET_ALL_PROXIES_LOADING_IN_PROGRESS = 'GET_ALL_PROXIES_LOADING_IN_PROGRESS';

export const PROXIES_TABLE_COLUMNS = [
    {
        title: '№',
        dataIndex: 'id',
        key: 'id',
        align: 'center',
        sorter: true
    },
    {
        title: 'Создан',
        dataIndex: 'creationDatePrintable',
        key: 'creationDatePrintable',
        align: 'center',
        sorter: true
    },
    {
        title: 'IP-адрес',
        dataIndex: 'ip',
        key: 'ip'
    },
	{
        title: 'Порт',
        dataIndex: 'port',
        key: 'port'
    },
	{
        title: 'Успешные соед.',
        dataIndex: 'successAmount',
        key: 'successAmount',
        align: 'center',
        sorter: true
    },
	{
        title: 'Неуд. соед.',
        dataIndex: 'failureAmount',
        key: 'failureAmount',
        align: 'center',
        sorter: true
    },
	{
        title: 'Последнее успеш. соед.',
        dataIndex: 'lastSuccessDateTime',
        key: 'lastSuccessDateTime',
        align: 'center',
        sorter: true
    },
	{
        title: 'Последнеяя попытка соед.',
        dataIndex: 'lastUseDateTime',
        key: 'lastUseDateTime',
        align: 'center',
        sorter: true
    }
];
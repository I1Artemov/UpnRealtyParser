export const GET_ALL_PROXIES_SUCCESS = 'GET_ALL_PROXIES_SUCCESS';
export const GET_ALL_PROXIES_ERROR = 'GET_ALL_PROXIES_ERROR';

export const PROXIES_TABLE_COLUMNS = [
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
        key: 'successAmount'
    },
	{
        title: 'Неуд. соед.',
        dataIndex: 'failureAmount',
        key: 'failureAmount'
    },
	{
        title: 'Последнее успеш. соед.',
        dataIndex: 'lastSuccessDateTime',
        key: 'lastSuccessDateTime'
    },
	{
        title: 'Последнеяя попытка соед.',
        dataIndex: 'lastUseDateTime',
        key: 'lastUseDateTime'
    }
];
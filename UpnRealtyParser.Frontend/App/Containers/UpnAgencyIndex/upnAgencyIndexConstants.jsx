import React from 'react';

export const agencyIndexConst = {
    GET_ALL_AGENCIES_SUCCESS: 'GET_ALL_AGENCIES_SUCCESS',
    GET_ALL_AGENCIES_ERROR: 'GET_ALL_AGENCIES_ERROR',
    GET_ALL_AGENCIES_LOADING_IN_PROGRESS: 'GET_ALL_AGENCIES_LOADING_IN_PROGRESS',

    AGENCIES_TABLE_COLUMNS: [
        {
            title: '№',
            dataIndex: 'id',
            key: 'id'
        },
        {
            title: 'Сайт',
            dataIndex: 'sourceSite',
            key: 'sourceSite',
            render: (text, row) => (
                (text === 'UPN') ? <span className="siteSourceUpn">{text}</span> :
                    <span className="siteSourceN1">{text}</span>
            )
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
        },
        {
            title: 'Агент',
            dataIndex: 'agentName',
            key: 'agentName'
        },
        {
            title: 'Агентство',
            dataIndex: 'isAgency',
            key: 'isAgency'
        }
    ]
};
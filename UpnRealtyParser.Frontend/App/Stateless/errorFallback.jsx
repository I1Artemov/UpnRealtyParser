import React from 'react';

/**
 * Компонент для использования в ErrorBoundary
 */
export default function ErrorFallback({ error }) {
    return (
        <div role="alert">
            <p>Ошибка при отображении данных:</p>
            <pre style={{ color: 'red' }}>{error.message}</pre>
        </div>
    )
}

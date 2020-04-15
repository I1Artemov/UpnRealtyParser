using System.Collections.Generic;

namespace UpnRealtyParser.Business.Models
{
    /// <summary>
    /// Общие параметры работы приложения. Для сериализации в XML-файл
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Задержка между запросами к сайту в миллисекундах
        /// </summary>
        public int RequestDelayInMs { get; set; }

        /// <summary>
        /// Максимальное время, в течение которого ждем ответа от прокси
        /// </summary>
        public int MaxRequestTimeoutInMs { get; set; }

        /// <summary>
        /// Сколько страниц таблицы с переченм квартир на УПН нужно пропустить
        /// </summary>
        public int UpnTablePagesToSkip { get; set; }

        /// <summary>
        /// Максимально число попыток выполнения одного и того же запроса
        /// </summary>
        public int MaxRetryAmountForSingleRequest { get; set; }

        public bool IsUseProxies { get; set; }

        /// <summary>
        /// Если установлено, то прокси будут браться не из конфига, а с https://github.com/clarketm/proxy-list
        /// </summary>
        public bool IsGetProxiesListFromGithub { get; set; }

        /// <summary>
        /// Список прокси в формате "IP:Port"
        /// </summary>
        public List<string> ProxyList { get; set; }
    }
}

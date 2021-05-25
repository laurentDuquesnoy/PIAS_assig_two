using System;
namespace AppConfiguration.Data
{
    /// <summary>
    /// Class die wordt gebruikt om transfer objecten te maken.
    /// De transfer objecten worden gebruikt om data tussen de lagen van de applicatie te versturen.
    /// </summary>
    /// <typeparam name="TQueryResult"></typeparam>
    public class ConfigurationQueryResult<TQueryResult>
    {
        public Exception Error { get; set; }
        public TQueryResult QueryResult { get; set; }
        public QueryStatus Status { get; set; }
    }
}
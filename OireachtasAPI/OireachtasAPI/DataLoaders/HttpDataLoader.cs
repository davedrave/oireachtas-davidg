using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OireachtasAPI.DataLoaders
{
    /// <summary>
    /// Class for loading data from an HTTP source.
    /// </summary>
    /// <seealso cref="OireachtasAPI.DataLoaders.IDataLoader" />
    public class HttpDataLoader : IDataLoader
    {
        private readonly ILogger logger;

        public HttpDataLoader(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public dynamic Load(string source)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(source).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        logger.Debug(responseBody.Substring(0, 25));
                    }
                    else
                    {
                        logger.Warning("Data from {ResponseBody} is empty", responseBody);
                    }

                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
            catch (HttpRequestException ex)
            {
                logger.Error(ex, "HTTP request failed for source {Source}", source);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                logger.Error(ex, "HTTP request timed out for source {Source}", source);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading data from {Source}", source);
                throw;
            }
        }
    }
}

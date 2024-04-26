using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace OireachtasAPI.DataLoaders
{
    /// <summary>
    /// Class for loading data from an HTTP source.
    /// </summary>
    /// <seealso cref="OireachtasAPI.DataLoaders.IDataLoader" />
    public class HttpDataLoader : IDataLoader
    {
        ILogger logger;

        public HttpDataLoader(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public dynamic Load(string source)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(source).Result;
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
    }
}

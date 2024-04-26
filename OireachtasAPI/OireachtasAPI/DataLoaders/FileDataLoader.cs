﻿using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OireachtasAPI.DataLoaders
{
    /// <summary>
    /// Class for loading data from a file source.
    /// </summary>
    /// <seealso cref="OireachtasAPI.DataLoaders.IDataLoader" />
    public class FileDataLoader : IDataLoader
    {
        ILogger logger;

        public FileDataLoader(ILogger logger)
        {
            this.logger = logger;
        }
        /// <inheritdoc/>
        public dynamic Load(string source)
        {
            using(StreamReader streamReader = new System.IO.StreamReader(source))
            {
                string content = streamReader.ReadToEnd();

                if (!string.IsNullOrEmpty(content))
                {
                    logger.Debug(content.Substring(0, 25));
                }else
                {
                    logger.Warning("Data from {Source} is empty", content);
                }

                return JsonConvert.DeserializeObject(content);
            }
        }
    }
}

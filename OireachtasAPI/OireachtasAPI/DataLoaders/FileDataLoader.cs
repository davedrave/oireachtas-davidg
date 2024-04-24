using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// <inheritdoc/>
        public dynamic Load(string source)
        {
            return JsonConvert.DeserializeObject((new System.IO.StreamReader(source)).ReadToEnd());
        }
    }
}

using Newtonsoft.Json;
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
        /// <inheritdoc/>
        public dynamic Load(string source)
        {
            using(StreamReader streamReader = new System.IO.StreamReader(source))
            {
                return JsonConvert.DeserializeObject(streamReader.ReadToEnd());
            }
        }
    }
}

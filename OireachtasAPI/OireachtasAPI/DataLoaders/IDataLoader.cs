using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OireachtasAPI.DataLoaders
{
    /// <summary>
    /// Interface for data loaders.
    /// </summary>
    internal interface IDataLoader
    {
        /// <summary>
        /// Loads data from the specified source.
        /// </summary>
        /// <param name="source">The source from which to load data.</param>
        /// <returns>The loaded data.</returns>
        dynamic Load(string source);
    }
}

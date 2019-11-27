using System.Threading.Tasks;
using EasyMorph.Data;

namespace EasyMorph.Drivers
{
    /// <summary>
    /// Driver to import dataset
    /// </summary>
    public interface IImportDriver
    {
        /// <summary>
        /// Imports dataset
        /// </summary>
        /// <param name="config">Settings to configure import</param>
        /// <returns>Datasets from file</returns>
        Task<IDataset[]> ImportAsync(ImportConfig config);
    }
}
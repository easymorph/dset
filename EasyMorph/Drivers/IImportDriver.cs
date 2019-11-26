using System.Threading.Tasks;
using EasyMorph.Data;

namespace EasyMorph.Drivers
{
    public interface IImportDriver
    {
        Task<IDataset[]> ImportAsync(ImportConfig config);
    }
}
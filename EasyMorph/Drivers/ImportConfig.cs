using System.Threading;

namespace EasyMorph.Drivers
{
    public class ImportConfig
    {
        public string FileName { get; }
        public CancellationToken Token { get; }

        public ImportConfig(string fileName, CancellationToken token)
        {
            FileName = fileName;
            Token = token;
        }

        public ImportConfig(string fileName) : this(fileName, CancellationToken.None) { }
    }
}
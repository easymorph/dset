using System.Threading;

namespace EasyMorph.Drivers
{
    /// <summary>
    /// Settings to configure import
    /// </summary>
    public class ImportConfig
    {
        /// <summary>
        /// File path for import
        /// </summary>
        public string FileName { get; }
        /// <summary>
        /// Token to monitor cancellation requests. Default is None
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// Creates Import settings
        /// </summary>
        /// <param name="fileName">Path to the file to import</param>
        /// <param name="token">Token to monitor cancellation requests</param>
        public ImportConfig(string fileName, CancellationToken token)
        {
            FileName = fileName;
            Token = token;
        }

        /// <summary>
        /// Creates Import settings
        /// </summary>
        /// <param name="fileName">Path to the file to import</param>
        public ImportConfig(string fileName) : this(fileName, CancellationToken.None) { }
    }
}
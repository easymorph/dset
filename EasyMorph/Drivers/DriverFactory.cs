namespace EasyMorph.Drivers
{
    /// <summary>
    /// Factory to get drivers to work with datasets
    /// </summary>
    public static class DriverFactory
    {
        /// <summary>
        /// Get Import driver
        /// </summary>
        /// <returns>Import driver</returns>
        public static IImportDriver GetImportDriver()
        {
            return new ImportDriver();
        }
    }
}
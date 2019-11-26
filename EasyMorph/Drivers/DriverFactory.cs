namespace EasyMorph.Drivers
{
    public static class DriverFactory
    {
        public static IImportDriver GetImportDriver()
        {
            return new ImportDriver();
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyMorph.Data;
using EasyMorph.Drivers;
using DsetToCsv;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyMorph.Tests
{
    [TestClass]
    public class TestImportDriver
    {
        public static string GetProjectFolderPath()
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingAssemblyPath, @"..\..\..");
        }

        public static string GetDatasetFilePath(string fileName)
        {
            return Path.Combine(GetProjectFolderPath(), "SourceDatasets", fileName);
        }

        public static string GetExpectedFilePath(string fileName)
        {
            return Path.Combine(GetProjectFolderPath(), "ExpectedFiles", fileName);
        }

        public static async Task<IDataset[]> ImportSourceDatasetAsync(string datasetFileName)
        {
            var datasetFilePath = GetDatasetFilePath(datasetFileName);

            var driver = DriverFactory.GetImportDriver();

            var config = new ImportConfig(datasetFilePath);

            return await driver.ImportAsync(config);
        }

        public static async Task<string> ImportExpectedFileAsync(string expectedFileName)
        {
            var expectedFilePath = GetExpectedFilePath(expectedFileName);

            return await File.ReadAllTextAsync(expectedFilePath, default);
        }

        public static string ConvertDatasetToCsv(IDataset dataset)
        {
            using (StringWriter writer = new StringWriter())
            {
                CsvExportDriver.WriteData(dataset, writer, ";", default);

                return writer.ToString();
            }
        }

        [TestMethod]
        public async Task TestWithMixedDataset()
        {
            var datasets = await ImportSourceDatasetAsync("MixedDataset.dset");

            Assert.AreEqual(1, datasets.Length);

            Assert.AreEqual(7, datasets[0].ColumnsCount);
            Assert.AreEqual(26, datasets[0].RowsCount);

            // For now imported dataset is converted to a CSV text and
            // compared with an expected CSV text, stored in a file.
            // A test that checks import of different cell values should be added.
            var actualCsv = ConvertDatasetToCsv(datasets[0]);
            var expectedCsv = await ImportExpectedFileAsync("ExpectedMixedDataset.csv");

            Assert.AreEqual(expectedCsv, actualCsv);
        }

        [TestMethod]
        public async Task TestWithEmptyDataset()
        {
            var datasets = await ImportSourceDatasetAsync("EmptyDataset.dset");

            Assert.AreEqual(1, datasets.Length);

            Assert.AreEqual(0, datasets[0].ColumnsCount);
            Assert.AreEqual(0, datasets[0].RowsCount);
        }

        [TestMethod]
        public async Task TestWithZeroLengthDataset()
        {
            var datasets = await ImportSourceDatasetAsync("ZeroLengthDataset.dset");

            Assert.AreEqual(1, datasets.Length);

            Assert.AreEqual(3, datasets[0].ColumnsCount);
            Assert.AreEqual(0, datasets[0].RowsCount);

            var expectedColumnNames = new string[] { "Foo", "Bar", "Baz" };
            var actualColumnNames = datasets[0].Columns.Select(c => c.Name).ToArray();

            CollectionAssert.AreEqual(expectedColumnNames, actualColumnNames);
        }

        [TestMethod]
        public async Task TestWithEncryptedFile()
        {
            try
            {
                var datasets = await ImportSourceDatasetAsync("EncryptedMixedDataset.dset");

                Assert.Fail("Import of an encrypted file didn't throw an exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Unsupported item type: EasyMorph.EncryptedField.0", ex.Message);
            }
        }
    }
}

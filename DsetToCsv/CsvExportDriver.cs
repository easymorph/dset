using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using EasyMorph.Data;

namespace DsetToCsv
{
    public class CsvExportDriver
    {
        /// <summary>
        /// Writes CSV data to specified file.
        /// </summary>
        public static void WriteData(IDataset dataset, string fileName, string separator, CancellationToken token)
        {
            using (StreamWriter file = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                token.ThrowIfCancellationRequested();

                int width = dataset.ColumnsCount;
                if (width == 0)
                    return;

                int length = dataset.RowsCount;

                string[] columnNames = dataset.Columns.Select(c => c.Name).ToArray();


                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < width; i++)
                {
                    if (i > 0)
                        sb.Append(separator);

                    AppendValue(sb, columnNames[i]);
                }

                if (length > 0)
                    sb.Append(Environment.NewLine);

                file.Write(sb.ToString());
                sb.Clear();

                token.ThrowIfCancellationRequested();

                // Values.
                for (int row = 0; row < length; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        token.ThrowIfCancellationRequested();

                        if (column > 0)
                            sb.Append(separator);

                        string value;
                        switch (dataset.GetType(column, row))
                        {
                            case CellType.Boolean:
                                value = dataset.GetBoolean(column, row).GetValueOrDefault() ? "TRUE" : "FALSE";
                                break;
                            case CellType.Text:
                                value = dataset.GetText(column, row);
                                break;
                            case CellType.Error:
                                value = dataset.GetError(column, row);
                                break;
                            case CellType.Number:
                                value = dataset.GetNumber(column, row).GetValueOrDefault()
                                    .ToString(CultureInfo.InvariantCulture);
                                break;
                            case CellType.Nothing:
                                value = "";
                                break;
                            default:
                                throw new Exception("Unknown cell type");
                        }

                        AppendValue(sb, value);
                    }

                    if (row < length - 1)
                        sb.Append(Environment.NewLine);

                    file.Write(sb.ToString());
                    sb.Clear();
                }
            }
        }

        const string DoubleQuote = "\"";

        private static void AppendValue(StringBuilder sb, string value)
        {
            sb.Append(DoubleQuote);
            value = value.Replace(DoubleQuote, DoubleQuote + DoubleQuote);
            sb.Append(value);
            sb.Append(DoubleQuote);
        }
    }
}
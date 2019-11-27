using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyMorph.Data;

namespace EasyMorph.Drivers
{

    /// <summary>
    /// Driver to import dataset
    /// </summary>
    class ImportDriver: IImportDriver
    {
        //Version of dset file
        const string VERSION = "HIBC00";
        //Constant which defines compressed type of column in file
        const string COMPRESSED_COLUMN_TYPE_NAME = "Column.Compressed";
        //Constant which defines constant type of column in file
        const string CONSTANT_COLUMN_TYPE_NAME = "Column.Constant";

        /// <summary>
        /// Imports dataset
        /// </summary>
        /// <param name="config">Settings to configure import</param>
        /// <returns>Datasets from file</returns>
        public async Task<IDataset[]> ImportAsync(ImportConfig config)
        {
            // Validate input
            if (config == null)
                throw new ArgumentException("Should be not null", nameof(config));

            if (string.IsNullOrWhiteSpace(config.FileName))
                throw new ArgumentException("Should be not empty", nameof(config.FileName));

            if (!File.Exists(config.FileName))
                throw new FileNotFoundException("File not found.", config.FileName);

            var columns = new List<IColumn>();
            string tableName = "";

            //Open file for read
            using (FileStream fs = new FileStream(config.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    //Read version from file
                    string version = new string(br.ReadChars(6));

                    if (!version.Equals(VERSION))
                        throw new Exception("Wrong file header format.");

                    while (fs.Position < fs.Length)
                    {
                        config.Token.ThrowIfCancellationRequested();

                        //Read table name
                        tableName = br.ReadString();
                        // SectionType, for now only one type is supported: EasyMorph.CompressedField.0
                        br.ReadString();
                        //Length of current block(field)
                        int blockLength = br.ReadInt32();

                        long blockStartPosition = fs.Position;

                        //Read field name
                        string fieldName = br.ReadString();
                        //Read column type Compressed / Constant
                        var columnType = br.ReadString();

                        IColumn column;

                        if (columnType == COMPRESSED_COLUMN_TYPE_NAME)
                            column = await ReadCompressedColumn(br, fieldName, config.Token);
                        else if (columnType == CONSTANT_COLUMN_TYPE_NAME)
                            column = ReadConstantColumn(br, fieldName);
                        else
                            throw new Exception($"Unsupported column type: {columnType}");

                        //Add columns to result
                        columns.Add(column);

                        //Seek to end of block
                        fs.Seek(blockStartPosition + blockLength, SeekOrigin.Begin);
                    }
                }
            }

            return new[]
            {
                (IDataset) new Dataset(columns.ToArray(), tableName)
            };
        }

        /// <summary>
        /// Read constant column from binary reader
        /// </summary>
        /// <param name="br">binary reader</param>
        /// <param name="fieldName">name of read column</param>
        /// <returns>Column from binary reader</returns>
        private static ConstantColumn ReadConstantColumn(BinaryReader br, string fieldName)
        {
            //Read constant with its type
            var (cellType, constant) = ReadCell(br);
            //Read column length
            int length = br.ReadInt32();

            return new ConstantColumn(length, constant, cellType, fieldName);
        }

        /// <summary>
        /// Read compressed column from binary reader
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="fieldName">name of read column</param>
        /// <param name="token">token to monitor cancellation requests</param>
        /// <returns>Column from binary reader</returns>
        private static async Task<CompressedColumn> ReadCompressedColumn(BinaryReader br, string fieldName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            //Read vocabulary
            int vocabularyLength = br.ReadInt32();
            VocabularyItem[] vocabularyItems = new VocabularyItem[vocabularyLength];
            var strings = new List<string>();
            var decimals = new List<decimal>();

            for (int i = 0; i < vocabularyLength; i++)
            {
                token.ThrowIfCancellationRequested();
                vocabularyItems[i] = ReadVocabularyItem(br, strings, decimals);
            }

            var vocabulary = new Vocabulary(strings.ToArray(), decimals.ToArray(), vocabularyItems);
            // The max width of bitindex in bits.
            int bitWidth = (int)Math.Floor(Math.Log(vocabularyLength - 1, 2) + 1);      // -1 because we need max index.

            //Read vector as byte array
            int vectorLength = br.ReadInt32();

            var stream = br.BaseStream;
            var numberOfBytes = (int) Math.Ceiling((double) bitWidth * vectorLength / 8.0);
            byte[] bytes = new byte[numberOfBytes];
            int offset = 0;
            const int chunkSize = 10240;
            while(offset < bytes.Length)
            {
                var count = Math.Min(bytes.Length - offset, chunkSize);
                offset += await stream.ReadAsync(bytes, offset, count, token);
            }

            return new CompressedColumn(bytes, vocabulary, bitWidth, vectorLength, fieldName);
        }

        /// <summary>
        /// Read cell element from reader
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>Cell type and cell value boxed in object</returns>
        /// <exception cref="Exception">Unknown symbol type</exception>
        private static (CellType, object) ReadCell(BinaryReader br)
        {
            SymbolType type = (SymbolType)br.ReadByte();
            object constant;
            CellType cellType;
            switch(type)
            {
                case SymbolType.Nothing:
                    constant = Nothing.Instance;
                    cellType = CellType.Nothing;
                    break;

                case SymbolType.Int8:
                    constant = (decimal) br.ReadSByte();
                    cellType = CellType.Number;
                    break;

                case SymbolType.Int16:
                    constant = (decimal) br.ReadInt16();
                    cellType = CellType.Number;
                    break;

                case SymbolType.Int32:
                    constant = (decimal) br.ReadInt32();
                    cellType = CellType.Number;
                    break;

                case SymbolType.Decimal:
                    constant = br.ReadDecimal();
                    cellType = CellType.Number;
                    break;

                case SymbolType.Text:
                    constant = br.ReadString();
                    cellType = CellType.Text;
                    break;

                case SymbolType.BoolTrue:
                    constant = true;
                    cellType = CellType.Boolean;
                    break;

                case SymbolType.BoolFalse:
                    constant = false;
                    cellType = CellType.Boolean;
                    break;

                case SymbolType.Error:
                    br.ReadInt32();     //Error code. Not used as of now. Reserved for future versions.
                    constant = br.ReadString();
                    cellType = CellType.Error;
                    break;

                default:
                    throw new Exception($"Unknown symbol type: {type}");
            }

            return (cellType, constant);
        }

        /// <summary>
        /// Read cell and store it directly in typed list, to avoid boxing
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="strings">Collection of string values from vocabulary</param>
        /// <param name="decimals">Collection of decimal values from vocabulary</param>
        /// <returns>Vocabulary item with cell type and value index in typed array</returns>
        /// <exception cref="Exception">Unknown symbol type</exception>
        private static VocabularyItem ReadVocabularyItem(BinaryReader br, List<string> strings, List<decimal> decimals)
        {
            SymbolType type = (SymbolType)br.ReadByte();

            VocabularyItem result;
            switch(type)
            {
                case SymbolType.Nothing:
                    result = new VocabularyItem(CellType.Nothing, 0);
                    break;

                case SymbolType.Int8:
                    result = new VocabularyItem(CellType.Number, decimals.Count);
                    decimals.Add(br.ReadSByte());
                    break;

                case SymbolType.Int16:
                    result = new VocabularyItem(CellType.Number, decimals.Count);
                    decimals.Add(br.ReadInt16());
                    break;

                case SymbolType.Int32:
                    result = new VocabularyItem(CellType.Number, decimals.Count);
                    decimals.Add(br.ReadInt32());
                    break;

                case SymbolType.Decimal:
                    result = new VocabularyItem(CellType.Number, decimals.Count);
                    decimals.Add(br.ReadDecimal());
                    break;

                case SymbolType.Text:
                    result = new VocabularyItem(CellType.Text, strings.Count);
                    strings.Add(br.ReadString());
                    break;

                case SymbolType.BoolTrue:
                    result = new VocabularyItem(CellType.Boolean, 1);
                    break;

                case SymbolType.BoolFalse:
                    result = new VocabularyItem(CellType.Boolean, 0);
                    break;

                case SymbolType.Error:
                    br.ReadInt32();     //Error code. Not used as of now. Reserved for future versions.

                    result = new VocabularyItem(CellType.Error, strings.Count);
                    strings.Add(br.ReadString());
                    break;

                default:
                    throw new Exception($"Unknown symbol type: {type}");
            }

            return result;
        }
    }
}
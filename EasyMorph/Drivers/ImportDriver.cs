using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasyMorph.Data;

namespace EasyMorph.Drivers
{
    class ImportDriver: IImportDriver
    {
        const string VERSION = "HIBC00";
        const string COMPRESSED_COLUMN_TYPE_NAME = "Column.Compressed";
        const string CONSTANT_COLUMN_TYPE_NAME = "Column.Constant";

        public async Task<IDataset[]> ImportAsync(ImportConfig config)
        {
            // Validate input
            if (config == null)
                throw new ArgumentException("Should be not null", nameof(config));

            if (string.IsNullOrWhiteSpace(config.FileName))
                throw new ArgumentException("Should be not empty", nameof(config.FileName));

            if (!File.Exists(config.FileName))
                throw new FileNotFoundException("File not found.", config.FileName);

            var items = new List<IColumn>();
            string tableName = null;

            using (FileStream fs = new FileStream(config.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    string version = new string(br.ReadChars(6));

                    if (!version.Equals(VERSION))
                        throw new Exception("Wrong file header format.");

                    while (fs.Position < fs.Length)
                    {
                        config.Token.ThrowIfCancellationRequested();

                        tableName = br.ReadString();
                        br.ReadString(); // SectionType, for now only one type is supported: EasyMorph.CompressedField.0
                        int blockLength = br.ReadInt32();

                        long blockStartPosition = fs.Position;

                        string fieldName = br.ReadString();
                        var columnType = br.ReadString();

                        IColumn column;

                        if (columnType == COMPRESSED_COLUMN_TYPE_NAME)
                            column = await ReadCompressedColumn(br, fieldName, config.Token);
                        else if (columnType == CONSTANT_COLUMN_TYPE_NAME)
                            column = ReadConstantColumn(br, fieldName);
                        else
                            throw new Exception($"Unsupported column type: {columnType}");

                        items.Add(column);

                        fs.Seek(blockStartPosition + blockLength, SeekOrigin.Begin);
                    }
                }
            }

            return new[]
            {
                (IDataset) new Dataset(items.ToArray(), tableName)
            };
        }

        private static ConstantColumn ReadConstantColumn(BinaryReader br, string fieldName)
        {
            var (cellType, constant) = ReadCell(br);
            int length = br.ReadInt32();

            return new ConstantColumn(length, constant, cellType, fieldName);
        }

        private static async Task<CompressedColumn> ReadCompressedColumn(BinaryReader br, string fieldName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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

            int vectorLength = br.ReadInt32();

            var stream = br.BaseStream;
            byte[] bytes = new byte[stream.Length - stream.Position];
            int offset = 0;
            int bytesRead;
            while((bytesRead = await stream.ReadAsync(bytes, offset, 10240, token)) > 0)
            {
                offset += bytesRead;
            }

            return new CompressedColumn(bytes, vocabulary, bitWidth, vectorLength, fieldName);
        }

        private static (CellType, object) ReadCell(BinaryReader br)
        {
            SymbolType type = (SymbolType)br.ReadByte();
            object constant;
            CellType cellType;
            switch(type)
            {
                case SymbolType.Nothing:
                    constant = new Nothing();
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
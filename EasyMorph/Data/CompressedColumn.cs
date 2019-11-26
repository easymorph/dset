namespace EasyMorph.Data
{
    class CompressedColumn: IColumn
    {
        private readonly byte[] _vectorBytes;
        private readonly Vocabulary _vocabulary;
        private readonly int _bitWidth;
        private readonly ulong _mask;
        public string Name { get; }
        public int Length { get; }

        public CompressedColumn(byte[] vectorBytes, Vocabulary vocabulary, int bitWidth, int length, string name)
        {
            _vectorBytes = vectorBytes;
            _vocabulary = vocabulary;
            _bitWidth = bitWidth;
            Length = length;
            Name = name;
            _mask = ((ulong)1 << bitWidth) - 1;
        }

        private int GetVocabularyIndex(int rowIndex)
        {
            var firstBit = rowIndex * _bitWidth;
            var byteIndex = firstBit / 8;
            var byteOffset = firstBit % 8;
            var result = (ulong) (_vectorBytes[byteIndex] >> byteOffset);
            var bitsRead = 8 - byteOffset;
            while (bitsRead < _bitWidth)
            {
                byteIndex++;
                result = (ulong)_vectorBytes[byteIndex] << bitsRead | result;
            }
            return (int)(result & _mask);
        }

        public string GetError(int rowIndex)
        {
            return _vocabulary.GetError(GetVocabularyIndex(rowIndex));
        }

        public string GetText(int rowIndex)
        {
            return _vocabulary.GetText(GetVocabularyIndex(rowIndex));
        }

        public decimal? GetNumber(int rowIndex)
        {
            return _vocabulary.GetNumber(GetVocabularyIndex(rowIndex));
        }

        public bool? GetBoolean(int rowIndex)
        {
            return _vocabulary.GetBoolean(GetVocabularyIndex(rowIndex));
        }

        public Nothing? GetNothing(int rowIndex)
        {
            return _vocabulary.GetNothing(GetVocabularyIndex(rowIndex));
        }

        public CellType GetType(int rowIndex)
        {
            return _vocabulary.GetType(GetVocabularyIndex(rowIndex));
        }
    }
}
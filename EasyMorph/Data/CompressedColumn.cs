namespace EasyMorph.Data
{
    /// <summary>
    /// Compressed column with unique vocabulary and vector with references to values from vocabulary
    /// </summary>
    class CompressedColumn : IColumn
    {
        //Bit-compressed vector array
        private readonly byte[] _vectorBytes;

        //Vocabulary with unique values
        private readonly Vocabulary _vocabulary;

        //Number of bits which takes encode one vector index
        private readonly int _bitWidth;

        //Mask for 00001111 where number of '1' is equal notwidth
        private readonly ulong _mask;

        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Column length
        /// </summary>
        public int Length { get; }

        public CompressedColumn(byte[] vectorBytes, Vocabulary vocabulary, int bitWidth, int length, string name)
        {
            _vectorBytes = vectorBytes;
            _vocabulary = vocabulary;
            _bitWidth = bitWidth;
            Length = length;
            Name = name;
            _mask = ((ulong) 1 << bitWidth) - 1;
        }

        /// <summary>
        /// Decodes vocabularyIndex from vector bytes at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row number from which to get vocabularyIndex</param>
        /// <returns>vocabulary index</returns>
        private int GetVocabularyIndex(int rowIndex)
        {
            //calculate firstBut position in byte array
            ulong firstBit = (ulong) rowIndex * (ulong) _bitWidth;
            //byte index where first bit is located
            var byteIndex = firstBit / 8;
            //Bit number in first byte
            var byteOffset = (int) (firstBit % 8);
            //Move first bit to 0 position and store to result
            var result = (ulong) (_vectorBytes[byteIndex] >> byteOffset);
            //calculate number of read bits
            var bitsRead = 8 - byteOffset;
            //while need to read more bits
            while (bitsRead < _bitWidth)
            {
                //increase byte index
                byteIndex++;
                //store new byte in result
                result = (ulong) _vectorBytes[byteIndex] << bitsRead | result;
                bitsRead += 8;
            }
            //trim result with bit mask
            return (int) (result & _mask);
        }

        /// <summary>
        /// Get Error element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Error</returns>
        public string GetError(int rowIndex)
        {
            return _vocabulary.GetError(GetVocabularyIndex(rowIndex));
        }

        /// <summary>
        /// Get Text element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Text</returns>
        public string GetText(int rowIndex)
        {
            return _vocabulary.GetText(GetVocabularyIndex(rowIndex));
        }

        /// <summary>
        /// Get Number element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Number</returns>
        public decimal? GetNumber(int rowIndex)
        {
            return _vocabulary.GetNumber(GetVocabularyIndex(rowIndex));
        }

        /// <summary>
        /// Get Boolean element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Boolean</returns>
        public bool? GetBoolean(int rowIndex)
        {
            return _vocabulary.GetBoolean(GetVocabularyIndex(rowIndex));
        }

        /// <summary>
        /// Get Nothing element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Nothing</returns>
        public Nothing GetNothing(int rowIndex)
        {
            return _vocabulary.GetNothing(GetVocabularyIndex(rowIndex));
        }

        /// <summary>
        /// Get type of element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get type</param>
        /// <returns>Element type at rowIndex</returns>
        public CellType GetType(int rowIndex)
        {
            return _vocabulary.GetType(GetVocabularyIndex(rowIndex));
        }
    }
}
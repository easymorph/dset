namespace EasyMorph.Data
{
    /// <summary>
    /// Collection with unique elements in column
    /// </summary>
    class Vocabulary
    {
        //Array with string values in vocabulary
        private readonly string[] _strings;
        //Array with decimal values in vocabulary
        private readonly decimal[] _decimals;

        //Array with unique vocabulary items,
        //where item contains information about element type and its index in corresponding typed array
        // for boolean index '1' is true and index '0' is false
        // for nothing index is not relevant
        private readonly VocabularyItem[] _items;

        public Vocabulary(string[] strings, decimal[] decimals, VocabularyItem[] items)
        {
            _strings = strings;
            _decimals = decimals;
            _items = items;
        }

        /// <summary>
        /// Returns 'i' Error element in vocabulary
        /// </summary>
        /// <param name="i">position to get element from</param>
        /// <returns>Error value, null if element was not of Error type</returns>
        public string GetError(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Error ? _strings[item.Index] : null;
        }

        /// <summary>
        /// Returns 'i' Text element in vocabulary
        /// </summary>
        /// <param name="i">position to get element from</param>
        /// <returns>Text value, null if element was not of Text type</returns>
        public string GetText(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Text ? _strings[item.Index] : null;
        }

        /// <summary>
        /// Returns 'i' Number element in vocabulary
        /// </summary>
        /// <param name="i">position to get element from</param>
        /// <returns>Number value, null if element was not of Number type</returns>
        public decimal? GetNumber(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Number ? (int?)_decimals[item.Index] : null;
        }

        /// <summary>
        /// Returns 'i' Boolean element in vocabulary
        /// </summary>
        /// <param name="i">position to get element from</param>
        /// <returns>Boolean value, null if element was not of Boolean type</returns>
        public bool? GetBoolean(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Boolean ? (bool?)(item.Index == 1) : null;
        }

        /// <summary>
        /// Returns 'i' Nothing element in vocabulary
        /// </summary>
        /// <param name="i">position to get element from</param>
        /// <returns>Nothing value, null if element was not of Nothing type</returns>
        public Nothing GetNothing(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Nothing ? Nothing.Instance :null;
        }

        /// <summary>
        /// Get type of element at position i
        /// </summary>
        /// <param name="i">row position to get type</param>
        /// <returns>Element type</returns>
        public CellType GetType(int i)
        {
            return _items[i].Type;
        }
    }
}
namespace EasyMorph.Data
{
    /// <summary>
    /// Information about vocabulary item
    /// </summary>
    struct VocabularyItem
    {
        /// <summary>
        /// Vocabulary item type
        /// </summary>
        public CellType Type { get; }
        /// <summary>
        /// Index of value in typed array
        /// </summary>
        public int Index { get; }

        public VocabularyItem(CellType type, int index)
        {
            Type = type;
            Index = index;
        }
    }
}
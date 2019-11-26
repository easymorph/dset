namespace EasyMorph.Data
{
    struct VocabularyItem
    {
        public CellType Type { get; }
        public int Index { get; }

        public VocabularyItem(CellType type, int index)
        {
            Type = type;
            Index = index;
        }
    }
}
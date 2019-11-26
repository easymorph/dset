namespace EasyMorph.Data
{
    class Vocabulary
    {
        private readonly string[] _strings;
        private readonly decimal[] _decimals;
        //for booleans 0 - false, 1 - true
        private readonly VocabularyItem[] _items;

        public Vocabulary(string[] strings, decimal[] decimals, VocabularyItem[] items)
        {
            _strings = strings;
            _decimals = decimals;
            _items = items;
        }

        public string GetError(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Error ? _strings[item.Index] : null;
        }

        public string GetText(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Text ? _strings[item.Index] : null;
        }

        public decimal? GetNumber(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Number ? (int?)_decimals[item.Index] : null;
        }

        public bool? GetBoolean(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Boolean ? (bool?)(item.Index == 1) : null;
        }

        public Nothing? GetNothing(int i)
        {
            var item = _items[i];
            return item.Type == CellType.Nothing ? (Nothing?)new Nothing() : null;
        }

        public CellType GetType(int i)
        {
            return _items[i].Type;
        }
    }
}
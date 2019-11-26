namespace EasyMorph.Data
{
    class ConstantColumn: IColumn
    {
        private readonly CellType _type;
        private readonly object _constant;
        public string Name { get; }
        public int Length { get; }

        public ConstantColumn(int length, object constant, CellType type, string name)
        {
            Length = length;
            _constant = constant;
            _type = type;
            Name = name;
        }

        public string GetError(int rowIndex)
        {
            return _type == CellType.Error ? (string) _constant: null;
        }

        public string GetText(int rowIndex)
        {
            return _type == CellType.Text ? (string) _constant: null;
        }

        public decimal? GetNumber(int rowIndex)
        {
            return _type == CellType.Number ? (decimal?) _constant: null;
        }

        public bool? GetBoolean(int rowIndex)
        {
            return _type == CellType.Boolean ? (bool?) _constant: null;
        }

        public Nothing? GetNothing(int rowIndex)
        {
            return _type == CellType.Nothing ? (Nothing?) _constant: null;
        }

        public CellType GetType(int rowIndex)
        {
            return _type;
        }
    }
}
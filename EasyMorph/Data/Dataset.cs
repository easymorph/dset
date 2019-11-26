namespace EasyMorph.Data
{
    class Dataset: IDataset
    {
        public IColumn[] Columns { get; }
        public int ColumnsCount => Columns.Length;
        public string Name { get; }
        public int RowsCount => Columns.Length > 0 ? Columns[0].Length: 0;

        public Dataset(IColumn[] columns, string name)
        {
            Columns = columns;
            Name = name;
        }

        public string GetError(int column, int rowIndex)
        {
            return Columns[column].GetError(rowIndex);
        }

        public string GetText(int column, int rowIndex)
        {
            return Columns[column].GetText(rowIndex);
        }

        public decimal? GetNumber(int column, int rowIndex)
        {
            return Columns[column].GetNumber(rowIndex);
        }

        public bool? GetBoolean(int column, int rowIndex)
        {
            return Columns[column].GetBoolean(rowIndex);
        }

        public Nothing? GetNothing(int column, int rowIndex)
        {
            return Columns[column].GetNothing(rowIndex);
        }

        public CellType GetType(int column, int rowIndex)
        {
            return Columns[column].GetType(rowIndex);
        }
    }
}
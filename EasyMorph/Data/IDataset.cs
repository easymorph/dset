namespace EasyMorph.Data
{
    public interface IDataset
    {
        IColumn[] Columns { get; }
        int ColumnsCount { get; }
        string Name { get; }
        int RowsCount { get; }
        string GetError(int column, int rowIndex);
        string GetText(int column, int rowIndex);
        decimal? GetNumber(int column, int rowIndex);
        bool? GetBoolean(int column, int rowIndex);
        Nothing? GetNothing(int column, int rowIndex);
        CellType GetType(int column, int rowIndex);
    }
}
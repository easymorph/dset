namespace EasyMorph.Data
{
    public interface IColumn {
        string Name { get; }
        int Length { get; }

        string GetError(int rowIndex);
        string GetText(int rowIndex);
        decimal? GetNumber(int rowIndex);
        bool? GetBoolean(int rowIndex);
        Nothing? GetNothing(int rowIndex);
        CellType GetType(int rowIndex);
    }
}
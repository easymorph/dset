namespace EasyMorph.Data
{
    /// <summary>
    /// Dataset which represents table with columns
    /// </summary>
    public interface IDataset
    {
        /// <summary>
        /// Dataset columns
        /// </summary>
        IColumn[] Columns { get; }
        /// <summary>
        /// Number of columns in dataset
        /// </summary>
        int ColumnsCount { get; }
        /// <summary>
        /// Column name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Number of rows in dataset
        /// </summary>
        int RowsCount { get; }
        /// <summary>
        /// Get Error element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Error</returns>
        string GetError(int column, int rowIndex);
        /// <summary>
        /// Get Text element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Text</returns>
        string GetText(int column, int rowIndex);
        /// <summary>
        /// Get Number element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Number</returns>
        decimal? GetNumber(int column, int rowIndex);
        /// <summary>
        /// Get Boolean element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Boolean</returns>
        bool? GetBoolean(int column, int rowIndex);
        /// <summary>
        /// Get Nothing element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Nothing</returns>
        Nothing GetNothing(int column, int rowIndex);
        /// <summary>
        /// Get type of element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Type of table element</returns>
        CellType GetType(int column, int rowIndex);
    }
}
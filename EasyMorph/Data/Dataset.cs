namespace EasyMorph.Data
{
    /// <summary>
    /// Dataset which represents table with columns
    /// </summary>
    class Dataset: IDataset
    {
        /// <summary>
        /// Dataset columns
        /// </summary>
        public IColumn[] Columns { get; }
        /// <summary>
        /// Number of columns in dataset
        /// </summary>
        public int ColumnsCount => Columns.Length;
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Number of rows in dataset
        /// </summary>
        public int RowsCount => Columns.Length > 0 ? Columns[0].Length: 0;

        public Dataset(IColumn[] columns, string name)
        {
            Columns = columns;
            Name = name;
        }

        /// <summary>
        /// Get Error element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Error</returns>
        public string GetError(int column, int rowIndex)
        {
            return Columns[column].GetError(rowIndex);
        }

        /// <summary>
        /// Get Text element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Text</returns>
        public string GetText(int column, int rowIndex)
        {
            return Columns[column].GetText(rowIndex);
        }

        /// <summary>
        /// Get Number element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Number</returns>
        public decimal? GetNumber(int column, int rowIndex)
        {
            return Columns[column].GetNumber(rowIndex);
        }

        /// <summary>
        /// Get Boolean element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Boolean</returns>
        public bool? GetBoolean(int column, int rowIndex)
        {
            return Columns[column].GetBoolean(rowIndex);
        }

        /// <summary>
        /// Get Nothing element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Table element , null if element type is not Nothing</returns>
        public Nothing GetNothing(int column, int rowIndex)
        {
            return Columns[column].GetNothing(rowIndex);
        }

        /// <summary>
        /// Get type of element at row 'rowIndex' and column 'column'
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Type of table element</returns>
        public CellType GetType(int column, int rowIndex)
        {
            return Columns[column].GetType(rowIndex);
        }
    }
}
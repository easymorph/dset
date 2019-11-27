namespace EasyMorph.Data
{
    /// <summary>
    /// Column with all same elements
    /// </summary>
    class ConstantColumn: IColumn
    {
        //Element type
        private readonly CellType _type;
        //Element values
        private readonly object _constant;
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Column length
        /// </summary>
        public int Length { get; }

        public ConstantColumn(int length, object constant, CellType type, string name)
        {
            Length = length;
            _constant = constant;
            _type = type;
            Name = name;
        }

        /// <summary>
        /// Get Error element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Error</returns>
        public string GetError(int rowIndex)
        {
            return _type == CellType.Error ? (string) _constant: null;
        }

        /// <summary>
        /// Get Text element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Text</returns>
        public string GetText(int rowIndex)
        {
            return _type == CellType.Text ? (string) _constant: null;
        }

        /// <summary>
        /// Get Number element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Number</returns>
        public decimal? GetNumber(int rowIndex)
        {
            return _type == CellType.Number ? (decimal?) _constant: null;
        }

        /// <summary>
        /// Get Boolean element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Boolean</returns>
        public bool? GetBoolean(int rowIndex)
        {
            return _type == CellType.Boolean ? (bool?) _constant: null;
        }

        /// <summary>
        /// Get Nothing element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Nothing</returns>
        public Nothing GetNothing(int rowIndex)
        {
            return _type == CellType.Nothing ? (Nothing) _constant: null;
        }

        /// <summary>
        /// Get type of element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get type</param>
        /// <returns>Element type at rowIndex</returns>
        public CellType GetType(int rowIndex)
        {
            return _type;
        }
    }
}
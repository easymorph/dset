namespace EasyMorph.Data
{
    /// <summary>
    /// Column with number of elements, where elements can have different types
    /// </summary>
    public interface IColumn {
        /// <summary>
        /// Column name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Column length
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get Error element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Error</returns>
        string GetError(int rowIndex);
        /// <summary>
        /// Get Text element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Text</returns>
        string GetText(int rowIndex);
        /// <summary>
        /// Get Number element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Number</returns>
        decimal? GetNumber(int rowIndex);
        /// <summary>
        /// Get Boolean element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Boolean</returns>
        bool? GetBoolean(int rowIndex);
        /// <summary>
        /// Get Nothing element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get element</param>
        /// <returns>Element at rowIndex, null if element type is not Nothing</returns>
        Nothing GetNothing(int rowIndex);
        /// <summary>
        /// Get type of element at position rowIndex
        /// </summary>
        /// <param name="rowIndex">row position to get type</param>
        /// <returns>Element type at rowIndex</returns>
        CellType GetType(int rowIndex);
    }
}
using System;

namespace EasyMorph.Data
{
    /// <summary>
    /// Type of cell in dataset
    /// </summary>
    public enum CellType
    {
        Number = 0,
        Text = 1,
        Boolean = 2,
        Nothing = 3,
        Error = 4
    }

    /// <summary>
    /// Nothing value
    /// </summary>
    public class Nothing : IEquatable<Nothing>
    {
        private Nothing()
        {
        }

        public static Nothing Instance { get; } = new Nothing();

        public override bool Equals(object obj)
        {
            return Equals(obj as Nothing);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Nothing obj)
        {
            return obj != null;
        }
    }
}
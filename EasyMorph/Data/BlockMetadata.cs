namespace EasyMorph.Data
{
    public class BlockMetadata
    {
        public BlockMetadata(string name, string type, long? blockStartPosition, long? blockLength)
        {
            BlockLength = blockLength;
            Name = name;
            Type = type;
            BlockStartPosition = blockStartPosition;
        }

        /// <summary>
        /// Length of block content 
        /// </summary>
        /// <remarks>
        /// The length of the block without it's name, type and length
        /// records.
        /// 
        /// Calculated as a difference between Stream.Postion values
        /// before and after execution of the WriterBase.WriteContent() method.
        /// </remarks>
        public long? BlockLength { get; }
        /// <summary>
        /// Start of block content
        /// </summary>
        /// <remarks>
        /// Value of the Stream.Position after block's name, type and length
        /// are read from the stream.
        /// </remarks>
        public long? BlockStartPosition { get; }

        public string Name { get; }
        public string Type { get; }
    }
}

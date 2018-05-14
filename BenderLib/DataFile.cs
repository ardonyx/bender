﻿namespace BenderLib
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Wraps arbitrary data
    /// </summary>
    [DebuggerDisplay("Size = {Size}")]
    public class DataFile
    {
        private readonly byte[] _mData;

        private DataFile(byte[] data)
        {
            _mData = data;
        }

        /// <summary>
        /// Wraps data in a DataFile
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataFile From(byte[] data)
        {
            return new DataFile(data);
        }

        /// <summary>
        /// Reads bytes from filePath and returns new DataFile. If file read
        /// fails, and empty DataFile will be returned
        /// </summary>
        /// <param name="filePath">Path to file to read</param>
        /// <returns>Datafile</returns>
        public static DataFile From(string filePath)
        {
            try
            {
                return new DataFile(File.ReadAllBytes(filePath));
            } catch {
                return new DataFile(new byte[0]);
            }
        }

        /// <summary>
        /// Returns raw data as a UTF8 string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => System.Text.Encoding.UTF8.GetString(_mData);

        /// <summary>
        /// Wraps raw data in a string reader
        /// </summary>
        /// <returns></returns>
        public StringReader AsStringReader()
        {
            return new StringReader(ToString());
        }

        /// <summary>
        /// Returns a copy of underlying data
        /// </summary>
        public byte[] Data => (byte[])_mData.Clone();

        /// <summary>
        /// Returns true if this DataFile is empty
        /// </summary>
        public bool Empty => _mData.Length == 0;

        /// <summary>
        /// Returns length of data in bytes
        /// </summary>
        /// <returns></returns>
        public int Size => _mData.Length;
    }
}

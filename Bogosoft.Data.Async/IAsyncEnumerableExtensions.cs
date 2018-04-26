using Bogosoft.Collections.Async;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Bogosoft.Data.Async
{
    /// <summary>
    /// Extended functionality for an <see cref="IAsyncEnumerable{T}"/> type.
    /// </summary>
    public static class IAsyncEnumerableExtensions
    {
        /// <summary>
        /// Convert the current asynchronously enumerable sequence into a data reader.
        /// </summary>
        /// <typeparam name="T">The type of the items in the current sequence.</typeparam>
        /// <param name="items">The current <see cref="IAsyncEnumerable{T}"/> implementation.</param>
        /// <param name="fields">
        /// A sequence of field adapters for the new data record. The ordinal position of each field in the sequence
        /// will populate the corresponding ordinal position in a record with its value extractor.
        /// </param>
        /// <returns>A new data reader.</returns>
        public static DbDataReader ToDbDataReader<T>(this IAsyncEnumerable<T> items, IEnumerable<FieldAdapter<T>> fields)
        {
            return new AsyncCollectionToDataReaderAdapter<T>(items.GetEnumerator(), fields.ToArray());
        }

        /// <summary>
        /// Convert the current asynchronously enumerable sequence into a data reader.
        /// </summary>
        /// <typeparam name="T">The type of the items in the current sequence.</typeparam>
        /// <param name="items">The current <see cref="IAsyncEnumerable{T}"/> implementation.</param>
        /// <param name="fields">
        /// A sequence of field adapters for the new data record. The ordinal position of each field in the sequence
        /// will populate the corresponding ordinal position in a record with its value extractor.
        /// </param>
        /// <returns>A new data reader.</returns>
        public static DbDataReader ToDbDataReader<T>(this IAsyncEnumerable<T> items, params FieldAdapter<T>[] fields)
        {
            return new AsyncCollectionToDataReaderAdapter<T>(items.GetEnumerator(), fields);
        }
    }
}
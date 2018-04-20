using Bogosoft.Collections.Async;
using System;
using System.Data.Common;

namespace Bogosoft.Data.Async
{
    /// <summary>
    /// Extended functionality for the <see cref="DbDataReader"/> type.
    /// </summary>
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// Convert the current .NET data reader into an asynchronously enumerable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the items in the new collection.</typeparam>
        /// <param name="reader">The current <see cref="DbDataReader"/>.</param>
        /// <param name="mapper">
        /// A strategy for mapping each record of the current data reader to an object of the specified type.
        /// </param>
        /// <returns>An asynchronously enumerable sequence of items of the specified type.</returns>
        /// <exception cref="ArgumentNullException">
        /// Throw in the event that the either the current reader or given mapping strategy is null.
        /// </exception>
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(
            this DbDataReader reader,
            Func<DbDataReader, T> mapper
            )
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            return new DbDataReaderToAsyncCollectionAdapter<T>(reader, mapper);
        }

    }
}
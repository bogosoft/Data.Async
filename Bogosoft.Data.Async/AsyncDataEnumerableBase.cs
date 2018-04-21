using Bogosoft.Collections.Async;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Data.Async
{
    /// <summary>
    /// A partial implementation of an asynchronously enumerable data reader which handles the connection,
    /// command and reader construction as well as automatic disposal.
    /// </summary>
    /// <typeparam name="TConnection">The type of the database connection.</typeparam>
    /// <typeparam name="TCommand">The type of the database command to be executed.</typeparam>
    /// <typeparam name="TReader">The type of the data reader.</typeparam>
    /// <typeparam name="TEntity">The type of the objects in the sequence.</typeparam>
    public abstract class AsyncDataEnumerableBase<TConnection, TCommand, TReader, TEntity> : IAsyncEnumerable<TEntity>
        where TConnection : DbConnection
        where TCommand : DbCommand
        where TReader : DbDataReader
    {
        /// <summary>
        /// Get a value indicating whether the current sequence is fully asynchronous, i.e. if during entity mapping,
        /// an asynchronous mapper will be used.
        /// </summary>
        protected abstract bool IsFullyAsync { get; }

        /// <summary>
        /// When overridden in a derived class, builds a command off of a given database connection.
        /// </summary>
        /// <param name="connection">A database connection that can be used to build a new command.</param>
        /// <returns>An executable database command.</returns>
        protected abstract TCommand BuildCommand(TConnection connection);

        /// <summary>
        /// Get an object that represents a connection to a data source.
        /// </summary>
        /// <returns>A data source connection.</returns>
        protected abstract TConnection Connect();

        /// <summary>
        /// Get an object capable of asynchronously enumerating over the current sequence.
        /// </summary>
        /// <returns>An asynchronous enumerator.</returns>
        public IAsyncEnumerator<TEntity> GetEnumerator()
        {
            TCommand command = null;
            TConnection connection = null;
            TReader reader = null;

            try
            {
                connection = Connect();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                command = BuildCommand(connection);

                reader = command.ExecuteReader() as TReader;
            }
            catch (Exception)
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();

                throw;
            }

            if (IsFullyAsync)
            {
                return new FullyAsyncDataEnumerator<TConnection, TCommand, TReader, TEntity>(
                    connection,
                    command,
                    reader,
                    MapAsync
                    );
            }
            else
            {
                return new AsyncDataEnumerator<TConnection, TCommand, TReader, TEntity>(
                    connection,
                    command,
                    reader,
                    Map
                    );
            }
        }

        /// <summary>
        /// Convert the current row in a given data reader into an object of the entity type.
        /// </summary>
        /// <param name="reader">A database reader.</param>
        /// <returns>An object of the entity type.</returns>
        protected abstract TEntity Map(TReader reader);

        /// <summary>
        /// Convert the current row in a given data reader into an object of the entity type.
        /// </summary>
        /// <param name="reader">A database reader.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>An object of the entity type.</returns>
        protected abstract Task<TEntity> MapAsync(TReader reader, CancellationToken token);
    }
}
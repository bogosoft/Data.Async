using Bogosoft.Collections.Async;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Data.Async
{
    class AsyncDataEnumerator<TConnection, TCommand, TReader, TEntity> : IAsyncEnumerator<TEntity>
        where TConnection : DbConnection
        where TCommand : DbCommand
        where TReader : DbDataReader
    {
        TCommand command;
        TConnection connection;
        Func<TReader, TEntity> mapper;
        TReader reader;

        public TEntity Current => mapper.Invoke(reader);

        internal AsyncDataEnumerator(
            TConnection connection,
            TCommand command,
            TReader reader,
            Func<TReader, TEntity> mapper
            )
        {
            this.command = command;
            this.connection = connection;
            this.mapper = mapper;
            this.reader = reader;
        }

        public void Dispose()
        {
            reader.Dispose();
            command.Dispose();
            connection.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken token) => reader.ReadAsync(token);
    }
}
using Bogosoft.Collections.Async;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Data.Async
{
    class FullyAsyncDataEnumerator<TConnection, TCommand, TReader, TEntity> : IAsyncEnumerator<TEntity>
        where TConnection : DbConnection
        where TCommand : DbCommand
        where TReader : DbDataReader
    {
        TEntity buffer;
        TCommand command;
        TConnection connection;
        Func<TReader, CancellationToken, Task<TEntity>> mapper;
        TReader reader;

        public TEntity Current => buffer;

        internal FullyAsyncDataEnumerator(
            TConnection connection,
            TCommand command,
            TReader reader,
            Func<TReader, CancellationToken, Task<TEntity>> mapper
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

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            if (await reader.ReadAsync(token))
            {
                buffer = await mapper.Invoke(reader, token);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
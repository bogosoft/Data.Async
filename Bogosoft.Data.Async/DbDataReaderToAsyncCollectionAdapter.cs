using Bogosoft.Collections.Async;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Data.Async
{
    class DbDataReaderToAsyncCollectionAdapter<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        Func<DbDataReader, T> mapper;
        DbDataReader reader;

        public T Current => mapper.Invoke(reader);

        internal DbDataReaderToAsyncCollectionAdapter(DbDataReader reader, Func<DbDataReader, T> mapper)
        {
            this.mapper = mapper;
            this.reader = reader;
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        public IAsyncEnumerator<T> GetEnumerator() => this;

        public Task<bool> MoveNextAsync(CancellationToken token) => reader.ReadAsync(token);
    }
}
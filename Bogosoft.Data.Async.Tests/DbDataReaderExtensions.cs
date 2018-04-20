using Shouldly;
using System.Data.Common;

namespace Bogosoft.Data.Async.Tests
{
    static class DbDataReaderExtensions
    {
        internal static void ShouldHaveSameDataAs(this DbDataReader actual, DbDataReader expected)
        {
            actual.FieldCount.ShouldBe(expected.FieldCount);

            var len = actual.FieldCount;

            while (actual.Read())
            {
                expected.Read().ShouldBeTrue();

                for (var i = 0; i < len; i++)
                {
                    actual.GetValue(i).ShouldBe(expected.GetValue(i));
                }
            }

            expected.Read().ShouldBeFalse();
        }
    }
}
using Bogosoft.Collections.Async;
using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Bogosoft.Data.Async.Tests
{
    [TestFixture, Category("End2End")]
    public class End2EndTests
    {
        const string Mass = "Mass";
        const string Name = "Name";
        const string PrimaryDistance = "Distance to Primary";
        const string Type = "Type";

        static List<FieldAdapter<CelestialBody>> Columns = new List<FieldAdapter<CelestialBody>>();

        static CelestialBody ReadCelestialBody(DbDataReader reader)
        {
            return new CelestialBody
            {
                Mass = reader.GetFieldValue<float>(Mass),
                Name = reader.GetFieldValue<string>(Name),
                Orbit = new OrbitalInfo
                {
                    DistanceToPrimary = reader.GetFieldValue<float>(PrimaryDistance)
                },
                Type = (CelestialBodyType)Enum.Parse(typeof(CelestialBodyType), reader.GetFieldValue<string>(Type))
            };
        }

        [OneTimeSetUp]
        public void Setup()
        {
            Columns.Add(new FieldAdapter<CelestialBody>(Name, typeof(string), x => x.Name));
            Columns.Add(new FieldAdapter<CelestialBody>(Type, typeof(string), x => x.Type.ToString()));
            Columns.Add(new FieldAdapter<CelestialBody>(Mass, typeof(float), x => x.Mass));
            Columns.Add(new FieldAdapter<CelestialBody>(PrimaryDistance, typeof(float), x => x.Orbit.DistanceToPrimary));
        }

        [TestCase]
        public async Task CanConvertAsyncEnumerableToDbDataReaderAndBack()
        {
            var expected = CelestialBody.All.ToAsyncEnumerable();

            var actual = expected.ToDbDataReader(Columns).ToAsyncEnumerable(ReadCelestialBody);

            using (var a = actual.GetEnumerator())
            using (var e = expected.GetEnumerator())
            {
                while (await a.MoveNextAsync())
                {
                    (await e.MoveNextAsync()).ShouldBeTrue();

                    a.Current.ShouldNotBeNull();
                    e.Current.ShouldNotBeNull();

                    a.Current.Mass.ShouldBe(e.Current.Mass);
                    a.Current.Name.ShouldBe(e.Current.Name);

                    if (a.Current.Orbit is null)
                    {
                        e.Current.Orbit.ShouldBeNull();
                    }
                    else
                    {
                        a.Current.Orbit.DistanceToPrimary.ShouldBe(e.Current.Orbit.DistanceToPrimary);
                    }

                    a.Current.Type.ShouldBe(e.Current.Type);
                }

                (await e.MoveNextAsync()).ShouldBeFalse();
            }
        }

        [TestCase]
        public void CanConvertDbDataReaderToAsyncEnumerableAndBack()
        {
            new CelestialBodyDataReader().ToAsyncEnumerable(ReadCelestialBody)
                                         .ToDbDataReader(Columns)
                                         .ShouldHaveSameDataAs(new CelestialBodyDataReader());
        }
    }
}
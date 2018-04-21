﻿using Bogosoft.Collections.Async;
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

        static List<Column<CelestialBody>> Columns = new List<Column<CelestialBody>>();

        static bool AreEqual(CelestialBody a, CelestialBody b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else
            {
                return a.Mass == b.Mass
                    && a.Name == b.Name
                    && a.Orbit?.DistanceToPrimary == b.Orbit?.DistanceToPrimary
                    && a.Type == b.Type;
            }
        }

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
#if NET45
                Type = (CelestialBodyType)Enum.Parse(typeof(CelestialBodyType), reader.GetFieldValue<string>(Type))
#else
                Type = Enum.Parse<CelestialBodyType>(reader.GetFieldValue<string>(Type))
#endif
            };
        }

        [OneTimeSetUp]
        public void Setup()
        {
            Columns.Add(new Column<CelestialBody>(Name, typeof(string), x => x.Name));
            Columns.Add(new Column<CelestialBody>(Type, typeof(string), x => x.Type.ToString()));
            Columns.Add(new Column<CelestialBody>(Mass, typeof(float), x => x.Mass));
            Columns.Add(new Column<CelestialBody>(PrimaryDistance, typeof(float), x => x.Orbit.DistanceToPrimary));
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

                    AreEqual(a.Current, e.Current).ShouldBeTrue();
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
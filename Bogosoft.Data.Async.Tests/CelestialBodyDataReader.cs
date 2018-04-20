using Bogosoft.Testing.Objects;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bogosoft.Data.Async.Tests
{
    class CelestialBodyDataReader : ExtendedDataReaderBase
    {
        IEnumerator<CelestialBody> enumerator = CelestialBody.All.GetEnumerator();

        DataTable schemaTable;

        public override object this[int ordinal] => GetValue(ordinal);

        public override int Depth => 0;

        public override int FieldCount => 4;

        public override bool HasRows => true;

        public override bool IsClosed => enumerator == null;

        public override int RecordsAffected => 0;

        internal CelestialBodyDataReader()
        {
            var columns = (schemaTable = new DataTable()).Columns;

            columns.Add("Name", typeof(string));
            columns.Add("Type", typeof(string));
            columns.Add("Mass", typeof(float));
            columns.Add("Distance to Primary", typeof(float));
        }

        public override void Close()
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            enumerator.Dispose();

            enumerator = null;
        }

        public override Type GetFieldType(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                case 1:
                    return typeof(string);
                case 2:
                case 3:
                    return typeof(float);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string GetName(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return "Name";
                case 1:
                    return "Type";
                case 2:
                    return "Mass";
                case 3:
                    return "Distance to Primary";
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override int GetOrdinal(string name)
        {
            switch (name.ToLower())
            {
                case "distance to primary":
                    return 3;
                case "mass":
                    return 2;
                case "name":
                    return 0;
                case "type":
                    return 1;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override DataTable GetSchemaTable() => schemaTable;

        public override long GetValue<T>(int ordinal, long dataOffset, T[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return enumerator.Current.Name;
                case 1:
                    return enumerator.Current.Type.ToString();
                case 2:
                    return enumerator.Current.Mass;
                case 3:
                    return enumerator.Current.Orbit.DistanceToPrimary;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override int GetValues(object[] values)
        {
            values[0] = enumerator.Current.Name;
            values[1] = enumerator.Current.Type.ToString();
            values[2] = enumerator.Current.Mass;
            values[3] = enumerator.Current.Orbit.DistanceToPrimary;

            return 4;
        }

        public override bool IsDBNull(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return enumerator.Current.Name is null;
                case 1:
                case 2:
                case 3:
                    return false;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override bool NextResult() => false;

        public override bool Read() => enumerator.MoveNext();
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

namespace CellLib
{
    [JsonConverter(typeof(CellLocation))]
    public struct CellLocation : IEquatable<CellLocation>, IEnumerable<CellLocation>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public CellLocation() => Row = Column = 0;

        public CellLocation(int column, int row)
        {
            Row = row;
            Column = column;
        }

        private CellLocation(Ways ways) : this()
        {
            foreach (var (way, modifer) in rowModifier)
                if (ways.HasFlag(way))
                {
                    Row += modifer;
                    break;
                }
                    

            foreach (var (way, modifer) in columnModifier)
                if (ways.HasFlag(way))
                {
                    Column += modifer;
                    break;
                }
                    
        }

        private static readonly (Ways way, int modifier)[] rowModifier = new[]
        {
            (Ways.Top, 1),
            (Ways.TopRight, 1),
            (Ways.BottomRight, -1),
            (Ways.Bottom, -1),
            (Ways.BottomLeft, -1),
            (Ways.TopLeft, 1),
        };

        private static readonly (Ways way, int modifier)[] columnModifier = new[]
        {
            (Ways.TopRight, 1),
            (Ways.Right, 1),
            (Ways.BottomRight, 1),
            (Ways.BottomLeft, -1),
            (Ways.Left, -1),
            (Ways.TopLeft, -1),
        };

        public static CellLocation operator *(CellLocation a, int b) => new(a.Row * b, a.Column * b);
        public static CellLocation operator +(CellLocation a, CellLocation b) => new(a.Column + b.Column, a.Row + b.Row);
        public static CellLocation operator +(CellLocation a, Ways way) => a + new CellLocation(way);
        public static CellLocation operator +(CellLocation a, (Ways way, int length) b) => a + (new CellLocation(b.way) * b.length);

        public static bool operator ==(CellLocation a, CellLocation b) => a.Column == b.Column && a.Row == b.Row;
        public static bool operator !=(CellLocation a, CellLocation b) => a.Column != b.Column || a.Row != b.Row;

        public static implicit operator CellLocation((int column, int row) tuple) => new(tuple.column, tuple.row);
        public static explicit operator CellLocation((uint column, uint row) tuple) => new((int)tuple.column, (int)tuple.row);

        public override string ToString() => $"({Column},{Row})";
        public override bool Equals(object obj)
        {
            if(obj is CellLocation b)
                return this == b;
            return base.Equals(obj);
        
        }
        public bool Equals(CellLocation other) => this == other;

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public IEnumerator<CellLocation> GetEnumerator()
        {
            yield return this;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

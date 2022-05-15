using CellLib;
using ManewryMorskie;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManewryTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FieldIteratorTest()
        {
            RectangleCellMap<MapField> map = new(12, 18);

            map[(11, 0)].Unit = new Pancernik();
            map[(0, 17)].Unit = new OkretPodwodny();

            int i = 0;

            foreach (MapField field in map)
            {
                if(i++ == 0)
                {
                    if(field.Unit is not OkretPodwodny)
                        Assert.Fail();
                }
                else if(i == map.Width*map.Height)
                {
                    if (field.Unit is not Pancernik)
                        Assert.Fail();
                }
            }
        }

        [Test]
        public void NextCells()
        {
            CellLocation pivot = (3, 3);

            IList<CellLocation> locations = pivot.NextLocations(Ways.Right | Ways.BottomLeft, 2);

            CellLocation[] expectedLocations = new[]
            {
                (4, 3),
                (5, 3),
                pivot,
                (2, 2),
                (1, 1),
            };

            foreach (CellLocation item in locations)
                if (!expectedLocations.Contains(item))
                    Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public void SelectRegion()
        {
            IEnumerable<CellLocation>[] cellLocations = new[]
            {
                (1, 1).Region((3, 3)),
                (3, 1).Region((1, 3)),
                (3, 3).Region((1, 1)),
            };

            IEnumerable<CellLocation> expectedLocations = new CellLocation[]
            {
                (1,1),
                (1,2),
                (1,3),
                (2,1),
                (2,2),
                (2,3),
                (3,1),
                (3,2),
                (3,3),
            };

            foreach (IEnumerable<CellLocation> cellLocationSet in cellLocations)
                if (!expectedLocations.SequenceEqual(cellLocationSet, EqualityComparer<CellLocation>.Default))
                    Assert.Fail();
                

            Assert.Pass();
        }

        [Test]
        public void InternationalWaterManagerTest()
        {
            Unit pancernik = new Pancernik();
            Unit podwodny = new OkretPodwodny();
            Unit rakietowy = new OkretRakietowy();

            int ticks = 0;

            RectangleCellMap<MapField> map = new(12, 18);
            map[(6, 7)].Unit = podwodny;
            map[(6, 9)].Unit = pancernik;

            map.MarkInternationalWaters((5, 8).NextLocations(Ways.All));

            var manager = new InternationalWaterManager(map, 3);

            manager.InternedUnit += (sender, unit) => 
            {
                if (unit == pancernik)
                    Assert.Fail();
                if (unit == podwodny && ticks != 3)
                    Assert.Fail();
                if(unit == rakietowy && ticks != 4)
                    Assert.Fail();
            };

            ticks++;
            manager.Iterate();
            
            map[(5, 8)].Unit = rakietowy;

            map[(6, 9)].Unit = null;
            map[(0, 0)].Unit = pancernik;

            for(int i = 0; i < 3; i++, ++ticks)
                manager.Iterate();

            map[(6, 9)].Unit = pancernik;

            for (int i = 0; i < 2; i++, ++ticks)
                manager.Iterate();
        }

        [Test]
        public void BarrierBuilder()
        {
            RectangleCellMap<MapField> map = new(12, 18);

            BarrierBuilder barierBuilder = new(map);
            barierBuilder
                .AddRange((0, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((3, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((5, 1).NextLocations(Ways.Right, 3).Select(l => (l, l + Ways.Top)))
                .AddRange((10, 2).NextLocations(Ways.Right).Select(l => (l, l + Ways.Top)))
                .AddRange((1, 3).NextLocations(Ways.Right).Select(l => (l, l + Ways.Right)))
                .AddRange((4, 3).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Right)))
                .AddRange((10, 2).NextLocations(Ways.Bottom).Select(l => (l, l + Ways.Left)))
                .Add(((8, 1), (9, 1)))
                .AddSimmetricBarriers()
                .BuildBarriers();

            List<(CellLocation, Ways)> expected = new()
            {
                ((0, 3), Ways.Top | Ways.Left),
                ((11, 17), Ways.Top | Ways.Right),

                ((4, 3), Ways.Top | Ways.Right),
                ((1, 3), Ways.Top | Ways.Right),
                ((2, 3), Ways.Right | Ways.Left),
                ((3, 3), Ways.Top | Ways.Left),

                ((9, 1), Ways.Right | Ways.Left),
                ((6, 15), Ways.Top | Ways.Right),
                ((9, 14), Ways.Left | Ways.Right),
                ((4, 16), Ways.Bottom),
            };

            foreach (var item in expected)
                if (map[item.Item1].Barriers != item.Item2)
                    Assert.Fail();

            Assert.Pass();
        }
    }
}
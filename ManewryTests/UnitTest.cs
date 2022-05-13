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

            IList<CellLocation> locations = pivot.GetNext(Ways.Right | Ways.BottomLeft, 2);

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
            IEnumerable<CellLocation> cellLocations = (1, 1).GetRegion((3, 3));

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

            foreach (CellLocation location in expectedLocations)
                if (!cellLocations.Contains(location))
                    Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public void BarrierManager()
        {
            var manager = new BarrierManager();

            CellLocation refer = (0, 0);

            manager.Add((refer, (0, 1)));
            manager.Add(((1, 0), refer));

            Ways barriers = manager.GetBarriers(refer);

            if (!barriers.Contain(Ways.Right))
                Assert.Fail();
            if(!barriers.Contain(Ways.Top))
                Assert.Fail();
            if (barriers.Contain(Ways.TopRight))
                Assert.Fail();
            if (barriers.Contain(Ways.Bottom))
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

            var manager = new InternationalWaterManager(map, (5,8).GetNext(Ways.All), 3);

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
    }
}
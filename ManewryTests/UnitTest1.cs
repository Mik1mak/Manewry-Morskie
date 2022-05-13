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

            map[new CellLocation(11, 0)].Unit = new Pancernik();
            map[new CellLocation(0, 17)].Unit = new OkretPodwodny();

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
            var pivot = new CellLocation(3, 3);

            IList<CellLocation> locations = pivot.GetNext(Ways.Right | Ways.BottomLeft, 2);

            CellLocation[] expectedLocations = new[]
            {
                new CellLocation(4, 3),
                new CellLocation(5, 3),
                pivot,
                new CellLocation(2, 2),
                new CellLocation(1, 1),
            };

            foreach (CellLocation item in locations)
                if (!expectedLocations.Contains(item))
                    Assert.Fail();

            Assert.Pass();
        }
    }
}
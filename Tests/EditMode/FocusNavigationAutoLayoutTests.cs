using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.FocusNav;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class FocusNavigationAutoLayoutTests
    {
        [Test]
        public void GenerateNearest_SimpleRow_LinksLeftAndRight()
        {
            var elements = new List<FocusNavigationAutoLayout.ElementBounds>
            {
                new FocusNavigationAutoLayout.ElementBounds("left", new Vector2(0f, 0f)),
                new FocusNavigationAutoLayout.ElementBounds("middle", new Vector2(100f, 0f)),
                new FocusNavigationAutoLayout.ElementBounds("right", new Vector2(200f, 0f))
            };

            var links = FocusNavigationAutoLayout.GenerateNearest(elements);

            Assert.AreEqual("middle", links["left"].RightElementId);
            Assert.IsNull(links["left"].LeftElementId);
            Assert.AreEqual("left", links["middle"].LeftElementId);
            Assert.AreEqual("right", links["middle"].RightElementId);
            Assert.AreEqual("middle", links["right"].LeftElementId);
        }

        [Test]
        public void GenerateNearest_Grid_LinksUpAndDown()
        {
            // Y increases downward (Designer canvas convention), so "top" at y=0 is above "bottom" at y=100.
            var elements = new List<FocusNavigationAutoLayout.ElementBounds>
            {
                new FocusNavigationAutoLayout.ElementBounds("top", new Vector2(0f, 0f)),
                new FocusNavigationAutoLayout.ElementBounds("bottom", new Vector2(0f, 100f))
            };

            var links = FocusNavigationAutoLayout.GenerateNearest(elements);

            Assert.AreEqual("bottom", links["top"].DownElementId);
            Assert.AreEqual("top", links["bottom"].UpElementId);
            Assert.IsNull(links["top"].UpElementId);
        }

        [Test]
        public void GenerateNearest_PicksNearestOverFarthestInSameDirection()
        {
            var elements = new List<FocusNavigationAutoLayout.ElementBounds>
            {
                new FocusNavigationAutoLayout.ElementBounds("origin", new Vector2(0f, 0f)),
                new FocusNavigationAutoLayout.ElementBounds("near", new Vector2(50f, 0f)),
                new FocusNavigationAutoLayout.ElementBounds("far", new Vector2(500f, 0f))
            };

            var links = FocusNavigationAutoLayout.GenerateNearest(elements);

            Assert.AreEqual("near", links["origin"].RightElementId);
        }

        [Test]
        public void GenerateNearest_NoElementInDirection_LeavesLinkNull()
        {
            var elements = new List<FocusNavigationAutoLayout.ElementBounds>
            {
                new FocusNavigationAutoLayout.ElementBounds("solo", new Vector2(0f, 0f))
            };

            var links = FocusNavigationAutoLayout.GenerateNearest(elements);

            Assert.IsNull(links["solo"].UpElementId);
            Assert.IsNull(links["solo"].DownElementId);
            Assert.IsNull(links["solo"].LeftElementId);
            Assert.IsNull(links["solo"].RightElementId);
        }
    }
}

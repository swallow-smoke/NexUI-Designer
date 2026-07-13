using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Scenario;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure evaluation tests for the Scenario timeline (brief §35.2): numeric keys interpolate between
    /// neighbors, bool/text keys step, keys start only at their first keyframe, and the output feeds
    /// the same binding shape the static-scenario resolver consumes.
    /// </summary>
    public sealed class ScenarioTimelineEvaluatorTests
    {
        private static DesignerScenarioTimelineKey NumberKey(float time, string key, float value)
            => new DesignerScenarioTimelineKey(time, key, DesignerScenarioValueKind.Number) { numberValue = value };

        [Test]
        public void NumericKey_InterpolatesLinearlyBetweenNeighbors()
        {
            var keys = new List<DesignerScenarioTimelineKey>
            {
                NumberKey(0f, "hp", 100f),
                NumberKey(2f, "hp", 0f)
            };

            Assert.AreEqual(100f, Value(keys, 0f), 0.001f);
            Assert.AreEqual(50f, Value(keys, 1f), 0.001f);
            Assert.AreEqual(25f, Value(keys, 1.5f), 0.001f);
            Assert.AreEqual(0f, Value(keys, 2f), 0.001f);
        }

        [Test]
        public void PastLastKey_HoldsFinalValue()
        {
            var keys = new List<DesignerScenarioTimelineKey> { NumberKey(0f, "hp", 100f), NumberKey(2f, "hp", 10f) };
            Assert.AreEqual(10f, Value(keys, 5f), 0.001f);
        }

        [Test]
        public void BeforeFirstKey_KeyIsOmitted()
        {
            var keys = new List<DesignerScenarioTimelineKey> { NumberKey(1f, "hp", 100f) };
            Assert.IsEmpty(ScenarioTimelineEvaluator.EvaluateAt(keys, 0.5f));
            Assert.AreEqual(1, ScenarioTimelineEvaluator.EvaluateAt(keys, 1f).Count);
        }

        [Test]
        public void BoolKey_StepsNoInterpolation()
        {
            var keys = new List<DesignerScenarioTimelineKey>
            {
                new DesignerScenarioTimelineKey(0f, "healing", DesignerScenarioValueKind.Bool) { boolValue = false },
                new DesignerScenarioTimelineKey(2f, "healing", DesignerScenarioValueKind.Bool) { boolValue = true }
            };

            Assert.IsFalse(Single(keys, 1.9f).boolValue);
            Assert.IsTrue(Single(keys, 2f).boolValue);
            Assert.AreEqual(DesignerScenarioValueKind.Bool, Single(keys, 1f).kind);
        }

        [Test]
        public void TextKey_StepsToActiveValue()
        {
            var keys = new List<DesignerScenarioTimelineKey>
            {
                new DesignerScenarioTimelineKey(0f, "state", DesignerScenarioValueKind.Text) { textValue = "idle" },
                new DesignerScenarioTimelineKey(1f, "state", DesignerScenarioValueKind.Text) { textValue = "alert" }
            };

            Assert.AreEqual("idle", Single(keys, 0.5f).textValue);
            Assert.AreEqual("alert", Single(keys, 1.5f).textValue);
        }

        [Test]
        public void MultipleKeys_EvaluatedIndependently()
        {
            var keys = new List<DesignerScenarioTimelineKey>
            {
                NumberKey(0f, "hp", 100f),
                NumberKey(2f, "hp", 0f),
                new DesignerScenarioTimelineKey(1f, "flag", DesignerScenarioValueKind.Bool) { boolValue = true }
            };

            var atOnePointFive = ScenarioTimelineEvaluator.EvaluateAt(keys, 1.5f);
            Assert.AreEqual(2, atOnePointFive.Count);
            Assert.AreEqual(25f, atOnePointFive.Single(b => b.key == "hp").numberValue, 0.001f);
            Assert.IsTrue(atOnePointFive.Single(b => b.key == "flag").boolValue);
        }

        [Test]
        public void UnsortedKeyframes_AreSortedByTime()
        {
            var keys = new List<DesignerScenarioTimelineKey>
            {
                NumberKey(2f, "hp", 0f),
                NumberKey(0f, "hp", 100f)
            };
            Assert.AreEqual(50f, Value(keys, 1f), 0.001f);
        }

        [Test]
        public void EmptyOrNull_ReturnsEmpty()
        {
            Assert.IsEmpty(ScenarioTimelineEvaluator.EvaluateAt(null, 1f));
            Assert.IsEmpty(ScenarioTimelineEvaluator.EvaluateAt(new List<DesignerScenarioTimelineKey>(), 1f));
        }

        [Test]
        public void SpriteAndListKeys_StepAndCopyMutableList()
        {
            var sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            var sourceItems = new List<string> { "Sword", "Potion" };
            try
            {
                var keys = new List<DesignerScenarioTimelineKey>
                {
                    new DesignerScenarioTimelineKey(0f, "icon", DesignerScenarioValueKind.Sprite) { spriteValue = sprite },
                    new DesignerScenarioTimelineKey(0f, "items", DesignerScenarioValueKind.List) { listValue = sourceItems }
                };

                var result = ScenarioTimelineEvaluator.EvaluateAt(keys, .5f);
                Assert.AreSame(sprite, result.Single(x => x.key == "icon").spriteValue);
                var evaluatedItems = result.Single(x => x.key == "items").listValue;
                CollectionAssert.AreEqual(sourceItems, evaluatedItems);
                Assert.AreNotSame(sourceItems, evaluatedItems, "evaluation must not expose the asset's mutable list");
            }
            finally
            {
                Object.DestroyImmediate(sprite);
            }
        }

        private static float Value(List<DesignerScenarioTimelineKey> keys, float time) => Single(keys, time).numberValue;

        private static DesignerScenarioBinding Single(List<DesignerScenarioTimelineKey> keys, float time)
            => ScenarioTimelineEvaluator.EvaluateAt(keys, time).Single();
    }
}

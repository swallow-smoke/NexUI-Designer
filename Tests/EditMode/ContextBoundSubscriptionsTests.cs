using System;
using emiteat.NexUI.Designer.Editor;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class ContextBoundSubscriptionsTests
    {
        [Test]
        public void ActivateDetachAndReattach_NeverDuplicatesCallback()
        {
            Action raised = null;
            var calls = 0;
            var subscriptions = new ContextBoundSubscriptions(new VisualElement());
            subscriptions.Add(h => raised += h, h => raised -= h, () => calls++);

            subscriptions.Activate();
            subscriptions.Activate();
            raised?.Invoke();
            Assert.That(calls, Is.EqualTo(1));

            subscriptions.Deactivate();
            raised?.Invoke();
            Assert.That(calls, Is.EqualTo(1));

            subscriptions.Activate();
            raised?.Invoke();
            Assert.That(calls, Is.EqualTo(2));
            subscriptions.Dispose();
            Assert.That(raised, Is.Null);
        }
    }
}

using NUnit.Framework;
using emiteat.NexUI.Designer.Editor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class DesignerSessionRegistryTests
    {
        private NexUIDesignerWindow _firstWindow;
        private NexUIDesignerWindow _secondWindow;
        private NexUIDesignerContext _firstContext;
        private NexUIDesignerContext _secondContext;

        [SetUp]
        public void SetUp()
        {
            _firstWindow = ScriptableObject.CreateInstance<NexUIDesignerWindow>();
            _secondWindow = ScriptableObject.CreateInstance<NexUIDesignerWindow>();
            _firstContext = new NexUIDesignerContext();
            _secondContext = new NexUIDesignerContext();
        }

        [TearDown]
        public void TearDown()
        {
            _firstContext.Dispose();
            _secondContext.Dispose();
            Object.DestroyImmediate(_firstWindow);
            Object.DestroyImmediate(_secondWindow);
        }

        [Test]
        public void RegisterAndActivate_ReturnsFocusedWindowContext()
        {
            var registry = new DesignerSessionRegistry();
            registry.Register(_firstWindow, _firstContext);
            registry.Register(_secondWindow, _secondContext);
            registry.SetActive(_secondWindow);
            Assert.That(registry.ActiveContext, Is.SameAs(_secondContext));
            registry.SetActive(_firstWindow);
            Assert.That(registry.ActiveContext, Is.SameAs(_firstContext));
        }

        [Test]
        public void DuplicateRegister_DoesNotCreateStaleFallback()
        {
            var registry = new DesignerSessionRegistry();
            registry.Register(_firstWindow, _firstContext);
            registry.Register(_firstWindow, _firstContext);
            registry.Unregister(_firstWindow, _firstContext);
            Assert.That(registry.ActiveContext, Is.Null);
        }

        [Test]
        public void UnregisterActive_FallsBackToRemainingWindow()
        {
            var registry = new DesignerSessionRegistry();
            registry.Register(_firstWindow, _firstContext);
            registry.Register(_secondWindow, _secondContext);
            registry.SetActive(_secondWindow);
            registry.Unregister(_secondWindow, _secondContext);
            Assert.That(registry.ActiveContext, Is.SameAs(_firstContext));
        }

        [Test]
        public void DestroyedWindow_IsNeverReturned()
        {
            var registry = new DesignerSessionRegistry();
            registry.Register(_firstWindow, _firstContext);
            Object.DestroyImmediate(_firstWindow);
            _firstWindow = null;
            Assert.That(registry.ActiveContext, Is.Null);
        }
    }
}

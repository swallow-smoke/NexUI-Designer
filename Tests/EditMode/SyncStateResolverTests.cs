using System.Linq;
using emiteat.NexUI.Designer.Editor.Sync;
using NUnit.Framework;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure tests for bidirectional sync (brief §32): three-way state classification and LCS line diff.
    /// </summary>
    public sealed class SyncStateResolverTests
    {
        [Test]
        public void NoFile_IsNew()
        {
            Assert.AreEqual(SyncState.New, SyncStateResolver.Resolve(false, "d", "", ""));
        }

        [Test]
        public void DesignerEqualsFile_IsInSync()
        {
            Assert.AreEqual(SyncState.InSync, SyncStateResolver.Resolve(true, "same", "same", "base"));
        }

        [Test]
        public void OnlyDesignerDivergedFromBase_IsDesignerChanged()
        {
            // file still equals the published baseline, designer moved on
            Assert.AreEqual(SyncState.DesignerChanged, SyncStateResolver.Resolve(true, "d2", "base", "base"));
        }

        [Test]
        public void OnlyFileDivergedFromBase_IsBackendChanged()
        {
            // designer still equals baseline, someone hand-edited the file
            Assert.AreEqual(SyncState.BackendChanged, SyncStateResolver.Resolve(true, "base", "f2", "base"));
        }

        [Test]
        public void BothDivergedFromBase_IsConflict()
        {
            Assert.AreEqual(SyncState.Conflict, SyncStateResolver.Resolve(true, "d2", "f2", "base"));
        }

        [Test]
        public void DifferNoBase_IsConflict()
        {
            Assert.AreEqual(SyncState.Conflict, SyncStateResolver.Resolve(true, "d", "f", ""));
        }

        [Test]
        public void AutoPublishable_OnlyNewAndDesignerChanged()
        {
            Assert.IsTrue(SyncStateResolver.IsAutoPublishable(SyncState.New));
            Assert.IsTrue(SyncStateResolver.IsAutoPublishable(SyncState.DesignerChanged));
            Assert.IsFalse(SyncStateResolver.IsAutoPublishable(SyncState.InSync));
            Assert.IsFalse(SyncStateResolver.IsAutoPublishable(SyncState.BackendChanged));
            Assert.IsFalse(SyncStateResolver.IsAutoPublishable(SyncState.Conflict));
        }

        [Test]
        public void LineDiff_DetectsAddedAndRemoved()
        {
            var diff = TextLineDiff.Diff("a\nb\nc", "a\nc\nd");

            Assert.IsTrue(diff.Any(l => l.Type == DiffLine.Kind.Removed && l.Text == "b"));
            Assert.IsTrue(diff.Any(l => l.Type == DiffLine.Kind.Added && l.Text == "d"));
            Assert.IsTrue(diff.Any(l => l.Type == DiffLine.Kind.Unchanged && l.Text == "a"));
        }

        [Test]
        public void LineDiff_IdenticalText_HasNoChanges()
        {
            Assert.AreEqual(0, TextLineDiff.ChangeCount(TextLineDiff.Diff("x\ny", "x\ny")));
        }

        [Test]
        public void LineDiff_EmptyToContent_AllAdded()
        {
            var diff = TextLineDiff.Diff("", "a\nb");
            Assert.AreEqual(2, diff.Count(l => l.Type == DiffLine.Kind.Added));
        }
    }
}

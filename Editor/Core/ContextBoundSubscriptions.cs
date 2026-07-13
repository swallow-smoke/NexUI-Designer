using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Owns event subscriptions for a VisualElement and mirrors its panel lifetime.
    /// Handlers are retained so lambda-based callbacks can be removed reliably.
    /// </summary>
    public sealed class ContextBoundSubscriptions : IDisposable
    {
        private readonly List<ISubscription> _subscriptions = new List<ISubscription>();
        private readonly VisualElement _owner;
        private bool _attached;
        private bool _disposed;

        public ContextBoundSubscriptions(VisualElement owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _owner.RegisterCallback<AttachToPanelEvent>(OnAttach);
            _owner.RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        public void Add(Action<Action> subscribe, Action<Action> unsubscribe, Action handler)
        {
            AddInternal(new Subscription(subscribe, unsubscribe, handler));
        }

        public void Add<T>(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe, Action<T> handler)
        {
            AddInternal(new Subscription<T>(subscribe, unsubscribe, handler));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            UnsubscribeAll();
            _owner.UnregisterCallback<AttachToPanelEvent>(OnAttach);
            _owner.UnregisterCallback<DetachFromPanelEvent>(OnDetach);
            _subscriptions.Clear();
        }

        /// <summary>Activates all registered callbacks once. Primarily useful to test the lifetime independently of a panel.</summary>
        public void Activate()
        {
            if (_disposed || _attached) return;
            _attached = true;
            for (var i = 0; i < _subscriptions.Count; i++) _subscriptions[i].Subscribe();
        }

        /// <summary>Removes every active callback. Calling it repeatedly is safe.</summary>
        public void Deactivate() => UnsubscribeAll();

        private void AddInternal(ISubscription subscription)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ContextBoundSubscriptions));
            _subscriptions.Add(subscription);
            if (_attached) subscription.Subscribe();
        }

        private void OnAttach(AttachToPanelEvent _)
        {
            Activate();
        }

        private void OnDetach(DetachFromPanelEvent _)
        {
            Deactivate();
        }

        private void UnsubscribeAll()
        {
            if (!_attached) return;
            _attached = false;
            for (var i = _subscriptions.Count - 1; i >= 0; i--) _subscriptions[i].Unsubscribe();
        }

        private interface ISubscription
        {
            void Subscribe();
            void Unsubscribe();
        }

        private sealed class Subscription : ISubscription
        {
            private readonly Action<Action> _subscribe;
            private readonly Action<Action> _unsubscribe;
            private readonly Action _handler;
            private bool _subscribed;

            public Subscription(Action<Action> subscribe, Action<Action> unsubscribe, Action handler)
            {
                _subscribe = subscribe ?? throw new ArgumentNullException(nameof(subscribe));
                _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            public void Subscribe()
            {
                if (_subscribed) return;
                _subscribe(_handler);
                _subscribed = true;
            }

            public void Unsubscribe()
            {
                if (!_subscribed) return;
                _unsubscribe(_handler);
                _subscribed = false;
            }
        }

        private sealed class Subscription<T> : ISubscription
        {
            private readonly Action<Action<T>> _subscribe;
            private readonly Action<Action<T>> _unsubscribe;
            private readonly Action<T> _handler;
            private bool _subscribed;

            public Subscription(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe, Action<T> handler)
            {
                _subscribe = subscribe ?? throw new ArgumentNullException(nameof(subscribe));
                _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            public void Subscribe()
            {
                if (_subscribed) return;
                _subscribe(_handler);
                _subscribed = true;
            }

            public void Unsubscribe()
            {
                if (!_subscribed) return;
                _unsubscribe(_handler);
                _subscribed = false;
            }
        }
    }
}

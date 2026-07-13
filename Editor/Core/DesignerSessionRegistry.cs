using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>Provides the context belonging to the currently focused Designer window.</summary>
    public interface IDesignerSessionProvider
    {
        NexUIDesignerContext ActiveContext { get; }
        event Action<NexUIDesignerContext> ActiveContextChanged;
    }

    /// <summary>
    /// Tracks Designer windows without scanning Unity resources. Instances are independent so
    /// tests and integrations can supply an isolated provider.
    /// </summary>
    public sealed class DesignerSessionRegistry : IDesignerSessionProvider
    {
        private readonly List<Session> _sessions = new List<Session>();
        private NexUIDesignerWindow _activeWindow;

        public NexUIDesignerContext ActiveContext
        {
            get
            {
                PruneClosedWindows();
                var session = Find(_activeWindow);
                if (session != null) return session.Context;
                return _sessions.Count > 0 ? _sessions[_sessions.Count - 1].Context : null;
            }
        }

        public event Action<NexUIDesignerContext> ActiveContextChanged;

        public void Register(NexUIDesignerWindow window, NexUIDesignerContext context)
        {
            if (window == null || context == null) return;
            PruneClosedWindows();
            var existing = Find(window);
            if (existing != null)
            {
                existing.Context = context;
                return;
            }

            _sessions.Add(new Session(window, context));
            if (_activeWindow == null)
            {
                _activeWindow = window;
                ActiveContextChanged?.Invoke(context);
            }
        }

        public void Unregister(NexUIDesignerWindow window, NexUIDesignerContext context)
        {
            if (window == null) return;
            var previous = ActiveContext;
            for (var i = _sessions.Count - 1; i >= 0; i--)
                if (_sessions[i].Window == window && (context == null || _sessions[i].Context == context))
                    _sessions.RemoveAt(i);

            if (_activeWindow == window)
                _activeWindow = _sessions.Count > 0 ? _sessions[_sessions.Count - 1].Window : null;

            var current = ActiveContext;
            if (previous != current) ActiveContextChanged?.Invoke(current);
        }

        public void SetActive(NexUIDesignerWindow window)
        {
            PruneClosedWindows();
            var session = Find(window);
            if (session == null || _activeWindow == window) return;
            _activeWindow = window;
            ActiveContextChanged?.Invoke(session.Context);
        }

        private Session Find(NexUIDesignerWindow window)
        {
            if (window == null) return null;
            for (var i = 0; i < _sessions.Count; i++)
                if (_sessions[i].Window == window) return _sessions[i];
            return null;
        }

        private void PruneClosedWindows()
        {
            for (var i = _sessions.Count - 1; i >= 0; i--)
                if (_sessions[i].Window == null || _sessions[i].Context == null)
                    _sessions.RemoveAt(i);
            if (_activeWindow == null || Find(_activeWindow) == null)
                _activeWindow = _sessions.Count > 0 ? _sessions[_sessions.Count - 1].Window : null;
        }

        private sealed class Session
        {
            public readonly NexUIDesignerWindow Window;
            public NexUIDesignerContext Context;

            public Session(NexUIDesignerWindow window, NexUIDesignerContext context)
            {
                Window = window;
                Context = context;
            }
        }
    }

    /// <summary>Replaceable access point used by satellite windows and tests.</summary>
    public static class DesignerSessions
    {
        private static readonly DesignerSessionRegistry DefaultRegistry = new DesignerSessionRegistry();
        private static IDesignerSessionProvider _provider = DefaultRegistry;

        public static IDesignerSessionProvider Provider
        {
            get => _provider;
            set => _provider = value ?? DefaultRegistry;
        }

        public static NexUIDesignerContext ActiveContext => Provider.ActiveContext;
        internal static DesignerSessionRegistry Registry => DefaultRegistry;
    }
}

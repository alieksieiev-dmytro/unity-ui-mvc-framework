using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI
{
    public sealed class UIService : IUIService, IDisposable
    {
        private readonly UIScreenFactory _factory;
        private readonly EventSystem _eventSystem;
        private readonly Dictionary<Type, UIScreenController> _cache = new Dictionary<Type, UIScreenController>();
        private readonly List<UIWindowController> _windows = new List<UIWindowController>();
        private readonly HashSet<Type> _opening = new HashSet<Type>();
        private bool _isDisposed;

        public UIService(UIScreenFactory factory, UIRootView root)
        {
            _factory = factory;
            _eventSystem = root.EventSystem;
        }

        public bool HasOpenWindows => _windows.Count > 0;
        public event Action WindowStackChanged;

        public TController Open<TController>() where TController : UIScreenController
        {
            ThrowIfDisposed();
            Type type = typeof(TController);
            if (!_opening.Add(type))
            {
                throw new InvalidOperationException($"UI controller {type.Name} is already opening.");
            }

            try
            {
                return OpenScreen<TController>();
            }
            finally
            {
                _opening.Remove(type);
            }
        }

        private TController OpenScreen<TController>() where TController : UIScreenController
        {
            UIScreenScope owner = _factory.GetOwner<TController>();
            bool isNew = !_cache.TryGetValue(typeof(TController), out UIScreenController controller);
            if (isNew)
            {
                controller = _factory.Create<TController>();
            }

            if (controller.IsOpen)
            {
                if (controller is UIWindowController existingWindow && _windows[_windows.Count - 1] != existingWindow)
                {
                    Promote(existingWindow);
                }

                return (TController)controller;
            }

            try
            {
                if (!(controller is UIWindowController) && !(controller is UIHudController))
                {
                    throw new InvalidOperationException("Screens must be window or HUD controllers.");
                }

                controller.Open();
                ThrowIfDisposed();
                if (owner.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(UIScreenScope));
                }
            }
            catch
            {
                if (isNew)
                {
                    DestroyController(controller);
                }

                throw;
            }

            if (isNew)
            {
                _cache.Add(typeof(TController), controller);
                controller.CloseRequested += Close;
            }

            if (controller is UIWindowController window)
            {
                Promote(window);
            }
            else
            {
                controller.View.SetInteraction(false);
            }

            return (TController)controller;
        }

        public void Close<TController>() where TController : UIScreenController
        {
            ThrowIfDisposed();
            if (_cache.TryGetValue(typeof(TController), out UIScreenController controller))
            {
                Close(controller);
            }
        }

        public bool IsOpen<TController>() where TController : UIScreenController
        {
            return !_isDisposed && _cache.TryGetValue(typeof(TController), out UIScreenController controller) && controller.IsOpen;
        }

        internal void HandleCancel()
        {
            if (!_isDisposed && HasOpenWindows)
            {
                _windows[_windows.Count - 1].HandleCancel();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _factory.Clear();
            foreach (UIScreenController controller in _cache.Values)
            {
                controller.CloseRequested -= Close;
                try
                {
                    DestroyController(controller);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            _cache.Clear();
            _windows.Clear();
            if (_eventSystem != null)
            {
                _eventSystem.SetSelectedGameObject(null);
            }

            WindowStackChanged = null;
        }

        internal void RemoveScreens(IReadOnlyList<Type> types)
        {
            if (_isDisposed)
            {
                return;
            }

            RememberSelection();
            var controllers = new List<UIScreenController>();
            bool windowsChanged = false;
            foreach (Type type in types)
            {
                if (!_cache.TryGetValue(type, out UIScreenController controller))
                {
                    continue;
                }

                _cache.Remove(type);
                controller.CloseRequested -= Close;
                controllers.Add(controller);
                if (controller is UIWindowController window)
                {
                    windowsChanged |= _windows.Remove(window);
                    window.SelectedObject = null;
                }
            }

            foreach (UIScreenController controller in controllers)
            {
                try
                {
                    DestroyController(controller);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            if (windowsChanged)
            {
                RefreshWindows();
                WindowStackChanged?.Invoke();
            }
        }

        private void Close(UIScreenController controller)
        {
            if (_isDisposed || !controller.IsOpen)
            {
                return;
            }

            RememberSelection();
            try
            {
                controller.Close();
            }
            finally
            {
                if (controller is UIWindowController window)
                {
                    _windows.Remove(window);
                    window.SelectedObject = null;
                    RefreshWindows();
                    WindowStackChanged?.Invoke();
                }
            }
        }

        private void Promote(UIWindowController window)
        {
            RememberSelection();
            _windows.Remove(window);
            _windows.Add(window);
            RefreshWindows();
            WindowStackChanged?.Invoke();
        }

        private void RememberSelection()
        {
            if (HasOpenWindows && _eventSystem != null)
            {
                UIWindowController top = _windows[_windows.Count - 1];
                top.SelectedObject = _eventSystem.currentSelectedGameObject;
            }
        }

        private void RefreshWindows()
        {
            for (int i = 0; i < _windows.Count; i++)
            {
                _windows[i].View.transform.SetAsLastSibling();
                _windows[i].View.SetInteraction(i == _windows.Count - 1);
            }

            if (_eventSystem == null)
            {
                return;
            }

            _eventSystem.SetSelectedGameObject(null);
            if (HasOpenWindows)
            {
                UIWindowController top = _windows[_windows.Count - 1];
                GameObject selected = IsSelectable(top.SelectedObject, top) ? top.SelectedObject : top.WindowView.DefaultSelection;
                if (IsSelectable(selected, top))
                {
                    _eventSystem.SetSelectedGameObject(selected);
                }
            }
        }

        private static bool IsSelectable(GameObject selected, UIWindowController window)
        {
            return selected != null && selected.transform.IsChildOf(window.View.transform)
                && selected.TryGetComponent(out Selectable selectable) && selectable.IsActive() && selectable.IsInteractable();
        }

        private static void DestroyController(UIScreenController controller)
        {
            try
            {
                controller.Dispose();
            }
            finally
            {
                if (controller.View != null)
                {
                    Object.Destroy(controller.View.gameObject);
                }
            }
        }

        internal void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(UIService));
            }
        }
    }
}

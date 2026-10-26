using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace UI
{
    public sealed class UIScreenScope : IDisposable
    {
        private readonly DiContainer _container;
        private readonly UIScreenFactory _factory;
        private readonly UIService _ui;
        private readonly UIRootView _root;

        public UIScreenScope([Inject(Source = InjectSources.Local)] DiContainer container,
            UIScreenFactory factory, UIService ui, UIRootView root)
        {
            _container = container;
            _factory = factory;
            _ui = ui;
            _root = root;
        }

        internal bool IsDisposed { get; private set; }

        public void RegisterHud<TModel, TView, TController>(TView prefab)
            where TModel : class, IUIScreenModel
            where TView : UIHudView
            where TController : UIHudController
        {
            Register<TModel, TView, TController>(prefab, _root.Huds);
        }

        public void RegisterWindow<TModel, TView, TController>(TView prefab)
            where TModel : class, IUIScreenModel
            where TView : UIWindowView
            where TController : UIWindowController
        {
            Register<TModel, TView, TController>(prefab, _root.Windows);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            _ui.RemoveScreens(_factory.Unregister(this));
        }

        private void Register<TModel, TView, TController>(TView prefab, Transform parent)
            where TModel : class, IUIScreenModel
            where TView : UIScreenView
            where TController : UIScreenController
        {
            ThrowIfDisposed();
            _ui.ThrowIfDisposed();
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (prefab.gameObject.activeSelf)
            {
                throw new InvalidOperationException($"Screen prefab {prefab.name} must start inactive.");
            }

            _factory.Register(this, () => Create<TModel, TView, TController>(prefab, parent));
        }

        private TController Create<TModel, TView, TController>(TView prefab, Transform parent)
            where TModel : class, IUIScreenModel
            where TView : UIScreenView
            where TController : UIScreenController
        {
            ThrowIfDisposed();
            TView view = null;
            try
            {
                view = Object.Instantiate(prefab, parent, false);
                _container.InjectGameObject(view.gameObject);
                view.Validate();
                view.SetInteraction(false);
                TModel model = _container.Instantiate<TModel>();
                return _container.Instantiate<TController>(new object[] { model, view });
            }
            catch
            {
                if (view != null)
                {
                    Object.Destroy(view.gameObject);
                }

                throw;
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(UIScreenScope));
            }
        }
    }
}

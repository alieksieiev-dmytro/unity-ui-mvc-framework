using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UI
{
    public sealed class UIFrameworkInstaller : MonoInstaller
    {
        [SerializeField] private UIRootView _rootPrefab;
        [SerializeField] private InputActionAsset _inputActions;
        [Inject] private Context _context;

        public override void InstallBindings()
        {
            if (!(_context is ProjectContext))
            {
                throw new InvalidOperationException("Install the UI framework once in ProjectContext.");
            }

            if (Container.HasBindingId(typeof(UIService), null, InjectSources.Local))
            {
                throw new InvalidOperationException("The UI framework is already installed in this ProjectContext.");
            }

            if (_rootPrefab == null || _inputActions == null)
            {
                throw new InvalidOperationException("The UI framework requires a root prefab and UI input actions.");
            }

            _rootPrefab.Validate();
            if (!_rootPrefab.gameObject.activeSelf || _rootPrefab.InputModule.enabled)
            {
                throw new InvalidOperationException("The UI root prefab must be active with its input module disabled.");
            }

            UIRootView root = Instantiate(_rootPrefab, _context.transform, false);
            try
            {
                Container.InjectGameObject(root.gameObject);
                root.Validate();
            }
            catch
            {
                Destroy(root.gameObject);
                throw;
            }

            Container.BindInstance(root);
            Container.Bind<InputActionAsset>().WithId("UI").FromInstance(_inputActions);
            Container.Bind<UIScreenFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIScreenScope>().AsSingle();
            Container.BindInitializableExecutionOrder<UIInputService>(-50);
            Container.BindTickableExecutionOrder<UIInputService>(-70);
            Container.BindDisposableExecutionOrder<UIScreenScope>(100);
            Container.BindDisposableExecutionOrder<UIInputService>(50);
            Container.BindDisposableExecutionOrder<UIService>(-100);
        }
    }
}

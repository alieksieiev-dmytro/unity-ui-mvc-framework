using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace UI
{
    public sealed class UIInputService : IInitializable, ITickable, IDisposable
    {
        private readonly UIService _ui;
        private readonly InputActionAsset _actions;
        private readonly InputSystemUIInputModule _module;
        private readonly InputAction _cancel;
        private readonly List<InputActionReference> _references = new List<InputActionReference>();
        private bool _isInitialized;
        private bool _isDisposed;

        public UIInputService(UIService ui, [Inject(Id = "UI")] InputActionAsset actions, UIRootView root)
        {
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
            _module = root != null ? root.InputModule : throw new ArgumentNullException(nameof(root));
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            InputActionMap map = actions.FindActionMap("UI", true);
            string[] requiredActions =
            {
                "Point", "Click", "RightClick", "MiddleClick", "ScrollWheel", "Navigate", "Submit",
                "TrackedDevicePosition", "TrackedDeviceOrientation", "Cancel"
            };
            foreach (string actionName in requiredActions)
            {
                map.FindAction(actionName, true);
            }

            _actions = ScriptableObject.CreateInstance<InputActionAsset>();
            _actions.name = "UI Runtime Actions";
            _actions.AddActionMap(map.Clone());
            _cancel = _actions.FindAction("UI/Cancel", true);
        }

        public void Initialize()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(UIInputService));
            }

            if (_isInitialized)
            {
                return;
            }

            try
            {
                ConfigureModule();
                _isInitialized = true;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void ConfigureModule()
        {
            _module.enabled = false;
            _module.actionsAsset = _actions;
            _module.point = CreateReference("Point");
            _module.leftClick = CreateReference("Click");
            _module.rightClick = CreateReference("RightClick");
            _module.middleClick = CreateReference("MiddleClick");
            _module.scrollWheel = CreateReference("ScrollWheel");
            _module.move = CreateReference("Navigate");
            _module.submit = CreateReference("Submit");
            _module.trackedDevicePosition = CreateReference("TrackedDevicePosition");
            _module.trackedDeviceOrientation = CreateReference("TrackedDeviceOrientation");
            _module.cancel = null;
            _module.deselectOnBackgroundClick = false;
            _module.enabled = true;
            _cancel.Enable();
        }

        public void Tick()
        {
            if (_isInitialized && !_isDisposed && Application.isFocused && _cancel.WasPressedThisFrame())
            {
                _ui.HandleCancel();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _actions.Disable();
            if (_module != null)
            {
                _module.enabled = false;
                _module.point = null;
                _module.leftClick = null;
                _module.rightClick = null;
                _module.middleClick = null;
                _module.scrollWheel = null;
                _module.move = null;
                _module.submit = null;
                _module.trackedDevicePosition = null;
                _module.trackedDeviceOrientation = null;
                _module.cancel = null;
                _module.actionsAsset = null;
            }

            foreach (InputActionReference reference in _references)
            {
                Object.Destroy(reference);
            }

            _references.Clear();
            Object.Destroy(_actions);
        }

        private InputActionReference CreateReference(string actionName)
        {
            InputActionReference reference = InputActionReference.Create(_actions.FindAction($"UI/{actionName}", true));
            _references.Add(reference);
            return reference;
        }

    }
}

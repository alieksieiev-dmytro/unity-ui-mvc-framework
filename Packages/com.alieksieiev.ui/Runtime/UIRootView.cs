using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace UI
{
    public sealed class UIRootView : MonoBehaviour
    {
        [SerializeField] private RectTransform _huds;
        [SerializeField] private RectTransform _windows;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private InputSystemUIInputModule _inputModule;

        public RectTransform Huds => _huds;
        public RectTransform Windows => _windows;
        public EventSystem EventSystem => _eventSystem;
        public InputSystemUIInputModule InputModule => _inputModule;

        public void Validate()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null || !canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay
                || _huds == null || _windows == null || _huds == _windows
                || _huds.parent != transform || _windows.parent != transform
                || _huds.GetSiblingIndex() >= _windows.GetSiblingIndex()
                || _eventSystem == null || _inputModule == null
                || !_eventSystem.transform.IsChildOf(transform)
                || _eventSystem.gameObject != _inputModule.gameObject)
            {
                throw new InvalidOperationException("UI root requires an overlay Canvas, HUDs below Windows, and an EventSystem with its input module.");
            }
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIWindowView : UIScreenView
    {
        [SerializeField] private Image _background;
        [SerializeField] private Selectable _defaultSelectable;

        internal GameObject DefaultSelection => _defaultSelectable == null ? null : _defaultSelectable.gameObject;

        public override void Validate()
        {
            base.Validate();
            if (_background == null || !_background.transform.IsChildOf(transform)
                || GetComponent<GraphicRaycaster>() == null)
            {
                throw new InvalidOperationException($"Window {name} requires a background and GraphicRaycaster.");
            }

            RectTransform background = _background.rectTransform;
            if (background.parent != transform || background.anchorMin != Vector2.zero
                || background.anchorMax != Vector2.one || background.offsetMin != Vector2.zero
                || background.offsetMax != Vector2.zero)
            {
                throw new InvalidOperationException($"Window {name} requires a full-screen background.");
            }

            if (_defaultSelectable != null && !_defaultSelectable.transform.IsChildOf(transform))
            {
                throw new InvalidOperationException($"Window {name} has a default selection outside its hierarchy.");
            }

            _background.raycastTarget = true;
            _background.transform.SetAsFirstSibling();
        }
    }
}

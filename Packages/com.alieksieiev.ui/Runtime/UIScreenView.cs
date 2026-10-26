using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class UIScreenView : MonoBehaviour
    {
        public virtual void Validate()
        {
            Canvas canvas = GetComponent<Canvas>();
            CanvasGroup group = GetComponent<CanvasGroup>();
            if (canvas == null || group == null || canvas.overrideSorting || group.ignoreParentGroups)
            {
                throw new InvalidOperationException($"Screen {name} requires an inherited Canvas and CanvasGroup.");
            }
        }

        internal void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        internal void SetInteraction(bool enabled)
        {
            CanvasGroup group = GetComponent<CanvasGroup>();
            group.interactable = enabled;
            group.blocksRaycasts = enabled;
        }
    }
}

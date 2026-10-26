using UnityEngine.UI;

namespace UI
{
    public abstract class UIHudView : UIScreenView
    {
        public override void Validate()
        {
            base.Validate();
            SetInteraction(false);
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }

            foreach (GraphicRaycaster raycaster in GetComponentsInChildren<GraphicRaycaster>(true))
            {
                raycaster.enabled = false;
            }
        }
    }
}

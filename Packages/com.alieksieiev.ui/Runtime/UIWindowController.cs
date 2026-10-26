using UnityEngine;

namespace UI
{
    public abstract class UIWindowController : UIScreenController
    {
        protected UIWindowController(IUIScreenModel model, UIWindowView view) : base(model, view)
        {
        }

        internal GameObject SelectedObject { get; set; }
        internal UIWindowView WindowView => (UIWindowView)View;

        public virtual void HandleCancel()
        {
        }
    }
}

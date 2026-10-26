namespace UI
{
    public abstract class UIHudController : UIScreenController
    {
        protected UIHudController(IUIScreenModel model, UIHudView view) : base(model, view)
        {
        }
    }
}

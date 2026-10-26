using System;

namespace UI
{
    public interface IUIService
    {
        bool HasOpenWindows { get; }
        event Action WindowStackChanged;
        TController Open<TController>() where TController : UIScreenController;
        void Close<TController>() where TController : UIScreenController;
        bool IsOpen<TController>() where TController : UIScreenController;
    }
}

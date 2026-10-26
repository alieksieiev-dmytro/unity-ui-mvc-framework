using System;

namespace UI
{
    public abstract class UIScreenController : IDisposable
    {
        private readonly IUIScreenModel _model;
        private bool _isDisposed;

        protected UIScreenController(IUIScreenModel model, UIScreenView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            View = view != null ? view : throw new ArgumentNullException(nameof(view));
        }

        public bool IsOpen { get; private set; }
        internal UIScreenView View { get; }
        internal event Action<UIScreenController> CloseRequested;

        internal void Open()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (IsOpen)
            {
                return;
            }

            _model.Reset();
            try
            {
                OnOpen();
                View.SetVisible(true);
                IsOpen = true;
            }
            catch
            {
                CloseView();
                throw;
            }
        }

        internal void Close()
        {
            if (IsOpen)
            {
                CloseView();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            try
            {
                Close();
            }
            finally
            {
                CloseRequested = null;
                OnDispose();
            }
        }

        protected void RequestClose()
        {
            if (IsOpen)
            {
                CloseRequested?.Invoke(this);
            }
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnDispose()
        {
        }

        private void CloseView()
        {
            IsOpen = false;
            try
            {
                OnClose();
            }
            finally
            {
                if (View != null)
                {
                    View.SetInteraction(false);
                    View.SetVisible(false);
                }
            }
        }
    }
}

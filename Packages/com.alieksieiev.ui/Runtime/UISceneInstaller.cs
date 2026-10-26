using System;
using Zenject;

namespace UI
{
    public sealed class UISceneInstaller : Installer<UISceneInstaller>
    {
        public override void InstallBindings()
        {
            if (Container.HasBindingId(typeof(UIScreenScope), null, InjectSources.Local))
            {
                throw new InvalidOperationException("A UI screen scope is already installed in this context.");
            }

            Container.BindInterfacesAndSelfTo<UIScreenScope>().AsSingle();
        }
    }
}

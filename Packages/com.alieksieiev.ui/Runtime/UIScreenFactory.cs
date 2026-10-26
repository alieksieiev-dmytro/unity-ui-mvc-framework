using System;
using System.Collections.Generic;

namespace UI
{
    public sealed class UIScreenFactory
    {
        private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();

        internal void Register<TController>(UIScreenScope owner, Func<TController> create) where TController : UIScreenController
        {
            if (create == null)
            {
                throw new ArgumentNullException(nameof(create));
            }

            Type type = typeof(TController);
            if (_registrations.ContainsKey(type))
            {
                throw new InvalidOperationException($"UI controller {type.Name} is already registered.");
            }

            _registrations.Add(type, new Registration(owner, () => create()));
        }

        internal UIScreenScope GetOwner<TController>() where TController : UIScreenController
        {
            return GetRegistration<TController>().Owner;
        }

        internal UIScreenController Create<TController>() where TController : UIScreenController
        {
            UIScreenController controller = GetRegistration<TController>().Create();
            if (controller == null)
            {
                throw new InvalidOperationException($"UI factory for {typeof(TController).Name} returned no controller.");
            }

            return controller;
        }

        internal List<Type> Unregister(UIScreenScope owner)
        {
            var types = new List<Type>();
            foreach (KeyValuePair<Type, Registration> pair in _registrations)
            {
                if (pair.Value.Owner == owner)
                {
                    types.Add(pair.Key);
                }
            }

            foreach (Type type in types)
            {
                _registrations.Remove(type);
            }

            return types;
        }

        internal void Clear()
        {
            _registrations.Clear();
        }

        private Registration GetRegistration<TController>() where TController : UIScreenController
        {
            if (!_registrations.TryGetValue(typeof(TController), out Registration registration))
            {
                throw new InvalidOperationException($"UI controller {typeof(TController).Name} is not registered.");
            }

            return registration;
        }

        private sealed class Registration
        {
            public Registration(UIScreenScope owner, Func<UIScreenController> create)
            {
                Owner = owner;
                Create = create;
            }

            public UIScreenScope Owner { get; }
            public Func<UIScreenController> Create { get; }
        }
    }
}

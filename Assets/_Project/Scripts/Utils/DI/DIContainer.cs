using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    public class DIContainer : IDisposable
    {
        private readonly DIContainer _parentContainer;

        private readonly Dictionary<(string, Type), DIEntry> _registrations = new();

        private readonly HashSet<(string, Type)> _resolutions = new();

        public DIContainer() : this(null) { }

        public DIContainer(DIContainer parentContainer)
        {
            _parentContainer = parentContainer;
        }

        public DIEntry RegisterFactory<T>(Func<DIContainer, T> factory)
        {
            return RegisterFactory<T>(null, factory);
        }

        public DIEntry RegisterFactory<T>(string tag, Func<DIContainer, T> factory)
        {
            if (factory == null)
            {
                throw new Exception($"DI: Factory cannot be null");
            }

            var key = (tag, typeof(T));

            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            DIEntry newEntry = new DIEntry<T>(this, factory);

            _registrations.Add(key, newEntry);

            return newEntry;
        }

        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance<T>(null, instance);
        }

        public void RegisterInstance<T>(string tag, T instance)
        {
            (string, Type) key = (tag, typeof(T));

            if (_registrations.ContainsKey(key))
            {
                throw new Exception($"DI: Factory with tag {key.Item1} and type {key.Item2.FullName} has already registered");
            }

            _registrations.Add(key, new DIEntry<T>(instance));
        }

        public T Resolve<T>()
        {
            return Resolve<T>(null);    
        } 

        public T Resolve<T>(string tag)
        {
            var key = (tag, typeof(T)); 

            if (_resolutions.Contains(key))
            {
                throw new Exception($"Cyclic dependency for tag {tag} and type {key.Item2.FullName}");
            }

            _resolutions.Add(key);

            try
            {
                if (_registrations.TryGetValue(key, out var registration))
                {
                    return registration.Resolve<T>();
                }
                else if (_parentContainer != null)
                {
                    return _parentContainer.Resolve<T>(tag);
                }
            }
            finally
            {
                _resolutions.Remove(key);
            }

            throw new Exception($"Could not find dependency for tag {tag} and type {key.Item2.FullName}");
        }

        public void Dispose()
        {
            _resolutions.Clear();

            if (_registrations == null || _registrations.Count == 0) return;

            foreach (var pair in _registrations)
            {
                (pair.Value as IDisposable)?.Dispose();
            }

            _registrations.Clear();
        }
    }

}

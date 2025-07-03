using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    public abstract class DIEntry : IDisposable
    {
        protected DIContainer Containter { get; }

        protected bool IsSingleton { get; set; }

        protected DIEntry(DIContainer containter)
        {
            Containter = containter;

            IsSingleton = false;
        }

        public void AsSingle()
        {
            IsSingleton = true;
        }

        public T Resolve<T>()
        {
            return ((DIEntry<T>)this).Resolve();
        }

        public abstract void Dispose();
    }

    public class DIEntry<T> : DIEntry
    {
        private T _instance;

        private Func<DIContainer, T> _factory;

        public DIEntry(DIContainer containter, Func<DIContainer, T> factory) : base(containter)
        {
            _factory = factory;
        }

        public DIEntry(T instance) : base(null)
        {
            _instance = instance;

            IsSingleton = true;
        }

        public T Resolve()
        {
            if (IsSingleton)
            {
                if (_instance == null)
                {
                    _instance = _factory(Containter);
                }

                return _instance;
            }
            else
            {
                return _factory(Containter);
            }
        }

        public override void Dispose()
        {
            _factory = null;

            if (_instance == null) return;

            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _instance = default(T);
        }
    }
}

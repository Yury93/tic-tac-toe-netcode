using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.Infrastructure.StateMachine
{
    public interface IStateFactory
    {
        T Create<T>() where T : class, IExitableState;
    }

    public class StateFactory : IStateFactory
    {
        private readonly DiContainer _container;

        public StateFactory(DiContainer container)
        {
            _container = container;
        }

        public T Create<T>() where T : class, IExitableState
        {
            return _container.Instantiate<T>();
        }
    }
}
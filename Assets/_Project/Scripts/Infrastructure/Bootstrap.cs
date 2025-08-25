using Assets._Project.Scripts.Infrastructure.StateMachine;
using Assets._Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.Infrastructure
{
    public class Bootstrap : MonoBehaviour
    {
        private IStateMachine _stateMachine;
        [Inject]
        public void Construct(StateMachine.IStateMachine  stateMachine)
        {
           _stateMachine = stateMachine;
        }
        private void Start()
        {
            _stateMachine.Enter<CreateMenuState>();
        }
    }
}
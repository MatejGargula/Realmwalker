using System;
using UnityEngine;

namespace BT
{
    public class LimiterNode : DecoratorNode
    {
        public int numberOfRepeats = 3;
        [SerializeField] private int _repeats;
        private State _lastState = State.Running;

        protected override State OnUpdate()
        {
            if (_repeats >= numberOfRepeats)
            {
                _repeats = 0;

                child.OnStop();

                return State.Failure;
            }

            var childState = child.Update();

            switch (childState)
            {
                case State.Running:
                    break;
                case State.Failure:
                    _lastState = childState;
                    break;
                case State.Success:
                    if (_lastState == State.Success)
                        _repeats++;
                    _lastState = childState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return childState;
        }

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            //_repeats = 0;

            nodeTitle = "Limiter";
            description = "limits how many times a child node can return Success in a row";
        }
    }
}
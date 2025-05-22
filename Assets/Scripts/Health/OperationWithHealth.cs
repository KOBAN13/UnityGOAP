using System;
using R3;
using UnityEngine;

namespace Health
{
    public class OperationWithHealth<T> where T : MonoBehaviour
    {
        private readonly Subject<Unit> _hit;
        private readonly Subject<Unit> _die;
        private readonly CompositeDisposable _compositeDisposable = new();

        public OperationWithHealth(Subject<Unit> hit,  Subject<Unit> die) 
        {
            _die = die;
            _hit = hit;
        }

        public void SubscribeHit(Action hit)
        { 
            _hit.Subscribe(_ => hit()).AddTo(_compositeDisposable);
        }

        public void SubscribeDead(Action dead)
        {
            _die.Subscribe(_ => dead()).AddTo(_compositeDisposable);
        }
    }
}
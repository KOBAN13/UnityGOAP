using System;
using R3;

namespace Health
{
    public class OperationWithHealth : IDisposable
    {
        private readonly Subject<Unit> _hit;
        private readonly Subject<Unit> _die;
        private readonly CompositeDisposable _compositeDisposable = new();

        public OperationWithHealth(Subject<Unit> hit, Subject<Unit> die) 
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
        
        public void InvokeHit() => _hit.OnNext(Unit.Default);
        
        public void InvokeDead() => _die.OnNext(Unit.Default);
        
        public void Dispose()
        {
            _compositeDisposable.Clear();
            _compositeDisposable.Dispose();
        }
    }
}
using System;
using R3;
using UnityEngine;

namespace Game.Core.Health
{
    public class OperationWithHealth<T> where T : MonoBehaviour
    {
        private readonly Subject<Unit> _hit;
        private readonly Subject<Unit> _die;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly RestoringHealth<T> _restoringHealth;

        public OperationWithHealth(Subject<Unit> hit,  Subject<Unit> die, RestoringHealth<T> restoringHealth) 
        {
            _die = die;
            _hit = hit;
            _restoringHealth = restoringHealth;
        }

        public void SubscribeHit(Action hit)
        { 
            _hit.Subscribe(_ => hit()).AddTo(_compositeDisposable);
        }

        public void SubscribeDead(Action dead)
        {
            _die.Subscribe(_ => dead()).AddTo(_compositeDisposable);
        }
    
        public void EnemyDie()
        {
            _restoringHealth.IsHealthRestoringAfterDieEnemy = true;
            _restoringHealth.AddHealth().Forget();
            _restoringHealth.IsHealthRestoringAfterDieEnemy = false;
        }

        public void EnemyHitBullet()
        {
            switch (_restoringHealth.IsHealthRestoringAfterHitEnemy)
            {
                case true:
                    _restoringHealth.CancellationTokenSource?.Cancel();
                    break;
                case false:
                    _restoringHealth.AddHealth().Forget();
                    break;
            }
        }
    }
}
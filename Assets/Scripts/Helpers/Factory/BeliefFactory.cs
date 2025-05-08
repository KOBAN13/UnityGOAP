using System;
using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace CharacterOrEnemyEffect.Factory
{
    public class BeliefFactory
    {
        private readonly Dictionary<string, AgentBelief> _beliefs;

        public BeliefFactory(Dictionary<string, AgentBelief> beliefs)
        {
            _beliefs = beliefs;
        }
        
        public void AddBelief(string nativeKey, Func<bool> condition)
        {
            var belief = new BeliefBuilder(nativeKey);
            _beliefs.Add(nativeKey, 
                belief
                .WithCondition(condition)
                .BuildBelief());
        }
        
        public void AddLocationBelief(string nativeKey, Vector3 locationCondition, Func<bool> condition)
        {
            var belief = new BeliefBuilder(nativeKey);
            _beliefs.Add(nativeKey, 
                belief
                .WithCondition(condition)
                .WithLocation(locationCondition)
                .BuildBelief());
        }

        public void AddSensorBelief(string nativeKey, ISensor sensor)
        {
            var belief = new BeliefBuilder(nativeKey);
            
            _beliefs.Add(nativeKey, 
                belief
                .WithLocation(sensor.Target)
                .WithSensor(() => sensor.IsActivate)
                .BuildBelief());
        }

        //bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(_agent, pos) < range; ///СОМНИТЕЛЬНО, ПОЗЖЕ ПЕРЕДЕЛАТЬ
    }
}
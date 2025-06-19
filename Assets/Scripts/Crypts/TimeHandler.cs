using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Crypts
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] float _maxSecondsInCrypt = 15*60;
        [SerializeField] List<TimedEvent> _events;
        [SerializeField] Clockhand[] _clockHands;

        private void Awake()
        {
            if (_events.Count > 0)
                _maxSecondsInCrypt = 0;
         
            foreach(var e in _events)
                _maxSecondsInCrypt = Mathf.Max( _maxSecondsInCrypt, e.TimeOfEventSeconds);
        }

        private void Update()
        {
            for(int i = 0; i < _events.Count; i++)
            {
                if (_events[i].TimeOfEventSeconds > Time.timeSinceLevelLoad) 
                    continue;

                _events[i].Event?.Invoke();
                _events.RemoveAt(i);
                i--;
            }

            foreach(var h in _clockHands)
            {
                if (h.ClockHandTransform == null) 
                    return;
                
                float t = Time.timeSinceLevelLoad / h.PeriodSeconds;
                t *= -360;
                if (float.IsNaN(t)) 
                    return;
                
                h.ClockHandTransform.rotation = Quaternion.AngleAxis(t, Vector3.forward);
            }
        }

        [Serializable]
        struct Clockhand
        {
            public float PeriodSeconds;
            public Transform ClockHandTransform;
        }

        [Serializable]
        struct TimedEvent
        {
            public float TimeOfEventSeconds;
            public UnityEvent Event;
        }
    }
}

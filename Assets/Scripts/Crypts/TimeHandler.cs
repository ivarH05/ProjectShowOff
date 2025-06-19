using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Crypts
{
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField] float _maxSecondsInCrypt = 15*60;
        [SerializeField] List<TimedEvent> _timedEvents;
        [SerializeField] Clockhand[] _clockHands;

        private void Awake()
        {
            if (_timedEvents.Count > 0)
                _maxSecondsInCrypt = 0;
         
            foreach(var e in _timedEvents)
                _maxSecondsInCrypt = Mathf.Max( _maxSecondsInCrypt, e.TimeOfEventSeconds);
        }

        private void Update()
        {
            for(int i = 0; i < _timedEvents.Count; i++)
            {
                if (_timedEvents[i].TimeOfEventSeconds > Time.timeSinceLevelLoad) 
                    continue;

                _timedEvents[i].Event?.Invoke();
                _timedEvents.RemoveAt(i);
                i--;
            }

            if (Time.timeSinceLevelLoad > _maxSecondsInCrypt)
            {
                UIManager.SetState<DeathUIState>();
                Destroy(this);
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

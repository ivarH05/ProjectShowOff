using System;
using UnityEngine;


// whos bob?
namespace Player
{
    /// <summary>
    /// Bobs the head.
    /// </summary>
    public class HeadBob : MonoBehaviour
    {
        [SerializeField] PlayerController _controller;
        [SerializeField] float _frequency = 1;
        [SerializeField] float _magnitude = .2f;
        [SerializeField] float _speedThreshold = .3f;
        [SerializeField] float _windupSpeed = 1;
        [SerializeField, Range(0,1)] float _idleBobStrength = .3f;
        [SerializeField] float _idleBobFrequency = 1;

        Vector3 _previousParentPosition;
        float _bobStrength;
        double _bobProgress;

        private void Update()
        {
            var parentPos = transform.parent.position;
            var diff = parentPos - _previousParentPosition;
            _previousParentPosition = parentPos;

            float vel = diff.magnitude;
            if(vel < _speedThreshold * Time.deltaTime) // not moving fast at all
                _bobStrength = Mathf.Clamp01(_bobStrength - Time.deltaTime*_windupSpeed);
            else // moving fast enough for bobs
                _bobStrength = Mathf.Clamp01(_bobStrength + Time.deltaTime*_windupSpeed);

            if(_controller != null && _controller.UncoyotedGrounded)
            {
                _bobProgress += (_frequency * Mathf.Sqrt(vel/Time.deltaTime) + _idleBobFrequency)*Time.deltaTime;
                float strength = _magnitude * ((1 - _idleBobStrength) * _bobStrength + _idleBobStrength);

                transform.localPosition = new Vector3((float)Math.Sin(_bobProgress / 2) * strength / 35, (float)Math.Sin(_bobProgress) * strength / 50, 0);

                transform.localEulerAngles = new Vector3(
                    (float)Math.Sin(_bobProgress) / 4,
                    (float)Math.Sin(_bobProgress / 2),
                    0) * strength;
            }
        }
    }
}

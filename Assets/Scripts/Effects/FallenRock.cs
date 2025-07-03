using System.Collections;
using UnityEngine;

namespace Effects
{
    public class FallenRock : MonoBehaviour
    {
        [SerializeField] float _threshold = .01f;
        [SerializeField] float _sinkSpeed = .2f;
        [SerializeField] float _sinkTime = 5f;

        Vector3 _lastPos;
        Quaternion _lastRot;
        int _framesThresholdNotMet;

        bool _sinking = false;

        void FixedUpdate()
        {
            if (_sinking)
                return;

            float dist = (_lastPos - transform.position).magnitude;
            dist += Mathf.Abs(1 - Quaternion.Dot(_lastRot, transform.rotation));

            if (dist < _threshold * Time.fixedDeltaTime)
                _framesThresholdNotMet++;
            else _framesThresholdNotMet = 0;

            if(_framesThresholdNotMet > 5)
            {
                _sinking = true;
                StartCoroutine(Sink());
            }

            _lastPos = transform.position;
            _lastRot = transform.rotation;

            if (transform.position.y < -100)
                Destroy(gameObject);
        }

        IEnumerator Sink()
        {
            Destroy(GetComponent<Rigidbody>());
            float t = 0;
            while(true)
            {
                t += Time.deltaTime;
                transform.position -= Vector3.up * Time.deltaTime * _sinkSpeed;
                if (t > _sinkTime)
                    break;

                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

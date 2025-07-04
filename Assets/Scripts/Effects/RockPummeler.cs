using UnityEngine;

namespace Effects
{
    public class RockPummeler : MonoBehaviour
    {
        [SerializeField] GameObject[] _rockPrefabs;
        [SerializeField] float _maxRange = 10;
        public float RockFrequency = 1f;

        float _cooldown;

        private void Update()
        {
            _cooldown += Time.deltaTime * RockFrequency;

            int instances = 0;
            while(_cooldown > 1 && instances < 10)
            {
                instances++;
                _cooldown -= 1;

                var pos = Random.insideUnitCircle * _maxRange;
                if(Physics.Raycast(new Vector3(pos.x, 0, pos.y) + transform.position, Vector3.up, out RaycastHit hitinfo, 10))
                {
                    var instance = Instantiate(_rockPrefabs[Random.Range(0, _rockPrefabs.Length)], hitinfo.point, Random.rotation);
                    instance.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere + Vector3.down, ForceMode.VelocityChange);
                }
            }
        }
    }
}

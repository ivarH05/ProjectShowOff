using AdvancedSound;
using Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NPC
{
    public class Enemy : Character
    {
        private List<Clue> clues = new List<Clue>();
        [SerializeField] private AudioBehaviourStrategy _audioBehaviourStrategy;
        [SerializeField] private VisionBehaviourStrategy _visionBehaviourStrategy;
        [SerializeField] private Animator _animator;
        public bool defaultBehaviour = true;

        internal override void Start()
        {
            base.Start();
            events.OnHearSound.AddListener(OnHearSound);
            events.OnNoticePlayer.AddListener(OnNoticePlayer);
        }

        internal override void Update()
        {
            base.Update();
            _animator.SetFloat("Speed", agent.velocity.magnitude);
            CleanupClues();
        }

        public void SetSeesPlayer(bool value)
        {
            _animator.SetBool("SeesPlayer", value);
        }

        internal override void OnDestroy()
        {
            base.OnDestroy();
            events.OnHearSound.RemoveListener(OnHearSound);
            events.OnNoticePlayer.RemoveListener(OnNoticePlayer);
        }

        void OnHearSound(Character character, HeardSound sound)
        {
            if (!defaultBehaviour)
                return;

            _audioBehaviourStrategy?.OnHearSound(this, sound);
        }

        void OnNoticePlayer(Character character, PlayerController player)
        {
            if (!defaultBehaviour)
                return;

            _visionBehaviourStrategy?.OnNoticePlayer(this, player);
        }


        public void SetVisionStrategy<T>() where T : VisionBehaviourStrategy, new()
        {
            if (_visionBehaviourStrategy is T)
                return;

            T newState = GetComponent<T>();
            _visionBehaviourStrategy = newState == null ? transform.AddComponent<T>() : newState;
        }

        public void SetAudioStrategy<T>() where T : AudioBehaviourStrategy, new()
        {
            if (_audioBehaviourStrategy is T)
                return;

            T newState = GetComponent<T>();
            _audioBehaviourStrategy = newState == null ? transform.AddComponent<T>() : newState;
        }


        internal virtual void CleanupClues()
        {
            List<Clue> newClues = new List<Clue>();
            for (int i = 0; i < clues.Count; i++)
            {
                Clue clue = clues[i];
                if (Time.time - clue.time < clue.lingerTime)
                    newClues.Add(clue);
            }
            clues = newClues;
        }

        public void AddClue(Clue clue) 
        {
            if (clues.Count > 1)
                clues[clues.Count - 1].color = Color.orange;
            clue.color = Color.green;
            clues.Add(clue); 
        }
        public Clue GetClue(int i) => clues[i];
        public int ClueCount => clues.Count;


        internal override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            base.OnDrawGizmos();

            if (!Application.isPlaying) return;

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.orange;
            for (int i = 0; i < clues.Count; i++)
                Gizmos.DrawWireSphere(clues[i].position, 0.15f);
#endif
        }

        internal override void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            base.OnDrawGizmosSelected();

            if (!Application.isPlaying) return;

            Gizmos.color = Color.orange;
            for (int i = 0; i < clues.Count; i++)
                DrawClue(clues[i]);
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawSphere(agent.destination, 0.2f);
#endif  
        }

        void DrawClue(Clue clue)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = clue.color;
            Gizmos.DrawSphere(clue.position, 0.25f);
            if (clue.direction != Vector3.zero)
                DrawDirectionCone(clue);

            if (clue.errorMargin > 0.25f)
                DrawClueErrorMargin(clue);
        }

        void DrawClueErrorMargin(Clue clue)
        {
            float timeElapsed = Time.time - clue.time;
            float squaredLerpFactor = Mathf.Sqrt(timeElapsed);

            Color color = clue.color;
            if(clue.color == Color.orange)
                color.a = Mathf.Clamp(1 - squaredLerpFactor, 0.01f, 1);
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireSphere(clue.position, clue.errorMargin);
        }

        void DrawDirectionCone(Clue clue)
        {
            const float radius = 0.8f;

            List<Vector3> points = new List<Vector3>() { Vector3.zero };
            for (float a = -45; a <= 45; a += 10)
            {
                float rads = a * Mathf.Deg2Rad;
                float x = Mathf.Sin(rads) * radius;
                float z = Mathf.Cos(rads) * radius;

                points.Add(new Vector3(x, 0f, z));
            }

            Quaternion rotation = Quaternion.LookRotation(clue.direction.normalized);
            Gizmos.matrix = Matrix4x4.TRS(clue.position, rotation, Vector3.one);
            Gizmos.DrawLineStrip(points.ToArray(), true);
        }
    }
}

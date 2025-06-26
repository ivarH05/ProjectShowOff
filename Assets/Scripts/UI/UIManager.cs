using GameManagement;
using Player;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _singleton;
        private static Dictionary<Type, UIState> _states = new Dictionary<Type, UIState>();
        private static UIState _currentState;

        static UIManager()
        {
            SceneManager.activeSceneChanged += (Scene A, Scene B) =>
            {
                _singleton = null;
                _states.Clear();
                _currentState = null;
            };
        }

        private void Start()
        {
            if (_singleton == null)
                _singleton = this;
            else
                Debug.LogError("More than one UIManager found in the scene");

            UIState[] states = GetComponentsInChildren<UIState>(true);
            for (int i = 0; i < states.Length; i++)
                _states.Add(states[i].GetType(), states[i]);

            CloseAll();
        }
        private void CloseAll()
        {
            foreach (var state in _states.Values)
                state.gameObject.SetActive(false);
        }

        public static void Close()
        {
            _currentState?.OnStateStop();
            _currentState = null;

            for (int i = 0; i < PlayerManager.PlayerCount; i++)
            {
                PlayerController player = PlayerManager.GetPlayer(i);
                player.enabled = true;
                var playerInput = player.gameObject.GetComponent<PlayerInput>();
                if (playerInput != null) playerInput.enabled = true;
            }
        }

        public static void SetState<T>() where T : UIState
        {
            if(_singleton == null)
            {
                Debug.LogError("Scene doesnt have a UIManager!");
                return;
            }
            if(!_states.ContainsKey(typeof(T)))
            {
                Debug.LogError($"UIState {typeof(T)} not assigned", _singleton);
                return;
            }

            UIState state = _states[typeof(T)];
            if (state == _currentState) 
                return;
            
            _currentState?.OnStateStop();
            _currentState = state;
            _currentState.OnStateStart();

            for (int i = 0; i < PlayerManager.PlayerCount; i++)
            {
                PlayerController player = PlayerManager.GetPlayer(i);
                player.enabled = false;
                var playerInput = player.gameObject.GetComponent<PlayerInput>();
                if (playerInput != null) playerInput.enabled = false;
            }
        }

        public static void CallUIEvent<T>(T context) where T : UIEvent => _currentState.OnEvent(context);
    }
}

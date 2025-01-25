using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ {
    public class GameController : SingletonBehaviour<GameController> {
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private List<ObstacleController> obstacles;
        [SerializeField]
        private new Camera camera;

        private List<ObstacleController> obstaclePool;

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }

        public float Timer { get; private set; }

        private void Start() {
            GameStart();
        }

        public void GameStart() {
            IsPlaying = true;
            this.OnGameStart?.Invoke();
        }

        public void GameOver() {
            this.OnGameEnd?.Invoke();   
        }
    }
}

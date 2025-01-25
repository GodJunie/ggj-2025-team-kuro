using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ {
    public class GameController : SingletonBehaviour<GameController> {
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private List<ObstacleController> obstacles;
        [SerializeField]
        private new Camera camera;
        
        [SerializeField]
        private float maxDistance = 2f;

        [SerializeField]
        private int maxOrder = 4;

        private List<ObstacleController> obstaclePool;

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }
        public float Timer { get; private set; }
        public int Order { get; private set; }
        public int Count { get; private set; }

        protected override void Awake() {
            base.Awake();
            obstaclePool = new List<ObstacleController>();
        }

        private void Start() {
            GameStart();
        }

        private void Update() {
            Timer += Time.deltaTime;

            while(obstaclePool.Count < 10) {
                GenerateObject();
            }
        }

        private Rect GetCamRect() {
            return new Rect(camera.transform.position.x, camera.transform.position.y, camera.orthographicSize * 18f / 16f, camera.orthographicSize * 2f);
        }

        private void GenerateObject() {
            var obj = Instantiate(obstacles.Random());

            Vector3 pos = getRandomCord();

            var dir = player.transform.position - pos;
            obj.Init(pos, dir.normalized * 2f);
            obstaclePool.Add(obj);
        }

        private Vector3 getRandomCord() {
            var camRect = GetCamRect();

            float randomRadius = UnityEngine.Random.Range(camRect.xMax, camRect.xMax + maxDistance);

            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            // circular
            float x = randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            float y = randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

            Vector3 randomPosition = new Vector3(x, y, 0f);

            return randomPosition;
        }

        public void GameStart() {
            Order = 0;
            Count = 0;

            this.OnGameStart?.Invoke();
            obstaclePool.Clear();

            camera.orthographicSize = 10f;
            camera.transform.localScale = Vector3.one;
            player.transform.localScale = Vector3.one;

            IsPlaying = true;
        }

        public void GameOver() {
            this.OnGameEnd?.Invoke();
        }

        public void Eat() {
            Count++;
            // order 0~4
            Order = Count / 10;
            int clampedOrder = Mathf.Clamp(Order, 0, maxOrder);

            float size = Mathf.Lerp(Mathf.Pow(2, Order), Mathf.Pow(2, Order + 1), (Count % 10) / 10f);
            float clampedSize = Mathf.Lerp(Mathf.Pow(2, clampedOrder), Mathf.Pow(2, clampedOrder + 1), (Count % 10) / 10f);

            camera.DOKill();
            camera.DOOrthoSize(clampedSize * 10f, .5f);

            camera.transform.DOKill();
            camera.transform.DOScale(clampedSize, .5f);

            player.transform.DOKill();
            player.transform.DOScale(size, .5f);
        }

        public void DestroyObstacle(ObstacleController obstacle) {
            obstaclePool.Remove(obstacle);
            Destroy(obstacle.gameObject);
        }
    }
}

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ
{
    public class GameController : SingletonBehaviour<GameController>
    {
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private List<ObstacleController> obstacles;
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private float sizeMax;
        [SerializeField]
        private float maxDistance = 2f;

        private List<ObstacleController> obstaclePool;

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }
        public float Timer { get; private set; }

        [SerializeField]
        private int objCountMin;
        [SerializeField]
        private int objCountMax;

        private float size;

        protected override void Awake()
        {
            base.Awake();
            obstaclePool = new List<ObstacleController>();
        }

        private void Start()
        {
            GameStart();
        }

        private void Update()
        {
            Timer += Time.deltaTime;

            float objCount = Mathf.Lerp(objCountMin, objCountMax, Timer / 100f);

            while (obstaclePool.Count < objCount)
            {
                GenerateObject();
            }
        }

        private Rect GetCamRect()
        {
            return new Rect(camera.transform.position.x, camera.transform.position.y, camera.orthographicSize * 18f / 16f, camera.orthographicSize * 2f);
        }

        private void GenerateObject()
        {
            var obj = Instantiate(obstacles.Where(e => e.Size < player.Size).Random());


            Vector3 pos = getRandomCord();
            //var camRect = GetCamRect();



            // switch(obstaclePool.Count % 4) {
            // case 0:
            //     pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //     break;
            // case 1:
            //     pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //     break;
            // case 2:
            //     pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //     break;
            // case 3:
            //     pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //     break;
            // default:
            //     pos = Vector3.zero;
            //     break;
            // }

            var dir = player.transform.position - pos;
            obj.Init(pos, dir.normalized * 2f);
            obstaclePool.Add(obj);
        }

        private Vector3 getRandomCord()
        {
            var camRect = GetCamRect();

            float randomRadius = UnityEngine.Random.Range(camRect.xMax, camRect.xMax + maxDistance);

            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            // circular
            float x = randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            float y = randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

            Vector3 randomPosition = new Vector3(x, y, 0f);

            return randomPosition;
        }

        public void GameStart()
        {
            IsPlaying = true;
            this.OnGameStart?.Invoke();
            obstaclePool.Clear();
            size = 5f;
            camera.orthographicSize = size;
        }

        public void GameOver()
        {
            this.OnGameEnd?.Invoke();
        }

        public void Eat()
        {
            camera.DOKill();
            size += 0.5f;
            camera.DOOrthoSize(size, 0.05f);
        }

        public void DestroyObstacle(ObstacleController obstacle)
        {
            obstaclePool.Remove(obstacle);
            Destroy(obstacle.gameObject);
        }
    }
}

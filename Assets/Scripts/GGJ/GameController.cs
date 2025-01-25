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
        private int sizeMax;

        private List<ObstacleController> obstaclePool;

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }
        public float Timer { get; private set; }
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
            Vector3 pos;
            var camRect = GetCamRect();


            pos = new Vector3(camRect.xMin - 3f, UnityEngine.Random.Range(camRect.yMax, camRect.yMin), 0f);

            //switch(obstaclePool.Count % 4) {
            //case 0:
            //    break;
            //case 1:
            //    pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //    break;
            //case 2:
            //    pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //    break;
            //case 3:
            //    pos = new Vector3(camRect.xMin, UnityEngine.Random.Range(camRect.yMin, camRect.yMax), 0f);
            //    break;
            //default:
            //    pos = Vector3.zero;
            //    break;
            //}

            var dir = player.transform.position - pos;
            obj.Init(pos, dir.normalized * 2f);
            obstaclePool.Add(obj);
        }

        public void GameStart() {
            IsPlaying = true;
            this.OnGameStart?.Invoke();
            obstaclePool.Clear();
            Count = 0;
            camera.orthographicSize = 10f;
            camera.transform.localScale = Vector3.one;
        }

        public void GameOver() {
            this.OnGameEnd?.Invoke();   
        }

        public void Eat() {
            Count++;
            if(Count > sizeMax) Count = sizeMax;
            
            camera.DOKill();
            camera.DOOrthoSize(Count + 10f, .3f);
            camera.transform.DOKill();
            camera.transform.DOScale(Count * 0.1f + 1f, .3f);
        }

        public void DestroyObstacle(ObstacleController obstacle) {
            obstaclePool.Remove(obstacle);
            Destroy(obstacle.gameObject);
        }
    }
}

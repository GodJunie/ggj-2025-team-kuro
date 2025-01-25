using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ {
    public class GameController : SingletonBehaviour<GameController> {
        [System.Serializable]
        public class ObstacleInfo {
            [SerializeField]
            [HorizontalGroup("g", 80f)]
            [BoxGroup("g/Image")]
            [PreviewField(70f, Alignment = ObjectFieldAlignment.Center)]
            [HideLabel]
            private Sprite image;

            [SerializeField]
            [HorizontalGroup("g")]
            [BoxGroup("g/Infos")]
            private int level;
            [SerializeField]
            [BoxGroup("g/Infos")]
            private float velocity;
            [SerializeField]
            [BoxGroup("g/Infos")]
            private bool followPlayer;

            public int Level => level;
            public Sprite Image => image;
            public float Velocity => velocity;
            public bool FollowPlayer => followPlayer;
        }

        [System.Serializable]
        public class ProbInfo {
            [SerializeField]
            private float[] probs;

            public float[] Probs => probs;

            public int GetLevel() {
                float rand = UnityEngine.Random.Range(0f, probs.Sum());

                for(int i = 0; i < probs.Length; i++) {
                    float prob = probs[i];
                    if(rand < prob) return i - 2;
                    rand -= prob;
                }

                return 0;
            }
        }


        #region Serialized Fields
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private List<ObstacleInfo> obstacleInfos;
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private List<ProbInfo> probInfos;
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private int maxLevel = 4;
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private int maxCount = 60;

        [SerializeField]
        [TabGroup("g", "밸런스")]
        private int minObjCount = 10;
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private int maxObjCount = 20;

        [SerializeField]
        [TabGroup("g", "Objects")]
        private ObstacleController obstaclePrefab;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private PlayerController player;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private new Camera camera;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private float maxDistance = 2f;
        #endregion

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }
        public float Timer { get; private set; }
        public int Level { get; private set; }
        public int Count { get; private set; }

        private List<ObstacleController> obstacles;


        protected override void Awake() {
            base.Awake();
            obstacles = new List<ObstacleController>();
        }

        private void Start() {
            GameStart();
        }

        private void Update() {
            Timer += Time.deltaTime;

            while(obstacles.Count(e => e.Activated) < Mathf.Lerp(minObjCount, maxObjCount, (float)Level / maxLevel)) {
                GenerateObject();
            }
        }

        private void GenerateObject() {
            var obstacle = obstacles.Find(e => !e.Activated);

            if(obstacle == null) {
                obstacle = Instantiate(obstaclePrefab);
                obstacles.Add(obstacle);
            }

            int level = probInfos[Level].GetLevel() + Level;
            var info = obstacleInfos.Where(e => e.Level == level).Random();

            var pos = GetRandomSpawnPoint();
            var dir = (Vector2)player.transform.position - pos;

            obstacle.Init(pos, GetRandomVelocity(pos).normalized * info.Velocity, info.Image, info.Level);
        }

        private Vector2 GetRandomSpawnPoint() {
            var center = player.transform.position;
            var rad = camera.orthographicSize + maxDistance;
            var right = Vector3.right * rad;
            return center + Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.forward) * right;
        }

        private Vector2 GetRandomVelocity(Vector3 pos) {
            var dir = player.transform.position - pos;

            return Quaternion.AngleAxis(UnityEngine.Random.Range(-30f, 30f), Vector3.forward) * dir;
        }


        public void GameStart() {
            Level = 0;
            Count = 0;

            this.OnGameStart?.Invoke();
            obstacles.Clear();

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

            if(Count >= maxCount) {
                GameOver();
                return;
            }

            // order 0~4
            Level = Count / 10;
            int clampedLevel = Mathf.Clamp(Level, 0, maxLevel);

            float size = Mathf.Lerp(Mathf.Pow(2, Level), Mathf.Pow(2, Level + 1), (Count % 10) / 10f);
            float clampedSize = Mathf.Lerp(Mathf.Pow(2, clampedLevel), Mathf.Pow(2, clampedLevel + 1), (Count % 10) / 10f);

            camera.DOKill();
            camera.DOOrthoSize(clampedSize * 10f, .5f);

            camera.transform.DOKill();
            camera.transform.DOScale(clampedSize, .5f);

            player.transform.DOKill();
            player.transform.DOScale(size, .5f);
        }
    }
}

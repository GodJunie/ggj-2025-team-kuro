using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
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
            private float speed;
            [SerializeField]
            [BoxGroup("g/Infos")]
            private float rotSpeed;
            [SerializeField]
            [BoxGroup("g/Infos")]
            private bool followPlayer;

            public int Level => level;
            public Sprite Image => image;
            public float Speed => speed;
            public bool FollowPlayer => followPlayer;
            public float RotSpeed => rotSpeed;
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
        [TabGroup("g", "밸런스")]
        private float spawnIntervalStart = .4f;
        [SerializeField]
        [TabGroup("g", "밸런스")]
        private float spawnIntervalEnd = .1f;

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
        [SerializeField]
        [TabGroup("g", "Objects")]
        private Material blueMat;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private Material redMat;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private GameObject hand;

        [SerializeField]
        [TabGroup("g", "Objects")]
        private RectTransform title;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private ObstacleController start;
        [SerializeField]
        [TabGroup("g", "Objects")]
        private Sprite startSprite;
        #endregion

        public bool IsPlaying { get; private set; }
        public Action OnGameStart { get; set; }
        public Action OnGameEnd { get; set; }
        public float Timer { get; private set; }
        public int Level { get; private set; }
        public int Count { get; private set; }

        private List<ObstacleController> obstacles;

        private float spawnTimer;

        private ProCamera2D proCam;


        protected override void Awake() {
            base.Awake();

            proCam = Camera.main.GetComponent<ProCamera2D>();   
            obstacles = new List<ObstacleController>();
            obstacles.Add(start);
        }

        private async void ReadyGame() {
            start.gameObject.SetActive(false);
            title.anchoredPosition = new Vector2(0f, 1000f);

            title.DOKill();
            title.DOAnchorPosY(0f, 1f);

            proCam.FollowHorizontal = false;
            proCam.FollowVertical = false;

            await UniTask.Delay(1000);

            start.InitStart(proCam.transform.position + new Vector3(0f, -8f, 0f), startSprite);
        }

        private void Start() {
            ReadyGame();
        }

        private void Update() {
            if(!IsPlaying) {
                if(Input.GetMouseButtonDown(0)) {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    foreach(var hit in Physics2D.RaycastAll(pos, Vector2.zero)) {
                        if(hit.collider.gameObject == start.gameObject) {
                            GameStart();
                            return;
                        }
                    }
                }

                if(Input.touchCount > 0) {
                    var touch = Input.touches[0];
                    
                    var pos = Camera.main.ScreenToViewportPoint(touch.position);
                    
                    foreach(var hit in Physics2D.RaycastAll(pos, Vector2.zero)) {
                        if(hit.collider.gameObject == start.gameObject) {
                            GameStart();
                            return;
                        }
                    }
                }

                return;
            }
            Timer += Time.deltaTime;
            spawnTimer -= Time.deltaTime;

            if(spawnTimer < 0f && obstacles.Count(e => e.Activated) < Mathf.Lerp(minObjCount, maxObjCount, (float)Level / maxLevel)) {
                spawnTimer = Mathf.Lerp(spawnIntervalStart, spawnIntervalEnd, (float)Level / maxLevel);
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

            obstacle.Init(pos, GetRandomVelocity(pos).normalized * info.Speed, info.RotSpeed, info.Image, info.Level);
            obstacle.SetMat(level <= this.Level ? blueMat : redMat);
        }

        private Vector2 GetRandomSpawnPoint() {
            var center = player.transform.position;
            var rad = camera.orthographicSize + maxDistance;
            var right = Vector3.right * rad;

            var pos = center + Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.forward) * right;

            float width = camera.orthographicSize * 9f / 16f;

            pos.x = Mathf.Clamp(pos.x, camera.transform.position.x - width - 1f, camera.transform.position.x + width + 1f);
            pos.y = Mathf.Clamp(pos.y, camera.transform.position.y - camera.orthographicSize - 1f, camera.transform.position.y + camera.orthographicSize + 1f);

            return pos;
        }

        private Vector2 GetRandomVelocity(Vector3 pos) {
            var dir = player.transform.position - pos;

            return Quaternion.AngleAxis(UnityEngine.Random.Range(-45f, 45f), Vector3.forward) * dir;
        }


        public async void GameStart() {
            Level = 0;
            Count = 0;

            Time.timeScale = 1f;

            player.gameObject.SetActive(true);

            obstacles.Clear();

            camera.transform.DOKill();
            camera.DOKill();
            player.transform.DOKill();
            
            camera.DOOrthoSize(10f, 1f);
            camera.transform.DOScale(1f, 1f);
            player.transform.localScale = Vector3.zero;
            player.transform.DOScale(1f, 1f);

            proCam.FollowHorizontal = true;
            proCam.FollowVertical = true;

            title.DOKill();
            title.DOAnchorPosY(1000f, 1f);

            player.transform.DOMove(start.transform.position, 1f);

            this.OnGameStart?.Invoke();

            await UniTask.Delay(1000);

            IsPlaying = true;
        }

        public void GameOver() {
            IsPlaying = false;
            this.OnGameEnd?.Invoke();

            foreach(var obstacle in obstacles) {
                obstacle.gameObject.SetActive(false);
            }

            ReadyGame();
        }

        public async void GameClear() {
            this.OnGameEnd?.Invoke();
            IsPlaying = false;
            foreach(var obstacle in obstacles) {
                obstacle.gameObject.SetActive(false);
            }
            hand.SetActive(true);
            await UniTask.Delay(1000, true);
            player.Dead();
            await UniTask.Delay(1000, true);
            hand.SetActive(false);

            ReadyGame();
        }

        public void Eat() {
            Count++;

            if(Count >= maxCount) {
                GameClear();
                return;
            }

            // order 0~4
            Level = Count / 10;
            int clampedLevel = Mathf.Clamp(Level, 0, maxLevel);

            float size = Mathf.Lerp(Mathf.Pow(2, Level), Mathf.Pow(2, Level + 1), (Count % 10) / 10f);
            float clampedSize = Mathf.Lerp(Mathf.Pow(2, clampedLevel), Mathf.Pow(2, clampedLevel + 1), (Count % 10) / 10f);

            camera.DOKill();
            camera.DOOrthoSize(clampedSize * Mathf.Lerp(10f, 3f, (float)Level / maxLevel), .5f);

            camera.transform.DOKill();
            camera.transform.DOScale(clampedSize * Mathf.Lerp(1f, .3f, (float)Level / maxLevel), .5f);

            player.transform.DOKill();
            player.transform.DOScale(size, .5f);

            Level = clampedLevel;

            foreach(var obstacle in obstacles) {
                obstacle.SetMat(obstacle.Level <= this.Level ? blueMat : redMat);
            }
        }

        [Button]
        private void OrderObstacles() {
            obstacleInfos = obstacleInfos.OrderBy(e => e.Level).ToList();
        }
    }
}

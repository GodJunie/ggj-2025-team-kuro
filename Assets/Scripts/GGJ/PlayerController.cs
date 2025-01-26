using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody2D))]
    //[RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private AudioClip biteClip;
        [SerializeField] private AudioClip popClip;

        [SerializeField]
        private float maxSpeed = 5f;
        [SerializeField]
        [Range(0f, 100f)]
        private float sensitivity = 50f;

        [SerializeField]
        private Animator anim;

        private Rigidbody2D rigid;
        private AudioSource audioSource;

        private Vector2 targetVelocity;

        private new Camera camera;
        private float size;

        public bool IsDead { get; private set; }

        private void Awake() {
            rigid = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            this.camera = Camera.main;
        }

        void Start() {
            // 초기 목표 위치는 현재 위치
            GameController.Instance.OnGameStart += GameStart;
        }

        void Update() {
            if(IsDead) return;

            MouseInput();
            TouchInput();

            var diff = targetVelocity - rigid.linearVelocity;
            rigid.linearVelocity += diff * Mathf.Clamp01(sensitivity * Time.deltaTime);

            var direction = rigid.linearVelocity;
            var rotation = Quaternion.LookRotation(Vector3.forward, direction);
            this.transform.rotation = rotation;
        }

        private void MouseInput() {
            if(Input.GetMouseButton(0)) {
                var pos = camera.ScreenToWorldPoint(Input.mousePosition);

                targetVelocity = pos - transform.position;
                targetVelocity = targetVelocity.normalized * Mathf.Clamp(targetVelocity.magnitude, 0f, maxSpeed) * transform.localScale.x;
            }
        }

        private void TouchInput() {
            if(Input.touchCount > 0) {
                var touch = Input.touches[0];
                var pos = camera.ScreenToWorldPoint(touch.position);

                targetVelocity = pos - transform.position;
                targetVelocity = targetVelocity.normalized * Mathf.Clamp(targetVelocity.magnitude, 0f, maxSpeed) * transform.localScale.x;
            }
        }

        private void GameStart() {
            this.size = 1f;
            IsDead = false;
            anim.SetBool("Dead", false);
            this.transform.localScale = Vector3.one;
        }

        private void Eat() {
            GameController.Instance.Eat();
            anim.SetTrigger("Eat");
            PlayBiteEffect();
        }

        private void PlayBiteEffect() {
            audioSource.PlayOneShot(biteClip);
        }

        private void PlayPopEffect() {
            audioSource.PlayOneShot(popClip);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(IsDead) return;

            if(collision.tag.Equals("Obstacle")) {
                var obstacle = collision.GetComponent<ObstacleController>();

                if(GameController.Instance.Level < obstacle.Level) {
                    IsDead = true;
                    anim.SetBool("Dead", true);
                    anim.SetTrigger("Pop");
                    PlayPopEffect();
                    GameController.Instance.GameOver();
                } else {
                    Eat();
                }
            }
        }
    }
}

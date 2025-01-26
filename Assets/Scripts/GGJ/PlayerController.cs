using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(Rigidbody2D))]
    //[RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private AudioClip biteClip;
        [SerializeField] private AudioClip popClip;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float increaseSpeed = 1.5f;
        //[SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float deceleration = 3f; // 감속도
        [SerializeField] private float rotationSpeed = 5f;

        [SerializeField]
        private Animator anim;

        private Rigidbody2D rigid;
        private AudioSource audioSource;

        private Vector3 targetPosition;
        private Vector3 currentVelocity = Vector3.zero; // 현재 속도
        private bool isAlive = true;
        private bool isMoving = false;

        private float size;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            GameStart();
        }

        void Start()
        {
            // 초기 목표 위치는 현재 위치
            GameController.Instance.OnGameStart += GameStart;
            targetPosition = transform.position;
        }

        void Update()
        {
            if (!isAlive) return;

            // mouse click input
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetTargetPosition(mousePosition);
            }

            //screen touch input
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        SetTargetPosition(touch.position);
                    }
                }
            }

            // move
            if (isMoving)
            {
                MoveToTarget();
            }
        }

        private void SetTargetPosition(Vector3 position)
        {
            //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(position.x, position.y, transform.position.z); // Y축만 변경
            isMoving = true;
        }

        private void MoveToTarget()
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            currentVelocity = Vector3.MoveTowards(currentVelocity, direction * speed, acceleration * Time.deltaTime);
            transform.position += currentVelocity * Time.deltaTime;

            RotateToTarget(transform.position);

            // Arrived at dest
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                currentVelocity = Vector3.zero; // 정지 상태로 속도 초기화
            }
        }

        private void RotateToTarget(Vector3 position)
        {
            // 목표 각도 계산
            float targetAngle = Mathf.Atan2(targetPosition.y - position.y, targetPosition.x - position.x) * Mathf.Rad2Deg;

            // 현재 각도와 목표 각도 사이의 부드러운 회전 적용
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle - 90, rotationSpeed * Time.deltaTime);

            // Z축 회전 적용
            transform.rotation = Quaternion.AngleAxis(smoothedAngle, Vector3.forward);
        }

        private void SetSpeed()
        {
            speed *= gameObject.transform.localScale.x;
        }

        private void GameStart()
        {
            this.size = 1f;
            IsDead = false;
            anim.SetBool("Dead", false);
            this.transform.localScale = Vector3.one;
        }

        private void Eat()
        {
            GameController.Instance.Eat();
            anim.SetTrigger("Eat");
            SetSpeed();
            PlayBiteEffect();
        }

        private void PlayBiteEffect()
        {
            audioSource.PlayOneShot(biteClip);
        }

        private void PlayPopEffect()
        {
            audioSource.PlayOneShot(popClip);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsDead) return;

            if (collision.tag.Equals("Obstacle"))
            {
                var obstacle = collision.GetComponent<ObstacleController>();

                if (GameController.Instance.Level < obstacle.Level)
                {
                    IsDead = true;
                    anim.SetBool("Dead", true);
                    anim.SetTrigger("Pop");
                    PlayPopEffect();
                    GameController.Instance.GameOver();
                }
                else
                {
                    Eat();
                }
            }
        }
    }
}

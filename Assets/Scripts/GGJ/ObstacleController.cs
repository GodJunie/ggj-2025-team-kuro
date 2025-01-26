using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace GGJ {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ObstacleController : MonoBehaviour {
        private new SpriteRenderer renderer;
        private Rigidbody2D rigid;
        private new BoxCollider2D collider;

        public int Level {  get; private set; }

        private bool followPlayer;
        private float rotSpeed;

        public bool Activated {
            get {
                return this.gameObject.activeInHierarchy;
            }
        }

        private void Awake() {
            this.rigid = GetComponent<Rigidbody2D>(); 
            this.renderer = GetComponent<SpriteRenderer>();
            this.collider = GetComponent<BoxCollider2D>();
        }

        private void Start() {

        }

        public void InitStart(Vector2 position, Sprite image) {
            this.renderer.sprite = image;
            this.transform.position = position;
            this.transform.localScale = Vector3.one;
            this.Level = 0;

            this.gameObject.SetActive(true);

            this.rigid.linearVelocity = Vector2.zero;
            this.rigid.angularVelocity = 0f;
        }

        public void Init(Vector2 position, Vector2 velocity, float rotSpeed, Sprite image, int level) {
            this.gameObject.SetActive(false);

            this.renderer.sprite = image;

            this.transform.position = position;
            Debug.Log(velocity);

            var rect = image.rect;
            rect.width /= 100f;
            rect.height /= 100f;
            this.collider.size = rect.size;

            float size = Mathf.Lerp(Mathf.Pow(2, level), Mathf.Pow(2, level + 1), Random.Range(0f, .3f));

            var multiplier = size / Mathf.Max(rect.width, rect.height);
            this.transform.localScale = Vector3.one * multiplier;

            this.Level = level;

            this.gameObject.SetActive(true);
            this.rigid.linearVelocity = velocity;
            this.rigid.angularVelocity = rotSpeed;
        }

        public void SetMat(Material mat) {
            this.renderer.material = mat;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.tag.Equals("Player")) {
                this.gameObject.SetActive(false);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if(collision.tag.Equals("MainCamera")) {
                this.gameObject.SetActive(false);
            }
        }
    }
}

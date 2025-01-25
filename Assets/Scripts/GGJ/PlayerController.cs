using UnityEngine;

namespace GGJ {
    public class PlayerController : MonoBehaviour {
        [SerializeField]
        private float speed;

        private float size;


        private void Start() {
            GameController.Instance.OnGameStart += GameStart;
        }

        private void Update() {
            var deltaPos = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
            deltaPos *= speed * Time.deltaTime;
            this.transform.position += deltaPos;
        }

        private void GameStart() {
            this.size = 1f;
            this.transform.localScale = Vector3.one;
        }

        private void Eat() {
            this.size += 0.1f;
            this.transform.localScale = Vector3.one * size;
        }


        private void OnTriggerEnter(Collider other) {
            if(other.tag.Equals("Obstacle")) {
                var obstacle = other.GetComponent<ObstacleController>();

                var size = obstacle.Size;

                if(this.size < size) {

                }
            }
        }
    }
}

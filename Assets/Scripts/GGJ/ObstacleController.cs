using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleController : MonoBehaviour {
        [SerializeField]
        private float velocity;
        [SerializeField]
        private float defaultSize;

        public float Size {
            get {
                return defaultSize * transform.localScale.x;
            }
        }

        private Rigidbody rigid;
        
        private void Awake() {
            this.rigid = GetComponent<Rigidbody>(); 
        }

        private void Start() {

        }

        public void Init(Vector3 position, Vector2 velocity) {
            this.transform.position = position;
            this.rigid.angularVelocity = GetRandomVector3() * 2f;
            this.rigid.linearVelocity = velocity;
        }

        public static Vector3 GetRandomVector3() {
            return new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }

        public static Vector2 GetRandomVector2() {
            return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }
    }
}

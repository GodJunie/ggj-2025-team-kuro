using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]    
    public class ObstacleController : MonoBehaviour {
        [SerializeField]
        private float velocity;

        public float Size {
            get {
                return coll.radius * transform.localScale.x;
            }
        }

        private Rigidbody rigid;
        private SphereCollider coll;
        
        private void Awake() {
            this.rigid = GetComponent<Rigidbody>(); 
            this.coll = GetComponent<SphereCollider>();
        }

        private void Start() {
            Init(velocity);
        }

        public void Init(float velocity) {
            this.rigid.angularVelocity = GetRandomVector3() * 2f;
            this.rigid.linearVelocity = GetRandomVector2() * velocity;
        }

        public static Vector3 GetRandomVector3() {
            return new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }

        public static Vector2 GetRandomVector2() {
            return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }
    }
}

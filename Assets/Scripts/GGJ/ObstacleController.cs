using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]    
    public class ObstacleController : MonoBehaviour {
        [SerializeField]
        private float velocity;
        [SerializeField]
        private float size;

        public float Size {
            get {
                return size * transform.localScale.x;
            }
        }

        private Rigidbody rigid;
        private Collider coll;
        
        private void Awake() {
            this.rigid = GetComponent<Rigidbody>(); 
            this.coll = GetComponent<Collider>();
        }

        private void Start() {
            Init(velocity);
        }

        [Button]
        public void Init(float velocity) {
            this.rigid.angularVelocity = GetRandomVector3() * 10f;
            this.rigid.linearVelocity = GetRandomVector3() * velocity;
        }

        private Vector3 GetRandomVector3() {
            return new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }
    }
}

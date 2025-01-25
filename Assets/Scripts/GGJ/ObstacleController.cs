using UnityEngine;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]    
    public class ObstacleController : MonoBehaviour {
        [SerializeField]
        private float velocity;
        [SerializeField]
        private Transform obj;

        private Rigidbody rigid;
        private Collider coll;
        
        private void Awake() {
            this.rigid = GetComponent<Rigidbody>(); 
            this.coll = GetComponent<Collider>();
        }

        public void Init() {

        }

        private void Update() {
                
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

namespace GGJ {
    [RequireComponent(typeof(Rigidbody))]
    public class ObstacleController : MonoBehaviour {
        [SerializeField]
        private float velocity;
        [SerializeField]
        private int order;
        [SerializeField]
        private Material redMat;
        [SerializeField]
        private Material blueMat;
        [SerializeField]
        private float rotSpeed;
        [SerializeField]
        private bool is2DObj;

        public int Order => order;

        private Rigidbody rigid;
        
        private void Awake() {
            this.rigid = GetComponent<Rigidbody>(); 
        }

        private void Start() {

        }

        public void Init(Vector3 position, Vector2 velocity) {
            this.transform.position = position;
            if(is2DObj) {
                this.rigid.angularVelocity = Vector3.forward * rotSpeed;
            } else {
                this.rigid.angularVelocity = GetRandomVector3() * rotSpeed;
            }
            this.rigid.linearVelocity = velocity;
            CheckOrder();
        }

        public static Vector3 GetRandomVector3() {
            return new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }

        public static Vector2 GetRandomVector2() {
            return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player") {
                GameController.Instance.DestroyObstacle(this);
            }
        }

        private void OnTriggerExit(Collider other) {
            if(other.tag == "MainCamera") {
                GameController.Instance.DestroyObstacle(this);
            }
        }

        public void CheckOrder() {
            var mat = GameController.Instance.Order < this.order ? redMat : blueMat;
            foreach(var renderer in this.GetComponentsInChildren<Renderer>()) {
                var newMats = renderer.materials.Where(e => e != blueMat && e != redMat).ToList();
                newMats.Add(mat);
                newMats.Reverse();
                renderer.materials = newMats.ToArray();
            }
            //if(is2DObj) {
            //} else {
                
            //}
        }
    }
}

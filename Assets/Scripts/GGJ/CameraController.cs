using UnityEngine;

namespace GGJ
{
    public class CameraController : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                
            }
        }
    }

}

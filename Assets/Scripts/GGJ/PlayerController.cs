using UnityEngine;

namespace GGJ
{
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;

        private Vector3 targetPosition;
        private bool isAlive = true;
        private bool isMoving = false;

        void Start()
        {
            // 초기 목표 위치는 현재 위치
            targetPosition = transform.position;
        }

        void Update()
        {
            if (!isAlive) return;

            // mouse click input
            if (Input.GetMouseButton(0))
            {
                SetTargetPosition();
            }

            // move
            if (isMoving)
            {
                MoveToTarget();
            }
        }

        private void SetTargetPosition()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(mousePosition.x, mousePosition.y, transform.position.z); // Y축만 변경
            isMoving = true;
        }

        private void MoveToTarget()
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Arrived at dest
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

}

using UnityEngine;

namespace Enemy
{
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("Patrol Points")]
        [SerializeField] private Transform leftEdge;
        [SerializeField] private Transform rightEdge;

        [Header("Enemy")]
        [SerializeField] private Transform enemy;

        [Header("Movement parameters")]
        [SerializeField] private float speed;
        private Vector3 initScale;
        private bool movingLeft;

        [Header("Idle Behaviour")]
        [SerializeField] private float idleDuration;
        private float idleTimer;

        [Header("Enemy Animator")]
        [SerializeField] private Animator anim;

        [Header("Chase Player")]
        public Transform playerTransform;
        public float chaseSpeed;
        public bool isChasing;
        public float chaseDistance;

        private void Awake()
        {
            initScale = enemy.localScale;
        }
        private void OnDisable()
        {
            //anim.SetBool("moving", false);
        }

        private void Update()
        {
            Vector2 rotator;

            if (isChasing)
            {
                if (transform.position.x > playerTransform.position.x)
                {
                    transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
                    transform.position += Vector3.left * chaseSpeed * Time.deltaTime;
                }
                if (transform.position.x < playerTransform.position.x)
                {
                    transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    transform.position += Vector3.right * chaseSpeed * Time.deltaTime;
                    
                }
                if (Vector2.Distance(transform.position, playerTransform.position) > chaseDistance)
                {
                    isChasing = false;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
                {
                    isChasing = true;
                }
                if (movingLeft)
                {
                    if (enemy.position.x >= leftEdge.position.x)
                        MoveInDirection(-1);
                    else
                        DirectionChange();
                }
                else
                {
                    if (enemy.position.x <= rightEdge.position.x)
                        MoveInDirection(1);
                    else
                        DirectionChange();
                }
            }
        }

        private void DirectionChange()
        {
            //anim.SetBool("moving", false);
            idleTimer += Time.deltaTime;

            if (idleTimer > idleDuration)
                movingLeft = !movingLeft;
        }

        private void MoveInDirection(int _direction)
        {
            idleTimer = 0;
            //anim.SetBool("moving", true);

            //Make enemy face direction
            enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
                initScale.y, initScale.z);

            //Move in that direction
            enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
                enemy.position.y, enemy.position.z);
        }
    }
}
using UnityEngine;

namespace Enemy
{
    public class MeleeEnemy : MonoBehaviour
    {
        [Header("Attack Parameters")]
        [SerializeField] private float attackCooldown;
        [SerializeField] private float range;
        [SerializeField] private int damage;

        [Header("Collider Parameters")]
        [SerializeField] private float colliderDistance;
        [SerializeField] private CapsuleCollider2D capsuleCollider;

        [Header("Player Layer")]
        [SerializeField] private LayerMask playerLayer;
        private float cooldownTimer = Mathf.Infinity;

        //References
        private Animator anim;
        private Player.PlayerHealth playerHealth;
        private EnemyPatrol enemyPatrol;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            enemyPatrol = GetComponentInParent<EnemyPatrol>();
        }

        private void Update()
        {
            cooldownTimer += Time.deltaTime;

            //Attack only when player in sight?
            if (PlayerInSight())
            {
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0;
                    anim.SetTrigger("isEnemyAttack");
                }
            }

            if (enemyPatrol != null)
                enemyPatrol.enabled = !PlayerInSight();
        }

        private bool PlayerInSight()
        {
            RaycastHit2D hit =
                Physics2D.BoxCast(capsuleCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(capsuleCollider.bounds.size.x * range, capsuleCollider.bounds.size.y, capsuleCollider.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

            if (hit.collider != null)
                playerHealth = hit.transform.GetComponent<Player.PlayerHealth>();

            return hit.collider != null;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(capsuleCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(capsuleCollider.bounds.size.x * range, capsuleCollider.bounds.size.y, capsuleCollider.bounds.size.z));
        }

        private void DamagePlayer()
        {
            if (PlayerInSight())
                playerHealth.TakeDamage(damage);
        }
    }
}
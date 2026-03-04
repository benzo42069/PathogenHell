using PathogenHell.Combat;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.AI
{
    [RequireComponent(typeof(HealthComponent), typeof(WeaponController))]
    public class EnemyController : MonoBehaviour
    {
        public EnemyDef def;
        private Transform _player;
        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _health.Died += OnDeath;
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (def == null) return;
            _health.Initialize(def.hp);
            GetComponent<WeaponController>().Configure(def.projectile, 1f / def.attackInterval, false);
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = def.tint;
        }

        private void Update()
        {
            if (_player == null || def == null) return;
            var toPlayer = (_player.position - transform.position).normalized;
            var speed = def.moveSpeed;
            Vector3 move = def.movementPattern switch
            {
                MovementPattern.Sine => new Vector3(Mathf.Sin(Time.time * 4f), -1f, 0f),
                MovementPattern.Chase => toPlayer,
                _ => Vector3.down
            };

            transform.position += move.normalized * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var hp = other.GetComponent<HealthComponent>();
            if (hp != null) hp.Damage(def.contactDamage);
            OnDeath();
        }

        private void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}

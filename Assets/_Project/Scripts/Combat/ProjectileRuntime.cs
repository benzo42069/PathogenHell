using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Combat
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ProjectileRuntime : MonoBehaviour
    {
        private ProjectileDef _def;
        private Vector2 _direction;
        private float _life;
        private int _remainingPierce;
        private bool _fromPlayer;

        public void Fire(ProjectileDef def, Vector2 position, Vector2 direction, bool fromPlayer)
        {
            _def = def;
            _direction = direction.normalized;
            _life = def.lifetime;
            _remainingPierce = def.pierce;
            _fromPlayer = fromPlayer;
            transform.position = position;
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = def.tint;
            var col = GetComponent<CircleCollider2D>();
            col.radius = def.collisionRadius;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.position += (Vector3)(_direction * (_def.speed * Time.deltaTime));
            _life -= Time.deltaTime;
            if (_life <= 0f) ProjectilePool.Return(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_fromPlayer && other.CompareTag("Enemy"))
            {
                var hp = other.GetComponent<HealthComponent>();
                if (hp != null) hp.Damage(_def.damage);
                Impact();
            }
            else if (!_fromPlayer && other.CompareTag("Player"))
            {
                var hp = other.GetComponent<HealthComponent>();
                if (hp != null) hp.Damage(_def.damage);
                Impact();
            }
        }

        private void Impact()
        {
            if (_remainingPierce > 0)
            {
                _remainingPierce--;
                return;
            }

            ProjectilePool.Return(this);
        }
    }
}

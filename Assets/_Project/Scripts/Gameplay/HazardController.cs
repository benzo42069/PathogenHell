using PathogenHell.Combat;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Gameplay
{
    public class HazardController : MonoBehaviour
    {
        public HazardDef def;
        private float _life;

        private void Start()
        {
            _life = def.lifetime;
            transform.localScale = Vector3.one * def.radius;
            GetComponent<SpriteRenderer>().color = def.tint;
        }

        private void Update()
        {
            _life -= Time.deltaTime;
            if (_life <= 0f) Destroy(gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<HealthComponent>()?.Damage(def.tickDamage * Time.deltaTime);
        }
    }
}

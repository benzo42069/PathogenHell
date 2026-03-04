using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Combat
{
    public class WeaponController : MonoBehaviour
    {
        private ProjectileDef _projectile;
        private float _fireInterval;
        private float _timer;
        private bool _fromPlayer;

        public void Configure(ProjectileDef projectile, float fireRate, bool fromPlayer)
        {
            _projectile = projectile;
            _fireInterval = 1f / Mathf.Max(0.1f, fireRate);
            _fromPlayer = fromPlayer;
        }

        private void Update()
        {
            if (_projectile == null) return;
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            _timer = _fireInterval;
            var dir = _fromPlayer ? Vector2.up : (Vector2)(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
            var proj = ProjectilePool.Get();
            proj.transform.SetParent(null);
            proj.Fire(_projectile, transform.position, dir, _fromPlayer);
        }
    }
}

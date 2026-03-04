using PathogenHell.Combat;
using PathogenHell.Core;
using PathogenHell.Data;
using PathogenHell.Roguelite;
using UnityEngine;

namespace PathogenHell.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(HealthComponent), typeof(WeaponController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float baseMoveSpeed = 5f;
        private Rigidbody2D _rb;
        private HealthComponent _health;
        private WeaponController _weapon;
        private MutationManager _mutationManager;
        private Vector2 _move;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = GetComponent<HealthComponent>();
            _weapon = GetComponent<WeaponController>();
            _mutationManager = GetComponent<MutationManager>();
            _health.Died += OnDied;
        }

        private void Start()
        {
            var def = GameSession.Instance.selectedPathogen;
            if (def == null) def = GameSession.Instance.catalog.pathogens[0];

            _health.Initialize(100f + def.baseStats.resistance * 10f);
            _weapon.Configure(def.primaryProjectile, def.fireRate, true);
            _mutationManager.Initialize(def);
        }

        private void Update()
        {
            _move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        private void FixedUpdate()
        {
            var speed = baseMoveSpeed + _mutationManager.CurrentStats.mobility;
            _rb.linearVelocity = _move * speed;
        }

        private void OnDied()
        {
            FindAnyObjectByType<RunManager>()?.EndRun(false);
        }
    }
}

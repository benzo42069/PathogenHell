using System;
using UnityEngine;

namespace PathogenHell.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        public float CurrentHealth { get; private set; }
        public event Action Died;
        public event Action<float, float> HealthChanged;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Initialize(float hp)
        {
            maxHealth = hp;
            CurrentHealth = hp;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Damage(float amount)
        {
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
            if (CurrentHealth <= 0f)
            {
                Died?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}

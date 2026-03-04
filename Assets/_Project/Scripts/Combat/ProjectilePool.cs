using System.Collections.Generic;
using UnityEngine;

namespace PathogenHell.Combat
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }
        [SerializeField] private ProjectileRuntime projectilePrefab;
        [SerializeField] private int initialSize = 128;
        private static readonly Stack<ProjectileRuntime> Pool = new();

        private void Awake()
        {
            Instance = this;
            Warmup();
        }

        private void Warmup()
        {
            for (var i = 0; i < initialSize; i++)
            {
                var p = Instantiate(projectilePrefab, transform);
                p.gameObject.SetActive(false);
                Pool.Push(p);
            }
        }

        public static ProjectileRuntime Get()
        {
            if (Pool.Count > 0) return Pool.Pop();
            var p = Instantiate(Instance.projectilePrefab, Instance.transform);
            p.gameObject.SetActive(false);
            return p;
        }

        public static void Return(ProjectileRuntime projectile)
        {
            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(Instance.transform);
            Pool.Push(projectile);
        }
    }
}

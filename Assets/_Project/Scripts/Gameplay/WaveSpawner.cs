using System.Collections;
using PathogenHell.AI;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Gameplay
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject enemyPrefab;

        public IEnumerator RunWave(WaveDef wave)
        {
            for (var i = 0; i < wave.count; i++)
            {
                var point = spawnPoints[i % spawnPoints.Length];
                var enemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);
                enemy.GetComponent<EnemyController>().def = wave.enemy;
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }
}

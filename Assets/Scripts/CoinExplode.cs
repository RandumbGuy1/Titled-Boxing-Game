using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinExplode : MonoBehaviour
{
    [SerializeField] AudioClip cashSpawn;
    [SerializeField] AudioClip coinSpawn;

    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;

    [SerializeField] private GameObject ariCoin;
    [SerializeField] private GameObject beastBill;

    [SerializeField] int itemCount;
    [SerializeField] int beastBillChance;

    public void SpawnCoins()
    {
        for (int i = 0; i < itemCount; i++)
        {
            bool shouldSpawnBill = Random.Range(0, 100) < beastBillChance;
            GameObject drop = Instantiate(shouldSpawnBill ? beastBill : ariCoin, transform.position, Quaternion.identity);

            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 dir = Random.insideUnitSphere;
            dir.y = Mathf.Abs(dir.y);
            rb.AddForce(dir * explosionForce, ForceMode.Impulse);

            AudioManager.Instance.PlayOnce(shouldSpawnBill ? cashSpawn : coinSpawn);
        }
    }
}

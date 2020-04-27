using UnityEngine;

public class EnterVillageTrigger : MonoBehaviour
{
    private GameObject playerVillage = null;
    private IDamageable damageablePlayerVillage = null;

    private void Awake() 
    {
        playerVillage = GameObject.FindGameObjectWithTag("Player");
        damageablePlayerVillage = playerVillage.GetComponent<IDamageable>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        GameObject enteredObject = other.gameObject;
        if (enteredObject.CompareTag("Enemy"))
        {
            if (damageablePlayerVillage != null)
            {
                IDamageable damageableEnteredObject = enteredObject.GetComponent<IDamageable>();
                if (damageableEnteredObject != null)
                {
                    float damage = damageableEnteredObject.GetCurrentHealth();
                    damageablePlayerVillage.TakeDamage(damage);
                    damageableEnteredObject.SelfDestroy(false);
                }
            }
            else
            {
                Debug.LogError("No IDamageable on " + playerVillage.name);
            }
        }
    }
}

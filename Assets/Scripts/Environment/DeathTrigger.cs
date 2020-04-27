using UnityEngine;

namespace TowerDefence.Environment
{
    public class DeathTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other) 
        {
            IDamageable damageableEnteredObject = other.gameObject.GetComponent<IDamageable>();
            if (damageableEnteredObject != null)
            {
                damageableEnteredObject.SelfDestroy(true);
            }
        }
    }
}

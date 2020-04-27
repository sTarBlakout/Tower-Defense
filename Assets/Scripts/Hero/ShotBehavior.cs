using UnityEngine;

namespace TowerDefence.Hero
{
    public class ShotBehavior : MonoBehaviour
    {
        [SerializeField] GameObject hitParticle = null, muzzleParticle = null;

        [SerializeField] bool isAOE = false;
        [SerializeField] float damageAreaAOE = 1f;

        private Transform lookAtTarget = null;
        private float moveSpeed = 0f, damage = 0f;
        private Vector3 translation;

        private void Update() 
        {
            if (lookAtTarget != null)
                transform.LookAt(lookAtTarget);
        }

        private void Start() 
        {
            if (muzzleParticle != null)    
            {
                GameObject muzzle = Instantiate(muzzleParticle, this.transform);
                muzzle.transform.parent = null;
            }
        }

        private void FixedUpdate() 
        {
            if (moveSpeed != 0f)
            {
                translation = transform.forward * moveSpeed;
                transform.Translate(translation, Space.World);
            }
        }

        public void SetBeamStats(float speed, float dmg, Transform target)
        {
            moveSpeed = speed;
            lookAtTarget = target;
            damage = dmg;
        }

        private void OnTriggerEnter(Collider other) 
        {
            IDamageable damageableEnteredObject = other.gameObject.GetComponent<IDamageable>();
            if (damageableEnteredObject != null && hitParticle != null)
            {
                this.transform.GetComponent<Collider>().enabled = false;
                GameObject explosion = Instantiate(hitParticle, this.transform);
                explosion.transform.parent = null;
                if (!isAOE)
                {
                    damageableEnteredObject.TakeDamage(damage);
                }
                else
                {
                    Collider[] overlappingObjects = Physics.OverlapSphere(explosion.transform.position, damageAreaAOE);
                    foreach (Collider overlappingObject in overlappingObjects)
                    {
                        IDamageable damageableOverlappingObj = overlappingObject.gameObject.GetComponent<IDamageable>();
                        if (damageableOverlappingObj != null)
                            damageableOverlappingObj.TakeDamage(damage);
                    }
                }
                this.transform.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace TowerDefence.Hero
{
    public class HeroBehavior : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] GameObject shotPrefab = null;
        [SerializeField] AnimatorOverrideController overrideController = null;
        [SerializeField] Transform shotStartPoint = null;

        [Header("Stats")]
        [SerializeField] float attackRange = 10f;
        [SerializeField] float damage = 15f;
        [SerializeField] float fireRate = 1f;
        [SerializeField] float shotSpeed = 10f;
        [SerializeField] float timeBeforeShotInstantiation = 0.1f;
 
        private Animator animator;

        private float lastTimeShot = 0f;
        public Transform target = null;


        private void Awake() 
        {
            animator = this.transform.GetComponent<Animator>();
            if (animator != null && overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController;
            }
        }

        private void Update() 
        {
            if (target != null)
            {
                transform.LookAt(target);
            }
            ShootingBehavior();
        }

        private void LateUpdate() 
        {
            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);
            transform.rotation = q;  
        }

        public bool IsReadyToShoot()
        {
            return Time.time > lastTimeShot + fireRate;
        }

        private void ShootingBehavior()
        {
            if (IsReadyToShoot())    
            {
                lastTimeShot = Time.time;
                target = FindClosestEnemy();
                if (target != null)
                {
                    if (animator != null) animator.SetTrigger("Shoot");
                    StartCoroutine(InstantiateShotInSeconds(timeBeforeShotInstantiation));
                }
            } 
        }

        private IEnumerator InstantiateShotInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (target != null)
            {
                GameObject shot = Instantiate(shotPrefab, shotStartPoint);
                shot.transform.parent = null;
                shot.GetComponent<ShotBehavior>().SetBeamStats(shotSpeed, damage, target);
            }
        }

        private Transform FindClosestEnemy()
        {
            float closestDistance = attackRange;
            Transform closestEnemy = null;
            Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, attackRange);
            foreach (Collider overlappedObject in overlappedObjects)
            {
                if (!overlappedObject.gameObject.CompareTag("Enemy")) continue;
                IDamageable damageableEnemy =  overlappedObject.GetComponent<IDamageable>();
                if (damageableEnemy != null)
                {
                    if (damageableEnemy.IsDead()) continue;
                }
                float dist = Vector3.Distance(transform.position, overlappedObject.transform.position);
                if (dist < closestDistance)
                {   
                    closestEnemy = overlappedObject.transform;
                    closestDistance = dist;
                }
            }
            return closestEnemy;
        }

    }
}
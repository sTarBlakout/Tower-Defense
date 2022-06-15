using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

namespace TowerDefence.Enemy
{
    public class EnemyBehavior : MonoBehaviour, IDamageable, IScoreable
    {
        [SerializeField] float initialHealth = 100f;
        [SerializeField] int moneyOnDeath = 20;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        private CapsuleCollider capsuleCollider;
        public float currHealth = 0f;
        private bool gaveMoneyForKill = false;

        private NavMeshAgent navMeshAgent = null;
        private Animator animator = null;

        [SerializeField] GameObject healthSliderGO = null;
        private Slider healthSlider = null;

        private List<Transform> wayPoints = new List<Transform>();
        private int nextDestWayPointIdx = 0;
        private bool movingToFinalWayPoint = false;
        private bool isDead = false, selfDestroying = false;

        public Action<IScoreable> OnDie;

        const float remainingDistToChangeDest = 1f;
        const float movingUndrGroundSpd = 0.01f;
        const float selfDestoryInSeconds = 6f;
        const float colliderDecrSizeValueDeath = 0.2f;
    
    #region Unity Methods

        private void Awake() 
        {
            currHealth = initialHealth;
            capsuleCollider =  this.transform.GetComponent<CapsuleCollider>();
            navMeshAgent = this.transform.GetComponent<NavMeshAgent>();
            animator = transform.GetComponentInChildren<Animator>();
            if (healthSliderGO != null)
            {
                healthSlider = healthSliderGO.GetComponent<Slider>();
                healthSlider.value = currHealth / initialHealth;
            }
            FindWayPoints();      
        }

        private void Start() 
        {
            if (animatorOverrideController != null)
                animator.runtimeAnimatorController = animatorOverrideController;
            GoToNextWayPoint();
        }

        private void Update() 
        {
            if (isDead && !selfDestroying)
            {
                MoveUnderGround(movingUndrGroundSpd);
                return;
            }
            MoveProcessing();
            if (healthSliderGO != null)
            {
                healthSliderGO.transform.LookAt(Camera.main.transform);
            }
        }

    #endregion

    #region Private Methods

        private void MoveProcessing()
        {
            if (movingToFinalWayPoint) { return; }
            if (navMeshAgent.remainingDistance < remainingDistToChangeDest)
            {
                GoToNextWayPoint();
            }    
        }

        private void GoToNextWayPoint()
        {
            navMeshAgent.SetDestination(wayPoints[nextDestWayPointIdx].position);
            if (nextDestWayPointIdx != wayPoints.Count - 1)
            {
                nextDestWayPointIdx++;
            }
            else 
            {
                movingToFinalWayPoint = true;
            }
        }

        private void FindWayPoints()
        {
            GameObject wayPointsHolder = GameObject.Find("WayPointsHolder");
            if (wayPointsHolder != null)
            {
                foreach (Transform wayPoint in wayPointsHolder.transform)
                {
                    wayPoints.Add(wayPoint);
                }
            }
            else
            {
                Debug.Log("WayPointsHolder not found!");
            }
        }

        private void MoveUnderGround(float speed)
        {
            transform.Translate(-transform.up * speed);
        }

        private IEnumerator SelfDestroyInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(this.gameObject);
        }

    #endregion

    #region Public Methods

        public void Die()
        {
            isDead = true;
            if (OnDie != null)
                OnDie(this);
            animator.SetTrigger("Die");
            navMeshAgent.enabled = false;
            healthSliderGO.SetActive(false);
            capsuleCollider.center = Vector3.zero;
            capsuleCollider.radius = colliderDecrSizeValueDeath;
            capsuleCollider.height = colliderDecrSizeValueDeath;
        }

        public void TakeDamage(float damage)
        {
            currHealth = Mathf.Clamp(currHealth - damage, 0f, initialHealth);
            if (currHealth == 0f)
            {
                Die();
            }
            if (healthSlider != null) healthSlider.value = currHealth / initialHealth;
        }

        public float GetCurrentHealth()
        {
            return currHealth;
        }

        public void SelfDestroy(bool instantly)
        {
            if (instantly)
            {
                Destroy(this.gameObject);
            }
            else
            {
                selfDestroying = true;
                isDead = true;
                StartCoroutine(SelfDestroyInSeconds(selfDestoryInSeconds));
            }
        }

        public bool IsDead()
        {
            return isDead;
        }

        public int GetMoneyForKill()
        {
            if (gaveMoneyForKill) return 0;
            gaveMoneyForKill = true;
            return moneyOnDeath;
        }

        #endregion

    }
}

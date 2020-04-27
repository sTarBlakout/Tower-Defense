using System;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence.Player
{
    public class PlayerVillageBehavior : MonoBehaviour, IDamageable
    {
        [SerializeField] float initialHealth = 100f;
        [SerializeField] Image villageHealthImg = null;
        [SerializeField] Text healthText = null;

        public Action OnDestroy;

        private float currentHealth = 0f;
        private bool isDestroyed = false;

        private void Awake() 
        {
            currentHealth = initialHealth;    
        }

        private void Start() 
        {
            UpdateCurrentHealthUI();
        }

        private void UpdateCurrentHealthUI()
        {
            if (villageHealthImg != null)    
                villageHealthImg.fillAmount = currentHealth / initialHealth;
            if (healthText != null)
                healthText.text = currentHealth + "/" + initialHealth;
        }

        public void Die()
        {
            isDestroyed = true;
            if (OnDestroy != null)
            {
                OnDestroy();
            }
        }

        public void SelfDestroy(bool instantly)
        {
            Destroy(this.gameObject);
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, initialHealth);
            if (currentHealth == 0f)
            {
                Die();
            }
            UpdateCurrentHealthUI();
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public bool IsDead()
        {
            return isDestroyed;
        }
    }
}
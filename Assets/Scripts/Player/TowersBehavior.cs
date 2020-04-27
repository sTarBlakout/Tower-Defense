using System;
using UnityEngine;

namespace TowerDefence.Player
{
    public class TowersBehavior : MonoBehaviour
    {
        [SerializeField] GameObject spawnButton = null;

        private Transform heroPos = null;

        private GameObject nextHeroInstantiate = null;

        public Action OnHeroSet;

        private void Awake() 
        {
            heroPos = transform.Find("HeroPosition");
        }

        private void Start() 
        {
            if (spawnButton != null)    
            {
                ActivateSpawnButton(false);
            }
        }

        private void ClearHeroPos()
        {
            foreach (Transform child in heroPos)
            {
                Destroy(child.gameObject);
            }
        }

        public void ActivateSpawnButton(bool activate)
        {
            if (spawnButton == null) return;
            spawnButton.transform.LookAt(Camera.main.transform);
            spawnButton.SetActive(activate);
        }

        public void SetNewHero(GameObject hero)
        {
            nextHeroInstantiate = hero;
            ActivateSpawnButton(true);
        }

        public void InstantiateHero()
        {
            ClearHeroPos();
            Instantiate(nextHeroInstantiate, heroPos);
            OnHeroSet();
        }
    }
}   
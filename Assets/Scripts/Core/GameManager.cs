using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TowerDefence.Enemy;
using TowerDefence.Player;
using TowerDefence.UI;

namespace TowerDefence.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Necessary Components")]
        [SerializeField] ParticleSystem enemySpawnPortal = null;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] GameObject gameplayVirtCam = null;
        [SerializeField] GameObject[] enemyPrefabs = null;
        [SerializeField] Hero[] availableHeroes;
        [SerializeField] private GameObject heroButtons;

        [Header("Values")]
        [SerializeField] int initialWaveEnemAmount = 3;
        [SerializeField] float timeWaitBeforeSpawnNext = 1f;
        [SerializeField] float nextWaveEnemAmountMultiplier = 0.5f;
        [SerializeField] Vector2 waveSpawnRandomRange = new Vector2(1f, 2f);
        [SerializeField] int currentMoney = 0;

        private List<TowersBehavior> towersInLvl = new List<TowersBehavior>();

        private PlayerVillageBehavior playerVillage = null;
        private UIManager managerUI = null;

        private bool isWaveEnded = true;

        private float nextSpawnTime = 0f;

        private int maxCurrWaveEnemAmount = 0;
        private int currWaveEnemAmount = 0;

        const float deadEnemiesCheckThreshold = 0.1f;

    #region Unity Methods

        private void Awake() 
        {
            maxCurrWaveEnemAmount = initialWaveEnemAmount;
            playerVillage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerVillageBehavior>();
            managerUI = GameObject.Find("UI").GetComponent<UIManager>();

            if (managerUI == null)
            {
                Debug.LogError("GM can not find UIManager");
            }

            if (playerVillage != null)
            {
                playerVillage.OnDestroy += PlayerDeathProcessing;
            }
            else
            {
                Debug.LogError("GM can not find PlayerVillageBehavior");
            }

            GetAllTowersInLvl();
        }

        private void Start() 
        {
            if (enemySpawnPortal != null)
                enemySpawnPortal.Stop();
            managerUI.SwitchToPanel(PanelType.PANEL_START);
            managerUI.SetMoneyAmount(currentMoney);
            foreach (Hero hero in availableHeroes)
            {
                managerUI.SetHeroesCostText(hero.type, hero.moneyCost);
            }
        }

        private void Update() 
        {
            SpawnNextEnemy();
        }

    #endregion

    #region Private Methods    

        private void GetAllTowersInLvl()
        {
            GameObject[] towersGameObjects = GameObject.FindGameObjectsWithTag("Tower");
            foreach (GameObject tower in towersGameObjects)
            {
                TowersBehavior towersBehavior = tower.GetComponent<TowersBehavior>();
                if (towersBehavior != null)
                {
                    towersInLvl.Add(towersBehavior);
                    towersBehavior.OnHeroSet += DisableTowerChoosing;
                }
            }
        }

        private void SpawnNextEnemy()
        {
            if (Time.time > nextSpawnTime)
            {   
                nextSpawnTime = Time.time + timeWaitBeforeSpawnNext;
                if (!isWaveEnded)
                {
                    if (currWaveEnemAmount < maxCurrWaveEnemAmount)
                    {
                        currWaveEnemAmount++;
                        int randomEnemyID = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                        GameObject spawnedEnemy = Instantiate(enemyPrefabs[randomEnemyID], spawnPoint.position, Quaternion.identity);
                        EnemyBehavior spawnedEnemyBehavior = spawnedEnemy.GetComponent<EnemyBehavior>();
                        if (spawnedEnemyBehavior != null)
                            spawnedEnemyBehavior.OnDie += GetMoney;
                    }
                    else
                    {
                        isWaveEnded = true;
                        if (enemySpawnPortal != null)
                            enemySpawnPortal.Stop();
                    }
                }
                else
                {
                    WaveFinishing();
                }
            }
        }

        private void WaveFinishing()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                managerUI.ShowButton(ButtonType.BUTTON_START_WAVE, true);
                heroButtons.SetActive(true);
            }
        }

        private void GetMoney(IScoreable scoreable)
        {
            if (scoreable != null)
            {
                currentMoney += scoreable.GetMoneyForKill();
                managerUI.SetMoneyAmount(currentMoney);
            }
        }

        private void PlayerDeathProcessing()
        {
            managerUI.SwitchToPanel(PanelType.PANEL_DEATH);
        }

    #endregion

    #region Public Methods    

        public void DisableTowerChoosing()
        {
            foreach (TowersBehavior tower in towersInLvl)
            {
                tower.ActivateSpawnButton(false);
            }
        }

        //Called by UI button
        public void ChooseHero(int typeID)
        {
            Hero currentHero = null;
            foreach (Hero hero in availableHeroes)
            {
                if ((int)hero.type == typeID)
                {
                    currentHero = hero;
                }
            }

            if (currentHero == null)
            {
                Debug.LogError("Wrong hero type!");
                return;
            }

            if (currentMoney >= currentHero.moneyCost)
            {
                currentMoney -= currentHero.moneyCost;
                managerUI.SetMoneyAmount(currentMoney);
                foreach (TowersBehavior towersBehavior in towersInLvl)
                {
                    towersBehavior.SetNewHero(currentHero.heroPrefab);
                }
            }
            else
            {
                managerUI.ActivatePopUp("Not enough money!!!");
            }
        }

        //Called by UI button
        public void StartWave()
        {
            heroButtons.SetActive(false);
            if (enemySpawnPortal != null)
                enemySpawnPortal.Play();
            managerUI.ShowButton(ButtonType.BUTTON_START_WAVE, false);
            maxCurrWaveEnemAmount += (int)(maxCurrWaveEnemAmount * nextWaveEnemAmountMultiplier);
            currWaveEnemAmount = 0;
            isWaveEnded = false;
        }

        //Called by UI button
        public void ReloadScene()
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        public void ReadyToPlay()
        {
            managerUI.SwitchToPanel(PanelType.PANEL_GAMEPLAY);
            if (gameplayVirtCam != null)
                gameplayVirtCam.SetActive(true);
        }

    #endregion    
    }

    [System.Serializable]
    public class Hero
    {
        public HeroType type;
        public int moneyCost;
        public GameObject heroPrefab;
    }

}

public enum HeroType
{
    HERO_ARCHER,
    HERO_GUNNER,
    HERO_MAGE
}
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace TowerDefence.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject panelStart, panelGameplay, panelDeath;

        private GameObject startWaveButton = null;
        private GameObject popUp = null;
        private Text popUpMsg = null;
        private Text moneyText = null;

        private Text mageCostText, gunnerCostText, archerCostText;

        private Coroutine hidePopUpCor = null;
        const float deactivatePopUpThreshold = 1f;

        private void Awake() 
        {
            startWaveButton = panelGameplay.transform.Find("StartWaveButton").gameObject;
            moneyText = panelGameplay.transform.Find("MoneyText").GetComponent<Text>();
            popUp = panelGameplay.transform.Find("PopUp").gameObject;
            popUpMsg = popUp.transform.GetChild(0).GetComponent<Text>();

            mageCostText = panelGameplay.transform.Find("HeroButtons").Find("MageButton").Find("Image").Find("CostText").GetComponent<Text>();
            gunnerCostText = panelGameplay.transform.Find("HeroButtons").Find("GunnerButton").Find("Image").Find("CostText").GetComponent<Text>();
            archerCostText = panelGameplay.transform.Find("HeroButtons").Find("ArcherButton").Find("Image").Find("CostText").GetComponent<Text>();
        }

        private void Start() 
        {
            popUp.SetActive(false);
        }

        private IEnumerator DeactivatePopUpInSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            popUp.SetActive(false);
        }

        public void SetHeroesCostText(HeroType type, int value)
        {
            switch (type)
            {
                case HeroType.HERO_ARCHER:
                    if (archerCostText == null) break;
                    archerCostText.text = value.ToString();
                    break;
                case HeroType.HERO_GUNNER:
                    if (gunnerCostText == null) break;
                    gunnerCostText.text = value.ToString();
                    break;
                case HeroType.HERO_MAGE:
                    if (mageCostText == null) break;
                    mageCostText.text = value.ToString();
                    break;
                default:
                    Debug.LogError("Wrong hero type!");
                    break;
            }
        }

        public void SwitchToPanel(PanelType type)
        {
            panelStart.SetActive(false);
            panelGameplay.SetActive(false);
            panelDeath.SetActive(false);

            switch (type)
            {
                case PanelType.PANEL_START:
                    panelStart.SetActive(true);
                    break;
                case PanelType.PANEL_GAMEPLAY:
                    panelGameplay.SetActive(true);
                    break;
                case PanelType.PANEL_DEATH:
                    panelDeath.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void ShowButton(ButtonType type, bool show)
        {
            switch (type)
            {
                case ButtonType.BUTTON_START_WAVE:
                    if (startWaveButton != null)
                        startWaveButton.SetActive(show);
                    else
                        Debug.LogError("UIManager can not find StartWaveButton");
                    break;
                default:
                    break;
            }
        }

        public void ActivatePopUp(string text)
        {
            if (popUp == null) return;
            popUp.SetActive(false);
            if (hidePopUpCor != null)
            {
                StopCoroutine(hidePopUpCor);
                hidePopUpCor = null;
            }
            popUpMsg.text = text;
            popUp.SetActive(true);
            hidePopUpCor = StartCoroutine(DeactivatePopUpInSeconds(deactivatePopUpThreshold));
        }

        public void SetMoneyAmount(int amount)
        {
            moneyText.text = amount.ToString();
        }
    }
}

public enum PanelType
{
    PANEL_START,
    PANEL_GAMEPLAY,
    PANEL_DEATH
}

public enum ButtonType
{
    BUTTON_START_WAVE
}

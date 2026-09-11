using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public int currentGold = 100;
    public int currentTurretCost = 50;
    public int costIncreaseStep = 10;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() { UpdateUI(); }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    public void OnBuyTurretClicked()
    {
        if (currentGold >= currentTurretCost)
        {
            // Gọi sang DragManager để kiểm tra xem còn chỗ trống đẻ súng không
            if (DragManager.Instance.SpawnBoughtTurret())
            {
                currentGold -= currentTurretCost;
                currentTurretCost += costIncreaseStep;
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        if (goldText != null) goldText.text = currentGold.ToString();
        if (costText != null) costText.text = currentTurretCost.ToString();
        if (buyButton != null) buyButton.interactable = (currentGold >= currentTurretCost);
    }
}
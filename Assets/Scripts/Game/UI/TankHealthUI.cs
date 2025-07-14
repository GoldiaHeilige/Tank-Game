using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TankHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TankHealth playerTank; 

    private void Start()
    {
        if (playerTank != null)
        {
            playerTank.OnHealthChanged += HandleHealthChanged;
            playerTank.OnTankDestroyed += HandleDestroyed;

            HandleHealthChanged(playerTank.CurrentHP, playerTank.tankData.maxHP);
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        hpText.text = $"{current} / {max}";
        hpSlider.value = (float)current / max;
    }

    private void HandleDestroyed()
    {
        hpText.text = "Destroyed";
        hpSlider.value = 0;
    }

    private void OnDisable()
    {
        if (playerTank != null)
        {
            playerTank.OnHealthChanged -= HandleHealthChanged;
            playerTank.OnTankDestroyed -= HandleDestroyed;
        }
    }
}

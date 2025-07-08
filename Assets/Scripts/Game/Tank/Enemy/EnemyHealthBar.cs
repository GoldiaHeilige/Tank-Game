using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Target")]
    [SerializeField] private TankHealth tankHealth;
    [SerializeField] private Transform followTarget;

    private void Start()
    {
        if (tankHealth == null)
            tankHealth = GetComponentInParent<TankHealth>();

        if (followTarget == null)
            followTarget = tankHealth != null ? tankHealth.transform : transform;

        if (tankHealth != null)
        {
            tankHealth.OnHealthChanged += UpdateHealth;
            tankHealth.OnTankDestroyed += Hide;

            UpdateHealth(tankHealth.CurrentHP, tankHealth.tankData.maxHP);
        }
    }

    private void UpdateHealth(int current, int max)
    {
        if (hpSlider != null)
            hpSlider.value = (float)current / max;

        if (hpText != null)
            hpText.text = $"HP: {current} / {max}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (followTarget == null || Camera.main == null) return;

        // Gắn lên đầu đối tượng
        transform.position = followTarget.position + Vector3.up * 2f;

        // Tính hướng camera theo mặt phẳng XZ (không lật trục X/Z)
        Vector3 dirToCam = Camera.main.transform.position - transform.position;
        dirToCam.y = 0; // Loại bỏ ảnh hưởng trục Y

        if (dirToCam.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(-dirToCam); // -dir để nó quay về phía camera
            transform.rotation = targetRot;
        }
    }

    private void OnDisable()
    {
        if (tankHealth != null)
        {
            tankHealth.OnHealthChanged -= UpdateHealth;
            tankHealth.OnTankDestroyed -= Hide;
        }
    }
}

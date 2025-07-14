using TMPro;
using UnityEngine;

public class TankAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadingText;

    private float reloadRemaining;
    private bool isReloading;

    private void OnEnable()
    {
        TankShooting.OnAmmoChanged += HandleAmmoChanged;
        TankShooting.OnReloadStart += HandleReloadStart;
        TankShooting.OnReloadComplete += HandleReloadComplete;
    }

    private void OnDisable()
    {
        TankShooting.OnAmmoChanged -= HandleAmmoChanged;
        TankShooting.OnReloadStart -= HandleReloadStart;
        TankShooting.OnReloadComplete -= HandleReloadComplete;
    }

    private void HandleAmmoChanged(int current, int max)
    {
        ammoText.text = $"{current}";
    }

    private void HandleReloadStart(float duration)
    {
        reloadingText.gameObject.SetActive(true);
        reloadRemaining = duration;
        isReloading = true;
    }

    private void HandleReloadComplete(int current, int max)
    {
        reloadingText.gameObject.SetActive(false);
        ammoText.text = $"{current}";
        isReloading = false;
    }

    private void Update()
    {
        if (isReloading)
        {
            reloadRemaining -= Time.deltaTime;
            if (reloadRemaining < 0) reloadRemaining = 0;
            reloadingText.text = $"Reloading ({reloadRemaining:F1}s)";
        }
    }
}

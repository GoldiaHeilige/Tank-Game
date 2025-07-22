using UnityEngine;
using System.Collections;

public class TankRepairSystem : MonoBehaviour
{
    [Header("Repair Kit Settings")]
    [SerializeField] private int defaultRepairKitCount = 1;
    [SerializeField] private float repairDuration = 10f;

    [Header("Fire Extinguisher Settings")]
    [SerializeField] private int defaultExtinguisherCount = 1;
    [SerializeField] private float extinguishDuration = 5f;

    private TankModuleHP[] allModules;
    private int remainingRepairKits;
    private int remainingExtinguishers;

    private bool isRepairing = false;
    private bool isExtinguishing = false;
    private bool isBurning = false;

    private EngineFireHandler fireHandler;
    private InputHandler inputHandler;

    public bool IsRepairing => isRepairing || isExtinguishing;

    public System.Action OnRepairStarted;
    public System.Action OnRepairCompleted;

    private void Start()
    {
        allModules = GetComponentsInChildren<TankModuleHP>();
        remainingRepairKits = defaultRepairKitCount;
        remainingExtinguishers = defaultExtinguisherCount;

        fireHandler = GetComponentInChildren<EngineFireHandler>();
        inputHandler = GetComponentInParent<InputHandler>();
    }

    private void Update()
    {
        if (InputHandler.Instance.RepairPressed)
        {
            UseRepairKit();
        }

        if (InputHandler.Instance.ExtinguishPressed)
        {
            UseExtinguisher();
        }
    }

    public void SetBurning(bool burning)
    {
        isBurning = burning;
    }

    public bool CanUseRepairKit()
    {
        if (IsRepairing || remainingRepairKits <= 0) return false;

        foreach (var module in allModules)
        {
            if (module != null && module.IsDestroyed)
                return true;
        }

        return false;
    }

    public void UseRepairKit()
    {
        if (!CanUseRepairKit()) return;

        StartCoroutine(RepairCoroutine());
    }

    public bool CanUseExtinguisher()
    {
        return isBurning && !IsRepairing && remainingExtinguishers > 0;
    }

    public void UseExtinguisher()
    {
        if (!CanUseExtinguisher()) return;

        StartCoroutine(ExtinguishCoroutine());
        remainingExtinguishers--;
    }

    private IEnumerator RepairCoroutine()
    {
        isRepairing = true;
        remainingRepairKits--;
        OnRepairStarted?.Invoke();

        Debug.Log("[RepairSystem] Repair started...");
        inputHandler?.BlockInput(true);

        yield return new WaitForSeconds(repairDuration);

        foreach (var module in allModules)
        {
            if (module != null && module.IsDestroyed)
                module.Repair();
        }

        Debug.Log("[RepairSystem] Repair completed.");
        inputHandler?.BlockInput(false);
        isRepairing = false;
        OnRepairCompleted?.Invoke();
    }

    private IEnumerator ExtinguishCoroutine()
    {
        isExtinguishing = true;
        OnRepairStarted?.Invoke();

        Debug.Log("[RepairSystem] Extinguishing fire...");
        inputHandler?.BlockInput(true);

        yield return new WaitForSeconds(extinguishDuration);

        fireHandler?.StopFire();
        isBurning = false;

        Debug.Log("[RepairSystem] Fire extinguished.");
        inputHandler?.BlockInput(false);
        isExtinguishing = false;
        OnRepairCompleted?.Invoke();
    }

    public int GetRemainingRepairKits() => remainingRepairKits;
    public int GetRemainingExtinguishers() => remainingExtinguishers;
}

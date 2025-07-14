using UnityEngine;

[CreateAssetMenu(menuName = "Tank/ModuleConfig")]
public class ModuleConfig : ScriptableObject
{
    public string displayName = "Gun";
    public ModuleType type = ModuleType.Custom;

    [Header("HP")]
    public int moduleHP = 50;

    [Header("Penetration Behavior")]
    public bool allowPenetrateWhenDestroyed = false;
    [Range(0f, 1f)] public float damagePercentToTank = 0.25f;

    [Header("Optional FX")]
    public GameObject onDestroyedFX;
}

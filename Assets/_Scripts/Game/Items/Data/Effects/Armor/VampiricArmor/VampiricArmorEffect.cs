using System;
using System.Collections;
using UnityEngine;

public class VampiricArmorEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [Header("Start of combat")]
    [SerializeField] private Buff _vampirismBuff;
    [SerializeField] private float _convertHpAmount;
    [SerializeField] private int _gainArmorAmount;

    [Header("Routine")]
    [SerializeField] private float _convertCooldown = 2.8f;
    [SerializeField] private float _convertHpRoutineAmount;
    [SerializeField] private int _gainArmorRoutineAmount;


    private Coroutine _convertHpRoutine;


    private ItemBehaviour _vampiricArmor;
    private void Awake()
    {
        _vampiricArmor = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_convertHpRoutine != null)
        {
            StopCoroutine(_convertHpRoutine);
            _convertHpRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_convertHpRoutine != null)
        {
            StopCoroutine(_convertHpRoutine);
            _convertHpRoutine = null;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_vampiricArmor == null || _vampiricArmor.OwnerCharacter == null) return;

        _vampiricArmor.OwnerCharacter.TakeDamage(_convertHpAmount, ItemDataSO.ExtraType.Effect);
        _vampiricArmor.OwnerCharacter.AddArmor(_gainArmorAmount);

        if (_convertHpRoutine == null)
        {
            _convertHpRoutine = StartCoroutine(ConvertHpRoutine());
        }

    }
    private IEnumerator ConvertHpRoutine()
    {
        while (true)
        {
            _vampiricArmor.OwnerCharacter.TakeDamage(_convertHpRoutineAmount, ItemDataSO.ExtraType.Effect);
            _vampiricArmor.OwnerCharacter.AddArmor(_gainArmorRoutineAmount);

            OnActivate();

            yield return new WaitForSeconds(_convertCooldown);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

}

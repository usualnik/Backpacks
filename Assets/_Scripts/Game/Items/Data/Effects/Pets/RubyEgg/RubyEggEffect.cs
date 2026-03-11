using System;
using UnityEngine;

public class RubyEggEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _empowerBuff;
    [SerializeField] private int _reflectDebuffsAmount = 3;


    //Hatch the egg 
    [SerializeField] private int _rounds = 0;
    [SerializeField] private int _roundsUntillHatchNeeded = 2;

    private ItemBehaviour _eggItem;
    private void Awake()
    {
        _eggItem = GetComponentInParent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _rounds++;

        if (_rounds == _roundsUntillHatchNeeded)
        {
            HatchTheEgg();
        }
    }

    public void StartOfCombatInit(ItemBehaviour target, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_empowerBuff);
        targetCharacter.AddReflectStacks(_reflectDebuffsAmount);

        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void HatchTheEgg()
    {
        if (_eggItem == null) return;

        _eggItem.CombineItemWithIngridient(new ItemBehaviour[]
        {
            
        }, _eggItem.ItemData.Recepies[0].RecipeResult);
    }
}

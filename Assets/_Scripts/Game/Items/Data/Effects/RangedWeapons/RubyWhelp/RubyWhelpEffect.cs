using System;
using UnityEngine;

public class RubyWhelpEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _heatBuff;
    [SerializeField] private int _reflectStacksAmount = 3;

    private WeaponBehaviour _rubyWhelp;

    private void Awake()
    {
        _rubyWhelp = GetComponent<WeaponBehaviour>();
    }     

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_rubyWhelp.OwnerCharacter == null) return;

        _rubyWhelp.OwnerCharacter.ApplyBuff(_heatBuff);

        _rubyWhelp.OwnerCharacter.AddReflectStacks(_reflectStacksAmount);

        OnActivate();
    }
}

using System;

[System.Serializable]
public struct Buff
{
    public string Name;
    public BuffType Type;
    public bool IsPositive;
    public int Value;

    public enum BuffType
    {
        None,

        //Buffs
        Empower,
        Heat,
        Luck,
        Mana,
        Regeneration,
        Thorns,
        Vampirism,

        //Debuffs
        Poison,
        Blindness,
        Cold,
    }


    public string GetDescription() =>
        $"{Name} ({(IsPositive ? "Баф" : "Дебаф")}): {Type} +{Value}";

    public static BuffType[] GetAllBuffTypes() => new BuffType[]
    {
        BuffType.Empower,
        BuffType.Heat,
        BuffType.Luck,
        BuffType.Mana,
        BuffType.Regeneration,
        BuffType.Thorns,
        BuffType.Vampirism
    };

    public static BuffType[] GetAllDebuffTypes() => new BuffType[]
    {
        BuffType.Poison,
        BuffType.Blindness,
        BuffType.Cold
    };

    public static BuffType GetRandomBuffType()
    {
        var buffs = GetAllBuffTypes();
        return buffs[UnityEngine.Random.Range(0, buffs.Length)];
    }

    public static BuffType GetRandomDebuffType()
    {
        var debuffs = GetAllDebuffTypes();
        return debuffs[UnityEngine.Random.Range(0, debuffs.Length)];
    }

    public static Buff CreateRandomBuff(int value)
    {
        return new Buff
        {
            Name = "RandomBuff",
            Type = GetRandomBuffType(),
            IsPositive = true,
            Value = value
        };
    }
    public static Buff CreateRandomBuff(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue должен быть меньше maxValue");

        return new Buff
        {
            Name = "RandomBuff",
            Type = GetRandomBuffType(),
            IsPositive = true,
            Value = UnityEngine.Random.Range(minValue, maxValue + 1)
        };
    }

    public static Buff CreateRandomDebuff(int value)
    {
        return new Buff
        {
            Name = "RandomDebuff",
            Type = GetRandomDebuffType(),
            IsPositive = false,
            Value = value
        };
    }

    public static Buff CreateRandomDebuff(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue должен быть меньше maxValue");

        return new Buff
        {
            Name = "RandomDebuff",
            Type = GetRandomDebuffType(),
            IsPositive = false,
            Value = UnityEngine.Random.Range(minValue, maxValue + 1)
        };
    }
}
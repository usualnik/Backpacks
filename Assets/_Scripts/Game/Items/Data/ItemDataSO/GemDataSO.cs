using UnityEngine;

[CreateAssetMenu(fileName = "GemDataSO", menuName = "Items/Gem Data")]
public class GemDataSO : ItemDataSO
{
	public enum GemRarity
	{
        Chipped,
        Flawed,
        Regular,
        Flawless,
        Perfect
    }

    [SerializeField]private GemRarity _gemRarity;
}

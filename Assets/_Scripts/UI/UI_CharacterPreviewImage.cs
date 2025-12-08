using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterPreviewImage : MonoBehaviour
{
    private enum ImageOwner
    {
        Player,
        Enemy
    }

    [SerializeField] private ImageOwner imageOwner;

    private Image _characterPreviewImage;
   
    private void Awake()
    {
        _characterPreviewImage = GetComponent<Image>();
    }
    private void Start()
    {
        InitPlayerPreviewImage();

        if (imageOwner == ImageOwner.Player)
        {
            PlayerCharacter.Instance.OnPlayerClassChanged += PlayerCharacter_OnPlayerClassChanged;
        }
        else
        {
            EnemyCharacter.Instance.OnEnemyGenerated += EnemyCharacter_OnEnemyClassChanged;
        }
    }
    private void OnDestroy()
    {
        if (imageOwner == ImageOwner.Player)
        {
            PlayerCharacter.Instance.OnPlayerClassChanged -= PlayerCharacter_OnPlayerClassChanged;
        }
        else
        {
            EnemyCharacter.Instance.OnEnemyGenerated -= EnemyCharacter_OnEnemyClassChanged;

        }
    }


    //__________________ENEMY_________________________________
    private void EnemyCharacter_OnEnemyClassChanged(ClassDataSO generatedEnemy)
    {
        UpdatePreviewImage(generatedEnemy.ClassSprite);
    }


    //___________________Player________________________________
    private void InitPlayerPreviewImage()
    {
        if (PlayerCharacter.Instance.ClassData.ClassSprite != null)
        {
            _characterPreviewImage.sprite = PlayerCharacter.Instance.ClassData.ClassSprite;
        }
    }
    private void PlayerCharacter_OnPlayerClassChanged(ClassDataSO updatedPlayerClass)
    {
        UpdatePreviewImage(updatedPlayerClass.ClassSprite);
    }


    //_____________________Common________________________________
    private void UpdatePreviewImage(Sprite newPlayerSprite)
    {
        _characterPreviewImage.sprite = newPlayerSprite;
    }
}

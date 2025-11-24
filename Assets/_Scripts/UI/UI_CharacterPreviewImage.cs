using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterPreviewImage : MonoBehaviour
{
    private Image _characterPreviewImage;
   
    private void Awake()
    {
        _characterPreviewImage = GetComponent<Image>();
    }
    private void Start()
    {
        InitPlayerPreviewImage();
        PlayerCharacter.Instance.OnPlayerClassChanged += PlayerCharacter_OnPlayerClassChanged;
    }
    private void OnDestroy()
    {
        PlayerCharacter.Instance.OnPlayerClassChanged -= PlayerCharacter_OnPlayerClassChanged;
    }

    private void InitPlayerPreviewImage()
    {
        if (PlayerCharacter.Instance.ClassData.ClassSprite != null)
        {
            _characterPreviewImage.sprite = PlayerCharacter.Instance.ClassData.ClassSprite;
        }
    }

    private void PlayerCharacter_OnPlayerClassChanged(ClassDataSO updatedPlayerClass)
    {
        UpdatePlayerPreviewImage(updatedPlayerClass.ClassSprite);
    }

    private void UpdatePlayerPreviewImage(Sprite newPlayerSprite)
    {
        _characterPreviewImage.sprite = newPlayerSprite;
    }
}

using UnityEngine;


[System.Serializable]
public class SavedData
{
    //Player stats
    [SerializeField]
    private PlayerCharacter.CharacterStats playerStats;
    
    //Game manager 
        //Tries
        //Trophies
        //Round

    //Inventory
        //Items in inventory
        //Opened recepies

    //Settings

    //Character customization
}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    
    [SerializeField] private SavedData _savedData;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.transform.SetParent(null, false);
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
      
    }

    private void InitFirstTimePlayingData()
    {
        
    }

    private void LoadPlayerData()
    {
       
    }
    private void SavePlayerData()
    {
        
    }

    #region Get
    
    #endregion

    #region Set

    #endregion

}

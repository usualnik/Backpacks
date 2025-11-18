using System.Collections.Generic;
using UnityEngine;

public class BoxOfReachesEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private GameObject[] _amethysts;
    [SerializeField] private GameObject[] _emeralds;
    [SerializeField] private GameObject[] _rubys;
    [SerializeField] private GameObject[] _sapphires;
    [SerializeField] private GameObject[] _topazes;

    private Transform _spawnGemTransform;
    private Vector3 _spawnGemOffset = new Vector3(0, 100,0);


    private ItemBehaviour _itemBehaviour;
    private Storage storage;

    [SerializeField]
    private bool _canSpawngems = false;

    private void Awake()
    {
        _itemBehaviour = GetComponent<ItemBehaviour>();
    }
    private void Start()
    {
        _itemBehaviour.OnItemStateChanged += ItemBehaviour_OnItemStateChanged;
        WindowManager.Instance.OnWindowOpened += WindowManager_OnWindowOpened;
        _spawnGemTransform = Storage.Instance.transform;
    }

    private void WindowManager_OnWindowOpened(WindowManager.WindowType openedWindowType)
    {
        if (openedWindowType == WindowManager.WindowType.Store)
        {
            ShopEntered();
        }
    }

    private void OnDestroy()
    {
        _itemBehaviour.OnItemStateChanged -= ItemBehaviour_OnItemStateChanged;
        WindowManager.Instance.OnWindowOpened -= WindowManager_OnWindowOpened;

    }
    private void ItemBehaviour_OnItemStateChanged(ItemBehaviour.ItemState prevState, ItemBehaviour.ItemState currentState)
    {
        if (currentState.HasFlag(ItemBehaviour.ItemState.Inventory))
        {
            _canSpawngems = true;
        }
        else
        {
            _canSpawngems = false;
        }

    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

    private void ShopEntered()
    {
        SpawnGems();
    }

    private void SpawnGems()
    {
        if (_canSpawngems)
        {
            List<GameObject> _allSpawnebleGems = new List<GameObject>();

            _allSpawnebleGems.Add(_amethysts[0]);
            _allSpawnebleGems.Add(_emeralds[0]);
            _allSpawnebleGems.Add(_rubys[0]);
            _allSpawnebleGems.Add(_sapphires[0]);
            _allSpawnebleGems.Add(_topazes[0]);


           int randomGemIndex = Random.Range(0,_allSpawnebleGems.Count);
            
            GameObject spawnedGem = Instantiate(
                _allSpawnebleGems[randomGemIndex],
                _spawnGemTransform.position + _spawnGemOffset,
                Quaternion.identity);

            spawnedGem.transform.SetParent(transform, true);  
        }
    }
}

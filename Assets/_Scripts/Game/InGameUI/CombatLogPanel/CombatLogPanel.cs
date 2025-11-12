using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatLogPanel : MonoBehaviour, IDragHandler
{
    public static CombatLogPanel Instance {  get; private set; }

    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _logMessage;
    [SerializeField] private GameObject _content;

    private Color _playerColor = Color.green;
    private Color _enemyColor = Color.red;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of combat log panel");
    }

    private void Start()
    {
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnBuffApplied += CombatManager_OnBuffApplied;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnBuffApplied -= CombatManager_OnBuffApplied;


    }
    private void CombatManager_OnDamageDealt(ItemDataSO attackWeapon, string targetName)
    {
       LogDamage(attackWeapon.ItemName, targetName);
    }
    private void CombatManager_OnBuffApplied(Buff effect, string targetName)
    {
        LogEffect(effect.Name, targetName);
    }

    public void ShowPanel()
    {
        _mainPanel.SetActive(!_mainPanel.activeInHierarchy);
    }


    private void LogDamage(string weaponName, string targetName)
    {

        string combatTimeText = "[" + TimeControlPanel.Instance.GetTimePassed().ToString("F1") + "]";


        GameObject newDamageLog = Instantiate(_logMessage, _content.transform.position, Quaternion.identity);
        newDamageLog.transform.SetParent(_content.transform, false);

        TextMeshProUGUI _damageText = newDamageLog.GetComponent<TextMeshProUGUI>();
        _damageText.text = combatTimeText + " Damage Dealt " + "(" + weaponName + ")";

        _damageText.color = targetName == "Player" ? _playerColor : _enemyColor;
    }
    private void LogEffect(string effectName, string targetName)
    {
        string combatTimeText = "[" + TimeControlPanel.Instance.GetTimePassed().ToString("F1") + "]";

        GameObject newEffectLog = Instantiate(_logMessage, _content.transform.position, Quaternion.identity);
        newEffectLog.transform.SetParent(_content.transform, false);

        TextMeshProUGUI _effectText = newEffectLog.GetComponent<TextMeshProUGUI>();

        _effectText.text = combatTimeText + " AffectApplied " + "(" + effectName + ")";

        _effectText.color = targetName == "Player" ? _playerColor : _enemyColor;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
}

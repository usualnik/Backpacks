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
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

        PlayerCharacter.Instance.OnNewBuffApplied += PlayerCharacter_OnNewBuffApplied;
        EnemyCharacter.Instance.OnNewBuffApplied += EnemyCharacter_OnNewBuffApplied;
    }



    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        PlayerCharacter.Instance.OnNewBuffApplied -= PlayerCharacter_OnNewBuffApplied;
        EnemyCharacter.Instance.OnNewBuffApplied -= EnemyCharacter_OnNewBuffApplied;
    }

    private void EnemyCharacter_OnNewBuffApplied(Buff buff)
    {
        LogEffect(EnemyCharacter.Instance, buff);
    }

    private void PlayerCharacter_OnNewBuffApplied(Buff buff)
    {
        LogEffect(PlayerCharacter.Instance, buff);
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        HidePanelAfterCombat();
    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour attackWeapon, Character vitimCharacter, float damage)
    {
       LogDamage(attackWeapon.ItemData.ItemName, vitimCharacter, damage);
    }

    public void ShowPanel()
    {
        _mainPanel.SetActive(!_mainPanel.activeInHierarchy);
    }

    private void HidePanelAfterCombat()
    {
        LogText[] logs = GetComponentsInChildren<LogText>(true);

        foreach (LogText log in logs)
        {
            Destroy(log.gameObject);
        }

        logs = null;

        _mainPanel.SetActive(false);
    }

    private void LogDamage(string weaponName, Character victimCharacter, float damage)
    {

        string combatTimeText = "[" + TimeControlPanel.Instance.GetTimePassed().ToString("F1") + "]";


        GameObject newDamageLog = Instantiate(_logMessage, _content.transform.position, Quaternion.identity);
        newDamageLog.transform.SetParent(_content.transform, false);

        TextMeshProUGUI _damageText = newDamageLog.GetComponent<TextMeshProUGUI>();

        _damageText.text = string.Format($"{combatTimeText} <color=white><b>{Mathf.RoundToInt(damage)}</b></color> " +
            $"Damage Dealt by <i>({weaponName})</i>");

        _damageText.color = victimCharacter is PlayerCharacter ? _enemyColor : _playerColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    private void LogEffect(Character targetCharacter, Buff buff)
    {
        string combatTimeText = "[" + TimeControlPanel.Instance.GetTimePassed().ToString("F1") + "]";

        GameObject newDamageLog = Instantiate(_logMessage, _content.transform.position, Quaternion.identity);
        newDamageLog.transform.SetParent(_content.transform, false);

        TextMeshProUGUI _damageText = newDamageLog.GetComponent<TextMeshProUGUI>();

        string appliedText = buff.IsPositive ? "Applied" : "Inflicted";

        _damageText.text = string.Format($"{combatTimeText} <color=White><b>{buff.Value} stack</b></color> of <color=White>{buff.Type}</color> " +
           $"{appliedText} on <i>({targetCharacter.NickName})</i>");

        _damageText.color = targetCharacter is PlayerCharacter ? _playerColor : _enemyColor;


    }

}

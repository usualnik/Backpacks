using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchClassButton : MonoBehaviour
{
    public static event Action OnSwitchClassButtonPressed;

    private Button _switchClassButton;

    private void Awake()
    {
        _switchClassButton = GetComponent<Button>();

        if (_switchClassButton == null)
        {         
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        _switchClassButton.onClick.AddListener(Pressed);
    }

    private void OnDestroy()
    {
        if (_switchClassButton != null)
            _switchClassButton.onClick.RemoveListener(Pressed);
    }

    private void Pressed()
    {
        OnSwitchClassButtonPressed?.Invoke();
    }
}
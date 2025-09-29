using UnityEngine;

    public class WindowManager : MonoBehaviour
    {
        public static WindowManager Instance { get; private set; }

        [Header("Menu windows refs")]
        [SerializeField] private CanvasGroup[] _windowCanvasGroups;

      
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one instance of WindowManager");
            }
        }

        private void Start()
        {          
            //Open main menu
            OpenWindow(0);
        }

        public void OpenWindow(int index)
        {                     
            if (index < 0 || index >= _windowCanvasGroups.Length)
            {
                Debug.LogError($"Window index {index} is out of range!");
                return;
            }

            foreach (var canvasGroup in _windowCanvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
          
            _windowCanvasGroups[index].alpha = 1;
            _windowCanvasGroups[index].interactable = true;
            _windowCanvasGroups[index].blocksRaycasts = true;
        }

        public CanvasGroup GetWindowByIndex(int index)
        {
            if (index >= 0 && index < _windowCanvasGroups.Length)
                return _windowCanvasGroups[index];
            return null;
        }    
    }

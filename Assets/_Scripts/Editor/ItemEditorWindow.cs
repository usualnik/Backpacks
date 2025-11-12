//using UnityEngine;
//using UnityEditor;
//using System;

//public class ItemEditorWindow : EditorWindow
//{
//    private ItemDataSO currentItem;
//    private Vector2 scrollPosition;
//    private bool[,] shapeGrid;
//    private int gridWidth = 1;
//    private int gridHeight = 1;

//    private SerializedObject serializedObject;
//    private SerializedProperty effectsProperty;

//    [MenuItem("Tools/Item Data Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<ItemEditorWindow>("Item Data Editor");
//    }

//    private void OnEnable()
//    {
//        if (currentItem != null)
//        {
//            InitializeSerializedObject();
//        }
//    }

//    private void InitializeSerializedObject()
//    {
//        serializedObject = new SerializedObject(currentItem);
//        effectsProperty = serializedObject.FindProperty("effects");
//        InitializeShapeGrid();
//    }

//    private void OnGUI()
//    {
//        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//        DrawItemSelection();

//        if (currentItem != null)
//        {
//            if (serializedObject == null)
//                InitializeSerializedObject();

//            serializedObject.Update();

//            DrawBasicInfo();
//            DrawShapeEditor();
//            DrawEffectsList();
//            DrawProperties();

//            serializedObject.ApplyModifiedProperties();

//            DrawSaveButtons();
//        }

//        EditorGUILayout.EndScrollView();
//    }

//    private void DrawItemSelection()
//    {
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Item Selection", EditorStyles.boldLabel);

//        EditorGUILayout.BeginHorizontal();

//        ItemDataSO newItem = (ItemDataSO)EditorGUILayout.ObjectField("Current Item", currentItem, typeof(ItemDataSO), false);
//        if (newItem != currentItem)
//        {
//            currentItem = newItem;
//            if (currentItem != null)
//                InitializeSerializedObject();
//        }

//        if (GUILayout.Button("Create New", GUILayout.Width(100)))
//        {
//            CreateNewItem();
//        }

//        EditorGUILayout.EndHorizontal();

//        EditorGUILayout.Space();
//    }

//    private void DrawBasicInfo()
//    {
//        EditorGUILayout.LabelField("Basic Information", EditorStyles.boldLabel);

//        SerializedProperty nameProperty = serializedObject.FindProperty("itemName");
//        SerializedProperty iconProperty = serializedObject.FindProperty("icon");
//        SerializedProperty priceProperty = serializedObject.FindProperty("price");

//        EditorGUILayout.PropertyField(nameProperty);
//        EditorGUILayout.PropertyField(iconProperty);
//        EditorGUILayout.PropertyField(priceProperty);

//        EditorGUILayout.Space();
//    }

//    private void DrawShapeEditor()
//    {
//        EditorGUILayout.LabelField("Shape Configuration", EditorStyles.boldLabel);

//        // Controls for grid size
//        EditorGUILayout.BeginHorizontal();
//        int newWidth = EditorGUILayout.IntField("Width", gridWidth);
//        int newHeight = EditorGUILayout.IntField("Height", gridHeight);

//        if ((newWidth != gridWidth || newHeight != gridHeight) && newWidth > 0 && newHeight > 0)
//        {
//            gridWidth = newWidth;
//            gridHeight = newHeight;
//            ResizeShapeGrid(gridWidth, gridHeight);
//        }

//        EditorGUILayout.EndHorizontal();

//        // Shape grid editor
//        EditorGUILayout.LabelField("Click cells to toggle shape:");

//        if (shapeGrid != null)
//        {
//            for (int y = gridHeight - 1; y >= 0; y--)
//            {
//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField($"", GUILayout.Width(20));

//                for (int x = 0; x < gridWidth; x++)
//                {
//                    bool currentValue = shapeGrid[x, y];
//                    GUIStyle buttonStyle = currentValue ?
//                        new GUIStyle(GUI.skin.button) { normal = { background = Texture2D.whiteTexture } } :
//                        GUI.skin.button;

//                    if (GUILayout.Toggle(currentValue, "", buttonStyle, GUILayout.Width(25), GUILayout.Height(25)))
//                    {
//                        if (!currentValue)
//                        {
//                            shapeGrid[x, y] = true;
//                            SaveShapeToItem();
//                        }
//                    }
//                    else
//                    {
//                        if (currentValue)
//                        {
//                            shapeGrid[x, y] = false;
//                            SaveShapeToItem();
//                        }
//                    }
//                }
//                EditorGUILayout.EndHorizontal();
//            }
//        }

//        // Preview
//        DrawShapePreview();

//        EditorGUILayout.Space();
//    }

//    private void DrawShapePreview()
//    {
//        EditorGUILayout.LabelField("Preview:", EditorStyles.miniLabel);
//        Rect previewRect = GUILayoutUtility.GetRect(100, 60);
//        EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f));

//        if (shapeGrid != null && gridWidth > 0 && gridHeight > 0)
//        {
//            float cellSize = Mathf.Min(previewRect.width / gridWidth, previewRect.height / gridHeight);
//            float startX = previewRect.x + (previewRect.width - (gridWidth * cellSize)) * 0.5f;
//            float startY = previewRect.y + (previewRect.height - (gridHeight * cellSize)) * 0.5f;

//            for (int x = 0; x < gridWidth; x++)
//            {
//                for (int y = 0; y < gridHeight; y++)
//                {
//                    if (shapeGrid[x, y])
//                    {
//                        Rect cellRect = new Rect(
//                            startX + x * cellSize,
//                            startY + y * cellSize,
//                            cellSize - 1,
//                            cellSize - 1
//                        );
//                        EditorGUI.DrawRect(cellRect, Color.green);
//                    }
//                }
//            }
//        }
//    }

//    private void DrawEffectsList()
//    {
//        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

//        if (effectsProperty != null)
//        {
//            EditorGUILayout.PropertyField(effectsProperty, true);

//            EditorGUILayout.BeginHorizontal();
//            if (GUILayout.Button("Add Effect"))
//            {
//                ShowEffectCreationMenu();
//            }
//            if (GUILayout.Button("Clear All"))
//            {
//                effectsProperty.ClearArray();
//            }
//            EditorGUILayout.EndHorizontal();
//        }

//        EditorGUILayout.Space();
//    }

//    private void DrawProperties()
//    {
//        EditorGUILayout.LabelField("Item Properties", EditorStyles.boldLabel);

//        // Здесь можно добавить дополнительные свойства
//        // Например: weight, rarity, etc.

//        EditorGUILayout.Space();
//    }

//    private void DrawSaveButtons()
//    {
//        EditorGUILayout.BeginHorizontal();

//        if (GUILayout.Button("Save Item"))
//        {
//            SaveItem();
//        }

//        if (GUILayout.Button("Save As New..."))
//        {
//            SaveItemAsNew();
//        }

//        if (GUILayout.Button("Revert"))
//        {
//            InitializeShapeGrid();
//        }

//        EditorGUILayout.EndHorizontal();
//    }

//    private void CreateNewItem()
//    {
//        string path = EditorUtility.SaveFilePanelInProject(
//            "Create New Item",
//            "NewItem",
//            "asset",
//            "Select where to save the new item"
//        );

//        if (!string.IsNullOrEmpty(path))
//        {
//            ItemDataSO newItem = CreateInstance<ItemDataSO>();
//            AssetDatabase.CreateAsset(newItem, path);

//            currentItem = newItem;
//            InitializeSerializedObject();

//            // Set default shape
//            shapeGrid = new bool[1, 1] { { true } };
//            SaveShapeToItem();

//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();

//            Debug.Log($"Created new item: {path}");
//        }
//    }

//    private void InitializeShapeGrid()
//    {
//        if (currentItem != null)
//        {
//            bool[,] itemShape = currentItem.GetShape();
//            gridWidth = currentItem.GetShapeSize().x;
//            gridHeight = currentItem.GetShapeSize().y;
//            shapeGrid = itemShape;
//        }
//        else
//        {
//            gridWidth = 1;
//            gridHeight = 1;
//            shapeGrid = new bool[1, 1] { { true } };
//        }
//    }

//    private void ResizeShapeGrid(int newWidth, int newHeight)
//    {
//        bool[,] newGrid = new bool[newWidth, newHeight];

//        // Copy existing data
//        if (shapeGrid != null)
//        {
//            int copyWidth = Mathf.Min(newWidth, shapeGrid.GetLength(0));
//            int copyHeight = Mathf.Min(newHeight, shapeGrid.GetLength(1));

//            for (int x = 0; x < copyWidth; x++)
//            {
//                for (int y = 0; y < copyHeight; y++)
//                {
//                    newGrid[x, y] = shapeGrid[x, y];
//                }
//            }
//        }

//        shapeGrid = newGrid;
//        SaveShapeToItem();
//    }

//    private void SaveShapeToItem()
//    {
//        if (currentItem != null && shapeGrid != null)
//        {
//            currentItem.SetShape(shapeGrid);
//            EditorUtility.SetDirty(currentItem);
//        }
//    }

//    private void ShowEffectCreationMenu()
//    {
//        GenericMenu menu = new GenericMenu();

//        // Добавляем все доступные типы эффектов
//        //menu.AddItem(new GUIContent("Health Effect"), false, () => CreateAndAddEffect<HealthEffectSO>());
//        // Добавь другие эффекты здесь

//        menu.ShowAsContext();
//    }

//    private void CreateAndAddEffect<T>() where T : Buff
//    {
//        string path = EditorUtility.SaveFilePanelInProject(
//            "Create Effect",
//            $"New{typeof(T).Name}",
//            "asset",
//            "Save the effect asset"
//        );

//        if (!string.IsNullOrEmpty(path))
//        {
//            T effect = CreateInstance<T>();
//            AssetDatabase.CreateAsset(effect, path);

//            // Добавляем эффект в массив
//            effectsProperty.arraySize++;
//            effectsProperty.GetArrayElementAtIndex(effectsProperty.arraySize - 1).objectReferenceValue = effect;

//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }
//    }

//    private void SaveItem()
//    {
//        if (currentItem != null)
//        {
//            EditorUtility.SetDirty(currentItem);
//            AssetDatabase.SaveAssets();
//            Debug.Log($"Item '{currentItem.ItemName}' saved!");
//        }
//    }

//    private void SaveItemAsNew()
//    {
//        if (currentItem != null)
//        {
//            string path = EditorUtility.SaveFilePanelInProject(
//                "Save Item As",
//                currentItem.ItemName,
//                "asset",
//                "Save the item as new asset"
//            );

//            if (!string.IsNullOrEmpty(path))
//            {
//                ItemDataSO newItem = Instantiate(currentItem);
//                AssetDatabase.CreateAsset(newItem, path);
//                AssetDatabase.SaveAssets();
//                AssetDatabase.Refresh();

//                currentItem = newItem;
//                InitializeSerializedObject();

//                Debug.Log($"Item saved as: {path}");
//            }
//        }
//    }
//}
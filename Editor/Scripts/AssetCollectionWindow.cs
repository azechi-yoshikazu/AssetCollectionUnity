using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetCollectionWindow : EditorWindow
{
    private DragAndDropManipulator _dragAndDropManipulator;

    private ListView _assetList;
    private List<string> _assetListSource = new();

    [MenuItem("Window/AssetCollection")]
    public static void ShowWindow()
    {
        GetWindow<AssetCollectionWindow>("AssetCollection");
    }

    private void OnEnable()
    {
        var tree = Resources.Load<VisualTreeAsset>("AssetCollection");

        tree.CloneTree(rootVisualElement);

        var dropArea = rootVisualElement.Q<VisualElement>("DropArea");
        _dragAndDropManipulator = new(dropArea);
        _dragAndDropManipulator.DroppedAction += OnDropped;

        var assetList = rootVisualElement.Q<ListView>("AssetList");
        assetList.makeItem = () => new Label();
        assetList.bindItem = (element, index) => (element as Label).text = _assetListSource[index];
        assetList.itemsSource = _assetListSource;
        assetList.itemsSourceChanged += () => Debug.Log("itemsSourceChanged");
        assetList.itemsChosen += OnItemChosen;

        _assetList = assetList;
    }

    private void OnItemChosen(IEnumerable<object> itemObject)
    {
        var path = itemObject.First() as string;
        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        EditorGUIUtility.PingObject(obj);
    }

    private void OnDropped()
    {
        Debug.Log("OnDropped");
        foreach(var path in DragAndDrop.paths)
        {
            if(!_assetListSource.Contains(path))
            {
                _assetListSource.Add(path);
            }
        }
        _assetListSource.Sort();

        _assetList.Rebuild();
    }

    private void OnDisable()
    {
        _dragAndDropManipulator.Dispose();
    }
}

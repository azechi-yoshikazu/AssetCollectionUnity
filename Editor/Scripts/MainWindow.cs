using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AssetCollection.Models;

namespace AssetCollection
{
    public class MainWindow : EditorWindow
    {
        private const string _prefsKey = "asset_collection_key";
        private DragAndDropManipulator _dragAndDropManipulator;

        private CollectionList _collections = null;
        private Collection _selectedCollection = null;

        private ListView _assetList;

        [MenuItem("Window/AssetCollection")]
        public static void ShowWindow()
        {
            GetWindow<MainWindow>("AssetCollection");
        }

        private void OnEnable()
        {
            Load();

            var tree = Resources.Load<VisualTreeAsset>("MainWindow");

            tree.CloneTree(rootVisualElement);

            // Collection
            var collectionList = rootVisualElement.Q<ListView>("CollectionList");
            collectionList.makeItem = () => new Label();
            collectionList.bindItem = (element, index) =>
            {
                var collection = collectionList.itemsSource[index] as Collection;
                (element as Label).text = collection.Name;
            };
            collectionList.itemsSource = _collections.Collections;
            collectionList.selectedIndicesChanged += OnCollectionSelected;

            var addCollectionButton = rootVisualElement.Q<Button>("ButtonAddCollection");
            addCollectionButton.clicked += () =>
            {
                _collections.AddNewCollection();
                collectionList.Rebuild();

                Save();
            };

            // Assets
            var dropArea = rootVisualElement.Q<VisualElement>("DropArea");
            _dragAndDropManipulator = new(dropArea);
            _dragAndDropManipulator.DroppedAction += OnDropped;

            var assetList = rootVisualElement.Q<ListView>("AssetList");
            assetList.makeItem = () => new Label();
            assetList.bindItem = (element, index) =>
            {
                var name = assetList.itemsSource[index] as string;
                (element as Label).text = name;
            };
            assetList.itemsChosen += OnItemChosen;

            _assetList = assetList;
        }

        private void OnCollectionSelected(IEnumerable<int> selectedIndices)
        {
            if(selectedIndices.Count() == 0)
            {
                _selectedCollection = null;
            }
            else
            {
                _selectedCollection = _collections.FindBy(selectedIndices.First());
            }
            Debug.Log($"Selected: {_selectedCollection?.Name} {_selectedCollection?.Assets?.Count}");
            _assetList.itemsSource = _selectedCollection?.Assets;
            _assetList.Rebuild();
        }

        private void OnItemChosen(IEnumerable<object> itemObject)
        {
            var path = itemObject.First() as string;

            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

            if(AssetDatabase.IsMainAsset(obj))
            {
                EditorGUIUtility.PingObject(obj);
            }
        }

        private void OnDropped()
        {
            if(_selectedCollection != null)
            {
                foreach (var path in DragAndDrop.paths)
                {
                    if (!_selectedCollection.Assets.Contains(path))
                    {
                        _selectedCollection.Assets.Add(path);
                    }
                }
                _selectedCollection.Assets.Sort();

                _assetList.Rebuild();

                Save();
            }
        }

        private void OnDisable()
        {
            _dragAndDropManipulator.Dispose();
        }

        private void Save()
        {
            EditorPrefs.SetString(_prefsKey, JsonUtility.ToJson(_collections, false));
        }

        private void Load()
        {
            var json = EditorPrefs.GetString(_prefsKey, null);
            _collections = JsonUtility.FromJson<CollectionList>(json);
            if(_collections == null)
            {
                _collections = new();
            }
        }
    }

};

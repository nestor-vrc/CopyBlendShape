using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CopyBlendShape : EditorWindow
{
    private ObjectField _sourceObjectField;
    private ObjectField _targetObjectField;
    private Button _copyButton;
    private VisualElement _root;

    [MenuItem("Nestor/Tools/Copy BlendShape")]
    public static void ShowWindow()
    {
        var window = GetWindow<CopyBlendShape>("Copy BlendShape");
        window.minSize = new Vector2(300, 260);
        window.maxSize = new Vector2(300, 260);
    }

    private void CreateGUI()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("CopyBlendShapeWindow");
        _root = visualTree.CloneTree();
        rootVisualElement.Add(_root);

        var styleSheet = Resources.Load<StyleSheet>("CopyBlendShapeWindow");
        rootVisualElement.styleSheets.Add(styleSheet);

        _sourceObjectField = _root.Q<ObjectField>("sourceObject");
        _sourceObjectField.objectType = typeof(GameObject);
        _sourceObjectField.RegisterValueChangedCallback(_ => ValidateFields());

        _targetObjectField = _root.Q<ObjectField>("targetObject");
        _targetObjectField.objectType = typeof(GameObject);
        _targetObjectField.RegisterValueChangedCallback(_ => ValidateFields());

        _copyButton = _root.Q<Button>("copyButton");
        _copyButton.clicked += OnCopyButtonClicked;

        ValidateFields();
    }

    private void ValidateFields()
    {
        var isValid = _sourceObjectField.value != null && _targetObjectField.value != null;

        _copyButton.SetEnabled(isValid);
        _copyButton.style.opacity = isValid ? 1 : 0.5f;
    }

    private void OnCopyButtonClicked()
    {
        var sourceObject = _sourceObjectField.value as GameObject;
        var targetObject = _targetObjectField.value as GameObject;

        if (sourceObject == null || targetObject == null)
        {
            Debug.LogError("Source or Target GameObject is not set.");
            return;
        }

        if (sourceObject == targetObject)
        {
            Debug.LogError("Source and Target GameObjects cannot be the same.");
            return;
        }

        CopyBlendShapes(sourceObject, targetObject);
        Debug.Log($"BlendShapes copied from {sourceObject.name} to {targetObject.name}");
    }

    private static void CopyBlendShapes(GameObject source, GameObject target)
    {
        var sourceRenderer = source.GetComponent<SkinnedMeshRenderer>();
        var targetRenderer = target.GetComponent<SkinnedMeshRenderer>();

        if (sourceRenderer == null || targetRenderer == null)
        {
            Debug.LogError("Source or target GameObject does not have SkinnedMeshRenderer component.");
            return;
        }

        var blendShapeCount = sourceRenderer.sharedMesh.blendShapeCount;
        for (var i = 0; i < blendShapeCount; i++)
        {
            var blendShapeValue = sourceRenderer.GetBlendShapeWeight(i);
            targetRenderer.SetBlendShapeWeight(i, blendShapeValue);
        }
    }
}

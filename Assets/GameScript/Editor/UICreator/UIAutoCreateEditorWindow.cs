using System.IO;
using UnityEngine;
using UnityEditor;
public class UIAutoCreateEditorWindow : EditorWindow
{
    private string newUIName;
    private GameObject uiRootGo;
    
    [MenuItem("自定义工具/UI绑定生成工具", false, 10)]
    static void ShowEditor()
    {
        UIAutoCreateEditorWindow window = GetWindow<UIAutoCreateEditorWindow>();
        window.minSize = new Vector2(400, 250);
        window.maxSize = new Vector2(400, 250);
        window.titleContent.text = "UI绑定生成工具";
    }

    private void OnGUI()
    {//创建窗口
        #region GUIStyle设置

        Color fontColor = Color.white;
        GUIStyle titleStyle = new GUIStyle() { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = fontColor;

        GUIStyle sonTittleStyle = new GUIStyle() { fontSize = 15, alignment = TextAnchor.MiddleCenter };
        sonTittleStyle.normal.textColor = fontColor;

        GUIStyle leftStyle = new GUIStyle() { fontSize = 15, alignment = TextAnchor.MiddleLeft };
        leftStyle.normal.textColor = fontColor;

        GUIStyle littoleStyle = new GUIStyle() { fontSize = 13, alignment = TextAnchor.MiddleCenter };
        littoleStyle.normal.textColor = fontColor;

        #endregion

        GUILayout.BeginArea(new Rect(0, 0, 400, 600));
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("注意事项:\n" +
                                 "新UI的名字需要以UI作为前缀\n" +
                                 "例如: UITest\n" +
                                 "UI组件命名规范请查看 UIViewAutoCreateConfig 文件", leftStyle, GUILayout.Width(600));
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUI.skin.button.wordWrap = true;

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("代码自动生成设置",leftStyle,GUILayout.Width(600));
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Prefab根节点",leftStyle,GUILayout.Width(100));
        uiRootGo = (GameObject)EditorGUILayout.ObjectField(uiRootGo, typeof(GameObject), true);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("生成", GUILayout.Width(150), GUILayout.Height(30)))
        {
            Generator(uiRootGo);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    private void Generator(GameObject go)
    {
        var config = AssetDatabase.LoadAssetAtPath<UIAutoCreateInfoConfig>("Assets/Editor/UIAutoCreateInfoConfig.asset");
        CreatePrefab(go,config);
        CreateUIClass(config);
    }
    
    private void CreatePrefab(GameObject gameObject,UIAutoCreateInfoConfig config)
    {
        //检查目录
        if (!Directory.Exists(config.PrefabPath))
        {
            throw new System.Exception($"路径{config.PrefabPath} 不存在");
        }

        var uiName = GetUIName();
        var localPath = string.Format("{0}/{1}.prefab", config.PrefabPath, uiName);
        //确保prefab唯一
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        //创建一个prefab 并且输出日志
        bool prefabSuccess;
        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (prefabSuccess)
            Debug.Log($"{uiName}.prefab创建成功");
        else
            Debug.Log($"{uiName}.prefab创建失败");
        AssetDatabase.Refresh();
    }
    
    private void CreateUIClass(UIAutoCreateInfoConfig config)
    {
        if (uiRootGo == null) throw new System.Exception("请拖入需要生成的预制体节点");
        string uiName = GetUIName();

        
        var targetPath = config.ScriptPath;
        CheckTargetPath(targetPath);
        new UIClassAutoCreate().Create(uiName,uiRootGo,config);
    }

    private string GetUIName()
    {
        string uiName = uiRootGo.name.Replace("UI", "");
        return uiName;
    }
    
    private void CheckTargetPath(string targetPath)
    {
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
    }
    
}

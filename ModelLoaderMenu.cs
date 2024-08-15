using UnityEditor;
using UnityEngine;

public class ModelLoaderMenu : EditorWindow
{
    private string imagePath = "";
    private Texture2D loadedImage;

    [MenuItem("Automação Ambiente/Processar Imagem")]
    public static void ShowWindow()
    {
        ModelLoaderMenu window = GetWindow<ModelLoaderMenu>("Processar Imagem");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(300, 300);
    }

    void OnGUI()
    {
        GUILayout.Label("Carregar e Processar Imagem", EditorStyles.boldLabel);

        if (GUILayout.Button("Selecionar Imagem"))
        {
            imagePath = EditorUtility.OpenFilePanel("Selecione uma imagem", "", "png,jpg,jpeg");
            Debug.Log("imagePath: " + imagePath);
        }

        imagePath = EditorGUILayout.TextField("Caminho da Imagem", imagePath);

        if (GUILayout.Button("Processar"))
        {
            ProcessImage();
        }
    }

    void ProcessImage()
    {
        if (imagePath == null)
        {
            Debug.LogError("Nenhuma imagem carregada para processar.");
            return;
        }

        string modelPath = "Assets/Model/FILTER_F10.fbx";
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        if (model != null)
        {
            GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            if (modelInstance != null)
            {
                modelInstance.transform.position = Vector3.zero;
                modelInstance.transform.rotation = Quaternion.identity;
                Debug.Log("Modelo virtual inserido no ambiente com sucesso.");
            }
            else
            {
                Debug.LogError("Erro ao instanciar o modelo virtual.");
            }
        }
        else
        {
            Debug.LogError("Erro ao carregar o modelo virtual. Verifique o caminho.");
        }
    }
}

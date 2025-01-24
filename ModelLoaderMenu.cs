
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    
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
    
        async void ProcessImage()
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                Debug.LogError("Nenhuma imagem carregada para processar.");
                return;
            }
    
            Vector3 basePosition = Vector3.zero;
            Vector3 spacing = new Vector3(10, 0, 0); 
    
            List<DetectionResult> detectionResults = await SendImageForPrediction(imagePath);
    
            if (detectionResults != null && detectionResults.Count >= 4)
            {
                string modelPath = "Assets/Model/FILTER_F10.fbx";
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
    
                if (model != null)
                {
                    for (int i = 0; i < detectionResults.Count; i++)
                    {
                        GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(model);
                        if (modelInstance != null)
                        {
                            modelInstance.transform.position = basePosition + spacing * i;
                            modelInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
                            Debug.Log("Reator inserido na posição: " + modelInstance.transform.position);
                        }
                        else
                        {
                            Debug.LogError("Erro ao instanciar o modelo virtual.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Erro ao carregar o modelo virtual. Verifique o caminho.");
                }
            }
        }
    
        async Task<List<DetectionResult>> SendImageForPrediction(string imagePath)
        {
            string url = "http://192.168.12.13:5000/predict";
            var httpClient = new HttpClient();
            var content = new StringContent($"{{\"image_path\":\"{imagePath}\"}}", Encoding.UTF8, "application/json");
    
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
    
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                DetectionResultsWrapper wrapper = JsonUtility.FromJson<DetectionResultsWrapper>($"{{\"results\":{jsonResponse}}}");
    
                return wrapper.results;
            }
            else
            {
                Debug.LogError("Erro ao chamar o endpoint Flask: " + response.ReasonPhrase);
                return null;
            }
        }
    }
    
    [System.Serializable]
    public class DetectionResult
    {
        public string ClassName;
        public List<float> BoundingBox;
    }
    
    [System.Serializable]
    public class DetectionResultsWrapper
    {
        public List<DetectionResult> results;
    }

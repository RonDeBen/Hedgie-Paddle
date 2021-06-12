using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestManager : MonoBehaviour
{
    public static RequestManager instance;
    public SpawnWorkflow sw;
    public bool isLocalGame;
    public string url;

    void Awake() {
        if (instance != null) {
            GameObject.Destroy(instance);
        }
        instance = this;
    #if UNITY_EDITOR || UNITY_STANDALONE
        if (isLocalGame) {
            url = "http://localhost:3000";
        }
    #endif
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator PostRequest(string uri, string bodyJsonString) {
        var request = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log("Response: " + request.downloadHandler.text);
    }

    public void PostPoint(bool win, int dimension, int num_inner_balls, int num_moves, float start_entropy) {
        string uri = url + "/points.json";
        string body = "{\"win\":\"" + win + 
                        "\",\"dimension\":\"" + dimension + 
                        "\",\"num_inner_balls\":\"" + num_inner_balls + 
                        "\",\"num_moves\":\"" + num_moves + 
                        "\",\"inner_norm_tend\":\"" + sw.innerTendencies[0] + 
                        "\",\"inner_armor_tend\":\"" + sw.innerTendencies[1] + 
                        "\",\"inner_splitter_tend\":\"" + sw.innerTendencies[2] + 
                        "\",\"outer_norm_tend\":\"" + sw.outerTendencies[0] + 
                        "\",\"outer_armor_tend\":\"" + sw.outerTendencies[1] + 
                        "\",\"outer_splitter_tend\":\"" + sw.outerTendencies[2] + 
                        "\",\"armor_min\":\"" + sw.armorMin + 
                        "\",\"armor_max\":\"" + sw.armorMax + 
                        "\",\"splitter_min\":\"" + sw.splitterMin + 
                        "\",\"splitter_max\":\"" + sw.splitterMax + 
                        "\",\"start_entropy\":\"" + start_entropy + "\"}";
        StartCoroutine(PostRequest(uri, body));
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DatabaseSync : MonoBehaviour
{
    // Dados da tabela JOGADORES e PARTIDAS
    private int playerID;
    private int partidaID;

    [System.Serializable]
    public class EquipeData
    {
        public int jogador;
        public int partida;
        public string nomeEquipe;
        public int vida;
        public int rodada;
    }

    // Envia o estado atual para a tabela EQUIPES
    public void SalvarEquipe(EquipeData data)
    {
        string json = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest("sua-api.com/equipes", json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }
}
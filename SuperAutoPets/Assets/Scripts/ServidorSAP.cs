using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ServidorSAP : MonoBehaviour
{
    private string urlBase = "http://localhost:58747/WSJogador.asmx";

    public TMP_InputField inputNome;
    public TMP_InputField inputUsuario;
    public TMP_InputField inputSenha;


    public void ChamarCadastroPeloBotao()
    {
        string nomeDigitado = inputNome.text;
        string usuarioDigitado = inputUsuario.text;
        string senhaDigitada = inputSenha.text;

        Debug.Log("Enviando dados do formulário...");

        StartCoroutine(CadastrarJogador(nomeDigitado, usuarioDigitado, senhaDigitada));
    }

    IEnumerator CadastrarJogador(string nomeParametro, string usuarioParametro, string senhaParametro)
    {
        string urlCompleta = urlBase + "/CadastrarJogador";

        WWWForm formulario = new WWWForm();
        formulario.AddField("nome", nomeParametro);
        formulario.AddField("usuario", usuarioParametro);
        formulario.AddField("senha", senhaParametro);

        using (UnityWebRequest www = UnityWebRequest.Post(urlCompleta, formulario))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Deu ruim na conexão: " + www.error);
            }
            else
            {
                string respostaLimpa = ExtrairTextoDoXML(www.downloadHandler.text);
                Debug.Log("Resposta do Banco: " + respostaLimpa);
            }
        }
    }

    private string ExtrairTextoDoXML(string xmlBruto)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlBruto);
            return doc.InnerText;
        }
        catch
        {
            return xmlBruto;
        }
    }
}
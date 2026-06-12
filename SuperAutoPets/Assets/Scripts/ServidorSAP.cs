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

    public static int idJogadorLogado = 0;

    public void ChamarCadastroPeloBotao()
    {
        string nomeDigitado = inputNome.text;
        string usuarioDigitado = inputUsuario.text;
        string senhaDigitada = inputSenha.text;

        Debug.Log("Enviando dados de CADASTRO...");
        StartCoroutine(CadastrarJogador(nomeDigitado, usuarioDigitado, senhaDigitada));
    }

    public void ChamarLoginPeloBotao()
    {
        string usuarioDigitado = inputUsuario.text;
        string senhaDigitada = inputSenha.text;

        Debug.Log("Tentando fazer LOGIN...");
        StartCoroutine(LogarJogador(usuarioDigitado, senhaDigitada));
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

    IEnumerator LogarJogador(string usuarioParametro, string senhaParametro)
    {
        string urlCompleta = urlBase + "/LogarJogador";

        WWWForm formulario = new WWWForm();
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

                if (int.TryParse(respostaLimpa, out int idRecebido))
                {
                    idJogadorLogado = idRecebido;
                    Debug.Log("LOGIN APROVADO! O seu ID de jogador agora é: " + idJogadorLogado);
                }
                else
                {
                    Debug.LogWarning("FALHA NO LOGIN: " + respostaLimpa);
                }
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

    public static int idPartidaAtual = 0;

    public void ChamarNovaPartidaPeloBotao()
    {
        if (idJogadorLogado > 0)
        {
            Debug.Log("Iniciando uma nova partida no servidor...");
            StartCoroutine(CriarPartida());
        }
        else
        {
            Debug.LogWarning("Calma lá! Você precisa fazer o Login antes de jogar.");
        }
    }

    IEnumerator CriarPartida()
    {
        string urlCompleta = urlBase + "/CriarPartida";

        WWWForm formulario = new WWWForm();

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

                if (int.TryParse(respostaLimpa, out int idGerado))
                {
                    idPartidaAtual = idGerado;
                    Debug.Log("PARTIDA CRIADA COM SUCESSO! O ID da Partida é: " + idPartidaAtual);
                }
                else
                {
                    Debug.LogWarning("Erro ao criar partida: " + respostaLimpa);
                }
            }
        }
    }

    public TMP_InputField inputNomeEquipe;

    public void ChamarSalvarEquipePeloBotao()
    {
        if (idJogadorLogado > 0 && idPartidaAtual > 0)
        {
            string nomeDaEquipeDigitado = inputNomeEquipe.text;

            int vidaInicial = 5;
            int rodadaInicial = 1;

            Debug.Log("Salvando a equipe '" + nomeDaEquipeDigitado + "' no servidor...");
            StartCoroutine(SalvarEquipe(idJogadorLogado, idPartidaAtual, nomeDaEquipeDigitado, vidaInicial, rodadaInicial));
        }
        else
        {
            Debug.LogWarning("Opa! Você precisa logar e criar uma partida antes de salvar a equipe.");
        }
    }

    IEnumerator SalvarEquipe(int jogador, int partida, string nomeEquipe, int vida, int rodada)
    {
        string urlCompleta = urlBase + "/SalvarEquipe";

        WWWForm formulario = new WWWForm();

        formulario.AddField("jogador", jogador);
        formulario.AddField("partida", partida);
        formulario.AddField("nomeEquipe", nomeEquipe);
        formulario.AddField("vida", vida);
        formulario.AddField("rodada", rodada);

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
}
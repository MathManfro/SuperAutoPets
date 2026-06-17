using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // IMPORTANTE: Para o código conseguir mexer nos textos da UI

public class BattleSimulator : MonoBehaviour
{
    [Header("Painéis de UI (Telas)")]
    public GameObject panelLoja;
    public GameObject panelInimigo;
    public GameObject botaoRoletar;
    public GameObject botaoBatalha;

    [Header("Textos da Barra UI")]
    public TextMeshProUGUI textoVida;
    public TextMeshProUGUI textoRodada;

    [Header("Os Painéis dos Slots")]
    public Transform panelEquipePlayer;
    public Transform panelEquipeInimigo;

    private List<PetInstance> timePlayer = new List<PetInstance>();
    private List<PetInstance> timeInimigo = new List<PetInstance>();

    private Dictionary<PetInstance, int> poderesOriginais = new Dictionary<PetInstance, int>();

    [Header("Status do Jogador")]
    public int vidasAtuais = 5;
    public int rodadaAtual = 1;

    private void Start()
    {
        AtualizarBarraUI();
    }

    public void IniciarBatalha()
    {
        panelLoja.SetActive(false);
        botaoRoletar.SetActive(false);
        botaoBatalha.SetActive(false);

        System.Collections.Generic.List<string> dadosDosMeusPets = new System.Collections.Generic.List<string>();
        foreach (Transform slot in panelEquipePlayer)
        {
            if (slot.childCount > 0)
            {
                PetInstance pet = slot.GetChild(0).GetComponent<PetInstance>();
                dadosDosMeusPets.Add($"{pet.data.cod}-{pet.nivelAtual}-{pet.poderAtual}");
            }
        }
        string minhaFormacaoPronta = string.Join(",", dadosDosMeusPets);
        Debug.Log("Minha formação enviada pra internet: " + minhaFormacaoPronta);

        ServidorSAP.Instance.EntrarNaFilaDeEspera(rodadaAtual, minhaFormacaoPronta, (equipeInimigaRecebida) =>
        {
            panelInimigo.SetActive(true);

            GetComponent<BotGenerator>().GerarEquipeInimiga(equipeInimigaRecebida);

            timePlayer.Clear();
            timeInimigo.Clear();
            poderesOriginais.Clear();

            foreach (Transform slot in panelEquipePlayer)
            {
                if (slot.childCount > 0)
                {
                    PetInstance pet = slot.GetChild(0).GetComponent<PetInstance>();
                    timePlayer.Add(pet);
                    poderesOriginais[pet] = pet.poderAtual;
                }
            }

            foreach (Transform slot in panelEquipeInimigo)
            {
                if (slot.childCount > 0)
                    timeInimigo.Add(slot.GetChild(0).GetComponent<PetInstance>());
            }

            StartCoroutine(RotinaDeCombate());
        });
    }

    private IEnumerator RotinaDeCombate()
    {
        Debug.Log(" A BATALHA VAI COMEÇAR!");

        while (timePlayer.Count > 0 && timeInimigo.Count > 0)
        {
            PetInstance petPlayer = timePlayer[0];
            PetInstance petInimigo = timeInimigo[0];

            yield return new WaitForSeconds(1.2f);

            int danoNoPlayer = petInimigo.poderAtual;
            int danoNoInimigo = petPlayer.poderAtual;

            petPlayer.poderAtual -= danoNoPlayer;
            petInimigo.poderAtual -= danoNoInimigo;

            petPlayer.GetComponent<PetDisplay>().textoPoder.text = Mathf.Max(0, petPlayer.poderAtual).ToString();
            petInimigo.GetComponent<PetDisplay>().textoPoder.text = Mathf.Max(0, petInimigo.poderAtual).ToString();

            yield return new WaitForSeconds(0.8f);

            if (petPlayer.poderAtual <= 0)
            {
                GerenciadorDeHabilidades.ExecutarHabilidade(petPlayer, timePlayer, TipoGatilho.Ao_Desmaiar);
                timePlayer.RemoveAt(0);
                petPlayer.gameObject.SetActive(false);
            }

            if (petInimigo.poderAtual <= 0)
            {
                GerenciadorDeHabilidades.ExecutarHabilidade(petInimigo, timeInimigo, TipoGatilho.Ao_Desmaiar);
                timeInimigo.RemoveAt(0);
                Destroy(petInimigo.gameObject);
            }
        }

        FinalizarRodada();
    }

    private void FinalizarRodada()
    {
        if (timePlayer.Count > 0)
        {
            Debug.Log(" VITÓRIA! Seu time resistiu.");
        }
        else if (timeInimigo.Count > 0)
        {
            Debug.Log(" DERROTA! Você perdeu uma vida.");
            vidasAtuais--;
            if (vidasAtuais < 0) vidasAtuais = 0;
        }
        else
        {
            Debug.Log(" EMPATE! Todo mundo caiu junto.");
        }

        AtualizarBarraUI();

        if (vidasAtuais <= 0)
        {
            Debug.Log(" GAME OVER! Suas vidas acabaram de vez.");

        }
        else
        {

            StartCoroutine(VoltarParaLoja());
        }
    }

    private IEnumerator VoltarParaLoja()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log(" Retornando para a Loja...");

        rodadaAtual++;
        EconomyManager.Instance.ResetarOuro(11);

        foreach (Transform slot in panelEquipeInimigo)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }

        foreach (Transform slot in panelEquipePlayer)
        {
            if (slot.childCount > 0)
            {
                PetInstance pet = slot.GetChild(0).GetComponent<PetInstance>();
                pet.gameObject.SetActive(true);

                if (poderesOriginais.ContainsKey(pet))
                {
                    pet.poderAtual = poderesOriginais[pet];
                }
                pet.GetComponent<PetDisplay>().Setup(pet.data);
            }
        }

        panelInimigo.SetActive(false);
        panelLoja.SetActive(true);
        botaoRoletar.SetActive(true);
        botaoBatalha.SetActive(true);

        AtualizarBarraUI();

        GetComponent<ShopManager>().RoletarLoja();

        if (ServidorSAP.Instance != null)
        {
            ServidorSAP.Instance.ChamarAtualizarStatus(vidasAtuais, rodadaAtual);
        }
    }

    public void AtualizarBarraUI()
    {
        if (textoVida != null) textoVida.text = "Vida: " + vidasAtuais;
        if (textoRodada != null) textoRodada.text = "Rodada: " + rodadaAtual;
    }
}
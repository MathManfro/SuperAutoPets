using UnityEngine;
using System.Collections.Generic;

public class BotGenerator : MonoBehaviour
{
    [Header("Configurações do Bot")]
    public Transform[] slotsInimigos;       // Os 5 espaços do Panel_Inimigo
    public List<EntityData> poolDePets;     // A lista de pets que o bot pode sortear
    public GameObject petPrefab;            // O Prefab universal do pet

    public void GerarEquipeInimiga(int rodadaAtual)
    {
        // 1. Limpa os inimigos da rodada anterior (se houver)
        foreach (Transform slot in slotsInimigos)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        // 2. Define a dificuldade (ex: Rodada 1 = 2 pets, Rodada 2 = 3 pets... até o máximo de 5)
        int quantidadePets = Mathf.Clamp(rodadaAtual + 1, 2, 5);

        // 3. Sorteia e cria os pets
        for (int i = 0; i < quantidadePets; i++)
        {
            if (poolDePets.Count > 0 && i < slotsInimigos.Length)
            {
                // Pega um pet aleatório da lista
                EntityData sorteado = poolDePets[Random.Range(0, poolDePets.Count)];

                // Instancia o pet no slot
                GameObject novoInimigo = Instantiate(petPrefab, slotsInimigos[i]);

                // Zera a posição para encaixar direitinho no UI
                RectTransform rt = novoInimigo.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localPosition = Vector3.zero;
                    rt.localScale = Vector3.one;
                }

                // Injeta os dados do ScriptableObject
                novoInimigo.GetComponent<PetInstance>().Setup(sorteado);

                // TRUQUE DE MESTRE: Desliga o script Draggable do inimigo
                // Assim o jogador não consegue clicar e arrastar o pet do bot!
                Draggable drag = novoInimigo.GetComponent<Draggable>();
                if (drag != null)
                {
                    drag.enabled = false;
                }
            }
        }

        Debug.Log($" Bot gerou um time de {quantidadePets} pets para a Rodada {rodadaAtual}!");
    }
}
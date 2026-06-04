using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Bancos de Dados Locais (Coloque TODOS aqui)")]
    public List<EntityData> poolDePets;
    public List<EntityData> poolDeComidas;

    [Header("Locais na UI")]
    public Transform[] slotsDePets;
    public Transform[] slotsDeComidas;

    [Header("Configurações")]
    public GameObject petPrefab;

    public void RoletarLoja()
    {
        if (EconomyManager.Instance.PodeGastar(1))
        {
            EconomyManager.Instance.GastarOuro(1);

            // 1. Descobre a rodada atual direto do BattleSimulator
            int rodada = GetComponent<BattleSimulator>().rodadaAtual;

            // 2. Calcula o Tier Máximo liberado (Regra clássica do SAP)
            int tierMaximo = 1;
            if (rodada >= 3) tierMaximo = 2;
            if (rodada >= 5) tierMaximo = 3;
            if (rodada >= 7) tierMaximo = 4;
            if (rodada >= 9) tierMaximo = 5;
            if (rodada >= 11) tierMaximo = 6;

            // 3. Filtra as listas para pegar apenas o que o jogador tem nível para ver
            List<EntityData> petsDisponiveis = poolDePets.FindAll(pet => pet.tier <= tierMaximo);
            List<EntityData> comidasDisponiveis = poolDeComidas.FindAll(comida => comida.tier <= tierMaximo);

            // 4. Enche a prateleira de PETS filtrados
            foreach (Transform slot in slotsDePets)
            {
                LimparSlot(slot);
                if (petsDisponiveis.Count > 0)
                {
                    EntityData sorteado = petsDisponiveis[Random.Range(0, petsDisponiveis.Count)];
                    InstanciarEntidade(sorteado, slot);
                }
            }

            // 5. Enche a prateleira de COMIDAS filtradas
            foreach (Transform slot in slotsDeComidas)
            {
                LimparSlot(slot);
                if (comidasDisponiveis.Count > 0)
                {
                    EntityData sorteado = comidasDisponiveis[Random.Range(0, comidasDisponiveis.Count)];
                    InstanciarEntidade(sorteado, slot);
                }
            }

            Debug.Log($" Loja roletada! Tier Máximo Liberado na Rodada {rodada}: Tier {tierMaximo}");
        }
        else
        {
            Debug.Log("Sem dinheiro para roletar a loja!");
        }
    }

    private void LimparSlot(Transform slot)
    {
        if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
    }

    private void InstanciarEntidade(EntityData dado, Transform slot)
    {
        GameObject novaEntidade = Instantiate(petPrefab, slot);

        RectTransform rt = novaEntidade.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
        }

        novaEntidade.GetComponent<PetInstance>().Setup(dado);
    }
}
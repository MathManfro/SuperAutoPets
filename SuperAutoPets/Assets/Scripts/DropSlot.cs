using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public bool isTeamSlot;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastado = eventData.pointerDrag;
        Draggable draggable = objetoArrastado.GetComponent<Draggable>();
        PetInstance petArrastado = objetoArrastado.GetComponent<PetInstance>();

        if (draggable != null && petArrastado != null)
        {
            // Descobre se o pet/comida veio da loja ou se o jogador só está movendo ele dentro da equipe
            bool veioDaLoja = true;
            if (draggable.parentAfterDrag != null && draggable.parentAfterDrag.GetComponent<DropSlot>() != null)
            {
                veioDaLoja = !draggable.parentAfterDrag.GetComponent<DropSlot>().isTeamSlot;
            }

            // ==========================================
            // NOVA REGRA: VENDA DE PET (Arrastou da equipe para um slot da loja)
            // ==========================================
            if (!isTeamSlot && !veioDaLoja)
            {
                int ouroGanho = petArrastado.nivelAtual; // Ganha ouro = nível do pet
                EconomyManager.Instance.GanharOuro(ouroGanho);
                Debug.Log($" {petArrastado.data.nome} vendido na loja por {ouroGanho} de ouro!");
                if (ServidorSAP.Instance != null)
                    ServidorSAP.Instance.VenderPetDireto(petArrastado.data.cod, petArrastado.nivelAtual);

                List<PetInstance> equipeAtual = new List<PetInstance>();
                foreach (Transform slot in draggable.parentAfterDrag.parent)
                {
                    if (slot.childCount > 0) equipeAtual.Add(slot.GetChild(0).GetComponent<PetInstance>());
                }
                GerenciadorDeHabilidades.ExecutarHabilidade(petArrastado, equipeAtual, TipoGatilho.Ao_Vender);

                Destroy(objetoArrastado); // Destrói o pet
                return; // Encerra o código aqui para não tentar colocar ele no slot
            }

            // ==========================================
            // CASO 1: O SLOT ESTÁ VAZIO (0 Filhos)
            // ==========================================
            if (transform.childCount == 0)
            {
                // A comida não pode morar num slot vazio!
                if (petArrastado.data.isFood)
                {
                    Debug.Log("Comida não pode ocupar espaço na equipe!");
                    return; // Cancela a ação
                }

                // Se for pet e o slot for da equipe, verifica a grana
                if (isTeamSlot && veioDaLoja)
                {
                    int custo = (int)petArrastado.data.valor;
                    if (EconomyManager.Instance.PodeGastar(custo))
                    {
                        EconomyManager.Instance.GastarOuro(custo);
                        draggable.parentAfterDrag = transform; // Oficializa a compra

                        if (ServidorSAP.Instance != null)
                            ServidorSAP.Instance.ComprarItemDireto(petArrastado.data.cod);
                    }
                    else
                    {
                        Debug.Log("Dinheiro insuficiente!");
                    }
                }
                else
                {
                    draggable.parentAfterDrag = transform; // Movimentação livre
                }
            }
            // ==========================================
            // CASO 2: O SLOT JÁ TEM 1 PET DENTRO (1 Filho)
            // ==========================================
            else if (transform.childCount == 1 && isTeamSlot)
            {
                // AGORA SIM o petNoSlot existe!
                PetInstance petNoSlot = transform.GetChild(0).GetComponent<PetInstance>();

                // A) É COMIDA? (Dá o buff de poder e destrói a maçã)
                if (petArrastado.data.isFood)
                {
                    if (veioDaLoja)
                    {
                        int custo = (int)petArrastado.data.valor;
                        if (EconomyManager.Instance.PodeGastar(custo))
                        {
                            EconomyManager.Instance.GastarOuro(custo);
                            petNoSlot.ReceberBuff(petArrastado.data.poderBase);
                            if (ServidorSAP.Instance != null)
                                ServidorSAP.Instance.ComprarItemDireto(petArrastado.data.cod, "Buff Poder");
                            Destroy(objetoArrastado);
                        }
                    }
                }
                // B) É PET IGUAL? (Faz a fusão e sobe de nível)
                else if (petNoSlot.data.cod == petArrastado.data.cod)
                {
                    if (veioDaLoja)
                    {
                        int custo = (int)petArrastado.data.valor;
                        if (EconomyManager.Instance.PodeGastar(custo))
                        {
                            EconomyManager.Instance.GastarOuro(custo);
                            RealizarFusao(petNoSlot, objetoArrastado);
                            if (ServidorSAP.Instance != null)
                                ServidorSAP.Instance.ComprarItemDireto(petArrastado.data.cod, "Fusão");
                        }
                    }
                    else
                    {
                        RealizarFusao(petNoSlot, objetoArrastado);
                    }
                }
            }
        }
    }

    // A função de fusão que estava faltando no seu print fica aqui embaixo!
    private void RealizarFusao(PetInstance petBase, GameObject petSacrificado)
    {
        petBase.SubirNivel();
        
        List<PetInstance> equipeAtual = new List<PetInstance>();
        foreach (Transform slot in transform.parent)
        {
            if (slot.childCount > 0) equipeAtual.Add(slot.GetChild(0).GetComponent<PetInstance>());
        }
        GerenciadorDeHabilidades.ExecutarHabilidade(petBase, equipeAtual, TipoGatilho.Ao_Subir_Nivel);

        Destroy(petSacrificado);
    }
}
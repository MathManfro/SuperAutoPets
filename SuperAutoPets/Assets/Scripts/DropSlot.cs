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
            bool veioDaLoja = true;
            if (draggable.parentAfterDrag != null && draggable.parentAfterDrag.GetComponent<DropSlot>() != null)
            {
                veioDaLoja = !draggable.parentAfterDrag.GetComponent<DropSlot>().isTeamSlot;
            }

            // ==========================================
            // REGRA: VENDA DE PET 
            // ==========================================
            if (!isTeamSlot && !veioDaLoja)
            {
                int ouroGanho = petArrastado.nivelAtual;
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

                Destroy(objetoArrastado);
                return;
            }

            // ==========================================
            // CASO 1: O SLOT ESTÁ VAZIO (0 Filhos)
            // ==========================================
            if (transform.childCount == 0)
            {
                if (petArrastado.data.isFood)
                {
                    Debug.Log("Comida não pode ocupar espaço na equipe!");
                    return;
                }

                if (isTeamSlot && veioDaLoja)
                {
                    int custo = (int)petArrastado.data.valor;
                    if (EconomyManager.Instance.PodeGastar(custo))
                    {
                        EconomyManager.Instance.GastarOuro(custo);
                        draggable.parentAfterDrag = transform;

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
                    draggable.parentAfterDrag = transform;
                }
            }
            // ==========================================
            // CASO 2: O SLOT JÁ TEM 1 PET DENTRO (1 Filho)
            // ==========================================
            else if (transform.childCount == 1 && isTeamSlot)
            {
                PetInstance petNoSlot = transform.GetChild(0).GetComponent<PetInstance>();

                // A) É COMIDA?
                if (petArrastado.data.isFood)
                {
                    if (veioDaLoja)
                    {
                        int custo = (int)petArrastado.data.valor;
                        if (EconomyManager.Instance.PodeGastar(custo))
                        {
                            EconomyManager.Instance.GastarOuro(custo);

                            // O jogo lê o "Poder Base" que você configurou no Inspector da comida!
                            petNoSlot.ReceberBuff(petArrastado.data.poderBase);

                            if (ServidorSAP.Instance != null)
                                ServidorSAP.Instance.ComprarItemDireto(petArrastado.data.cod, "Buff Poder");

                            Destroy(objetoArrastado);
                        }
                    }
                }
                // B) É PET IGUAL? (Fusão Clássica)
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

    private void RealizarFusao(PetInstance petBase, GameObject petSacrificado)
    {
        petBase.SubirNivel();

        List<PetInstance> equipeAtual = new List<PetInstance>();
        foreach (Transform slot in transform.parent)
        {
            if (slot.childCount > 0) equipeAtual.Add(slot.GetChild(0).GetComponent<PetInstance>());
        }

        GerenciadorDeHabilidades.ExecutarHabilidade(petBase, equipeAtual, TipoGatilho.Ao_Subir_Nivel);

        foreach (PetInstance aliado in equipeAtual)
        {
            if (aliado != petBase && aliado != null && aliado.data.nome == "Água-viva")
            {
                GerenciadorDeHabilidades.ExecutarHabilidade(aliado, equipeAtual, TipoGatilho.Ao_Subir_Nivel);
            }
        }

        Destroy(petSacrificado);
    }
}
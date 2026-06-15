using UnityEngine;
using System.Collections.Generic;

public class BotGenerator : MonoBehaviour
{
    [Header("Configurações do Bot")]
    public Transform[] slotsInimigos;
    public List<EntityData> poolDePets;
    public GameObject petPrefab;

    public void GerarEquipeInimiga(string dadosDoServidor)
    {
        foreach (Transform slot in slotsInimigos)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }

        Debug.Log("Tentando gerar equipe com os IDs: " + dadosDoServidor);

        string[] idsInimigos = dadosDoServidor.Split(',');

        for (int i = 0; i < idsInimigos.Length; i++)
        {
            if (i < slotsInimigos.Length && int.TryParse(idsInimigos[i], out int idDoBanco))
            {
                EntityData sorteado = poolDePets.Find(pet => pet.cod == idDoBanco);

                if (sorteado != null)
                {
                    GameObject novoInimigo = Instantiate(petPrefab, slotsInimigos[i]);
                    RectTransform rt = novoInimigo.GetComponent<RectTransform>();
                    if (rt != null) { rt.localPosition = Vector3.zero; rt.localScale = Vector3.one; }

                    novoInimigo.GetComponent<PetInstance>().Setup(sorteado);

                    Draggable drag = novoInimigo.GetComponent<Draggable>();
                    if (drag != null) drag.enabled = false;
                }
                else
                {
                    Debug.LogWarning("ALERTA: O Banco mandou o ID " + idDoBanco + ", mas ele não existe na sua lista do Inspector!");
                }
            }
        }
    }
}
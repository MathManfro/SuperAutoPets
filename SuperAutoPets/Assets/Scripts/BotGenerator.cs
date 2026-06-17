using UnityEngine;
using System.Collections.Generic;

public class BotGenerator : MonoBehaviour
{
    [Header("Configurações do Bot")]
    public Transform[] slotsInimigos;
    public List<EntityData> poolDePets;
    public GameObject petPrefab;

    public void GerarEquipeInimiga(string dadosCompletos)
    {
        foreach (Transform slot in slotsInimigos)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }

        Debug.Log("Lendo Fotografia do Inimigo: " + dadosCompletos);

        string[] petsInimigos = dadosCompletos.Split(',');

        for (int i = 0; i < petsInimigos.Length; i++)
        {
            if (i < slotsInimigos.Length && !string.IsNullOrEmpty(petsInimigos[i]))
            {
                string[] infoPet = petsInimigos[i].Split('-');

                if (infoPet.Length == 3 && int.TryParse(infoPet[0], out int idDoBanco))
                {
                    EntityData sorteado = poolDePets.Find(pet => pet.cod == idDoBanco);

                    if (sorteado != null)
                    {
                        GameObject novoInimigo = Instantiate(petPrefab, slotsInimigos[i]);
                        RectTransform rt = novoInimigo.GetComponent<RectTransform>();
                        if (rt != null) { rt.localPosition = Vector3.zero; rt.localScale = Vector3.one; }

                        PetInstance petInstance = novoInimigo.GetComponent<PetInstance>();
                        petInstance.Setup(sorteado);

                        petInstance.nivelAtual = int.Parse(infoPet[1]);
                        petInstance.poderAtual = int.Parse(infoPet[2]);

                        PetDisplay display = novoInimigo.GetComponent<PetDisplay>();
                        if (display != null)
                        {
                            display.textoPoder.text = petInstance.poderAtual.ToString();
                            if (display.textoNivel != null) display.textoNivel.text = "Lv." + petInstance.nivelAtual;
                        }

                        Draggable drag = novoInimigo.GetComponent<Draggable>();
                        if (drag != null) drag.enabled = false;
                    }
                }
            }
        }
    }
}
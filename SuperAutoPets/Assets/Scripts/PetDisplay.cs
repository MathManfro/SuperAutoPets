using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetDisplay : MonoBehaviour
{
    public Image imagemPet;
    public TextMeshProUGUI textoPoder;

    // A NOVA VARIÁVEL AQUI
    public TextMeshProUGUI textoNivel;

    // Atualizamos o Setup para receber o nível também
    public void Setup(EntityData data, int nivel = 1)
    {
        imagemPet.sprite = data.icon;
        textoPoder.text = data.poderBase.ToString();

        if (textoNivel != null)
        {
            textoNivel.text = "Lv." + nivel;
        }
    }
}
using UnityEngine;
using TMPro;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoOuro;

    private void OnEnable()
    {
        EconomyManager.OnGoldChanged += AtualizarDisplay;
    }

    private void OnDisable()
    {
        EconomyManager.OnGoldChanged -= AtualizarDisplay;
    }

    private void AtualizarDisplay(int ouro)
    {
        textoOuro.text = "Ouro: " + ouro.ToString();
    }
}
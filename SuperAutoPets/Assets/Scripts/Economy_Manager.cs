using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Configurações de Economia")]
    public int ouroInicial = 10;
    public int custoRoll = 1;

    public int OuroAtual { get; private set; }

    // Evento para avisar a UI que o ouro mudou
    public static event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetarOuro();
    }

    public void ResetarOuro(int valor = 10)
    {
        OuroAtual = valor;
        OnGoldChanged?.Invoke(OuroAtual);
    }

    public bool PodeGastar(int valor)
    {
        return OuroAtual >= valor;
    }

    public void GastarOuro(int valor)
    {
        OuroAtual -= valor;
        OnGoldChanged?.Invoke(OuroAtual);
    }

    public void GanharOuro(int valor)
    {
        OuroAtual += valor;
        OnGoldChanged?.Invoke(OuroAtual);
    }
}
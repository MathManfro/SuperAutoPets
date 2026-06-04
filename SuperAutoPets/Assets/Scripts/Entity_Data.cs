using UnityEngine;

// Criamos a lista de Momentos (Gatilhos) possíveis
public enum TipoGatilho { Nenhum, Ao_Desmaiar, Ao_Vender, Ao_Subir_Nivel }

[CreateAssetMenu(fileName = "NovaEntidade", menuName = "SAP/Entidade")]
public class EntityData : ScriptableObject
{
    public int cod;
    public string nome;
    public int poderBase;
    public float valor;
    public Sprite icon;
    public bool isFood;
    [Range(1, 6)] public int tier = 1;

    [Header("Habilidades")]
    public TipoGatilho gatilho;
    public int forcaDaHabilidade; // Quanto de poder a habilidade dá
}
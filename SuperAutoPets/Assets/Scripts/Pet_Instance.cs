using UnityEngine;

public class PetInstance : MonoBehaviour
{
    public EntityData data;
    public int nivelAtual = 1; // Reflete a coluna 'nivel' da tabela COMPRAS
    public int poderAtual;     // Reflete a coluna 'poder'

    public void Setup(EntityData newData)
    {
        data = newData;
        poderAtual = data.poderBase;

        // A mágica que faltava: avisa o script de UI para mostrar a foto e o texto na tela!
        GetComponent<PetDisplay>().Setup(data, nivelAtual);
    }

    // Função que será chamada pelo DropSlot quando você fundir dois pets iguais
    public void SubirNivel()
    {
        nivelAtual++;
        poderAtual += 1; // Regra simples: ganha +1 de poder ao fundir

        // Atualiza o texto na tela
        GetComponent<PetDisplay>().textoPoder.text = poderAtual.ToString();
        GetComponent<PetDisplay>().textoNivel.text = "Lv." + nivelAtual.ToString();
    }

    public void ReceberBuff(int buffPoder)
    {
        poderAtual += buffPoder;
        // Atualiza a UI imediatamente para refletir o novo poder
        GetComponent<PetDisplay>().textoPoder.text = poderAtual.ToString();
        Debug.Log(data.nome + " comeu e agora tem " + poderAtual + " de poder!");
    }
}
using UnityEngine;
using System.Collections.Generic;

public static class GerenciadorDeHabilidades
{
    // Agora a função recebe O GATILHO REAL que acabou de acontecer no jogo
    public static void ExecutarHabilidade(PetInstance petAtivador, List<PetInstance> equipe, TipoGatilho gatilhoDisparado)
    {
        if (petAtivador.data.gatilho == TipoGatilho.Nenhum) return;

        // A SEGURANÇA: Se o gatilho do pet não for igual ao que aconteceu, ignora e para aqui!
        if (petAtivador.data.gatilho != gatilhoDisparado) return;

        int forca = petAtivador.data.forcaDaHabilidade;

        // PEIXE: Só vai rodar se o gatilho disparado for Ao_Subir_Nivel
        if (petAtivador.data.nome == "Peixe")
        {
            Debug.Log($" {petAtivador.data.nome} subiu de nível e deu +{forca} para todos!");
            foreach (PetInstance aliado in equipe)
            {
                if (aliado != petAtivador) aliado.ReceberBuff(forca);
            }
        }

        // CASTOR: Só vai rodar se o gatilho disparado for Ao_Vender
        else if (petAtivador.data.nome == "Castor")
        {
            Debug.Log($" {petAtivador.data.nome} foi vendido e deu +{forca} para 2 aliados!");
            DarBuffAleatorio(equipe, petAtivador, 2, forca);
        }

        // FORMIGA: Só vai rodar se o gatilho disparado for Ao_Desmaiar
        else if (petAtivador.data.nome == "Formiga")
        {
            Debug.Log($" {petAtivador.data.nome} desmaiou e deixou +{forca} para 1 aliado!");
            DarBuffAleatorio(equipe, petAtivador, 1, forca);
        }
    }

    private static void DarBuffAleatorio(List<PetInstance> equipe, PetInstance ignorar, int quantidadeAlvos, int forca)
    {
        List<PetInstance> alvosPossiveis = new List<PetInstance>();

        foreach (PetInstance p in equipe)
        {
            if (p != ignorar && p != null && p.poderAtual > 0)
            {
                alvosPossiveis.Add(p);
            }
        }

        for (int i = 0; i < quantidadeAlvos; i++)
        {
            if (alvosPossiveis.Count > 0)
            {
                int index = Random.Range(0, alvosPossiveis.Count);
                alvosPossiveis[index].ReceberBuff(forca);
                alvosPossiveis.RemoveAt(index);
            }
        }
    }
}
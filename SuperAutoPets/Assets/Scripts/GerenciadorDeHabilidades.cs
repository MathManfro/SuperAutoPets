using UnityEngine;
using System.Collections.Generic;

public static class GerenciadorDeHabilidades
{
    public static void ExecutarHabilidade(PetInstance petAtivador, List<PetInstance> equipe, TipoGatilho gatilhoDisparado, List<PetInstance> equipeInimiga = null)
    {

        string nomeDaHabilidade = petAtivador.data.nome;
        int forca = petAtivador.data.forcaDaHabilidade;
        TipoGatilho gatilhoDoPet = petAtivador.data.gatilho;

        if (nomeDaHabilidade == "Tucano")
        {
            int indexDele = equipe.IndexOf(petAtivador);

            if (indexDele >= 0 && indexDele + 1 < equipe.Count)
            {
                PetInstance aliadoAtras = equipe[indexDele + 1];

                if (aliadoAtras != null && aliadoAtras.data.nome != "Tucano")
                {
                    nomeDaHabilidade = aliadoAtras.data.nome;
                    forca = aliadoAtras.data.forcaDaHabilidade;
                    gatilhoDoPet = aliadoAtras.data.gatilho;

                    Debug.Log($" O Tucano ativou o modo espião e copiou a habilidade do(a) {nomeDaHabilidade}!");
                }
            }
        }

        if (gatilhoDoPet == TipoGatilho.Nenhum) return;
        if (gatilhoDoPet != gatilhoDisparado) return;


        // PEIXE
        if (nomeDaHabilidade == "Peixe")
        {
            Debug.Log($" {petAtivador.data.nome} subiu de nível e deu +{forca} para todos!");
            foreach (PetInstance aliado in equipe)
            {
                if (aliado != petAtivador) aliado.ReceberBuff(forca);
            }
        }
        // CASTOR
        else if (nomeDaHabilidade == "Castor")
        {
            Debug.Log($" {petAtivador.data.nome} foi vendido e deu +{forca} para 2 aliados!");
            DarBuffAleatorio(equipe, petAtivador, 2, forca);
        }
        // FORMIGA
        else if (nomeDaHabilidade == "Formiga")
        {
            Debug.Log($" {petAtivador.data.nome} desmaiou e deixou +{forca} para 1 aliado!");
            DarBuffAleatorio(equipe, petAtivador, 1, forca);
        }
        // CISNE
        else if (nomeDaHabilidade == "Cisne")
        {
            Debug.Log($" {petAtivador.data.nome} rendeu +{forca} moedas no início da rodada!");
            EconomyManager.Instance.GanharOuro(forca);
        }
        // ÁGUA-VIVA
        else if (nomeDaHabilidade == "Água-viva")
        {
            Debug.Log($" {petAtivador.data.nome} viu um aliado evoluir e ganhou +{forca} de status!");
            petAtivador.ReceberBuff(forca);
        }

        // ABELHA
        else if (nomeDaHabilidade == "Abelha")
        {
            Debug.Log($" {petAtivador.data.nome} viu a loja evoluir de nível e ganhou +{forca} de poder!");
            petAtivador.ReceberBuff(forca);
        }

        // GAMBÁ   
        else if (nomeDaHabilidade == "Gambá")
        {
            if (equipeInimiga != null && equipeInimiga.Count > 0)
            {
                PetInstance alvoMaisForte = equipeInimiga[0];
                foreach (PetInstance inimigo in equipeInimiga)
                {
                    if (inimigo.poderAtual > alvoMaisForte.poderAtual)
                    {
                        alvoMaisForte = inimigo;
                    }
                }

                int danoCausado = alvoMaisForte.poderAtual / 2;
                alvoMaisForte.poderAtual -= danoCausado;

                if (alvoMaisForte.poderAtual < 1) alvoMaisForte.poderAtual = 1;

                alvoMaisForte.GetComponent<PetDisplay>().textoPoder.text = alvoMaisForte.poderAtual.ToString();

                Debug.Log($" O {petAtivador.data.nome} soltou o gás! O poder do {alvoMaisForte.data.nome} inimigo caiu pela metade (-{danoCausado})!");
            }
        }

        // LHAMA
        else if (nomeDaHabilidade == "Lhama")
        {
            if (equipe.Count <= 4)
            {
                Debug.Log($"O(a) {petAtivador.data.nome} viu que a equipe tem só {equipe.Count} pets e ganhou +{forca} de status!");
                petAtivador.ReceberBuff(forca);
            }
        }

        //T-REX
        else if (nomeDaHabilidade == "T-Rex")
        {
            List<PetInstance> alvosValidos = new List<PetInstance>();
            foreach (PetInstance p in equipe)
            {
                if (p != petAtivador && p != null && p.poderAtual > 0 && p.data.tier >= 4)
                {
                    alvosValidos.Add(p);
                }
            }

            Debug.Log($"O {petAtivador.data.nome} rugiu! Encontrou {alvosValidos.Count} aliados de Tier 4+ para buffar.");

            int buffsAplicados = 0;
            while (buffsAplicados < 2 && alvosValidos.Count > 0)
            {
                int indexAleatorio = Random.Range(0, alvosValidos.Count);
                PetInstance petSorteado = alvosValidos[indexAleatorio];

                petSorteado.ReceberBuff(forca);

                alvosValidos.RemoveAt(indexAleatorio);
                buffsAplicados++;
            }
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
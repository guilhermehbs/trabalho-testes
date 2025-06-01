using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using ConsoleApp1.Entities;
using ConsoleApp1.Enums;
public class Empresa
{

    public List<Espaco> _listaEspacos = new List<Espaco>()
    {
        new Espaco("A", 100, 10000.00),
        new Espaco("B", 100, 10000.00),
        new Espaco("C", 100, 10000.00),
        new Espaco("D", 100, 10000.00),
        new Espaco("E", 200, 17000.00),
        new Espaco("F", 200, 17000.00),
        new Espaco("G", 500, 8000.00),
        new Espaco("H", 500, 35000.00) 
    };
    public List<Evento> _listaEventos;

    public Empresa()
    {
        _listaEventos = new List<Evento>();
    }
    public Espaco EscolherMelhorEspaco(int quantidadeConvidados)
    {
        _listaEspacos.OrderBy(p => p.capacidadeMaxima); //ordena a lista de espaços por capacidade afim de escolher o melhor espaço
        for (int i = 0; i < _listaEspacos.Count; i++)
        {
            if (_listaEspacos[i].capacidadeMaxima >= quantidadeConvidados)
            {
                return _listaEspacos[i];
            }
        }
        return null;
    }
    public DateTime ProcurarDataMaisProxima(Espaco espaco)
    {
        DateTime dataAtual = DateTime.Now;
        DateTime data30DiasDepois = dataAtual.AddDays(30);
        DateTime diaValido = DateTime.MinValue;
        bool deuBom = false;
        while (!deuBom)
        {
            if (data30DiasDepois.DayOfWeek == DayOfWeek.Friday || data30DiasDepois.DayOfWeek == DayOfWeek.Saturday)
            {
                diaValido = data30DiasDepois;
                bool verificador = VerificarEspacoDisponivel(espaco, diaValido);

                if (verificador)
                {
                    deuBom = true;
                    return diaValido;
                }
            }
            data30DiasDepois = data30DiasDepois.AddDays(1);
        }
        return diaValido;
    }

    public bool VerificarEspacoDisponivel(Espaco espaco, DateTime data)
    {
        {
            for (int i = 0; i < _listaEventos.Count; i++)
            {
                if (_listaEspacos[i].nomeEspaco == espaco.nomeEspaco)
                {
                    if (_listaEspacos[i].datas.Count == 0)
                    {
                        _listaEspacos[i].datas.Add(data);
                        return true;
                    }
                    for (int j = 0; j < _listaEspacos[i].datas.Count; j++)
                    {
                        if (_listaEspacos[i].datas[j] != data)
                        {
                            _listaEventos[i]._espacoEvento.datas.Add(data);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void AdicionarEventoNoArquivo(Evento festa, string caminhoArquivo)
    {

        _listaEventos.Add(festa);
        string eventoString = FormatEvento(festa);

        // Abre o arquivo para escrita, criando-o se não existir, e usando append para adicionar ao final
        using (StreamWriter arq = new StreamWriter(caminhoArquivo, append: true))
        {
            arq.WriteLine(eventoString);
        }
        Console.WriteLine("Evento adicionado no arquivo com sucesso!");
    }
    static string FormatEvento(Evento evento)
    {
        return $"{evento._qtdConvidados}|{evento._espacoEvento.nomeEspaco}|{evento._espacoEvento.capacidadeMaxima}|{evento._espacoEvento.valorEspaco}|{evento._dataEvento.ToShortDateString()}|{evento._categoriaEvento}|{evento.valorTotalFesta}|{evento._tipoEvento}";
    }
}
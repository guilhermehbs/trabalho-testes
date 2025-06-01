using ConsoleApp1;
using ConsoleApp1.Entities;
using ConsoleApp1.Enums;


public class Evento
{
    public int _qtdConvidados;
    public Espaco _espacoEvento;
    public DateTime _dataEvento;
    public TipoEvento _tipoEvento;
    public CategoriaEvento _categoriaEvento;
    public double valorTotalFesta;
    public List<Produto> _produtos = new List<Produto>()
    {
        new Produto { _nome = "Croquete carne seca", _tipo = TipoEvento.Luxo },
        new Produto { _nome = "Barquetes Legumes", _tipo = TipoEvento.Luxo},
        new Produto { _nome = "Empadinha Gourmet", _tipo= TipoEvento.Luxo},
        new Produto { _nome = "Cestinha de Bacalhau", _tipo = TipoEvento.Luxo },
        new Produto { _nome = "Canapé", _tipo = TipoEvento.Premier },
        new Produto { _nome = "Tartine", _tipo = TipoEvento.Premier},
        new Produto { _nome = "Bruschetta", _tipo = TipoEvento.Premier},
        new Produto { _nome = "Espetinho Caprese", _tipo = TipoEvento.Premier },
        new Produto { _nome = "Coxinha", _tipo = TipoEvento.Standard },
        new Produto { _nome = "Kibe", _tipo = TipoEvento.Standard },
        new Produto { _nome = "Empadinha", _tipo = TipoEvento.Standard },
        new Produto { _nome = "Pão de Queijo", _tipo = TipoEvento.Standard }

    };
    public List<Bebida> _bebidas = new List<Bebida>()
    {
        new Bebida {_nome = "Água com gás", _preco = 5.00, _tipo = TipoEvento.Geral},
        new Bebida {_nome = "Suco Natural", _preco = 7.00, _tipo = TipoEvento.Geral},
        new Bebida {_nome = "Refrigerante", _preco = 8.00, _tipo = TipoEvento.Geral},
        new Bebida {_nome = "Cerveja", _preco = 20.00, _tipo = TipoEvento.Geral},
        new Bebida {_nome = "Cerveja Artesanal", _preco = 30.00, _tipo= TipoEvento.LuxoIPremier},
        new Bebida {_nome = "Espumante Nac.", _preco = 80.00, _tipo = TipoEvento.Geral},
        new Bebida {_nome = "Espumante Imp.", _preco = 140.00, _tipo = TipoEvento.LuxoIPremier}
    };
     
    public Evento(DateTime data, int qtdConvidados, Espaco espaco, TipoEvento tipoEvento, CategoriaEvento categoriaEvento)
    {
        
        _dataEvento = data;
        _qtdConvidados = qtdConvidados;
        _espacoEvento = espaco;
        _tipoEvento = tipoEvento;
        _categoriaEvento = categoriaEvento;
        valorTotalFesta = 0;
       
    }

    
 
    public double CalcularPrecoProdutosEvento(TipoEvento tipoEvento)
    {
        double valorProdutos = 0;
        if (tipoEvento == TipoEvento.Premier)
        {
            valorProdutos = _qtdConvidados * 60;
        }
        else if (tipoEvento == TipoEvento.Luxo)
        {
            valorProdutos = _qtdConvidados * 48;
        }
        else
        {
            valorProdutos = _qtdConvidados * 40;
        }
        return valorProdutos;
    }
    public double CalcularPrecoBebidasEvento(List<Bebida> bebidasEvento)
    {
        double totalBebidas = 0;

        foreach (var b in bebidasEvento)
        {
            totalBebidas += (b._preco * b._quantidade);
        }
        return totalBebidas;
    }
    public void ColocarBebidasEventoPorTipo(TipoEvento tipoBebida, List<Bebida> bebidas)
    {
        for (int i = 0; i < _bebidas.Count; i++)
        {
            if (_bebidas[i]._tipo == TipoEvento.Geral)
            {
                bebidas.Add(_bebidas[i]);
            }

            if (tipoBebida == TipoEvento.Premier && _bebidas[i]._tipo == TipoEvento.LuxoIPremier)
            {
                bebidas.Add(_bebidas[i]);
            }
        }
    }
}



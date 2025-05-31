using ConsoleApp1.Entities;
using ConsoleApp1.Enums;
using System;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1
{
	public class Program
	{
		static Empresa empresa = new Empresa();
		static string diretorioSaida = AppDomain.CurrentDomain.BaseDirectory;
		static string nomeArquivo = "eventos.txt";
		static string caminhoArquivo = Path.Combine(diretorioSaida, nomeArquivo);
		static void Main(string[] args)
		{
			empresa._listaEventos = LerTodosEventosDoArquivo(caminhoArquivo);
			bool foiPossivelDefinir = false;
			int opcao = 0;

			while (!foiPossivelDefinir) // esse laço garante que se cair na exceção o código continuará solicitando a digitação
			{
				try
				{
					while (opcao != 2)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.WriteLine("Bem-vindo ao AGGV Festas\n");
						Console.ResetColor();
						Console.ForegroundColor = ConsoleColor.Yellow;
						#region Exibição do calendário de eventos
						Console.WriteLine("Nosso calendário atual de eventos:");
						Console.ResetColor();
						ExibirCalendário();
						#endregion
						empresa._listaEventos = LerTodosEventosDoArquivo(caminhoArquivo);
						Console.WriteLine("\nDigite a quantidade de convidados desejada no seu evento:");
						int quantidadeConvidados = int.Parse(Console.ReadLine());
						Espaco espacoEvento = EscolherEspaco(quantidadeConvidados);
						DateTime data = EscolherDataMaisProxima(espacoEvento);
						InformarDataEscolhidaPeloSistema(data, espacoEvento);
						TipoEvento tipoEvento = EscolherTipoEvento(TipoEvento.Nulo);
						EscolherTipoFesta(data, espacoEvento, tipoEvento, quantidadeConvidados);
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine("\nVocê deseja contratar mais eventos?");
						Console.ResetColor();
						Console.WriteLine("1) Sim\n2) Não");
						opcao = int.Parse(Console.ReadLine());
						if (opcao == 2)
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							Console.WriteLine("\nPrograma finalizado!");
							Console.ResetColor();
							break;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("\nOpção inválida!!");
							Console.ResetColor();
						}
					}
					break;
				}
				catch (FormatException)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nERRO! para definifir a quantidade de convidados, digite apenas números inteiros.\n");
					Console.ResetColor();
				}
			}
		}
		public static void ExibirCalendário()
		{
			for (int i = 0; i < empresa._listaEventos.Count; i++)
			{
				Console.WriteLine($"Tipo do evento: {empresa._listaEventos[i]._tipoEvento} - Data do evento: {empresa._listaEventos[i]._dataEvento.ToShortDateString()} - Nome do espaço: {empresa._listaEventos[i]._espacoEvento.nomeEspaco} - Valor Festa: {empresa._listaEventos[i].valorTotalFesta} - Quantidade Convidados: {empresa._listaEventos[i]._qtdConvidados} - Categoria Evento: {empresa._listaEventos[i]._categoriaEvento}");
			}
		}
		static DateTime EscolherDataMaisProxima(Espaco espacoEvento)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"\nNossos casamentos são realizados na data mais próxima da atual, contando 30 dias a partir do dia de hoje.");
			Console.WriteLine("\nComo são realizados apenas um casamento por dia, é possível que haja um casamento já agendado para este espaço na data mais próxima. Logo, calcularemos a próxima data mais próxima.\n");
			Console.ResetColor();
			return empresa.ProcurarDataMaisProxima(espacoEvento);
		}
		static void InformarDataEscolhidaPeloSistema(DateTime data, Espaco espacoEvento)
		{
			Console.Write("\nA data escolhida para o seu evento é: ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(data.ToShortDateString());
			Console.ResetColor();

			Console.Write("\nO espaço selecionado foi o espaço:");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(espacoEvento);
			Console.ResetColor();
		}
		static TipoEvento EscolherTipoEvento(TipoEvento tipoEvento)
		{
			bool foiPossivelDefinir = false;
			while (!foiPossivelDefinir) // esse laço garante que se cair na exceção o código continuará solicitando a digitação
			{
				try
				{
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.Write("\nPara continuarmos com o agendamento do seu evento, precisamos de estabelecer o tipo de evento.\n");
					Console.ResetColor();
					Console.WriteLine("\n1) Premier - R$ 60,00:\n- Canapé\n- Tartin\n- Bruschetta\n- Espetinho caprese");
					Console.WriteLine("\n2) Luxo - R$ 48,00:\n- Cestinha de bacalhau\n- Empadinha gourmet\n-Barquetes de legumes\n- Croquete de carne seca\n");
					Console.WriteLine("\b3) Standard - R$ 40,00:\n- Coxinha\n- Kibe\n- Empadinha\n- Pão de queijo\n");
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine("O valor de cada tipo será multiplicado pela quantidade de convidados no evento! Escolha o tipo desejado:\n");
					Console.ResetColor();
					int opcao = int.Parse(Console.ReadLine());
					do
					{
						switch (opcao)
						{
							case 1:
								Console.WriteLine("\nVocê escolheu a opção Premier, portanto o seu evento será baseado nas categorias de Premier informadas anteriormente.");
								tipoEvento = TipoEvento.Premier;
								break;
							case 2:
								Console.WriteLine("\nVocê escolheu a opção Luxo, portanto o seu evento será baseado nas categorias de Luxo informadas anteriormente.");
								tipoEvento = TipoEvento.Luxo;
								break;
							case 3:
								Console.WriteLine("\nVocê escolheu a opção Standard, portanto o seu evento será baseado nas categorias de Standard informadas anteriormente");
								tipoEvento = TipoEvento.Standard;
								break;
							default:
								Console.WriteLine("\nVocê escolheu uma opção inválida, escolha outra opção: ");
								opcao = int.Parse(Console.ReadLine());
								break;
						}
						foiPossivelDefinir = true;
					} while (tipoEvento == TipoEvento.Nulo);

				}
				catch (FormatException) //exceção para captar bebidas inválidas 
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nERRO! nosso menu suporta apenas números!");
					Console.ResetColor();
				}
			}
			return tipoEvento;
		}

		static void EscolherTipoFesta(DateTime data, Espaco espacoEvento, TipoEvento tipoEvento, int quantidadeConvidados)
		{
			bool foiPossivelDefinir = false;
			while (!foiPossivelDefinir) // esse laço garante que se cair na exceção o código continuará solicitando a digitação
			{
				try
				{
					Console.WriteLine("Qual tipo de festa você deseja contratar?\n");
					Console.WriteLine("1 - Casamento\n2 - Formatura\n3 - Festa de empresa\n4 - Festa de aniversário\n5 - Evento livre\n");
					int opcao = int.Parse(Console.ReadLine());
					switch (opcao)
					{
						case 1:
							contratarCasamento(data, quantidadeConvidados, espacoEvento, tipoEvento);
							break;
						case 2:
							contratarFormatura(data, quantidadeConvidados, espacoEvento, tipoEvento);
							break;
						case 3:
							contratarFestaEmpresa(data, quantidadeConvidados, espacoEvento, tipoEvento);
							break;
						case 4:
							if (tipoEvento == TipoEvento.Standard)
								contratarFestaAniversario(data, quantidadeConvidados, espacoEvento);

							else
								Console.WriteLine("Festa de aniversário só aceita eventos do padrão Standard, qualquer outro tipo de evento não se encaixa nas condições de Festa de Aniversário");

							break;
						case 5:
							contratarEventoLivre(data, quantidadeConvidados, espacoEvento, tipoEvento);
							break;
						default:
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Opção inválida, digite apenas uma opção existente");
							Console.ResetColor();
							break;

					}
					foiPossivelDefinir = true;
				}
				catch (FormatException) //exceção para captar bebidas inválidas 
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nERRO! nosso menu suporta apenas números!");
					Console.ResetColor();
				}
			}
		}
		static Espaco EscolherEspaco(int quantidadeConvidados)
		{
			Espaco espacoEvento;
			Empresa empresa = new Empresa();
			Console.WriteLine("Temos 8 tipos de espaços incríveis para você, com capacidades variando entre 100, 200 e 500 convidados!");
			Console.WriteLine("\nLista de espaços:");
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var a in empresa._listaEspacos) //listar todos os espaços existentes 
			{
				Console.WriteLine($"Nome do espaço: {a.nomeEspaco} - Valor: {a.valorEspaco} - Capacidade: {a.capacidadeMaxima}");
			}
			Console.ResetColor();
			bool foiPossivelDefinir = false;
			while (!foiPossivelDefinir) // esse laço garante que se cair na exceção o código continuará solicitando a digitação
			{
				try
				{

					do //laço while para verificar se a quantidade de convidados é inválida (menor menor que 1 ou maior que 500)
					{

						if (quantidadeConvidados <= 0 || quantidadeConvidados > 500)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("\nNós não temos espaço adequado para suportar a quantidade de pessoas informada. A quantidade máxima que nossos espaços suportam é de 500 pessoas");
							Console.ResetColor();
						}
						else
						{
							return espacoEvento = empresa.EscolherMelhorEspaco(quantidadeConvidados);
						}
					} while (true);
				}
				catch (FormatException) //exceção para captar bebidas inválidas 
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nERRO! o número de convidados precisa ser números inteiros!");
					Console.ResetColor();
				}
			}
			return null;
		}
		public static void EscolherQuantidadePrecoBebidas(List<Bebida> bebidasEvento)
		{
			int quantidade = 0;
			bool foiPossivelDefinir = false;
			while (!foiPossivelDefinir) // esse laço garante que se cair na exceção o código continuará solicitando a digitação
			{
				try
				{
					for (int i = 0; i < bebidasEvento.Count; i++) //laço para solicitar todas as bebidas do usuário
					{
						Console.Write($"\nDigite a quantidade de {bebidasEvento[i]._nome} que você quer em seu evento: ");
						quantidade = int.Parse(Console.ReadLine());
						bebidasEvento[i]._quantidade = quantidade;
					}
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine("\nForam escolhidas as quantidades de bebidas do seu evento, a seguir te mostrarei as quantidades escolhidas com os valores totais: ");
					Console.ResetColor();
					for (int i = 0; i < bebidasEvento.Count; i++)
					{
						Console.WriteLine($"{bebidasEvento[i]._nome} - {bebidasEvento[i]._quantidade}: {bebidasEvento[i]._preco * bebidasEvento[i]._quantidade}.00 ");
					}
					foiPossivelDefinir = true;
				}
				catch (FormatException) //exceção para captar bebidas inválidas 
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\nERRO! a quantidade de bebidas deve ser apenas números inteiros!");
					Console.ResetColor();
					throw;
				}
			}

		}
		static void contratarCasamento(DateTime data, int quantidadeConvidados, Espaco espacoEvento, TipoEvento tipoEvento)
		{
			//construtor do casamento 
			Casamento casamento = new Casamento(data, quantidadeConvidados, espacoEvento, tipoEvento, CategoriaEvento.Casamento);
			Console.WriteLine("\nAgora vou precisar que você escolha as quantidades de bebidas que ofereceremos em seu casamento: ");
			casamento.ColocarBebidasEventoPorTipo(tipoEvento, casamento._bebidasCasamento); //método para colocar bebidas por tipo, pois algumas bebidas são exclusivas de alguns tipos
			casamento.ColocarProdutosCasamentoPorTipo(tipoEvento);
			EscolherQuantidadePrecoBebidas(casamento._bebidasCasamento);

			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("\nVocê já selecionou o que precisamos para gerar um orçamento para você. Estamos calculando o valor do casamento!");
			Console.ResetColor();
			#region Mostrar valor total do casamento
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nAqui está o valor total do seu casamento: {casamento.CalcularPrecoTotalCasamento(tipoEvento)}");
			Console.WriteLine($"O casamento será na data: {data.ToShortDateString()}"); //exibe a data no formato apenas de data
			Console.ResetColor();
			Console.WriteLine("Segue abaixo o resumo dos itens do casamento:\n");
			#endregion
			casamento.MostrarResumoCasamento(tipoEvento);
			empresa.AdicionarEventoNoArquivo(casamento, caminhoArquivo); //adiciona o casamento no arquvio

		}
		static void contratarFormatura(DateTime data, int quantidadeConvidados, Espaco espacoEvento, TipoEvento tipoEvento)
		{
			Formatura formatura = new Formatura(data, quantidadeConvidados, espacoEvento, tipoEvento, CategoriaEvento.Formatura);
			Console.WriteLine("\nAgora vou precisar que você escolha as quantidades de bebidas que ofereceremos em seu evento de formatura: ");
			formatura.ColocarBebidasEventoPorTipo(tipoEvento, formatura._bebidasFormatura); //método para exibir as bebidas de acordo com o tipo de evento, uma vez que alguns eventos tem bebidas exclusivas

			EscolherQuantidadePrecoBebidas(formatura._bebidasFormatura); //definir os valores
			Console.WriteLine("\nVocê já selecionou o que precisamos para gerar um orçamento para você. Estamos calculando o valor do casamento!");
			#region Mostrar valor total da formatura
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nAqui está o valor total da sua formatura: {formatura.CalcularPrecoTotalFormatura(tipoEvento)}");
			Console.WriteLine($"A formatura será na data: {data.ToShortDateString()}");//exibe a data no formato apenas de data
			Console.ResetColor();
			Console.WriteLine("Segue abaixo o resumo dos itens da formatura:\n");
			#endregion
			formatura.MostrarResumoFormatura(tipoEvento);
			empresa.AdicionarEventoNoArquivo(formatura, caminhoArquivo); //adiciona a formatura no arquivo
		}
		static void contratarFestaEmpresa(DateTime data, int quantidadeConvidados, Espaco espacoEvento, TipoEvento tipoEvento)
		{
			FestaEmpresa festaEmpresa = new FestaEmpresa(data, quantidadeConvidados, espacoEvento, tipoEvento, CategoriaEvento.FestaEmpresa);

			Console.WriteLine("\nAgora vou precisar que você escolha as quantidades de bebidas que ofereceremos em seu evento de festa empresarial: ");
			festaEmpresa.ColocarBebidasEventoPorTipo(tipoEvento, festaEmpresa._bebidasFestaEmpresa);
			EscolherQuantidadePrecoBebidas(festaEmpresa._bebidasFestaEmpresa);
			Console.WriteLine("\nVocê já selecionou o que precisamos para gerar um orçamento para você. Estamos calculando o valor da festa de sua empresa!");
			Console.ForegroundColor = ConsoleColor.Green;
			#region Mostrar valor total da festa da empresa
			Console.WriteLine($"\nAqui está o valor total da festa da sua empresa: {festaEmpresa.CalcularPrecoTotalFestaEmpresa(tipoEvento)}");
			Console.WriteLine($"A festa da empresa será na data: {data.ToShortDateString()}");//exibe a data no formato apenas de data
			Console.ResetColor();
			#endregion
			festaEmpresa.MostrarResumoFestaEmpresa(tipoEvento);
			empresa.AdicionarEventoNoArquivo(festaEmpresa, caminhoArquivo); //adiciona a festa no arquvio
		}
		static void contratarFestaAniversario(DateTime data, int quantidadeConvidados, Espaco espacoEvento)
		{
			FestaAniversario festaAniversario = new FestaAniversario(data, quantidadeConvidados, espacoEvento, CategoriaEvento.FestaAniversario);
			Console.WriteLine("\nAgora vou precisar que você escolha as quantidades de bebidas que ofereceremos em seu evento de festa de aniversário: ");
			festaAniversario.ColocarBebidasEventoPorTipo(TipoEvento.Standard, festaAniversario._bebidasFestaAniversario);
			EscolherQuantidadePrecoBebidas(festaAniversario._bebidasFestaAniversario);
			Console.WriteLine("\nVocê já selecionou o que precisamos para gerar um orçamento para você. Estamos calculando o valor do casamento!");
			#region Mostrar valor total da festa de aniversário
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nAqui está o valor total da sua formatura: {festaAniversario.CalcularPrecoTotalFestaAniversario()}");
			Console.WriteLine($"A festa de aniversário será na data: {data.ToShortDateString()}");//exibe a data no formato apenas de data
			Console.ResetColor();
			#endregion
			festaAniversario.MostrarResumoAniversario(TipoEvento.Standard);
			empresa.AdicionarEventoNoArquivo(festaAniversario, caminhoArquivo); //adiciona a festa no arquvio
		}
		static void contratarEventoLivre(DateTime data, int quantidadeConvidados, Espaco espacoEvento, TipoEvento tipoEvento)
		{
			FestaLivre festaLivre = new FestaLivre(data, quantidadeConvidados, espacoEvento, tipoEvento, CategoriaEvento.Livre);
			Console.WriteLine("\nAgora vou precisar que você escolha as quantidades de bebidas que ofereceremos em seu evento de festa: ");
			festaLivre.ColocarBebidasEventoPorTipo(tipoEvento, festaLivre._listaBebidasFestaLivre);
			Console.WriteLine("\nVocê já selecionou o que precisamos para gerar um orçamento para você. Estamos calculando o valor do casamento!");
			#region Mostrar valor total da festa de aniversário
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\nAqui está o valor total da sua formatura: {festaLivre.CalcularPrecoTotalFestaLivre(tipoEvento)}");
			Console.WriteLine($"A festa de aniversário será na data: {data.ToShortDateString()}");  //exibe a data no formato apenas de data
			Console.ResetColor();
			#endregion
			festaLivre.MostrarResumoFestaLivre();
			empresa.AdicionarEventoNoArquivo(festaLivre, caminhoArquivo);//adicionao o evento no arquvio

		}
		public static List<Evento> LerTodosEventosDoArquivo(string caminhoArquivo)
		{
			List<Evento> eventos = new List<Evento>();
			try
			{
				using (StreamReader sr = new StreamReader(caminhoArquivo))
				{
					string linha;
					while ((linha = sr.ReadLine()) != null)
					{
						Evento evento = LerEventoArquivo(linha);
						eventos.Add(evento);
					}
				}
			}
			catch (Exception ex) //exceção de erro no arquivo
			{
				Console.WriteLine($"Erro ao ler arquivo: {ex.Message}");
			}

			return eventos;
		}
		public static Evento LerEventoArquivo(string linha)
		{
			string[] partesArquivo = linha.Split('|');
			int qtdConvidados = int.Parse(partesArquivo[0]);
			string espacoNome = partesArquivo[1];
			int qtdMaxEspaco = int.Parse(partesArquivo[2]);
			double valorEspaco = double.Parse(partesArquivo[3]);
			DateTime data = DateTime.Parse(partesArquivo[4]);
			CategoriaEvento categoriaEvento = (CategoriaEvento)Enum.Parse(typeof(CategoriaEvento), partesArquivo[5]);
			double valorTotalFesta = double.Parse(partesArquivo[6]);
			TipoEvento tipoEvento = (TipoEvento)Enum.Parse(typeof(TipoEvento), partesArquivo[7]);
			Espaco espaco = new Espaco(espacoNome, qtdMaxEspaco, valorEspaco);
			Evento evento = new Evento(data, qtdConvidados, espaco, tipoEvento, categoriaEvento);
			evento.valorTotalFesta = valorTotalFesta;

			return evento;
		}

	}
}
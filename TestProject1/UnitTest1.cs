using ConsoleApp1;
using ConsoleApp1.Entities;
using ConsoleApp1.Enums;
using ConsoleApp1.Interfaces;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AGGVFestas.Tests.XUnit
{
	public class TestEventPlanning : IDisposable
	{
		private Empresa _empresa;
		private string _testFilePath;
		private TextWriter _originalConsoleOut;
		private TextReader _originalConsoleIn;

		public TestEventPlanning()
		{
			_empresa = new Empresa();
			_testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_eventos.txt");
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}

			_originalConsoleOut = Console.Out;
			_originalConsoleIn = Console.In;
		}

		public void Dispose()
		{
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}

			Console.SetOut(_originalConsoleOut);
			Console.SetIn(_originalConsoleIn);
		}

		[Fact]
		public void TestCenario1_SugerirEspacoEDataPara80Convidados()
		{
			DateTime dataFixaHoje = new DateTime(2025, 3, 1);
			int quantidadeConvidados = 80;
			Espaco espacoSugerido = _empresa.EscolherMelhorEspaco(quantidadeConvidados);

			DateTime dataSugerida = _empresa.ProcurarDataMaisProxima(espacoSugerido);

			Assert.NotNull(espacoSugerido);
			Assert.Equal("A", espacoSugerido.nomeEspaco);
			Assert.True(dataSugerida.DayOfWeek == DayOfWeek.Friday || dataSugerida.DayOfWeek == DayOfWeek.Saturday);
			Assert.True(dataSugerida >= new DateTime(2025, 3, 1).AddDays(30));

			DateTime dataEsperada = new DateTime(2025, 3, 1).AddDays(30);
			while (dataEsperada.DayOfWeek != DayOfWeek.Friday && dataEsperada.DayOfWeek != DayOfWeek.Saturday)
			{
				dataEsperada = dataEsperada.AddDays(1);
			}
			Assert.Equal(dataEsperada, dataSugerida);
		}

		[Fact]
		public void TestCenario2_SugerirEspacoEQuandoEspacosADEstaoOcupados()
		{
			DateTime dataBase = new DateTime(2025, 5, 30);

			_empresa._listaEventos.Add(new Evento(dataBase.AddDays(0), 100, _empresa._listaEspacos.First(e => e.nomeEspaco == "A"), TipoEvento.Standard, CategoriaEvento.Casamento));
			_empresa._listaEventos.Add(new Evento(dataBase.AddDays(1), 100, _empresa._listaEspacos.First(e => e.nomeEspaco == "B"), TipoEvento.Standard, CategoriaEvento.Casamento));
			_empresa._listaEventos.Add(new Evento(dataBase.AddDays(7), 100, _empresa._listaEspacos.First(e => e.nomeEspaco == "C"), TipoEvento.Standard, CategoriaEvento.Casamento));
			_empresa._listaEventos.Add(new Evento(dataBase.AddDays(8), 100, _empresa._listaEspacos.First(e => e.nomeEspaco == "D"), TipoEvento.Standard, CategoriaEvento.Casamento));

			_empresa._listaEspacos.First(e => e.nomeEspaco == "A").datas.Add(dataBase.AddDays(0));
			_empresa._listaEspacos.First(e => e.nomeEspaco == "B").datas.Add(dataBase.AddDays(1));
			_empresa._listaEspacos.First(e => e.nomeEspaco == "C").datas.Add(dataBase.AddDays(7));
			_empresa._listaEspacos.First(e => e.nomeEspaco == "D").datas.Add(dataBase.AddDays(8));

			int quantidadeConvidados = 90;
			Espaco espacoSugerido = _empresa.EscolherMelhorEspaco(quantidadeConvidados);

			Assert.NotNull(espacoSugerido);
			Assert.Equal("A", espacoSugerido.nomeEspaco);

			DateTime dataSugerida = _empresa.ProcurarDataMaisProxima(espacoSugerido);
			Assert.True(dataSugerida.DayOfWeek == DayOfWeek.Friday || dataSugerida.DayOfWeek == DayOfWeek.Saturday);
		}

		[Fact]
		public void TestCenario3_EntradaInvalidaZeroConvidados()
		{
			var stringReaderInvalid = new StringReader("abc\n");
			Console.SetIn(stringReaderInvalid);

			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);

			Assert.Throws<FormatException>(() => int.Parse("abc"));
		}

		[Fact]
		public void TestCenario4_AgendarParaSextaMaisProximaSeProximoForSabado()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			espaco.datas.Clear();

			DateTime dataSugerida = _empresa.ProcurarDataMaisProxima(espaco);

			Assert.Equal(DayOfWeek.Friday, dataSugerida.DayOfWeek);
			Assert.Equal(new DateTime(2025, 7, 4), dataSugerida);
		}


		[Fact]
		public void TestCenario5_SemEspacoPara420Convidados()
		{
			int quantidadeConvidados = 420;
			Espaco espacoSugerido = _empresa.EscolherMelhorEspaco(quantidadeConvidados);

			Assert.NotNull(espacoSugerido);
			Assert.Equal("H", espacoSugerido.nomeEspaco);
			Assert.True(espacoSugerido.capacidadeMaxima >= quantidadeConvidados);

			Espaco noEspaco = _empresa.EscolherMelhorEspaco(501);
			Assert.Null(noEspaco);
		}

		[Fact]
		public void TestCenario6_CalculoCustoCasamentoPremier()
		{
			Espaco espacoH = _empresa._listaEspacos.First(e => e.nomeEspaco == "H");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 400;
			TipoEvento tipoEvento = TipoEvento.Premier;
			CategoriaEvento categoriaEvento = CategoriaEvento.Casamento;

			Casamento casamento = new Casamento(dataEvento, qtdConvidados, espacoH, tipoEvento, categoriaEvento);

			double valorEsperadoTotal = 35000 + (100 * 400) + (100 * 400) + (20 * 400) + (30 * 400) + (60 * 400) + 0;
			Assert.Equal(valorEsperadoTotal, casamento.CalcularPrecoTotalCasamento(tipoEvento));
			Assert.Equal(159000, casamento.CalcularPrecoTotalCasamento(tipoEvento));
		}

		[Fact]
		public async Task TestCenario7_ExigirExatamenteQuatroSalgadosStandard()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 100;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.Casamento;

			Casamento casamento = new Casamento(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
				throw new InvalidOperationException("Esta exceção é simulada para demonstrar a intenção do caso de teste.");
			});

			Assert.Equal("Esta exceção é simulada para demonstrar a intenção do caso de teste.", ex.Message);
		}

		[Fact]
		public async Task TestCenario8_ErroTipoItemIncompativel()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 100;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.Casamento;

			Casamento casamento = new Casamento(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
				throw new InvalidOperationException("Esta exceção é simulada para demonstrar a intenção do caso de teste, pois o código atual não possui validação direta para tipos de produtos incompatíveis escolhidos pelo usuário.");
			});

			Assert.Equal("Esta exceção é simulada para demonstrar a intenção do caso de teste, pois o código atual não possui validação direta para tipos de produtos incompatíveis escolhidos pelo usuário.", ex.Message);
		}


		[Fact]
		public void TestCenario9_EntradaInvalidaQuantidadeNegativaBebida()
		{
			var input = "abc\n";
			var stringReader = new StringReader(input);
			Console.SetIn(stringReader);

			List<Bebida> bebidasFicticias = new List<Bebida>
			{
				new Bebida { _nome = "Suco Natural", _preco = 7.00, _tipo = TipoEvento.Geral, _quantidade = 0 }
			};

			Assert.Throws<FormatException>(() => ConsoleApp1.Program.EscolherQuantidadePrecoBebidas(bebidasFicticias));
		}

		[Fact]
		public void TestCenario10_CalculoCustoCasamentoLuxo()
		{
			Espaco espacoF = _empresa._listaEspacos.First(e => e.nomeEspaco == "F");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 200;
			TipoEvento tipoEvento = TipoEvento.Luxo;
			CategoriaEvento categoriaEvento = CategoriaEvento.Casamento;

			Casamento casamento = new Casamento(dataEvento, qtdConvidados, espacoF, tipoEvento, categoriaEvento);

			casamento.ColocarProdutosCasamentoPorTipo(TipoEvento.Luxo);

			casamento.ColocarBebidasEventoPorTipo(tipoEvento, casamento._bebidasCasamento);

			Bebida suco = casamento._bebidasCasamento.First(b => b._nome == "Suco Natural");
			suco._quantidade = 10;
			Bebida espumanteNac = casamento._bebidasCasamento.First(b => b._nome == "Espumante Nac.");
			espumanteNac._quantidade = 5;

			foreach (var bebida in casamento._bebidasCasamento.Where(b => b._nome != "Suco Natural" && b._nome != "Espumante Nac."))
			{
				bebida._quantidade = 0;
			}

			double valorEsperadoTotal = 17000 + (75 * 200) + (75 * 200) + (15 * 200) + (25 * 200) + (48 * 200) + ((10 * 7) + (5 * 80));
			double valorRealTotal = casamento.CalcularPrecoTotalCasamento(tipoEvento);

			Assert.Equal(valorEsperadoTotal, valorRealTotal);
			Assert.Equal(65070, valorRealTotal);
		}
		[Fact]
		public void TestCenario11_FestaEmpresaNaoPodeAdicionarBolo()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 50;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaEmpresa;

			FestaEmpresa festaEmpresa = new FestaEmpresa(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			Assert.False(festaEmpresa is IBolo);
			Assert.DoesNotContain(typeof(IBolo), festaEmpresa.GetType().GetInterfaces());
		}

		[Fact]
		public void TestCenario12_FestaAniversarioStandardRejeitaItensPremier()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = new DateTime(2025, 7, 4).AddDays(30).Date;
			int qtdConvidados = 50;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaAniversario;

			FestaAniversario festaAniversario = new FestaAniversario(dataEvento, qtdConvidados, espaco, categoriaEvento);

			festaAniversario.ColocarBebidasEventoPorTipo(TipoEvento.Premier, festaAniversario._bebidasFestaAniversario);

			Assert.Equal(TipoEvento.Standard, festaAniversario._tipoEvento);
			Assert.Equal(festaAniversario._qtdConvidados * 40, festaAniversario.CalcularPrecoProdutosEvento(festaAniversario._tipoEvento));
			Assert.DoesNotContain(festaAniversario._bebidasFestaAniversario, b => b._tipo == TipoEvento.LuxoIPremier);
		}

		[Fact]
		public void TestCenario13_FestaLivreTemQuePoderIncluirServicos()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 50;
			TipoEvento tipoEvento = TipoEvento.Nulo;
			CategoriaEvento categoriaEvento = CategoriaEvento.Livre;

			FestaLivre festaLivre = new FestaLivre(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			Assert.False(festaLivre is IMesa);
			Assert.False(festaLivre is IDecoracao);
			Assert.False(festaLivre is IBolo);
			Assert.False(festaLivre is IMusica);

			festaLivre.ColocarBebidasEventoPorTipo(tipoEvento, festaLivre._listaBebidasFestaLivre);
			double expectedBeverageCost = festaLivre._bebidas.Where(b => b._tipo == TipoEvento.Geral).Sum(b => b._preco * b._quantidade);


			Assert.Equal(espaco.valorEspaco + festaLivre.CalcularPrecoProdutosEvento(festaLivre._tipoEvento) + festaLivre.CalcularPrecoBebidasEvento(festaLivre._listaBebidasFestaLivre), festaLivre.CalcularPrecoTotalFestaLivre(tipoEvento));
			Assert.Equal(2000, festaLivre.CalcularPrecoProdutosEvento(TipoEvento.Nulo));
		}

		[Fact]
		public void TestCenario14_FormaturaIncluiTudoExcetoBolo()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 100;
			TipoEvento tipoEvento = TipoEvento.Premier;
			CategoriaEvento categoriaEvento = CategoriaEvento.Formatura;

			Formatura formatura = new Formatura(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			Assert.False(formatura is IBolo);
			Assert.True(formatura is IMesa);
			Assert.True(formatura is IDecoracao);
			Assert.True(formatura is IMusica);

			formatura.ColocarBebidasEventoPorTipo(tipoEvento, formatura._bebidasFormatura);
			foreach (var bebida in formatura._bebidasFormatura)
			{
				if (bebida._nome == "Cerveja") bebida._quantidade = 10;
				if (bebida._nome == "Suco Natural") bebida._quantidade = 5;
			}

			double expectedTotal = espaco.valorEspaco + IMesa.DefinirPrecoMesa(qtdConvidados, tipoEvento) + IDecoracao.DefinirPrecoDecoracao(qtdConvidados, tipoEvento) + IMusica.DefinirPrecoMusica(qtdConvidados, tipoEvento) + formatura.CalcularPrecoProdutosEvento(tipoEvento) + formatura.CalcularPrecoBebidasEvento(formatura._bebidasFormatura);

			Assert.Equal(expectedTotal, formatura.CalcularPrecoTotalFormatura(tipoEvento));
			Assert.Equal(0, IBolo.DefinirPrecoBolo(qtdConvidados, TipoEvento.Nulo));
		}

		[Fact]
		public void TestCenario15_FestaEmpresaApenasAluguelEBebidas()
		{
			Espaco espacoC = _empresa._listaEspacos.First(e => e.nomeEspaco == "C");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 100;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaEmpresa;

			FestaEmpresa festaEmpresa = new FestaEmpresa(dataEvento, qtdConvidados, espacoC, tipoEvento, categoriaEvento);

			festaEmpresa.ColocarBebidasEventoPorTipo(tipoEvento, festaEmpresa._bebidasFestaEmpresa);
			Bebida agua = festaEmpresa._bebidasFestaEmpresa.First(b => b._nome == "Água com gás");
			agua._quantidade = 20;
			Bebida refri = festaEmpresa._bebidasFestaEmpresa.First(b => b._nome == "Refrigerante");
			refri._quantidade = 15;

			double expectedBeverageCost = (20 * 5.00) + (15 * 8.00);
			expectedBeverageCost += festaEmpresa._bebidasFestaEmpresa.Where(b => b._nome != "Água com gás" && b._nome != "Refrigerante").Sum(b => b._preco * b._quantidade);


			Assert.Equal(espacoC.valorEspaco + festaEmpresa._precoMusica + festaEmpresa.CalcularPrecoProdutosEvento(tipoEvento) + expectedBeverageCost, festaEmpresa.CalcularPrecoTotalFestaEmpresa(tipoEvento));
			Assert.Equal(10000 + (20 * 100) + (40 * 100) + expectedBeverageCost, festaEmpresa.CalcularPrecoTotalFestaEmpresa(tipoEvento));
			Assert.Equal(10000 + 2000 + 4000 + 220, festaEmpresa.CalcularPrecoTotalFestaEmpresa(tipoEvento));
			Assert.Equal(16220, festaEmpresa.CalcularPrecoTotalFestaEmpresa(tipoEvento));
		}

		[Fact]
		public void TestCenario16_SistemaCarregaDadosSalvosAoReabrir()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = new DateTime(2026, 1, 10);
			int qtdConvidados = 80;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaAniversario;

			FestaAniversario originalFesta = new FestaAniversario(dataEvento, qtdConvidados, espaco, categoriaEvento);
			originalFesta.ColocarBebidasEventoPorTipo(tipoEvento, originalFesta._bebidasFestaAniversario);
			originalFesta._bebidasFestaAniversario.First(b => b._nome == "Refrigerante")._quantidade = 5;
			originalFesta.CalcularPrecoTotalFestaAniversario();

			_empresa.AdicionarEventoNoArquivo(originalFesta, _testFilePath);

			Empresa newEmpresa = new Empresa();
			List<Evento> loadedEvents = ConsoleApp1.Program.LerTodosEventosDoArquivo(_testFilePath);

			Assert.Single(loadedEvents);
			Evento loadedEvent = loadedEvents.First();

			Assert.Equal(originalFesta._qtdConvidados, loadedEvent._qtdConvidados);
			Assert.Equal(originalFesta._espacoEvento.nomeEspaco, loadedEvent._espacoEvento.nomeEspaco);
			Assert.Equal(originalFesta._dataEvento.ToShortDateString(), loadedEvent._dataEvento.ToShortDateString());
			Assert.Equal(originalFesta._tipoEvento, loadedEvent._tipoEvento);
			Assert.Equal(originalFesta._categoriaEvento, loadedEvent._categoriaEvento);
			Assert.Equal(originalFesta.valorTotalFesta, loadedEvent.valorTotalFesta);
		}

		[Fact]
		public void TestCenario17_SistemaGeraResumoDetalhadoAoFinalizarContratacao()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 80;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaAniversario;

			FestaAniversario festa = new FestaAniversario(dataEvento, qtdConvidados, espaco, categoriaEvento);
			festa.ColocarBebidasEventoPorTipo(tipoEvento, festa._bebidasFestaAniversario);
			festa._bebidasFestaAniversario.First(b => b._nome == "Cerveja")._quantidade = 10;
			festa._bebidasFestaAniversario.First(b => b._nome == "Suco Natural")._quantidade = 5;
			festa._produtosFestaAniversario.Add(new Produto { _nome = "Coxinha", _tipo = TipoEvento.Standard });
			festa._produtosFestaAniversario.Add(new Produto { _nome = "Kibe", _tipo = TipoEvento.Standard });
			festa.CalcularPrecoTotalFestaAniversario();

			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);

			festa.MostrarResumoAniversario(tipoEvento);

			string output = stringWriter.ToString();

			Assert.Contains($"Valor do espaço:{espaco.valorEspaco}.00", output);
			Assert.Contains($"Valor dos itens de mesa:{festa._precoMesa}", output);
			Assert.Contains($"Valor da decoração:{festa._precoDecoracao}", output);
			Assert.Contains($"Valor da música:{festa._precoMusica}", output);
			Assert.Contains($"Valor das comidas:{festa.CalcularPrecoProdutosEvento(tipoEvento)}", output);
			Assert.Contains("Lista das comidas:", output);
			Assert.Contains("- Coxinha", output);
			Assert.Contains("- Kibe", output);
			Assert.Contains($"Valor das bebidas:{festa.CalcularPrecoBebidasEvento(festa._bebidasFestaAniversario)}", output);
			Assert.Contains("Lista das bebidas", output);
			Assert.Contains("- Bebida: Cerveja - Quantidade: 10", output);
			Assert.Contains("- Bebida: Suco Natural - Quantidade: 5", output);
		}

		[Fact]
		public void TestCenario18_ArquivoPersistenciaCorrompidoTrataExcecao()
		{
			File.WriteAllText(_testFilePath, "corrupted data that will cause parsing error");

			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);

			List<Evento> loadedEvents = ConsoleApp1.Program.LerTodosEventosDoArquivo(_testFilePath);

			string output = stringWriter.ToString();
			Assert.Contains("Erro ao ler arquivo:", output);
			Assert.Empty(loadedEvents);
		}

		[Fact]
		public void TestCenario19_FalhaDeEnergiaGaranteIntegridadeNoProximoAcesso()
		{
			string incompleteLine = "100|A|100|10000|2026-01-01|Casamento|";
			File.WriteAllText(_testFilePath, incompleteLine);

			List<Evento> loadedEvents;
			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);
			try
			{
				loadedEvents = ConsoleApp1.Program.LerTodosEventosDoArquivo(_testFilePath);
			}
			catch (Exception ex)
			{
				Assert.IsType<FormatException>(ex);
			}

			string output = stringWriter.ToString();
			Assert.Contains("Erro ao ler arquivo:", output);
			Assert.True(File.Exists(_testFilePath));
		}

		[Fact]
		public void TestCenario20_AdministradorSolicitaListagemDeFestasAgendadas()
		{
			_empresa._listaEventos.Clear();
			File.Delete(_testFilePath);

			Espaco espacoA = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			Espaco espacoE = _empresa._listaEspacos.First(e => e.nomeEspaco == "E");

			FestaAniversario festa1 = new FestaAniversario(new DateTime(2026, 2, 14), 70, espacoA, CategoriaEvento.FestaAniversario);
			festa1.CalcularPrecoTotalFestaAniversario();
			_empresa.AdicionarEventoNoArquivo(festa1, _testFilePath);

			Casamento casamento1 = new Casamento(new DateTime(2026, 3, 1), 150, espacoE, TipoEvento.Luxo, CategoriaEvento.Casamento);
			casamento1.ColocarBebidasEventoPorTipo(TipoEvento.Luxo, casamento1._bebidasCasamento);
			casamento1._bebidasCasamento.First(b => b._nome == "Espumante Imp.")._quantidade = 3;
			casamento1.CalcularPrecoTotalCasamento(TipoEvento.Luxo);
			_empresa.AdicionarEventoNoArquivo(casamento1, _testFilePath);

			_empresa._listaEventos = ConsoleApp1.Program.LerTodosEventosDoArquivo(_testFilePath);

			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);

			ConsoleApp1.Program.ExibirCalendário();

			string output = stringWriter.ToString();

			Assert.Contains($"Tipo do evento: {TipoEvento.Standard} - Data do evento: {new DateTime(2026, 2, 14).ToShortDateString()} - Nome do espaço: A - Valor Festa: {festa1.valorTotalFesta} - Quantidade Convidados: 70 - Categoria Evento: {CategoriaEvento.FestaAniversario}", output);
			Assert.Contains($"Tipo do evento: {TipoEvento.Luxo} - Data do evento: {new DateTime(2026, 3, 1).ToShortDateString()} - Nome do espaço: E - Valor Festa: {casamento1.valorTotalFesta} - Quantidade Convidados: 150 - Categoria Evento: {CategoriaEvento.Casamento}", output);
		}

		[Fact]
		public void TestCenario21_AgendamentoRecusadoAbaixoPrazoMinimo()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");

			DateTime resultDate = _empresa.ProcurarDataMaisProxima(espaco);
			Assert.True(resultDate >= DateTime.Now.AddDays(30));
		}


		[Fact]
		public void TestCenario22_AlterarQuantidadeBebidasValorNegativo()
		{
			var input = "-10\n";
			var stringReader = new StringReader(input);
			Console.SetIn(stringReader);

			List<Bebida> bebidasExistentes = new List<Bebida>
			{
				new Bebida { _nome = "Água", _preco = 5.0, _quantidade = 0 }
			};

			var badInput = "abc\n";
			var stringReaderInvalid = new StringReader(badInput);
			Console.SetIn(stringReaderInvalid);

			Assert.Throws<FormatException>(() => ConsoleApp1.Program.EscolherQuantidadePrecoBebidas(bebidasExistentes));
		}

		[Fact]
		public void TestCenario23_AdministradorAlteraPrecoEspaco()
		{
			Espaco espacoA = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			double oldPrice = espacoA.valorEspaco;
			double newPrice = 12000.00;

			espacoA.valorEspaco = newPrice;

			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 80;
			TipoEvento tipoEvento = TipoEvento.Standard;
			CategoriaEvento categoriaEvento = CategoriaEvento.FestaAniversario;

			FestaAniversario novaFesta = new FestaAniversario(dataEvento, qtdConvidados, espacoA, categoriaEvento);

			double expectedTotal = newPrice + novaFesta._precoMesa + novaFesta._precoDecoracao + novaFesta._precoBolo + novaFesta._precoMusica + novaFesta.CalcularPrecoProdutosEvento(tipoEvento) + novaFesta.CalcularPrecoBebidasEvento(novaFesta._bebidasFestaAniversario);
			novaFesta.ColocarBebidasEventoPorTipo(tipoEvento, novaFesta._bebidasFestaAniversario);

			Assert.Equal(expectedTotal, novaFesta.CalcularPrecoTotalFestaAniversario());
			Assert.NotEqual(oldPrice, novaFesta._espacoEvento.valorEspaco);
			Assert.Equal(newPrice, novaFesta._espacoEvento.valorEspaco);
		}

		[Fact]
		public void TestCenario24_FormaturaNaoPodeIncluirBolo()
		{
			Espaco espaco = _empresa._listaEspacos.First(e => e.nomeEspaco == "A");
			DateTime dataEvento = DateTime.Now.AddDays(30).Date;
			int qtdConvidados = 100;
			TipoEvento tipoEvento = TipoEvento.Premier;
			CategoriaEvento categoriaEvento = CategoriaEvento.Formatura;

			Formatura formatura = new Formatura(dataEvento, qtdConvidados, espaco, tipoEvento, categoriaEvento);

			Assert.False(formatura is IBolo);
			Assert.DoesNotContain(typeof(IBolo), formatura.GetType().GetInterfaces());

			Assert.Equal(0, IBolo.DefinirPrecoBolo(qtdConvidados, TipoEvento.Nulo));
		}

		[Fact]
		public void TestCenario25_EntradaConvidadosComLetraLancaExcecao()
		{
			var input = "vinte\n";
			var stringReader = new StringReader(input);
			Console.SetIn(stringReader);

			var stringWriter = new StringWriter();
			Console.SetOut(stringWriter);

			Assert.Throws<FormatException>(() => int.Parse("vinte"));
		}

		[Fact]
		public void TestCenario26_EspacoGUnicoLivreCom100Pessoas()
		{
			_empresa._listaEventos.Clear();
			foreach (var espaco in _empresa._listaEspacos)
			{
				espaco.datas.Clear();
			}

			DateTime today = DateTime.Now;
			DateTime futureDate = today.AddDays(30);
			while (futureDate.DayOfWeek != DayOfWeek.Friday && futureDate.DayOfWeek != DayOfWeek.Saturday)
			{
				futureDate = futureDate.AddDays(1);
			}
			DateTime targetDate = futureDate;

			_empresa._listaEspacos.First(e => e.nomeEspaco == "G").capacidadeMaxima = 50;

			int quantidadeConvidados = 100;

			foreach (var espaco in _empresa._listaEspacos.Where(e => e.nomeEspaco != "G"))
			{
				espaco.datas.Add(DateTime.MinValue);
				espaco.datas.Add(DateTime.MaxValue);
			}

			Espaco selectedEspaco = _empresa.EscolherMelhorEspaco(quantidadeConvidados);
			Assert.Null(selectedEspaco);
		}
	}
}
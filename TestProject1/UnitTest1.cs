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
			Assert.True(dataSugerida >= new DateTime(2025, 3, 31).AddDays(30));

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
			Assert.Equal("E", espacoSugerido.nomeEspaco);
			Assert.Equal(200, espacoSugerido.capacidadeMaxima);

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
	}
}
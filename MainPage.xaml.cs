using EstadosCidades.Models;
using EstadosCidades.Services;

namespace EstadosCidades
{
    public partial class MainPage : ContentPage
    {
        private BrasilApiService _brasilApiService;

        public MainPage()
        {
            InitializeComponent();
            _brasilApiService = new BrasilApiService();
            CarregarEstados();
        }

        private async void CarregarEstados()
        {
            var estados = await _brasilApiService.BuscarEstados();
            PickerEstado.ItemsSource = estados.Select(e => e.Sigla).ToList();
        }

        private async void OnEstadoSelecionado(object sender, EventArgs e)
        {
            // Atribuir o valor selecionado como string
            var estadoSelecionado = PickerEstado.SelectedItem as string;

            if (estadoSelecionado != null)
            {
                // Corrigir para usar await e garantir que o método assíncrono seja aguardado
                var cidades = await _brasilApiService.BuscarCidadesPorEstado(estadoSelecionado);
                PickerCidade.ItemsSource = cidades.Select(c => c.Nome).ToList();
            }
        }


        private BrasilApiService Get_brasilApiService()
        {
            return _brasilApiService;
        }

        private async void OnCidadeSelecionada(object sender, EventArgs e)
        {
            var cidadeSelecionada = PickerCidade.SelectedItem as string;

            if (!string.IsNullOrEmpty(cidadeSelecionada))
            {
                // Corrigir para garantir que estamos pegando o estado selecionado corretamente
                var estadoSelecionado = PickerEstado.SelectedItem as string;

                // Certifique-se de que estamos usando await para métodos assíncronos
                var cidades = await _brasilApiService.BuscarCidadesPorEstado(estadoSelecionado);

                var cidade = cidades.FirstOrDefault(c => c.Nome == cidadeSelecionada);

                if (cidade != null)
                {
                    // Usar await para buscar a previsão da cidade de forma assíncrona
                    var previsao = await _brasilApiService.BuscarPrevisaoPorCidade(cidade.Id);

                    if (previsao != null && previsao.ClimaProximosDias.Any())
                    {
                        LabelPrevisaoAtual.Text = $"Previsão para {cidadeSelecionada}: {previsao.ClimaProximosDias.First().CondicaoDesc}";
                        ListaPrevisao.ItemsSource = previsao.ClimaProximosDias.Skip(1);
                    }
                }
            }
        }




    }


}

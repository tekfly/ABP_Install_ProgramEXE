using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABP_Install_ProgramEXE
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Dictionary<string, string>> _uipathData;
        private Dictionary<string, Dictionary<string, string>> _ExtraData;

        public MainWindow()
        {
            InitializeComponent();
            _ = CarregarDadosLocais();
        }

        // =========================
        // 1. DOWNLOAD JSON
        // =========================
        public async Task DownloadJsonFiles(string urlProducts, string urlExtra)
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFiles");

            try
            {
                if (!Directory.Exists(jsonPath))
                    Directory.CreateDirectory(jsonPath);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "C# App");

                    // Download do primeiro ficheiro
                    string conteudoProducts = await client.GetStringAsync(urlProducts);
                    File.WriteAllText(System.IO.Path.Combine(jsonPath, "versoes_uipath.json"), conteudoProducts);

                    // Download do segundo ficheiro
                    string conteudoExtra = await client.GetStringAsync(urlExtra);
                    File.WriteAllText(System.IO.Path.Combine(jsonPath, "extra_products_versions.json"), conteudoExtra);
                }

                MessageBox.Show("Todos os ficheiros JSON foram atualizados!");
                await CarregarDadosLocais(); // Carrega apenas uma vez no fim
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no download: {ex.Message}");
            }
        }





        // =========================
        // 2. LOAD JSON
        // =========================
        private async Task CarregarDadosLocais()
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonFiles");
            string mainPath = System.IO.Path.Combine(jsonPath, "versoes_uipath.json");
            string extraPath = System.IO.Path.Combine(jsonPath, "extra_products_versions.json");

            try
            {
                // Limpar painéis antes de carregar
                MainPairsPanel.Children.Clear();
                ExtraPairsPanel.Children.Clear();

                // 1. Carregar Main Products
                if (File.Exists(mainPath))
                {
                    string mainJson = File.ReadAllText(mainPath);
                    _uipathData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(mainJson);
                    AddMainPairRow(); // Adiciona a primeira linha de produtos principais
                }

                // 2. Carregar Extra Products
                if (File.Exists(extraPath))
                {
                    string extraJson = File.ReadAllText(extraPath);
                    _ExtraData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(extraJson);
                    AddExtraPairRow(); // Adiciona a primeira linha de produtos extra
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
            }
        }


        // =========================
        // 3. CREATE ROW MAIN PRODUCTS
        // =========================
        private void AddMainPairRow()
        {
            Grid rowGrid = new Grid
            {
                Margin = new Thickness(0, 5, 0, 5)
            };

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            // PRODUCT COMBO
            ComboBox comboProduto = new ComboBox
            {
                Margin = new Thickness(5)
            };

            // VERSION COMBO
            ComboBox comboVersao = new ComboBox
            {
                Margin = new Thickness(5)
            };

            // REMOVE BUTTON
            Button btnRemove = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5)
            };

            // Populate products
            if (_uipathData != null)
            {
                foreach (var produto in _uipathData.Keys)
                    comboProduto.Items.Add(produto);
            }

            // EVENT: product changed → update versions
            comboProduto.SelectionChanged += (s, e) =>
            {
                comboVersao.Items.Clear();

                if (comboProduto.SelectedItem == null) return;

                string produto = comboProduto.SelectedItem.ToString();

                if (_uipathData != null && _uipathData.ContainsKey(produto))
                {
                    foreach (var versao in _uipathData[produto].Keys)
                        comboVersao.Items.Add(versao);

                    comboVersao.SelectedIndex = 0;
                }
            };

            // EVENT: remove row
            btnRemove.Click += (s, e) =>
            {
                MainPairsPanel.Children.Remove(rowGrid);
            };

            // Add controls to grid
            Grid.SetColumn(comboProduto, 0);
            Grid.SetColumn(comboVersao, 1);
            Grid.SetColumn(btnRemove, 2);

            rowGrid.Children.Add(comboProduto);
            rowGrid.Children.Add(comboVersao);
            rowGrid.Children.Add(btnRemove);

            MainPairsPanel.Children.Add(rowGrid);
        }

        // =========================
        // 3.1 CREATE ROW EXTRA PRODUCTS #####################S
        // =========================
        private void AddExtraPairRow()
        {
            Grid rowGrid = new Grid
            {
                Margin = new Thickness(0, 5, 0, 5)
            };

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            // Extra COMBO
            ComboBox comboExtraProduto = new ComboBox
            {
                Margin = new Thickness(5)
            };

            // VERSION Extra COMBO
            ComboBox comboExtraVersao = new ComboBox
            {
                Margin = new Thickness(5)
            };

            // REMOVE BUTTON
            Button btnRemoveExtra = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5)
            };

            // Populate products
            if (_ExtraData != null)
            {
                foreach (var produto in _ExtraData.Keys)
                    comboExtraProduto.Items.Add(produto);
            }

            // EVENT: product changed → update versions
            comboExtraProduto.SelectionChanged += (s, e) =>
            {
                comboExtraVersao.Items.Clear();

                if (comboExtraProduto.SelectedItem == null) return;

                string produto = comboExtraProduto.SelectedItem.ToString();

                if (_ExtraData != null && _ExtraData.ContainsKey(produto))
                {
                    foreach (var versao in _ExtraData[produto].Keys)
                        comboExtraVersao.Items.Add(versao);

                    comboExtraVersao.SelectedIndex = 0;
                }
            };

            // EVENT: remove row
            btnRemoveExtra.Click += (s, e) =>
            {
                ExtraPairsPanel.Children.Remove(rowGrid);
            };

            // Add controls to grid
            Grid.SetColumn(comboExtraProduto, 0);
            Grid.SetColumn(comboExtraVersao, 1);
            Grid.SetColumn(btnRemoveExtra, 2);

            rowGrid.Children.Add(comboExtraProduto);
            rowGrid.Children.Add(comboExtraVersao);
            rowGrid.Children.Add(btnRemoveExtra);

            ExtraPairsPanel.Children.Add(rowGrid);
        }





        // =========================
        // 4. BUTTON EVENTS
        // =========================
        private async void BtnRefreshJson_Click(object sender, RoutedEventArgs e)
        {
            // ⚠️ IMPORTANT: replace with your actual raw GitHub JSON file
            string urlRawProducts = "https://raw.githubusercontent.com/tekfly/New_VisualUI_UIPATH/refs/heads/main/json_files/product_versions.json";
            string urlRawExtra = "https://raw.githubusercontent.com/tekfly/New_VisualUI_UIPATH/refs/heads/main/json_files/extra_products_versions.json";
            await DownloadJsonFiles(urlRawProducts, urlRawExtra);
        }

        private void AddMainPairBtn_Click(object sender, RoutedEventArgs e)
        {
            AddMainPairRow();
        }

        private void BtnInstalar_Click(object sender, RoutedEventArgs e)
        {
            if (_uipathData == null)
            {
                MessageBox.Show("Dados não carregados.");
                return;
            }

            List<string> resultados = new List<string>();

            foreach (Grid row in MainPairsPanel.Children)
            {
                ComboBox produtoCombo = row.Children[0] as ComboBox;
                ComboBox versaoCombo = row.Children[1] as ComboBox;

                if (produtoCombo?.SelectedItem == null || versaoCombo?.SelectedItem == null)
                    continue;

                string produto = produtoCombo.SelectedItem.ToString();
                string versao = versaoCombo.SelectedItem.ToString();

                if (_uipathData.ContainsKey(produto) &&
                    _uipathData[produto].ContainsKey(versao))
                {
                    string url = _uipathData[produto][versao];
                    resultados.Add($"{produto} {versao}\n{url}");
                }
            }

            if (resultados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um produto e versão.");
                return;
            }

            MessageBox.Show(string.Join("\n\n", resultados));

            // 👉 Aqui podes depois chamar downloads reais
        }

        // =========================
        // 5. OPTIONAL (Browse button)
        // =========================
        private void BrowseDownloadTargetBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.CheckFileExists = false;
            dialog.FileName = "Select Folder";

            if (dialog.ShowDialog() == true)
            {
                string folder = System.IO.Path.GetDirectoryName(dialog.FileName);
                DownloadTargetBox.Text = folder;
            }
        }


        // =========================
        // 6. OPTIONAL (Download button)
        // =========================
        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            string pastaDestino = DownloadTargetBox.Text;

            if (string.IsNullOrWhiteSpace(pastaDestino) || !Directory.Exists(pastaDestino))
            {
                MessageBox.Show("Por favor, selecione uma pasta de destino válida.");
                return;
            }

            var botao = sender as Button;
            botao.IsEnabled = false;

            try
            {
                // 1. PROCESSAR PRODUTOS PRINCIPAIS
                if (_uipathData != null)
                {
                    foreach (Grid row in MainPairsPanel.Children)
                    {
                        await ProcessarLinhaDownload(row, _uipathData, pastaDestino);
                    }
                }

                // 2. PROCESSAR PRODUTOS EXTRA
                if (_ExtraData != null)
                {
                    foreach (Grid row in ExtraPairsPanel.Children)
                    {
                        await ProcessarLinhaDownload(row, _ExtraData, pastaDestino);
                    }
                }

                MessageBox.Show("Todos os downloads (Principais e Extras) foram concluídos!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante o download: {ex.Message}");
            }
            finally
            {
                botao.IsEnabled = true;
                DownloadProgressBar.Value = 0;
            }
        }

        // Método auxiliar para evitar repetição de código
        private async Task ProcessarLinhaDownload(Grid row, Dictionary<string, Dictionary<string, string>> fonteDados, string pasta)
        {
            var produtoCombo = row.Children[0] as ComboBox;
            var versaoCombo = row.Children[1] as ComboBox;

            if (produtoCombo?.SelectedItem == null || versaoCombo?.SelectedItem == null) return;

            string produto = produtoCombo.SelectedItem.ToString();
            string versao = versaoCombo.SelectedItem.ToString();

            if (fonteDados.ContainsKey(produto) && fonteDados[produto].ContainsKey(versao))
            {
                string url = fonteDados[produto][versao];
                string nomeFicheiro = System.IO.Path.GetFileName(new Uri(url).LocalPath);

                // Caso o URL não termine com o nome do ficheiro, gera um nome padrão
                if (string.IsNullOrEmpty(nomeFicheiro) || !nomeFicheiro.Contains("."))
                    nomeFicheiro = $"{produto}_{versao}.exe";

                string caminhoCompleto = System.IO.Path.Combine(pasta, nomeFicheiro);
                await BaixarFicheiroComProgresso(url, caminhoCompleto);
            }
        }


        private async Task BaixarFicheiroComProgresso(string url, string destino)
        {
            using (HttpClient client = new HttpClient())
            {
                // Obtém apenas o cabeçalho primeiro para saber o tamanho total
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destino, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalLido = 0;
                        int lido;

                        while ((lido = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, lido);
                            totalLido += lido;

                            if (totalBytes.HasValue)
                            {
                                // Calcula a percentagem e atualiza a UI
                                double progresso = (double)totalLido / totalBytes.Value * 100;
                                DownloadProgressBar.Value = progresso;
                            }
                        }
                    }
                }
            }
        }

        private void DownloadTargetBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InstallerFolderBox.Text = DownloadTargetBox.Text;
        }

        private void ValidateInstallerBtn_Click(object sender, RoutedEventArgs e)
        {
            string filespath = InstallerFolderBox.Text;
            Validate_InstallerFiles(filespath);
        }

        public void Validate_InstallerFiles(string filespath)
        {
            if (filespath == null || !Directory.Exists(filespath))
            {
                MessageBox.Show("Por favor, selecione uma pasta válida para validação.");
                return;
            }
            string filetoinstall = filespath + "\\UiPathStudio.msi";
            if (File.Exists(filetoinstall))
            {
                MessageBox.Show("Ficheiro de instalação encontrado: " + filetoinstall);
                // Aqui podes adicionar código para iniciar a instalação, se necessário
                NextToInstallBtn.IsEnabled = true; // Habilita o botão de instalação
            }
            else
            {
                MessageBox.Show("Ficheiro de instalação não encontrado em: " + filetoinstall);
            }
        }

        private void BrowseInstallerFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.CheckFileExists = false;
            dialog.FileName = "Select Folder";

            if (dialog.ShowDialog() == true)
            {
                string folder = System.IO.Path.GetDirectoryName(dialog.FileName);
                InstallerFolderBox.Text = folder;
            }
        }

        private void NextToInstallBtn_Click(object sender, RoutedEventArgs e)
        {
            PopularListaFicheiros();
            Screen1Grid.Visibility = Visibility.Collapsed;
            Screen2Grid.Visibility = Visibility.Visible;
        }


        // ===============================================================================================================================================================================
        // 6. SCREEN 2 
        // ===============================================================================================================================================================================

        private void BackToScreen1Btn_Click(object sender, RoutedEventArgs e)
        {
            Screen1Grid.Visibility = Visibility.Visible;
            Screen2Grid.Visibility = Visibility.Collapsed;
        }
        public void PopularListaFicheiros()
        {
            string pasta = InstallerFolderBox.Text; // Use o caminho definido no Screen 1

            if (string.IsNullOrWhiteSpace(pasta) || !Directory.Exists(pasta))
            {
                // Se a pasta não existe, limpa a lista e sai
                FilesListView.ItemsSource = null;
                return;
            }

            // Busca ficheiros .msi e .exe
            var ficheiros = Directory.EnumerateFiles(pasta, "*.*")
                // Filtra apenas os ficheiros que terminam com .msi ou .exe (ignorando maiúsculas/minúsculas) - pode-se adicionar mais extensões se necessário
                .Where(f => f.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
                .Select(f => new InstallFile
                {
                    FileName = System.IO.Path.GetFileName(f),
                    FullPath = f,
                    IsSelected = true
                })
                .ToList();
            FilesListView.ItemsSource = ficheiros;
        }
        public class InstallFile
        {
            public bool IsSelected { get; set; } = true;
            public string FileName { get; set; }
            public string FullPath { get; set; }
        }

        private void AddExtraPairBtn_Click(object sender, RoutedEventArgs e)
        {
            AddMainPairRow();
            AddExtraPairRow();
        }

        private void GenerateScriptBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = (FilesListView.ItemsSource as List<InstallFile>)?.Where(f => f.IsSelected).ToList();

            if (selectedFiles == null || !selectedFiles.Any())
            {
                MessageBox.Show("Selecione pelo menos um ficheiro da lista.");
                return;
            }

            string script = "# Script Gerado para Instalação\n";
            script += "$ProgressPreference = 'SilentlyContinue'\n\n";

            foreach (var file in selectedFiles)
            {
                string extensao = System.IO.Path.GetExtension(file.FullPath).ToLower();

                script += $"# Instalando: {file.FileName}\n";

                if (extensao == ".msi")
                {
                    // Opções UiPath MSI (Quiet, Service Mode, etc)
                    string msiArgs = "/i \"" + file.FullPath + "\" /quiet /passive /norestart";

                    if (ServiceModeCheck1.IsChecked == true) msiArgs += " ADDLOCAL=DesktopFeature,Robot,RegisterService";
                    else msiArgs += " ADDLOCAL=DesktopFeature,Robot,UserMode";

                    if (StudioInstallCheck1.IsChecked == true) msiArgs += ",Studio";
                    if (AllUsersCheck1.IsChecked == true) msiArgs += " ALLUSERS=1";

                    script += $"Start-Process msiexec.exe -ArgumentList '{msiArgs}' -Wait\n\n";
                }
                else if (extensao == ".exe")
                {
                    // Opções padrão para EXE (geralmente /S ou /silent)
                    script += $"Start-Process \"{file.FullPath}\" -ArgumentList '/S /silent' -Wait\n\n";
                }
                else if (extensao == ".ps1")
                {
                    // Executar script PowerShell externo
                    script += $"& \"{file.FullPath}\"\n\n";
                }
            }

            ScriptOutputBox.Text = script;
        }

        private void RunInstallerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ScriptOutputBox.Text))
            {
                MessageBox.Show("Gere o script primeiro!");
                return;
            }

            try
            {
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "install_temp.ps1");
                File.WriteAllText(tempPath, ScriptOutputBox.Text);

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{tempPath}\"",
                    UseShellExecute = true,
                    Verb = "runas" // Força a execução como Administrador
                };

                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar instalador: {ex.Message}");
            }
        }

    }
}

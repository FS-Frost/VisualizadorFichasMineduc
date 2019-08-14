using KBCsv;
using Microsoft.Win32;
using MineducRbd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace MineducRbdViewer {
    public partial class MainWindow : Window {
        ObservableCollection<School> ListSchools = new ObservableCollection<School>();
        CancellationTokenSource cancellationTokenSourceLoadRbd;
        CancellationToken cancellationTokenLoadRbd;

        public MainWindow() {
            InitializeComponent();
            Utils.SetWindowMinSize(this);
            TaskbarItemInfo = new TaskbarItemInfo();
            
            // Components.
            dgData.ItemsSource = ListSchools;
            dgData.HeadersVisibility = DataGridHeadersVisibility.Column;
            dgData.AutoGenerateColumns = true;
            dgData.AutoGeneratingColumn += DgData_AutoGeneratingColumn;
            HiddeCancelButton();
            DisableSaveButton();

            // Events.
            btnLoadRbd.Click += BtnLoadRbd_Click;
            btnCancelLoadRbd.Click += BtnCancelLoadRbd_Click;
            btnSave.Click += BtnSave_Click;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            var exitAnyway = ProcessExistsAndCancel();

            if (!exitAnyway) {
                e.Cancel = true;
            }
        }

        private void CancelProcess() {
            if (cancellationTokenSourceLoadRbd != null && !cancellationTokenSourceLoadRbd.IsCancellationRequested) {
                cancellationTokenSourceLoadRbd.Cancel();
            }
        }

        private bool ProcessExistsAndCancel() {
            if (cancellationTokenSourceLoadRbd != null && !cancellationTokenSourceLoadRbd.IsCancellationRequested) {
                var result = MessageBox.Show(
                    "Hay un proceso en ejecución. ¿Cancelarlo para proseguir?",
                    "Advertencia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                switch (result) {
                    case MessageBoxResult.Yes:
                        CancelProcess();
                        return true;
                    case MessageBoxResult.No:
                        return false;
                }
            }

            return true;
        }

        private void DgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor) {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }

        private void BtnCancelLoadRbd_Click(object sender, RoutedEventArgs e) {
            if (cancellationTokenLoadRbd.CanBeCanceled) {
                HiddeCancelButton();
                EnableSaveButton();
                cancellationTokenSourceLoadRbd.Cancel();

                Utils.DoOnBackground(() => {
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    lblNotification.Content = string.Empty;
                });
            }
        }

        private void BtnLoadRbd_Click(object sender, RoutedEventArgs e) {
            var dialog = new OpenFileDialog() {
                Title = "Cargar listado RBD",
                DefaultExt = ".csv",
                Filter = "CSV (*.csv)|*csv|Todos los archivos|*.*"
            };

            if (dialog.ShowDialog() == true) {
                var path = dialog.FileName;
                LoadRbdFromCsv(path);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            var text = DataToCsv(";");
            var fileName = string.Format("Ficha RBD - {0}", DateTime.Now.ToShortDateString());

            var dialog = new SaveFileDialog() {
                Title = "Guardar fichas RBD",
                DefaultExt = ".csv",
                FileName = fileName,
                Filter = "CSV (*.csv)|*.csv|Texto plano (*.txt)|*.txt|Todos los archivos|*.*"
            };

            if (dialog.ShowDialog() == true) {
                File.WriteAllText(dialog.FileName, text, Encoding.UTF8);
            }
        }

        private void SearchSchool(int rbd) {
            var school = Connector.GetSchoolData(rbd);
            Dispatcher.Invoke(() => {
                ListSchools.Add(school);
            });
        }

        private string DataToCsv(string separator) {
            var text = string.Format("RBD{0}Dirección{0}Mapa{0}Comuna{0}Teléfono{0}Página Web{0}E-mail contacto{0}Director{0}Sostenedor{0}Estado\n",
                separator);

            foreach (var school in ListSchools) {
                text += string.Format(
                    "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}\n",
                    separator,
                    school.Rbd,
                    school.Direccion,
                    school.Mapa,
                    school.Comuna,
                    school.Telefono,
                    school.PaginaWeb,
                    school.Correo,
                    school.Director,
                    school.Sostenedor,
                    school.Estado
                );
            }

            return text;
        }

        private async void LoadRbdFromCsv(string path) {
            ListSchools.Clear();
            var listRdb = new List<int>();

            using (var streamReader = new StreamReader(path))
            using (var csvReader = new CsvReader(streamReader)) {
                csvReader.ReadHeaderRecord();

                if (!csvReader.HeaderRecord.Contains("RBD")) {
                    MessageBox.Show(
                            "El archivo seleccionado no contiene la columna RBD.",
                            "Error de lectura",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    return;
                }

                while (csvReader.HasMoreRecords) {
                    var record = csvReader.ReadDataRecord();
                    var parsed = int.TryParse(record["RBD"], out var rbd);

                    if (parsed) {
                        listRdb.Add(rbd);
                    }
                }
            }

            cancellationTokenSourceLoadRbd = new CancellationTokenSource();
            cancellationTokenLoadRbd = cancellationTokenSourceLoadRbd.Token;
            ShowCancelButton();
            DisableSaveButton();

            for (int i = 0; i < listRdb.Count; i++) {
                if (cancellationTokenLoadRbd.IsCancellationRequested) {
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;

                    if (ListSchools.Count > 0) {
                        EnableSaveButton();
                    }
                    return;
                }

                var progress = i / (double)listRdb.Count;

                Utils.DoOnBackground(() => {
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    TaskbarItemInfo.ProgressValue = progress;
                    lblNotification.Content = string.Format("Obteniendo fichas: {0} de {1}", i + 1, listRdb.Count);
                });

                var rbd = listRdb[i];
                // await Task.Factory.StartNew(() => SearchSchool(rbd), cancellationTokenLoadRbd);
                await Task.Run(() => SearchSchool(rbd), cancellationTokenLoadRbd);
            }

            HiddeCancelButton();
            EnableSaveButton();
            cancellationTokenSourceLoadRbd.Cancel();
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            lblNotification.Content = string.Empty;
            MessageBox.Show("Datos cargados correctamente.", "Proceso finalizado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowCancelButton() {
            btnCancelLoadRbd.Visibility = Visibility.Visible;
        }

        private void HiddeCancelButton() {
            btnCancelLoadRbd.Visibility = Visibility.Hidden;
        }

        private void EnableSaveButton() {
            btnSave.IsEnabled = true;
        }

        private void DisableSaveButton() {
            btnSave.IsEnabled = false;
        }
    }
}

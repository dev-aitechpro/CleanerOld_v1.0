using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace CleanerProWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public class CleanTask : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Path { get; set; }
            private string _size = "не проанализировано";
            public string Size { get => _size; set { _size = value; OnPropertyChanged(nameof(Size)); } }
            private bool _selected = true;
            public bool Selected { get => _selected; set { _selected = value; OnPropertyChanged(nameof(Selected)); } }
            public string Icon { get; set; }
            public bool IsAction { get; set; }
            public Action CleanAction { get; set; }
            public Func<long> GetSizeFunc { get; set; }
            public Label SizeLabel { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<CleanTask> Tasks { get; set; } = new ObservableCollection<CleanTask>();

        private string _totalSizeText = "📊 Общий размер: (нажмите АНАЛИЗ)";
        public string TotalSizeText { get => _totalSizeText; set { _totalSizeText = value; OnPropertyChanged(nameof(TotalSizeText)); } }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeTasks();
            CheckLicense();
        }

        private void InitializeTasks()
        {
            Tasks.Add(new CleanTask
            {
                Name = "Временные файлы пользователя (%TEMP%)",
                Path = Environment.GetEnvironmentVariable("TEMP"),
                Icon = "🗑️",
                GetSizeFunc = () => GetDirectorySize(Environment.GetEnvironmentVariable("TEMP"))
            });
            Tasks.Add(new CleanTask
            {
                Name = "Системные временные файлы (Windows\\Temp)",
                Path = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Temp"),
                Icon = "🗑️",
                GetSizeFunc = () => GetDirectorySize(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Temp"))
            });
            Tasks.Add(new CleanTask
            {
                Name = "Кэш обновлений (SoftwareDistribution\\Download)",
                Path = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "SoftwareDistribution", "Download"),
                Icon = "🔄",
                GetSizeFunc = () => GetDirectorySize(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "SoftwareDistribution", "Download"))
            });
            Tasks.Add(new CleanTask
            {
                Name = "Prefetch (ускоритель запуска)",
                Path = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Prefetch"),
                Icon = "⚡",
                GetSizeFunc = () => GetDirectorySize(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Prefetch"))
            });
            Tasks.Add(new CleanTask
            {
                Name = "Кэш шейдеров NVIDIA",
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "GLCache"),
                Icon = "🎮",
                GetSizeFunc = () => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "GLCache"))
            });
            Tasks.Add(new CleanTask
            {
                Name = "Кэш шейдеров AMD",
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "ShaderCache"),
                Icon = "🎮",
                GetSizeFunc = () => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "ShaderCache"))
            });
            // Действия
            Tasks.Add(new CleanTask
            {
                Name = "Сброс DNS-кэша",
                Icon = "🌐",
                IsAction = true,
                Selected = true,
                CleanAction = () => { Process.Start("ipconfig", "/flushdns").WaitForExit(); AppendLog("✅ DNS-кэш сброшен."); },
                Size = "—"
            });
            Tasks.Add(new CleanTask
            {
                Name = "Очистка истории Проводника",
                Icon = "📂",
                IsAction = true,
                Selected = true,
                CleanAction = () =>
                {
                    try
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", false);
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", false);
                        AppendLog("✅ История Проводника очищена.");
                    }
                    catch { AppendLog("⚠️ Не удалось полностью очистить историю (возможно, уже пусто)."); }
                },
                Size = "—"
            });
            Tasks.Add(new CleanTask
            {
                Name = "Запуск очистки диска (cleanmgr)",
                Icon = "🧹",
                IsAction = true,
                Selected = true,
                CleanAction = () => { Process.Start("cleanmgr.exe", "/sageset:1"); AppendLog("✅ Открыто окно очистки диска."); },
                Size = "—"
            });
        }

        // ---------- ОБРАБОТЧИКИ КНОПОК ----------
        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            TxtLog.Clear();
            AppendLog("🔍 Начинаю анализ...");
            long totalSize = 0;
            int processed = 0;

            var folderTasks = Tasks.Where(t => t.GetSizeFunc != null).ToList();
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Maximum = folderTasks.Count;
            ProgressBar.Value = 0;

            foreach (var task in folderTasks)
            {
                if (!Directory.Exists(task.Path))
                    task.Size = "❌ не найдена";
                else
                {
                    try
                    {
                        long size = task.GetSizeFunc();
                        task.Size = FormatSize(size);
                        if (task.Selected) totalSize += size;
                    }
                    catch (Exception ex)
                    {
                        task.Size = "⚠️ ошибка";
                        AppendLog($"⚠️ {task.Name}: {ex.Message}");
                    }
                }
                ProgressBar.Value = ++processed;
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
            }

            TotalSizeText = $"📊 Общий размер выбранных папок: {FormatSize(totalSize)}";
            AppendLog($"✅ Анализ завершён. Освобождаемый объём: {FormatSize(totalSize)}");
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void BtnClean_Click(object sender, RoutedEventArgs e)
        {
            var selectedTasks = Tasks.Where(t => t.Selected && ((!t.IsAction && Directory.Exists(t.Path)) || t.IsAction)).ToList();

            if (selectedTasks.Count == 0)
            {
                MessageBox.Show("Нет задач для очистки.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string message = "Вы собираетесь выполнить следующие действия:\n\n";
            foreach (var t in selectedTasks)
            {
                if (t.IsAction)
                    message += $"• {t.Icon} {t.Name}\n";
                else
                    message += $"• {t.Icon} {t.Name}\n   Путь: {t.Path}\n   Размер: {t.Size}\n";
                message += "\n";
            }
            message += "Продолжить?";

            var result = MessageBox.Show(message, "Подтверждение очистки", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            TxtLog.Clear();
            AppendLog("🧹 Начинаю очистку...");
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Maximum = selectedTasks.Count;
            ProgressBar.Value = 0;

            foreach (var task in selectedTasks)
            {
                try
                {
                    if (task.IsAction)
                    {
                        AppendLog($"Выполняю: {task.Name}");
                        task.CleanAction?.Invoke();
                        task.Size = "✅ выполнено";
                    }
                    else
                    {
                        AppendLog($"Удаляю: {task.Name} ({task.Path})");
                        DeleteDirectoryContents(task.Path);
                        task.Size = "✅ очищено";
                        AppendLog($"✅ {task.Name} – очищено!");
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"❌ Ошибка: {ex.Message}");
                }
                ProgressBar.Value++;
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
            }

            AppendLog("✅ Очистка завершена.");
            ProgressBar.Visibility = Visibility.Collapsed;
            BtnAnalyze_Click(null, null);
        }

        private void SocialButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is string url)
                Process.Start(url);
        }

        // ---------- ОБРАБОТЧИК ДВОЙНОГО КЛИКА ПО КАРТОЧКЕ (исправленный) ----------
        private void TaskBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var border = sender as Border;
                if (border?.DataContext is CleanTask task)
                {
                    if (task.IsAction)
                    {
                        string description;
                        if (task.Name.Contains("DNS"))
                            description = "Сбрасывает кэш DNS, что может помочь при проблемах с доступом к сайтам.";
                        else if (task.Name.Contains("истории"))
                            description = "Очищает историю недавних файлов и папок в Проводнике.";
                        else if (task.Name.Contains("cleanmgr"))
                            description = "Запускает встроенную утилиту очистки диска Windows.";
                        else
                            description = "Выполнение системного действия.";

                        string message = "Вы собираетесь выполнить действие:\n\n" +
                                         $"{task.Icon} {task.Name}\n\n" +
                                         $"Описание: {description}\n\n" +
                                         "Продолжить?";

                        var result = MessageBox.Show(message, "Подтверждение действия",
                                                     MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            task.CleanAction?.Invoke();
                            task.Size = "✅ выполнено";
                            AppendLog($"✅ Действие '{task.Name}' выполнено.");
                        }
                        else
                        {
                            AppendLog($"⏹️ Действие '{task.Name}' отменено пользователем.");
                        }
                    }
                    else if (!string.IsNullOrEmpty(task.Path) && Directory.Exists(task.Path))
                    {
                        Process.Start("explorer.exe", task.Path);
                    }
                    else
                    {
                        MessageBox.Show($"Папка не существует:\n{task.Path}", "Не найдено", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        // ---------- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ----------
        private void AppendLog(string text)
        {
            TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            TxtLog.ScrollToEnd();
        }

        private long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path)) return 0;
            return new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
        }

        private string FormatSize(long bytes)
        {
            if (bytes == 0) return "0 байт";
            if (bytes > 1024L * 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024 * 1024 * 1024):0.##} ТБ";
            if (bytes > 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024 * 1024):0.##} ГБ";
            if (bytes > 1024L * 1024) return $"{bytes / (1024.0 * 1024):0.##} МБ";
            if (bytes > 1024) return $"{bytes / 1024.0:0.##} КБ";
            return $"{bytes} байт";
        }

        private void DeleteDirectoryContents(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                try { File.Delete(file); } catch { }
            foreach (var dir in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
                try { Directory.Delete(dir, true); } catch { }
        }

        // ---------- ЛИЦЕНЗИЯ ----------
        private void CheckLicense()
        {
            const string keyPath = @"Software\PavelDaily\CleanerPro";
            const string valueName = "LicenseAccepted";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                if (key != null && key.GetValue(valueName) is int accepted && accepted == 1)
                    return;
            }

            string licenseText =
                "ЛИЦЕНЗИОННОЕ СОГЛАШЕНИЕ\n\n" +
                "1. Данное ПО создано Нейрокод | Neuralis (Dev_Ai Tech).\n" +
                "2. ПО предоставляется БЕСПЛАТНО для ЛИЧНОГО НЕКОММЕРЧЕСКОГО использования.\n" +
                "3. ЗАПРЕЩАЕТСЯ:\n" +
                "   - Использование в коммерческих целях.\n" +
                "   - Распространение, продажа, передача третьим лицам без разрешения автора.\n" +
                "   - Модификация и выдача за своё ПО.\n" +
                "4. Автор не несёт ответственности за потерю данных.\n" +
                "5. Используя ПО, вы соглашаетесь с условиями.\n\n" +
                "© 2026 Нейрокод | Neuralis (Dev_Ai Tech). Все права защищены.";

            var result = MessageBox.Show(licenseText, "📜 Лицензионное соглашение",
                MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
                {
                    key.SetValue(valueName, 1, RegistryValueKind.DWord);
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
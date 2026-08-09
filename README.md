🧹 CleanerOld v1.0

[Version](https://img.shields.io/badge/version-1.0-blue.svg)](https://github.com/dev-aitechpro/CleanerOld_v1.0)
[.NET](https://img.shields.io/badge/.NET-4.8-purple.svg)](https://dotnet.microsoft.com/)
[License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[Windows](https://img.shields.io/badge/Windows-7%2B-0078D6.svg)](https://www.microsoft.com/windows)
> CleanerOld v1.0 — лёгкая утилита для очистки системы Windows с винтажным интерфейсом в стиле 2000-х.
Описание
CleanerOld v1.0 — это простая, но мощная программа для очистки вашей системы от "цифрового мусора". Приложение позволяет контролировать каждый шаг очистки, а его интерфейс выполнен в стиле старых добрых программ 2000-х, при этом вся функциональность полностью сохранена.

Функция
Временные файлы Очистка системных временных папок Windows
Кэш обновлений Удаление файлов кэша Центра обновления Windows
Prefetch Очистка кэша для ускорения загрузки приложений
Шейдеры GPU Очистка кэша шейдеров видеокарты
DNS Сброс кэша DNS для обновления сетевых настроек
Проводник Очистка истории недавних документов и папок

Установка
Способ 1: Портативная версия (рекомендуется для тестирования)
1. Скачайте CleanerOld_v1.0_Portable.zip из раздела [Releases](https://github.com/dev-aitechpro/CleanerOld_v1.0/releases)
2. Распакуйте архив в любую папку
3. Запустите RUN.bat или CleanerProWPF.exe
4. Готово! Никакой установки не требуется.

Способ 2: Установщик (для постоянного использования)
1. Скачайте CleanerOld_v1.0_Setup.zip из раздела [Releases](https://github.com/dev-aitechpro/CleanerOld_v1.0/releases)
2. Распакуйте архив
3. Запустите INSTALL.bat от имени администратора
4. Программа установится в C:\Program Files\CleanerOld_v1.0

Способ 3: Сборка из исходников
bash
Клонируйте репозиторий
git clone https://github.com/dev-aitechpro/CleanerOld_v1.0.git
Откройте решение в Visual Studio
CleanerProWPF.slnx
Восстановите пакеты NuGet
Соберите проект в конфигурации Release

Системные требования
Компонент Требование
Операционная система Windows 7 и выше
.NET Framework 4.8 и выше
Архитектура x64
Права Администратор (рекомендуется)
📁 Структура проекта
CleanerProWPF/
├── Properties/          Настройки и информация о сборке
│   └── AssemblyInfo.cs  Автор: Нейрокод Neuralis (Dev_Ai Tech)
├── Resources/           Шрифты и ресурсы
│   ├── Noto/           Шрифты Noto Sans
│   └── Roboto/         Шрифты Roboto
├── MainWindow.xaml      Главное окно (интерфейс)
├── MainWindow.xaml.cs   Основная логика приложения
├── App.xaml            Точка входа
├── App.xaml.cs         Логика запуска
├── App.config          Конфигурация .NET
├── app.manifest        Манифест для Windows
└── packages.config     Зависимости NuGet

Технологии
Технология Версия
[C](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white) .NET Framework 4.8
[WPF](https://img.shields.io/badge/WPF-5C2D91?style=flat&logo=windows&logoColor=white) Windows Presentation Foundation
[Material Design](https://img.shields.io/badge/MaterialDesign-757575?style=flat&logo=material-design&logoColor=white) Material Design Themes 5.3.2

Разработка
Подготовка окружения
1. Установите Visual Studio 2019/2022 с компонентом ".NET desktop development"
2. Установите .NET Framework 4.8
3. Клонируйте репозиторий

Сборка проекта
bash
Через Visual Studio
Откройте CleanerProWPF.slnx → Построить → Построить решение
Через командную строку
dotnet build -c Release
Создание релизных сборок
bash
Портативная версия
.\Build-Portable.ps1
Установщик
.\Build-Installer.ps1

Вклад в проект
Мы приветствуем вклад в развитие проекта!
1. Сделайте форк репозитория
2. Создайте ветку для изменений: git checkout -b feature/your-feature
3. Внесите изменения
4. Проверьте сборку в Release
5. Создайте Pull Request

👤 Автор
Нейрокод | Neuralis (Dev_Ai Tech)
- Telegram: [@dev_aitech](https://t.me/dev_aitech)
- Поддержать: [Boosty](https://boosty.to/kpavels1997/donate)
- GitHub: [dev-aitechpro](https://github.com/dev-aitechpro)

📄 Лицензия
Этот проект распространяется под лицензией MIT. Подробнее см. в файле [LICENSE](LICENSE). Благодарности
- [Material Design for WPF](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) за отличные темы оформления
- Всем пользователям за поддержку и обратную связь

История изменений
v1.0.0 (2026-08-09)
- Первый стабильный релиз
- Все основные функции очистки
- Портативная версия и установщик
Если вам понравился проект, поставьте звезду на GitHub!

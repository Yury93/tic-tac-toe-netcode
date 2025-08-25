# Tic-Tac-Toe (Крестики-нолики) на Unity
Многопользовательская игра в крестики-нолики с сетевым взаимодействием через Unity Services (Lobby и Relay).

# Архитектура проекта
Проект построен по паттерну MVVM (Model-View-ViewModel) с использованием Zenject для dependency injection.

# Основные компоненты
Модели
BoardModel - управляет состоянием игрового поля (3x3)
CellModel - представляет отдельную ячейку на поле
StateCell - перечисление состояний ячейки (Empty, Player1, Player2)
MatchResult - перечисление результатов игры

# Представления
BoardView - визуальное представление игрового поля
Cross - визуальное представление крестика/нолика
ViewModel
BoardViewModel - связывает модель и представление, обрабатывает логику игры

# Сетевое взаимодействие
NetworkSevice - управляет сетевым состоянием игры через Netcode for GameObjects
NetworkMediator - посредник между ViewModel и сетевым сервисом
LobbyService - управляет созданием и подключением к лобби

# Инфраструктура
State Machine - система управления состояниями приложения
GameFactory - фабрика для создания игровых объектов
AssetProvider - загрузка ресурсов через Addressables

# Установка и запуск
Убедитесь, что установлены необходимые пакеты Unity:
Netcode for GameObjects
Unity Services (Authentication, Lobby, Relay)
Zenject (Dependency Injection)
R3 (Reactive Extensions)
Настройте Unity Services в проекте:
Перейдите в Window → Unity Services
Создайте или подключите проект
Активируйте сервисы Authentication, Lobby и Relay
Импортируйте необходимые ассеты через Addressables

# Как играть
Запустите игру
Нажмите "Join Lobby" для поиска или создания лобби
Дождитесь подключения второго игрока 

# Особенности реализации
Сетевая игра через Unity Relay
Система лобби для подключения игроков
Реактивное программирование с R3
Dependency Injection через Zenject
Addressables для управления ресурсами
Состояние игры синхронизируется между клиентами

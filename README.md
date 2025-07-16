English:

Demo: https://youtu.be/KDpO6-fTt54

Build Location: "Release Build/v1"

Brief Information:

Architectural Decisions:
- Scalable architecture based on MVVM + FSM UI
- The game has a defined entry point where basic initialization takes place
- Each scene also has its own entry point and DI container, which is released when the scene is unloaded
- A loading scene handles and displays the progress of an easily extendable set of loading operations – for example, SDK initialization for consoles

Features:
- Automatic detection of the input device type
- Dynamic replacement of control button icons + animation when corresponding buttons are pressed
- Custom UI element – a horizontal option selector (inherits from Selectable) – easily integrates into UI navigation
- Simple gameplay scene with the ability to save and load state (for demonstrating Save/Load functionality)
- Auto-scroll when a selected element goes outside the viewport
- Adaptive control info bar (at the bottom of the screen), adjusts based on selected screen and currently selected UI element

Русский:

Демонстрация: https://youtu.be/KDpO6-fTt54

Билд в папке "Release Build/v1"

Краткая информация:

Архитектурные решения:
- Масштабируемая архитектура, основанная на MVVM + FSM UI
- Игра имеет конкретную точку входа, в ней производятся базовые настройки
- Каждая сцена также обладает точкой входа, DI контейнером, который высвобождается при выгрузке сцены
- Загрузочная сцена обрабатывает и отображает прогресс легко расширяемого набора загрузочных операции - например, инициализация консольного SDK

Фичи:
- Автоопределение типа устройства ввода
- Подмена иконок кнопкок управления + их анимация при нажатии соответствующих кнопок
- Кастомный UI элемент - горизонтальный селектор опций (наследуется от  Selectable) - легко встраивается в UI навигацию
- Простая геймплейная сцена с возможностью сохранения и загрузки состояния (для демонстрации Save/Load)
- Авто-скролл при выделении элемента за пределами viewport
- Адаптивная полоса с информацией об управлении (внизу экрана), подстраивается под экран и выбран элемент UI

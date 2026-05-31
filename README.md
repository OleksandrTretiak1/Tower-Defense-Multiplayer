# Unity Cooperative Tower Defense Game - Oleksandr Tretiak

This project is a fully functional, cross-platform multiplayer Tower Defense game developed in Unity. The primary focus of the development was building a robust Client-Server architecture using Mirror Networking, combining a shared economy, complex targeting AI, and adaptive mobile UI. The game allows players on PC and Android devices to seamlessly connect over a local network (LAN/Wi-Fi) and defend the base cooperatively in real-time.

## Multiplayer & Network Architecture (Mirror)
The core achievement of this project is the integration of a stable cooperative multiplayer mode. 
* **Client-Server Model:** Powered by Mirror Networking, ensuring authoritative server logic and real-time state synchronization via RPCs and SyncVars.
* **Cross-Platform Play:** Fully supports local sessions where a PC acts as the Host and an Android device connects as a Client (or vice-versa).
* **Shared Economy:** Players share a global currency pool, fully synchronized across the network, requiring tactical communication to build and upgrade defenses.
* **Robust Session Management:** Graceful handling of lobby disconnects, offline scene loading, and a custom TimeScale reset protocol to prevent race conditions during game-over restarts.

## Core Gameplay & Combat System
The gameplay revolves around strategic tower placement along a predefined enemy path.
* **Advanced Targeting AI:** Towers calculate targets based on the waypoint system, prioritizing enemies closest to the final base.
* **Tower Varieties:** Includes Standard towers for rapid hitscan damage and Missile towers that deal area-of-effect (AoE) explosive damage. Both feature a 3-tier upgrade system altering stats and visuals.
* **Dynamic Wave System:** A serialized wave spawner manages enemy progression, instantiating network-synced creeps with varying speeds and health. 

## Mobile Adaptation & Optimization (Android)
The project was heavily optimized for a flawless mobile gaming experience:
* **Touch Input Integration:** The interaction system leverages `Pointer.current` and `Input.touches` to perfectly register screen taps, ignoring clicks blocked by UI elements.
* **Safe Area Handling:** A custom `SafeAreaHandler` script dynamically resizes the HUD to avoid overlapping with modern smartphone notches and rounded corners.
* **Dynamic Camera Scaling:** Implemented a `MapFitter` algorithm to dynamically adjust the camera's `orthographicSize`, ensuring the entire battlefield is visible regardless of the device's aspect ratio (from 21:9 phones to 4:3 tablets).
* **Performance:** Unlocked to 60 FPS (`Application.targetFrameRate`), forced Landscape orientation, physics interpolation for smooth projectile motion, and compressed texture settings for a lightweight `.apk` build.

## Controls & Interaction
* **PC / Mouse:** Left-click to interact with nodes, build, upgrade, and navigate menus. Escape to toggle the pause menu.
* **Android / Touch:** Tap to open contextual build/upgrade menus (ShopUI, NodeUI). The UI is protected against accidental double-taps during intense waves.
* **Audio Management:** Real-time toggles for music and SFX, automatically synchronized with device memory via `PlayerPrefs`.

<details>
  <summary><h2>Показати українську версію (Ukrainian Version)</h2></summary>

# Unity Cooperative Tower Defense Game - Третяк Олександр

Цей проєкт представляє собою повноцінну кросплатформну багатокористувацьку гру в жанрі Tower Defense, розроблену на рушії Unity. Головний акцент було зроблено на створенні стабільної архітектури «Клієнт-Сервер» за допомогою фреймворку Mirror Networking, об'єднанні спільної економіки, складного штучного інтелекту веж та мобільної оптимізації. Гра дозволяє гравцям на ПК та Android-пристроях з'єднуватися через локальну мережу (LAN/Wi-Fi) для спільного захисту бази в реальному часі.

## Мультиплеєр та Мережева Архітектура (Mirror)
Ключовим досягненням проєкту є інтеграція стабільного кооперативного режиму.
* **Клієнт-Серверна модель:** Використання Mirror Networking для забезпечення серверної авторизації та синхронізації станів через RPC та SyncVar.
* **Кросплатформна гра:** Повна підтримка локальних сесій, де ПК виступає Хостом, а Android-пристрій — Клієнтом (і навпаки).
* **Спільна економіка:** Гравці використовують єдиний синхронізований банк валюти, що вимагає комунікації для прийняття рішень про будівництво чи покращення.
* **Стабільність сесій:** Безпечна обробка розривів з'єднання, повернення в лобі та кастомна система скидання `Time.timeScale`, що запобігає зависанням під час перезапуску після поразки.

## Ігрова механіка та бойова система
В основі лежить стратегічне розміщення оборонних споруд уздовж маршруту ворогів.
* **Штучний інтелект веж:** Алгоритми пошуку цілі інтегровані з системою вейпоінтів, що дозволяє вежам фокусуватися на ворогах, які підійшли найближче до бази.
* **Типи споруд:** Стандартні вежі для швидкого точкового ураження (hitscan) та Ракетні установки для шкоди по радіусу (AoE). Кожна вежа має 3 рівні покращень зі зміною візуалу та характеристик.
* **Система хвиль:** Динамічний спавнер керує прогресією, синхронізуючи створення ворогів із різною швидкістю та запасом здоров'я через мережевий сервер.

## Адаптація під мобільні платформи (Android)
Проєкт було глибоко оптимізовано для забезпечення ідеального мобільного геймплею:
* **Touch Input:** Систему керування адаптовано під тачскрін (`Pointer.current` та `Input.touches`), забезпечуючи точну взаємодію без конфліктів із UI.
* **Safe Area Integration:** Кастомний `SafeAreaHandler` динамічно підлаштовує інтерфейс, щоб уникнути перекриття вирізами камер ("чубчиками") на сучасних смартфонах.
* **Dynamic Camera Scaling:** Скрипт `MapFitter` автоматично розраховує `orthographicSize` камери, гарантуючи повний огляд ігрового поля на пристроях із будь-яким співвідношенням сторін (від вузьких екранів 21:9 до планшетів 4:3).
* **Продуктивність:** Розблоковано 60 FPS (`Application.targetFrameRate`), зафіксовано альбомну орієнтацію екрана (Landscape), налаштовано інтерполяцію фізики для плавного польоту снарядів та зібрано оптимізований `.apk` файл.

## Керування та Інтерфейс
* **ПК / Миша:** Лівий клік для взаємодії з вузлами, купівлі, покращень та меню. Escape для виклику паузи.
* **Android / Touch:** Натискання (Tap) для відкриття контекстних меню (ShopUI, NodeUI). Інтерфейс має захист від випадкових натискань крізь кнопки.
* **Налаштування:** Миттєве збереження налаштувань аудіо (музика/звуки) в пам'ять пристрою за допомогою `PlayerPrefs`.
</details>
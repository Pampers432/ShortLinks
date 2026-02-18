# ShortLinks

Сервис для сокращения URL, построенный на платформе **.NET 10 LTS** с использованием **ASP.NET MVC**, **Entity Framework Core** и **MariaDB/MySQL**.  
Проект полностью контейнеризирован и запускается одной командой через Docker Compose.

---

## Возможности

- Создание коротких ссылок, которые невозможно предугадать  
- Редирект по коротким ссылкам  
- Подсчет количества переходов  
- Управление ссылками через веб-интерфейс (создание, редактирование, удаление)  
- Кэширование редиректов через IMemoryCache  
- Автоматическое применение миграций при старте контейнера  
- Полная поддержка Docker и Docker Compose  

---

## Технологии

- .NET 10 LTS  
- ASP.NET MVC / Razor Pages  
- Entity Framework Core (Code First)  
- MariaDB / MySQL  
- IMemoryCache  
- Docker / Docker Compose  

---

## Быстрый запуск через Docker

### 1. Клонировать репозиторий

```
git clone https://github.com/Pampers432/ShortLinks.git
cd ShortLinks
```

### 2. Настроить строку подключения

В файле `docker-compose.yml` в секции `environment` укажите строку подключения к MariaDB:

```
ConnectionStrings__DefaultConnection=server=host.docker.internal;port=3306;database=ShortLinksDb;user=root;password=1234;Pooling=true;MaximumPoolSize=50;
```

### 3. Собрать и запустить контейнеры

```
docker-compose up -d
```

При старте автоматически применяются миграции.

Приложение будет доступно по адресу:

```
http://localhost:8080
```

### 4. Остановить контейнеры

```
docker-compose down
```

---

## Контакты

Автор: **Максим Поляков**  
Email: max23012007@gmail.com  
Telegram: @S1gmash

# 📚 LibraryManager – ASP.NET Core MVC Project



## 🎯 Opis projektu

Aplikacja służy do zarządzania biblioteką – umożliwia dodawanie książek, egzemplarzy (volume), użytkowników i wypożyczeń. Użytkownicy mogą się logować, zmieniać hasła, a administratorzy zarządzać całą strukturą i danymi.

---

## 🔐 Logowanie i role

- **Admin**:
  - pełny dostęp do wszystkich danych
  - może tworzyć użytkowników, książki, wypożyczać dla innych
- **Zwykły użytkownik**:
  - widzi dane
  - może zmienić swoje hasło

---

## 🧩 Funkcjonalności

### 📖 Książki (Books)
- dodawanie, edytowanie, usuwanie
- każdy tytuł może mieć wiele egzemplarzy

### 🔢 Egzemplarze (Volumes)
- powiązane z książkami
- informacja o dostępności (IsAvailable)
- można wypożyczać tylko dostępne

### 👥 Użytkownicy
- admin tworzy konta z domyślnym hasłem (haszowane SHA256)
- zmiana hasła możliwa z poziomu użytkownika

### 📦 Wypożyczenia (Borrowings)
- tylko dostępne egzemplarze mogą być wypożyczone
- admin może wypożyczać dla innych
- oznaczenie daty wypożyczenia i zwrotu
- przy zwrocie egzemplarz staje się ponownie dostępny

### 📊 Statystyki (Reports)
- liczba wypożyczeń łącznie i aktywnych
- liczba użytkowników z aktywnymi wypożyczeniami
- TOP 5 najczęściej wypożyczanych książek

---

## 🏗️ Struktura bazy danych

- **Users**: `Id`, `Login`, `PasswordHash`, `IsAdmin`, `FirstName`, `LastName`
- **Books**: `Id`, `Title`, `Author`, `Description`
- **Volumes**: `Id`, `InventoryNumber`, `IsAvailable`, `BookId`
- **Borrowings**: `Id`, `UserId`, `VolumeId`, `BorrowedAt`, `ReturnedAt`

  

---

## 🚀 Uruchomienie

1. `dotnet restore`
2. `dotnet ef database update`
3. `dotnet run`

Domyślny admin tworzy się automatycznie:
- login: `admin`
- hasło: `admin123`

---

## 📎 Licencja

Projekt stworzony wyłącznie w celach edukacyjnych.

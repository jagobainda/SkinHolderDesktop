# 🗂️ Estructura de Carpetas - Proyecto WPF MVVM

Esta es una guía para la estructura base de un proyecto WPF siguiendo una arquitectura limpia con patrón MVVM.

---

## 📁 Views
Contiene las interfaces gráficas del usuario.

- Archivos `.xaml` (UI) y `.xaml.cs` (eventos de UI).
- Lógica mínima: navegación, animaciones, cosas que no se pueden bindear.

---

## 📁 ViewModels
Contiene la lógica de presentación.

- Propiedades bindables, comandos (`ICommand` o `[RelayCommand]`).
- Llama a los servicios.
- Valida y transforma datos para la vista.
- Mantiene el estado de la UI.

---

## 📁 Models
Representa el dominio o los datos transferidos.

- Clases DTO o entidades de dominio.
- Puede contener propiedades derivadas (`Total = Subtotal + IVA`).
- Sin lógica de presentación ni acceso a servicios.

---

## 📁 Services
Acceden a datos o servicios externos.

- Llamadas HTTP/API, base de datos, archivos, etc.
- Pueden encapsular lógica de infraestructura (manejo de errores, serialización).
- Devuelven `Models`.

---

## 📁 Mappers
Transformaciones de datos entre modelos y viewmodels.

- Métodos tipo `UserDto → UserModel`.
- Ideal para mantener separación clara entre capas.
- Pueden ser métodos estáticos o clases de extensión.

---

## 📁 Utils
Herramientas auxiliares reutilizables.

- Extensiones (`string.IsValidEmail()`), funciones de ayuda.
- Constantes, enums, helpers comunes.

---

## 📁 Themes
Recursos visuales reutilizables.

- Colores, estilos, plantillas.
- Archivos `.xaml` con `ResourceDictionary`.
- Fuente central para estilos globales.

---

## 📁 Converters (opcional)
Contiene convertidores de binding.

- `BooleanToVisibilityConverter`, `EnumToStringConverter`, etc.
- Clases que implementan `IValueConverter`.

---

## 📁 Navigation (opcional)
Gestión centralizada de navegación.

- Interfaces y servicios para moverse entre vistas.
- Útil si querés desacoplar navegación del code-behind.

---

## 🧭 Flujo de Datos en MVVM

```plaintext
View (XAML, xaml.cs)
        ↓ (binding)
ViewModel (estado y lógica de presentación)
        ↓ (llama)
Service (API externa, DB, archivos)
        ↓
Modelo (entidad de dominio o DTO)
```
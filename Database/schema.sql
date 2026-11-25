-- Esquema de la base de datos (el programa lo ejecuta automáticamente al iniciar)
CREATE TABLE IF NOT EXISTS Videojuegos (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Titulo      TEXT    NOT NULL,
    Plataforma  TEXT    NOT NULL,
    Genero      TEXT    NOT NULL,
    Anio        INTEGER NOT NULL,
    Precio      REAL    NOT NULL,
    Estado      TEXT    NOT NULL
);

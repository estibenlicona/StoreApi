---------------------- Tablas-------------------------------------------
--Tabla Clientes
CREATE TABLE Clientes (
	idCliente INTEGER PRIMARY KEY NOT NULL, 
	Nombre VARCHAR2(200)
);

INSERT INTO Clientes(idCliente, Nombre) VALUES
(1, 'Juan'),(2, 'Pepe'),(3, 'Maria'),(4, 'Jose'),
(5, 'David'),(6, 'Jaime'),(7, 'Diana'),(8, 'Ivan'),
(9, 'Cata');

--Tabla Datos
CREATE TABLE Datos (
	idDatos INTEGER IDENTITY(1,1) PRIMARY KEY,
	variable VARCHAR2(200) NOT NULL,
	valor VARCHAR2(200),
	idCliente INTEGER, 
	CONSTRAINT FK_ClienteDatos FOREIGN KEY (idCliente) 
    REFERENCES Clientes(idCliente)
);

INSERT INTO Datos(idCliente, Variable, Valor) VALUES
(1, 'Genero', 'M'),    (1, 'Ciudad', 'Bogota'),       (1, 'Mascota', 'Si'),
(2, 'Genero', 'M'),    (2, 'Ciudad', 'Cali'),         (2, 'Mascota', 'Si'),
(3, 'Genero', 'F'),    (3, 'Ciudad', 'Bogota'),       (3, 'Mascota', 'Si'),
(4, 'Genero', 'M'),    (4, 'Ciudad', 'Medellin'),     (4, 'Mascota', 'No'),
(5, 'Genero', 'M'),    (5, 'Ciudad', 'Medellin'),     (5, 'Mascota', 'Si'),
(6, 'Genero', 'M'),    (6, 'Ciudad', 'Barranquilla'), (6, 'Mascota', 'No'),
(7, 'Genero', 'F'),    (7, 'Ciudad', 'Cali'),         (7, 'Mascota', 'No'),
(8, 'Genero', 'M'),    (8, 'Ciudad', 'Bogota'),       (8, 'Mascota', 'No'),
(9, 'Genero', 'F'),    (9, 'Ciudad', 'Medellin'),     (9, 'Mascota', 'Si');

-- Tabla Auditoria
CREATE TABLE Auditoria (
    id INTEGER IDENTITY(1,1) PRIMARY KEY,
	cliente VARCHAR2(200), 
	variable VARCHAR2(200),
	Valor VARCHAR2(200),
	FechaMod DATE,
	accion VARCHAR2(200)
);

--------------------- Consultas -----------------------------------------
-- Mujeres que viven en Bogota y tienen mascota
SELECT 
DISTINCT C.idCliente, C.Nombre FROM Clientes C 
INNER JOIN Datos D1 ON D1.idCliente = C.idCliente
INNER JOIN Datos D2 ON D2.idCliente = C.idCliente
INNER JOIN Datos D3 ON D3.idCliente = C.idCliente
WHERE 
(D1.Variable = 'Genero' AND D1.Valor = 'F') AND
(D2.Variable = 'Mascota' AND D2.Valor = 'Si') AND
(D3.Variable = 'Ciudad' AND D3.Valor = 'Bogota');

-- Cuantos clientes hay por ciudad
SELECT D.Valor, COUNT(C.idCliente) AS Clientes FROM Clientes C 
INNER JOIN Datos D ON D.idCliente = C.idCliente
WHERE Variable = 'Ciudad'
GROUP BY D.Valor;

-- Cuantas mascotas hay por genero
SELECT D1.Valor AS Genero, COUNT(D2.Variable) AS Mascota FROM Clientes C 
INNER JOIN Datos D1 ON D1.idCliente = C.idCliente
INNER JOIN Datos D2 ON D2.idCliente = C.idCliente
WHERE 
D1.Variable = 'Genero' AND
(D2.Variable = 'Mascota' AND D2.Valor = 'Si')
GROUP BY D1.Valor;
-- Promedio de mascotas por ciudad
SELECT T1.Ciudad, T1.Mascotas, CAST(CAST(T1.Mascotas AS DECIMAL) / CAST(T2.Total AS DECIMAL) AS DECIMAL(7,2)) AS Promedio
FROM (SELECT D1.Valor AS Ciudad, COUNT(D2.Variable) AS Mascotas FROM Clientes C 
INNER JOIN Datos D1 ON D1.idCliente = C.idCliente
INNER JOIN Datos D2 ON D2.idCliente = C.idCliente
WHERE D1.Variable = 'Ciudad' AND (D2.Variable = 'Mascota' AND D2.Valor = 'Si')
GROUP BY D1.Valor) AS T1,
(SELECT COUNT(*) AS Total FROM Datos D WHERE D.Variable = 'Mascota' AND D.Valor = 'Si') AS T2;

-- Promedio de mascotas de cada ciudad
SELECT D1.Valor AS Ciudad,
(SUM(CASE When D2.Valor = 'Si' Then 1.0 ELSE 0.0 END) / CAST(COUNT(D2.Valor) AS DECIMAL)) AS Promedio
FROM Clientes C 
INNER JOIN Datos D1 ON D1.idCliente = C.idCliente AND D1.Variable = 'Ciudad'
INNER JOIN Datos D2 ON D2.idCliente = C.idCliente AND D2.Variable = 'Mascota'
GROUP BY D1.Valor;
go
--------------------- Procedimientos almacenados -----------------------
--Insert
create procedure SP_CrearCliente @IdCliente int, @Nombre varchar(max)
as 
Insert into Clientes (idCliente, Nombre) 
values(@IdCliente,@Nombre)
go

--Update
create procedure SP_ActualizarCliente @IdCliente int, @Nombre varchar(max)
as 
Update Clientes  
set Nombre= @Nombre 
where idCliente= @IdCliente
go

--Delete
create procedure SP_EliminarCliente @IdCliente int
as 
delete from Clientes 
where idCliente= @IdCliente
go

--------------------- Triggers -----------------------
--Insert
Create trigger Tri_Insert_Datos
on Datos for insert
as
begin
	insert into Auditoria (Cliente,Variable,Valor,FechaMod,accion)
	select idCliente,Variable,Valor, getdate(),'Insert'
	from inserted
end
go

-- Update 
Create trigger Tri_Update_Datos
on Datos AFTER update
as
begin
	insert into Auditoria (Cliente,Variable,Valor,FechaMod,accion)
	select idCliente,Variable,Valor, getdate(),'Update antes: @idCliente: ' + convert(varchar(20),idCliente) + ', @Variable: '+ Variable +', @Valor: ' + Valor
	from deleted
end
go
--Delete
create trigger Tri_delete_Datos
on Datos for delete
as
begin
	insert into Auditoria (Cliente,Variable,Valor,FechaMod,accion)
	select idCliente,Variable,Valor, getdate(),'delete'
	from deleted
end


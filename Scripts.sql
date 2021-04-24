--------------------- Procedimientos almacenados -----------------------
--Insert
create procedure SP_CrearCliente @IdCliente int, @Nombre varchar(max)
as 
Insert into Clientes (idCliente, Nombre) 
values(@IdCliente,@Nombre)
go

--Update
create procedure SP_ActualizarCliente @IdCliente int
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
on Cliente_Prueba for insert
as
begin
	insert into Auditoria_Prueba (Cliente,Variable,Valor,FechaMod,accion)
	select Cliente,Variable,Valor, getdate(),'Insert'
	from inserted
end

-- Update 
Create trigger Tri_Update_Datos
on Cliente_Prueba AFTER update
as
begin
	insert into Auditoria_Prueba (Cliente,Variable,Valor,FechaMod,accion)
	select Cliente,Variable,Valor, getdate(),'Update antes: @Cliente: ' + convert(varchar(20),Cliente) + ', @Variable: '+ Variable +', @Valor: ' + Valor
	from deleted
end

--Delete
create trigger Tri_delete_Datos
on Cliente_Prueba for delete
as
begin
	insert into Auditoria_Prueba (Cliente,Variable,Valor,FechaMod,accion)
	select Cliente,Variable,Valor, getdate(),'delete'
	from deleted
end
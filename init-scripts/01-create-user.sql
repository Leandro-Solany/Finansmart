-- Script de inicializa��o do banco de dados Oracle
-- Este script � executado automaticamente quando o container Oracle � criado

-- Criar usu�rio para a aplica��o
ALTER SESSION SET "_ORACLE_SCRIPT"=true;

-- Criar usu�rio FINANSMART
CREATE USER FINANSMART IDENTIFIED BY FinansmartPassword123
DEFAULT TABLESPACE USERS
TEMPORARY TABLESPACE TEMP
QUOTA UNLIMITED ON USERS;

-- Conceder privil�gios
GRANT CONNECT, RESOURCE, DBA TO FINANSMART;
GRANT CREATE SESSION TO FINANSMART;
GRANT CREATE TABLE TO FINANSMART;
GRANT CREATE VIEW TO FINANSMART;
GRANT CREATE SEQUENCE TO FINANSMART;
GRANT CREATE PROCEDURE TO FINANSMART;

-- Criar tablespace (opcional)
-- CREATE TABLESPACE FINANSMART_DATA
-- DATAFILE '/opt/oracle/oradata/XE/finansmart_data.dbf' SIZE 100M
-- AUTOEXTEND ON NEXT 10M MAXSIZE UNLIMITED;

-- Mensagem de confirma��o
SELECT 'Usuario FINANSMART criado com sucesso!' AS status FROM dual;

EXIT;

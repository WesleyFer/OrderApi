#!/bin/sh
set -e

echo "Aguardando MySQL..."
while ! nc -z mysql-mysqldb-1 3306; do
  echo "MySQL não está pronto. Tentando novamente em 30s..."
  sleep 30
done
echo "MySQL pronto!"

echo "Aguardando RabbitMQ..."
while ! nc -z rabbitmq-1 5672; do
  echo "RabbitMQ não está pronto. Tentando novamente em 30s..."
  sleep 30
done
echo "RabbitMQ pronto!"

echo "Iniciando a API..."
exec dotnet Order.Api.dll
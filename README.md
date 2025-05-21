
# 🏦 [DESAFIO BTG Pactual] - Sistema de Processamento de Pedidos

Este repositório contém a solução para o desafio técnico de engenheiro de software proposto pelo BTG Pactual. O sistema implementado utiliza arquitetura moderna com microsserviços, mensageria, persistência em banco de dados e exposição via API REST.

---

## ✅ Tecnologias Utilizadas

- **.NET 8 (C#)** - API e Worker
- **RabbitMQ 3 Management** - Mensageria (Fila)
- **MySQL** - Base de dados relacional
- **Docker & Docker Compose** - Ambientes isolados e orquestração local
- **DDD** - Domain-Driven Design
- **CQRS** - Separation of read and write responsibilities

---

## ⚙️ Como Executar o Projeto

### Pré-requisitos

- **Docker instalado** (ou Docker Desktop)
- Docker Compose instalado (normalmente já incluso no Docker Desktop)

### Passo a passo

1. **Clone o repositório:**

```bash
git clone https://github.com/WesleyFer/OrderApi.git
cd OrderApi
```

2. **Suba todos os serviços com Docker Compose:**

```bash
docker-compose up -d
```

3. Aguarde alguns instantes para que os containers sejam inicializados corretamente.

---

## 🌐 Endpoints da API

Após os serviços estarem no ar, acesse no navegador:

🔗 **Swagger da API:**  
[https://localhost:7127/swagger/index.html](https://localhost:7127/swagger/index.html)

---

## 🐇 Acesso ao RabbitMQ

O RabbitMQ já estará disponível com o plugin de administração Web:

- URL: [http://localhost:15672](http://localhost:15672)
- Usuário: `rabbitmq`
- Senha: `B4nc0`

Você pode usar esta interface para:

- Visualizar as filas
- Verificar se as mensagens estão sendo consumidas
- Monitorar o tráfego de mensagens

---

## 🛢️ Acesso ao MySQL

Você pode conectar ao banco de dados usando **MySQL Workbench** ou outro cliente, com os dados abaixo:

- Host: `localhost`
- Porta: `3306` (ou altere no `docker-compose.yml`)
- Usuário: `root`
- Senha: `B4nc0`
- Base de dados: `OrderApi`

> Os pedidos processados pelo consumidor serão persistidos automaticamente após consumo da fila RabbitMQ.

---

## ✉️ Exemplo de Mensagem na Fila

```json
{
  "codigoPedido": 1001,
  "codigoCliente": 1,
  "itens": [
    {
      "produto": "lápis",
      "quantidade": 100,
      "preco": 1.10
    },
    {
      "produto": "caderno",
      "quantidade": 10,
      "preco": 1.00
    }
  ]
}
```

Envie essa mensagem para a fila via RabbitMQ Management ou ferramenta de testes (Postman, Insomnia, etc).

---

## 🧪 Funcionalidades Implementadas

✅ Consumo de pedidos via RabbitMQ  
✅ Persistência dos dados no MySQL  
✅ API com Swagger para consulta:  
- Valor total do pedido  
- Quantidade de pedidos por cliente  
- Lista de pedidos por cliente

---

## 🧠 Arquitetura Utilizada

- Separação clara entre **Domínio**, **Infraestrutura**, **Aplicação** (padrão DDD)
- Divisão de comandos (gravação) e queries (leitura) com **CQRS**
- Comunicação assíncrona via **RabbitMQ**
- Todos os serviços encapsulados com **Docker Compose**

---

## 🔗 Links

- Repositório GitHub: [https://github.com/Wesleyfer/OrderApi](https://github.com/Wesleyfer/OrderApi)
- Imagem Docker (se publicada): [localhost](localhost)

---

## 👨‍💻 Autor

**Wesley Fernando**  
[GitHub](https://github.com/WesleyFer) | [DockerDesktop](localhost)
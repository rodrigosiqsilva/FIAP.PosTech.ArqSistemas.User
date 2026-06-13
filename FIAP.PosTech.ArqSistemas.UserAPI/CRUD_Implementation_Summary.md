# FIAP Cloud Games - Usuario API CRUD Implementation

## ✅ Implementation Complete

A full CRUD API for the `Usuario` entity has been implemented following MVC best practices, with middleware for correlation ID tracking, structured logging, and comprehensive validation.

---

## 📋 What Was Implemented

### 1. **Domain Model** (Usuario.cs)
- Properties: `Id` (int, auto-generated), `Nome` (string), `Email` (string), `Senha` (string)

### 2. **Service Layer**
- **IUsuarioService**: Interface defining the CRUD contract
- **UsuarioService**: Implementation with:
  - In-memory data storage using `List<Usuario>`
  - Automatic unique ID generation (starts at 6 after seed data)
  - 5 pre-loaded test records in constructor
  - Full validation on Create/Update operations

### 3. **Controller** (UsuarioController.cs)
Implements REST endpoints:
- **GET /api/usuario** - Retrieve all users
- **GET /api/usuario/{id}** - Retrieve user by ID
- **POST /api/usuario** - Create new user
- **PUT /api/usuario/{id}** - Update existing user
- **DELETE /api/usuario/{id}** - Delete user

### 4. **Validation** (UsuarioValidador.cs)
Comprehensive input validation:
- **Email**: Standard email format validation (RFC-compliant)
- **Password Security**: 
  - Minimum 8 characters
  - At least 1 letter (a-zA-Z)
  - At least 1 digit (0-9)
  - At least 1 special character (!@#$%^&*-_=+)

### 5. **Middleware** (CorrelationIdMiddleware.cs)
- Reads/generates `X-Correlation-Id` header
- Stores correlation ID in `HttpContext.Items["CorrelationId"]`
- Logs request start, end, and duration
- Global exception handling with JSON error response
- Includes correlation ID in all responses

### 6. **Response Wrapper** (ApiResponse<T>)
Unified API response format:
```csharp
{
  "sucesso": true/false,
  "mensagem": "Operation message",
  "dados": {...},
  "listaErros": ["error1", "error2"],
  "correlationId": "guid",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 7. **Logging**
- Structured console logging with timestamps
- Logs for all CRUD operations
- Error and warning logs for validation failures
- Startup/shutdown logging

---

## 🔐 Validation Rules

### **CREATE (POST)**
- All fields required: `Nome`, `Email`, `Senha`
- `Id` must NOT be provided (auto-generated server-side)
- Email must be unique
- Password must meet security requirements
- Auto-generates sequential ID

### **UPDATE (PUT)**
- `Id` is REQUIRED in URL path
- All fields required: `Nome`, `Email`, `Senha`
- Email must be unique (excluding current user)
- Password must meet security requirements
- Used to locate the record to update

### **DELETE (DELETE)**
- Only `Id` required in URL path
- Soft validation (other fields ignored)

### **READ (GET)**
- All users: No validation needed
- By ID: ID must be positive integer

---

## 📊 Seed Data (5 Test Records)

| ID | Nome | Email | Senha |
|---|---|---|---|
| 1 | João Silva | joao@example.com | SenhaSegura@123 |
| 2 | Maria Santos | maria@example.com | OutraSenha#456 |
| 3 | Pedro Oliveira | pedro@example.com | MaisSenha!789 |
| 4 | Ana Costa | ana@example.com | Senha@Teste#101 |
| 5 | Carlos Mendes | carlos@example.com | CarlosSenha$202 |

---

## 🧪 Testing the API

### Using Swagger UI
1. Run the application
2. Navigate to: `https://localhost:7011/swagger` or `http://localhost:5017/swagger`
3. All endpoints are documented with request/response examples

### Using HTTP Test Files
- See `CRUD_Testing.http` for example requests (compatible with VS Code REST Client and Rider)

### Example Requests

**Get All Users:**
```bash
curl -X GET "https://localhost:7011/api/usuario" \
  -H "X-Correlation-Id: test-001"
```

**Create User:**
```bash
curl -X POST "https://localhost:7011/api/usuario" \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: test-002" \
  -d '{
	"nome": "Novo Usuario",
	"email": "novo@example.com",
	"senha": "SenhaFort3@"
  }'
```

**Update User:**
```bash
curl -X PUT "https://localhost:7011/api/usuario/1" \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: test-003" \
  -d '{
	"nome": "João Atualizado",
	"email": "joao.novo@example.com",
	"senha": "NovaSenha@456"
  }'
```

**Delete User:**
```bash
curl -X DELETE "https://localhost:7011/api/usuario/6" \
  -H "X-Correlation-Id: test-004"
```

---

## 📁 Project Structure

```
FIAP.PosTech.ArqSistemas.UserAPI/
├── Model/
│   └── Usuario.cs
├── Controllers/
│   └── UsuarioController.cs
├── Services/
│   ├── IUsuarioService.cs
│   └── UsuarioService.cs
├── Models/
│   └── ApiResponse.cs
├── Middlewares/
│   └── CorrelationIdMiddleware.cs
├── Validators/
│   └── UsuarioValidador.cs
├── DTOs/
│   └── UsuarioDto.cs
├── Program.cs
├── appsettings.json
├── CRUD_Testing.http
└── CRUD_Implementation_Summary.md (this file)
```

---

## ✨ Key Features

✅ **MVC Pattern**: Clean separation of concerns (Controller → Service → Data)
✅ **Correlation ID Tracking**: All requests/responses include correlation ID for traceability
✅ **Structured Logging**: Timestamps, duration, request/response details
✅ **Comprehensive Validation**: Email format + password security rules
✅ **Auto ID Generation**: Unique, sequential IDs generated server-side
✅ **Error Handling**: Global exception handling with structured error responses
✅ **Swagger Documentation**: Full API documentation with examples
✅ **RESTful Design**: Standard HTTP methods and status codes
✅ **DTOs**: Clean request/response models
✅ **Seed Data**: 5 test records pre-loaded for immediate testing

---

## 🚀 Running the Application

```bash
dotnet run
```

Then access:
- **Swagger UI**: https://localhost:7011/swagger
- **API Base URL**: https://localhost:7011/api/usuario

---

## 📝 Notes

- Passwords are stored as plain text (for demo only; use hashing in production)
- Data is stored in-memory and will be reset on application restart
- Correlation IDs persist across the request lifecycle for debugging
- All timestamps in responses are in UTC format (ISO 8601)

---

**Implementation completed successfully! 🎉**

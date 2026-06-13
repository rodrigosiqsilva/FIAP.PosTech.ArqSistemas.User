# PUT Endpoint - Partial Update Implementation

## Overview
The PUT endpoint for the Usuario API has been refactored to support **partial updates** (PATCH-like behavior on PUT).

## Key Changes

### 1. **Requirement Analysis**
- **Only ID is mandatory** for locating the record (provided in URL path: `/api/usuario/{id}`)
- **All body fields are optional** (Nome, Email, Senha)
- **Only provided fields are validated and updated**
- **Omitted fields remain unchanged**

### 2. **New DTO: AtualizarUsuarioDto**
```csharp
public class AtualizarUsuarioDto
{
	public string? Nome { get; set; }      // Optional
	public string? Email { get; set; }     // Optional
	public string? Senha { get; set; }     // Optional
}
```

All fields are nullable, allowing clients to omit them if they don't want to update them.

### 3. **Updated Service Layer**

#### Interface (`IUsuarioService`)
```csharp
(bool Sucesso, string Mensagem, Usuario Usuario) Alterar(int id, AtualizarUsuarioDto usuarioAtualizado);
```

#### Implementation (`UsuarioService`)
The `Alterar()` method now:
1. ✓ Requires only the ID (mandatory)
2. ✓ For each field:
   - Only validates if the field is provided (not null/empty)
   - Only updates if validation passes
   - Skips the field if not provided
3. ✓ Email duplicate check only considers the current update
4. ✓ Password security validation only applied to provided passwords
5. ✓ Logs which fields were updated

```csharp
if (!string.IsNullOrWhiteSpace(usuarioAtualizado.Nome))
{
	usuarioExistente.Nome = usuarioAtualizado.Nome.Trim();
	// Updated
}
// else: Nome not changed

if (!string.IsNullOrWhiteSpace(usuarioAtualizado.Email))
{
	// Validate email format only if provided
	// Check for duplicates only if provided
	if (valid) { usuarioExistente.Email = usuarioAtualizado.Email.Trim(); }
}
// else: Email not changed

if (!string.IsNullOrWhiteSpace(usuarioAtualizado.Senha))
{
	// Validate password security only if provided
	if (valid) { usuarioExistente.Senha = usuarioAtualizado.Senha; }
}
// else: Senha not changed
```

### 4. **Updated Controller Endpoint**
```csharp
[HttpPut("{id}")]
public ActionResult<ApiResponse<Usuario>> Alterar(int id, [FromBody] AtualizarUsuarioDto usuarioAtualizado)
```

## Usage Examples

### Example 1: Update All Fields
```http
PUT /api/usuario/1
Content-Type: application/json

{
  "nome": "João Silva Atualizado",
  "email": "joao.novo@example.com",
  "senha": "NovaSenha@456"
}
```

### Example 2: Update Only Name
```http
PUT /api/usuario/1
Content-Type: application/json

{
  "nome": "Novo Nome"
}
```
**Result:** Only the name is updated; email and password remain unchanged.

### Example 3: Update Only Email
```http
PUT /api/usuario/1
Content-Type: application/json

{
  "email": "novo.email@example.com"
}
```
**Result:** Only the email is updated; name and password remain unchanged.

### Example 4: Update Only Password
```http
PUT /api/usuario/1
Content-Type: application/json

{
  "senha": "NovaSenha@123"
}
```
**Result:** Only the password is updated; name and email remain unchanged.

### Example 5: Partial Update (Name and Email)
```http
PUT /api/usuario/1
Content-Type: application/json

{
  "nome": "Novo Nome",
  "email": "novo@example.com"
}
```
**Result:** Name and email are updated; password remains unchanged.

## Validation Rules

### Applied to Partial Updates:

| Field | Validation | When Applied |
|-------|-----------|---------------|
| Nome | - Not required for update | Only if provided in request |
| Email | - Valid email format<br>- No duplicate with other users | Only if provided in request |
| Senha | - Min 8 characters<br>- At least 1 letter<br>- At least 1 digit<br>- At least 1 special char | Only if provided in request |

⚠️ **Important:** Fields with null/empty values in the request body are **ignored** (not treated as validation errors).

## Request/Response Examples

### Successful Partial Update
**Request:**
```json
PUT /api/usuario/1
{
  "nome": "João Silva - Updated"
}
```

**Response (200 OK):**
```json
{
  "sucesso": true,
  "mensagem": "Usuário alterado com sucesso",
  "dados": {
	"id": 1,
	"nome": "João Silva - Updated",
	"email": "joao@example.com",
	"senha": "SenhaSegura@123"
  },
  "listaErros": [],
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### Validation Error on Update
**Request:**
```json
PUT /api/usuario/1
{
  "email": "invalid-email-format"
}
```

**Response (400 Bad Request):**
```json
{
  "sucesso": false,
  "mensagem": "Erro ao alterar usuário",
  "dados": null,
  "listaErros": ["Formato de email inválido"],
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### User Not Found
**Request:**
```json
PUT /api/usuario/999
{
  "nome": "Any Name"
}
```

**Response (404 Not Found):**
```json
{
  "sucesso": false,
  "mensagem": "Usuário não encontrado",
  "dados": null,
  "listaErros": [],
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## Backward Compatibility

✅ **Fully backward compatible**: Clients that send all fields will continue to work exactly as before.

## Implementation Benefits

1. **Flexible Updates**: Clients can update any combination of fields
2. **Reduced Bandwidth**: Only send fields that need updating
3. **Safer**: Unintended field omissions become non-issues
4. **Standard Pattern**: Aligns with REST best practices for partial updates
5. **Clear Validation**: Only validates fields being updated
6. **Better Logging**: Logs indicate which fields were actually changed

## Testing Recommendations

Test the following scenarios:

1. ✅ Update single field (name only)
2. ✅ Update single field (email only)
3. ✅ Update single field (password only)
4. ✅ Update multiple fields (name + email)
5. ✅ Update multiple fields (email + password)
6. ✅ Update all fields
7. ✅ Update with empty body `{}`
8. ✅ Update with null values (should be ignored)
9. ❌ Update with invalid email → 400 response
10. ❌ Update with invalid password → 400 response
11. ❌ Update non-existent user → 404 response
12. ❌ Update with invalid ID → 400 response

Use the examples in `API_Rest_Client.http` for comprehensive testing.

---

**Implementation Complete** ✅

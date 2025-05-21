using System.ComponentModel.DataAnnotations;

namespace Order.Aplicacao.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public double ExpiresIn { get; set; }
    public UserTokenRequest? UserToken { get; set; }
}

public class LoginUserRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Password { get; set; }
}

public class RegisterUserRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    public string ConfirmPassword { get; set; }
}

public class ClaimRequest
{
    public string Value { get; set; }
    public string Type { get; set; }
}

public class UserTokenRequest
{
    public string Id { get; set; }

    public string Email { get; set; }

    public IEnumerable<ClaimRequest> Claims { get; set; }
}
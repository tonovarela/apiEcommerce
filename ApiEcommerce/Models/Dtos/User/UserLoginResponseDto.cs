

using ApiEcommerce.Models.Dtos.User;

namespace ApiEcommerce.Models.Dtos;

public class UserLoginResponseDto
{


      //public UserRegisterDto? User { get; set; }
      public UserDataDto? User { get; set; }

      public string? token { get; set; }

      public string? Message { get; set; }

}

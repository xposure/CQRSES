// namespace MindMatrix.EventSourcing.Tests
// {
//     using Shouldly;
//     using Xunit;
//     using System;
//     using MediatR;
//     using System.Threading.Tasks;

//     public class AuthServiceTest
//     {
//         public struct Username
//         {
//             public readonly string Value;
//             public bool IsValid => Validate(Value);

//             public Username(string username)
//             {
//                 Value = username;
//             }

//             public static bool Validate(string username)
//             {
//                 return false;
//             }

//             public static implicit operator string(Username it) => it.Value;
//             public static implicit operator Username(string it) => new Username(it);
//         }

//         public interface IAuthService
//         {
//             Task<JWT> Login(string userName, string password);
//         }

//         public class JWT
//         {
//             public bool IsValid => false;
//         }


//         [Fact]
//         public void ShoudLogin()
//         {
//             using var di = DIFixture.Scope();
//             var mediator = di.GetInstance<IMediator>();

//             var login = new Login() { Username = "john", Password = "password123" };

//             mediator.Send(login);
//             var jwt = authService.Login("john", "password123");

//             jwt.IsValid.ShouldBe(true);
//         }
//     }
// }
using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions
{
    public class SignIn(ILogger<SignIn> logger, SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager)
    {
        private readonly ILogger<SignIn> _logger = logger;
        private readonly SignInManager<UserAccount> _signInManager = signInManager;
        private readonly UserManager<UserAccount> _userManager = userManager;

        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"StreamReader :: {ex.Message}");
            }

            if(body != null)
            {
                SignInRequest signInRequest = null!;
                try
                {
                    signInRequest = JsonConvert.DeserializeObject<SignInRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert.DeserializeObject<SignInRequest> :: {ex.Message}");
                }


                if (signInRequest != null && !string.IsNullOrEmpty(signInRequest.Email) && !string.IsNullOrEmpty(signInRequest.Password))
                {
                   try
                   {
                        var userAccount = await _userManager.FindByEmailAsync(signInRequest.Email);
                        var result = await _signInManager.CheckPasswordSignInAsync(userAccount, signInRequest.Password, false);
                        if(result.Succeeded)
                        {
                            //hämta token från tokenProvider, säkrast med en servicebus
                            return new OkObjectResult("accesstoken");
                        }
                        else
                        {
                            return new UnauthorizedResult();
                        }

                   }
                   catch(Exception ex)
                   {
                        _logger.LogError($"_signInManager.PasswordSignInAsync :: {ex.Message}");

                    }
                }
            }

            return new BadRequestResult();
        }
    }
}

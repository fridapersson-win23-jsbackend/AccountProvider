using AccountProvider.Models;
using Azure.Messaging.ServiceBus;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;


namespace AccountProvider.Functions;

public class Signup(ILogger<Signup> logger, UserManager<UserAccount> userManager)
{
    private readonly ILogger<Signup> _logger = logger;
    private readonly UserManager<UserAccount> _userManager = userManager;

    [Function("Signup")]
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

        if (body != null)
        {

            UserRegistrationRequest userRegistrationRequest = null!;
            try
            {
                userRegistrationRequest = JsonConvert.DeserializeObject<UserRegistrationRequest>(body)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"JsonConvert.DeserializeObject :: {ex.Message}");

            }

            if (userRegistrationRequest != null && !string.IsNullOrEmpty(userRegistrationRequest.Email) && !string.IsNullOrEmpty(userRegistrationRequest.Password))
            {
                if (!await _userManager.Users.AnyAsync(x => x.Email == userRegistrationRequest.Email))
                {
                    var userAccount = new UserAccount
                    {
                        FirstName = userRegistrationRequest.FirstName,
                        LastName = userRegistrationRequest.LastName,
                        Email = userRegistrationRequest.Email,
                        UserName = userRegistrationRequest.Email,
                    };

                    try
                    {
                        var result = await _userManager.CreateAsync(userAccount, userRegistrationRequest.Password);
                        if (result.Succeeded)
                        {
                            try
                            {
                                using var http = new HttpClient();
                                StringContent content = new StringContent(JsonConvert.SerializeObject(new { Email = userAccount.Email }), Encoding.UTF8, "application/json");
                                var response = await http.PostAsync("https://verificationprovider.silicon.azurewebsite.net/api/generate", content);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"http.PostAsync :: {ex.Message}");

                            }


                            //send verification code
                            /*

                             */
                            return new OkResult();


                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"_usermanager.CreateAsync :: {ex.Message}");

                    }
                }
                else
                {
                    return new ConflictResult();
                }
            }
        }
        return new BadRequestResult();
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data
{
    /// <summary>
    /// Class responsible for loading initial data into the application's database.
    /// </summary>
    public class LoadDataBase
    {
        /// <summary>
        /// Inserts initial user data into the database. The user data is retrieved from the configuration file (appsettings.json).
        /// </summary>
        /// <param name="appDbContext">The database context to access the database.</param>
        /// <param name="userManager">The user manager to handle user-related operations such as creating users.</param>
        /// <param name="userDataConfig">Configuration object containing the user's initial data, such as FirstName, LastName, Email, and Password.</param>
        /// <returns>A task that represents the asynchronous operation of inserting user data into the database.</returns>
        public static async Task InsertDataAsync(AppDbContext appDbContext, UserManager<User> userManager, IOptions<UserDataConfig> userDataConfig)
        {
            if (!userManager.Users.Any())
            {
                // As sensitive data, it is fetched from appsettings.json
                var userData = userDataConfig.Value;

                User newUser = new()
                {
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    Email = userData.Email,
                };

                await userManager.CreateAsync(newUser, userData?.Password ?? "");
            }

            await appDbContext.SaveChangesAsync();
        }
    }
}

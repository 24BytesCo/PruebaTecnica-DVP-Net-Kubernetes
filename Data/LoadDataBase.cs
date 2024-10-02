using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data
{
    /// <summary>
    /// Class responsible for loading initial data into the application's database.
    /// </summary>
    public class LoadDataBase
    {
        /// <summary>
        /// Inserts initial user data and roles into the database. The user data is retrieved from the configuration file (appsettings.json).
        /// Also creates roles like Administrator and assigns the newly created user to the Administrator role.
        /// </summary>
        /// <param name="appDbContext">The database context to access the database.</param>
        /// <param name="userManager">The user manager to handle user-related operations such as creating users.</param>
        /// <param name="roleManager">The role manager to handle role-related operations such as creating roles.</param>
        /// <param name="userDataConfig">Configuration object containing the user's initial data, such as FirstName, LastName, Email, and Password.</param>
        /// <returns>A task that represents the asynchronous operation of inserting user data and roles into the database.</returns>
        public static async Task InsertDataAsync(AppDbContext appDbContext, UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<UserDataConfig> userDataConfig)
        {
            if (!userManager.Users.Any())
            {
                // As sensitive data, it is fetched from appsettings.json
                var userData = userDataConfig.Value;

                // Ensure roles exist with custom 'Code' property
                string[] roleNames = { "Administrador", "Supervisor", "Empleado" };
                string[] roleCodes = { "ADMIN", "SUPV", "EMP" }; // Códigos asociados a cada rol

                for (int i = 0; i < roleNames.Length; i++)
                {
                    var roleName = roleNames[i];
                    var roleCode = roleCodes[i];

                    // Verificar si el rol ya existe
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        // Crear el rol con la propiedad Code
                        var role = new Role
                        {
                            Name = roleName,
                            Code = roleCode 
                        };

                        await roleManager.CreateAsync(role);
                    }
                }


                // Diccionario para mapear nombres a códigos y descripciones
                var workTaskStatusMapping = new Dictionary<string, (string Code, string Description)>
                {
                    { "Pendiente", ("PEN", "Pendiente por terminar") },
                    { "En Proceso", ("ENPRO", "En proceso") },
                    { "Completada", ("COMP", "Terminada") }
                };

                foreach (var status in workTaskStatusMapping)
                {
                    WorkTaskStatus? existWorkTaskStatus = await appDbContext.WorkTaskStatuses!.SingleOrDefaultAsync(r => r.Name == status.Key);

                    if (existWorkTaskStatus == null)
                    {
                        WorkTaskStatus newWorkTaskStatus = new()
                        {
                            Code = status.Value.Code,
                            Description = status.Value.Description,
                            Name = status.Key,
                            WorkTaskStatusId = Guid.NewGuid().ToString(),
                        };

                        await appDbContext.AddAsync(newWorkTaskStatus);
                    }
                }


                User newUser = new()
                {
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    Email = userData.Email,
                    UserName = userData.Email,
                    Id = Guid.NewGuid().ToString(),
                };

                // Create the user with a password
                var result = await userManager.CreateAsync(newUser, userData?.Password ?? string.Empty);

                if (result.Succeeded)
                {
                    // Assign the Administrator role to the newly created user
                    await userManager.AddToRoleAsync(newUser, "Administrador");
                }
                else
                {
                    throw new ApplicationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            await appDbContext.SaveChangesAsync();
        }
    }
}

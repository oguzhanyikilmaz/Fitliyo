using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Fitliyo.EntityFrameworkCore.Data;

/// <summary>
/// Fitliyo için varsayılan roller (Admin, Trainer, Student) ve örnek kullanıcıları seed eder.
/// DbMigrator çalıştırıldığında bir admin, bir eğitmen ve bir öğrenci hesabı oluşturulur.
/// </summary>
public class FitliyoIdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public const string AdminUserName = "admin";
    public const string AdminEmail = "admin@fitliyo.com";
    public const string AdminPassword = "Test123!";
    public const string TrainerUserName = "egitmen";
    public const string TrainerEmail = "egitmen@fitliyo.com";
    public const string TrainerPassword = "Test123!";
    public const string StudentUserName = "ogrenci";
    public const string StudentEmail = "ogrenci@fitliyo.com";
    public const string StudentPassword = "Test123!";

    public const string RoleAdmin = "Admin";
    public const string RoleTrainer = "Trainer";
    public const string RoleStudent = "Student";

    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<FitliyoIdentityDataSeedContributor> _logger;

    public FitliyoIdentityDataSeedContributor(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IGuidGenerator guidGenerator,
        ILogger<FitliyoIdentityDataSeedContributor> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateRolesIfNotExistsAsync();
        await CreateAdminUserIfNotExistsAsync(context);
        await CreateTrainerUserIfNotExistsAsync(context);
        await CreateStudentUserIfNotExistsAsync(context);
    }

    private async Task CreateRolesIfNotExistsAsync()
    {
        if (await _roleRepository.FindByNormalizedNameAsync(RoleAdmin.ToUpperInvariant()) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), RoleAdmin)
            {
                IsPublic = true
            });
            _logger.LogInformation("Rol oluşturuldu: {Role}", RoleAdmin);
        }

        if (await _roleRepository.FindByNormalizedNameAsync(RoleTrainer.ToUpperInvariant()) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), RoleTrainer)
            {
                IsPublic = true
            });
            _logger.LogInformation("Rol oluşturuldu: {Role}", RoleTrainer);
        }

        if (await _roleRepository.FindByNormalizedNameAsync(RoleStudent.ToUpperInvariant()) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), RoleStudent)
            {
                IsPublic = true
            });
            _logger.LogInformation("Rol oluşturuldu: {Role}", RoleStudent);
        }
    }

    private async Task CreateAdminUserIfNotExistsAsync(DataSeedContext context)
    {
        if (await _userRepository.FindByNormalizedUserNameAsync(AdminUserName.ToUpperInvariant()) != null)
            return;

        var admin = new IdentityUser(_guidGenerator.Create(), AdminUserName, AdminEmail, context.TenantId);
        await _userManager.CreateAsync(admin, AdminPassword);
        await _userManager.AddToRoleAsync(admin, RoleAdmin);
        _logger.LogInformation("Admin kullanıcı oluşturuldu: {UserName} ({Email})", AdminUserName, AdminEmail);
    }

    private async Task CreateTrainerUserIfNotExistsAsync(DataSeedContext context)
    {
        if (await _userRepository.FindByNormalizedUserNameAsync(TrainerUserName.ToUpperInvariant()) != null)
            return;

        var trainer = new IdentityUser(_guidGenerator.Create(), TrainerUserName, TrainerEmail, context.TenantId);
        await _userManager.CreateAsync(trainer, TrainerPassword);
        await _userManager.AddToRoleAsync(trainer, RoleTrainer);
        _logger.LogInformation("Eğitmen kullanıcı oluşturuldu: {UserName} ({Email})", TrainerUserName, TrainerEmail);
    }

    private async Task CreateStudentUserIfNotExistsAsync(DataSeedContext context)
    {
        if (await _userRepository.FindByNormalizedUserNameAsync(StudentUserName.ToUpperInvariant()) != null)
            return;

        var student = new IdentityUser(_guidGenerator.Create(), StudentUserName, StudentEmail, context.TenantId);
        await _userManager.CreateAsync(student, StudentPassword);
        await _userManager.AddToRoleAsync(student, RoleStudent);
        _logger.LogInformation("Öğrenci kullanıcı oluşturuldu: {UserName} ({Email})", StudentUserName, StudentEmail);
    }
}

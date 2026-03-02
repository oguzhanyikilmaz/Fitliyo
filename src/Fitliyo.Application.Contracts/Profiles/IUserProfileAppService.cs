using System;
using System.Threading.Tasks;
using Fitliyo.Profiles.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Profiles;

/// <summary>
/// Kullanıcının kendi profil/sağlık bilgisini getirir ve günceller.
/// BMR, BMI, TDEE vb. hesaplanan alanlar response'ta döner.
/// </summary>
public interface IUserProfileAppService : IApplicationService
{
    /// <summary>
    /// Giriş yapan kullanıcının kendi profilini getirir (hesaplanan metriklerle).
    /// </summary>
    Task<UserProfileDto> GetMyProfileAsync();

    /// <summary>
    /// Giriş yapan kullanıcının kendi profilini günceller.
    /// </summary>
    Task<UserProfileDto> UpdateMyProfileAsync(CreateUpdateUserProfileDto input);
}

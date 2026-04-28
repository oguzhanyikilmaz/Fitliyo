using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUserWorkoutLogLineDto
{
    [Required]
    [StringLength(WellnessConsts.MaxExerciseNameLength)]
    public string ExerciseName { get; set; } = string.Empty;

    [Range(0.1, 24 * 60)]
    public decimal DurationMinutes { get; set; }

    /// <summary>Compendium MET (yürüyüş ~3, tempolu koşu ~9)</summary>
    [Range(0.1, 25)]
    public decimal Met { get; set; } = 5m;
}

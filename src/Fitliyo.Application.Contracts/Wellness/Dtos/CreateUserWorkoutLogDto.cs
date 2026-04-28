using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUserWorkoutLogDto
{
    public Guid? PersonalWorkoutProgramId { get; set; }

    [Required]
    public DateTime LogDate { get; set; }

    [StringLength(WellnessConsts.MaxWorkoutNoteLength)]
    public string? Notes { get; set; }

    [MinLength(1)]
    public List<CreateUserWorkoutLogLineDto> Lines { get; set; } = new();
}

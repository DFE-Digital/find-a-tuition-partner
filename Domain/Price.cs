﻿using System.Diagnostics;

namespace Domain;

[DebuggerDisplay("Price = Tuition = {TuitionSettingId} Subject = {SubjectId} - 1:{GroupSize} £{HourlyRate}")]
public class Price
{
    public int Id { get; set; }
    public int TuitionPartnerId { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public int TuitionSettingId { get; set; }
    public TuitionSetting TuitionSetting { get; set; } = null!;
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int GroupSize { get; set; }
    public decimal HourlyRate { get; set; }
}
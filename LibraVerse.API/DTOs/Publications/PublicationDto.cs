﻿namespace LibraVerse.DTOs.Publications;

public class PublicationDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
}
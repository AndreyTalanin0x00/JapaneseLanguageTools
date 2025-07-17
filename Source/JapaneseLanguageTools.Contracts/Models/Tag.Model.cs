using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapaneseLanguageTools.Contracts.Models;

public class TagModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(256)]
    public string Caption { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? PlaceholderMarker { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset CreatedOn { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset UpdatedOn { get; set; }
}

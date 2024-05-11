using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Model.DbSets;

public class Student
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    [Required]
    public required string Name { get; set; }
}
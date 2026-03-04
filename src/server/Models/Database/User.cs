using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Server.Models.Database;

[Index(nameof(UserName), IsUnique = true)]
public class User : DatabaseObject
{
    [StringLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500)]
    public required string UserName { get; set; } = string.Empty;

    public ICollection<UserGameInfo> UserGameInfo { get; set; } = [];
    public ICollection<Account>      Accounts     { get; set; } = [];
    public ICollection<WishListGame> WishList     { get; set; } = [];
    public ICollection<WishListCategory> WishListCategories { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];
}
using HelsiListOfTasks.Domain.Models;
using HelsiListOfTasks.UI.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelsiListOfTasks.UI.Pages;

public class IndexModel(ILogger<IndexModel> logger, IUserService userService) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    public List<User> Users { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Users = await userService.GetUsersAsync();
    }
}
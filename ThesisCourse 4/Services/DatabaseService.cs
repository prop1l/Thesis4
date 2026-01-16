using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Controls;
using ThesisCourse_4.Database;
using ThesisCourse_4.Database.Models;

public static class DatabaseService
{
    //private static GraphDbContext? _context;

    //private static GraphDbContext Context =>
    //    _context ??= new GraphDbContext(new DbContextOptionsBuilder<GraphDbContext>()
    //        .UseNpgsql("Host=localhost;Database=thesis_graphs;Username=postgres;Password=root")
    //        .Options);

    private readonly static PostgresContext Context = new PostgresContext();

    public static async Task<bool> ValidateUserAsync(string login, string password)
    {
        try
        {
            var user = await Context.Users
                .FirstOrDefaultAsync(u => u.UserName == login);

            return user != null && user.PasswordHash == password;
        }
        catch
        {
            return false;
        }
    }

    //public static async Task<List<Graph>> GetGraphsForUserAsync(string username)
    //{
    //    var user = await Context.Users
    //        .Include(u => u.Graphs)
    //            .ThenInclude(g => g.GraphNodes)
    //        .Include(u => u.Graphs)
    //            .ThenInclude(g => g.GraphEdges)
    //        .FirstOrDefaultAsync(u => u.Name == username);

    //    return (List<Graph>)(user?.Graphs ?? new List<Graph>());
    //}

    //public static async Task<Graph?> GetFullGraphAsync(int graphId)
    //{
    //    return await Context.Graphs
    //        .Include(g => g.GraphNodes)
    //        .Include(g => g.GraphEdges)
    //        .FirstOrDefaultAsync(g => g.Id == graphId);
    //}

    //public static async Task<int> SaveFullGraphAsync(Graph graph)
    //{
    //    if (graph.Id == 0)
    //    {
    //        Context.Graphs.Add(graph);
    //    }
    //    else
    //    {
    //        Context.Graphs.Update(graph);
    //    }

    //    await Context.SaveChangesAsync();
    //    return graph.Id;
    //}

    //public static async Task DeleteGraphAsync(int graphId)
    //{
    //    var graph = await Context.Graphs
    //        .Include(g => g.GraphNodes)
    //        .Include(g => g.GraphEdges)
    //        .FirstOrDefaultAsync(g => g.Id == graphId);

    //    if (graph != null)
    //    {
    //        Context.Graphs.Remove(graph);
    //        await Context.SaveChangesAsync();
    //    }
    //}

    public static async Task<User?> CreateUserAsync(string name, string password)
    {
        string passwordHash = password; //TODO: make hash passwords

        var getName = await Context.Users.FirstOrDefaultAsync(x => x.UserName == name);

        if (getName != null) return null;

        var user = new User
        { 
            Id = default,
            UserName = name,
            PasswordHash = passwordHash,
            CreatedAt = default,
            Avatar = null
        };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    public static async Task<User?> GetUserAsync(string name)
    {
        return await Context.Users.FirstOrDefaultAsync(u => u.UserName == name);
    }
}
